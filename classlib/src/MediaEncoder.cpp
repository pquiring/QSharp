#include "Media.hpp"

namespace Qt { namespace Media {

//AVPicture was deprecated in ffmpeg/3.0 - use AVFrame instead - this function does the same as avpicture_alloc() except using AVFrame
static int _avframe_alloc(AVFrame *picture, enum AVPixelFormat pix_fmt, int width, int height)
{
  int ret = (*_av_image_alloc)(picture->data, picture->linesize, width, height, pix_fmt, 1);
  if (ret < 0) {
    memset(picture, 0, sizeof(AVFrame));
    return ret;
  }

  return 0;
}

static bool add_stream(std::shared_ptr<Qt::Media::FFContext> ctx, int codec_id) {
  AVCodecContext *codec_ctx;
  AVStream *stream;
  AVCodec *codec;

  codec = (*_avcodec_find_encoder)(codec_id);
  if (codec == nullptr) return false;
  stream = (*_avformat_new_stream)(ctx->fmt_ctx, codec);
  if (stream == nullptr) return false;
  stream->id = ctx->fmt_ctx->nb_streams-1;
  codec_ctx = (*_avcodec_alloc_context3)(codec);

  switch (codec->type) {
    case AVMEDIA_TYPE_AUDIO:
      if (codec->sample_fmts != nullptr) {
        codec_ctx->sample_fmt = codec->sample_fmts[0];
      } else {
        codec_ctx->sample_fmt = AV_SAMPLE_FMT_S16;
      }

      codec_ctx->bit_rate = ctx->config_audio_bit_rate;
      codec_ctx->sample_rate = ctx->freq;
      codec_ctx->channels = ctx->chs;
      switch (ctx->chs) {
        case 1: codec_ctx->channel_layout = AV_CH_LAYOUT_MONO; break;
        case 2: codec_ctx->channel_layout = AV_CH_LAYOUT_STEREO; break;
        case 4: codec_ctx->channel_layout = AV_CH_LAYOUT_QUAD; break;
      }
      stream->time_base.num = 1;
      stream->time_base.den = ctx->freq;

      ctx->audio_ratio.num = 1;
      ctx->audio_ratio.den = ctx->freq;

      ctx->audio_stream = stream;
      ctx->audio_codec = codec;
      ctx->audio_codec_ctx = codec_ctx;
      break;
    case AVMEDIA_TYPE_VIDEO:
      codec_ctx->bit_rate = ctx->config_video_bit_rate;
      codec_ctx->width = ctx->width;
      codec_ctx->height = ctx->height;
      if (ctx->fps_1000_1001) {
        codec_ctx->time_base.num = 1000;
        codec_ctx->time_base.den = ctx->fps * 1001;
        stream->time_base.num = 1000;
        stream->time_base.den = ctx->fps * 1001;
      } else {
        codec_ctx->time_base.num = 1;
        codec_ctx->time_base.den = ctx->fps;
        stream->time_base.num = 1;
        stream->time_base.den = ctx->fps;
      }
      codec_ctx->gop_size = ctx->config_gop_size;
      codec_ctx->pix_fmt = AV_PIX_FMT_YUV420P;
      if (codec_ctx->codec_id == AV_CODEC_ID_H264) {
        (*_av_opt_set)(codec_ctx->priv_data, "profile", "baseline", 0);
        (*_av_opt_set)(codec_ctx->priv_data, "preset", "slow", 0);
      }

      ctx->video_stream = stream;
      ctx->video_codec = codec;
      ctx->video_codec_ctx = codec_ctx;
      break;
    case AVMEDIA_TYPE_UNKNOWN:
    case AVMEDIA_TYPE_DATA:
    case AVMEDIA_TYPE_SUBTITLE:
    case AVMEDIA_TYPE_ATTACHMENT:
    case AVMEDIA_TYPE_NB:
      break;
  }

  if ((ctx->out_fmt->flags & AVFMT_GLOBALHEADER) != 0) {
    codec_ctx->flags |= CODEC_FLAG_GLOBAL_HEADER;
  }

  return true;
}

static bool open_video(std::shared_ptr<Qt::Media::FFContext> ctx) {
  int ret = (*_avcodec_open2)(ctx->video_codec_ctx, ctx->video_codec, nullptr);
  if (ret < 0) return false;
  ctx->video_frame = (*_av_frame_alloc)();
  if (ctx->video_frame == nullptr) return false;
  ctx->dst_pic = (*_av_frame_alloc)();
  ret = _avframe_alloc(ctx->dst_pic, ctx->video_codec_ctx->pix_fmt, ctx->video_codec_ctx->width, ctx->video_codec_ctx->height);
  if (ret < 0) return false;
  if (ctx->video_codec_ctx->pix_fmt != AV_PIX_FMT_BGRA) {
    ctx->src_pic = (*_av_frame_alloc)();
    ret = _avframe_alloc(ctx->src_pic, AV_PIX_FMT_BGRA, ctx->video_codec_ctx->width, ctx->video_codec_ctx->height);
    if (ret < 0) return false;
    ctx->sws_ctx = (*_sws_getContext)(ctx->video_codec_ctx->width, ctx->video_codec_ctx->height, AV_PIX_FMT_BGRA
      , ctx->video_codec_ctx->width, ctx->video_codec_ctx->height, ctx->video_codec_ctx->pix_fmt, SWS_BICUBIC, nullptr, nullptr, nullptr);
  }
  //set width/height/format
  ctx->video_frame->width = ctx->width;
  ctx->video_frame->height = ctx->height;
  ctx->video_frame->format = ctx->video_codec_ctx->pix_fmt;
  ret = (*_av_frame_get_buffer)(ctx->video_frame, 32);
  if (ret < 0) {
    printf("av_frame_get_buffer() failed! %d\n", ret);
    return false;
  }
  //copy data/linesize pointers from dst_pic to frame
  for(int a=0;a<8;a++) {
    ctx->video_frame->data[a] = ctx->dst_pic->data[a];
    ctx->video_frame->linesize[a] = ctx->dst_pic->linesize[a];
  }
  //copy params
  ret = (*_avcodec_parameters_from_context)(ctx->video_stream->codecpar, ctx->video_codec_ctx);
  if (ret < 0) {
    printf("avcoidec_parameters_from_context() failed!\n");
    return false;
  }
  return true;
}

static bool open_audio(std::shared_ptr<Qt::Media::FFContext> ctx) {
  int ret = (*_avcodec_open2)(ctx->audio_codec_ctx, ctx->audio_codec, nullptr);
  if (ret < 0) {
    printf("avcodec_open2() failed!\n");
    return false;
  }
  ctx->audio_frame = (*_av_frame_alloc)();
  if (ctx->audio_frame == nullptr) {
    printf("av_frame_alloc() failed!\n");
    return false;
  }
  ctx->audio_frame->format = ctx->audio_codec_ctx->sample_fmt;
  ctx->audio_frame->sample_rate = ctx->freq;
  ctx->audio_frame->channel_layout = ctx->audio_codec_ctx->channel_layout;
  ctx->audio_frame_size = ctx->audio_codec_ctx->frame_size * ctx->chs;  //max samples that encoder will accept
  ctx->audio_frame_size_variable = (ctx->audio_codec->capabilities & CODEC_CAP_VARIABLE_FRAME_SIZE) != 0;
  ctx->audio_frame->nb_samples = ctx->audio_codec_ctx->frame_size;
  ret = (*_av_frame_get_buffer)(ctx->audio_frame, 0);
  if (ret < 0) {
    printf("av_frame_get_buffer() failed! %d\n", ret);
    return false;
  }
  if (!ctx->audio_frame_size_variable) {
    ctx->audio_buffer = (short*)malloc(ctx->audio_frame_size * 2);
    ctx->audio_buffer_size = 0;
  }
  if (ctx->audio_codec_ctx->sample_fmt == AV_SAMPLE_FMT_S16) {
    return true;
  }
  //copy params
  ret = (*_avcodec_parameters_from_context)(ctx->audio_stream->codecpar, ctx->audio_codec_ctx);
  if (ret < 0) {
    printf("avcoidec_parameters_from_context() failed!\n");
    return false;
  }
  //create audio conversion (S16 -> FLTP)
  if (libav_org)
    ctx->swr_ctx = (*_avresample_alloc_context)();
  else
    ctx->swr_ctx = (*_swr_alloc)();
  (*_av_opt_set_int)(ctx->swr_ctx, "in_channel_layout",     ctx->audio_codec_ctx->channel_layout, 0);
  (*_av_opt_set_int)(ctx->swr_ctx, "in_sample_rate",        ctx->freq, 0);
  (*_av_opt_set_int)(ctx->swr_ctx, "in_sample_fmt",  AV_SAMPLE_FMT_S16, 0);
  (*_av_opt_set_int)(ctx->swr_ctx, "out_channel_layout",    ctx->audio_codec_ctx->channel_layout, 0);
  (*_av_opt_set_int)(ctx->swr_ctx, "out_sample_rate",       ctx->freq, 0);
  (*_av_opt_set_int)(ctx->swr_ctx, "out_sample_fmt", ctx->audio_codec_ctx->sample_fmt, 0);
  if (libav_org)
    (*_avresample_open)(ctx->swr_ctx);
  else
    (*_swr_init)(ctx->swr_ctx);
  return true;
}

//libav.org does not provide this function : easy to implement
//see http://ffmpeg.org/doxygen/trunk/mux_8c_source.html#l00148
static AVFormatContext *_avformat_alloc_output_context2(const char *codec) {
  AVFormatContext *fmt_ctx = (*_avformat_alloc_context)();
  fmt_ctx->oformat = (*_av_guess_format)(codec, nullptr, nullptr);
  if (fmt_ctx->oformat == nullptr) {
    printf("av_guess_format() failed! (codec=%s)\n", codec);
    return nullptr;
  }
  if (fmt_ctx->oformat->priv_data_size > 0) {
    fmt_ctx->priv_data = (*_av_malloc)(fmt_ctx->oformat->priv_data_size);
    if (fmt_ctx->oformat->priv_class != nullptr) {
      *(const AVClass**)fmt_ctx->priv_data = fmt_ctx->oformat->priv_class;
      (*_av_opt_set_defaults)(fmt_ctx->priv_data);
    }
  }
  return fmt_ctx;
}

static bool encoder_start(std::shared_ptr<Qt::Media::FFContext> ctx, const char *codec, bool doVideo, bool doAudio, void*read, void*write, void*seek) {
  ctx->fmt_ctx = _avformat_alloc_output_context2(codec);
  if (ctx->fmt_ctx == nullptr) {
    printf("Error:Unable to find codec:%s\n", codec);
    return false;
  }
  ctx->ff_buffer = (*_av_malloc)(ffiobufsiz);
  ctx->io_ctx = (*_avio_alloc_context)(ctx->ff_buffer, ffiobufsiz, 1, (void*)ctx.get(), read, write, seek);
  if (ctx->io_ctx == nullptr) return false;
  ctx->fmt_ctx->pb = ctx->io_ctx;
  ctx->out_fmt = ctx->fmt_ctx->oformat;
  if ((ctx->out_fmt->video_codec != AV_CODEC_ID_NONE) && doVideo) {
    if (!add_stream(ctx, ctx->out_fmt->video_codec)) {
      printf("add_stream:video failed!\n");
      return false;
    }
  }
  if ((ctx->out_fmt->audio_codec != AV_CODEC_ID_NONE) && doAudio) {
    if (!add_stream(ctx, ctx->out_fmt->audio_codec)) {
      printf("add_stream:audio failed!\n");
      return false;
    }
  }
  if (ctx->video_stream != nullptr) {
    if (!open_video(ctx)) {
      printf("open_video failed!\n");
      return false;
    }
  }
  if (ctx->audio_stream != nullptr) {
    if (!open_audio(ctx)) {
      printf("open_audio failed!\n");
      return false;
    }
  }
  int ret = (*_avformat_write_header)(ctx->fmt_ctx, nullptr);
  if (ret < 0) {
    printf("avformat_write_header failed! %d\n", ret);
  }
  if (ctx->audio_frame != nullptr) {
    ctx->audio_pts = 0;
  }
  if (ctx->video_frame != nullptr) {
    ctx->video_pts = 0;
  }
  (*_av_dump_format)(ctx->fmt_ctx, 0, "dump.avi", 1);
  return true;
}

bool Qt::Media::MediaEncoder::Start(std::shared_ptr<MediaIO> io, int width, int height, int fps, int chs, int freq, std::shared_ptr<Qt::Core::String> codec, bool doVideo, bool doAudio)
{
  ctx = std::make_shared<FFContext>(io, std::dynamic_pointer_cast<MediaCoder>($weak_this.lock()));

  if (doVideo && (width <= 0 || height <= 0)) {
    return false;
  }
  if (doAudio && (chs <= 0 || freq <= 0)) {
    return false;
  }
  if (fps <= 0) fps = 24;  //must be valid, even for audio only

  ctx->fps_1000_1001 = FPS_1000_1001;
  ctx->config_gop_size = FramesPerKeyFrame;
  ctx->config_video_bit_rate = VideoBitRate;
  ctx->config_audio_bit_rate = AudioBitRate;
  ctx->width = width;
  ctx->height = height;
  ctx->fps = fps;
  ctx->chs = chs;
  ctx->freq = freq;
  const char *ccodec = codec->cstring();
  bool ret = encoder_start(ctx, ccodec, doVideo, doAudio, (void*)&read_packet, (void*)&write_packet, (void*)&seek_packet);
  return ret;
}

static bool addAudioFrame(std::shared_ptr<Qt::Media::FFContext> ctx, short *sams, int offset, int length)
{
  int nb_samples = length / ctx->chs;
  int buffer_size = (*_av_samples_get_buffer_size)(nullptr, ctx->chs, nb_samples, AV_SAMPLE_FMT_S16, 0);
  void* samples_data = (*_av_malloc)(buffer_size);
  //copy sams -> samples_data
  std::memcpy(samples_data, sams + offset, length * 2);
  AVPacket *pkt = AVPacket_New();
  (*_av_init_packet)(pkt);

  if (ctx->swr_ctx != nullptr) {
    //convert S16 -> FLTP (some codecs do not support S16)
    //sample rate is not changed
    if (((*_av_samples_alloc)(ctx->audio_dst_data, ctx->audio_dst_linesize, ctx->chs
      , nb_samples, ctx->audio_codec_ctx->sample_fmt, 1)) < 0)
    {
      printf("av_samples_alloc failed!\n");
      return false;
    }
    ctx->audio_src_data[0] = (uint8_t*)samples_data;
    if (libav_org)
      (*_avresample_convert)(ctx->swr_ctx, ctx->audio_dst_data, 0, nb_samples
        , ctx->audio_src_data, 0, nb_samples);
    else
      (*_swr_convert)(ctx->swr_ctx, ctx->audio_dst_data, nb_samples
        , ctx->audio_src_data, nb_samples);
  } else {
    ctx->audio_dst_data[0] = (uint8_t*)samples_data;
  }
  ctx->audio_frame->nb_samples = nb_samples;
  buffer_size = (*_av_samples_get_buffer_size)(nullptr, ctx->chs, nb_samples, ctx->audio_codec_ctx->sample_fmt, 0);
  (*_av_frame_make_writable)(ctx->audio_frame);  //ensure we can write to it now
  int res = (*_avcodec_fill_audio_frame)(ctx->audio_frame, ctx->chs, ctx->audio_codec_ctx->sample_fmt, ctx->audio_dst_data[0]
    , buffer_size, 0);
  if (res < 0) {
    printf("avcodec_fill_audio_frame() failed:%d\n", res);
    return false;
  }
  int got_frame = 0;
  ctx->audio_frame->pts = (*_av_rescale_q)(ctx->audio_pts, ctx->audio_ratio, ctx->audio_codec_ctx->time_base);
  int ret = (*_avcodec_encode_audio2)(ctx->audio_codec_ctx, pkt, ctx->audio_frame, &got_frame);
  if (ret < 0) {
    printf("avcodec_encode_audio2() failed!%d\n", ret);
    return false;
  }
  if (got_frame && pkt->size > 0) {
    pkt->stream_index = ctx->audio_stream->index;
    (*_av_packet_rescale_ts)(pkt, ctx->audio_codec_ctx->time_base, ctx->audio_stream->time_base);
//printf("audio : write_frame() : %lld, %lld, %d, %d\n", pkt->pts, pkt->dts, pkt->duration, pkt->stream_index);
    ret = (*_av_interleaved_write_frame)(ctx->fmt_ctx, pkt);
    (*_av_free_packet)(pkt);
    (*_av_free)(pkt);
    pkt = nullptr;
    if (ret < 0) {
      printf("av_interleaved_write_frame() failed!\n");
      return false;
    }
  }
  (*_av_free)(samples_data);
  if (ctx->swr_ctx != nullptr) {
    //free audio_dst_data (only the first pointer)
    if (ctx->audio_dst_data[0] != nullptr) {
      (*_av_free)(ctx->audio_dst_data[0]);
      ctx->audio_dst_data[0] = nullptr;
    }
  }
  ctx->audio_pts += nb_samples;
  return ret == 0;
}

static bool addAudio(std::shared_ptr<Qt::Media::FFContext> ctx, short *sams, int offset, int length) {
  bool ok = true;

  int frame_size = length;
  if (!ctx->audio_frame_size_variable) {
    frame_size = ctx->audio_frame_size;  //max samples that encoder will accept
    if (ctx->audio_buffer_size > 0) {
      //fill audio_buffer with input samples
      int size = ctx->audio_frame_size - ctx->audio_buffer_size;
      if (size > length) size = length;
      std::memcpy(ctx->audio_buffer + ctx->audio_buffer_size, sams + offset, size * 2);
      ctx->audio_buffer_size += size;
      if (ctx->audio_buffer_size < ctx->audio_frame_size) return true;  //frame still not full
      addAudioFrame(ctx, ctx->audio_buffer, 0, ctx->audio_buffer_size);
      ctx->audio_buffer_size = 0;
      offset += size;
      length -= size;
    }
  }

  while (length > 0) {
    int size = length;
    if (size > frame_size) size = frame_size;
    if (size < frame_size && !ctx->audio_frame_size_variable) {
      //partial frame : copy the rest to temp storage for next call
      std::memcpy(ctx->audio_buffer, sams + offset, size * 2);
      ctx->audio_buffer_size = size;
      return true;
    }
    if (!addAudioFrame(ctx, sams, offset, size)) {
      ok = false;
      break;
    }
    offset += size;
    length -= size;
  }

  return ok;
}

bool Qt::Media::MediaEncoder::AddAudio(Qt::QSharp::FixedArray1D<short> sams, int offset, int length)
{
  if (ctx == nullptr) return false;

  if (ctx->audio_codec_ctx == nullptr) return false;

  //TODO : check buffer size

  bool ok = addAudio(ctx, sams.data(), offset, length);

  return ok;
}

static bool addVideo(std::shared_ptr<Qt::Media::FFContext> ctx, int *px)
{
  int length = ctx->width * ctx->height * 4;
  if (ctx->video_codec_ctx->pix_fmt != AV_PIX_FMT_BGRA) {
    //copy px -> ctx->src_pic->data[0];
    std::memcpy(ctx->src_pic->data[0], px, length);
    (*_sws_scale)(ctx->sws_ctx, ctx->src_pic->data, ctx->src_pic->linesize, 0, ctx->video_codec_ctx->height
      , ctx->dst_pic->data, ctx->dst_pic->linesize);
  } else {
    //copy px -> ctx->dst_pic->data[0];
    std::memcpy(ctx->dst_pic->data[0], px, length);
  }
  AVPacket *pkt = AVPacket_New();
  (*_av_init_packet)(pkt);

  int got_frame = 0;
  ctx->video_frame->pts = ctx->video_pts;
  int ret = (*_avcodec_encode_video2)(ctx->video_codec_ctx, pkt, ctx->video_frame, &got_frame);
  if (ret < 0) {
    printf("avcodec_encode_video2() failed!\n");
    return false;
  }
  if (got_frame != 0 && pkt->size > 0) {
    pkt->stream_index = ctx->video_stream->index;
    (*_av_packet_rescale_ts)(pkt, ctx->video_codec_ctx->time_base, ctx->video_stream->time_base);
//printf("video : write_frame() : %lld, %lld, %d, %d\n", pkt->pts, pkt->dts, pkt->duration, pkt->stream_index);
    ret = (*_av_interleaved_write_frame)(ctx->fmt_ctx, pkt);
    (*_av_free_packet)(pkt);
    (*_av_free)(pkt);
    pkt = nullptr;
  }
  ctx->video_pts++;
  return ret == 0;
}

bool Qt::Media::MediaEncoder::AddVideo(Qt::QSharp::FixedArray1D<int> px)
{
  if (ctx == nullptr) return false;

  if (ctx->video_codec_ctx == nullptr) return false;

  bool ok = addVideo(ctx, px.data());

  return ok;
}

int Qt::Media::MediaEncoder::GetAudioFramesize()
{
  if (ctx == nullptr) return 0;
  if (ctx->audio_codec_ctx == nullptr) return 0;
  return ctx->audio_codec_ctx->frame_size;
}

static bool flush(std::shared_ptr<Qt::Media::FFContext> ctx) {
  if (ctx->audio_frame == nullptr) return false;
  AVPacket *pkt = AVPacket_New();
  (*_av_init_packet)(pkt);

  int got_frame = 0;
  int ret = (*_avcodec_encode_audio2)(ctx->audio_codec_ctx, pkt, nullptr, &got_frame);
  if (ret < 0) {
    printf("avcodec_encode_audio2() failed!\n");
    return false;
  }
  if (got_frame != 0 && pkt->size > 0) {
    pkt->stream_index = ctx->audio_stream->index;
    ret = (*_av_interleaved_write_frame)(ctx->fmt_ctx, pkt);
    (*_av_free_packet)(pkt);
    (*_av_free)(pkt);
    pkt = nullptr;
    if (ret < 0) printf("av_interleaved_write_frame() failed!\n");
    return ret == 0;
  }
  return false;
}

static void encoder_stop(std::shared_ptr<Qt::Media::FFContext> ctx)
{
  //flush audio encoder
  while (flush(ctx)) {}
  int ret = (*_av_write_trailer)(ctx->fmt_ctx);
  if (ret < 0) {
    printf("av_write_trailer() failed! %d\n", ret);
  }
  if (ctx->io_ctx != nullptr) {
    (*_avio_flush)(ctx->io_ctx);
    ctx->io_ctx = nullptr;
    ctx->ff_buffer = nullptr;
  }
  if (ctx->audio_stream != nullptr) {
    (*_avcodec_close)(ctx->audio_codec_ctx);
    ctx->audio_stream = nullptr;
  }
  if (ctx->video_stream != nullptr) {
    (*_avcodec_close)(ctx->video_codec_ctx);
    ctx->video_stream = nullptr;
  }
  if (ctx->audio_frame != nullptr) {
    (*_av_frame_free)((void**)&ctx->audio_frame);
  }
  if (ctx->video_frame != nullptr) {
    (*_av_frame_free)((void**)&ctx->video_frame);
  }
  if (ctx->fmt_ctx != nullptr) {
    (*_avformat_free_context)(ctx->fmt_ctx);
    ctx->fmt_ctx = nullptr;
  }
  if (ctx->src_pic != nullptr) {
    (*_av_frame_free)((void**)&ctx->src_pic);
  }
  if (ctx->dst_pic != nullptr) {
    (*_av_frame_free)((void**)&ctx->dst_pic);
  }
  if (ctx->sws_ctx != nullptr) {
    (*_sws_freeContext)(ctx->sws_ctx);
    ctx->sws_ctx = nullptr;
  }
  if (ctx->swr_ctx != nullptr) {
    if (libav_org)
      (*_avresample_free)(&ctx->swr_ctx);
    else
      (*_swr_free)(&ctx->swr_ctx);
    ctx->swr_ctx = nullptr;
  }
  if (ctx->audio_buffer != nullptr) {
    free(ctx->audio_buffer);
    ctx->audio_buffer = nullptr;
  }
}

void Qt::Media::MediaEncoder::Stop()
{
  if (ctx == nullptr) return;
  encoder_stop(ctx);
  ctx = nullptr;
}

} }  //namespace Qt::Media

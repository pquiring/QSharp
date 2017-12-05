#include "Media.hpp"

namespace Qt::Media {

//returns stream idx >= 0
static int open_codec_context(std::shared_ptr<Qt::Media::FFContext> ctx, AVFormatContext *fmt_ctx, int type)
{
  int ret;
  int stream_idx;
  AVStream *stream;
  AVCodec *codec;
  stream_idx = (*_av_find_best_stream)(ctx->fmt_ctx, type, -1, -1, nullptr, 0);
  if (stream_idx >= 0) {
    stream = (AVStream*)ctx->fmt_ctx->streams[stream_idx];
    codec = (*_avcodec_find_decoder)(stream->codecpar->codec_id);
    if (codec == nullptr) {
      return -1;
    }
    ctx->codec_ctx = (*_avcodec_alloc_context3)(codec);
    (*_avcodec_parameters_to_context)(ctx->codec_ctx, stream->codecpar);
    ctx->codec_ctx->flags |= CODEC_FLAG_LOW_DELAY;
    if ((ret = (*_avcodec_open2)(ctx->codec_ctx, codec, nullptr)) < 0) {
      return ret;
    }
  }
  return stream_idx;
}

static bool open_codecs(std::shared_ptr<Qt::Media::FFContext> ctx, int new_width, int new_height, int new_chs, int new_freq) {
  AVCodecContext *codec_ctx;
  if ((ctx->video_stream_idx = open_codec_context(ctx, ctx->fmt_ctx, AVMEDIA_TYPE_VIDEO)) >= 0) {
    ctx->video_stream = (AVStream*)ctx->fmt_ctx->streams[ctx->video_stream_idx];
    ctx->video_codec_ctx = ctx->codec_ctx;
    if (new_width == -1) new_width = ctx->video_codec_ctx->width;
    if (new_height == -1) new_height = ctx->video_codec_ctx->height;
    if ((ctx->video_dst_bufsize = (*_av_image_alloc)(ctx->video_dst_data, ctx->video_dst_linesize
      , ctx->video_codec_ctx->width, ctx->video_codec_ctx->height
      , ctx->video_codec_ctx->pix_fmt, 1)) < 0)
    {
      return false;
    }
    if ((ctx->rgb_video_dst_bufsize = (*_av_image_alloc)(ctx->rgb_video_dst_data, ctx->rgb_video_dst_linesize
      , new_width, new_height
      , AV_PIX_FMT_BGRA, 1)) < 0)
    {
      return false;
    }
    ctx->video_length = ctx->rgb_video_dst_bufsize/4;
    ctx->video = std::make_shared<QSharpArray<int>>(ctx->video_length);
    //create video conversion context
    ctx->sws_ctx = (*_sws_getContext)(ctx->video_codec_ctx->width, ctx->video_codec_ctx->height, ctx->video_codec_ctx->pix_fmt
      , new_width, new_height, AV_PIX_FMT_BGRA
      , SWS_BILINEAR, nullptr, nullptr, nullptr);
  }

  if ((ctx->audio_stream_idx = open_codec_context(ctx, ctx->fmt_ctx, AVMEDIA_TYPE_AUDIO)) >= 0) {
    ctx->audio_stream = (AVStream*)ctx->fmt_ctx->streams[ctx->audio_stream_idx];
    ctx->audio_codec_ctx = ctx->codec_ctx;
    //create audio conversion context
    if (libav_org)
      ctx->swr_ctx = (*_avresample_alloc_context)();
    else
      ctx->swr_ctx = (*_swr_alloc)();
    if (new_chs == -1) new_chs = ctx->audio_codec_ctx->channels;
    int64_t new_layout;
    switch (new_chs) {
      case 1: new_layout = AV_CH_LAYOUT_MONO; ctx->dst_nb_channels = 1; break;
      case 2: new_layout = AV_CH_LAYOUT_STEREO; ctx->dst_nb_channels = 2; break;
      case 4: new_layout = AV_CH_LAYOUT_QUAD; ctx->dst_nb_channels = 4; break;
      default: return false;
    }
    int64_t src_layout = ctx->audio_codec_ctx->channel_layout;
    if (src_layout == 0) {
      switch (ctx->audio_codec_ctx->channels) {
        case 1: src_layout = AV_CH_LAYOUT_MONO; break;
        case 2: src_layout = AV_CH_LAYOUT_STEREO; break;
        case 4: src_layout = AV_CH_LAYOUT_QUAD; break;
        default: return false;
      }
    }
    ctx->dst_sample_fmt = AV_SAMPLE_FMT_S16;
    ctx->src_rate = ctx->audio_codec_ctx->sample_rate;
    if (new_freq == -1) new_freq = ctx->src_rate;
    (*_av_opt_set_int)(ctx->swr_ctx, "in_channel_layout",     src_layout, 0);
    (*_av_opt_set_int)(ctx->swr_ctx, "in_sample_rate",        ctx->src_rate, 0);
    (*_av_opt_set_int)(ctx->swr_ctx, "in_sample_fmt",  ctx->audio_codec_ctx->sample_fmt, 0);
    (*_av_opt_set_int)(ctx->swr_ctx, "out_channel_layout",    new_layout, 0);
    (*_av_opt_set_int)(ctx->swr_ctx, "out_sample_rate",       new_freq, 0);
    (*_av_opt_set_int)(ctx->swr_ctx, "out_sample_fmt", ctx->dst_sample_fmt, 0);
    if (libav_org)
      (*_avresample_open)(ctx->swr_ctx);
    else
      (*_swr_init)(ctx->swr_ctx);
    ctx->dst_rate = new_freq;
  }

  if ((ctx->frame = (*_av_frame_alloc)()) == nullptr) return false;
  ctx->pkt = AVPacket_New();
  (*_av_init_packet)(ctx->pkt);
  ctx->pkt->data = nullptr;
  ctx->pkt->size = 0;
  ctx->pkt_size_left = 0;

  return true;
}

/**
 * Starts demuxing/decoding a stream.
 * @param new_width - scale video to new width (-1 = use stream width)
 * @param new_height - scale video to new height (-1 = use stream height)
 * @param new_chs - # of channels to mix to (-1 = use stream channels)
 * @param new_freq - output sampling rate (-1 = use stream rate)
 * @param seekable - can you seek input? (true=file false=stream)
 * NOTE : Audio output is always 16bit
 */

bool MediaDecoder::Start(std::shared_ptr<MediaIO> io, int new_width, int new_height, int new_chs, int new_freq, bool seekable)
{
  ctx = std::make_shared<FFContext>(io, $this.lock());

  ctx->ff_buffer = (*_av_malloc)(ffiobufsiz);
  ctx->io_ctx = (*_avio_alloc_context)(ctx->ff_buffer, ffiobufsiz, 0, (void*)ctx.get(), (void*)&read_packet, (void*)&write_packet, seekable ? (void*)&seek_packet : nullptr);
  ctx->fmt_ctx = (*_avformat_alloc_context)();
  ctx->fmt_ctx->pb = ctx->io_ctx;
  int res;
  if ((res = (*_avformat_open_input)((void**)&ctx->fmt_ctx, "stream", nullptr, nullptr)) != 0) {
    printf("avformat_open_input() failed : %d\n", res);
    return false;
  }
  if ((res = (*_avformat_find_stream_info)(ctx->fmt_ctx, nullptr)) < 0) {
    printf("avformat_find_stream_info() failed : %d\n", res);
    return false;
  }
  (*_av_dump_format)(ctx->fmt_ctx, 0, "memory_io", 0);
  return open_codecs(ctx, new_width, new_height, new_chs, new_freq);
}

/**
 * Alternative start that works with files.
 *
 * Example: start("/dev/video0", "v4l2", ...);
 *
 * NOTE:input_format may be nullptr
 */

bool MediaDecoder::Start(std::shared_ptr<String> file, std::shared_ptr<String> input_format, int new_width, int new_height, int new_chs, int new_freq)
{
  ctx = std::make_shared<FFContext>($this.lock());
  int res;
  ctx->fmt_ctx = (*_avformat_alloc_context)();
  const char *cinput_format = input_format->cstring();
  if (cinput_format != nullptr) {
    ctx->input_fmt = (*_av_find_input_format)(cinput_format);
    if (ctx->input_fmt == nullptr) {
      printf("FFMPEG:av_find_input_format failed:%s\n", cinput_format);
      return false;
    }
  }
  const char *cfile = file->cstring();
  if ((res = (*_avformat_open_input)((void**)&ctx->fmt_ctx, cfile, ctx->input_fmt, nullptr)) != 0) {
    printf("avformat_open_input() failed : %d\n", res);
    return false;
  }

  (*_av_dump_format)(ctx->fmt_ctx, 0, "memory_io", 0);

  if ((res = (*_avformat_find_stream_info)(ctx->fmt_ctx, nullptr)) < 0) {
    printf("avformat_find_stream_info() failed : %d\n", res);
    return false;
  }
  return open_codecs(ctx, new_width, new_height, new_chs, new_freq);
}

void MediaDecoder::Stop()
{
  if (ctx->io_ctx != nullptr) {
    (*_avio_flush)(ctx->io_ctx);
    ctx->io_ctx = nullptr;
    ctx->ff_buffer = nullptr;
  }
  if (ctx->fmt_ctx != nullptr) {
    (*_avformat_free_context)(ctx->fmt_ctx);
    ctx->fmt_ctx = nullptr;
  }
  if (ctx->frame != nullptr) {
    (*_av_frame_free)((void**)&ctx->frame);
  }
  if (ctx->video_dst_data[0] != nullptr) {
    (*_av_free)(ctx->video_dst_data[0]);
    ctx->video_dst_data[0] = nullptr;
  }
  if (ctx->rgb_video_dst_data[0] != nullptr) {
    (*_av_free)(ctx->rgb_video_dst_data[0]);
    ctx->rgb_video_dst_data[0] = nullptr;
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
  if (ctx->audio != nullptr) {
    ctx->audio = nullptr;
  }
  if (ctx->video != nullptr) {
    ctx->video = nullptr;
  }
  if (ctx->pkt != nullptr) {
    (*_av_free_packet)(ctx->pkt);
    (*_av_free)(ctx->pkt);
    ctx->pkt = nullptr;
  }
  ctx = nullptr;
}

/** Reads next frame in stream and returns what type it was : AUDIO_FRAME, VIDEO_FRAME, NULL_FRAME or END_FRAME */
int MediaDecoder::Read()
{
  //read another frame
  if (ctx->pkt_size_left == 0) {
    if ((*_av_read_frame)(ctx->fmt_ctx, ctx->pkt) >= 0) {
      ctx->pkt_size_left = ctx->pkt->size;
      ctx->pkt_key_frame = ((ctx->pkt->flags & 0x0001) == 0x0001);
    } else {
      return END_FRAME;
    }
  }

  //try to decode another frame
  if (ctx->pkt->stream_index == ctx->video_stream_idx) {
    //extract a video frame
    int got_frame = 0;
    int ret = (*_avcodec_decode_video2)(ctx->video_codec_ctx, ctx->frame, &got_frame, ctx->pkt);
    if (ret < 0) {
      ctx->pkt_size_left = 0;
      printf("Error:%d\n", ret);
      return NULL_FRAME;
    }
    (*_av_free_packet)(ctx->pkt);
    ctx->pkt_size_left = 0;  //use entire packet
    if (got_frame == 0) return NULL_FRAME;
    (*_av_image_copy)(ctx->video_dst_data, ctx->video_dst_linesize
      , ctx->frame->data, ctx->frame->linesize
      , ctx->video_codec_ctx->pix_fmt, ctx->video_codec_ctx->width, ctx->video_codec_ctx->height);
    //convert image to RGBA format
    (*_sws_scale)(ctx->sws_ctx, ctx->video_dst_data, ctx->video_dst_linesize, 0, ctx->video_codec_ctx->height
      , ctx->rgb_video_dst_data, ctx->rgb_video_dst_linesize);
    return VIDEO_FRAME;
  }

  if (ctx->pkt->stream_index == ctx->audio_stream_idx) {
    //extract an audio frame
    int got_frame = 0;
    int ret = (*_avcodec_decode_audio4)(ctx->audio_codec_ctx, ctx->frame, &got_frame, ctx->pkt);
    if (ret < 0) {
      ctx->pkt_size_left = 0;
      printf("Error:%d\n", ret);
      return NULL_FRAME;
    }
    ret = min(ctx->pkt_size_left, ret);
    ctx->pkt_size_left -= ret;
    if (ctx->pkt_size_left == 0) {
      (*_av_free_packet)(ctx->pkt);
    }
    if (got_frame == 0) {
      return NULL_FRAME;
    }
//    int unpadded_linesize = frame.nb_samples * avutil.av_get_bytes_per_sample(audio_codec_ctx.sample_fmt);
    //convert to new format
    int dst_nb_samples;
    if (libav_org) {
      dst_nb_samples = (int)(*_av_rescale_rnd)((*_avresample_get_delay)(ctx->swr_ctx)
        + ctx->frame->nb_samples, ctx->dst_rate, ctx->src_rate, AV_ROUND_UP);
    } else {
      dst_nb_samples = (int)(*_av_rescale_rnd)((*_swr_get_delay)(ctx->swr_ctx, ctx->src_rate)
        + ctx->frame->nb_samples, ctx->dst_rate, ctx->src_rate, AV_ROUND_UP);
    }
    if (((*_av_samples_alloc)(ctx->audio_dst_data, ctx->audio_dst_linesize, ctx->dst_nb_channels
      , dst_nb_samples, ctx->dst_sample_fmt, 1)) < 0) return NULL_FRAME;
    int converted_nb_samples = 0;
    if (libav_org) {
      converted_nb_samples = (*_avresample_convert)(ctx->swr_ctx, ctx->audio_dst_data, 0, dst_nb_samples
        , ctx->frame->extended_data, 0, ctx->frame->nb_samples);
    } else {
      converted_nb_samples = (*_swr_convert)(ctx->swr_ctx, ctx->audio_dst_data, dst_nb_samples
        , ctx->frame->extended_data, ctx->frame->nb_samples);
    }
    if (converted_nb_samples < 0) {
      printf("FFMPEG:Resample failed!\n");
      return NULL_FRAME;
    }
    int count = converted_nb_samples * ctx->dst_nb_channels;
    if (ctx->audio == nullptr || ctx->audio_length != count) {
      ctx->audio_length = count;
      ctx->audio = std::make_shared<QSharpArray<short>>(count);
    }
    std::memcpy(ctx->audio->data(), (const short*)ctx->audio_dst_data[0], ctx->audio_length * 2);
    //free audio_dst_data
    if (ctx->audio_dst_data[0] != nullptr) {
      (*_av_free)(ctx->audio_dst_data[0]);
      ctx->audio_dst_data[0] = nullptr;
    }
    return AUDIO_FRAME;
  }

  //discard unknown packet
  (*_av_free_packet)(ctx->pkt);
  ctx->pkt_size_left = 0;  //use entire packet

  return NULL_FRAME;
}

std::shared_ptr<QSharpArray<int>> MediaDecoder::GetVideo()
{
  if (ctx == nullptr) return nullptr;
  if (ctx->video == nullptr) return nullptr;
  std::memcpy(ctx->video->data(), (const int*)ctx->rgb_video_dst_data[0], ctx->video_length * 4);
  return ctx->video;
}

std::shared_ptr<QSharpArray<short>> MediaDecoder::GetAudio()
{
  if (ctx == nullptr) return nullptr;
  return ctx->audio;
}

int MediaDecoder::GetWidth()
{
  if (ctx == nullptr) return 0;
  if (ctx->video_codec_ctx == nullptr) return 0;
  return ctx->video_codec_ctx->width;
}

int MediaDecoder::GetHeight()
{
  if (ctx == nullptr) return 0;
  if (ctx->video_codec_ctx == nullptr) return 0;
  return ctx->video_codec_ctx->height;
}

float MediaDecoder::GetFrameRate()
{
  if (ctx == nullptr) return 0;
  if (ctx->video_codec_ctx == nullptr) return 0;
  AVRational value = ctx->video_stream->avg_frame_rate;
  float num = (float)value.num;
  float den = (float)value.den;
  return num / den;
}

int64 MediaDecoder::GetDuration()
{
  if (ctx == nullptr) return 0;
  if (ctx->fmt_ctx == nullptr) return 0;
  if (ctx->fmt_ctx->duration << 1 == 0) return 0;  //0x8000000000000000
  return ctx->fmt_ctx->duration / AV_TIME_BASE;  //return in seconds
}

int MediaDecoder::GetSampleRate()
{
  if (ctx == nullptr) return 0;
  if (ctx->audio_codec_ctx == nullptr) return 0;
  return ctx->audio_codec_ctx->sample_rate;
}

int MediaDecoder::GetChannels()
{
  if (ctx == nullptr) return 0;
  return ctx->dst_nb_channels;
}

int MediaDecoder::GetBitsPerSample()
{
  return 16;  //output is always converted to 16bits/sample (signed)
}

bool MediaDecoder::Seek(int64 seconds)
{
  if (ctx == nullptr) return false;
  //AV_TIME_BASE is 1000000fps
  seconds *= AV_TIME_BASE;
/*      int ret = avformat.avformat_seek_file(fmt_ctx, -1
    , seconds - AV_TIME_BASE_PARTIAL, seconds, seconds + AV_TIME_BASE_PARTIAL, 0);*/
  int ret = (*_av_seek_frame)(ctx->fmt_ctx, -1, seconds, 0);
  if (ret < 0) printf("av_seek_frame failed:%d\n", ret);
  return ret >= 0;
}

int MediaDecoder::GetVideoBitRate()
{
  if (ctx == nullptr) return 0;
  if (ctx->video_codec_ctx == nullptr) return 0;
  return ctx->video_codec_ctx->bit_rate;
}

int MediaDecoder::GetAudioBitRate()
{
  if (ctx == nullptr) return 0;
  if (ctx->audio_codec_ctx == nullptr) return 0;
  return ctx->audio_codec_ctx->bit_rate;
}

bool MediaDecoder::IsKeyFrame()
{
  if (ctx == nullptr) return false;
  return ctx->pkt_key_frame;
}

bool MediaDecoder::Resize(int new_width, int new_height)
{
  if (ctx == nullptr) return false;
  if (ctx->video_stream == nullptr) return false;  //no video

  if (ctx->rgb_video_dst_data[0] != nullptr) {
    (*_av_free)(ctx->rgb_video_dst_data[0]);
    ctx->rgb_video_dst_data[0] = nullptr;
  }

  if ((ctx->rgb_video_dst_bufsize = (*_av_image_alloc)(ctx->rgb_video_dst_data, ctx->rgb_video_dst_linesize
    , new_width, new_height
    , AV_PIX_FMT_BGRA, 1)) < 0) return false;

  if (ctx->video != nullptr) {
    ctx->video = nullptr;
  }
  ctx->video_length = ctx->rgb_video_dst_bufsize/4;
  ctx->video = std::make_shared<QSharpArray<int>>(ctx->video_length);

  if (ctx->sws_ctx != nullptr) {
    (*_sws_freeContext)(ctx->sws_ctx);
    ctx->sws_ctx = nullptr;
  }

  ctx->sws_ctx = (*_sws_getContext)(ctx->video_codec_ctx->width, ctx->video_codec_ctx->height, ctx->video_codec_ctx->pix_fmt
    , new_width, new_height, AV_PIX_FMT_BGRA
    , SWS_BILINEAR, nullptr, nullptr, nullptr);

  return true;
}

}  //namespace Qt::Media

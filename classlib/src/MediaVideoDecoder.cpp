#include "Media.hpp"

namespace Qt { namespace Media {

std::shared_ptr<MediaVideoDecoder> MediaVideoDecoder::$new() {
  return std::make_shared<MediaVideoDecoder>();
}

bool Qt::Media::MediaVideoDecoder::Start(int codec_id, int new_width, int new_height)
{
  ctx = std::make_shared<FFContext>(std::dynamic_pointer_cast<MediaCoder>($weak_this.lock()));

  ctx->video_codec = (*_avcodec_find_decoder)(codec_id);
  if (ctx->video_codec == nullptr) {
    return false;
  }
  ctx->video_codec_ctx = (*_avcodec_alloc_context3)(ctx->video_codec);

  //set default values
  ctx->video_codec_ctx->codec_id = (AVCodecID)codec_id;
  ctx->video_codec_ctx->width = new_width;
  ctx->video_codec_ctx->height = new_height;
  ctx->video_codec_ctx->pix_fmt = AV_PIX_FMT_YUV420P;

  if (((*_avcodec_open2)(ctx->video_codec_ctx, ctx->video_codec, nullptr)) < 0) {
    return false;
  }

  if (new_width == -1) new_width = ctx->video_codec_ctx->width;
  if (new_height == -1) new_height = ctx->video_codec_ctx->height;
  if ((ctx->video_dst_bufsize = (*_av_image_alloc)(ctx->video_dst_data, ctx->video_dst_linesize
    , ctx->video_codec_ctx->width, ctx->video_codec_ctx->height, ctx->video_codec_ctx->pix_fmt, 1)) < 0)
  {
    return false;
  }
  if ((ctx->rgb_video_dst_bufsize = (*_av_image_alloc)(ctx->rgb_video_dst_data, ctx->rgb_video_dst_linesize
    , new_width, new_height, AV_PIX_FMT_BGRA, 1)) < 0)
  {
    return false;
  }
  //create video conversion context
  ctx->sws_ctx = (*_sws_getContext)(ctx->video_codec_ctx->width, ctx->video_codec_ctx->height, ctx->video_codec_ctx->pix_fmt
    , new_width, new_height, AV_PIX_FMT_BGRA
    , SWS_BILINEAR, nullptr, nullptr, nullptr);

  if ((ctx->frame = (*_av_frame_alloc)()) == nullptr) return false;
  ctx->pkt = AVPacket_New();
  (*_av_init_packet)(ctx->pkt);

  int px_count = new_width * new_height;
  ctx->video_length = px_count;
  ctx->video = Qt::QSharp::FixedArray1D<int>::$new(ctx->video_length);

  return true;
}

void Qt::Media::MediaVideoDecoder::Stop()
{
  if (ctx == nullptr) return;
  if (ctx->frame != nullptr) {
    (*_av_frame_free)((void**)&ctx->frame);
  }
  if (ctx->video_codec_ctx != nullptr) {
    (*_avcodec_close)(ctx->video_codec_ctx);
    (*_av_free)(ctx->video_codec_ctx);
    ctx->video_codec_ctx = nullptr;
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
  if (ctx->pkt != nullptr) {
    (*_av_free_packet)(ctx->pkt);
    (*_av_free)(ctx->pkt);
    ctx->pkt = nullptr;
  }
  if (ctx->video != nullptr) {
    ctx->video = nullptr;
  }
  ctx = nullptr;
}

Qt::QSharp::FixedArray1D<int> Qt::Media::MediaVideoDecoder::Decode(Qt::QSharp::FixedArray1D<uint8> data)
{
  if (ctx == nullptr) return nullptr;

  //read another frame
  if (ctx->pkt_size_left == 0) {
    if (ctx->pkt->data != nullptr) {
      (*_av_free)(ctx->pkt->data);
      ctx->pkt->data = nullptr;
    }
    int data_length = data.Length;
    ctx->pkt->data = (uint8_t*)(*_av_malloc)(data_length);
    uint8 *data_ptr = data.data();
    std::memcpy(ctx->pkt->data, data_ptr, data_length);
    ctx->pkt->size = data_length;
    ctx->pkt_size_left = ctx->pkt->size;
  }

  //extract a video frame
  int got_frame = 0;
  if ((*_avcodec_decode_video2)(ctx->video_codec_ctx, ctx->frame, &got_frame, ctx->pkt) < 0) {
    ctx->pkt_size_left = 0;
    return nullptr;
  }
  if (got_frame == 0) return nullptr;
  (*_av_image_copy)(ctx->video_dst_data, ctx->video_dst_linesize
    , ctx->frame->data, ctx->frame->linesize
    , ctx->video_codec_ctx->pix_fmt, ctx->video_codec_ctx->width, ctx->video_codec_ctx->height);
  //convert image to RGBA format
  (*_sws_scale)(ctx->sws_ctx, ctx->video_dst_data, ctx->video_dst_linesize, 0, ctx->video_codec_ctx->height
    , ctx->rgb_video_dst_data, ctx->rgb_video_dst_linesize);

  ctx->pkt_size_left = 0;  //use entire packet
  //do NOT free pkt - it's done in next decode() or stop()

  std::memcpy((void*)ctx->video->data(), (const int*)ctx->rgb_video_dst_data[0], ctx->video_length * 4);

  return ctx->video;
}

int Qt::Media::MediaVideoDecoder::GetWidth()
{
  if (ctx == nullptr) return 0;
  if (ctx->video_codec_ctx == nullptr) return 0;
  return ctx->video_codec_ctx->width;
}

int Qt::Media::MediaVideoDecoder::GetHeight()
{
  if (ctx == nullptr) return 0;
  if (ctx->video_codec_ctx == nullptr) return 0;
  return ctx->video_codec_ctx->height;
}

float Qt::Media::MediaVideoDecoder::GetFrameRate()
{
  if (ctx == nullptr) return 0;
  if (ctx->video_codec_ctx == nullptr) return 0;
  return ctx->video_codec_ctx->time_base.den / ctx->video_codec_ctx->time_base.num / ctx->video_codec_ctx->ticks_per_frame;
}

} }  //namespace Qt::Media

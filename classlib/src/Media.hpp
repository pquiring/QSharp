#ifndef MEDIA_HPP
#define MEDIA_HPP

#ifdef __WIN32__
  #define UINT64_C(val) val##ULL  //stdint.h ???
#endif

#include <libavcodec/avcodec.h>
#include <libavformat/avformat.h>
#include <libavutil/avutil.h>
#include <libavutil/channel_layout.h>
#include <libavutil/mathematics.h>
#include <libswscale/swscale.h>

namespace Qt { namespace Media {

static bool libav_org = false;
static bool loaded = false;

QLibrary *codec = nullptr;
QLibrary *device = nullptr;
QLibrary *filter = nullptr;
QLibrary *format = nullptr;
QLibrary *util = nullptr;
QLibrary *resample = nullptr;
QLibrary *postproc = nullptr;
QLibrary *scale = nullptr;

static void $getFunction(QLibrary *lib, void**funcptr, const char *func) {
  *funcptr = (void*)lib->resolve(func);
}

//avcodec functions
void (*_avcodec_register_all)();
AVCodec* (*_avcodec_find_decoder)(int codec_id);
int (*_avcodec_decode_video2)(AVCodecContext *avctx,AVFrame *picture,int* got_picture_ptr,AVPacket *avpkt);
int (*_avcodec_decode_audio4)(AVCodecContext *avctx,AVFrame *frame,int* got_frame_ptr,AVPacket *avpkt);
int (*_avcodec_open2)(AVCodecContext *avctx,AVCodec *codec,void* options);
AVCodecContext* (*_avcodec_alloc_context3)(AVCodec *codec);
void (*_av_init_packet)(AVPacket *pkt);
void (*_av_free_packet)(AVPacket *pkt);  //free data inside packet (not packet itself)
//encoding
AVCodec* (*_avcodec_find_encoder)(int codec_id);
//int (*_avpicture_alloc)(AVPicture *pic, int pix_fmt, int width, int height);
//int (*_avpicture_free)(AVPicture *pic);
int (*_avcodec_encode_video2)(AVCodecContext *cc, AVPacket *pkt, AVFrame *frame, int* intref);
int (*_avcodec_encode_audio2)(AVCodecContext *cc, AVPacket *pkt, AVFrame *frame, int* intref);
int (*_avcodec_fill_audio_frame)(AVFrame *frame, int nb_channels, int fmt, void* buf, int bufsize, int align);
int (*_avcodec_close)(AVCodecContext *cc);
const char* (*_avcodec_get_name)(AVCodecID id);
void (*_av_packet_rescale_ts)(AVPacket *pkt, AVRational src, AVRational dst);
int (*_avcodec_parameters_to_context)(AVCodecContext *ctx, const AVCodecParameters *par);
int (*_avcodec_parameters_from_context)(AVCodecParameters *par, const AVCodecContext *ctx);

//avdevice functions
void (*_avdevice_register_all)();

//avfilter functions
void (*_avfilter_register_all)();

//avformat functions
void (*_av_register_all)();
void (*_av_register_output_format)(AVOutputFormat *oformat);
AVOutputFormat* (*_av_guess_format)(const char* shortName, const char* fileName, const char* mimeType);
int (*_av_find_best_stream)(AVFormatContext *ic,int type,int wanted_stream_nb,int related_stream,void** decoder_ret, int flags);
AVIOContext* (*_avio_alloc_context)(void* buffer,int buffer_size,int write_flag,void* opaque,void* read,void* write,void* seek);
AVFormatContext* (*_avformat_alloc_context)();
int (*_avio_close)(void* ctx);
void (*_avformat_free_context)(AVFormatContext *s);
int (*_avformat_open_input)(void** ps,const char* filename,void* fmt,void* options);
int (*_avformat_find_stream_info)(AVFormatContext *ic,void** options);
int (*_av_read_frame)(AVFormatContext *s,AVPacket *pkt);
void* (*_av_find_input_format)(const char* name);
void* (*_av_iformat_next)(void* ptr);
int (*_avformat_seek_file)(AVFormatContext *ctx, int stream_idx, int64_t min_ts, int64_t ts, int64_t max_ts, int flags);
int (*_av_seek_frame)(AVFormatContext *ctx, int stream_idx, int64_t ts, int flags);
//encoding
AVStream* (*_avformat_new_stream)(AVFormatContext *fc, AVCodec *codec);
int (*_avformat_write_header)(AVFormatContext *fc, void* opts);
int (*_av_interleaved_write_frame)(AVFormatContext *fc, AVPacket *pkt);
int (*_av_write_frame)(AVFormatContext *fc, AVPacket *pkt);
int (*_avio_flush)(void* io_ctx);
void (*_av_dump_format)(AVFormatContext *fmt_ctx, int index, const char* url, int is_output);
int (*_av_write_trailer)(AVFormatContext *fc);

//avutil functions
void (*_av_image_copy)(uint8_t* dst_data[],int dst_linesizes[],uint8_t* src_data[],int src_linesizes[],int pix_fmt,int width,int height);
int (*_av_get_bytes_per_sample)(int sample_fmt);
void* (*_av_malloc)(int size);
void* (*_av_mallocz)(int size);
void (*_av_free)(void* ptr);
void (*_av_freep)(void** ptr);
int (*_av_image_alloc)(uint8_t* ptrs[],int linesizes[],int w,int h,int pix_fmt,int align);
int (*_av_opt_set)(void* obj,const char* name,const char* val,int search_flags);
int (*_av_opt_set_int)(void* obj,const char* name,int64_t val,int search_flags);
int (*_av_opt_get)(void* obj,const char* name,int search_flags,void* val[]);
int (*_av_opt_get_int)(void* obj,const char* name,int search_flags,int64_t val[]);
//    int (*_av_opt_get_pixel_fmt)(void* obj,const char* name,int search_flags,int val[]);
void* (*_av_opt_find)(void* obj, const char* name, const char* unit, int opt_flags, int search_flags);
void* (*_av_opt_next)(void* obj, void* prev);
void* (*_av_opt_child_next)(void* obj, void* prev);
void (*_av_opt_set_defaults)(void* obj);
int64_t (*_av_rescale_rnd)(int64_t a,int64_t b,int64_t c,AVRounding r);
int (*_av_samples_alloc)(uint8_t* audio_data[],int linesize[],int nb_channels,int nb_samples,int sample_fmt,int align);
int64_t (*_av_rescale_q)(int64_t a, AVRational bq, AVRational cq);
int (*_av_samples_get_buffer_size)(void* linesize, int chs, int samples, int sample_fmt, int align);
int (*_av_log_set_level)(int lvl);
void* (*_av_dict_get)(void* dict, const char* key, void* prev, int flags);
int (*_av_dict_set)(void** dictref, const char* key, const char* value, int flags);
int (*_av_frame_make_writable)(AVFrame *frame);
int (*_av_compare_ts)(int64_t ts_a, AVRational tb_a, int64_t ts_b, AVRational tb_b);
int (*_av_frame_get_buffer)(AVFrame *frame, int align);
AVFrame* (*_av_frame_alloc)();
void (*_av_frame_free)(void** frame);

//swresample functions )(ffmpeg.org)
void* (*_swr_alloc)();
int (*_swr_init)(void* ctx);
int64_t (*_swr_get_delay)(void* ctx,int64_t base);
int (*_swr_convert)(void* ctx,uint8_t* out_arg[],int out_count,uint8_t* in_arg[],int in_count);
void (*_swr_free)(void** ctx);

//avresample functions )(libav.org);
void* (*_avresample_alloc_context)();
int (*_avresample_open)(void* ctx);
int (*_avresample_free)(void* ctx);
int64_t (*_avresample_get_delay)(void* ctx);
int (*_avresample_convert)(void* ctx,uint8_t* out_arg[],int out_plane_size, int out_count,uint8_t* in_arg[],int in_plane_size,int in_count);

//swscale functions
void* (*_sws_getContext)(int srcW,int srcH,int srcFormat,int dstW,int dstH,int dstFormat,int flags,void* srcFilter,void* dstFilter, void* param);
int (*_sws_scale)(void* c, uint8_t* srcSlice[],int srcStride[],int srcSliceY,int srcSliceH,uint8_t* dst[],int dstStride[]);
void (*_sws_freeContext)(void* ctx);

static AVPacket *AVPacket_New() {
  AVPacket *pkt = (AVPacket*)(*_av_malloc)(sizeof(AVPacket));
  memset(pkt, 0, sizeof(AVPacket));
  return pkt;
}

static AVOutputFormat *AVOutputFormat_New() {
  AVOutputFormat *ofmt = (AVOutputFormat*)(*_av_malloc)(sizeof(AVOutputFormat));
  memset(ofmt, 0, sizeof(AVOutputFormat));
  return ofmt;
}

/*
static AVPicture *AVPicture_New() {
  AVPicture *pic = (AVPicture*)(*_av_malloc)(sizeof(AVPicture));
  memset(pic, 0, sizeof(AVPicture));
  return pic;
}
*/

static AVOutputFormat *vpx = nullptr;

static int min(int a, int b) {
  if (a < b) return a; else return b;
}

struct FFContext {
  std::shared_ptr<MediaIO> io;
  std::shared_ptr<MediaCoder> coder;

  FFContext(std::shared_ptr<MediaIO> io, std::shared_ptr<MediaCoder> coder) {
    this->io = io;
    this->coder = coder;
  }
  FFContext(std::shared_ptr<MediaCoder> coder) {
    this->coder = coder;
  }

  //decoder fields
  void *ff_buffer;
  AVFormatContext *fmt_ctx;
  AVIOContext *io_ctx;

  void* input_fmt;

  AVCodecContext *codec_ctx;  //returned by open_codec_context()

  int video_stream_idx;
  AVStream *video_stream;
  AVCodecContext *video_codec_ctx;
  void* sws_ctx;
  //compressed video
  int video_dst_bufsize;
  uint8_t* video_dst_data[4];
  int video_dst_linesize[4];
  //rgb video
  int rgb_video_dst_bufsize;
  uint8_t* rgb_video_dst_data[4];
  int rgb_video_dst_linesize[4];

  int audio_stream_idx;
  AVStream *audio_stream;
  AVCodecContext *audio_codec_ctx;
  void* swr_ctx;

  //compressed audio
  int src_rate;
  //user audio
  int dst_rate;
  int dst_nb_channels;
  int dst_sample_fmt;
  uint8_t* audio_dst_data[4];
  int audio_dst_linesize[4];

  AVPacket *pkt;  //decoders only
  int pkt_size_left;
  bool pkt_key_frame;

  AVFrame *frame;

  std::shared_ptr<Qt::QSharp::FixedArray<int>> video;
  int video_length;

  std::shared_ptr<Qt::QSharp::FixedArray<short>> audio;
  int audio_length;

  //additional raw video decoder fields
  AVCodec *video_codec;

  //additional encoder fields
  AVCodec *audio_codec;
  AVOutputFormat *out_fmt;
  int width, height, fps;
  int chs, freq;
  AVFrame *audio_frame, *video_frame;
  AVFrame *src_pic, *dst_pic;
  bool audio_frame_size_variable;
  int audio_frame_size;
  short *audio_buffer;
  int audio_buffer_size;
  int64_t audio_pts;
  int64_t video_pts;
  AVRational audio_ratio;

  uint8_t* audio_src_data[4];

  /** Set to make fps = fps * 1000 / 1001. */
  bool fps_1000_1001;

  /** Number of frames per group of pictures (GOP).
   * Determines how often key frame is generated.
   * Default = 12
   */
  int config_gop_size;

  /** Video bit rate.
   * Default = 400000
   */
  int config_video_bit_rate;

  /** Audio bit rate.
   * Default = 128000
   */
  int config_audio_bit_rate;
};

#define ffiobufsiz (32 * 1024)

static int read_packet(FFContext *ctx, void*buf, int size) {
  std::shared_ptr<Qt::QSharp::FixedArray<uint8>> array = Qt::QSharp::FixedArray<uint8>::$new(buf, size);
  return ctx->io->Read(ctx->coder, array);
}

static int write_packet(FFContext *ctx, void*buf, int size) {
  std::shared_ptr<Qt::QSharp::FixedArray<uint8>> array = Qt::QSharp::FixedArray<uint8>::$new(buf, size);
  return ctx->io->Write(ctx->coder, array);
}

static uint64 zero64 = 0;

static int64 seek_packet(FFContext *ctx, int64 offset, int how) {
  switch (how) {
    case AVSEEK_SIZE:
      return ctx->io->GetSize(ctx->coder);
    case SEEK_CUR:
      offset += ctx->io->GetPosition(ctx->coder);
      return ctx->io->Seek(ctx->coder, offset);
    case SEEK_SET:
      return ctx->io->Seek(ctx->coder, offset);
    default:
      return -1;
  }
}

} } //namespace Qt::Media

#endif

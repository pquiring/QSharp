#include "Media.hpp"

namespace Qt { namespace Media {

static void register_vpx() {
  vpx = (*_av_guess_format)("vpx", nullptr, nullptr);
  if (vpx != nullptr) {
    return;
  }
  //basically just clone h264
  AVOutputFormat *h264 = (*_av_guess_format)("h264", nullptr, nullptr);
  if (h264 == nullptr) {
    printf("FFMPEG:Unable to register vpx codec\n");
    return;
  }
  vpx = AVOutputFormat_New();
  vpx->name = "vpx";
  vpx->long_name = "raw vpx";
  vpx->extensions = "vpx";
  vpx->audio_codec = AV_CODEC_ID_NONE;
  vpx->video_codec = AV_CODEC_ID_VP8;
  vpx->write_packet = h264->write_packet;
  vpx->flags = h264->flags;
  (*_av_register_output_format)(vpx);
}

#ifndef __WIN32__
int GetLastError() {
  return errno;
}
#endif

static QLibrary* $lib_open(const char*filename) {
  QLibrary* lib = new QLibrary(filename);
  if (!lib->load()) {delete lib; return nullptr;}
  return lib;
}

bool MediaCoder::Init()
{
  if (loaded) return true;

  //load libraries (order is important)

  util = $lib_open("avutil");
  if (util == nullptr) {
    return false;
  }

  resample = $lib_open("swresample");
  if (resample == nullptr) {
    resample = $lib_open("swresample");
    if (resample == nullptr) {
      return false;
    }
    libav_org = true;
  } else {
    libav_org = false;
  }

  scale = $lib_open("swscale");
  if (scale == nullptr) {
    return false;
  }

  postproc = $lib_open("postproc");
  if (postproc == nullptr) {
    return false;
  }

  codec = $lib_open("avcodec");
  if (codec == nullptr) {
    return false;
  }

  format = $lib_open("avformat");
  if (format == nullptr)  {
    return false;
  }

  filter = $lib_open("avfilter");
  if (filter == nullptr) {
    return false;
  }

  device = $lib_open("avdevice");
  if (device == nullptr) {
    return false;
  }

  //get functions
  $getFunction(codec, (void**)&_avcodec_register_all, "avcodec_register_all");
  $getFunction(codec, (void**)&_avcodec_find_decoder, "avcodec_find_decoder");
  $getFunction(codec, (void**)&_avcodec_decode_video2, "avcodec_decode_video2");
  $getFunction(codec, (void**)&_avcodec_decode_audio4, "avcodec_decode_audio4");
  $getFunction(codec, (void**)&_avcodec_open2, "avcodec_open2");
  $getFunction(codec, (void**)&_avcodec_alloc_context3, "avcodec_alloc_context3");
  $getFunction(codec, (void**)&_av_init_packet, "av_init_packet");
  $getFunction(codec, (void**)&_av_free_packet, "av_free_packet");
  $getFunction(codec, (void**)&_avcodec_find_encoder, "avcodec_find_encoder");
//  $getFunction(codec, (void**)&_avpicture_alloc, "avpicture_alloc");
//  $getFunction(codec, (void**)&_avpicture_free, "avpicture_free");
  $getFunction(codec, (void**)&_avcodec_encode_video2, "avcodec_encode_video2");
  $getFunction(codec, (void**)&_avcodec_encode_audio2, "avcodec_encode_audio2");
  $getFunction(codec, (void**)&_avcodec_fill_audio_frame, "avcodec_fill_audio_frame");
  $getFunction(codec, (void**)&_avcodec_close, "avcodec_close");
  if (!libav_org) {
    $getFunction(codec, (void**)&_avcodec_get_name, "avcodec_get_name");  //for debug output only
  }
  $getFunction(codec, (void**)&_av_packet_rescale_ts, "av_packet_rescale_ts");
  $getFunction(codec, (void**)&_avcodec_parameters_to_context, "avcodec_parameters_to_context");
  $getFunction(codec, (void**)&_avcodec_parameters_from_context, "avcodec_parameters_from_context");

  $getFunction(device, (void**)&_avdevice_register_all, "avdevice_register_all");

  $getFunction(filter, (void**)&_avfilter_register_all, "avfilter_register_all");

  $getFunction(format, (void**)&_av_register_all, "av_register_all");
  $getFunction(format, (void**)&_av_register_output_format, "av_register_output_format");
  $getFunction(format, (void**)&_av_guess_format, "av_guess_format");
  $getFunction(format, (void**)&_av_find_best_stream, "av_find_best_stream");
  $getFunction(format, (void**)&_avio_alloc_context, "avio_alloc_context");
  $getFunction(format, (void**)&_avformat_alloc_context, "avformat_alloc_context");
  $getFunction(format, (void**)&_avio_close, "avio_close");
  $getFunction(format, (void**)&_avformat_free_context, "avformat_free_context");
  $getFunction(format, (void**)&_avformat_open_input, "avformat_open_input");
  $getFunction(format, (void**)&_avformat_find_stream_info, "avformat_find_stream_info");
  $getFunction(format, (void**)&_av_read_frame, "av_read_frame");
  $getFunction(format, (void**)&_av_find_input_format, "av_find_input_format");
  $getFunction(format, (void**)&_av_iformat_next, "av_iformat_next");
  $getFunction(format, (void**)&_avformat_seek_file, "avformat_seek_file");
  $getFunction(format, (void**)&_av_seek_frame, "av_seek_frame");
  $getFunction(format, (void**)&_avformat_new_stream, "avformat_new_stream");
  $getFunction(format, (void**)&_avformat_write_header, "avformat_write_header");
  $getFunction(format, (void**)&_av_interleaved_write_frame, "av_interleaved_write_frame");
  $getFunction(format, (void**)&_av_write_frame, "av_write_frame");
  $getFunction(format, (void**)&_avio_flush, "avio_flush");
  $getFunction(format, (void**)&_av_dump_format, "av_dump_format");
  $getFunction(format, (void**)&_av_write_trailer, "av_write_trailer");

  $getFunction(util, (void**)&_av_image_copy, "av_image_copy");
  $getFunction(util, (void**)&_av_get_bytes_per_sample, "av_get_bytes_per_sample");
  $getFunction(util, (void**)&_av_malloc, "av_malloc");
  $getFunction(util, (void**)&_av_mallocz, "av_mallocz");
  $getFunction(util, (void**)&_av_free, "av_free");
  $getFunction(util, (void**)&_av_freep, "av_freep");
  $getFunction(util, (void**)&_av_image_alloc, "av_image_alloc");
  $getFunction(util, (void**)&_av_opt_set, "av_opt_set");
  $getFunction(util, (void**)&_av_opt_set_int, "av_opt_set_int");
  $getFunction(util, (void**)&_av_opt_get, "av_opt_get");
  $getFunction(util, (void**)&_av_opt_get_int, "av_opt_get_int");
  $getFunction(util, (void**)&_av_opt_find, "av_opt_find");
  $getFunction(util, (void**)&_av_opt_next, "av_opt_next");
  $getFunction(util, (void**)&_av_opt_child_next, "av_opt_child_next");
  $getFunction(util, (void**)&_av_opt_set_defaults, "av_opt_set_defaults");
  $getFunction(util, (void**)&_av_rescale_rnd, "av_rescale_rnd");
  $getFunction(util, (void**)&_av_samples_alloc, "av_samples_alloc");
  $getFunction(util, (void**)&_av_rescale_q, "av_rescale_q");
  $getFunction(util, (void**)&_av_samples_get_buffer_size, "av_samples_get_buffer_size");
  $getFunction(util, (void**)&_av_log_set_level, "av_log_set_level");
  $getFunction(util, (void**)&_av_dict_get, "av_dict_get");
  $getFunction(util, (void**)&_av_dict_set, "av_dict_set");
  $getFunction(util, (void**)&_av_frame_make_writable, "av_frame_make_writable");
  $getFunction(util, (void**)&_av_compare_ts, "av_compare_ts");
  $getFunction(util, (void**)&_av_frame_get_buffer, "av_frame_get_buffer");
  $getFunction(util, (void**)&_av_frame_alloc, "av_frame_alloc");
  $getFunction(util, (void**)&_av_frame_free, "av_frame_free");

  $getFunction(scale, (void**)&_sws_getContext, "sws_getContext");
  $getFunction(scale, (void**)&_sws_scale, "sws_scale");
  $getFunction(scale, (void**)&_sws_freeContext, "sws_freeContext");

  if (!libav_org) {
    $getFunction(resample, (void**)&_swr_alloc, "swr_alloc");
    $getFunction(resample, (void**)&_swr_init, "swr_init");
    $getFunction(resample, (void**)&_swr_get_delay, "swr_get_delay");
    $getFunction(resample, (void**)&_swr_convert, "swr_convert");
    $getFunction(resample, (void**)&_swr_free, "swr_free");
  } else {
    $getFunction(resample, (void**)&_avresample_alloc_context, "avresample_alloc_context");
    $getFunction(resample, (void**)&_avresample_open, "avresample_open");
    $getFunction(resample, (void**)&_avresample_free, "avresample_free");
    $getFunction(resample, (void**)&_avresample_get_delay, "avresample_get_delay");
    $getFunction(resample, (void**)&_avresample_convert, "avresample_convert");
  }

  //register_all
  (*_avcodec_register_all)();
  (*_avdevice_register_all)();
  (*_avfilter_register_all)();
  (*_av_register_all)();
  register_vpx();

  return true;
}

} }  //namespace Qt::Media

﻿using System;
using FFmpeg.AutoGen;

namespace Captura.FFmpeg.Interop
{
    public unsafe class FFmpegStream : IDisposable
    {
        AVCodec* Codec { get; }

        public AVCodecContext* CodecContext { get; }

        protected AVStream* Stream { get; }

        protected FFmpegStream(AVFormatContext* FormatContext, FFmpegCodecInfo CodecInfo)
        {
            Codec = CodecInfo.Codec;

            if (Codec == null)
            {
                throw new Exception("Could not find Codec");
            }

            Stream = ffmpeg.avformat_new_stream(FormatContext, Codec);

            if (Stream == null)
            {
                throw new Exception("Could not allocate stream");
            }

            Stream->id = (int)(FormatContext->nb_streams - 1);

            CodecContext = Stream->codec;

            if ((FormatContext->oformat->flags & ffmpeg.AVFMT_GLOBALHEADER) != 0)
            {
                CodecContext->flags = ffmpeg.AV_CODEC_FLAG_GLOBAL_HEADER;
            }
        }

        protected void OpenCodec()
        {
            ffmpeg.avcodec_open2(CodecContext, Codec, null).ThrowExceptionIfError();
        }

        public virtual void Dispose()
        {
            ffmpeg.avcodec_close(CodecContext);
        }
    }
}
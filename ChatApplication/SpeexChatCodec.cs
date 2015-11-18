using System;
using System.Linq;
using NAudio.Wave;
using NSpeex;
using System.ComponentModel.Composition;
using System.Diagnostics;

namespace NAudioDemo.NetworkChatDemo {
    class SpeexChatCodec {
        private readonly WaveFormat recordingFormat;
        private readonly SpeexDecoder decoder;
        private readonly SpeexEncoder encoder;
        private readonly WaveBuffer encoderInputBuffer;
        private readonly string description;

        public SpeexChatCodec(BandMode bandMode, int sampleRate, string description) {
            decoder = new SpeexDecoder(bandMode);
            encoder = new SpeexEncoder(bandMode);
            recordingFormat = new WaveFormat(sampleRate, 16, 1);
            this.description = description;
            encoderInputBuffer = new WaveBuffer(recordingFormat.AverageBytesPerSecond);
        }

        public string Name {
            get { return description; }
        }

        public int BitsPerSecond {
            get { return -1; }
        }

        public WaveFormat RecordFormat {
            get { return recordingFormat; }
        }

        public byte[] Encode(byte[] data, int offset, int length) {
            FeedSamplesIntoEncoderInputBuffer(data, offset, length);
            int samplesToEncode = encoderInputBuffer.ShortBufferCount;
            if (samplesToEncode % encoder.FrameSize != 0) {
                samplesToEncode -= samplesToEncode % encoder.FrameSize;
            }
            var outputBufferTemp = new byte[length];
            int bytesWritten = encoder.Encode(encoderInputBuffer.ShortBuffer, 0, samplesToEncode, outputBufferTemp, 0, length);
            var encoded = new byte[bytesWritten];
            Array.Copy(outputBufferTemp, 0, encoded, 0, bytesWritten);
            ShiftLeftoverSamplesDown(samplesToEncode);
            return encoded;
        }

        private void ShiftLeftoverSamplesDown(int samplesEncoded) {
            int leftoverSamples = encoderInputBuffer.ShortBufferCount - samplesEncoded;
            Array.Copy(encoderInputBuffer.ByteBuffer, samplesEncoded * 2, encoderInputBuffer.ByteBuffer, 0, leftoverSamples * 2);
            encoderInputBuffer.ShortBufferCount = leftoverSamples;
        }

        private void FeedSamplesIntoEncoderInputBuffer(byte[] data, int offset, int length) {
            Array.Copy(data, offset, encoderInputBuffer.ByteBuffer, encoderInputBuffer.ByteBufferCount, length);
            encoderInputBuffer.ByteBufferCount += length;
        }

        public byte[] Decode(byte[] data, int offset, int length) {
            var outputBufferTemp = new byte[length * 320];
            var wb = new WaveBuffer(outputBufferTemp);
            int samplesDecoded = decoder.Decode(data, offset, length, wb.ShortBuffer, 0, false);
            int bytesDecoded = samplesDecoded * 2;
            var decoded = new byte[bytesDecoded];
            Array.Copy(outputBufferTemp, 0, decoded, 0, bytesDecoded);
            return decoded;
        }

        public void Dispose() {
            // nothing to do
        }

        public bool IsAvailable { get { return true; } }
    }
}

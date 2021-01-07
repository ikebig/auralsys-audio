using System;

namespace Auralsys.Audio
{
    public class AudioSamplesConverter : IAudioSamplesConverter
    {
        #region Private Utils

        private float[] Convert8BitToFloat(byte[] input)
        {
            int inputSamples = input.Length; // 8 bit input, so 1 byte per sample
            float[] output = new float[inputSamples];
            int outputIndex = 0;
            for (int n = 0; n < inputSamples; n++)
            {
                short sample = Convert.ToInt16(input[n]);
                output[outputIndex++] = sample / 128f;
            }

            return output;
        }

        private float[] Convert16BitToFloat(byte[] input)
        {
            int inputSamples = input.Length / 2; // 16 bit input, so 2 bytes per sample
            float[] output = new float[inputSamples];
            int outputIndex = 0;
            for (int n = 0; n < inputSamples; n++)
            {
                short sample = BitConverter.ToInt16(input, n * 2);
                output[outputIndex++] = sample / 32768f;
            }
            return output;
        }

        private float[] Convert24BitToFloat(byte[] input)
        {
            int inputSamples = input.Length / 3; // 24 bit input
            float[] output = new float[inputSamples];
            int outputIndex = 0;
            var temp = new byte[4];
            for (int n = 0; n < inputSamples; n++)
            {
                // copy 3 bytes in
                Array.Copy(input, n * 3, temp, 0, 3);
                int sample = BitConverter.ToInt32(temp, 0);
                output[outputIndex++] = sample / 16777216f;
            }
            return output;
        }

        #endregion
        public float[] ByteToFloat(byte[] input, BitDepth inputBitDepth)
        {
            float[] output;
            switch (inputBitDepth)
            {
                case BitDepth.Bit_8:
                    output = Convert8BitToFloat(input);
                    break;
                case BitDepth.Bit_16:
                    output = Convert16BitToFloat(input);
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }

            return output;
        }

        public byte[] MonoToStereo(byte[] input)
        {
            byte[] output = new byte[input.Length * 2];
            int outputIndex = 0;
            for (int n = 0; n < input.Length; n += 2)
            {
                // copy in the first 16 bit sample
                output[outputIndex++] = input[n];
                output[outputIndex++] = input[n + 1];
                // now copy it in again
                output[outputIndex++] = input[n];
                output[outputIndex++] = input[n + 1];
            }
            return output;
        }

        public byte[] StereoToMono(byte[] input)
        {
            byte[] output = new byte[input.Length / 2];
            int outputIndex = 0;
            for (int n = 0; n < input.Length; n += 4)
            {
                // copy in the first 16 bit sample
                output[outputIndex++] = input[n];
                output[outputIndex++] = input[n + 1];
            }
            return output;
        }

        public byte[] MixStereoToMono(byte[] input)
        {
            byte[] output = new byte[input.Length / 2];
            int outputIndex = 0;
            for (int n = 0; n < input.Length; n += 4)
            {
                int leftChannel = BitConverter.ToInt16(input, n);
                int rightChannel = BitConverter.ToInt16(input, n + 2);
                int mixed = (leftChannel + rightChannel) / 2;
                byte[] outSample = BitConverter.GetBytes((short)mixed);

                // copy in the first 16 bit sample
                output[outputIndex++] = outSample[0];
                output[outputIndex++] = outSample[1];
            }
            return output;
        }
    }
}

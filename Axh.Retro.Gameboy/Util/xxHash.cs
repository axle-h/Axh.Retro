using System;

namespace Axh.Retro.GameBoy.Util
{
    public class XxHash
    {
        private const uint Prime321 = 2654435761U;
        private const uint Prime322 = 2246822519U;
        private const uint Prime323 = 3266489917U;
        private const uint Prime324 = 668265263U;
        private const uint Prime325 = 374761393U;
        private readonly byte[] memory;
        private readonly uint seed;
        private int memsize;

        private ulong totalLen;
        private uint v1;
        private uint v2;
        private uint v3;
        private uint v4;

        public XxHash(uint seed)
        {
            this.seed = seed;
            memory = new byte[16];
            Init();
        }

        public void Reset()
        {
            for (var i = 0; i < memory.Length; i++)
            {
                memory[i] = 0;
            }
            Init();
        }

        private void Init()
        {
            v1 = seed + Prime321 + Prime322;
            v2 = seed + Prime322;
            v3 = seed + 0;
            v4 = seed - Prime321;
            totalLen = 0;
            memsize = 0;
        }

        public bool Update(byte[] input, int len)
        {
            var index = 0;

            totalLen += (uint) len;

            if (memsize + len < 16)
            {
                Array.Copy(input, 0, memory, memsize, len);
                memsize += len;

                return true;
            }

            if (memsize > 0)
            {
                Array.Copy(input, 0, memory, memsize, 16 - memsize);

                v1 = CalcSubHash(v1, memory, index);
                index += 4;
                v2 = CalcSubHash(v2, memory, index);
                index += 4;
                v3 = CalcSubHash(v3, memory, index);
                index += 4;
                v4 = CalcSubHash(v4, memory, index);

                index = 0;
                memsize = 0;
            }

            if (index <= len - 16)
            {
                var limit = len - 16;
                var v1 = this.v1;
                var v2 = this.v2;
                var v3 = this.v3;
                var v4 = this.v4;

                do
                {
                    v1 = CalcSubHash(v1, input, index);
                    index += 4;
                    v2 = CalcSubHash(v2, input, index);
                    index += 4;
                    v3 = CalcSubHash(v3, input, index);
                    index += 4;
                    v4 = CalcSubHash(v4, input, index);
                    index += 4;
                } while (index <= limit);

                this.v1 = v1;
                this.v2 = v2;
                this.v3 = v3;
                this.v4 = v4;
            }

            if (index < len)
            {
                Array.Copy(input, index, memory, 0, len - index);
                memsize = len - index;
            }
            return true;
        }

        public uint Digest()
        {
            uint h32;
            var index = 0;
            if (totalLen >= 16)
            {
                h32 = RotateLeft(v1, 1) + RotateLeft(v2, 7) + RotateLeft(v3, 12) + RotateLeft(v4, 18);
            }
            else
            {
                h32 = seed + Prime325;
            }

            h32 += (uint) totalLen;

            while (index <= memsize - 4)
            {
                h32 += BitConverter.ToUInt32(memory, index) * Prime323;
                h32 = RotateLeft(h32, 17) * Prime324;
                index += 4;
            }

            while (index < memsize)
            {
                h32 += memory[index] * Prime325;
                h32 = RotateLeft(h32, 11) * Prime321;
                index++;
            }

            h32 ^= h32 >> 15;
            h32 *= Prime322;
            h32 ^= h32 >> 13;
            h32 *= Prime323;
            h32 ^= h32 >> 16;

            return h32;
        }

        private static uint CalcSubHash(uint value, byte[] buf, int index)
        {
            var readValue = BitConverter.ToUInt32(buf, index);
            value += readValue * Prime322;
            value = RotateLeft(value, 13);
            value *= Prime321;
            return value;
        }

        private static uint RotateLeft(uint value, int count)
        {
            return (value << count) | (value >> (32 - count));
        }
    }
}
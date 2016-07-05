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
        private readonly byte[] _memory;
        private readonly uint _seed;
        private int _memsize;

        private ulong _totalLen;
        private uint _v1;
        private uint _v2;
        private uint _v3;
        private uint _v4;

        public XxHash(uint seed)
        {
            _seed = seed;
            _memory = new byte[16];
            Init();
        }

        public void Reset()
        {
            for (var i = 0; i < _memory.Length; i++)
            {
                _memory[i] = 0;
            }
            Init();
        }

        private void Init()
        {
            _v1 = _seed + Prime321 + Prime322;
            _v2 = _seed + Prime322;
            _v3 = _seed + 0;
            _v4 = _seed - Prime321;
            _totalLen = 0;
            _memsize = 0;
        }

        public bool Update(byte[] input, int len)
        {
            var index = 0;

            _totalLen += (uint) len;

            if (_memsize + len < 16)
            {
                Array.Copy(input, 0, _memory, _memsize, len);
                _memsize += len;

                return true;
            }

            if (_memsize > 0)
            {
                Array.Copy(input, 0, _memory, _memsize, 16 - _memsize);

                _v1 = CalcSubHash(_v1, _memory, index);
                index += 4;
                _v2 = CalcSubHash(_v2, _memory, index);
                index += 4;
                _v3 = CalcSubHash(_v3, _memory, index);
                index += 4;
                _v4 = CalcSubHash(_v4, _memory, index);

                index = 0;
                _memsize = 0;
            }

            if (index <= len - 16)
            {
                var limit = len - 16;
                var v1 = _v1;
                var v2 = _v2;
                var v3 = _v3;
                var v4 = _v4;

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

                _v1 = v1;
                _v2 = v2;
                _v3 = v3;
                _v4 = v4;
            }

            if (index < len)
            {
                Array.Copy(input, index, _memory, 0, len - index);
                _memsize = len - index;
            }
            return true;
        }

        public uint Digest()
        {
            uint h32;
            var index = 0;
            if (_totalLen >= 16)
            {
                h32 = RotateLeft(_v1, 1) + RotateLeft(_v2, 7) + RotateLeft(_v3, 12) + RotateLeft(_v4, 18);
            }
            else
            {
                h32 = _seed + Prime325;
            }

            h32 += (uint) _totalLen;

            while (index <= _memsize - 4)
            {
                h32 += BitConverter.ToUInt32(_memory, index) * Prime323;
                h32 = RotateLeft(h32, 17) * Prime324;
                index += 4;
            }

            while (index < _memsize)
            {
                h32 += _memory[index] * Prime325;
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
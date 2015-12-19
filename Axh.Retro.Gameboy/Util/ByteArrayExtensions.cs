namespace Axh.Retro.GameBoy.Util
{
    using System;

    internal static class ByteArrayExtensions
    {
        public static bool SequenceEquals(this byte[] bytes0, byte[] bytes1)
        {
            if (bytes0 == null)
            {
                throw new ArgumentException(nameof(bytes0));
            }

            if (bytes0.Length != bytes1?.Length)
            {
                return false;
            }

            for (var i = 0; i < bytes0.Length; i++)
            {
                if (bytes0[i] != bytes1[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}

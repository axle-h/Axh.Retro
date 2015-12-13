namespace Axh.Retro.GameBoy.BlarggTests
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    using Axh.Retro.GameBoy.Contracts.Devices;

    public class BlarggTestSerialPort : ISerialPort
    {
        private readonly IList<char> currentWord;

        private readonly BlockingCollection<string> wordQueue;

        public BlarggTestSerialPort()
        {
            this.currentWord = new List<char>();
            wordQueue = new BlockingCollection<string>();
            Words = new List<string>();
        }

        public void Connect(ISerialPort serialPort)
        {
        }

        public void Disconnect()
        {
        }

        public byte Transfer(byte value)
        {
            var c = (char)value;
            Console.Write(c);

            if (!char.IsWhiteSpace(c))
            {
                currentWord.Add(c);
                return 0x00;
            }

            if (!currentWord.Any())
            {
                return 0x00;
            }

            var word = new string(currentWord.ToArray());
            wordQueue.Add(word);
            currentWord.Clear();
            return 0x00;
        }

        public IList<string> Words { get; }

        public bool WaitForNextWord()
        {
            string word;
            var result = wordQueue.TryTake(out word);
            if (result)
            {
                Words.Add(word);
            }
            
            return result;
        }
    }
}

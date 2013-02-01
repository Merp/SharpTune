using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace NateW.Ssm
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes")]
    public struct EcuImageRange
    {
        private int start;
        private int length;
        private byte[] data;

        public int Start
        {
            get { return this.start; }
            set { this.start = value; }
        }

        public int Length
        {
            get { return this.length; }
            set { this.length = value; }
        }

        public EcuImageRange(int start, int length)
        {
            this.start = start;
            this.length = length;
            this.data = new byte[length];
        }

        public EcuImageRange(int start, int length, byte[] data)
        {
            this.start = start;
            this.length = length;
            this.data = data;

            if (length != data.Length)
            {
                throw new InvalidOperationException("length parameter != data.Length");
            }
        }

        public bool TryGetValue(int address, out byte value)
        {
            value = 0;
            if (address < this.start)
            {
                return false;
            }

            if (address > this.start + this.length)
            {
                return false;
            }

            value = data[address - this.start];
            return true;
        }

        public void Write(TextWriter writer)
        {
            writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "        this.ranges.Add(new EcuImageRange(0x{0:X8}, 0x{1:X8}, new byte[]", this.start, this.length));
            writer.WriteLine("            {");

            StringBuilder builder = new StringBuilder(100);            
            for (int i = 0; i < length; i++)
            {
                if (i % 16 == 0)
                {
                    builder.AppendFormat("/* {0} */ ", i.ToString("X08", CultureInfo.InvariantCulture));
                }

                builder.Append("0x");
                builder.Append(this.data[i].ToString("X2", CultureInfo.InvariantCulture));
                builder.Append(',');

                if (i % 16 == 15)
                {
                    writer.WriteLine(builder.ToString());
                    builder = new StringBuilder(100);
                }
            }
            writer.WriteLine(builder.ToString());
            writer.WriteLine("}));");
            writer.WriteLine();
        }

        public void Read(SsmInterface ecu)
        {
            const int blockSize = 200;
            int reads = this.length / blockSize + 1;
            for (int i = 0; i < reads; i++)
            {
                int blockStart = i * blockSize;
                int thisBlockSize = Math.Min(blockSize, (this.length - blockStart));
                if (thisBlockSize < 0)
                {
                    break;
                }

                //IAsyncResult result = ecu.BeginBlockRead(blockStart, blockSize, null, null);
                //result.AsyncWaitHandle.WaitOne();
                //byte[] values = ecu.EndBlockRead(result);
                byte[] values = ecu.SyncReadBlock(blockStart, blockSize);

                if (values.Length != thisBlockSize)
                {
                    Console.Write("Failed at read {0} of {1}, ", i, reads);
                    Console.WriteLine("Values.Length={0}, thisBlockSize={1}", values.Length, thisBlockSize);
                    System.Diagnostics.Debugger.Break();
                }

                for (int j = 0; j < thisBlockSize; j++)
                {
                    int index = blockStart + j;
                    this.data[index] = values[j];
                }

                if (i % 10 == 0)
                {
                    Console.Write("Read {0} of {1}, ", i, reads);
                    Console.WriteLine((i * 100 / reads).ToString(CultureInfo.InvariantCulture) + "%");
                }
            }
        }

        public void ReadSlow(SsmInterface ecu)
        {
            List<int> addresses = new List<int>();

            int reads = ((this.length) / 16);

            for (int i = 0; i < reads; i++)
            {
                for (int offset = 16 * i; (offset < (16 * (i + 1)) && (offset < length)); offset++)
                {
                    addresses.Add(this.start + offset);
                }

                IAsyncResult result = ecu.BeginMultipleRead(addresses, null, null);
                result.AsyncWaitHandle.WaitOne();
                byte[] values = ecu.EndMultipleRead(result);
                addresses.Clear();
                for (int j = 0; j < 16; j++)
                {
                    this.data[(16 * i) + j] = values[j];
                }

                if (i % 200 == 0)
                {
                    Console.Write(".");
                }
            }
        }
    }
}

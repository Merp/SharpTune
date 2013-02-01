using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EcuHack
{
    class PatchReader
    {
        private string path;
        private StreamReader reader;

        public PatchReader(string path)
        {
            this.path = path;
        }

        public void Open()
        {
            this.reader = new StreamReader(path, Encoding.ASCII);
        }

        public bool TryGetRecordDescription(out string description)
        {
            string record = this.reader.ReadLine();
            if (string.IsNullOrEmpty(record))
            {
                description = string.Empty;
                return false;
            }

            char s = record[0];
            if (s != 'S')
            {
                description = "Record should begin with S, but actually begins with " + s;
                return true;
            }

            char typeCode = record[1];
            int addressBytes = 0;
            bool header = false;
            bool data = false;
            bool totalCount = false;
            bool startAddress = false;

            switch (typeCode)
            {
                case '0':
                    addressBytes = 2;
                    header = true;
                    break;

                case '1':
                    addressBytes = 2;
                    data = true;
                    break;

                case '2':
                    addressBytes = 3;
                    data = true;
                    break;

                case '3':
                    addressBytes = 4;
                    data = true;
                    break;

                case '4':
                    description = "This is an S4 record.  Not sure how to interpret that.";
                    return true;

                case '5':
                    addressBytes = 2;
                    totalCount = true;
                    break;

                case '6':
                    description = "This is an S6 record.  Not sure how to interpret that.";
                    return true;

                case '7':
                    addressBytes = 4;
                    startAddress = true;
                    break;

                case '8':
                    addressBytes = 3;
                    startAddress = true;
                    break;

                case '9':
                    addressBytes = 2;
                    startAddress = true;
                    break;

                default:
                    description = "I have no idea what to do with a record of type " + typeCode;
                    return true;
            }

            int index = 2;
            int count = GetByte(record, ref index);
            count -= addressBytes;

            // We'll ignore the checksum byte.
            count -= 1;
           
            int address = GetByte(record, ref index);
            address <<= 8;
            address += GetByte(record, ref index);

            if (addressBytes >= 3)
            {
                address <<= 8;
                address += GetByte(record, ref index);
            }

            if (addressBytes == 4)
            {
                address <<= 8;
                address += GetByte(record, ref index);
            }            

            if (startAddress)
            {
                description = "Execution start address: " + address.ToString("X");
                return true;
            }

            if (totalCount)
            {
                description = "Total record count thus far should be: " + address;
                return true;
            }

            List<byte> bytes = new List<byte>();
            StringBuilder headerBuilder = new StringBuilder();
            for (int payloadIndex = 0; payloadIndex < count; payloadIndex++)
            {
                byte temp = GetByte(record, ref index);
                bytes.Add(temp);
            }

            byte checksum = GetByte(record, ref index);
            //VerifyChecksum(bytes, checksum);

            if (header)
            {
                byte[] array = bytes.ToArray();
                string headerText = ASCIIEncoding.ASCII.GetString(array);
                description = "Header: " + headerText;
                return true;
            }
            
            if (data)
            {
                List<string> byteStrings = new List<string>();
                foreach (byte b in bytes)
                {
                    byteStrings.Add(b.ToString("X2"));
                }

                description = "Data start address: " + address.ToString("X") + ", payload: " + string.Join(",", byteStrings.ToArray());
                return true;
            }

            description = "Wait, what's a record of type " + typeCode + " for?";
            return true;
        }

        private static byte GetByte(string record, ref int index)
        {
            if (record.Length < index + 1)
            {
                Console.WriteLine("index out of bounds");
                return 0;
            }

            char c1 = record[index];
            index++;
            char c2 = record[index];
            index++;

            string s = string.Format("{0}{1}", c1, c2);
            byte b = byte.Parse(s, System.Globalization.NumberStyles.HexNumber);
            return b;
        }
    }
}

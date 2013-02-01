///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// MockEcuStream.cs
///////////////////////////////////////////////////////////////////////////////
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using NateW.Ssm;

namespace NateW.Ssm
{
    /// <summary>
    /// Simulates an ECU, to allow testing withing sitting in a car
    /// </summary>
    public class MockEcuStream : Stream
    {
        public const string PortName = "Mock ECU";

        private const int defoggerAddress = 0x64;
        private const int defoggerBit = 5;

        /// <summary>
        /// Default ECU image
        /// </summary>
        private static EcuImage image = new EcuImageRandom();
        
        /// <summary>
        /// Last instance created (to ease testing with SsmLogger, which does not expose the stream)
        /// </summary>
        private static MockEcuStream lastInstance;
                
        /// <summary>
        /// TODO: unify with SsmPacket.HeaderLength
        /// </summary>
        private const int payloadOffset = 4;

        /// <summary>
        /// Memory (think of it as a sparse array)
        /// </summary>
        private Dictionary<int, byte> memory;

        /// <summary>
        /// Bytes to provide to the caller in subsequent read operations
        /// </summary>
        private byte[] response;

        /// <summary>
        /// Current position in the response array
        /// </summary>
        private int responseIndex;

        /// <summary>
        /// If true, the next response will be corrupted
        /// </summary>
        private bool corruptNextResponse;

        public static EcuImage Image
        {
            get { return image; }
            set { image = value; }
        }

        public override bool CanRead
        {
            [DebuggerStepThrough()]
            get { return true; }
        }

        public override bool CanSeek
        {
            [DebuggerStepThrough()]
            get { return false; }
        }

        public override bool CanTimeout
        {
            [DebuggerStepThrough()]
            get { return false; }
        }

        public override bool CanWrite
        {
            [DebuggerStepThrough()]
            get { return true; }
        }

        public override long Length
        {
            [DebuggerStepThrough()]
            get { return 0; }
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        public override long Position
        {
            get
            {
                throw new InvalidOperationException();
            }

            set
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Determines what the read response will be for the defogger switch address
        /// </summary>
        public bool DefoggerSwitch
        {
            get
            {
                return (this.memory[defoggerAddress] & (1 << defoggerBit)) > 0;
            }

            set
            {
                int temp = memory[defoggerAddress];

                if (value)
                {
                    temp |= (1 << defoggerBit);
                }
                else
                {
                    temp &= ~(1 << defoggerBit);
                }

                this.memory[defoggerAddress] = (byte) temp;
            }
        }

        /// <summary>
        /// Private constuctor - use factory instead
        /// </summary>
        private MockEcuStream()
        {
            this.InitializeMemory();
        }

        /// <summary>
        /// Factory
        /// </summary>
        public static MockEcuStream CreateInstance()
        {
            MockEcuStream instance = new MockEcuStream();
            lastInstance = instance;
            return instance;
        }

        /// <summary>
        /// Force the last-created-instance to corrupt its next response.
        /// </summary>
        public static void CorruptResponse()
        {
            lastInstance.InternalCorruptResponse();
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Read a response from the mock ECU
        /// </summary>
        public override int Read(byte[] buffer, int offset, int count)
        {
            System.Threading.Thread.Sleep(1);

            int i = 0;
            for (; i < count && i + this.responseIndex < this.response.Length; i++)
            {
                buffer[offset + i] = this.response[this.responseIndex + i];
            }

            this.responseIndex += i;
            return i;
        }

        /// <summary>
        /// Write a request to the mock ECU
        /// </summary>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (buffer[0] != 0x80)
            {
                throw new InvalidOperationException("First byte sent to ECU must be 0x80 (command start byte)");
            }
            /*
            if (buffer[1] != 0x10)
            {
                throw new InvalidOperationException("Second byte sent to ECU must be 0x10 (destination = ECU)");
            }

            if (buffer[2] != 0xF0)
            {
                throw new InvalidOperationException("Third byte sent to ECU must be 0xF0 (source = diagnostic tool)");
            }
            */
            if (MockEcuStream.GetPacketLength(buffer) != buffer.Length - 5)
            {
                throw new InvalidOperationException("Fourth byte sent to ECU must be equal to \"buffer_length - 5\"");
            }

            if (this.corruptNextResponse)
            {
                byte[] data = new byte[100];
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = (byte) 0x55;
                }

                this.SetResponse(data);
                this.corruptNextResponse = false;
                return;
            }

            SsmPacket packet = SsmPacket.ParseRequest(buffer, offset, count);

            switch (packet.Command)
            {
                case SsmCommand.ReadBlockRequest:
                    this.ReadBlock(packet.BlockStart, packet.BlockLength);
                    break;

                case SsmCommand.ReadAddressesRequest:
                    this.ReadAddresses(packet.Addresses);
                    break;

                case SsmCommand.WriteBlockRequest:
                    //this.WriteBlock(packet.BlockStart, packet.BlockData);
                    break;

                case SsmCommand.WriteAddressesRequest:
                    //this.WriteAddresses(packet.Addresses, packet.Values);
                    break;

                case SsmCommand.EcuInitRequest:
                    this.Handshake();
                    break;
            }

            List<byte> temporaryBuffer = new List<byte>(packet.Data.Length + this.response.Length);
            temporaryBuffer.AddRange(packet.Data);
            temporaryBuffer.AddRange(this.response);
            this.SetResponse(temporaryBuffer.ToArray());
        }

        /// <summary>
        /// No-op
        /// </summary>
        public override void Flush()
        {
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Corrupt the next response.
        /// </summary>
        private void InternalCorruptResponse()
        {
            this.corruptNextResponse = true;
        }

        /// <summary>
        /// Initialize a few locations in memory
        /// </summary>
        private void InitializeMemory()
        {
            this.memory = new Dictionary<int, byte>();

            // Engine Speed
            this.memory[0x0E] = 0x03;
            this.memory[0x0F] = 0xE8;

            // IPW (10ms)
            this.memory[0x20] = 39;
            this.memory[0x21] = 39;

            // Atmospheric pressure (14.7)
            this.memory[0x23] = 101;

            // Manifold absolute pressure (15.7)
            this.memory[0x0d] = 108;

            // Accelerator pedal (50%)
            this.memory[0x29] = 127;

            // Bit 5 is the defogger switch
            this.memory[defoggerAddress] = 0;
        }


        /// <summary>
        /// Gets the length byte from an SSM packet
        /// </summary>
        private static int GetPacketLength(byte[] buffer)
        {
            return buffer[(int)SsmPacketIndex.DataSize];
        }

        /// <summary>
        /// SSM's handshake operation
        /// </summary>
        /// <param name="buffer"></param>
        private void Handshake()
        {
            SsmPacket packet = SsmPacket.CreateEcuIdentifierResponse();
            this.SetResponse(packet.Data);
        }

        /// <summary>
        /// SSM's read-block operation
        /// </summary>
        private void ReadBlock(int blockStart, byte blockLength)
        {
            byte[] payload = new byte[blockLength];

            for (int offset = 0; offset < blockLength; offset++)
            {
                int address = blockStart + offset;
                byte value = this.GetValue(address);
                payload[offset] = value;
            }

            SsmPacket responsePacket = SsmPacket.CreateBlockReadResponse(payload);
            this.SetResponse(responsePacket.Data);
        }

        /// <summary>
        /// SSM's read-multiple-addresses operation
        /// </summary>
        /// <param name="addresses"></param>
        private void ReadAddresses(IList<int> addresses)
        {
            byte[] payload = new byte[addresses.Count];
            
            for (int i = 0; i < addresses.Count; i++)
            {
                int address = addresses[i];
                payload[i] = this.GetValue(address);
            }

            SsmPacket responsePacket = SsmPacket.CreateMultipleReadResponse(payload);
            this.SetResponse(responsePacket.Data);
        }

        private byte GetValue(int address)
        {
            if (this.memory.ContainsKey(address))
            {
                return this.memory[address];
            }

            return image.GetValue(address);
        }
        /*
                /// <summary>
                /// SSM's write-block operation
                /// </summary>
                private void WriteBlock(int startAddress, byte[] payload)
                {
                    for (int index = 0; index < payload.Length; index++)
                    {
                        int address = startAddress + index;
                        byte value = payload[index];
                        this.memory[address] = value;
                    }
                    throw new NotImplementedException("Implemented, but not tested.");
                }

                /// <summary>
                /// SSM's write-multiple-addresses operation
                /// </summary>
                private void WriteAddresses(List<int> addresses, List<byte> values)
                {
                    for (int index = 0; index < addresses.Length; index++)
                    {
                        int address = addresses[index];
                        byte value = values[index];
                        this.memory[address] = value;
                    }
                    throw new NotImplementedException("Implemented, but not tested.");
                }
        */

/*
        /// <summary>
        /// Extracts a 24-bit address from an SSM packet
        /// </summary>
        /// <param name="buffer">buffer containing the entire packet</param>
        /// <param name="offset">offset into the buffer</param>
        /// <returns>the address</returns>
        private static int GetAddress(byte[] buffer, int offset)
        {
            int result = buffer[offset++];
            result <<= 8;
            result |= buffer[offset++];
            result <<= 8;
            result |= buffer[offset];
            return result;
        }

        /// <summary>
        /// Stamps the SSM packet header and checksum for the given buffer
        /// </summary>
        private void PrepareResponse(byte[] buffer)
        {
            buffer[0] = 0x80;
            buffer[1] = 0x10;
            buffer[2] = 0xF0;
            buffer[3] = (byte)(buffer.Length - 4);
            byte checksum = this.ComputeChecksum(buffer);
            buffer[buffer.Length-1] = checksum;
        }

        /// <summary>
        /// Computes the SSM checksum for the given buffer
        /// </summary>
        private static byte ComputeChecksum(byte[] buffer)
        {
            int result = 0;
            for (int i = 0; i < buffer.Length - 1; i++)
            {
                result += buffer[i];
            }
            return (byte)result;
        }
*/
        /// <summary>
        /// Sets the given buffer as the response to use for subsequent read operations on the stream
        /// </summary>
        private void SetResponse(byte[] buffer)
        {
            this.response = buffer;
            this.responseIndex = 0;
        }
    }
}

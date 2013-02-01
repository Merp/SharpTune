///////////////////////////////////////////////////////////////////////////////
// Original Copyright (c) Nate Waddoups
// Modified by Merrill Myers February 2012
// Distributed under GPL v2.0 License
// SsmPacket.cs
///////////////////////////////////////////////////////////////////////////////
using System;
using System.IO;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace NateW.Ssm
{
    #region enums
    /// <summary>
    /// SSM packet direction
    /// </summary>
    public enum SsmDirection
    {
        Invalid,
        ToEcu, 
        FromEcu
    };

    /// <summary>
    /// SSM command identifier
    /// </summary>
    public enum SsmCommand
    {
        None = 0,
        ReadBlockRequest = 0xA0,
        ReadBlockResponse = 0xE0,
        ReadAddressesRequest = 0xA8,
        ReadAddressesResponse =0xE8,
        WriteBlockRequest = 0xB0,
        WriteBlockResponse = 0xF0,
        WriteAddressesRequest = 0xB8,
        WriteAddressesResponse = 0xF8,
        EcuInitRequest = 0xBF,
        EcuInitResponse = 0xFF,
    }

    /// <summary>
    /// SSM device type
    /// </summary>
    public enum SsmDeviceType
    {
        Unknown = 0,
        SubaruEcu = 0x10,
        DiagnosticTool = 0xF0
    }

    /// <summary>
    /// Well-known indices into an SSM packet
    /// </summary>
    public enum SsmPacketIndex : int
    {
        Header,
        Destination,
        Source,
        DataSize,
        Command
    }

    #endregion enums

    /// <summary>
    /// Exception thrown when packet parsing fails due to corrupt data
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors")]
    [Serializable]
    public class SsmPacketFormatException : IOException
    {
        public SsmPacketFormatException(string message)
            :
            base(message)
        {
        }
    }

    /// <summary>
    /// Builds and interprets SSM packets
    /// </summary>
    /// <remarks>
    /// Objects of this type are immutable.  (At least conceptually. There can
    /// be some optimisations that violate that rule.)
    /// Consider creating subclasses for each packet type.
    /// </remarks>
    public class SsmPacket
    {
        #region private variables

        /// <summary>
        /// Value of the first byte of all SSM packets
        /// </summary>
        internal const byte FirstByte = 0x80;

        /// <summary>
        /// Number of bytes in the header of all SSM packets
        /// </summary>
        internal const int HeaderLength = 5;

        /// <summary>
        /// Raw data bytes
        /// </summary>
        private List<byte> data = new List<byte>(20);

        /// <summary>
        /// Valid for multiple-addread read and write request packets only: addresses to read from / write to
        /// </summary>
        private IList<int> addresses;

        /// <summary>
        /// Valid for multiple-address read and write response packets only: values read from / written to the ECU
        /// </summary>
        private IList<byte> values;

        /// <summary>
        /// Packet data in byte[] form
        /// </summary>
        private byte[] allData;

        /// <summary>
        /// Valid for ECU-identifer response packest only
        /// </summary>
        private string ecuIdentifier;

        /// <summary>
        /// Valid for ECU-identifer response packest only
        /// </summary>
        private byte[] compatibilityMap;

#endregion private variables

        #region public variables

        /// <summary>
        /// Raw packet data
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        public byte[] Data
        {
            get
            {
                if (this.allData == null)
                {
                    this.allData = this.data.ToArray();
                }
                return this.allData;
            }
        }

        /// <summary>
        /// Which direction this packet is going
        /// </summary>
        public SsmDirection Direction
        {
            get
            {
                this.AssertHeaderPresent();
                if ((this.data[(int)SsmPacketIndex.Destination] == (byte)SsmDeviceType.SubaruEcu) &&
                    (this.data[(int)SsmPacketIndex.Source] == (byte)SsmDeviceType.DiagnosticTool))
                {
                    return SsmDirection.ToEcu;
                }

                if ((this.data[(int)SsmPacketIndex.Destination] == (byte)SsmDeviceType.DiagnosticTool) &&
                    (this.data[(int)SsmPacketIndex.Source] == (byte)SsmDeviceType.SubaruEcu))
                {
                    return SsmDirection.FromEcu;
                }

                return SsmDirection.Invalid;
            }
        }

        /// <summary>
        /// Length of the payload of this packet
        /// </summary>
        public int PayloadLength
        {
            get
            {
                this.AssertHeaderPresent();
                return this.data[(int)SsmPacketIndex.DataSize];
            }
        }

        /// <summary>
        /// Packet command ID
        /// </summary>
        public SsmCommand Command
        {
            get
            {
                this.AssertHeaderPresent();
                return (SsmCommand) this.data[(int)SsmPacketIndex.Command];
            }
        }

        /// <summary>
        /// Valid for block-read and block-write requests only
        /// </summary>
        public int BlockStart
        {
            get
            {
                this.AssertHeaderPresent();
                
                if (this.Command != SsmCommand.ReadBlockRequest)
                {
                    throw new InvalidOperationException("SsmPacket.BlockStart is only valid for block read requests");
                }
                
                if (this.data.Count < 10)
                {
                    throw new InvalidOperationException("SsmPacket.BlockStart: Packet data is too small to be a block read or write");
                }

                //MAM: This process creates int result by picking off bytes from the data array using left shift and OR operators.
                int result = this.data[6];
                result <<= 8;
                result |= this.data[7];
                result <<= 8;
                result |= this.data[8];
                return result;
            }
        }

        /// <summary>
        /// Valid for block-read and block-write requests only
        /// </summary>
        public byte BlockLength
        {
            get
            {
                this.AssertHeaderPresent();
                
                if (this.Command != SsmCommand.ReadBlockRequest)
                {
                    throw new InvalidOperationException("SsmPacket.BlockLength is only valid for block read requests");
                }

                if (this.data.Count <= 10)
                {
                    throw new InvalidOperationException("SsmPacket.BlockLength: Packet data is too small to be a block read or write");
                }
                return (byte) (this.data[9] + 1);
            }
        }

        /// <summary>
        /// Valid for multiple-address-read and -write requests only
        /// </summary>
        public IList<int> Addresses
        {
            get
            {
                if (this.Command != SsmCommand.ReadAddressesRequest)
                {
                    throw new InvalidOperationException("SsmPacket.Addresses is only valid for read-addresses request");
                }

                if (this.addresses == null)
                {
                    this.addresses = new List<int>();
                    int headerLength = 6;
                    int count = (this.data[3] - 2) / 3;
                    
                    if (count > 100)
                    {
                        throw new InvalidOperationException("SsmPacket.Addresses: packet data is corrupt");
                    }

                    int offset = headerLength;
                    for (int addressIndex = 0; addressIndex < count; addressIndex++)
                    {
                        int address = data[offset++];
                        address <<= 8;
                        address |= data[offset++];
                        address <<= 8;
                        address |= data[offset++];
                        this.addresses.Add(address);
                    }
                }
                return this.addresses;
            }
        }

        /// <summary>
        /// Valid for multiple-address-read and -write responses only
        /// </summary>
        public IList<byte> Values
        {
            get
            {
                if ((this.Command != SsmCommand.ReadAddressesResponse) && (this.Command != SsmCommand.ReadBlockResponse))
                {
                    Console.WriteLine("Packet command is {0}/(1)", (int)this.Command, this.Command);
                    //throw new InvalidOperationException("SsmPacket.Addresses is only valid for read-addresses and read-block responses");
                }

                if (this.values == null)
                {
                    this.values = new List<byte>();
                    int headerLength = 5;
                    int count = this.data[3] - 1;
                    int offset = headerLength;
                    for (int valueIndex = 0; valueIndex < count; valueIndex++)
                    {
                        byte value = data[offset++];
                        this.values.Add(value);
                    }
                }
                return this.values;
            }
        }

        /// <summary>
        /// Valid for ECU-identifier responses only
        /// </summary>
        public string EcuIdentifier
        {
            get
            {
                if (this.Command != SsmCommand.EcuInitResponse)
                {
                    throw new InvalidOperationException("SsmPacket.EcuIdentifier is only valid for ecu-init response");
                }
                
                if (this.ecuIdentifier == null)
                {
                    StringBuilder builder = new StringBuilder(10);
                    int start = 8;
                    int length = 5;
                    for (int i = start; i < start+length; i++)
                    {
                        string byteString = this.data[i].ToString("X02", CultureInfo.InvariantCulture);
                        builder.Append(byteString);
                    }
                    this.ecuIdentifier = builder.ToString();
                }

                return this.ecuIdentifier;
            }
        }

        public IList<byte> CompatibilityMap
        {
            get
            {
                if (this.Command != SsmCommand.EcuInitResponse)
                {
                    throw new InvalidOperationException("SsmPacket.EcuIdentifier is only valid for ecu-init response");
                }

                if (this.compatibilityMap == null)
                {
                    int start = 13;
                    int length = (this.data.Count - start);
                    this.compatibilityMap = new byte[length];

                    for (int i = 0; i < length; i++)
                    {
                        this.compatibilityMap[i] = this.data[i + start];
                    }
                }

                return this.compatibilityMap;
            }
        }

#endregion public variables

        #region constructors

        /// <summary>
        /// Private constructor - use factory instead
        /// </summary>
        private SsmPacket()
        {
        }

        /// <summary>
        /// Construct a packet from the given (subset of a) byte array
        /// </summary>
        private SsmPacket(byte[] source, int offset, int length)
        {
            this.data = new List<byte>(length);
            for (int i = 0; i < length; i++)
            {
                byte b = source[offset + i];
                this.data.Add(b);
            }
        }

#endregion constructors

        #region public static methods
        /// <summary>
        /// Parse a request packet from the given (subset of a) byte array
        /// </summary>
        public static SsmPacket ParseRequest(byte[] data, int offset, int length)
        {
            SsmPacket request = new SsmPacket(data, offset, length);
            return request;
        }

        /// <summary>
        /// Parse a response packet (which begins with an echo of the request
        /// that elicited the response) from the given (subset of a) byte array.
        /// </summary>
        public static SsmPacket ParseResponse(byte[] data, int offset, int length)
        {
            SsmPacket request = new SsmPacket(data, offset, length);
            int requestLength = SsmPacket.HeaderLength + request.PayloadLength;
            SsmPacket response = new SsmPacket(data, offset + requestLength, length - requestLength);
            return response;
        }
                      
        /// <summary>
        /// Create an ECU request packet with an arbitrary command and payload.
        /// </summary>
        /// <remarks>
        /// For experimentation...
        /// </remarks>
        /// <param name="direction">Which direction the packet will be traveling.</param>
        /// <param name="command">Command byte.</param>
        /// <param name="payload">Payload.</param>
        /// <returns>SsmPacket built from the given parameters.</returns>
        public static SsmPacket CreateArbitrary(
            SsmDirection direction,
            SsmCommand command,
            byte[] payload)
        {
            SsmPacket packet = new SsmPacket();
            packet.SetHeader(direction, command);
            packet.AppendPayload(payload);
            packet.SetLengthByte();
            packet.AppendChecksum();
            return packet;
        }

        /// <summary>
        /// Create a request packet for an arbitrary device with an arbitrary command and payload.
        /// </summary>
        /// <remarks>
        /// For experimentation...
        /// </remarks>
        /// <param name="device">Which device the packet will to go.</param>
        /// <param name="command">Command byte.</param>
        /// <param name="pad">If true, an extra byte will be added to the header.</param>
        /// <param name="payload">Payload.</param>
        /// <returns>SsmPacket built from the given parameters.</returns>
        public static SsmPacket CreateArbitrary(
            byte device,
            byte command,
            bool pad,
            byte[] payload)
        {
            SsmPacket packet = new SsmPacket();
            packet.SetHeader(device, command, pad);
            packet.AppendPayload(payload);
            packet.SetLengthByte();
            packet.AppendChecksum();
            return packet;
        }

        /// <summary>
        /// Create an ECU identifier request packet
        /// </summary>
        public static SsmPacket CreateEcuIdentifierRequest()
        {
            SsmPacket packet = new SsmPacket();
            packet.SetHeader(SsmDirection.ToEcu, SsmCommand.EcuInitRequest);
            packet.SetLengthByte();
            packet.AppendChecksum();
            return packet;
        }

        /// <summary>
        /// Create an ECU identifier response packet
        /// </summary>
        public static SsmPacket CreateEcuIdentifierResponse()
        {
            SsmPacket packet = new SsmPacket();
            packet.SetHeader(SsmDirection.FromEcu, SsmCommand.EcuInitResponse);

            // Remove header and checksum from sample packet data
            List<byte> payload = new List<byte>(SamplePacketData.EcuInitResponse);            
            payload.RemoveRange(0, 5);
            payload.RemoveRange(payload.Count - 1, 1);

            packet.AppendPayload(payload.ToArray());
            packet.SetLengthByte();
            packet.AppendChecksum();
            return packet;
        }

        /// <summary>
        /// Create a block-read request packet
        /// </summary>
        public static SsmPacket CreateBlockReadRequest(int address, int length)
        {
            if (length == int.MinValue)
            {
                throw new ArgumentOutOfRangeException("length");
            }

            SsmPacket packet = new SsmPacket();
            packet.SetHeader(SsmDirection.ToEcu, SsmCommand.ReadBlockRequest);
            packet.AppendAddress(address);
            packet.AppendByte((byte) (length - 1));
            packet.SetLengthByte();
            packet.AppendChecksum();
            return packet;
        }

        /// <summary>
        /// Create a block-read response packet
        /// </summary>
        public static SsmPacket CreateBlockReadResponse(byte[] payload)
        {
            SsmPacket packet = new SsmPacket();
            packet.SetHeader(SsmDirection.FromEcu, SsmCommand.ReadBlockResponse);
            packet.AppendPayload(payload);
            packet.SetLengthByte();
            packet.AppendChecksum();
            return packet;
        }

        #region Block Write

        /// <summary>
        /// Create a block-write request packet
        /// </summary>
        public static SsmPacket CreateBlockWriteRequest(int address, byte[] bytes)
        {
            if (bytes.Length == int.MinValue)
            {
                throw new ArgumentOutOfRangeException("length");
            }

            SsmPacket packet = new SsmPacket();
            packet.SetHeader(SsmDirection.ToEcu, SsmCommand.WriteBlockRequest);
            packet.AppendAddress(address);
            foreach (byte b in bytes)
            {
                packet.AppendByte(b);
            }
            packet.SetLengthByte();
            packet.AppendChecksum();
            return packet;
        }

        /// <summary>
        /// Create a block-read response packet
        /// </summary>
        public static SsmPacket CreateBlockWriteResponse(byte[] payload)
        {
            SsmPacket packet = new SsmPacket();
            packet.SetHeader(SsmDirection.FromEcu, SsmCommand.WriteBlockResponse);
            packet.AppendPayload(payload);
            packet.SetLengthByte();
            packet.AppendChecksum();
            return packet;
        }

        #endregion

        /// <summary>
        /// Create a multiple-address-read request packet
        /// </summary>
        public static SsmPacket CreateMultipleReadRequest(IList<int> addresses)
        {
            SsmPacket packet = new SsmPacket();
            packet.SetHeader(SsmDirection.ToEcu, SsmCommand.ReadAddressesRequest);
            for (int i = 0; i < addresses.Count; i++)
            {
                packet.AppendAddress(addresses[i]);
            }
            packet.SetLengthByte();
            packet.AppendChecksum();
            return packet;
        }

        /// <summary>
        /// Create a multiple-address-read response packet
        /// </summary>
        public static SsmPacket CreateMultipleReadResponse(IList<byte> values)
        {
            SsmPacket packet = new SsmPacket();
            packet.SetHeader(SsmDirection.FromEcu, SsmCommand.ReadAddressesResponse);
            for (int i = 0; i < values.Count; i++)
            {
                packet.AppendByte(values[i]);
            }
            packet.SetLengthByte();
            packet.AppendChecksum();
            return packet;
        }

        /// <summary>
        /// For debugging use only - do not expose in UI
        /// </summary>
        public override string ToString()
        {
            return this.Command + " " + this.Direction;
        }

#endregion public static methods

        #region private methods
        /// <summary>
        /// Set the basic header bytes for a packet
        /// </summary>
        private void SetHeader(SsmDirection direction, SsmCommand command)
        {
            this.data.Add(0x80);
            if (direction == SsmDirection.ToEcu)
            {
                this.data.Add((byte)SsmDeviceType.SubaruEcu);
                this.data.Add((byte)SsmDeviceType.DiagnosticTool);
            }
            else
            {
                this.data.Add((byte)SsmDeviceType.DiagnosticTool);
                this.data.Add((byte)SsmDeviceType.SubaruEcu);
            }

            // placeholder for the length
            this.data.Add(0);
            this.data.Add((byte) command);

            // Padding byte required for read requests
            if (command == SsmCommand.ReadBlockRequest)
            {
                this.data.Add(0x00);
            }
            if (command == SsmCommand.ReadAddressesRequest)
            {
                this.data.Add(0x00);
            }
        }

        /// <summary>
        /// Set up packet header for arbitrary device and command.
        /// </summary>
        /// <param name="device">Device to send to (will always be from DiagnosticTool).</param>
        /// <param name="command">Command byte.</param>
        /// <param name="pad">If true, an extra byte will be added (see ECU read requests).</param>
        private void SetHeader(byte device, byte command, bool pad)
        {
            this.data.Add(0x80);
            this.data.Add((byte)device);
            this.data.Add((byte)SsmDeviceType.DiagnosticTool);
            this.data.Add(0);
            this.data.Add((byte)command);

            if (pad)
            {
                this.data.Add(0);
            }
        }

        /// <summary>
        /// Append an address to a multiple-address-read or -write request packet
        /// </summary>
        private void AppendAddress(int address)
        {
            int temp = address;
            this.data.Add((byte)((temp & 0xFF0000) >> 16));
            this.data.Add((byte)((temp & 0xFF00) >> 8));
            this.data.Add((byte)temp);
        }

        /// <summary>
        /// Append a value to a multiple-address-read or -write response
        /// </summary>
        /// <param name="b"></param>
        private void AppendByte(byte b)
        {
            this.data.Add(b);
        }

        /// <summary>
        /// Append payload to a block-read response
        /// </summary>
        /// <param name="payload"></param>
        private void AppendPayload(byte[] payload)
        {
            for (int i = 0; i < payload.Length; i++)
            {
                this.data.Add(payload[i]);
            }
        }

        /// <summary>
        /// Set the length byte in the packet's header
        /// </summary>
        private void SetLengthByte()
        {
            this.data[3] = (byte) (this.data.Count - 4);
        }

        /// <summary>
        /// Append the checksum byte to the packet
        /// </summary>
        private void AppendChecksum()
        {
            byte checksum = this.ComputeChecksum();
            this.data.Add(checksum);
        }

        /// <summary>
        /// Compute the packet's checksum 
        /// </summary>
        private byte ComputeChecksum()
        {
            int result = 0;
            for (int i = 0; i < this.data.Count; i++)
            {
                result += this.data[i];
            }
            return (byte)result;
        }

        /// <summary>
        /// Throw if the packet's header byte's aren't preset
        /// </summary>
        /// <remarks>
        /// This is used to forestall IndexOutOfRangeExceptions later
        /// </remarks>
        private void AssertHeaderPresent()
        {
            if (this.data == null)
            {
                throw new InvalidOperationException("this.data is null");
            }

            if (this.data.Count < 5)
            {
                throw new SsmPacketFormatException("this.data must be at least 5 bytes");
            }
        }

        #endregion private methods
    }
}

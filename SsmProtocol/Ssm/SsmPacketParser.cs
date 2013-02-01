///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// SsmPacketParser.cs
///////////////////////////////////////////////////////////////////////////////
using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace NateW.Ssm
{
    /// <summary>
    /// Assembles SSM packets from fragments received over a stream
    /// </summary>
    public class SsmPacketParser
    {
        /// <summary>
        /// Packet receive buffer
        /// </summary>
        private byte[] buffer;

        /// <summary>
        /// Number of bytes received so far
        /// </summary>
        private int bytesReceived;

        /// <summary>
        /// If true, the parser has received enough information to know how many bytes it should receive
        /// </summary>
        private bool checkLength;

        /// <summary>
        /// Number of bytes in the request echo that precedes a response packet
        /// </summary>
        private int echoLength;

        /// <summary>
        /// Number of bytes in the response packet itself
        /// </summary>
        private int responseLength;

        /// <summary>
        /// Will become true when a complete packet has been received
        /// </summary>
        private bool IsComplete
        {
            get
            {
                if (this.checkLength)
                {
                    if (this.bytesReceived == this.echoLength + this.responseLength)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        
        /// <summary>
        /// Private constructor - use factory instead
        /// </summary>
        private SsmPacketParser()
        {
            this.buffer = new byte[300];
            this.echoLength = -1;
            this.responseLength = -1;
        }

        /// <summary>
        /// Factory
        /// </summary>
        public static SsmPacketParser CreateInstance()
        {
            return new SsmPacketParser();
        }

        /// <summary>
        /// Synchronously read a complete packet from a possibly-fragmented stream
        /// </summary>
        /// <remarks>
        /// Not very useful in production, but it made development and testing easier
        /// </remarks>
        public static SsmPacket ReadFromStream(Stream stream)
        {
            SsmPacketParser parser = new SsmPacketParser();

            while (!parser.IsComplete)
            {
                if (parser.bytesReceived != 0)
                {
                    System.Threading.Thread.Sleep(1);
                }

                int bufferLength = stream.Read(parser.buffer, parser.bytesReceived, parser.buffer.Length - parser.bytesReceived);
                if (bufferLength == 0)
                {
                    throw new SsmPacketFormatException("Unexpected end of stream.");
                }

                for (int i = 0; i < bufferLength; i++)
                {
                    int index = parser.bytesReceived + i;
                    parser.CheckByte(index, parser.buffer[index]);
                }
                parser.bytesReceived += bufferLength;
            }

            return SsmPacket.ParseResponse(parser.buffer, 0, parser.bytesReceived);
        }

        /// <summary>
        /// Asynchronously read a complete packet from a possibly-fragmented stream
        /// </summary>
        public IAsyncResult BeginReadFromStream(Stream stream, AsyncCallback asyncCallback, object asyncState)
        {
            ReadFromStreamAsyncResult internalState = new ReadFromStreamAsyncResult(
                stream,
                this,
                asyncCallback,
                asyncState);

            this.InternalBeginReadFromStream(
                stream, 
                SsmPacketParser.ReadCompleted, 
                internalState);

            return internalState;
        }

        /// <summary>
        /// Returns the new packet to the caller
        /// </summary>
        public SsmPacket EndReadFromStream(IAsyncResult asyncResult)
        {
            ReadFromStreamAsyncResult internalState = (ReadFromStreamAsyncResult)asyncResult;
            try
            {
                if (internalState.Exception != null)
                {
                    throw internalState.Exception;
                }

                return SsmPacket.ParseResponse(this.buffer, 0, this.bytesReceived);
            }
            finally
            {
                internalState.Dispose();
            }
        }

        /// <summary>
        /// Invoked each time stream.BeginRead completes
        /// </summary>
        /// <remarks>
        /// Parse the received bytes, notify the consumer if a full packet is
        /// received or if anything goes wrong.
        /// </remarks>
        private static void ReadCompleted(IAsyncResult asyncResult)
        {
            ReadFromStreamAsyncResult internalState = (ReadFromStreamAsyncResult) asyncResult.AsyncState;
            try
            {
                SsmPacketParser parser = internalState.Parser;
                int offset = parser.bytesReceived;
                int bytesReceived = internalState.Stream.EndRead(asyncResult);
                
                for (int i = 0; i < bytesReceived; i++)
                {
                    int index = offset + i;
                    internalState.Parser.CheckByte(index, internalState.Parser.buffer[index]);
                    parser.bytesReceived++;
                }

                if (internalState.Parser.IsComplete)
                {
                    internalState.Completed();
                }
                else
                {
                    internalState.Parser.InternalBeginReadFromStream(
                        internalState.Stream,
                        SsmPacketParser.ReadCompleted,
                        internalState);
                }
            }
            catch (System.Security.SecurityException ex)
            {
                internalState.Exception = ex;
                internalState.Completed();
            }
            catch (IOException ex)
            {
                internalState.Exception = ex;
                internalState.Completed();
            }
        }

        /// <summary>
        /// Wrapper to invoke stream.BeginRead
        /// </summary>
        private IAsyncResult InternalBeginReadFromStream(Stream stream, AsyncCallback asyncCallback, object asyncState)
        {
            return stream.BeginRead(
                this.buffer,
                this.bytesReceived,
                this.buffer.Length - this.bytesReceived,
                asyncCallback,
                asyncState);
        }

        /// <summary>
        /// Validate / interpret a single received byte
        /// </summary>
        private void CheckByte(int index, byte b)
        {
            if (index == (int)SsmPacketIndex.Header)
            {
                if (b != SsmPacket.FirstByte)
                {
                    throw new SsmPacketFormatException("Lost synchronization with ECU (illegal first byte)");
                }
            }
            else if (index == (int)SsmPacketIndex.Destination)
            {
                if (b != (byte)SsmDeviceType.SubaruEcu)
                {
                    throw new SsmPacketFormatException("Illegal 'destination' byte in echo");
                }
            }
            else if (index == (int)SsmPacketIndex.Source)
            {
                if (b != (byte)SsmDeviceType.DiagnosticTool)
                {
                    throw new SsmPacketFormatException("Illegal 'source' byte in echo");
                }
            }
            else if (index == (int)SsmPacketIndex.DataSize)
            {
                echoLength = SsmPacket.HeaderLength + b;
            }
            else if (index == echoLength + (int)SsmPacketIndex.Header)
            {
                if (b != SsmPacket.FirstByte)
                {
                    throw new SsmPacketFormatException("Illegal first byte in response payload");
                }
            }
            else if (index == echoLength + (int)SsmPacketIndex.Destination)
            {
                if (b != (byte)SsmDeviceType.DiagnosticTool)
                {
                    throw new SsmPacketFormatException("Illegal 'destination' byte in response");
                }
            }
            else if (index == echoLength + (int)SsmPacketIndex.Source)
            {
                if (b != (byte)SsmDeviceType.SubaruEcu)
                {
                    throw new SsmPacketFormatException("Illegal 'source' byte in response");
                }
            }
            else if (index == echoLength + (int)SsmPacketIndex.DataSize)
            {
                responseLength = SsmPacket.HeaderLength + b;
                checkLength = true;
            }
            else if (index > this.echoLength + this.responseLength)
            {
                throw new SsmPacketFormatException("Response contains more bytes than expected.");
            }
        }
    }
}

///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// SsmInterface.cs
///////////////////////////////////////////////////////////////////////////////
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Security;

namespace NateW.Ssm
{
    /// <summary>
    /// Knows how to send and receive SSM packets, so the caller can invoke
    /// SSM operations without knowing anything about the SSM packet syntax.
    /// </summary>
    public class SsmInterface
    {
        /// <summary>
        /// Stream to use for communications with the ECU
        /// </summary>
        private Stream stream;

        /// <summary>
        /// This is used to guard against multiple simultaneous read/write operations
        /// </summary>
        private int outstandingQueries;

        /// <summary>
        /// The ECU's identifer
        /// </summary>
        private string ecuIdentifier;

        /// <summary>
        /// The ECU's parameter compatibility map
        /// </summary>
        private IList<byte> compatibilityMap;

        /// <summary>
        /// The ECU's identifier
        /// </summary>
        public string EcuIdentifier
        {
            [DebuggerStepThrough()]
            get { return this.ecuIdentifier; }
        }

        /// <summary>
        /// The ECU's parameter compatibility map
        /// </summary>
        public IList<byte> CompatibilityMap
        {
            [DebuggerStepThrough()]
            get { return this.compatibilityMap; }
        }

        /// <summary>
        /// True if a read/write operation is in progress
        /// </summary>
        public bool OperationInProgress
        {
            get { return this.outstandingQueries != 0; }
        }

        /// <summary>
        /// Private constructor - use factory instead
        /// </summary>
        private SsmInterface(Stream stream)
        {
            this.stream = stream;
        }

        /// <summary>
        /// Factory
        /// </summary>
        public static SsmInterface GetInstance(Stream stream)
        {
            return new SsmInterface(stream);
        }

        #region GetEcuIdentifier

        /// <summary>
        /// Sends an ECU identifier request
        /// </summary>
        public IAsyncResult BeginGetEcuIdentifier(AsyncCallback callback, object state)
        {
            Trace.WriteLine("SsmInterface.BeginGetEcuIdentifier");
            this.BeginOperation();

            GetEcuIdentifierAsyncResult internalState = new GetEcuIdentifierAsyncResult(
                callback, 
                state);
            SsmPacket request = SsmPacket.CreateEcuIdentifierRequest();
            this.stream.BeginWrite(
                request.Data, 
                0, 
                request.Data.Length, 
                this.EcuIdentifierRequestWritten,
                internalState);
            return internalState;
        }

        /// <summary>
        /// Invoked after the ECU identifier request has been written - begins reading the response
        /// </summary>
        private void EcuIdentifierRequestWritten(IAsyncResult asyncResult)
        {
            Trace.WriteLine("SsmInterface.EcuIdentifierRequestWritten");
            GetEcuIdentifierAsyncResult internalState = (GetEcuIdentifierAsyncResult)asyncResult.AsyncState;
                
            try
            {
                this.stream.EndWrite(asyncResult);
                internalState.Parser.BeginReadFromStream(this.stream, EcuIdentifierResponseRead, internalState);
            }
            catch (IOException ex)
            {
                internalState.Exception = ex;
                internalState.Completed();
            }
            catch (SecurityException ex)
            {
                internalState.Exception = ex;
                internalState.Completed();
            }
        }

        /// <summary>
        /// Invoked after the ECU identifier response has been received
        /// </summary>
        private void EcuIdentifierResponseRead(IAsyncResult asyncResult)
        {
            Trace.WriteLine("SsmInterface.EcuIdentifierResponseRead");
            GetEcuIdentifierAsyncResult internalState = (GetEcuIdentifierAsyncResult)asyncResult.AsyncState;

            try
            {
                SsmPacket response = internalState.Parser.EndReadFromStream(asyncResult);
                this.ecuIdentifier = response.EcuIdentifier;
                this.compatibilityMap = response.CompatibilityMap;                
            }
            catch (IOException ex)
            {
                internalState.Exception = ex;
            }
            catch (SecurityException ex)
            {
                internalState.Exception = ex;
            }

            Exception localException = internalState.Exception;
            if (localException == null)
            {
                Trace.WriteLine("SsmInterface.EcuIdentifierResponseRead: " + this.ecuIdentifier);
            }
            else
            {
                Trace.WriteLine("SsmInterface.EcuIdentifierResponseRead: " + localException.ToString());
            }
            internalState.Completed();
        }

        /// <summary>
        /// Returns the ECU identifier to the caller
        /// </summary>
        public void EndGetEcuIdentifier(IAsyncResult asyncResult)
        {
            Trace.WriteLine("SsmInterface.EndGetEcuIdentifier");
            GetEcuIdentifierAsyncResult internalState = (GetEcuIdentifierAsyncResult)asyncResult;

            try
            {
                this.EndOperation();

                if (internalState.Exception != null)
                {
                    throw internalState.Exception;
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine("Error sending init!");
            }
            finally
            {
                internalState.Dispose();
            }
        }

        #endregion

        #region MultipleRead

        /// <summary>
        /// Begins reading multiple disjoint addesses from the ECU
        /// </summary>
        public IAsyncResult BeginMultipleRead(IList<int> addresses, AsyncCallback callback, object state)
        {
            this.BeginOperation();

            MultipleReadAsyncResult internalState = new MultipleReadAsyncResult(callback, state);
            SsmPacket request = SsmPacket.CreateMultipleReadRequest(addresses);
            this.stream.BeginWrite(
                request.Data,
                0,
                request.Data.Length,
                this.MultipleReadRequestWritten,
                internalState);
            return internalState;
        }

        /// <summary>
        /// Invoked after the read request has been written to the ECU - begins waiting for the response
        /// </summary>
        private void MultipleReadRequestWritten(IAsyncResult asyncResult)
        {
            MultipleReadAsyncResult internalState = (MultipleReadAsyncResult)asyncResult.AsyncState;

            try
            {
                this.stream.EndWrite(asyncResult);
                internalState.Parser.BeginReadFromStream(
                    stream, 
                    MultipleReadResponseReceived, 
                    internalState);
            }
            catch (IOException ex)
            {
                internalState.Exception = ex;
                internalState.Completed();
            }
            catch (SecurityException ex)
            {
                internalState.Exception = ex;
                internalState.Completed();
            }
        }
        
        /// <summary>
        /// Invoked after the read response has been received from the ECU
        /// </summary>
        private void MultipleReadResponseReceived(IAsyncResult asyncResult)
        {
            MultipleReadAsyncResult internalState = (MultipleReadAsyncResult)asyncResult.AsyncState;

            try
            {
                SsmPacket response = internalState.Parser.EndReadFromStream(asyncResult);
                
                // Uncomment for easier debugging:                
                //byte[] data = response.Data;
                //SsmCommand command = response.Command;
                //int payloadLength = response.PayloadLength;
                //IList<byte> values = response.Values;

                foreach (byte value in response.Values)
                {
                    internalState.ValueList.Add(value);
                }
            }
            catch (IOException ex)
            {
                internalState.Exception = ex;
            }
            catch (SecurityException ex)
            {
                internalState.Exception = ex;
            }
            internalState.Completed();
        }

        /// <summary>
        /// Returns the read results to the caller
        /// </summary>
        public byte[] EndMultipleRead(IAsyncResult asyncResult)
        {
            MultipleReadAsyncResult internalState = (MultipleReadAsyncResult)asyncResult;

            try
            {
                this.EndOperation();

                if (internalState.Exception != null)
                {
                    throw internalState.Exception;
                }

                return internalState.Values;
            }
            finally
            {
                //internalState.Dispose();
            }
        }

        #endregion

        #region BlockRead

        /// <summary>
        /// Read a block of RAM from the ECU
        /// </summary>
        public IAsyncResult BeginBlockRead(int address, int length, AsyncCallback callback, object state)
        {
            this.BeginOperation();

            BlockReadAsyncResult internalState = new BlockReadAsyncResult(callback, state);
            SsmPacket request = SsmPacket.CreateBlockReadRequest(address, length);
            this.stream.BeginWrite(
                request.Data,
                0,
                request.Data.Length,
                this.BlockReadRequestWritten,
                internalState);
            return internalState;
        }

        /// <summary>
        /// Invoked after the read request has been written to the ECU - begins waiting for the response
        /// </summary>
        private void BlockReadRequestWritten(IAsyncResult asyncResult)
        {
            BlockReadAsyncResult internalState = (BlockReadAsyncResult)asyncResult.AsyncState;

            try
            {
                this.stream.EndWrite(asyncResult);
                internalState.Parser.BeginReadFromStream(
                    stream,
                    BlockReadResponseReceived,
                    internalState);
            }
            catch (IOException ex)
            {
                internalState.Exception = ex;
                internalState.Completed();
            }
            catch (SecurityException ex)
            {
                internalState.Exception = ex;
                internalState.Completed();
            }
        }

        /// <summary>
        /// Invoked after the read response has been received from the ECU
        /// </summary>
        private void BlockReadResponseReceived(IAsyncResult asyncResult)
        {
            BlockReadAsyncResult internalState = (BlockReadAsyncResult)asyncResult.AsyncState;

            try
            {
                SsmPacket response = internalState.Parser.EndReadFromStream(asyncResult);

                // Uncomment for easier debugging:                
                //byte[] data = response.Data;
                //SsmCommand command = response.Command;
                //int payloadLength = response.PayloadLength;
                //IList<byte> values = response.Values;

                foreach (byte value in response.Values)
                {
                    internalState.ValueList.Add(value);
                }
            }
            catch (IOException ex)
            {
                internalState.Exception = ex;
            }
            catch (SecurityException ex)
            {
                internalState.Exception = ex;
            }
            internalState.Completed();
        }

        /// <summary>
        /// Returns the read results to the caller
        /// </summary>
        public byte[] EndBlockRead(IAsyncResult asyncResult)
        {
            BlockReadAsyncResult internalState = (BlockReadAsyncResult)asyncResult;

            try
            {
                this.EndOperation();

                if (internalState.Exception != null)
                {
                    throw internalState.Exception;
                }

                return internalState.Values;
            }
            finally
            {
                //internalState.Dispose();
            }
        }

        #endregion 

        #region BlockWrite

        /// <summary>
        /// Write a block of RAM from the ECU
        /// </summary>
        public IAsyncResult BeginBlockWrite(int address, byte[] bytes, AsyncCallback callback, object state)
        {
            this.BeginOperation();

            BlockReadAsyncResult internalState = new BlockReadAsyncResult(callback, state);
            SsmPacket request = SsmPacket.CreateBlockWriteRequest(address, bytes);
            this.stream.BeginWrite(
                request.Data,
                0,
                request.Data.Length,
                this.BlockWriteRequestWritten,
                internalState);
            return internalState;
        }

        /// <summary>
        /// Invoked after the read request has been written to the ECU - begins waiting for the response
        /// </summary>
        private void BlockWriteRequestWritten(IAsyncResult asyncResult)
        {
            BlockReadAsyncResult internalState = (BlockReadAsyncResult)asyncResult.AsyncState;

            try
            {
                this.stream.EndWrite(asyncResult);
                internalState.Parser.BeginReadFromStream(
                    stream,
                    BlockWriteResponseReceived,
                    internalState);
            }
            catch (IOException ex)
            {
                internalState.Exception = ex;
                internalState.Completed();
            }
            catch (SecurityException ex)
            {
                internalState.Exception = ex;
                internalState.Completed();
            }
        }

        /// <summary>
        /// Invoked after the read response has been received from the ECU
        /// </summary>
        private void BlockWriteResponseReceived(IAsyncResult asyncResult)
        {
            BlockReadAsyncResult internalState = (BlockReadAsyncResult)asyncResult.AsyncState;

            try
            {
                SsmPacket response = internalState.Parser.EndReadFromStream(asyncResult);

                // Uncomment for easier debugging:                
                //byte[] data = response.Data;
                //SsmCommand command = response.Command;
                //int payloadLength = response.PayloadLength;
                //IList<byte> values = response.Values;

                foreach (byte value in response.Values)
                {
                    internalState.ValueList.Add(value);
                }
            }
            catch (IOException ex)
            {
                internalState.Exception = ex;
            }
            catch (SecurityException ex)
            {
                internalState.Exception = ex;
            }
            internalState.Completed();
        }

        /// <summary>
        /// Returns the read results to the caller
        /// </summary>
        public byte[] EndBlockWrite(IAsyncResult asyncResult)
        {
            BlockReadAsyncResult internalState = (BlockReadAsyncResult)asyncResult;

            try
            {
                this.EndOperation();

                if (internalState.Exception != null)
                {
                    throw internalState.Exception;
                }

                return internalState.Values;
            }
            finally
            {
                //internalState.Dispose();
            }
        }

        #endregion

        #region Synchronous methods

        /// <summary>
        /// Synchronously issues a read request and waits for the response
        /// </summary>
        public byte[] SyncReadMultiple(IList<int> addresses)
        {
            SsmPacket request = SsmPacket.CreateMultipleReadRequest(addresses);
            SsmPacket response = this.SyncSendReceive(request);
            return ((List<byte>)response.Values).ToArray();
        }

        /// <summary>
        /// Synchronously issues a block read request and waits for the response
        /// </summary>
        public byte[] SyncReadBlock(int address, int length)
        {
            SsmPacket request = SsmPacket.CreateBlockReadRequest(address, length);
            SsmPacket response = this.SyncSendReceive(request);
            return ((List<byte>)response.Values).ToArray();
        }

        /// <summary>
        /// Synchronously sends a request and waits for the response
        /// </summary>
        public SsmPacket SyncSendReceive(SsmPacket request)
        {
            //Console.WriteLine("#### Raw request data:");
            byte[] requestBuffer = request.Data;
            SsmUtility.DumpBuffer(requestBuffer);
            
            stream.Write(requestBuffer, 0, requestBuffer.Length);
            Thread.Sleep(250);

            byte[] responseBuffer = new byte[2000];
            int bytesRead = stream.Read(responseBuffer, 0, responseBuffer.Length);
            Console.WriteLine("Bytes read: {0}", bytesRead);
            Console.WriteLine("#### Raw response data:");
            SsmUtility.DumpBuffer(responseBuffer);
            
            SsmPacket response = SsmPacket.ParseResponse(responseBuffer, 0, bytesRead);
            return response;
        }

        #endregion

        /// <summary>
        /// Invoke this to throw an InvalidOperationException when the consumer
        /// tries to invoke a new transaction before an existing transaction
        /// has finished.
        /// </summary>
        private void BeginOperation()
        {
            int result = Interlocked.Increment(ref this.outstandingQueries);
            if (result != 1)
            {
                throw new InvalidOperationException("Simultaneous operations attempted");
            }
        }

        /// <summary>
        /// Invoke when a read/write operation completes
        /// </summary>
        private void EndOperation()
        {
            int result = Interlocked.Decrement(ref this.outstandingQueries);
            if (result != 0)
            {
                throw new InvalidOperationException("No operation in progress now.");
            }
        }
    }
}

///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// Utility.cs
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Runtime.CompilerServices;
using NateW.J2534;

namespace NateW.Ssm
{
    public delegate void VoidVoid();
    public delegate void TraceLine(string line);

    public static class SsmUtility
    {
        public const string OpenPort20DisplayName = "OpenPort 2.0";
        public const string MockEcuDisplayName = "Mock ECU";
        private const string OpenPort20PortName = "op20pt32.dll";
        
        /// <summary>
        /// Find out if the OpenPort 2.0 DLL is available.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static bool OpenPort20Exists()
        {
            try
            {
                using (DynamicPassThru temp = DynamicPassThru.GetInstance("op20pt32.dll"))
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Indicate whether the operation that caused the exception should be retried.
        /// </summary>
        public static bool IsTransientException(Exception exception)
        {
            if (exception is SsmPacketFormatException)
            {
                return true;
            }

            //if (exception is System.IO.IOException)
            //{
            //    return true;
            //}

            if (exception is UnauthorizedAccessException)
            {
                return true;
            }

            return false;
        }


        /// <summary>
        /// Open a port, open an OpenPort, or create a mock ECU stream.
        /// </summary>
        /// <param name="portName">COM port name, passthru DLL name, or "Mock ECU".</param>
        /// <param name="baudRate">Baud rate to use for COM port.</param>
        /// <returns>Stream.</returns>
        public static Stream GetDataStream(
            string portName, 
            int baudRate,
            ref SerialPort port,
            TraceLine traceLine)
        {
            if (portName == SsmUtility.MockEcuDisplayName)
            {
                MockEcuStream.Image = new EcuImage2F12785206();
                return MockEcuStream.CreateInstance();
            }

            if (SsmUtility.OpenPort20DisplayName.Equals(portName, StringComparison.OrdinalIgnoreCase))
            {
                portName = SsmUtility.OpenPort20PortName;
            }

            if (portName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
            {
                PassThruStream passThruStream = PassThruStream.GetInstance(portName);
                passThruStream.OpenSsmChannel();
                return passThruStream;
            }

            Stream stream = null;

            if (port == null)
            {
                traceLine("SsmUtility.GetDataStream: Creating port.");
                port = new SerialPort(portName, baudRate, Parity.None, 8);
                port.ReadTimeout = 500;
                port.WriteTimeout = 500;
            }

            if (port.IsOpen)
            {
                traceLine("SsmUtility.GetDataStream: Port already open, draining input queue...");
                int bytesToRead = 0;
                while ((bytesToRead = port.BytesToRead) > 0)
                {
                    Trace.WriteLine("SsmUtility.GetDataStream: " + bytesToRead.ToString(CultureInfo.InvariantCulture) + " bytes in queue, reading...");
                    byte[] buffer = new byte[bytesToRead];
                    port.Read(buffer, 0, buffer.Length);
                    traceLine("SsmUtility.GetDataStream: Read completed.");
                    Thread.Sleep(500);
                }
            }
            else
            {
                traceLine("SsmUtility.GetDataStream: Port not open.");

                bool exists = false;
                foreach (string name in System.IO.Ports.SerialPort.GetPortNames())
                {
                    if (string.Compare(name, port.PortName, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        traceLine("SsmUtility.GetDataStream: Port " + name + " exists.");
                        exists = true;
                        break;
                    }
                }

                if (exists)
                {
                    port.Open();
                    Trace.WriteLine("SsmUtility.GetDataStream: Port opened.");
                }
                else
                {
                    string message = "Port " + port.PortName + " does not exist.";
                    throw new IOException(message);
                }
            }

            stream = port.BaseStream;
            Trace.WriteLine("SsmUtility.GetDataStream: Input queue drained, returning stream.");
            return stream;
        }

        internal static T First<T>(IEnumerable<T> enumerable)
        {
            IEnumerator<T> enumerator = enumerable.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                throw new InvalidOperationException("Enumerable is empty.");
            }
            
            return enumerator.Current;
        }

        internal static string GetExceptionMessage(Exception exception)
        {
            if (SsmUtility.IsTransientException(exception))
            {
                return exception.Message;
            }
            else
            {
                return exception.ToString();
            }
        }

        internal static void DumpBuffer(byte[] buffer)
        {
            Console.WriteLine("Buffer length: {0}", buffer.Length);

            StringBuilder builder = new StringBuilder(100);
            for (int i = 0; i < buffer.Length; i++)
            {
                builder.Append("0x");
                builder.Append(buffer[i].ToString("X2", CultureInfo.InvariantCulture));
                builder.Append(',');

                if ((i % 16 == 15) || (i == buffer.Length - 1))
                {
                    Console.WriteLine(builder.ToString());
                    builder = new StringBuilder(100);
                }
            }
        }
    }
}

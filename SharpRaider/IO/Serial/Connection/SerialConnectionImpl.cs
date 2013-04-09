/*
 * This code is derived from the Java version of RomRaider
 *
 * RomRaider Open-Source Tuning, Logging and Reflashing
 * Copyright (C) 2006-2012 RomRaider.com
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License along
 * with this program; if not, write to the Free Software Foundation, Inc.,
 * 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using System;
using System.IO;
using Gnu.IO;
using Org.Apache.Log4j;
using RomRaider.IO.Connection;
using RomRaider.IO.Serial.Connection;
using RomRaider.Logger.Ecu.Exception;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.IO.Serial.Connection
{
	public sealed class SerialConnectionImpl : SerialConnection
	{
		private static readonly Logger LOGGER = Logger.GetLogger(typeof(RomRaider.IO.Serial.Connection.SerialConnectionImpl
			));

		private static readonly string RXTX_READ_LINE_HACK = "Underlying input stream returned zero bytes";

		private readonly SerialPort serialPort;

		private readonly BufferedOutputStream os;

		private readonly BufferedInputStream @is;

		private readonly BufferedReader reader;

		public SerialConnectionImpl(string portName, ConnectionProperties connectionProperties
			)
		{
			ParamChecker.CheckNotNullOrEmpty(portName, "portName");
			ParamChecker.CheckNotNull(connectionProperties, "connectionProperties");
			try
			{
				serialPort = Connect(portName, connectionProperties);
				os = new BufferedOutputStream(serialPort.GetOutputStream());
				@is = new BufferedInputStream(serialPort.GetInputStream());
				reader = new BufferedReader(new InputStreamReader(@is));
				LOGGER.Info("Serial connection initialised: " + connectionProperties);
			}
			catch (Exception e)
			{
				Close();
				throw new NotConnectedException(e);
			}
		}

		public void Write(byte[] bytes)
		{
			try
			{
				os.Write(bytes, 0, bytes.Length);
				os.Flush();
			}
			catch (IOException e)
			{
				throw new SerialCommunicationException(e);
			}
		}

		public int Available()
		{
			try
			{
				return @is.Available();
			}
			catch (IOException e)
			{
				throw new SerialCommunicationException(e);
			}
		}

		public int Read()
		{
			try
			{
				WaitForBytes(1);
				return @is.Read();
			}
			catch (IOException e)
			{
				throw new SerialCommunicationException(e);
			}
		}

		public void Read(byte[] bytes)
		{
			try
			{
				WaitForBytes(bytes.Length);
				@is.Read(bytes, 0, bytes.Length);
			}
			catch (IOException e)
			{
				throw new SerialCommunicationException(e);
			}
		}

		public string ReadLine()
		{
			try
			{
				WaitForBytes(1);
				return reader.ReadLine();
			}
			catch (IOException e)
			{
				if (Sharpen.Runtime.EqualsIgnoreCase(RXTX_READ_LINE_HACK, e.Message))
				{
					return null;
				}
				throw new SerialCommunicationException(e);
			}
		}

		public byte[] ReadAvailable()
		{
			byte[] response = new byte[Available()];
			Read(response);
			return response;
		}

		public void ReadStaleData()
		{
			if (Available() <= 0)
			{
				return;
			}
			long end = Runtime.CurrentTimeMillis() + 100L;
			do
			{
				byte[] staleBytes = ReadAvailable();
				LOGGER.Debug("Stale data read: " + HexUtil.AsHex(staleBytes));
				ThreadUtil.Sleep(2);
			}
			while ((Available() > 0) && (Runtime.CurrentTimeMillis() <= end));
		}

		public void Close()
		{
			if (os != null)
			{
				try
				{
					os.Close();
				}
				catch (IOException e)
				{
					LOGGER.Error("Error closing output stream", e);
				}
			}
			if (reader != null)
			{
				try
				{
					//readStaleData();
					reader.Close();
				}
				catch (IOException e)
				{
					LOGGER.Error("Error closing input stream reader", e);
				}
			}
			if (@is != null)
			{
				try
				{
					//readStaleData();
					@is.Close();
				}
				catch (IOException e)
				{
					LOGGER.Error("Error closing input stream", e);
				}
			}
			if (serialPort != null)
			{
				try
				{
					serialPort.Close();
				}
				catch (System.Exception e)
				{
					LOGGER.Error("Error closing serial port", e);
				}
			}
			LOGGER.Info("Connection closed.");
		}

		public void SendBreak(int duration)
		{
			try
			{
				serialPort.SendBreak(duration);
			}
			catch (System.Exception e)
			{
				throw new SerialCommunicationException(e);
			}
		}

		private SerialPort Connect(string portName, ConnectionProperties connectionProperties
			)
		{
			CommPortIdentifier portIdentifier = ResolvePortIdentifier(portName);
			SerialPort serialPort = OpenPort(portIdentifier, connectionProperties.GetConnectTimeout
				());
			InitSerialPort(serialPort, connectionProperties.GetBaudRate(), connectionProperties
				.GetDataBits(), connectionProperties.GetStopBits(), connectionProperties.GetParity
				());
			LOGGER.Info("Connected to: " + portName);
			return serialPort;
		}

		private SerialPort OpenPort(CommPortIdentifier portIdentifier, int connectTimeout
			)
		{
			CheckIsSerialPort(portIdentifier);
			try
			{
				return (SerialPort)portIdentifier.Open(this.GetType().FullName, connectTimeout);
			}
			catch (PortInUseException)
			{
				throw new SerialCommunicationException("Port is currently in use: " + portIdentifier
					.GetName());
			}
		}

		private void CheckIsSerialPort(CommPortIdentifier portIdentifier)
		{
			if (portIdentifier.GetPortType() != CommPortIdentifier.PORT_SERIAL)
			{
				throw new UnsupportedPortTypeException("Port type " + portIdentifier.GetPortType(
					) + " not supported - must be serial.");
			}
		}

		private void InitSerialPort(SerialPort serialPort, int baudrate, int dataBits, int
			 stopBits, int parity)
		{
			try
			{
				serialPort.SetFlowControlMode(SerialPort.FLOWCONTROL_NONE);
				serialPort.SetSerialPortParams(baudrate, dataBits, stopBits, parity);
				serialPort.DisableReceiveTimeout();
				serialPort.SetRTS(false);
			}
			catch (UnsupportedCommOperationException e)
			{
				throw new NotSupportedException(e);
			}
		}

		private CommPortIdentifier ResolvePortIdentifier(string portName)
		{
			try
			{
				return CommPortIdentifier.GetPortIdentifier(portName);
			}
			catch (NoSuchPortException)
			{
				throw new PortNotFoundException("Unable to resolve port: " + portName);
			}
		}

		private void WaitForBytes(int numBytes)
		{
			while (Available() < numBytes)
			{
				ThreadUtil.Sleep(2L);
			}
		}
	}
}

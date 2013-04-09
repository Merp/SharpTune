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
using RomRaider.IO.Connection;
using RomRaider.IO.Serial.Connection;
using RomRaider.Logger.Ecu.Exception;
using RomRaider.Logger.External.Fourteenpoint7.IO;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.External.Fourteenpoint7.IO
{
	public sealed class NawConnectionImpl : NawConnection
	{
		private static readonly Org.Apache.Log4j.Logger LOGGER = Org.Apache.Log4j.Logger.GetLogger
			(typeof(RomRaider.Logger.External.Fourteenpoint7.IO.NawConnectionImpl));

		private readonly RomRaider.IO.Serial.Connection.SerialConnection connection;

		public NawConnectionImpl(string port)
		{
			ParamChecker.CheckNotNullOrEmpty(port, "port");
			connection = SerialConnection(port);
		}

		public byte[] ReadBytes()
		{
			try
			{
				byte[] bytes = new byte[9];
				connection.Read(bytes);
				LOGGER.Trace("NAW_7S Response: " + HexUtil.AsHex(bytes));
				return bytes;
			}
			catch (Exception e)
			{
				Close();
				throw new SerialCommunicationException(e);
			}
		}

		public void Write(byte[] bytes)
		{
			try
			{
				connection.Write(bytes);
			}
			catch (System.Exception e)
			{
				Close();
				throw new SerialCommunicationException(e);
			}
		}

		public void Close()
		{
			connection.Close();
		}

		private SerialConnectionImpl SerialConnection(string port)
		{
			ConnectionProperties connectionProperties = new NawConnectionProperties();
			return new SerialConnectionImpl(port, connectionProperties);
		}
	}
}

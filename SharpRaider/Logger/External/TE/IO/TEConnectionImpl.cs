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

using RomRaider.IO.Connection;
using RomRaider.IO.Serial.Connection;
using RomRaider.Logger.External.TE.IO;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.External.TE.IO
{
	public sealed class TEConnectionImpl : TEConnection
	{
		private readonly RomRaider.IO.Serial.Connection.SerialConnection connection;

		public TEConnectionImpl(string port)
		{
			ParamChecker.CheckNotNullOrEmpty(port, "port");
			connection = SerialConnection(port);
		}

		//      connection = new TestTEConnection();
		public byte ReadByte()
		{
			return unchecked((byte)connection.Read());
		}

		public void Close()
		{
			connection.Close();
		}

		private RomRaider.IO.Serial.Connection.SerialConnection SerialConnection(string port
			)
		{
			ConnectionProperties connectionProperties = new TEConnectionProperties();
			return new SerialConnectionImpl(port, connectionProperties);
		}
	}
}

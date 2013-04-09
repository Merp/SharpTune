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
using Sharpen;

namespace RomRaider.IO.Connection
{
	public sealed class ConnectionPropertiesImpl : ConnectionProperties
	{
		private readonly int baudRate;

		private readonly int dataBits;

		private readonly int stopBits;

		private readonly int parity;

		private readonly int connectTimeout;

		private readonly int sendTimeout;

		public ConnectionPropertiesImpl(int baudRate, int dataBits, int stopBits, int parity
			, int connectTimeout, int sendTimeout)
		{
			this.baudRate = baudRate;
			this.dataBits = dataBits;
			this.stopBits = stopBits;
			this.parity = parity;
			this.connectTimeout = connectTimeout;
			this.sendTimeout = sendTimeout;
		}

		public int GetBaudRate()
		{
			return baudRate;
		}

		public void SetBaudRate(int b)
		{
		}

		public int GetDataBits()
		{
			return dataBits;
		}

		public int GetStopBits()
		{
			return stopBits;
		}

		public int GetParity()
		{
			return parity;
		}

		public int GetConnectTimeout()
		{
			return connectTimeout;
		}

		public int GetSendTimeout()
		{
			return sendTimeout;
		}

		public override string ToString()
		{
			string properties = string.Format("%s[baudRate=%d, dataBits=%d, stopBits=%d, parity=%d, "
				 + "connectTimeout=%d, sendTimeout=%d]", GetType().Name, GetBaudRate(), GetDataBits
				(), GetStopBits(), GetParity(), GetConnectTimeout(), GetSendTimeout());
			return properties;
		}
	}
}

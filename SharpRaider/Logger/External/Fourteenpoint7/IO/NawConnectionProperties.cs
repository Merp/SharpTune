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
using RomRaider.Logger.External.Fourteenpoint7.IO;
using Sharpen;

namespace RomRaider.Logger.External.Fourteenpoint7.IO
{
	public sealed class NawConnectionProperties : ConnectionProperties
	{
		public int GetBaudRate()
		{
			return 9600;
		}

		public void SetBaudRate(int b)
		{
		}

		public int GetDataBits()
		{
			return 8;
		}

		public int GetStopBits()
		{
			return 1;
		}

		public int GetParity()
		{
			return 0;
		}

		public int GetConnectTimeout()
		{
			return 2000;
		}

		public int GetSendTimeout()
		{
			return 500;
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

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

using RomRaider.Logger.Ecu.Comms.Query;
using RomRaider.Logger.Ecu.Definition;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.Ecu.Comms.Query
{
	public sealed class EcuQueryImpl : EcuQuery
	{
		private readonly EcuData ecuData;

		private readonly byte[] bytes;

		private readonly string hex;

		private double response;

		public EcuQueryImpl(EcuData ecuData)
		{
			ParamChecker.CheckNotNull(ecuData);
			this.ecuData = ecuData;
			bytes = ecuData.GetAddress().GetBytes();
			hex = HexUtil.AsHex(bytes);
		}

		public LoggerData GetLoggerData()
		{
			return ecuData;
		}

		public string[] GetAddresses()
		{
			return ecuData.GetAddress().GetAddresses();
		}

		public byte[] GetBytes()
		{
			return bytes;
		}

		public string GetHex()
		{
			return hex;
		}

		public double GetResponse()
		{
			return response;
		}

		public void SetResponse(byte[] bytes)
		{
			this.response = ecuData.GetSelectedConvertor().Convert(bytes);
		}

		public override bool Equals(object @object)
		{
			return @object is RomRaider.Logger.Ecu.Comms.Query.EcuQueryImpl && GetHex().Equals
				(((RomRaider.Logger.Ecu.Comms.Query.EcuQueryImpl)@object).GetHex());
		}

		public override int GetHashCode()
		{
			return GetHex().GetHashCode();
		}

		public override string ToString()
		{
			return "0x" + GetHex();
		}
	}
}

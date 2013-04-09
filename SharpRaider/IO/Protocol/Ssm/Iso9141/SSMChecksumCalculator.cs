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
using RomRaider.Util;
using Sharpen;

namespace RomRaider.IO.Protocol.Ssm.Iso9141
{
	public sealed class SSMChecksumCalculator
	{
		public SSMChecksumCalculator()
		{
			throw new NotSupportedException();
		}

		public static byte CalculateChecksum(byte[] bytes)
		{
			int total = 0;
			for (int i = 0; i < (bytes.Length - 1); i++)
			{
				byte b = bytes[i];
				total += ByteUtil.AsInt(b);
			}
			return ByteUtil.AsByte(total - (((int)(((uint)total) >> 16)) << 16));
		}
	}
}
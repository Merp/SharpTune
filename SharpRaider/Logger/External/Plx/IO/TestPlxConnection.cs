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
using RomRaider.IO.Serial.Connection;
using RomRaider.Logger.External.Plx.IO;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.External.Plx.IO
{
	public sealed class TestPlxConnection : SerialConnection
	{
		private readonly byte[] data = new byte[] { unchecked((byte)unchecked((int)(0x80)
			)), unchecked((int)(0x00)), unchecked((int)(0x00)), unchecked((int)(0x00)), unchecked(
			(int)(0x00)), unchecked((int)(0x20)), unchecked((int)(0x00)), unchecked((int)(0x00
			)), unchecked((int)(0x01)), unchecked((int)(0x00)), unchecked((int)(0x05)), unchecked(
			(int)(0x00)), unchecked((int)(0x01)), unchecked((int)(0x00)), unchecked((int)(0x3F
			)), unchecked((int)(0x3F)), unchecked((int)(0x00)), unchecked((int)(0x02)), unchecked(
			(int)(0x00)), unchecked((int)(0x02)), unchecked((int)(0x30)), unchecked((int)(0x40
			)), unchecked((byte)unchecked((int)(0x80))), unchecked((int)(0x00)), unchecked((
			int)(0x00)), unchecked((int)(0x00)), unchecked((int)(0x00)), unchecked((int)(0x10
			)), unchecked((int)(0x00)), unchecked((int)(0x00)), unchecked((int)(0x01)), unchecked(
			(int)(0x00)), unchecked((int)(0x07)), unchecked((int)(0x00)), unchecked((int)(0x01
			)), unchecked((int)(0x00)), unchecked((int)(0x2F)), unchecked((int)(0x3F)), unchecked(
			(int)(0x00)), unchecked((int)(0x02)), unchecked((int)(0x00)), unchecked((int)(0x01
			)), unchecked((int)(0x00)), unchecked((int)(0x40)), unchecked((byte)unchecked((int
			)(0x80))), unchecked((int)(0x00)), unchecked((int)(0x00)), unchecked((int)(0x00)
			), unchecked((int)(0x00)), unchecked((int)(0x08)), unchecked((int)(0x00)), unchecked(
			(int)(0x00)), unchecked((int)(0x01)), unchecked((int)(0x00)), unchecked((int)(0x09
			)), unchecked((int)(0x00)), unchecked((int)(0x01)), unchecked((int)(0x00)), unchecked(
			(int)(0x1F)), unchecked((int)(0x3F)), unchecked((int)(0x00)), unchecked((int)(0x02
			)), unchecked((int)(0x00)), unchecked((int)(0x00)), unchecked((int)(0x3F)), unchecked(
			(int)(0x40)) };

		private int index;

		private byte[] result = new byte[1];

		// max byte value for address and data is 0x3F
		// AFR 0
		// AFR 1
		// EGT 0
		// Fluid Temp 0
		// AFR 0
		// AFR 1
		// EGT 0
		// Fluid Temp 0
		// AFR 0
		// AFR 1
		// EGT 0
		// Fluid Temp 0
		public void Write(byte[] bytes)
		{
			throw new NotSupportedException();
		}

		public int Available()
		{
			return 1;
		}

		public void Read(byte[] bytes)
		{
			if (bytes.Length != 1)
			{
				throw new ArgumentException();
			}
			if (index >= data.Length)
			{
				index = 0;
			}
			bytes[0] = data[index++];
			ThreadUtil.Sleep(10);
		}

		public byte[] ReadAvailable()
		{
			throw new NotSupportedException();
		}

		public void ReadStaleData()
		{
			throw new NotSupportedException();
		}

		public string ReadLine()
		{
			throw new NotSupportedException();
		}

		public int Read()
		{
			Read(result);
			return result[0];
		}

		public void Close()
		{
			index = 0;
		}

		public void SendBreak(int duration)
		{
		}
	}
}

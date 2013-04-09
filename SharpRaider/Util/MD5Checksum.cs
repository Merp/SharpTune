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

using System.IO;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Util
{
	public class MD5Checksum
	{
		/// <exception cref="System.Exception"></exception>
		public static byte[] CreateChecksum(string filename)
		{
			InputStream fis = new FileInputStream(filename);
			byte[] buffer = new byte[1024];
			MessageDigest complete = MessageDigest.GetInstance("MD5");
			int numRead;
			do
			{
				numRead = fis.Read(buffer);
				if (numRead > 0)
				{
					complete.Update(buffer, 0, numRead);
				}
			}
			while (numRead != -1);
			fis.Close();
			return complete.Digest();
		}

		/// <exception cref="System.Exception"></exception>
		public static string GetMD5Checksum(string filename)
		{
			byte[] b = CreateChecksum(filename);
			string result = string.Empty;
			for (int i = 0; i < b.Length; i++)
			{
				result += Sharpen.Runtime.Substring(Sharpen.Extensions.ToString((b[i] & unchecked(
					(int)(0xff))) + unchecked((int)(0x100)), 16), 1);
			}
			return result;
		}
	}
}

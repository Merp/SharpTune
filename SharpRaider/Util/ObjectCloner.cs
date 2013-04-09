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
using Sharpen;

namespace RomRaider.Util
{
	public sealed class ObjectCloner
	{
		public ObjectCloner()
		{
		}

		// returns a deep copy of an object
		/// <exception cref="System.Exception"></exception>
		public static object DeepCopy(object obj)
		{
			//obj2DeepCopy must be serializable
			ObjectOutputStream outStream = null;
			ObjectInputStream inStream = null;
			try
			{
				ByteArrayOutputStream byteOut = new ByteArrayOutputStream();
				outStream = new ObjectOutputStream(byteOut);
				outStream.WriteObject(obj);
				outStream.Flush();
				ByteArrayInputStream byteIn = new ByteArrayInputStream(byteOut.ToByteArray());
				inStream = new ObjectInputStream(byteIn);
				// read the serialized, and deep copied, object and return it
				return inStream.ReadObject();
			}
			catch (Exception e)
			{
				throw (e);
			}
			finally
			{
				//always close your streams in finally clauses
				outStream.Close();
				inStream.Close();
			}
		}
	}
}

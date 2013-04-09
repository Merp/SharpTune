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
using RomRaider.IO.J2534.Api;
using Sharpen;
using Sharpen.Reflect;

namespace RomRaider.IO.J2534.Api
{
	public sealed class J2534TransportFactory
	{
		public J2534TransportFactory()
		{
		}

		public static ConnectionManager GetManager(string protocolName, ConnectionProperties
			 connectionProperties, string library)
		{
			try
			{
				Type cls = Sharpen.Runtime.GetType(typeof(RomRaider.IO.J2534.Api.J2534TransportFactory
					).Assembly.GetName() + ".J2534Connection" + protocolName.ToUpper());
				Constructor<object> cnstrtr = cls.GetConstructor(typeof(ConnectionProperties), typeof(
					string));
				return (ConnectionManager)cnstrtr.NewInstance(connectionProperties, library);
			}
			catch (Exception e)
			{
				throw new J2534Exception("J2534 initialization: " + e.InnerException.Message, e);
			}
		}
	}
}

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

using System.Collections.Generic;
using RomRaider.Ramtune.Test.Command.Generator;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Ramtune.Test.Command.Generator
{
	public abstract class AbstractCommandGenerator : CommandGenerator
	{
		protected internal readonly RomRaider.IO.Protocol.Protocol protocol;

		public AbstractCommandGenerator(RomRaider.IO.Protocol.Protocol protocol)
		{
			ParamChecker.CheckNotNull(protocol, "protocol");
			this.protocol = protocol;
		}

		public abstract override string ToString();

		public abstract IList<byte[]> CreateCommands(byte arg1, byte[] arg2, byte[] arg3, 
			int arg4);
	}
}

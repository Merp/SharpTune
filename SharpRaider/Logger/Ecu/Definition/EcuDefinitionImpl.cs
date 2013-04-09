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

using RomRaider.Logger.Ecu.Definition;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.Ecu.Definition
{
	public sealed class EcuDefinitionImpl : EcuDefinition
	{
		private readonly string ecuId;

		private readonly string calId;

		private readonly string carString;

		public EcuDefinitionImpl(string ecuId, string calId, string carString)
		{
			ParamChecker.CheckNotNullOrEmpty(ecuId, "ecuId");
			ParamChecker.CheckNotNullOrEmpty(calId, "calId");
			ParamChecker.CheckNotNullOrEmpty(carString, "carString");
			this.ecuId = ecuId;
			this.calId = calId;
			this.carString = carString;
		}

		public string GetEcuId()
		{
			return ecuId;
		}

		public string GetCalId()
		{
			return calId;
		}

		public string GetCarString()
		{
			return carString;
		}
	}
}

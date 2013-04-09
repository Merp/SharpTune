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
	public sealed class EcuSwitchImpl : EcuSwitch
	{
		private readonly string id;

		private readonly string name;

		private readonly string description;

		private readonly EcuAddress address;

		private readonly EcuDataConvertor[] convertors;

		private int selectedConvertorIndex;

		private bool fileLogController;

		private bool selected;

		public EcuSwitchImpl(string id, string name, string description, EcuAddress address
			, EcuDataConvertor[] convertors)
		{
			ParamChecker.CheckNotNullOrEmpty(id, "id");
			ParamChecker.CheckNotNullOrEmpty(name, "name");
			ParamChecker.CheckNotNull(description, "description");
			ParamChecker.CheckNotNull(address, "address");
			ParamChecker.CheckNotNullOrEmpty(convertors, "convertors");
			this.id = id;
			this.name = name;
			this.description = description;
			this.address = address;
			this.convertors = convertors;
		}

		public string GetId()
		{
			return id;
		}

		public string GetName()
		{
			return name;
		}

		public string GetDescription()
		{
			return description;
		}

		public EcuAddress GetAddress()
		{
			return address;
		}

		public EcuDataConvertor GetSelectedConvertor()
		{
			return convertors[selectedConvertorIndex];
		}

		public EcuDataConvertor[] GetConvertors()
		{
			return convertors;
		}

		public void SelectConvertor(EcuDataConvertor convertor)
		{
			for (int i = 0; i < convertors.Length; i++)
			{
				EcuDataConvertor dataConvertor = convertors[i];
				if (convertor == dataConvertor)
				{
					selectedConvertorIndex = i;
				}
			}
		}

		public EcuDataType GetDataType()
		{
			return EcuDataType.SWITCH;
		}

		public bool IsSelected()
		{
			return selected;
		}

		public void SetSelected(bool selected)
		{
			this.selected = selected;
		}

		public void SetFileLogController(bool fileLogController)
		{
			this.fileLogController = fileLogController;
		}

		public bool IsFileLogController()
		{
			return fileLogController;
		}
	}
}

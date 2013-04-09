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
using RomRaider.Logger.Ecu.Definition;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.Ecu.Definition
{
	public sealed class EcuDerivedParameterImpl : EcuParameter
	{
		private readonly string id;

		private readonly string name;

		private readonly string description;

		private readonly EcuDerivedParameterConvertor[] convertors;

		private readonly EcuAddress address;

		private readonly ICollection<ConvertorUpdateListener> listeners = new HashSet<ConvertorUpdateListener
			>();

		private int selectedConvertorIndex;

		private bool selected;

		public EcuDerivedParameterImpl(string id, string name, string description, EcuData
			[] ecuDatas, EcuDerivedParameterConvertor[] convertors)
		{
			ParamChecker.CheckNotNullOrEmpty(id, "id");
			ParamChecker.CheckNotNullOrEmpty(name, "name");
			ParamChecker.CheckNotNull(description, "description");
			ParamChecker.CheckNotNullOrEmpty(ecuDatas, "ecuDatas");
			ParamChecker.CheckNotNullOrEmpty(convertors, "convertors");
			this.id = id;
			this.name = name;
			this.description = description;
			this.convertors = convertors;
			this.address = BuildCombinedAddress(ecuDatas);
			SetEcuDatas(ecuDatas);
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
			if (convertor != GetSelectedConvertor())
			{
				for (int i = 0; i < convertors.Length; i++)
				{
					EcuDerivedParameterConvertor parameterConvertor = convertors[i];
					if (convertor == parameterConvertor)
					{
						selectedConvertorIndex = i;
					}
				}
				NotifyUpdateListeners();
			}
		}

		public EcuDataType GetDataType()
		{
			return EcuDataType.PARAMETER;
		}

		public bool IsSelected()
		{
			return selected;
		}

		public void SetSelected(bool selected)
		{
			this.selected = selected;
		}

		public void AddConvertorUpdateListener(ConvertorUpdateListener listener)
		{
			ParamChecker.CheckNotNull(listener, "listener");
			listeners.AddItem(listener);
		}

		private EcuAddress BuildCombinedAddress(EcuData[] ecuDatas)
		{
			string[] addresses = new string[0];
			foreach (EcuData ecuData in ecuDatas)
			{
				string[] newAddresses = ecuData.GetAddress().GetAddresses();
				string[] tmp = new string[addresses.Length + newAddresses.Length];
				System.Array.Copy(addresses, 0, tmp, 0, addresses.Length);
				System.Array.Copy(newAddresses, 0, tmp, addresses.Length, newAddresses.Length);
				addresses = tmp;
			}
			return new EcuAddressImpl(addresses);
		}

		private void SetEcuDatas(EcuData[] ecuDatas)
		{
			foreach (EcuDerivedParameterConvertor convertor in convertors)
			{
				convertor.SetEcuDatas(ecuDatas);
			}
		}

		private void NotifyUpdateListeners()
		{
			foreach (ConvertorUpdateListener listener in listeners)
			{
				listener.NotifyConvertorUpdate(this);
			}
		}
	}
}

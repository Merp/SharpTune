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
using RomRaider.Logger.External.Core;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.Ecu.Definition
{
	public sealed class ExternalDataImpl : ExternalData
	{
		private readonly string id;

		private readonly string name;

		private readonly string description;

		private readonly ExternalDataSource dataSource;

		private readonly EcuDataConvertor[] convertors;

		private readonly ICollection<ConvertorUpdateListener> listeners = new HashSet<ConvertorUpdateListener
			>();

		private int selectedConvertorIndex;

		private bool selected;

		public ExternalDataImpl(ExternalDataItem dataItem, ExternalDataSource dataSource)
		{
			ParamChecker.CheckNotNull(dataItem, dataSource);
			this.dataSource = dataSource;
			this.convertors = dataItem.GetConvertors();
			id = CreateId(dataItem);
			name = dataItem.GetName();
			description = dataItem.GetDescription();
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
			return null;
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
					EcuDataConvertor dataConvertor = convertors[i];
					if (convertor == dataConvertor)
					{
						selectedConvertorIndex = i;
					}
				}
				NotifyUpdateListeners();
			}
		}

		public EcuDataType GetDataType()
		{
			return EcuDataType.EXTERNAL;
		}

		public bool IsSelected()
		{
			return selected;
		}

		public void SetSelected(bool selected)
		{
			this.selected = selected;
			UpdateConnection(selected);
		}

		private string CreateId(ExternalDataItem dataItem)
		{
			return "X_" + dataItem.GetName().ReplaceAll(" ", "_");
		}

		public void AddConvertorUpdateListener(ConvertorUpdateListener listener)
		{
			ParamChecker.CheckNotNull(listener, "listener");
			listeners.AddItem(listener);
		}

		private void NotifyUpdateListeners()
		{
			foreach (ConvertorUpdateListener listener in listeners)
			{
				listener.NotifyConvertorUpdate(this);
			}
		}

		private void UpdateConnection(bool connect)
		{
			if (connect)
			{
				dataSource.Connect();
			}
			else
			{
				dataSource.Disconnect();
			}
		}
	}
}

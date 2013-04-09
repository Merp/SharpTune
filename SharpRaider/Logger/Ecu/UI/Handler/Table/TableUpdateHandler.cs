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
using RomRaider.Logger.Ecu.Comms.Query;
using RomRaider.Logger.Ecu.Definition;
using RomRaider.Logger.Ecu.UI.Handler;
using RomRaider.Maps;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.Ecu.UI.Handler.Table
{
	public sealed class TableUpdateHandler : DataUpdateHandler
	{
		private static readonly RomRaider.Logger.Ecu.UI.Handler.Table.TableUpdateHandler 
			INSTANCE = new RomRaider.Logger.Ecu.UI.Handler.Table.TableUpdateHandler();

		private readonly IDictionary<string, IList<RomRaider.Maps.Table>> tableMap = Sharpen.Collections.SynchronizedMap
			(new Dictionary<string, IList<RomRaider.Maps.Table>>());

		public TableUpdateHandler()
		{
		}

		public void RegisterData(LoggerData loggerData)
		{
		}

		public void HandleDataUpdate(Response response)
		{
			foreach (LoggerData loggerData in response.GetData())
			{
				IList<RomRaider.Maps.Table> tables = tableMap.Get(loggerData.GetId());
				if (tables != null && !tables.IsEmpty())
				{
					string formattedValue = loggerData.GetSelectedConvertor().Format(response.GetDataValue
						(loggerData));
					foreach (RomRaider.Maps.Table table in tables)
					{
						table.SetLiveValue(formattedValue);
					}
				}
			}
		}

		public void DeregisterData(LoggerData loggerData)
		{
		}

		public void CleanUp()
		{
		}

		public void Reset()
		{
		}

		public void RegisterTable(RomRaider.Maps.Table table)
		{
			string logParam = table.GetLogParam();
			if (!ParamChecker.IsNullOrEmpty(logParam))
			{
				if (!tableMap.ContainsKey(logParam))
				{
					tableMap.Put(logParam, new AList<RomRaider.Maps.Table>());
				}
				IList<RomRaider.Maps.Table> tables = tableMap.Get(logParam);
				if (!tables.Contains(table))
				{
					tables.AddItem(table);
				}
			}
			RegisterAxes(table);
		}

		public void DeregisterTable(RomRaider.Maps.Table table)
		{
			string logParam = table.GetLogParam();
			if (tableMap.ContainsKey(logParam))
			{
				IList<RomRaider.Maps.Table> tables = tableMap.Get(logParam);
				tables.Remove(table);
				if (tables.IsEmpty())
				{
					Sharpen.Collections.Remove(tableMap, logParam);
				}
			}
			DeregisterAxes(table);
		}

		public static RomRaider.Logger.Ecu.UI.Handler.Table.TableUpdateHandler GetInstance
			()
		{
			return INSTANCE;
		}

		private void RegisterAxes(RomRaider.Maps.Table table)
		{
			if (table is Table2D)
			{
				RegisterTable(((Table2D)table).GetAxis());
			}
			if (table is Table3D)
			{
				RegisterTable(((Table3D)table).GetXAxis());
				RegisterTable(((Table3D)table).GetYAxis());
			}
		}

		private void DeregisterAxes(RomRaider.Maps.Table table)
		{
			if (table is Table2D)
			{
				DeregisterTable(((Table2D)table).GetAxis());
			}
			if (table is Table3D)
			{
				DeregisterTable(((Table3D)table).GetXAxis());
				DeregisterTable(((Table3D)table).GetYAxis());
			}
		}
	}
}

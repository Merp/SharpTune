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
using Javax.Swing.Table;
using RomRaider.Logger.Ecu.Definition;
using RomRaider.Logger.Ecu.UI.Handler.Livedata;
using Sharpen;

namespace RomRaider.Logger.Ecu.UI.Handler.Livedata
{
	[System.Serializable]
	public sealed class LiveDataTableModel : AbstractTableModel
	{
		private const long serialVersionUID = 3712433453224086342L;

		private readonly string[] columnNames = new string[] { "Logger Data", "Min Value"
			, "Current Value", "Max Value", "Units" };

		private readonly IList<LoggerData> registeredLoggerData = Sharpen.Collections.SynchronizedList
			(new List<LoggerData>());

		private readonly IDictionary<LoggerData, LiveDataRow> dataRowMap = Sharpen.Collections.SynchronizedMap
			(new LinkedHashMap<LoggerData, LiveDataRow>());

		public override int GetRowCount()
		{
			lock (this)
			{
				return dataRowMap.Count;
			}
		}

		public override int GetColumnCount()
		{
			return columnNames.Length;
		}

		public override string GetColumnName(int col)
		{
			return columnNames[col];
		}

		public override bool IsCellEditable(int row, int col)
		{
			return false;
		}

		public override object GetValueAt(int row, int col)
		{
			lock (this)
			{
				LiveDataRow dataRow = dataRowMap.Get(registeredLoggerData[row]);
				switch (col)
				{
					case 0:
					{
						return dataRow.GetName();
					}

					case 1:
					{
						return dataRow.GetMinValue();
					}

					case 2:
					{
						return dataRow.GetCurrentValue();
					}

					case 3:
					{
						return dataRow.GetMaxValue();
					}

					case 4:
					{
						return dataRow.GetUnits();
					}

					default:
					{
						return "Error!";
						break;
					}
				}
			}
		}

		public void AddParam(LoggerData loggerData)
		{
			lock (this)
			{
				if (!registeredLoggerData.Contains(loggerData))
				{
					dataRowMap.Put(loggerData, new LiveDataRow(loggerData));
					registeredLoggerData.AddItem(loggerData);
					FireTableDataChanged();
				}
			}
		}

		public void RemoveParam(LoggerData loggerData)
		{
			lock (this)
			{
				registeredLoggerData.Remove(loggerData);
				Sharpen.Collections.Remove(dataRowMap, loggerData);
				FireTableDataChanged();
			}
		}

		public void UpdateParam(LoggerData loggerData, double value)
		{
			lock (this)
			{
				LiveDataRow dataRow = dataRowMap.Get(loggerData);
				if (dataRow != null)
				{
					dataRow.UpdateValue(value);
					int index = registeredLoggerData.IndexOf(loggerData);
					FireTableRowsUpdated(index, index);
				}
			}
		}

		public void Reset()
		{
			lock (this)
			{
				foreach (LiveDataRow liveDataRow in dataRowMap.Values)
				{
					liveDataRow.Reset();
				}
				FireTableDataChanged();
			}
		}

		public void ResetRow(LoggerData loggerData)
		{
			lock (this)
			{
				LiveDataRow liveDataRow = dataRowMap.Get(loggerData);
				if (liveDataRow != null)
				{
					liveDataRow.Reset();
					FireTableDataChanged();
				}
			}
		}
	}
}

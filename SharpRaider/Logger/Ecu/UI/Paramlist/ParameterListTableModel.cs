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
using System.Collections.Generic;
using Javax.Swing.Table;
using RomRaider.Logger.Ecu.Definition;
using RomRaider.Logger.Ecu.UI;
using RomRaider.Logger.Ecu.UI.Paramlist;
using Sharpen;

namespace RomRaider.Logger.Ecu.UI.Paramlist
{
	[System.Serializable]
	public sealed class ParameterListTableModel : AbstractTableModel
	{
		private const long serialVersionUID = -2556400867696538881L;

		private readonly string[] columnNames;

		private readonly IList<LoggerData> registeredLoggerData = Sharpen.Collections.SynchronizedList
			(new List<LoggerData>());

		private readonly IDictionary<LoggerData, ParameterRow> paramRowMap = Sharpen.Collections.SynchronizedMap
			(new LinkedHashMap<LoggerData, ParameterRow>());

		private readonly DataRegistrationBroker broker;

		public ParameterListTableModel(DataRegistrationBroker broker, string dataType)
		{
			this.broker = broker;
			columnNames = new string[] { "Selected?", dataType, "Units" };
		}

		public override int GetRowCount()
		{
			lock (this)
			{
				return paramRowMap.Count;
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
			return col == 0 || col == 2;
		}

		public override object GetValueAt(int row, int col)
		{
			lock (this)
			{
				ParameterRow paramRow = paramRowMap.Get(registeredLoggerData[row]);
				switch (col)
				{
					case 0:
					{
						return paramRow.IsSelected();
					}

					case 1:
					{
						return paramRow.GetLoggerData().GetName();
					}

					case 2:
					{
						LoggerData loggerData = paramRow.GetLoggerData();
						return loggerData.GetConvertors().Length > 1 ? loggerData : loggerData.GetSelectedConvertor
							().GetUnits();
					}

					default:
					{
						return "Error!";
						break;
					}
				}
			}
		}

		public override void SetValueAt(object value, int row, int col)
		{
			lock (this)
			{
				ParameterRow paramRow = paramRowMap.Get(registeredLoggerData[row]);
				if (col == 0 && paramRow != null)
				{
					bool selected = (bool)value;
					SetSelected(paramRow, selected);
					FireTableRowsUpdated(row, row);
				}
			}
		}

		public override Type GetColumnClass(int col)
		{
			return GetValueAt(0, col).GetType();
		}

		public void AddParam(LoggerData loggerData, bool selected)
		{
			lock (this)
			{
				if (!registeredLoggerData.Contains(loggerData))
				{
					ParameterRow paramRow = new ParameterRow(loggerData);
					paramRowMap.Put(loggerData, paramRow);
					registeredLoggerData.AddItem(loggerData);
					SetSelected(paramRow, selected);
					FireTableDataChanged();
				}
			}
		}

		public void SelectParam(LoggerData loggerData, bool selected)
		{
			lock (this)
			{
				if (registeredLoggerData.Contains(loggerData))
				{
					SetSelected(paramRowMap.Get(loggerData), selected);
					FireTableDataChanged();
				}
			}
		}

		public void Clear()
		{
			lock (this)
			{
				broker.Clear();
				paramRowMap.Clear();
				registeredLoggerData.Clear();
				try
				{
					FireTableDataChanged();
				}
				catch (Exception)
				{
				}
			}
		}

		// Swallow complaints from TableRowSorter when the table is empty
		public IList<ParameterRow> GetParameterRows()
		{
			return new AList<ParameterRow>(paramRowMap.Values);
		}

		private void SetSelected(ParameterRow paramRow, bool selected)
		{
			paramRow.SetSelected(selected);
			if (selected)
			{
				broker.RegisterLoggerDataForLogging(paramRow.GetLoggerData());
			}
			else
			{
				broker.DeregisterLoggerDataFromLogging(paramRow.GetLoggerData());
			}
		}
	}
}

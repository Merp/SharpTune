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
using Java.Awt.Event;
using Javax.Swing;
using Javax.Swing.Table;
using RomRaider.Logger.Ecu.Definition;
using RomRaider.Logger.Ecu.UI.Paramlist;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.Ecu.UI.Paramlist
{
	[System.Serializable]
	public sealed class ParameterListTable : JTable
	{
		private const long serialVersionUID = -8489190548281346227L;

		private UnitsComboBoxEditor comboBoxEditor = new UnitsComboBoxEditor();

		private UnitsComboBoxRenderer comboBoxRenderer = new UnitsComboBoxRenderer();

		private readonly ParameterListTableModel tableModel;

		public ParameterListTable(ParameterListTableModel tableModel) : base(tableModel)
		{
			this.tableModel = tableModel;
			this.GetTableHeader().SetReorderingAllowed(false);
			for (int column = 0; column < tableModel.GetColumnCount(); column++)
			{
				if (Sharpen.Runtime.EqualsIgnoreCase(tableModel.GetColumnName(2), "units"))
				{
					SetColumnSortable(column, false);
				}
				else
				{
					SetColumnSortable(column, true);
				}
			}
		}

		public override TableCellRenderer GetCellRenderer(int row, int col)
		{
			return DisplayComboBox(row, col) ? comboBoxRenderer : base.GetCellRenderer(row, col
				);
		}

		public override TableCellEditor GetCellEditor(int row, int col)
		{
			return DisplayComboBox(row, col) ? comboBoxEditor : base.GetCellEditor(row, col);
		}

		public override string GetToolTipText(MouseEvent mouseEvent)
		{
			IList<ParameterRow> parameterRows = tableModel.GetParameterRows();
			if (!ParamChecker.IsNullOrEmpty(parameterRows))
			{
				ParameterRow parameterRow = parameterRows[RowAtPoint(mouseEvent.GetPoint())];
				if (parameterRow != null)
				{
					string description = parameterRow.GetLoggerData().GetDescription();
					if (!ParamChecker.IsNullOrEmpty(description))
					{
						return description;
					}
				}
			}
			return base.GetToolTipText(mouseEvent);
		}

		private bool DisplayComboBox(int row, int col)
		{
			object value = GetValueAt(row, col);
			if (typeof(EcuData).IsAssignableFrom(value.GetType()))
			{
				EcuData ecuData = (EcuData)value;
				if (ecuData.GetConvertors().Length > 1)
				{
					return true;
				}
			}
			if (typeof(ExternalData).IsAssignableFrom(value.GetType()))
			{
				ExternalData externalData = (ExternalData)value;
				if (externalData.GetConvertors().Length > 1)
				{
					return true;
				}
			}
			return false;
		}

		private void SetColumnSortable(int column, bool state)
		{
			TableRowSorter<ParameterListTableModel> sorter = new TableRowSorter<ParameterListTableModel
				>(tableModel);
			sorter.SetSortable(column, state);
			SetRowSorter(sorter);
		}
	}
}

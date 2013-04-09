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

using Java.Awt;
using Java.Awt.Event;
using Javax.Swing;
using Javax.Swing.Table;
using RomRaider.Logger.Ecu.Definition;
using RomRaider.Logger.Ecu.UI.Paramlist;
using Sharpen;

namespace RomRaider.Logger.Ecu.UI.Paramlist
{
	[System.Serializable]
	public sealed class UnitsComboBoxEditor : AbstractCellEditor, TableCellEditor, ActionListener
	{
		private const long serialVersionUID = -3472910399604360821L;

		private static readonly string EDIT_COMMAND = "EDIT";

		private LoggerData currentEcuData;

		public override object GetCellEditorValue()
		{
			return currentEcuData.GetSelectedConvertor();
		}

		public Component GetTableCellEditorComponent(JTable table, object ecuData, bool isSelected
			, int row, int column)
		{
			currentEcuData = (LoggerData)ecuData;
			EcuDataConvertor[] convertors = currentEcuData.GetConvertors();
			JComboBox comboBox = new JComboBox();
			foreach (EcuDataConvertor convertor in convertors)
			{
				comboBox.AddItem(convertor);
			}
			comboBox.SetSelectedItem(currentEcuData.GetSelectedConvertor());
			comboBox.SetEditable(false);
			comboBox.SetEnabled(true);
			comboBox.SetActionCommand(EDIT_COMMAND);
			comboBox.AddActionListener(this);
			return comboBox;
		}

		public void ActionPerformed(ActionEvent actionEvent)
		{
			if (EDIT_COMMAND.Equals(actionEvent.GetActionCommand()))
			{
				object source = actionEvent.GetSource();
				if (source != null && typeof(JComboBox).IsAssignableFrom(source.GetType()))
				{
					JComboBox comboBox = (JComboBox)source;
					currentEcuData.SelectConvertor((EcuDataConvertor)comboBox.GetSelectedItem());
					FireEditingStopped();
				}
			}
		}
	}
}

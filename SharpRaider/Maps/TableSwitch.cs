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
using Java.Awt;
using Javax.Swing;
using RomRaider.Editor.Ecu;
using RomRaider.Maps;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Maps
{
	[System.Serializable]
	public class TableSwitch : Table
	{
		private const long serialVersionUID = -4887718305447362308L;

		private readonly ButtonGroup buttonGroup = new ButtonGroup();

		private readonly IDictionary<string, byte[]> switchStates = new Dictionary<string
			, byte[]>();

		private int dataSize = 0;

		public TableSwitch(ECUEditor editor) : base(editor)
		{
			storageType = 1;
			type = TABLE_SWITCH;
			locked = true;
			RemoveAll();
			SetLayout(new BorderLayout());
		}

		public override void SetDataSize(int size)
		{
			if (dataSize == 0)
			{
				dataSize = size;
			}
		}

		public override int GetDataSize()
		{
			return dataSize;
		}

		public override void PopulateTable(byte[] input)
		{
			JPanel radioPanel = new JPanel(new GridLayout(0, 1));
			radioPanel.Add(new JLabel("  " + name));
			foreach (string stateName in switchStates.Keys)
			{
				JRadioButton button = new JRadioButton(stateName);
				buttonGroup.Add(button);
				radioPanel.Add(button);
			}
			Add(radioPanel, BorderLayout.CENTER);
			// Validate the ROM image checksums.
			// if the result is >0: position of failed checksum
			// if the result is  0: all the checksums matched
			// if the result is -1: all the checksums have been previously disabled
			if (Sharpen.Runtime.EqualsIgnoreCase(base.GetName(), "Checksum Fix"))
			{
				int result = RomChecksum.ValidateRomChecksum(input, storageAddress, dataSize);
				string message = string.Format("Checksum No. %d is invalid.%n" + "The ROM image may be corrupt.%n"
					 + "Use of this ROM image is not advised!", result);
				if (result > 0)
				{
					JOptionPane.ShowMessageDialog(this, message, "ERROR - Checksums Failed", JOptionPane
						.WARNING_MESSAGE);
					SetButtonsUnselected(buttonGroup);
				}
				else
				{
					if (result == -1)
					{
						message = "All Checksums are disabled.";
						JOptionPane.ShowMessageDialog(this, message, "Warning - Checksum Status", JOptionPane
							.INFORMATION_MESSAGE);
						GetButtonByText(buttonGroup, "on").SetSelected(true);
					}
					else
					{
						GetButtonByText(buttonGroup, "off").SetSelected(true);
						locked = false;
					}
				}
				return;
			}
			// Validate XML switch definition data against the ROM data to select
			// the appropriate switch setting or throw an error if there is a
			// mismatch and disable this table's editing ability.
			if (!beforeRam)
			{
				ramOffset = container.GetRomID().GetRamOffset();
			}
			IDictionary<string, int> sourceStatus = new Dictionary<string, int>();
			foreach (string stateName_1 in switchStates.Keys)
			{
				byte[] sourceData = new byte[dataSize];
				System.Array.Copy(input, storageAddress - ramOffset, sourceData, 0, dataSize);
				int compareResult = ByteUtil.IndexOfBytes(sourceData, GetValues(stateName_1));
				if (compareResult == -1)
				{
					GetButtonByText(buttonGroup, stateName_1).SetSelected(false);
				}
				else
				{
					GetButtonByText(buttonGroup, stateName_1).SetSelected(true);
				}
				sourceStatus.Put(stateName_1, compareResult);
			}
			foreach (string source in sourceStatus.Keys)
			{
				if (sourceStatus.Get(source) != -1)
				{
					locked = false;
					break;
				}
			}
			if (locked)
			{
				string mismatch = string.Format("Table: %s%nTable editing has been disabled.%nDefinition file or ROM image may be corrupt."
					, base.GetName());
				JOptionPane.ShowMessageDialog(this, mismatch, "ERROR - Data Mismatch", JOptionPane
					.ERROR_MESSAGE);
				SetButtonsUnselected(buttonGroup);
			}
		}

		public override void SetName(string name)
		{
			base.SetName(name);
		}

		public override int GetType()
		{
			return TABLE_SWITCH;
		}

		public override void SetDescription(string description)
		{
			base.SetDescription(description);
			JTextArea descriptionArea = new JTextArea(description);
			descriptionArea.SetOpaque(false);
			descriptionArea.SetEditable(false);
			descriptionArea.SetWrapStyleWord(true);
			descriptionArea.SetLineWrap(true);
			descriptionArea.SetMargin(new Insets(0, 5, 5, 5));
			Add(descriptionArea, BorderLayout.SOUTH);
		}

		public override byte[] SaveFile(byte[] input)
		{
			if (!Sharpen.Runtime.EqualsIgnoreCase(base.GetName(), "Checksum Fix"))
			{
				if (!locked)
				{
					JRadioButton selectedButton = GetSelectedButton(buttonGroup);
					System.Array.Copy(switchStates.Get(selectedButton.GetText()), 0, input, storageAddress
						 - ramOffset, dataSize);
				}
			}
			return input;
		}

		public virtual void SetValues(string name, string input)
		{
			switchStates.Put(name, HexUtil.AsBytes(input));
		}

		public virtual byte[] GetValues(string key)
		{
			return switchStates.Get(key);
		}

		public override Dimension GetFrameSize()
		{
			int height = verticalOverhead + 75;
			int width = horizontalOverhead;
			if (height < minHeight)
			{
				height = minHeight;
			}
			int minWidth = IsLiveDataSupported() ? minWidthOverlay : minWidthNoOverlay;
			if (width < minWidth)
			{
				width = minWidth;
			}
			return new Dimension(width, height);
		}

		public override void Colorize()
		{
		}

		public override void CursorUp()
		{
		}

		public override void CursorDown()
		{
		}

		public override void CursorLeft()
		{
		}

		public override void CursorRight()
		{
		}

		public override void SetAxisColor(Color color)
		{
		}

		public override bool IsLiveDataSupported()
		{
			return false;
		}

		public override bool IsButtonSelected()
		{
			if (buttonGroup.GetSelection() == null)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		// returns the selected radio button in the specified group
		private static JRadioButton GetSelectedButton(ButtonGroup group)
		{
			for (Enumeration<AbstractButton> e = group.GetElements(); e.MoveNext(); )
			{
				JRadioButton b = (JRadioButton)e.Current;
				if (b.GetModel() == group.GetSelection())
				{
					return b;
				}
			}
			return null;
		}

		// Unselects & disables all radio buttons in the specified group
		private static void SetButtonsUnselected(ButtonGroup group)
		{
			for (Enumeration<AbstractButton> e = group.GetElements(); e.MoveNext(); )
			{
				JRadioButton b = (JRadioButton)e.Current;
				b.SetSelected(false);
				b.SetEnabled(false);
			}
		}

		// returns the radio button based on its display text
		private static JRadioButton GetButtonByText(ButtonGroup group, string text)
		{
			for (Enumeration<AbstractButton> e = group.GetElements(); e.MoveNext(); )
			{
				JRadioButton b = (JRadioButton)e.Current;
				if (Sharpen.Runtime.EqualsIgnoreCase(b.GetText(), text))
				{
					return b;
				}
			}
			return null;
		}
	}
}

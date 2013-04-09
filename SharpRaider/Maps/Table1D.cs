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

using System.Text;
using Java.Awt;
using Javax.Swing;
using RomRaider.Editor.Ecu;
using RomRaider.Maps;
using Sharpen;

namespace RomRaider.Maps
{
	[System.Serializable]
	public class Table1D : Table
	{
		private const long serialVersionUID = -8747180767803835631L;

		private Color axisColor = new Color(255, 255, 255);

		public Table1D(ECUEditor editor) : base(editor)
		{
		}

		public override void PopulateTable(byte[] input)
		{
			centerLayout.SetRows(1);
			centerLayout.SetColumns(this.GetDataSize());
			base.PopulateTable(input);
			// add to table
			for (int i = 0; i < this.GetDataSize(); i++)
			{
				centerPanel.Add(this.GetDataCell(i));
			}
			Add(new JLabel(name + " (" + scales[scaleIndex].GetUnit() + ")", JLabel.CENTER), 
				BorderLayout.NORTH);
		}

		public override string ToString()
		{
			return base.ToString() + " (1D)";
		}

		public virtual bool IsIsAxis()
		{
			return isAxis;
		}

		public virtual void SetIsAxis(bool isAxis)
		{
			this.isAxis = isAxis;
		}

		public override void ClearSelection()
		{
			base.ClearSelection();
		}

		//if (isAxis) axisParent.clearSelection();
		public virtual void ClearSelection(bool calledByParent)
		{
			if (calledByParent)
			{
				base.ClearSelection();
			}
			else
			{
				this.ClearSelection();
			}
		}

		public override void CursorUp()
		{
			if (type == Table.TABLE_Y_AXIS)
			{
				if (highlightY > 0 && data[highlightY].IsSelected())
				{
					SelectCellAt(highlightY - 1);
				}
			}
			else
			{
				if (type == Table.TABLE_X_AXIS)
				{
				}
				else
				{
					// Y axis is on top.. nothing happens
					if (type == Table.TABLE_1D)
					{
					}
				}
			}
		}

		// no where to move up to
		public override void CursorDown()
		{
			if (type == Table.TABLE_Y_AXIS)
			{
				if (axisParent.GetType() == Table.TABLE_3D)
				{
					if (highlightY < GetDataSize() - 1 && data[highlightY].IsSelected())
					{
						SelectCellAt(highlightY + 1);
					}
				}
				else
				{
					if (axisParent.GetType() == Table.TABLE_2D)
					{
						if (data[highlightY].IsSelected())
						{
							axisParent.SelectCellAt(highlightY);
						}
					}
				}
			}
			else
			{
				if (type == Table.TABLE_X_AXIS && data[highlightY].IsSelected())
				{
					((Table3D)axisParent).SelectCellAt(highlightY, this);
				}
				else
				{
					if (type == Table.TABLE_1D)
					{
					}
				}
			}
		}

		// no where to move down to
		public override void CursorLeft()
		{
			if (type == Table.TABLE_Y_AXIS)
			{
				// X axis is on left.. nothing happens
				if (axisParent.GetType() == Table.TABLE_2D)
				{
					if (data[highlightY].IsSelected())
					{
						SelectCellAt(highlightY - 1);
					}
				}
			}
			else
			{
				if (type == Table.TABLE_X_AXIS && data[highlightY].IsSelected())
				{
					if (highlightY > 0)
					{
						SelectCellAt(highlightY - 1);
					}
				}
				else
				{
					if (type == Table.TABLE_1D && data[highlightY].IsSelected())
					{
						if (highlightY > 0)
						{
							SelectCellAt(highlightY - 1);
						}
					}
				}
			}
		}

		public override void CursorRight()
		{
			if (type == Table.TABLE_Y_AXIS && data[highlightY].IsSelected())
			{
				if (axisParent.GetType() == Table.TABLE_3D)
				{
					((Table3D)axisParent).SelectCellAt(highlightY, this);
				}
				else
				{
					if (axisParent.GetType() == Table.TABLE_2D)
					{
						SelectCellAt(highlightY + 1);
					}
				}
			}
			else
			{
				if (type == Table.TABLE_X_AXIS && data[highlightY].IsSelected())
				{
					if (highlightY < GetDataSize() - 1)
					{
						SelectCellAt(highlightY + 1);
					}
				}
				else
				{
					if (type == Table.TABLE_1D && data[highlightY].IsSelected())
					{
						if (highlightY < GetDataSize() - 1)
						{
							SelectCellAt(highlightY + 1);
						}
					}
				}
			}
		}

		public override void StartHighlight(int x, int y)
		{
			if (isAxis)
			{
				axisParent.ClearSelection();
			}
			base.StartHighlight(x, y);
		}

		public override StringBuilder GetTableAsString()
		{
			StringBuilder output = new StringBuilder(string.Empty);
			for (int i = 0; i < GetDataSize(); i++)
			{
				output.Append(data[i].GetText());
				if (i < GetDataSize() - 1)
				{
					output.Append(TAB);
				}
			}
			return output;
		}

		public override string GetCellAsString(int index)
		{
			return data[index].GetText();
		}

		public virtual Color GetAxisColor()
		{
			return axisColor;
		}

		public override void SetAxisColor(Color axisColor)
		{
			this.axisColor = axisColor;
		}

		public override void SetLiveValue(string value)
		{
			liveValue = value;
			Table parent = GetAxisParent();
			if (parent != null)
			{
				parent.HighlightLiveData();
			}
		}

		public override bool IsLiveDataSupported()
		{
			return false;
		}

		public override bool IsButtonSelected()
		{
			return true;
		}
	}
}

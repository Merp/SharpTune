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
using Java.Awt;
using Java.Awt.Event;
using Javax.Swing;
using Org.Apache.Log4j;
using RomRaider.Maps;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Maps
{
	[System.Serializable]
	public class DataCell : JLabel, MouseListener
	{
		private const long serialVersionUID = -2904293227148940937L;

		private static readonly Logger LOGGER = Logger.GetLogger(typeof(RomRaider.Maps.DataCell
			));

		private static readonly DecimalFormat PERCENT_FORMAT = new DecimalFormat("#,##0.0%"
			);

		private readonly Javax.Swing.Border.Border defaultBorder = BorderFactory.CreateLineBorder
			(Color.BLACK, 1);

		private readonly Javax.Swing.Border.Border modifiedBorder = BorderFactory.CreateLineBorder
			(Color.RED, 3);

		private readonly Font defaultFont = new Font("Arial", Font.BOLD, 12);

		private double binValue = 0;

		private double originalValue = 0;

		private Scale scale = new Scale();

		private string displayValue = string.Empty;

		private Color scaledColor = new Color(0, 0, 0);

		private Color highlightColor = new Color(155, 155, 255);

		private Color increaseBorder = Color.RED;

		private Color decreaseBorder = Color.BLUE;

		private bool selected = false;

		private bool highlighted = false;

		private Table table;

		private int x = 0;

		private int y = 0;

		private double compareValue = 0;

		private int compareDisplay = Table.COMPARE_DISPLAY_OFF;

		public DataCell()
		{
		}

		public DataCell(Scale scale, Dimension size)
		{
			this.scale = scale;
			this.SetHorizontalAlignment(CENTER);
			this.SetVerticalAlignment(CENTER);
			this.SetFont(defaultFont);
			this.SetBorder(defaultBorder);
			this.SetOpaque(true);
			this.SetVisible(true);
			this.AddMouseListener(this);
			this.SetPreferredSize(size);
		}

		public virtual void UpdateDisplayValue()
		{
			DecimalFormat formatter = new DecimalFormat(scale.GetFormat());
			if (GetCompareDisplay() == Table.COMPARE_DISPLAY_OFF)
			{
				displayValue = GetRealValue();
			}
			else
			{
				if (GetCompareDisplay() == Table.COMPARE_DISPLAY_ABSOLUTE)
				{
					displayValue = formatter.Format(CalcDisplayValue(binValue, table.GetScale().GetExpression
						()) - CalcDisplayValue(compareValue, table.GetScale().GetExpression()));
				}
				else
				{
					if (GetCompareDisplay() == Table.COMPARE_DISPLAY_PERCENT)
					{
						string expression = table.GetScale().GetExpression();
						double thisValue = CalcDisplayValue(binValue, expression);
						double thatValue = CalcDisplayValue(compareValue, expression);
						double difference = thisValue - thatValue;
						if (difference == 0)
						{
							displayValue = PERCENT_FORMAT.Format(0.0);
						}
						else
						{
							if (thatValue == 0.0)
							{
								displayValue = '\u221e' + "%";
							}
							else
							{
								double d = difference / Math.Abs(thatValue);
								displayValue = PERCENT_FORMAT.Format(d);
							}
						}
					}
				}
			}
			SetText(displayValue);
		}

		public virtual double CalcDisplayValue(double input, string expression)
		{
			return JEPUtil.Evaluate(expression, input);
		}

		public virtual void SetColor(Color color)
		{
			scaledColor = color;
			if (!selected)
			{
				base.SetBackground(color);
			}
		}

		public virtual void SetDisplayValue(string displayValue)
		{
			this.displayValue = displayValue;
			this.SetText(displayValue);
		}

		public virtual void SetBinValue(double binValue)
		{
			this.binValue = binValue;
			// make sure it's in range
			if (table.GetStorageType() != Table.STORAGE_TYPE_FLOAT)
			{
				if (table.IsSignedData())
				{
					int minAllowedValue = 0;
					int maxAllowedValue = 0;
					switch (table.GetStorageType())
					{
						case 1:
						{
							minAllowedValue = byte.MinValue;
							maxAllowedValue = byte.MaxValue;
							break;
						}

						case 2:
						{
							minAllowedValue = short.MinValue;
							maxAllowedValue = short.MaxValue;
							break;
						}

						case 4:
						{
							minAllowedValue = int.MinValue;
							maxAllowedValue = int.MaxValue;
							break;
						}
					}
					if (binValue < minAllowedValue)
					{
						this.SetBinValue(minAllowedValue);
					}
					else
					{
						if (binValue > maxAllowedValue)
						{
							this.SetBinValue(maxAllowedValue);
						}
					}
				}
				else
				{
					if (binValue < 0)
					{
						this.SetBinValue(0);
					}
					else
					{
						if (binValue > Math.Pow(256, table.GetStorageType()) - 1)
						{
							this.SetBinValue((int)(Math.Pow(256, table.GetStorageType()) - 1));
						}
					}
				}
			}
			this.UpdateDisplayValue();
			table.RefreshCompares();
		}

		public virtual double GetBinValue()
		{
			return binValue;
		}

		public override string ToString()
		{
			return displayValue;
		}

		public virtual bool IsSelected()
		{
			return selected;
		}

		public virtual void SetSelected(bool selected)
		{
			this.selected = selected;
			if (selected)
			{
				this.SetBackground(GetHighlightColor());
				table.GetFrame().GetToolBar().SetFineValue(Math.Abs(table.GetScale().GetFineIncrement
					()));
				table.GetFrame().GetToolBar().SetCoarseValue(Math.Abs(table.GetScale().GetCoarseIncrement
					()));
			}
			else
			{
				this.SetBackground(scaledColor);
			}
		}

		//TODO Uncomment if needed after further testing
		//Removed to test with 3d graph
		//requestFocus();
		public virtual void SetHighlighted(bool highlighted)
		{
			if (!table.IsStatic())
			{
				this.highlighted = highlighted;
				if (highlighted)
				{
					this.SetBackground(GetHighlightColor());
				}
				else
				{
					if (!selected)
					{
						this.SetBackground(scaledColor);
					}
				}
			}
		}

		public virtual bool IsHighlighted()
		{
			return highlighted;
		}

		public virtual void MouseEntered(MouseEvent e)
		{
			table.Highlight(x, y);
		}

		public virtual void MousePressed(MouseEvent e)
		{
			if (!table.IsStatic())
			{
				if (!e.IsControlDown())
				{
					table.ClearSelection();
				}
				table.StartHighlight(x, y);
			}
			RequestFocus();
		}

		public virtual void MouseReleased(MouseEvent e)
		{
			if (!table.IsStatic())
			{
				table.StopHighlight();
			}
		}

		public virtual void MouseClicked(MouseEvent e)
		{
		}

		public virtual void MouseExited(MouseEvent e)
		{
		}

		public virtual void Increment(double increment)
		{
			UpdateDisplayValue();
			double oldValue = double.ParseDouble(GetText());
			if (table.GetScale().GetCoarseIncrement() < 0)
			{
				increment = 0 - increment;
			}
			SetRealValue((CalcDisplayValue(binValue, scale.GetExpression()) + increment).ToString
				());
			// make sure table is incremented if change isn't great enough
			int maxValue = (int)Math.Pow(8, table.GetStorageType());
			if (table.GetStorageType() != Table.STORAGE_TYPE_FLOAT && oldValue == double.ParseDouble
				(GetText()) && binValue > 0 && binValue < maxValue)
			{
				LOGGER.Debug(maxValue + " " + binValue);
				Increment(increment * 2);
			}
		}

		public virtual void SetTable(Table table)
		{
			this.table = table;
		}

		public virtual void SetXCoord(int x)
		{
			this.x = x;
		}

		public virtual void SetYCoord(int y)
		{
			this.y = y;
		}

		public virtual double GetOriginalValue()
		{
			return originalValue;
		}

		public virtual void SetOriginalValue(double originalValue)
		{
			this.originalValue = originalValue;
			if (binValue != GetOriginalValue())
			{
				this.SetBorder(modifiedBorder);
			}
			else
			{
				this.SetBorder(defaultBorder);
			}
		}

		public virtual void Undo()
		{
			if (this.GetBinValue() != originalValue)
			{
				this.SetBinValue(originalValue);
			}
		}

		public virtual void SetRevertPoint()
		{
			this.SetOriginalValue(binValue);
		}

		public virtual double GetValue()
		{
			return CalcDisplayValue(binValue, table.GetScale().GetExpression());
		}

		public virtual string GetRealValue()
		{
			return new DecimalFormat(scale.GetFormat()).Format(GetValue());
		}

		public virtual void SetRealValue(string input)
		{
			// create parser
			try
			{
				if (!Sharpen.Runtime.EqualsIgnoreCase("x", input))
				{
					double result = JEPUtil.Evaluate(table.GetScale().GetByteExpression(), double.ParseDouble
						(input));
					if (table.GetStorageType() == Table.STORAGE_TYPE_FLOAT)
					{
						if (this.GetBinValue() != result)
						{
							this.SetBinValue(result);
						}
					}
					else
					{
						int roundResult = (int)Math.Round(result);
						if (this.GetBinValue() != roundResult)
						{
							this.SetBinValue(roundResult);
						}
					}
				}
			}
			catch (FormatException)
			{
			}
		}

		// Do nothing.  input is null or not a valid number.
		public virtual Color GetHighlightColor()
		{
			return highlightColor;
		}

		public virtual void SetHighlightColor(Color highlightColor)
		{
			this.highlightColor = highlightColor;
		}

		public virtual Color GetIncreaseBorder()
		{
			return increaseBorder;
		}

		public virtual void SetIncreaseBorder(Color increaseBorder)
		{
			this.increaseBorder = increaseBorder;
		}

		public virtual Color GetDecreaseBorder()
		{
			return decreaseBorder;
		}

		public virtual void SetDecreaseBorder(Color decreaseBorder)
		{
			this.decreaseBorder = decreaseBorder;
		}

		public virtual double GetCompareValue()
		{
			return compareValue;
		}

		public virtual void SetCompareValue(double compareValue)
		{
			this.compareValue = compareValue;
		}

		public virtual int GetCompareDisplay()
		{
			return compareDisplay;
		}

		public virtual void SetCompareDisplay(int compareDisplay)
		{
			this.compareDisplay = compareDisplay;
		}

		public virtual void RefreshValue()
		{
			SetBinValue(binValue);
		}

		public virtual void Multiply(double factor)
		{
			UpdateDisplayValue();
			SetRealValue((double.ParseDouble(GetText()) * factor).ToString());
		}

		public virtual void SetLiveDataTrace(bool trace)
		{
			if (trace)
			{
				//setBorder(liveDataTraceBorder);
				SetForeground(Color.RED);
			}
			else
			{
				//setBorder(defaultBorder);
				SetForeground(Color.BLACK);
			}
		}
	}
}

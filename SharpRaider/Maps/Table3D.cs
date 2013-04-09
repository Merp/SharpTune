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
using System.IO;
using System.Text;
using Java.Awt;
using Java.Awt.Datatransfer;
using Java.Awt.Event;
using Javax.Swing;
using RomRaider;
using RomRaider.Editor.Ecu;
using RomRaider.Logger.Ecu.UI.Swing.Vertical;
using RomRaider.Maps;
using RomRaider.Swing;
using RomRaider.Util;
using RomRaider.Xml;
using Sharpen;

namespace RomRaider.Maps
{
	[System.Serializable]
	public class Table3D : Table
	{
		private const long serialVersionUID = 3103448753263606599L;

		private Table1D xAxis;

		private Table1D yAxis;

		private AList<AList<DataCell>> data = new AList<AList<DataCell>>();

		private bool swapXY = false;

		private bool flipX = false;

		private bool flipY = false;

		internal CopyTable3DWorker copyTable3DWorker;

		internal CopySelection3DWorker copySelection3DWorker;

		public Table3D(ECUEditor editor) : base(editor)
		{
			xAxis = new Table1D(editor);
			yAxis = new Table1D(editor);
			verticalOverhead += 39;
			horizontalOverhead += 10;
		}

		public virtual Table1D GetXAxis()
		{
			return xAxis;
		}

		public virtual void SetXAxis(Table1D xAxis)
		{
			this.xAxis = xAxis;
		}

		public virtual Table1D GetYAxis()
		{
			return yAxis;
		}

		public virtual void SetYAxis(Table1D yAxis)
		{
			this.yAxis = yAxis;
		}

		public virtual bool GetSwapXY()
		{
			return swapXY;
		}

		public virtual void SetSwapXY(bool swapXY)
		{
			this.swapXY = swapXY;
		}

		public virtual bool GetFlipX()
		{
			return flipX;
		}

		public virtual void SetFlipX(bool flipX)
		{
			this.flipX = flipX;
		}

		public virtual bool GetFlipY()
		{
			return flipY;
		}

		public virtual void SetFlipY(bool flipY)
		{
			this.flipY = flipY;
		}

		public virtual void SetSizeX(int size)
		{
			//data = new ArrayList<DataCell[]>(data[0].length);
			centerLayout.SetColumns(size + 1);
		}

		public virtual int GetSizeX()
		{
			return data.Count;
		}

		public virtual void SetSizeY(int size)
		{
			//data = new DataCell[data.length][size];
			centerLayout.SetRows(size + 1);
		}

		public virtual int GetSizeY()
		{
			return data[0].Count;
		}

		/// <exception cref="System.ArgumentNullException"></exception>
		/// <exception cref="System.IndexOutOfRangeException"></exception>
		public override void PopulateTable(byte[] input)
		{
			// fill first empty cell
			centerPanel.Add(new JLabel());
			if (!beforeRam)
			{
				ramOffset = container.GetRomID().GetRamOffset();
			}
			// temporarily remove lock
			bool tempLock = locked;
			locked = false;
			// populate axiis
			try
			{
				xAxis.SetRom(container);
				xAxis.PopulateTable(input);
				yAxis.SetRom(container);
				yAxis.PopulateTable(input);
			}
			catch (IndexOutOfRangeException)
			{
				throw new IndexOutOfRangeException();
			}
			for (int x = 0; x < xAxis.GetDataSize(); x++)
			{
				centerPanel.Add(xAxis.GetDataCell(x));
			}
			int offset = 0;
			int iMax = swapXY ? xAxis.GetDataSize() : yAxis.GetDataSize();
			int jMax = swapXY ? yAxis.GetDataSize() : xAxis.GetDataSize();
			for (int i = 0; i < iMax; i++)
			{
				for (int j = 0; j < jMax; j++)
				{
					int x_1 = flipY ? jMax - j - 1 : j;
					int y = flipX ? iMax - i - 1 : i;
					if (swapXY)
					{
						int z = x_1;
						x_1 = y;
						y = z;
					}
					data[x_1].Set(y, new DataCell(scales[scaleIndex], editor.GetSettings().GetCellSize
						()));
					data[x_1][y].SetTable(this);
					// populate data cells
					if (storageType == STORAGE_TYPE_FLOAT)
					{
						//float storage type
						byte[] byteValue = new byte[4];
						byteValue[0] = input[storageAddress + offset * 4 - ramOffset];
						byteValue[1] = input[storageAddress + offset * 4 - ramOffset + 1];
						byteValue[2] = input[storageAddress + offset * 4 - ramOffset + 2];
						byteValue[3] = input[storageAddress + offset * 4 - ramOffset + 3];
						data[x_1][y].SetBinValue(RomAttributeParser.ByteToFloat(byteValue, endian));
					}
					else
					{
						// integer storage type
						data[x_1][y].SetBinValue(RomAttributeParser.ParseByteValue(input, endian, storageAddress
							 + offset * storageType - ramOffset, storageType, signed));
					}
					// show locked cell
					if (tempLock)
					{
						data[x_1][y].SetForeground(Color.GRAY);
					}
					data[x_1][y].SetXCoord(x_1);
					data[x_1][y].SetYCoord(y);
					data[x_1][y].SetOriginalValue(data[x_1][y].GetBinValue());
					offset++;
				}
			}
			for (int y_1 = 0; y_1 < yAxis.GetDataSize(); y_1++)
			{
				centerPanel.Add(yAxis.GetDataCell(y_1));
				for (int x_1 = 0; x_1 < xAxis.GetDataSize(); x_1++)
				{
					centerPanel.Add(data[x_1][y_1]);
				}
			}
			// reset locked status
			locked = tempLock;
			GridLayout topLayout = new GridLayout(2, 1);
			JPanel topPanel = new JPanel(topLayout);
			this.Add(topPanel, BorderLayout.NORTH);
			topPanel.Add(new JLabel(name, JLabel.CENTER), BorderLayout.NORTH);
			topPanel.Add(new JLabel(xAxis.GetName() + " (" + xAxis.GetScale().GetUnit() + ")"
				, JLabel.CENTER), BorderLayout.NORTH);
			JLabel yLabel = new JLabel(yAxis.GetName() + " (" + yAxis.GetScale().GetUnit() + 
				")");
			yLabel.SetUI(new VerticalLabelUI(false));
			Add(yLabel, BorderLayout.WEST);
			Add(new JLabel(GetScale().GetUnit(), JLabel.CENTER), BorderLayout.SOUTH);
		}

		public override StringBuilder GetTableAsString()
		{
			// Make a string of the table
			StringBuilder output = new StringBuilder(BLANK);
			output.Append(xAxis.GetTableAsString()).Append(NEW_LINE);
			for (int y = 0; y < GetSizeY(); y++)
			{
				output.Append(yAxis.GetCellAsString(y)).Append(TAB);
				for (int x = 0; x < GetSizeX(); x++)
				{
					output.Append(data[x][y].GetText());
					if (x < GetSizeX() - 1)
					{
						output.Append(TAB);
					}
				}
				if (y < GetSizeY() - 1)
				{
					output.Append(NEW_LINE);
				}
			}
			return output;
		}

		public override void Colorize()
		{
			if (compareDisplay == COMPARE_DISPLAY_OFF)
			{
				if (!isStatic && !isAxis)
				{
					double high = double.MinValue;
					double low = double.MaxValue;
					if (GetScale().GetMax() != 0 || GetScale().GetMin() != 0)
					{
						// set min and max values if they are set in scale
						high = GetScale().GetMax();
						low = GetScale().GetMin();
					}
					else
					{
						// min/max not set in scale
						foreach (AList<DataCell> column in data)
						{
							foreach (DataCell cell in column)
							{
								double value = cell.GetValue();
								if (value > high)
								{
									high = value;
								}
								if (value < low)
								{
									low = value;
								}
							}
						}
					}
					foreach (AList<DataCell> column_1 in data)
					{
						foreach (DataCell cell in column_1)
						{
							double value = cell.GetValue();
							if (value > high || value < low)
							{
								// value exceeds limit
								cell.SetColor(editor.GetSettings().GetWarningColor());
							}
							else
							{
								// limits not set, scale based on table values
								double scale;
								if (high - low == 0)
								{
									// if all values are the same, color will be middle value
									scale = .5;
								}
								else
								{
									scale = (value - low) / (high - low);
								}
								cell.SetColor(ColorScaler.GetScaledColor(scale, editor.GetSettings()));
							}
						}
					}
				}
			}
			else
			{
				// comparing is on
				if (!isStatic)
				{
					double high = double.MinValue;
					// determine ratios
					foreach (AList<DataCell> column in data)
					{
						foreach (DataCell cell in column)
						{
							if (Math.Abs(cell.GetBinValue() - cell.GetCompareValue()) > high)
							{
								high = Math.Abs(cell.GetBinValue() - cell.GetCompareValue());
							}
						}
					}
					// colorize
					foreach (AList<DataCell> column_1 in data)
					{
						foreach (DataCell cell in column_1)
						{
							double cellDifference = Math.Abs(cell.GetBinValue() - cell.GetCompareValue());
							double scale;
							if (high == 0)
							{
								scale = 0;
							}
							else
							{
								scale = cellDifference / high;
							}
							if (scale == 0)
							{
								cell.SetColor(UNCHANGED_VALUE_COLOR);
							}
							else
							{
								cell.SetColor(ColorScaler.GetScaledColor(scale, editor.GetSettings()));
							}
							// set border
							if (cell.GetBinValue() > cell.GetCompareValue())
							{
								cell.SetBorder(BorderFactory.CreateLineBorder(editor.GetSettings().GetIncreaseBorder
									()));
							}
							else
							{
								if (cell.GetBinValue() < cell.GetCompareValue())
								{
									cell.SetBorder(BorderFactory.CreateLineBorder(editor.GetSettings().GetDecreaseBorder
										()));
								}
								else
								{
									cell.SetBorder(BorderFactory.CreateLineBorder(Color.BLACK, 1));
								}
							}
						}
					}
				}
			}
			// colorize borders
			if (!isStatic)
			{
				foreach (AList<DataCell> column in data)
				{
					foreach (DataCell cell in column)
					{
						double checkValue;
						if (compareDisplay == Table.COMPARE_DISPLAY_OFF)
						{
							checkValue = cell.GetOriginalValue();
						}
						else
						{
							checkValue = cell.GetCompareValue();
						}
						if (checkValue > cell.GetBinValue())
						{
							cell.SetBorder(BorderFactory.CreateLineBorder(editor.GetSettings().GetIncreaseBorder
								()));
						}
						else
						{
							if (checkValue < cell.GetBinValue())
							{
								cell.SetBorder(BorderFactory.CreateLineBorder(editor.GetSettings().GetDecreaseBorder
									()));
							}
							else
							{
								cell.SetBorder(BorderFactory.CreateLineBorder(Color.BLACK, 1));
							}
						}
					}
				}
			}
		}

		public override bool FillCompareValues()
		{
			if (null == compareTable || !(compareTable is RomRaider.Maps.Table3D))
			{
				return false;
			}
			RomRaider.Maps.Table3D compareTable3D = (RomRaider.Maps.Table3D)compareTable;
			if (data.Count != compareTable3D.data.Count || data[0].Count != compareTable3D.data
				[0].Count || xAxis.GetDataSize() != compareTable3D.xAxis.GetDataSize() || yAxis.
				GetDataSize() != compareTable3D.yAxis.GetDataSize())
			{
				return false;
			}
			ClearLiveDataTrace();
			int x = 0;
			int y = 0;
			foreach (AList<DataCell> column in data)
			{
				y = 0;
				foreach (DataCell cell in column)
				{
					if (compareType == COMPARE_TYPE_BIN)
					{
						cell.SetCompareValue(compareTable3D.data[x][y].GetBinValue());
					}
					else
					{
						cell.SetCompareValue(compareTable3D.data[x][y].GetOriginalValue());
					}
					y++;
				}
				x++;
			}
			if (!xAxis.isStatic)
			{
				xAxis.FillCompareValues();
			}
			if (!yAxis.isStatic)
			{
				yAxis.FillCompareValues();
			}
			return true;
		}

		public override void SetFrame(TableFrame frame)
		{
			this.frame = frame;
			xAxis.SetFrame(frame);
			yAxis.SetFrame(frame);
			//frame.setSize(getFrameSize());
			frame.Pack();
		}

		public override Dimension GetFrameSize()
		{
			int height = verticalOverhead + cellHeight * data[0].Count;
			int width = horizontalOverhead + data.Count * cellWidth;
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

		public override string ToString()
		{
			return base.ToString() + " (3D)";
		}

		public override void Increment(double increment)
		{
			if (!isStatic && !locked)
			{
				for (int x = 0; x < this.GetSizeX(); x++)
				{
					for (int y = 0; y < this.GetSizeY(); y++)
					{
						if (data[x][y].IsSelected())
						{
							data[x][y].Increment(increment);
						}
					}
				}
			}
			xAxis.Increment(increment);
			yAxis.Increment(increment);
			Colorize();
		}

		public override void Multiply(double factor)
		{
			if (!isStatic && !locked)
			{
				for (int x = 0; x < this.GetSizeX(); x++)
				{
					for (int y = 0; y < this.GetSizeY(); y++)
					{
						if (data[x][y].IsSelected())
						{
							data[x][y].Multiply(factor);
						}
					}
				}
			}
			xAxis.Multiply(factor);
			yAxis.Multiply(factor);
			Colorize();
		}

		public override void ClearSelection()
		{
			xAxis.ClearSelection(true);
			yAxis.ClearSelection(true);
			for (int x = 0; x < this.GetSizeX(); x++)
			{
				for (int y = 0; y < this.GetSizeY(); y++)
				{
					data[x][y].SetSelected(false);
				}
			}
		}

		public override void Highlight(int xCoord, int yCoord)
		{
			if (highlight)
			{
				for (int x = 0; x < this.GetSizeX(); x++)
				{
					for (int y = 0; y < this.GetSizeY(); y++)
					{
						if (((y >= highlightY && y <= yCoord) || (y <= highlightY && y >= yCoord)) && ((x
							 >= highlightX && x <= xCoord) || (x <= highlightX && x >= xCoord)))
						{
							data[x][y].SetHighlighted(true);
						}
						else
						{
							data[x][y].SetHighlighted(false);
						}
					}
				}
			}
		}

		public override void StopHighlight()
		{
			highlight = false;
			// loop through, selected and un-highlight
			for (int x = 0; x < this.GetSizeX(); x++)
			{
				for (int y = 0; y < this.GetSizeY(); y++)
				{
					if (data[x][y].IsHighlighted())
					{
						data[x][y].SetSelected(true);
						data[x][y].SetHighlighted(false);
					}
				}
			}
		}

		public override void SetRevertPoint()
		{
			for (int x = 0; x < this.GetSizeX(); x++)
			{
				for (int y = 0; y < this.GetSizeY(); y++)
				{
					data[x][y].SetOriginalValue(data[x][y].GetBinValue());
				}
			}
			yAxis.SetRevertPoint();
			xAxis.SetRevertPoint();
			Colorize();
		}

		public override void UndoAll()
		{
			ClearLiveDataTrace();
			for (int x = 0; x < this.GetSizeX(); x++)
			{
				for (int y = 0; y < this.GetSizeY(); y++)
				{
					if (data[x][y].GetBinValue() != data[x][y].GetOriginalValue())
					{
						data[x][y].SetBinValue(data[x][y].GetOriginalValue());
					}
				}
			}
			yAxis.UndoAll();
			xAxis.UndoAll();
			Colorize();
		}

		public override void UndoSelected()
		{
			ClearLiveDataTrace();
			for (int x = 0; x < this.GetSizeX(); x++)
			{
				for (int y = 0; y < this.GetSizeY(); y++)
				{
					if (data[x][y].IsSelected())
					{
						if (data[x][y].GetBinValue() != data[x][y].GetOriginalValue())
						{
							data[x][y].SetBinValue(data[x][y].GetOriginalValue());
						}
					}
				}
			}
			yAxis.UndoSelected();
			xAxis.UndoSelected();
			Colorize();
		}

		public override byte[] SaveFile(byte[] binData)
		{
			if (!isStatic && userLevel <= editor.GetSettings().GetUserLevel() && (userLevel <
				 5 || editor.GetSettings().IsSaveDebugTables()))
			{
				// save if table is not static
				// and user level is great enough
				// and table is not in debug mode, unless saveDebugTables is true
				binData = xAxis.SaveFile(binData);
				binData = yAxis.SaveFile(binData);
				int offset = 0;
				int iMax = swapXY ? xAxis.GetDataSize() : yAxis.GetDataSize();
				int jMax = swapXY ? yAxis.GetDataSize() : xAxis.GetDataSize();
				for (int i = 0; i < iMax; i++)
				{
					for (int j = 0; j < jMax; j++)
					{
						int x = flipY ? jMax - j - 1 : j;
						int y = flipX ? iMax - i - 1 : i;
						if (swapXY)
						{
							int z = x;
							x = y;
							y = z;
						}
						// determine output byte values
						byte[] output;
						if (storageType != STORAGE_TYPE_FLOAT)
						{
							output = RomAttributeParser.ParseIntegerValue((int)data[x][y].GetBinValue(), endian
								, storageType);
							for (int z = 0; z < storageType; z++)
							{
								binData[offset * storageType + z + storageAddress - ramOffset] = output[z];
							}
						}
						else
						{
							// float
							output = RomAttributeParser.FloatToByte((float)data[x][y].GetBinValue(), endian);
							for (int z = 0; z < 4; z++)
							{
								binData[offset * 4 + z + storageAddress - ramOffset] = output[z];
							}
						}
						offset++;
					}
				}
			}
			return binData;
		}

		public override void SetRealValue(string realValue)
		{
			if (!isStatic && !locked)
			{
				for (int x = 0; x < this.GetSizeX(); x++)
				{
					for (int y = 0; y < this.GetSizeY(); y++)
					{
						if (data[x][y].IsSelected())
						{
							data[x][y].SetRealValue(realValue);
						}
					}
				}
			}
			xAxis.SetRealValue(realValue);
			yAxis.SetRealValue(realValue);
			Colorize();
		}

		public override void AddKeyListener(KeyListener listener)
		{
			xAxis.AddKeyListener(listener);
			yAxis.AddKeyListener(listener);
			for (int x = 0; x < this.GetSizeX(); x++)
			{
				for (int y = 0; y < this.GetSizeY(); y++)
				{
					data[x][y].AddKeyListener(listener);
				}
			}
		}

		public virtual void SelectCellAt(int y, Table1D axisType)
		{
			if (axisType.GetType() == TABLE_Y_AXIS)
			{
				SelectCellAt(0, y);
			}
			else
			{
				// y axis
				SelectCellAt(y, 0);
			}
		}

		public virtual void DeSelectCellAt(int x, int y)
		{
			ClearSelection();
			data[x][y].SetSelected(false);
			highlightX = x;
			highlightY = y;
		}

		public virtual void SelectCellAt(int x, int y)
		{
			ClearSelection();
			data[x][y].SetSelected(true);
			highlightX = x;
			highlightY = y;
		}

		public virtual void SelectCellAtWithoutClear(int x, int y)
		{
			data[x][y].SetSelected(true);
			highlightX = x;
			highlightY = y;
		}

		public override void CursorUp()
		{
			if (highlightY > 0 && data[highlightX][highlightY].IsSelected())
			{
				SelectCellAt(highlightX, highlightY - 1);
			}
			else
			{
				if (!xAxis.IsStatic() && data[highlightX][highlightY].IsSelected())
				{
					xAxis.SelectCellAt(highlightX);
				}
				else
				{
					xAxis.CursorUp();
					yAxis.CursorUp();
				}
			}
		}

		public override void CursorDown()
		{
			if (highlightY < GetSizeY() - 1 && data[highlightX][highlightY].IsSelected())
			{
				SelectCellAt(highlightX, highlightY + 1);
			}
			else
			{
				xAxis.CursorDown();
				yAxis.CursorDown();
			}
		}

		public override void CursorLeft()
		{
			if (highlightX > 0 && data[highlightX][highlightY].IsSelected())
			{
				SelectCellAt(highlightX - 1, highlightY);
			}
			else
			{
				if (!yAxis.IsStatic() && data[highlightX][highlightY].IsSelected())
				{
					yAxis.SelectCellAt(highlightY);
				}
				else
				{
					xAxis.CursorLeft();
					yAxis.CursorLeft();
				}
			}
		}

		public override void CursorRight()
		{
			if (highlightX < GetSizeX() - 1 && data[highlightX][highlightY].IsSelected())
			{
				SelectCellAt(highlightX + 1, highlightY);
			}
			else
			{
				xAxis.CursorRight();
				yAxis.CursorRight();
			}
		}

		public override void StartHighlight(int x, int y)
		{
			xAxis.ClearSelection();
			yAxis.ClearSelection();
			base.StartHighlight(x, y);
		}

		public override void CopySelection()
		{
			Window ancestorWindow = SwingUtilities.GetWindowAncestor(this);
			if (null != ancestorWindow)
			{
				ancestorWindow.SetCursor(Cursor.GetPredefinedCursor(Cursor.WAIT_CURSOR));
			}
			GetEditor().SetCursor(Cursor.GetPredefinedCursor(Cursor.WAIT_CURSOR));
			SetCursor(Cursor.GetPredefinedCursor(Cursor.WAIT_CURSOR));
			copySelection3DWorker = new CopySelection3DWorker(this);
			copySelection3DWorker.Execute();
		}

		public override void CopyTable()
		{
			Window ancestorWindow = SwingUtilities.GetWindowAncestor(this);
			if (null != ancestorWindow)
			{
				ancestorWindow.SetCursor(Cursor.GetPredefinedCursor(Cursor.WAIT_CURSOR));
			}
			GetEditor().SetCursor(Cursor.GetPredefinedCursor(Cursor.WAIT_CURSOR));
			SetCursor(Cursor.GetPredefinedCursor(Cursor.WAIT_CURSOR));
			copyTable3DWorker = new CopyTable3DWorker(editor.GetSettings(), this);
			copyTable3DWorker.Execute();
		}

		public override void Paste()
		{
			StringTokenizer st = new StringTokenizer(string.Empty);
			string input = string.Empty;
			try
			{
				input = (string)Toolkit.GetDefaultToolkit().GetSystemClipboard().GetContents(null
					).GetTransferData(DataFlavor.stringFlavor);
				st = new StringTokenizer(input);
			}
			catch (UnsupportedFlavorException)
			{
			}
			catch (IOException)
			{
			}
			string pasteType = st.NextToken();
			if (Sharpen.Runtime.EqualsIgnoreCase("[Table3D]", pasteType))
			{
				// Paste table
				string newline = Runtime.GetProperty("line.separator");
				string xAxisValues = "[Table1D]" + newline + st.NextToken(newline);
				// build y axis and data values
				StringBuilder yAxisValues = new StringBuilder("[Table1D]" + newline + st.NextToken
					("\t"));
				StringBuilder dataValues = new StringBuilder("[Table3D]" + newline + st.NextToken
					("\t") + st.NextToken(newline));
				while (st.HasMoreTokens())
				{
					yAxisValues.Append("\t").Append(st.NextToken("\t"));
					dataValues.Append(newline).Append(st.NextToken("\t")).Append(st.NextToken(newline
						));
				}
				// put x axis in clipboard and paste
				Toolkit.GetDefaultToolkit().GetSystemClipboard().SetContents(new StringSelection(
					xAxisValues), null);
				xAxis.Paste();
				// put y axis in clipboard and paste
				Toolkit.GetDefaultToolkit().GetSystemClipboard().SetContents(new StringSelection(
					yAxisValues.ToString()), null);
				yAxis.Paste();
				// put datavalues in clipboard and paste
				Toolkit.GetDefaultToolkit().GetSystemClipboard().SetContents(new StringSelection(
					dataValues.ToString()), null);
				PasteValues();
				Colorize();
				// reset clipboard
				Toolkit.GetDefaultToolkit().GetSystemClipboard().SetContents(new StringSelection(
					input), null);
			}
			else
			{
				if (Sharpen.Runtime.EqualsIgnoreCase("[Selection3D]", pasteType))
				{
					// paste selection
					PasteValues();
					Colorize();
				}
				else
				{
					if (Sharpen.Runtime.EqualsIgnoreCase("[Selection1D]", pasteType))
					{
						// paste selection
						xAxis.Paste();
						yAxis.Paste();
					}
				}
			}
		}

		public virtual void PasteValues()
		{
			StringTokenizer st = new StringTokenizer(string.Empty);
			string newline = Runtime.GetProperty("line.separator");
			try
			{
				string input = (string)Toolkit.GetDefaultToolkit().GetSystemClipboard().GetContents
					(null).GetTransferData(DataFlavor.stringFlavor);
				st = new StringTokenizer(input);
			}
			catch (UnsupportedFlavorException)
			{
			}
			catch (IOException)
			{
			}
			string pasteType = st.NextToken();
			// figure paste start cell
			int startX = 0;
			int startY = 0;
			// if pasting a table, startX and Y at 0, else highlight is start
			if (Sharpen.Runtime.EqualsIgnoreCase("[Selection3D]", pasteType))
			{
				startX = highlightX;
				startY = highlightY;
			}
			// set values
			for (int y = startY; y < GetSizeY(); y++)
			{
				if (st.HasMoreTokens())
				{
					StringTokenizer currentLine = new StringTokenizer(st.NextToken(newline));
					for (int x = startX; x < GetSizeX(); x++)
					{
						if (currentLine.HasMoreTokens())
						{
							string currentToken = currentLine.NextToken();
							try
							{
								if (!Sharpen.Runtime.EqualsIgnoreCase(data[x][y].GetText(), currentToken))
								{
									data[x][y].SetRealValue(currentToken);
								}
							}
							catch (IndexOutOfRangeException)
							{
							}
						}
					}
				}
			}
		}

		public override void ApplyColorSettings()
		{
			// apply settings to cells
			for (int y = 0; y < GetSizeY(); y++)
			{
				for (int x = 0; x < GetSizeX(); x++)
				{
					this.SetMaxColor(editor.GetSettings().GetMaxColor());
					this.SetMinColor(editor.GetSettings().GetMinColor());
					data[x][y].SetHighlightColor(editor.GetSettings().GetHighlightColor());
					data[x][y].SetIncreaseBorder(editor.GetSettings().GetIncreaseBorder());
					data[x][y].SetDecreaseBorder(editor.GetSettings().GetDecreaseBorder());
					data[x][y].SetFont(editor.GetSettings().GetTableFont());
					data[x][y].Repaint();
				}
			}
			this.SetAxisColor(editor.GetSettings().GetAxisColor());
			xAxis.ApplyColorSettings();
			yAxis.ApplyColorSettings();
			cellHeight = (int)editor.GetSettings().GetCellSize().GetHeight();
			cellWidth = (int)editor.GetSettings().GetCellSize().GetWidth();
			ValidateScaling();
			Resize();
			Colorize();
		}

		public override void SetAxisColor(Color axisColor)
		{
			xAxis.SetAxisColor(axisColor);
			yAxis.SetAxisColor(axisColor);
		}

		public override void ValidateScaling()
		{
			base.ValidateScaling();
			xAxis.ValidateScaling();
			yAxis.ValidateScaling();
		}

		public override void RefreshValues()
		{
			if (!isStatic && !isAxis)
			{
				foreach (AList<DataCell> column in data)
				{
					foreach (DataCell cell in column)
					{
						cell.RefreshValue();
					}
				}
			}
		}

		public override bool IsLiveDataSupported()
		{
			return !ParamChecker.IsNullOrEmpty(xAxis.GetLogParam()) && !ParamChecker.IsNullOrEmpty
				(yAxis.GetLogParam());
		}

		public override bool IsButtonSelected()
		{
			return true;
		}

		protected internal override void HighlightLiveData()
		{
			if (overlayLog && frame.IsVisible())
			{
				AxisRange rangeX = TableAxisUtil.GetLiveDataRangeForAxis(xAxis);
				AxisRange rangeY = TableAxisUtil.GetLiveDataRangeForAxis(yAxis);
				ClearSelection();
				bool first = true;
				for (int x = rangeX.GetStartIndex(); x <= rangeX.GetEndIndex(); x++)
				{
					for (int y = rangeY.GetStartIndex(); y <= rangeY.GetEndIndex(); y++)
					{
						if (first)
						{
							StartHighlight(x, y);
							first = false;
						}
						else
						{
							Highlight(x, y);
						}
						DataCell cell = data[x][y];
						cell.SetLiveDataTrace(true);
						cell.SetDisplayValue(cell.GetRealValue() + (ParamChecker.IsNullOrEmpty(liveValue)
							 ? string.Empty : (':' + liveValue)));
					}
				}
				StopHighlight();
				frame.GetToolBar().SetLiveDataValue(liveValue);
			}
		}

		public override void ClearLiveDataTrace()
		{
			for (int x = 0; x < GetSizeX(); x++)
			{
				for (int y = 0; y < GetSizeY(); y++)
				{
					data[x][y].SetLiveDataTrace(false);
					data[x][y].UpdateDisplayValue();
				}
			}
		}

		public override void SetScaleIndex(int scaleIndex)
		{
			base.SetScaleIndex(scaleIndex);
			xAxis.SetScaleByName(GetScale().GetName());
			yAxis.SetScaleByName(GetScale().GetName());
		}

		public virtual AList<AList<DataCell>> Get3dData()
		{
			return data;
		}

		public override double GetMin()
		{
			if (GetScale().GetMin() == 0 && GetScale().GetMax() == 0)
			{
				double low = double.MaxValue;
				foreach (AList<DataCell> column in data)
				{
					foreach (DataCell cell in column)
					{
						double value = cell.GetValue();
						if (value < low)
						{
							low = value;
						}
					}
				}
				return low;
			}
			else
			{
				return GetScale().GetMin();
			}
		}

		public override double GetMax()
		{
			if (GetScale().GetMin() == 0 && GetScale().GetMax() == 0)
			{
				double high = double.MinValue;
				foreach (AList<DataCell> column in data)
				{
					foreach (DataCell cell in column)
					{
						double value = cell.GetValue();
						if (value > high)
						{
							high = value;
						}
					}
				}
				return high;
			}
			else
			{
				return GetScale().GetMax();
			}
		}

		public override void SetCompareDisplay(int compareDisplay)
		{
			base.SetCompareDisplay(compareDisplay);
			if (!xAxis.isStatic)
			{
				xAxis.SetCompareDisplay(compareDisplay);
			}
			if (!yAxis.isStatic)
			{
				yAxis.SetCompareDisplay(compareDisplay);
			}
		}

		public override void SetCompareType(int comparetype)
		{
			base.SetCompareType(comparetype);
			if (!xAxis.isStatic)
			{
				xAxis.SetCompareType(comparetype);
			}
			if (!yAxis.isStatic)
			{
				yAxis.SetCompareType(comparetype);
			}
		}

		public override void SetCompareTable(Table compareTable)
		{
			base.SetCompareTable(compareTable);
			if (null == compareTable || !(compareTable is RomRaider.Maps.Table3D))
			{
				return;
			}
			RomRaider.Maps.Table3D compareTable3D = (RomRaider.Maps.Table3D)compareTable;
			if (!xAxis.isStatic)
			{
				this.xAxis.SetCompareTable(compareTable3D.xAxis);
			}
			if (!yAxis.isStatic)
			{
				this.yAxis.SetCompareTable(compareTable3D.yAxis);
			}
		}

		public override void RefreshCellDisplay()
		{
			foreach (AList<DataCell> column in data)
			{
				foreach (DataCell cell in column)
				{
					cell.SetCompareDisplay(compareDisplay);
					cell.UpdateDisplayValue();
				}
			}
			if (!xAxis.isStatic)
			{
				xAxis.RefreshCellDisplay();
			}
			if (!yAxis.isStatic)
			{
				yAxis.RefreshCellDisplay();
			}
			Colorize();
		}

		public override void AddComparedToTable(Table table)
		{
			base.AddComparedToTable(table);
			if (!(table is RomRaider.Maps.Table3D))
			{
				return;
			}
			RomRaider.Maps.Table3D table3D = (RomRaider.Maps.Table3D)table;
			if (!xAxis.isStatic)
			{
				xAxis.AddComparedToTable(table3D.xAxis);
			}
			if (!yAxis.isStatic)
			{
				yAxis.AddComparedToTable(table3D.yAxis);
			}
		}
	}

	internal class CopySelection3DWorker : SwingWorker<Void, Void>
	{
		internal Table3D table;

		public CopySelection3DWorker(Table3D table)
		{
			this.table = table;
		}

		/// <exception cref="System.Exception"></exception>
		protected override Void DoInBackground()
		{
			// find bounds of selection
			// coords[0] = x min, y min, x max, y max
			bool copy = false;
			int[] coords = new int[4];
			coords[0] = table.GetSizeX();
			coords[1] = table.GetSizeY();
			for (int x = 0; x < table.GetSizeX(); x++)
			{
				for (int y = 0; y < table.GetSizeY(); y++)
				{
					if (table.Get3dData()[x][y].IsSelected())
					{
						if (x < coords[0])
						{
							coords[0] = x;
							copy = true;
						}
						if (x > coords[2])
						{
							coords[2] = x;
							copy = true;
						}
						if (y < coords[1])
						{
							coords[1] = y;
							copy = true;
						}
						if (y > coords[3])
						{
							coords[3] = y;
							copy = true;
						}
					}
				}
			}
			// make string of selection
			if (copy)
			{
				string newline = Runtime.GetProperty("line.separator");
				StringBuilder output = new StringBuilder("[Selection3D]" + newline);
				for (int y = coords[1]; y <= coords[3]; y++)
				{
					for (int x_1 = coords[0]; x_1 <= coords[2]; x_1++)
					{
						if (table.Get3dData()[x_1][y].IsSelected())
						{
							output.Append(table.Get3dData()[x_1][y].GetText());
						}
						else
						{
							output.Append("x");
						}
						// x represents non-selected cell
						if (x_1 < coords[2])
						{
							output.Append("\t");
						}
					}
					if (y < coords[3])
					{
						output.Append(newline);
					}
					//copy to clipboard
					Toolkit.GetDefaultToolkit().GetSystemClipboard().SetContents(new StringSelection(
						output.ToString()), null);
				}
			}
			else
			{
				table.GetXAxis().CopySelection();
				table.GetYAxis().CopySelection();
			}
			return null;
		}

		protected override void Done()
		{
			Window ancestorWindow = SwingUtilities.GetWindowAncestor(table);
			if (null != ancestorWindow)
			{
				ancestorWindow.SetCursor(null);
			}
			table.SetCursor(null);
			table.GetEditor().SetCursor(null);
		}
	}

	internal class CopyTable3DWorker : SwingWorker<Void, Void>
	{
		internal Settings settings;

		internal Table3D table;

		public CopyTable3DWorker(Settings settings, Table3D table)
		{
			this.settings = settings;
			this.table = table;
		}

		/// <exception cref="System.Exception"></exception>
		protected override Void DoInBackground()
		{
			string tableHeader = settings.GetTable3DHeader();
			StringBuilder output = new StringBuilder(tableHeader);
			output.Append(table.GetXAxis().GetTableAsString()).Append(Table3D.NEW_LINE);
			for (int y = 0; y < table.GetSizeY(); y++)
			{
				output.Append(table.GetYAxis().GetCellAsString(y)).Append(Table3D.TAB);
				for (int x = 0; x < table.GetSizeX(); x++)
				{
					output.Append(table.Get3dData()[x][y].GetText());
					if (x < table.GetSizeX() - 1)
					{
						output.Append(Table3D.TAB);
					}
				}
				if (y < table.GetSizeY() - 1)
				{
					output.Append(Table3D.NEW_LINE);
				}
			}
			//copy to clipboard
			Toolkit.GetDefaultToolkit().GetSystemClipboard().SetContents(new StringSelection(
				output.ToString()), null);
			return null;
		}

		protected override void Done()
		{
			Window ancestorWindow = SwingUtilities.GetWindowAncestor(table);
			if (null != ancestorWindow)
			{
				ancestorWindow.SetCursor(null);
			}
			table.SetCursor(null);
			table.GetEditor().SetCursor(null);
		}
	}
}

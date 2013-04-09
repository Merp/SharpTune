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
using RomRaider.Maps;
using RomRaider.Swing;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Maps
{
	[System.Serializable]
	public class Table2D : Table
	{
		private const long serialVersionUID = -7684570967109324784L;

		internal static readonly string NEW_LINE = Runtime.GetProperty("line.separator");

		private Table1D axis;

		private CopyTable2DWorker copyTable2DWorker;

		private CopySelection2DWorker copySelection2DWorker;

		public Table2D(ECUEditor editor) : base(editor)
		{
			axis = new Table1D(editor);
			verticalOverhead += 18;
		}

		public virtual Table1D GetAxis()
		{
			return axis;
		}

		public virtual void SetAxis(Table1D axis)
		{
			this.axis = axis;
		}

		public override string ToString()
		{
			return base.ToString() + " (2D)";
		}

		// + axis;
		public override bool FillCompareValues()
		{
			base.FillCompareValues();
			if (null == compareTable || !(compareTable is RomRaider.Maps.Table2D))
			{
				return false;
			}
			RomRaider.Maps.Table2D compareTable2D = (RomRaider.Maps.Table2D)compareTable;
			if (data.Length != compareTable2D.data.Length || axis.data.Length != compareTable2D
				.axis.data.Length)
			{
				return false;
			}
			if (!axis.isStatic)
			{
				axis.FillCompareValues();
			}
			return true;
		}

		public override void Colorize()
		{
			base.Colorize();
			axis.Colorize();
		}

		public override void SetFrame(TableFrame frame)
		{
			this.frame = frame;
			axis.SetFrame(frame);
			frame.SetSize(GetFrameSize());
		}

		public override Dimension GetFrameSize()
		{
			int height = verticalOverhead + cellHeight * 2;
			int width = horizontalOverhead + data.Length * cellWidth;
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

		public override void ApplyColorSettings()
		{
			this.SetAxisColor(editor.GetSettings().GetAxisColor());
			axis.ApplyColorSettings();
			base.ApplyColorSettings();
		}

		/// <exception cref="System.IndexOutOfRangeException"></exception>
		public override void PopulateTable(byte[] input)
		{
			centerLayout.SetRows(2);
			centerLayout.SetColumns(this.GetDataSize());
			try
			{
				axis.SetRom(container);
				axis.PopulateTable(input);
				base.PopulateTable(input);
			}
			catch (IndexOutOfRangeException)
			{
				throw new IndexOutOfRangeException();
			}
			// add to table
			for (int i = 0; i < this.GetDataSize(); i++)
			{
				centerPanel.Add(axis.GetDataCell(i));
			}
			if (flip)
			{
				for (int i_1 = this.GetDataSize() - 1; i_1 >= 0; i_1--)
				{
					centerPanel.Add(this.GetDataCell(i_1));
				}
			}
			else
			{
				for (int i_1 = 0; i_1 < this.GetDataSize(); i_1++)
				{
					centerPanel.Add(this.GetDataCell(i_1));
				}
			}
			Add(new JLabel(axis.GetName() + " (" + axis.GetScale().GetUnit() + ")", JLabel.CENTER
				), BorderLayout.NORTH);
			if (axis.IsStatic())
			{
				Add(new JLabel(axis.GetName(), JLabel.CENTER), BorderLayout.NORTH);
			}
			else
			{
				Add(new JLabel(axis.GetName() + " (" + axis.GetScale().GetUnit() + ")", JLabel.CENTER
					), BorderLayout.NORTH);
			}
			Add(new JLabel(scales[scaleIndex].GetUnit(), JLabel.CENTER), BorderLayout.SOUTH);
		}

		//this.colorize();
		public override void Increment(double increment)
		{
			base.Increment(increment);
			axis.Increment(increment);
		}

		public override void Multiply(double factor)
		{
			base.Multiply(factor);
			axis.Multiply(factor);
		}

		public override void ClearSelection()
		{
			axis.ClearSelection(true);
			foreach (DataCell aData in data)
			{
				aData.SetSelected(false);
			}
		}

		public override void SetRevertPoint()
		{
			base.SetRevertPoint();
			axis.SetRevertPoint();
		}

		public override void UndoAll()
		{
			base.UndoAll();
			axis.UndoAll();
		}

		public override void UndoSelected()
		{
			base.UndoSelected();
			axis.UndoSelected();
		}

		public override byte[] SaveFile(byte[] binData)
		{
			binData = base.SaveFile(binData);
			binData = axis.SaveFile(binData);
			return binData;
		}

		public override void SetRealValue(string realValue)
		{
			axis.SetRealValue(realValue);
			base.SetRealValue(realValue);
		}

		public override void AddKeyListener(KeyListener listener)
		{
			base.AddKeyListener(listener);
			axis.AddKeyListener(listener);
		}

		public virtual void SelectCellAt(int y, Table1D axisType)
		{
			SelectCellAt(y);
		}

		public override void CursorUp()
		{
			if (!axis.IsStatic() && data[highlightY].IsSelected())
			{
				axis.SelectCellAt(highlightY);
			}
		}

		public override void CursorDown()
		{
			axis.CursorDown();
		}

		public override void CursorLeft()
		{
			if (highlightY > 0 && data[highlightY].IsSelected())
			{
				SelectCellAt(highlightY - 1);
			}
			else
			{
				axis.CursorLeft();
			}
		}

		public override void CursorRight()
		{
			if (highlightY < data.Length - 1 && data[highlightY].IsSelected())
			{
				SelectCellAt(highlightY + 1);
			}
			else
			{
				axis.CursorRight();
			}
		}

		public override void StartHighlight(int x, int y)
		{
			axis.ClearSelection();
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
			base.CopySelection();
			copySelection2DWorker = new CopySelection2DWorker(this);
			copySelection2DWorker.Execute();
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
			copyTable2DWorker = new CopyTable2DWorker(editor.GetSettings(), this);
			copyTable2DWorker.Execute();
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
			if (Sharpen.Runtime.EqualsIgnoreCase(pasteType, "[Table2D]"))
			{
				// Paste table
				string axisValues = "[Table1D]" + NEW_LINE + st.NextToken(NEW_LINE);
				string dataValues = "[Table1D]" + NEW_LINE + st.NextToken(NEW_LINE);
				// put axis in clipboard and paste
				Toolkit.GetDefaultToolkit().GetSystemClipboard().SetContents(new StringSelection(
					axisValues), null);
				axis.Paste();
				// put datavalues in clipboard and paste
				Toolkit.GetDefaultToolkit().GetSystemClipboard().SetContents(new StringSelection(
					dataValues), null);
				base.Paste();
				// reset clipboard
				Toolkit.GetDefaultToolkit().GetSystemClipboard().SetContents(new StringSelection(
					input), null);
			}
			else
			{
				if (Sharpen.Runtime.EqualsIgnoreCase(pasteType, "[Selection1D]"))
				{
					// paste selection
					if (data[highlightY].IsSelected())
					{
						base.Paste();
					}
					else
					{
						axis.Paste();
					}
				}
			}
			Colorize();
		}

		public override void SetAxisColor(Color axisColor)
		{
			axis.SetAxisColor(axisColor);
		}

		public override void ValidateScaling()
		{
			base.ValidateScaling();
			axis.ValidateScaling();
		}

		public override void SetScaleIndex(int scaleIndex)
		{
			base.SetScaleIndex(scaleIndex);
			axis.SetScaleByName(GetScale().GetName());
		}

		public override bool IsLiveDataSupported()
		{
			return !ParamChecker.IsNullOrEmpty(axis.GetLogParam());
		}

		public override bool IsButtonSelected()
		{
			return true;
		}

		protected internal override void HighlightLiveData()
		{
			if (overlayLog && frame.IsVisible())
			{
				AxisRange range = TableAxisUtil.GetLiveDataRangeForAxis(axis);
				ClearSelection();
				bool first = true;
				for (int i = range.GetStartIndex(); i <= range.GetEndIndex(); i++)
				{
					int x = 0;
					int y = i;
					if (axis.GetType() == TABLE_X_AXIS)
					{
						x = i;
						y = 0;
					}
					if (first)
					{
						StartHighlight(x, y);
						first = false;
					}
					else
					{
						Highlight(x, y);
					}
					DataCell cell = data[i];
					cell.SetLiveDataTrace(true);
					cell.SetDisplayValue(cell.GetRealValue() + (ParamChecker.IsNullOrEmpty(liveValue)
						 ? string.Empty : (':' + liveValue)));
				}
				StopHighlight();
				frame.GetToolBar().SetLiveDataValue(liveValue);
			}
		}

		public override void ClearLiveDataTrace()
		{
			foreach (DataCell cell in data)
			{
				cell.SetLiveDataTrace(false);
				cell.UpdateDisplayValue();
			}
		}

		public override void SetCompareDisplay(int compareDisplay)
		{
			base.SetCompareDisplay(compareDisplay);
			if (!axis.isStatic)
			{
				axis.SetCompareDisplay(compareDisplay);
			}
		}

		public override void SetCompareType(int compareType)
		{
			base.SetCompareType(compareType);
			if (!axis.isStatic)
			{
				axis.SetCompareType(compareType);
			}
		}

		public override void SetCompareTable(Table compareTable)
		{
			base.SetCompareTable(compareTable);
			if (compareTable == null || !(compareTable is RomRaider.Maps.Table2D))
			{
				return;
			}
			RomRaider.Maps.Table2D compareTable2D = (RomRaider.Maps.Table2D)compareTable;
			if (!axis.isStatic)
			{
				this.axis.SetCompareTable(compareTable2D.axis);
			}
		}

		public override void RefreshCellDisplay()
		{
			base.RefreshCellDisplay();
			if (!axis.isStatic)
			{
				axis.RefreshCellDisplay();
			}
		}

		public override void AddComparedToTable(Table table)
		{
			base.AddComparedToTable(table);
			if (!(table is RomRaider.Maps.Table2D))
			{
				return;
			}
			RomRaider.Maps.Table2D table2D = (RomRaider.Maps.Table2D)table;
			if (!axis.isStatic)
			{
				axis.AddComparedToTable(table2D.axis);
			}
		}
	}

	internal class CopySelection2DWorker : SwingWorker<Void, Void>
	{
		internal Table2D table;

		internal Table extendedTable;

		public CopySelection2DWorker(Table2D table)
		{
			this.table = table;
		}

		/// <exception cref="System.Exception"></exception>
		protected override Void DoInBackground()
		{
			table.GetAxis().CopySelection();
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

	internal class CopyTable2DWorker : SwingWorker<Void, Void>
	{
		internal Settings settings;

		internal Table2D table;

		public CopyTable2DWorker(Settings settings, Table2D table)
		{
			this.settings = settings;
			this.table = table;
		}

		/// <exception cref="System.Exception"></exception>
		protected override Void DoInBackground()
		{
			string tableHeader = settings.GetTable2DHeader();
			// create string
			StringBuilder output = new StringBuilder(tableHeader);
			output.Append(table.GetAxis().GetTableAsString()).Append(Table2D.NEW_LINE);
			output.Append(table.GetTableAsString());
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

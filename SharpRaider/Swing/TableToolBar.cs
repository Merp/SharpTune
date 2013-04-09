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
using Com.Ecm.Graphics;
using Com.Ecm.Graphics.Data;
using Java.Awt;
using Java.Awt.Event;
using Java.Math;
using Javax.Swing;
using Org.Apache.Log4j;
using RomRaider.Editor.Ecu;
using RomRaider.Maps;
using Sharpen;

namespace RomRaider.Swing
{
	[System.Serializable]
	public class TableToolBar : JToolBar, MouseListener, ItemListener, ActionListener
		, GraphDataListener
	{
		private const long serialVersionUID = 8697645329367637930L;

		private static readonly Logger LOGGER = Logger.GetLogger(typeof(RomRaider.Swing.TableToolBar
			));

		private readonly JButton incrementFine = new JButton();

		private readonly JButton decrementFine = new JButton();

		private readonly JButton incrementCoarse = new JButton();

		private readonly JButton decrementCoarse = new JButton();

		private readonly JButton enable3d = new JButton();

		private readonly JButton setValue = new JButton("Set");

		private readonly JButton multiply = new JButton("Mul");

		private readonly JFormattedTextField incrementByFine = new JFormattedTextField(new 
			DecimalFormat("#.####"));

		private readonly JFormattedTextField incrementByCoarse = new JFormattedTextField(
			new DecimalFormat("#.####"));

		private readonly JFormattedTextField setValueText = new JFormattedTextField(new DecimalFormat
			("#.####"));

		private readonly JComboBox scaleSelection = new JComboBox();

		private readonly JPanel liveDataPanel = new JPanel();

		private readonly JCheckBox overlayLog = new JCheckBox("Overlay Log");

		private readonly JButton clearOverlay = new JButton("Clear Overlay");

		private readonly JLabel liveDataValue = new JLabel();

		private readonly Uri incrementFineImage = GetType().GetResource("/graphics/icon-incfine.png"
			);

		private readonly Uri decrementFineImage = GetType().GetResource("/graphics/icon-decfine.png"
			);

		private readonly Uri incrementCoarseImage = GetType().GetResource("/graphics/icon-inccoarse.png"
			);

		private readonly Uri decrementCoarseImage = GetType().GetResource("/graphics/icon-deccoarse.png"
			);

		private readonly Uri enable3dImage = GetType().GetResource("/graphics/3d_render.png"
			);

		private Table table = null;

		private readonly ECUEditor editor;

		public TableToolBar(string name, ECUEditor editor) : base(name)
		{
			//private final String defaultToolBarName = "Table Tools";
			this.editor = editor;
			this.SetFloatable(true);
			this.SetRollover(true);
			FlowLayout toolBarLayout = new FlowLayout(FlowLayout.LEFT, 0, 0);
			this.SetLayout(toolBarLayout);
			//this.setBorder(BorderFactory.createTitledBorder("Table Tools"));
			this.UpdateIcons();
			JPanel finePanel = new JPanel();
			finePanel.Add(incrementFine);
			finePanel.Add(decrementFine);
			finePanel.Add(incrementByFine);
			this.Add(finePanel);
			JPanel coarsePanel = new JPanel();
			coarsePanel.Add(incrementCoarse);
			coarsePanel.Add(decrementCoarse);
			coarsePanel.Add(incrementByCoarse);
			this.Add(coarsePanel);
			JPanel setValuePanel = new JPanel();
			setValuePanel.Add(setValueText);
			setValuePanel.Add(setValue);
			setValuePanel.Add(multiply);
			this.Add(setValuePanel);
			//incrementFine.setPreferredSize(new Dimension(33, 33));
			incrementFine.SetBorder(BorderFactory.CreateLineBorder(new Color(150, 150, 150), 
				1));
			//decrementFine.setPreferredSize(new Dimension(33, 33));
			decrementFine.SetBorder(BorderFactory.CreateLineBorder(new Color(150, 150, 150), 
				1));
			//incrementCoarse.setPreferredSize(new Dimension(33, 33));
			incrementCoarse.SetBorder(BorderFactory.CreateLineBorder(new Color(150, 150, 150)
				, 1));
			//decrementCoarse.setPreferredSize(new Dimension(33, 33));
			decrementCoarse.SetBorder(BorderFactory.CreateLineBorder(new Color(150, 150, 150)
				, 1));
			//enable3d.setPreferredSize(new Dimension(33, 33));
			enable3d.SetBorder(BorderFactory.CreateLineBorder(new Color(150, 150, 150), 1));
			setValue.SetPreferredSize(new Dimension(33, 23));
			setValue.SetBorder(BorderFactory.CreateLineBorder(new Color(150, 150, 150), 1));
			multiply.SetPreferredSize(new Dimension(33, 23));
			multiply.SetBorder(BorderFactory.CreateLineBorder(new Color(150, 150, 150), 1));
			scaleSelection.SetPreferredSize(new Dimension(80, 23));
			scaleSelection.SetFont(new Font("Tahoma", Font.PLAIN, 11));
			clearOverlay.SetPreferredSize(new Dimension(75, 23));
			clearOverlay.SetBorder(BorderFactory.CreateLineBorder(new Color(150, 150, 150), 1
				));
			incrementByFine.SetAlignmentX(JTextArea.CENTER_ALIGNMENT);
			incrementByFine.SetAlignmentY(JTextArea.CENTER_ALIGNMENT);
			incrementByFine.SetPreferredSize(new Dimension(45, 23));
			incrementByCoarse.SetAlignmentX(JTextArea.CENTER_ALIGNMENT);
			incrementByCoarse.SetAlignmentY(JTextArea.CENTER_ALIGNMENT);
			incrementByCoarse.SetPreferredSize(new Dimension(45, 23));
			setValueText.SetAlignmentX(JTextArea.CENTER_ALIGNMENT);
			setValueText.SetAlignmentY(JTextArea.CENTER_ALIGNMENT);
			setValueText.SetPreferredSize(new Dimension(45, 23));
			incrementFine.SetToolTipText("Increment Value (Fine)");
			decrementFine.SetToolTipText("Decrement Value (Fine)");
			incrementCoarse.SetToolTipText("Increment Value (Coarse)");
			decrementCoarse.SetToolTipText("Decrement Value (Coarse)");
			enable3d.SetToolTipText("Render data in 3d");
			setValue.SetToolTipText("Set Absolute Value");
			setValueText.SetToolTipText("Set Absolute Value");
			incrementByFine.SetToolTipText("Fine Value Adjustment");
			incrementByCoarse.SetToolTipText("Coarse Value Adjustment");
			multiply.SetToolTipText("Multiply Value");
			overlayLog.SetToolTipText("Enable Overlay Of Real Time Log Data");
			clearOverlay.SetToolTipText("Clear Log Data Overlay Highlights");
			incrementFine.AddMouseListener(this);
			decrementFine.AddMouseListener(this);
			incrementCoarse.AddMouseListener(this);
			decrementCoarse.AddMouseListener(this);
			enable3d.AddMouseListener(this);
			setValue.AddMouseListener(this);
			multiply.AddMouseListener(this);
			scaleSelection.AddItemListener(this);
			overlayLog.AddItemListener(this);
			clearOverlay.AddActionListener(this);
			// key binding actions
			Action enterAction = new _AbstractAction_187(this);
			// set input mapping
			InputMap im = GetInputMap(WHEN_IN_FOCUSED_WINDOW);
			KeyStroke enter = KeyStroke.GetKeyStroke(KeyEvent.VK_ENTER, 0);
			im.Put(enter, "enterAction");
			GetActionMap().Put(im.Get(enter), enterAction);
			this.Add(enable3d);
			enable3d.SetEnabled(false);
			//this.add(scaleSelection);
			liveDataPanel.Add(overlayLog);
			liveDataPanel.Add(clearOverlay);
			//liveDataPanel.add(liveDataValue);
			this.Add(liveDataPanel);
			overlayLog.SetEnabled(false);
			clearOverlay.SetEnabled(false);
			incrementFine.GetInputMap().Put(enter, "enterAction");
			decrementFine.GetInputMap().Put(enter, "enterAction");
			incrementCoarse.GetInputMap().Put(enter, "enterAction");
			decrementCoarse.GetInputMap().Put(enter, "enterAction");
			incrementByFine.GetInputMap().Put(enter, "enterAction");
			incrementByCoarse.GetInputMap().Put(enter, "enterAction");
			setValueText.GetInputMap().Put(enter, "enterAction");
			setValue.GetInputMap().Put(enter, "enterAction");
			incrementFine.GetInputMap().Put(enter, "enterAction");
			this.SetEnabled(true);
		}

		private sealed class _AbstractAction_187 : AbstractAction
		{
			public _AbstractAction_187(TableToolBar _enclosing)
			{
				this._enclosing = _enclosing;
				this.serialVersionUID = -6008026264821746092L;
			}

			private const long serialVersionUID;

			public override void ActionPerformed(ActionEvent e)
			{
				Table getTable = this._enclosing.GetTable();
				if (null != getTable)
				{
					this._enclosing.GetTable().RequestFocus();
					this._enclosing.SetValue();
				}
			}

			private readonly TableToolBar _enclosing;
		}

		public virtual void UpdateIcons()
		{
			incrementFine.SetIcon(RescaleImageIcon(new ImageIcon(incrementFineImage), editor.
				GetSettings().GetTableIconScale()));
			decrementFine.SetIcon(RescaleImageIcon(new ImageIcon(decrementFineImage), editor.
				GetSettings().GetTableIconScale()));
			incrementCoarse.SetIcon(RescaleImageIcon(new ImageIcon(incrementCoarseImage), editor
				.GetSettings().GetTableIconScale()));
			decrementCoarse.SetIcon(RescaleImageIcon(new ImageIcon(decrementCoarseImage), editor
				.GetSettings().GetTableIconScale()));
			enable3d.SetIcon(RescaleImageIcon(new ImageIcon(enable3dImage), editor.GetSettings
				().GetTableIconScale()));
		}

		private ImageIcon RescaleImageIcon(ImageIcon imageIcon, int percentOfOriginal)
		{
			int newHeight = (int)(imageIcon.GetImage().GetHeight(this) * (percentOfOriginal *
				 .01));
			int newWidth = (int)(imageIcon.GetImage().GetWidth(this) * (percentOfOriginal * .01
				));
			imageIcon.SetImage(imageIcon.GetImage().GetScaledInstance(newWidth, newHeight, Image
				.SCALE_SMOOTH));
			return imageIcon;
		}

		public virtual void SetTable(Table table)
		{
			this.table = table;
		}

		public virtual Table GetTable()
		{
			return table;
		}

		public virtual void UpdateTableToolBar(Table table)
		{
			double fineIncrement = 0;
			double coarseIncrement = 0;
			Vector<Scale> scales = new Vector<Scale>();
			SetTable(table);
			if (null == table)
			{
				// disable the toolbar.
				ToggleTableToolBar(false);
				return;
			}
			try
			{
				// enable the toolbar.
				fineIncrement = Math.Abs(table.GetScale().GetFineIncrement());
				coarseIncrement = Math.Abs(table.GetScale().GetCoarseIncrement());
			}
			catch (Exception)
			{
			}
			// scaling units haven't been added yet -- no problem
			scales = table.GetScales();
			incrementByFine.SetValue(fineIncrement);
			incrementByCoarse.SetValue(coarseIncrement);
			this.overlayLog.SetSelected(table.GetOverlayLog());
			this.enable3d.SetEnabled(table.GetType() == Table.TABLE_3D);
			SetScales(scales);
			ToggleTableToolBar(true);
		}

		private void ToggleTableToolBar(bool enabled)
		{
			incrementFine.SetEnabled(enabled);
			decrementFine.SetEnabled(enabled);
			incrementCoarse.SetEnabled(enabled);
			decrementCoarse.SetEnabled(enabled);
			setValue.SetEnabled(enabled);
			multiply.SetEnabled(enabled);
			incrementByFine.SetEnabled(enabled);
			incrementByCoarse.SetEnabled(enabled);
			setValueText.SetEnabled(enabled);
			scaleSelection.SetEnabled(enabled);
			liveDataValue.SetEnabled(enabled);
			//Only enable the 3d button if table includes 3d data
			if (null != table && table.GetType() == Table.TABLE_3D && enabled)
			{
				enable3d.SetEnabled(true);
			}
			else
			{
				enable3d.SetEnabled(false);
			}
			if (null != table && table.IsLiveDataSupported() && enabled)
			{
				overlayLog.SetEnabled(true);
				clearOverlay.SetEnabled(true);
			}
			else
			{
				overlayLog.SetEnabled(false);
				clearOverlay.SetEnabled(false);
			}
		}

		public virtual void SetScales(Vector<Scale> scales)
		{
			// remove item listener to avoid null pointer exception when populating
			scaleSelection.RemoveItemListener(this);
			for (int i = 0; i < scales.Count; i++)
			{
				scaleSelection.AddItem(scales[i].GetName());
			}
			// and put it back
			scaleSelection.AddItemListener(this);
		}

		public virtual void MouseClicked(MouseEvent e)
		{
			if (null == table)
			{
				// case where no table is activated.
				return;
			}
			if (e.GetSource() == incrementCoarse)
			{
				IncrementCoarse();
			}
			else
			{
				if (e.GetSource() == decrementCoarse)
				{
					DecrementCoarse();
				}
				else
				{
					if (e.GetSource() == enable3d)
					{
						Enable3d();
					}
					else
					{
						if (e.GetSource() == incrementFine)
						{
							IncrementFine();
						}
						else
						{
							if (e.GetSource() == decrementFine)
							{
								DecrementFine();
							}
							else
							{
								if (e.GetSource() == multiply)
								{
									Multiply();
								}
								else
								{
									if (e.GetSource() == setValue)
									{
										SetValue();
									}
								}
							}
						}
					}
				}
			}
			table.Colorize();
		}

		public virtual void SetValue()
		{
			table.SetRealValue(setValueText.GetText());
		}

		public virtual void Multiply()
		{
			try
			{
				table.Multiply(double.ParseDouble(setValueText.GetText()));
			}
			catch (FormatException)
			{
			}
		}

		// Do Nothing.  setValueText is null or not a valid double.
		public virtual void IncrementFine()
		{
			table.Increment(double.ParseDouble(incrementByFine.GetValue().ToString()));
		}

		public virtual void DecrementFine()
		{
			table.Increment(0 - double.ParseDouble(incrementByFine.GetValue().ToString()));
		}

		public virtual void IncrementCoarse()
		{
			table.Increment(double.ParseDouble(incrementByCoarse.GetValue().ToString()));
		}

		public virtual void DecrementCoarse()
		{
			table.Increment(0 - double.ParseDouble(incrementByCoarse.GetValue().ToString()));
		}

		/// <summary>Method launches a 3d Frame.</summary>
		/// <remarks>Method launches a 3d Frame.</remarks>
		public virtual void Enable3d()
		{
			int rowCount = 0;
			int valueCount = 0;
			//Pull data into format 3d graph understands
			Vector<float[]> graphValues = new Vector<float[]>();
			if (table.GetType() == Table.TABLE_3D)
			{
				Table3D table3d = (Table3D)table;
				AList<AList<DataCell>> tableData = table3d.Get3dData();
				valueCount = tableData.Count;
				AList<DataCell> dataRow = tableData[0];
				rowCount = dataRow.Count;
				for (int j = (rowCount - 1); j >= 0; j--)
				{
					float[] rowValues = new float[valueCount];
					for (int i = 0; i < valueCount; i++)
					{
						DataCell theCell = tableData[i][j];
						rowValues[i] = (float)theCell.GetValue();
					}
					//float theValue = (float)theCell.getValue();
					//BigDecimal finalRoundedValue = new BigDecimal(theValue).setScale(2,BigDecimal.ROUND_HALF_UP);
					//rowValues[i] = finalRoundedValue.floatValue();
					graphValues.AddItem(rowValues);
				}
				Table1D xAxisTable1D = ((Table3D)table).GetXAxis();
				Table1D yAxisTable1D = ((Table3D)table).GetYAxis();
				//Gather x axis values
				DataCell[] dataCells = xAxisTable1D.GetData();
				int length = dataCells.Length;
				double[] xValues = new double[length];
				for (int i_1 = 0; i_1 < length; i_1++)
				{
					xValues[i_1] = dataCells[i_1].GetValue();
				}
				//double theValue = dataCells[i].getValue();
				//BigDecimal finalRoundedValue = new BigDecimal(theValue).setScale(2,BigDecimal.ROUND_HALF_UP);
				//xValues[i] = finalRoundedValue.doubleValue();
				//Gather y/z axis values
				dataCells = yAxisTable1D.GetData();
				length = dataCells.Length;
				double[] yValues = new double[length];
				for (int i_2 = 0; i_2 < length; i_2++)
				{
					double theValue = dataCells[i_2].GetValue();
					BigDecimal finalRoundedValue = new BigDecimal(theValue).SetScale(2, BigDecimal.ROUND_HALF_UP
						);
					yValues[i_2] = finalRoundedValue;
				}
				//Define Labels for graph
				string xLabel = ((Table3D)table).GetXAxis().GetName();
				string zLabel = ((Table3D)table).GetYAxis().GetName();
				string yLabel = ((Table3D)table).GetCategory();
				//TODO Figure out mix between heavy weight and lightweight components
				//Below is initial work on making graph3d a JInternal Frame
				double maxV = table.GetMax();
				double minV = table.GetMin();
				//TODO Remove this when above is working
				//***********
				LOGGER.Debug("Scale: " + maxV + "," + minV);
				//***********
				//Render 3d
				Graph3dFrameManager.OpenGraph3dFrame(graphValues, minV, maxV, xValues, yValues, xLabel
					, yLabel, zLabel, table.GetName());
				GraphData.AddGraphDataListener(this);
			}
		}

		public virtual void SetCoarseValue(double input)
		{
			incrementByCoarse.SetText(input.ToString());
			try
			{
				incrementByCoarse.CommitEdit();
			}
			catch (ParseException)
			{
			}
		}

		public virtual void SetFineValue(double input)
		{
			incrementByFine.SetText(input.ToString());
			try
			{
				incrementByFine.CommitEdit();
			}
			catch (ParseException)
			{
			}
		}

		public virtual void FocusSetValue(char input)
		{
			setValueText.RequestFocus();
			setValueText.SetText(input.ToString());
		}

		public virtual void SetInputMap(InputMap im)
		{
			incrementFine.SetInputMap(WHEN_FOCUSED, im);
			decrementFine.SetInputMap(WHEN_FOCUSED, im);
			incrementCoarse.SetInputMap(WHEN_FOCUSED, im);
			decrementCoarse.SetInputMap(WHEN_FOCUSED, im);
			setValue.SetInputMap(WHEN_FOCUSED, im);
		}

		public virtual void MousePressed(MouseEvent e)
		{
		}

		public virtual void MouseReleased(MouseEvent e)
		{
		}

		public virtual void MouseEntered(MouseEvent e)
		{
		}

		public virtual void MouseExited(MouseEvent e)
		{
		}

		public virtual void ItemStateChanged(ItemEvent e)
		{
			if (e.GetSource() == scaleSelection)
			{
				// scale changed
				table.SetScaleIndex(scaleSelection.GetSelectedIndex());
			}
			else
			{
				if (e.GetSource() == overlayLog)
				{
					// enable/disable log overlay and live data display
					table.SetOverlayLog(overlayLog.IsSelected());
				}
			}
		}

		public virtual void ActionPerformed(ActionEvent e)
		{
			if (e.GetSource() == clearOverlay)
			{
				// clear log overlay
				table.ClearLiveDataTrace();
			}
		}

		public virtual void SetLiveDataValue(string value)
		{
			liveDataValue.SetText(value);
		}

		// ******************************************
		// Code for listening to graph3d data changes
		// ******************************************
		public virtual void NewGraphData(int x, int z, float value)
		{
			Table3D table3d = (Table3D)table;
			table3d.SelectCellAt(x, table3d.GetSizeY() - z - 1);
			//Set the value
			table.SetRealValue(value.ToString());
		}

		public virtual void SelectStateChange(int x, int z, bool value)
		{
			if (value)
			{
				Table3D table3d = (Table3D)table;
				table3d.SelectCellAtWithoutClear(x, table3d.GetSizeY() - z - 1);
			}
			else
			{
				Table3D table3d = (Table3D)table;
				table3d.DeSelectCellAt(x, table3d.GetSizeY() - z - 1);
			}
		}
	}
}

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
using Java.Awt;
using Java.Awt.Event;
using Javax.Swing;
using Javax.Swing.Border;
using Org.Jfree.UI;
using RomRaider.Editor.Ecu;
using RomRaider.Logger.Ecu.Definition;
using RomRaider.Logger.Ecu.UI;
using RomRaider.Logger.Ecu.UI.Tab;
using RomRaider.Maps;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.Ecu.UI.Tab.Maf
{
	[System.Serializable]
	public sealed class MafControlPanel : JPanel
	{
		private const long serialVersionUID = 5787020251107365950L;

		private static readonly Org.Apache.Log4j.Logger LOGGER = Org.Apache.Log4j.Logger.
			GetLogger(typeof(RomRaider.Logger.Ecu.UI.Tab.Maf.MafControlPanel));

		private static readonly string COOLANT_TEMP = "P2";

		private static readonly string AF_CORRECTION_1 = "P3";

		private static readonly string AF_LEARNING_1 = "P4";

		private static readonly string ENGINE_SPEED = "P8";

		private static readonly string INTAKE_AIR_TEMP = "P11";

		private static readonly string MASS_AIR_FLOW = "P12";

		private static readonly string MASS_AIR_FLOW_V = "P18";

		private static readonly string AFR = "P58";

		private static readonly string CL_OL_16 = "E3";

		private static readonly string CL_OL_32 = "E33";

		private static readonly string TIP_IN_THROTTLE_16 = "E23";

		private static readonly string TIP_IN_THROTTLE_32 = "E54";

		private readonly JToggleButton recordDataButton = new JToggleButton("Record Data"
			);

		private readonly JTextField mafvMin = new JTextField("1.20", 3);

		private readonly JTextField mafvMax = new JTextField("2.60", 3);

		private readonly JTextField afrMin = new JTextField("13.0", 3);

		private readonly JTextField afrMax = new JTextField("16.0", 3);

		private readonly JTextField rpmMin = new JTextField("0", 3);

		private readonly JTextField rpmMax = new JTextField("4500", 3);

		private readonly JTextField mafMin = new JTextField("0", 3);

		private readonly JTextField mafMax = new JTextField("100", 3);

		private readonly JTextField iatMax = new JTextField("100", 3);

		private readonly JTextField coolantMin = new JTextField("70", 3);

		private readonly JTextField mafvChangeMax = new JTextField("0.1", 3);

		private readonly JComboBox afrSourceList = new JComboBox();

		private readonly DataRegistrationBroker broker;

		private readonly LoggerChartPanel chartPanel;

		private readonly ECUEditor ecuEditor;

		private readonly Component parent;

		private IList<ExternalData> externals = new AList<ExternalData>();

		private IList<EcuParameter> @params = new AList<EcuParameter>();

		private IList<EcuSwitch> switches = new AList<EcuSwitch>();

		public MafControlPanel(Component parent, DataRegistrationBroker broker, ECUEditor
			 ecuEditor, LoggerChartPanel chartPanel)
		{
			ParamChecker.CheckNotNull(parent, broker, chartPanel);
			this.parent = parent;
			this.broker = broker;
			this.chartPanel = chartPanel;
			this.ecuEditor = ecuEditor;
			AddControls();
		}

		public bool IsRecordData()
		{
			return recordDataButton.IsSelected();
		}

		public bool IsValidClOl(double value)
		{
			return value == 8;
		}

		public bool IsValidAfr(double value)
		{
			return CheckInRange("AFR", afrMin, afrMax, value);
		}

		public bool IsValidRpm(double value)
		{
			return CheckInRange("RPM", rpmMin, rpmMax, value);
		}

		public bool IsValidMaf(double value)
		{
			return CheckInRange("MAF", mafMin, mafMax, value);
		}

		public bool IsValidMafv(double value)
		{
			return CheckInRange("MAFv", mafvMin, mafvMax, value);
		}

		public bool IsValidCoolantTemp(double value)
		{
			return CheckGreaterThan("Coolant Temp.", coolantMin, value);
		}

		public bool IsValidIntakeAirTemp(double value)
		{
			return CheckLessThan("Intake Air Temp.", iatMax, value);
		}

		public bool IsValidMafvChange(double value)
		{
			return CheckLessThan("dMAFv/dt", mafvChangeMax, value);
		}

		public bool IsValidTipInThrottle(double value)
		{
			return value == 0.0;
		}

		private bool CheckInRange(string name, JTextField min, JTextField max, double value
			)
		{
			if (IsValidRange(min, max))
			{
				return InRange(value, min, max);
			}
			else
			{
				JOptionPane.ShowMessageDialog(parent, "Invalid " + name + " range specified.", "Error"
					, JOptionPane.ERROR_MESSAGE);
				recordDataButton.SetSelected(false);
				return false;
			}
		}

		private bool CheckGreaterThan(string name, JTextField min, double value)
		{
			if (IsNumber(min))
			{
				return value >= ParseDouble(min);
			}
			else
			{
				JOptionPane.ShowMessageDialog(parent, "Invalid " + name + " specified.", "Error", 
					JOptionPane.ERROR_MESSAGE);
				recordDataButton.SetSelected(false);
				return false;
			}
		}

		private bool CheckLessThan(string name, JTextField max, double value)
		{
			if (IsNumber(max))
			{
				return value <= ParseDouble(max);
			}
			else
			{
				JOptionPane.ShowMessageDialog(parent, "Invalid " + name + " specified.", "Error", 
					JOptionPane.ERROR_MESSAGE);
				recordDataButton.SetSelected(false);
				return false;
			}
		}

		private void AddControls()
		{
			JPanel panel = new JPanel();
			GridBagLayout gridBagLayout = new GridBagLayout();
			panel.SetLayout(gridBagLayout);
			//        add(panel, gridBagLayout, buildAfrSourcePanel(), 0, 0, 1, HORIZONTAL);
			//        add(panel, gridBagLayout, buildFilterPanel(), 0, 1, 1, HORIZONTAL);
			//        add(panel, gridBagLayout, buildInterpolatePanel(), 0, 2, 1, HORIZONTAL);
			//        add(panel, gridBagLayout, buildUpdateMafPanel(), 0, 3, 1, HORIZONTAL);
			//        add(panel, gridBagLayout, buildResetPanel(), 0, 4, 1, HORIZONTAL);
			Add(panel, gridBagLayout, BuildFilterPanel(), 0, 0, 1, GridBagConstraints.HORIZONTAL
				);
			Add(panel, gridBagLayout, BuildInterpolatePanel(), 0, 1, 1, GridBagConstraints.HORIZONTAL
				);
			Add(panel, gridBagLayout, BuildUpdateMafPanel(), 0, 2, 1, GridBagConstraints.HORIZONTAL
				);
			Add(panel, gridBagLayout, BuildResetPanel(), 0, 3, 1, GridBagConstraints.HORIZONTAL
				);
			Add(panel);
		}

		private void Add(JPanel panel, GridBagLayout gridBagLayout, JComponent component, 
			int x, int y, int spanX, int fillType)
		{
			GridBagConstraints constraints = BuildBaseConstraints();
			UpdateConstraints(constraints, x, y, spanX, 1, 1, 1, fillType);
			gridBagLayout.SetConstraints(component, constraints);
			panel.Add(component);
		}

		private JPanel BuildAfrSourcePanel()
		{
			JPanel panel = new JPanel();
			panel.SetBorder(new TitledBorder("AFR Source"));
			panel.Add(afrSourceList);
			return panel;
		}

		private JPanel BuildResetPanel()
		{
			JPanel panel = new JPanel();
			panel.SetBorder(new TitledBorder("Reset"));
			panel.Add(BuildResetButton());
			return panel;
		}

		private JPanel BuildUpdateMafPanel()
		{
			JPanel panel = new JPanel();
			panel.SetBorder(new TitledBorder("Update MAF"));
			GridBagLayout gridBagLayout = new GridBagLayout();
			panel.SetLayout(gridBagLayout);
			AddMinMaxFilter(panel, gridBagLayout, "MAFv Range", mafvMin, mafvMax, 0);
			AddComponent(panel, gridBagLayout, BuildUpdateMafButton(), 3);
			return panel;
		}

		private JPanel BuildInterpolatePanel()
		{
			JPanel panel = new JPanel();
			panel.SetBorder(new TitledBorder("Interpolate"));
			GridBagLayout gridBagLayout = new GridBagLayout();
			panel.SetLayout(gridBagLayout);
			JComboBox orderComboBox = BuildPolyOrderComboBox();
			AddLabeledComponent(panel, gridBagLayout, "Poly. order", orderComboBox, 0);
			AddComponent(panel, gridBagLayout, BuildInterpolateButton(orderComboBox), 2);
			return panel;
		}

		private void AddLabeledComponent(JPanel panel, GridBagLayout gridBagLayout, string
			 name, JComponent component, int y)
		{
			Add(panel, gridBagLayout, new JLabel(name), 0, y, 3, GridBagConstraints.HORIZONTAL
				);
			Add(panel, gridBagLayout, component, 0, y + 1, 3, GridBagConstraints.NONE);
		}

		private JPanel BuildFilterPanel()
		{
			JPanel panel = new JPanel();
			panel.SetBorder(new TitledBorder("Filter Data"));
			GridBagLayout gridBagLayout = new GridBagLayout();
			panel.SetLayout(gridBagLayout);
			AddMinMaxFilter(panel, gridBagLayout, "AFR Range", afrMin, afrMax, 0);
			AddMinMaxFilter(panel, gridBagLayout, "RPM Range", rpmMin, rpmMax, 3);
			AddMinMaxFilter(panel, gridBagLayout, "MAF Range (g/s)", mafMin, mafMax, 6);
			AddLabeledComponent(panel, gridBagLayout, "Min. Coolant Temp.", coolantMin, 9);
			AddLabeledComponent(panel, gridBagLayout, "Max. Intake Temp.", iatMax, 12);
			AddLabeledComponent(panel, gridBagLayout, "Max. dMAFv/dt (V/s)", mafvChangeMax, 15
				);
			AddComponent(panel, gridBagLayout, BuildRecordDataButton(), 18);
			return panel;
		}

		private JToggleButton BuildRecordDataButton()
		{
			recordDataButton.AddActionListener(new _ActionListener_269(this));
			//                    afrSourceList.setEnabled(false);
			//                    registerAfr();
			//                    deregisterAfr();
			//                    afrSourceList.setEnabled(true);
			return recordDataButton;
		}

		private sealed class _ActionListener_269 : ActionListener
		{
			public _ActionListener_269(MafControlPanel _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void ActionPerformed(ActionEvent actionEvent)
			{
				if (this._enclosing.recordDataButton.IsSelected())
				{
					this._enclosing.RegisterData(RomRaider.Logger.Ecu.UI.Tab.Maf.MafControlPanel.COOLANT_TEMP
						, RomRaider.Logger.Ecu.UI.Tab.Maf.MafControlPanel.AF_CORRECTION_1, RomRaider.Logger.Ecu.UI.Tab.Maf.MafControlPanel
						.AF_LEARNING_1, RomRaider.Logger.Ecu.UI.Tab.Maf.MafControlPanel.ENGINE_SPEED, RomRaider.Logger.Ecu.UI.Tab.Maf.MafControlPanel
						.INTAKE_AIR_TEMP, RomRaider.Logger.Ecu.UI.Tab.Maf.MafControlPanel.MASS_AIR_FLOW, 
						RomRaider.Logger.Ecu.UI.Tab.Maf.MafControlPanel.MASS_AIR_FLOW_V, RomRaider.Logger.Ecu.UI.Tab.Maf.MafControlPanel
						.CL_OL_16, RomRaider.Logger.Ecu.UI.Tab.Maf.MafControlPanel.CL_OL_32, RomRaider.Logger.Ecu.UI.Tab.Maf.MafControlPanel
						.TIP_IN_THROTTLE_16, RomRaider.Logger.Ecu.UI.Tab.Maf.MafControlPanel.TIP_IN_THROTTLE_32
						);
				}
				else
				{
					this._enclosing.DeregisterData(RomRaider.Logger.Ecu.UI.Tab.Maf.MafControlPanel.COOLANT_TEMP
						, RomRaider.Logger.Ecu.UI.Tab.Maf.MafControlPanel.AF_CORRECTION_1, RomRaider.Logger.Ecu.UI.Tab.Maf.MafControlPanel
						.AF_LEARNING_1, RomRaider.Logger.Ecu.UI.Tab.Maf.MafControlPanel.ENGINE_SPEED, RomRaider.Logger.Ecu.UI.Tab.Maf.MafControlPanel
						.INTAKE_AIR_TEMP, RomRaider.Logger.Ecu.UI.Tab.Maf.MafControlPanel.MASS_AIR_FLOW, 
						RomRaider.Logger.Ecu.UI.Tab.Maf.MafControlPanel.MASS_AIR_FLOW_V, RomRaider.Logger.Ecu.UI.Tab.Maf.MafControlPanel
						.CL_OL_16, RomRaider.Logger.Ecu.UI.Tab.Maf.MafControlPanel.CL_OL_32, RomRaider.Logger.Ecu.UI.Tab.Maf.MafControlPanel
						.TIP_IN_THROTTLE_16, RomRaider.Logger.Ecu.UI.Tab.Maf.MafControlPanel.TIP_IN_THROTTLE_32
						);
				}
			}

			private readonly MafControlPanel _enclosing;
		}

		private void RegisterAfr()
		{
			LoggerData data = GetSelectedAfrSource();
			if (data != null)
			{
				broker.RegisterLoggerDataForLogging(data);
			}
		}

		private void DeregisterAfr()
		{
			LoggerData data = GetSelectedAfrSource();
			if (data != null)
			{
				broker.DeregisterLoggerDataFromLogging(data);
			}
		}

		private LoggerData GetSelectedAfrSource()
		{
			KeyedComboBoxModel model = (KeyedComboBoxModel)afrSourceList.GetModel();
			return (LoggerData)model.GetSelectedKey();
		}

		private void RegisterData(params string[] ids)
		{
			foreach (string id in ids)
			{
				LoggerData data = FindData(id);
				if (data != null)
				{
					broker.RegisterLoggerDataForLogging(data);
				}
			}
		}

		private void DeregisterData(params string[] ids)
		{
			foreach (string id in ids)
			{
				LoggerData data = FindData(id);
				if (data != null)
				{
					broker.DeregisterLoggerDataFromLogging(data);
				}
			}
		}

		private LoggerData FindData(string id)
		{
			foreach (EcuParameter param in @params)
			{
				if (id.Equals(param.GetId()))
				{
					return param;
				}
			}
			foreach (EcuSwitch sw in switches)
			{
				if (id.Equals(sw.GetId()))
				{
					return sw;
				}
			}
			foreach (ExternalData external in externals)
			{
				if (id.Equals(external.GetId()))
				{
					return external;
				}
			}
			return null;
		}

		private void AddComponent(JPanel panel, GridBagLayout gridBagLayout, JComponent component
			, int y)
		{
			Add(panel, gridBagLayout, component, 0, y, 3, GridBagConstraints.HORIZONTAL);
		}

		private void AddMinMaxFilter(JPanel panel, GridBagLayout gridBagLayout, string name
			, JTextField min, JTextField max, int y)
		{
			Add(panel, gridBagLayout, new JLabel(name), 0, y, 3, GridBagConstraints.HORIZONTAL
				);
			y += 1;
			Add(panel, gridBagLayout, min, 0, y, 1, GridBagConstraints.NONE);
			Add(panel, gridBagLayout, new JLabel(" - "), 1, y, 1, GridBagConstraints.NONE);
			Add(panel, gridBagLayout, max, 2, y, 1, GridBagConstraints.NONE);
		}

		private GridBagConstraints BuildBaseConstraints()
		{
			GridBagConstraints constraints = new GridBagConstraints();
			constraints.anchor = GridBagConstraints.CENTER;
			constraints.fill = GridBagConstraints.NONE;
			return constraints;
		}

		private void UpdateConstraints(GridBagConstraints constraints, int gridx, int gridy
			, int gridwidth, int gridheight, int weightx, int weighty, int fill)
		{
			constraints.gridx = gridx;
			constraints.gridy = gridy;
			constraints.gridwidth = gridwidth;
			constraints.gridheight = gridheight;
			constraints.weightx = weightx;
			constraints.weighty = weighty;
			constraints.fill = fill;
		}

		private void UpdateAfrSourceList()
		{
			IList<LoggerData> sources = new AList<LoggerData>();
			LoggerData afr = FindData(AFR);
			if (afr != null)
			{
				sources.AddItem(afr);
			}
			Sharpen.Collections.AddAll(sources, externals);
			IList<string> keys = new AList<string>();
			foreach (LoggerData source in sources)
			{
				keys.AddItem(source.GetName());
			}
			afrSourceList.SetModel(new KeyedComboBoxModel(Sharpen.Collections.ToArray(sources
				, new LoggerData[sources.Count]), Sharpen.Collections.ToArray(keys, new string[keys
				.Count])));
			afrSourceList.SetSelectedIndex(0);
		}

		private JButton BuildResetButton()
		{
			JButton resetButton = new JButton("Reset Data");
			resetButton.AddActionListener(new _ActionListener_369(this));
			return resetButton;
		}

		private sealed class _ActionListener_369 : ActionListener
		{
			public _ActionListener_369(MafControlPanel _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void ActionPerformed(ActionEvent actionEvent)
			{
				this._enclosing.chartPanel.Clear();
				this._enclosing.parent.Repaint();
			}

			private readonly MafControlPanel _enclosing;
		}

		private JButton BuildInterpolateButton(JComboBox orderComboBox)
		{
			JButton interpolateButton = new JButton("Interpolate");
			interpolateButton.AddActionListener(new _ActionListener_380(this, orderComboBox));
			return interpolateButton;
		}

		private sealed class _ActionListener_380 : ActionListener
		{
			public _ActionListener_380(MafControlPanel _enclosing, JComboBox orderComboBox)
			{
				this._enclosing = _enclosing;
				this.orderComboBox = orderComboBox;
			}

			public void ActionPerformed(ActionEvent actionEvent)
			{
				this._enclosing.chartPanel.Interpolate((int)orderComboBox.GetSelectedItem());
				this._enclosing.parent.Repaint();
			}

			private readonly MafControlPanel _enclosing;

			private readonly JComboBox orderComboBox;
		}

		private JComboBox BuildPolyOrderComboBox()
		{
			JComboBox orderComboBox = new JComboBox(new object[] { 3, 4, 5, 6, 7, 8, 9, 10, 11
				, 12, 13, 14, 15, 16, 17, 18, 19, 20 });
			orderComboBox.SetSelectedItem(10);
			return orderComboBox;
		}

		private JButton BuildUpdateMafButton()
		{
			JButton updateMafButton = new JButton("Update MAF");
			updateMafButton.AddActionListener(new _ActionListener_397(this));
			return updateMafButton;
		}

		private sealed class _ActionListener_397 : ActionListener
		{
			public _ActionListener_397(MafControlPanel _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void ActionPerformed(ActionEvent actionEvent)
			{
				try
				{
					if (this._enclosing.ShowUpdateMafConfirmation() == JOptionPane.OK_OPTION)
					{
						Table2D table = this._enclosing.GetMafTable(this._enclosing.ecuEditor);
						if (table != null)
						{
							if (this._enclosing.IsValidRange(this._enclosing.mafvMin, this._enclosing.mafvMax
								))
							{
								DataCell[] axisCells = table.GetAxis().GetData();
								double[] x = new double[axisCells.Length];
								for (int i = 0; i < axisCells.Length; i++)
								{
									DataCell cell = axisCells[i];
									x[i] = cell.GetValue();
								}
								double[] percentChange = this._enclosing.chartPanel.Calculate(x);
								DataCell[] dataCells = table.GetData();
								for (int i_1 = 0; i_1 < dataCells.Length; i_1++)
								{
									if (this._enclosing.InRange(axisCells[i_1].GetValue(), this._enclosing.mafvMin, this
										._enclosing.mafvMax))
									{
										DataCell cell = dataCells[i_1];
										double value = cell.GetValue();
										cell.SetRealValue(string.Empty + (value * (1.0 + percentChange[i_1] / 100.0)));
									}
								}
								table.Colorize();
							}
							else
							{
								JOptionPane.ShowMessageDialog(this._enclosing.parent, "Invalid MAFv range specified."
									, "Error", JOptionPane.ERROR_MESSAGE);
							}
						}
						else
						{
							JOptionPane.ShowMessageDialog(this._enclosing.parent, "Error finding MAF Sensor Scaling table."
								, "Error", JOptionPane.ERROR_MESSAGE);
						}
					}
				}
				catch (Exception e)
				{
					string msg = e.Message != null && e.Message.Length > 0 ? e.Message : "Unknown";
					JOptionPane.ShowMessageDialog(this._enclosing.parent, "Error: " + msg, "Error", JOptionPane
						.ERROR_MESSAGE);
				}
			}

			private readonly MafControlPanel _enclosing;
		}

		private bool AreNumbers(params JTextField[] textFields)
		{
			foreach (JTextField field in textFields)
			{
				if (!IsNumber(field))
				{
					return false;
				}
			}
			return true;
		}

		private bool IsValidRange(JTextField min, JTextField max)
		{
			return AreNumbers(min, max) && ParseDouble(min) < ParseDouble(max);
		}

		private bool IsNumber(JTextField textField)
		{
			try
			{
				ParseDouble(textField);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		private bool InRange(double val, double min, double max)
		{
			return val >= min && val <= max;
		}

		private bool InRange(double value, JTextField min, JTextField max)
		{
			return InRange(value, ParseDouble(min), ParseDouble(max));
		}

		private double ParseDouble(JTextField field)
		{
			return double.ParseDouble(field.GetText().Trim());
		}

		private int ShowUpdateMafConfirmation()
		{
			return JOptionPane.ShowConfirmDialog(parent, "Update MAF Sensor Scaling table?", 
				"Confirm Update", JOptionPane.YES_NO_OPTION, JOptionPane.WARNING_MESSAGE);
		}

		private Table2D GetMafTable(ECUEditor ecuEditor)
		{
			return GetTable(ecuEditor, "MAF Sensor Scaling");
		}

		private T GetTable<T>(ECUEditor ecuEditor, string name) where T:Table
		{
			try
			{
				Rom rom = ecuEditor.GetLastSelectedRom();
				return (T)TableFinder.FindTableStartsWith(rom, name);
			}
			catch (Exception e)
			{
				LOGGER.Warn("Error getting " + name + " table", e);
				return null;
			}
		}

		public void SetEcuParams(IList<EcuParameter> @params)
		{
			this.@params = new AList<EcuParameter>(@params);
		}

		//        updateAfrSourceList();
		public void SetEcuSwitches(IList<EcuSwitch> switches)
		{
			this.switches = new AList<EcuSwitch>(switches);
		}

		public void SetExternalDatas(IList<ExternalData> externals)
		{
			this.externals = new AList<ExternalData>(externals);
		}
		//        updateAfrSourceList();
	}
}

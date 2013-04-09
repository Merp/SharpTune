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
using RomRaider.Editor.Ecu;
using RomRaider.Logger.Ecu.Definition;
using RomRaider.Logger.Ecu.UI;
using RomRaider.Logger.Ecu.UI.Tab;
using RomRaider.Maps;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.Ecu.UI.Tab.Injector
{
	[System.Serializable]
	public sealed class InjectorControlPanel : JPanel
	{
		private const long serialVersionUID = -3570410894599258706L;

		private static readonly Org.Apache.Log4j.Logger LOGGER = Org.Apache.Log4j.Logger.
			GetLogger(typeof(RomRaider.Logger.Ecu.UI.Tab.Injector.InjectorControlPanel));

		private static readonly string COOLANT_TEMP = "P2";

		private static readonly string ENGINE_SPEED = "P8";

		private static readonly string INTAKE_AIR_TEMP = "P11";

		private static readonly string MASS_AIR_FLOW = "P12";

		private static readonly string MASS_AIR_FLOW_V = "P18";

		private static readonly string AFR = "P58";

		private static readonly string CL_OL_16 = "E3";

		private static readonly string CL_OL_32 = "E33";

		private static readonly string PULSE_WIDTH_16 = "E28";

		private static readonly string PULSE_WIDTH_32 = "E60";

		private static readonly string TIP_IN_THROTTLE_16 = "E23";

		private static readonly string TIP_IN_THROTTLE_32 = "E54";

		private static readonly string ENGINE_LOAD_16 = "E2";

		private static readonly string ENGINE_LOAD_32 = "E32";

		private readonly JToggleButton recordDataButton = new JToggleButton("Record Data"
			);

		private readonly JTextField mafvMin = new JTextField("1.20", 3);

		private readonly JTextField mafvMax = new JTextField("2.60", 3);

		private readonly JTextField afrMin = new JTextField("13.0", 3);

		private readonly JTextField afrMax = new JTextField("16.0", 3);

		private readonly JTextField rpmMin = new JTextField("0", 3);

		private readonly JTextField rpmMax = new JTextField("4500", 3);

		private readonly JTextField mafMin = new JTextField("20", 3);

		private readonly JTextField mafMax = new JTextField("100", 3);

		private readonly JTextField iatMax = new JTextField("45", 3);

		private readonly JTextField coolantMin = new JTextField("70", 3);

		private readonly JTextField mafvChangeMax = new JTextField("0.1", 3);

		private readonly JTextField fuelStoichAfr = new JTextField("14.7", 5);

		private readonly JTextField fuelDensity = new JTextField("732", 5);

		private readonly JTextField flowScaling = new JTextField(string.Empty, 5);

		private readonly JTextField latencyOffset = new JTextField(string.Empty, 5);

		private readonly DataRegistrationBroker broker;

		private readonly LoggerChartPanel chartPanel;

		private readonly ECUEditor ecuEditor;

		private readonly Component parent;

		private IList<EcuParameter> @params;

		private IList<EcuSwitch> switches;

		private IList<ExternalData> externals;

		public InjectorControlPanel(Component parent, DataRegistrationBroker broker, ECUEditor
			 ecuEditor, LoggerChartPanel chartPanel)
		{
			ParamChecker.CheckNotNull(parent, broker, chartPanel);
			this.broker = broker;
			this.parent = parent;
			this.chartPanel = chartPanel;
			this.ecuEditor = ecuEditor;
			AddControls();
		}

		public double GetFuelStoichAfr()
		{
			return GetProperty(fuelStoichAfr, "Fuel Stoich. AFR");
		}

		public double GetFuelDensity()
		{
			return GetProperty(fuelDensity, "Fuel Density");
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

		private double GetProperty(JTextField field, string name)
		{
			if (IsNumber(field))
			{
				return ParseDouble(field);
			}
			JOptionPane.ShowMessageDialog(parent, "Invalid " + name + " value specified.", "Error"
				, JOptionPane.ERROR_MESSAGE);
			recordDataButton.SetSelected(false);
			return 0.0;
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
			Add(panel, gridBagLayout, BuildFuelPropertiesPanel(), 0, 0, 1, GridBagConstraints
				.HORIZONTAL);
			Add(panel, gridBagLayout, BuildFilterPanel(), 0, 1, 1, GridBagConstraints.HORIZONTAL
				);
			Add(panel, gridBagLayout, BuildInterpolatePanel(), 0, 2, 1, GridBagConstraints.HORIZONTAL
				);
			Add(panel, gridBagLayout, BuildUpdateInjectorPanel(), 0, 3, 1, GridBagConstraints
				.HORIZONTAL);
			Add(panel, gridBagLayout, BuildResetPanel(), 0, 4, 1, GridBagConstraints.HORIZONTAL
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

		private JPanel BuildResetPanel()
		{
			JPanel panel = new JPanel();
			panel.SetBorder(new TitledBorder("Reset"));
			panel.Add(BuildResetButton());
			return panel;
		}

		private JPanel BuildInterpolatePanel()
		{
			JPanel panel = new JPanel();
			panel.SetBorder(new TitledBorder("Interpolate"));
			GridBagLayout gridBagLayout = new GridBagLayout();
			panel.SetLayout(gridBagLayout);
			AddComponent(panel, gridBagLayout, BuildInterpolateButton(), 2);
			return panel;
		}

		private JPanel BuildUpdateInjectorPanel()
		{
			JPanel panel = new JPanel();
			panel.SetBorder(new TitledBorder("Update Injector"));
			GridBagLayout gridBagLayout = new GridBagLayout();
			panel.SetLayout(gridBagLayout);
			flowScaling.SetEditable(false);
			latencyOffset.SetEditable(false);
			AddLabeledComponent(panel, gridBagLayout, "Flow Scaling (cc/min)", flowScaling, 0
				);
			AddComponent(panel, gridBagLayout, BuildUpdateInjectorScalerButton(), 2);
			AddLabeledComponent(panel, gridBagLayout, "Latency Offset (ms)", latencyOffset, 3
				);
			AddComponent(panel, gridBagLayout, BuildUpdateInjectorLatencyButton(), 5);
			return panel;
		}

		private void AddLabeledComponent(JPanel panel, GridBagLayout gridBagLayout, string
			 name, JComponent component, int y)
		{
			Add(panel, gridBagLayout, new JLabel(name), 0, y, 3, GridBagConstraints.HORIZONTAL
				);
			Add(panel, gridBagLayout, component, 0, y + 1, 3, GridBagConstraints.NONE);
		}

		private JPanel BuildFuelPropertiesPanel()
		{
			JPanel panel = new JPanel();
			panel.SetBorder(new TitledBorder("Fuel Properties"));
			GridBagLayout gridBagLayout = new GridBagLayout();
			panel.SetLayout(gridBagLayout);
			AddLabeledComponent(panel, gridBagLayout, "Stoich. AFR", fuelStoichAfr, 0);
			AddLabeledComponent(panel, gridBagLayout, "Density (kg/m3)", fuelDensity, 3);
			return panel;
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
			recordDataButton.AddActionListener(new _ActionListener_296(this));
			return recordDataButton;
		}

		private sealed class _ActionListener_296 : ActionListener
		{
			public _ActionListener_296(InjectorControlPanel _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void ActionPerformed(ActionEvent actionEvent)
			{
				if (this._enclosing.recordDataButton.IsSelected())
				{
					this._enclosing.RegisterData(RomRaider.Logger.Ecu.UI.Tab.Injector.InjectorControlPanel
						.COOLANT_TEMP, RomRaider.Logger.Ecu.UI.Tab.Injector.InjectorControlPanel.ENGINE_SPEED
						, RomRaider.Logger.Ecu.UI.Tab.Injector.InjectorControlPanel.INTAKE_AIR_TEMP, RomRaider.Logger.Ecu.UI.Tab.Injector.InjectorControlPanel
						.MASS_AIR_FLOW, RomRaider.Logger.Ecu.UI.Tab.Injector.InjectorControlPanel.MASS_AIR_FLOW_V
						, RomRaider.Logger.Ecu.UI.Tab.Injector.InjectorControlPanel.AFR, RomRaider.Logger.Ecu.UI.Tab.Injector.InjectorControlPanel
						.CL_OL_16, RomRaider.Logger.Ecu.UI.Tab.Injector.InjectorControlPanel.CL_OL_32, RomRaider.Logger.Ecu.UI.Tab.Injector.InjectorControlPanel
						.TIP_IN_THROTTLE_16, RomRaider.Logger.Ecu.UI.Tab.Injector.InjectorControlPanel.TIP_IN_THROTTLE_32
						, RomRaider.Logger.Ecu.UI.Tab.Injector.InjectorControlPanel.PULSE_WIDTH_16, RomRaider.Logger.Ecu.UI.Tab.Injector.InjectorControlPanel
						.PULSE_WIDTH_32, RomRaider.Logger.Ecu.UI.Tab.Injector.InjectorControlPanel.ENGINE_LOAD_16
						, RomRaider.Logger.Ecu.UI.Tab.Injector.InjectorControlPanel.ENGINE_LOAD_32);
				}
				else
				{
					this._enclosing.DeregisterData(RomRaider.Logger.Ecu.UI.Tab.Injector.InjectorControlPanel
						.COOLANT_TEMP, RomRaider.Logger.Ecu.UI.Tab.Injector.InjectorControlPanel.ENGINE_SPEED
						, RomRaider.Logger.Ecu.UI.Tab.Injector.InjectorControlPanel.INTAKE_AIR_TEMP, RomRaider.Logger.Ecu.UI.Tab.Injector.InjectorControlPanel
						.MASS_AIR_FLOW, RomRaider.Logger.Ecu.UI.Tab.Injector.InjectorControlPanel.MASS_AIR_FLOW_V
						, RomRaider.Logger.Ecu.UI.Tab.Injector.InjectorControlPanel.AFR, RomRaider.Logger.Ecu.UI.Tab.Injector.InjectorControlPanel
						.CL_OL_16, RomRaider.Logger.Ecu.UI.Tab.Injector.InjectorControlPanel.CL_OL_32, RomRaider.Logger.Ecu.UI.Tab.Injector.InjectorControlPanel
						.TIP_IN_THROTTLE_16, RomRaider.Logger.Ecu.UI.Tab.Injector.InjectorControlPanel.TIP_IN_THROTTLE_32
						, RomRaider.Logger.Ecu.UI.Tab.Injector.InjectorControlPanel.PULSE_WIDTH_16, RomRaider.Logger.Ecu.UI.Tab.Injector.InjectorControlPanel
						.PULSE_WIDTH_32, RomRaider.Logger.Ecu.UI.Tab.Injector.InjectorControlPanel.ENGINE_LOAD_16
						, RomRaider.Logger.Ecu.UI.Tab.Injector.InjectorControlPanel.ENGINE_LOAD_32);
				}
			}

			private readonly InjectorControlPanel _enclosing;
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
			LOGGER.Warn("Logger data not found for id: " + id);
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

		private JButton BuildResetButton()
		{
			JButton resetButton = new JButton("Reset Data");
			resetButton.AddActionListener(new _ActionListener_367(this));
			return resetButton;
		}

		private sealed class _ActionListener_367 : ActionListener
		{
			public _ActionListener_367(InjectorControlPanel _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void ActionPerformed(ActionEvent actionEvent)
			{
				this._enclosing.chartPanel.Clear();
				this._enclosing.parent.Repaint();
			}

			private readonly InjectorControlPanel _enclosing;
		}

		private JButton BuildInterpolateButton()
		{
			JButton interpolateButton = new JButton("Interpolate");
			interpolateButton.AddActionListener(new _ActionListener_378(this));
			return interpolateButton;
		}

		private sealed class _ActionListener_378 : ActionListener
		{
			public _ActionListener_378(InjectorControlPanel _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void ActionPerformed(ActionEvent actionEvent)
			{
				this._enclosing.chartPanel.Interpolate(1);
				double[] coefficients = this._enclosing.chartPanel.GetPolynomialCoefficients();
				double scaling = coefficients[0] * 1000 * 60;
				DecimalFormat format = new DecimalFormat("0.00");
				this._enclosing.flowScaling.SetText(format.Format(scaling));
				double offset = -1 * coefficients[1] / coefficients[0];
				this._enclosing.latencyOffset.SetText(format.Format(offset));
				this._enclosing.parent.Repaint();
			}

			private readonly InjectorControlPanel _enclosing;
		}

		private JButton BuildUpdateInjectorScalerButton()
		{
			JButton updateButton = new JButton("Update Scaling");
			updateButton.AddActionListener(new _ActionListener_395(this));
			return updateButton;
		}

		private sealed class _ActionListener_395 : ActionListener
		{
			public _ActionListener_395(InjectorControlPanel _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void ActionPerformed(ActionEvent actionEvent)
			{
				try
				{
					if (this._enclosing.ShowUpdateTableConfirmation("Injector Flow Scaling") == JOptionPane
						.OK_OPTION)
					{
						Table2D table = this._enclosing.GetInjectorFlowTable(this._enclosing.ecuEditor);
						if (table != null)
						{
							DataCell[] cells = table.GetData();
							if (cells.Length == 1)
							{
								if (this._enclosing.IsNumber(this._enclosing.flowScaling))
								{
									string value = this._enclosing.flowScaling.GetText().Trim();
									cells[0].SetRealValue(value);
									table.Colorize();
								}
								else
								{
									JOptionPane.ShowMessageDialog(this._enclosing.parent, "Invalid Injector Flow Scaling value."
										, "Error", JOptionPane.ERROR_MESSAGE);
								}
							}
						}
						else
						{
							JOptionPane.ShowMessageDialog(this._enclosing.parent, "Injector Flow Scaling table not found."
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

			private readonly InjectorControlPanel _enclosing;
		}

		private JButton BuildUpdateInjectorLatencyButton()
		{
			JButton updateButton = new JButton("Update Latency");
			updateButton.AddActionListener(new _ActionListener_426(this));
			return updateButton;
		}

		private sealed class _ActionListener_426 : ActionListener
		{
			public _ActionListener_426(InjectorControlPanel _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void ActionPerformed(ActionEvent actionEvent)
			{
				try
				{
					if (this._enclosing.ShowUpdateTableConfirmation("Injector Latency") == JOptionPane
						.OK_OPTION)
					{
						Table2D table = this._enclosing.GetInjectorLatencyTable(this._enclosing.ecuEditor
							);
						if (table != null)
						{
							DataCell[] cells = table.GetData();
							if (this._enclosing.IsNumber(this._enclosing.latencyOffset))
							{
								foreach (DataCell cell in cells)
								{
									double newLatency = cell.GetValue() + this._enclosing.ParseDouble(this._enclosing
										.latencyOffset);
									cell.SetRealValue(string.Empty + newLatency);
								}
								table.Colorize();
							}
							else
							{
								JOptionPane.ShowMessageDialog(this._enclosing.parent, "Invalid Injector Latency Offset value."
									, "Error", JOptionPane.ERROR_MESSAGE);
							}
						}
						else
						{
							JOptionPane.ShowMessageDialog(this._enclosing.parent, "Error finding Injector Latency table."
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

			private readonly InjectorControlPanel _enclosing;
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

		private int ShowUpdateTableConfirmation(string table)
		{
			return JOptionPane.ShowConfirmDialog(parent, "Update " + table + "?", "Confirm Update"
				, JOptionPane.YES_NO_OPTION, JOptionPane.WARNING_MESSAGE);
		}

		private Table2D GetInjectorFlowTable(ECUEditor ecuEditor)
		{
			return GetTable(ecuEditor, "Injector Flow Scaling");
		}

		private Table2D GetInjectorLatencyTable(ECUEditor ecuEditor)
		{
			return GetTable(ecuEditor, "Injector Latency");
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

		public void SetEcuSwitches(IList<EcuSwitch> switches)
		{
			this.switches = new AList<EcuSwitch>(switches);
		}

		public void SetExternalDatas(IList<ExternalData> externals)
		{
			this.externals = new AList<ExternalData>(externals);
		}
	}
}

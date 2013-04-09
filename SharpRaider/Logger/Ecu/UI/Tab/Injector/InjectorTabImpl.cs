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
using RomRaider.Logger.Ecu.Definition;
using RomRaider.Logger.Ecu.UI;
using RomRaider.Logger.Ecu.UI.Tab;
using RomRaider.Logger.Ecu.UI.Tab.Injector;
using Sharpen;

namespace RomRaider.Logger.Ecu.UI.Tab.Injector
{
	[System.Serializable]
	public sealed class InjectorTabImpl : JPanel, InjectorTab
	{
		private const long serialVersionUID = 5365322624406058883L;

		private readonly LoggerChartPanel chartPanel = new LoggerChartPanel("Pulse Width (ms)"
			, "Fuel per Combustion Event (cc)");

		private readonly InjectorControlPanel controlPanel;

		public InjectorTabImpl(DataRegistrationBroker broker, ECUEditor ecuEditor) : base
			(new BorderLayout(2, 2))
		{
			controlPanel = new InjectorControlPanel(this, broker, ecuEditor, chartPanel);
			JScrollPane scrollPane = new JScrollPane(controlPanel, ScrollPaneConstants.VERTICAL_SCROLLBAR_ALWAYS
				, ScrollPaneConstants.HORIZONTAL_SCROLLBAR_NEVER);
			Add(scrollPane, BorderLayout.WEST);
			Add(chartPanel, BorderLayout.CENTER);
		}

		public double GetFuelStoichAfr()
		{
			return controlPanel.GetFuelStoichAfr();
		}

		public double GetFuelDensity()
		{
			return controlPanel.GetFuelDensity();
		}

		public bool IsRecordData()
		{
			return controlPanel.IsRecordData();
		}

		public bool IsValidClOl(double value)
		{
			return controlPanel.IsValidClOl(value);
		}

		public bool IsValidAfr(double value)
		{
			return controlPanel.IsValidAfr(value);
		}

		public bool IsValidRpm(double value)
		{
			return controlPanel.IsValidRpm(value);
		}

		public bool IsValidMaf(double value)
		{
			return controlPanel.IsValidMaf(value);
		}

		public bool IsValidCoolantTemp(double value)
		{
			return controlPanel.IsValidCoolantTemp(value);
		}

		public bool IsValidIntakeAirTemp(double value)
		{
			return controlPanel.IsValidIntakeAirTemp(value);
		}

		public bool IsValidMafvChange(double value)
		{
			return controlPanel.IsValidMafvChange(value);
		}

		public bool IsValidTipInThrottle(double value)
		{
			return controlPanel.IsValidTipInThrottle(value);
		}

		public void AddData(double pulseWidth, double fuelcc)
		{
			chartPanel.AddData(pulseWidth, fuelcc);
		}

		public void SetEcuParams(IList<EcuParameter> @params)
		{
			controlPanel.SetEcuParams(@params);
		}

		public void SetEcuSwitches(IList<EcuSwitch> switches)
		{
			controlPanel.SetEcuSwitches(switches);
		}

		public void SetExternalDatas(IList<ExternalData> external)
		{
			controlPanel.SetExternalDatas(external);
		}

		public JPanel GetPanel()
		{
			return this;
		}
	}
}

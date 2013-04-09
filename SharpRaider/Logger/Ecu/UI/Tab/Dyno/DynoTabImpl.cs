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
using RomRaider.Logger.Ecu.UI.Tab.Dyno;
using Sharpen;

namespace RomRaider.Logger.Ecu.UI.Tab.Dyno
{
	[System.Serializable]
	public sealed class DynoTabImpl : JPanel, DynoTab
	{
		private const long serialVersionUID = 2787020251963102201L;

		private readonly DynoChartPanel chartPanel = new DynoChartPanel("Engine Speed (RPM)"
			, "Calculated Wheel Power", "Calculated Engine Torque");

		private readonly DynoControlPanel controlPanel;

		public DynoTabImpl(DataRegistrationBroker broker, ECUEditor ecuEditor) : base(new 
			BorderLayout(2, 2))
		{
			controlPanel = new DynoControlPanel(this, broker, ecuEditor, chartPanel);
			JScrollPane scrollPane = new JScrollPane(controlPanel, ScrollPaneConstants.VERTICAL_SCROLLBAR_ALWAYS
				, ScrollPaneConstants.HORIZONTAL_SCROLLBAR_NEVER);
			Add(scrollPane, BorderLayout.WEST);
			Add(chartPanel, BorderLayout.CENTER);
		}

		public double CalcRpm(double vs)
		{
			return controlPanel.CalcRpm(vs);
		}

		public void UpdateEnv(double iat, double pressure)
		{
			controlPanel.UpdateEnv(iat, pressure);
		}

		public bool IsValidET(long now, double vs)
		{
			return controlPanel.IsValidET(now, vs);
		}

		public bool IsRecordET()
		{
			return controlPanel.IsRecordET();
		}

		public bool IsRecordData()
		{
			return controlPanel.IsRecordData();
		}

		public bool IsManual()
		{
			return controlPanel.IsManual();
		}

		public bool GetEnv()
		{
			return controlPanel.GetEnv();
		}

		public bool IsValidData(double rpm, double ta)
		{
			return controlPanel.IsValidData(rpm, ta);
		}

		public void AddData(double rpm, double hp, double tq)
		{
			chartPanel.AddData(rpm, hp, tq);
		}

		public void AddRawData(double time, double rpm)
		{
			chartPanel.AddRawData(time, rpm);
		}

		public void AddData(double rpm, double hp)
		{
		}

		public int GetSampleCount()
		{
			return chartPanel.GetSampleCount();
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

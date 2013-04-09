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
using RomRaider.Logger.Ecu.Definition;
using RomRaider.Logger.Ecu.UI.Handler.Dash;
using RomRaider.Logger.External.Core;
using Sharpen;

namespace RomRaider.Logger.External.Innovate.Lm2.Mts.Plugin
{
	public sealed class Lm2MtsDataItem : ExternalDataItem, DataListener
	{
		private readonly string name;

		private readonly GaugeMinMax gaugeMinMax;

		private int channel;

		private double data;

		private string units;

		public Lm2MtsDataItem(string name, int channel, string units, float minValue, float
			 maxValue) : base()
		{
			this.name = name;
			this.channel = channel;
			this.units = units;
			float step = (Math.Abs(maxValue) + Math.Abs(minValue)) / 10f;
			if (step > 0.5f)
			{
				step = (float)Math.Round(step);
			}
			else
			{
				step = 0.5f;
			}
			gaugeMinMax = new GaugeMinMax(minValue, maxValue, step);
		}

		public string GetName()
		{
			return "Innovate MTS " + name + " CH" + channel;
		}

		public string GetDescription()
		{
			return "Innovate MTS " + name + " CH" + channel + " data";
		}

		public int GetChannel()
		{
			return channel;
		}

		public double GetData()
		{
			return data;
		}

		public string GetUnits()
		{
			return units;
		}

		public void SetData(double data)
		{
			this.data = data;
		}

		public EcuDataConvertor[] GetConvertors()
		{
			EcuDataConvertor[] convertors = new EcuDataConvertor[] { new ExternalDataConvertorImpl
				(this, units, "x", "0.##", gaugeMinMax) };
			return convertors;
		}
	}
}

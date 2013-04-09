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

using RomRaider.Logger.Ecu.UI.Handler.Dash;
using RomRaider.Logger.External.Core;
using RomRaider.Logger.External.Txs.Plugin;
using Sharpen;

namespace RomRaider.Logger.External.Txs.Plugin
{
	public class TxsSensorConversions
	{
		public static readonly ExternalSensorConversion TXS_RPM = new ExternalSensorConversion
			("rpm", "x", "0", new GaugeMinMax(0, 10000, 1000));

		public static readonly ExternalSensorConversion TXS_BOOST = new ExternalSensorConversion
			("psi", "x", "0.0", new GaugeMinMax(-100, 100, 5));

		public static readonly ExternalSensorConversion TXS_MAFV = new ExternalSensorConversion
			("mafv", "x", "0.00", new GaugeMinMax(0, 5, 0.05));

		public static readonly ExternalSensorConversion TXS_TPS = new ExternalSensorConversion
			("tps", "x", "0", new GaugeMinMax(0, 100, 5));

		public static readonly ExternalSensorConversion TXS_LOAD = new ExternalSensorConversion
			("load", "x", "0", new GaugeMinMax(0, 100, 10));

		public static readonly ExternalSensorConversion TXS_KNOCK = new ExternalSensorConversion
			("knock", "x", "0", new GaugeMinMax(0, 100, 1));

		public static readonly ExternalSensorConversion TXS_IGN = new ExternalSensorConversion
			("ign", "x", "0.00", new GaugeMinMax(-15, 60, 5));

		public static readonly ExternalSensorConversion TXS_IDC = new ExternalSensorConversion
			("%", "x", "0", new GaugeMinMax(0, 125, 5));

		public static readonly ExternalSensorConversion TXS_MAPVE = new ExternalSensorConversion
			("mapve", "x", "0", new GaugeMinMax(0, 200, 5));

		public static readonly ExternalSensorConversion TXS_MODFUEL = new ExternalSensorConversion
			("modfuel %", "x", "0.00", new GaugeMinMax(-50, 50, .05));
	}
}

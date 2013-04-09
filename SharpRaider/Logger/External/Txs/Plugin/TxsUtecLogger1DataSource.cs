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
using Javax.Swing;
using RomRaider.Logger.Ecu;
using RomRaider.Logger.External.Core;
using RomRaider.Logger.External.Txs.IO;
using RomRaider.Logger.External.Txs.Plugin;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.External.Txs.Plugin
{
	public class TxsUtecLogger1DataSource : ExternalDataSource
	{
		private static readonly string LOGGER = "1";

		private static readonly string DEVICE = "u";

		private static readonly string DEVICE_NAME = "TXS UTEC ";

		private const int RPM = 0;

		private const int BOOST = 1;

		private const int MAFV = 2;

		private const int TPS = 3;

		private const int LOAD = 4;

		private const int KNOCK = 5;

		private const int ECUIGN = 7;

		private const int IDC = 8;

		private const int MODIGN = 9;

		private const int MAPVE_MODFUEL = 10;

		private const int MODMAFV = 12;

		private const int AFR = 13;

		private readonly AList<TxsDataItem> dataItems = new AList<TxsDataItem>();

		private TxsRunner runner;

		private string port;

		public virtual string GetId()
		{
			return GetType().FullName;
		}

		public virtual string GetName()
		{
			return DEVICE_NAME + "Logger";
		}

		public virtual string GetVersion()
		{
			return "2012.09.20";
		}

		public virtual IList<ExternalDataItem> GetDataItems()
		{
			return Sharpen.Collections.UnmodifiableList(new AList<TxsDataItem>(dataItems));
		}

		public virtual Action GetMenuAction(EcuLogger logger)
		{
			return null;
		}

		public virtual void SetPort(string port)
		{
			this.port = port;
		}

		public virtual string GetPort()
		{
			return port;
		}

		public virtual void Connect()
		{
			runner = new TxsRunner(port, dataItems, LOGGER, DEVICE);
			ThreadUtil.RunAsDaemon(runner);
		}

		public virtual void Disconnect()
		{
			if (runner != null)
			{
				runner.Stop();
			}
		}

		public TxsUtecLogger1DataSource()
		{
			{
				dataItems.AddItem(new TxsDataItem(DEVICE_NAME + "RPM", RPM, TxsSensorConversions.
					TXS_RPM));
				dataItems.AddItem(new TxsDataItem(DEVICE_NAME + "BOOST", BOOST, TxsSensorConversions
					.TXS_BOOST));
				dataItems.AddItem(new TxsDataItem(DEVICE_NAME + "MAFV", MAFV, TxsSensorConversions
					.TXS_MAFV));
				dataItems.AddItem(new TxsDataItem(DEVICE_NAME + "TPS", TPS, TxsSensorConversions.
					TXS_TPS));
				dataItems.AddItem(new TxsDataItem(DEVICE_NAME + "LOAD", LOAD, TxsSensorConversions
					.TXS_LOAD));
				dataItems.AddItem(new TxsDataItem(DEVICE_NAME + "KNOCK", KNOCK, TxsSensorConversions
					.TXS_KNOCK));
				dataItems.AddItem(new TxsDataItem(DEVICE_NAME + "ECUIGN", ECUIGN, TxsSensorConversions
					.TXS_IGN));
				dataItems.AddItem(new TxsDataItem(DEVICE_NAME + "IDC", IDC, TxsSensorConversions.
					TXS_IDC));
				dataItems.AddItem(new TxsDataItem(DEVICE_NAME + "MODIGN", MODIGN, TxsSensorConversions
					.TXS_IGN));
				dataItems.AddItem(new TxsDataItem(DEVICE_NAME + "MAPVE/MODFUEL", MAPVE_MODFUEL, TxsSensorConversions
					.TXS_MODFUEL, TxsSensorConversions.TXS_MAPVE));
				dataItems.AddItem(new TxsDataItem(DEVICE_NAME + "MODMAFV", MODMAFV, TxsSensorConversions
					.TXS_MAFV));
				dataItems.AddItem(new TxsDataItem(DEVICE_NAME + "AFR", AFR, SensorConversionsAFR.
					AFR_147, SensorConversionsAFR.LAMBDA, SensorConversionsAFR.AFR_90, SensorConversionsAFR
					.AFR_146, SensorConversionsAFR.AFR_64, SensorConversionsAFR.AFR_155, SensorConversionsAFR
					.AFR_172, SensorConversionsAFR.AFR_34));
			}
		}
	}
}

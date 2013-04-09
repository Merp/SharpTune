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
	public class TxsTunerDataSource : ExternalDataSource
	{
		private static readonly string LOGGER = "1";

		private static readonly string DEVICE = "t";

		private static readonly string DEVICE_NAME = "TXS Tuner ";

		private const int TUNER_LITE_AFR = 0;

		private const int TUNER_PRO_RPM = 0;

		private const int TUNER_PRO_AFR = 1;

		private const int TUNER_PRO_KNOCK = 11;

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
			return "2012.09.22";
		}

		public virtual IList<ExternalDataItem> GetDataItems()
		{
			return Sharpen.Collections.UnmodifiableList(dataItems);
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

		public TxsTunerDataSource()
		{
			{
				dataItems.AddItem(new TxsDataItem(DEVICE_NAME + "Lite AFR", TUNER_LITE_AFR, SensorConversionsAFR
					.AFR_147, SensorConversionsAFR.LAMBDA, SensorConversionsAFR.AFR_90, SensorConversionsAFR
					.AFR_146, SensorConversionsAFR.AFR_64, SensorConversionsAFR.AFR_155, SensorConversionsAFR
					.AFR_172, SensorConversionsAFR.AFR_34));
				dataItems.AddItem(new TxsDataItem(DEVICE_NAME + "Pro RPM", TUNER_PRO_RPM, TxsSensorConversions
					.TXS_RPM));
				dataItems.AddItem(new TxsDataItem(DEVICE_NAME + "Pro AFR", TUNER_PRO_AFR, SensorConversionsAFR
					.LAMBDA, SensorConversionsAFR.AFR_147, SensorConversionsAFR.AFR_90, SensorConversionsAFR
					.AFR_146, SensorConversionsAFR.AFR_64, SensorConversionsAFR.AFR_155, SensorConversionsAFR
					.AFR_172, SensorConversionsAFR.AFR_34));
				dataItems.AddItem(new TxsDataItem(DEVICE_NAME + "Pro Knock", TUNER_PRO_KNOCK, TxsSensorConversions
					.TXS_KNOCK));
			}
		}
	}
}

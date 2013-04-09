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
using RomRaider.IO.Connection;
using RomRaider.IO.Serial.Connection;
using RomRaider.Logger.External.Core;
using RomRaider.Logger.External.Txs.IO;
using RomRaider.Logger.External.Txs.Plugin;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.External.Txs.IO
{
	public sealed class TxsRunner : Stoppable
	{
		private static readonly Org.Apache.Log4j.Logger LOGGER = Org.Apache.Log4j.Logger.GetLogger
			(typeof(RomRaider.Logger.External.Txs.IO.TxsRunner));

		private static readonly ConnectionProperties CONNECTION_PROPS = new TxsConnectionProperties
			();

		private static readonly string WHITESPACE_REGEX = "\\s+";

		private static readonly string SPLIT_DELIMITER = " ";

		private static readonly byte[] EXIT = new byte[] { 24 };

		private readonly AList<TxsDataItem> dataItems;

		private readonly SerialConnection connection;

		private bool stop;

		private string txsLogger;

		private string txsDevice;

		public TxsRunner(string port, AList<TxsDataItem> dataItems, string logger, string
			 device)
		{
			this.connection = new SerialConnectionImpl(port, CONNECTION_PROPS);
			this.dataItems = dataItems;
			this.txsLogger = logger;
			this.txsDevice = device;
		}

		public void Run()
		{
			try
			{
				//Convert string into bytes[]
				byte[] device = Sharpen.Runtime.GetBytesForString(txsDevice);
				byte[] logger = Sharpen.Runtime.GetBytesForString(this.txsLogger);
				//Exit to main screen 
				connection.Write(EXIT);
				//wait for exit to complete.
				Sharpen.Thread.Sleep(250L);
				string response = connection.ReadLine();
				//Send command to switch device: utec / tuner.
				connection.Write(device);
				//Read and Trace response switching device.
				response = connection.ReadLine();
				LOGGER.Trace("TXS Runner Response: " + response);
				//Start device logger
				connection.Write(logger);
				while (!stop)
				{
					//Get Response from TXS Device
					response = connection.ReadLine();
					connection.Write(logger);
					//Continue if no data was received.
					if (ParamChecker.IsNullOrEmpty(response))
					{
						continue;
					}
					//Trace response
					LOGGER.Trace("TXS Runner Response: " + response);
					//Split Values for parsing
					string[] values = SplitUtecString(response);
					//Set Data Item Values
					SetDataItemValues(values);
				}
				connection.Close();
			}
			catch (Exception t)
			{
				LOGGER.Error("Error occurred", t);
			}
			finally
			{
				connection.Close();
			}
		}

		public void Stop()
		{
			stop = true;
		}

		private string[] SplitUtecString(string value)
		{
			try
			{
				value = value.Trim();
				value = value.ReplaceAll(WHITESPACE_REGEX, SPLIT_DELIMITER);
				string[] utecArray = value.Split(SPLIT_DELIMITER);
				return utecArray;
			}
			catch (Exception)
			{
				return new string[] {  };
			}
		}

		private void SetDataItemValues(string[] values)
		{
			for (int i = 0; i < values.Length; i++)
			{
				//GetDataItem via Index Hash defined in DataSoruce
				TxsDataItem dataItem = dataItems[i];
				if (dataItem != null)
				{
					//Set value to dataItem
					dataItem.SetData(ParseDouble(values[dataItem.GetItemIndex()]));
				}
			}
		}

		private double ParseDouble(string value)
		{
			try
			{
				//try to parse value.
				return double.ParseDouble(value);
			}
			catch (Exception)
			{
				//return 0 if value could not be parsed.
				return 0.0;
			}
		}
	}
}

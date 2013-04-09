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
using RomRaider.Logger.External.Core;
using RomRaider.Logger.External.Innovate.Generic.Mts.IO;
using RomRaider.Logger.External.Innovate.Lm2.Mts.Plugin;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.External.Innovate.Generic.Mts.IO
{
	public sealed class MTSRunner : MTSEvents, Stoppable
	{
		private static readonly Org.Apache.Log4j.Logger LOGGER = Org.Apache.Log4j.Logger.GetLogger
			(typeof(RomRaider.Logger.External.Innovate.Generic.Mts.IO.MTSRunner));

		private readonly IDictionary<int, Lm2MtsDataItem> dataItems;

		private readonly MTS mts;

		private bool running;

		private bool stop;

		/// <summary>
		/// MTSRunner contains the work-horse methods to process the MTS stream
		/// data and update the appropriate sensor result.
		/// </summary>
		/// <remarks>
		/// MTSRunner contains the work-horse methods to process the MTS stream
		/// data and update the appropriate sensor result.  Once started this class
		/// reads data at a set interval from the MTS stream.
		/// Events processing appears to be problematic via COM4J and therefore not used.
		/// </remarks>
		public MTSRunner(int mtsPort, IDictionary<int, Lm2MtsDataItem> dataItems)
		{
			// import com4j.EventCookie;
			//    private EventCookie connectionEventCookie;
			MTSConnector connection = new MTSConnector(mtsPort);
			this.mts = connection.GetMts();
			this.dataItems = dataItems;
		}

		public void Run()
		{
			running = true;
			try
			{
				DoRun();
			}
			catch (Exception t)
			{
				LOGGER.Error("Innovate MTS error occurred", t);
			}
			finally
			{
				running = false;
			}
		}

		public void Stop()
		{
			stop = true;
			// wait for it to stop running so mts can disconnect/dispose... timeout after 5secs
			long timeout = Runtime.CurrentTimeMillis() + 5000L;
			while (running && Runtime.CurrentTimeMillis() < timeout)
			{
				ThreadUtil.Sleep(100L);
			}
		}

		private void DoRun()
		{
			try
			{
				// attempt to connect to the specified device
				//            connectionEventCookie = mts.advise(MTSEvents.class, this);
				mts.Connect();
				try
				{
					if (mts.InputCount() > 0)
					{
						while (!stop)
						{
							ReadData();
							ThreadUtil.Sleep(12L);
						}
					}
					else
					{
						LOGGER.Error("Innovate MTS: Error while reading data - no input channels found to log from!"
							);
					}
				}
				finally
				{
					mts.Disconnect();
				}
			}
			finally
			{
				//connectionEventCookie.close();
				mts.Dispose();
			}
		}

		public void ConnectionEvent(int result)
		{
			if (result == 0)
			{
				mts.StartData();
			}
			else
			{
				if (result == -1)
				{
					throw new InvalidOperationException("No Innovate MTS Data detected");
				}
				else
				{
					throw new InvalidOperationException("Innovate MTS Connect Error: " + result);
				}
			}
		}

		public void ConnectionError()
		{
			mts.Disconnect();
			throw new InvalidOperationException("Innovate MTS Connection Timeout");
		}

		public void NewData()
		{
			LOGGER.Debug("Innovate MTS newData");
		}

		//readData();
		public void ReadData()
		{
			for (int i = 0; i < mts.InputCount(); i++)
			{
				float data = 0f;
				// select the input
				mts.CurrentInput(i);
				int type = mts.InputType();
				int function = mts.InputFunction();
				int sample = mts.InputSample();
				LOGGER.Trace("Innovate MTS input = " + i + ", type = " + type + ", function = " +
					 function + ", sample = " + sample);
				// 5V channel
				// Determine the range between min and max,
				// calculate what percentage of that our sample represents,
				// shift back to match our offset from 0.0 for min
				if (type == MTSSensor.MTSSensorInputType.MTS_TYPE_VDC.inputType)
				{
					if (function == MTSSensor.MTSSensorInputFunction.MTS_FUNC_NOTLAMBDA.function)
					{
						float min = mts.InputMinValue();
						float max = mts.InputMaxValue();
						data = ((max - min) * ((float)sample / 1024f)) + min;
					}
					else
					{
						// this will report other functions, such as ERROR states
						// as a negative constant value
						data = (float)function * -1f;
					}
				}
				// AFR
				// Take each sample step as .001 Lambda,
				// add 0.5 (so our range is 0.5 to 1.523 for our 1024 steps),
				// then multiply by the AFR multiplier
				if (type == MTSSensor.MTSSensorInputType.MTS_TYPE_AFR.inputType)
				{
					if (function == MTSSensor.MTSSensorInputFunction.MTS_FUNC_LAMBDA.function)
					{
						float multiplier = mts.InputAFRMultiplier();
						data = ((float)sample / 1000f + 0.5f) * multiplier;
					}
					else
					{
						if (function == MTSSensor.MTSSensorInputFunction.MTS_FUNC_O2.function)
						{
							data = ((float)sample / 10f);
						}
						else
						{
							// this will report other functions, such as ERROR states
							// as a negative constant value
							data = (float)function * -1f;
						}
					}
				}
				// LAMBDA
				// Identical to AFR, except we do not multiply for AFR.
				if (type == MTSSensor.MTSSensorInputType.MTS_TYPE_LAMBDA.inputType)
				{
					if (function == MTSSensor.MTSSensorInputFunction.MTS_FUNC_LAMBDA.function)
					{
						data = (float)sample / 1000f + 0.5f;
					}
					else
					{
						// this will report other functions, such as ERROR states
						// as a negative constant value
						data = (float)function * -1f;
					}
				}
				// set data for this sensor based on inputNumber
				Lm2MtsDataItem dataItem = dataItems.Get(i);
				if (dataItem != null)
				{
					dataItem.SetData(data);
				}
			}
		}
	}
}

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
using Javax.Swing;
using RomRaider.Logger.Ecu.Comms.Query;
using RomRaider.Logger.Ecu.Definition;
using RomRaider.Logger.Ecu.UI.Handler;
using RomRaider.Logger.Ecu.UI.Handler.Maf;
using RomRaider.Logger.Ecu.UI.Tab.Maf;
using Sharpen;

namespace RomRaider.Logger.Ecu.UI.Handler.Maf
{
	public sealed class MafUpdateHandler : DataUpdateHandler
	{
		private static readonly Org.Apache.Log4j.Logger LOGGER = Org.Apache.Log4j.Logger.GetLogger
			(typeof(MafUpdateHandler));

		private static readonly string MAFV = "P18";

		private static readonly string AF_LEARNING_1 = "P4";

		private static readonly string AF_CORRECTION_1 = "P3";

		private MafTab mafTab;

		private double lastMafv;

		private long lastUpdate;

		public void RegisterData(LoggerData loggerData)
		{
			lock (this)
			{
			}
		}

		public void HandleDataUpdate(Response response)
		{
			lock (this)
			{
				if (mafTab.IsRecordData() && ContainsData(response, MAFV, AF_LEARNING_1, AF_CORRECTION_1
					))
				{
					bool valid = true;
					// cl/ol check
					if ((ContainsData(response, "E3") || ContainsData(response, "E33")))
					{
						double clOl = -1;
						if (ContainsData(response, "E3"))
						{
							clOl = (int)FindValue(response, "E3");
							LOGGER.Trace("MAF:[CL/OL:E3]:  " + clOl);
						}
						if (ContainsData(response, "E33"))
						{
							clOl = (int)FindValue(response, "E33");
							LOGGER.Trace("MAF:[CL/OL:E33]: " + clOl);
						}
						valid = mafTab.IsValidClOl(clOl);
						LOGGER.Trace("MAF:[CL/OL]:     " + valid);
					}
					// afr check
					if (valid && ContainsData(response, "P58"))
					{
						double afr = FindValue(response, "P58");
						LOGGER.Trace("MAF:[AFR:P58]: " + afr);
						valid = mafTab.IsValidAfr(afr);
						LOGGER.Trace("MAF:[AFR]:     " + valid);
					}
					// rpm check
					if (valid && ContainsData(response, "P8"))
					{
						double rpm = FindValue(response, "P8");
						LOGGER.Trace("MAF:[RPM:P8]: " + rpm);
						valid = mafTab.IsValidRpm(rpm);
						LOGGER.Trace("MAF:[RPM]:    " + valid);
					}
					// maf check
					if (valid && ContainsData(response, "P12"))
					{
						double maf = FindValue(response, "P12");
						LOGGER.Trace("MAF:[MAF:P12]: " + maf);
						valid = mafTab.IsValidMaf(maf);
						LOGGER.Trace("MAF:[MAF]:     " + valid);
					}
					// intake air temp check
					if (valid && ContainsData(response, "P11"))
					{
						double temp = FindValue(response, "P11");
						LOGGER.Trace("MAF:[IAT:P11]: " + temp);
						valid = mafTab.IsValidIntakeAirTemp(temp);
						LOGGER.Trace("MAF:[IAT]:     " + valid);
					}
					// coolant temp check
					if (valid && ContainsData(response, "P2"))
					{
						double temp = FindValue(response, "P2");
						LOGGER.Trace("MAF:[CT:P2]: " + temp);
						valid = mafTab.IsValidCoolantTemp(temp);
						LOGGER.Trace("MAF:[CT]:    " + valid);
					}
					// dMAFv/dt check
					if (valid && ContainsData(response, "P18"))
					{
						double mafv = FindValue(response, "P18");
						long now = Runtime.CurrentTimeMillis();
						double mafvChange = Math.Abs((mafv - lastMafv) / (now - lastUpdate) * 1000);
						LOGGER.Trace("MAF:[dMAFv/dt]: " + mafvChange);
						valid = mafTab.IsValidMafvChange(mafvChange);
						LOGGER.Trace("MAF:[dMAFv/dt]: " + valid);
						lastMafv = mafv;
						lastUpdate = now;
					}
					// tip-in throttle check
					if (valid && (ContainsData(response, "E23") || ContainsData(response, "E54")))
					{
						double tipIn = -1;
						if (ContainsData(response, "E23"))
						{
							tipIn = FindValue(response, "E23");
							LOGGER.Trace("MAF:[TIP:E23]: " + tipIn);
						}
						if (ContainsData(response, "E54"))
						{
							tipIn = FindValue(response, "E54");
							LOGGER.Trace("MAF:[TIP:E54]: " + tipIn);
						}
						valid = mafTab.IsValidTipInThrottle(tipIn);
						LOGGER.Trace("MAF:[TIP]:     " + valid);
					}
					if (valid)
					{
						double mafv = FindValue(response, MAFV);
						double learning = FindValue(response, AF_LEARNING_1);
						double correction = FindValue(response, AF_CORRECTION_1);
						LOGGER.Trace("MAF Data: " + mafv + "v, " + correction + "%");
						SwingUtilities.InvokeLater(new _Runnable_136(this, mafv, learning, correction));
					}
				}
			}
		}

		private sealed class _Runnable_136 : Runnable
		{
			public _Runnable_136(MafUpdateHandler _enclosing, double mafv, double learning, double
				 correction)
			{
				this._enclosing = _enclosing;
				this.mafv = mafv;
				this.learning = learning;
				this.correction = correction;
			}

			public void Run()
			{
				this._enclosing.mafTab.AddData(mafv, learning + correction);
			}

			private readonly MafUpdateHandler _enclosing;

			private readonly double mafv;

			private readonly double learning;

			private readonly double correction;
		}

		private bool ContainsData(Response response, params string[] ids)
		{
			ICollection<LoggerData> datas = response.GetData();
			foreach (string id in ids)
			{
				bool found = false;
				foreach (LoggerData data in datas)
				{
					if (data.GetId().Equals(id))
					{
						found = true;
						break;
					}
				}
				if (!found)
				{
					return false;
				}
			}
			return true;
		}

		private double FindValue(Response response, string id)
		{
			foreach (LoggerData loggerData in response.GetData())
			{
				if (id.Equals(loggerData.GetId()))
				{
					return response.GetDataValue(loggerData);
				}
			}
			throw new InvalidOperationException("Expected data item " + id + " not in response."
				);
		}

		public void DeregisterData(LoggerData loggerData)
		{
			lock (this)
			{
			}
		}

		public void CleanUp()
		{
			lock (this)
			{
			}
		}

		public void Reset()
		{
			lock (this)
			{
			}
		}

		public void SetMafTab(MafTab mafTab)
		{
			this.mafTab = mafTab;
		}
	}
}

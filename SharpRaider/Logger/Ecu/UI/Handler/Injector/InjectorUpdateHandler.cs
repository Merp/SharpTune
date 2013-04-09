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
using RomRaider.Logger.Ecu.UI.Handler.Injector;
using RomRaider.Logger.Ecu.UI.Tab.Injector;
using Sharpen;

namespace RomRaider.Logger.Ecu.UI.Handler.Injector
{
	public sealed class InjectorUpdateHandler : DataUpdateHandler
	{
		private static readonly Org.Apache.Log4j.Logger LOGGER = Org.Apache.Log4j.Logger.
			GetLogger(typeof(InjectorUpdateHandler));

		private static readonly string PULSE_WIDTH_16 = "E28";

		private static readonly string PULSE_WIDTH_32 = "E60";

		private static readonly string ENGINE_LOAD_16 = "E2";

		private static readonly string ENGINE_LOAD_32 = "E32";

		private InjectorTab injectorTab;

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
				if (injectorTab.IsRecordData() && (ContainsData(response, PULSE_WIDTH_16, ENGINE_LOAD_16
					) || ContainsData(response, PULSE_WIDTH_32, ENGINE_LOAD_32)))
				{
					bool valid = true;
					// cl/ol check
					if ((ContainsData(response, "E3") || ContainsData(response, "E33")))
					{
						double clOl = -1;
						if (ContainsData(response, "E3"))
						{
							clOl = (int)FindValue(response, "E3");
							LOGGER.Trace("INJ:[CL/OL:E3]:  " + clOl);
						}
						if (ContainsData(response, "E33"))
						{
							clOl = (int)FindValue(response, "E33");
							LOGGER.Trace("INJ:[CL/OL:E33]: " + clOl);
						}
						valid = injectorTab.IsValidClOl(clOl);
						LOGGER.Trace("INJ:[CL/OL]:     " + valid);
					}
					// afr check
					if (valid && ContainsData(response, "P58"))
					{
						double afr = FindValue(response, "P58");
						LOGGER.Trace("INJ:[AFR:P58]: " + afr);
						valid = injectorTab.IsValidAfr(afr);
						LOGGER.Trace("INJ:[AFR]:     " + valid);
					}
					// rpm check
					if (valid && ContainsData(response, "P8"))
					{
						double rpm = FindValue(response, "P8");
						LOGGER.Trace("INJ:[RPM:P8]: " + rpm);
						valid = injectorTab.IsValidRpm(rpm);
						LOGGER.Trace("INJ:[RPM]:    " + valid);
					}
					// maf check
					if (valid && ContainsData(response, "P12"))
					{
						double maf = FindValue(response, "P12");
						LOGGER.Trace("INJ:[MAF:P12]: " + maf);
						valid = injectorTab.IsValidMaf(maf);
						LOGGER.Trace("INJ:[MAF]:     " + valid);
					}
					// intake air temp check
					if (valid && ContainsData(response, "P11"))
					{
						double temp = FindValue(response, "P11");
						LOGGER.Trace("INJ:[IAT:P11]: " + temp);
						valid = injectorTab.IsValidIntakeAirTemp(temp);
						LOGGER.Trace("INJ:[IAT]:     " + valid);
					}
					// coolant temp check
					if (valid && ContainsData(response, "P2"))
					{
						double temp = FindValue(response, "P2");
						LOGGER.Trace("INJ:[CT:P2]: " + temp);
						valid = injectorTab.IsValidCoolantTemp(temp);
						LOGGER.Trace("INJ:[CT]:    " + valid);
					}
					// dMAFv/dt check
					if (valid && ContainsData(response, "P18"))
					{
						double mafv = FindValue(response, "P18");
						long now = Runtime.CurrentTimeMillis();
						double mafvChange = Math.Abs((mafv - lastMafv) / (now - lastUpdate) * 1000);
						LOGGER.Trace("INJ:[dMAFv/dt]: " + mafvChange);
						valid = injectorTab.IsValidMafvChange(mafvChange);
						LOGGER.Trace("INJ:[dMAFv/dt]: " + valid);
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
							LOGGER.Trace("INJ:[TIP:E23]: " + tipIn);
						}
						if (ContainsData(response, "E54"))
						{
							tipIn = FindValue(response, "E54");
							LOGGER.Trace("INJ:[TIP:E54]: " + tipIn);
						}
						valid = injectorTab.IsValidTipInThrottle(tipIn);
						LOGGER.Trace("INJ:[TIP]:     " + valid);
					}
					if (valid)
					{
						double pulseWidth = ContainsData(response, PULSE_WIDTH_16) ? FindValue(response, 
							PULSE_WIDTH_16) : FindValue(response, PULSE_WIDTH_32);
						double load = ContainsData(response, ENGINE_LOAD_16) ? FindValue(response, ENGINE_LOAD_16
							) : FindValue(response, ENGINE_LOAD_32);
						double stoichAfr = injectorTab.GetFuelStoichAfr();
						double density = injectorTab.GetFuelDensity();
						double fuelcc = load / 2 / stoichAfr * 1000 / density;
						LOGGER.Trace("Injector Data: " + pulseWidth + "ms, " + fuelcc + "cc");
						SwingUtilities.InvokeLater(new _Runnable_140(this, pulseWidth, fuelcc));
					}
				}
			}
		}

		private sealed class _Runnable_140 : Runnable
		{
			public _Runnable_140(InjectorUpdateHandler _enclosing, double pulseWidth, double 
				fuelcc)
			{
				this._enclosing = _enclosing;
				this.pulseWidth = pulseWidth;
				this.fuelcc = fuelcc;
			}

			public void Run()
			{
				this._enclosing.injectorTab.AddData(pulseWidth, fuelcc);
			}

			private readonly InjectorUpdateHandler _enclosing;

			private readonly double pulseWidth;

			private readonly double fuelcc;
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

		public void SetInjectorTab(InjectorTab injectorTab)
		{
			this.injectorTab = injectorTab;
		}
	}
}

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
using Java.Awt;
using Javax.Swing;
using RomRaider.Logger.Ecu.Comms.Query;
using RomRaider.Logger.Ecu.Definition;
using RomRaider.Logger.Ecu.UI.Handler;
using RomRaider.Logger.Ecu.UI.Handler.Dash;
using RomRaider.Util;
using Sharpen;
using Sharpen.Reflect;

namespace RomRaider.Logger.Ecu.UI.Handler.Dash
{
	public sealed class DashboardUpdateHandler : DataUpdateHandler, ConvertorUpdateListener
	{
		private static readonly Type[] STYLES = new Type[] { typeof(PlainGaugeStyle), typeof(
			SmallGaugeStyle), typeof(NoFrillsGaugeStyle), typeof(DialGaugeStyle), typeof(SmallDialGaugeStyle
			) };

		private readonly IDictionary<LoggerData, Gauge> gauges = Sharpen.Collections.SynchronizedMap
			(new Dictionary<LoggerData, Gauge>());

		private readonly JPanel dashboardPanel;

		private int styleIndex;

		public DashboardUpdateHandler(JPanel dashboardPanel)
		{
			this.dashboardPanel = dashboardPanel;
		}

		public void RegisterData(LoggerData loggerData)
		{
			lock (this)
			{
				GaugeStyle style = GetGaugeStyle(STYLES[styleIndex], loggerData);
				Gauge gauge = new Gauge(style);
				gauges.Put(loggerData, gauge);
				dashboardPanel.Add(gauge);
				RepaintDashboardPanel();
			}
		}

		public void HandleDataUpdate(Response response)
		{
			lock (this)
			{
				foreach (LoggerData loggerData in response.GetData())
				{
					Gauge gauge = gauges.Get(loggerData);
					if (gauge != null)
					{
						double value = response.GetDataValue(loggerData);
						gauge.UpdateValue(value);
					}
				}
			}
		}

		public void DeregisterData(LoggerData loggerData)
		{
			lock (this)
			{
				dashboardPanel.Remove(gauges.Get(loggerData));
				Sharpen.Collections.Remove(gauges, loggerData);
				RepaintDashboardPanel();
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
				foreach (Gauge gauge in gauges.Values)
				{
					gauge.ResetValue();
				}
			}
		}

		public void NotifyConvertorUpdate(LoggerData updatedLoggerData)
		{
			lock (this)
			{
				Gauge gauge = gauges.Get(updatedLoggerData);
				if (gauge != null)
				{
					gauge.ResetValue();
					gauge.RefreshTitle();
				}
			}
		}

		public void ToggleGaugeStyle()
		{
			lock (this)
			{
				Type styleClass = GetNextStyleClass();
				foreach (KeyValuePair<LoggerData, Gauge> entry in gauges.EntrySet())
				{
					GaugeStyle style = GetGaugeStyle(styleClass, entry.Key);
					entry.Value.SetGaugeStyle(style);
				}
				RepaintDashboardPanel();
			}
		}

		private Type GetNextStyleClass()
		{
			styleIndex = styleIndex == STYLES.Length - 1 ? 0 : styleIndex + 1;
			return STYLES[styleIndex];
		}

		private GaugeStyle GetGaugeStyle<_T0>(Type<_T0> styleClass, LoggerData loggerData
			) where _T0:GaugeStyle
		{
			try
			{
				Constructor<GaugeStyle> constructor = styleClass.GetDeclaredConstructor(typeof(LoggerData
					));
				return constructor.NewInstance(loggerData);
			}
			catch (Exception e)
			{
				throw new InvalidOperationException(e);
			}
		}

		private void RepaintDashboardPanel()
		{
			ThreadUtil.Run(new _Runnable_110(this));
		}

		private sealed class _Runnable_110 : Runnable
		{
			public _Runnable_110(DashboardUpdateHandler _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				SwingUtilities.InvokeLater(new _Runnable_112(this));
			}

			private sealed class _Runnable_112 : Runnable
			{
				public _Runnable_112(_Runnable_110 _enclosing)
				{
					this._enclosing = _enclosing;
				}

				public void Run()
				{
					Container parent = this._enclosing._enclosing.dashboardPanel.GetParent();
					if (parent != null)
					{
						parent.Validate();
					}
					else
					{
						this._enclosing._enclosing.dashboardPanel.Validate();
					}
					this._enclosing._enclosing.dashboardPanel.Repaint();
				}

				private readonly _Runnable_110 _enclosing;
			}

			private readonly DashboardUpdateHandler _enclosing;
		}
	}
}

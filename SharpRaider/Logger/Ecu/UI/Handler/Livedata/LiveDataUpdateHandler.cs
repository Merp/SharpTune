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

using Javax.Swing;
using RomRaider.Logger.Ecu.Comms.Query;
using RomRaider.Logger.Ecu.Definition;
using RomRaider.Logger.Ecu.UI.Handler;
using RomRaider.Logger.Ecu.UI.Handler.Livedata;
using Sharpen;

namespace RomRaider.Logger.Ecu.UI.Handler.Livedata
{
	public sealed class LiveDataUpdateHandler : DataUpdateHandler, ConvertorUpdateListener
	{
		private readonly LiveDataTableModel dataTableModel;

		public LiveDataUpdateHandler(LiveDataTableModel dataTableModel)
		{
			this.dataTableModel = dataTableModel;
		}

		public void RegisterData(LoggerData loggerData)
		{
			lock (this)
			{
				// add to datatable
				dataTableModel.AddParam(loggerData);
			}
		}

		public void HandleDataUpdate(Response response)
		{
			lock (this)
			{
				foreach (LoggerData loggerData in response.GetData())
				{
					SwingUtilities.InvokeLater(new _Runnable_42(this, loggerData, response));
				}
			}
		}

		private sealed class _Runnable_42 : Runnable
		{
			public _Runnable_42(LiveDataUpdateHandler _enclosing, LoggerData loggerData, Response
				 response)
			{
				this._enclosing = _enclosing;
				this.loggerData = loggerData;
				this.response = response;
			}

			public void Run()
			{
				this._enclosing.dataTableModel.UpdateParam(loggerData, response.GetDataValue(loggerData
					));
			}

			private readonly LiveDataUpdateHandler _enclosing;

			private readonly LoggerData loggerData;

			private readonly Response response;
		}

		public void DeregisterData(LoggerData loggerData)
		{
			lock (this)
			{
				// remove from datatable
				dataTableModel.RemoveParam(loggerData);
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
				dataTableModel.Reset();
			}
		}

		public void NotifyConvertorUpdate(LoggerData updatedLoggerData)
		{
			lock (this)
			{
				dataTableModel.ResetRow(updatedLoggerData);
			}
		}
	}
}

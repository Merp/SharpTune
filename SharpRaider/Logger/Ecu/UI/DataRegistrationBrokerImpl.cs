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
using RomRaider.Logger.Ecu.Comms.Controller;
using RomRaider.Logger.Ecu.Definition;
using RomRaider.Logger.Ecu.UI;
using RomRaider.Logger.Ecu.UI.Handler;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.Ecu.UI
{
	public sealed class DataRegistrationBrokerImpl : DataRegistrationBroker
	{
		private readonly IList<LoggerData> registeredLoggerData = Sharpen.Collections.SynchronizedList
			(new AList<LoggerData>());

		private readonly LoggerController controller;

		private readonly DataUpdateHandlerManager handlerManager;

		private readonly string id;

		public DataRegistrationBrokerImpl(LoggerController controller, DataUpdateHandlerManager
			 handlerManager)
		{
			ParamChecker.CheckNotNull(controller, handlerManager);
			this.controller = controller;
			this.handlerManager = handlerManager;
			id = Runtime.CurrentTimeMillis() + "_" + GetHashCode();
		}

		public void RegisterLoggerDataForLogging(LoggerData loggerData)
		{
			lock (this)
			{
				if (!registeredLoggerData.Contains(loggerData))
				{
					// register param with handlers
					handlerManager.RegisterData(loggerData);
					// add logger
					controller.AddLogger(id, loggerData);
					// add to registered parameters list
					registeredLoggerData.AddItem(loggerData);
				}
			}
		}

		public void DeregisterLoggerDataFromLogging(LoggerData loggerData)
		{
			lock (this)
			{
				if (registeredLoggerData.Contains(loggerData))
				{
					// deregister from dependant objects
					DeregisterLoggerDataFromDependants(loggerData);
					// remove from registered list
					registeredLoggerData.Remove(loggerData);
				}
			}
		}

		public void Clear()
		{
			lock (this)
			{
				foreach (LoggerData loggerData in registeredLoggerData)
				{
					DeregisterLoggerDataFromDependants(loggerData);
				}
				registeredLoggerData.Clear();
			}
		}

		public void Connecting()
		{
			lock (this)
			{
			}
		}

		public void ReadingData()
		{
			lock (this)
			{
			}
		}

		public void LoggingData()
		{
			lock (this)
			{
			}
		}

		public void Stopped()
		{
			lock (this)
			{
			}
		}

		private void DeregisterLoggerDataFromDependants(LoggerData loggerData)
		{
			// remove logger
			controller.RemoveLogger(id, loggerData);
			// deregister param from handlers
			handlerManager.DeregisterData(loggerData);
		}
	}
}

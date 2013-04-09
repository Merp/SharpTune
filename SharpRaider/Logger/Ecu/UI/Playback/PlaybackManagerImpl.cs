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
using RomRaider.Logger.Ecu.Comms.Query;
using RomRaider.Logger.Ecu.Definition;
using RomRaider.Logger.Ecu.UI.Handler;
using RomRaider.Logger.Ecu.UI.Playback;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.Ecu.UI.Playback
{
	public sealed class PlaybackManagerImpl : PlaybackManager
	{
		private readonly IList<LoggerData> loggerDatas;

		private readonly DataUpdateHandler[] dataUpdateHandlers;

		public PlaybackManagerImpl(IList<LoggerData> loggerDatas, params DataUpdateHandler
			[] dataUpdateHandlers)
		{
			//TODO: Finish me.
			this.loggerDatas = loggerDatas;
			this.dataUpdateHandlers = dataUpdateHandlers;
		}

		public void Load(FilePath file)
		{
			// TODO: Finish me!
			foreach (DataUpdateHandler handler in dataUpdateHandlers)
			{
				handler.RegisterData(loggerDatas[10]);
				handler.RegisterData(loggerDatas[20]);
				handler.RegisterData(loggerDatas[30]);
			}
		}

		public void Play()
		{
			double d = 0.0;
			while (true)
			{
				foreach (DataUpdateHandler handler in dataUpdateHandlers)
				{
					Response response = new ResponseImpl();
					response.SetDataValue(loggerDatas[10], d);
					response.SetDataValue(loggerDatas[20], d);
					response.SetDataValue(loggerDatas[30], d);
					handler.HandleDataUpdate(response);
					d += 100.0;
				}
				ThreadUtil.Sleep(100L);
			}
		}

		public void Play(int speed)
		{
			throw new NotSupportedException();
		}

		public void Step(int increment)
		{
			throw new NotSupportedException();
		}

		public void Pause()
		{
			throw new NotSupportedException();
		}

		public void Stop()
		{
			throw new NotSupportedException();
		}

		public void Reset()
		{
			throw new NotSupportedException();
		}
	}
}

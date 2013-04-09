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

using System.Reflection;
using Org.Apache.Log4j;
using RomRaider.Util;
using RomRaider.Util.Proxy;
using Sharpen;

namespace RomRaider.Util.Proxy
{
	public sealed class TimerWrapper : Wrapper
	{
		private static readonly Logger LOGGER = Logger.GetLogger(typeof(RomRaider.Util.Proxy.TimerWrapper
			));

		private readonly object delegate_;

		public TimerWrapper(object delegate_)
		{
			ParamChecker.CheckNotNull(delegate_);
			this.delegate_ = delegate_;
		}

		/// <exception cref="System.Exception"></exception>
		public object Invoke(object proxy, MethodInfo method, object[] args)
		{
			long start = Runtime.CurrentTimeMillis();
			try
			{
				return method.Invoke(delegate_, args);
			}
			finally
			{
				long time = Runtime.CurrentTimeMillis() - start;
				Log(method, time);
			}
		}

		private void Log(MethodInfo method, long time)
		{
			string c = delegate_.GetType().Name;
			string m = method.Name;
			LOGGER.Error("[TIMER] - " + c + "." + m + ": " + time + "ms");
		}
	}
}

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
using RomRaider.Logger.External.Core;
using RomRaider.Logger.External.Fourteenpoint7.IO;
using Sharpen;

namespace RomRaider.Logger.External.Fourteenpoint7.IO
{
	public sealed class NawRunner : Stoppable
	{
		private static readonly Org.Apache.Log4j.Logger LOGGER = Org.Apache.Log4j.Logger.GetLogger
			(typeof(RomRaider.Logger.External.Fourteenpoint7.IO.NawRunner));

		private static readonly byte[] NAW_PROMPT = new byte[] { unchecked((int)(0x07)) };

		private readonly NawConnection connection;

		private readonly RawDataListener listener;

		private bool stop;

		public NawRunner(string port, RawDataListener listener)
		{
			connection = new NawConnectionImpl(port);
			this.listener = listener;
		}

		public void Run()
		{
			try
			{
				while (!stop)
				{
					connection.Write(NAW_PROMPT);
					byte[] buffer = connection.ReadBytes();
					listener.SetBytes(buffer);
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
	}
}

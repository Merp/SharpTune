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
using Javax.Swing;
using RomRaider;
using RomRaider.Logger.Ecu;
using RomRaider.Swing;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.Ecu
{
	public sealed class EcuLoggerExec
	{
		public EcuLoggerExec()
		{
			throw new NotSupportedException();
		}

		public static void Main(params string[] args)
		{
			// init debug loging
			LogManager.InitDebugLogging();
			// check for dodgy threading - dev only
			//        RepaintManager.setCurrentManager(new ThreadCheckingRepaintManager(true));
			// set look and feel
			LookAndFeelManager.InitLookAndFeel();
			// load settings
			//SettingsManager manager = new SettingsManagerImpl();
			//Settings settings = manager.load();
			Settings settings = ECUExec.settings;
			// start logger
			EcuLogger.StartLogger(WindowConstants.EXIT_ON_CLOSE, settings, args);
		}
	}
}

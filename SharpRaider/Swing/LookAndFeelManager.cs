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
using Org.Apache.Log4j;
using RomRaider;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Swing
{
	public sealed class LookAndFeelManager
	{
		private static readonly Logger LOGGER = Logger.GetLogger(typeof(RomRaider.Swing.LookAndFeelManager
			));

		public LookAndFeelManager()
		{
			throw new NotSupportedException();
		}

		public static void InitLookAndFeel()
		{
			try
			{
				if (Platform.IsPlatform(Platform.MAC_OS_X))
				{
					Runtime.SetProperty("apple.awt.rendering", "true");
					Runtime.SetProperty("apple.awt.brushMetalLook", "true");
					Runtime.SetProperty("apple.laf.useScreenMenuBar", "true");
					Runtime.SetProperty("apple.awt.window.position.forceSafeCreation", "true");
					Runtime.SetProperty("com.apple.mrj.application.apple.menu.about.name", Version.PRODUCT_NAME
						);
				}
				UIManager.SetLookAndFeel(GetLookAndFeel());
				// make sure we have nice window decorations.
				JFrame.SetDefaultLookAndFeelDecorated(true);
				JDialog.SetDefaultLookAndFeelDecorated(true);
			}
			catch (Exception ex)
			{
				LOGGER.Error("Error loading system look and feel.", ex);
			}
		}

		private static string GetLookAndFeel()
		{
			//        if (true) return "com.sun.java.swing.plaf.nimbus.NimbusLookAndFeel";
			// Linux has issues with the gtk look and feel themes.
			if (Platform.IsPlatform(Platform.LINUX))
			{
				return UIManager.GetCrossPlatformLookAndFeelClassName();
			}
			return UIManager.GetSystemLookAndFeelClassName();
		}
	}
}

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
using Java.Awt.Event;
using Javax.Swing;
using RomRaider;
using RomRaider.Logger.Ecu;
using RomRaider.Logger.Ecu.UI.Swing.Menubar.Action;
using RomRaider.Logger.External.Core;
using RomRaider.Swing.Menubar;
using RomRaider.Swing.Menubar.Action;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.Ecu.UI.Swing.Menubar
{
	[System.Serializable]
	public class EcuLoggerMenuBar : JMenuBar
	{
		private const long serialVersionUID = 7081586516953740186L;

		public EcuLoggerMenuBar(EcuLogger logger, IList<ExternalDataSource> externalDataSources
			)
		{
			// file menu items
			JMenu fileMenu = new Menu("File", KeyEvent.VK_F);
			fileMenu.Add(new MenuItem("Load Profile...", new LoadProfileAction(logger), KeyEvent
				.VK_L, KeyStroke.GetKeyStroke(KeyEvent.VK_L, InputEvent.CTRL_MASK)));
			fileMenu.Add(new MenuItem("Reload Profile", new ReloadProfileAction(logger), KeyEvent
				.VK_P, KeyStroke.GetKeyStroke(KeyEvent.VK_P, InputEvent.CTRL_MASK)));
			fileMenu.Add(new MenuItem("Save Profile", new SaveProfileAction(logger), KeyEvent
				.VK_S, KeyStroke.GetKeyStroke(KeyEvent.VK_S, InputEvent.CTRL_MASK)));
			fileMenu.Add(new MenuItem("Save Profile As...", new SaveProfileAsAction(logger), 
				KeyEvent.VK_A, KeyStroke.GetKeyStroke(KeyEvent.VK_S, InputEvent.CTRL_MASK | InputEvent
				.SHIFT_MASK)));
			fileMenu.Add(new JSeparator());
			fileMenu.Add(new MenuItem("Exit", new ExitAction(logger), KeyEvent.VK_X));
			Add(fileMenu);
			// settings menu items
			JMenu settingsMenu = new Menu("Settings", KeyEvent.VK_S);
			//settingsMenu.add(new MenuItem("Logger Definition Location...", new LoggerDefinitionLocationAction(logger), VK_F, getKeyStroke(VK_F, CTRL_MASK)));
			settingsMenu.Add(new MenuItem("Advanced Settings...", new LoggerDefinitionLocationAction
				(logger), KeyEvent.VK_F, KeyStroke.GetKeyStroke(KeyEvent.VK_F, InputEvent.CTRL_MASK
				)));
			settingsMenu.Add(new MenuItem("Log File Output Location...", new LogFileLocationAction
				(logger), KeyEvent.VK_O, KeyStroke.GetKeyStroke(KeyEvent.VK_O, InputEvent.CTRL_MASK
				)));
			settingsMenu.Add(new JSeparator());
			settingsMenu.Add(new RadioButtonMenuItem("Control File Logging With Defogger Switch"
				, KeyEvent.VK_C, KeyStroke.GetKeyStroke(KeyEvent.VK_C, InputEvent.CTRL_MASK), new 
				LogFileControllerSwitchAction(logger), logger.GetSettings().IsFileLoggingControllerSwitchActive
				()));
			RadioButtonMenuItem autoRefresh = new RadioButtonMenuItem("Enable COM port Auto Refresh"
				, KeyEvent.VK_E, KeyStroke.GetKeyStroke(KeyEvent.VK_E, InputEvent.CTRL_MASK), new 
				ComPortAutoRefreshAction(logger), logger.GetSettings().GetRefreshMode());
			autoRefresh.SetToolTipText("Select to enable automatic COM port refreshing");
			settingsMenu.Add(autoRefresh);
			RadioButtonMenuItem fastPoll = new RadioButtonMenuItem("Enable Fast Polling Mode"
				, KeyEvent.VK_M, KeyStroke.GetKeyStroke(KeyEvent.VK_M, InputEvent.CTRL_MASK), new 
				FastPollModeAction(logger), logger.GetSettings().IsFastPoll());
			fastPoll.SetToolTipText("Select to enable faster K-line polling of the ECU");
			settingsMenu.Add(fastPoll);
			RadioButtonMenuItem canBus = new RadioButtonMenuItem("CAN bus Logging (2007+)", KeyEvent
				.VK_N, KeyStroke.GetKeyStroke(KeyEvent.VK_N, InputEvent.CTRL_MASK), new CanBusModeAction
				(logger), Settings.IsCanBus());
			canBus.SetToolTipText("Select to enable logging via CAN bus using a J2534 compatible cable"
				);
			if (Platform.IsPlatform(Platform.WINDOWS))
			{
				settingsMenu.Add(canBus);
			}
			settingsMenu.Add(new JSeparator());
			settingsMenu.Add(new RadioButtonMenuItem("Use Absolute Timestamp In Log File", KeyEvent
				.VK_T, KeyStroke.GetKeyStroke(KeyEvent.VK_T, InputEvent.CTRL_MASK), new LogFileAbsoluteTimestampAction
				(logger), logger.GetSettings().IsFileLoggingAbsoluteTimestamp()));
			RadioButtonMenuItem numFormat = new RadioButtonMenuItem("Use US English number format in Log File"
				, KeyEvent.VK_B, KeyStroke.GetKeyStroke(KeyEvent.VK_B, InputEvent.CTRL_MASK), new 
				LogFileNumberFormatAction(logger), logger.GetSettings().IsUsNumberFormat());
			numFormat.SetToolTipText("Select to force log files to be written with . decimal point and , field separator"
				);
			settingsMenu.Add(numFormat);
			Add(settingsMenu);
			// connection menu items
			JMenu connectionMenu = new Menu("Connection", KeyEvent.VK_C);
			connectionMenu.Add(new MenuItem("Reset", new ResetConnectionAction(logger), KeyEvent
				.VK_R, KeyStroke.GetKeyStroke(KeyEvent.VK_R, InputEvent.CTRL_MASK)));
			connectionMenu.Add(new MenuItem("Disconnect", new DisconnectAction(logger), KeyEvent
				.VK_D, KeyStroke.GetKeyStroke(KeyEvent.VK_D, InputEvent.CTRL_MASK)));
			Add(connectionMenu);
			// tools menu items
			JMenu toolsMenu = new Menu("Tools", KeyEvent.VK_T);
			toolsMenu.Add(new MenuItem("Reset ECU/TCU", new ResetEcuAction(logger), KeyEvent.
				VK_R, KeyStroke.GetKeyStroke(KeyEvent.VK_F7, 0)));
			Add(toolsMenu);
			// plugins menu items
			JMenu pluginsMenu = new Menu("Plugins", KeyEvent.VK_P);
			pluginsMenu.SetEnabled(!externalDataSources.IsEmpty());
			foreach (ExternalDataSource dataSource in externalDataSources)
			{
				Javax.Swing.Action action = dataSource.GetMenuAction(logger);
				if (action != null)
				{
					pluginsMenu.Add(new MenuItem(dataSource.GetName(), action));
				}
			}
			Add(pluginsMenu);
			// help menu stuff
			JMenu helpMenu = new Menu("Help", KeyEvent.VK_H);
			helpMenu.Add(new MenuItem("Update Logger Definition...", new UpdateLoggerDefAction
				(logger), KeyEvent.VK_U));
			helpMenu.Add(new JSeparator());
			ButtonGroup group = new ButtonGroup();
			JMenu debug = new JMenu("Debugging Level");
			debug.SetMnemonic(KeyEvent.VK_D);
			debug.SetToolTipText("Level of detail recorded in the rr_system.log file");
			RadioButtonMenuItem info = new RadioButtonMenuItem("INFO - normal", KeyEvent.VK_I
				, null, new LoggerDebuggingLevelAction(logger, "INFO"), Sharpen.Runtime.EqualsIgnoreCase
				(logger.GetSettings().GetLoggerDebuggingLevel(), "INFO"));
			RadioButtonMenuItem db = new RadioButtonMenuItem("DEBUG - detailed", KeyEvent.VK_D
				, null, new LoggerDebuggingLevelAction(logger, "DEBUG"), Sharpen.Runtime.EqualsIgnoreCase
				(logger.GetSettings().GetLoggerDebuggingLevel(), "DEBUG"));
			RadioButtonMenuItem trace = new RadioButtonMenuItem("TRACE - verbose", KeyEvent.VK_T
				, null, new LoggerDebuggingLevelAction(logger, "TRACE"), Sharpen.Runtime.EqualsIgnoreCase
				(logger.GetSettings().GetLoggerDebuggingLevel(), "TRACE"));
			group.Add(info);
			group.Add(db);
			group.Add(trace);
			debug.Add(info);
			debug.Add(db);
			debug.Add(trace);
			debug.Add(new JSeparator());
			debug.Add(new MenuItem("Open Debug Log Location...", new LoggerDebugLocationAction
				(logger), KeyEvent.VK_O, KeyStroke.GetKeyStroke(KeyEvent.VK_O, InputEvent.ALT_MASK
				)));
			helpMenu.Add(debug);
			helpMenu.Add(new JSeparator());
			helpMenu.Add(new MenuItem("About " + Version.PRODUCT_NAME, new AboutAction(logger
				), KeyEvent.VK_A));
			Add(helpMenu);
		}
	}
}

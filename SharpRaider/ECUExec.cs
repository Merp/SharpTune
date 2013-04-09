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
using RomRaider.Definition;
using RomRaider.Editor.Ecu;
using RomRaider.Logger.Ecu;
using RomRaider.Swing;
using RomRaider.Util;
using Sharpen;

namespace RomRaider
{
	public class ECUExec
	{
		private static readonly Logger LOGGER = Logger.GetLogger(typeof(RomRaider.ECUExec
			));

		private static readonly string START_LOGGER_ARG = "-logger";

		private static readonly string START_LOGGER_FULLSCREEN_ARG = "-logger.fullscreen";

		public static Settings settings;

		public static SettingsManager settingsManager;

		public static DefinitionRepoManager definitionRepoManager;

		private static DefinitionManager definitionManager;

		protected internal static object Lock;

		public ECUExec()
		{
			throw new NotSupportedException();
		}

		public static void Main(string[] args)
		{
			// init debug logging
			LogManager.InitDebugLogging();
			// dump the system properties to the log file as early as practical to
			// help debugging/support
			LOGGER.Info(Version.PRODUCT_NAME + " " + Version.VERSION + " Build: " + Version.BUILDNUMBER
				);
			LOGGER.Info("When requesting assistance at " + Version.SUPPORT_URL + " please include the System Properties information below:"
				);
			LOGGER.Info(DateFormat.GetDateTimeInstance(DateFormat.FULL, DateFormat.LONG).Format
				(Runtime.CurrentTimeMillis()));
			LOGGER.Info("System Properties: \n\t" + Runtime.GetProperties().ToString().Replace
				(",", "\n\t"));
			// 64-bit won't work with the native libs (e.g. serial rxtx) but won't
			// fail until we actually try to use them we'll just warn here
			if (!JREChecker.Is32bit() && !ContainsLoggerArg(args))
			{
				JOptionPane.ShowMessageDialog(null, "Incompatible JRE detected.\n" + Version.PRODUCT_NAME
					 + " requires a 32-bit JRE for some operations.\nSome features may be unavailable."
					, "JRE Incompatibility Warning", JOptionPane.WARNING_MESSAGE);
			}
			// check for dodgy threading - dev only
			//        RepaintManager.setCurrentManager(new ThreadCheckingRepaintManager(true));
			// set look and feel
			LookAndFeelManager.InitLookAndFeel();
			// check if already running
			if (RomServer.IsRunning())
			{
				if (args.Length == 0 || ContainsLoggerArg(args))
				{
					ShowAlreadyRunningMessage();
				}
				else
				{
					RomServer.SendRomToOpenInstance(args[0]);
				}
			}
			else
			{
				// open editor or logger
				Initialize();
				if (ContainsLoggerArg(args))
				{
					OpenLogger(args);
				}
				else
				{
					OpenEditor(args);
				}
			}
		}

		private static void ShowAlreadyRunningMessage()
		{
			JOptionPane.ShowMessageDialog(null, Version.PRODUCT_NAME + " is already running."
				, Version.PRODUCT_NAME, JOptionPane.INFORMATION_MESSAGE);
		}

		private static bool ContainsLoggerArg(string[] args)
		{
			foreach (string arg in args)
			{
				if (Sharpen.Runtime.EqualsIgnoreCase(arg, START_LOGGER_ARG) || Sharpen.Runtime.EqualsIgnoreCase
					(arg, START_LOGGER_FULLSCREEN_ARG))
				{
					return true;
				}
			}
			return false;
		}

		private static void Initialize()
		{
			settingsManager = new SettingsManagerImpl();
			settings = settingsManager.Load();
			definitionRepoManager = new DefinitionRepoManager();
			definitionRepoManager.Load();
			settings.CheckDefs();
			if (!settings.IsEcuDefExists())
			{
				definitionManager = new DefinitionManager();
				definitionManager.RunModal(true);
			}
			if (!settings.IsCarsDefExists())
			{
				JOptionPane.ShowMessageDialog(definitionRepoManager, "Error configuring dyno definition, configure manually! "
					, "Dyno definition configuration failed.", JOptionPane.INFORMATION_MESSAGE);
			}
			if (!settings.IsLoggerDefExists())
			{
				JOptionPane.ShowMessageDialog(definitionRepoManager, "Error configuring logger definition, configure definitions manually! "
					, "Logger definition configuration failed.", JOptionPane.INFORMATION_MESSAGE);
			}
		}

		private static void OpenLogger(string[] args)
		{
			EcuLogger.StartLogger(WindowConstants.EXIT_ON_CLOSE, settings, args);
		}

		private static void OpenRom(ECUEditor editor, string rom)
		{
			SwingUtilities.InvokeLater(new _Runnable_164(rom, editor));
		}

		private sealed class _Runnable_164 : Runnable
		{
			public _Runnable_164(string rom, ECUEditor editor)
			{
				this.rom = rom;
				this.editor = editor;
			}

			public void Run()
			{
				try
				{
					FilePath file = new FilePath(rom);
					editor.OpenImage(file);
				}
				catch (Exception ex)
				{
					RomRaider.ECUExec.LOGGER.Error("Error opening rom", ex);
				}
			}

			private readonly string rom;

			private readonly ECUEditor editor;
		}

		private static void OpenEditor(string[] args)
		{
			ECUEditor editor = ECUEditorManager.GetECUEditor();
			if (args.Length > 0)
			{
				OpenRom(editor, args[0]);
			}
			StartRomListener(editor);
		}

		private static void StartRomListener(ECUEditor editor)
		{
			try
			{
				while (true)
				{
					string rom = RomServer.WaitForRom();
					OpenRom(editor, rom);
				}
			}
			catch (Exception e)
			{
				LOGGER.Error("Error occurred", e);
			}
		}
	}
}

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
using Java.Awt.Event;
using Javax.Swing;
using RomRaider;
using RomRaider.Logger.Ecu;
using RomRaider.Logger.Ecu.UI.Swing.Menubar.Util;
using RomRaider.Swing.Menubar.Action;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.Ecu.UI.Swing.Menubar.Action
{
	public sealed class LoadProfileAction : AbstractAction
	{
		public LoadProfileAction(EcuLogger logger) : base(logger)
		{
		}

		public override void ActionPerformed(ActionEvent actionEvent)
		{
			try
			{
				LoadProfileDialog();
			}
			catch (Exception e)
			{
				logger.ReportError(e);
			}
		}

		/// <exception cref="System.Exception"></exception>
		private void LoadProfileDialog()
		{
			logger.GetSettings();
			FilePath lastProfileFile = FileHelper.GetFile(Settings.GetLoggerProfileFilePath()
				);
			JFileChooser fc = FileHelper.GetProfileFileChooser(lastProfileFile);
			if (fc.ShowOpenDialog(logger) == JFileChooser.APPROVE_OPTION)
			{
				string profileFilePath = fc.GetSelectedFile().GetAbsolutePath();
				logger.LoadUserProfile(profileFilePath);
				logger.GetSettings().SetLoggerProfileFilePath(profileFilePath);
				logger.ReportMessageInTitleBar(string.Empty + "Profile: " + FormatFilename.GetShortName
					(profileFilePath));
				logger.RestartLogging();
				logger.ReportMessage("Profile succesfully loaded: " + profileFilePath);
			}
		}
	}
}

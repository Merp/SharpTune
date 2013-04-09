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
using System.IO;
using Javax.Swing;
using RomRaider.Logger.Ecu.Profile;
using RomRaider.Swing;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.Ecu.UI.Swing.Menubar.Util
{
	public sealed class FileHelper
	{
		private static readonly string USER_HOME_DIR = Runtime.GetProperty("user.home");

		public FileHelper()
		{
			throw new NotSupportedException();
		}

		public static FilePath GetFile(string filePath)
		{
			return ParamChecker.IsNullOrEmpty(filePath) ? new FilePath(USER_HOME_DIR) : new FilePath
				(filePath);
		}

		public static JFileChooser GetProfileFileChooser(FilePath lastProfileFile)
		{
			return GetFileChooser(lastProfileFile, "ECU Logger User Profiles", "xml");
		}

		public static JFileChooser GetDefinitionFileChooser(FilePath lastDefFile)
		{
			return GetFileChooser(lastDefFile, "ECU Logger Definitions", "xml");
		}

		/// <exception cref="System.IO.IOException"></exception>
		public static string SaveProfileToFile(UserProfile profile, FilePath destinationFile
			)
		{
			string profileFilePath = destinationFile.GetAbsolutePath();
			if (!profileFilePath.EndsWith(".xml"))
			{
				profileFilePath += ".xml";
				destinationFile = new FilePath(profileFilePath);
			}
			FileOutputStream fos = new FileOutputStream(destinationFile);
			try
			{
				fos.Write(profile.GetBytes());
			}
			finally
			{
				fos.Close();
			}
			return profileFilePath;
		}

		public static JFileChooser GetLoggerOutputDirFileChooser(FilePath lastLoggerOutputDir
			)
		{
			JFileChooser fc;
			if (lastLoggerOutputDir.Exists() && lastLoggerOutputDir.IsDirectory())
			{
				fc = new JFileChooser(lastLoggerOutputDir.GetAbsolutePath());
			}
			else
			{
				fc = new JFileChooser();
			}
			fc.SetFileSelectionMode(JFileChooser.DIRECTORIES_ONLY);
			return fc;
		}

		private static JFileChooser GetFileChooser(FilePath file, string description, params 
			string[] extensions)
		{
			JFileChooser fc = GetFileChooser(file);
			fc.SetFileFilter(new GenericFileFilter(description, extensions));
			return fc;
		}

		private static JFileChooser GetFileChooser(FilePath file)
		{
			if (file.Exists() && file.IsFile() && file.GetParentFile() != null)
			{
				string dir = file.GetParentFile().GetAbsolutePath();
				JFileChooser fc = new JFileChooser(dir);
				fc.SetSelectedFile(file);
				return fc;
			}
			return new JFileChooser();
		}
	}
}

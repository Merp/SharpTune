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
using Org.Apache.Log4j;
using Org.Jdesktop.Jdic.Filetypes;
using Sharpen;

namespace RomRaider.Util
{
	public sealed class FileAssociator
	{
		private static readonly Logger LOGGER = Logger.GetLogger(typeof(RomRaider.Util.FileAssociator
			));

		public FileAssociator()
		{
			throw new NotSupportedException();
		}

		public static bool AddAssociation(string extension, string command, string description
			)
		{
			// Add association
			// StringTokenizer osName = new StringTokenizer(System.getProperties().getProperty("os.name"));
			// remove association if it already exists
			LOGGER.Debug("Removing 1...");
			RemoveAssociation(extension);
			LOGGER.Debug("Removing 2...");
			AssociationService serv = new AssociationService();
			Association logassoc = new Association();
			logassoc.AddFileExtension(extension.ToUpper());
			logassoc.SetDescription(description);
			logassoc.AddAction(new Action("open", command + " %1"));
			logassoc.SetIconFileName(typeof(RomRaider.Util.FileAssociator).GetType().GetResource
				("/graphics/romraider-ico.ico").ToString());
			LOGGER.Debug("Adding ...\n" + logassoc + "\n\n\n");
			try
			{
				serv.RegisterUserAssociation(logassoc);
			}
			catch (Exception e)
			{
				LOGGER.Error("Error adding association", e);
			}
			return true;
		}

		public static bool RemoveAssociation(string extension)
		{
			AssociationService serv = new AssociationService();
			Association logassoc = serv.GetFileExtensionAssociation(extension.ToUpper());
			LOGGER.Debug("Removing ...\n" + logassoc + "\n\n\n");
			try
			{
				serv.UnregisterUserAssociation(logassoc);
			}
			catch (Exception e)
			{
				LOGGER.Error("Error removing association", e);
			}
			return true;
		}
	}
}

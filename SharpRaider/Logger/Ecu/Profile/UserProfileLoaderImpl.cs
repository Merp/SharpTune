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
using Org.Xml.Sax;
using RomRaider.Logger.Ecu.Profile;
using RomRaider.Logger.Ecu.Profile.Xml;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.Ecu.Profile
{
	public sealed class UserProfileLoaderImpl : UserProfileLoader
	{
		private static readonly Org.Apache.Log4j.Logger LOGGER = Org.Apache.Log4j.Logger.
			GetLogger(typeof(UserProfileLoaderImpl));

		public override UserProfile LoadProfile(string userProfileFilePath)
		{
			ParamChecker.CheckNotNullOrEmpty(userProfileFilePath, "userProfileFilePath");
			LOGGER.Info("Loading profile: " + userProfileFilePath);
			try
			{
				InputStream inputStream = new BufferedInputStream(new FileInputStream(new FilePath
					(userProfileFilePath)));
				try
				{
					UserProfileHandler handler = new UserProfileHandler();
					SaxParserFactory.GetSaxParser().Parse(inputStream, handler);
					return handler.GetUserProfile();
				}
				finally
				{
					inputStream.Close();
				}
			}
			catch (SAXParseException)
			{
				// catch general parsing exception - enough people don't unzip the defs that a better error message is in order
				LOGGER.Error("Error loading user profile file: " + userProfileFilePath + ".  Please make sure the definition file is correct.  If it is in a ZIP archive, unzip the file and try again."
					);
				return null;
			}
			catch (Exception e)
			{
				LOGGER.Error("Error loading user profile file: " + userProfileFilePath, e);
				return null;
			}
		}
	}
}

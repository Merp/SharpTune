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
using System.Reflection;
using Org.Apache.Log4j;
using Sharpen;

namespace RomRaider.Net
{
	public class BrowserControl
	{
		private static readonly Logger LOGGER = Logger.GetLogger(typeof(RomRaider.Net.BrowserControl
			));

		public BrowserControl()
		{
			throw new NotSupportedException();
		}

		public static void DisplayURL(string url)
		{
			try
			{
				Type display = Sharpen.Runtime.GetType("java.awt.Desktop");
				object getDesktopMethod = Sharpen.Runtime.GetDeclaredMethod(display, "getDesktop"
					).Invoke(null);
				MethodInfo browseMethod = Sharpen.Runtime.GetDeclaredMethod(display, "browse", typeof(
					URI));
				browseMethod.Invoke(getDesktopMethod, new URI(url));
			}
			catch (Exception e)
			{
				LOGGER.Debug("Failed to display URL via java.awt.Desktop. Calling by OS depended method."
					, e);
				DisplayURLtraditional(url);
			}
		}

		private static void DisplayURLtraditional(string url)
		{
			bool windows = IsWindowsPlatform();
			string cmd = null;
			try
			{
				if (windows)
				{
					// cmd = 'rundll32 url.dll,FileProtocolHandler http://...'
					cmd = WIN_PATH + " " + WIN_FLAG + " " + url;
					Runtime.GetRuntime().Exec(cmd);
				}
				else
				{
					cmd = UNIX_PATH + " " + UNIX_FLAG + "(" + url + ")";
					SystemProcess p = Runtime.GetRuntime().Exec(cmd);
					try
					{
						int exitCode = p.WaitFor();
						if (exitCode != 0)
						{
							cmd = UNIX_PATH + " " + url;
							Runtime.GetRuntime().Exec(cmd);
						}
					}
					catch (Exception x)
					{
						LOGGER.Error("Error bringing up browser, command=" + cmd, x);
					}
				}
			}
			catch (IOException x)
			{
				// couldn't exec browser
				LOGGER.Error("Could not invoke browser, command=" + cmd, x);
			}
		}

		public static bool IsWindowsPlatform()
		{
			string os = Runtime.GetProperty("os.name");
			if (os != null && os.StartsWith(WIN_ID))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		private static readonly string WIN_ID = "Windows";

		private static readonly string WIN_PATH = "rundll32";

		private static readonly string WIN_FLAG = "url.dll,FileProtocolHandler";

		private static readonly string UNIX_PATH = "netscape";

		private static readonly string UNIX_FLAG = "-remote openURL";
	}
}

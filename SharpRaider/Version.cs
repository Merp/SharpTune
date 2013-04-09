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

using Javax.Swing;
using Sharpen;

namespace RomRaider
{
	public sealed class Version
	{
		public static readonly string PRODUCT_NAME = "RomRaider";

		public static readonly string VERSION = "0.5.6 RC 5";

		public static readonly string BUILDNUMBER = "445";

		public static readonly string SUPPORT_URL = "http://www.romraider.com";

		public static readonly string ROM_REVISION_URL = "http://www.scoobypedia.co.uk/index.php/Knowledge/ECUVersionCompatibilityList";

		public static readonly string ECU_DEFS_URL = "http://www.romraider.com/forum/topic360.html";

		public static readonly string LOGGER_DEFS_URL = "http://www.romraider.com/forum/topic1642.html";

		public static readonly string CARS_DEFS_URL = "http://www.romraider.com/forum/topic5792.html";

		public static readonly string RELEASE_NOTES = "release_notes.txt";

		public static readonly ImageIcon ABOUT_ICON = new ImageIcon(typeof(RomRaider.Version
			).GetType().GetResource("/graphics/romraider-ico-large.gif"));

		public Version()
		{
		}
		// DO NOT EDIT.  This file is automatically generated.
	}
}
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
using Org.Xml.Sax;
using Org.Xml.Sax.Helpers;
using RomRaider.Logger.Ecu.Profile;
using RomRaider.Logger.Ecu.Profile.Xml;
using Sharpen;

namespace RomRaider.Logger.Ecu.Profile.Xml
{
	public sealed class UserProfileHandler : DefaultHandler
	{
		private static readonly string SELECTED = "selected";

		private static readonly string TAG_PARAMETER = "parameter";

		private static readonly string TAG_SWITCH = "switch";

		private static readonly string TAG_EXTERNAL = "external";

		private static readonly string ATTR_ID = "id";

		private static readonly string ATTR_UNITS = "units";

		private static readonly string ATTR_LIVE_DATA = "livedata";

		private static readonly string ATTR_GRAPH = "graph";

		private static readonly string ATTR_DASH = "dash";

		private IDictionary<string, UserProfileItem> @params;

		private IDictionary<string, UserProfileItem> switches;

		private IDictionary<string, UserProfileItem> external;

		public override void StartDocument()
		{
			@params = new Dictionary<string, UserProfileItem>();
			switches = new Dictionary<string, UserProfileItem>();
			external = new Dictionary<string, UserProfileItem>();
		}

		public override void StartElement(string uri, string localName, string qName, Attributes
			 attributes)
		{
			if (TAG_PARAMETER.Equals(qName))
			{
				@params.Put(attributes.GetValue(ATTR_ID), GetUserProfileItem(attributes));
			}
			else
			{
				if (TAG_SWITCH.Equals(qName))
				{
					switches.Put(attributes.GetValue(ATTR_ID), GetUserProfileItem(attributes));
				}
				else
				{
					if (TAG_EXTERNAL.Equals(qName))
					{
						external.Put(attributes.GetValue(ATTR_ID), GetUserProfileItem(attributes));
					}
				}
			}
		}

		public UserProfile GetUserProfile()
		{
			return new UserProfileImpl(@params, switches, external);
		}

		private UserProfileItem GetUserProfileItem(Attributes attributes)
		{
			return new UserProfileItemImpl(attributes.GetValue(ATTR_UNITS), Sharpen.Runtime.EqualsIgnoreCase
				(SELECTED, attributes.GetValue(ATTR_LIVE_DATA)), Sharpen.Runtime.EqualsIgnoreCase
				(SELECTED, attributes.GetValue(ATTR_GRAPH)), Sharpen.Runtime.EqualsIgnoreCase(SELECTED
				, attributes.GetValue(ATTR_DASH)));
		}
	}
}

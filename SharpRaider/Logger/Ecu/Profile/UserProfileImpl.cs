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
using System.Collections.Generic;
using System.Text;
using RomRaider.Logger.Ecu.Definition;
using RomRaider.Logger.Ecu.Exception;
using RomRaider.Logger.Ecu.Profile;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.Ecu.Profile
{
	public sealed class UserProfileImpl : UserProfile
	{
		private static readonly string NEW_LINE = Runtime.GetProperty("line.separator");

		private readonly IDictionary<string, UserProfileItem> @params;

		private readonly IDictionary<string, UserProfileItem> switches;

		private readonly IDictionary<string, UserProfileItem> external;

		public UserProfileImpl(IDictionary<string, UserProfileItem> @params, IDictionary<
			string, UserProfileItem> switches, IDictionary<string, UserProfileItem> external
			)
		{
			ParamChecker.CheckNotNull(@params, "params");
			ParamChecker.CheckNotNull(switches, "switches");
			ParamChecker.CheckNotNull(external, "external");
			this.@params = @params;
			this.switches = switches;
			this.external = external;
		}

		public bool Contains(LoggerData loggerData)
		{
			ParamChecker.CheckNotNull(loggerData, "loggerData");
			return GetMap(loggerData).Keys.Contains(loggerData.GetId());
		}

		public bool IsSelectedOnLiveDataTab(LoggerData loggerData)
		{
			ParamChecker.CheckNotNull(loggerData, "loggerData");
			return Contains(loggerData) && GetUserProfileItem(loggerData).IsLiveDataSelected(
				);
		}

		public bool IsSelectedOnGraphTab(LoggerData loggerData)
		{
			ParamChecker.CheckNotNull(loggerData, "loggerData");
			return Contains(loggerData) && GetUserProfileItem(loggerData).IsGraphSelected();
		}

		public bool IsSelectedOnDashTab(LoggerData loggerData)
		{
			ParamChecker.CheckNotNull(loggerData, "loggerData");
			return Contains(loggerData) && GetUserProfileItem(loggerData).IsDashSelected();
		}

		public EcuDataConvertor GetSelectedConvertor(LoggerData loggerData)
		{
			ParamChecker.CheckNotNull(loggerData, "loggerData");
			if (Contains(loggerData))
			{
				string defaultUnits = GetUserProfileItem(loggerData).GetUnits();
				if (defaultUnits != null && loggerData.GetConvertors().Length > 1)
				{
					foreach (EcuDataConvertor convertor in loggerData.GetConvertors())
					{
						if (defaultUnits.Equals(convertor.GetUnits()))
						{
							return convertor;
						}
					}
					throw new ConfigurationException("Unknown default units, '" + defaultUnits + "', specified for ["
						 + loggerData.GetId() + "] " + loggerData.GetName());
				}
			}
			return loggerData.GetSelectedConvertor();
		}

		public byte[] GetBytes()
		{
			return Sharpen.Runtime.GetBytesForString(BuildXml());
		}

		private string BuildXml()
		{
			StringBuilder builder = new StringBuilder();
			builder.Append("<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>").Append(NEW_LINE
				);
			builder.Append("<!DOCTYPE profile SYSTEM \"profile.dtd\">").Append(NEW_LINE).Append
				(NEW_LINE);
			builder.Append("<profile>").Append(NEW_LINE);
			if (!@params.IsEmpty())
			{
				builder.Append("    <parameters>").Append(NEW_LINE);
				AppendLoggerDataElements(builder, "parameter", @params, true);
				builder.Append("    </parameters>").Append(NEW_LINE);
			}
			if (!switches.IsEmpty())
			{
				builder.Append("    <switches>").Append(NEW_LINE);
				AppendLoggerDataElements(builder, "switch", switches, false);
				builder.Append("    </switches>").Append(NEW_LINE);
			}
			if (!external.IsEmpty())
			{
				builder.Append("    <externals>").Append(NEW_LINE);
				// Comment out the next line to disable Externals from being saved
				// as there seems to be a bug when a profile is reloaded, the 
				// Logger tries to connect twice causing one or both to fail. 
				AppendLoggerDataElements(builder, "external", external, true);
				builder.Append("    </externals>").Append(NEW_LINE);
			}
			builder.Append("</profile>").Append(NEW_LINE);
			return builder.ToString();
		}

		private void AppendLoggerDataElements(StringBuilder builder, string dataType, IDictionary
			<string, UserProfileItem> dataMap, bool showUnits)
		{
			foreach (string id in dataMap.Keys)
			{
				UserProfileItem item = dataMap.Get(id);
				builder.Append("        <").Append(dataType).Append(" id=\"").Append(id).Append("\""
					);
				if (item.IsLiveDataSelected())
				{
					builder.Append(" livedata=\"selected\"");
				}
				if (item.IsGraphSelected())
				{
					builder.Append(" graph=\"selected\"");
				}
				if (item.IsDashSelected())
				{
					builder.Append(" dash=\"selected\"");
				}
				if (showUnits && !ParamChecker.IsNullOrEmpty(item.GetUnits()))
				{
					builder.Append(" units=\"").Append(item.GetUnits()).Append("\"");
				}
				builder.Append("/>").Append(NEW_LINE);
			}
		}

		private UserProfileItem GetUserProfileItem(LoggerData loggerData)
		{
			return GetMap(loggerData).Get(loggerData.GetId());
		}

		private IDictionary<string, UserProfileItem> GetMap(LoggerData loggerData)
		{
			if (loggerData is EcuParameter)
			{
				return @params;
			}
			else
			{
				if (loggerData is EcuSwitch)
				{
					return switches;
				}
				else
				{
					if (loggerData is ExternalData)
					{
						return external;
					}
					else
					{
						throw new NotSupportedException("Unknown LoggerData type: " + loggerData.GetType(
							));
					}
				}
			}
		}
	}
}

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
using System.IO;
using Com.Sun.Org.Apache.Xml.Internal.Serialize;
using Javax.Imageio.Metadata;
using RomRaider;
using RomRaider.Swing;
using RomRaider.Xml;
using Sharpen;

namespace RomRaider.Xml
{
	public sealed class DOMSettingsBuilder
	{
		/// <exception cref="System.IO.IOException"></exception>
		public void BuildSettings(Settings settings, FilePath output, JProgressPane progress
			, string versionNumber)
		{
			IIOMetadataNode settingsNode = new IIOMetadataNode("settings");
			// create settings
			progress.Update("Saving window settings...", 15);
			settingsNode.AppendChild(BuildWindow(settings));
			progress.Update("Saving file settings...", 30);
			settingsNode.AppendChild(BuildFiles(settings));
			progress.Update("Saving options...", 45);
			settingsNode.AppendChild(BuildOptions(settings, versionNumber));
			progress.Update("Saving display settings...", 60);
			settingsNode.AppendChild(BuildTableDisplay(settings));
			progress.Update("Saving logger settings...", 75);
			settingsNode.AppendChild(BuildLogger(settings));
			progress.Update("Saving table clipboard format settings...", 80);
			settingsNode.AppendChild(BuildTableClipboardFormat(settings));
			progress.Update("Saving icon scale settings...", 85);
			settingsNode.AppendChild(BuildIcons(settings));
			OutputFormat of = new OutputFormat("XML", "ISO-8859-1", true);
			of.SetIndent(1);
			of.SetIndenting(true);
			progress.Update("Writing to file...", 90);
			FileOutputStream fos = new FileOutputStream(output);
			try
			{
				XMLSerializer serializer = new XMLSerializer(fos, of);
				serializer.Serialize(settingsNode);
				fos.Flush();
			}
			finally
			{
				fos.Close();
			}
		}

		private IIOMetadataNode BuildWindow(Settings settings)
		{
			IIOMetadataNode windowSettings = new IIOMetadataNode("window");
			// window maximized
			IIOMetadataNode maximized = new IIOMetadataNode("maximized");
			maximized.SetAttribute("value", (settings.IsWindowMaximized()).ToString());
			windowSettings.AppendChild(maximized);
			// window size
			IIOMetadataNode size = new IIOMetadataNode("size");
			size.SetAttribute("x", ((int)settings.GetWindowSize().GetHeight()).ToString());
			size.SetAttribute("y", ((int)settings.GetWindowSize().GetWidth()).ToString());
			windowSettings.AppendChild(size);
			// window location
			IIOMetadataNode location = new IIOMetadataNode("location");
			location.SetAttribute("x", ((int)settings.GetWindowLocation().GetX()).ToString());
			location.SetAttribute("y", ((int)settings.GetWindowLocation().GetY()).ToString());
			windowSettings.AppendChild(location);
			// splitpane location
			IIOMetadataNode splitpane = new IIOMetadataNode("splitpane");
			splitpane.SetAttribute("location", settings.GetSplitPaneLocation().ToString());
			windowSettings.AppendChild(splitpane);
			return windowSettings;
		}

		private IIOMetadataNode BuildFiles(Settings settings)
		{
			IIOMetadataNode files = new IIOMetadataNode("files");
			// image directory
			IIOMetadataNode imageDir = new IIOMetadataNode("image_dir");
			imageDir.SetAttribute("path", settings.GetLastImageDir().GetAbsolutePath());
			files.AppendChild(imageDir);
			// repository directory
			IIOMetadataNode repositoryDir = new IIOMetadataNode(Settings.REPOSITORY_ELEMENT_NAME
				);
			repositoryDir.SetAttribute(Settings.REPOSITORY_ATTRIBUTE_NAME, settings.GetLastRepositoryDir
				().GetAbsolutePath());
			files.AppendChild(repositoryDir);
			// ecu definition files
			Vector<FilePath> defFiles = settings.GetEcuDefinitionFiles();
			foreach (FilePath defFile in defFiles)
			{
				IIOMetadataNode ecuDef = new IIOMetadataNode("ecudefinitionfile");
				ecuDef.SetAttribute("name", defFile.GetAbsolutePath());
				files.AppendChild(ecuDef);
			}
			return files;
		}

		private IIOMetadataNode BuildOptions(Settings settings, string versionNumber)
		{
			IIOMetadataNode options = new IIOMetadataNode("options");
			// obsolete warning
			IIOMetadataNode obsoleteWarning = new IIOMetadataNode("obsoletewarning");
			obsoleteWarning.SetAttribute("value", settings.IsObsoleteWarning().ToString());
			options.AppendChild(obsoleteWarning);
			// calcultion conflicting warning
			IIOMetadataNode calcConflictWarning = new IIOMetadataNode("calcconflictwarning");
			calcConflictWarning.SetAttribute("value", settings.IsCalcConflictWarning().ToString
				());
			options.AppendChild(calcConflictWarning);
			// debug mode
			IIOMetadataNode debug = new IIOMetadataNode("debug");
			debug.SetAttribute("value", settings.IsDebug().ToString());
			options.AppendChild(debug);
			// userlevel
			IIOMetadataNode userLevel = new IIOMetadataNode("userlevel");
			userLevel.SetAttribute("value", settings.GetUserLevel().ToString());
			options.AppendChild(userLevel);
			// table click count
			IIOMetadataNode tableClickCount = new IIOMetadataNode("tableclickcount");
			tableClickCount.SetAttribute("value", settings.GetTableClickCount().ToString());
			options.AppendChild(tableClickCount);
			// last version used
			IIOMetadataNode version = new IIOMetadataNode("version");
			version.SetAttribute("value", versionNumber);
			options.AppendChild(version);
			// save debug level tables
			IIOMetadataNode saveDebugTables = new IIOMetadataNode("savedebugtables");
			saveDebugTables.SetAttribute("value", settings.IsSaveDebugTables().ToString());
			options.AppendChild(saveDebugTables);
			// display tables higher than userlevel
			IIOMetadataNode displayHighTables = new IIOMetadataNode("displayhightables");
			displayHighTables.SetAttribute("value", settings.IsDisplayHighTables().ToString()
				);
			options.AppendChild(displayHighTables);
			// warning when exceeding limits
			IIOMetadataNode valueLimitWarning = new IIOMetadataNode("valuelimitwarning");
			valueLimitWarning.SetAttribute("value", settings.IsValueLimitWarning().ToString()
				);
			options.AppendChild(valueLimitWarning);
			return options;
		}

		private IIOMetadataNode BuildTableDisplay(Settings settings)
		{
			IIOMetadataNode tableDisplay = new IIOMetadataNode("tabledisplay");
			// font
			IIOMetadataNode font = new IIOMetadataNode("font");
			font.SetAttribute("face", settings.GetTableFont().GetName());
			font.SetAttribute("size", settings.GetTableFont().GetSize().ToString());
			font.SetAttribute("decoration", settings.GetTableFont().GetStyle().ToString());
			tableDisplay.AppendChild(font);
			// table cell size
			IIOMetadataNode cellSize = new IIOMetadataNode("cellsize");
			cellSize.SetAttribute("height", (int)settings.GetCellSize().GetHeight().ToString(
				));
			cellSize.SetAttribute("width", ((int)settings.GetCellSize().GetWidth()).ToString(
				));
			tableDisplay.AppendChild(cellSize);
			// colors
			IIOMetadataNode colors = new IIOMetadataNode("colors");
			// max
			IIOMetadataNode max = new IIOMetadataNode("max");
			max.SetAttribute("r", settings.GetMaxColor().GetRed().ToString());
			max.SetAttribute("g", settings.GetMaxColor().GetGreen().ToString());
			max.SetAttribute("b", settings.GetMaxColor().GetBlue().ToString());
			colors.AppendChild(max);
			// min
			IIOMetadataNode min = new IIOMetadataNode("min");
			min.SetAttribute("r", settings.GetMinColor().GetRed().ToString());
			min.SetAttribute("g", settings.GetMinColor().GetGreen().ToString());
			min.SetAttribute("b", settings.GetMinColor().GetBlue().ToString());
			colors.AppendChild(min);
			// highlight
			IIOMetadataNode highlight = new IIOMetadataNode("highlight");
			highlight.SetAttribute("r", settings.GetHighlightColor().GetRed().ToString());
			highlight.SetAttribute("g", settings.GetHighlightColor().GetGreen().ToString());
			highlight.SetAttribute("b", settings.GetHighlightColor().GetBlue().ToString());
			colors.AppendChild(highlight);
			// increased cell border
			IIOMetadataNode increaseBorder = new IIOMetadataNode("increaseborder");
			increaseBorder.SetAttribute("r", settings.GetIncreaseBorder().GetRed().ToString()
				);
			increaseBorder.SetAttribute("g", settings.GetIncreaseBorder().GetGreen().ToString
				());
			increaseBorder.SetAttribute("b", settings.GetIncreaseBorder().GetBlue().ToString(
				));
			colors.AppendChild(increaseBorder);
			// decreased cell border
			IIOMetadataNode decreaseBorder = new IIOMetadataNode("decreaseborder");
			decreaseBorder.SetAttribute("r", settings.GetDecreaseBorder().GetRed().ToString()
				);
			decreaseBorder.SetAttribute("g", settings.GetDecreaseBorder().GetGreen().ToString
				());
			decreaseBorder.SetAttribute("b", settings.GetDecreaseBorder().GetBlue().ToString(
				));
			colors.AppendChild(decreaseBorder);
			// axis cells
			IIOMetadataNode axis = new IIOMetadataNode("axis");
			axis.SetAttribute("r", settings.GetAxisColor().GetRed().ToString());
			axis.SetAttribute("g", settings.GetAxisColor().GetGreen().ToString());
			axis.SetAttribute("b", settings.GetAxisColor().GetBlue().ToString());
			colors.AppendChild(axis);
			// warning cells
			IIOMetadataNode warning = new IIOMetadataNode("warning");
			warning.SetAttribute("r", settings.GetWarningColor().GetRed().ToString());
			warning.SetAttribute("g", settings.GetWarningColor().GetGreen().ToString());
			warning.SetAttribute("b", settings.GetWarningColor().GetBlue().ToString());
			colors.AppendChild(warning);
			tableDisplay.AppendChild(colors);
			return tableDisplay;
		}

		private IIOMetadataNode BuildLogger(Settings settings)
		{
			IIOMetadataNode loggerSettings = new IIOMetadataNode("logger");
			loggerSettings.SetAttribute("locale", settings.GetLocale());
			// serial connection
			IIOMetadataNode serial = new IIOMetadataNode("serial");
			serial.SetAttribute("port", settings.GetLoggerPortDefault());
			serial.SetAttribute("refresh", settings.GetRefreshMode().ToString());
			serial.SetAttribute("ecuid", Settings.GetDestinationId().ToString());
			serial.SetAttribute("fastpoll", settings.IsFastPoll().ToString());
			loggerSettings.AppendChild(serial);
			// Protocol connection
			IIOMetadataNode protocol = new IIOMetadataNode("protocol");
			protocol.SetAttribute("name", Settings.GetLoggerProtocol());
			protocol.SetAttribute("transport", Settings.GetTransportProtocol());
			protocol.SetAttribute("library", Settings.GetJ2534Device());
			loggerSettings.AppendChild(protocol);
			// window maximized
			IIOMetadataNode maximized = new IIOMetadataNode("maximized");
			maximized.SetAttribute("value", (settings.IsLoggerWindowMaximized()).ToString());
			loggerSettings.AppendChild(maximized);
			// window size
			IIOMetadataNode size = new IIOMetadataNode("size");
			size.SetAttribute("x", ((int)settings.GetLoggerWindowSize().GetHeight()).ToString
				());
			size.SetAttribute("y", ((int)settings.GetLoggerWindowSize().GetWidth()).ToString(
				));
			size.SetAttribute("divider", ((int)settings.GetDividerLocation()).ToString());
			loggerSettings.AppendChild(size);
			// window location
			IIOMetadataNode location = new IIOMetadataNode("location");
			location.SetAttribute("x", ((int)settings.GetLoggerWindowLocation().GetX()).ToString
				());
			location.SetAttribute("y", ((int)settings.GetLoggerWindowLocation().GetY()).ToString
				());
			loggerSettings.AppendChild(location);
			// last tab index
			IIOMetadataNode tabs = new IIOMetadataNode("tabs");
			tabs.SetAttribute("selected", settings.GetLoggerSelectedTabIndex().ToString());
			tabs.SetAttribute("showlist", settings.GetLoggerParameterListState().ToString());
			loggerSettings.AppendChild(tabs);
			// definition path
			IIOMetadataNode definition = new IIOMetadataNode("definition");
			definition.SetAttribute("path", settings.GetLoggerDefFilePath());
			loggerSettings.AppendChild(definition);
			// profile path
			IIOMetadataNode profile = new IIOMetadataNode("profile");
			profile.SetAttribute("path", Settings.GetLoggerProfileFilePath());
			loggerSettings.AppendChild(profile);
			// file logging
			IIOMetadataNode filelogging = new IIOMetadataNode("filelogging");
			filelogging.SetAttribute("path", settings.GetLoggerOutputDirPath());
			filelogging.SetAttribute("switchid", settings.GetFileLoggingControllerSwitchId());
			filelogging.SetAttribute("active", settings.IsFileLoggingControllerSwitchActive()
				.ToString());
			filelogging.SetAttribute("absolutetimestamp", settings.IsFileLoggingAbsoluteTimestamp
				().ToString());
			loggerSettings.AppendChild(filelogging);
			// debug level
			IIOMetadataNode debug = new IIOMetadataNode("debug");
			debug.SetAttribute("level", settings.GetLoggerDebuggingLevel());
			loggerSettings.AppendChild(debug);
			// plugin ports
			IDictionary<string, string> pluginPorts = settings.GetLoggerPluginPorts();
			if (pluginPorts != null && !pluginPorts.IsEmpty())
			{
				IIOMetadataNode plugins = new IIOMetadataNode("plugins");
				foreach (KeyValuePair<string, string> entry in pluginPorts.EntrySet())
				{
					IIOMetadataNode plugin = new IIOMetadataNode("plugin");
					plugin.SetAttribute("id", entry.Key);
					plugin.SetAttribute("port", entry.Value);
					plugins.AppendChild(plugin);
				}
				loggerSettings.AppendChild(plugins);
			}
			return loggerSettings;
		}

		private IIOMetadataNode BuildTableClipboardFormat(Settings settings)
		{
			// Head Node
			IIOMetadataNode tableClipboardFormatSetting = new IIOMetadataNode(Settings.TABLE_CLIPBOARD_FORMAT_ELEMENT
				);
			tableClipboardFormatSetting.SetAttribute(Settings.TABLE_CLIPBOARD_FORMAT_ATTRIBUTE
				, settings.GetTableClipboardFormat());
			// Table Child
			IIOMetadataNode tableFormatSetting = new IIOMetadataNode(Settings.TABLE_ELEMENT);
			// Table1D Child
			IIOMetadataNode table1DFormatSetting = new IIOMetadataNode(Settings.TABLE1D_ELEMENT
				);
			// Table2D Child
			IIOMetadataNode table2DFormatSetting = new IIOMetadataNode(Settings.TABLE2D_ELEMENT
				);
			// Table3D Child
			IIOMetadataNode table3DFormatSetting = new IIOMetadataNode(Settings.TABLE3D_ELEMENT
				);
			tableFormatSetting.SetAttribute(Settings.TABLE_HEADER_ATTRIBUTE, settings.GetTableHeader
				());
			table1DFormatSetting.SetAttribute(Settings.TABLE_HEADER_ATTRIBUTE, settings.GetTable1DHeader
				());
			table2DFormatSetting.SetAttribute(Settings.TABLE_HEADER_ATTRIBUTE, settings.GetTable2DHeader
				());
			table3DFormatSetting.SetAttribute(Settings.TABLE_HEADER_ATTRIBUTE, settings.GetTable3DHeader
				());
			tableClipboardFormatSetting.AppendChild(tableFormatSetting);
			tableClipboardFormatSetting.AppendChild(table1DFormatSetting);
			tableClipboardFormatSetting.AppendChild(table2DFormatSetting);
			tableClipboardFormatSetting.AppendChild(table3DFormatSetting);
			return tableClipboardFormatSetting;
		}

		private IIOMetadataNode BuildIcons(Settings settings)
		{
			// Head Node
			IIOMetadataNode iconsSettings = new IIOMetadataNode(Settings.ICONS_ELEMENT_NAME);
			// Editor Icons Child
			IIOMetadataNode editorIconsScaleSettings = new IIOMetadataNode(Settings.EDITOR_ICONS_ELEMENT_NAME
				);
			editorIconsScaleSettings.SetAttribute(Settings.EDITOR_ICONS_SCALE_ATTRIBUTE_NAME, 
				settings.GetEditorIconScale().ToString());
			// Table Icons Child
			IIOMetadataNode tableIconsScaleSettings = new IIOMetadataNode(Settings.TABLE_ICONS_ELEMENT_NAME
				);
			tableIconsScaleSettings.SetAttribute(Settings.TABLE_ICONS_SCALE_ATTRIBUTE_NAME, settings
				.GetTableIconScale().ToString());
			iconsSettings.AppendChild(editorIconsScaleSettings);
			iconsSettings.AppendChild(tableIconsScaleSettings);
			return iconsSettings;
		}
	}
}

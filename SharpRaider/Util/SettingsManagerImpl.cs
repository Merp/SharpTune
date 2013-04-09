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
using Com.Sun.Org.Apache.Xerces.Internal.Parsers;
using Javax.Swing;
using Org.W3c.Dom;
using Org.Xml.Sax;
using RomRaider;
using RomRaider.Swing;
using RomRaider.Util;
using RomRaider.Xml;
using RomRaider.Yaml;
using Sharpen;

namespace RomRaider.Util
{
	public sealed class SettingsManagerImpl : SettingsManager
	{
		private static readonly string SETTINGS_FILE = "/.RomRaider/settings.xml";

		private static readonly string HOME = Runtime.GetProperty("user.home");

		private static readonly string SETTINGS_YAML_FILE = HOME + "/.RomRaider/settings.yaml";

		public Settings Load()
		{
			return LoadYaml();
		}

		public void Save(Settings s)
		{
			SaveYaml(s);
		}

		public Settings LoadYaml()
		{
			try
			{
				Org.Yaml.Snakeyaml.Yaml yaml = new Org.Yaml.Snakeyaml.Yaml();
				FileReader fr = new FileReader(SETTINGS_YAML_FILE);
				return (Settings)yaml.Load(fr);
			}
			catch (FileNotFoundException)
			{
				JOptionPane.ShowMessageDialog(null, "Settings file not found.\nUsing default settings."
					, "Error Loading Settings", JOptionPane.INFORMATION_MESSAGE);
				return new Settings();
			}
			catch (Exception e)
			{
				throw new RuntimeException(e);
			}
		}

		public void SaveYaml(Settings settings)
		{
			Org.Yaml.Snakeyaml.Yaml yaml = new Org.Yaml.Snakeyaml.Yaml(new SkipEmptyRepresenterAwtSafe
				());
			try
			{
				new FilePath(HOME + "/.RomRaider").Mkdir();
				FileWriter fw = new FileWriter(SETTINGS_YAML_FILE);
				yaml.Dump(settings, fw);
				fw.Close();
			}
			catch (IOException e)
			{
				// TODO Auto-generated catch block
				Sharpen.Runtime.PrintStackTrace(e);
			}
		}

		public Settings LoadXML()
		{
			try
			{
				InputSource src = new InputSource(new FileInputStream(new FilePath(HOME + SETTINGS_FILE
					)));
				DOMSettingsUnmarshaller domUms = new DOMSettingsUnmarshaller();
				DOMParser parser = new DOMParser();
				parser.Parse(src);
				Document doc = parser.GetDocument();
				return domUms.UnmarshallSettings(doc.GetDocumentElement());
			}
			catch (FileNotFoundException)
			{
				JOptionPane.ShowMessageDialog(null, "Settings file not found.\nUsing default settings."
					, "Error Loading Settings", JOptionPane.INFORMATION_MESSAGE);
				return new Settings();
			}
			catch (Exception e)
			{
				throw new RuntimeException(e);
			}
		}

		public void SaveXML(Settings settings)
		{
			Save(settings, new JProgressPane());
		}

		public void Save(Settings settings, JProgressPane progress)
		{
			DOMSettingsBuilder builder = new DOMSettingsBuilder();
			try
			{
				new FilePath(HOME + "/.RomRaider/").Mkdir();
				// Creates directory if it does not exist
				builder.BuildSettings(settings, new FilePath(HOME + SETTINGS_FILE), progress, Version
					.VERSION);
			}
			catch (Exception e)
			{
				throw new RuntimeException(e);
			}
		}
	}
}

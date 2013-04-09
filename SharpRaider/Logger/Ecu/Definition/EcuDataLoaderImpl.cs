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
using Org.Xml.Sax;
using RomRaider.IO.Connection;
using RomRaider.Logger.Ecu.Comms.Query;
using RomRaider.Logger.Ecu.Definition;
using RomRaider.Logger.Ecu.Definition.Xml;
using RomRaider.Logger.Ecu.Exception;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.Ecu.Definition
{
	public sealed class EcuDataLoaderImpl : EcuDataLoader
	{
		private IDictionary<string, EcuDefinition> ecuDefinitionMap = new Dictionary<string
			, EcuDefinition>();

		private IList<EcuParameter> ecuParameters = new AList<EcuParameter>();

		private IList<EcuSwitch> ecuSwitches = new AList<EcuSwitch>();

		private EcuSwitch fileLoggingControllerSwitch;

		private ConnectionProperties connectionProperties;

		private string defVersion;

		public void LoadEcuDefsFromXml(FilePath ecuDefsFile)
		{
			ParamChecker.CheckNotNull(ecuDefsFile, "ecuDefsFile");
			try
			{
				InputStream inputStream = new BufferedInputStream(new FileInputStream(ecuDefsFile
					));
				try
				{
					EcuDefinitionHandler handler = new EcuDefinitionHandler();
					SaxParserFactory.GetSaxParser().Parse(inputStream, handler);
					ecuDefinitionMap = handler.GetEcuDefinitionMap();
				}
				finally
				{
					inputStream.Close();
				}
			}
			catch (SAXParseException)
			{
				// catch general parsing exception - enough people don't unzip the defs that a better error message is in order
				throw new ConfigurationException("Unable to read ECU definition file " + ecuDefsFile
					 + ".  Please make sure the definition file is correct.  If it is in a ZIP archive, unzip the file and try again."
					);
			}
			catch (FileNotFoundException)
			{
				throw new ConfigurationException("The specified ECU definition file " + ecuDefsFile
					 + " does not exist.");
			}
			catch (System.Exception e)
			{
				throw new ConfigurationException(e);
			}
		}

		public void LoadConfigFromXml(string loggerConfigFilePath, string protocol, string
			 fileLoggingControllerSwitchId, EcuInit ecuInit)
		{
			ParamChecker.CheckNotNullOrEmpty(loggerConfigFilePath, "loggerConfigFilePath");
			ParamChecker.CheckNotNullOrEmpty(protocol, "protocol");
			ParamChecker.CheckNotNullOrEmpty(fileLoggingControllerSwitchId, "fileLoggingControllerSwitchId"
				);
			try
			{
				InputStream inputStream = new BufferedInputStream(new FileInputStream(new FilePath
					(loggerConfigFilePath)));
				try
				{
					LoggerDefinitionHandler handler = new LoggerDefinitionHandler(protocol, fileLoggingControllerSwitchId
						, ecuInit);
					SaxParserFactory.GetSaxParser().Parse(inputStream, handler);
					ecuParameters = handler.GetEcuParameters();
					ecuSwitches = handler.GetEcuSwitches();
					fileLoggingControllerSwitch = handler.GetFileLoggingControllerSwitch();
					connectionProperties = handler.GetConnectionProperties();
					defVersion = handler.GetVersion();
				}
				finally
				{
					inputStream.Close();
				}
			}
			catch (FileNotFoundException)
			{
				throw new ConfigurationException("The specified Logger Config file " + loggerConfigFilePath
					 + " does not exist.");
			}
			catch (SAXParseException)
			{
				// catch general parsing exception - enough people don't unzip the defs that a better error message is in order
				throw new ConfigurationException("Unable to read Logger Config file " + loggerConfigFilePath
					 + ".  Please make sure the configuration file is correct.  If it is in a ZIP archive, unzip the file and try again."
					);
			}
			catch (System.Exception e)
			{
				throw new ConfigurationException(e);
			}
		}

		public IDictionary<string, EcuDefinition> GetEcuDefinitionMap()
		{
			return ecuDefinitionMap;
		}

		public IList<EcuParameter> GetEcuParameters()
		{
			return ecuParameters;
		}

		public IList<EcuSwitch> GetEcuSwitches()
		{
			return ecuSwitches;
		}

		public EcuSwitch GetFileLoggingControllerSwitch()
		{
			return fileLoggingControllerSwitch;
		}

		public ConnectionProperties GetConnectionProperties()
		{
			return connectionProperties;
		}

		public string GetDefVersion()
		{
			return defVersion;
		}
	}
}

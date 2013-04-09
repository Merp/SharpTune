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
using System.IO;
using RomRaider.Logger.Ecu.Definition.Plugin;
using RomRaider.Logger.Ecu.Exception;
using RomRaider.Logger.External.Core;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.External.Core
{
	public sealed class ExternalDataSourceLoaderImpl : ExternalDataSourceLoader
	{
		private static readonly Org.Apache.Log4j.Logger LOGGER = Org.Apache.Log4j.Logger.
			GetLogger(typeof(ExternalDataSourceLoaderImpl));

		private IList<ExternalDataSource> externalDataSources = new AList<ExternalDataSource
			>();

		public void LoadExternalDataSources(IDictionary<string, string> loggerPluginPorts
			)
		{
			try
			{
				FilePath pluginsDir = new FilePath("./plugins");
				if (pluginsDir.Exists() && pluginsDir.IsDirectory())
				{
					//                externalDataSources = new ArrayList<ExternalDataSource>();
					FilePath[] pluginPropertyFiles = pluginsDir.ListFiles(new PluginFilenameFilter());
					foreach (FilePath pluginPropertyFile in pluginPropertyFiles)
					{
						Properties pluginProps = new Properties();
						FileInputStream inputStream = new FileInputStream(pluginPropertyFile);
						try
						{
							pluginProps.Load(inputStream);
							string datasourceClassName = pluginProps.GetProperty("datasource.class");
							if (!ParamChecker.IsNullOrEmpty(datasourceClassName))
							{
								try
								{
									Type dataSourceClass = GetType().GetClassLoader().LoadClass(datasourceClassName);
									if (dataSourceClass != null && typeof(ExternalDataSource).IsAssignableFrom(dataSourceClass
										))
									{
										ExternalDataSource dataSource = DataSource(dataSourceClass, loggerPluginPorts);
										ExternalDataSource managedDataSource = new GenericDataSourceManager(dataSource);
										externalDataSources.AddItem(managedDataSource);
										LOGGER.Info("Plugin loaded: " + dataSource.GetName() + " v" + dataSource.GetVersion
											());
									}
								}
								catch (Exception t)
								{
									LOGGER.Error("Error loading external datasource: " + datasourceClassName + ", specified in: "
										 + pluginPropertyFile.GetAbsolutePath(), t);
								}
							}
						}
						finally
						{
							inputStream.Close();
						}
					}
				}
			}
			catch (Exception e)
			{
				throw new ConfigurationException(e);
			}
		}

		/// <exception cref="System.Exception"></exception>
		private ExternalDataSource DataSource<_T0>(Type<_T0> dataSourceClass, IDictionary
			<string, string> loggerPluginPorts)
		{
			ExternalDataSource dataSource = (ExternalDataSource)System.Activator.CreateInstance
				(dataSourceClass);
			if (loggerPluginPorts != null)
			{
				string port = loggerPluginPorts.Get(dataSource.GetId());
				if (port != null && port.Trim().Length > 0)
				{
					dataSource.SetPort(port);
				}
			}
			return dataSource;
		}

		public IList<ExternalDataSource> GetExternalDataSources()
		{
			return externalDataSources;
		}
	}
}

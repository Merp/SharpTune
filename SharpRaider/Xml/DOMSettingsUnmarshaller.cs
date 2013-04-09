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
using Java.Awt;
using Org.W3c.Dom;
using RomRaider;
using RomRaider.Xml;
using Sharpen;

namespace RomRaider.Xml
{
	public sealed class DOMSettingsUnmarshaller
	{
		public Settings UnmarshallSettings(Node rootNode)
		{
			Settings settings = new Settings();
			Node n;
			NodeList nodes = rootNode.GetChildNodes();
			for (int i = 0; i < nodes.GetLength(); i++)
			{
				n = nodes.Item(i);
				if (n.GetNodeType() == Node.ELEMENT_NODE && Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName
					(), "window"))
				{
					settings = UnmarshallWindow(n, settings);
				}
				else
				{
					if (n.GetNodeType() == Node.ELEMENT_NODE && Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName
						(), "files"))
					{
						settings = UnmarshallFiles(n, settings);
					}
					else
					{
						if (n.GetNodeType() == Node.ELEMENT_NODE && Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName
							(), "options"))
						{
							settings = UnmarshallOptions(n, settings);
						}
						else
						{
							if (n.GetNodeType() == Node.ELEMENT_NODE && Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName
								(), "tabledisplay"))
							{
								settings = UnmarshallTableDisplay(n, settings);
							}
							else
							{
								if (n.GetNodeType() == Node.ELEMENT_NODE && Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName
									(), "logger"))
								{
									settings = UnmarshallLogger(n, settings);
								}
								else
								{
									if (n.GetNodeType() == Node.ELEMENT_NODE && Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName
										(), Settings.TABLE_CLIPBOARD_FORMAT_ELEMENT))
									{
										settings = this.UnmarshallClipboardFormat(n, settings);
									}
									else
									{
										if (n.GetNodeType() == Node.ELEMENT_NODE && Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName
											(), Settings.ICONS_ELEMENT_NAME))
										{
											settings = this.UnmarshallIcons(n, settings);
										}
									}
								}
							}
						}
					}
				}
			}
			return settings;
		}

		private Settings UnmarshallWindow(Node windowNode, Settings settings)
		{
			Node n;
			NodeList nodes = windowNode.GetChildNodes();
			for (int i = 0; i < nodes.GetLength(); i++)
			{
				n = nodes.Item(i);
				if (n.GetNodeType() == Node.ELEMENT_NODE && Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName
					(), "maximized"))
				{
					settings.SetWindowMaximized(DOMHelper.UnmarshallAttribute(n, "value", false));
				}
				else
				{
					if (n.GetNodeType() == Node.ELEMENT_NODE && Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName
						(), "size"))
					{
						settings.SetWindowSize(new Dimension(DOMHelper.UnmarshallAttribute(n, "y", 600), 
							DOMHelper.UnmarshallAttribute(n, "x", 800)));
					}
					else
					{
						if (n.GetNodeType() == Node.ELEMENT_NODE && Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName
							(), "location"))
						{
							// set default location in top left screen if no settings file found
							settings.SetWindowLocation(new Point(DOMHelper.UnmarshallAttribute(n, "x", 0), DOMHelper.UnmarshallAttribute
								(n, "y", 0)));
						}
						else
						{
							if (n.GetNodeType() == Node.ELEMENT_NODE && Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName
								(), "splitpane"))
							{
								settings.SetSplitPaneLocation(DOMHelper.UnmarshallAttribute(n, "location", 150));
							}
						}
					}
				}
			}
			return settings;
		}

		private Settings UnmarshallFiles(Node urlNode, Settings settings)
		{
			Node n;
			NodeList nodes = urlNode.GetChildNodes();
			for (int i = 0; i < nodes.GetLength(); i++)
			{
				n = nodes.Item(i);
				if (n.GetNodeType() == Node.ELEMENT_NODE && Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName
					(), "ecudefinitionfile"))
				{
					settings.AddEcuDefinitionFile(new FilePath(DOMHelper.UnmarshallAttribute(n, "name"
						, "ecu_defs.xml")));
				}
				else
				{
					if (n.GetNodeType() == Node.ELEMENT_NODE && Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName
						(), "image_dir"))
					{
						settings.SetLastImageDir(new FilePath(DOMHelper.UnmarshallAttribute(n, "path", "ecu_defs.xml"
							)));
					}
					else
					{
						if (n.GetNodeType() == Node.ELEMENT_NODE && Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName
							(), Settings.REPOSITORY_ELEMENT_NAME))
						{
							settings.SetLastRepositoryDir(new FilePath(DOMHelper.UnmarshallAttribute(n, Settings
								.REPOSITORY_ATTRIBUTE_NAME, "repositories")));
						}
					}
				}
			}
			return settings;
		}

		private Settings UnmarshallOptions(Node optionNode, Settings settings)
		{
			Node n;
			NodeList nodes = optionNode.GetChildNodes();
			for (int i = 0; i < nodes.GetLength(); i++)
			{
				n = nodes.Item(i);
				if (n.GetNodeType() == Node.ELEMENT_NODE && Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName
					(), "obsoletewarning"))
				{
					settings.SetObsoleteWarning(System.Boolean.Parse(DOMHelper.UnmarshallAttribute(n, 
						"value", "true")));
				}
				else
				{
					if (n.GetNodeType() == Node.ELEMENT_NODE && Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName
						(), "debug"))
					{
						settings.SetDebug(System.Boolean.Parse(DOMHelper.UnmarshallAttribute(n, "value", 
							"true")));
					}
					else
					{
						if (n.GetNodeType() == Node.ELEMENT_NODE && Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName
							(), "calcconflictwarning"))
						{
							settings.SetCalcConflictWarning(System.Boolean.Parse(DOMHelper.UnmarshallAttribute
								(n, "value", "true")));
						}
						else
						{
							if (n.GetNodeType() == Node.ELEMENT_NODE && Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName
								(), "userlevel"))
							{
								settings.SetUserLevel(DOMHelper.UnmarshallAttribute(n, "value", 1));
							}
							else
							{
								if (n.GetNodeType() == Node.ELEMENT_NODE && Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName
									(), "tableclickcount"))
								{
									settings.SetTableClickCount(DOMHelper.UnmarshallAttribute(n, "value", 2));
								}
								else
								{
									if (n.GetNodeType() == Node.ELEMENT_NODE && Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName
										(), "version"))
									{
										settings.SetRecentVersion(DOMHelper.UnmarshallAttribute(n, "value", string.Empty)
											);
									}
									else
									{
										if (n.GetNodeType() == Node.ELEMENT_NODE && Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName
											(), "savedebugtables"))
										{
											settings.SetSaveDebugTables(System.Boolean.Parse(DOMHelper.UnmarshallAttribute(n, 
												"value", "false")));
										}
										else
										{
											if (n.GetNodeType() == Node.ELEMENT_NODE && Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName
												(), "displayhightables"))
											{
												settings.SetDisplayHighTables(System.Boolean.Parse(DOMHelper.UnmarshallAttribute(
													n, "value", "false")));
											}
											else
											{
												if (n.GetNodeType() == Node.ELEMENT_NODE && Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName
													(), "valuelimitwarning"))
												{
													settings.SetValueLimitWarning(System.Boolean.Parse(DOMHelper.UnmarshallAttribute(
														n, "value", "true")));
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return settings;
		}

		private Settings UnmarshallTableDisplay(Node tableDisplayNode, Settings settings)
		{
			Node n;
			NodeList nodes = tableDisplayNode.GetChildNodes();
			for (int i = 0; i < nodes.GetLength(); i++)
			{
				n = nodes.Item(i);
				if (n.GetNodeType() == Node.ELEMENT_NODE && Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName
					(), "font"))
				{
					settings.SetTableFont(new Font(DOMHelper.UnmarshallAttribute(n, "face", "Arial"), 
						DOMHelper.UnmarshallAttribute(n, "decoration", Font.BOLD), DOMHelper.UnmarshallAttribute
						(n, "size", 12)));
				}
				else
				{
					if (n.GetNodeType() == Node.ELEMENT_NODE && Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName
						(), "cellsize"))
					{
						settings.SetCellSize(new Dimension(DOMHelper.UnmarshallAttribute(n, "width", 42), 
							DOMHelper.UnmarshallAttribute(n, "height", 18)));
					}
					else
					{
						if (n.GetNodeType() == Node.ELEMENT_NODE && Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName
							(), "colors"))
						{
							settings = UnmarshallColors(n, settings);
						}
					}
				}
			}
			return settings;
		}

		private Settings UnmarshallColors(Node colorNode, Settings settings)
		{
			Node n;
			NodeList nodes = colorNode.GetChildNodes();
			for (int i = 0; i < nodes.GetLength(); i++)
			{
				n = nodes.Item(i);
				if (n.GetNodeType() == Node.ELEMENT_NODE && Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName
					(), "max"))
				{
					settings.SetMaxColor(UnmarshallColor(n));
				}
				else
				{
					if (n.GetNodeType() == Node.ELEMENT_NODE && Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName
						(), "min"))
					{
						settings.SetMinColor(UnmarshallColor(n));
					}
					else
					{
						if (n.GetNodeType() == Node.ELEMENT_NODE && Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName
							(), "highlight"))
						{
							settings.SetHighlightColor(UnmarshallColor(n));
						}
						else
						{
							if (n.GetNodeType() == Node.ELEMENT_NODE && Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName
								(), "increaseborder"))
							{
								settings.SetIncreaseBorder(UnmarshallColor(n));
							}
							else
							{
								if (n.GetNodeType() == Node.ELEMENT_NODE && Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName
									(), "decreaseborder"))
								{
									settings.SetDecreaseBorder(UnmarshallColor(n));
								}
								else
								{
									if (n.GetNodeType() == Node.ELEMENT_NODE && Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName
										(), "axis"))
									{
										settings.SetAxisColor(UnmarshallColor(n));
									}
									else
									{
										if (n.GetNodeType() == Node.ELEMENT_NODE && Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName
											(), "warning"))
										{
											settings.SetWarningColor(UnmarshallColor(n));
										}
									}
								}
							}
						}
					}
				}
			}
			return settings;
		}

		private Settings UnmarshallLogger(Node loggerNode, Settings settings)
		{
			NodeList nodes = loggerNode.GetChildNodes();
			if (loggerNode.GetNodeType() == Node.ELEMENT_NODE && Sharpen.Runtime.EqualsIgnoreCase
				(loggerNode.GetNodeName(), "logger"))
			{
				settings.SetLocale(DOMHelper.UnmarshallAttribute(loggerNode, "locale", "system"));
			}
			for (int i = 0; i < nodes.GetLength(); i++)
			{
				Node n = nodes.Item(i);
				if (n.GetNodeType() == Node.ELEMENT_NODE && Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName
					(), "serial"))
				{
					settings.SetLoggerPortDefault(DOMHelper.UnmarshallAttribute(n, "port", string.Empty
						));
					settings.SetRefreshMode(DOMHelper.UnmarshallAttribute(n, "refresh", false));
					Settings.SetDestinationId(unchecked((byte)DOMHelper.UnmarshallAttribute(n, "ecuid"
						, unchecked((byte)unchecked((int)(0x10))))));
					settings.SetFastPoll(DOMHelper.UnmarshallAttribute(n, "fastpoll", true));
				}
				else
				{
					if (n.GetNodeType() == Node.ELEMENT_NODE && Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName
						(), "protocol"))
					{
						Settings.SetLoggerProtocol(DOMHelper.UnmarshallAttribute(n, "name", "SSM"));
						Settings.SetTransportProtocol(DOMHelper.UnmarshallAttribute(n, "transport", "ISO9141"
							));
						Settings.SetJ2534Device(DOMHelper.UnmarshallAttribute(n, "library", null));
					}
					else
					{
						if (n.GetNodeType() == Node.ELEMENT_NODE && Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName
							(), "maximized"))
						{
							settings.SetLoggerWindowMaximized(DOMHelper.UnmarshallAttribute(n, "value", false
								));
						}
						else
						{
							if (n.GetNodeType() == Node.ELEMENT_NODE && Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName
								(), "size"))
							{
								settings.SetLoggerWindowSize(new Dimension(DOMHelper.UnmarshallAttribute(n, "y", 
									600), DOMHelper.UnmarshallAttribute(n, "x", 1000)));
								settings.SetLoggerDividerLocation(DOMHelper.UnmarshallAttribute(n, "divider", 500
									));
							}
							else
							{
								if (n.GetNodeType() == Node.ELEMENT_NODE && Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName
									(), "location"))
								{
									settings.SetLoggerWindowLocation(new Point(DOMHelper.UnmarshallAttribute(n, "x", 
										150), DOMHelper.UnmarshallAttribute(n, "y", 150)));
								}
								else
								{
									if (n.GetNodeType() == Node.ELEMENT_NODE && Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName
										(), "tabs"))
									{
										settings.SetLoggerSelectedTabIndex(DOMHelper.UnmarshallAttribute(n, "selected", 0
											));
										settings.SetLoggerParameterListState(DOMHelper.UnmarshallAttribute(n, "showlist", 
											true));
									}
									else
									{
										if (n.GetNodeType() == Node.ELEMENT_NODE && Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName
											(), "definition"))
										{
											settings.SetLoggerDefFilePath(DOMHelper.UnmarshallAttribute(n, "path", settings.GetLoggerDefFilePath
												()));
										}
										else
										{
											if (n.GetNodeType() == Node.ELEMENT_NODE && Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName
												(), "profile"))
											{
												settings.SetLoggerProfileFilePath(DOMHelper.UnmarshallAttribute(n, "path", string.Empty
													));
											}
											else
											{
												if (n.GetNodeType() == Node.ELEMENT_NODE && Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName
													(), "filelogging"))
												{
													settings.SetLoggerOutputDirPath(DOMHelper.UnmarshallAttribute(n, "path", string.Empty
														));
													settings.SetFileLoggingControllerSwitchId(DOMHelper.UnmarshallAttribute(n, "switchid"
														, settings.GetFileLoggingControllerSwitchId()));
													settings.SetFileLoggingControllerSwitchActive(DOMHelper.UnmarshallAttribute(n, "active"
														, true));
													settings.SetFileLoggingAbsoluteTimestamp(DOMHelper.UnmarshallAttribute(n, "absolutetimestamp"
														, false));
												}
												else
												{
													if (n.GetNodeType() == Node.ELEMENT_NODE && Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName
														(), "debug"))
													{
														settings.SetLoggerDebuggingLevel(DOMHelper.UnmarshallAttribute(n, "level", "info"
															));
													}
													else
													{
														if (n.GetNodeType() == Node.ELEMENT_NODE && Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName
															(), "plugins"))
														{
															IDictionary<string, string> pluginPorts = new Dictionary<string, string>();
															NodeList pluginNodes = n.GetChildNodes();
															for (int j = 0; j < pluginNodes.GetLength(); j++)
															{
																Node pluginNode = pluginNodes.Item(j);
																if (pluginNode.GetNodeType() == Node.ELEMENT_NODE && Sharpen.Runtime.EqualsIgnoreCase
																	(pluginNode.GetNodeName(), "plugin"))
																{
																	string id = DOMHelper.UnmarshallAttribute(pluginNode, "id", null);
																	if (id == null || id.Trim().Length == 0)
																	{
																		continue;
																	}
																	string port = DOMHelper.UnmarshallAttribute(pluginNode, "port", null);
																	if (port == null || port.Trim().Length == 0)
																	{
																		continue;
																	}
																	pluginPorts.Put(id.Trim(), port.Trim());
																}
															}
															settings.SetLoggerPluginPorts(pluginPorts);
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return settings;
		}

		private Color UnmarshallColor(Node colorNode)
		{
			return new Color(DOMHelper.UnmarshallAttribute(colorNode, "r", 155), DOMHelper.UnmarshallAttribute
				(colorNode, "g", 155), DOMHelper.UnmarshallAttribute(colorNode, "b", 155));
		}

		private Settings UnmarshallClipboardFormat(Node formatNode, Settings settings)
		{
			string tableClipboardFormat = DOMHelper.UnmarshallAttribute(formatNode, Settings.
				TABLE_CLIPBOARD_FORMAT_ATTRIBUTE, Settings.DEFAULT_CLIPBOARD_FORMAT);
			if (Sharpen.Runtime.EqualsIgnoreCase(tableClipboardFormat, Settings.CUSTOM_CLIPBOARD_FORMAT
				))
			{
				settings.SetTableClipboardFormat(Settings.CUSTOM_CLIPBOARD_FORMAT);
			}
			else
			{
				if (Sharpen.Runtime.EqualsIgnoreCase(tableClipboardFormat, Settings.AIRBOYS_CLIPBOARD_FORMAT
					))
				{
					settings.SetAirboysFormat();
					return settings;
				}
				else
				{
					settings.SetDefaultFormat();
					return settings;
				}
			}
			NodeList tableFormats = formatNode.GetChildNodes();
			for (int i = 0; i < tableFormats.GetLength(); i++)
			{
				Node tableNode = tableFormats.Item(i);
				if (tableNode.GetNodeType() == Node.ELEMENT_NODE)
				{
					if (Sharpen.Runtime.EqualsIgnoreCase(tableNode.GetNodeName(), Settings.TABLE_ELEMENT
						))
					{
						settings.SetTableHeader(DOMHelper.UnmarshallAttribute(tableNode, Settings.TABLE_HEADER_ATTRIBUTE
							, Settings.DEFAULT_TABLE_HEADER));
					}
					else
					{
						if (Sharpen.Runtime.EqualsIgnoreCase(tableNode.GetNodeName(), Settings.TABLE1D_ELEMENT
							))
						{
							settings.SetTable1DHeader(DOMHelper.UnmarshallAttribute(tableNode, Settings.TABLE_HEADER_ATTRIBUTE
								, Settings.DEFAULT_TABLE1D_HEADER));
						}
						else
						{
							if (Sharpen.Runtime.EqualsIgnoreCase(tableNode.GetNodeName(), Settings.TABLE2D_ELEMENT
								))
							{
								settings.SetTable2DHeader(DOMHelper.UnmarshallAttribute(tableNode, Settings.TABLE_HEADER_ATTRIBUTE
									, Settings.DEFAULT_TABLE2D_HEADER));
							}
							else
							{
								if (Sharpen.Runtime.EqualsIgnoreCase(tableNode.GetNodeName(), Settings.TABLE3D_ELEMENT
									))
								{
									settings.SetTable3DHeader(DOMHelper.UnmarshallAttribute(tableNode, Settings.TABLE_HEADER_ATTRIBUTE
										, Settings.DEFAULT_TABLE3D_HEADER));
								}
							}
						}
					}
				}
			}
			return settings;
		}

		private Settings UnmarshallIcons(Node iconsNode, Settings settings)
		{
			NodeList iconScales = iconsNode.GetChildNodes();
			for (int i = 0; i < iconScales.GetLength(); i++)
			{
				Node scaleNode = iconScales.Item(i);
				if (scaleNode.GetNodeType() == Node.ELEMENT_NODE)
				{
					if (Sharpen.Runtime.EqualsIgnoreCase(scaleNode.GetNodeName(), Settings.EDITOR_ICONS_ELEMENT_NAME
						))
					{
						try
						{
							settings.SetEditorIconScale(DOMHelper.UnmarshallAttribute(scaleNode, Settings.EDITOR_ICONS_SCALE_ATTRIBUTE_NAME
								, Settings.DEFAULT_EDITOR_ICON_SCALE));
						}
						catch (FormatException)
						{
							settings.SetEditorIconScale(Settings.DEFAULT_EDITOR_ICON_SCALE);
						}
					}
					else
					{
						if (Sharpen.Runtime.EqualsIgnoreCase(scaleNode.GetNodeName(), Settings.TABLE_ICONS_ELEMENT_NAME
							))
						{
							try
							{
								settings.SetTableIconScale(DOMHelper.UnmarshallAttribute(scaleNode, Settings.TABLE_ICONS_SCALE_ATTRIBUTE_NAME
									, Settings.DEFAULT_TABLE_ICON_SCALE));
							}
							catch (FormatException)
							{
								settings.SetTableIconScale(Settings.DEFAULT_TABLE_ICON_SCALE);
							}
						}
					}
				}
			}
			return settings;
		}
	}
}

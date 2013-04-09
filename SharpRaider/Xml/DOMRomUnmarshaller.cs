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
using Javax.Management.Modelmbean;
using Javax.Swing;
using Org.Apache.Log4j;
using Org.W3c.Dom;
using RomRaider;
using RomRaider.Editor.Ecu;
using RomRaider.Maps;
using RomRaider.Swing;
using RomRaider.Util;
using RomRaider.Xml;
using Sharpen;

namespace RomRaider.Xml
{
	public sealed class DOMRomUnmarshaller
	{
		private static readonly Logger LOGGER = Logger.GetLogger(typeof(RomRaider.Xml.DOMRomUnmarshaller
			));

		private JProgressPane progress = null;

		private readonly IList<Scale> scales = new AList<Scale>();

		private readonly ECUEditor parent;

		public DOMRomUnmarshaller(Settings settings, ECUEditor parent)
		{
			//DOM XML parser for ROMs
			this.parent = parent;
		}

		/// <exception cref="RomRaider.Xml.RomNotFoundException"></exception>
		/// <exception cref="Javax.Management.Modelmbean.XMLParseException"></exception>
		/// <exception cref="Sharpen.StackOverflowError"></exception>
		/// <exception cref="System.Exception"></exception>
		public Rom UnmarshallXMLDefinition(Node rootNode, byte[] input, JProgressPane progress
			)
		{
			this.progress = progress;
			Node n;
			NodeList nodes = rootNode.GetChildNodes();
			// unmarshall scales first
			for (int i = 0; i < nodes.GetLength(); i++)
			{
				n = nodes.Item(i);
				if (n.GetNodeType() == Node.ELEMENT_NODE && Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName
					(), "scalingbase"))
				{
					scales.AddItem(UnmarshallScale(n, new Scale()));
				}
			}
			// now unmarshall roms
			for (int i_1 = 0; i_1 < nodes.GetLength(); i_1++)
			{
				n = nodes.Item(i_1);
				if (n.GetNodeType() == Node.ELEMENT_NODE && Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName
					(), "rom"))
				{
					Node n2;
					NodeList nodes2 = n.GetChildNodes();
					for (int z = 0; z < nodes2.GetLength(); z++)
					{
						n2 = nodes2.Item(z);
						if (n2.GetNodeType() == Node.ELEMENT_NODE && Sharpen.Runtime.EqualsIgnoreCase(n2.
							GetNodeName(), "romid"))
						{
							RomID romID = UnmarshallRomID(n2, new RomID());
							if (romID.GetInternalIdString().Length > 0 && FoundMatch(romID, input))
							{
								Rom output = UnmarshallRom(n, new Rom());
								//set ram offset
								output.GetRomID().SetRamOffset(output.GetRomID().GetFileSize() - input.Length);
								return output;
							}
						}
					}
				}
			}
			throw new RomNotFoundException();
		}

		public static bool FoundMatch(RomID romID, byte[] file)
		{
			string idString = romID.GetInternalIdString();
			// romid is hex string
			if (idString.Length > 2 && Sharpen.Runtime.EqualsIgnoreCase(Sharpen.Runtime.Substring
				(idString, 0, 2), "0x"))
			{
				try
				{
					// put romid in to byte array to check for match
					idString = Sharpen.Runtime.Substring(idString, 2);
					// remove "0x"
					int[] romIDBytes = new int[idString.Length / 2];
					for (int i = 0; i < romIDBytes.Length; i++)
					{
						// check to see if each byte matches
						if ((file[romID.GetInternalIdAddress() + i] & unchecked((int)(0xff))) != System.Convert.ToInt32
							(Sharpen.Runtime.Substring(idString, i * 2, i * 2 + 2), 16))
						{
							return false;
						}
					}
					// if no mismatched bytes found, return true
					return true;
				}
				catch (Exception ex)
				{
					// if any exception is encountered, names do not match
					LOGGER.Warn("Error finding match", ex);
					return false;
				}
			}
			else
			{
				// else romid is NOT hex string
				try
				{
					string ecuID = Sharpen.Runtime.GetStringForBytes(file, romID.GetInternalIdAddress
						(), romID.GetInternalIdString().Length);
					return FoundMatchByString(romID, ecuID);
				}
				catch (Exception)
				{
					// if any exception is encountered, names do not match
					return false;
				}
			}
		}

		public static bool FoundMatchByString(RomID romID, string ecuID)
		{
			try
			{
				if (Sharpen.Runtime.EqualsIgnoreCase(ecuID, romID.GetInternalIdString()))
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			catch (Exception)
			{
				// if any exception is encountered, names do not match
				return false;
			}
		}

		public static void Main(string[] args)
		{
			LogManager.InitDebugLogging();
			RomRaider.Xml.DOMRomUnmarshaller um = new RomRaider.Xml.DOMRomUnmarshaller(new Settings
				(), new ECUEditor());
			um.parent.Dispose();
			RomID romID = new RomID();
			romID.SetInternalIdString("Asdfd");
			byte[] file = Sharpen.Runtime.GetBytesForString("Asdfd");
			LOGGER.Debug(FoundMatch(romID, file));
			file[0] = 1;
			file[1] = 1;
			file[2] = 1;
			file[3] = 1;
			LOGGER.Debug(FoundMatch(romID, file));
			romID.SetInternalIdString("0x010101");
			LOGGER.Debug(FoundMatch(romID, file));
		}

		/// <exception cref="Javax.Management.Modelmbean.XMLParseException"></exception>
		/// <exception cref="RomRaider.Xml.RomNotFoundException"></exception>
		/// <exception cref="Sharpen.StackOverflowError"></exception>
		/// <exception cref="System.Exception"></exception>
		public Rom UnmarshallRom(Node rootNode, Rom rom)
		{
			Node n;
			NodeList nodes = rootNode.GetChildNodes();
			progress.Update("Creating tables...", 15);
			if (!Sharpen.Runtime.EqualsIgnoreCase(DOMHelper.UnmarshallAttribute(rootNode, "base"
				, "none"), "none"))
			{
				rom = GetBaseRom(rootNode.GetParentNode(), DOMHelper.UnmarshallAttribute(rootNode
					, "base", "none"), rom);
				rom.GetRomID().SetObsolete(false);
			}
			for (int i = 0; i < nodes.GetLength(); i++)
			{
				n = nodes.Item(i);
				// update progress
				int currProgress = (int)((double)i / (double)nodes.GetLength() * 40);
				progress.Update("Creating tables...", 10 + currProgress);
				if (n.GetNodeType() == Node.ELEMENT_NODE)
				{
					if (Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName(), "romid"))
					{
						rom.SetRomID(UnmarshallRomID(n, rom.GetRomID()));
					}
					else
					{
						if (Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName(), "table"))
						{
							Table table = null;
							try
							{
								table = rom.GetTable(DOMHelper.UnmarshallAttribute(n, "name", "unknown"));
							}
							catch (TableNotFoundException)
							{
							}
							try
							{
								table = UnmarshallTable(n, table, rom);
								table.SetRom(rom);
								rom.AddTable(table);
							}
							catch (TableIsOmittedException)
							{
								// table is not supported in inherited def (skip)
								if (table != null)
								{
									rom.RemoveTable(table.GetName());
								}
							}
							catch (XMLParseException ex)
							{
								LOGGER.Error("Error unmarshalling rom", ex);
							}
						}
					}
				}
			}
			return rom;
		}

		/// <exception cref="Javax.Management.Modelmbean.XMLParseException"></exception>
		/// <exception cref="RomRaider.Xml.RomNotFoundException"></exception>
		/// <exception cref="Sharpen.StackOverflowError"></exception>
		/// <exception cref="System.Exception"></exception>
		public Rom GetBaseRom(Node rootNode, string xmlID, Rom rom)
		{
			Node n;
			NodeList nodes = rootNode.GetChildNodes();
			for (int i = 0; i < nodes.GetLength(); i++)
			{
				n = nodes.Item(i);
				if (n.GetNodeType() == Node.ELEMENT_NODE && Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName
					(), "rom"))
				{
					Node n2;
					NodeList nodes2 = n.GetChildNodes();
					for (int z = 0; z < nodes2.GetLength(); z++)
					{
						n2 = nodes2.Item(z);
						if (n2.GetNodeType() == Node.ELEMENT_NODE && Sharpen.Runtime.EqualsIgnoreCase(n2.
							GetNodeName(), "romid"))
						{
							RomID romID = UnmarshallRomID(n2, new RomID());
							if (Sharpen.Runtime.EqualsIgnoreCase(romID.GetXmlid(), xmlID))
							{
								Rom returnrom = UnmarshallRom(n, rom);
								returnrom.GetRomID().SetObsolete(false);
								return returnrom;
							}
						}
					}
				}
			}
			throw new RomNotFoundException();
		}

		public RomID UnmarshallRomID(Node romIDNode, RomID romID)
		{
			Node n;
			NodeList nodes = romIDNode.GetChildNodes();
			for (int i = 0; i < nodes.GetLength(); i++)
			{
				n = nodes.Item(i);
				if (n.GetNodeType() == Node.ELEMENT_NODE)
				{
					if (Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName(), "xmlid"))
					{
						romID.SetXmlid(DOMHelper.UnmarshallText(n));
					}
					else
					{
						if (Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName(), "internalidaddress"))
						{
							romID.SetInternalIdAddress(RomAttributeParser.ParseHexString(DOMHelper.UnmarshallText
								(n)));
						}
						else
						{
							if (Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName(), "internalidstring"))
							{
								romID.SetInternalIdString(DOMHelper.UnmarshallText(n));
								if (romID.GetInternalIdString() == null)
								{
									romID.SetInternalIdString(string.Empty);
								}
							}
							else
							{
								if (Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName(), "caseid"))
								{
									romID.SetCaseId(DOMHelper.UnmarshallText(n));
								}
								else
								{
									if (Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName(), "ecuid"))
									{
										romID.SetEcuId(DOMHelper.UnmarshallText(n));
									}
									else
									{
										if (Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName(), "make"))
										{
											romID.SetMake(DOMHelper.UnmarshallText(n));
										}
										else
										{
											if (Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName(), "market"))
											{
												romID.SetMarket(DOMHelper.UnmarshallText(n));
											}
											else
											{
												if (Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName(), "model"))
												{
													romID.SetModel(DOMHelper.UnmarshallText(n));
												}
												else
												{
													if (Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName(), "submodel"))
													{
														romID.SetSubModel(DOMHelper.UnmarshallText(n));
													}
													else
													{
														if (Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName(), "transmission"))
														{
															romID.SetTransmission(DOMHelper.UnmarshallText(n));
														}
														else
														{
															if (Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName(), "year"))
															{
																romID.SetYear(DOMHelper.UnmarshallText(n));
															}
															else
															{
																if (Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName(), "flashmethod"))
																{
																	romID.SetFlashMethod(DOMHelper.UnmarshallText(n));
																}
																else
																{
																	if (Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName(), "memmodel"))
																	{
																		romID.SetMemModel(DOMHelper.UnmarshallText(n));
																	}
																	else
																	{
																		if (Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName(), "filesize"))
																		{
																			romID.SetFileSize(RomAttributeParser.ParseFileSize(DOMHelper.UnmarshallText(n)));
																		}
																		else
																		{
																			if (Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName(), "obsolete"))
																			{
																				romID.SetObsolete(System.Boolean.Parse(DOMHelper.UnmarshallText(n)));
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
							}
						}
					}
				}
			}
			return romID;
		}

		/// <exception cref="Javax.Management.Modelmbean.XMLParseException"></exception>
		/// <exception cref="RomRaider.Xml.TableIsOmittedException"></exception>
		/// <exception cref="System.Exception"></exception>
		private Table UnmarshallTable(Node tableNode, Table table, Rom rom)
		{
			if (Sharpen.Runtime.EqualsIgnoreCase(DOMHelper.UnmarshallAttribute(tableNode, "omit"
				, "false"), "true"))
			{
				// remove table if omitted
				throw new TableIsOmittedException();
			}
			if (!Sharpen.Runtime.EqualsIgnoreCase(DOMHelper.UnmarshallAttribute(tableNode, "base"
				, "none"), "none"))
			{
				// copy base table for inheritance
				try
				{
					table = (Table)ObjectCloner.DeepCopy(rom.GetTable(DOMHelper.UnmarshallAttribute(tableNode
						, "base", "none")));
				}
				catch (TableNotFoundException)
				{
				}
				catch (ArgumentNullException ex)
				{
					JOptionPane.ShowMessageDialog(parent, new DebugPanel(ex, parent.GetSettings().GetSupportURL
						()), "Exception", JOptionPane.ERROR_MESSAGE);
				}
			}
			try
			{
				if (table.GetType() < 1)
				{
				}
			}
			catch (ArgumentNullException)
			{
				// if type is null or less than 0, create new instance (otherwise it is inherited)
				if (Sharpen.Runtime.EqualsIgnoreCase(DOMHelper.UnmarshallAttribute(tableNode, "type"
					, "unknown"), "3D"))
				{
					table = new Table3D(parent);
				}
				else
				{
					if (Sharpen.Runtime.EqualsIgnoreCase(DOMHelper.UnmarshallAttribute(tableNode, "type"
						, "unknown"), "2D"))
					{
						table = new Table2D(parent);
					}
					else
					{
						if (Sharpen.Runtime.EqualsIgnoreCase(DOMHelper.UnmarshallAttribute(tableNode, "type"
							, "unknown"), "1D"))
						{
							table = new Table1D(parent);
						}
						else
						{
							if (Sharpen.Runtime.EqualsIgnoreCase(DOMHelper.UnmarshallAttribute(tableNode, "type"
								, "unknown"), "X Axis") || Sharpen.Runtime.EqualsIgnoreCase(DOMHelper.UnmarshallAttribute
								(tableNode, "type", "unknown"), "Y Axis"))
							{
								table = new Table1D(parent);
							}
							else
							{
								if (Sharpen.Runtime.EqualsIgnoreCase(DOMHelper.UnmarshallAttribute(tableNode, "type"
									, "unknown"), "Static Y Axis") || Sharpen.Runtime.EqualsIgnoreCase(DOMHelper.UnmarshallAttribute
									(tableNode, "type", "unknown"), "Static X Axis"))
								{
									table = new Table1D(parent);
								}
								else
								{
									if (Sharpen.Runtime.EqualsIgnoreCase(DOMHelper.UnmarshallAttribute(tableNode, "type"
										, "unknown"), "Switch"))
									{
										table = new TableSwitch(parent);
									}
									else
									{
										throw new XMLParseException("Error loading table, " + tableNode.GetAttributes().GetNamedItem
											("name"));
									}
								}
							}
						}
					}
				}
			}
			// unmarshall table attributes
			table.SetName(DOMHelper.UnmarshallAttribute(tableNode, "name", table.GetName()));
			table.SetType(RomAttributeParser.ParseTableType(DOMHelper.UnmarshallAttribute(tableNode
				, "type", table.GetType().ToString())));
			if (Sharpen.Runtime.EqualsIgnoreCase(DOMHelper.UnmarshallAttribute(tableNode, "beforeram"
				, "false"), "true"))
			{
				table.SetBeforeRam(true);
			}
			if (Sharpen.Runtime.EqualsIgnoreCase(DOMHelper.UnmarshallAttribute(tableNode, "type"
				, "unknown"), "Static X Axis") || Sharpen.Runtime.EqualsIgnoreCase(DOMHelper.UnmarshallAttribute
				(tableNode, "type", "unknown"), "Static Y Axis"))
			{
				table.SetIsStatic(true);
				((Table1D)table).SetIsAxis(true);
			}
			else
			{
				if (Sharpen.Runtime.EqualsIgnoreCase(DOMHelper.UnmarshallAttribute(tableNode, "type"
					, "unknown"), "X Axis") || Sharpen.Runtime.EqualsIgnoreCase(DOMHelper.UnmarshallAttribute
					(tableNode, "type", "unknown"), "Y Axis"))
				{
					((Table1D)table).SetIsAxis(true);
				}
			}
			table.SetCategory(DOMHelper.UnmarshallAttribute(tableNode, "category", table.GetCategory
				()));
			if (table.GetStorageType() < 1)
			{
				table.SetSignedData(RomAttributeParser.ParseStorageDataSign(DOMHelper.UnmarshallAttribute
					(tableNode, "storagetype", table.GetStorageType().ToString())));
			}
			table.SetStorageType(RomAttributeParser.ParseStorageType(DOMHelper.UnmarshallAttribute
				(tableNode, "storagetype", table.GetStorageType().ToString())));
			table.SetEndian(RomAttributeParser.ParseEndian(DOMHelper.UnmarshallAttribute(tableNode
				, "endian", table.GetEndian().ToString())));
			table.SetStorageAddress(RomAttributeParser.ParseHexString(DOMHelper.UnmarshallAttribute
				(tableNode, "storageaddress", table.GetStorageAddress().ToString())));
			table.SetDescription(DOMHelper.UnmarshallAttribute(tableNode, "description", table
				.GetDescription()));
			table.SetDataSize(DOMHelper.UnmarshallAttribute(tableNode, "sizey", DOMHelper.UnmarshallAttribute
				(tableNode, "sizex", table.GetDataSize())));
			table.SetFlip(DOMHelper.UnmarshallAttribute(tableNode, "flipy", DOMHelper.UnmarshallAttribute
				(tableNode, "flipx", table.GetFlip())));
			table.SetUserLevel(DOMHelper.UnmarshallAttribute(tableNode, "userlevel", table.GetUserLevel
				()));
			table.SetLocked(DOMHelper.UnmarshallAttribute(tableNode, "locked", table.IsLocked
				()));
			table.SetLogParam(DOMHelper.UnmarshallAttribute(tableNode, "logparam", table.GetLogParam
				()));
			if (table.GetType() == Table.TABLE_3D)
			{
				((Table3D)table).SetSwapXY(DOMHelper.UnmarshallAttribute(tableNode, "swapxy", ((Table3D
					)table).GetSwapXY()));
				((Table3D)table).SetFlipX(DOMHelper.UnmarshallAttribute(tableNode, "flipx", ((Table3D
					)table).GetFlipX()));
				((Table3D)table).SetFlipY(DOMHelper.UnmarshallAttribute(tableNode, "flipy", ((Table3D
					)table).GetFlipY()));
				((Table3D)table).SetSizeX(DOMHelper.UnmarshallAttribute(tableNode, "sizex", ((Table3D
					)table).GetSizeX()));
				((Table3D)table).SetSizeY(DOMHelper.UnmarshallAttribute(tableNode, "sizey", ((Table3D
					)table).GetSizeY()));
			}
			Node n;
			NodeList nodes = tableNode.GetChildNodes();
			for (int i = 0; i < nodes.GetLength(); i++)
			{
				n = nodes.Item(i);
				if (n.GetNodeType() == Node.ELEMENT_NODE)
				{
					if (Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName(), "table"))
					{
						if (table.GetType() == Table.TABLE_2D)
						{
							// if table is 2D, parse axis
							if (RomAttributeParser.ParseTableType(DOMHelper.UnmarshallAttribute(n, "type", "unknown"
								)) == Table.TABLE_Y_AXIS || RomAttributeParser.ParseTableType(DOMHelper.UnmarshallAttribute
								(n, "type", "unknown")) == Table.TABLE_X_AXIS)
							{
								Table1D tempTable = (Table1D)UnmarshallTable(n, ((Table2D)table).GetAxis(), rom);
								if (tempTable.GetDataSize() != table.GetDataSize())
								{
									tempTable.SetDataSize(table.GetDataSize());
								}
								tempTable.SetData(((Table2D)table).GetAxis().GetData());
								tempTable.SetAxisParent(table);
								((Table2D)table).SetAxis(tempTable);
							}
						}
						else
						{
							if (table.GetType() == Table.TABLE_3D)
							{
								// if table is 3D, populate axiis
								if (RomAttributeParser.ParseTableType(DOMHelper.UnmarshallAttribute(n, "type", "unknown"
									)) == Table.TABLE_X_AXIS)
								{
									Table1D tempTable = (Table1D)UnmarshallTable(n, ((Table3D)table).GetXAxis(), rom);
									if (tempTable.GetDataSize() != ((Table3D)table).GetSizeX())
									{
										tempTable.SetDataSize(((Table3D)table).GetSizeX());
									}
									tempTable.SetData(((Table3D)table).GetXAxis().GetData());
									tempTable.SetAxisParent(table);
									((Table3D)table).SetXAxis(tempTable);
								}
								else
								{
									if (RomAttributeParser.ParseTableType(DOMHelper.UnmarshallAttribute(n, "type", "unknown"
										)) == Table.TABLE_Y_AXIS)
									{
										Table1D tempTable = (Table1D)UnmarshallTable(n, ((Table3D)table).GetYAxis(), rom);
										if (tempTable.GetDataSize() != ((Table3D)table).GetSizeY())
										{
											tempTable.SetDataSize(((Table3D)table).GetSizeY());
										}
										tempTable.SetData(((Table3D)table).GetYAxis().GetData());
										tempTable.SetAxisParent(table);
										((Table3D)table).SetYAxis(tempTable);
									}
								}
							}
						}
					}
					else
					{
						if (Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName(), "scaling"))
						{
							// check whether scale already exists. if so, modify, else use new instance
							Scale baseScale = new Scale();
							try
							{
								baseScale = table.GetScaleByName(DOMHelper.UnmarshallAttribute(n, "name", "x"));
							}
							catch (Exception)
							{
							}
							table.SetScale(UnmarshallScale(n, baseScale));
						}
						else
						{
							if (Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName(), "data"))
							{
								// parse and add data to table
								DataCell dataCell = new DataCell();
								dataCell.SetDisplayValue(DOMHelper.UnmarshallText(n));
								dataCell.SetTable(table);
								table.AddStaticDataCell(dataCell);
							}
							else
							{
								if (Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName(), "description"))
								{
									table.SetDescription(DOMHelper.UnmarshallText(n));
								}
								else
								{
									if (Sharpen.Runtime.EqualsIgnoreCase(n.GetNodeName(), "state"))
									{
										((TableSwitch)table).SetValues(DOMHelper.UnmarshallAttribute(n, "name", string.Empty
											), DOMHelper.UnmarshallAttribute(n, "data", "0"));
									}
								}
							}
						}
					}
				}
			}
			return table;
		}

		private Scale UnmarshallScale(Node scaleNode, Scale scale)
		{
			// look for base scale first
			string @base = DOMHelper.UnmarshallAttribute(scaleNode, "base", "none");
			if (!Sharpen.Runtime.EqualsIgnoreCase(@base, "none"))
			{
				foreach (Scale scaleItem in scales)
				{
					// check whether name matches base and set scale if so
					if (Sharpen.Runtime.EqualsIgnoreCase(scaleItem.GetName(), @base))
					{
						try
						{
							scale = (Scale)ObjectCloner.DeepCopy(scaleItem);
						}
						catch (Exception ex)
						{
							JOptionPane.ShowMessageDialog(parent, new DebugPanel(ex, parent.GetSettings().GetSupportURL
								()), "Exception", JOptionPane.ERROR_MESSAGE);
						}
					}
				}
			}
			// set remaining attributes
			scale.SetName(DOMHelper.UnmarshallAttribute(scaleNode, "name", scale.GetName()));
			scale.SetUnit(DOMHelper.UnmarshallAttribute(scaleNode, "units", scale.GetUnit()));
			scale.SetExpression(DOMHelper.UnmarshallAttribute(scaleNode, "expression", scale.
				GetExpression()));
			scale.SetByteExpression(DOMHelper.UnmarshallAttribute(scaleNode, "to_byte", scale
				.GetByteExpression()));
			scale.SetFormat(DOMHelper.UnmarshallAttribute(scaleNode, "format", "#"));
			scale.SetMax(DOMHelper.UnmarshallAttribute(scaleNode, "max", 0.0));
			scale.SetMin(DOMHelper.UnmarshallAttribute(scaleNode, "min", 0.0));
			// get coarse increment with new attribute name (coarseincrement), else look for old (increment)
			scale.SetCoarseIncrement(DOMHelper.UnmarshallAttribute(scaleNode, "coarseincrement"
				, DOMHelper.UnmarshallAttribute(scaleNode, "increment", scale.GetCoarseIncrement
				())));
			scale.SetFineIncrement(DOMHelper.UnmarshallAttribute(scaleNode, "fineincrement", 
				scale.GetFineIncrement()));
			return scale;
		}
	}
}

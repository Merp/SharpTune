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
using Org.Xml.Sax;
using Org.Xml.Sax.Helpers;
using RomRaider.Logger.Ecu.Definition;
using RomRaider.Logger.Ecu.Definition.Xml;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.Ecu.Definition.Xml
{
	public sealed class EcuDefinitionHandler : DefaultHandler
	{
		private static readonly Org.Apache.Log4j.Logger LOGGER = Org.Apache.Log4j.Logger.
			GetLogger(typeof(EcuDefinitionHandler));

		private static readonly string TAG_ROMID = "romid";

		private static readonly string TAG_CALID = "internalidstring";

		private static readonly string TAG_ECUID = "ecuid";

		private static readonly string TAG_CASEID = "caseid";

		private static readonly string TAG_ADDRESS = "internalidaddress";

		private static readonly string TAG_YEAR = "year";

		private static readonly string TAG_MARKET = "market";

		private static readonly string TAG_MAKE = "make";

		private static readonly string TAG_MODEL = "model";

		private static readonly string TAG_SUBMODEL = "submodel";

		private static readonly string TAG_TRANS = "transmission";

		private static readonly string TAG_MEMMODEL = "memmodel";

		private static readonly string TAG_FLASH = "flashmethod";

		private static readonly string TAG_SIZE = "filesize";

		private static readonly string TAG_OBSOLETE = "obsolete";

		private IDictionary<string, EcuDefinition> ecuDefinitionMap = new Dictionary<string
			, EcuDefinition>();

		private string calId;

		private string ecuId;

		private string caseId;

		private string address;

		private string year;

		private string market;

		private string make;

		private string model;

		private string submodel;

		private string transmission;

		private string memmodel;

		private string flashmethod;

		private string filesize;

		private string obsolete;

		private string inherit;

		private string carString;

		private StringBuilder charBuffer;

		public override void StartDocument()
		{
			ecuDefinitionMap = new Dictionary<string, EcuDefinition>();
		}

		public override void StartElement(string uri, string localName, string qName, Attributes
			 attributes)
		{
			if (TAG_ROMID.Equals(qName))
			{
				calId = string.Empty;
				ecuId = string.Empty;
				caseId = string.Empty;
				address = string.Empty;
				year = string.Empty;
				market = string.Empty;
				make = string.Empty;
				model = string.Empty;
				submodel = string.Empty;
				transmission = string.Empty;
				memmodel = string.Empty;
				flashmethod = string.Empty;
				filesize = string.Empty;
				obsolete = "0";
				inherit = string.Empty;
				carString = string.Empty;
			}
			charBuffer = new StringBuilder();
		}

		public override void Characters(char[] ch, int start, int length)
		{
			charBuffer.Append(ch, start, length);
		}

		public override void EndElement(string uri, string localName, string qName)
		{
			if (TAG_ROMID.Equals(qName))
			{
				if (!ParamChecker.IsNullOrEmpty(ecuId) && !ParamChecker.IsNullOrEmpty(calId) && !
					ParamChecker.IsNullOrEmpty(year) && !ParamChecker.IsNullOrEmpty(market) && !ParamChecker.IsNullOrEmpty
					(make) && !ParamChecker.IsNullOrEmpty(model) && !ParamChecker.IsNullOrEmpty(submodel
					) && !ParamChecker.IsNullOrEmpty(transmission))
				{
					carString = string.Format("%s %s %s %s %s %s", year, market, make, model, submodel
						, transmission);
					ecuDefinitionMap.Put(ecuId, new EcuDefinitionImpl(ecuId, calId, carString));
				}
				if (!ParamChecker.IsNullOrEmpty(ecuId) && !ParamChecker.IsNullOrEmpty(calId) && !
					ParamChecker.IsNullOrEmpty(address) && !ParamChecker.IsNullOrEmpty(year) && !ParamChecker.IsNullOrEmpty
					(market) && !ParamChecker.IsNullOrEmpty(make) && !ParamChecker.IsNullOrEmpty(model
					) && !ParamChecker.IsNullOrEmpty(submodel) && !ParamChecker.IsNullOrEmpty(transmission
					) && !ParamChecker.IsNullOrEmpty(memmodel) && !ParamChecker.IsNullOrEmpty(flashmethod
					) && !ParamChecker.IsNullOrEmpty(obsolete))
				{
					LOGGER.Debug(RomDetail());
				}
			}
			else
			{
				if (TAG_CALID.Equals(qName))
				{
					calId = charBuffer.ToString();
				}
				else
				{
					if (TAG_ECUID.Equals(qName))
					{
						ecuId = charBuffer.ToString();
					}
					else
					{
						if (TAG_CASEID.Equals(qName))
						{
							caseId = charBuffer.ToString();
						}
						else
						{
							if (TAG_ADDRESS.Equals(qName))
							{
								address = charBuffer.ToString();
							}
							else
							{
								if (TAG_YEAR.Equals(qName))
								{
									year = charBuffer.ToString();
									if (!year.IsEmpty())
									{
										try
										{
											if (System.Convert.ToInt32(year) < 90)
											{
												year = "20" + year;
											}
										}
										catch (FormatException)
										{
											if ((year.Contains("/") || year.Contains("-")) && year.Length < 6)
											{
												year = "20" + year;
											}
										}
									}
									else
									{
										year = "20xx";
									}
								}
								else
								{
									if (TAG_MARKET.Equals(qName))
									{
										market = charBuffer.ToString();
									}
									else
									{
										if (TAG_MAKE.Equals(qName))
										{
											make = charBuffer.ToString();
										}
										else
										{
											if (TAG_MODEL.Equals(qName))
											{
												model = charBuffer.ToString();
											}
											else
											{
												if (TAG_SUBMODEL.Equals(qName))
												{
													submodel = charBuffer.ToString();
												}
												else
												{
													if (TAG_TRANS.Equals(qName))
													{
														transmission = charBuffer.ToString();
													}
													else
													{
														if (TAG_MEMMODEL.Equals(qName))
														{
															memmodel = charBuffer.ToString();
														}
														else
														{
															if (TAG_FLASH.Equals(qName))
															{
																flashmethod = charBuffer.ToString();
															}
															else
															{
																if (TAG_SIZE.Equals(qName))
																{
																	filesize = charBuffer.ToString();
																}
																else
																{
																	if (TAG_OBSOLETE.Equals(qName))
																	{
																		obsolete = charBuffer.ToString();
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

		public IDictionary<string, EcuDefinition> GetEcuDefinitionMap()
		{
			return ecuDefinitionMap;
		}

		public string RomDetail()
		{
			return string.Format("calid='%s'," + "address='%s'," + "string='%s'," + "caseid='%s',"
				 + "year='%s'," + "market='%s'," + "make='%s'," + "model='%s'," + "submodel='%s',"
				 + "transmission='%s'," + "memmodel='%s'," + "flash='%s'," + "filesize='%s'," + 
				"obsolete='%s'," + "inherit='%s'", calId, address, calId, caseId, year, market, 
				make, model, submodel, transmission, memmodel, flashmethod, filesize, obsolete, 
				inherit);
		}
	}
}

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
using System.Text;
using Org.Xml.Sax;
using Org.Xml.Sax.Helpers;
using RomRaider.IO.Connection;
using RomRaider.Logger.Ecu.Comms.Query;
using RomRaider.Logger.Ecu.Definition;
using RomRaider.Logger.Ecu.Definition.Xml;
using RomRaider.Logger.Ecu.UI.Handler.Dash;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.Ecu.Definition.Xml
{
	public sealed class LoggerDefinitionHandler : DefaultHandler
	{
		private static readonly string FLOAT = "float";

		private static readonly string TAG_LOGGER = "logger";

		private static readonly string TAG_PROTOCOL = "protocol";

		private static readonly string TAG_PARAMETER = "parameter";

		private static readonly string TAG_ADDRESS = "address";

		private static readonly string TAG_DEPENDS = "depends";

		private static readonly string TAG_CONVERSION = "conversion";

		private static readonly string TAG_REPLACE = "replace";

		private static readonly string TAG_REF = "ref";

		private static readonly string TAG_SWITCH = "switch";

		private static readonly string TAG_ECUPARAM = "ecuparam";

		private static readonly string TAG_ECU = "ecu";

		private static readonly string ATTR_VERSION = "version";

		private static readonly string ATTR_ID = "id";

		private static readonly string ATTR_NAME = "name";

		private static readonly string ATTR_DESC = "desc";

		private static readonly string ATTR_ECUBYTEINDEX = "ecubyteindex";

		private static readonly string ATTR_LENGTH = "length";

		private static readonly string ATTR_ECUBIT = "ecubit";

		private static readonly string ATTR_UNITS = "units";

		private static readonly string ATTR_EXPRESSION = "expr";

		private static readonly string ATTR_FORMAT = "format";

		private static readonly string ATTR_BYTE = "byte";

		private static readonly string ATTR_BIT = "bit";

		private static readonly string ATTR_PARAMETER = "parameter";

		private static readonly string ATTR_STORAGETYPE = "storagetype";

		private static readonly string ATTR_BAUD = "baud";

		private static readonly string ATTR_DATABITS = "databits";

		private static readonly string ATTR_STOPBITS = "stopbits";

		private static readonly string ATTR_PARITY = "parity";

		private static readonly string ATTR_CONNECT_TIMEOUT = "connect_timeout";

		private static readonly string ATTR_SEND_TIMEOUT = "send_timeout";

		private static readonly string ATTR_VALUE = "value";

		private static readonly string ATTR_WITH = "with";

		private static readonly string ATTR_GAUGE_MIN = "gauge_min";

		private static readonly string ATTR_GAUGE_MAX = "gauge_max";

		private static readonly string ATTR_GAUGE_STEP = "gauge_step";

		private static readonly string ATTR_TARGET = "target";

		private readonly string protocol;

		private readonly string fileLoggingControllerSwitchId;

		private readonly EcuInit ecuInit;

		private IList<EcuParameter> @params = new AList<EcuParameter>();

		private IList<EcuSwitch> switches = new AList<EcuSwitch>();

		private EcuSwitch fileLoggingControllerSwitch;

		private ConnectionProperties connectionProperties;

		private IDictionary<string, EcuData> ecuDataMap;

		private IDictionary<string, string> replaceMap;

		private string id;

		private string name;

		private string desc;

		private string ecuByteIndex;

		private string ecuBit;

		private string ecuIds;

		private EcuAddress address;

		private ICollection<string> dependsList;

		private IDictionary<string, EcuAddress> ecuAddressMap;

		private bool derived;

		private int addressBit;

		private int addressLength;

		private ICollection<EcuDataConvertor> convertorList;

		private ICollection<EcuDerivedParameterConvertor> derivedConvertorList;

		private StringBuilder charBuffer;

		private bool parseProtocol;

		private string conversionUnits;

		private string conversionExpression;

		private string conversionFormat;

		private string conversionStorageType;

		private GaugeMinMax conversionGauge;

		private string target;

		private string version;

		public LoggerDefinitionHandler(string protocol, string fileLoggingControllerSwitchId
			, EcuInit ecuInit)
		{
			ParamChecker.CheckNotNullOrEmpty(protocol, "protocol");
			ParamChecker.CheckNotNullOrEmpty(fileLoggingControllerSwitchId, "fileLoggingControllerSwitchId"
				);
			this.protocol = protocol;
			this.fileLoggingControllerSwitchId = fileLoggingControllerSwitchId;
			this.ecuInit = ecuInit;
		}

		public override void StartDocument()
		{
			@params = new AList<EcuParameter>();
			switches = new AList<EcuSwitch>();
			ecuDataMap = new Dictionary<string, EcuData>();
		}

		public override void StartElement(string uri, string localName, string qName, Attributes
			 attributes)
		{
			if (TAG_LOGGER.Equals(qName))
			{
				version = attributes.GetValue(ATTR_VERSION);
			}
			else
			{
				if (TAG_PROTOCOL.Equals(qName))
				{
					parseProtocol = Sharpen.Runtime.EqualsIgnoreCase(protocol, attributes.GetValue(ATTR_ID
						));
					if (parseProtocol)
					{
						connectionProperties = new ConnectionPropertiesImpl(System.Convert.ToInt32(attributes
							.GetValue(ATTR_BAUD)), System.Convert.ToInt32(attributes.GetValue(ATTR_DATABITS)
							), System.Convert.ToInt32(attributes.GetValue(ATTR_STOPBITS)), System.Convert.ToInt32
							(attributes.GetValue(ATTR_PARITY)), System.Convert.ToInt32(attributes.GetValue(ATTR_CONNECT_TIMEOUT
							)), System.Convert.ToInt32(attributes.GetValue(ATTR_SEND_TIMEOUT)));
					}
				}
				else
				{
					if (parseProtocol)
					{
						if (TAG_PARAMETER.Equals(qName))
						{
							id = attributes.GetValue(ATTR_ID);
							name = attributes.GetValue(ATTR_NAME);
							desc = attributes.GetValue(ATTR_DESC);
							ecuByteIndex = attributes.GetValue(ATTR_ECUBYTEINDEX);
							ecuBit = attributes.GetValue(ATTR_ECUBIT);
							target = attributes.GetValue(ATTR_TARGET);
							ResetConvertorLists();
						}
						else
						{
							if (TAG_ADDRESS.Equals(qName))
							{
								string length = attributes.GetValue(ATTR_LENGTH);
								addressLength = length == null ? 1 : Sharpen.Extensions.ValueOf(length);
								string bit = attributes.GetValue(ATTR_BIT);
								addressBit = bit == null ? -1 : Sharpen.Extensions.ValueOf(bit);
								derived = false;
							}
							else
							{
								if (TAG_DEPENDS.Equals(qName))
								{
									dependsList = new LinkedHashSet<string>();
									derived = true;
								}
								else
								{
									if (TAG_REF.Equals(qName))
									{
										dependsList.AddItem(attributes.GetValue(ATTR_PARAMETER));
									}
									else
									{
										if (TAG_CONVERSION.Equals(qName))
										{
											conversionUnits = attributes.GetValue(ATTR_UNITS);
											conversionExpression = attributes.GetValue(ATTR_EXPRESSION);
											conversionFormat = attributes.GetValue(ATTR_FORMAT);
											conversionStorageType = attributes.GetValue(ATTR_STORAGETYPE);
											double gaugeMin = GetConversionMin(attributes, conversionUnits);
											double gaugeMax = GetConversionMax(attributes, conversionUnits);
											double gaugeStep = GetConversionStep(attributes, conversionUnits);
											conversionGauge = new GaugeMinMax(gaugeMin, gaugeMax, gaugeStep);
											replaceMap = new Dictionary<string, string>();
										}
										else
										{
											if (TAG_REPLACE.Equals(qName))
											{
												replaceMap.Put(attributes.GetValue(ATTR_VALUE), attributes.GetValue(ATTR_WITH));
											}
											else
											{
												if (TAG_SWITCH.Equals(qName))
												{
													id = attributes.GetValue(ATTR_ID);
													name = attributes.GetValue(ATTR_NAME);
													desc = attributes.GetValue(ATTR_DESC);
													ecuByteIndex = attributes.GetValue(ATTR_ECUBYTEINDEX);
													ecuBit = attributes.GetValue(ATTR_BIT);
													target = attributes.GetValue(ATTR_TARGET);
													address = new EcuAddressImpl(attributes.GetValue(ATTR_BYTE), 1, Sharpen.Extensions.ValueOf
														(attributes.GetValue(ATTR_BIT)));
													ResetConvertorLists();
												}
												else
												{
													if (TAG_ECUPARAM.Equals(qName))
													{
														id = attributes.GetValue(ATTR_ID);
														name = attributes.GetValue(ATTR_NAME);
														desc = attributes.GetValue(ATTR_DESC);
														target = attributes.GetValue(ATTR_TARGET);
														ResetConvertorLists();
														ecuAddressMap = new Dictionary<string, EcuAddress>();
														derived = false;
													}
													else
													{
														if (TAG_ECU.Equals(qName))
														{
															ecuIds = attributes.GetValue(ATTR_ID);
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
			charBuffer = new StringBuilder();
		}

		public override void Characters(char[] ch, int start, int length)
		{
			if (parseProtocol)
			{
				charBuffer.Append(ch, start, length);
			}
		}

		public override void EndElement(string uri, string localName, string qName)
		{
			if (TAG_PROTOCOL.Equals(qName))
			{
				parseProtocol = false;
			}
			else
			{
				if (parseProtocol)
				{
					if (TAG_ADDRESS.Equals(qName))
					{
						address = new EcuAddressImpl(charBuffer.ToString(), addressLength, addressBit);
					}
					else
					{
						if (TAG_PARAMETER.Equals(qName))
						{
							if (derived)
							{
								ICollection<EcuData> dependencies = new HashSet<EcuData>();
								foreach (string refid in dependsList)
								{
									if (ecuDataMap.ContainsKey(refid))
									{
										dependencies.AddItem(ecuDataMap.Get(refid));
									}
								}
								if (dependsList.Count == dependencies.Count)
								{
									EcuParameter param = new EcuDerivedParameterImpl(id, name, desc, Sharpen.Collections.ToArray
										(dependencies, new EcuData[dependencies.Count]), Sharpen.Collections.ToArray(derivedConvertorList
										, new EcuDerivedParameterConvertor[derivedConvertorList.Count]));
									@params.AddItem(param);
									ecuDataMap.Put(param.GetId(), param);
								}
							}
							else
							{
								if (ecuByteIndex == null || ecuBit == null || ecuInit == null || IsSupportedParameter
									(ecuInit, ecuByteIndex, ecuBit))
								{
									if (convertorList.IsEmpty())
									{
										convertorList.AddItem(new EcuParameterConvertorImpl());
									}
									EcuParameter param = new EcuParameterImpl(id, name, desc, address, Sharpen.Collections.ToArray
										(convertorList, new EcuDataConvertor[convertorList.Count]));
									@params.AddItem(param);
									ecuDataMap.Put(param.GetId(), param);
								}
							}
						}
						else
						{
							if (TAG_CONVERSION.Equals(qName))
							{
								if (derived)
								{
									derivedConvertorList.AddItem(new EcuDerivedParameterConvertorImpl(conversionUnits
										, conversionExpression, conversionFormat, replaceMap, conversionGauge));
								}
								else
								{
									convertorList.AddItem(new EcuParameterConvertorImpl(conversionUnits, conversionExpression
										, conversionFormat, address.GetBit(), Sharpen.Runtime.EqualsIgnoreCase(FLOAT, conversionStorageType
										), replaceMap, conversionGauge));
								}
							}
							else
							{
								if (TAG_ECUPARAM.Equals(qName))
								{
									if (ecuInit != null && ecuAddressMap.ContainsKey(ecuInit.GetEcuId()))
									{
										if (convertorList.IsEmpty())
										{
											convertorList.AddItem(new EcuParameterConvertorImpl());
										}
										EcuParameter param = new EcuParameterImpl(id, name, desc, ecuAddressMap.Get(ecuInit
											.GetEcuId()), Sharpen.Collections.ToArray(convertorList, new EcuDataConvertor[convertorList
											.Count]));
										@params.AddItem(param);
										ecuDataMap.Put(param.GetId(), param);
									}
								}
								else
								{
									if (TAG_ECU.Equals(qName))
									{
										foreach (string ecuId in ecuIds.Split(","))
										{
											ecuAddressMap.Put(ecuId, address);
										}
									}
									else
									{
										if (TAG_SWITCH.Equals(qName))
										{
											if (ecuByteIndex == null || ecuBit == null || ecuInit == null || IsSupportedParameter
												(ecuInit, ecuByteIndex, ecuBit))
											{
												EcuDataConvertor[] convertors = new EcuDataConvertor[] { new EcuSwitchConvertorImpl
													(address.GetBit()) };
												EcuSwitch ecuSwitch = new EcuSwitchImpl(id, name, desc, address, convertors);
												switches.AddItem(ecuSwitch);
												ecuDataMap.Put(ecuSwitch.GetId(), ecuSwitch);
												if (Sharpen.Runtime.EqualsIgnoreCase(id, fileLoggingControllerSwitchId))
												{
													fileLoggingControllerSwitch = new EcuSwitchImpl(id, name, desc, address, convertors
														);
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

		public IList<EcuParameter> GetEcuParameters()
		{
			return @params;
		}

		public IList<EcuSwitch> GetEcuSwitches()
		{
			return switches;
		}

		public EcuSwitch GetFileLoggingControllerSwitch()
		{
			return fileLoggingControllerSwitch;
		}

		public ConnectionProperties GetConnectionProperties()
		{
			return connectionProperties;
		}

		public string GetVersion()
		{
			return version;
		}

		private void ResetConvertorLists()
		{
			convertorList = new LinkedHashSet<EcuDataConvertor>();
			derivedConvertorList = new LinkedHashSet<EcuDerivedParameterConvertor>();
		}

		private bool IsSupportedParameter(EcuInit ecuInit, string ecuByteIndex, string ecuBit
			)
		{
			byte[] ecuInitBytes = ecuInit.GetEcuInitBytes();
			int index = System.Convert.ToInt32(ecuByteIndex);
			if (index < ecuInitBytes.Length)
			{
				byte[] bytes = new byte[1];
				System.Array.Copy(ecuInitBytes, index, bytes, 0, 1);
				return (bytes[0] & 1 << System.Convert.ToInt32(ecuBit)) > 0;
			}
			else
			{
				return false;
			}
		}

		private double GetConversionMin(Attributes attributes, string units)
		{
			string value = attributes.GetValue(ATTR_GAUGE_MIN);
			if (!ParamChecker.IsNullOrEmpty(value))
			{
				return double.ParseDouble(value);
			}
			return ConverterMaxMinDefaults.GetMin(units);
		}

		private double GetConversionMax(Attributes attributes, string units)
		{
			string value = attributes.GetValue(ATTR_GAUGE_MAX);
			if (!ParamChecker.IsNullOrEmpty(value))
			{
				return double.ParseDouble(value);
			}
			return ConverterMaxMinDefaults.GetMax(units);
		}

		private double GetConversionStep(Attributes attributes, string units)
		{
			string value = attributes.GetValue(ATTR_GAUGE_STEP);
			if (!ParamChecker.IsNullOrEmpty(value))
			{
				return double.ParseDouble(value);
			}
			return ConverterMaxMinDefaults.GetStep(units);
		}
	}
}

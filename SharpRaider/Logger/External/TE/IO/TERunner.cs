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
using RomRaider.Logger.External.Core;
using RomRaider.Logger.External.TE.IO;
using RomRaider.Logger.External.TE.Plugin;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.External.TE.IO
{
	public sealed class TERunner : Stoppable
	{
		private static readonly Org.Apache.Log4j.Logger LOGGER = Org.Apache.Log4j.Logger.GetLogger
			(typeof(RomRaider.Logger.External.TE.IO.TERunner));

		private readonly IDictionary<ExternalSensorType, TEDataItem> dataItems;

		private readonly TEConnection connection;

		private bool stop;

		private byte byteSum;

		private int sequenceNo;

		private int lastSequenceNo = -1;

		public TERunner(string port, IDictionary<ExternalSensorType, TEDataItem> dataItems
			)
		{
			this.connection = new TEConnectionImpl(port);
			this.dataItems = dataItems;
		}

		public void Run()
		{
			try
			{
				bool packetStarted = false;
				IList<byte> buffer = new AList<byte>(28);
				while (!stop)
				{
					byte b = connection.ReadByte();
					if (b == (unchecked((byte)unchecked((int)(0xa5)))) && buffer.Count >= 1 && buffer
						[buffer.Count - 1] == (unchecked((byte)unchecked((int)(0x5a)))))
					{
						packetStarted = true;
						buffer.Clear();
						buffer.AddItem(unchecked((byte)unchecked((int)(0x5a))));
						buffer.AddItem(b);
					}
					else
					{
						if (packetStarted && buffer.Count <= 28)
						{
							buffer.AddItem(b);
							switch (buffer.Count)
							{
								case 3:
								{
									sequenceNo = ByteUtil.AsUnsignedInt(buffer[2]);
									break;
								}

								case 27:
								{
									byteSum = 0;
									foreach (byte b1 in buffer)
									{
										byteSum = unchecked((byte)(byteSum + b1));
									}
									byteSum = unchecked((byte)~byteSum);
									// 1's complement of sum
									break;
								}

								case 28:
								{
									LOGGER.Trace("Tech Edge (data 2.0): LastSeq:" + lastSequenceNo + " seq:" + sequenceNo
										 + " data:" + buffer);
									if (byteSum != b)
									{
										LOGGER.Error("Tech Edge (data 2.0): CheckSum Failed, calculated:" + byteSum + ", received:"
											 + b);
									}
									if (lastSequenceNo == -1)
									{
										lastSequenceNo = sequenceNo;
									}
									else
									{
										if (lastSequenceNo == unchecked((int)(0xff)))
										{
											if (sequenceNo != unchecked((int)(0x00)))
											{
												LOGGER.Error("Tech Edge (data 2.0): Packet Drop: expected sequence number:0" + ", received:"
													 + sequenceNo);
												lastSequenceNo = sequenceNo;
											}
											else
											{
												lastSequenceNo = sequenceNo;
											}
										}
										else
										{
											if ((lastSequenceNo + 1) != sequenceNo)
											{
												LOGGER.Error("Tech Edge (data 2.0): Packet Drop: expected sequence number:" + (lastSequenceNo
													 + 1) + ", received:" + sequenceNo);
												lastSequenceNo = sequenceNo;
											}
											else
											{
												TEDataItem dataItem = dataItems.Get(ExternalSensorType.WIDEBAND);
												if (dataItem != null)
												{
													int raw1 = ByteUtil.AsUnsignedInt(buffer[5]);
													int raw2 = ByteUtil.AsUnsignedInt(buffer[6]);
													dataItem.SetRaw(raw1, raw2);
												}
												dataItem = dataItems.Get(ExternalSensorType.USER1);
												if (dataItem != null)
												{
													int raw1 = ByteUtil.AsUnsignedInt(buffer[9]);
													int raw2 = ByteUtil.AsUnsignedInt(buffer[10]);
													dataItem.SetRaw(raw1, raw2);
												}
												dataItem = dataItems.Get(ExternalSensorType.USER2);
												if (dataItem != null)
												{
													int raw1 = ByteUtil.AsUnsignedInt(buffer[11]);
													int raw2 = ByteUtil.AsUnsignedInt(buffer[12]);
													dataItem.SetRaw(raw1, raw2);
												}
												dataItem = dataItems.Get(ExternalSensorType.USER3);
												if (dataItem != null)
												{
													int raw1 = ByteUtil.AsUnsignedInt(buffer[13]);
													int raw2 = ByteUtil.AsUnsignedInt(buffer[14]);
													dataItem.SetRaw(raw1, raw2);
												}
												dataItem = dataItems.Get(ExternalSensorType.THERMACOUPLE1);
												if (dataItem != null)
												{
													int raw1 = ByteUtil.AsUnsignedInt(buffer[15]);
													int raw2 = ByteUtil.AsUnsignedInt(buffer[16]);
													dataItem.SetRaw(raw1, raw2);
												}
												dataItem = dataItems.Get(ExternalSensorType.THERMACOUPLE2);
												if (dataItem != null)
												{
													int raw1 = ByteUtil.AsUnsignedInt(buffer[17]);
													int raw2 = ByteUtil.AsUnsignedInt(buffer[18]);
													dataItem.SetRaw(raw1, raw2);
												}
												dataItem = dataItems.Get(ExternalSensorType.THERMACOUPLE3);
												if (dataItem != null)
												{
													int raw1 = ByteUtil.AsUnsignedInt(buffer[19]);
													int raw2 = ByteUtil.AsUnsignedInt(buffer[20]);
													dataItem.SetRaw(raw1, raw2);
												}
												dataItem = dataItems.Get(ExternalSensorType.TorVss);
												if (dataItem != null)
												{
													int raw1 = ByteUtil.AsUnsignedInt(buffer[21]);
													int raw2 = ByteUtil.AsUnsignedInt(buffer[22]);
													dataItem.SetRaw(raw1, raw2);
												}
												dataItem = dataItems.Get(ExternalSensorType.ENGINE_SPEED);
												if (dataItem != null)
												{
													int raw1 = ByteUtil.AsUnsignedInt(buffer[23]);
													int raw2 = ByteUtil.AsUnsignedInt(buffer[24]);
													dataItem.SetRaw(raw1, raw2);
												}
												lastSequenceNo++;
											}
										}
									}
									buffer.Clear();
									packetStarted = false;
									break;
								}
							}
						}
						else
						{
							buffer.AddItem(b);
							packetStarted = false;
						}
					}
				}
				connection.Close();
			}
			catch (Exception t)
			{
				LOGGER.Error("Error occurred", t);
			}
			finally
			{
				connection.Close();
			}
		}

		public void Stop()
		{
			stop = true;
		}
	}
}

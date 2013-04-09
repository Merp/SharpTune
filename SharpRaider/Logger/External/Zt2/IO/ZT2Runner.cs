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
using RomRaider.Logger.External.Zt2.IO;
using RomRaider.Logger.External.Zt2.Plugin;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.External.Zt2.IO
{
	public sealed class ZT2Runner : Stoppable
	{
		private static readonly Org.Apache.Log4j.Logger LOGGER = Org.Apache.Log4j.Logger.GetLogger
			(typeof(RomRaider.Logger.External.Zt2.IO.ZT2Runner));

		private readonly IDictionary<ExternalSensorType, ZT2DataItem> dataItems;

		private readonly ZT2Connection connection;

		private bool stop;

		public ZT2Runner(string port, IDictionary<ExternalSensorType, ZT2DataItem> dataItems
			)
		{
			this.connection = new ZT2ConnectionImpl(port);
			this.dataItems = dataItems;
		}

		public void Run()
		{
			try
			{
				bool packetStarted = false;
				IList<byte> buffer = new AList<byte>(14);
				while (!stop)
				{
					byte b = connection.ReadByte();
					if (b == unchecked((int)(0x02)) && buffer.Count >= 2 && buffer[buffer.Count - 1] 
						== unchecked((int)(0x01)) && buffer[buffer.Count - 2] == unchecked((int)(0x00)))
					{
						packetStarted = true;
						buffer.Clear();
						buffer.AddItem(unchecked((byte)unchecked((int)(0x00))));
						buffer.AddItem(unchecked((byte)unchecked((int)(0x01))));
						buffer.AddItem(b);
					}
					else
					{
						if (packetStarted && buffer.Count <= 14)
						{
							buffer.AddItem(b);
							ZT2DataItem dataItem = dataItems.Get(ExternalSensorType.WIDEBAND);
							switch (buffer.Count)
							{
								case 4:
								{
									if (dataItem != null)
									{
										int raw = ByteUtil.AsUnsignedInt(buffer[3]);
										dataItem.SetRaw(raw);
									}
									break;
								}

								case 6:
								{
									dataItem = dataItems.Get(ExternalSensorType.EGT);
									if (dataItem != null)
									{
										int raw1 = ByteUtil.AsUnsignedInt(buffer[4]);
										int raw2 = ByteUtil.AsUnsignedInt(buffer[5]);
										dataItem.SetRaw(raw1, raw2);
									}
									break;
								}

								case 8:
								{
									dataItem = dataItems.Get(ExternalSensorType.ENGINE_SPEED);
									if (dataItem != null)
									{
										int raw1 = ByteUtil.AsUnsignedInt(buffer[6]);
										int raw2 = ByteUtil.AsUnsignedInt(buffer[7]);
										dataItem.SetRaw(raw1, raw2);
									}
									break;
								}

								case 10:
								{
									dataItem = dataItems.Get(ExternalSensorType.MAP);
									if (dataItem != null)
									{
										int raw1 = ByteUtil.AsUnsignedInt(buffer[8]);
										int raw2 = ByteUtil.AsUnsignedInt(buffer[9]);
										dataItem.SetRaw(raw1, raw2);
									}
									break;
								}

								case 11:
								{
									dataItem = dataItems.Get(ExternalSensorType.TPS);
									if (dataItem != null)
									{
										int raw = ByteUtil.AsUnsignedInt(buffer[10]);
										dataItem.SetRaw(raw);
									}
									break;
								}

								case 12:
								{
									dataItem = dataItems.Get(ExternalSensorType.USER1);
									if (dataItem != null)
									{
										int raw = ByteUtil.AsUnsignedInt(buffer[11]);
										dataItem.SetRaw(raw);
									}
									break;
								}

								case 14:
								{
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
				LOGGER.Error("ZT2 error occurred", t);
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

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
using RomRaider.IO.Connection;
using RomRaider.IO.Serial.Connection;
using RomRaider.Logger.External.Core;
using RomRaider.Logger.External.Innovate.Generic.Serial.IO;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.External.Innovate.Generic.Serial.IO
{
	public sealed class InnovateRunner : Stoppable
	{
		private static readonly Org.Apache.Log4j.Logger LOGGER = Org.Apache.Log4j.Logger.GetLogger
			(typeof(RomRaider.Logger.External.Innovate.Generic.Serial.IO.InnovateRunner));

		private const double MAX_AFR = 20.33;

		private readonly RomRaider.IO.Serial.Connection.SerialConnection connection;

		private readonly DataListener listener;

		private bool stop;

		public InnovateRunner(string port, DataListener listener)
		{
			ParamChecker.CheckNotNullOrEmpty(port, "port");
			this.connection = SerialConnection(port);
			// LC-1 & LM-2
			//        this.connection = new TestInnovateConnection("13036B00000000000000000000B2874313036B00000000000000000000B28743");
			// LM-1
			//        this.connection = new TestInnovateConnection("8113037C1E66012600720049003B003B");
			//        this.connection = new TestInnovateConnection("B28242310024B28242310000"); // bad data?
			this.listener = listener;
		}

		public void Run()
		{
			try
			{
				while (!stop)
				{
					byte b0 = NextByte();
					if (IsHeaderHighByte(b0))
					{
						byte b1 = NextByte();
						if (IsHeaderLowByte(b1))
						{
							int numWords = NumWords(b0, b1);
							byte[] bytes = new byte[numWords * 2];
							connection.Read(bytes);
							LOGGER.Trace("Innovate response: " + Packet(b0, b1, bytes));
							Process(bytes);
						}
						else
						{
							LOGGER.Trace("Innovate discarded: " + HexUtil.AsHex(b1));
						}
					}
					else
					{
						if (IsLm1HighByte(b0))
						{
							byte b1 = NextByte();
							if (IsLm1LowByte(b1))
							{
								byte[] rest = new byte[14];
								connection.Read(rest);
								byte[] bytes = new byte[16];
								bytes[0] = b0;
								bytes[1] = b1;
								System.Array.Copy(rest, 0, bytes, 2, rest.Length);
								LOGGER.Trace("Innovate response: " + HexUtil.AsHex(bytes));
								Process(bytes);
							}
							else
							{
								LOGGER.Trace("Innovate discarded: " + HexUtil.AsHex(b1));
							}
						}
						else
						{
							LOGGER.Trace("Innovate discarded: " + HexUtil.AsHex(b0));
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

		private void Process(byte[] bytes)
		{
			if (IsError(bytes))
			{
				double error = -1d * GetLambda(bytes);
				LOGGER.Error("Innovate error: " + error);
				listener.SetData(error);
			}
			else
			{
				if (IsOk(bytes))
				{
					double afr = GetAfr(bytes);
					LOGGER.Trace("Innovate AFR: " + afr);
					listener.SetData(afr > MAX_AFR ? MAX_AFR : afr);
				}
			}
		}

		private SerialConnectionImpl SerialConnection(string port)
		{
			ConnectionProperties properties = new InnovateConnectionProperties();
			return new SerialConnectionImpl(port, properties);
		}

		private byte NextByte()
		{
			return unchecked((byte)connection.Read());
		}

		// 1x11xx1x
		private bool IsHeaderHighByte(byte b)
		{
			return ByteUtil.MatchOnes(b, 178);
		}

		// 1xxxxxxx
		private bool IsHeaderLowByte(byte b)
		{
			return ByteUtil.MatchOnes(b, 128);
		}

		// 1x0xxx0x
		private bool IsLm1HighByte(byte b)
		{
			return ByteUtil.MatchOnes(b, 128) && ByteUtil.MatchZeroes(b, 34);
		}

		// 0xxxxxxx
		private bool IsLm1LowByte(byte b)
		{
			return ByteUtil.MatchZeroes(b, 128);
		}

		private double GetAfr(byte[] bytes)
		{
			return (GetLambda(bytes) + 500) * GetAf(bytes) / 10000.0;
		}

		private int GetAf(byte[] bytes)
		{
			return ((bytes[0] & 1) << 7) | bytes[1];
		}

		// xxx000xx
		private bool IsOk(byte[] bytes)
		{
			return ByteUtil.MatchZeroes(bytes[0], 28);
		}

		// xxx110xx
		private bool IsError(byte[] bytes)
		{
			return ByteUtil.MatchOnes(bytes[0], 24) && ByteUtil.MatchZeroes(bytes[0], 4);
		}

		// 01xxxxxx 0xxxxxxx
		private int GetLambda(byte[] bytes)
		{
			return ((bytes[2] & 63) << 7) | bytes[3];
		}

		private int NumWords(byte b0, byte b1)
		{
			int result = 0;
			if (ByteUtil.MatchOnes(b0, 1))
			{
				result |= 128;
			}
			if (ByteUtil.MatchOnes(b1, 64))
			{
				result |= 64;
			}
			if (ByteUtil.MatchOnes(b1, 32))
			{
				result |= 32;
			}
			if (ByteUtil.MatchOnes(b1, 16))
			{
				result |= 16;
			}
			if (ByteUtil.MatchOnes(b1, 8))
			{
				result |= 8;
			}
			if (ByteUtil.MatchOnes(b1, 4))
			{
				result |= 4;
			}
			if (ByteUtil.MatchOnes(b1, 2))
			{
				result |= 2;
			}
			if (ByteUtil.MatchOnes(b1, 1))
			{
				result |= 1;
			}
			return result;
		}

		private string Packet(byte b0, byte b1, byte[] bytes)
		{
			byte[] result = new byte[bytes.Length + 2];
			result[0] = b0;
			result[1] = b1;
			System.Array.Copy(bytes, 0, result, 2, bytes.Length);
			return HexUtil.AsHex(result);
		}
	}
}

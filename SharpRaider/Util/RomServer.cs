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
using System.Net.Sockets;
using Org.Apache.Log4j;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Util
{
	public sealed class RomServer
	{
		private static readonly Logger LOGGER = Logger.GetLogger(typeof(RomServer));

		private static readonly string HOST = "localhost";

		private const int PORT = 50001;

		public static bool IsRunning()
		{
			try
			{
				Socket sock = Sharpen.Extensions.CreateServerSocket(PORT);
				sock.Close();
				return false;
			}
			catch (Exception)
			{
				return true;
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		public static string WaitForRom()
		{
			Socket sock = Sharpen.Extensions.CreateServerSocket(PORT);
			try
			{
				return WaitForRom(sock);
			}
			finally
			{
				sock.Close();
			}
		}

		public static void SendRomToOpenInstance(string rom)
		{
			try
			{
				Socket socket = Sharpen.Extensions.CreateSocket(HOST, PORT);
				OutputStream os = socket.GetOutputStream();
				try
				{
					Write(os, rom);
				}
				finally
				{
					socket.Close();
				}
			}
			catch (Exception e)
			{
				LOGGER.Error("Error occurred", e);
			}
		}

		private static void Write(OutputStream os, string rom)
		{
			PrintWriter pw = new PrintWriter(os, true);
			try
			{
				pw.WriteLine(rom);
			}
			finally
			{
				pw.Close();
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		private static string WaitForRom(Socket sock)
		{
			Socket client = sock.Accept();
			try
			{
				return GetRom(client);
			}
			finally
			{
				client.Close();
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		private static string GetRom(Socket client)
		{
			BufferedReader br = new BufferedReader(new InputStreamReader(client.GetInputStream
				()));
			try
			{
				return br.ReadLine();
			}
			finally
			{
				br.Close();
			}
		}
	}
}

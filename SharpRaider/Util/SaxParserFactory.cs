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
using Javax.Xml.Parsers;
using Org.Xml.Sax.Helpers;
using Sharpen;

namespace RomRaider.Util
{
	public sealed class SaxParserFactory
	{
		public SaxParserFactory()
		{
			throw new NotSupportedException();
		}

		/// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"></exception>
		/// <exception cref="Org.Xml.Sax.SAXException"></exception>
		public static SAXParser GetSaxParser()
		{
			SAXParserFactory parserFactory = SAXParserFactory.NewInstance();
			parserFactory.SetNamespaceAware(false);
			parserFactory.SetValidating(true);
			parserFactory.SetXIncludeAware(false);
			return parserFactory.NewSAXParser();
		}

		public static void Main(string[] args)
		{
			try
			{
				SAXParser parser = RomRaider.Util.SaxParserFactory.GetSaxParser();
				BufferedInputStream b = new BufferedInputStream(new FileInputStream(new FilePath(
					"/ecu_defs.xml")));
				System.Console.Out.WriteLine(b.Available());
				parser.Parse(b, new DefaultHandler());
				System.Console.Out.WriteLine(parser.IsValidating());
			}
			catch (Exception ex)
			{
				System.Console.Error.WriteLine(ex);
			}
		}
	}
}

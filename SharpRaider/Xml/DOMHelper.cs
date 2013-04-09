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

using System.Text;
using Org.W3c.Dom;
using Sharpen;

namespace RomRaider.Xml
{
	public sealed class DOMHelper
	{
		public DOMHelper()
		{
		}

		public static string UnmarshallText(Node textNode)
		{
			StringBuilder buf = new StringBuilder();
			Node n;
			NodeList nodes = textNode.GetChildNodes();
			for (int i = 0; i < nodes.GetLength(); i++)
			{
				n = nodes.Item(i);
				if (n.GetNodeType() == Node.TEXT_NODE)
				{
					buf.Append(n.GetNodeValue());
				}
			}
			// expected a text-only node (skip)
			return buf.ToString();
		}

		public static string UnmarshallAttribute(Node node, string name, string defaultValue
			)
		{
			Node n = node.GetAttributes().GetNamedItem(name);
			return (n != null) ? (n.GetNodeValue()) : (defaultValue);
		}

		public static double UnmarshallAttribute(Node node, string name, double defaultValue
			)
		{
			return double.ParseDouble(UnmarshallAttribute(node, name, defaultValue.ToString()
				));
		}

		public static int UnmarshallAttribute(Node node, string name, int defaultValue)
		{
			return System.Convert.ToInt32(UnmarshallAttribute(node, name, defaultValue.ToString
				()));
		}

		public static bool UnmarshallAttribute(Node node, string name, bool defaultValue)
		{
			return System.Boolean.Parse(UnmarshallAttribute(node, name, defaultValue.ToString
				()));
		}
	}
}

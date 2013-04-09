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
using Org.Nfunk.Jep;
using Sharpen;

namespace RomRaider.Util
{
	public sealed class JEPUtil
	{
		public JEPUtil()
		{
		}

		public static double Evaluate(string expression, double value)
		{
			JEP parser = new JEP();
			parser.InitSymTab();
			// clear the contents of the symbol table
			parser.AddVariable("x", value);
			parser.ParseExpression(expression);
			return parser.GetValue();
		}

		public static double Evaluate(string expression, IDictionary<string, double> valueMap
			)
		{
			JEP parser = new JEP();
			parser.InitSymTab();
			// clear the contents of the symbol table
			foreach (string id in valueMap.Keys)
			{
				parser.AddVariable(id, valueMap.Get(id));
			}
			parser.ParseExpression(expression);
			return parser.GetValue();
		}
	}
}

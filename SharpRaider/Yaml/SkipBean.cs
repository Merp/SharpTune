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
using RomRaider.Yaml;
using Sharpen;

namespace RomRaider.Yaml
{
	public class SkipBean
	{
		private IList<int> listInt;

		private IList<string> listStr;

		private IList<DateTime> listDate;

		private IList<FilePath> empty = new AList<FilePath>(0);

		private IDictionary<string, int> map = new Dictionary<string, int>(0);

		private string text;

		private int number;

		public virtual IList<int> GetListInt()
		{
			return listInt;
		}

		public virtual void SetListInt(IList<int> listInt)
		{
			this.listInt = listInt;
		}

		public virtual IList<string> GetListStr()
		{
			return listStr;
		}

		public virtual void SetListStr(IList<string> listStr)
		{
			this.listStr = listStr;
		}

		public virtual IList<DateTime> GetListDate()
		{
			return listDate;
		}

		public virtual void SetListDate(IList<DateTime> listDate)
		{
			this.listDate = listDate;
		}

		public virtual string GetText()
		{
			return text;
		}

		public virtual void SetText(string text)
		{
			this.text = text;
		}

		public virtual int GetNumber()
		{
			return number;
		}

		public virtual void SetNumber(int number)
		{
			this.number = number;
		}

		public virtual IList<FilePath> GetEmpty()
		{
			return empty;
		}

		public virtual void SetEmpty(IList<FilePath> empty)
		{
			this.empty = empty;
		}

		public virtual IDictionary<string, int> GetMap()
		{
			return map;
		}

		public virtual void SetMap(IDictionary<string, int> map)
		{
			this.map = map;
		}
	}
}

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

using Javax.Swing.Filechooser;
using Sharpen;

namespace RomRaider.Swing
{
	public class XMLFilter : FileFilter
	{
		private Hashtable<string, FileFilter> filters = null;

		private string description = null;

		private string fullDescription = null;

		private bool useExtensionsInDescription = true;

		public XMLFilter()
		{
			this.filters = new Hashtable<string, FileFilter>();
			this.AddExtension("xml");
			this.SetDescription("ECU Definition Files");
		}

		public override bool Accept(FilePath f)
		{
			if (f != null)
			{
				if (f.IsDirectory())
				{
					return true;
				}
				string extension = GetExtension(f);
				if (extension != null && filters[GetExtension(f)] != null)
				{
					return true;
				}
			}
			return false;
		}

		public virtual string GetExtension(FilePath f)
		{
			if (f != null)
			{
				string filename = f.GetName();
				int i = filename.LastIndexOf('.');
				if (i > 0 && i < filename.Length - 1)
				{
					return Sharpen.Runtime.Substring(filename, i + 1).ToLower();
				}
			}
			return null;
		}

		public virtual void AddExtension(string extension)
		{
			filters.Put(extension.ToLower(), this);
			fullDescription = null;
		}

		public override string GetDescription()
		{
			if (fullDescription == null)
			{
				if (description == null || IsExtensionListInDescription())
				{
					fullDescription = description == null ? "(" : description + " (";
					// build the description from the extension list
					Enumeration<string> extensions = filters.Keys;
					if (extensions != null)
					{
						fullDescription += "." + extensions.Current;
						while (extensions.MoveNext())
						{
							fullDescription += ", ." + extensions.Current;
						}
					}
					fullDescription += ")";
				}
				else
				{
					fullDescription = description;
				}
			}
			return fullDescription;
		}

		public virtual void SetDescription(string description)
		{
			this.description = description;
			fullDescription = null;
		}

		public virtual void SetExtensionListInDescription(bool b)
		{
			useExtensionsInDescription = b;
			fullDescription = null;
		}

		public virtual bool IsExtensionListInDescription()
		{
			return useExtensionsInDescription;
		}
	}
}

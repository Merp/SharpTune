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

using Javax.Swing.Tree;
using RomRaider.Maps;
using RomRaider.Swing;
using Sharpen;

namespace RomRaider.Swing
{
	[System.Serializable]
	public class TableTreeNode : DefaultMutableTreeNode
	{
		private const long serialVersionUID = 2824050968863990871L;

		private string type;

		private Rom rom;

		private Table table;

		private string toolTip;

		private TableFrame frame;

		public TableTreeNode(Table table) : base(table)
		{
			//super(table.getName() + " (" + table.getType() + "D)");
			this.table = table;
			this.frame = table.GetFrame();
		}

		public virtual string GetType()
		{
			return type;
		}

		public virtual Rom GetRom()
		{
			return rom;
		}

		public virtual void SetRom(Rom rom)
		{
			this.rom = rom;
		}

		public virtual Table GetTable()
		{
			return table;
		}

		public virtual void SetTable(Table table)
		{
			this.table = table;
		}

		public virtual void SetToolTipText(string input)
		{
			toolTip = input;
		}

		public virtual string GetToolTipText()
		{
			return toolTip;
		}

		public virtual TableFrame GetFrame()
		{
			return frame;
		}

		public virtual void SetFrame(TableFrame frame)
		{
			this.frame = frame;
		}
	}
}

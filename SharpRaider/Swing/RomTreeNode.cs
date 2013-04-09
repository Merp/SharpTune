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
using RomRaider.Editor.Ecu;
using RomRaider.Maps;
using RomRaider.Swing;
using Sharpen;

namespace RomRaider.Swing
{
	[System.Serializable]
	public class RomTreeNode : DefaultMutableTreeNode
	{
		private const long serialVersionUID = -5534315445738460608L;

		private Rom rom = new Rom();

		internal ECUEditor parent;

		public RomTreeNode(Rom rom, int userLevel, bool isDisplayHighTables, ECUEditor parent
			)
		{
			this.parent = parent;
			SetRom(rom);
			Refresh(userLevel, isDisplayHighTables);
			UpdateFileName();
		}

		public virtual void Refresh(int userLevel, bool isDisplayHighTables)
		{
			RemoveAllChildren();
			Vector<Table> tables = rom.GetTables();
			for (int i = 0; i < tables.Count; i++)
			{
				Table table = tables[i];
				Add(table);
				if (isDisplayHighTables || userLevel >= table.GetUserLevel())
				{
					bool categoryExists = false;
					for (int j = 0; j < GetChildCount(); j++)
					{
						if (((DefaultMutableTreeNode)GetChildAt(j)).ToString().Equals(table.GetCategory()
							))
						{
							// add to appropriate category
							TableTreeNode tableNode = new TableTreeNode(table);
							((DefaultMutableTreeNode)GetChildAt(j)).Add(tableNode);
							categoryExists = true;
							break;
						}
					}
					if (!categoryExists)
					{
						// if category does not already exist, create it
						Add(new CategoryTreeNode(table.GetCategory(), table.GetRom()));
						TableTreeNode tableNode = new TableTreeNode(table);
						((DefaultMutableTreeNode)GetLastChild()).Add(tableNode);
					}
				}
			}
		}

		public override void RemoveAllChildren()
		{
			// close all table windows
			// loop through categories first
			for (int i = 0; i < GetChildCount(); i++)
			{
				DefaultMutableTreeNode category = ((DefaultMutableTreeNode)GetChildAt(i));
				// loop through tables in each category
				for (Enumeration<object> j = category.Children(); j.MoveNext(); )
				{
					((TableTreeNode)j.Current).GetFrame().Dispose();
				}
			}
			// removeAllChildren
			base.RemoveAllChildren();
		}

		public virtual void UpdateFileName()
		{
			SetUserObject(rom);
		}

		public virtual void Add(Table table)
		{
			TableFrame frame = new TableFrame(table, parent);
			table.SetFrame(frame);
		}

		public override TreeNode GetChildAt(int i)
		{
			return (DefaultMutableTreeNode)base.GetChildAt(i);
		}

		public override TreeNode GetLastChild()
		{
			return (DefaultMutableTreeNode)base.GetLastChild();
		}

		public virtual Rom GetRom()
		{
			return rom;
		}

		public virtual void SetRom(Rom rom)
		{
			this.rom = rom;
		}
	}
}

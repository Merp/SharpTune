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

using Javax.Swing;
using Javax.Swing.Event;
using RomRaider.Editor.Ecu;
using RomRaider.Maps;
using RomRaider.Swing;
using Sharpen;

namespace RomRaider.Swing
{
	[System.Serializable]
	public class TableFrame : JInternalFrame, InternalFrameListener
	{
		private const long serialVersionUID = -2651279694660392351L;

		private Table table;

		private readonly ECUEditor parent;

		private TableMenuBar tableMenuBar = null;

		public TableFrame(Table table, ECUEditor parent) : base(table.GetRom().GetFileName
			() + " - " + table.GetName(), true, true)
		{
			this.parent = parent;
			SetTable(table);
			Add(table);
			SetFrameIcon(null);
			SetBorder(BorderFactory.CreateBevelBorder(0));
			if (Runtime.GetProperty("os.name").StartsWith("Mac OS"))
			{
				PutClientProperty("JInternalFrame.isPalette", true);
			}
			SetVisible(false);
			tableMenuBar = new TableMenuBar(table);
			SetJMenuBar(tableMenuBar);
			SetDefaultCloseOperation(HIDE_ON_CLOSE);
			table.SetFrame(this);
			AddInternalFrameListener(this);
		}

		public virtual TableToolBar GetToolBar()
		{
			return parent.GetTableToolBar();
		}

		public virtual void InternalFrameActivated(InternalFrameEvent e)
		{
			GetTable().GetEditor().SetLastSelectedRom(GetTable().GetRom());
			parent.UpdateTableToolBar(this.table);
			parent.GetToolBar().UpdateButtons();
			parent.GetEditorMenuBar().UpdateMenu();
		}

		public virtual void InternalFrameOpened(InternalFrameEvent e)
		{
		}

		public virtual void InternalFrameClosing(InternalFrameEvent e)
		{
			GetTable().GetEditor().RemoveDisplayTable(this);
		}

		public virtual void InternalFrameClosed(InternalFrameEvent e)
		{
			parent.UpdateTableToolBar(null);
		}

		public virtual void InternalFrameIconified(InternalFrameEvent e)
		{
		}

		public virtual void InternalFrameDeiconified(InternalFrameEvent e)
		{
		}

		public virtual void InternalFrameDeactivated(InternalFrameEvent e)
		{
			parent.UpdateTableToolBar(null);
		}

		public virtual Table GetTable()
		{
			return table;
		}

		public virtual void SetTable(Table table)
		{
			this.table = table;
		}

		public virtual void UpdateFileName()
		{
			SetTitle(table.GetRom().GetFileName() + " - " + table.GetName());
		}

		public virtual TableMenuBar GetTableMenuBar()
		{
			return this.tableMenuBar;
		}
	}
}

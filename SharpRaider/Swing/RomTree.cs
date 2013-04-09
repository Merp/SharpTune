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
using Java.Awt;
using Java.Awt.Event;
using Javax.Swing;
using Javax.Swing.Tree;
using RomRaider.Editor.Ecu;
using RomRaider.Swing;
using Sharpen;

namespace RomRaider.Swing
{
	[System.Serializable]
	public class RomTree : JTree, MouseListener
	{
		private const long serialVersionUID = 1630446543383498886L;

		public static ECUEditor editor;

		public RomTree(DefaultMutableTreeNode input) : base(input)
		{
			SetRootVisible(false);
			SetRowHeight(0);
			AddMouseListener(this);
			SetCellRenderer(new RomCellRenderer());
			SetFont(new Font("Tahoma", Font.PLAIN, 11));
			// key binding actions
			Action tableSelectAction = new _AbstractAction_52(this);
			this.GetInputMap().Put(KeyStroke.GetKeyStroke(KeyEvent.VK_ENTER, 0), "enter");
			this.GetInputMap().Put(KeyStroke.GetKeyStroke(KeyEvent.VK_SPACE, 0), "space");
			this.GetActionMap().Put("enter", tableSelectAction);
			this.GetActionMap().Put("space", tableSelectAction);
		}

		private sealed class _AbstractAction_52 : AbstractAction
		{
			public _AbstractAction_52(RomTree _enclosing)
			{
				this._enclosing = _enclosing;
				this.serialVersionUID = -6008026264821746092L;
			}

			private const long serialVersionUID;

			public override void ActionPerformed(ActionEvent e)
			{
				try
				{
					object selectedRow = this._enclosing.GetSelectionPath().GetLastPathComponent();
					this._enclosing.ShowHideTable(selectedRow);
				}
				catch (ArgumentNullException)
				{
				}
			}

			private readonly RomTree _enclosing;
		}

		public virtual ECUEditor GetEditor()
		{
			return editor;
		}

		public virtual void SetContainer(ECUEditor container)
		{
			RomRaider.Swing.RomTree.editor = container;
		}

		public virtual void MouseClicked(MouseEvent e)
		{
			try
			{
				object selectedRow = GetPathForLocation(e.GetX(), e.GetY()).GetLastPathComponent(
					);
				if (e.GetClickCount() >= editor.GetSettings().GetTableClickCount())
				{
					ShowHideTable(selectedRow);
				}
				SetLastSelectedRom(selectedRow);
			}
			catch (ArgumentNullException)
			{
			}
		}

		private void ShowHideTable(object selectedRow)
		{
			try
			{
				if (selectedRow is TableTreeNode)
				{
					TableTreeNode node = (TableTreeNode)selectedRow;
					editor.DisplayTable(node.GetFrame());
				}
				SetLastSelectedRom(selectedRow);
			}
			catch (ArgumentNullException)
			{
			}
		}

		private void SetLastSelectedRom(object selectedNode)
		{
			TreePath selectedPath = GetSelectionPath();
			if (selectedNode is TableTreeNode || selectedNode is CategoryTreeNode || selectedNode
				 is RomTreeNode)
			{
				object lastSelectedPathComponent = GetLastSelectedPathComponent();
				if (lastSelectedPathComponent is TableTreeNode)
				{
					TableTreeNode node = (TableTreeNode)GetLastSelectedPathComponent();
					editor.SetLastSelectedRom(node.GetTable().GetRom());
				}
				else
				{
					if (lastSelectedPathComponent is CategoryTreeNode)
					{
						CategoryTreeNode node = (CategoryTreeNode)GetLastSelectedPathComponent();
						editor.SetLastSelectedRom(node.GetRom());
					}
					else
					{
						if (lastSelectedPathComponent is RomTreeNode)
						{
							RomTreeNode node = (RomTreeNode)GetLastSelectedPathComponent();
							editor.SetLastSelectedRom(node.GetRom());
						}
					}
				}
			}
			editor.GetEditorMenuBar().UpdateMenu();
			editor.GetToolBar().UpdateButtons();
		}

		public virtual void MousePressed(MouseEvent e)
		{
		}

		public virtual void MouseReleased(MouseEvent e)
		{
		}

		public virtual void MouseEntered(MouseEvent e)
		{
		}

		public virtual void MouseExited(MouseEvent e)
		{
		}

		protected override void RemoveDescendantToggledPaths(Enumeration<TreePath> toRemove
			)
		{
			base.RemoveDescendantToggledPaths(toRemove);
		}
	}
}

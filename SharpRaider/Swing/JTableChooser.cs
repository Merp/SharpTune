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

using Java.Awt;
using Java.Awt.Event;
using Javax.Swing;
using Javax.Swing.Tree;
using RomRaider.Maps;
using RomRaider.Swing;
using Sharpen;

namespace RomRaider.Swing
{
	[System.Serializable]
	public class JTableChooser : JOptionPane, MouseListener
	{
		public JTableChooser()
		{
			displayTree = new JTree(rootNode);
		}

		private const long serialVersionUID = 5611729002131147882L;

		internal JPanel displayPanel = new JPanel();

		internal DefaultMutableTreeNode rootNode = new DefaultMutableTreeNode("Open Images"
			);

		internal JTree displayTree;

		internal JScrollPane displayScrollPane;

		public virtual Table ShowChooser(Table targetTable)
		{
			Vector<Rom> roms = targetTable.GetEditor().GetImages();
			int nameLength = 0;
			for (int i = 0; i < roms.Count; i++)
			{
				Rom rom = roms[i];
				DefaultMutableTreeNode romNode = new DefaultMutableTreeNode(rom.GetFileName());
				rootNode.Add(romNode);
				for (int j = 0; j < rom.GetTables().Count; j++)
				{
					Table table = rom.GetTables()[j];
					// use the length of the table name to set the width of the displayTree
					// so the entire name can be read without being cut off on the right
					if (table.GetName().Length > nameLength)
					{
						nameLength = table.GetName().Length;
					}
					TableChooserTreeNode tableNode = new TableChooserTreeNode(table.GetName(), table);
					// categories
					bool categoryExists = false;
					for (int k = 0; k < romNode.GetChildCount(); k++)
					{
						if (Sharpen.Runtime.EqualsIgnoreCase(romNode.GetChildAt(k).ToString(), table.GetCategory
							()))
						{
							((DefaultMutableTreeNode)romNode.GetChildAt(k)).Add(tableNode);
							categoryExists = true;
							break;
						}
					}
					if (!categoryExists)
					{
						DefaultMutableTreeNode categoryNode = new DefaultMutableTreeNode(table.GetCategory
							());
						romNode.Add(categoryNode);
						categoryNode.Add(tableNode);
					}
				}
			}
			displayTree.SetPreferredSize(new Dimension(nameLength * 7, 400));
			displayTree.SetMinimumSize(new Dimension(nameLength * 7, 400));
			displayTree.ExpandPath(new TreePath(rootNode.GetPath()));
			displayTree.SetRootVisible(false);
			displayTree.AddMouseListener(this);
			displayScrollPane = new JScrollPane(displayTree);
			displayScrollPane.SetVerticalScrollBarPolicy(ScrollPaneConstants.VERTICAL_SCROLLBAR_ALWAYS
				);
			displayPanel.Add(displayScrollPane);
			object[] values = new object[] { "Compare", "Cancel" };
			if ((ShowOptionDialog(targetTable.GetEditor(), displayPanel, "Select a Map", JOptionPane
				.DEFAULT_OPTION, JOptionPane.PLAIN_MESSAGE, null, values, values[0]) == 0 && (displayTree
				.GetLastSelectedPathComponent() is TableChooserTreeNode)))
			{
				return ((TableChooserTreeNode)displayTree.GetLastSelectedPathComponent()).GetTable
					();
			}
			else
			{
				return null;
			}
		}

		public virtual void MouseReleased(MouseEvent e)
		{
			displayTree.SetPreferredSize(new Dimension(displayTree.GetWidth(), (displayTree.GetRowCount
				() * displayTree.GetRowHeight())));
			displayTree.Revalidate();
		}

		public virtual void MouseClicked(MouseEvent e)
		{
		}

		public virtual void MouseEntered(MouseEvent e)
		{
		}

		public virtual void MouseExited(MouseEvent e)
		{
		}

		public virtual void MousePressed(MouseEvent e)
		{
		}
	}
}

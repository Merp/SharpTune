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
using Javax.Swing;
using Javax.Swing.Tree;
using RomRaider.Maps;
using RomRaider.Swing;
using Sharpen;

namespace RomRaider.Swing
{
	public class RomCellRenderer : TreeCellRenderer
	{
		internal JLabel fileName;

		internal JLabel carInfo;

		internal DefaultTreeCellRenderer defaultRenderer = new DefaultTreeCellRenderer();

		public RomCellRenderer()
		{
			fileName = new JLabel(" ");
			fileName.SetFont(new Font("Tahoma", Font.BOLD, 11));
			fileName.SetHorizontalAlignment(JLabel.CENTER);
			carInfo = new JLabel(" ");
			carInfo.SetFont(new Font("Tahoma", Font.PLAIN, 10));
			carInfo.SetHorizontalAlignment(JLabel.CENTER);
		}

		public virtual Component GetTreeCellRendererComponent(JTree tree, object value, bool
			 selected, bool expanded, bool leaf, int row, bool hasFocus)
		{
			Component returnValue = null;
			if (value != null && value is RomTreeNode)
			{
				object userObject = ((DefaultMutableTreeNode)value).GetUserObject();
				if (userObject is Rom)
				{
					Rom rom = (Rom)userObject;
					if (expanded)
					{
						fileName.SetText("- " + rom.GetFileName());
					}
					else
					{
						fileName.SetText("+ " + rom.GetFileName());
					}
					carInfo.SetText(rom.GetRomIDString() + ", " + rom.GetRomID().GetCaseId() + "; " +
						 rom.GetRomID().GetYear() + " " + rom.GetRomID().GetMake() + " " + rom.GetRomID(
						).GetModel() + " " + rom.GetRomID().GetSubModel() + ", " + rom.GetRomID().GetTransmission
						());
					JPanel renderer = new JPanel(new GridLayout(2, 1));
					renderer.Add(fileName);
					renderer.Add(carInfo);
					if (selected)
					{
						renderer.SetBackground(new Color(220, 220, 255));
						renderer.SetBorder(BorderFactory.CreateLineBorder(new Color(0, 0, 225)));
					}
					else
					{
						renderer.SetBorder(BorderFactory.CreateLineBorder(new Color(220, 0, 0)));
						renderer.SetBackground(new Color(255, 210, 210));
					}
					renderer.SetPreferredSize(new Dimension(tree.GetParent().GetWidth(), 30));
					renderer.SetMaximumSize(new Dimension(tree.GetParent().GetWidth(), 30));
					renderer.SetEnabled(tree.IsEnabled());
					returnValue = renderer;
				}
			}
			else
			{
				if (value != null && value is TableTreeNode)
				{
					Table table = (Table)((DefaultMutableTreeNode)value).GetUserObject();
					JPanel renderer = new JPanel(new GridLayout(1, 1));
					renderer.SetBorder(BorderFactory.CreateLineBorder(Color.WHITE));
					JLabel tableName = new JLabel(string.Empty);
					renderer.SetBackground(Color.WHITE);
					// display icon
					if (table.GetType() == Table.TABLE_1D)
					{
						tableName = new JLabel(table.GetName() + " ", new ImageIcon(GetType().GetResource
							("/graphics/1d.gif")), JLabel.LEFT);
					}
					else
					{
						if (table.GetType() == Table.TABLE_2D)
						{
							tableName = new JLabel(table.GetName() + " ", new ImageIcon(GetType().GetResource
								("/graphics/2d.gif")), JLabel.LEFT);
						}
						else
						{
							if (table.GetType() == Table.TABLE_3D)
							{
								tableName = new JLabel(table.GetName() + " ", new ImageIcon(GetType().GetResource
									("/graphics/3d.gif")), JLabel.LEFT);
							}
							else
							{
								if (table.GetType() == Table.TABLE_SWITCH)
								{
									tableName = new JLabel(table.GetName() + " ", new ImageIcon(GetType().GetResource
										("/graphics/switch.gif")), JLabel.LEFT);
								}
							}
						}
					}
					// set color
					renderer.Add(tableName);
					tableName.SetFont(new Font("Tahoma", Font.PLAIN, 11));
					if (selected)
					{
						renderer.SetBackground(new Color(220, 220, 255));
						renderer.SetBorder(BorderFactory.CreateLineBorder(new Color(0, 0, 225)));
					}
					if (table.GetUserLevel() == 5)
					{
						tableName.SetForeground(new Color(255, 150, 150));
						tableName.SetFont(new Font("Tahoma", Font.ITALIC, 11));
					}
					else
					{
						if (table.GetUserLevel() > table.GetEditor().GetSettings().GetUserLevel())
						{
							//tableName.setForeground(new Color(185, 185, 185));
							tableName.SetFont(new Font("Tahoma", Font.ITALIC, 11));
						}
					}
					returnValue = renderer;
				}
			}
			if (returnValue == null)
			{
				returnValue = defaultRenderer.GetTreeCellRendererComponent(tree, value, selected, 
					expanded, leaf, row, hasFocus);
			}
			return returnValue;
		}
	}
}

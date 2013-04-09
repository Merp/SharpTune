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

using Java.Awt.Event;
using Javax.Swing;
using RomRaider.Maps;
using RomRaider.Swing;
using Sharpen;

namespace RomRaider.Swing
{
	[System.Serializable]
	public class TableMenuBar : JMenuBar, ActionListener
	{
		private const long serialVersionUID = -695692646459410510L;

		private readonly Table table;

		private JMenu fileMenu;

		private JMenuItem graph;

		private JMenu compareMenu;

		private JRadioButtonMenuItem compareOriginal;

		private JRadioButtonMenuItem compareMap;

		private JMenu similarOpenTables;

		private JRadioButtonMenuItem compareOff;

		private JMenu compareDisplay;

		private JRadioButtonMenuItem comparePercent;

		private JRadioButtonMenuItem compareAbsolute;

		private JMenu compareToValue;

		private JRadioButtonMenuItem compareToOriginal;

		private JRadioButtonMenuItem compareToBin;

		private JMenuItem close;

		private JMenu editMenu;

		private JMenuItem undoSel;

		private JMenuItem undoAll;

		private JMenuItem revert;

		private JMenuItem copySel;

		private JMenuItem copyTable;

		private JMenuItem paste;

		private JMenu viewMenu;

		private JMenuItem tableProperties;

		private ButtonGroup compareGroup;

		private ButtonGroup compareDisplayGroup;

		private ButtonGroup compareToGroup;

		public TableMenuBar(Table table)
		{
			//private JRadioButtonMenuItem overlay = new JRadioButtonMenuItem("Overlay Log");
			this.table = table;
			InitTableMenuBar();
		}

		public virtual void InitTableMenuBar()
		{
			InitFileMenu();
			InitEditMenu();
			InitViewMenu();
		}

		public virtual void RefreshTableMenuBar()
		{
			RefreshSimilarOpenTables();
			InitCompareGroup();
		}

		private void InitFileMenu()
		{
			fileMenu = new JMenu("Table");
			graph = new JMenuItem("View Graph");
			compareMenu = new JMenu("Compare");
			close = new JMenuItem("Close Table");
			InitCompareMenu();
			close.SetText("Close " + table.GetName());
			graph.AddActionListener(this);
			close.AddActionListener(this);
			graph.SetMnemonic('G');
			close.SetMnemonic('X');
			graph.SetEnabled(false);
			fileMenu.Add(graph);
			fileMenu.Add(compareMenu);
			fileMenu.Add(new JSeparator());
			fileMenu.Add(close);
			this.Add(fileMenu);
		}

		private void InitEditMenu()
		{
			editMenu = new JMenu("Edit");
			undoSel = new JMenuItem("Undo Selected Changes");
			undoAll = new JMenuItem("Undo All Changes");
			revert = new JMenuItem("Set Revert Point");
			copySel = new JMenuItem("Copy Selection");
			copyTable = new JMenuItem("Copy Table");
			paste = new JMenuItem("Paste");
			editMenu.Add(undoSel);
			editMenu.Add(undoAll);
			editMenu.Add(revert);
			editMenu.Add(new JSeparator());
			editMenu.Add(copySel);
			editMenu.Add(copyTable);
			editMenu.Add(new JSeparator());
			editMenu.Add(paste);
			editMenu.SetMnemonic('E');
			undoSel.SetMnemonic('U');
			undoAll.SetMnemonic('A');
			revert.SetMnemonic('R');
			copySel.SetMnemonic('C');
			copyTable.SetMnemonic('T');
			paste.SetMnemonic('P');
			undoSel.AddActionListener(this);
			undoAll.AddActionListener(this);
			revert.AddActionListener(this);
			copySel.AddActionListener(this);
			copyTable.AddActionListener(this);
			paste.AddActionListener(this);
			this.Add(editMenu);
		}

		private void InitViewMenu()
		{
			tableProperties = new JMenuItem("Table Properties");
			viewMenu = new JMenu("View");
			viewMenu.Add(tableProperties);
			viewMenu.SetMnemonic('V');
			tableProperties.SetMnemonic('P');
			tableProperties.AddActionListener(this);
			fileMenu.SetMnemonic('F');
			fileMenu.SetMnemonic('T');
			this.Add(viewMenu);
		}

		private void InitCompareMenu()
		{
			compareOriginal = new JRadioButtonMenuItem("Show Changes");
			compareOriginal.SetToolTipText("Compares the current values to the original or revert point values."
				);
			compareMap = new JRadioButtonMenuItem("Compare to Another Map");
			compareMap.SetToolTipText("Compares this table and a selected table.");
			similarOpenTables = new JMenu("Compare to Table");
			similarOpenTables.SetToolTipText("Compares this table to a similar table.");
			compareOff = new JRadioButtonMenuItem("Off");
			comparePercent = new JRadioButtonMenuItem("Percent Difference");
			compareAbsolute = new JRadioButtonMenuItem("Absolute Difference");
			compareDisplayGroup = new ButtonGroup();
			compareDisplayGroup.Add(comparePercent);
			compareDisplayGroup.Add(compareAbsolute);
			compareDisplay = new JMenu("Display");
			compareDisplay.Add(comparePercent);
			compareDisplay.Add(compareAbsolute);
			compareToOriginal = new JRadioButtonMenuItem("Compre to Original Value");
			compareToOriginal.SetToolTipText("Compares this table to the selected table's original or revert point values."
				);
			compareToBin = new JRadioButtonMenuItem("Compare to Bin Value");
			compareToBin.SetToolTipText("Compares this table to the selected table's current values."
				);
			compareToGroup = new ButtonGroup();
			compareToGroup.Add(compareToOriginal);
			compareToGroup.Add(compareToBin);
			compareToValue = new JMenu("Compare to");
			compareToValue.Add(compareToOriginal);
			compareToValue.Add(compareToBin);
			compareMenu.Add(compareOriginal);
			compareMenu.Add(compareMap);
			compareMenu.Add(similarOpenTables);
			compareMenu.Add(compareOff);
			compareMenu.Add(new JSeparator());
			compareMenu.Add(compareDisplay);
			compareMenu.Add(new JSeparator());
			compareMenu.Add(compareToValue);
			compareMenu.SetMnemonic('C');
			compareOriginal.SetMnemonic('C');
			compareMap.SetMnemonic('M');
			compareOff.SetMnemonic('O');
			compareDisplay.SetMnemonic('D');
			comparePercent.SetMnemonic('P');
			compareAbsolute.SetMnemonic('A');
			similarOpenTables.SetMnemonic('S');
			compareToValue.SetMnemonic('T');
			compareToOriginal.SetMnemonic('R');
			compareToOriginal.SetMnemonic('B');
			compareOff.SetSelected(true);
			compareAbsolute.SetSelected(true);
			compareToOriginal.SetSelected(true);
			InitCompareGroup();
			compareOriginal.AddActionListener(this);
			compareMap.AddActionListener(this);
			compareOff.AddActionListener(this);
			comparePercent.AddActionListener(this);
			compareAbsolute.AddActionListener(this);
			compareToOriginal.AddActionListener(this);
			compareToBin.AddActionListener(this);
		}

		private void InitCompareGroup()
		{
			compareGroup = new ButtonGroup();
			compareGroup.Add(compareOriginal);
			compareGroup.Add(compareMap);
			compareGroup.Add(compareOff);
			for (int i = 0; i < similarOpenTables.GetItemCount(); i++)
			{
				compareGroup.Add(similarOpenTables.GetItem(i));
			}
		}

		private void RefreshSimilarOpenTables()
		{
			similarOpenTables.RemoveAll();
			string currentTableName = table.GetName();
			Vector<Rom> roms = table.GetEditor().GetImages();
			foreach (Rom rom in roms)
			{
				Vector<Table> tables = rom.GetTables();
				foreach (Table table in tables)
				{
					if (Sharpen.Runtime.EqualsIgnoreCase(table.GetName(), currentTableName))
					{
						JRadioButtonMenuItem similarTable = new TableMenuItem(table);
						similarTable.AddActionListener(this);
						similarOpenTables.Add(similarTable);
					}
				}
			}
		}

		public virtual void ActionPerformed(ActionEvent e)
		{
			if (e.GetSource() == undoAll)
			{
				table.UndoAll();
			}
			else
			{
				if (e.GetSource() == revert)
				{
					table.SetRevertPoint();
				}
				else
				{
					if (e.GetSource() == undoSel)
					{
						table.UndoSelected();
					}
					else
					{
						if (e.GetSource() == close)
						{
							table.GetEditor().RemoveDisplayTable(table.GetFrame());
						}
						else
						{
							if (e.GetSource() == tableProperties)
							{
								JOptionPane.ShowMessageDialog(table, new TablePropertyPanel(table), table.GetName
									() + " Table Properties", JOptionPane.INFORMATION_MESSAGE);
							}
							else
							{
								if (e.GetSource() == copySel)
								{
									table.CopySelection();
								}
								else
								{
									if (e.GetSource() == copyTable)
									{
										table.CopyTable();
									}
									else
									{
										if (e.GetSource() == paste)
										{
											table.Paste();
										}
										else
										{
											if (e.GetSource() == compareOff)
											{
												CompareByDisplay(Table.COMPARE_DISPLAY_OFF);
											}
											else
											{
												if (e.GetSource() == compareAbsolute)
												{
													CompareByDisplay(Table.COMPARE_DISPLAY_ABSOLUTE);
												}
												else
												{
													if (e.GetSource() == comparePercent)
													{
														CompareByDisplay(Table.COMPARE_DISPLAY_PERCENT);
													}
													else
													{
														if (e.GetSource() == compareOriginal)
														{
															table.SetCompareType(Table.COMPARE_TYPE_ORIGINAL);
															compareToOriginal.SetSelected(true);
															CompareByTable(table);
														}
														else
														{
															if (e.GetSource() == compareMap)
															{
																JTableChooser chooser = new JTableChooser();
																Table selectedTable = chooser.ShowChooser(table);
																if (null != selectedTable)
																{
																	CompareByTable(selectedTable);
																}
															}
															else
															{
																if (e.GetSource() is TableMenuItem)
																{
																	Table selectedTable = ((TableMenuItem)e.GetSource()).GetTable();
																	CompareByTable(selectedTable);
																}
																else
																{
																	if (e.GetSource() == compareToOriginal)
																	{
																		CompareByType(Table.COMPARE_TYPE_ORIGINAL);
																	}
																	else
																	{
																		if (e.GetSource() == compareToBin)
																		{
																			CompareByType(Table.COMPARE_TYPE_BIN);
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		private void CompareByType(int compareType)
		{
			table.SetCompareType(compareType);
			if (table.FillCompareValues())
			{
				table.RefreshCellDisplay();
			}
		}

		private void CompareByTable(Table selectedTable)
		{
			if (null == selectedTable)
			{
				return;
			}
			if (table.GetCompareDisplay() == Table.COMPARE_DISPLAY_OFF)
			{
				// Default to absolute if none selected.
				this.compareAbsolute.SetSelected(true);
				table.SetCompareDisplay(Table.COMPARE_DISPLAY_ABSOLUTE);
			}
			selectedTable.AddComparedToTable(table);
			table.SetCompareTable(selectedTable);
			if (table.FillCompareValues())
			{
				table.RefreshCellDisplay();
			}
		}

		public virtual void CompareByDisplay(int compareDisplay)
		{
			table.SetCompareDisplay(compareDisplay);
			table.RefreshCellDisplay();
		}
	}
}

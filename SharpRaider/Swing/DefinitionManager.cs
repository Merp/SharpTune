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
using RomRaider;
using RomRaider.Swing;
using Sharpen;

namespace RomRaider.Swing
{
	[System.Serializable]
	public class DefinitionManager : JFrame, ActionListener, Runnable
	{
		private const long serialVersionUID = -3920843496218196737L;

		public static int MOVE_UP = 0;

		public static int MOVE_DOWN = 1;

		private static Settings settings = ECUExec.settings;

		internal Vector<string> fileNames;

		protected internal object Lock;

		public DefinitionManager()
		{
			//ECUEditor parent;
			//ECUEditor parent) {
			//this.setIconImage(parent.getIconImage());
			InitComponents();
			//this.parent = parent;
			InitSettings();
			definitionList.SetFont(new Font("Tahoma", Font.PLAIN, 11));
			definitionList.SetSelectionMode(ListSelectionModel.SINGLE_SELECTION);
			btnCancel.AddActionListener(this);
			btnSave.AddActionListener(this);
			btnAddDefinition.AddActionListener(this);
			btnRemoveDefinition.AddActionListener(this);
			btnMoveUp.AddActionListener(this);
			btnMoveDown.AddActionListener(this);
			btnApply.AddActionListener(this);
			btnUndo.AddActionListener(this);
		}

		public virtual void Run()
		{
			this.SetLocationByPlatform(true);
			//this.setLocationRelativeTo(parent);
			this.SetVisible(true);
			this.InitSettings();
		}

		public virtual void RunModal(bool b)
		{
			this.Run();
			if (b)
			{
				this.AddFile();
			}
			while (this.IsVisible())
			{
				try
				{
					Sharpen.Thread.Sleep(1000);
				}
				catch (Exception e)
				{
					// TODO Auto-generated catch block
					Sharpen.Runtime.PrintStackTrace(e);
				}
			}
		}

		private sealed class _WindowAdapter_97 : WindowAdapter
		{
			public _WindowAdapter_97()
			{
			}

			public override void WindowClosed(WindowEvent evt)
			{
			}
		}

		private WindowListener tDMWL = new _WindowAdapter_97();

		private void InitSettings()
		{
			// add definitions to list
			Vector<FilePath> definitionFiles = settings.GetEcuDefinitionFiles();
			fileNames = new Vector<string>();
			for (int i = 0; i < definitionFiles.Count; i++)
			{
				fileNames.AddItem(definitionFiles[i].GetAbsolutePath());
			}
			UpdateListModel();
		}

		// <editor-fold defaultstate="collapsed" desc=" Generated Code ">//GEN-BEGIN:initComponents
		private void InitComponents()
		{
			jScrollPane1 = new JScrollPane();
			definitionList = new JList();
			defLabel = new JLabel();
			btnMoveUp = new JButton();
			btnMoveDown = new JButton();
			btnAddDefinition = new JButton();
			btnRemoveDefinition = new JButton();
			btnSave = new JButton();
			btnCancel = new JButton();
			btnApply = new JButton();
			btnUndo = new JButton();
			SetDefaultCloseOperation(WindowConstants.DISPOSE_ON_CLOSE);
			SetTitle("Definition File Manager");
			jScrollPane1.SetViewportView(definitionList);
			defLabel.SetText("ECU Definition File Priority");
			btnMoveUp.SetText("Move Up");
			btnMoveDown.SetText("Move Down");
			btnAddDefinition.SetText("Add...");
			btnRemoveDefinition.SetText("Remove");
			btnSave.SetText("Save");
			btnCancel.SetText("Cancel");
			btnApply.SetText("Apply");
			btnUndo.SetText("Undo");
			GroupLayout layout = new GroupLayout(GetContentPane());
			layout.LinkSize(SwingConstants.HORIZONTAL, new Component[] { btnMoveUp, btnMoveDown
				, btnAddDefinition, btnRemoveDefinition });
			layout.SetVerticalGroup(((GroupLayout.ParallelGroup)layout.CreateParallelGroup(GroupLayout.Alignment
				.LEADING).AddGroup(((GroupLayout.SequentialGroup)((GroupLayout.SequentialGroup)(
				(GroupLayout.SequentialGroup)((GroupLayout.SequentialGroup)layout.CreateSequentialGroup
				().AddContainerGap().AddComponent(defLabel)).AddPreferredGap(LayoutStyle.ComponentPlacement
				.RELATED).AddComponent(jScrollPane1, GroupLayout.PREFERRED_SIZE, GroupLayout.DEFAULT_SIZE
				, GroupLayout.PREFERRED_SIZE)).AddPreferredGap(LayoutStyle.ComponentPlacement.RELATED
				).AddGroup(((GroupLayout.ParallelGroup)((GroupLayout.ParallelGroup)((GroupLayout.ParallelGroup
				)((GroupLayout.ParallelGroup)layout.CreateParallelGroup(GroupLayout.Alignment.BASELINE
				).AddComponent(btnMoveUp)).AddComponent(btnMoveDown)).AddComponent(btnRemoveDefinition
				, GroupLayout.PREFERRED_SIZE, 23, GroupLayout.PREFERRED_SIZE)).AddComponent(btnAddDefinition
				)))).AddPreferredGap(LayoutStyle.ComponentPlacement.RELATED).AddGroup(((GroupLayout.ParallelGroup
				)((GroupLayout.ParallelGroup)((GroupLayout.ParallelGroup)((GroupLayout.ParallelGroup
				)layout.CreateParallelGroup(GroupLayout.Alignment.BASELINE).AddComponent(btnSave
				)).AddComponent(btnApply)).AddComponent(btnUndo)).AddComponent(btnCancel)))).AddContainerGap
				(GroupLayout.DEFAULT_SIZE, short.MaxValue))));
			layout.SetHorizontalGroup(((GroupLayout.ParallelGroup)layout.CreateParallelGroup(
				GroupLayout.Alignment.LEADING).AddGroup(((GroupLayout.SequentialGroup)layout.CreateSequentialGroup
				().AddContainerGap().AddGroup(((GroupLayout.ParallelGroup)((GroupLayout.ParallelGroup
				)layout.CreateParallelGroup(GroupLayout.Alignment.TRAILING).AddComponent(jScrollPane1
				, GroupLayout.DEFAULT_SIZE, 448, short.MaxValue)).AddGroup(((GroupLayout.SequentialGroup
				)((GroupLayout.SequentialGroup)layout.CreateSequentialGroup().AddGroup(((GroupLayout.ParallelGroup
				)((GroupLayout.ParallelGroup)layout.CreateParallelGroup(GroupLayout.Alignment.TRAILING
				).AddGroup(((GroupLayout.SequentialGroup)((GroupLayout.SequentialGroup)((GroupLayout.SequentialGroup
				)((GroupLayout.SequentialGroup)layout.CreateSequentialGroup().AddComponent(btnSave
				)).AddPreferredGap(LayoutStyle.ComponentPlacement.RELATED).AddComponent(btnApply
				)).AddPreferredGap(LayoutStyle.ComponentPlacement.RELATED).AddComponent(btnUndo)
				).AddPreferredGap(LayoutStyle.ComponentPlacement.RELATED).AddComponent(btnCancel
				)))).AddGroup(((GroupLayout.ParallelGroup)((GroupLayout.ParallelGroup)layout.CreateParallelGroup
				(GroupLayout.Alignment.LEADING).AddComponent(defLabel)).AddGroup(((GroupLayout.SequentialGroup
				)((GroupLayout.SequentialGroup)((GroupLayout.SequentialGroup)layout.CreateSequentialGroup
				().AddComponent(btnMoveDown)).AddPreferredGap(LayoutStyle.ComponentPlacement.RELATED
				).AddComponent(btnMoveUp)).AddPreferredGap(LayoutStyle.ComponentPlacement.UNRELATED
				).AddComponent(btnAddDefinition)))))))).AddPreferredGap(LayoutStyle.ComponentPlacement
				.RELATED).AddComponent(btnRemoveDefinition)))))).AddContainerGap())));
			GetContentPane().SetLayout(layout);
			Pack();
		}

		// </editor-fold>//GEN-END:initComponents
		public virtual void ActionPerformed(ActionEvent e)
		{
			if (e.GetSource() == btnCancel)
			{
				Dispose();
			}
			else
			{
				if (e.GetSource() == btnSave)
				{
					SaveSettings();
					Dispose();
				}
				else
				{
					if (e.GetSource() == btnApply)
					{
						SaveSettings();
					}
					else
					{
						if (e.GetSource() == btnMoveUp)
						{
							MoveSelection(MOVE_UP);
						}
						else
						{
							if (e.GetSource() == btnMoveDown)
							{
								MoveSelection(MOVE_DOWN);
							}
							else
							{
								if (e.GetSource() == btnAddDefinition)
								{
									AddFile();
								}
								else
								{
									if (e.GetSource() == btnRemoveDefinition)
									{
										RemoveSelection();
									}
									else
									{
										if (e.GetSource() == btnUndo)
										{
											InitSettings();
										}
									}
								}
							}
						}
					}
				}
			}
		}

		public virtual void SaveSettings()
		{
			Vector<FilePath> output = new Vector<FilePath>();
			// create file vector
			for (int i = 0; i < fileNames.Count; i++)
			{
				output.AddItem(new FilePath(fileNames[i]));
			}
			// save
			//parent.getSettings().
			settings.SetEcuDefinitionFiles(output);
		}

		public virtual void AddFile()
		{
			JFileChooser fc = new JFileChooser(Settings.RRECUDEFREPO);
			fc.SetFileFilter(new XMLFilter());
			if (fc.ShowOpenDialog(this) == JFileChooser.APPROVE_OPTION)
			{
				fileNames.AddItem(fc.GetSelectedFile().GetAbsolutePath());
				UpdateListModel();
			}
		}

		public virtual void MoveSelection(int direction)
		{
			int selectedIndex = definitionList.GetSelectedIndex();
			string fileName = fileNames[selectedIndex];
			if (direction == MOVE_UP && selectedIndex > 0)
			{
				fileNames.Remove(selectedIndex);
				fileNames.Add(--selectedIndex, fileName);
			}
			else
			{
				if (direction == MOVE_DOWN && selectedIndex < definitionList.GetModel().GetSize())
				{
					fileNames.Remove(selectedIndex);
					fileNames.Add(++selectedIndex, fileName);
				}
			}
			UpdateListModel();
			definitionList.SetSelectedIndex(selectedIndex);
		}

		public virtual void RemoveSelection()
		{
			int index = definitionList.GetSelectedIndex();
			if (index < 0)
			{
				return;
			}
			fileNames.Remove(index);
			UpdateListModel();
		}

		public virtual void UpdateListModel()
		{
			definitionList.SetListData(fileNames);
		}

		private JButton btnAddDefinition;

		private JButton btnApply;

		private JButton btnCancel;

		private JButton btnMoveDown;

		private JButton btnMoveUp;

		private JButton btnRemoveDefinition;

		private JButton btnSave;

		private JButton btnUndo;

		private JLabel defLabel;

		private JList definitionList;

		private JScrollPane jScrollPane1;
		// Variables declaration - do not modify//GEN-BEGIN:variables
		// End of variables declaration//GEN-END:variables
	}
}

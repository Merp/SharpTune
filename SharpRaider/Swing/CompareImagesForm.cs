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
using Javax.Swing.Border;
using RomRaider.Maps;
using RomRaider.Swing;
using Sharpen;

namespace RomRaider.Swing
{
	[System.Serializable]
	public class CompareImagesForm : JFrame, ActionListener
	{
		private const long serialVersionUID = -8937472127815934398L;

		private readonly Vector<Rom> roms;

		private readonly JPanel contentPane;

		private readonly JComboBox comboBoxImageLeft;

		private readonly JComboBox comboBoxImageRight;

		private readonly JButton btnCompare;

		private readonly JList listChanges;

		private readonly DefaultListModel listModelChanges = new DefaultListModel();

		private readonly CompareImagesForm.ChangeListCellRenderer changeRenderer;

		private readonly JScrollPane scrollPaneResults;

		private readonly JLabel lblImageResultString;

		public static Color equal = new Color(52, 114, 53);

		public static Color different = new Color(193, 27, 23);

		public static Color missing = new Color(251, 185, 23);

		public CompareImagesForm(Vector<Rom> roms, Image parentImage)
		{
			changeRenderer = new CompareImagesForm.ChangeListCellRenderer(this);
			this.SetIconImage(parentImage);
			SetResizable(false);
			this.roms = roms;
			SetTitle("Compare Images");
			SetBounds(100, 100, 600, 450);
			this.contentPane = new JPanel();
			this.contentPane.SetBorder(new EmptyBorder(5, 5, 5, 5));
			SetContentPane(this.contentPane);
			JLabel lblSelectImages = new JLabel("Selected Images");
			lblSelectImages.SetBounds(10, 11, 79, 14);
			contentPane.SetLayout(null);
			JPanel panelImageSelector = new JPanel();
			panelImageSelector.SetBounds(10, 36, 574, 94);
			panelImageSelector.SetBorder(new EtchedBorder(EtchedBorder.LOWERED, null, null));
			panelImageSelector.SetLayout(null);
			JLabel lblImageLeft = new JLabel("Image (Left):");
			lblImageLeft.SetBounds(10, 10, 70, 14);
			panelImageSelector.Add(lblImageLeft);
			this.comboBoxImageLeft = new JComboBox();
			this.comboBoxImageLeft.SetBounds(89, 7, 475, 20);
			this.comboBoxImageLeft.SetToolTipText("Select an image to compare.");
			this.comboBoxImageLeft.SetRenderer(new CompareImagesForm.ComboBoxRenderer(this));
			panelImageSelector.Add(this.comboBoxImageLeft);
			JLabel lblImageRight = new JLabel("Image (Right):");
			lblImageRight.SetBounds(10, 35, 70, 14);
			panelImageSelector.Add(lblImageRight);
			this.comboBoxImageRight = new JComboBox();
			this.comboBoxImageRight.SetBounds(89, 32, 475, 20);
			this.comboBoxImageRight.SetToolTipText("Select an image to compare.");
			this.comboBoxImageRight.SetRenderer(new CompareImagesForm.ComboBoxRenderer(this));
			panelImageSelector.Add(this.comboBoxImageRight);
			this.btnCompare = new JButton("Compare");
			this.btnCompare.AddActionListener(this);
			this.btnCompare.SetBounds(10, 64, 89, 23);
			panelImageSelector.Add(this.btnCompare);
			this.contentPane.Add(panelImageSelector);
			this.contentPane.Add(lblSelectImages);
			JLabel lblResults = new JLabel("Results:");
			lblResults.SetBounds(10, 141, 46, 14);
			contentPane.Add(lblResults);
			lblImageResultString = new JLabel("Compare images...");
			lblImageResultString.SetBounds(66, 141, 518, 14);
			contentPane.Add(lblImageResultString);
			scrollPaneResults = new JScrollPane();
			scrollPaneResults.SetBounds(10, 166, 574, 245);
			contentPane.Add(scrollPaneResults);
			this.listChanges = new JList(this.listModelChanges);
			scrollPaneResults.SetViewportView(this.listChanges);
			listChanges.SetCellRenderer(changeRenderer);
			listChanges.SetSelectionMode(ListSelectionModel.MULTIPLE_INTERVAL_SELECTION);
			PopulateComboBoxes();
		}

		public virtual void PopulateComboBoxes()
		{
			for (int i = 0; i < roms.Count; i++)
			{
				Rom curRom = roms[i];
				comboBoxImageLeft.AddItem(curRom);
				comboBoxImageRight.AddItem(curRom);
			}
			if (comboBoxImageRight.GetItemCount() > 1)
			{
				comboBoxImageRight.SetSelectedIndex(1);
			}
		}

		public virtual void CompareTables(Rom left, Rom right)
		{
			listModelChanges.Clear();
			Vector<Table> leftTables = left.GetTables();
			Vector<Table> rightTables = right.GetTables();
			int equal = 0;
			int different = 0;
			int missing = 0;
			string leftTableName;
			string rightTableName;
			string leftTableAsString;
			string rightTableAsString;
			bool found = false;
			// Compare the tables.
			for (int x = 0; x < leftTables.Count; x++)
			{
				found = false;
				leftTableName = leftTables[x].GetName().Trim().ToLower();
				for (int y = 0; y < rightTables.Count; y++)
				{
					rightTableName = rightTables[y].GetName().Trim().ToLower();
					if (leftTableName.Equals(rightTableName))
					{
						// Same table.  Compare table as string
						found = true;
						leftTableAsString = leftTables[x].GetTableAsString().ToString().Trim().ToLower();
						rightTableAsString = rightTables[y].GetTableAsString().ToString().Trim().ToLower(
							);
						if (leftTableAsString.Equals(rightTableAsString))
						{
							// Tables are equal
							equal++;
							listModelChanges.AddElement(new CompareImagesForm.ListItem(this, 1, leftTables[x]
								.GetName()));
						}
						else
						{
							// Tables are different
							different++;
							listModelChanges.AddElement(new CompareImagesForm.ListItem(this, 2, leftTables[x]
								.GetName()));
						}
						break;
					}
				}
				if (!found)
				{
					missing++;
					listModelChanges.AddElement(new CompareImagesForm.ListItem(this, 3, leftTables[x]
						.GetName()));
				}
			}
			// Check if rightTables has tables that do not exist in left table.
			for (int x_1 = 0; x_1 < rightTables.Count; x_1++)
			{
				found = false;
				rightTableName = rightTables[x_1].GetName().Trim().ToLower();
				for (int y = 0; y < leftTables.Count; y++)
				{
					leftTableName = leftTables[y].GetName().Trim().ToLower();
					if (rightTableName.Equals(leftTableName))
					{
						found = true;
					}
				}
				if (!found)
				{
					missing++;
					listModelChanges.AddElement(new CompareImagesForm.ListItem(this, 3, rightTables[x_1
						].GetName()));
				}
			}
			// Fill out the result string.
			if (equal > 0 && different == 0 && missing == 0)
			{
				lblImageResultString.SetText("Images are equal.");
				lblImageResultString.SetForeground(RomRaider.Swing.CompareImagesForm.equal);
			}
			else
			{
				if (different > 0)
				{
					lblImageResultString.SetText("Images are NOT equal.  Equal Tables: " + equal + ", Changed Tables: "
						 + different + ", Missing Tables: " + missing);
					lblImageResultString.SetForeground(RomRaider.Swing.CompareImagesForm.different);
				}
				else
				{
					lblImageResultString.SetText("Images are NOT equal.  Equal Tables: " + equal + ", Changed Tables: "
						 + different + ", Missing Tables: " + missing);
					lblImageResultString.SetForeground(RomRaider.Swing.CompareImagesForm.missing);
				}
			}
			// Check if the list has items.
			if (listModelChanges.Size() < 1)
			{
				listModelChanges.AddElement(new CompareImagesForm.ListItem(this, 0, "No tables are equal, different, or missing."
					));
				lblImageResultString.SetText("Unable to compare images.");
				lblImageResultString.SetForeground(Color.RED);
				return;
			}
			// Add list items for 0 counts.
			if (equal == 0)
			{
				listModelChanges.AddElement(new CompareImagesForm.ListItem(this, 1, "No Equal Tables."
					));
			}
			if (different == 0)
			{
				listModelChanges.AddElement(new CompareImagesForm.ListItem(this, 2, "No Changed Tables."
					));
			}
			if (missing == 0)
			{
				listModelChanges.AddElement(new CompareImagesForm.ListItem(this, 3, "No Missing Tables."
					));
			}
		}

		public virtual void ActionPerformed(ActionEvent e)
		{
			this.SetCursor(Cursor.GetPredefinedCursor(Cursor.WAIT_CURSOR));
			if (e.GetSource() == this.btnCompare)
			{
				if (this.comboBoxImageLeft.GetItemCount() > 0 && this.comboBoxImageRight.GetItemCount
					() > 0)
				{
					Rom leftRom = (Rom)this.comboBoxImageLeft.GetSelectedItem();
					Rom rightRom = (Rom)this.comboBoxImageRight.GetSelectedItem();
					if (leftRom != null && rightRom != null)
					{
						CompareTables(leftRom, rightRom);
					}
				}
			}
			this.SetCursor(Cursor.GetDefaultCursor());
		}

		[System.Serializable]
		internal class ComboBoxRenderer : JLabel, ListCellRenderer
		{
			private const long serialVersionUID = 831689602429105854L;

			public ComboBoxRenderer(CompareImagesForm _enclosing)
			{
				this._enclosing = _enclosing;
				this.SetOpaque(true);
				this.SetHorizontalAlignment(SwingConstants.LEFT);
				this.SetVerticalAlignment(SwingConstants.CENTER);
			}

			public virtual Component GetListCellRendererComponent(JList list, object value, int
				 index, bool isSelected, bool cellHasFocus)
			{
				if (isSelected)
				{
					this.SetBackground(list.GetSelectionBackground());
					this.SetForeground(list.GetSelectionForeground());
				}
				else
				{
					this.SetBackground(list.GetBackground());
					this.SetForeground(list.GetForeground());
				}
				if (value != null)
				{
					// Set the text to the rom file name.
					Rom rom = (Rom)value;
					this.SetText(rom.GetFileName());
					this.SetFont(list.GetFont());
				}
				return this;
			}

			private readonly CompareImagesForm _enclosing;
		}

		[System.Serializable]
		internal class ChangeListCellRenderer : JLabel, ListCellRenderer
		{
			private const long serialVersionUID = -3645192077787635239L;

			public ChangeListCellRenderer(CompareImagesForm _enclosing)
			{
				this._enclosing = _enclosing;
				this.SetOpaque(true);
				this.SetHorizontalAlignment(SwingConstants.LEFT);
				this.SetVerticalAlignment(SwingConstants.CENTER);
			}

			public virtual Component GetListCellRendererComponent(JList paramList, object value
				, int index, bool isSelected, bool cellHasFocus)
			{
				// Set the background color.
				if (isSelected)
				{
					this.SetBackground(paramList.GetSelectionBackground());
				}
				else
				{
					this.SetBackground(paramList.GetBackground());
				}
				// Set the foreground color based on the item type.
				CompareImagesForm.ListItem item = (CompareImagesForm.ListItem)value;
				switch (item.GetType())
				{
					case 1:
					{
						// equal - default green
						this.SetForeground(CompareImagesForm.equal);
						break;
					}

					case 2:
					{
						// different - default red
						this.SetForeground(CompareImagesForm.different);
						break;
					}

					case 3:
					{
						// missing - default yellow
						this.SetForeground(CompareImagesForm.missing);
						break;
					}

					default:
					{
						this.SetForeground(paramList.GetForeground());
						break;
						break;
					}
				}
				this.SetText(item.GetValue());
				return this;
			}

			private readonly CompareImagesForm _enclosing;
		}

		internal class ListItem
		{
			private int type;

			private string value;

			public ListItem(CompareImagesForm _enclosing, int type, string value)
			{
				this._enclosing = _enclosing;
				this.type = type;
				this.value = value;
			}

			public virtual int GetType()
			{
				return this.type;
			}

			public virtual void SetType(int type)
			{
				this.type = type;
			}

			public virtual string GetValue()
			{
				return this.value;
			}

			public virtual void SetValue(string value)
			{
				this.value = value;
			}

			private readonly CompareImagesForm _enclosing;
		}
	}
}

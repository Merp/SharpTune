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
using Org.Jdesktop.Layout;
using RomRaider.Maps;
using Sharpen;

namespace RomRaider.Swing
{
	[System.Serializable]
	public class TablePropertyPanel : JPanel
	{
		private const long serialVersionUID = -5817685772039277602L;

		public TablePropertyPanel(Table table)
		{
			InitComponents();
			SetVisible(true);
			tableName.SetText(table.GetName() + " (" + table.GetType() + "D)");
			category.SetText(table.GetCategory());
			unit.SetText(table.GetScale().GetUnit());
			byteToReal.SetText(table.GetScale().GetExpression());
			realToByte.SetText(table.GetScale().GetByteExpression());
			string intType;
			if (table.IsSignedData())
			{
				intType = "int";
			}
			else
			{
				intType = "uint";
			}
			storageSize.SetText(intType + (table.GetStorageType() * 8));
			storageAddress.SetText("0x" + Sharpen.Extensions.ToHexString(table.GetStorageAddress
				()));
			if (table.GetEndian() == Table.ENDIAN_BIG)
			{
				endian.SetText("big");
			}
			else
			{
				endian.SetText("little");
			}
			description.SetText(table.GetDescription());
			fine.SetText(table.GetScale().GetFineIncrement() + string.Empty);
			coarse.SetText(table.GetScale().GetCoarseIncrement() + string.Empty);
			if (table.GetUserLevel() == 1)
			{
				userLevel.SetText("Beginner");
			}
			else
			{
				if (table.GetUserLevel() == 2)
				{
					userLevel.SetText("Intermediate");
				}
				else
				{
					if (table.GetUserLevel() == 3)
					{
						userLevel.SetText("Advanced");
					}
					else
					{
						if (table.GetUserLevel() == 4)
						{
							userLevel.SetText("All");
						}
						else
						{
							if (table.GetUserLevel() == 5)
							{
								userLevel.SetText("Debug");
							}
						}
					}
				}
			}
		}

		// <editor-fold defaultstate="collapsed" desc=" Generated Code ">//GEN-BEGIN:initComponents
		private void InitComponents()
		{
			lblTable = new JLabel();
			tableName = new JLabel();
			lblCategory = new JLabel();
			category = new JLabel();
			jPanel1 = new JPanel();
			lblUnit = new JLabel();
			unit = new JLabel();
			lblByteToReal = new JLabel();
			byteToReal = new JLabel();
			realToByte = new JLabel();
			lblRealToByte = new JLabel();
			jLabel1 = new JLabel();
			jLabel2 = new JLabel();
			coarse = new JLabel();
			fine = new JLabel();
			jPanel2 = new JPanel();
			lblStorageAddress = new JLabel();
			lblStorageSize = new JLabel();
			lblEndian = new JLabel();
			endian = new JLabel();
			storageSize = new JLabel();
			storageAddress = new JLabel();
			jPanel3 = new JPanel();
			jScrollPane1 = new JScrollPane();
			description = new JTextArea();
			jLabel5 = new JLabel();
			userLevel = new JLabel();
			SetAutoscrolls(true);
			SetFont(new Font("Tahoma", 0, 12));
			SetInheritsPopupMenu(true);
			lblTable.SetText("Table:");
			lblTable.SetFocusable(false);
			tableName.SetText("Tablename (3D)");
			tableName.SetFocusable(false);
			lblCategory.SetText("Category:");
			lblCategory.SetFocusable(false);
			category.SetText("Category");
			category.SetFocusable(false);
			jPanel1.SetBorder(BorderFactory.CreateTitledBorder(BorderFactory.CreateTitledBorder
				("Conversion")));
			lblUnit.SetText("Unit:");
			lblUnit.SetFocusable(false);
			unit.SetText("unit");
			unit.SetFocusable(false);
			lblByteToReal.SetText("Byte to Real:");
			lblByteToReal.SetFocusable(false);
			byteToReal.SetText("bytetoreal");
			byteToReal.SetFocusable(false);
			realToByte.SetText("realtobyte");
			realToByte.SetFocusable(false);
			lblRealToByte.SetText("Real to Byte:");
			lblRealToByte.SetFocusable(false);
			jLabel1.SetText("Coarse adjust:");
			jLabel2.SetText("Fine adjust:");
			coarse.SetText("coarse");
			fine.SetText("fine");
			GroupLayout jPanel1Layout = new GroupLayout(jPanel1);
			jPanel1.SetLayout(jPanel1Layout);
			jPanel1Layout.SetHorizontalGroup(jPanel1Layout.CreateParallelGroup(GroupLayout.LEADING
				).Add(jPanel1Layout.CreateSequentialGroup().AddContainerGap().Add(jPanel1Layout.
				CreateParallelGroup(GroupLayout.LEADING).Add(lblUnit).Add(lblByteToReal).Add(lblRealToByte
				).Add(jLabel1).Add(jLabel2)).AddPreferredGap(LayoutStyle.RELATED, 14, short.MaxValue
				).Add(jPanel1Layout.CreateParallelGroup(GroupLayout.LEADING).Add(jPanel1Layout.CreateSequentialGroup
				().Add(2, 2, 2).Add(unit)).Add(byteToReal).Add(realToByte).Add(coarse).Add(fine)
				).Add(27, 27, 27)));
			jPanel1Layout.SetVerticalGroup(jPanel1Layout.CreateParallelGroup(GroupLayout.LEADING
				).Add(jPanel1Layout.CreateSequentialGroup().Add(jPanel1Layout.CreateParallelGroup
				(GroupLayout.TRAILING).Add(jPanel1Layout.CreateSequentialGroup().Add(unit).AddPreferredGap
				(LayoutStyle.RELATED).Add(byteToReal).AddPreferredGap(LayoutStyle.RELATED).Add(realToByte
				)).Add(jPanel1Layout.CreateSequentialGroup().Add(lblUnit).AddPreferredGap(LayoutStyle
				.RELATED).Add(lblByteToReal).AddPreferredGap(LayoutStyle.RELATED).Add(lblRealToByte
				))).AddPreferredGap(LayoutStyle.RELATED).Add(jPanel1Layout.CreateParallelGroup(GroupLayout
				.BASELINE).Add(jLabel1).Add(coarse)).AddPreferredGap(LayoutStyle.RELATED).Add(jPanel1Layout
				.CreateParallelGroup(GroupLayout.BASELINE).Add(jLabel2).Add(fine))));
			jPanel2.SetBorder(BorderFactory.CreateTitledBorder("Storage"));
			lblStorageAddress.SetText("Storage Address:");
			lblStorageAddress.SetFocusable(false);
			lblStorageSize.SetText("Data Type:");
			lblStorageSize.SetFocusable(false);
			lblEndian.SetText("Endian:");
			lblEndian.SetFocusable(false);
			endian.SetText("little");
			endian.SetFocusable(false);
			storageSize.SetText("unkn");
			storageSize.SetFocusable(false);
			storageAddress.SetText("0x00");
			storageAddress.SetFocusable(false);
			GroupLayout jPanel2Layout = new GroupLayout(jPanel2);
			jPanel2.SetLayout(jPanel2Layout);
			jPanel2Layout.SetHorizontalGroup(jPanel2Layout.CreateParallelGroup(GroupLayout.LEADING
				).Add(jPanel2Layout.CreateSequentialGroup().AddContainerGap().Add(jPanel2Layout.
				CreateParallelGroup(GroupLayout.LEADING).Add(lblStorageAddress).Add(lblStorageSize
				).Add(lblEndian)).AddPreferredGap(LayoutStyle.RELATED).Add(jPanel2Layout.CreateParallelGroup
				(GroupLayout.LEADING).Add(endian).Add(storageSize).Add(storageAddress)).AddContainerGap
				(28, short.MaxValue)));
			jPanel2Layout.SetVerticalGroup(jPanel2Layout.CreateParallelGroup(GroupLayout.LEADING
				).Add(jPanel2Layout.CreateSequentialGroup().AddContainerGap().Add(jPanel2Layout.
				CreateParallelGroup(GroupLayout.BASELINE).Add(lblStorageSize).Add(storageSize)).
				AddPreferredGap(LayoutStyle.RELATED).Add(jPanel2Layout.CreateParallelGroup(GroupLayout
				.BASELINE).Add(lblStorageAddress).Add(storageAddress)).AddPreferredGap(LayoutStyle
				.RELATED).Add(jPanel2Layout.CreateParallelGroup(GroupLayout.BASELINE).Add(lblEndian
				).Add(endian)).AddContainerGap(37, short.MaxValue)));
			jPanel3.SetBorder(BorderFactory.CreateTitledBorder("Description"));
			jScrollPane1.SetBorder(null);
			description.SetBackground(new Color(236, 233, 216));
			description.SetColumns(20);
			description.SetEditable(false);
			description.SetFont(new Font("Tahoma", 0, 11));
			description.SetLineWrap(true);
			description.SetRows(5);
			description.SetText("Description");
			description.SetWrapStyleWord(true);
			description.SetBorder(null);
			description.SetOpaque(false);
			description.SetRequestFocusEnabled(false);
			jScrollPane1.SetViewportView(description);
			GroupLayout jPanel3Layout = new GroupLayout(jPanel3);
			jPanel3.SetLayout(jPanel3Layout);
			jPanel3Layout.SetHorizontalGroup(jPanel3Layout.CreateParallelGroup(GroupLayout.LEADING
				).Add(GroupLayout.TRAILING, jScrollPane1, GroupLayout.DEFAULT_SIZE, 360, short.MaxValue
				));
			jPanel3Layout.SetVerticalGroup(jPanel3Layout.CreateParallelGroup(GroupLayout.LEADING
				).Add(jPanel3Layout.CreateSequentialGroup().Add(jScrollPane1, GroupLayout.DEFAULT_SIZE
				, 102, short.MaxValue).AddContainerGap()));
			jLabel5.SetText("User Level:");
			userLevel.SetText("Beginner");
			GroupLayout layout = new GroupLayout(this);
			this.SetLayout(layout);
			layout.SetHorizontalGroup(layout.CreateParallelGroup(GroupLayout.LEADING).Add(layout
				.CreateSequentialGroup().AddContainerGap().Add(layout.CreateParallelGroup(GroupLayout
				.LEADING).Add(layout.CreateSequentialGroup().Add(jPanel1, GroupLayout.PREFERRED_SIZE
				, GroupLayout.DEFAULT_SIZE, GroupLayout.PREFERRED_SIZE).AddPreferredGap(LayoutStyle
				.RELATED).Add(jPanel2, GroupLayout.DEFAULT_SIZE, GroupLayout.DEFAULT_SIZE, short.MaxValue
				)).Add(layout.CreateSequentialGroup().Add(layout.CreateParallelGroup(GroupLayout
				.LEADING).Add(lblCategory).Add(lblTable)).AddPreferredGap(LayoutStyle.RELATED).Add
				(layout.CreateParallelGroup(GroupLayout.LEADING).Add(layout.CreateSequentialGroup
				().Add(category).Add(110, 110, 110).Add(jLabel5).AddPreferredGap(LayoutStyle.RELATED
				).Add(userLevel)).Add(tableName, GroupLayout.DEFAULT_SIZE, 321, short.MaxValue))
				).Add(jPanel3, GroupLayout.DEFAULT_SIZE, GroupLayout.DEFAULT_SIZE, short.MaxValue
				)).AddContainerGap()));
			layout.SetVerticalGroup(layout.CreateParallelGroup(GroupLayout.LEADING).Add(layout
				.CreateSequentialGroup().AddContainerGap().Add(layout.CreateParallelGroup(GroupLayout
				.BASELINE).Add(tableName).Add(lblTable)).AddPreferredGap(LayoutStyle.RELATED).Add
				(layout.CreateParallelGroup(GroupLayout.BASELINE).Add(lblCategory).Add(category)
				.Add(jLabel5).Add(userLevel)).AddPreferredGap(LayoutStyle.RELATED).Add(layout.CreateParallelGroup
				(GroupLayout.LEADING, false).Add(jPanel2, GroupLayout.DEFAULT_SIZE, GroupLayout.
				DEFAULT_SIZE, short.MaxValue).Add(jPanel1, GroupLayout.DEFAULT_SIZE, GroupLayout
				.DEFAULT_SIZE, short.MaxValue)).AddPreferredGap(LayoutStyle.RELATED).Add(jPanel3
				, GroupLayout.PREFERRED_SIZE, GroupLayout.DEFAULT_SIZE, GroupLayout.PREFERRED_SIZE
				).AddContainerGap(GroupLayout.DEFAULT_SIZE, short.MaxValue)));
		}

		private JLabel byteToReal;

		private JLabel category;

		private JLabel coarse;

		private JTextArea description;

		private JLabel endian;

		private JLabel fine;

		private JLabel jLabel1;

		private JLabel jLabel2;

		private JLabel jLabel5;

		private JPanel jPanel1;

		private JPanel jPanel2;

		private JPanel jPanel3;

		private JScrollPane jScrollPane1;

		private JLabel lblByteToReal;

		private JLabel lblCategory;

		private JLabel lblEndian;

		private JLabel lblRealToByte;

		private JLabel lblStorageAddress;

		private JLabel lblStorageSize;

		private JLabel lblTable;

		private JLabel lblUnit;

		private JLabel realToByte;

		private JLabel storageAddress;

		private JLabel storageSize;

		private JLabel tableName;

		private JLabel unit;

		private JLabel userLevel;
		// </editor-fold>//GEN-END:initComponents
		// Variables declaration - do not modify//GEN-BEGIN:variables
		// End of variables declaration//GEN-END:variables
	}
}

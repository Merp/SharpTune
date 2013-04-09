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
using Org.Jdesktop.Layout;
using RomRaider.Maps;
using Sharpen;

namespace RomRaider.Swing
{
	[System.Serializable]
	public class RomPropertyPanel : JPanel
	{
		private const long serialVersionUID = 5583360728106071942L;

		internal Rom rom = new Rom();

		public RomPropertyPanel(Rom rom)
		{
			InitComponents();
			// populate fields
			fileName.SetText(rom.GetFileName());
			xmlID.SetText(rom.GetRomID().GetXmlid());
			ecuVersion.SetText(rom.GetRomID().GetCaseId());
			fileSize.SetText((rom.GetRealFileSize() / 1024) + "kb");
			internalID.SetText(rom.GetRomID().GetInternalIdString());
			storageAddress.SetText("0x" + Sharpen.Extensions.ToHexString(rom.GetRomID().GetInternalIdAddress
				()));
			make.SetText(rom.GetRomID().GetMake());
			market.SetText(rom.GetRomID().GetMarket());
			year.SetText(rom.GetRomID().GetYear() + string.Empty);
			model.SetText(rom.GetRomID().GetModel());
			submodel.SetText(rom.GetRomID().GetSubModel());
			transmission.SetText(rom.GetRomID().GetTransmission());
			editStamp.SetText(rom.GetRomID().GetEditStamp());
			tableList.SetListData(rom.GetTables());
		}

		// <editor-fold defaultstate="collapsed" desc=" Generated Code ">//GEN-BEGIN:initComponents
		private void InitComponents()
		{
			lblFilename = new JLabel();
			fileName = new JLabel();
			lblECURevision = new JLabel();
			xmlID = new JLabel();
			lblFilesize = new JLabel();
			fileSize = new JLabel();
			lblEcuVersion = new JLabel();
			ecuVersion = new JLabel();
			lblInternalId = new JLabel();
			internalID = new JLabel();
			lblStorageAddress = new JLabel();
			storageAddress = new JLabel();
			lblMake = new JLabel();
			lblMarket = new JLabel();
			lblTransmission = new JLabel();
			lblModel = new JLabel();
			lblSubmodel = new JLabel();
			lblYear = new JLabel();
			make = new JLabel();
			market = new JLabel();
			year = new JLabel();
			model = new JLabel();
			submodel = new JLabel();
			transmission = new JLabel();
			jScrollPane1 = new JScrollPane();
			tableList = new JList();
			lblTables = new JLabel();
			lblEditStamp = new JLabel();
			editStamp = new JLabel();
			lblEditStamp.SetText("Edit Stamp:");
			editStamp.SetText("stamp");
			lblFilename.SetText("Filename:");
			fileName.SetText("Filename");
			lblECURevision.SetText("ECU Revision:");
			xmlID.SetText("XMLID");
			lblFilesize.SetText("Filesize:");
			fileSize.SetText("999kb");
			lblEcuVersion.SetText("ECU Version:");
			ecuVersion.SetText("ECUVER");
			lblInternalId.SetText("Internal ID:");
			internalID.SetText("INTERNAL");
			lblStorageAddress.SetText("ID Storage Address:");
			storageAddress.SetText("0x00");
			lblMake.SetText("Make:");
			lblMarket.SetText("Market:");
			lblTransmission.SetText("Transmission:");
			lblModel.SetText("Model:");
			lblSubmodel.SetText("Submodel:");
			lblYear.SetText("Year:");
			make.SetText("Make");
			market.SetText("Market");
			year.SetText("Year");
			model.SetText("Model");
			submodel.SetText("Submodel");
			transmission.SetText("Transmission");
			tableList.SetModel(new _AbstractListModel_135());
			jScrollPane1.SetViewportView(tableList);
			lblTables.SetText("Tables:");
			GroupLayout layout = new GroupLayout(this);
			this.SetLayout(layout);
			layout.SetHorizontalGroup(layout.CreateParallelGroup(GroupLayout.LEADING).Add(layout
				.CreateSequentialGroup().AddContainerGap().Add(layout.CreateParallelGroup(GroupLayout
				.LEADING).Add(layout.CreateSequentialGroup().Add(lblFilename).AddPreferredGap(LayoutStyle
				.RELATED).Add(fileName, GroupLayout.PREFERRED_SIZE, 302, GroupLayout.PREFERRED_SIZE
				)).Add(layout.CreateSequentialGroup().Add(layout.CreateParallelGroup(GroupLayout
				.LEADING).Add(layout.CreateSequentialGroup().Add(layout.CreateParallelGroup(GroupLayout
				.LEADING).Add(lblECURevision).Add(lblEcuVersion).Add(lblFilesize)).AddPreferredGap
				(LayoutStyle.RELATED).Add(layout.CreateParallelGroup(GroupLayout.LEADING).Add(fileSize
				).Add(ecuVersion).Add(xmlID))).Add(layout.CreateSequentialGroup().Add(layout.CreateParallelGroup
				(GroupLayout.LEADING).Add(lblYear).Add(lblModel).Add(lblSubmodel).Add(lblTransmission
				).Add(lblMarket).Add(lblMake)).Add(7, 7, 7).Add(layout.CreateParallelGroup(GroupLayout
				.LEADING).Add(make).Add(market).Add(year).Add(layout.CreateSequentialGroup().AddPreferredGap
				(LayoutStyle.RELATED).Add(layout.CreateParallelGroup(GroupLayout.LEADING).Add(transmission
				).Add(submodel))).Add(model)))).Add(32, 32, 32).Add(layout.CreateParallelGroup(GroupLayout
				.LEADING).Add(layout.CreateSequentialGroup().Add(layout.CreateParallelGroup(GroupLayout
				.LEADING).Add(lblInternalId).Add(lblStorageAddress).Add(lblEditStamp)).AddPreferredGap
				(LayoutStyle.RELATED, 53, short.MaxValue).Add(layout.CreateParallelGroup(GroupLayout
				.LEADING).Add(internalID).Add(storageAddress).Add(editStamp)).Add(36, 36, 36)).Add
				(lblTables).Add(jScrollPane1, GroupLayout.PREFERRED_SIZE, 226, GroupLayout.PREFERRED_SIZE
				)))).AddContainerGap()));
			layout.SetVerticalGroup(layout.CreateParallelGroup(GroupLayout.LEADING).Add(GroupLayout
				.TRAILING, layout.CreateSequentialGroup().Add(21, 21, 21).Add(layout.CreateParallelGroup
				(GroupLayout.LEADING).Add(layout.CreateParallelGroup(GroupLayout.BASELINE).Add(lblFilename
				).Add(fileName)).Add(layout.CreateSequentialGroup().Add(40, 40, 40).Add(layout.CreateParallelGroup
				(GroupLayout.BASELINE).Add(lblECURevision).Add(xmlID).Add(lblInternalId).Add(internalID
				)).AddPreferredGap(LayoutStyle.RELATED).Add(layout.CreateParallelGroup(GroupLayout
				.BASELINE).Add(ecuVersion).Add(lblEcuVersion).Add(storageAddress).Add(lblStorageAddress
				)).AddPreferredGap(LayoutStyle.RELATED).Add(layout.CreateParallelGroup(GroupLayout
				.BASELINE).Add(lblFilesize).Add(fileSize).Add(lblEditStamp).Add(editStamp)))).AddPreferredGap
				(LayoutStyle.RELATED).Add(lblTables).AddPreferredGap(LayoutStyle.RELATED).Add(layout
				.CreateParallelGroup(GroupLayout.LEADING).Add(layout.CreateSequentialGroup().Add
				(layout.CreateParallelGroup(GroupLayout.BASELINE).Add(lblMake).Add(make)).AddPreferredGap
				(LayoutStyle.RELATED).Add(layout.CreateParallelGroup(GroupLayout.BASELINE).Add(lblMarket
				).Add(market)).AddPreferredGap(LayoutStyle.RELATED).Add(layout.CreateParallelGroup
				(GroupLayout.BASELINE).Add(lblYear).Add(year)).AddPreferredGap(LayoutStyle.RELATED
				).Add(layout.CreateParallelGroup(GroupLayout.BASELINE).Add(lblModel).Add(model))
				.AddPreferredGap(LayoutStyle.RELATED).Add(layout.CreateParallelGroup(GroupLayout
				.BASELINE).Add(lblSubmodel).Add(submodel)).AddPreferredGap(LayoutStyle.RELATED).
				Add(layout.CreateParallelGroup(GroupLayout.BASELINE).Add(lblTransmission).Add(transmission
				))).Add(jScrollPane1, 0, 0, short.MaxValue)).AddContainerGap()));
		}

		private sealed class _AbstractListModel_135 : AbstractListModel
		{
			public _AbstractListModel_135()
			{
				this.serialVersionUID = -8498656966410761726L;
				this.strings = new string[] { "Item 1", "Item 2", "Item 3", "Item 4", "Item 5" };
			}

			private const long serialVersionUID;

			internal string[] strings;

			public override int GetSize()
			{
				return this.strings.Length;
			}

			public override object GetElementAt(int i)
			{
				return this.strings[i];
			}
		}

		private JLabel ecuVersion;

		private JLabel fileName;

		private JLabel fileSize;

		private JLabel internalID;

		private JScrollPane jScrollPane1;

		private JLabel lblECURevision;

		private JLabel lblEcuVersion;

		private JLabel lblFilename;

		private JLabel lblFilesize;

		private JLabel lblInternalId;

		private JLabel lblMake;

		private JLabel lblMarket;

		private JLabel lblModel;

		private JLabel lblStorageAddress;

		private JLabel lblSubmodel;

		private JLabel lblTables;

		private JLabel lblTransmission;

		private JLabel lblYear;

		private JLabel make;

		private JLabel market;

		private JLabel model;

		private JLabel storageAddress;

		private JLabel submodel;

		private JList tableList;

		private JLabel transmission;

		private JLabel xmlID;

		private JLabel year;

		private JLabel lblEditStamp;

		private JLabel editStamp;
		// </editor-fold>//GEN-END:initComponents
		// Variables declaration - do not modify//GEN-BEGIN:variables
		// End of variables declaration//GEN-END:variables
	}
}

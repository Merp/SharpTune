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
using Javax.Swing.Border;
using Org.Jdesktop.Layout;
using RomRaider;
using RomRaider.Editor.Ecu;
using RomRaider.Logger.Ecu;
using RomRaider.Swing;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Swing
{
	[System.Serializable]
	public class SettingsForm : JFrame, MouseListener
	{
		private const long serialVersionUID = 3910602424260147767L;

		internal Settings settings = ECUExec.settings;

		internal ECUEditor parentEditor;

		internal EcuLogger parentLogger;

		public static int MOVE_UP = 0;

		public static int MOVE_DOWN = 1;

		private Vector<string> ecuDefFileNames = new Vector<string>();

		/// <wbp.parser.constructor></wbp.parser.constructor>
		public SettingsForm(ECUEditor parent)
		{
			this.SetIconImage(parent.GetIconImage());
			this.parentEditor = parent;
			InitComponents();
			InitSettings();
			maxColor.AddMouseListener(this);
			minColor.AddMouseListener(this);
			highlightColor.AddMouseListener(this);
			axisColor.AddMouseListener(this);
			increaseColor.AddMouseListener(this);
			decreaseColor.AddMouseListener(this);
			warningColor.AddMouseListener(this);
			btnOk.AddMouseListener(this);
			btnApply.AddMouseListener(this);
			btnCancel.AddMouseListener(this);
			btnChooseFont.AddMouseListener(this);
			reset.AddMouseListener(this);
			btnAddAssocs.AddMouseListener(this);
			btnRemoveAssocs.AddMouseListener(this);
			tableClickCount.SetBackground(Color.WHITE);
			// disable file association buttons if user is not in Windows
			StringTokenizer osName = new StringTokenizer(Runtime.GetProperties().GetProperty(
				"os.name"));
			if (!Sharpen.Runtime.EqualsIgnoreCase(osName.NextToken(), "windows"))
			{
				btnAddAssocs.SetEnabled(false);
				btnRemoveAssocs.SetEnabled(false);
				extensionHex.SetEnabled(false);
				extensionBin.SetEnabled(false);
			}
		}

		public SettingsForm(EcuLogger parent)
		{
			this.SetIconImage(parent.GetIconImage());
			this.parentLogger = parent;
			InitComponents();
			InitSettings();
			maxColor.AddMouseListener(this);
			minColor.AddMouseListener(this);
			highlightColor.AddMouseListener(this);
			axisColor.AddMouseListener(this);
			increaseColor.AddMouseListener(this);
			decreaseColor.AddMouseListener(this);
			warningColor.AddMouseListener(this);
			btnOk.AddMouseListener(this);
			btnApply.AddMouseListener(this);
			btnCancel.AddMouseListener(this);
			btnChooseFont.AddMouseListener(this);
			reset.AddMouseListener(this);
			btnAddAssocs.AddMouseListener(this);
			btnRemoveAssocs.AddMouseListener(this);
			tableClickCount.SetBackground(Color.WHITE);
			// disable file association buttons if user is not in Windows
			StringTokenizer osName = new StringTokenizer(Runtime.GetProperties().GetProperty(
				"os.name"));
			if (!Sharpen.Runtime.EqualsIgnoreCase(osName.NextToken(), "windows"))
			{
				btnAddAssocs.SetEnabled(false);
				btnRemoveAssocs.SetEnabled(false);
				extensionHex.SetEnabled(false);
				extensionBin.SetEnabled(false);
			}
		}

		private void InitSettings()
		{
			obsoleteWarning.SetSelected(settings.IsObsoleteWarning());
			calcConflictWarning.SetSelected(settings.IsCalcConflictWarning());
			displayHighTables.SetSelected(settings.IsDisplayHighTables());
			saveDebugTables.SetSelected(settings.IsSaveDebugTables());
			debug.SetSelected(settings.IsDebug());
			maxColor.SetBackground(settings.GetMaxColor());
			minColor.SetBackground(settings.GetMinColor());
			highlightColor.SetBackground(settings.GetHighlightColor());
			axisColor.SetBackground(settings.GetAxisColor());
			increaseColor.SetBackground(settings.GetIncreaseBorder());
			decreaseColor.SetBackground(settings.GetDecreaseBorder());
			cellWidth.SetText(((int)settings.GetCellSize().GetWidth()) + string.Empty);
			cellHeight.SetText(((int)settings.GetCellSize().GetHeight()) + string.Empty);
			btnChooseFont.SetFont(settings.GetTableFont());
			btnChooseFont.SetText(settings.GetTableFont().GetFontName());
			if (settings.GetTableClickCount() == 1)
			{
				// single click opens table
				tableClickCount.SetSelectedIndex(0);
			}
			else
			{
				// double click opens table
				tableClickCount.SetSelectedIndex(1);
			}
			valueLimitWarning.SetSelected(settings.IsValueLimitWarning());
			warningColor.SetBackground(settings.GetWarningColor());
			if (Sharpen.Runtime.EqualsIgnoreCase(settings.GetTableClipboardFormat(), Settings
				.AIRBOYS_CLIPBOARD_FORMAT))
			{
				this.rdbtnAirboys.SetSelected(true);
			}
			else
			{
				if (Sharpen.Runtime.EqualsIgnoreCase(settings.GetTableClipboardFormat(), Settings
					.CUSTOM_CLIPBOARD_FORMAT))
				{
					this.rdbtnCustom.SetSelected(true);
				}
				else
				{
					this.rdbtnDefault.SetSelected(true);
				}
			}
			this.textFieldEditorIconScale.SetText(settings.GetEditorIconScale().ToString());
			this.textFieldTableIconScale.SetText(settings.GetTableIconScale().ToString());
			// add definitions to list
			Vector<FilePath> definitionFiles = settings.GetEcuDefinitionFiles();
			ecuDefFileNames = new Vector<string>();
			for (int i = 0; i < definitionFiles.Count; i++)
			{
				ecuDefFileNames.AddItem(definitionFiles[i].GetAbsolutePath());
			}
			UpdateEcuDefListModel();
		}

		// <editor-fold defaultstate="collapsed" desc=" Generated Code ">//GEN-BEGIN:initComponents
		private void InitComponents()
		{
			obsoleteWarning = new JCheckBox();
			calcConflictWarning = new JCheckBox();
			debug = new JCheckBox();
			btnCancel = new JButton();
			btnOk = new JButton();
			btnApply = new JButton();
			reset = new JButton();
			settingsTabbedPane = new JTabbedPane();
			jPanelClipboard = new JPanel();
			jPanelDefault = new JPanel();
			jPanelTableDisplay = new JPanel();
			jPanelIcons = new JPanel();
			jPanel2 = new JPanel();
			lblAxis = new JLabel();
			lblHighlight = new JLabel();
			lblMin = new JLabel();
			lblMax = new JLabel();
			maxColor = new JLabel();
			minColor = new JLabel();
			highlightColor = new JLabel();
			axisColor = new JLabel();
			warningColor = new JLabel();
			lblWarning = new JLabel();
			jPanel3 = new JPanel();
			lblIncrease = new JLabel();
			increaseColor = new JLabel();
			decreaseColor = new JLabel();
			lblDecrease = new JLabel();
			lblCellHeight = new JLabel();
			cellHeight = new JTextField();
			cellWidth = new JTextField();
			lblCellWidth = new JLabel();
			lblFont = new JLabel();
			btnChooseFont = new JButton();
			saveDebugTables = new JCheckBox();
			displayHighTables = new JCheckBox();
			valueLimitWarning = new JCheckBox();
			jPanel4 = new JPanel();
			extensionHex = new JCheckBox();
			extensionBin = new JCheckBox();
			btnAddAssocs = new JButton();
			btnRemoveAssocs = new JButton();
			jLabel1 = new JLabel();
			tableClickCount = new JComboBox();
			editorIconsPanel = new JPanel();
			tableIconsPanel = new JPanel();
			clipboardButtonGroup = new ButtonGroup();
			rdbtnDefault = new JRadioButton("RomRaider Default");
			rdbtnAirboys = new JRadioButton("Airboys Spreadsheet");
			rdbtnCustom = new JRadioButton("Custom (manually specify formats in settings.xml)"
				);
			clipboardButtonGroup.Add(this.rdbtnDefault);
			clipboardButtonGroup.Add(this.rdbtnAirboys);
			clipboardButtonGroup.Add(this.rdbtnCustom);
			SetDefaultCloseOperation(WindowConstants.DISPOSE_ON_CLOSE);
			SetTitle(Version.PRODUCT_NAME + " Settings");
			SetCursor(new Cursor(Cursor.DEFAULT_CURSOR));
			SetFont(new Font("Tahoma", 0, 11));
			obsoleteWarning.SetText("Warn me when opening out of date ECU image revision");
			obsoleteWarning.SetBorder(BorderFactory.CreateEmptyBorder(0, 0, 0, 0));
			obsoleteWarning.SetMargin(new Insets(0, 0, 0, 0));
			calcConflictWarning.SetText("Warn me when real and byte value calculations conflict"
				);
			calcConflictWarning.SetBorder(BorderFactory.CreateEmptyBorder(0, 0, 0, 0));
			calcConflictWarning.SetMargin(new Insets(0, 0, 0, 0));
			debug.SetText("Debug mode");
			debug.SetBorder(BorderFactory.CreateEmptyBorder(0, 0, 0, 0));
			debug.SetEnabled(false);
			debug.SetMargin(new Insets(0, 0, 0, 0));
			btnCancel.SetMnemonic('C');
			btnCancel.SetText("Cancel");
			btnOk.SetMnemonic('O');
			btnOk.SetText("OK");
			btnApply.SetMnemonic('A');
			btnApply.SetText("Apply");
			reset.SetText("Restore Defaults");
			jPanel2.SetBorder(BorderFactory.CreateTitledBorder("Background"));
			lblAxis.SetText("Axis Cell:");
			lblHighlight.SetText("Highlighted Cell:");
			lblMin.SetText("Minimum Value:");
			lblMax.SetText("Maximum Value:");
			maxColor.SetBackground(new Color(255, 0, 0));
			maxColor.SetBorder(BorderFactory.CreateLineBorder(new Color(0, 0, 0)));
			maxColor.SetOpaque(true);
			minColor.SetBackground(new Color(255, 0, 0));
			minColor.SetBorder(BorderFactory.CreateLineBorder(new Color(0, 0, 0)));
			minColor.SetOpaque(true);
			highlightColor.SetBackground(new Color(255, 0, 0));
			highlightColor.SetBorder(BorderFactory.CreateLineBorder(new Color(0, 0, 0)));
			highlightColor.SetOpaque(true);
			axisColor.SetBackground(new Color(255, 0, 0));
			axisColor.SetBorder(BorderFactory.CreateLineBorder(new Color(0, 0, 0)));
			axisColor.SetOpaque(true);
			warningColor.SetBackground(new Color(255, 0, 0));
			warningColor.SetBorder(BorderFactory.CreateLineBorder(new Color(0, 0, 0)));
			warningColor.SetOpaque(true);
			lblWarning.SetText("Warning:");
			GroupLayout jPanel2Layout = new GroupLayout(jPanel2);
			jPanel2.SetLayout(jPanel2Layout);
			jPanel2Layout.SetHorizontalGroup(jPanel2Layout.CreateParallelGroup(GroupLayout.LEADING
				).Add(GroupLayout.TRAILING, jPanel2Layout.CreateSequentialGroup().Add(jPanel2Layout
				.CreateParallelGroup(GroupLayout.TRAILING).Add(lblWarning).Add(jPanel2Layout.CreateParallelGroup
				(GroupLayout.LEADING).Add(jPanel2Layout.CreateSequentialGroup().Add(4, 4, 4).Add
				(lblMin)).Add(lblMax))).AddPreferredGap(LayoutStyle.RELATED).Add(jPanel2Layout.CreateParallelGroup
				(GroupLayout.LEADING).Add(jPanel2Layout.CreateSequentialGroup().Add(maxColor, GroupLayout
				.PREFERRED_SIZE, 50, GroupLayout.PREFERRED_SIZE).AddPreferredGap(LayoutStyle.RELATED
				, 22, short.MaxValue).Add(lblHighlight).AddPreferredGap(LayoutStyle.RELATED).Add
				(highlightColor, GroupLayout.PREFERRED_SIZE, 50, GroupLayout.PREFERRED_SIZE)).Add
				(jPanel2Layout.CreateSequentialGroup().Add(minColor, GroupLayout.PREFERRED_SIZE, 
				50, GroupLayout.PREFERRED_SIZE).AddPreferredGap(LayoutStyle.RELATED, 55, short.MaxValue
				).Add(lblAxis).AddPreferredGap(LayoutStyle.RELATED).Add(axisColor, GroupLayout.PREFERRED_SIZE
				, 50, GroupLayout.PREFERRED_SIZE)).Add(warningColor, GroupLayout.PREFERRED_SIZE, 
				50, GroupLayout.PREFERRED_SIZE)).AddContainerGap()));
			jPanel2Layout.SetVerticalGroup(jPanel2Layout.CreateParallelGroup(GroupLayout.LEADING
				).Add(jPanel2Layout.CreateSequentialGroup().Add(jPanel2Layout.CreateParallelGroup
				(GroupLayout.BASELINE).Add(lblMax).Add(maxColor, GroupLayout.PREFERRED_SIZE, 15, 
				GroupLayout.PREFERRED_SIZE).Add(highlightColor, GroupLayout.PREFERRED_SIZE, 15, 
				GroupLayout.PREFERRED_SIZE).Add(lblHighlight)).AddPreferredGap(LayoutStyle.RELATED
				).Add(jPanel2Layout.CreateParallelGroup(GroupLayout.BASELINE).Add(lblMin).Add(minColor
				, GroupLayout.PREFERRED_SIZE, 15, GroupLayout.PREFERRED_SIZE).Add(axisColor, GroupLayout
				.PREFERRED_SIZE, 15, GroupLayout.PREFERRED_SIZE).Add(lblAxis)).AddPreferredGap(LayoutStyle
				.RELATED).Add(jPanel2Layout.CreateParallelGroup(GroupLayout.BASELINE).Add(warningColor
				, GroupLayout.PREFERRED_SIZE, 15, GroupLayout.PREFERRED_SIZE).Add(lblWarning))));
			jPanel3.SetBorder(BorderFactory.CreateTitledBorder("Cell Borders"));
			lblIncrease.SetText("Increased:");
			increaseColor.SetBackground(new Color(255, 0, 0));
			increaseColor.SetBorder(BorderFactory.CreateLineBorder(new Color(0, 0, 0)));
			increaseColor.SetOpaque(true);
			decreaseColor.SetBackground(new Color(255, 0, 0));
			decreaseColor.SetBorder(BorderFactory.CreateLineBorder(new Color(0, 0, 0)));
			decreaseColor.SetOpaque(true);
			lblDecrease.SetText("Decreased:");
			GroupLayout jPanel3Layout = new GroupLayout(jPanel3);
			jPanel3.SetLayout(jPanel3Layout);
			jPanel3Layout.SetHorizontalGroup(jPanel3Layout.CreateParallelGroup(GroupLayout.LEADING
				).Add(GroupLayout.TRAILING, jPanel3Layout.CreateSequentialGroup().AddContainerGap
				().Add(lblIncrease).AddPreferredGap(LayoutStyle.RELATED).Add(increaseColor, GroupLayout
				.PREFERRED_SIZE, 50, GroupLayout.PREFERRED_SIZE).AddPreferredGap(LayoutStyle.RELATED
				, 59, short.MaxValue).Add(lblDecrease).AddPreferredGap(LayoutStyle.RELATED).Add(
				decreaseColor, GroupLayout.PREFERRED_SIZE, 50, GroupLayout.PREFERRED_SIZE).AddContainerGap
				()));
			jPanel3Layout.SetVerticalGroup(jPanel3Layout.CreateParallelGroup(GroupLayout.LEADING
				).Add(jPanel3Layout.CreateParallelGroup(GroupLayout.BASELINE).Add(decreaseColor, 
				GroupLayout.PREFERRED_SIZE, 15, GroupLayout.PREFERRED_SIZE).Add(lblDecrease).Add
				(lblIncrease).Add(increaseColor, GroupLayout.PREFERRED_SIZE, 15, GroupLayout.PREFERRED_SIZE
				)));
			lblCellHeight.SetText("Cell Height:");
			lblCellWidth.SetText("Cell Width:");
			lblFont.SetText("Font:");
			btnChooseFont.SetText("Choose");
			saveDebugTables.SetText("Save changes made on tables in debug mode");
			saveDebugTables.SetBorder(BorderFactory.CreateEmptyBorder(0, 0, 0, 0));
			saveDebugTables.SetMargin(new Insets(0, 0, 0, 0));
			displayHighTables.SetText("List tables that are above my userlevel");
			displayHighTables.SetBorder(BorderFactory.CreateEmptyBorder(0, 0, 0, 0));
			displayHighTables.SetMargin(new Insets(0, 0, 0, 0));
			valueLimitWarning.SetText("Warn when values exceed limits");
			valueLimitWarning.SetBorder(BorderFactory.CreateEmptyBorder(0, 0, 0, 0));
			valueLimitWarning.SetMargin(new Insets(0, 0, 0, 0));
			jPanel4.SetBorder(BorderFactory.CreateTitledBorder("File Associations"));
			extensionHex.SetText("HEX");
			extensionHex.SetBorder(BorderFactory.CreateEmptyBorder(0, 0, 0, 0));
			extensionHex.SetMargin(new Insets(0, 0, 0, 0));
			extensionBin.SetText("BIN");
			extensionBin.SetBorder(BorderFactory.CreateEmptyBorder(0, 0, 0, 0));
			extensionBin.SetMargin(new Insets(0, 0, 0, 0));
			btnAddAssocs.SetText("Add Associations");
			btnRemoveAssocs.SetText("Remove Associations");
			GroupLayout jPanel4Layout = new GroupLayout(jPanel4);
			jPanel4.SetLayout(jPanel4Layout);
			jPanel4Layout.SetHorizontalGroup(jPanel4Layout.CreateParallelGroup(GroupLayout.LEADING
				).Add(GroupLayout.TRAILING, jPanel4Layout.CreateSequentialGroup().AddContainerGap
				().Add(jPanel4Layout.CreateParallelGroup(GroupLayout.LEADING).Add(extensionBin).
				Add(extensionHex)).AddPreferredGap(LayoutStyle.RELATED, 93, short.MaxValue).Add(
				jPanel4Layout.CreateParallelGroup(GroupLayout.LEADING, false).Add(btnAddAssocs, 
				GroupLayout.DEFAULT_SIZE, GroupLayout.DEFAULT_SIZE, short.MaxValue).Add(btnRemoveAssocs
				)).Add(25, 25, 25)));
			jPanel4Layout.SetVerticalGroup(jPanel4Layout.CreateParallelGroup(GroupLayout.LEADING
				).Add(jPanel4Layout.CreateSequentialGroup().Add(jPanel4Layout.CreateParallelGroup
				(GroupLayout.BASELINE).Add(btnAddAssocs).Add(extensionHex)).AddPreferredGap(LayoutStyle
				.RELATED).Add(jPanel4Layout.CreateParallelGroup(GroupLayout.BASELINE).Add(btnRemoveAssocs
				).Add(extensionBin))));
			jLabel1.SetText("click to open tables");
			tableClickCount.SetModel(new DefaultComboBoxModel(new string[] { "Single", "Double"
				 }));
			InitTabs();
			settingsTabbedPane.AddTab("General", jPanelDefault);
			JPanel jPanelDefs = new JPanel();
			settingsTabbedPane.AddTab("Definitions", null, jPanelDefs, null);
			JPanel panel_1 = new JPanel();
			panel_1.SetBorder(new TitledBorder(UIManager.GetBorder("TitledBorder.border"), "Git Settings"
				, TitledBorder.LEADING, TitledBorder.TOP, null, null));
			JLabel lblRepositoryUrl = new JLabel("Remote URL:");
			textFieldGitRepo = new JTextField(settings.GetGitCurrentRemoteUrl());
			textFieldGitRepo.SetToolTipText("The percentage of the icons original size.");
			textFieldGitRepo.SetColumns(10);
			JLabel lblBranch = new JLabel("Branch:");
			comboBoxGitBranch = new JComboBox();
			comboBoxGitBranch.SetEditable(true);
			UpdateComboBoxGitBranch();
			btnAddRemote = new JButton("Add + Fetch Remote");
			btnAddRemote.AddMouseListener(this);
			btnFetchResetGit = new JButton("Fetch + Checkout Branch");
			btnFetchResetGit.AddMouseListener(this);
			lblRemoteName = new JLabel("Remote Name:");
			textFieldRemoteName = new JTextField(settings.GetGitCurrentRemoteName());
			textFieldRemoteName.SetToolTipText("Enter a short name for this remote repository"
				);
			textFieldRemoteName.SetColumns(10);
			GroupLayout gl_panel_1 = new GroupLayout(panel_1);
			gl_panel_1.SetHorizontalGroup(gl_panel_1.CreateParallelGroup(GroupLayout.Alignment
				.LEADING).AddGroup(GroupLayout.Alignment.TRAILING, ((GroupLayout.SequentialGroup
				)((GroupLayout.SequentialGroup)gl_panel_1.CreateSequentialGroup().AddContainerGap
				().AddGroup(((GroupLayout.ParallelGroup)((GroupLayout.ParallelGroup)gl_panel_1.CreateParallelGroup
				(GroupLayout.Alignment.TRAILING).AddComponent(btnFetchResetGit)).AddGroup(((GroupLayout.SequentialGroup
				)((GroupLayout.SequentialGroup)gl_panel_1.CreateSequentialGroup().AddGroup(((GroupLayout.ParallelGroup
				)((GroupLayout.ParallelGroup)((GroupLayout.ParallelGroup)gl_panel_1.CreateParallelGroup
				(GroupLayout.Alignment.TRAILING).AddGroup(((GroupLayout.SequentialGroup)((GroupLayout.SequentialGroup
				)gl_panel_1.CreateSequentialGroup().AddComponent(lblRemoteName)).AddGap(6)))).AddGroup
				(((GroupLayout.SequentialGroup)gl_panel_1.CreateSequentialGroup().AddComponent(lblBranch
				, GroupLayout.PREFERRED_SIZE, 43, GroupLayout.PREFERRED_SIZE)).AddPreferredGap(LayoutStyle.ComponentPlacement
				.RELATED))).AddGroup(((GroupLayout.SequentialGroup)gl_panel_1.CreateSequentialGroup
				().AddComponent(lblRepositoryUrl)).AddPreferredGap(LayoutStyle.ComponentPlacement
				.RELATED))))).AddGroup(((GroupLayout.ParallelGroup)((GroupLayout.ParallelGroup)gl_panel_1
				.CreateParallelGroup(GroupLayout.Alignment.TRAILING).AddComponent(textFieldGitRepo
				, GroupLayout.PREFERRED_SIZE, 300, GroupLayout.PREFERRED_SIZE)).AddGroup(((GroupLayout.ParallelGroup
				)gl_panel_1.CreateParallelGroup(GroupLayout.Alignment.TRAILING).AddComponent(comboBoxGitBranch
				, GroupLayout.Alignment.LEADING, 0, 300, short.MaxValue).AddGroup(((GroupLayout.SequentialGroup
				)((GroupLayout.SequentialGroup)gl_panel_1.CreateSequentialGroup().AddComponent(textFieldRemoteName
				, GroupLayout.DEFAULT_SIZE, 157, short.MaxValue)).AddPreferredGap(LayoutStyle.ComponentPlacement
				.RELATED).AddComponent(btnAddRemote)))))))))))).AddGap(285))));
			gl_panel_1.SetVerticalGroup(((GroupLayout.ParallelGroup)gl_panel_1.CreateParallelGroup
				(GroupLayout.Alignment.LEADING).AddGroup(((GroupLayout.SequentialGroup)((GroupLayout.SequentialGroup
				)((GroupLayout.SequentialGroup)((GroupLayout.SequentialGroup)((GroupLayout.SequentialGroup
				)gl_panel_1.CreateSequentialGroup().AddGroup(((GroupLayout.ParallelGroup)((GroupLayout.ParallelGroup
				)gl_panel_1.CreateParallelGroup(GroupLayout.Alignment.BASELINE).AddComponent(textFieldGitRepo
				, GroupLayout.PREFERRED_SIZE, GroupLayout.DEFAULT_SIZE, GroupLayout.PREFERRED_SIZE
				)).AddComponent(lblRepositoryUrl)))).AddPreferredGap(LayoutStyle.ComponentPlacement
				.UNRELATED).AddGroup(((GroupLayout.ParallelGroup)((GroupLayout.ParallelGroup)((GroupLayout.ParallelGroup
				)gl_panel_1.CreateParallelGroup(GroupLayout.Alignment.BASELINE).AddComponent(textFieldRemoteName
				, GroupLayout.PREFERRED_SIZE, GroupLayout.DEFAULT_SIZE, GroupLayout.PREFERRED_SIZE
				)).AddComponent(lblRemoteName)).AddComponent(btnAddRemote)))).AddPreferredGap(LayoutStyle.ComponentPlacement
				.UNRELATED).AddGroup(((GroupLayout.ParallelGroup)((GroupLayout.ParallelGroup)gl_panel_1
				.CreateParallelGroup(GroupLayout.Alignment.BASELINE).AddComponent(comboBoxGitBranch
				, GroupLayout.PREFERRED_SIZE, GroupLayout.DEFAULT_SIZE, GroupLayout.PREFERRED_SIZE
				)).AddComponent(lblBranch)))).AddPreferredGap(LayoutStyle.ComponentPlacement.UNRELATED
				).AddComponent(btnFetchResetGit)).AddGap(57)))));
			panel_1.SetLayout(gl_panel_1);
			JPanel panel_2 = new JPanel();
			panel_2.SetBorder(new TitledBorder(UIManager.GetBorder("TitledBorder.border"), "ECU Definition File Priority"
				, TitledBorder.LEADING, TitledBorder.TOP, null, null));
			buttonEcuDefMoveUp = new JButton();
			buttonEcuDefMoveUp.AddMouseListener(new _MouseAdapter_568(this));
			buttonEcuDefMoveUp.SetText("Move Up");
			buttonEcuDefAdd = new JButton();
			buttonEcuDefAdd.AddMouseListener(new _MouseAdapter_577(this));
			buttonEcuDefAdd.SetText("Add...");
			buttonEcuDefRemove = new JButton();
			buttonEcuDefRemove.AddMouseListener(new _MouseAdapter_586(this));
			buttonEcuDefRemove.SetText("Remove");
			buttonEcuDefMoveDown = new JButton();
			buttonEcuDefMoveDown.AddMouseListener(new _MouseAdapter_595(this));
			buttonEcuDefMoveDown.SetText("Move Down");
			JScrollPane scrollPane = new JScrollPane();
			GroupLayout gl_panel_2 = new GroupLayout(panel_2);
			gl_panel_2.SetHorizontalGroup(gl_panel_2.CreateParallelGroup(GroupLayout.Alignment
				.TRAILING).AddGroup(GroupLayout.Alignment.LEADING, ((GroupLayout.SequentialGroup
				)gl_panel_2.CreateSequentialGroup().AddContainerGap().AddGroup(((GroupLayout.ParallelGroup
				)gl_panel_2.CreateParallelGroup(GroupLayout.Alignment.LEADING).AddComponent(scrollPane
				, GroupLayout.Alignment.TRAILING, GroupLayout.DEFAULT_SIZE, 375, short.MaxValue)
				.AddGroup(((GroupLayout.SequentialGroup)((GroupLayout.SequentialGroup)((GroupLayout.SequentialGroup
				)((GroupLayout.SequentialGroup)((GroupLayout.SequentialGroup)gl_panel_2.CreateSequentialGroup
				().AddComponent(buttonEcuDefMoveDown)).AddGap(6)).AddComponent(buttonEcuDefMoveUp
				)).AddPreferredGap(LayoutStyle.ComponentPlacement.RELATED, 61, short.MaxValue).AddComponent
				(buttonEcuDefAdd)).AddPreferredGap(LayoutStyle.ComponentPlacement.UNRELATED).AddComponent
				(buttonEcuDefRemove)))))).AddContainerGap()));
			gl_panel_2.SetVerticalGroup(gl_panel_2.CreateParallelGroup(GroupLayout.Alignment.
				LEADING).AddGroup(GroupLayout.Alignment.TRAILING, ((GroupLayout.SequentialGroup)
				((GroupLayout.SequentialGroup)((GroupLayout.SequentialGroup)gl_panel_2.CreateSequentialGroup
				().AddComponent(scrollPane, GroupLayout.DEFAULT_SIZE, 178, short.MaxValue)).AddPreferredGap
				(LayoutStyle.ComponentPlacement.RELATED).AddGroup(((GroupLayout.ParallelGroup)((
				GroupLayout.ParallelGroup)((GroupLayout.ParallelGroup)gl_panel_2.CreateParallelGroup
				(GroupLayout.Alignment.LEADING).AddComponent(buttonEcuDefMoveDown)).AddComponent
				(buttonEcuDefMoveUp)).AddGroup(((GroupLayout.ParallelGroup)((GroupLayout.ParallelGroup
				)gl_panel_2.CreateParallelGroup(GroupLayout.Alignment.BASELINE).AddComponent(buttonEcuDefRemove
				)).AddComponent(buttonEcuDefAdd)))))).AddGap(6))));
			ecuDefinitionList = new JList();
			scrollPane.SetViewportView(ecuDefinitionList);
			ecuDefinitionList.SetSelectionMode(ListSelectionModel.SINGLE_SELECTION);
			ecuDefinitionList.SetBorder(new BevelBorder(BevelBorder.LOWERED, null, null, null
				, null));
			panel_2.SetLayout(gl_panel_2);
			panel = new JPanel();
			panel.SetBorder(new TitledBorder(UIManager.GetBorder("TitledBorder.border"), "Logger Definition Settings"
				, TitledBorder.LEADING, TitledBorder.TOP, null, null));
			lblFilepath = new JLabel("FilePath:");
			comboBoxLoggerDef = new JComboBox(Sharpen.Collections.ToArray(settings.GetAvailableLoggerDefs
				().Keys));
			comboBoxLoggerDef.SetSelectedItem(new FilePath(settings.GetLoggerDefFilePath()).GetName
				());
			comboBoxLoggerDef.SetEditable(true);
			btnLoggerDefChooseFile = new JButton("Choose External File...");
			btnLoggerDefChooseFile.AddMouseListener(this);
			GroupLayout gl_panel = new GroupLayout(panel);
			gl_panel.SetHorizontalGroup(((GroupLayout.ParallelGroup)gl_panel.CreateParallelGroup
				(GroupLayout.Alignment.TRAILING).AddGroup(((GroupLayout.SequentialGroup)((GroupLayout.SequentialGroup
				)gl_panel.CreateSequentialGroup().AddContainerGap(31, short.MaxValue).AddGroup((
				(GroupLayout.ParallelGroup)((GroupLayout.ParallelGroup)gl_panel.CreateParallelGroup
				(GroupLayout.Alignment.TRAILING).AddComponent(btnLoggerDefChooseFile)).AddGroup(
				((GroupLayout.SequentialGroup)((GroupLayout.SequentialGroup)gl_panel.CreateSequentialGroup
				().AddComponent(lblFilepath)).AddPreferredGap(LayoutStyle.ComponentPlacement.UNRELATED
				).AddComponent(comboBoxLoggerDef, GroupLayout.PREFERRED_SIZE, 288, GroupLayout.PREFERRED_SIZE
				)))))).AddGap(24)))));
			gl_panel.SetVerticalGroup(((GroupLayout.ParallelGroup)gl_panel.CreateParallelGroup
				(GroupLayout.Alignment.LEADING).AddGroup(((GroupLayout.SequentialGroup)((GroupLayout.SequentialGroup
				)gl_panel.CreateSequentialGroup().AddContainerGap().AddGroup(((GroupLayout.ParallelGroup
				)((GroupLayout.ParallelGroup)gl_panel.CreateParallelGroup(GroupLayout.Alignment.
				BASELINE).AddComponent(lblFilepath)).AddComponent(comboBoxLoggerDef, GroupLayout
				.PREFERRED_SIZE, GroupLayout.DEFAULT_SIZE, GroupLayout.PREFERRED_SIZE)))).AddPreferredGap
				(LayoutStyle.ComponentPlacement.RELATED, 11, short.MaxValue).AddComponent(btnLoggerDefChooseFile
				)).AddContainerGap())));
			panel.SetLayout(gl_panel);
			GroupLayout gl_jPanelDefs = new GroupLayout(jPanelDefs);
			gl_jPanelDefs.SetHorizontalGroup(gl_jPanelDefs.CreateParallelGroup(GroupLayout.Alignment
				.LEADING).AddGroup(GroupLayout.Alignment.TRAILING, ((GroupLayout.SequentialGroup
				)gl_jPanelDefs.CreateSequentialGroup().AddContainerGap().AddGroup(((GroupLayout.ParallelGroup
				)gl_jPanelDefs.CreateParallelGroup(GroupLayout.Alignment.TRAILING).AddComponent(
				panel_2, GroupLayout.Alignment.LEADING, GroupLayout.DEFAULT_SIZE, 407, short.MaxValue
				).AddComponent(panel_1, GroupLayout.Alignment.LEADING, GroupLayout.PREFERRED_SIZE
				, 407, short.MaxValue).AddComponent(panel, GroupLayout.PREFERRED_SIZE, 407, GroupLayout
				.PREFERRED_SIZE)))).AddContainerGap()));
			gl_jPanelDefs.SetVerticalGroup(((GroupLayout.ParallelGroup)gl_jPanelDefs.CreateParallelGroup
				(GroupLayout.Alignment.TRAILING).AddGroup(((GroupLayout.SequentialGroup)((GroupLayout.SequentialGroup
				)((GroupLayout.SequentialGroup)gl_jPanelDefs.CreateSequentialGroup().AddContainerGap
				().AddComponent(panel_1, GroupLayout.PREFERRED_SIZE, 145, GroupLayout.PREFERRED_SIZE
				)).AddPreferredGap(LayoutStyle.ComponentPlacement.RELATED).AddComponent(panel_2, 
				GroupLayout.DEFAULT_SIZE, 236, short.MaxValue)).AddPreferredGap(LayoutStyle.ComponentPlacement
				.RELATED).AddComponent(panel, GroupLayout.PREFERRED_SIZE, 99, GroupLayout.PREFERRED_SIZE
				)).AddContainerGap())));
			jPanelDefs.SetLayout(gl_jPanelDefs);
			settingsTabbedPane.AddTab("Table Display", jPanelTableDisplay);
			settingsTabbedPane.AddTab("Clipboard", jPanelClipboard);
			settingsTabbedPane.AddTab("Icons", jPanelIcons);
			editorIconsPanel = new JPanel();
			// Content Pane Layout
			GroupLayout layout = new GroupLayout(GetContentPane());
			layout.SetHorizontalGroup(((GroupLayout.ParallelGroup)layout.CreateParallelGroup(
				GroupLayout.Alignment.LEADING).AddGroup(((GroupLayout.SequentialGroup)layout.CreateSequentialGroup
				().AddContainerGap().AddGroup(((GroupLayout.ParallelGroup)layout.CreateParallelGroup
				(GroupLayout.Alignment.LEADING).AddComponent(settingsTabbedPane, GroupLayout.Alignment
				.TRAILING, GroupLayout.PREFERRED_SIZE, 432, short.MaxValue).AddGroup(((GroupLayout.SequentialGroup
				)((GroupLayout.SequentialGroup)((GroupLayout.SequentialGroup)((GroupLayout.SequentialGroup
				)layout.CreateSequentialGroup().AddComponent(reset)).AddPreferredGap(LayoutStyle.ComponentPlacement
				.RELATED, 136, short.MaxValue).AddComponent(btnApply)).AddPreferredGap(LayoutStyle.ComponentPlacement
				.RELATED).AddComponent(btnOk)).AddPreferredGap(LayoutStyle.ComponentPlacement.RELATED
				).AddComponent(btnCancel)))))).AddContainerGap())));
			layout.SetVerticalGroup(((GroupLayout.ParallelGroup)layout.CreateParallelGroup(GroupLayout.Alignment
				.LEADING).AddGroup(((GroupLayout.SequentialGroup)((GroupLayout.SequentialGroup)layout
				.CreateSequentialGroup().AddContainerGap().AddComponent(settingsTabbedPane, GroupLayout
				.PREFERRED_SIZE, 542, GroupLayout.PREFERRED_SIZE)).AddPreferredGap(LayoutStyle.ComponentPlacement
				.RELATED).AddGroup(((GroupLayout.ParallelGroup)((GroupLayout.ParallelGroup)((GroupLayout.ParallelGroup
				)((GroupLayout.ParallelGroup)layout.CreateParallelGroup(GroupLayout.Alignment.BASELINE
				).AddComponent(btnCancel)).AddComponent(btnOk)).AddComponent(btnApply)).AddComponent
				(reset)))).AddContainerGap())));
			GetContentPane().SetLayout(layout);
			Pack();
		}

		private sealed class _MouseAdapter_568 : MouseAdapter
		{
			public _MouseAdapter_568(SettingsForm _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public override void MouseClicked(MouseEvent e)
			{
				this._enclosing.MoveSelection(RomRaider.Swing.SettingsForm.MOVE_UP);
			}

			private readonly SettingsForm _enclosing;
		}

		private sealed class _MouseAdapter_577 : MouseAdapter
		{
			public _MouseAdapter_577(SettingsForm _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public override void MouseClicked(MouseEvent e)
			{
				this._enclosing.AddEcuDefFile();
			}

			private readonly SettingsForm _enclosing;
		}

		private sealed class _MouseAdapter_586 : MouseAdapter
		{
			public _MouseAdapter_586(SettingsForm _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public override void MouseClicked(MouseEvent e)
			{
				this._enclosing.RemoveSelection();
			}

			private readonly SettingsForm _enclosing;
		}

		private sealed class _MouseAdapter_595 : MouseAdapter
		{
			public _MouseAdapter_595(SettingsForm _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public override void MouseClicked(MouseEvent e)
			{
				this._enclosing.MoveSelection(RomRaider.Swing.SettingsForm.MOVE_DOWN);
			}

			private readonly SettingsForm _enclosing;
		}

		// </editor-fold>//GEN-END:initComponents
		private void InitTabs()
		{
			// Init Default Tab Panel
			GroupLayout jPanelDefaultLayout = new GroupLayout(jPanelDefault);
			jPanelDefaultLayout.SetVerticalGroup(((GroupLayout.ParallelGroup)jPanelDefaultLayout
				.CreateParallelGroup(GroupLayout.Alignment.LEADING).AddGroup(((GroupLayout.SequentialGroup
				)((GroupLayout.SequentialGroup)((GroupLayout.SequentialGroup)((GroupLayout.SequentialGroup
				)((GroupLayout.SequentialGroup)jPanelDefaultLayout.CreateSequentialGroup().AddContainerGap
				().AddGroup(((GroupLayout.ParallelGroup)((GroupLayout.ParallelGroup)jPanelDefaultLayout
				.CreateParallelGroup(GroupLayout.Alignment.BASELINE).AddComponent(jLabel1)).AddComponent
				(tableClickCount, GroupLayout.PREFERRED_SIZE, 18, GroupLayout.PREFERRED_SIZE))))
				.AddPreferredGap(LayoutStyle.ComponentPlacement.RELATED).AddComponent(obsoleteWarning
				)).AddPreferredGap(LayoutStyle.ComponentPlacement.RELATED).AddComponent(calcConflictWarning
				)).AddPreferredGap(LayoutStyle.ComponentPlacement.RELATED).AddComponent(debug)).
				AddGap(17)).AddContainerGap(GroupLayout.DEFAULT_SIZE, short.MaxValue))));
			jPanelDefaultLayout.SetHorizontalGroup(jPanelDefaultLayout.CreateParallelGroup(GroupLayout.Alignment
				.TRAILING).AddGroup(GroupLayout.Alignment.LEADING, ((GroupLayout.SequentialGroup
				)jPanelDefaultLayout.CreateSequentialGroup().AddContainerGap().AddGroup(((GroupLayout.ParallelGroup
				)jPanelDefaultLayout.CreateParallelGroup(GroupLayout.Alignment.LEADING).AddGroup
				(((GroupLayout.SequentialGroup)jPanelDefaultLayout.CreateSequentialGroup().AddGroup
				(((GroupLayout.ParallelGroup)((GroupLayout.ParallelGroup)((GroupLayout.ParallelGroup
				)((GroupLayout.ParallelGroup)jPanelDefaultLayout.CreateParallelGroup(GroupLayout.Alignment
				.LEADING).AddComponent(calcConflictWarning)).AddComponent(obsoleteWarning)).AddGroup
				(((GroupLayout.SequentialGroup)((GroupLayout.SequentialGroup)jPanelDefaultLayout
				.CreateSequentialGroup().AddComponent(tableClickCount, GroupLayout.PREFERRED_SIZE
				, GroupLayout.DEFAULT_SIZE, GroupLayout.PREFERRED_SIZE)).AddPreferredGap(LayoutStyle.ComponentPlacement
				.RELATED).AddComponent(jLabel1)))).AddComponent(debug)))).AddContainerGap(45, short.MaxValue
				))))).AddContainerGap()));
			jPanelDefault.SetLayout(jPanelDefaultLayout);
			// Init Table Display Tab
			GroupLayout jPanelTableDisplayLayout = new GroupLayout(jPanelTableDisplay);
			jPanelTableDisplayLayout.SetHorizontalGroup(((GroupLayout.ParallelGroup)((GroupLayout.ParallelGroup
				)jPanelTableDisplayLayout.CreateParallelGroup(GroupLayout.Alignment.TRAILING).AddGroup
				(((GroupLayout.SequentialGroup)((GroupLayout.SequentialGroup)jPanelTableDisplayLayout
				.CreateSequentialGroup().AddContainerGap().AddGroup(((GroupLayout.ParallelGroup)
				((GroupLayout.ParallelGroup)jPanelTableDisplayLayout.CreateParallelGroup(GroupLayout.Alignment
				.TRAILING).AddGroup(((GroupLayout.ParallelGroup)((GroupLayout.ParallelGroup)((GroupLayout.ParallelGroup
				)jPanelTableDisplayLayout.CreateParallelGroup(GroupLayout.Alignment.LEADING).AddComponent
				(saveDebugTables)).AddComponent(displayHighTables)).AddComponent(valueLimitWarning
				)))).AddGroup(((GroupLayout.SequentialGroup)((GroupLayout.SequentialGroup)jPanelTableDisplayLayout
				.CreateSequentialGroup().AddGroup(((GroupLayout.ParallelGroup)((GroupLayout.ParallelGroup
				)jPanelTableDisplayLayout.CreateParallelGroup(GroupLayout.Alignment.LEADING).AddComponent
				(lblCellHeight)).AddComponent(lblFont)))).AddPreferredGap(LayoutStyle.ComponentPlacement
				.RELATED).AddGroup(((GroupLayout.ParallelGroup)((GroupLayout.ParallelGroup)jPanelTableDisplayLayout
				.CreateParallelGroup(GroupLayout.Alignment.LEADING).AddComponent(btnChooseFont))
				.AddGroup(((GroupLayout.SequentialGroup)((GroupLayout.SequentialGroup)((GroupLayout.SequentialGroup
				)jPanelTableDisplayLayout.CreateSequentialGroup().AddComponent(cellHeight, GroupLayout
				.PREFERRED_SIZE, 50, GroupLayout.PREFERRED_SIZE)).AddPreferredGap(LayoutStyle.ComponentPlacement
				.RELATED, 139, short.MaxValue).AddComponent(lblCellWidth)).AddPreferredGap(LayoutStyle.ComponentPlacement
				.RELATED).AddComponent(cellWidth, GroupLayout.PREFERRED_SIZE, 50, GroupLayout.PREFERRED_SIZE
				)))))))))).AddGap(47)))).AddGroup(GroupLayout.Alignment.LEADING, ((GroupLayout.SequentialGroup
				)jPanelTableDisplayLayout.CreateSequentialGroup().AddComponent(jPanel4, GroupLayout
				.DEFAULT_SIZE, 411, short.MaxValue)).AddContainerGap()).AddGroup(GroupLayout.Alignment
				.LEADING, ((GroupLayout.SequentialGroup)jPanelTableDisplayLayout.CreateSequentialGroup
				().AddComponent(jPanel3, GroupLayout.DEFAULT_SIZE, 411, short.MaxValue)).AddContainerGap
				()).AddComponent(jPanel2, GroupLayout.DEFAULT_SIZE, 411, short.MaxValue)));
			jPanelTableDisplayLayout.SetVerticalGroup(((GroupLayout.ParallelGroup)jPanelTableDisplayLayout
				.CreateParallelGroup(GroupLayout.Alignment.LEADING).AddGroup(((GroupLayout.SequentialGroup
				)((GroupLayout.SequentialGroup)((GroupLayout.SequentialGroup)((GroupLayout.SequentialGroup
				)((GroupLayout.SequentialGroup)((GroupLayout.SequentialGroup)((GroupLayout.SequentialGroup
				)((GroupLayout.SequentialGroup)((GroupLayout.SequentialGroup)jPanelTableDisplayLayout
				.CreateSequentialGroup().AddContainerGap().AddComponent(jPanel2, GroupLayout.PREFERRED_SIZE
				, 85, GroupLayout.PREFERRED_SIZE)).AddPreferredGap(LayoutStyle.ComponentPlacement
				.RELATED).AddComponent(jPanel3, GroupLayout.PREFERRED_SIZE, GroupLayout.DEFAULT_SIZE
				, GroupLayout.PREFERRED_SIZE)).AddPreferredGap(LayoutStyle.ComponentPlacement.RELATED
				).AddComponent(jPanel4, GroupLayout.PREFERRED_SIZE, GroupLayout.DEFAULT_SIZE, GroupLayout
				.PREFERRED_SIZE)).AddPreferredGap(LayoutStyle.ComponentPlacement.RELATED).AddComponent
				(saveDebugTables)).AddPreferredGap(LayoutStyle.ComponentPlacement.RELATED).AddComponent
				(displayHighTables)).AddPreferredGap(LayoutStyle.ComponentPlacement.RELATED).AddComponent
				(valueLimitWarning)).AddGap(27)).AddGroup(((GroupLayout.ParallelGroup)((GroupLayout.ParallelGroup
				)((GroupLayout.ParallelGroup)((GroupLayout.ParallelGroup)jPanelTableDisplayLayout
				.CreateParallelGroup(GroupLayout.Alignment.BASELINE).AddComponent(lblCellWidth))
				.AddComponent(cellWidth, GroupLayout.PREFERRED_SIZE, GroupLayout.DEFAULT_SIZE, GroupLayout
				.PREFERRED_SIZE)).AddComponent(lblCellHeight)).AddComponent(cellHeight, GroupLayout
				.PREFERRED_SIZE, GroupLayout.DEFAULT_SIZE, GroupLayout.PREFERRED_SIZE)))).AddPreferredGap
				(LayoutStyle.ComponentPlacement.RELATED).AddGroup(((GroupLayout.ParallelGroup)((
				GroupLayout.ParallelGroup)jPanelTableDisplayLayout.CreateParallelGroup(GroupLayout.Alignment
				.BASELINE).AddComponent(lblFont)).AddComponent(btnChooseFont, GroupLayout.PREFERRED_SIZE
				, 18, GroupLayout.PREFERRED_SIZE)))).AddContainerGap())));
			jPanelTableDisplay.SetLayout(jPanelTableDisplayLayout);
			// Init Clipboard Tab Panel
			GroupLayout jPanelClipboardLayout = new GroupLayout(jPanelClipboard);
			jPanelClipboardLayout.SetHorizontalGroup(((GroupLayout.ParallelGroup)jPanelClipboardLayout
				.CreateParallelGroup(GroupLayout.Alignment.TRAILING).AddGroup(((GroupLayout.SequentialGroup
				)((GroupLayout.SequentialGroup)jPanelClipboardLayout.CreateSequentialGroup().AddContainerGap
				().AddGroup(((GroupLayout.ParallelGroup)jPanelClipboardLayout.CreateParallelGroup
				(GroupLayout.Alignment.LEADING).AddGroup(((GroupLayout.SequentialGroup)((GroupLayout.SequentialGroup
				)jPanelClipboardLayout.CreateSequentialGroup().AddGap(17)).AddGroup(((GroupLayout.ParallelGroup
				)((GroupLayout.ParallelGroup)((GroupLayout.ParallelGroup)jPanelClipboardLayout.CreateParallelGroup
				(GroupLayout.Alignment.LEADING).AddComponent(rdbtnAirboys)).AddComponent(rdbtnDefault
				)).AddComponent(rdbtnCustom)))))))).AddGap(157)))));
			jPanelClipboardLayout.SetVerticalGroup(((GroupLayout.ParallelGroup)jPanelClipboardLayout
				.CreateParallelGroup(GroupLayout.Alignment.LEADING).AddGroup(((GroupLayout.SequentialGroup
				)((GroupLayout.SequentialGroup)((GroupLayout.SequentialGroup)((GroupLayout.SequentialGroup
				)jPanelClipboardLayout.CreateSequentialGroup().AddContainerGap().AddPreferredGap
				(LayoutStyle.ComponentPlacement.RELATED).AddComponent(rdbtnDefault)).AddPreferredGap
				(LayoutStyle.ComponentPlacement.RELATED).AddComponent(rdbtnAirboys)).AddPreferredGap
				(LayoutStyle.ComponentPlacement.RELATED).AddComponent(rdbtnCustom)).AddGap(435))
				)));
			jPanelClipboard.SetLayout(jPanelClipboardLayout);
			// Init Icons Tab panel
			editorIconsPanel.SetBorder(new TitledBorder(null, "Editor Toolbar Icons", TitledBorder
				.LEADING, TitledBorder.TOP, null, null));
			tableIconsPanel.SetBorder(new TitledBorder(null, "Table Toolbar Icons", TitledBorder
				.LEADING, TitledBorder.TOP, null, null));
			GroupLayout jPanelIconsLayout = new GroupLayout(jPanelIcons);
			jPanelIconsLayout.SetHorizontalGroup(((GroupLayout.ParallelGroup)jPanelIconsLayout
				.CreateParallelGroup(GroupLayout.Alignment.TRAILING).AddGroup(((GroupLayout.SequentialGroup
				)jPanelIconsLayout.CreateSequentialGroup().AddContainerGap().AddGroup(((GroupLayout.ParallelGroup
				)((GroupLayout.ParallelGroup)jPanelIconsLayout.CreateParallelGroup(GroupLayout.Alignment
				.LEADING).AddComponent(editorIconsPanel, GroupLayout.DEFAULT_SIZE, 407, short.MaxValue
				)).AddComponent(tableIconsPanel, GroupLayout.DEFAULT_SIZE, 407, short.MaxValue))
				)).AddContainerGap())));
			jPanelIconsLayout.SetVerticalGroup(((GroupLayout.ParallelGroup)jPanelIconsLayout.
				CreateParallelGroup(GroupLayout.Alignment.LEADING).AddGroup(((GroupLayout.SequentialGroup
				)((GroupLayout.SequentialGroup)jPanelIconsLayout.CreateSequentialGroup().AddContainerGap
				().AddComponent(editorIconsPanel, GroupLayout.PREFERRED_SIZE, 66, GroupLayout.PREFERRED_SIZE
				)).AddPreferredGap(LayoutStyle.ComponentPlacement.RELATED).AddComponent(tableIconsPanel
				, GroupLayout.PREFERRED_SIZE, 64, GroupLayout.PREFERRED_SIZE)).AddContainerGap(367
				, short.MaxValue))));
			JLabel lblTableIconScale = new JLabel("Scale:");
			textFieldTableIconScale = new JTextField();
			textFieldTableIconScale.SetToolTipText("The percentage of the icons original size."
				);
			textFieldTableIconScale.SetColumns(10);
			JLabel labelTableScalePercent = new JLabel("%");
			GroupLayout tableIconsPanelLayout = new GroupLayout(tableIconsPanel);
			tableIconsPanelLayout.SetHorizontalGroup(((GroupLayout.ParallelGroup)tableIconsPanelLayout
				.CreateParallelGroup(GroupLayout.Alignment.LEADING).AddGroup(((GroupLayout.SequentialGroup
				)((GroupLayout.SequentialGroup)((GroupLayout.SequentialGroup)tableIconsPanelLayout
				.CreateSequentialGroup().AddContainerGap().AddComponent(lblTableIconScale)).AddPreferredGap
				(LayoutStyle.ComponentPlacement.RELATED).AddComponent(textFieldTableIconScale, GroupLayout
				.PREFERRED_SIZE, GroupLayout.DEFAULT_SIZE, GroupLayout.PREFERRED_SIZE)).AddPreferredGap
				(LayoutStyle.ComponentPlacement.RELATED).AddComponent(labelTableScalePercent)).AddContainerGap
				(216, short.MaxValue))));
			tableIconsPanelLayout.SetVerticalGroup(((GroupLayout.ParallelGroup)tableIconsPanelLayout
				.CreateParallelGroup(GroupLayout.Alignment.LEADING).AddGroup(((GroupLayout.SequentialGroup
				)tableIconsPanelLayout.CreateSequentialGroup().AddContainerGap().AddGroup(((GroupLayout.ParallelGroup
				)((GroupLayout.ParallelGroup)((GroupLayout.ParallelGroup)tableIconsPanelLayout.CreateParallelGroup
				(GroupLayout.Alignment.BASELINE).AddComponent(lblTableIconScale)).AddComponent(textFieldTableIconScale
				, GroupLayout.PREFERRED_SIZE, GroupLayout.DEFAULT_SIZE, GroupLayout.PREFERRED_SIZE
				)).AddComponent(labelTableScalePercent)))).AddContainerGap(116, short.MaxValue))
				));
			tableIconsPanel.SetLayout(tableIconsPanelLayout);
			JLabel lblEditorIconScale = new JLabel("Scale:");
			textFieldEditorIconScale = new JTextField();
			textFieldEditorIconScale.SetToolTipText("The percentage of the icons original size."
				);
			textFieldEditorIconScale.SetColumns(10);
			JLabel labelEditorScalePercent = new JLabel("%");
			GroupLayout editorIconsPanelLayout = new GroupLayout(editorIconsPanel);
			editorIconsPanelLayout.SetHorizontalGroup(((GroupLayout.ParallelGroup)editorIconsPanelLayout
				.CreateParallelGroup(GroupLayout.Alignment.LEADING).AddGroup(((GroupLayout.SequentialGroup
				)((GroupLayout.SequentialGroup)((GroupLayout.SequentialGroup)editorIconsPanelLayout
				.CreateSequentialGroup().AddContainerGap().AddComponent(lblEditorIconScale)).AddPreferredGap
				(LayoutStyle.ComponentPlacement.RELATED).AddComponent(textFieldEditorIconScale, 
				GroupLayout.PREFERRED_SIZE, GroupLayout.DEFAULT_SIZE, GroupLayout.PREFERRED_SIZE
				)).AddPreferredGap(LayoutStyle.ComponentPlacement.RELATED).AddComponent(labelEditorScalePercent
				)).AddContainerGap(216, short.MaxValue))));
			editorIconsPanelLayout.SetVerticalGroup(((GroupLayout.ParallelGroup)editorIconsPanelLayout
				.CreateParallelGroup(GroupLayout.Alignment.LEADING).AddGroup(((GroupLayout.SequentialGroup
				)editorIconsPanelLayout.CreateSequentialGroup().AddContainerGap().AddGroup(((GroupLayout.ParallelGroup
				)((GroupLayout.ParallelGroup)((GroupLayout.ParallelGroup)editorIconsPanelLayout.
				CreateParallelGroup(GroupLayout.Alignment.BASELINE).AddComponent(lblEditorIconScale
				)).AddComponent(textFieldEditorIconScale, GroupLayout.PREFERRED_SIZE, GroupLayout
				.DEFAULT_SIZE, GroupLayout.PREFERRED_SIZE)).AddComponent(labelEditorScalePercent
				)))).AddContainerGap(95, short.MaxValue))));
			editorIconsPanel.SetLayout(editorIconsPanelLayout);
			jPanelIcons.SetLayout(jPanelIconsLayout);
		}

		public virtual void AddLoggerDefFile()
		{
			JFileChooser fc = new JFileChooser(Settings.RR_LOGGER_REPO);
			fc.SetFileFilter(new XMLFilter());
			if (fc.ShowOpenDialog(this) == JFileChooser.APPROVE_OPTION && fc.GetSelectedFile(
				).GetName().ToLower().Contains(".xml"))
			{
				if (settings.GetAvailableLoggerDefFiles().ContainsValue(fc.GetSelectedFile()))
				{
					comboBoxLoggerDef.SetSelectedItem(fc.GetSelectedFile().GetName());
				}
				else
				{
					settings.AddAvailableLoggerDefFile(fc.GetSelectedFile(), true);
					settings.SetLoggerDefFilePath(settings.GetAvailableLoggerDefFiles().Get(fc.GetSelectedFile
						().GetName()).GetAbsolutePath());
					//TODO abstract this?
					DefaultComboBoxModel cmb = new DefaultComboBoxModel(Sharpen.Collections.ToArray(settings
						.GetAvailableLoggerDefs().Keys));
					comboBoxLoggerDef.SetModel(cmb);
					comboBoxLoggerDef.SetSelectedItem(fc.GetSelectedFile().GetAbsolutePath());
				}
			}
		}

		public virtual void SaveecuDefinitionList()
		{
			Vector<FilePath> output = new Vector<FilePath>();
			// create file vector
			for (int i = 0; i < ecuDefFileNames.Count; i++)
			{
				output.AddItem(new FilePath(ecuDefFileNames[i]));
			}
			// save
			//parent.getSettings().
			settings.SetEcuDefinitionFiles(output);
		}

		public virtual void AddEcuDefFile()
		{
			JFileChooser fc = new JFileChooser(Settings.RRECUDEFREPO);
			fc.SetFileFilter(new XMLFilter());
			if (fc.ShowOpenDialog(this) == JFileChooser.APPROVE_OPTION)
			{
				ecuDefFileNames.AddItem(fc.GetSelectedFile().GetAbsolutePath());
				UpdateEcuDefListModel();
			}
		}

		public virtual void MoveSelection(int direction)
		{
			int selectedIndex = ecuDefinitionList.GetSelectedIndex();
			string fileName = ecuDefFileNames[selectedIndex];
			if (direction == MOVE_UP && selectedIndex > 0)
			{
				ecuDefFileNames.Remove(selectedIndex);
				ecuDefFileNames.Add(--selectedIndex, fileName);
			}
			else
			{
				if (direction == MOVE_DOWN && selectedIndex < ecuDefinitionList.GetModel().GetSize
					())
				{
					ecuDefFileNames.Remove(selectedIndex);
					ecuDefFileNames.Add(++selectedIndex, fileName);
				}
			}
			UpdateEcuDefListModel();
			ecuDefinitionList.SetSelectedIndex(selectedIndex);
		}

		public virtual void RemoveSelection()
		{
			int index = ecuDefinitionList.GetSelectedIndex();
			if (index < 0)
			{
				return;
			}
			ecuDefFileNames.Remove(index);
			UpdateEcuDefListModel();
		}

		public virtual void UpdateEcuDefListModel()
		{
			ecuDefinitionList.SetListData(ecuDefFileNames);
		}

		public virtual void AddGitRemoteAndFetch()
		{
			if (settings.GetGitRemotes().ContainsKey(this.textFieldRemoteName.GetText()))
			{
				JOptionPane.ShowMessageDialog(this, "A remote named " + this.textFieldRemoteName.
					GetText() + " already exists!", "Git Config", JOptionPane.INFORMATION_MESSAGE);
			}
			else
			{
				if (settings.GetGitRemotes().ContainsValue(this.textFieldGitRepo.GetText()))
				{
					JOptionPane.ShowMessageDialog(this, "A remote with URL " + this.textFieldGitRepo.
						GetText() + " already exists!", "Git Config", JOptionPane.INFORMATION_MESSAGE);
				}
				else
				{
					SetCursor(Cursor.GetPredefinedCursor(Cursor.WAIT_CURSOR));
					if (this.textFieldGitRepo.GetText() != null && this.textFieldRemoteName.GetText()
						 != null)
					{
						ECUExec.definitionRepoManager.AddRemote(this.textFieldRemoteName.GetText(), this.
							textFieldGitRepo.GetText());
					}
					SetCursor(null);
				}
			}
		}

		public virtual void UpdateComboBoxGitBranch()
		{
			DefaultComboBoxModel dcmb = new DefaultComboBoxModel(ECUExec.definitionRepoManager
				.GetAvailableBranches());
			this.comboBoxGitBranch.SetModel(dcmb);
			this.comboBoxGitBranch.SetSelectedItem(settings.GetGitBranch());
		}

		public virtual void MouseClicked(MouseEvent e)
		{
			if (e.GetSource() == maxColor)
			{
				Color color = JColorChooser.ShowDialog(this.GetContentPane(), "Background Color", 
					settings.GetMaxColor());
				if (color != null)
				{
					maxColor.SetBackground(color);
				}
			}
			else
			{
				if (e.GetSource() == minColor)
				{
					Color color = JColorChooser.ShowDialog(this.GetContentPane(), "Background Color", 
						settings.GetMinColor());
					if (color != null)
					{
						minColor.SetBackground(color);
					}
				}
				else
				{
					if (e.GetSource() == highlightColor)
					{
						Color color = JColorChooser.ShowDialog(this.GetContentPane(), "Background Color", 
							settings.GetHighlightColor());
						if (color != null)
						{
							highlightColor.SetBackground(color);
						}
					}
					else
					{
						if (e.GetSource() == axisColor)
						{
							Color color = JColorChooser.ShowDialog(this.GetContentPane(), "Background Color", 
								settings.GetAxisColor());
							if (color != null)
							{
								axisColor.SetBackground(color);
							}
						}
						else
						{
							if (e.GetSource() == increaseColor)
							{
								Color color = JColorChooser.ShowDialog(this.GetContentPane(), "Background Color", 
									settings.GetIncreaseBorder());
								if (color != null)
								{
									increaseColor.SetBackground(color);
								}
							}
							else
							{
								if (e.GetSource() == decreaseColor)
								{
									Color color = JColorChooser.ShowDialog(this.GetContentPane(), "Background Color", 
										settings.GetDecreaseBorder());
									if (color != null)
									{
										decreaseColor.SetBackground(color);
									}
								}
								else
								{
									if (e.GetSource() == warningColor)
									{
										Color color = JColorChooser.ShowDialog(this.GetContentPane(), "Warning Color", settings
											.GetWarningColor());
										if (color != null)
										{
											warningColor.SetBackground(color);
										}
									}
									else
									{
										if (e.GetSource() == btnApply)
										{
											ApplySettings();
										}
										else
										{
											if (e.GetSource() == btnOk)
											{
												// Apply settings to Settings object.
												ApplySettings();
												// Write settings to file.
												SaveSettings();
												this.Dispose();
											}
											else
											{
												if (e.GetSource() == btnCancel)
												{
													this.Dispose();
												}
												else
												{
													if (e.GetSource() == btnChooseFont)
													{
														ZoeloeSoft.Projects.JFontChooser.JFontChooser fc = new ZoeloeSoft.Projects.JFontChooser.JFontChooser
															(this);
														fc.SetLocationRelativeTo(this);
														if (fc.ShowDialog(settings.GetTableFont()) == ZoeloeSoft.Projects.JFontChooser.JFontChooser
															.OK_OPTION)
														{
															btnChooseFont.SetFont(fc.GetFont());
															btnChooseFont.SetText(fc.GetFont().GetFontName());
														}
													}
													else
													{
														if (e.GetSource() == reset)
														{
															settings = new Settings();
															InitSettings();
														}
														else
														{
															if (e.GetSource() == btnAddAssocs)
															{
																// add file associations for selected file types
																try
																{
																	if (extensionHex.IsSelected())
																	{
																		FileAssociator.AddAssociation("HEX", new FilePath(".").GetCanonicalPath() + FilePath
																			.separator + Version.PRODUCT_NAME + ".exe", "ECU Image");
																	}
																	if (extensionBin.IsSelected())
																	{
																		FileAssociator.AddAssociation("BIN", new FilePath(".").GetCanonicalPath() + FilePath
																			.separator + Version.PRODUCT_NAME + ".exe", "ECU Image");
																	}
																}
																catch (Exception)
																{
																}
															}
															else
															{
																if (e.GetSource() == btnRemoveAssocs)
																{
																	// remove file associations for selected file types
																	if (extensionHex.IsSelected())
																	{
																		FileAssociator.RemoveAssociation("HEX");
																	}
																	if (extensionBin.IsSelected())
																	{
																		FileAssociator.RemoveAssociation("BIN");
																	}
																}
																else
																{
																	if (e.GetSource() == this.btnLoggerDefChooseFile)
																	{
																		AddLoggerDefFile();
																	}
																	else
																	{
																		if (e.GetSource() == btnFetchResetGit)
																		{
																			SetCursor(Cursor.GetPredefinedCursor(Cursor.WAIT_CURSOR));
																			ECUExec.definitionRepoManager.CheckoutBranch(comboBoxGitBranch.GetSelectedItem().
																				ToString());
																			SetCursor(null);
																		}
																		else
																		{
																			if (e.GetSource() == btnAddRemote)
																			{
																				AddGitRemoteAndFetch();
																				UpdateComboBoxGitBranch();
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
		}

		public virtual void ApplySettings()
		{
			try
			{
				System.Convert.ToInt32(cellHeight.GetText());
			}
			catch (FormatException)
			{
				// number formatted imporperly, reset
				cellHeight.SetText((int)(settings.GetCellSize().GetHeight()) + string.Empty);
			}
			try
			{
				System.Convert.ToInt32(cellWidth.GetText());
			}
			catch (FormatException)
			{
				// number formatted imporperly, reset
				cellWidth.SetText((int)(settings.GetCellSize().GetWidth()) + string.Empty);
			}
			settings.SetObsoleteWarning(obsoleteWarning.IsSelected());
			settings.SetCalcConflictWarning(calcConflictWarning.IsSelected());
			settings.SetDisplayHighTables(displayHighTables.IsSelected());
			settings.SetSaveDebugTables(saveDebugTables.IsSelected());
			settings.SetDebug(debug.IsSelected());
			settings.SetMaxColor(maxColor.GetBackground());
			settings.SetMinColor(minColor.GetBackground());
			settings.SetHighlightColor(highlightColor.GetBackground());
			settings.SetAxisColor(axisColor.GetBackground());
			settings.SetIncreaseBorder(increaseColor.GetBackground());
			settings.SetDecreaseBorder(decreaseColor.GetBackground());
			settings.SetCellSize(new Dimension(System.Convert.ToInt32(cellWidth.GetText()), System.Convert.ToInt32
				(cellHeight.GetText())));
			settings.SetTableFont(btnChooseFont.GetFont());
			if (tableClickCount.GetSelectedIndex() == 0)
			{
				// single click opens table
				settings.SetTableClickCount(1);
			}
			else
			{
				// double click opens table
				settings.SetTableClickCount(2);
			}
			settings.SetValueLimitWarning(valueLimitWarning.IsSelected());
			settings.SetWarningColor(warningColor.GetBackground());
			if (rdbtnAirboys.IsSelected())
			{
				settings.SetAirboysFormat();
			}
			else
			{
				if (rdbtnCustom.IsSelected())
				{
					settings.SetTableClipboardFormat(Settings.CUSTOM_CLIPBOARD_FORMAT);
				}
				else
				{
					// Table Header settings need to be manually edited in the settings.xml file;
					settings.SetDefaultFormat();
				}
			}
			try
			{
				settings.SetEditorIconScale(System.Convert.ToInt32(textFieldEditorIconScale.GetText
					()));
				if (parentEditor != null)
				{
					parentEditor.GetToolBar().UpdateIcons();
				}
			}
			catch (FormatException)
			{
				// Number formatted incorrectly reset.
				textFieldEditorIconScale.SetText(settings.GetEditorIconScale().ToString());
			}
			try
			{
				settings.SetTableIconScale(System.Convert.ToInt32(textFieldTableIconScale.GetText
					()));
				if (parentEditor != null)
				{
					parentEditor.GetTableToolBar().UpdateIcons();
				}
			}
			catch (FormatException)
			{
				// Number formatted incorrectly reset.
				textFieldTableIconScale.SetText(settings.GetTableIconScale().ToString());
			}
			Vector<FilePath> output = new Vector<FilePath>();
			for (int i = 0; i < ecuDefFileNames.Count; i++)
			{
				output.AddItem(new FilePath(ecuDefFileNames[i]));
			}
			settings.SetEcuDefinitionFiles(output);
			int index = 0;
			string s = comboBoxLoggerDef.GetSelectedItem().ToString();
			FilePath f = settings.GetAvailableLoggerDefs().Get(s);
			settings.SetLoggerDefFilePath(f.GetAbsolutePath());
			if (parentLogger != null)
			{
				try
				{
					parentLogger.LoadLoggerParams();
				}
				catch (Exception e)
				{
					parentLogger.ReportError(e);
				}
			}
		}

		public virtual void SaveSettings()
		{
			ECUExec.settingsManager.Save(settings);
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

		private JLabel axisColor;

		private JButton btnAddAssocs;

		private JButton btnApply;

		private JButton btnCancel;

		private JButton btnChooseFont;

		private JButton btnOk;

		private JButton btnRemoveAssocs;

		private JCheckBox calcConflictWarning;

		private JTextField cellHeight;

		private JTextField cellWidth;

		private JCheckBox debug;

		private JLabel decreaseColor;

		private JCheckBox displayHighTables;

		private JCheckBox extensionBin;

		private JCheckBox extensionHex;

		private JLabel highlightColor;

		private JLabel increaseColor;

		private JLabel jLabel1;

		private JTabbedPane settingsTabbedPane;

		private JPanel jPanelDefault;

		private JPanel jPanelClipboard;

		private JPanel jPanelTableDisplay;

		private JPanel jPanelIcons;

		private JPanel jPanel2;

		private JPanel jPanel3;

		private JPanel jPanel4;

		private JLabel lblAxis;

		private JLabel lblCellHeight;

		private JLabel lblCellWidth;

		private JLabel lblDecrease;

		private JLabel lblFont;

		private JLabel lblHighlight;

		private JLabel lblIncrease;

		private JLabel lblMax;

		private JLabel lblMin;

		private JLabel lblWarning;

		private JLabel maxColor;

		private JLabel minColor;

		private JCheckBox obsoleteWarning;

		private JButton reset;

		private JCheckBox saveDebugTables;

		private JComboBox tableClickCount;

		private JCheckBox valueLimitWarning;

		private JLabel warningColor;

		private ButtonGroup clipboardButtonGroup;

		private JRadioButton rdbtnDefault;

		private JRadioButton rdbtnAirboys;

		private JRadioButton rdbtnCustom;

		private JPanel editorIconsPanel;

		private JPanel tableIconsPanel;

		private JTextField textFieldTableIconScale;

		private JTextField textFieldEditorIconScale;

		private JTextField textFieldGitRepo;

		private JButton buttonEcuDefMoveUp;

		private JButton buttonEcuDefAdd;

		private JButton buttonEcuDefRemove;

		private JButton buttonEcuDefMoveDown;

		private JList ecuDefinitionList;

		private JPanel panel;

		private JLabel lblFilepath;

		private JComboBox comboBoxLoggerDef;

		private JComboBox comboBoxGitBranch;

		private JButton btnAddRemote;

		private JButton btnFetchResetGit;

		private JButton btnLoggerDefChooseFile;

		private JLabel lblRemoteName;

		private JTextField textFieldRemoteName;
		// Variables declaration - do not modify//GEN-BEGIN:variables
	}
}

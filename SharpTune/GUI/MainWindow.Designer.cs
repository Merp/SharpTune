namespace SharpTune
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.txtConsole = new System.Windows.Forms.TextBox();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openDeviceImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeDeviceImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveDeviceImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveDeviceImageAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rOMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.obfuscateCALIDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.xMLToIDCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iDAToHEWToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mAPToRRLoggerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.definitionEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.xMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.definitionLocationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manuallySelectPatchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.onlineHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.romRaiderIRCChannelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.romRaiderForumToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sharpTuningForumToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.licensingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.TableTreeView = new System.Windows.Forms.TreeView();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.ModTreeView = new System.Windows.Forms.TreeView();
            this.openDeviceListBox = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonPatchRom = new System.Windows.Forms.Button();
            this.DataTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtConsole
            // 
            this.txtConsole.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtConsole.BackColor = System.Drawing.Color.Black;
            this.txtConsole.ForeColor = System.Drawing.Color.LawnGreen;
            this.txtConsole.Location = new System.Drawing.Point(7, 272);
            this.txtConsole.Multiline = true;
            this.txtConsole.Name = "txtConsole";
            this.txtConsole.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtConsole.Size = new System.Drawing.Size(567, 243);
            this.txtConsole.TabIndex = 0;
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(109, 17);
            this.toolStripStatusLabel1.Text = "SharpTune Version ";
            this.toolStripStatusLabel1.Click += new System.EventHandler(this.toolStripStatusLabel1_Click);
            // 
            // StatusLabel
            // 
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(899, 17);
            this.StatusLabel.Spring = true;
            this.StatusLabel.Text = "Idle";
            this.StatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 16);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.StatusLabel,
            this.toolStripProgressBar1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 550);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1125, 22);
            this.statusStrip1.TabIndex = 5;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openDeviceImageToolStripMenuItem,
            this.closeDeviceImageToolStripMenuItem,
            this.saveDeviceImageToolStripMenuItem,
            this.saveDeviceImageAsToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openDeviceImageToolStripMenuItem
            // 
            this.openDeviceImageToolStripMenuItem.Name = "openDeviceImageToolStripMenuItem";
            this.openDeviceImageToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.openDeviceImageToolStripMenuItem.Text = "Open Device Image";
            this.openDeviceImageToolStripMenuItem.Click += new System.EventHandler(this.openDeviceImageToolStripMenuItem_Click);
            // 
            // closeDeviceImageToolStripMenuItem
            // 
            this.closeDeviceImageToolStripMenuItem.Enabled = false;
            this.closeDeviceImageToolStripMenuItem.Name = "closeDeviceImageToolStripMenuItem";
            this.closeDeviceImageToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.closeDeviceImageToolStripMenuItem.Text = "Close Device Image";
            this.closeDeviceImageToolStripMenuItem.Click += new System.EventHandler(this.closeDeviceImageToolStripMenuItem_Click);
            // 
            // saveDeviceImageToolStripMenuItem
            // 
            this.saveDeviceImageToolStripMenuItem.Name = "saveDeviceImageToolStripMenuItem";
            this.saveDeviceImageToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.saveDeviceImageToolStripMenuItem.Text = "Save Device Image";
            this.saveDeviceImageToolStripMenuItem.Click += new System.EventHandler(this.saveDeviceImageToolStripMenuItem_Click);
            // 
            // saveDeviceImageAsToolStripMenuItem
            // 
            this.saveDeviceImageAsToolStripMenuItem.Name = "saveDeviceImageAsToolStripMenuItem";
            this.saveDeviceImageAsToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.saveDeviceImageAsToolStripMenuItem.Text = "Save Device Image As";
            this.saveDeviceImageAsToolStripMenuItem.Click += new System.EventHandler(this.saveDeviceImageAsToolStripMenuItem_Click);
            // 
            // rOMToolStripMenuItem
            // 
            this.rOMToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.obfuscateCALIDToolStripMenuItem,
            this.xMLToIDCToolStripMenuItem,
            this.iDAToHEWToolStripMenuItem,
            this.mAPToRRLoggerToolStripMenuItem,
            this.definitionEditorToolStripMenuItem});
            this.rOMToolStripMenuItem.Name = "rOMToolStripMenuItem";
            this.rOMToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.rOMToolStripMenuItem.Text = "Tools";
            // 
            // obfuscateCALIDToolStripMenuItem
            // 
            this.obfuscateCALIDToolStripMenuItem.Enabled = false;
            this.obfuscateCALIDToolStripMenuItem.Name = "obfuscateCALIDToolStripMenuItem";
            this.obfuscateCALIDToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.obfuscateCALIDToolStripMenuItem.Text = "Obfuscate CALID";
            this.obfuscateCALIDToolStripMenuItem.Click += new System.EventHandler(this.obfuscateCALIDToolStripMenuItem_Click);
            // 
            // xMLToIDCToolStripMenuItem
            // 
            this.xMLToIDCToolStripMenuItem.Name = "xMLToIDCToolStripMenuItem";
            this.xMLToIDCToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.xMLToIDCToolStripMenuItem.Text = "XML to IDC";
            this.xMLToIDCToolStripMenuItem.Click += new System.EventHandler(this.xMLToIDCToolStripMenuItem_Click);
            // 
            // iDAToHEWToolStripMenuItem
            // 
            this.iDAToHEWToolStripMenuItem.Name = "iDAToHEWToolStripMenuItem";
            this.iDAToHEWToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.iDAToHEWToolStripMenuItem.Text = "IDA to HEW";
            this.iDAToHEWToolStripMenuItem.Click += new System.EventHandler(this.iDAToHEWToolStripMenuItem_Click);
            // 
            // mAPToRRLoggerToolStripMenuItem
            // 
            this.mAPToRRLoggerToolStripMenuItem.Name = "mAPToRRLoggerToolStripMenuItem";
            this.mAPToRRLoggerToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.mAPToRRLoggerToolStripMenuItem.Text = "MAP to Definition";
            this.mAPToRRLoggerToolStripMenuItem.Click += new System.EventHandler(this.mAPToRRLoggerToolStripMenuItem_Click);
            // 
            // definitionEditorToolStripMenuItem
            // 
            this.definitionEditorToolStripMenuItem.Name = "definitionEditorToolStripMenuItem";
            this.definitionEditorToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.definitionEditorToolStripMenuItem.Text = "Definition Editor";
            this.definitionEditorToolStripMenuItem.Click += new System.EventHandler(this.definitionEditorToolStripMenuItem_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.rOMToolStripMenuItem,
            this.xMLToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1125, 24);
            this.menuStrip1.TabIndex = 11;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // xMLToolStripMenuItem
            // 
            this.xMLToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.definitionLocationToolStripMenuItem,
            this.manuallySelectPatchToolStripMenuItem});
            this.xMLToolStripMenuItem.Name = "xMLToolStripMenuItem";
            this.xMLToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.xMLToolStripMenuItem.Text = "Settings";
            this.xMLToolStripMenuItem.Click += new System.EventHandler(this.xMLToolStripMenuItem_Click);
            // 
            // definitionLocationToolStripMenuItem
            // 
            this.definitionLocationToolStripMenuItem.AutoToolTip = true;
            this.definitionLocationToolStripMenuItem.Name = "definitionLocationToolStripMenuItem";
            this.definitionLocationToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.definitionLocationToolStripMenuItem.Text = "Definition Repo Location";
            this.definitionLocationToolStripMenuItem.ToolTipText = "Set this to the Git Repository base directory. (Should contain \'.git\' directory)";
            this.definitionLocationToolStripMenuItem.Click += new System.EventHandler(this.definitionLocationToolStripMenuItem_Click);
            // 
            // manuallySelectPatchToolStripMenuItem
            // 
            this.manuallySelectPatchToolStripMenuItem.AutoToolTip = true;
            this.manuallySelectPatchToolStripMenuItem.Name = "manuallySelectPatchToolStripMenuItem";
            this.manuallySelectPatchToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.manuallySelectPatchToolStripMenuItem.Text = "Mod Location";
            this.manuallySelectPatchToolStripMenuItem.ToolTipText = "Set this to the mod \'.patch\' file location.";
            this.manuallySelectPatchToolStripMenuItem.Click += new System.EventHandler(this.manuallySelectPatchToolStripMenuItem_Click_1);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.onlineHelpToolStripMenuItem,
            this.aboutToolStripMenuItem,
            this.licensingToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // onlineHelpToolStripMenuItem
            // 
            this.onlineHelpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.romRaiderIRCChannelToolStripMenuItem,
            this.romRaiderForumToolStripMenuItem,
            this.sharpTuningForumToolStripMenuItem});
            this.onlineHelpToolStripMenuItem.Name = "onlineHelpToolStripMenuItem";
            this.onlineHelpToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.onlineHelpToolStripMenuItem.Text = "Online Help";
            // 
            // romRaiderIRCChannelToolStripMenuItem
            // 
            this.romRaiderIRCChannelToolStripMenuItem.Name = "romRaiderIRCChannelToolStripMenuItem";
            this.romRaiderIRCChannelToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.romRaiderIRCChannelToolStripMenuItem.Text = "RomRaider IRC Channel";
            this.romRaiderIRCChannelToolStripMenuItem.Click += new System.EventHandler(this.romRaiderIRCChannelToolStripMenuItem_Click);
            // 
            // romRaiderForumToolStripMenuItem
            // 
            this.romRaiderForumToolStripMenuItem.Name = "romRaiderForumToolStripMenuItem";
            this.romRaiderForumToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.romRaiderForumToolStripMenuItem.Text = "RomRaider Forum";
            this.romRaiderForumToolStripMenuItem.Click += new System.EventHandler(this.romRaiderForumToolStripMenuItem_Click);
            // 
            // sharpTuningForumToolStripMenuItem
            // 
            this.sharpTuningForumToolStripMenuItem.Name = "sharpTuningForumToolStripMenuItem";
            this.sharpTuningForumToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.sharpTuningForumToolStripMenuItem.Text = "SharpTuning Forum";
            this.sharpTuningForumToolStripMenuItem.Click += new System.EventHandler(this.sharpTuningForumToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // licensingToolStripMenuItem
            // 
            this.licensingToolStripMenuItem.Name = "licensingToolStripMenuItem";
            this.licensingToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.licensingToolStripMenuItem.Text = "Licensing";
            this.licensingToolStripMenuItem.Click += new System.EventHandler(this.licensingToolStripMenuItem_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer2.Location = new System.Drawing.Point(0, 27);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.tabControl1);
            this.splitContainer2.Panel1.Controls.Add(this.openDeviceListBox);
            this.splitContainer2.Panel1.Controls.Add(this.label3);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.button1);
            this.splitContainer2.Panel2.Controls.Add(this.label4);
            this.splitContainer2.Panel2.Controls.Add(this.buttonPatchRom);
            this.splitContainer2.Panel2.Controls.Add(this.DataTextBox);
            this.splitContainer2.Panel2.Controls.Add(this.txtConsole);
            this.splitContainer2.Panel2.Controls.Add(this.label2);
            this.splitContainer2.Panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.splitContainer2_Panel2_Paint);
            this.splitContainer2.Size = new System.Drawing.Size(1125, 520);
            this.splitContainer2.SplitterDistance = 534;
            this.splitContainer2.TabIndex = 12;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(6, 191);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(523, 324);
            this.tabControl1.TabIndex = 44;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.TableTreeView);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(515, 298);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Tables";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // TableTreeView
            // 
            this.TableTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TableTreeView.Location = new System.Drawing.Point(6, 6);
            this.TableTreeView.Name = "TableTreeView";
            this.TableTreeView.Size = new System.Drawing.Size(503, 286);
            this.TableTreeView.TabIndex = 42;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.ModTreeView);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(515, 298);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Mods";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // ModTreeView
            // 
            this.ModTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ModTreeView.Location = new System.Drawing.Point(6, 6);
            this.ModTreeView.Name = "ModTreeView";
            this.ModTreeView.Size = new System.Drawing.Size(503, 286);
            this.ModTreeView.TabIndex = 41;
            this.ModTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // openDeviceListBox
            // 
            this.openDeviceListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.openDeviceListBox.FormattingEnabled = true;
            this.openDeviceListBox.IntegralHeight = false;
            this.openDeviceListBox.Location = new System.Drawing.Point(6, 16);
            this.openDeviceListBox.Name = "openDeviceListBox";
            this.openDeviceListBox.Size = new System.Drawing.Size(513, 169);
            this.openDeviceListBox.TabIndex = 2;
            this.openDeviceListBox.SelectedValueChanged += new System.EventHandler(this.openDeviceListBox_SelectedValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Open ROMs";
            // 
            // button1
            // 
            this.button1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button1.BackgroundImage")));
            this.button1.Location = new System.Drawing.Point(7, 203);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(288, 50);
            this.button1.TabIndex = 43;
            this.button1.Text = "Donate a beer!         ";
            this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 256);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(25, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Log";
            // 
            // buttonPatchRom
            // 
            this.buttonPatchRom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonPatchRom.Enabled = false;
            this.buttonPatchRom.Location = new System.Drawing.Point(301, 203);
            this.buttonPatchRom.Name = "buttonPatchRom";
            this.buttonPatchRom.Size = new System.Drawing.Size(275, 50);
            this.buttonPatchRom.TabIndex = 40;
            this.buttonPatchRom.Text = "Select A Patch";
            this.buttonPatchRom.UseVisualStyleBackColor = true;
            this.buttonPatchRom.Click += new System.EventHandler(this.buttonPatchRom_Click);
            // 
            // DataTextBox
            // 
            this.DataTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DataTextBox.Location = new System.Drawing.Point(7, 16);
            this.DataTextBox.Multiline = true;
            this.DataTextBox.Name = "DataTextBox";
            this.DataTextBox.Size = new System.Drawing.Size(569, 181);
            this.DataTextBox.TabIndex = 42;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Data";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1125, 572);
            this.Controls.Add(this.splitContainer2);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainWindow";
            this.Text = "SharpTune";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtConsole;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rOMToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem openDeviceImageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem obfuscateCALIDToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeDeviceImageToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ToolStripMenuItem saveDeviceImageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveDeviceImageAsToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox openDeviceListBox;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem onlineHelpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem romRaiderIRCChannelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem romRaiderForumToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem licensingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem xMLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem definitionLocationToolStripMenuItem;
        private System.Windows.Forms.Button buttonPatchRom;
        private System.Windows.Forms.TextBox DataTextBox;
        private System.Windows.Forms.TreeView ModTreeView;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ToolStripMenuItem xMLToIDCToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem iDAToHEWToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem manuallySelectPatchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mAPToRRLoggerToolStripMenuItem;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ToolStripMenuItem sharpTuningForumToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem definitionEditorToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TreeView TableTreeView;
    }
}


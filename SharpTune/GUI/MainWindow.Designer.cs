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
            this.txtConsole = new System.Windows.Forms.TextBox();
            this.comboBoxPorts = new System.Windows.Forms.ComboBox();
            this.buttonPortRefresh = new System.Windows.Forms.Button();
            this.imageTreeView = new System.Windows.Forms.TreeView();
            this.linkDonate = new System.Windows.Forms.LinkLabel();
            this.button1 = new System.Windows.Forms.Button();
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
            this.modUtilityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.obfuscateCALIDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyTablesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sSMTestAppToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.xMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.definitionEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportCurrentXMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.onlineHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.romRaiderIRCChannelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.romRaiderForumToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.licensingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.openDeviceListBox = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.convertEFXMLRRv2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtConsole
            // 
            this.txtConsole.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtConsole.BackColor = System.Drawing.Color.Black;
            this.txtConsole.ForeColor = System.Drawing.Color.LawnGreen;
            this.txtConsole.Location = new System.Drawing.Point(3, 16);
            this.txtConsole.Multiline = true;
            this.txtConsole.Name = "txtConsole";
            this.txtConsole.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtConsole.Size = new System.Drawing.Size(572, 485);
            this.txtConsole.TabIndex = 0;
            // 
            // comboBoxPorts
            // 
            this.comboBoxPorts.FormattingEnabled = true;
            this.comboBoxPorts.Location = new System.Drawing.Point(27, 42);
            this.comboBoxPorts.Name = "comboBoxPorts";
            this.comboBoxPorts.Size = new System.Drawing.Size(121, 21);
            this.comboBoxPorts.TabIndex = 6;
            this.comboBoxPorts.SelectedIndexChanged += new System.EventHandler(this.comboBoxPorts_SelectedIndexChanged);
            // 
            // buttonPortRefresh
            // 
            this.buttonPortRefresh.Location = new System.Drawing.Point(154, 42);
            this.buttonPortRefresh.Name = "buttonPortRefresh";
            this.buttonPortRefresh.Size = new System.Drawing.Size(100, 23);
            this.buttonPortRefresh.TabIndex = 7;
            this.buttonPortRefresh.Text = "Refresh Ports";
            this.buttonPortRefresh.UseVisualStyleBackColor = true;
            this.buttonPortRefresh.Click += new System.EventHandler(this.buttonPortRefresh_Click);
            // 
            // imageTreeView
            // 
            this.imageTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.imageTreeView.LabelEdit = true;
            this.imageTreeView.Location = new System.Drawing.Point(2, 16);
            this.imageTreeView.Name = "imageTreeView";
            this.imageTreeView.Size = new System.Drawing.Size(283, 365);
            this.imageTreeView.TabIndex = 31;
            this.imageTreeView.Click += new System.EventHandler(this.imageTreeView_Click);
            this.imageTreeView.DoubleClick += new System.EventHandler(this.imageTreeView_DoubleClick);
            // 
            // linkDonate
            // 
            this.linkDonate.AutoSize = true;
            this.linkDonate.Font = new System.Drawing.Font("Tahoma", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkDonate.Location = new System.Drawing.Point(412, 42);
            this.linkDonate.Name = "linkDonate";
            this.linkDonate.Size = new System.Drawing.Size(415, 39);
            this.linkDonate.TabIndex = 9;
            this.linkDonate.TabStop = true;
            this.linkDonate.Text = "Please Donate! - Click Here!";
            this.linkDonate.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkDonate_LinkClicked);
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(260, 42);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(96, 23);
            this.button1.TabIndex = 10;
            this.button1.Text = "Connect";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(60, 17);
            this.toolStripStatusLabel1.Text = "RomMod!";
            // 
            // StatusLabel
            // 
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(696, 17);
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
            this.statusStrip1.Location = new System.Drawing.Point(0, 600);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(873, 22);
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
            this.modUtilityToolStripMenuItem,
            this.obfuscateCALIDToolStripMenuItem,
            this.copyTablesToolStripMenuItem,
            this.sSMTestAppToolStripMenuItem});
            this.rOMToolStripMenuItem.Name = "rOMToolStripMenuItem";
            this.rOMToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.rOMToolStripMenuItem.Text = "Tools";
            // 
            // modUtilityToolStripMenuItem
            // 
            this.modUtilityToolStripMenuItem.Enabled = false;
            this.modUtilityToolStripMenuItem.Name = "modUtilityToolStripMenuItem";
            this.modUtilityToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.modUtilityToolStripMenuItem.Text = "Mod Utility";
            this.modUtilityToolStripMenuItem.Click += new System.EventHandler(this.modUtilityToolStripMenuItem_Click);
            // 
            // obfuscateCALIDToolStripMenuItem
            // 
            this.obfuscateCALIDToolStripMenuItem.Enabled = false;
            this.obfuscateCALIDToolStripMenuItem.Name = "obfuscateCALIDToolStripMenuItem";
            this.obfuscateCALIDToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.obfuscateCALIDToolStripMenuItem.Text = "Obfuscate CALID";
            this.obfuscateCALIDToolStripMenuItem.Click += new System.EventHandler(this.obfuscateCALIDToolStripMenuItem_Click);
            // 
            // copyTablesToolStripMenuItem
            // 
            this.copyTablesToolStripMenuItem.Name = "copyTablesToolStripMenuItem";
            this.copyTablesToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.copyTablesToolStripMenuItem.Text = "Copy Tables";
            // 
            // sSMTestAppToolStripMenuItem
            // 
            this.sSMTestAppToolStripMenuItem.Name = "sSMTestAppToolStripMenuItem";
            this.sSMTestAppToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.sSMTestAppToolStripMenuItem.Text = "SSM Test App";
            this.sSMTestAppToolStripMenuItem.Click += new System.EventHandler(this.sSMTestAppToolStripMenuItem_Click);
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
            this.menuStrip1.Size = new System.Drawing.Size(873, 24);
            this.menuStrip1.TabIndex = 11;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // xMLToolStripMenuItem
            // 
            this.xMLToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.definitionEditorToolStripMenuItem,
            this.exportCurrentXMLToolStripMenuItem,
            this.convertEFXMLRRv2ToolStripMenuItem});
            this.xMLToolStripMenuItem.Name = "xMLToolStripMenuItem";
            this.xMLToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.xMLToolStripMenuItem.Text = "XML";
            // 
            // definitionEditorToolStripMenuItem
            // 
            this.definitionEditorToolStripMenuItem.Name = "definitionEditorToolStripMenuItem";
            this.definitionEditorToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.definitionEditorToolStripMenuItem.Text = "Definition Editor";
            // 
            // exportCurrentXMLToolStripMenuItem
            // 
            this.exportCurrentXMLToolStripMenuItem.Name = "exportCurrentXMLToolStripMenuItem";
            this.exportCurrentXMLToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.exportCurrentXMLToolStripMenuItem.Text = "Export Current XML";
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
            this.romRaiderForumToolStripMenuItem});
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
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer2.Location = new System.Drawing.Point(0, 91);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.label2);
            this.splitContainer2.Panel2.Controls.Add(this.txtConsole);
            this.splitContainer2.Size = new System.Drawing.Size(873, 506);
            this.splitContainer2.SplitterDistance = 289;
            this.splitContainer2.TabIndex = 12;
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.openDeviceListBox);
            this.splitContainer1.Panel1.Controls.Add(this.label3);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Panel2.Controls.Add(this.imageTreeView);
            this.splitContainer1.Size = new System.Drawing.Size(289, 506);
            this.splitContainer1.SplitterDistance = 116;
            this.splitContainer1.TabIndex = 0;
            // 
            // openDeviceListBox
            // 
            this.openDeviceListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.openDeviceListBox.FormattingEnabled = true;
            this.openDeviceListBox.IntegralHeight = false;
            this.openDeviceListBox.Location = new System.Drawing.Point(3, 16);
            this.openDeviceListBox.Name = "openDeviceListBox";
            this.openDeviceListBox.Size = new System.Drawing.Size(281, 95);
            this.openDeviceListBox.TabIndex = 2;
            this.openDeviceListBox.SelectedValueChanged += new System.EventHandler(this.openDeviceListBox_SelectedValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Open Devices";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(134, 13);
            this.label1.TabIndex = 32;
            this.label1.Text = "Selected Device Metadata";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Log Window";
            // 
            // convertEFXMLRRv2ToolStripMenuItem
            // 
            this.convertEFXMLRRv2ToolStripMenuItem.Name = "convertEFXMLRRv2ToolStripMenuItem";
            this.convertEFXMLRRv2ToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.convertEFXMLRRv2ToolStripMenuItem.Text = "Convert EF XML -> RRv2";
            this.convertEFXMLRRv2ToolStripMenuItem.Click += new System.EventHandler(this.convertEFXMLRRv2ToolStripMenuItem_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(873, 622);
            this.Controls.Add(this.splitContainer2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.linkDonate);
            this.Controls.Add(this.buttonPortRefresh);
            this.Controls.Add(this.comboBoxPorts);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainWindow";
            this.Text = "RomMod";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtConsole;
        private System.Windows.Forms.ComboBox comboBoxPorts;
        private System.Windows.Forms.Button buttonPortRefresh;
        private System.Windows.Forms.LinkLabel linkDonate;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rOMToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem modUtilityToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem openDeviceImageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem obfuscateCALIDToolStripMenuItem;
        private System.Windows.Forms.TreeView imageTreeView;
        private System.Windows.Forms.ToolStripMenuItem closeDeviceImageToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ToolStripMenuItem saveDeviceImageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveDeviceImageAsToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox openDeviceListBox;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem onlineHelpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem romRaiderIRCChannelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem romRaiderForumToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem licensingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyTablesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem xMLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem definitionEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportCurrentXMLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sSMTestAppToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem convertEFXMLRRv2ToolStripMenuItem;
    }
}


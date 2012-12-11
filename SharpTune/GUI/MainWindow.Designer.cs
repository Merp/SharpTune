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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.xMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.definitionLocationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.onlineHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.romRaiderIRCChannelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.romRaiderForumToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.licensingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.openDeviceListBox = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonPatchRom = new System.Windows.Forms.Button();
            this.selectedModTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.manuallySelectPatchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtConsole
            // 
            this.txtConsole.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtConsole.BackColor = System.Drawing.Color.Black;
            this.txtConsole.ForeColor = System.Drawing.Color.LawnGreen;
            this.txtConsole.Location = new System.Drawing.Point(0, 439);
            this.txtConsole.Multiline = true;
            this.txtConsole.Name = "txtConsole";
            this.txtConsole.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtConsole.Size = new System.Drawing.Size(873, 158);
            this.txtConsole.TabIndex = 0;
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
            this.obfuscateCALIDToolStripMenuItem,
            this.manuallySelectPatchToolStripMenuItem});
            this.rOMToolStripMenuItem.Name = "rOMToolStripMenuItem";
            this.rOMToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.rOMToolStripMenuItem.Text = "Tools";
            // 
            // obfuscateCALIDToolStripMenuItem
            // 
            this.obfuscateCALIDToolStripMenuItem.Enabled = false;
            this.obfuscateCALIDToolStripMenuItem.Name = "obfuscateCALIDToolStripMenuItem";
            this.obfuscateCALIDToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.obfuscateCALIDToolStripMenuItem.Text = "Obfuscate CALID";
            this.obfuscateCALIDToolStripMenuItem.Click += new System.EventHandler(this.obfuscateCALIDToolStripMenuItem_Click);
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
            this.definitionLocationToolStripMenuItem});
            this.xMLToolStripMenuItem.Name = "xMLToolStripMenuItem";
            this.xMLToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.xMLToolStripMenuItem.Text = "XML";
            // 
            // definitionLocationToolStripMenuItem
            // 
            this.definitionLocationToolStripMenuItem.Name = "definitionLocationToolStripMenuItem";
            this.definitionLocationToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.definitionLocationToolStripMenuItem.Text = "Definition Location";
            this.definitionLocationToolStripMenuItem.Click += new System.EventHandler(this.definitionLocationToolStripMenuItem_Click);
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
            this.splitContainer2.Panel1.Controls.Add(this.openDeviceListBox);
            this.splitContainer2.Panel1.Controls.Add(this.label3);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.buttonPatchRom);
            this.splitContainer2.Panel2.Controls.Add(this.selectedModTextBox);
            this.splitContainer2.Panel2.Controls.Add(this.label2);
            this.splitContainer2.Panel2.Controls.Add(this.treeView1);
            this.splitContainer2.Size = new System.Drawing.Size(873, 415);
            this.splitContainer2.SplitterDistance = 289;
            this.splitContainer2.TabIndex = 12;
            // 
            // openDeviceListBox
            // 
            this.openDeviceListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.openDeviceListBox.FormattingEnabled = true;
            this.openDeviceListBox.IntegralHeight = false;
            this.openDeviceListBox.Location = new System.Drawing.Point(6, 16);
            this.openDeviceListBox.Name = "openDeviceListBox";
            this.openDeviceListBox.Size = new System.Drawing.Size(274, 152);
            this.openDeviceListBox.TabIndex = 2;
            this.openDeviceListBox.SelectedValueChanged += new System.EventHandler(this.openDeviceListBox_SelectedValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(107, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Open Device Images";
            // 
            // buttonPatchRom
            // 
            this.buttonPatchRom.Enabled = false;
            this.buttonPatchRom.Location = new System.Drawing.Point(36, 174);
            this.buttonPatchRom.Name = "buttonPatchRom";
            this.buttonPatchRom.Size = new System.Drawing.Size(434, 60);
            this.buttonPatchRom.TabIndex = 40;
            this.buttonPatchRom.Text = "Patch Rom";
            this.buttonPatchRom.UseVisualStyleBackColor = true;
            // 
            // selectedModTextBox
            // 
            this.selectedModTextBox.Location = new System.Drawing.Point(23, 148);
            this.selectedModTextBox.Name = "selectedModTextBox";
            this.selectedModTextBox.Size = new System.Drawing.Size(434, 20);
            this.selectedModTextBox.TabIndex = 42;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Patch Info";
            // 
            // treeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(36, 35);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(434, 107);
            this.treeView1.TabIndex = 41;
            this.treeView1.DoubleClick += new System.EventHandler(this.treeView1_DoubleClick_1);
            // 
            // manuallySelectPatchToolStripMenuItem
            // 
            this.manuallySelectPatchToolStripMenuItem.Name = "manuallySelectPatchToolStripMenuItem";
            this.manuallySelectPatchToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.manuallySelectPatchToolStripMenuItem.Text = "Manually Select Patch";
            this.manuallySelectPatchToolStripMenuItem.Click += new System.EventHandler(this.manuallySelectPatchToolStripMenuItem_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(873, 622);
            this.Controls.Add(this.splitContainer2);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.txtConsole);
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
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
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
        private System.Windows.Forms.TextBox selectedModTextBox;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.ToolStripMenuItem manuallySelectPatchToolStripMenuItem;
    }
}


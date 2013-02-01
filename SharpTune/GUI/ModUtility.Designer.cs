namespace SharpTune
{
    partial class ModUtility
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
            this.buttonTestPatch = new System.Windows.Forms.Button();
            this.buttonPatchRom = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.patchFileLocationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.selectedModTextBox = new System.Windows.Forms.TextBox();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonTestPatch
            // 
            this.buttonTestPatch.Enabled = false;
            this.buttonTestPatch.Location = new System.Drawing.Point(15, 166);
            this.buttonTestPatch.Name = "buttonTestPatch";
            this.buttonTestPatch.Size = new System.Drawing.Size(217, 60);
            this.buttonTestPatch.TabIndex = 34;
            this.buttonTestPatch.Text = "TEST MOD";
            this.buttonTestPatch.UseVisualStyleBackColor = true;
            this.buttonTestPatch.Click += new System.EventHandler(this.buttonTestPatch_Click);
            // 
            // buttonPatchRom
            // 
            this.buttonPatchRom.Enabled = false;
            this.buttonPatchRom.Location = new System.Drawing.Point(250, 166);
            this.buttonPatchRom.Name = "buttonPatchRom";
            this.buttonPatchRom.Size = new System.Drawing.Size(217, 60);
            this.buttonPatchRom.TabIndex = 33;
            this.buttonPatchRom.Text = "Patch Rom";
            this.buttonPatchRom.UseVisualStyleBackColor = true;
            this.buttonPatchRom.Click += new System.EventHandler(this.buttonPatchRom_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(479, 24);
            this.menuStrip1.TabIndex = 37;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.patchFileLocationToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // patchFileLocationToolStripMenuItem
            // 
            this.patchFileLocationToolStripMenuItem.Name = "patchFileLocationToolStripMenuItem";
            this.patchFileLocationToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.patchFileLocationToolStripMenuItem.Text = "Patch File Location";
            this.patchFileLocationToolStripMenuItem.Click += new System.EventHandler(this.patchFileLocationToolStripMenuItem_Click);
            // 
            // treeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(15, 27);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(450, 107);
            this.treeView1.TabIndex = 38;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            this.treeView1.DockChanged += new System.EventHandler(this.treeView1_DockChanged);
            this.treeView1.Click += new System.EventHandler(this.treeView1_Click);
            this.treeView1.DoubleClick += new System.EventHandler(this.treeView1_DoubleClick);
            // 
            // selectedModTextBox
            // 
            this.selectedModTextBox.Location = new System.Drawing.Point(15, 140);
            this.selectedModTextBox.Name = "selectedModTextBox";
            this.selectedModTextBox.Size = new System.Drawing.Size(450, 20);
            this.selectedModTextBox.TabIndex = 39;
            // 
            // ModUtility
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(479, 242);
            this.Controls.Add(this.selectedModTextBox);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.buttonTestPatch);
            this.Controls.Add(this.buttonPatchRom);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "ModUtility";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "ModUtility";
            this.Load += new System.EventHandler(this.ModUtility_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonTestPatch;
        private System.Windows.Forms.Button buttonPatchRom;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem patchFileLocationToolStripMenuItem;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.TextBox selectedModTextBox;

    }
}
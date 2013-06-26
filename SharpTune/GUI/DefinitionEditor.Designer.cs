namespace SharpTune.GUI
{
    partial class DefinitionEditor
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
            this.defTreeView = new System.Windows.Forms.TreeView();
            this.textBoxTableInfo = new System.Windows.Forms.TextBox();
            this.buttonSave = new System.Windows.Forms.Button();
            this.tabDefinition = new System.Windows.Forms.TabControl();
            this.comboBoxAvailableDefs = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // defTreeView
            // 
            this.defTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.defTreeView.Location = new System.Drawing.Point(12, 55);
            this.defTreeView.Name = "defTreeView";
            this.defTreeView.Size = new System.Drawing.Size(421, 584);
            this.defTreeView.TabIndex = 0;
            this.defTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.defTreeView_AfterSelect);
            this.defTreeView.DoubleClick += new System.EventHandler(this.defTreeView_DoubleClick);
            // 
            // textBoxTableInfo
            // 
            this.textBoxTableInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxTableInfo.Location = new System.Drawing.Point(1025, 28);
            this.textBoxTableInfo.Multiline = true;
            this.textBoxTableInfo.Name = "textBoxTableInfo";
            this.textBoxTableInfo.Size = new System.Drawing.Size(406, 196);
            this.textBoxTableInfo.TabIndex = 1;
            // 
            // buttonSave
            // 
            this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSave.Location = new System.Drawing.Point(1026, 230);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(405, 58);
            this.buttonSave.TabIndex = 2;
            this.buttonSave.Text = "SAVE";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // tabDefinition
            // 
            this.tabDefinition.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabDefinition.Location = new System.Drawing.Point(440, 28);
            this.tabDefinition.Name = "tabDefinition";
            this.tabDefinition.SelectedIndex = 0;
            this.tabDefinition.Size = new System.Drawing.Size(579, 611);
            this.tabDefinition.TabIndex = 3;
            // 
            // comboBoxAvailableDefs
            // 
            this.comboBoxAvailableDefs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxAvailableDefs.FormattingEnabled = true;
            this.comboBoxAvailableDefs.Location = new System.Drawing.Point(12, 28);
            this.comboBoxAvailableDefs.Name = "comboBoxAvailableDefs";
            this.comboBoxAvailableDefs.Size = new System.Drawing.Size(422, 21);
            this.comboBoxAvailableDefs.TabIndex = 4;
            this.comboBoxAvailableDefs.SelectedIndexChanged += new System.EventHandler(this.comboBoxAvailableDefs_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Available Definitions:";
            // 
            // DefinitionEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1586, 651);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxAvailableDefs);
            this.Controls.Add(this.tabDefinition);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.textBoxTableInfo);
            this.Controls.Add(this.defTreeView);
            this.Name = "DefinitionEditor";
            this.Text = "DefinitionEditor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DefinitionEditor_FormClosing);
            this.Load += new System.EventHandler(this.DefinitionEditor_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView defTreeView;
        private System.Windows.Forms.TextBox textBoxTableInfo;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.TabControl tabDefinition;
        private System.Windows.Forms.ComboBox comboBoxAvailableDefs;
        private System.Windows.Forms.Label label1;
    }
}
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
            this.SuspendLayout();
            // 
            // defTreeView
            // 
            this.defTreeView.Location = new System.Drawing.Point(12, 12);
            this.defTreeView.Name = "defTreeView";
            this.defTreeView.Size = new System.Drawing.Size(422, 666);
            this.defTreeView.TabIndex = 0;
            this.defTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.defTreeView_AfterSelect);
            this.defTreeView.DoubleClick += new System.EventHandler(this.defTreeView_DoubleClick);
            // 
            // textBoxTableInfo
            // 
            this.textBoxTableInfo.Location = new System.Drawing.Point(440, 12);
            this.textBoxTableInfo.Multiline = true;
            this.textBoxTableInfo.Name = "textBoxTableInfo";
            this.textBoxTableInfo.Size = new System.Drawing.Size(406, 601);
            this.textBoxTableInfo.TabIndex = 1;
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(441, 620);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(405, 58);
            this.buttonSave.TabIndex = 2;
            this.buttonSave.Text = "SAVE";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // DefinitionEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(858, 690);
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
    }
}
namespace SharpTune.GUI
{
    partial class TableDefinitionControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelType = new System.Windows.Forms.Label();
            this.labelDataAddress = new System.Windows.Forms.Label();
            this.textBoxDataAddress = new System.Windows.Forms.TextBox();
            this.labelCat = new System.Windows.Forms.Label();
            this.labelCategory = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.groupBoxTable = new System.Windows.Forms.GroupBox();
            this.groupBoxTable.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelType
            // 
            this.labelType.AutoSize = true;
            this.labelType.Location = new System.Drawing.Point(60, 26);
            this.labelType.Name = "labelType";
            this.labelType.Size = new System.Drawing.Size(13, 13);
            this.labelType.TabIndex = 1;
            this.labelType.Text = "--";
            // 
            // labelDataAddress
            // 
            this.labelDataAddress.AutoSize = true;
            this.labelDataAddress.Location = new System.Drawing.Point(21, 71);
            this.labelDataAddress.Name = "labelDataAddress";
            this.labelDataAddress.Size = new System.Drawing.Size(144, 13);
            this.labelDataAddress.TabIndex = 2;
            this.labelDataAddress.Text = "Data Address (Hexadecimal):";
            // 
            // textBoxDataAddress
            // 
            this.textBoxDataAddress.Enabled = false;
            this.textBoxDataAddress.Location = new System.Drawing.Point(171, 68);
            this.textBoxDataAddress.Name = "textBoxDataAddress";
            this.textBoxDataAddress.Size = new System.Drawing.Size(116, 20);
            this.textBoxDataAddress.TabIndex = 3;
            // 
            // labelCat
            // 
            this.labelCat.AutoSize = true;
            this.labelCat.Location = new System.Drawing.Point(21, 48);
            this.labelCat.Name = "labelCat";
            this.labelCat.Size = new System.Drawing.Size(52, 13);
            this.labelCat.TabIndex = 4;
            this.labelCat.Text = "Category:";
            // 
            // labelCategory
            // 
            this.labelCategory.AutoSize = true;
            this.labelCategory.Location = new System.Drawing.Point(80, 48);
            this.labelCategory.Name = "labelCategory";
            this.labelCategory.Size = new System.Drawing.Size(79, 13);
            this.labelCategory.TabIndex = 5;
            this.labelCategory.Text = "Table Category";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Type:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 95);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Description:";
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Location = new System.Drawing.Point(24, 112);
            this.textBoxDescription.Multiline = true;
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.Size = new System.Drawing.Size(435, 160);
            this.textBoxDescription.TabIndex = 8;
            // 
            // groupBoxTable
            // 
            this.groupBoxTable.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxTable.Controls.Add(this.label1);
            this.groupBoxTable.Controls.Add(this.textBoxDescription);
            this.groupBoxTable.Controls.Add(this.labelType);
            this.groupBoxTable.Controls.Add(this.label2);
            this.groupBoxTable.Controls.Add(this.labelDataAddress);
            this.groupBoxTable.Controls.Add(this.textBoxDataAddress);
            this.groupBoxTable.Controls.Add(this.labelCategory);
            this.groupBoxTable.Controls.Add(this.labelCat);
            this.groupBoxTable.Location = new System.Drawing.Point(3, 3);
            this.groupBoxTable.Name = "groupBoxTable";
            this.groupBoxTable.Size = new System.Drawing.Size(483, 301);
            this.groupBoxTable.TabIndex = 9;
            this.groupBoxTable.TabStop = false;
            this.groupBoxTable.Text = "Table Name";
            this.groupBoxTable.Enter += new System.EventHandler(this.groupBoxTable_Enter);
            // 
            // TableDefinitionTab
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBoxTable);
            this.Name = "TableDefinitionTab";
            this.Size = new System.Drawing.Size(489, 307);
            this.Load += new System.EventHandler(this.TableDefinitionTab_Load);
            this.groupBoxTable.ResumeLayout(false);
            this.groupBoxTable.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelType;
        private System.Windows.Forms.Label labelDataAddress;
        private System.Windows.Forms.TextBox textBoxDataAddress;
        private System.Windows.Forms.Label labelCat;
        private System.Windows.Forms.Label labelCategory;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxDescription;
        private System.Windows.Forms.GroupBox groupBoxTable;
    }
}

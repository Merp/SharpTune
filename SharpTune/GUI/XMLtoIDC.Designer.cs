namespace SharpTune.GUI
{
    partial class XMLtoIDC
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
            this.romTablesCheckBox = new System.Windows.Forms.CheckBox();
            this.ExtParamsCheckBox = new System.Windows.Forms.CheckBox();
            this.ssmParamsCheckBox = new System.Windows.Forms.CheckBox();
            this.ssmBaseTextBox = new System.Windows.Forms.TextBox();
            this.generateIdcButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.RomInfoTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // romTablesCheckBox
            // 
            this.romTablesCheckBox.AutoSize = true;
            this.romTablesCheckBox.Location = new System.Drawing.Point(232, 32);
            this.romTablesCheckBox.Name = "romTablesCheckBox";
            this.romTablesCheckBox.Size = new System.Drawing.Size(86, 17);
            this.romTablesCheckBox.TabIndex = 1;
            this.romTablesCheckBox.Text = "ROM Tables";
            this.romTablesCheckBox.UseVisualStyleBackColor = true;
            // 
            // ExtParamsCheckBox
            // 
            this.ExtParamsCheckBox.AutoSize = true;
            this.ExtParamsCheckBox.Location = new System.Drawing.Point(232, 55);
            this.ExtParamsCheckBox.Name = "ExtParamsCheckBox";
            this.ExtParamsCheckBox.Size = new System.Drawing.Size(106, 17);
            this.ExtParamsCheckBox.TabIndex = 2;
            this.ExtParamsCheckBox.Text = "RAM Parameters";
            this.ExtParamsCheckBox.UseVisualStyleBackColor = true;
            // 
            // ssmParamsCheckBox
            // 
            this.ssmParamsCheckBox.AutoSize = true;
            this.ssmParamsCheckBox.Location = new System.Drawing.Point(232, 78);
            this.ssmParamsCheckBox.Name = "ssmParamsCheckBox";
            this.ssmParamsCheckBox.Size = new System.Drawing.Size(105, 17);
            this.ssmParamsCheckBox.TabIndex = 3;
            this.ssmParamsCheckBox.Text = "SSM Parameters";
            this.ssmParamsCheckBox.UseVisualStyleBackColor = true;
            this.ssmParamsCheckBox.CheckedChanged += new System.EventHandler(this.ssmParamsCheckBox_CheckedChanged);
            // 
            // ssmBaseTextBox
            // 
            this.ssmBaseTextBox.Enabled = false;
            this.ssmBaseTextBox.Location = new System.Drawing.Point(232, 101);
            this.ssmBaseTextBox.Name = "ssmBaseTextBox";
            this.ssmBaseTextBox.Size = new System.Drawing.Size(140, 20);
            this.ssmBaseTextBox.TabIndex = 4;
            this.ssmBaseTextBox.Text = "Enter SSM Base Address";
            // 
            // generateIdcButton
            // 
            this.generateIdcButton.Location = new System.Drawing.Point(232, 127);
            this.generateIdcButton.Name = "generateIdcButton";
            this.generateIdcButton.Size = new System.Drawing.Size(140, 32);
            this.generateIdcButton.TabIndex = 5;
            this.generateIdcButton.Text = "Generate IDC Scripts!";
            this.generateIdcButton.UseVisualStyleBackColor = true;
            this.generateIdcButton.Click += new System.EventHandler(this.generateIdcButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(229, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 16);
            this.label3.TabIndex = 9;
            this.label3.Text = "Options:";
            // 
            // RomInfoTextBox
            // 
            this.RomInfoTextBox.Location = new System.Drawing.Point(12, 12);
            this.RomInfoTextBox.Multiline = true;
            this.RomInfoTextBox.Name = "RomInfoTextBox";
            this.RomInfoTextBox.Size = new System.Drawing.Size(211, 147);
            this.RomInfoTextBox.TabIndex = 10;
            // 
            // XMLtoIDC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(381, 172);
            this.Controls.Add(this.RomInfoTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.generateIdcButton);
            this.Controls.Add(this.ssmBaseTextBox);
            this.Controls.Add(this.ssmParamsCheckBox);
            this.Controls.Add(this.ExtParamsCheckBox);
            this.Controls.Add(this.romTablesCheckBox);
            this.Name = "XMLtoIDC";
            this.Text = "XMLtoIDC";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox romTablesCheckBox;
        private System.Windows.Forms.CheckBox ExtParamsCheckBox;
        private System.Windows.Forms.CheckBox ssmParamsCheckBox;
        private System.Windows.Forms.TextBox ssmBaseTextBox;
        private System.Windows.Forms.Button generateIdcButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox RomInfoTextBox;
    }
}
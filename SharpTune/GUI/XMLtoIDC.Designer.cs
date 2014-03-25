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
            this.comboBoxEcuDef = new System.Windows.Forms.ComboBox();
            this.comboBoxLoggerDTD = new System.Windows.Forms.ComboBox();
            this.comboBoxLoggerDef = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.checkBoxUseDef = new System.Windows.Forms.CheckBox();
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
            this.romTablesCheckBox.CheckStateChanged += new System.EventHandler(this.checkedChecker);
            // 
            // ExtParamsCheckBox
            // 
            this.ExtParamsCheckBox.AutoSize = true;
            this.ExtParamsCheckBox.Location = new System.Drawing.Point(232, 79);
            this.ExtParamsCheckBox.Name = "ExtParamsCheckBox";
            this.ExtParamsCheckBox.Size = new System.Drawing.Size(106, 17);
            this.ExtParamsCheckBox.TabIndex = 2;
            this.ExtParamsCheckBox.Text = "RAM Parameters";
            this.ExtParamsCheckBox.UseVisualStyleBackColor = true;
            this.ExtParamsCheckBox.CheckStateChanged += new System.EventHandler(this.checkedChecker);
            // 
            // ssmParamsCheckBox
            // 
            this.ssmParamsCheckBox.AutoSize = true;
            this.ssmParamsCheckBox.Location = new System.Drawing.Point(232, 102);
            this.ssmParamsCheckBox.Name = "ssmParamsCheckBox";
            this.ssmParamsCheckBox.Size = new System.Drawing.Size(105, 17);
            this.ssmParamsCheckBox.TabIndex = 3;
            this.ssmParamsCheckBox.Text = "SSM Parameters";
            this.ssmParamsCheckBox.UseVisualStyleBackColor = true;
            this.ssmParamsCheckBox.CheckedChanged += new System.EventHandler(this.ssmParamsCheckBox_CheckedChanged);
            this.ssmParamsCheckBox.CheckStateChanged += new System.EventHandler(this.checkedChecker);
            // 
            // ssmBaseTextBox
            // 
            this.ssmBaseTextBox.Enabled = false;
            this.ssmBaseTextBox.Location = new System.Drawing.Point(229, 125);
            this.ssmBaseTextBox.Name = "ssmBaseTextBox";
            this.ssmBaseTextBox.Size = new System.Drawing.Size(140, 20);
            this.ssmBaseTextBox.TabIndex = 4;
            this.ssmBaseTextBox.Text = "Enter SSM Base Address";
            // 
            // generateIdcButton
            // 
            this.generateIdcButton.Enabled = false;
            this.generateIdcButton.Location = new System.Drawing.Point(9, 291);
            this.generateIdcButton.Name = "generateIdcButton";
            this.generateIdcButton.Size = new System.Drawing.Size(360, 32);
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
            this.RomInfoTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.RomInfoTextBox.Size = new System.Drawing.Size(211, 133);
            this.RomInfoTextBox.TabIndex = 10;
            // 
            // comboBoxEcuDef
            // 
            this.comboBoxEcuDef.FormattingEnabled = true;
            this.comboBoxEcuDef.Location = new System.Drawing.Point(12, 184);
            this.comboBoxEcuDef.Name = "comboBoxEcuDef";
            this.comboBoxEcuDef.Size = new System.Drawing.Size(357, 21);
            this.comboBoxEcuDef.TabIndex = 11;
            // 
            // comboBoxLoggerDTD
            // 
            this.comboBoxLoggerDTD.FormattingEnabled = true;
            this.comboBoxLoggerDTD.Location = new System.Drawing.Point(12, 264);
            this.comboBoxLoggerDTD.Name = "comboBoxLoggerDTD";
            this.comboBoxLoggerDTD.Size = new System.Drawing.Size(357, 21);
            this.comboBoxLoggerDTD.TabIndex = 12;
            // 
            // comboBoxLoggerDef
            // 
            this.comboBoxLoggerDef.FormattingEnabled = true;
            this.comboBoxLoggerDef.Location = new System.Drawing.Point(12, 224);
            this.comboBoxLoggerDef.Name = "comboBoxLoggerDef";
            this.comboBoxLoggerDef.Size = new System.Drawing.Size(357, 21);
            this.comboBoxLoggerDef.TabIndex = 13;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 168);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "Ecu Def";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 208);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "Logger Def";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 248);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(66, 13);
            this.label4.TabIndex = 16;
            this.label4.Text = "Logger DTD";
            // 
            // checkBoxUseDef
            // 
            this.checkBoxUseDef.AutoSize = true;
            this.checkBoxUseDef.Enabled = false;
            this.checkBoxUseDef.Location = new System.Drawing.Point(232, 56);
            this.checkBoxUseDef.Name = "checkBoxUseDef";
            this.checkBoxUseDef.Size = new System.Drawing.Size(115, 17);
            this.checkBoxUseDef.TabIndex = 17;
            this.checkBoxUseDef.Text = "Use ECUFlash Def";
            this.checkBoxUseDef.UseVisualStyleBackColor = true;
            this.checkBoxUseDef.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // XMLtoIDC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(381, 335);
            this.Controls.Add(this.checkBoxUseDef);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxLoggerDef);
            this.Controls.Add(this.comboBoxLoggerDTD);
            this.Controls.Add(this.comboBoxEcuDef);
            this.Controls.Add(this.RomInfoTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.generateIdcButton);
            this.Controls.Add(this.ssmBaseTextBox);
            this.Controls.Add(this.ssmParamsCheckBox);
            this.Controls.Add(this.ExtParamsCheckBox);
            this.Controls.Add(this.romTablesCheckBox);
            this.Name = "XMLtoIDC";
            this.Text = "XMLtoIDC";
            this.Load += new System.EventHandler(this.XMLtoIDC_Load);
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
        private System.Windows.Forms.ComboBox comboBoxEcuDef;
        private System.Windows.Forms.ComboBox comboBoxLoggerDTD;
        private System.Windows.Forms.ComboBox comboBoxLoggerDef;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox checkBoxUseDef;
    }
}
namespace SharpTune.GUI
{
    partial class UndefinedWindow
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
            this.textBoxDefXml = new System.Windows.Forms.TextBox();
            this.comboBoxCopyDef = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.comboBoxIncludeDef = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonSave = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxDefXml
            // 
            this.textBoxDefXml.Location = new System.Drawing.Point(12, 119);
            this.textBoxDefXml.Multiline = true;
            this.textBoxDefXml.Name = "textBoxDefXml";
            this.textBoxDefXml.Size = new System.Drawing.Size(438, 265);
            this.textBoxDefXml.TabIndex = 0;
            // 
            // comboBoxCopyDef
            // 
            this.comboBoxCopyDef.FormattingEnabled = true;
            this.comboBoxCopyDef.Location = new System.Drawing.Point(110, 65);
            this.comboBoxCopyDef.Name = "comboBoxCopyDef";
            this.comboBoxCopyDef.Size = new System.Drawing.Size(249, 21);
            this.comboBoxCopyDef.TabIndex = 1;
            this.comboBoxCopyDef.SelectedValueChanged += new System.EventHandler(this.comboBoxCopyDef_SelectedValueChanged);
            this.comboBoxCopyDef.TextChanged += new System.EventHandler(this.comboBoxCopyDef_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 68);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Copy from existing";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(365, 67);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(85, 17);
            this.checkBox1.TabIndex = 4;
            this.checkBox1.Text = "Copy Tables";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // comboBoxIncludeDef
            // 
            this.comboBoxIncludeDef.FormattingEnabled = true;
            this.comboBoxIncludeDef.Location = new System.Drawing.Point(132, 92);
            this.comboBoxIncludeDef.Name = "comboBoxIncludeDef";
            this.comboBoxIncludeDef.Size = new System.Drawing.Size(318, 21);
            this.comboBoxIncludeDef.TabIndex = 5;
            this.comboBoxIncludeDef.SelectedIndexChanged += new System.EventHandler(this.comboBoxIncludeDef_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 95);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(114, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Include/Inherit existing";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(35, 7);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(397, 20);
            this.label3.TabIndex = 7;
            this.label3.Text = "The device image you have opened is undefined!";
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(233, 390);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(217, 56);
            this.buttonSave.TabIndex = 8;
            this.buttonSave.Text = "Save New Definition";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(128, 32);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(199, 20);
            this.label4.TabIndex = 9;
            this.label4.Text = "Create a new definition:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(10, 390);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(217, 56);
            this.button1.TabIndex = 10;
            this.button1.Text = "Find Calibration ID";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // UndefinedWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(462, 452);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBoxIncludeDef);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxCopyDef);
            this.Controls.Add(this.textBoxDefXml);
            this.Name = "UndefinedWindow";
            this.Text = "Undefined Device Image";
            this.Load += new System.EventHandler(this.UndefinedWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxDefXml;
        private System.Windows.Forms.ComboBox comboBoxCopyDef;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.ComboBox comboBoxIncludeDef;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button1;
    }
}
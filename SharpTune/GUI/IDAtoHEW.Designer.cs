namespace SharpTune.GUI
{
    partial class IDAtoHEW
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.convertToComboBox = new System.Windows.Forms.ComboBox();
            this.button2 = new System.Windows.Forms.Button();
            this.translationButton = new System.Windows.Forms.Button();
            this.translationTextBox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(29, 30);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(404, 20);
            this.textBox1.TabIndex = 0;
            this.textBox1.Text = "Input File";
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(339, 56);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(94, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Select Input File";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // convertToComboBox
            // 
            this.convertToComboBox.FormattingEnabled = true;
            this.convertToComboBox.Location = new System.Drawing.Point(29, 28);
            this.convertToComboBox.Name = "convertToComboBox";
            this.convertToComboBox.Size = new System.Drawing.Size(404, 21);
            this.convertToComboBox.TabIndex = 4;
            this.convertToComboBox.Text = "Select output format";
            this.convertToComboBox.SelectedIndexChanged += new System.EventHandler(this.convertToComboBox_SelectedIndexChanged);
            // 
            // button2
            // 
            this.button2.Enabled = false;
            this.button2.Location = new System.Drawing.Point(12, 308);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(476, 53);
            this.button2.TabIndex = 5;
            this.button2.Text = "Convert!";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // translationButton
            // 
            this.translationButton.Enabled = false;
            this.translationButton.Location = new System.Drawing.Point(297, 56);
            this.translationButton.Name = "translationButton";
            this.translationButton.Size = new System.Drawing.Size(136, 23);
            this.translationButton.TabIndex = 7;
            this.translationButton.Text = "Select Translation File";
            this.translationButton.UseVisualStyleBackColor = true;
            this.translationButton.Click += new System.EventHandler(this.button3_Click);
            // 
            // translationTextBox
            // 
            this.translationTextBox.Enabled = false;
            this.translationTextBox.Location = new System.Drawing.Point(29, 30);
            this.translationTextBox.Name = "translationTextBox";
            this.translationTextBox.Size = new System.Drawing.Size(404, 20);
            this.translationTextBox.TabIndex = 6;
            this.translationTextBox.Text = "Translation File";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(476, 100);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Input File";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.translationTextBox);
            this.groupBox2.Controls.Add(this.translationButton);
            this.groupBox2.Location = new System.Drawing.Point(12, 118);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(476, 102);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Translation File";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.convertToComboBox);
            this.groupBox3.Location = new System.Drawing.Point(12, 226);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(476, 76);
            this.groupBox3.TabIndex = 11;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Output Format";
            // 
            // IDAtoHEW
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(499, 374);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button2);
            this.Name = "IDAtoHEW";
            this.Text = "IDAtoHEW";
            this.Load += new System.EventHandler(this.IDAtoHEW_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox convertToComboBox;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button translationButton;
        private System.Windows.Forms.TextBox translationTextBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
    }
}
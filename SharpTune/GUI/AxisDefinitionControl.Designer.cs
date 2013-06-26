namespace SharpTune.GUI
{
    partial class AxisDefinitionControl
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
            this.groupBoxAxis = new System.Windows.Forms.GroupBox();
            this.labelType = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxElements = new System.Windows.Forms.TextBox();
            this.textBoxDefaultScaling = new System.Windows.Forms.TextBox();
            this.groupBoxAxis.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxAxis
            // 
            this.groupBoxAxis.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxAxis.Controls.Add(this.textBoxDefaultScaling);
            this.groupBoxAxis.Controls.Add(this.textBoxElements);
            this.groupBoxAxis.Controls.Add(this.label2);
            this.groupBoxAxis.Controls.Add(this.label1);
            this.groupBoxAxis.Controls.Add(this.labelType);
            this.groupBoxAxis.Location = new System.Drawing.Point(3, 3);
            this.groupBoxAxis.Name = "groupBoxAxis";
            this.groupBoxAxis.Size = new System.Drawing.Size(566, 278);
            this.groupBoxAxis.TabIndex = 2;
            this.groupBoxAxis.TabStop = false;
            this.groupBoxAxis.Text = "Axis Name";
            this.groupBoxAxis.Enter += new System.EventHandler(this.groupBoxAxis_Enter);
            // 
            // labelType
            // 
            this.labelType.AutoSize = true;
            this.labelType.Location = new System.Drawing.Point(22, 29);
            this.labelType.Name = "labelType";
            this.labelType.Size = new System.Drawing.Size(53, 13);
            this.labelType.TabIndex = 0;
            this.labelType.Text = "Axis Type";
            this.labelType.Click += new System.EventHandler(this.labelName_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 58);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Elements:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 85);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Default Scaling:";
            // 
            // textBoxElements
            // 
            this.textBoxElements.Location = new System.Drawing.Point(84, 55);
            this.textBoxElements.Name = "textBoxElements";
            this.textBoxElements.Size = new System.Drawing.Size(100, 20);
            this.textBoxElements.TabIndex = 3;
            // 
            // textBoxDefaultScaling
            // 
            this.textBoxDefaultScaling.Location = new System.Drawing.Point(113, 81);
            this.textBoxDefaultScaling.Name = "textBoxDefaultScaling";
            this.textBoxDefaultScaling.Size = new System.Drawing.Size(100, 20);
            this.textBoxDefaultScaling.TabIndex = 4;
            // 
            // AxisDefinition
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBoxAxis);
            this.Name = "AxisDefinition";
            this.Size = new System.Drawing.Size(572, 284);
            this.groupBoxAxis.ResumeLayout(false);
            this.groupBoxAxis.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxAxis;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelType;
        private System.Windows.Forms.TextBox textBoxElements;
        private System.Windows.Forms.TextBox textBoxDefaultScaling;
    }
}

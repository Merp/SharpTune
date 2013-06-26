using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SharpTuneCore;

namespace SharpTune.GUI
{
    public partial class AxisDefinitionControl : UserControl
    {
        public AxisDefinitionControl(TableAxis a)
        {
            InitializeComponent();

            groupBoxAxis.Text = a.Name;//TODO: get the right name from static axes!!! 
            labelType.Text = a.Type;
            textBoxElements.Text = a.Elements.ToString();
            //TODO: implement scalings! textBoxDefaultScaling.Text = a.scaling.name;

        }

        private void labelName_Click(object sender, EventArgs e)
        {

        }

        private void groupBoxAxis_Enter(object sender, EventArgs e)
        {

        }
    }
}

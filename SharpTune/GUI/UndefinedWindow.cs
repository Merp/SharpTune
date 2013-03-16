using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SharpTune.GUI
{
    public partial class UndefinedWindow : Form
    {
        public UndefinedWindow()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void UndefinedWindow_Load(object sender, EventArgs e)
        {
            comboBoxCopyDef.DataSource = SharpTuner.availableDevices.IdentifierList;
        }
    }
}

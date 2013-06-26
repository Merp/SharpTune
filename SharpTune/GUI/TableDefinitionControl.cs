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
    public partial class TableDefinitionControl : UserControl
    {
        public TableDefinitionControl(Table t)
        {
            InitializeComponent();


            groupBoxTable.Text = t.Name;
            labelType.Text = t.Type;
            labelCategory.Text = t.Category;
            textBoxDescription.Text = t.Description;

            if (!t.IsBase)
            {
                textBoxDescription.Enabled = false;
                textBoxDataAddress.Enabled = true;
                textBoxDataAddress.Text = t.Address.ToHexString();
            }
        }

        private void TableDefinitionTab_Load(object sender, EventArgs e)
        {

        }

        private void groupBoxTable_Enter(object sender, EventArgs e)
        {

        }
    }
}

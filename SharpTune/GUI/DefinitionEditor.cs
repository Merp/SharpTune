/*
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.
*/

using SharpTuneCore;
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
    public partial class DefinitionEditor : Form
    {
        private bool Unsaved;
        private Definition Def;
        private Dictionary<string,Table> Unexposed;
        private Dictionary<string,Table> BaseTables;
        private Dictionary<string,Table> Exposed;

        public DefinitionEditor()
        {
            InitializeComponent();

            //When this initializes, check for existing device and auto-select
            //If no device, default to all definitions

            //TODO: handle ROM VS RAM

        }

        private void DefinitionEditor_Load(object sender, EventArgs e)
        {
            Def = SharpTuner.ActiveImage.Definition;
            Exposed = Def.AggregateExposedRomTables;
            BaseTables = Def.AggregateBaseRomTables;

            TreeNode unexp = new TreeNode("Unexposed Rom Tables");
            TreeNode exp = new TreeNode("Exposed Rom Tables"); //TODO SORT BY CATEGORY!! (ROUTINE IN DEFINITION)
            
            foreach (var t in BaseTables)
            {
                if (!Exposed.ContainsKey(t.Key))
                {
                    TreeNode tn = new TreeNode(t.Key);//TODO PUT THIS IN DEFINITION!!
                    tn.Tag = t.Value;
                    unexp.Nodes.Add(tn);
                }
            }
            foreach (var t in Exposed)
            {
                TreeNode tn = new TreeNode(t.Key);
                tn.Tag = t.Value;
                exp.Nodes.Add(tn);
            }

            defTreeView.Nodes.Add(exp);
            defTreeView.Nodes.Add(unexp);
        }

        private void defTreeView_DoubleClick(object sender, EventArgs e)
        {
            Table t = (Table)defTreeView.SelectedNode.Tag;
            if (t == null)
                return;
            DialogResult overWrite;
            if(!defTreeView.SelectedNode.FullPath.ToString().ContainsCI("unexposed"))
            {
                overWrite = MessageBox.Show("Are you sure you want to overwrite " + t.name + "??", "Warning", MessageBoxButtons.YesNo,MessageBoxIcon.Warning);
                if (overWrite != DialogResult.Yes)
                    return;
            }
            try
            {
                uint address = uint.Parse(SimplePrompt.ShowDialog("Enter Hex Address of Lookup Table for " + t.name, "Enter Address"), System.Globalization.NumberStyles.AllowHexSpecifier);
                Def.ExposeTable(t.name, new Core.Lut(t.name, address));
                Unsaved = true;
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message);
            }
        }

        private void DefinitionEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(Unsaved)
            {
                DialogResult save;
                save = MessageBox.Show("Save changes??", "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (save == DialogResult.Yes)
                    Def.ExportEcuFlashXML();
                else if (save == DialogResult.Cancel)
                    e.Cancel = true;
            }
        }

        private void defTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            Table t = (Table)defTreeView.SelectedNode.Tag;
            if (t == null)
            {
                textBoxTableInfo.Clear();
                return;
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Table XML exposed in " + t.parentDef.calibrationlId.ToString());
            sb.AppendLine(t.xml.ToString());
            if (!t.isBase)
            {
                    sb.AppendLine();
                    sb.AppendLine();
                    sb.AppendLine("Base Table XML inherited from " + t.baseTable.parentDef.calibrationlId.ToString());
                    sb.AppendLine(t.baseTable.xml.ToString());
            }
            textBoxTableInfo.Text = sb.ToString();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            Def.ExportEcuFlashXML();
            Unsaved = false;
        }
    }
}

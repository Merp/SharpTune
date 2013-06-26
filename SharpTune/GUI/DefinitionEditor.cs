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
        private TreeNode unexp;
        private TreeNode exp;

        public DefinitionEditor()
        {
            InitializeComponent();

            //When this initializes, check for existing device and auto-select
            //If no device, default to all definitions

            //TODO: handle ROM VS RAM

        }

        private void DefinitionEditor_Load(object sender, EventArgs e)
        {
            unexp = new TreeNode("Unexposed Rom Tables");
            exp = new TreeNode("Exposed Rom Tables"); //TODO SORT BY CATEGORY!! (ROUTINE IN DEFINITION)
            if (SharpTuner.ActiveImage != null)
                Def = SharpTuner.ActiveImage.Definition;

            comboBoxAvailableDefs.DataSource = SharpTuner.AvailableDevices.DefDictionary.Keys.ToList();

            UpdateDefinitionEditor();
        }

        private void UpdateDefinitionEditor()
        {
            if (Def != null)
            {
                defTreeView.Nodes.Clear();
                exp.Nodes.Clear();
                unexp.Nodes.Clear();

                tabDefinition.Controls.Clear();

                Exposed = Def.ExposedRomTables;

                BaseTables = Def.ExposedBaseRomTables;

                comboBoxAvailableDefs.SelectedItem = Def.internalId.ToString();

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
        }

        private void defTreeView_DoubleClick(object sender, EventArgs e)
        {
            Table t = (Table)defTreeView.SelectedNode.Tag;
            if (t == null)
                return;
            DialogResult overWrite;
            if(!defTreeView.SelectedNode.FullPath.ToString().ContainsCI("unexposed"))
            {
                overWrite = MessageBox.Show("Are you sure you want to overwrite " + t.Name + "??", "Warning", MessageBoxButtons.YesNo,MessageBoxIcon.Warning);
                if (overWrite != DialogResult.Yes)
                    return;
            }
            try
            {
                uint address = uint.Parse(SimplePrompt.ShowDialog("Enter Hex Address of Lookup Table for " + t.Name, "Enter Address"), System.Globalization.NumberStyles.AllowHexSpecifier);
                Def.ExposeTable(t.Name, new Core.Lut(t.Name, address));
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
                    Def.ExportXML();
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
            sb.AppendLine("Table XML exposed in " + t.parentDef.internalId.ToString());
            sb.AppendLine(t.xml.ToString());
            if (!t.IsBase)
            {
                foreach(var i in t.InheritanceList)
                {
                    sb.AppendLine();
                    sb.AppendLine();
                    sb.AppendLine("Base Table XML inherited from " + i.parentDef.internalId.ToString());
                    sb.AppendLine(i.xml.ToString());
                }
            }
            textBoxTableInfo.Text = sb.ToString();

            PopulateTabs(t);
        }

        private void PopulateTabs(Table t)
        {
            tabDefinition.Controls.Clear();

            foreach (Table it in t.InheritanceList)
            {
                TabPage tp = new TabPage(it.parentDef.internalId);
                DataGrid grid = new DataGrid();

                //grid.Dock = DockStyle.Fill;
                //grid.DataSource = dataForTheCurrentLoop;

                //tabPage.Controls.Add(grid);
                tabDefinition.Controls.Add(GenerateTableTab(it));
            }
            tabDefinition.Controls.Add(GenerateTableTab(t));//TODO: anchor
            tabDefinition.SelectTab(t.parentDef.internalId.ToString());
        }

        private TabPage GenerateTableTab(Table t)
        {
            int bolc;
            int pad = 10;
            TabPage tp = new TabPage(t.parentDef.internalId.ToString());
            tp.Name = t.parentDef.internalId.ToString();
            TableDefinitionControl tdt = new TableDefinitionControl(t);
            tp.Controls.Add(tdt);
            bolc = tdt.Bottom;
            //TODO: Point locationOnForm = control.FindForm().PointToClient(
             // control.Parent.PointToScreen(control.Location));

            List<AxisDefinitionControl> axisControls = t.GenerateAxisControls();
            foreach (AxisDefinitionControl adc in axisControls)
            {
                if (adc != null)
                {
                    adc.Location = new Point(4, bolc + pad);
                    tp.Controls.Add(adc);
                    bolc = adc.Bottom;
                }
            }
            return tp;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            Def.ExportXML();
            Unsaved = false;
        }

        private void comboBoxAvailableDefs_SelectedIndexChanged(object sender, EventArgs e)
        {
            Def = SharpTuner.AvailableDevices.DefDictionary[comboBoxAvailableDefs.SelectedItem.ToString()];
            Def.Populate();
            UpdateDefinitionEditor();
        }
    }
}

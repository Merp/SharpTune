using SharpTune.Properties;
using SharpTuneCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace SharpTune.GUI
{
    public partial class UndefinedWindow : Form
    {

        private Definition def;
        private string filePath;
        private List<string> defList;

        public UndefinedWindow(string f)
        {
            InitializeComponent();
            filePath = f;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void UndefinedWindow_Load(object sender, EventArgs e)
        {
            defList = new List<string>(SharpTuner.AvailableDevices.IdentList.OrderBy(x => x.ToString()).ToList());
            def = new Definition();
            List<string> dss = new List<string>();
            dss.Add("DEFAULT");
            dss.AddRange(defList);
            comboBoxCopyDef.DataSource = dss;

            List<string> iss = new List<string>();
            iss.AddRange(defList);
            comboBoxIncludeDef.DataSource = iss;

            textBoxDefXml.Text = defaultShortDef;
        }

        private string defaultShortDef = @"<romid>
               <xmlid></xmlid>
               <internalidaddress></internalidaddress>
               <internalidstring></internalidstring>
               <ecuid></ecuid>
               <year></year>
               <market></market>
               <make></make>
               <model></model>
               <submodel></submodel>
               <transmission></transmission>
               <memmodel></memmodel>
               <flashmethod></flashmethod>
               <checksummodule></checksummodule>
              </romid>";

        private void comboBoxCopyDef_SelectedValueChanged(object sender, EventArgs e)
        {
            updateXml();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //TODO put this in definition class!
            try
            {
                def.xRomId = XElement.Parse(textBoxDefXml.Text);
                def.ParseRomId();
            }
            catch (Exception er)
            {
                MessageBox.Show("Error: " + er.Message);
                return;
            }

            using (FileStream fileStream = File.OpenRead(this.filePath))
            {
                MemoryStream memStream = new MemoryStream();
                memStream.SetLength(fileStream.Length);
                fileStream.Read(memStream.GetBuffer(), 0, (int)fileStream.Length);

                memStream.Seek(def.internalIdAddress, SeekOrigin.Begin);

                byte[] b = new byte[8];
                memStream.Read(b, 0, 8);
                string id = System.Text.Encoding.UTF8.GetString(b);
                DialogResult dialogResult = MessageBox.Show("Found Identifier: " + id +". Use this??", "Identifier", MessageBoxButtons.YesNo);
                if(dialogResult == DialogResult.Yes)
                {
                    def.CarInfo["internalidstring"] = id;
                    def.CarInfo["xmlid"] = id;
                    textBoxDefXml.Text = def.xRomId.ToString();

                }
                else if (dialogResult == DialogResult.No)
                {
                    return;
                }
            } 
        }

        private void comboBoxIncludeDef_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SharpTuner.AvailableDevices.DefDictionary.ContainsKey(comboBoxIncludeDef.SelectedItem.ToString()))
                def.include = comboBoxIncludeDef.SelectedItem.ToString();
            else
                def.include = null;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        { 
            if (checkBox1.Checked && SharpTuner.AvailableDevices.DefDictionary.ContainsKey(comboBoxCopyDef.SelectedItem.ToString()))
            {
                SharpTuner.AvailableDevices.DefDictionary[comboBoxCopyDef.SelectedItem.ToString()].Populate();
                def.CopyTables(SharpTuner.AvailableDevices.DefDictionary[comboBoxCopyDef.SelectedItem.ToString()]);//copy tables
            }
            //Save the definition XML
            try
            {
                def.xRomId = XElement.Parse(textBoxDefXml.Text);
                def.ParseRomId();
                def.include = comboBoxIncludeDef.SelectedValue.ToString();
            
            StringBuilder path = new StringBuilder();
            path.Append(SharpTuner.EcuFlashDefRepoPath + "/");
            if (def.CarInfo.ContainsKey("model") && def.CarInfo["model"] != null)
            {
                path.Append(def.CarInfo["model"].ToString());
                if (def.CarInfo.ContainsKey("submodel") && def.CarInfo["submodel"] != null)
                {
                    string s = " " + def.CarInfo["submodel"];
                    path.Append(s);
                }
                path.Append("/");
            }
            string dirpath = path.ToString();
            path.Append(def.internalId.ToString() + ".xml");
            if (!Directory.Exists(dirpath))
            {
                Directory.CreateDirectory(dirpath);
            }
            else if (File.Exists(path.ToString()))
            {
                DialogResult dialogResult = MessageBox.Show("Definition already exists at " + path.ToString() + System.Environment.NewLine + "Overwrite it??", "Warning", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.No)
                {
                    MessageBox.Show("Save definition aborted");
                    return;
                }
            }
            def.defPath = path.ToString();
            def.ExportXML();
            MessageBox.Show("Successfully saved definition to " + def.defPath);
            SharpTuner.PopulateAvailableDevices();
            this.Dispose();
            }
            catch (Exception er)
            {
                MessageBox.Show("Error: " + er.Message);
                return;
            }
        }

        private void comboBoxCopyDef_TextChanged(object sender, EventArgs e)
        {
            //TODO: this is bullshit, fix findDef();
        }

        private void findDef()
        {
            string s = null;
            if (defList.ContainsCI(comboBoxCopyDef.Text.ToString()))
            {
                s = defList.Find(x => x.ToString().ContainsCI(comboBoxCopyDef.Text.ToString()));
                if (s != null && s != comboBoxCopyDef.Text)
                {
                    comboBoxCopyDef.Text = s;
                }
                comboBoxCopyDef.SelectedValue = s;
            }
            updateXml();
        }

        private void updateXml()
        {
            if (SharpTuner.AvailableDevices.DefDictionary.ContainsKey(comboBoxCopyDef.SelectedItem.ToString()))
            {
                textBoxDefXml.Text = SharpTuner.AvailableDevices.DefDictionary[comboBoxCopyDef.SelectedItem.ToString()].xRomId.ToString();
                comboBoxIncludeDef.SelectedItem = comboBoxCopyDef.SelectedItem;
            }
            else
                textBoxDefXml.Text = defaultShortDef;
        }
    }
}

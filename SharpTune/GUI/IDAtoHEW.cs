using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using SharpTune;
using SharpTune.ConversionTools;

namespace SharpTune.GUI
{
    public partial class IDAtoHEW : Form
    {
        public IDAtoHEW()
        {
            InitializeComponent();
        }

        public List<string> mapoutputs = new List<string>() {"HEW (C header & section file)","IDA (IDC script)"};
        public List<string> headeroutputs = new List<string>() {"IDA (IDC script)"};
        public string mode = null;
        public string output;


        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "MAP/HEADER files (*.map; *.h)|*.map;*.h";
            DialogResult ret = SharpTune.Utils.STAShowOFDialog(ofd);
            if (ret == DialogResult.OK)
            {
                textBox1.Text = ofd.FileName;
                output = Path.GetDirectoryName(ofd.FileName);
            }
        }

        private void IDAtoHEW_Load(object sender, EventArgs e)
        {
            
        }

        //Console.WriteLine("Convert .map file to C defines header (.h) and section file (.txt) using .xml translation: IDAtoHEW <file.xml> <file.map> <file.h> <file.txt>");
        //        Console.WriteLine("Convert .map file to IDC script: IDAtoHEW <file.map> <file.idc>");
        //        Console.WriteLine("Convert .h file to IDC script: IDAtoHEW <file.h> <file.idc>");

        private void button2_Click(object sender, EventArgs e)
        {
            switch (mode)
            {
                case "header":
                    ConvTool.Run(new string[] { textBox1.Text, output + Path.GetFileName(textBox1.Text) + "_converted.idc" });
                    //call header->idc
                    break;

                case "map":
                    if (convertToComboBox.SelectedItem.ToString() == mapoutputs[0])
                    {
                        ConvTool.Run(new string[] { translationTextBox.Text, textBox1.Text, output + Path.GetFileName(textBox1.Text) + "_converted.h", output + Path.GetFileName(textBox1.Text) + "_converted_sections.txt" } );
                        break;
                        //call map->hew
                    }
                    else
                    {
                        ConvTool.Run(new string[] {textBox1.Text, output + Path.GetFileName(textBox1.Text) + "_converted.idc"});
                        //cal map>idc
                        break;
                    }
                default:
                    MessageBox.Show("Error! check your settings & files!!");
                    Console.WriteLine("Error! check your settings & files!!");
                    return;
            }
            MessageBox.Show("Finished Conversion!");
            Console.WriteLine("Finished Conversion!");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.ContainsCI(".map"))
            {
                convertToComboBox.DataSource = mapoutputs;
                mode = "map";
            }
            else
            {
                convertToComboBox.DataSource = headeroutputs;
                mode = "header";
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "XML Files (*.xml)|*xml";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                translationTextBox.Text = ofd.FileName;
                button2.Enabled = true;
            }
            else
            {
                translationTextBox.Clear();
                button2.Enabled = false;
            }
        }

        private void convertToComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (convertToComboBox.SelectedItem.ToString() == mapoutputs[0])
            {
                translationButton.Enabled = true;
                button2.Enabled = false;
            }
            else
            {
                translationButton.Enabled = false;
                button2.Enabled = true;
            }
        }

    }
}

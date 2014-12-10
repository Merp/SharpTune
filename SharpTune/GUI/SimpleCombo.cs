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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SharpTune
{
    public static class SimpleCombo
    {
        private static Form prompt;
        private static Label textLabel;
        private static ComboBox comboBox;
        private static Button confirmation;

        public static string ShowDialog(string text, string caption, List<string> comboitems)
        {
            prompt = new Form();
            prompt.SizeChanged += new System.EventHandler(SimpleCombo_SizeChanged);
            prompt.AutoSizeMode = AutoSizeMode.GrowOnly;
            prompt.Padding = new Padding(50);
            prompt.Text = caption;
            textLabel = new Label() { Text = text, AutoSize = true};
            textLabel.Left = (int)Math.Ceiling((double)(prompt.Width - textLabel.Width) * 0.5);
            textLabel.Top = (int)Math.Ceiling((double)(prompt.Height - textLabel.Height) * 0.25);
            comboBox = new ComboBox() { DataSource = comboitems, MinimumSize = new System.Drawing.Size(500,20)};
            comboBox.Left = (int)Math.Ceiling((double)(prompt.Width - comboBox.Width) * 0.5);
            comboBox.Top = (int)Math.Ceiling((double)(prompt.Height - comboBox.Height) * 0.5);
            confirmation = new Button() {Text = "OK" };
            confirmation.Left = (int)Math.Ceiling((double)(prompt.Width - confirmation.Width) * 0.5);
            confirmation.Top = (int)Math.Ceiling((double)(prompt.Height - confirmation.Height) * 0.75);
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(comboBox);
            prompt.AcceptButton = confirmation;
            prompt.AutoSize = true;
            prompt.Refresh();
            prompt.ShowDialog();
            return comboBox.SelectedItem.ToString();
        }

        private static void SimpleCombo_SizeChanged(object sender, EventArgs e)
        {
            textLabel.Left = (int)Math.Ceiling((double)(prompt.Width - textLabel.Width) * 0.5);
            textLabel.Top = (int)Math.Ceiling((double)(prompt.Height - textLabel.Height) * 0.25);
            comboBox.Left = (int)Math.Ceiling((double)(prompt.Width - comboBox.Width) * 0.5);
            comboBox.Top = (int)Math.Ceiling((double)(prompt.Height - comboBox.Height) * 0.5);
            confirmation.Left = (int)Math.Ceiling((double)(prompt.Width - confirmation.Width) * 0.5);
            confirmation.Top = (int)Math.Ceiling((double)(prompt.Height - confirmation.Height) * 0.75);
        }

    }

}

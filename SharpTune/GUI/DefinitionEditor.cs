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
        public DefinitionEditor()
        {
            InitializeComponent();

            //When this initializes, check for existing device and auto-select
            //If no device, default to all definitions

        }

        private void DefinitionEditor_Load(object sender, EventArgs e)
        {

        }
    }
}

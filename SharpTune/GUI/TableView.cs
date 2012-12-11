///*
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.

//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//*/

//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Windows.Forms;
//using SharpTune.Tables;
//using SharpTune.GUI;


//namespace SharpTune
//{
//    public partial class TableView : Form
//    {
//        private Label xAxisLabel { get; set; }
//        private Label yAxisLabel { get; set; }
//        private Label dataLabel { get; set; }
//        private DataTable displayTable { get; set; }

//        private Table parentTable { get; set; }

//        public TableView(ref Table3D table)
//        {
//            this.parentTable = table;
//            InitializeComponent();
//            this.PopulateTableView(table);        
//        }

//        public TableView(ref Table2D table)
//        {
//            this.parentTable = table;
//            InitializeComponent();
//            this.PopulateTableView(table);
//        }

//        public TableView(ref Table1D table)
//        {
//            this.parentTable = table;
//            InitializeComponent();
//            this.PopulateTableView(table);
//        }

//        private void TableView_Load(object sender, EventArgs e)
//        {
//            this.SuspendLayout();
//            this.SizeTableView(parentTable);

//            tableLayoutPanel1.PerformLayout();
//            tableLayoutPanel1.Refresh();
//            this.PerformAutoScale();
//            this.PerformLayout();
//            this.Refresh();
//            this.ResumeLayout();
//        }

//        private void customGrid1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
//        {
//            int check = dataGrid.Height;
//            check = dataGrid.Width;
//        }

//        private void customGrid1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
//        {
//            //Edit mode on cell has ended, update the byte values

//            //this.parentTable.ReadGui();
//            string newval = dataGrid[e.ColumnIndex, e.RowIndex].Value.ToString();
//            this.parentTable.displayValues[(e.RowIndex * this.parentTable.xAxis.elements) + e.ColumnIndex] = newval;
//            byte[] newbyteval = this.parentTable.scaling.fromDisplay(this.parentTable.displayValues[(e.RowIndex * this.parentTable.xAxis.elements) + e.ColumnIndex]);
//            this.parentTable.byteValues[(e.RowIndex * this.parentTable.xAxis.elements) + e.ColumnIndex] = newbyteval;
//            this.parentTable.Write();
//            //this.PopulateTableView(parentTable);
//            this.Refresh();
//        }

//        private void dataLabel_Click(object sender, EventArgs e)
//        {

//        }

//        public void PopulateTableView(Table3D tablep)
//        {
//            //TODO: Determine if X or Data has largest FORMAT!!!!

//            DeviceImage image = tablep.parentImage;
//            tablep.xAxis.Read(image);
//            tablep.yAxis.Read(image);
//            tablep.Read();

//            //populate labels
//            this.xAxisLabel.Text = tablep.xAxis.name + " (" + tablep.xAxis.scaling.units + ")";
//            this.yAxisLabel.Text = tablep.yAxis.name + " (" + tablep.yAxis.scaling.units + ")";
//            this.dataLabel.Text = tablep.name + " (" + tablep.scaling.units + ")";

//            //POPULATE AXES SOMEHOW

//            //add blank for row headers to reside
//            DataTable table = new DataTable();
//            //table.Columns.Add("", typeof(string));

//            for (int i = 1; i < tablep.xAxis.elements; i++)
//            {
//                table.Columns.Add(i.ToString(), typeof(string));
//            }

 
//            for (int i = 1; i < tablep.yAxis.elements; i++)
//            {
//                object[] row = new object[tablep.xAxis.elements -1];
//                for (int j = 1; j < tablep.xAxis.elements; j++)
//                {
//                    int index = tablep.xAxis.elements;
//                    index *= i-1;
//                    index += j-1;
//                    row[j-1] = tablep.displayValues[index];
//                }
//                table.Rows.Add(row);
//            }

//            tablep.dataTable = table;

//            this.dataGrid.DataSource = tablep.dataTable;

   
//        }

//        public void PopulateTableView(Table2D tablep)
//        {
//            //TODO: Determine if X or Data has largest FORMAT!!!!

//            DeviceImage image = tablep.parentImage;
//            tablep.yAxis.Read(image);
//            tablep.Read();

//            //populate labels
//            this.xAxisLabel.Text ="";
//            this.dataLabel.Text = tablep.name + " (" + tablep.scaling.units + ")";

//           //FIX tablep.yAxis.PopulateTableView(this);

//            //add blank for row headers to reside
//            DataTable table = new DataTable();
//            table.Columns.Add("", typeof(string));

//            for (int i = 0; i < tablep.yAxis.elements; i++)
//            {
//                object[] row = new object[1];
//                row[0] = tablep.displayValues[i];
//                table.Rows.Add(row);
//            }

//            tablep.dataTable = table;

//            this.dataGrid.DataSource = tablep.dataTable;
//        }

//        public void PopulateTableView(Table1D tablep)
//        {

//            //populate labels
//            this.xAxisLabel.Text = "";
//            this.dataLabel.Text = tablep.name + " (" + tablep.scaling.units + ")";
//            this.yAxisLabel.Text = "";

//            DeviceImage image = tablep.parentImage;
//            DataTable dataTable = new DataTable();
//            tablep.Read();

//            //add the column for single field
//            dataTable.Columns.Add("value", typeof(string));//, tt.xAxis.displayValues[i].ToString());


//            object[] hrow = new object[1];
//            hrow[0] = tablep.displayValues[0];

//            dataTable.Rows.Add(hrow);

//            tablep.dataTable = dataTable;

//            this.dataGrid.DataSource = tablep.dataTable;

//            int height = ((this.dataGrid.Rows.Count) * this.dataGrid.Rows[0].Height) + 1;
//            int width = 1;
//            foreach (DataGridViewColumn col in this.dataGrid.Columns)
//            {
//                //  if (col.Index != 0)
//                {
//                    width += col.Width;
//                }
//            }
//            this.dataGrid.Width = width;
//            this.dataGrid.Height = height;
//            this.tableLayoutPanel1.RowStyles[2].Height = height + this.dataGrid.Margin.Vertical;
//            this.tableLayoutPanel1.ColumnStyles[2].Width = width + this.dataGrid.Margin.Horizontal;
//            //this.dataGrid.Rows[0].DefaultCellStyle.BackColor = Color.Cyan;
//            //this.dataGrid.Columns[0].DefaultCellStyle.BackColor = Color.AliceBlue;
//        }

//        public void PopulateTableView(RamTable3D table)
//        {
//        }

//        public void PopulateTableView(RamTable2D table)
//        {
//        }

//        public void PopulateTableView(RamTable1D table)
//        {
//        }
//        public void SizeTable1DView(Table tablep)
//        {
//            this.dataGrid.PerformLayout();
//            this.dataGrid.Refresh();

//            this.xAxisGrid.Visible = false;
//            this.xAxisGrid.Height = 0;
//            this.xAxisGrid.Width = 0;
//            this.yAxisGrid.Height = 0;
//            this.yAxisGrid.Width = 0;
//            this.tableLayoutPanel1.RowStyles[0].Height = 0;
//            this.tableLayoutPanel1.RowStyles[1].Height = 0;
//            this.tableLayoutPanel1.RowStyles[1].Height = 0;
//            this.tableLayoutPanel1.ColumnStyles[0].Width = 0;

//            int height = ((this.dataGrid.Rows.Count) * this.dataGrid.Rows[0].Height) + 1;
//            int width = 1;
//            foreach (DataGridViewColumn col in this.dataGrid.Columns)
//            {
//                //if (col.Index != 0)
//                {
//                    width += col.Width;
//                }
//            }
//            this.dataGrid.Width = width;
//            this.dataGrid.Height = height;
//            this.tableLayoutPanel1.RowStyles[2].Height = height + this.dataGrid.Margin.Vertical;
//            this.tableLayoutPanel1.ColumnStyles[2].Width = width + this.dataGrid.Margin.Horizontal;
//            //this.dataGrid.Rows[0].DefaultCellStyle.BackColor = Color.Cyan;
//            //this.dataGrid.Columns[0].DefaultCellStyle.BackColor = Color.AliceBlue;
//        }

//        public void SizeTable2DView(Table tablep)
//        {
//            this.dataGrid.PerformLayout();
//            this.dataGrid.Refresh();

//            //tablep.yAxis.Sizethis(this);

//            this.xAxisGrid.Visible = false;
//            this.xAxisGrid.Height = 0;
//            this.xAxisGrid.Width = 0;
//            this.tableLayoutPanel1.RowStyles[0].Height = 0;
//            this.tableLayoutPanel1.RowStyles[1].Height = 0;




//            int height = 0;
//            foreach (DataGridViewRow row in this.dataGrid.Rows)
//            {
//                height += row.Height;
//            }
//            int width = this.dataGrid.Columns[0].Width;

//            this.dataGrid.Width = width;
//            this.dataGrid.Height = height;
//            this.tableLayoutPanel1.RowStyles[2].Height = this.dataGrid.Height + this.dataGrid.Margin.Vertical;
//            this.tableLayoutPanel1.ColumnStyles[2].Width = this.dataGrid.Width + this.dataGrid.Margin.Horizontal;
//            this.dataGrid.BringToFront();
//            //this.dataGrid.Rows[0].DefaultCellStyle.BackColor = Color.Cyan;
//            //this.dataGrid.Columns[0].DefaultCellStyle.BackColor = Color.AliceBlue;
//        }


//        public void SizeTable3DView(Table tablep)
//        {
//            this.dataGrid.PerformLayout();
//            this.dataGrid.Refresh();
//            //tablep.yAxis.Sizethis(this);
//            //tablep.xAxis.Sizethis(this);


//            int height = 0;
//            foreach (DataGridViewRow row in this.dataGrid.Rows)
//            {
//                height += row.Height;
//            }
//            int width = 0;
//            foreach (DataGridViewColumn col in this.dataGrid.Columns)
//            {
//                //if (col.Index != 0)
//                {
//                    col.Width = this.xAxisGrid.Columns[0].Width;
//                    width += col.Width;
//                }
//            }
//            this.dataGrid.Width = width;
//            this.dataGrid.Height = height;
//            this.tableLayoutPanel1.RowStyles[2].Height = height + this.dataGrid.Margin.Vertical;
//            this.tableLayoutPanel1.ColumnStyles[2].Width = width + this.dataGrid.Margin.Horizontal; ;
//            //this.dataGrid.Rows[0].DefaultCellStyle.BackColor = Color.Cyan;
//            //this.dataGrid.Columns[0].DefaultCellStyle.BackColor = Color.AliceBlue;
//        }

//        public void SizeXaxis()
//        {
//            this.xAxisGrid.PerformLayout();
//            this.xAxisGrid.Refresh();

//            double height = 0;
//            double max = 0;
//            foreach (DataGridViewRow row in this.xAxisGrid.Rows)
//            {
//                height += row.Height + 0.5;
//            }
//            double width = 0;
//            foreach (DataGridViewColumn col in this.xAxisGrid.Columns)
//            {
//                //if (col.Index != 0)
//                {

//                    if (col.Width > max)
//                    {
//                        max = col.Width;
//                    }
//                }
//            }
//            foreach (DataGridViewColumn col in this.xAxisGrid.Columns)
//            {
//                //This does not work, fuck microsoft.
//                col.Width = (int)max;
//                width += col.Width + 0.5;
//            }

//            this.xAxisGrid.Width = (int)width;
//            this.xAxisGrid.Height = (int)height;
//            this.tableLayoutPanel1.RowStyles[1].Height = (int)height + this.xAxisGrid.Margin.Vertical;
//            this.tableLayoutPanel1.ColumnStyles[2].Width = (int)width;
//            //this.dataGrid.Rows[0].DefaultCellStyle.BackColor = Color.Cyan;
//            //tableView.dataGrid.Columns[0].DefaultCellStyle.BackColor = Color.AliceBlue;
//        }

//        public void PopulateTableViewStaticYAxis(StaticYAxis syaxis)
//        {

//            DeviceImage image = syaxis.parentTable.parentImage;
//            syaxis.Read(image);

//            //add blank for row headers to reside
//            DataTable table = new DataTable();
//            for (int i = 0; i < syaxis.elements; i++)
//            {
//                table.Columns.Add("", typeof(string));
//            }
//            object[] row = new object[syaxis.elements];
//            for (int i = 0; i < syaxis.elements; i++)
//            {

//                row[i] = syaxis.displayValues[i].ToString();
//            }

//            table.Rows.Add(row);
//            syaxis.dataTable = table;
//            this.yAxisGrid.DataSource = syaxis.dataTable;

//            //Hide yaxis label for static tables
//            this.yAxisLabel.Text = "";
//            this.tableLayoutPanel1.ColumnStyles[0].Width = 0;


//        }

//        public void PopulateTableViewXAxis(XAxis xaxis)
//        {

//            DeviceImage image = xaxis.parentTable.parentImage;
//            xaxis.Read(image);

//            //add blank for row headers to reside
//            DataTable table = new DataTable();
//            for (int i = 0; i < xaxis.elements; i++)
//            {
//                table.Columns.Add("", typeof(string));
//            }
//            object[] row = new object[xaxis.elements];
//            for (int i = 0; i < xaxis.elements; i++)
//            {

//                row[i] = xaxis.displayValues[i].ToString();
//            }

//            table.Rows.Add(row);
//            xaxis.dataTable = table;
//            this.xAxisGrid.DataSource = xaxis.dataTable;
//            this.xAxisLabel.Text = xaxis.name + " (" + xaxis.scaling.units + ")";

//        }

//        public void PopulateTableViewYAxis(YAxis yaxis)
//        {

//            DeviceImage image = yaxis.parentTable.parentImage;
//            yaxis.Read(image);

//            //add blank for row headers to reside
//            DataTable table = new DataTable();
//            table.Columns.Add("", typeof(string));

//            for (int i = 0; i < yaxis.elements; i++)
//            {
//                object[] row = new object[1];
//                row[0] = yaxis.displayValues[i].ToString();
//                table.Rows.Add(row);
//            }

//            yaxis.dataTable = table;
//            this.yAxisGrid.DataSource = yaxis.dataTable;
//            this.yAxisLabel.Text = yaxis.name + " (" + yaxis.scaling.units + ")";

//        }

//        public void SizeTableView(Table tablep)
//        {

//            this.yAxisGrid.PerformLayout();
//            this.yAxisGrid.Refresh();

//            int height = 0;
//            foreach (DataGridViewRow row in this.yAxisGrid.Rows)
//            {
//                height += row.Height;
//            }
//            int width = 0;
//            foreach (DataGridViewColumn col in this.yAxisGrid.Columns)
//            {
//                //if (col.Index != 0)
//                {
//                    width += col.Width;
//                }
//            }
//            this.yAxisGrid.Width = width;
//            this.yAxisGrid.Height = height;
//            this.tableLayoutPanel1.RowStyles[2].Height = height + this.yAxisGrid.Margin.Vertical;
//            this.tableLayoutPanel1.ColumnStyles[1].Width = width + this.yAxisGrid.Margin.Vertical;
//            //tableView.dataGrid.Rows[0].DefaultCellStyle.BackColor = Color.Cyan;
//            //tableView.dataGrid.Columns[0].DefaultCellStyle.BackColor = Color.AliceBlue;
//        }
//    }
//}

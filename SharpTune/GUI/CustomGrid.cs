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
using System.Drawing;

namespace SharpTune.GUI
{
    public class CustomGrid : DataGridView
    {
        public CustomGrid()
        {
            this.RowTemplate = new DataGridViewCustomRow();
            this.EnableHeadersVisualStyles = true;
            //this.AutoSize = true;
            this.DefaultCellStyle.Font = new Font("Arial", 9F, GraphicsUnit.Pixel);
        }

        protected override Padding DefaultPadding
        {
            get
            {
                return new Padding(0);
            }
        }
        public override DataGridViewAdvancedBorderStyle AdjustedTopLeftHeaderBorderStyle
        {
            get
            {
                DataGridViewAdvancedBorderStyle newStyle =
                    new DataGridViewAdvancedBorderStyle();
                newStyle.Top = DataGridViewAdvancedCellBorderStyle.None;
                newStyle.Left = DataGridViewAdvancedCellBorderStyle.None;
                newStyle.Bottom = DataGridViewAdvancedCellBorderStyle.Outset;
                newStyle.Right = DataGridViewAdvancedCellBorderStyle.OutsetDouble;
                return newStyle;
            }
        }

        public override DataGridViewAdvancedBorderStyle AdjustColumnHeaderBorderStyle(
            DataGridViewAdvancedBorderStyle dataGridViewAdvancedBorderStyleInput,
            DataGridViewAdvancedBorderStyle dataGridViewAdvancedBorderStylePlaceHolder,
            bool firstDisplayedColumn,
            bool lastVisibleColumn)
        {
            // Customize the left border of the first column header and the
            // bottom border of all the column headers. Use the input style for 
            // all other borders.
            dataGridViewAdvancedBorderStylePlaceHolder.Left = firstDisplayedColumn ?
                DataGridViewAdvancedCellBorderStyle.OutsetDouble :
                DataGridViewAdvancedCellBorderStyle.None;
            dataGridViewAdvancedBorderStylePlaceHolder.Bottom =
                DataGridViewAdvancedCellBorderStyle.Single;

            dataGridViewAdvancedBorderStylePlaceHolder.Right =
                dataGridViewAdvancedBorderStyleInput.Right;
            dataGridViewAdvancedBorderStylePlaceHolder.Top =
                dataGridViewAdvancedBorderStyleInput.Top;

            return dataGridViewAdvancedBorderStylePlaceHolder;
        }
    }

    public class DataGridViewCustomColumn : DataGridViewColumn
    {
        public DataGridViewCustomColumn()
        {
            this.CellTemplate = new DataGridViewCustomCell();
        }
    }

    public class DataGridViewCustomCell : DataGridViewTextBoxCell
    {
        public override DataGridViewAdvancedBorderStyle AdjustCellBorderStyle(
            DataGridViewAdvancedBorderStyle dataGridViewAdvancedBorderStyleInput,
            DataGridViewAdvancedBorderStyle dataGridViewAdvancedBorderStylePlaceHolder,
            bool singleVerticalBorderAdded,
            bool singleHorizontalBorderAdded,
            bool firstVisibleColumn,
            bool firstVisibleRow)
        {
            // Customize the top border of cells in the first row and the 
            // right border of cells in the first column. Use the input style 
            // for all other borders.
            dataGridViewAdvancedBorderStylePlaceHolder.Left = firstVisibleColumn ?
                DataGridViewAdvancedCellBorderStyle.OutsetDouble :
                DataGridViewAdvancedCellBorderStyle.None;
            dataGridViewAdvancedBorderStylePlaceHolder.Top = firstVisibleRow ?
                DataGridViewAdvancedCellBorderStyle.InsetDouble :
                DataGridViewAdvancedCellBorderStyle.None;

            dataGridViewAdvancedBorderStylePlaceHolder.Right =
                dataGridViewAdvancedBorderStyleInput.Right;
            dataGridViewAdvancedBorderStylePlaceHolder.Bottom =
                dataGridViewAdvancedBorderStyleInput.Bottom;

            return dataGridViewAdvancedBorderStylePlaceHolder;
        }
    }

    public class DataGridViewCustomRow : DataGridViewRow
    {
        public override DataGridViewAdvancedBorderStyle AdjustRowHeaderBorderStyle(
            DataGridViewAdvancedBorderStyle dataGridViewAdvancedBorderStyleInput,
            DataGridViewAdvancedBorderStyle dataGridViewAdvancedBorderStylePlaceHolder,
            bool singleVerticalBorderAdded,
            bool singleHorizontalBorderAdded,
            bool isFirstDisplayedRow,
            bool isLastDisplayedRow)
        {
            // Customize the top border of the first row header and the
            // right border of all the row headers. Use the input style for 
            // all other borders.
            dataGridViewAdvancedBorderStylePlaceHolder.Top = isFirstDisplayedRow ?
                DataGridViewAdvancedCellBorderStyle.InsetDouble :
                DataGridViewAdvancedCellBorderStyle.None;
            dataGridViewAdvancedBorderStylePlaceHolder.Right =
                DataGridViewAdvancedCellBorderStyle.OutsetDouble;

            dataGridViewAdvancedBorderStylePlaceHolder.Left =
                dataGridViewAdvancedBorderStyleInput.Left;
            dataGridViewAdvancedBorderStylePlaceHolder.Bottom =
                dataGridViewAdvancedBorderStyleInput.Bottom;

            return dataGridViewAdvancedBorderStylePlaceHolder;
        }
    }
}

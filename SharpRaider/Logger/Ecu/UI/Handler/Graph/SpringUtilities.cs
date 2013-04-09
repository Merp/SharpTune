/*
 * This code is derived from the Java version of RomRaider
 *
 * RomRaider Open-Source Tuning, Logging and Reflashing
 * Copyright (C) 2006-2012 RomRaider.com
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License along
 * with this program; if not, write to the Free Software Foundation, Inc.,
 * 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using System;
using Java.Awt;
using Javax.Swing;
using Sharpen;

namespace RomRaider.Logger.Ecu.UI.Handler.Graph
{
	/// <summary>
	/// A 1.4 file that provides utility methods for
	/// creating form- or grid-style layouts with SpringLayout.
	/// </summary>
	/// <remarks>
	/// A 1.4 file that provides utility methods for
	/// creating form- or grid-style layouts with SpringLayout.
	/// These utilities are used by several programs, such as
	/// SpringBox and SpringCompactGrid.
	/// </remarks>
	public class SpringUtilities
	{
		private static readonly Org.Apache.Log4j.Logger LOGGER = Org.Apache.Log4j.Logger.
			GetLogger(typeof(RomRaider.Logger.Ecu.UI.Handler.Graph.SpringUtilities));

		public SpringUtilities()
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// A debugging utility that prints to stdout the component's
		/// minimum, preferred, and maximum sizes.
		/// </summary>
		/// <remarks>
		/// A debugging utility that prints to stdout the component's
		/// minimum, preferred, and maximum sizes.
		/// </remarks>
		public static void PrintSizes(Component c)
		{
			LOGGER.Debug("minimumSize = " + c.GetMinimumSize());
			LOGGER.Debug("preferredSize = " + c.GetPreferredSize());
			LOGGER.Debug("maximumSize = " + c.GetMaximumSize());
		}

		/// <summary>
		/// Aligns the first <code>rows</code> * <code>cols</code>
		/// components of <code>parent</code> in
		/// a grid.
		/// </summary>
		/// <remarks>
		/// Aligns the first <code>rows</code> * <code>cols</code>
		/// components of <code>parent</code> in
		/// a grid. Each component is as big as the maximum
		/// preferred width and height of the components.
		/// The parent is made just big enough to fit them all.
		/// </remarks>
		/// <param name="rows">number of rows</param>
		/// <param name="cols">number of columns</param>
		/// <param name="initialX">x location to start the grid at</param>
		/// <param name="initialY">y location to start the grid at</param>
		/// <param name="xPad">x padding between cells</param>
		/// <param name="yPad">y padding between cells</param>
		public static void MakeGrid(Container parent, int rows, int cols, int initialX, int
			 initialY, int xPad, int yPad)
		{
			SpringLayout layout;
			try
			{
				layout = (SpringLayout)parent.GetLayout();
			}
			catch (InvalidCastException)
			{
				LOGGER.Error("The first argument to makeGrid must use SpringLayout.");
				return;
			}
			Spring xPadSpring = Spring.Constant(xPad);
			Spring yPadSpring = Spring.Constant(yPad);
			Spring initialXSpring = Spring.Constant(initialX);
			Spring initialYSpring = Spring.Constant(initialY);
			int max = rows * cols;
			//Calculate Springs that are the max of the width/height so that all
			//cells have the same size.
			Spring maxWidthSpring = layout.GetConstraints(parent.GetComponent(0)).GetWidth();
			Spring maxHeightSpring = layout.GetConstraints(parent.GetComponent(0)).GetWidth();
			for (int i = 1; i < max; i++)
			{
				SpringLayout.Constraints cons = layout.GetConstraints(parent.GetComponent(i));
				maxWidthSpring = Spring.Max(maxWidthSpring, cons.GetWidth());
				maxHeightSpring = Spring.Max(maxHeightSpring, cons.GetHeight());
			}
			//Apply the new width/height Spring. This forces all the
			//components to have the same size.
			for (int i_1 = 0; i_1 < max; i_1++)
			{
				SpringLayout.Constraints cons = layout.GetConstraints(parent.GetComponent(i_1));
				cons.SetWidth(maxWidthSpring);
				cons.SetHeight(maxHeightSpring);
			}
			//Then adjust the x/y constraints of all the cells so that they
			//are aligned in a grid.
			SpringLayout.Constraints lastCons = null;
			SpringLayout.Constraints lastRowCons = null;
			for (int i_2 = 0; i_2 < max; i_2++)
			{
				SpringLayout.Constraints cons = layout.GetConstraints(parent.GetComponent(i_2));
				if (i_2 % cols == 0)
				{
					//start of new row
					lastRowCons = lastCons;
					cons.SetX(initialXSpring);
				}
				else
				{
					//x position depends on previous component
					cons.SetX(Spring.Sum(lastCons.GetConstraint(SpringLayout.EAST), xPadSpring));
				}
				if (i_2 / cols == 0)
				{
					//first row
					cons.SetY(initialYSpring);
				}
				else
				{
					//y position depends on previous row
					cons.SetY(Spring.Sum(lastRowCons.GetConstraint(SpringLayout.SOUTH), yPadSpring));
				}
				lastCons = cons;
			}
			//Set the parent's size.
			SpringLayout.Constraints pCons = layout.GetConstraints(parent);
			pCons.SetConstraint(SpringLayout.SOUTH, Spring.Sum(Spring.Constant(yPad), lastCons
				.GetConstraint(SpringLayout.SOUTH)));
			pCons.SetConstraint(SpringLayout.EAST, Spring.Sum(Spring.Constant(xPad), lastCons
				.GetConstraint(SpringLayout.EAST)));
		}

		private static SpringLayout.Constraints GetConstraintsForCell(int row, int col, Container
			 parent, int cols)
		{
			SpringLayout layout = (SpringLayout)parent.GetLayout();
			Component c = parent.GetComponent(row * cols + col);
			return layout.GetConstraints(c);
		}

		/// <summary>
		/// Aligns the first <code>rows</code> * <code>cols</code>
		/// components of <code>parent</code> in
		/// a grid.
		/// </summary>
		/// <remarks>
		/// Aligns the first <code>rows</code> * <code>cols</code>
		/// components of <code>parent</code> in
		/// a grid. Each component in a column is as wide as the maximum
		/// preferred width of the components in that column;
		/// height is similarly determined for each row.
		/// The parent is made just big enough to fit them all.
		/// </remarks>
		/// <param name="rows">number of rows</param>
		/// <param name="cols">number of columns</param>
		/// <param name="initialX">x location to start the grid at</param>
		/// <param name="initialY">y location to start the grid at</param>
		/// <param name="xPad">x padding between cells</param>
		/// <param name="yPad">y padding between cells</param>
		public static void MakeCompactGrid(Container parent, int rows, int cols, int initialX
			, int initialY, int xPad, int yPad)
		{
			SpringLayout layout;
			try
			{
				layout = (SpringLayout)parent.GetLayout();
			}
			catch (InvalidCastException)
			{
				LOGGER.Error("The first argument to makeCompactGrid must use SpringLayout.");
				return;
			}
			//Align all cells in each column and make them the same width.
			Spring x = Spring.Constant(initialX);
			for (int c = 0; c < cols; c++)
			{
				Spring width = Spring.Constant(0);
				for (int r = 0; r < rows; r++)
				{
					width = Spring.Max(width, GetConstraintsForCell(r, c, parent, cols).GetWidth());
				}
				for (int r_1 = 0; r_1 < rows; r_1++)
				{
					SpringLayout.Constraints constraints = GetConstraintsForCell(r_1, c, parent, cols
						);
					constraints.SetX(x);
					constraints.SetWidth(width);
				}
				x = Spring.Sum(x, Spring.Sum(width, Spring.Constant(xPad)));
			}
			//Align all cells in each row and make them the same height.
			Spring y = Spring.Constant(initialY);
			for (int r_2 = 0; r_2 < rows; r_2++)
			{
				Spring height = Spring.Constant(0);
				for (int c_1 = 0; c_1 < cols; c_1++)
				{
					height = Spring.Max(height, GetConstraintsForCell(r_2, c_1, parent, cols).GetHeight
						());
				}
				for (int c_2 = 0; c_2 < cols; c_2++)
				{
					SpringLayout.Constraints constraints = GetConstraintsForCell(r_2, c_2, parent, cols
						);
					constraints.SetY(y);
					constraints.SetHeight(height);
				}
				y = Spring.Sum(y, Spring.Sum(height, Spring.Constant(yPad)));
			}
			//Set the parent's size.
			SpringLayout.Constraints pCons = layout.GetConstraints(parent);
			pCons.SetConstraint(SpringLayout.SOUTH, y);
			pCons.SetConstraint(SpringLayout.EAST, x);
		}
	}
}

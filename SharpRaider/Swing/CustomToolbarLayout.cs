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
using Sharpen;

namespace RomRaider.Swing
{
	[System.Serializable]
	public class CustomToolbarLayout : FlowLayout
	{
		private const long serialVersionUID = 1L;

		public CustomToolbarLayout() : base()
		{
		}

		public CustomToolbarLayout(int align) : base(align)
		{
		}

		public CustomToolbarLayout(int align, int hgap, int vgap) : base(align, hgap, vgap
			)
		{
		}

		public override Dimension MinimumLayoutSize(Container target)
		{
			// Size of largest component, so we can resize it in
			// either direction with something like a split-pane.
			return ComputeMinSize(target);
		}

		public override Dimension PreferredLayoutSize(Container target)
		{
			return ComputeSize(target);
		}

		private Dimension ComputeSize(Container target)
		{
			lock (target.GetTreeLock())
			{
				int hgap = GetHgap();
				int vgap = GetVgap();
				int width = target.GetWidth();
				// Let this behave like a regular FlowLayout (single row)
				// if the container hasn't been assigned any size yet
				if (0 == width)
				{
					width = int.MaxValue;
				}
				Insets insets = target.GetInsets();
				if (null == insets)
				{
					insets = new Insets(0, 0, 0, 0);
				}
				int reqdWidth = 0;
				int maxwidth = width - (insets.left + insets.right + hgap * 2);
				int n = target.GetComponentCount();
				int x = 0;
				int y = insets.top + vgap;
				// FlowLayout starts by adding vgap, so do that here too.
				int rowHeight = 0;
				for (int i = 0; i < n; i++)
				{
					Component c = target.GetComponent(i);
					if (c.IsVisible())
					{
						Dimension d = c.GetPreferredSize();
						if ((x == 0) || ((x + d.width) <= maxwidth))
						{
							// fits in current row.
							if (x > 0)
							{
								x += hgap;
							}
							x += d.width;
							rowHeight = Math.Max(rowHeight, d.height);
						}
						else
						{
							// Start of new row
							x = d.width;
							y += vgap + rowHeight;
							rowHeight = d.height;
						}
						reqdWidth = Math.Max(reqdWidth, x);
					}
				}
				y += rowHeight;
				y += insets.bottom;
				return new Dimension(reqdWidth + insets.left + insets.right, y);
			}
		}

		private Dimension ComputeMinSize(Container target)
		{
			lock (target.GetTreeLock())
			{
				int minx = int.MaxValue;
				int miny = int.MinValue;
				bool found_one = false;
				int n = target.GetComponentCount();
				for (int i = 0; i < n; i++)
				{
					Component c = target.GetComponent(i);
					if (c.IsVisible())
					{
						found_one = true;
						Dimension d = c.GetPreferredSize();
						minx = Math.Min(minx, d.width);
						miny = Math.Min(miny, d.height);
					}
				}
				if (found_one)
				{
					return new Dimension(minx, miny);
				}
				return new Dimension(0, 0);
			}
		}
	}
}

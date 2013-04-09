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

namespace RomRaider.Logger.Ecu.UI.Swing.Layout
{
	[System.Serializable]
	public sealed class BetterFlowLayout : FlowLayout
	{
		private const long serialVersionUID = -6784712723817241270L;

		public BetterFlowLayout() : base()
		{
		}

		public BetterFlowLayout(int align) : base(align)
		{
		}

		public BetterFlowLayout(int align, int hgap, int vgap) : base(align, hgap, vgap)
		{
		}

		public override Dimension PreferredLayoutSize(Container target)
		{
			return BetterPreferredSize(target);
		}

		public override Dimension MinimumLayoutSize(Container target)
		{
			return BetterPreferredSize(target);
		}

		public Dimension BetterPreferredSize(Container target)
		{
			lock (target.GetTreeLock())
			{
				Insets insets = target.GetInsets();
				int maxwidth = target.GetWidth() - (insets.left + insets.right + GetHgap() * 2);
				int nmembers = target.GetComponentCount();
				int x = 0;
				int y = insets.top + GetVgap();
				int rowh = 0;
				for (int i = 0; i < nmembers; i++)
				{
					Component m = target.GetComponent(i);
					if (m.IsVisible())
					{
						Dimension d = m.GetPreferredSize();
						m.SetSize(d.width, d.height);
						if ((x == 0) || ((x + d.width) <= maxwidth))
						{
							if (x > 0)
							{
								x += GetHgap();
							}
							x += d.width;
							rowh = Math.Max(rowh, d.height);
						}
						else
						{
							x = d.width;
							y += GetVgap() + rowh;
							rowh = d.height;
						}
					}
				}
				return new Dimension(maxwidth, y + rowh + GetVgap());
			}
		}
	}
}

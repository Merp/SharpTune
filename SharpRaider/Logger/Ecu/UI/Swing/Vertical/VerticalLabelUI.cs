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
using Java.Awt.Geom;
using Javax.Swing;
using Javax.Swing.Plaf.Basic;
using Sharpen;

namespace RomRaider.Logger.Ecu.UI.Swing.Vertical
{
	public sealed class VerticalLabelUI : BasicLabelUI
	{
		private static Rectangle PAINT_ICON_R = new Rectangle();

		private static Rectangle PAINT_TEXT_R = new Rectangle();

		private static Rectangle PAINT_VIEW_R = new Rectangle();

		private static Insets PAINT_VIEW_INSETS = new Insets(0, 0, 0, 0);

		protected internal bool clockwise;

		static VerticalLabelUI()
		{
			labelUI = new RomRaider.Logger.Ecu.UI.Swing.Vertical.VerticalLabelUI(false);
		}

		public VerticalLabelUI(bool clockwise) : base()
		{
			this.clockwise = clockwise;
		}

		public override Dimension GetPreferredSize(JComponent c)
		{
			Dimension dim = base.GetPreferredSize(c);
			return new Dimension(dim.height, dim.width);
		}

		public override void Paint(Graphics graphics, JComponent component)
		{
			JLabel label = (JLabel)component;
			string text = label.GetText();
			Icon icon = (label.IsEnabled()) ? label.GetIcon() : label.GetDisabledIcon();
			if ((icon == null) && (text == null))
			{
				return;
			}
			FontMetrics fm = graphics.GetFontMetrics();
			PAINT_VIEW_INSETS = component.GetInsets(PAINT_VIEW_INSETS);
			PAINT_VIEW_R.x = PAINT_VIEW_INSETS.left;
			PAINT_VIEW_R.y = PAINT_VIEW_INSETS.top;
			// Use inverted height & width
			PAINT_VIEW_R.height = component.GetWidth() - (PAINT_VIEW_INSETS.left + PAINT_VIEW_INSETS
				.right);
			PAINT_VIEW_R.width = component.GetHeight() - (PAINT_VIEW_INSETS.top + PAINT_VIEW_INSETS
				.bottom);
			PAINT_ICON_R.x = PAINT_ICON_R.y = PAINT_ICON_R.width = PAINT_ICON_R.height = 0;
			PAINT_TEXT_R.x = PAINT_TEXT_R.y = PAINT_TEXT_R.width = PAINT_TEXT_R.height = 0;
			string clippedText = LayoutCL(label, fm, text, icon, PAINT_VIEW_R, PAINT_ICON_R, 
				PAINT_TEXT_R);
			int textWidth = fm.StringWidth(clippedText);
			Graphics2D g2 = (Graphics2D)graphics;
			AffineTransform tr = g2.GetTransform();
			if (clockwise)
			{
				g2.Rotate(Math.PI / 2);
				g2.Translate(component.GetHeight() / 2 - textWidth / 2, -component.GetWidth());
			}
			else
			{
				g2.Rotate(-Math.PI / 2);
				g2.Translate(-component.GetHeight() / 2 - textWidth / 2, 0);
			}
			if (icon != null)
			{
				icon.PaintIcon(component, graphics, PAINT_ICON_R.x, PAINT_ICON_R.y);
			}
			if (text != null)
			{
				int textX = PAINT_TEXT_R.x;
				int textY = PAINT_TEXT_R.y + fm.GetAscent();
				if (label.IsEnabled())
				{
					PaintEnabledText(label, graphics, clippedText, textX, textY);
				}
				else
				{
					PaintDisabledText(label, graphics, clippedText, textX, textY);
				}
			}
			g2.SetTransform(tr);
		}
	}
}

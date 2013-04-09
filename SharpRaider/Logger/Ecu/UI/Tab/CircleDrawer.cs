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

using Java.Awt;
using Java.Awt.Geom;
using Org.Jfree.UI;
using Sharpen;

namespace RomRaider.Logger.Ecu.UI.Tab
{
	/// <summary>An implementation of the {Drawable} interface.</summary>
	/// <remarks>An implementation of the {Drawable} interface.</remarks>
	/// <author>David Gilbert</author>
	public class CircleDrawer : Drawable
	{
		/// <summary>The outline paint.</summary>
		/// <remarks>The outline paint.</remarks>
		private Paint outlinePaint;

		/// <summary>The outline stroke.</summary>
		/// <remarks>The outline stroke.</remarks>
		private Stroke outlineStroke;

		/// <summary>The fill paint.</summary>
		/// <remarks>The fill paint.</remarks>
		private Paint fillPaint;

		/// <summary>Creates a new instance.</summary>
		/// <remarks>Creates a new instance.</remarks>
		/// <param name="outlinePaint">the outline paint.</param>
		/// <param name="outlineStroke">the outline stroke.</param>
		/// <param name="fillPaint">the fill paint.</param>
		public CircleDrawer(Paint outlinePaint, Stroke outlineStroke, Paint fillPaint)
		{
			this.outlinePaint = outlinePaint;
			this.outlineStroke = outlineStroke;
			this.fillPaint = fillPaint;
		}

		/// <summary>Draws the circle.</summary>
		/// <remarks>Draws the circle.</remarks>
		/// <param name="g2">the graphics device.</param>
		/// <param name="area">the area in which to draw.</param>
		public virtual void Draw(Graphics2D g2, Rectangle2D area)
		{
			Ellipse2D ellipse = new Ellipse2D.Double(area.GetX(), area.GetY(), area.GetWidth(
				), area.GetHeight());
			if (this.fillPaint != null)
			{
				g2.SetPaint(this.fillPaint);
				g2.Fill(ellipse);
			}
			if (this.outlinePaint != null && this.outlineStroke != null)
			{
				g2.SetPaint(this.outlinePaint);
				g2.SetStroke(this.outlineStroke);
				g2.Draw(ellipse);
			}
			g2.SetPaint(Color.black);
			g2.SetStroke(new BasicStroke(1.0f));
			Line2D line1 = new Line2D.Double(area.GetCenterX(), area.GetMinY(), area.GetCenterX
				(), area.GetMaxY());
			Line2D line2 = new Line2D.Double(area.GetMinX(), area.GetCenterY(), area.GetMaxX(
				), area.GetCenterY());
			g2.Draw(line1);
			g2.Draw(line2);
		}
	}
}

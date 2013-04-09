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
using Java.Awt.Event;
using Javax.Swing;
using RomRaider.Net;
using Sharpen;

namespace RomRaider.Net
{
	[System.Serializable]
	public class URL : JLabel, MouseListener
	{
		private const long serialVersionUID = 8972482185527165793L;

		internal string url = string.Empty;

		public URL(string url) : base(url)
		{
			this.url = url;
			this.SetFont(new Font("Arial", Font.PLAIN, 12));
			this.AddMouseListener(this);
		}

		public override void Paint(Graphics g)
		{
			base.Paint(g);
			Font f = GetFont();
			FontMetrics fm = GetFontMetrics(f);
			int x1 = 0;
			int y1 = fm.GetHeight() + 3;
			int x2 = fm.StringWidth(GetText());
			if (GetText().Length > 0)
			{
				g.DrawLine(x1, y1, x2, y1);
			}
		}

		public virtual void MouseClicked(MouseEvent e)
		{
			BrowserControl.DisplayURL(url);
		}

		public virtual void MousePressed(MouseEvent e)
		{
		}

		public virtual void MouseReleased(MouseEvent e)
		{
		}

		public virtual void MouseEntered(MouseEvent e)
		{
		}

		public virtual void MouseExited(MouseEvent e)
		{
		}
	}
}

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
using Java.Beans;
using Javax.Swing;
using RomRaider.Editor.Ecu;
using RomRaider.Swing;
using Sharpen;

namespace RomRaider.Swing
{
	/// <summary>An extension of WDesktopPane that supports often used MDI functionality.
	/// 	</summary>
	/// <remarks>
	/// An extension of WDesktopPane that supports often used MDI functionality. This
	/// class also handles setting scroll bars for when windows move too far to the left or
	/// bottom, providing the MDIDesktopPane is in a ScrollPane.
	/// </remarks>
	[System.Serializable]
	public class MDIDesktopPane : JDesktopPane
	{
		private const long serialVersionUID = -1839360490978587035L;

		private static int FRAME_OFFSET = 20;

		private readonly MDIDesktopManager manager;

		private readonly ECUEditor parent;

		public MDIDesktopPane(ECUEditor parent)
		{
			this.parent = parent;
			manager = new MDIDesktopManager(this);
			SetDesktopManager(manager);
			SetDragMode(JDesktopPane.OUTLINE_DRAG_MODE);
		}

		public override void SetBounds(int x, int y, int w, int h)
		{
			base.SetBounds(x, y, w, h);
			CheckDesktopSize();
		}

		public virtual Component Add(JInternalFrame frame)
		{
			JInternalFrame[] array = GetAllFrames();
			Point p;
			int w;
			int h;
			Component retval = base.Add(frame);
			CheckDesktopSize();
			if (array.Length > 0)
			{
				p = array[0].GetLocation();
				p.x = p.x + FRAME_OFFSET;
				p.y = p.y + FRAME_OFFSET;
			}
			else
			{
				p = new Point(0, 0);
			}
			frame.SetLocation(p.x, p.y);
			if (frame.IsResizable())
			{
				w = GetWidth() - (GetWidth() / 3);
				h = GetHeight() - (GetHeight() / 3);
				if (w < frame.GetMinimumSize().GetWidth())
				{
					w = (int)frame.GetMinimumSize().GetWidth();
				}
				if (h < frame.GetMinimumSize().GetHeight())
				{
					h = (int)frame.GetMinimumSize().GetHeight();
				}
				frame.SetSize(w, h);
			}
			MoveToFront(frame);
			frame.SetVisible(true);
			TableFrame tableFrame = (TableFrame)frame;
			parent.UpdateTableToolBar(tableFrame.GetTable());
			try
			{
				frame.SetSelected(true);
			}
			catch (PropertyVetoException)
			{
				frame.ToBack();
			}
			return retval;
		}

		public override void Remove(Component c)
		{
			base.Remove(c);
			parent.UpdateTableToolBar(null);
			CheckDesktopSize();
		}

		/// <summary>Cascade all internal frames</summary>
		public virtual void CascadeFrames()
		{
			int x = 0;
			int y = 0;
			JInternalFrame[] allFrames = GetAllFrames();
			manager.SetNormalSize();
			int frameHeight = (GetBounds().height - 5) - allFrames.Length * FRAME_OFFSET;
			int frameWidth = (GetBounds().width - 5) - allFrames.Length * FRAME_OFFSET;
			for (int i = allFrames.Length - 1; i >= 0; i--)
			{
				allFrames[i].SetSize(frameWidth, frameHeight);
				allFrames[i].SetLocation(x, y);
				x = x + FRAME_OFFSET;
				y = y + FRAME_OFFSET;
			}
		}

		/// <summary>Tile all internal frames</summary>
		public virtual void TileFrames()
		{
			Component[] allFrames = GetAllFrames();
			manager.SetNormalSize();
			int frameHeight = GetBounds().height / allFrames.Length;
			int y = 0;
			for (int i = 0; i < allFrames.Length; i++)
			{
				allFrames[i].SetSize(GetBounds().width, frameHeight);
				allFrames[i].SetLocation(0, y);
				y = y + frameHeight;
			}
		}

		/// <summary>
		/// Sets all component size properties ( maximum, minimum, preferred)
		/// to the given dimension.
		/// </summary>
		/// <remarks>
		/// Sets all component size properties ( maximum, minimum, preferred)
		/// to the given dimension.
		/// </remarks>
		public virtual void SetAllSize(Dimension d)
		{
			SetMinimumSize(d);
			SetMaximumSize(d);
			SetPreferredSize(d);
		}

		/// <summary>
		/// Sets all component size properties ( maximum, minimum, preferred)
		/// to the given width and height.
		/// </summary>
		/// <remarks>
		/// Sets all component size properties ( maximum, minimum, preferred)
		/// to the given width and height.
		/// </remarks>
		public virtual void SetAllSize(int width, int height)
		{
			SetAllSize(new Dimension(width, height));
		}

		private void CheckDesktopSize()
		{
			if (GetParent() != null && IsVisible())
			{
				manager.ResizeDesktop();
			}
		}
	}

	/// <summary>Private class used to replace the standard DesktopManager for JDesktopPane.
	/// 	</summary>
	/// <remarks>
	/// Private class used to replace the standard DesktopManager for JDesktopPane.
	/// Used to provide scrollbar functionality.
	/// </remarks>
	[System.Serializable]
	internal class MDIDesktopManager : DefaultDesktopManager
	{
		private const long serialVersionUID = -7668105643849176819L;

		private readonly MDIDesktopPane desktop;

		public MDIDesktopManager(MDIDesktopPane desktop)
		{
			this.desktop = desktop;
		}

		public override void EndResizingFrame(JComponent f)
		{
			base.EndResizingFrame(f);
			ResizeDesktop();
		}

		public override void EndDraggingFrame(JComponent f)
		{
			base.EndDraggingFrame(f);
			ResizeDesktop();
		}

		public virtual void SetNormalSize()
		{
			JScrollPane scrollPane = GetScrollPane();
			int x = 0;
			int y = 0;
			Insets scrollInsets = GetScrollPaneInsets();
			if (scrollPane != null)
			{
				Dimension d = scrollPane.GetVisibleRect().GetSize();
				if (scrollPane.GetBorder() != null)
				{
					d.SetSize(d.GetWidth() - scrollInsets.left - scrollInsets.right, d.GetHeight() - 
						scrollInsets.top - scrollInsets.bottom);
				}
				d.SetSize(d.GetWidth() - 20, d.GetHeight() - 20);
				desktop.SetAllSize(x, y);
				scrollPane.Invalidate();
				scrollPane.Validate();
			}
		}

		public virtual MDIDesktopPane GetDesktop()
		{
			return this.desktop;
		}

		private Insets GetScrollPaneInsets()
		{
			JScrollPane scrollPane = GetScrollPane();
			if (scrollPane == null)
			{
				return new Insets(0, 0, 0, 0);
			}
			else
			{
				return GetScrollPane().GetBorder().GetBorderInsets(scrollPane);
			}
		}

		private JScrollPane GetScrollPane()
		{
			if (desktop.GetParent() is JViewport)
			{
				JViewport viewPort = (JViewport)desktop.GetParent();
				if (viewPort.GetParent() is JScrollPane)
				{
					return (JScrollPane)viewPort.GetParent();
				}
			}
			return null;
		}

		protected internal virtual void ResizeDesktop()
		{
			int x = 0;
			int y = 0;
			JScrollPane scrollPane = GetScrollPane();
			Insets scrollInsets = GetScrollPaneInsets();
			if (scrollPane != null)
			{
				JInternalFrame[] allFrames = desktop.GetAllFrames();
				for (int i = 0; i < allFrames.Length; i++)
				{
					if (allFrames[i].GetX() + allFrames[i].GetWidth() > x)
					{
						x = allFrames[i].GetX() + allFrames[i].GetWidth();
					}
					if (allFrames[i].GetY() + allFrames[i].GetHeight() > y)
					{
						y = allFrames[i].GetY() + allFrames[i].GetHeight();
					}
				}
				Dimension d = scrollPane.GetVisibleRect().GetSize();
				if (scrollPane.GetBorder() != null)
				{
					d.SetSize(d.GetWidth() - scrollInsets.left - scrollInsets.right, d.GetHeight() - 
						scrollInsets.top - scrollInsets.bottom);
				}
				if (x <= d.GetWidth())
				{
					x = ((int)d.GetWidth()) - 20;
				}
				if (y <= d.GetHeight())
				{
					y = ((int)d.GetHeight()) - 20;
				}
				desktop.SetAllSize(x, y);
				scrollPane.Invalidate();
				scrollPane.Validate();
			}
		}
	}
}

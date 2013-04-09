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

using Java.Awt.Event;
using Java.Beans;
using Javax.Swing;
using Sharpen;

namespace RomRaider.Swing
{
	[System.Serializable]
	public abstract class AbstractFrame : JFrame, WindowListener, PropertyChangeListener
	{
		/// <exception cref="Java.Awt.HeadlessException"></exception>
		public AbstractFrame() : base()
		{
		}

		/// <exception cref="Java.Awt.HeadlessException"></exception>
		public AbstractFrame(string arg0) : base(arg0)
		{
		}

		private const long serialVersionUID = 7948304087075622157L;

		public virtual void WindowActivated(WindowEvent arg0)
		{
		}

		public virtual void WindowClosed(WindowEvent e)
		{
		}

		public virtual void WindowClosing(WindowEvent e)
		{
		}

		public virtual void WindowDeactivated(WindowEvent e)
		{
		}

		public virtual void WindowDeiconified(WindowEvent e)
		{
		}

		public virtual void WindowIconified(WindowEvent e)
		{
		}

		public virtual void WindowOpened(WindowEvent e)
		{
		}

		public virtual void PropertyChange(PropertyChangeEvent arg0)
		{
		}
	}
}

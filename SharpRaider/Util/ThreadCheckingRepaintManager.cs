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
using System.IO;
using Java.Awt;
using Javax.Swing;
using Sharpen;

namespace RomRaider.Util
{
	public class ThreadCheckingRepaintManager : RepaintManager
	{
		private int tabCount = 0;

		private bool checkIsShowing = false;

		public ThreadCheckingRepaintManager()
		{
		}

		public ThreadCheckingRepaintManager(bool checkIsShowing)
		{
			this.checkIsShowing = checkIsShowing;
		}

		public override void AddInvalidComponent(JComponent jComponent)
		{
			lock (this)
			{
				CheckThread(jComponent);
				base.AddInvalidComponent(jComponent);
			}
		}

		private void CheckThread(JComponent c)
		{
			if (!SwingUtilities.IsEventDispatchThread() && CheckIsShowing(c))
			{
				System.Console.Out.WriteLine("----------Wrong Thread START");
				System.Console.Out.WriteLine(GetStracktraceAsString(new Exception()));
				DumpComponentTree(c);
				System.Console.Out.WriteLine("----------Wrong Thread END");
			}
		}

		private string GetStracktraceAsString(Exception e)
		{
			ByteArrayOutputStream byteArrayOutputStream = new ByteArrayOutputStream();
			TextWriter printStream = new TextWriter(byteArrayOutputStream);
			Sharpen.Runtime.PrintStackTrace(e, printStream);
			printStream.Flush();
			return byteArrayOutputStream.ToString();
		}

		private bool CheckIsShowing(JComponent c)
		{
			return !this.checkIsShowing || c.IsShowing();
		}

		public override void AddDirtyRegion(JComponent jComponent, int i, int i1, int i2, 
			int i3)
		{
			lock (this)
			{
				CheckThread(jComponent);
				base.AddDirtyRegion(jComponent, i, i1, i2, i3);
			}
		}

		private void DumpComponentTree(Component c)
		{
			System.Console.Out.WriteLine("----------Component Tree");
			ResetTabCount();
			for (; c != null; c = c.GetParent())
			{
				PrintTabIndent();
				System.Console.Out.WriteLine(c);
				PrintTabIndent();
				System.Console.Out.WriteLine("Showing:" + c.IsShowing() + " Visible: " + c.IsVisible
					());
				IncrementTabCount();
			}
		}

		private void ResetTabCount()
		{
			this.tabCount = 0;
		}

		private void IncrementTabCount()
		{
			this.tabCount++;
		}

		private void PrintTabIndent()
		{
			for (int i = 0; i < this.tabCount; i++)
			{
				System.Console.Out.Write("\t");
			}
		}
	}
}

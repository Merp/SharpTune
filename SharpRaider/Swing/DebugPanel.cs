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
using RomRaider;
using RomRaider.Net;
using Sharpen;

namespace RomRaider.Swing
{
	[System.Serializable]
	public class DebugPanel : JPanel
	{
		private const long serialVersionUID = -7159385694793030962L;

		public DebugPanel(Exception ex, string url)
		{
			SetLayout(new BorderLayout());
			JPanel top = new JPanel(new GridLayout(7, 1));
			top.Add(new JLabel(Version.PRODUCT_NAME + " has encountered an exception. Please review the details below."
				));
			top.Add(new JLabel("If you are unable to fix this problem please visit the following website"
				));
			top.Add(new JLabel("and provide these details and the steps that lead to this error."
				));
			top.Add(new JLabel());
			top.Add(new URL(url));
			top.Add(new JLabel());
			top.Add(new JLabel("Details:"));
			Add(top, BorderLayout.NORTH);
			JTextArea output = new JTextArea(ex.Message);
			Add(output, BorderLayout.CENTER);
			output.SetAutoscrolls(true);
			output.SetRows(10);
			output.SetColumns(40);
			Sharpen.Runtime.PrintStackTrace(ex);
		}
	}
}

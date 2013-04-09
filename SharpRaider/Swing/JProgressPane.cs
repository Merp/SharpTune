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
using Sharpen;

namespace RomRaider.Swing
{
	[System.Serializable]
	public class JProgressPane : JPanel, PropertyChangeListener
	{
		private const long serialVersionUID = -6827936662738014543L;

		internal JLabel label = new JLabel();

		internal JProgressBar progressBar = new JProgressBar(JProgressBar.HORIZONTAL, 0, 
			100);

		internal string status = "ready";

		internal int percent = 0;

		public JProgressPane()
		{
			this.SetPreferredSize(new Dimension(500, 18));
			this.SetLayout(new BorderLayout(1, 2));
			label.SetHorizontalAlignment(JLabel.CENTER);
			label.SetText(" Ready...");
			label.SetFont(new Font("Tahoma", Font.PLAIN, 11));
			label.SetHorizontalAlignment(JLabel.LEFT);
			progressBar.SetMinimumSize(new Dimension(200, 50));
			progressBar.SetValue(0);
			this.Add(progressBar, BorderLayout.WEST);
			this.Add(label, BorderLayout.CENTER);
		}

		public virtual void Update(string status, int percent)
		{
			label.SetText(" " + status);
			progressBar.SetValue(percent);
		}

		public virtual void SetStatus(string status)
		{
			this.status = status;
		}

		public virtual JProgressBar GetProgressBar()
		{
			return this.progressBar;
		}

		public virtual void PropertyChange(PropertyChangeEvent evt)
		{
			if ("progress" == evt.GetPropertyName())
			{
				int progress = (int)evt.GetNewValue();
				progressBar.SetValue(progress);
				label.SetText(" " + status);
			}
		}
		// do nothing
	}
}

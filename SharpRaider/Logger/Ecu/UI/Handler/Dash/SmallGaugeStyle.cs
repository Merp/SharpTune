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
using Javax.Swing;
using RomRaider.Logger.Ecu.Definition;
using RomRaider.Logger.Ecu.UI.Handler.Dash;
using Sharpen;

namespace RomRaider.Logger.Ecu.UI.Handler.Dash
{
	public sealed class SmallGaugeStyle : PlainGaugeStyle
	{
		public SmallGaugeStyle(LoggerData loggerData) : base(loggerData)
		{
		}

		protected internal override void DoApply(JPanel panel)
		{
			RefreshTitle();
			ResetValue();
			panel.SetPreferredSize(new Dimension(150, 78));
			panel.SetBackground(LIGHT_GREY);
			panel.SetLayout(new BorderLayout(1, 0));
			// title
			title.SetFont(panel.GetFont().DeriveFont(Font.PLAIN, 10F));
			title.SetForeground(Color.WHITE);
			panel.Add(title, BorderLayout.NORTH);
			// data panel
			JPanel data = new JPanel(new FlowLayout(FlowLayout.CENTER, 2, 1));
			data.SetBackground(Color.BLACK);
			liveValueLabel.SetFont(panel.GetFont().DeriveFont(Font.PLAIN, 20F));
			liveValueLabel.SetForeground(Color.WHITE);
			liveValuePanel.SetBackground(LIGHT_GREY);
			liveValuePanel.SetPreferredSize(new Dimension(85, 60));
			liveValuePanel.Add(liveValueLabel, BorderLayout.CENTER);
			data.Add(liveValuePanel);
			// max/min panel
			JPanel maxMinPanel = new JPanel(new BorderLayout(1, 1));
			maxMinPanel.SetBackground(Color.BLACK);
			JPanel maxPanel = BuildMaxMinPanel("max", maxLabel);
			JPanel minPanel = BuildMaxMinPanel("min", minLabel);
			maxMinPanel.Add(maxPanel, BorderLayout.NORTH);
			maxMinPanel.Add(minPanel, BorderLayout.SOUTH);
			data.Add(maxMinPanel);
			// progress bar
			progressBar.SetStringPainted(false);
			progressBar.SetIndeterminate(false);
			progressBar.SetPreferredSize(new Dimension(10, 62));
			progressBar.SetBackground(Color.WHITE);
			progressBar.SetForeground(GREEN);
			data.Add(progressBar);
			// add panels
			panel.Add(data, BorderLayout.CENTER);
		}

		private JPanel BuildMaxMinPanel(string title, JLabel label)
		{
			JPanel panel = new JPanel(new BorderLayout(1, 1));
			label.SetFont(panel.GetFont().DeriveFont(Font.PLAIN, 10F));
			label.SetForeground(Color.WHITE);
			panel.SetPreferredSize(new Dimension(45, 28));
			panel.SetBackground(LIGHT_GREY);
			JLabel titleLabel = new JLabel(title, JLabel.CENTER);
			titleLabel.SetFont(panel.GetFont().DeriveFont(Font.PLAIN, 10F));
			titleLabel.SetForeground(Color.WHITE);
			JPanel dataPanel = new JPanel(new BorderLayout());
			dataPanel.SetBackground(DARK_GREY);
			dataPanel.Add(label, BorderLayout.CENTER);
			panel.Add(titleLabel, BorderLayout.NORTH);
			panel.Add(dataPanel, BorderLayout.CENTER);
			return panel;
		}
	}
}

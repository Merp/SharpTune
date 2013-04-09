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
using Java.Awt.Event;
using Javax.Swing;
using RomRaider.Logger.Ecu.Definition;
using RomRaider.Logger.Ecu.UI.Handler.Dash;
using RomRaider.Tts;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.Ecu.UI.Handler.Dash
{
	public class PlainGaugeStyle : GaugeStyle, ActionListener
	{
		private static readonly string BLANK = string.Empty;

		private static readonly string ABOVE = "above";

		private static readonly string BELOW = "below";

		protected internal static readonly Color RED = new Color(190, 30, 30);

		protected internal static readonly Color GREEN = new Color(34, 139, 34);

		protected internal static readonly Color DARK_GREY = new Color(40, 40, 40);

		protected internal static readonly Color LIGHT_GREY = new Color(56, 56, 56);

		protected internal static readonly Color LIGHTER_GREY = new Color(120, 120, 120);

		protected internal readonly JPanel liveValuePanel = new JPanel(new BorderLayout()
			);

		protected internal readonly JLabel liveValueLabel = new JLabel(BLANK, JLabel.CENTER
			);

		protected internal readonly JLabel maxLabel = new JLabel(BLANK, JLabel.CENTER);

		protected internal readonly JLabel minLabel = new JLabel(BLANK, JLabel.CENTER);

		protected internal readonly JLabel title = new JLabel(BLANK, JLabel.CENTER);

		protected internal readonly JProgressBar progressBar = new JProgressBar(JProgressBar
			.VERTICAL);

		protected internal readonly JCheckBox warnCheckBox = new JCheckBox("Warn");

		protected internal readonly JComboBox warnType = new JComboBox(new object[] { ABOVE
			, BELOW });

		protected internal readonly JTextField warnTextField = new JTextField();

		private readonly string zeroText;

		private readonly LoggerData loggerData;

		private double max = double.MinValue;

		private double min = double.MaxValue;

		private JPanel panel = new JPanel();

		public PlainGaugeStyle(LoggerData loggerData)
		{
			ParamChecker.CheckNotNull(loggerData, "loggerData");
			this.loggerData = loggerData;
			zeroText = Format(loggerData, 0.0);
		}

		public virtual void RefreshTitle()
		{
			SwingUtilities.InvokeLater(new _Runnable_78(this));
		}

		private sealed class _Runnable_78 : Runnable
		{
			public _Runnable_78(PlainGaugeStyle _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing.title.SetText(this._enclosing.loggerData.GetName() + " (" + this.
					_enclosing.loggerData.GetSelectedConvertor().GetUnits() + ')');
			}

			private readonly PlainGaugeStyle _enclosing;
		}

		public virtual void UpdateValue(double value)
		{
			RefreshValue(value);
			if (warnCheckBox.IsSelected() && IsValidWarnThreshold())
			{
				if (warnType.GetSelectedItem() == ABOVE)
				{
					SetWarning(value >= GetWarnThreshold());
				}
				else
				{
					if (warnType.GetSelectedItem() == BELOW)
					{
						SetWarning(value <= GetWarnThreshold());
					}
				}
			}
		}

		public virtual void ResetValue()
		{
			SwingUtilities.InvokeLater(new _Runnable_97(this));
		}

		private sealed class _Runnable_97 : Runnable
		{
			public _Runnable_97(PlainGaugeStyle _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing.liveValueLabel.SetText(this._enclosing.zeroText);
				this._enclosing.max = double.MinValue;
				this._enclosing.maxLabel.SetText(this._enclosing.zeroText);
				this._enclosing.min = double.MaxValue;
				this._enclosing.minLabel.SetText(this._enclosing.zeroText);
				this._enclosing.progressBar.SetMinimum(this._enclosing.ScaleForProgressBar(this._enclosing
					.min));
				this._enclosing.progressBar.SetMaximum(this._enclosing.ScaleForProgressBar(this._enclosing
					.max));
				this._enclosing.progressBar.SetValue(this._enclosing.ScaleForProgressBar(this._enclosing
					.min));
			}

			private readonly PlainGaugeStyle _enclosing;
		}

		public virtual void ActionPerformed(ActionEvent e)
		{
			if (e.GetSource() == warnCheckBox)
			{
				if (!warnCheckBox.IsSelected())
				{
					SetWarning(false);
				}
			}
		}

		public virtual void Apply(JPanel panel)
		{
			this.panel = panel;
			DoApply(panel);
		}

		protected internal virtual void DoApply(JPanel panel)
		{
			RefreshTitle();
			ResetValue();
			panel.SetPreferredSize(new Dimension(236, 144));
			panel.SetBackground(LIGHT_GREY);
			panel.SetLayout(new BorderLayout(3, 0));
			// title
			title.SetFont(panel.GetFont().DeriveFont(Font.BOLD, 12F));
			title.SetForeground(Color.WHITE);
			panel.Add(title, BorderLayout.NORTH);
			// data panel
			JPanel data = new JPanel(new FlowLayout(FlowLayout.CENTER, 3, 1));
			data.SetBackground(Color.BLACK);
			liveValueLabel.SetFont(panel.GetFont().DeriveFont(Font.PLAIN, 40F));
			liveValueLabel.SetForeground(Color.WHITE);
			liveValuePanel.SetBackground(LIGHT_GREY);
			liveValuePanel.SetPreferredSize(new Dimension(140, 80));
			liveValuePanel.Add(liveValueLabel, BorderLayout.CENTER);
			data.Add(liveValuePanel);
			// max/min panel
			JPanel maxMinPanel = new JPanel(new BorderLayout(2, 2));
			maxMinPanel.SetBackground(Color.BLACK);
			JPanel maxPanel = BuildMaxMinPanel("max", maxLabel);
			JPanel minPanel = BuildMaxMinPanel("min", minLabel);
			maxMinPanel.Add(maxPanel, BorderLayout.NORTH);
			maxMinPanel.Add(minPanel, BorderLayout.SOUTH);
			data.Add(maxMinPanel);
			// progress bar
			progressBar.SetStringPainted(false);
			progressBar.SetIndeterminate(false);
			progressBar.SetPreferredSize(new Dimension(20, 82));
			progressBar.SetBackground(Color.WHITE);
			progressBar.SetForeground(GREEN);
			data.Add(progressBar);
			// warn setting
			JPanel warnPanel = new JPanel();
			warnPanel.SetBackground(Color.BLACK);
			JPanel warnFormPanel = new JPanel(new FlowLayout(FlowLayout.CENTER));
			warnFormPanel.SetPreferredSize(new Dimension(226, 34));
			warnFormPanel.SetBackground(Color.BLACK);
			warnFormPanel.SetBorder(BorderFactory.CreateLineBorder(LIGHT_GREY, 1));
			warnCheckBox.SetFont(panel.GetFont().DeriveFont(Font.PLAIN, 10F));
			warnCheckBox.SetBackground(Color.BLACK);
			warnCheckBox.SetForeground(LIGHTER_GREY);
			warnCheckBox.SetSelected(false);
			warnCheckBox.AddActionListener(this);
			warnType.SetPreferredSize(new Dimension(60, 20));
			warnType.SetFont(panel.GetFont().DeriveFont(Font.PLAIN, 10F));
			warnType.SetBackground(Color.BLACK);
			warnType.SetForeground(LIGHTER_GREY);
			warnTextField.SetColumns(4);
			warnTextField.SetBackground(Color.BLACK);
			warnTextField.SetForeground(LIGHTER_GREY);
			warnTextField.SetCaretColor(LIGHTER_GREY);
			warnFormPanel.Add(warnCheckBox);
			warnFormPanel.Add(warnType);
			warnFormPanel.Add(warnTextField);
			warnPanel.Add(warnFormPanel);
			// add panels
			panel.Add(data, BorderLayout.CENTER);
			panel.Add(warnPanel, BorderLayout.SOUTH);
		}

		private JPanel BuildMaxMinPanel(string title, JLabel label)
		{
			JPanel panel = new JPanel(new BorderLayout(1, 1));
			label.SetFont(panel.GetFont().DeriveFont(Font.PLAIN, 12F));
			label.SetForeground(Color.WHITE);
			panel.SetPreferredSize(new Dimension(60, 38));
			panel.SetBackground(LIGHT_GREY);
			JLabel titleLabel = new JLabel(title, JLabel.CENTER);
			titleLabel.SetFont(panel.GetFont().DeriveFont(Font.BOLD, 12F));
			titleLabel.SetForeground(Color.WHITE);
			JPanel dataPanel = new JPanel(new BorderLayout());
			dataPanel.SetBackground(DARK_GREY);
			dataPanel.Add(label, BorderLayout.CENTER);
			panel.Add(titleLabel, BorderLayout.NORTH);
			panel.Add(dataPanel, BorderLayout.CENTER);
			return panel;
		}

		private void RefreshValue(double value)
		{
			string text = Format(loggerData, value);
			int scaledValue = ScaleForProgressBar(value);
			SwingUtilities.InvokeLater(new _Runnable_213(this, value, text, scaledValue));
		}

		private sealed class _Runnable_213 : Runnable
		{
			public _Runnable_213(PlainGaugeStyle _enclosing, double value, string text, int scaledValue
				)
			{
				this._enclosing = _enclosing;
				this.value = value;
				this.text = text;
				this.scaledValue = scaledValue;
			}

			public void Run()
			{
				if (value > this._enclosing.max)
				{
					this._enclosing.max = value;
					this._enclosing.maxLabel.SetText(text);
					this._enclosing.progressBar.SetMaximum(scaledValue);
				}
				if (value < this._enclosing.min)
				{
					this._enclosing.min = value;
					this._enclosing.minLabel.SetText(text);
					this._enclosing.progressBar.SetMinimum(scaledValue);
				}
				this._enclosing.liveValueLabel.SetText(text);
				this._enclosing.progressBar.SetValue(scaledValue);
			}

			private readonly PlainGaugeStyle _enclosing;

			private readonly double value;

			private readonly string text;

			private readonly int scaledValue;
		}

		private bool IsValidWarnThreshold()
		{
			try
			{
				GetWarnThreshold();
				return true;
			}
			catch (FormatException)
			{
				return false;
			}
		}

		private double GetWarnThreshold()
		{
			return double.ParseDouble(warnTextField.GetText());
		}

		private void SetWarning(bool enabled)
		{
			SwingUtilities.InvokeLater(new _Runnable_245(this, enabled));
		}

		private sealed class _Runnable_245 : Runnable
		{
			public _Runnable_245(PlainGaugeStyle _enclosing, bool enabled)
			{
				this._enclosing = _enclosing;
				this.enabled = enabled;
			}

			public void Run()
			{
				if (enabled)
				{
					this._enclosing.panel.SetBackground(RomRaider.Logger.Ecu.UI.Handler.Dash.PlainGaugeStyle
						.RED);
					this._enclosing.liveValuePanel.SetBackground(RomRaider.Logger.Ecu.UI.Handler.Dash.PlainGaugeStyle
						.RED);
					this._enclosing.progressBar.SetForeground(RomRaider.Logger.Ecu.UI.Handler.Dash.PlainGaugeStyle
						.RED);
					Speaker.Say("Warning!");
				}
				else
				{
					this._enclosing.panel.SetBackground(RomRaider.Logger.Ecu.UI.Handler.Dash.PlainGaugeStyle
						.LIGHT_GREY);
					this._enclosing.liveValuePanel.SetBackground(RomRaider.Logger.Ecu.UI.Handler.Dash.PlainGaugeStyle
						.LIGHT_GREY);
					this._enclosing.progressBar.SetForeground(RomRaider.Logger.Ecu.UI.Handler.Dash.PlainGaugeStyle
						.GREEN);
				}
			}

			private readonly PlainGaugeStyle _enclosing;

			private readonly bool enabled;
		}

		private string Format(LoggerData loggerData, double value)
		{
			return loggerData.GetSelectedConvertor().Format(value);
		}

		private int ScaleForProgressBar(double value)
		{
			return (int)(value * 1000.0);
		}
	}
}

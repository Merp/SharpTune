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
using RomRaider.Logger.Ecu.UI;
using Sharpen;

namespace RomRaider.Logger.Ecu.UI
{
	[System.Serializable]
	public sealed class StatusIndicator : JPanel, StatusChangeListener
	{
		private const long serialVersionUID = -3244690866698807677L;

		private readonly JLabel statusLabel = new JLabel();

		private static readonly string TEXT_CONNECTING = "Connecting ";

		private static readonly string TEXT_READING = "Reading data ";

		private static readonly string TEXT_LOGGING = "Logging to file ";

		private static readonly string TEXT_STOPPED = "Stopped ";

		private static readonly ImageIcon ICON_CONNECTING = new ImageIcon(typeof(RomRaider.Logger.Ecu.UI.StatusIndicator
			).GetType().GetResource("/graphics/logger_blue.png"));

		private static readonly ImageIcon ICON_READING = new ImageIcon(typeof(RomRaider.Logger.Ecu.UI.StatusIndicator
			).GetType().GetResource("/graphics/logger_green.png"));

		private static readonly ImageIcon ICON_LOGGING = new ImageIcon(typeof(RomRaider.Logger.Ecu.UI.StatusIndicator
			).GetType().GetResource("/graphics/logger_recording.png"));

		private static readonly ImageIcon ICON_STOPPED = new ImageIcon(typeof(RomRaider.Logger.Ecu.UI.StatusIndicator
			).GetType().GetResource("/graphics/logger_stop.png"));

		public StatusIndicator()
		{
			SetLayout(new BorderLayout());
			statusLabel.SetFont(GetFont().DeriveFont(Font.BOLD));
			Add(statusLabel, BorderLayout.WEST);
			Stopped();
		}

		public void Connecting()
		{
			UpdateStatusLabel(TEXT_CONNECTING, ICON_CONNECTING);
		}

		public void ReadingData()
		{
			UpdateStatusLabel(TEXT_READING, ICON_READING);
		}

		public void LoggingData()
		{
			UpdateStatusLabel(TEXT_LOGGING, ICON_LOGGING);
		}

		public void Stopped()
		{
			UpdateStatusLabel(TEXT_STOPPED, ICON_STOPPED);
		}

		private void UpdateStatusLabel(string text, ImageIcon icon)
		{
			SwingUtilities.InvokeLater(new _Runnable_66(this, text, icon));
		}

		private sealed class _Runnable_66 : Runnable
		{
			public _Runnable_66(StatusIndicator _enclosing, string text, ImageIcon icon)
			{
				this._enclosing = _enclosing;
				this.text = text;
				this.icon = icon;
			}

			public void Run()
			{
				this._enclosing.statusLabel.SetText(text);
				this._enclosing.statusLabel.SetIcon(icon);
			}

			private readonly StatusIndicator _enclosing;

			private readonly string text;

			private readonly ImageIcon icon;
		}
	}
}

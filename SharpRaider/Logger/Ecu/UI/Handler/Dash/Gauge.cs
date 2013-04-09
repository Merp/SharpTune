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
using RomRaider.Logger.Ecu.UI.Handler.Dash;
using Sharpen;

namespace RomRaider.Logger.Ecu.UI.Handler.Dash
{
	[System.Serializable]
	public sealed class Gauge : JPanel
	{
		private const long serialVersionUID = 7354117571944547043L;

		private GaugeStyle style;

		public Gauge(GaugeStyle style) : base()
		{
			SetLayout(new BorderLayout(0, 0));
			SetGaugeStyle(style);
		}

		public void RefreshTitle()
		{
			style.RefreshTitle();
		}

		public void UpdateValue(double value)
		{
			style.UpdateValue(value);
		}

		public void ResetValue()
		{
			style.ResetValue();
		}

		public void SetGaugeStyle(GaugeStyle style)
		{
			this.style = style;
			SwingUtilities.InvokeLater(new _Runnable_52(this, style));
		}

		private sealed class _Runnable_52 : Runnable
		{
			public _Runnable_52(Gauge _enclosing, GaugeStyle style)
			{
				this._enclosing = _enclosing;
				this.style = style;
			}

			public void Run()
			{
				this._enclosing.RemoveAll();
				JPanel child = new JPanel();
				style.Apply(child);
				this._enclosing.Add(child, BorderLayout.CENTER);
			}

			private readonly Gauge _enclosing;

			private readonly GaugeStyle style;
		}
	}
}

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
using Org.Jfree.Chart;
using Org.Jfree.Chart.Plot.Dial;
using Org.Jfree.Data.General;
using Org.Jfree.UI;
using RomRaider.Logger.Ecu.Definition;
using RomRaider.Logger.Ecu.UI.Handler.Dash;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.Ecu.UI.Handler.Dash
{
	public class DialGaugeStyle : GaugeStyle
	{
		protected internal readonly DefaultValueDataset current = new DefaultValueDataset
			(0.0);

		protected internal readonly DefaultValueDataset max = new DefaultValueDataset(0.0
			);

		protected internal readonly DefaultValueDataset min = new DefaultValueDataset(0.0
			);

		protected internal readonly DialTextAnnotation unitsLabel = new DialTextAnnotation
			(string.Empty);

		protected internal readonly LoggerData loggerData;

		private double maxValue = double.MIN_VALUE;

		private double minValue = double.MAX_VALUE;

		private JPanel panel;

		public DialGaugeStyle(LoggerData loggerData)
		{
			ParamChecker.CheckNotNull(loggerData);
			this.loggerData = loggerData;
		}

		public virtual void Apply(JPanel panel)
		{
			this.panel = panel;
			ResetValue();
			RefreshChart(panel);
		}

		private void RefreshChart(JPanel panel)
		{
			SwingUtilities.InvokeLater(new _Runnable_80(this, panel));
		}

		private sealed class _Runnable_80 : Runnable
		{
			public _Runnable_80(DialGaugeStyle _enclosing, JPanel panel)
			{
				this._enclosing = _enclosing;
				this.panel = panel;
			}

			public void Run()
			{
				JFreeChart chart = this._enclosing.BuildChart();
				ChartPanel chartPanel = new ChartPanel(chart);
				chartPanel.SetPreferredSize(this._enclosing.GetChartSize());
				panel.RemoveAll();
				panel.Add(chartPanel);
				panel.Revalidate();
			}

			private readonly DialGaugeStyle _enclosing;

			private readonly JPanel panel;
		}

		public virtual void RefreshTitle()
		{
			RefreshChart(panel);
		}

		public virtual void UpdateValue(double value)
		{
			SwingUtilities.InvokeLater(new _Runnable_97(this, value));
			UpdateMinMax(value);
		}

		private sealed class _Runnable_97 : Runnable
		{
			public _Runnable_97(DialGaugeStyle _enclosing, double value)
			{
				this._enclosing = _enclosing;
				this.value = value;
			}

			public void Run()
			{
				this._enclosing.current.SetValue(value);
			}

			private readonly DialGaugeStyle _enclosing;

			private readonly double value;
		}

		private void UpdateMinMax(double value)
		{
			SwingUtilities.InvokeLater(new _Runnable_106(this, value));
		}

		private sealed class _Runnable_106 : Runnable
		{
			public _Runnable_106(DialGaugeStyle _enclosing, double value)
			{
				this._enclosing = _enclosing;
				this.value = value;
			}

			public void Run()
			{
				if (value > this._enclosing.maxValue)
				{
					this._enclosing.maxValue = value;
					this._enclosing.max.SetValue(value);
				}
				else
				{
					if (value < this._enclosing.minValue)
					{
						this._enclosing.minValue = value;
						this._enclosing.min.SetValue(value);
					}
				}
			}

			private readonly DialGaugeStyle _enclosing;

			private readonly double value;
		}

		public virtual void ResetValue()
		{
			EcuDataConvertor convertor = loggerData.GetSelectedConvertor();
			GaugeMinMax minMax = convertor.GetGaugeMinMax();
			double value = minMax.min;
			UpdateValue(value);
			ResetMinMax(value);
		}

		private void ResetMinMax(double value)
		{
			SwingUtilities.InvokeLater(new _Runnable_128(this, value));
		}

		private sealed class _Runnable_128 : Runnable
		{
			public _Runnable_128(DialGaugeStyle _enclosing, double value)
			{
				this._enclosing = _enclosing;
				this.value = value;
			}

			public void Run()
			{
				this._enclosing.maxValue = double.MIN_VALUE;
				this._enclosing.minValue = double.MAX_VALUE;
				this._enclosing.max.SetValue(value);
				this._enclosing.min.SetValue(value);
			}

			private readonly DialGaugeStyle _enclosing;

			private readonly double value;
		}

		protected internal virtual Dimension GetChartSize()
		{
			return new Dimension(250, 270);
		}

		protected internal virtual JFreeChart BuildChart()
		{
			DialPlot plot = new DialPlot();
			plot.SetView(0.0, 0.0, 1.0, 1.0);
			plot.SetDataset(0, current);
			plot.SetDataset(1, max);
			plot.SetDataset(2, min);
			DialFrame dialFrame = new StandardDialFrame();
			plot.SetDialFrame(dialFrame);
			GradientPaint gp = new GradientPaint(new Point(), new Color(255, 255, 255), new Point
				(), new Color(170, 170, 220));
			DialBackground db = new DialBackground(gp);
			db.SetGradientPaintTransformer(new StandardGradientPaintTransformer(GradientPaintTransformType
				.VERTICAL));
			plot.SetBackground(db);
			unitsLabel.SetFont(new Font(Font.DIALOG, Font.BOLD, 15));
			unitsLabel.SetRadius(0.7);
			unitsLabel.SetLabel(loggerData.GetSelectedConvertor().GetUnits());
			plot.AddLayer(unitsLabel);
			DecimalFormat format = new DecimalFormat(loggerData.GetSelectedConvertor().GetFormat
				());
			DialValueIndicator dvi = new DialValueIndicator(0);
			dvi.SetNumberFormat(format);
			plot.AddLayer(dvi);
			EcuDataConvertor convertor = loggerData.GetSelectedConvertor();
			GaugeMinMax minMax = convertor.GetGaugeMinMax();
			StandardDialScale scale = new StandardDialScale(minMax.min, minMax.max, 225.0, -270.0
				, minMax.step, 5);
			scale.SetTickRadius(0.88);
			scale.SetTickLabelOffset(0.15);
			scale.SetTickLabelFont(new Font(Font.DIALOG, Font.PLAIN, 12));
			scale.SetTickLabelFormatter(format);
			plot.AddScale(0, scale);
			plot.AddScale(1, scale);
			plot.AddScale(2, scale);
			StandardDialRange range = new StandardDialRange(RangeLimit(minMax, 0.75), minMax.
				max, Color.RED);
			range.SetInnerRadius(0.52);
			range.SetOuterRadius(0.55);
			plot.AddLayer(range);
			StandardDialRange range2 = new StandardDialRange(RangeLimit(minMax, 0.5), RangeLimit
				(minMax, 0.75), Color.ORANGE);
			range2.SetInnerRadius(0.52);
			range2.SetOuterRadius(0.55);
			plot.AddLayer(range2);
			StandardDialRange range3 = new StandardDialRange(minMax.min, RangeLimit(minMax, 0.5
				), Color.GREEN);
			range3.SetInnerRadius(0.52);
			range3.SetOuterRadius(0.55);
			plot.AddLayer(range3);
			DialPointer needleCurrent = new DialPointer.Pointer(0);
			plot.AddLayer(needleCurrent);
			DialPointer needleMax = new DialPointer.Pin(1);
			needleMax.SetRadius(0.84);
			((DialPointer.Pin)needleMax).SetPaint(Color.RED);
			((DialPointer.Pin)needleMax).SetStroke(new BasicStroke(1.5F));
			plot.AddLayer(needleMax);
			DialPointer needleMin = new DialPointer.Pin(2);
			needleMin.SetRadius(0.84);
			((DialPointer.Pin)needleMin).SetPaint(Color.BLUE);
			((DialPointer.Pin)needleMin).SetStroke(new BasicStroke(1.5F));
			plot.AddLayer(needleMin);
			DialCap cap = new DialCap();
			cap.SetRadius(0.10);
			plot.SetCap(cap);
			JFreeChart chart = new JFreeChart(plot);
			chart.SetTitle(loggerData.GetName());
			return chart;
		}

		private double RangeLimit(GaugeMinMax minMax, double fraction)
		{
			return minMax.min + (minMax.max - minMax.min) * fraction;
		}
	}
}

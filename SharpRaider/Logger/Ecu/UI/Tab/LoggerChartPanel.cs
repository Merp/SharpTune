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

using Jamlab;
using Java.Awt;
using Javax.Swing;
using Org.Jfree.Chart;
using Org.Jfree.Chart.Plot;
using Org.Jfree.Chart.Renderer.XY;
using Org.Jfree.Data.XY;
using RomRaider.Logger.Ecu.UI.Handler.Graph;
using RomRaider.Logger.Ecu.UI.Tab;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.Ecu.UI.Tab
{
	[System.Serializable]
	public sealed class LoggerChartPanel : JPanel
	{
		private const long serialVersionUID = -6579979878171615665L;

		private static readonly Color DARK_GREY = new Color(80, 80, 80);

		private static readonly Color LIGHT_GREY = new Color(110, 110, 110);

		private readonly XYSeries data = new XYSeries("Data");

		private readonly XYTrendline trendline;

		private readonly XYSeries hilite = new XYSeries("Hilite");

		private readonly string labelX;

		private readonly string labelY;

		public LoggerChartPanel(string labelX, string labelY) : base(new SpringLayout())
		{
			trendline = new XYTrendline(data);
			ParamChecker.CheckNotNull(labelX, labelY);
			this.labelX = labelX;
			this.labelY = labelY;
			AddChart();
		}

		public void AddData(double x, double y)
		{
			lock (this)
			{
				if (hilite.GetItemCount() == 1)
				{
					XYDataItem item = hilite.Remove(0);
					data.Add(item);
				}
				hilite.Add(x, y);
			}
		}

		public void Clear()
		{
			trendline.Clear();
			hilite.Clear();
			data.Clear();
		}

		public void Interpolate(int order)
		{
			trendline.Update(order);
		}

		public double[] Calculate(double[] x)
		{
			return trendline.Calculate(x);
		}

		public double[] GetPolynomialCoefficients()
		{
			Polyfit fit = trendline.GetPolyFit();
			return fit.GetPolynomialCoefficients();
		}

		private void AddChart()
		{
			ChartPanel chartPanel = new ChartPanel(CreateChart(), false, true, true, true, true
				);
			chartPanel.SetMinimumSize(new Dimension(400, 300));
			chartPanel.SetPreferredSize(new Dimension(500, 400));
			Add(chartPanel);
			SpringUtilities.MakeCompactGrid(this, 1, 1, 2, 2, 2, 2);
		}

		private JFreeChart CreateChart()
		{
			JFreeChart chart = ChartFactory.CreateScatterPlot(null, labelX, labelY, null, PlotOrientation
				.VERTICAL, false, true, false);
			chart.SetBackgroundPaint(Color.BLACK);
			ConfigurePlot(chart);
			AddSeries(chart, 0, hilite, 4, Color.GREEN);
			AddTrendLine(chart, 1, trendline, Color.BLUE);
			AddSeries(chart, 2, data, 2, Color.RED);
			return chart;
		}

		private void ConfigurePlot(JFreeChart chart)
		{
			XYPlot plot = chart.GetXYPlot();
			plot.SetBackgroundPaint(Color.BLACK);
			plot.GetDomainAxis().SetLabelPaint(Color.WHITE);
			plot.GetRangeAxis().SetLabelPaint(Color.WHITE);
			plot.GetDomainAxis().SetTickLabelPaint(LIGHT_GREY);
			plot.GetRangeAxis().SetTickLabelPaint(LIGHT_GREY);
			plot.SetDomainGridlinePaint(DARK_GREY);
			plot.SetRangeGridlinePaint(DARK_GREY);
			plot.SetOutlinePaint(DARK_GREY);
			plot.SetRenderer(BuildScatterRenderer(2, Color.RED));
		}

		private XYDotRenderer BuildScatterRenderer(int size, Color color)
		{
			XYDotRenderer renderer = new XYDotRenderer();
			renderer.SetDotHeight(size);
			renderer.SetDotWidth(size);
			renderer.SetSeriesPaint(0, color);
			return renderer;
		}

		private void AddTrendLine(JFreeChart chart, int index, XYTrendline trendline, Color
			 color)
		{
			XYPlot plot = chart.GetXYPlot();
			plot.SetDataset(index, trendline);
			plot.SetRenderer(index, BuildTrendLineRenderer(color));
		}

		private void AddSeries(JFreeChart chart, int index, XYSeries series, int size, Color
			 color)
		{
			XYDataset dataset = new XYSeriesCollection(series);
			XYPlot plot = chart.GetXYPlot();
			plot.SetDataset(index, dataset);
			plot.SetRenderer(index, BuildScatterRenderer(size, color));
		}

		private StandardXYItemRenderer BuildTrendLineRenderer(Color color)
		{
			StandardXYItemRenderer renderer = new StandardXYItemRenderer();
			renderer.SetSeriesPaint(0, color);
			return renderer;
		}
	}
}

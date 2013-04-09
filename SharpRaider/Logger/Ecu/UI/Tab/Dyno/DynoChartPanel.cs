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
using Jamlab;
using Java.Awt;
using Javax.Swing;
using Org.Jfree.Chart;
using Org.Jfree.Chart.Annotations;
using Org.Jfree.Chart.Axis;
using Org.Jfree.Chart.Plot;
using Org.Jfree.Chart.Renderer.XY;
using Org.Jfree.Data.XY;
using Org.Jfree.UI;
using RomRaider.Logger.Ecu.UI.Handler.Graph;
using RomRaider.Logger.Ecu.UI.Tab;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.Ecu.UI.Tab.Dyno
{
	[System.Serializable]
	public sealed class DynoChartPanel : JPanel
	{
		private const long serialVersionUID = -6577979878171615665L;

		private static readonly Color DARK_GREY = new Color(80, 80, 80);

		private static readonly Color LIGHT_GREY = new Color(110, 110, 110);

		private static readonly string START_PROMPT = "Accelerate using WOT when ready!!";

		private static readonly string ET_PROMPT_I = "Accelerate for 1/4 mile when ready!!";

		private static readonly string ET_PROMPT_M = "Accelerate for 402 meters when ready!!";

		private readonly XYSeries data = new XYSeries("Raw HP");

		private readonly XYSeries data1 = new XYSeries("Raw TQ");

		private readonly XYSeries logRpm = new XYSeries("Logger RPM");

		private readonly XYTrendline rpmTrend;

		private readonly XYSeries hpRef = new XYSeries("HP Ref");

		private readonly XYSeries tqRef = new XYSeries("TQ Ref");

		private readonly string labelX;

		private string labelY1 = null;

		private string labelY2 = null;

		private StandardXYItemRenderer rendererY1 = new StandardXYItemRenderer();

		private StandardXYItemRenderer rendererY2 = new StandardXYItemRenderer();

		private NumberAxis hpAxis = new NumberAxis("pwr");

		private NumberAxis tqAxis = new NumberAxis("tq");

		private XYPlot plot;

		private readonly CircleDrawer cd = new CircleDrawer(Color.RED, new BasicStroke(1.0f
			), null);

		private readonly CircleDrawer cdGreen = new CircleDrawer(Color.GREEN, new BasicStroke
			(1.0f), null);

		private XYAnnotation bestHp;

		private XYAnnotation bestTq;

		private readonly XYPointerAnnotation hpPointer = new XYPointerAnnotation("Max HP"
			, 1, 1, 3.0 * Math.PI / 6.0);

		private readonly XYPointerAnnotation tqPointer = new XYPointerAnnotation("Max TQ"
			, 1, 1, 3.0 * Math.PI / 6.0);

		private readonly XYTextAnnotation refStat = new XYTextAnnotation(" ", 0, 0);

		public DynoChartPanel(string labelX, string labelY1, string labelY2) : base(new SpringLayout
			())
		{
			rpmTrend = new XYTrendline(logRpm);
			// series for HorsePower/RPM
			// series for Torque/RPM
			// series for raw sample time/RPM
			// series for reference HP/RPM
			// series for reference TQ/RPM
			ParamChecker.CheckNotNull(labelX, labelY1, labelY2);
			this.labelX = labelX;
			this.labelY1 = labelY1;
			this.labelY2 = labelY2;
			AddChart();
		}

		public void QuietUpdate(bool notify)
		{
			data.SetNotify(notify);
			data1.SetNotify(notify);
		}

		public void AddRawData(double x, double y)
		{
			lock (this)
			{
				logRpm.Add(x, y);
			}
		}

		public void AddData(double x, double y)
		{
			lock (this)
			{
				data.Add(x, y);
			}
		}

		public void AddData(double x, double y1, double y2)
		{
			lock (this)
			{
				data.Add(x, y1);
				data1.Add(x, y2);
			}
		}

		public void SetRefTrace(double x, double y1, double y2)
		{
			lock (this)
			{
				hpRef.Add(x, y1);
				tqRef.Add(x, y2);
			}
		}

		public void UpdateRefTrace(string[] line)
		{
			if (hpRef.GetItemCount() > 0)
			{
				refStat.SetText("Reference: " + line[2] + " (" + string.Format("%1.2f", double.ParseDouble
					(line[3])) + "; " + string.Format("%1.2f", double.ParseDouble(line[4])) + "; " +
					 string.Format("%1.2f", double.ParseDouble(line[5])) + "; " + string.Format("%1.2f"
					, double.ParseDouble(line[6])) + ")");
				refStat.SetX(plot.GetDomainAxis().GetLowerBound() + 10);
				refStat.SetY(hpAxis.GetUpperBound());
				plot.AddAnnotation(refStat);
			}
		}

		public int GetPwrTqCount()
		{
			return (int)data.GetItemCount();
		}

		public string GetPwrTq(int x)
		{
			string dataSet = data.GetX(x) + "," + data.GetY(x) + "," + data1.GetY(x);
			return dataSet;
		}

		public void ClearRefTrace()
		{
			refStat.SetText(" ");
			plot.RemoveAnnotation(refStat);
			hpRef.Clear();
			tqRef.Clear();
		}

		public void Clear()
		{
			logRpm.Clear();
			rpmTrend.Clear();
			ClearGraph();
		}

		public long GetTimeSample(int index)
		{
			return logRpm.GetX(index);
		}

		public int GetSampleCount()
		{
			ClearGraph();
			return logRpm.GetItemCount();
		}

		public void ClearGraph()
		{
			data.Clear();
			data1.Clear();
			rendererY1.RemoveAnnotation(bestHp);
			rendererY2.RemoveAnnotation(bestTq);
			rendererY1.RemoveAnnotation(hpPointer);
			rendererY2.RemoveAnnotation(tqPointer);
			hpAxis.SetAutoRange(true);
			tqAxis.SetAutoRange(true);
			plot.ClearAnnotations();
		}

		public double[] GetRpmCoeff(int order)
		{
			rpmTrend.Update(order);
			return GetPolynomialCoefficients(rpmTrend);
		}

		public void Interpolate(double[] results, string[] resultStrings)
		{
			hpAxis.SetAutoRange(true);
			tqAxis.SetAutoRange(true);
			double rangeMin = Math.Min(tqAxis.GetLowerBound(), hpAxis.GetLowerBound());
			double yMin = Math.Round(rangeMin);
			double ySpace = (hpAxis.GetUpperBound() - hpAxis.GetLowerBound()) / 25;
			double xMin = ((plot.GetDomainAxis().GetUpperBound() - plot.GetDomainAxis().GetLowerBound
				()) / 7) + plot.GetDomainAxis().GetLowerBound();
			hpAxis.SetRange(Math.Round(rangeMin), Math.Round(hpAxis.GetUpperBound() + ySpace)
				);
			tqAxis.SetRange(Math.Round(rangeMin), Math.Round(tqAxis.GetUpperBound() + ySpace)
				);
			bestHp = new XYDrawableAnnotation(results[1], results[0], 10, 10, cd);
			hpPointer.SetX(results[1]);
			hpPointer.SetY(results[0]);
			hpPointer.SetArrowPaint(Color.BLUE);
			hpPointer.SetTipRadius(7.0);
			hpPointer.SetBaseRadius(30.0);
			hpPointer.SetFont(new Font(Font.SANS_SERIF, Font.BOLD, 10));
			hpPointer.SetPaint(Color.BLUE);
			bestTq = new XYDrawableAnnotation(results[3], results[2], 10, 10, cd);
			tqPointer.SetX(results[3]);
			tqPointer.SetY(results[2]);
			tqPointer.SetArrowPaint(Color.YELLOW);
			tqPointer.SetTipRadius(7.0);
			tqPointer.SetBaseRadius(30.0);
			tqPointer.SetFont(new Font(Font.SANS_SERIF, Font.BOLD, 10));
			tqPointer.SetPaint(Color.YELLOW);
			XYTextAnnotation dynoResults = new XYTextAnnotation(resultStrings[1], xMin, yMin 
				+ (ySpace * 5));
			dynoResults.SetPaint(Color.RED);
			dynoResults.SetTextAnchor(TextAnchor.BOTTOM_LEFT);
			dynoResults.SetFont(new Font(Font.SANS_SERIF, Font.BOLD, 14));
			XYTextAnnotation carText = new XYTextAnnotation(resultStrings[0], xMin, yMin + (ySpace
				 * 4));
			carText.SetPaint(Color.RED);
			carText.SetTextAnchor(TextAnchor.BOTTOM_LEFT);
			carText.SetFont(new Font(Font.SANS_SERIF, Font.BOLD, 12));
			XYTextAnnotation stat1 = new XYTextAnnotation(resultStrings[2], xMin, yMin + (ySpace
				 * 3));
			stat1.SetPaint(Color.RED);
			stat1.SetTextAnchor(TextAnchor.BOTTOM_LEFT);
			stat1.SetFont(new Font(Font.SANS_SERIF, Font.BOLD, 12));
			XYTextAnnotation stat2 = new XYTextAnnotation(resultStrings[3], xMin, yMin + ySpace
				 * 2);
			stat2.SetPaint(Color.RED);
			stat2.SetTextAnchor(TextAnchor.BOTTOM_LEFT);
			stat2.SetFont(new Font(Font.SANS_SERIF, Font.BOLD, 12));
			XYTextAnnotation stat3 = new XYTextAnnotation(resultStrings[4], xMin, yMin + ySpace
				);
			stat3.SetPaint(Color.RED);
			stat3.SetTextAnchor(TextAnchor.BOTTOM_LEFT);
			stat3.SetFont(new Font(Font.SANS_SERIF, Font.BOLD, 12));
			XYTextAnnotation stat4 = new XYTextAnnotation(resultStrings[5], xMin, yMin);
			stat4.SetPaint(Color.RED);
			stat4.SetTextAnchor(TextAnchor.BOTTOM_LEFT);
			stat4.SetFont(new Font(Font.SANS_SERIF, Font.BOLD, 12));
			if (!refStat.Equals(" "))
			{
				refStat.SetX(plot.GetDomainAxis().GetLowerBound() + 10);
				refStat.SetY(hpAxis.GetUpperBound());
				plot.AddAnnotation(refStat);
			}
			rendererY1.AddAnnotation(bestHp);
			rendererY2.AddAnnotation(bestTq);
			rendererY1.AddAnnotation(hpPointer);
			rendererY2.AddAnnotation(tqPointer);
			plot.AddAnnotation(dynoResults);
			plot.AddAnnotation(carText);
			plot.AddAnnotation(stat1);
			plot.AddAnnotation(stat2);
			plot.AddAnnotation(stat3);
			plot.AddAnnotation(stat4);
		}

		public void UpdateEtResults(string carInfo, double[] etResults, string units)
		{
			string s60Text = "60 ft";
			string s330Text = "330 ft";
			string s660Text = "1/2 track";
			string s1000Text = "1,000 ft";
			string s1320Text = "1/4 mile";
			string zTo60Text = "60 mph";
			if (Sharpen.Runtime.EqualsIgnoreCase(units, "km/h"))
			{
				s60Text = "18.3m";
				s330Text = "100m";
				s1000Text = "305m";
				s1320Text = "402m";
				zTo60Text = "97 km/h";
			}
			hpAxis.SetLabel("Vehicle Speed (" + units + ")");
			string[] car = carInfo.Split(";");
			car[0] = "LANE 1: " + Sharpen.Runtime.Substring(car[0], 0, car[0].Length - 3) + " - ET: "
				 + string.Format("%1.3f", etResults[8]) + "\" / " + string.Format("%1.2f", etResults
				[9]) + " " + units;
			double ySpace = hpAxis.GetUpperBound() / 25;
			double xMin = ((plot.GetDomainAxis().GetUpperBound() - plot.GetDomainAxis().GetLowerBound
				()) / 7) + plot.GetDomainAxis().GetLowerBound();
			tqAxis.SetRange(hpAxis.GetLowerBound(), hpAxis.GetUpperBound());
			XYAnnotation s60Marker = new XYDrawableAnnotation(etResults[0], etResults[1], 10, 
				10, cd);
			XYTextAnnotation s60Label = new XYTextAnnotation(s60Text, etResults[0], (etResults
				[1] + ySpace));
			s60Label.SetPaint(Color.RED);
			s60Label.SetTextAnchor(TextAnchor.TOP_RIGHT);
			s60Label.SetFont(new Font(Font.SANS_SERIF, Font.BOLD, 10));
			XYTextAnnotation s60Time = new XYTextAnnotation(string.Format("%1.3f", etResults[
				0]) + "\" / " + string.Format("%1.2f", etResults[1]), etResults[0], (etResults[1
				] - ySpace));
			s60Time.SetPaint(Color.RED);
			s60Time.SetTextAnchor(TextAnchor.BOTTOM_LEFT);
			s60Time.SetFont(new Font(Font.SANS_SERIF, Font.BOLD, 10));
			XYAnnotation s330Marker = new XYDrawableAnnotation(etResults[2], etResults[3], 10
				, 10, cd);
			XYTextAnnotation s330Label = new XYTextAnnotation(s330Text, etResults[2], (etResults
				[3] + ySpace));
			s330Label.SetPaint(Color.RED);
			s330Label.SetTextAnchor(TextAnchor.TOP_RIGHT);
			s330Label.SetFont(new Font(Font.SANS_SERIF, Font.BOLD, 10));
			XYTextAnnotation s330Time = new XYTextAnnotation(string.Format("%1.3f", etResults
				[2]) + "\" / " + string.Format("%1.2f", etResults[3]), etResults[2], (etResults[
				3] - ySpace));
			s330Time.SetPaint(Color.RED);
			s330Time.SetTextAnchor(TextAnchor.BOTTOM_LEFT);
			s330Time.SetFont(new Font(Font.SANS_SERIF, Font.BOLD, 10));
			XYAnnotation s660Marker = new XYDrawableAnnotation(etResults[4], etResults[5], 10
				, 10, cd);
			XYTextAnnotation s660Label = new XYTextAnnotation(s660Text, etResults[4], (etResults
				[5] + ySpace));
			s660Label.SetPaint(Color.RED);
			s660Label.SetTextAnchor(TextAnchor.TOP_RIGHT);
			s660Label.SetFont(new Font(Font.SANS_SERIF, Font.BOLD, 10));
			XYTextAnnotation s660Time = new XYTextAnnotation(string.Format("%1.3f", etResults
				[4]) + "\" / " + string.Format("%1.2f", etResults[5]), etResults[4], (etResults[
				5] - ySpace));
			s660Time.SetPaint(Color.RED);
			s660Time.SetTextAnchor(TextAnchor.BOTTOM_LEFT);
			s660Time.SetFont(new Font(Font.SANS_SERIF, Font.BOLD, 10));
			XYAnnotation s1000Marker = new XYDrawableAnnotation(etResults[6], etResults[7], 10
				, 10, cd);
			XYTextAnnotation s1000Label = new XYTextAnnotation(s1000Text, etResults[6], (etResults
				[7] + ySpace));
			s1000Label.SetPaint(Color.RED);
			s1000Label.SetTextAnchor(TextAnchor.TOP_RIGHT);
			s1000Label.SetFont(new Font(Font.SANS_SERIF, Font.BOLD, 10));
			XYTextAnnotation s1000Time = new XYTextAnnotation(string.Format("%1.3f", etResults
				[6]) + "\" / " + string.Format("%1.2f", etResults[7]), etResults[6], (etResults[
				7] - ySpace));
			s1000Time.SetPaint(Color.RED);
			s1000Time.SetTextAnchor(TextAnchor.BOTTOM_LEFT);
			s1000Time.SetFont(new Font(Font.SANS_SERIF, Font.BOLD, 10));
			XYAnnotation s1320Marker = new XYDrawableAnnotation(etResults[8], etResults[9], 10
				, 10, cd);
			XYTextAnnotation s1320Label = new XYTextAnnotation(s1320Text, etResults[8], (etResults
				[9] - ySpace));
			s1320Label.SetPaint(Color.RED);
			s1320Label.SetTextAnchor(TextAnchor.BOTTOM_CENTER);
			s1320Label.SetFont(new Font(Font.SANS_SERIF, Font.BOLD, 10));
			XYTextAnnotation s1320Time = new XYTextAnnotation(string.Format("%1.3f", etResults
				[8]) + "\" / " + string.Format("%1.2f", etResults[9]), (etResults[8] - 0.2), etResults
				[9]);
			s1320Time.SetPaint(Color.RED);
			s1320Time.SetTextAnchor(TextAnchor.CENTER_RIGHT);
			s1320Time.SetFont(new Font(Font.SANS_SERIF, Font.BOLD, 10));
			XYTextAnnotation carText = new XYTextAnnotation(car[0], (plot.GetDomainAxis().GetUpperBound
				() - 0.2), (hpAxis.GetLowerBound() + ySpace));
			carText.SetPaint(Color.RED);
			carText.SetTextAnchor(TextAnchor.BOTTOM_RIGHT);
			carText.SetFont(new Font(Font.SANS_SERIF, Font.BOLD, 12));
			XYAnnotation zTo60Marker = new XYDrawableAnnotation(etResults[10], etResults[11], 
				10, 10, cdGreen);
			XYTextAnnotation zTo60Label = new XYTextAnnotation(zTo60Text, etResults[10], (etResults
				[11] + ySpace));
			zTo60Label.SetPaint(Color.GREEN);
			zTo60Label.SetTextAnchor(TextAnchor.TOP_RIGHT);
			zTo60Label.SetFont(new Font(Font.SANS_SERIF, Font.BOLD, 10));
			XYTextAnnotation zTo60Time = new XYTextAnnotation((string.Format("%1.3f", etResults
				[10]) + "\""), etResults[10], (etResults[11] - ySpace));
			zTo60Time.SetPaint(Color.GREEN);
			zTo60Time.SetTextAnchor(TextAnchor.BOTTOM_LEFT);
			zTo60Time.SetFont(new Font(Font.SANS_SERIF, Font.BOLD, 10));
			plot.AddAnnotation(s60Marker);
			plot.AddAnnotation(s60Label);
			plot.AddAnnotation(s60Time);
			plot.AddAnnotation(s330Marker);
			plot.AddAnnotation(s330Label);
			plot.AddAnnotation(s330Time);
			plot.AddAnnotation(s660Marker);
			plot.AddAnnotation(s660Label);
			plot.AddAnnotation(s660Time);
			plot.AddAnnotation(s1000Marker);
			plot.AddAnnotation(s1000Label);
			plot.AddAnnotation(s1000Time);
			plot.AddAnnotation(s1320Marker);
			plot.AddAnnotation(s1320Label);
			plot.AddAnnotation(s1320Time);
			plot.AddAnnotation(carText);
			plot.AddAnnotation(zTo60Marker);
			plot.AddAnnotation(zTo60Label);
			plot.AddAnnotation(zTo60Time);
		}

		public double[] GetPolynomialCoefficients(XYTrendline trendSeries)
		{
			Polyfit fit = trendSeries.GetPolyFit();
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
			JFreeChart chart = ChartFactory.CreateScatterPlot(null, labelX, labelY1, null, PlotOrientation
				.VERTICAL, false, true, false);
			chart.SetBackgroundPaint(Color.BLACK);
			ConfigurePlot(chart);
			AddSeries1(chart, 0, data, Color.BLUE);
			AddSeries2(chart, 1, data1, Color.YELLOW);
			AddRef(chart, 2, hpRef, Color.BLUE);
			AddRef(chart, 3, tqRef, Color.YELLOW);
			return chart;
		}

		private void ConfigurePlot(JFreeChart chart)
		{
			plot = chart.GetXYPlot();
			plot.SetOutlinePaint(DARK_GREY);
			plot.SetBackgroundPaint(Color.BLACK);
			// X axis settings
			plot.SetDomainAxisLocation(AxisLocation.BOTTOM_OR_RIGHT);
			plot.GetDomainAxis().SetLabelPaint(Color.WHITE);
			plot.GetDomainAxis().SetTickLabelPaint(LIGHT_GREY);
			plot.SetDomainGridlinePaint(DARK_GREY);
			// Y1 axis (left) settings
			hpAxis.SetLabel(labelY1);
			hpAxis.SetLabelPaint(Color.BLUE);
			hpAxis.SetTickLabelPaint(LIGHT_GREY);
			hpAxis.SetAutoRangeIncludesZero(false);
			hpAxis.SetAutoRange(true);
			plot.SetRangeAxis(0, hpAxis);
			plot.SetRangeAxisLocation(0, AxisLocation.TOP_OR_LEFT);
			plot.MapDatasetToRangeAxis(0, 0);
			plot.MapDatasetToRangeAxis(2, 0);
			// Y2 axis (right) settings
			tqAxis.SetLabel(labelY2);
			tqAxis.SetLabelPaint(Color.YELLOW);
			tqAxis.SetTickLabelPaint(LIGHT_GREY);
			tqAxis.SetAutoRangeIncludesZero(false);
			tqAxis.SetAutoRange(true);
			plot.SetRangeAxis(1, tqAxis);
			plot.SetRangeAxisLocation(1, AxisLocation.BOTTOM_OR_RIGHT);
			plot.MapDatasetToRangeAxis(1, 1);
			plot.MapDatasetToRangeAxis(3, 1);
			plot.SetRangeGridlinePaint(DARK_GREY);
			refStat.SetPaint(Color.WHITE);
			refStat.SetTextAnchor(TextAnchor.TOP_LEFT);
			refStat.SetFont(new Font(Font.SANS_SERIF, Font.BOLD, 12));
		}

		public void SetET()
		{
			Clear();
			plot.GetDomainAxis().SetLabel("Time (seconds)");
			hpAxis.SetLabel("Vehicle Speed");
			tqAxis.SetLabel(" ");
		}

		public void SetDyno()
		{
			Clear();
			plot.GetDomainAxis().SetLabel("Engine Speed (RPM)");
			hpAxis.SetLabel("Calculated Wheel Power");
			tqAxis.SetLabel("Calculated Engine Torque");
		}

		public void StartPrompt(string select)
		{
			string startPrompt = START_PROMPT;
			if (Sharpen.Runtime.EqualsIgnoreCase(select, "mph"))
			{
				startPrompt = ET_PROMPT_I;
			}
			if (Sharpen.Runtime.EqualsIgnoreCase(select, "km/h"))
			{
				startPrompt = ET_PROMPT_M;
			}
			double x = ((plot.GetDomainAxis().GetUpperBound() - plot.GetDomainAxis().GetLowerBound
				()) / 2) + plot.GetDomainAxis().GetLowerBound();
			double y = ((hpAxis.GetUpperBound() - hpAxis.GetLowerBound()) / 2) + hpAxis.GetLowerBound
				();
			XYTextAnnotation startMessage = new XYTextAnnotation(startPrompt, x, y);
			startMessage.SetPaint(Color.GREEN);
			startMessage.SetTextAnchor(TextAnchor.BOTTOM_CENTER);
			startMessage.SetFont(new Font("Arial", Font.BOLD, 20));
			plot.AddAnnotation(startMessage);
		}

		public void ClearPrompt()
		{
			plot.ClearAnnotations();
		}

		private void AddSeries1(JFreeChart chart, int index, XYSeries series, Color color
			)
		{
			XYDataset dataset = new XYSeriesCollection(series);
			XYPlot plot = chart.GetXYPlot();
			plot.SetDataset(index, dataset);
			plot.SetRenderer(index, BuildTrendLineRendererY1(color));
		}

		private void AddSeries2(JFreeChart chart, int index, XYSeries series, Color color
			)
		{
			XYDataset dataset = new XYSeriesCollection(series);
			XYPlot plot = chart.GetXYPlot();
			plot.SetDataset(index, dataset);
			plot.SetRenderer(index, BuildTrendLineRendererY2(color));
		}

		private void AddRef(JFreeChart chart, int index, XYSeries series, Color color)
		{
			XYDataset dataset = new XYSeriesCollection(series);
			XYPlot plot = chart.GetXYPlot();
			plot.SetDataset(index, dataset);
			plot.SetRenderer(index, BuildTrendLineRenderer(color));
		}

		private StandardXYItemRenderer BuildTrendLineRenderer(Color color)
		{
			StandardXYItemRenderer renderer = new StandardXYItemRenderer();
			renderer.SetSeriesPaint(0, color);
			float[] dash = new float[] { 2.0f };
			renderer.SetSeriesStroke(0, new BasicStroke(0.8f, BasicStroke.CAP_BUTT, BasicStroke
				.JOIN_MITER, 10.0f, dash, 0.0f));
			return renderer;
		}

		private StandardXYItemRenderer BuildTrendLineRendererY1(Color color)
		{
			rendererY1.SetSeriesPaint(0, color);
			rendererY1.SetSeriesStroke(0, new BasicStroke(1.6f));
			return rendererY1;
		}

		private StandardXYItemRenderer BuildTrendLineRendererY2(Color color)
		{
			rendererY2.SetSeriesPaint(0, color);
			rendererY2.SetSeriesStroke(0, new BasicStroke(1.6f));
			return rendererY2;
		}
	}
}

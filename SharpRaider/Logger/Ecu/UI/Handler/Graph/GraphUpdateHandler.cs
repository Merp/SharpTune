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

using System.Collections.Generic;
using Java.Awt;
using Java.Awt.Event;
using Javax.Swing;
using Org.Jfree.Chart;
using Org.Jfree.Chart.Plot;
using Org.Jfree.Chart.Renderer.XY;
using Org.Jfree.Chart.Title;
using Org.Jfree.Data.XY;
using RomRaider.Logger.Ecu.Comms.Query;
using RomRaider.Logger.Ecu.Definition;
using RomRaider.Logger.Ecu.UI.Handler;
using RomRaider.Logger.Ecu.UI.Handler.Graph;
using Sharpen;

namespace RomRaider.Logger.Ecu.UI.Handler.Graph
{
	public sealed class GraphUpdateHandler : DataUpdateHandler, ConvertorUpdateListener
	{
		private static readonly Color DARK_GREY = new Color(80, 80, 80);

		private static readonly Color LIGHT_GREY = new Color(110, 110, 110);

		private readonly IDictionary<LoggerData, ChartPanel> chartMap = Sharpen.Collections.SynchronizedMap
			(new Dictionary<LoggerData, ChartPanel>());

		private readonly IDictionary<LoggerData, XYSeries> seriesMap = Sharpen.Collections.SynchronizedMap
			(new Dictionary<LoggerData, XYSeries>());

		private readonly IDictionary<LoggerData, int> datasetIndexes = Sharpen.Collections.SynchronizedMap
			(new Dictionary<LoggerData, int>());

		private readonly JPanel graphPanel;

		private long startTime = Runtime.CurrentTimeMillis();

		private bool combinedChart = false;

		private bool paused = false;

		private long pauseStartTime = Runtime.CurrentTimeMillis();

		private ChartPanel combinedChartPanel = null;

		private int counter = 0;

		public GraphUpdateHandler(JPanel panel)
		{
			this.graphPanel = new JPanel(new SpringLayout());
			JCheckBox combinedCheckbox = new JCheckBox("Combine Graphs", combinedChart);
			combinedCheckbox.AddActionListener(new GraphUpdateHandler.CombinedActionListener(
				this, combinedCheckbox));
			JToggleButton playPauseButton = new JToggleButton("Pause Graphs");
			playPauseButton.AddActionListener(new _ActionListener_77(this));
			panel.GetInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW).Put(KeyStroke.GetKeyStroke("F12"
				), "toggleCombineGraphs");
			panel.GetActionMap().Put("toggleCombineGraphs", new _AbstractAction_88(combinedCheckbox
				));
			JPanel controlPanel = new JPanel();
			controlPanel.Add(combinedCheckbox);
			controlPanel.Add(playPauseButton);
			panel.Add(controlPanel, BorderLayout.NORTH);
			panel.Add(this.graphPanel, BorderLayout.CENTER);
		}

		private sealed class _ActionListener_77 : ActionListener
		{
			public _ActionListener_77(GraphUpdateHandler _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void ActionPerformed(ActionEvent actionEvent)
			{
				this._enclosing.paused = !this._enclosing.paused;
				if (this._enclosing.paused)
				{
					this._enclosing.pauseStartTime = Runtime.CurrentTimeMillis();
				}
				else
				{
					this._enclosing.startTime = this._enclosing.startTime + (Runtime.CurrentTimeMillis
						() - this._enclosing.pauseStartTime);
				}
			}

			private readonly GraphUpdateHandler _enclosing;
		}

		private sealed class _AbstractAction_88 : AbstractAction
		{
			public _AbstractAction_88(JCheckBox combinedCheckbox)
			{
				this.combinedCheckbox = combinedCheckbox;
				this.serialVersionUID = 1540427179539775534L;
			}

			private const long serialVersionUID;

			public override void ActionPerformed(ActionEvent e)
			{
				combinedCheckbox.DoClick();
			}

			private readonly JCheckBox combinedCheckbox;
		}

		public void RegisterData(LoggerData loggerData)
		{
			lock (this)
			{
				// add to charts
				RegisterSeries(loggerData);
				if (combinedChart)
				{
					AddToCombined(loggerData);
					LayoutForCombined();
				}
				else
				{
					AddToPanel(loggerData);
					LayoutForPanel();
				}
				graphPanel.UpdateUI();
			}
		}

		private void AddToPanel(LoggerData loggerData)
		{
			lock (this)
			{
				XYSeries series = seriesMap.Get(loggerData);
				ChartPanel chartPanel = new ChartPanel(CreateXYLineChart(loggerData, new XYSeriesCollection
					(series), false), false, true, true, true, true);
				chartPanel.SetMinimumSize(new Dimension(600, 200));
				chartPanel.SetMaximumSize(new Dimension(10000, 200));
				chartPanel.SetPreferredSize(new Dimension(600, 200));
				chartMap.Put(loggerData, chartPanel);
				graphPanel.Add(chartPanel);
			}
		}

		private void LayoutForPanel()
		{
			SpringUtilities.MakeCompactGrid(graphPanel, seriesMap.Count, 1, 2, 2, 2, 2);
		}

		private void AddToCombined(LoggerData loggerData)
		{
			lock (this)
			{
				if (combinedChartPanel == null)
				{
					combinedChartPanel = new ChartPanel(CreateXYLineChart(loggerData, null, true), false
						, true, true, true, true);
					LegendTitle legendTitle = new LegendTitle(combinedChartPanel.GetChart().GetXYPlot
						());
					legendTitle.SetItemPaint(Color.WHITE);
					combinedChartPanel.GetChart().AddLegend(legendTitle);
					combinedChartPanel.SetMinimumSize(new Dimension(500, 400));
					combinedChartPanel.SetPreferredSize(new Dimension(500, 400));
					graphPanel.Add(combinedChartPanel);
				}
				XYPlot plot = combinedChartPanel.GetChart().GetXYPlot();
				plot.SetDataset(counter, new XYSeriesCollection(seriesMap.Get(loggerData)));
				plot.SetRenderer(counter, new StandardXYItemRenderer());
				datasetIndexes.Put(loggerData, counter++);
			}
		}

		private void LayoutForCombined()
		{
			SpringUtilities.MakeCompactGrid(graphPanel, 1, 1, 2, 2, 2, 2);
		}

		public void HandleDataUpdate(Response response)
		{
			lock (this)
			{
				foreach (LoggerData loggerData in response.GetData())
				{
					XYSeries series = seriesMap.Get(loggerData);
					if (series != null && !paused)
					{
						SwingUtilities.InvokeLater(new _Runnable_153(this, series, response, loggerData));
					}
				}
			}
		}

		private sealed class _Runnable_153 : Runnable
		{
			public _Runnable_153(GraphUpdateHandler _enclosing, XYSeries series, Response response
				, LoggerData loggerData)
			{
				this._enclosing = _enclosing;
				this.series = series;
				this.response = response;
				this.loggerData = loggerData;
			}

			public void Run()
			{
				series.Add((response.GetTimestamp() - this._enclosing.startTime) / 1000.0, response
					.GetDataValue(loggerData));
			}

			private readonly GraphUpdateHandler _enclosing;

			private readonly XYSeries series;

			private readonly Response response;

			private readonly LoggerData loggerData;
		}

		public void DeregisterData(LoggerData loggerData)
		{
			lock (this)
			{
				Sharpen.Collections.Remove(seriesMap, loggerData);
				if (combinedChart)
				{
					RemoveFromCombined(loggerData);
				}
				else
				{
					RemoveFromPanel(loggerData);
					LayoutForPanel();
				}
				graphPanel.UpdateUI();
			}
		}

		private void RemoveFromCombined(LoggerData loggerData)
		{
			// remove from charts
			if (datasetIndexes.ContainsKey(loggerData))
			{
				combinedChartPanel.GetChart().GetXYPlot().SetDataset(datasetIndexes.Get(loggerData
					), null);
			}
			Sharpen.Collections.Remove(datasetIndexes, loggerData);
			Sharpen.Collections.Remove(chartMap, loggerData);
			if (datasetIndexes.IsEmpty())
			{
				graphPanel.Remove(combinedChartPanel);
				combinedChartPanel = null;
			}
		}

		private void RemoveFromPanel(LoggerData loggerData)
		{
			// remove from charts
			graphPanel.Remove(chartMap.Get(loggerData));
			Sharpen.Collections.Remove(datasetIndexes, loggerData);
			Sharpen.Collections.Remove(chartMap, loggerData);
		}

		public void CleanUp()
		{
			lock (this)
			{
			}
		}

		public void Reset()
		{
			lock (this)
			{
				foreach (XYSeries series in seriesMap.Values)
				{
					series.Clear();
				}
			}
		}

		public void NotifyConvertorUpdate(LoggerData updatedLoggerData)
		{
			lock (this)
			{
				if (chartMap.ContainsKey(updatedLoggerData))
				{
					seriesMap.Get(updatedLoggerData).Clear();
					JFreeChart chart = chartMap.Get(updatedLoggerData).GetChart();
					chart.GetXYPlot().GetRangeAxis().SetLabel(BuildRangeAxisTitle(updatedLoggerData));
				}
			}
		}

		private void RegisterSeries(LoggerData loggerData)
		{
			XYSeries series = new XYSeries(loggerData.GetName());
			series.SetMaximumItemCount(200);
			seriesMap.Put(loggerData, series);
		}

		private JFreeChart CreateXYLineChart(LoggerData loggerData, XYDataset dataset, bool
			 combined)
		{
			string title = combined ? "Combined Data" : loggerData.GetName();
			string rangeAxisTitle = combined ? "Data" : BuildRangeAxisTitle(loggerData);
			JFreeChart chart = ChartFactory.CreateXYLineChart(title, "Time (sec)", rangeAxisTitle
				, dataset, PlotOrientation.VERTICAL, false, true, false);
			chart.SetBackgroundPaint(Color.BLACK);
			chart.GetTitle().SetPaint(Color.WHITE);
			XYPlot plot = chart.GetXYPlot();
			plot.SetBackgroundPaint(Color.BLACK);
			plot.GetDomainAxis().SetLabelPaint(Color.WHITE);
			plot.GetRangeAxis().SetLabelPaint(Color.WHITE);
			plot.GetDomainAxis().SetTickLabelPaint(LIGHT_GREY);
			plot.GetRangeAxis().SetTickLabelPaint(LIGHT_GREY);
			plot.SetDomainGridlinePaint(DARK_GREY);
			plot.SetRangeGridlinePaint(DARK_GREY);
			plot.SetOutlinePaint(DARK_GREY);
			return chart;
		}

		private string BuildRangeAxisTitle(LoggerData loggerData)
		{
			return loggerData.GetName() + " (" + loggerData.GetSelectedConvertor().GetUnits()
				 + ")";
		}

		private sealed class CombinedActionListener : ActionListener
		{
			private readonly JCheckBox combinedCheckbox;

			private CombinedActionListener(GraphUpdateHandler _enclosing, JCheckBox combinedCheckbox
				)
			{
				this._enclosing = _enclosing;
				this.combinedCheckbox = combinedCheckbox;
			}

			public void ActionPerformed(ActionEvent actionEvent)
			{
				this._enclosing.combinedChart = this.combinedCheckbox.IsSelected();
				if (this._enclosing.combinedChart)
				{
					this.RemoveAllFromPanel();
					this.AddAllToCombined();
					this._enclosing.LayoutForCombined();
				}
				else
				{
					this.RemoveAllFromCombined();
					this.AddAllToPanel();
					this._enclosing.LayoutForPanel();
				}
				this._enclosing.graphPanel.UpdateUI();
			}

			private void AddAllToCombined()
			{
				foreach (LoggerData loggerData in this._enclosing.seriesMap.Keys)
				{
					this._enclosing.AddToCombined(loggerData);
				}
			}

			private void RemoveAllFromPanel()
			{
				foreach (LoggerData loggerData in this._enclosing.seriesMap.Keys)
				{
					this._enclosing.RemoveFromPanel(loggerData);
				}
			}

			private void AddAllToPanel()
			{
				foreach (LoggerData loggerData in this._enclosing.seriesMap.Keys)
				{
					this._enclosing.AddToPanel(loggerData);
				}
			}

			private void RemoveAllFromCombined()
			{
				foreach (LoggerData loggerData in this._enclosing.seriesMap.Keys)
				{
					this._enclosing.RemoveFromCombined(loggerData);
				}
			}

			private readonly GraphUpdateHandler _enclosing;
		}
	}
}

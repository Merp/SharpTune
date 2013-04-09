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
using System.Collections.Generic;
using Jamlab;
using Org.Jfree.Data.XY;
using RomRaider.Util;
using Sharpen;
using Sharpen.Reflect;

namespace RomRaider.Logger.Ecu.UI.Tab
{
	[System.Serializable]
	public sealed class XYTrendline : AbstractXYDataset
	{
		private const long serialVersionUID = 1375705537694372443L;

		private IList<XYDataItem> items = new AList<XYDataItem>();

		private double[] xVals = new double[0];

		private double[] yPoly = new double[0];

		private readonly XYSeries series;

		private Polyfit polyfit;

		public XYTrendline(XYSeries series)
		{
			ParamChecker.CheckNotNull(series);
			this.series = series;
		}

		public override int GetSeriesCount()
		{
			return 1;
		}

		public override IComparable GetSeriesKey(int seriesIndex)
		{
			return "foo";
		}

		public override int GetItemCount(int seriesIndex)
		{
			lock (this)
			{
				return yPoly.Length;
			}
		}

		public override Number GetY(int seriesIndex, int item)
		{
			lock (this)
			{
				return yPoly[item];
			}
		}

		public override Number GetX(int seriesIndex, int item)
		{
			lock (this)
			{
				return xVals[item];
			}
		}

		public void Update(int order)
		{
			lock (this)
			{
				if (series.GetItemCount() <= order)
				{
					return;
				}
				items = new AList<XYDataItem>(series.GetItems());
				xVals = new double[items.Count];
				double[] yVals = new double[items.Count];
				for (int i = 0; i < items.Count; i++)
				{
					XYDataItem dataItem = items[i];
					xVals[i] = dataItem.GetX();
					yVals[i] = dataItem.GetY();
				}
				try
				{
					polyfit = new Polyfit(xVals, yVals, order);
					yPoly = Calculate(xVals);
				}
				catch (Exception e)
				{
					throw new UndeclaredThrowableException(e);
				}
			}
		}

		public double[] Calculate(double[] x)
		{
			lock (this)
			{
				if (polyfit == null)
				{
					throw new InvalidOperationException("Interpolation required");
				}
				Polyval polyval = new Polyval(x, polyfit);
				return polyval.GetYout();
			}
		}

		public Polyfit GetPolyFit()
		{
			lock (this)
			{
				if (polyfit == null)
				{
					throw new InvalidOperationException("Interpolation required");
				}
				return polyfit;
			}
		}

		public void Clear()
		{
			lock (this)
			{
				items.Clear();
				xVals = new double[0];
				yPoly = new double[0];
				polyfit = null;
			}
		}
	}
}

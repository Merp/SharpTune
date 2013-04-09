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

using RomRaider.Maps;
using Sharpen;

namespace RomRaider.Maps
{
	[System.Serializable]
	public class Scale
	{
		private const long serialVersionUID = 5836610685159474795L;

		public const int LINEAR = 1;

		public const int INVERSE = 2;

		private string name = "Default";

		private string unit = "0x";

		private string expression = "x";

		private string byteExpression = "x";

		private string format = "#";

		private double coarseIncrement = 2;

		private double fineIncrement = 1;

		private double min = 0;

		private double max = 0;

		private Table table;

		public Scale()
		{
		}

		//This object defines the scaling factor and offset for calculating real values
		public override string ToString()
		{
			return "\n      ---- Scale ----" + "\n      Name: " + GetName() + "\n      Expression: "
				 + GetExpression() + "\n      Unit: " + GetUnit() + "\n      ---- End Scale ----";
		}

		public virtual string GetUnit()
		{
			return unit;
		}

		public virtual void SetUnit(string unit)
		{
			this.unit = unit;
		}

		public virtual string GetFormat()
		{
			return format;
		}

		public virtual void SetFormat(string format)
		{
			this.format = format;
		}

		public virtual string GetExpression()
		{
			return expression;
		}

		public virtual void SetExpression(string expression)
		{
			this.expression = expression;
		}

		public virtual double GetCoarseIncrement()
		{
			return coarseIncrement;
		}

		public virtual void SetCoarseIncrement(double increment)
		{
			this.coarseIncrement = increment;
		}

		public virtual bool IsReady()
		{
			if (unit == null)
			{
				return false;
			}
			else
			{
				if (expression == null)
				{
					return false;
				}
				else
				{
					if (format == null)
					{
						return false;
					}
					else
					{
						if (coarseIncrement < 1)
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		public virtual string GetByteExpression()
		{
			return byteExpression;
		}

		public virtual void SetByteExpression(string byteExpression)
		{
			this.byteExpression = byteExpression;
		}

		public virtual double GetFineIncrement()
		{
			return fineIncrement;
		}

		public virtual void SetFineIncrement(double fineIncrement)
		{
			this.fineIncrement = fineIncrement;
		}

		public virtual string GetName()
		{
			return name;
		}

		public virtual void SetName(string name)
		{
			this.name = name;
		}

		public virtual double GetMin()
		{
			return min;
		}

		public virtual void SetMin(double min)
		{
			this.min = min;
		}

		public virtual double GetMax()
		{
			return max;
		}

		public virtual void SetMax(double max)
		{
			this.max = max;
		}

		public virtual Table GetTable()
		{
			return table;
		}

		public virtual void SetTable(Table table)
		{
			this.table = table;
		}
	}
}

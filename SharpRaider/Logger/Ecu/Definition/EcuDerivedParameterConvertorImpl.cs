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
using RomRaider.Logger.Ecu.Definition;
using RomRaider.Logger.Ecu.UI.Handler.Dash;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.Ecu.Definition
{
	public sealed class EcuDerivedParameterConvertorImpl : EcuDerivedParameterConvertor
	{
		private EcuData[] ecuDatas;

		private readonly string units;

		private readonly string expression;

		private readonly DecimalFormat format;

		private readonly IDictionary<string, string> replaceMap;

		private readonly IDictionary<string, EcuDerivedParameterConvertorImpl.ExpressionInfo
			> expressionInfoMap = Sharpen.Collections.SynchronizedMap(new Dictionary<string, 
			EcuDerivedParameterConvertorImpl.ExpressionInfo>());

		private readonly GaugeMinMax gaugeMinMax;

		public EcuDerivedParameterConvertorImpl(string units, string expression, string format
			, IDictionary<string, string> replaceMap, GaugeMinMax gaugeMinMax)
		{
			ParamChecker.CheckNotNullOrEmpty(units, "units");
			ParamChecker.CheckNotNullOrEmpty(expression, "expression");
			ParamChecker.CheckNotNullOrEmpty(format, "format");
			ParamChecker.CheckNotNull(replaceMap, "replaceMap");
			ParamChecker.CheckNotNull(gaugeMinMax, "gaugeMinMax");
			this.units = units;
			this.expression = expression;
			this.format = new DecimalFormat(format);
			this.replaceMap = replaceMap;
			this.gaugeMinMax = gaugeMinMax;
		}

		public double Convert(byte[] bytes)
		{
			IDictionary<string, double> valueMap = new Dictionary<string, double>();
			string exp = expression;
			int index = 0;
			foreach (EcuData ecuData in ecuDatas)
			{
				int length = ecuData.GetAddress().GetLength();
				byte[] tmp = new byte[length];
				System.Array.Copy(bytes, index, tmp, 0, length);
				EcuDerivedParameterConvertorImpl.ExpressionInfo expressionInfo = expressionInfoMap
					.Get(ecuData.GetId());
				valueMap.Put(expressionInfo.GetReplacementKey(), expressionInfo.GetConvertor().Convert
					(tmp));
				exp = exp.Replace(BuildParameterKey(expressionInfo), expressionInfo.GetReplacementKey
					());
				index += length;
			}
			double result = JEPUtil.Evaluate(exp, valueMap);
			return double.IsNaN(result) || double.IsInfinite(result) ? 0.0 : result;
		}

		public string GetUnits()
		{
			return units;
		}

		public GaugeMinMax GetGaugeMinMax()
		{
			return gaugeMinMax;
		}

		public string GetFormat()
		{
			return format.ToPattern();
		}

		public string Format(double value)
		{
			string formattedValue = format.Format(value);
			if (replaceMap.ContainsKey(formattedValue))
			{
				return replaceMap.Get(formattedValue);
			}
			else
			{
				return formattedValue;
			}
		}

		public void SetEcuDatas(EcuData[] ecuDatas)
		{
			ParamChecker.CheckNotNullOrEmpty(ecuDatas, "ecuDatas");
			this.ecuDatas = ecuDatas;
			foreach (EcuData ecuData in ecuDatas)
			{
				AddExpressionInfo(ecuData);
			}
		}

		public override string ToString()
		{
			return GetUnits();
		}

		private void AddExpressionInfo(EcuData ecuData)
		{
			string id = ecuData.GetId();
			string lookup = '[' + id + ':';
			int i = expression.IndexOf(lookup);
			if (i >= 0)
			{
				int start = i + lookup.Length;
				int end = expression.IndexOf("]", start);
				string units = Sharpen.Runtime.Substring(expression, start, end);
				EcuDataConvertor selectedConvertor = null;
				EcuDataConvertor[] convertors = ecuData.GetConvertors();
				foreach (EcuDataConvertor convertor in convertors)
				{
					if (units.Equals(convertor.GetUnits()))
					{
						selectedConvertor = convertor;
					}
				}
				expressionInfoMap.Put(id, new EcuDerivedParameterConvertorImpl.ExpressionInfo(id, 
					selectedConvertor, true));
			}
			else
			{
				expressionInfoMap.Put(id, new EcuDerivedParameterConvertorImpl.ExpressionInfo(id, 
					ecuData.GetSelectedConvertor(), false));
			}
		}

		private string BuildParameterKey(EcuDerivedParameterConvertorImpl.ExpressionInfo 
			expressionInfo)
		{
			return '[' + expressionInfo.GetEcuDataId() + ':' + expressionInfo.GetConvertor().
				GetUnits() + ']';
		}

		private sealed class ExpressionInfo
		{
			private readonly string ecuDataId;

			private readonly EcuDataConvertor convertor;

			private readonly string replacementKey;

			public ExpressionInfo(string ecuDataId, EcuDataConvertor convertor, bool compositeKey
				)
			{
				ParamChecker.CheckNotNull(ecuDataId, convertor);
				this.ecuDataId = ecuDataId;
				this.convertor = convertor;
				this.replacementKey = compositeKey ? BuildCompositeKey(ecuDataId, convertor.GetUnits
					()) : ecuDataId;
			}

			public string GetEcuDataId()
			{
				return ecuDataId;
			}

			public string GetReplacementKey()
			{
				return replacementKey;
			}

			public EcuDataConvertor GetConvertor()
			{
				return convertor;
			}

			private string BuildCompositeKey(string ecuDataId, string convertorUnits)
			{
				if (convertorUnits == null || convertorUnits.Length == 0)
				{
					return ecuDataId;
				}
				else
				{
					convertorUnits = convertorUnits.ReplaceAll("[^\\w]", "_");
					return '_' + ecuDataId + '_' + convertorUnits + '_';
				}
			}
		}
	}
}

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
using Sharpen;

namespace RomRaider.Util
{
	public sealed class ParamChecker
	{
		public ParamChecker()
		{
		}

		public static void CheckNotNull(object param, string paramName)
		{
			if (param == null)
			{
				throw new ArgumentException("Parameter " + paramName + " must not be null");
			}
		}

		public static void CheckNotNull(params object[] @params)
		{
			for (int i = 0; i < @params.Length; i++)
			{
				CheckNotNull(@params[i], "arg" + i);
			}
		}

		public static void CheckNotNullOrEmpty(string param, string paramName)
		{
			if (IsNullOrEmpty(param))
			{
				throw new ArgumentException("Parameter " + paramName + " must not be null or empty"
					);
			}
		}

		public static void CheckNotNullOrEmpty(object[] param, string paramName)
		{
			if (param == null || param.Length == 0)
			{
				throw new ArgumentException("Parameter " + paramName + " must not be null or empty"
					);
			}
		}

		public static void CheckNotNullOrEmpty<_T0>(ICollection<_T0> param, string paramName
			)
		{
			if (param == null || param.IsEmpty())
			{
				throw new ArgumentException("Parameter " + paramName + " must not be null or empty"
					);
			}
		}

		public static void CheckNotNullOrEmpty<_T0>(IDictionary<_T0> param, string paramName
			)
		{
			if (param == null || param.IsEmpty())
			{
				throw new ArgumentException("Parameter " + paramName + " must not be null or empty"
					);
			}
		}

		public static void CheckGreaterThanZero(int param, string paramName)
		{
			if (param <= 0)
			{
				throw new ArgumentException("Parameter " + paramName + " must be > 0");
			}
		}

		public static void CheckNotNullOrEmpty(byte[] param, string paramName)
		{
			if (param == null || param.Length == 0)
			{
				throw new ArgumentException("Parameter " + paramName + " must not be null or empty"
					);
			}
		}

		public static void CheckBit(int bit)
		{
			if (!IsValidBit(bit))
			{
				throw new ArgumentException("Bit must be between 0 and 7 inclusive.");
			}
		}

		public static bool IsNullOrEmpty(string param)
		{
			return param == null || param.Length == 0;
		}

		public static bool IsNullOrEmpty<_T0>(ICollection<_T0> param)
		{
			return param == null || param.IsEmpty();
		}

		public static bool IsNullOrEmpty<_T0>(IDictionary<_T0> param)
		{
			return param == null || param.IsEmpty();
		}

		public static bool IsValidBit(int bit)
		{
			return bit >= 0 && bit <= 7;
		}
	}
}

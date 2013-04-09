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
using RomRaider.Util.Proxy;
using Sharpen;
using Sharpen.Reflect;

namespace RomRaider.Util.Proxy
{
	public sealed class Proxifier
	{
		public static T Proxy<T, _T1>(T t, Type<_T1> cls) where _T1:Wrapper
		{
			Wrapper wrapper = Instantiate(cls, t);
			return Proxy(t, wrapper);
		}

		private static T Proxy<T>(T t, Wrapper wrapper)
		{
			Type cls = t.GetType();
			ClassLoader loader = cls.GetClassLoader();
			Type[] interfaces = cls.GetInterfaces();
			return (T)Sharpen.Reflect.Proxy.NewProxyInstance(loader, interfaces, wrapper);
		}

		private static Wrapper Instantiate<T, _T1>(Type<_T1> wrapper, T t) where _T1:Wrapper
		{
			try
			{
				Constructor<object> constructor = wrapper.GetConstructor(typeof(object));
				return (Wrapper)constructor.NewInstance(t);
			}
			catch (Exception e)
			{
				throw new RuntimeException(e);
			}
		}
	}
}

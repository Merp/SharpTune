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

using Org.Yaml.Snakeyaml.Introspector;
using Org.Yaml.Snakeyaml.Nodes;
using RomRaider.Yaml;
using Sharpen;

namespace RomRaider.Yaml
{
	public class SkipNullRepresenter : Org.Yaml.Snakeyaml.Representer.Representer
	{
		protected override NodeTuple RepresentJavaBeanProperty(object javaBean, Property 
			property, object propertyValue, Tag customTag)
		{
			if (propertyValue == null)
			{
				return null;
			}
			else
			{
				return base.RepresentJavaBeanProperty(javaBean, property, propertyValue, customTag
					);
			}
		}

		private SkipBean GetBean()
		{
			SkipBean bean = new SkipBean();
			bean.SetText("foo");
			bean.SetListDate(null);
			bean.SetListInt(Arrays.AsList(new int[] { null, 1, 2, 3 }));
			bean.SetListStr(Arrays.AsList(new string[] { "bar", null, "foo", null }));
			return bean;
		}
	}
}

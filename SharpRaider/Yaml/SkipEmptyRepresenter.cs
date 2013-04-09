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
using Sharpen;

namespace RomRaider.Yaml
{
	public class SkipEmptyRepresenter : Org.Yaml.Snakeyaml.Representer.Representer
	{
		public SkipEmptyRepresenter(PropertyUtils propUtils) : base()
		{
			this.SetPropertyUtils(propUtils);
		}

		public SkipEmptyRepresenter() : base()
		{
		}

		protected override NodeTuple RepresentJavaBeanProperty(object javaBean, Property 
			property, object propertyValue, Tag customTag)
		{
			NodeTuple tuple = null;
			tuple = base.RepresentJavaBeanProperty(javaBean, property, propertyValue, customTag
				);
			Node valueNode = tuple.GetValueNode();
			if (Tag.NULL.Equals(valueNode.GetTag()))
			{
				return null;
			}
			// skip 'null' values
			if (valueNode is CollectionNode)
			{
				if (Tag.SEQ.Equals(valueNode.GetTag()))
				{
					SequenceNode seq = (SequenceNode)valueNode;
					if (seq.GetValue().IsEmpty())
					{
						return null;
					}
				}
				// skip empty lists
				if (Tag.MAP.Equals(valueNode.GetTag()))
				{
					MappingNode seq = (MappingNode)valueNode;
					if (seq.GetValue().IsEmpty())
					{
						return null;
					}
				}
			}
			// skip empty maps
			return tuple;
		}
	}
}

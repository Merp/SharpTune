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

using Java.Awt;
using Org.Yaml.Snakeyaml.Introspector;
using Org.Yaml.Snakeyaml.Nodes;
using Org.Yaml.Snakeyaml.Representer;
using RomRaider.Yaml;
using Sharpen;

namespace RomRaider.Yaml
{
	/// <summary>
	/// Skips empty and null values and prevents stack overflow caused by recursion
	/// Due to java.awt.Point and Dimension classes returning instances of themselves (DERP).
	/// </summary>
	/// <remarks>
	/// Skips empty and null values and prevents stack overflow caused by recursion
	/// Due to java.awt.Point and Dimension classes returning instances of themselves (DERP).
	/// </remarks>
	public class SkipEmptyRepresenterAwtSafe : Org.Yaml.Snakeyaml.Representer.Representer
	{
		public SkipEmptyRepresenterAwtSafe(PropertyUtils propUtils) : base()
		{
			this.SetPropertyUtils(propUtils);
		}

		public SkipEmptyRepresenterAwtSafe() : base()
		{
			this.representers.Put(typeof(FilePath), (Represent)new SkipEmptyRepresenterAwtSafe.FileRepresenter
				(this));
		}

		protected override NodeTuple RepresentJavaBeanProperty(object javaBean, Property 
			property, object propertyValue, Tag customTag)
		{
			if (javaBean is Point && "location".Equals(property.GetName().ToLower()))
			{
				return null;
			}
			if (javaBean is Dimension && "size".Equals(property.GetName().ToLower()))
			{
				return null;
			}
			Node valueNode;
			NodeTuple tuple = null;
			tuple = base.RepresentJavaBeanProperty(javaBean, property, propertyValue, customTag
				);
			valueNode = tuple.GetValueNode();
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

		public class FileRepresenter : Represent
		{
			public virtual Node RepresentData(object data)
			{
				FilePath file = (FilePath)data;
				Node scalar = this._enclosing.RepresentScalar(new Tag("!!java.io.File"), file.GetAbsolutePath
					());
				return scalar;
			}

			internal FileRepresenter(SkipEmptyRepresenterAwtSafe _enclosing)
			{
				this._enclosing = _enclosing;
			}

			private readonly SkipEmptyRepresenterAwtSafe _enclosing;
		}
	}
}

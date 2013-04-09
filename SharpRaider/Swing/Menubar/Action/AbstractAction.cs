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
using Java.Awt.Event;
using Java.Beans;
using RomRaider.Logger.Ecu;
using Sharpen;

namespace RomRaider.Swing.Menubar.Action
{
	public abstract class AbstractAction : Javax.Swing.Action
	{
		public static readonly string SELECTED_KEY = "selected";

		private readonly IDictionary<string, object> valueMap = new Dictionary<string, object
			>();

		private bool enabled = true;

		protected internal EcuLogger logger;

		public AbstractAction(EcuLogger logger)
		{
			this.logger = logger;
		}

		public override bool IsEnabled()
		{
			return enabled;
		}

		public override void SetEnabled(bool enabled)
		{
			this.enabled = enabled;
		}

		public override void AddPropertyChangeListener(PropertyChangeListener propertyChangeListener
			)
		{
		}

		public override void RemovePropertyChangeListener(PropertyChangeListener propertyChangeListener
			)
		{
		}

		public override object GetValue(string key)
		{
			return valueMap.Get(key);
		}

		public override void PutValue(string key, object value)
		{
			valueMap.Put(key, value);
		}

		public abstract void ActionPerformed(ActionEvent arg1);
	}
}

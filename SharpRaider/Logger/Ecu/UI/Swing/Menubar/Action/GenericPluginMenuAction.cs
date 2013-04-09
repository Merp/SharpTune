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
using Gnu.IO;
using Java.Awt.Event;
using Javax.Swing;
using RomRaider.IO.Serial.Port;
using RomRaider.Logger.Ecu;
using RomRaider.Logger.External.Core;
using RomRaider.Swing.Menubar.Action;
using Sharpen;

namespace RomRaider.Logger.Ecu.UI.Swing.Menubar.Action
{
	public sealed class GenericPluginMenuAction : AbstractAction
	{
		private readonly SerialPortDiscoverer portDiscoverer = new SerialPortDiscovererImpl
			();

		private readonly ExternalDataSource dataSource;

		public GenericPluginMenuAction(EcuLogger logger, ExternalDataSource dataSource) : 
			base(logger)
		{
			this.dataSource = dataSource;
		}

		public override void ActionPerformed(ActionEvent actionEvent)
		{
			string port = (string)JOptionPane.ShowInputDialog(logger, "Select COM port:", dataSource
				.GetName() + " Plugin Settings", JOptionPane.QUESTION_MESSAGE, null, GetPorts(), 
				dataSource.GetPort());
			if (port != null && port.Length > 0)
			{
				dataSource.SetPort(port);
			}
		}

		private string[] GetPorts()
		{
			IList<CommPortIdentifier> portIdentifiers = portDiscoverer.ListPorts();
			string[] ports = new string[portIdentifiers.Count];
			for (int i = 0; i < portIdentifiers.Count; i++)
			{
				CommPortIdentifier identifier = portIdentifiers[i];
				ports[i] = identifier.GetName();
			}
			return ports;
		}
	}
}

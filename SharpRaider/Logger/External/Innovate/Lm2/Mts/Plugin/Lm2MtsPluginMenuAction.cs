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

using Java.Awt.Event;
using Javax.Swing;
using RomRaider.Logger.Ecu;
using RomRaider.Logger.External.Core;
using RomRaider.Logger.External.Innovate.Generic.Mts.IO;
using RomRaider.Swing.Menubar.Action;
using Sharpen;

namespace RomRaider.Logger.External.Innovate.Lm2.Mts.Plugin
{
	public sealed class Lm2MtsPluginMenuAction : AbstractAction
	{
		private readonly ExternalDataSource dataSource;

		public Lm2MtsPluginMenuAction(EcuLogger logger, ExternalDataSource dataSource) : 
			base(logger)
		{
			this.dataSource = dataSource;
		}

		public override void ActionPerformed(ActionEvent actionEvent)
		{
			string port = (string)JOptionPane.ShowInputDialog(logger, "Select MTS port:", dataSource
				.GetName() + " Plugin Settings", JOptionPane.QUESTION_MESSAGE, null, GetPorts(), 
				dataSource.GetPort());
			if (port != null && port.Length > 0)
			{
				port = Sharpen.Runtime.Substring(port, 0, 2).Trim();
				dataSource.SetPort(port);
			}
		}

		private string[] GetPorts()
		{
			string[] results;
			MTS mts = MTSFactory.CreateMTS();
			try
			{
				mts.Disconnect();
				int portCount = mts.PortCount();
				results = new string[portCount];
				results[0] = "-1 - [ no ports found ]";
				for (int i = 0; i < portCount; i++)
				{
					mts.CurrentPort(i);
					string name = mts.PortName();
					mts.Connect();
					int inputs = mts.InputCount();
					string result = string.Format("%d - [ %s: %d sesnors ]", i, name, inputs);
					results[i] = result;
					mts.Disconnect();
				}
			}
			finally
			{
				mts.Dispose();
			}
			return results;
		}
	}
}

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

using System.Text;
using Java.Awt.Event;
using Javax.Swing;
using RomRaider.Logger.Ecu;
using RomRaider.Logger.External.Phidget.Interfacekit.IO;
using RomRaider.Swing.Menubar.Action;
using Sharpen;

namespace RomRaider.Logger.External.Phidget.Interfacekit.Plugin
{
	/// <summary>
	/// IntfKitPluginMenuAction is used to populate the Phidgets Plugins menu
	/// of the Logger.
	/// </summary>
	/// <remarks>
	/// IntfKitPluginMenuAction is used to populate the Phidgets Plugins menu
	/// of the Logger. It will report the device type and serial number of each
	/// PhidgetInterfaceKit found connected to the system.
	/// This is informational only.
	/// </remarks>
	public sealed class IntfKitPluginMenuAction : AbstractAction
	{
		/// <summary>Initialise the Phidgets Plugins menu item.</summary>
		/// <remarks>Initialise the Phidgets Plugins menu item.</remarks>
		/// <param name="logger">- the parent frame to bind the dialog message to</param>
		public IntfKitPluginMenuAction(EcuLogger logger) : base(logger)
		{
		}

		public override void ActionPerformed(ActionEvent actionEvent)
		{
			JOptionPane.ShowMessageDialog(logger, GetKits(), "Interface Kits found", JOptionPane
				.INFORMATION_MESSAGE);
		}

		/// <summary>Build a list of device types with serial numbers attached to the system.
		/// 	</summary>
		/// <remarks>Build a list of device types with serial numbers attached to the system.
		/// 	</remarks>
		/// <returns>a formated string to be displayed in the message box</returns>
		/// <seealso cref="RomRaider.Logger.External.Phidget.Interfacekit.IO.IntfKitManager">RomRaider.Logger.External.Phidget.Interfacekit.IO.IntfKitManager
		/// 	</seealso>
		private string GetKits()
		{
			int[] kits = IntfKitManager.FindIntfkits();
			StringBuilder sb = new StringBuilder();
			if (kits.Length < 1)
			{
				sb.Append("No Interface Kits attached");
			}
			else
			{
				IntfKitManager.LoadIk();
				foreach (int serial in kits)
				{
					string result = IntfKitManager.GetIkName(serial);
					if (result != null)
					{
						sb.Append(result);
					}
					else
					{
						sb.Append("Unable to read properties of serial # " + serial + ", it may be in use"
							);
					}
					sb.Append("\n");
				}
			}
			return sb.ToString();
		}
	}
}

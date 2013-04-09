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
using Javax.Swing;
using RomRaider;
using RomRaider.IO.Serial.Port;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.Ecu.UI
{
	[System.Serializable]
	public sealed class SerialPortComboBox : JComboBox, SerialPortRefreshListener
	{
		private const long serialVersionUID = 5693976713268676676L;

		private readonly Settings settings;

		public SerialPortComboBox(Settings settings)
		{
			ParamChecker.CheckNotNull(settings);
			this.settings = settings;
		}

		public void RefreshPortList(ICollection<string> ports, string defaultSelectedPort
			)
		{
			lock (this)
			{
				ParamChecker.CheckNotNull(ports);
				bool changeDetected = ports.IsEmpty() || ports.Count != GetItemCount();
				if (!changeDetected)
				{
					for (int i = 0; i < GetItemCount(); i++)
					{
						string port = (string)GetItemAt(i);
						if (!ports.Contains(port))
						{
							changeDetected = true;
							break;
						}
					}
					if (!changeDetected)
					{
						ICollection<string> comboPorts = new TreeSet<string>();
						for (int i_1 = 0; i_1 < GetItemCount(); i_1++)
						{
							comboPorts.AddItem((string)GetItemAt(i_1));
						}
						foreach (string port in ports)
						{
							if (!comboPorts.Contains(port))
							{
								changeDetected = true;
								break;
							}
						}
					}
				}
				if (changeDetected)
				{
					string selectedPort = (string)GetSelectedItem();
					if (selectedPort == null)
					{
						selectedPort = defaultSelectedPort;
					}
					RemoveAllItems();
					if (!ports.IsEmpty())
					{
						foreach (string port in ports)
						{
							AddItem(port);
						}
						if (selectedPort != null && ports.Contains(selectedPort))
						{
							SetSelectedItem(selectedPort);
							settings.SetLoggerPort(selectedPort);
						}
						else
						{
							SetSelectedIndex(0);
							settings.SetLoggerPort((string)GetItemAt(0));
						}
					}
				}
			}
		}

		public override void SetSelectedItem(object @object)
		{
			if (Contains(@object))
			{
				base.SetSelectedItem(@object);
			}
			else
			{
				if (GetItemCount() >= 1)
				{
					SetSelectedIndex(0);
				}
			}
		}

		private bool Contains(object @object)
		{
			for (int i = 0; i < GetItemCount(); i++)
			{
				if (GetItemAt(i) != null && GetItemAt(i).Equals(@object))
				{
					return true;
				}
			}
			return false;
		}
	}
}

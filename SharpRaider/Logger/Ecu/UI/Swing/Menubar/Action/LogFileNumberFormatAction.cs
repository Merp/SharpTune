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
using Java.Awt.Event;
using Javax.Swing;
using RomRaider.Logger.Ecu;
using RomRaider.Swing.Menubar.Action;
using Sharpen;

namespace RomRaider.Logger.Ecu.UI.Swing.Menubar.Action
{
	public sealed class LogFileNumberFormatAction : AbstractAction
	{
		private static readonly string EN_US = "en_US";

		private static readonly string SYSTEM_NUMFORMAT = "system";

		public LogFileNumberFormatAction(EcuLogger logger) : base(logger)
		{
		}

		public override void ActionPerformed(ActionEvent actionEvent)
		{
			try
			{
				if ((bool)GetValue(SELECTED_KEY))
				{
					logger.GetSettings().SetLocale(EN_US);
				}
				else
				{
					logger.GetSettings().SetLocale(SYSTEM_NUMFORMAT);
				}
				JOptionPane.ShowMessageDialog(logger, "The Logger has been set to use the " + logger
					.GetSettings().GetLocale() + " number format.\n\n" + "Exit and restart the Logger to apply the new setting."
					, "Log File Number Format Change", JOptionPane.INFORMATION_MESSAGE);
			}
			catch (Exception e)
			{
				logger.ReportError(e);
			}
		}
	}
}

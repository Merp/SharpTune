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
	public sealed class ResetEcuAction : AbstractAction
	{
		public ResetEcuAction(EcuLogger logger) : base(logger)
		{
		}

		public override void ActionPerformed(ActionEvent actionEvent)
		{
			if (ShowConfirmation() == JOptionPane.OK_OPTION)
			{
				bool logging = logger.IsLogging();
				if (logging)
				{
					logger.StopLogging();
				}
				ResetEcu();
				if (logging)
				{
					logger.StartLogging();
				}
			}
		}

		private int ShowConfirmation()
		{
			return JOptionPane.ShowConfirmDialog(logger, "Do you want to reset the " + logger
				.GetTarget() + "?", "Reset " + logger.GetTarget(), JOptionPane.YES_NO_OPTION, JOptionPane
				.WARNING_MESSAGE);
		}

		private void ResetEcu()
		{
			if (DoReset())
			{
				JOptionPane.ShowMessageDialog(logger, "Reset Successful!\nTurn your ignition OFF and then\nback ON to complete the process."
					, "Reset " + logger.GetTarget(), JOptionPane.INFORMATION_MESSAGE);
			}
			else
			{
				JOptionPane.ShowMessageDialog(logger, "Error performing " + logger.GetTarget() + 
					" reset.\nCheck the following:\n* Correct COM port selected\n" + "* Cable is connected properly\n* Ignition is ON\n* Logger is stopped"
					, "Reset " + logger.GetTarget(), JOptionPane.ERROR_MESSAGE);
			}
		}

		private bool DoReset()
		{
			try
			{
				return logger.ResetEcu();
			}
			catch (Exception e)
			{
				logger.ReportError("Error performing " + logger.GetTarget() + " reset", e);
				return false;
			}
		}
	}
}

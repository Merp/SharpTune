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

using RomRaider.Logger.Ecu.Comms.Manager;
using Sharpen;

namespace RomRaider.Logger.Ecu.Comms.Manager
{
	public sealed class PollingStateImpl : PollingState
	{
		private static int currentState;

		private static int lastpollState;

		private static bool newQuery;

		private static bool lastQuery;

		private static bool fastPoll;

		public PollingStateImpl()
		{
			SetCurrentState(0);
			SetLastState(0);
			SetNewQuery(true);
			SetLastQuery(false);
			SetFastPoll(false);
		}

		public int GetCurrentState()
		{
			return currentState;
		}

		public void SetCurrentState(int i)
		{
			currentState = i;
		}

		public int GetLastState()
		{
			return lastpollState;
		}

		public void SetLastState(int i)
		{
			lastpollState = i;
		}

		public bool IsNewQuery()
		{
			return newQuery;
		}

		public void SetNewQuery(bool state)
		{
			newQuery = state;
		}

		public bool IsLastQuery()
		{
			return lastQuery;
		}

		public void SetLastQuery(bool state)
		{
			lastQuery = state;
		}

		public bool IsFastPoll()
		{
			return fastPoll;
		}

		public void SetFastPoll(bool state)
		{
			fastPoll = state;
		}

		public override string ToString()
		{
			string state = string.Format("Polling State [isFastPoll=%s, CurrentState=%d, LastState=%d, "
				 + "isNewQuery=%s, isLastQuery=%s]", IsFastPoll(), GetCurrentState(), GetLastState
				(), IsNewQuery(), IsLastQuery());
			return state;
		}
	}
}

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

using RomRaider.IO.J2534.Api;
using Sharpen;

namespace RomRaider.IO.J2534.Api
{
	public interface J2534
	{
		int Open();

		Version ReadVersion(int deviceId);

		int Connect(int deviceId, int flags, int baud);

		void SetConfig(int channelId, params ConfigItem[] items);

		ConfigItem[] GetConfig(int channelId, params int[] parameters);

		int StartPassMsgFilter(int channelId, byte mask, byte pattern);

		int StartPassMsgFilter(int channelId, byte[] mask, byte[] pattern);

		int StartBlockMsgFilter(int channelId, byte[] mask, byte[] pattern);

		int StartFlowCntrlFilter(int channelId, byte[] mask, byte[] pattern, byte[] flowCntrl
			, J2534Impl.TxFlags flag);

		byte[] FastInit(int channelId, byte[] input);

		double GetVbattery(int deviceId);

		void WriteMsg(int channelId, byte[] data, long timeout, J2534Impl.TxFlags flag);

		byte[] ReadMsg(int channelId, int numMsg, long timeout);

		byte[] ReadMsg(int channelId, long maxWait);

		void ReadMsg(int channelId, byte[] response, long timeout);

		void StopMsgFilter(int channelId, int msgId);

		void ClearBuffers(int channelId);

		void Disconnect(int channelId);

		void Close(int deviceId);
	}
}

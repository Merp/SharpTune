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

using Com.Sun.Jna;
using Com.Sun.Jna.Ptr;
using RomRaider.IO.J2534.Api;
using Sharpen;

namespace RomRaider.IO.J2534.Api
{
	/// <summary>JNA Wrapper for Native library <b>J2534 v0404</b><br /></summary>
	public class J2534_v0404 : Library
	{
		public J2534_v0404(string library)
		{
			NativeLibrary.GetInstance(library);
			Native.Register(library);
		}

		public virtual NativeLong PassThruOpen(Pointer pName, NativeLongByReference pDeviceID
			)
		{
		}

		public virtual NativeLong PassThruClose(NativeLong DeviceID)
		{
		}

		public virtual NativeLong PassThruConnect(NativeLong DeviceID, NativeLong protocolID
			, NativeLong flags, NativeLong baud, NativeLongByReference pChannelID)
		{
		}

		public virtual NativeLong PassThruDisconnect(NativeLong channelID)
		{
		}

		public virtual NativeLong PassThruReadMsgs(NativeLong ChannelID, Pointer pMsg, NativeLongByReference
			 pNumMsgs, NativeLong Timeout)
		{
		}

		public virtual NativeLong PassThruWriteMsgs(NativeLong ChannelID, Pointer pMsg, NativeLongByReference
			 pNumMsgs, NativeLong Timeout)
		{
		}

		public virtual NativeLong PassThruStartPeriodicMsg(NativeLong channelID, Pointer 
			pMsg, NativeLongByReference pMsgID, NativeLong timeInterval)
		{
		}

		public virtual NativeLong PassThruStopPeriodicMsg(NativeLong channelID, NativeLong
			 msgID)
		{
		}

		public virtual NativeLong PassThruStartMsgFilter(NativeLong ChannelID, NativeLong
			 FilterType, Pointer pMaskMsg, Pointer pPatternMsg, Pointer pFlowControlMsg, NativeLongByReference
			 pMsgID)
		{
		}

		public virtual NativeLong PassThruStopMsgFilter(NativeLong channelID, NativeLong 
			msgID)
		{
		}

		public virtual NativeLong PassThruSetProgrammingVoltage(NativeLong pinNumber, NativeLong
			 voltage)
		{
		}

		public virtual NativeLong PassThruReadVersion(NativeLong DeviceID, ByteBuffer pFirmwareVersion
			, ByteBuffer pDllVersion, ByteBuffer pApiVersion)
		{
		}

		public virtual NativeLong PassThruGetLastError(ByteBuffer pErrorDescription)
		{
		}

		public virtual NativeLong PassThruIoctl(NativeLong channelID, NativeLong ioctlID, 
			Pointer pInput, Pointer pOutput)
		{
		}

		public class SCONFIG : Structure
		{
			public NativeLong parameter;

			public NativeLong value;

			public SCONFIG() : base()
			{
				InitFieldOrder();
			}

			protected internal virtual void InitFieldOrder()
			{
				SetFieldOrder(new string[] { "parameter", "value" });
			}

			public class ByReference : J2534_v0404.SCONFIG, Structure.ByReference
			{
			}

			public class ByValue : J2534_v0404.SCONFIG, Structure.ByValue
			{
			}
		}

		public class SCONFIG_LIST : Structure
		{
			public NativeLong numOfParams;

			public J2534_v0404.SCONFIG.ByReference configPtr;

			public SCONFIG_LIST() : base()
			{
				InitFieldOrder();
			}

			protected internal virtual void InitFieldOrder()
			{
				SetFieldOrder(new string[] { "numOfParams", "configPtr" });
			}

			public class ByReference : J2534_v0404.SCONFIG_LIST, Structure.ByReference
			{
			}

			public class ByValue : J2534_v0404.SCONFIG_LIST, Structure.ByValue
			{
			}
		}

		public class PASSTHRU_MSG : Structure
		{
			public NativeLong protocolID;

			public NativeLong rxStatus;

			public NativeLong txFlags;

			public NativeLong timestamp;

			public NativeLong dataSize;

			public NativeLong extraDataIndex;

			public byte[] data = new byte[4128];

			public PASSTHRU_MSG() : base()
			{
				InitFieldOrder();
			}

			protected internal virtual void InitFieldOrder()
			{
				SetFieldOrder(new string[] { "protocolID", "rxStatus", "txFlags", "timestamp", "dataSize"
					, "extraDataIndex", "data" });
			}

			public class ByReference : J2534_v0404.PASSTHRU_MSG, Structure.ByReference
			{
			}

			public class ByValue : J2534_v0404.PASSTHRU_MSG, Structure.ByValue
			{
			}
		}

		public class SBYTE_ARRAY : Structure
		{
			public NativeLong numOfBytes;

			public Pointer bytePtr;

			public SBYTE_ARRAY() : base()
			{
				InitFieldOrder();
			}

			protected internal virtual void InitFieldOrder()
			{
				SetFieldOrder(new string[] { "numOfBytes", "bytePtr" });
			}

			public class ByReference : J2534_v0404.SBYTE_ARRAY, Structure.ByReference
			{
			}

			public class ByValue : J2534_v0404.SBYTE_ARRAY, Structure.ByValue
			{
			}
		}
	}
}

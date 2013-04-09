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
using Com.Sun.Jna;
using Com.Sun.Jna.Ptr;
using Org.Apache.Log4j;
using RomRaider.IO.J2534.Api;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.IO.J2534.Api
{
	/// <summary>J2534 Implementation of the Native library wrapper <b>J2534 v0404</b></summary>
	public sealed class J2534Impl : RomRaider.IO.J2534.Api.J2534
	{
		private static readonly Logger LOGGER = Logger.GetLogger(typeof(RomRaider.IO.J2534.Api.J2534Impl
			));

		private readonly NativeLong protocolID;

		private bool loopback;

		private static J2534_v0404 lib;

		/// <summary>
		/// Enum class representing the J2534-1 protocols with methods to
		/// translate the mnemonic and numerical values.
		/// </summary>
		/// <remarks>
		/// Enum class representing the J2534-1 protocols with methods to
		/// translate the mnemonic and numerical values.
		/// </remarks>
		public enum Protocol
		{
			J1850VPW,
			J1850PWM,
			ISO9141,
			ISO14230,
			CAN,
			ISO15765,
			SCI_A_ENGINE,
			SCI_A_TRANS,
			SCI_B_ENGINE,
			SCI_B_TRANS,
			UNDEFINED
		}

		private static readonly IDictionary<int, J2534Impl.Protocol> ProtocolLookup = new 
			Dictionary<int, J2534Impl.Protocol>();

		static J2534Impl()
		{
			// OP2.0: Not supported
			// OP2.0: Not supported
			// OP2.0: Not supported
			// OP2.0: Not supported
			// Returned when no match is found for get()
			foreach (J2534Impl.Protocol s in EnumSet.AllOf<J2534Impl.Protocol>())
			{
				ProtocolLookup.Put(s.value, s);
			}
		}

		/// <param name="value">- numeric value to be translated.</param>
		/// <returns>
		/// the <b>Protocol</b> mnemonic mapped to the numeric
		/// value or UNDEFINED if value is undefined.
		/// </returns>
		public static J2534Impl.Protocol GetProtocol(int value)
		{
			if (ProtocolLookup.ContainsKey(value))
			{
				return ProtocolLookup.Get(value);
			}
			else
			{
				return J2534Impl.Protocol.UNDEFINED;
			}
		}

		/// <summary>
		/// Enum class representing the J2534-1 protocol flags with methods to
		/// translate the mnemonic and numerical values.
		/// </summary>
		/// <remarks>
		/// Enum class representing the J2534-1 protocol flags with methods to
		/// translate the mnemonic and numerical values.
		/// </remarks>
		public enum Flag
		{
			ISO9141_NO_CHECKSUM,
			UNDEFINED
		}

		private static readonly IDictionary<int, J2534Impl.Flag> FlagLookup = new Dictionary
			<int, J2534Impl.Flag>();

		static J2534Impl()
		{
			// Returned when no match is found for get()
			foreach (J2534Impl.Flag s in EnumSet.AllOf<J2534Impl.Flag>())
			{
				FlagLookup.Put(s.value, s);
			}
		}

		/// <param name="value">- numeric value to be translated.</param>
		/// <returns>
		/// the <b>Flag</b> mnemonic mapped to the numeric
		/// value or UNDEFINED if value is undefined.
		/// </returns>
		public static J2534Impl.Flag GetFlag(int value)
		{
			if (FlagLookup.ContainsKey(value))
			{
				return FlagLookup.Get(value);
			}
			else
			{
				return J2534Impl.Flag.UNDEFINED;
			}
		}

		/// <summary>
		/// Enum class representing the J2534/2 extension types with methods to
		/// translate the mnemonic and numerical values.
		/// </summary>
		/// <remarks>
		/// Enum class representing the J2534/2 extension types with methods to
		/// translate the mnemonic and numerical values.
		/// </remarks>
		public enum Extension
		{
			CAN_CH1,
			J1850VPW_CH1,
			J1850PWM_CH1,
			ISO9141_CH1,
			ISO9141_CH2,
			ISO9141_CH3,
			ISO9141_K,
			ISO9141_L,
			ISO9141_INNO,
			ISO14230_CH1,
			ISO14230_CH2,
			ISO14230_K,
			ISO14230_L,
			ISO15765_CH1,
			UNDEFINED
		}

		private static readonly IDictionary<int, J2534Impl.Extension> ExtensionLookup = new 
			Dictionary<int, J2534Impl.Extension>();

		static J2534Impl()
		{
			// OP2.0: ISO9141 communications over the L line
			// OP2.0: RS-232 receive-only via the 2.5mm jack
			// OP2.0: ISO14230 communications over the L line
			// Returned when no match is found for get()
			foreach (J2534Impl.Extension s in EnumSet.AllOf<J2534Impl.Extension>())
			{
				ExtensionLookup.Put(s.value, s);
			}
		}

		/// <param name="value">- numeric value to be translated.</param>
		/// <returns>
		/// the <b>Extension</b> mnemonic mapped to the numeric
		/// value or UNDEFINED if value is undefined.
		/// </returns>
		public static J2534Impl.Extension GetExtension(int value)
		{
			if (ExtensionLookup.ContainsKey(value))
			{
				return ExtensionLookup.Get(value);
			}
			else
			{
				return J2534Impl.Extension.UNDEFINED;
			}
		}

		/// <summary>
		/// Enum class representing the J2534-1 filter types with methods to
		/// translate the mnemonic and numerical values.
		/// </summary>
		/// <remarks>
		/// Enum class representing the J2534-1 filter types with methods to
		/// translate the mnemonic and numerical values.
		/// </remarks>
		public enum Filter
		{
			PASS_FILTER,
			BLOCK_FILTER,
			FLOW_CONTROL_FILTER,
			UNDEFINED
		}

		private static readonly IDictionary<int, J2534Impl.Filter> FilterLookup = new Dictionary
			<int, J2534Impl.Filter>();

		static J2534Impl()
		{
			// Returned when no match is found for get()
			foreach (J2534Impl.Filter s in EnumSet.AllOf<J2534Impl.Filter>())
			{
				FilterLookup.Put(s.value, s);
			}
		}

		/// <param name="value">- numeric value to be translated.</param>
		/// <returns>
		/// the <b>Filter</b> mnemonic mapped to the numeric
		/// value or UNDEFINED if value is undefined.
		/// </returns>
		public static J2534Impl.Filter GetFilter(int value)
		{
			if (FilterLookup.ContainsKey(value))
			{
				return FilterLookup.Get(value);
			}
			else
			{
				return J2534Impl.Filter.UNDEFINED;
			}
		}

		/// <summary>
		/// Enum class representing the J2534-1 IOCTL types with methods to
		/// translate the mnemonic and numerical values.
		/// </summary>
		/// <remarks>
		/// Enum class representing the J2534-1 IOCTL types with methods to
		/// translate the mnemonic and numerical values.
		/// </remarks>
		public enum IOCtl
		{
			GET_CONFIG,
			SET_CONFIG,
			READ_VBATT,
			FIVE_BAUD_INIT,
			FAST_INIT,
			CLEAR_TX_BUFFER,
			CLEAR_RX_BUFFER,
			CLEAR_PERIODIC_MSGS,
			CLEAR_MSG_FILTERS,
			CLEAR_FUNCT_MSG_LOOKUP_TABLE,
			ADD_TO_FUNCT_MSG_LOOKUP_TABLE,
			DELETE_FROM_FUNCT_MSG_LOOUP_TABLE,
			READ_PROG_VOLTAGE,
			UNDEFINED
		}

		private static readonly IDictionary<int, J2534Impl.IOCtl> IOCtlLookup = new Dictionary
			<int, J2534Impl.IOCtl>();

		static J2534Impl()
		{
			// IOCTL IDs
			// J2534-1
			// Returned when no match is found for get()      
			foreach (J2534Impl.IOCtl s in EnumSet.AllOf<J2534Impl.IOCtl>())
			{
				IOCtlLookup.Put(s.value, s);
			}
		}

		/// <param name="value">- numeric value to be translated.</param>
		/// <returns>
		/// the <b>IOCtl</b> mnemonic mapped to the numeric
		/// value or UNDEFINED if value is undefined.
		/// </returns>
		public static J2534Impl.IOCtl GetIOCtl(int value)
		{
			if (IOCtlLookup.ContainsKey(value))
			{
				return IOCtlLookup.Get(value);
			}
			else
			{
				return J2534Impl.IOCtl.UNDEFINED;
			}
		}

		/// <summary>
		/// Enum class representing the J2534-1 Get/Set config parameters with methods to
		/// translate the mnemonic and numerical values.
		/// </summary>
		/// <remarks>
		/// Enum class representing the J2534-1 Get/Set config parameters with methods to
		/// translate the mnemonic and numerical values.
		/// </remarks>
		public enum Config
		{
			DATA_RATE,
			LOOPBACK,
			NODE_ADDRESS,
			NETWORK_LINE,
			P1_MIN,
			P1_MAX,
			P2_MIN,
			P2_MAX,
			P3_MIN,
			P3_MAX,
			P4_MIN,
			P4_MAX,
			W0,
			W1,
			W2,
			W3,
			W4,
			W5,
			TIDLE,
			TINIL,
			TWUP,
			PARITY,
			BIT_SAMPLE_POINT,
			SYNC_JUMP_WIDTH,
			T1_MAX,
			T2_MAX,
			T3_MAX,
			T4_MAX,
			T5_MAX,
			ISO15765_BS,
			ISO15765_STMIN,
			DATA_BITS,
			FIVE_BAUD_MOD,
			BS_TX,
			STMIN_TX,
			ISO15765_WFT_MAX,
			UNDEFINED
		}

		private static readonly IDictionary<int, J2534Impl.Config> ConfigLookup = new Dictionary
			<int, J2534Impl.Config>();

		static J2534Impl()
		{
			// OP2.0: Not yet supported
			// OP2.0: Not yet supported
			// J2534 says this may not be changed
			// J2534 says this may not be changed
			// J2534 says this may not be changed
			// J2534 says this may not be changed
			// J2534 says this may not be changed
			// OP2.0: Not yet supported
			// OP2.0: Not yet supported
			// Returned when no match is found for get()
			foreach (J2534Impl.Config s in EnumSet.AllOf<J2534Impl.Config>())
			{
				ConfigLookup.Put(s.value, s);
			}
		}

		/// <param name="value">- numeric value to be translated.</param>
		/// <returns>
		/// the <b>Config</b> mnemonic mapped to the numeric
		/// value or RESERVED if value is undefined.
		/// </returns>
		public static J2534Impl.Config GetConfig(int value)
		{
			if (ConfigLookup.ContainsKey(value))
			{
				return ConfigLookup.Get(value);
			}
			else
			{
				return J2534Impl.Config.UNDEFINED;
			}
		}

		/// <summary>
		/// Enum class representing the J2534-1 return error values with methods to
		/// translate the mnemonic and numerical values.
		/// </summary>
		/// <remarks>
		/// Enum class representing the J2534-1 return error values with methods to
		/// translate the mnemonic and numerical values.
		/// </remarks>
		public enum Status
		{
			NOERROR,
			ERR_NOT_SUPPORTED,
			ERR_INVALID_CHANNEL_ID,
			ERR_INVALID_PROTOCOL_ID,
			ERR_NULL_PARAMETER,
			ERR_INVALID_IOCTL_VALUE,
			ERR_INVALID_FLAGS,
			ERR_FAILED,
			ERR_DEVICE_NOT_CONNECTED,
			ERR_TIMEOUT,
			ERR_INVALID_MSG,
			ERR_INVALID_TIME_INTERVAL,
			ERR_EXCEEDED_LIMIT,
			ERR_INVALID_MSG_ID,
			ERR_DEVICE_IN_USE,
			ERR_INVALID_IOCTL_ID,
			ERR_BUFFER_EMPTY,
			ERR_BUFFER_FULL,
			ERR_BUFFER_OVERFLOW,
			ERR_PIN_INVALID,
			ERR_CHANNEL_IN_USE,
			ERR_MSG_PROTOCOL_ID,
			ERR_INVALID_FILTER_ID,
			ERR_NO_FLOW_CONTROL,
			ERR_NOT_UNIQUE,
			ERR_INVALID_BAUDRATE,
			ERR_INVALID_DEVICE_ID,
			ERR_INVALID_DEVICE_ID_OP2,
			ERR_OEM_VOLTAGE_TOO_LOW,
			ERR_OEM_VOLTAGE_TOO_HIGH,
			UNDEFINED
		}

		private static readonly IDictionary<int, J2534Impl.Status> StatusLookup = new Dictionary
			<int, J2534Impl.Status>();

		static J2534Impl()
		{
			// OP2.0 Tactrix specific
			// OP2.0 Tactrix specific
			// OP2.0 Tactrix specific
			// Returned when no match is found for get()
			foreach (J2534Impl.Status s in EnumSet.AllOf<J2534Impl.Status>())
			{
				StatusLookup.Put(s.value, s);
			}
		}

		/// <param name="value">- numeric value to be translated.</param>
		/// <returns>
		/// the <b>Status</b> mnemonic mapped to the numeric
		/// value or UNDEFINED if value is undefined.
		/// </returns>
		public static J2534Impl.Status GetStatus(int value)
		{
			if (StatusLookup.ContainsKey(value))
			{
				return StatusLookup.Get(value);
			}
			else
			{
				return J2534Impl.Status.UNDEFINED;
			}
		}

		/// <summary>
		/// Enum class representing the J2534-1 RxStatus values with methods to
		/// translate the mnemonic and numerical values.
		/// </summary>
		/// <remarks>
		/// Enum class representing the J2534-1 RxStatus values with methods to
		/// translate the mnemonic and numerical values.
		/// </remarks>
		public enum RxStatus
		{
			RX_INDICATION,
			TX_LOOPBACK,
			START_OF_MESSAGE,
			RX_BREAK,
			TX_INDICATION,
			TX_DONE_LOOPBACK,
			ISO15765_PADDING_ERROR,
			ISO15765_ADDR_TYPE,
			CAN_29BIT_ID,
			UNDEFINED
		}

		private static readonly IDictionary<int, J2534Impl.RxStatus> RxStatusLookup = new 
			Dictionary<int, J2534Impl.RxStatus>();

		static J2534Impl()
		{
			// Returned when no match is found for get()
			foreach (J2534Impl.RxStatus s in EnumSet.AllOf<J2534Impl.RxStatus>())
			{
				RxStatusLookup.Put(s.value, s);
			}
		}

		/// <param name="value">- numeric value to be translated.</param>
		/// <returns>
		/// the <b>RxStatus</b> mnemonic mapped to the numeric
		/// value or UNDEFINED if value is undefined.
		/// </returns>
		public static J2534Impl.RxStatus GetRxStatus(int value)
		{
			if (RxStatusLookup.ContainsKey(value))
			{
				return RxStatusLookup.Get(value);
			}
			else
			{
				return J2534Impl.RxStatus.UNDEFINED;
			}
		}

		/// <summary>
		/// Enum class representing the J2534-1 TxFlags values with methods to
		/// translate the mnemonic and numerical values.
		/// </summary>
		/// <remarks>
		/// Enum class representing the J2534-1 TxFlags values with methods to
		/// translate the mnemonic and numerical values.
		/// </remarks>
		public enum TxFlags
		{
			NO_FLAGS,
			ISO15765_FRAME_PAD,
			ISO15765_ADDR_TYPE,
			CAN_29BIT_ID,
			WAIT_P3_MIN_ONLY,
			SCI_MODE,
			SCI_TX_VOLTAGE,
			UNDEFINED
		}

		private static readonly IDictionary<int, J2534Impl.TxFlags> TxFlagsLookup = new Dictionary
			<int, J2534Impl.TxFlags>();

		static J2534Impl()
		{
			// Returned when no match is found for get()
			foreach (J2534Impl.TxFlags s in EnumSet.AllOf<J2534Impl.TxFlags>())
			{
				TxFlagsLookup.Put(s.value, s);
			}
		}

		/// <param name="value">- numeric value to be translated.</param>
		/// <returns>
		/// the <b>TxFlags</b> mnemonic mapped to the numeric
		/// value or UNDEFINED if value is undefined.
		/// </returns>
		public static J2534Impl.TxFlags GetTxFlags(int value)
		{
			if (TxFlagsLookup.ContainsKey(value))
			{
				return TxFlagsLookup.Get(value);
			}
			else
			{
				return J2534Impl.TxFlags.UNDEFINED;
			}
		}

		/// <summary>Constructor declaration</summary>
		/// <param name="protocolID">- numeric ID specified by J2534-1</param>
		/// <exception>
		/// J2534Exception
		/// on various non-zero return status
		/// </exception>
		[System.ObsoleteAttribute]
		public J2534Impl(int protocolID)
		{
			this.protocolID = new NativeLong(protocolID);
		}

		/// <summary>Constructor declaration</summary>
		/// <param name="protocol">- <b>Protocol</b> enum specified by J2534-1</param>
		/// <param name="library">- native library of the J2534 device</param>
		/// <exception>
		/// J2534Exception
		/// on various non-zero return status
		/// </exception>
		public J2534Impl(J2534Impl.Protocol protocol, string library)
		{
			this.protocolID = new NativeLong(protocol.value);
			lib = new J2534_v0404(library);
		}

		/// <summary>Establish a connection and initialize the PassThru device.</summary>
		/// <remarks>Establish a connection and initialize the PassThru device.</remarks>
		/// <returns>DeviceID of PassThru device</returns>
		public int Open()
		{
			NativeLongByReference pDeviceID = new NativeLongByReference();
			NativeLong ret = lib.PassThruOpen(null, pDeviceID);
			if (ret != J2534Impl.Status.NOERROR.value)
			{
				HandleError("PassThruOpen", ret);
			}
			return pDeviceID.GetValue();
		}

		/// <summary>
		/// Retrieve the PassThru device firmware version,
		/// DLL version, and the J2534 implementation version.
		/// </summary>
		/// <remarks>
		/// Retrieve the PassThru device firmware version,
		/// DLL version, and the J2534 implementation version.
		/// </remarks>
		/// <param name="deviceId">- of PassThru device</param>
		/// <returns>an instance of <b>Version</b></returns>
		/// <seealso cref="Version">Version</seealso>
		public Version ReadVersion(int deviceId)
		{
			ByteBuffer pFirmwareVersion = ByteBuffer.Allocate(80);
			ByteBuffer pDllVersion = ByteBuffer.Allocate(80);
			ByteBuffer pApiVersion = ByteBuffer.Allocate(80);
			NativeLong ret = lib.PassThruReadVersion(new NativeLong(deviceId), pFirmwareVersion
				, pDllVersion, pApiVersion);
			if (ret != J2534Impl.Status.NOERROR.value)
			{
				HandleError("PassThruReadVersion", ret);
			}
			return new Version(Native.ToString(((byte[])pFirmwareVersion.Array())), Native.ToString
				(((byte[])pDllVersion.Array())), Native.ToString(((byte[])pApiVersion.Array())));
		}

		/// <summary>
		/// Establish a logical connection with a protocol channel of the specified
		/// device.
		/// </summary>
		/// <remarks>
		/// Establish a logical connection with a protocol channel of the specified
		/// device.
		/// </remarks>
		/// <param name="deviceId">- of PassThru device</param>
		/// <param name="flags">- protocol specific options</param>
		/// <param name="baud">- vehicle network communication rate</param>
		/// <returns>a handle to the open communications channel</returns>
		public int Connect(int deviceId, int flags, int baud)
		{
			NativeLongByReference pChannelID = new NativeLongByReference();
			NativeLong ret = lib.PassThruConnect(new NativeLong(deviceId), protocolID, new NativeLong
				(flags), new NativeLong(baud), pChannelID);
			if (ret != J2534Impl.Status.NOERROR.value)
			{
				HandleError("PassThruConnect", ret);
			}
			return pChannelID.GetValue();
		}

		/// <summary>Configures various PassThru parameters.</summary>
		/// <remarks>Configures various PassThru parameters.</remarks>
		/// <param name="channelId">- handle to the open communications channel</param>
		/// <param name="items">
		/// - values of multiple parameters can be set
		/// in an array of ConfigItem
		/// </param>
		public void SetConfig(int channelId, params ConfigItem[] items)
		{
			if (items.Length == 0)
			{
				return;
			}
			J2534_v0404.SCONFIG[] sConfigs = SConfigs(items);
			J2534_v0404.SCONFIG_LIST list = SConfigList(sConfigs);
			NativeLong ioctlID = new NativeLong();
			ioctlID.SetValue(J2534Impl.IOCtl.SET_CONFIG.value);
			NativeLong ret = lib.PassThruIoctl(new NativeLong(channelId), ioctlID, list.GetPointer
				(), null);
			if (ret != J2534Impl.Status.NOERROR.value)
			{
				HandleError("PassThruIoctl (SET_CONFIG)", ret);
			}
		}

		/// <summary>Retrieve various PassThru configuration parameters.</summary>
		/// <remarks>Retrieve various PassThru configuration parameters.</remarks>
		/// <param name="channelId">- handle to the open communications channel</param>
		/// <param name="parameters">
		/// - values of multiple parameters can be retrieved
		/// by setting an array of integer parameter IDs
		/// </param>
		/// <returns>an array of <b>ConfigItem</b></returns>
		/// <seealso cref="ConfigItem">ConfigItem</seealso>
		public ConfigItem[] GetConfig(int channelId, params int[] parameters)
		{
			if (parameters.Length == 0)
			{
				return new ConfigItem[0];
			}
			J2534_v0404.SCONFIG[] sConfigs = SConfigs(parameters);
			J2534_v0404.SCONFIG_LIST input = SConfigList(sConfigs);
			NativeLong ioctlID = new NativeLong();
			ioctlID.SetValue(J2534Impl.IOCtl.GET_CONFIG.value);
			NativeLong ret = lib.PassThruIoctl(new NativeLong(channelId), ioctlID, input.GetPointer
				(), null);
			if (ret != J2534Impl.Status.NOERROR.value)
			{
				HandleError("PassThruIoctl (GET_CONFIG)", ret);
			}
			return ConfigItems(input);
		}

		/// <summary>
		/// Setup network protocol filter(s) to selectively restrict or limit
		/// messages received.
		/// </summary>
		/// <remarks>
		/// Setup network protocol filter(s) to selectively restrict or limit
		/// messages received.  Then purge the PassThru device's receive buffer.
		/// </remarks>
		/// <param name="channelId">- handle to the open communications channel</param>
		/// <param name="mask">
		/// - used to isolate the receive message
		/// header section(s) of interest to pass
		/// </param>
		/// <param name="pattern">- a message pattern to compare with the receive messages</param>
		/// <returns>the message filter ID to be used to later stop the filter</returns>
		public int StartPassMsgFilter(int channelId, byte mask, byte pattern)
		{
			J2534_v0404.PASSTHRU_MSG maskMsg = PassThruMessage(mask);
			J2534_v0404.PASSTHRU_MSG patternMsg = PassThruMessage(pattern);
			int filterType = J2534Impl.Filter.PASS_FILTER.value;
			int rc = SetMsgFilter(channelId, filterType, maskMsg, patternMsg, null);
			return rc;
		}

		/// <summary>
		/// Setup network protocol filter(s) to selectively restrict or limit
		/// messages received.
		/// </summary>
		/// <remarks>
		/// Setup network protocol filter(s) to selectively restrict or limit
		/// messages received.  Then purge the PassThru device's receive buffer.
		/// </remarks>
		/// <param name="channelId">- handle to the open communications channel</param>
		/// <param name="mask">
		/// - used to isolate the receive message
		/// header section(s) of interest to pass
		/// </param>
		/// <param name="pattern">- a message pattern to compare with the receive messages</param>
		/// <returns>the message filter ID to be used to later stop the filter</returns>
		public int StartPassMsgFilter(int channelId, byte[] mask, byte[] pattern)
		{
			J2534_v0404.PASSTHRU_MSG maskMsg = PassThruMessage(mask);
			J2534_v0404.PASSTHRU_MSG patternMsg = PassThruMessage(pattern);
			int filterType = J2534Impl.Filter.PASS_FILTER.value;
			int rc = SetMsgFilter(channelId, filterType, maskMsg, patternMsg, null);
			return rc;
		}

		/// <summary>
		/// Setup network protocol filter(s) to selectively restrict or limit
		/// messages received.
		/// </summary>
		/// <remarks>
		/// Setup network protocol filter(s) to selectively restrict or limit
		/// messages received.  Then purge the PassThru device's receive buffer.
		/// </remarks>
		/// <param name="channelId">- handle to the open communications channel</param>
		/// <param name="mask">
		/// - used to isolate the receive message
		/// header section(s) of interest to block
		/// </param>
		/// <param name="pattern">- a message pattern to compare with the receive messages</param>
		/// <returns>the message filter ID to be used to later stop the filter</returns>
		public int StartBlockMsgFilter(int channelId, byte[] mask, byte[] pattern)
		{
			J2534_v0404.PASSTHRU_MSG maskMsg = PassThruMessage(mask);
			J2534_v0404.PASSTHRU_MSG patternMsg = PassThruMessage(pattern);
			int filterType = J2534Impl.Filter.BLOCK_FILTER.value;
			int rc = SetMsgFilter(channelId, filterType, maskMsg, patternMsg, null);
			return rc;
		}

		/// <summary>
		/// Setup network protocol filter(s) to selectively restrict or limit
		/// messages received.
		/// </summary>
		/// <remarks>
		/// Setup network protocol filter(s) to selectively restrict or limit
		/// messages received.  Then purge the PassThru device's receive buffer.
		/// </remarks>
		/// <param name="channelId">- handle to the open communications channel</param>
		/// <param name="mask">
		/// - used to isolate the receive message
		/// header section(s) of interest to pass
		/// </param>
		/// <param name="pattern">- a message pattern to compare with the receive messages</param>
		/// <returns>the message filter ID to be used to later stop the filter</returns>
		public int StartFlowCntrlFilter(int channelId, byte[] mask, byte[] pattern, byte[]
			 flowCntrl, J2534Impl.TxFlags flag)
		{
			J2534_v0404.PASSTHRU_MSG maskMsg = PassThruMessage(flag, mask);
			J2534_v0404.PASSTHRU_MSG patternMsg = PassThruMessage(flag, pattern);
			J2534_v0404.PASSTHRU_MSG flowCntrlMsg = PassThruMessage(flag, flowCntrl);
			int filterType = J2534Impl.Filter.FLOW_CONTROL_FILTER.value;
			int rc = SetMsgFilter(channelId, filterType, maskMsg, patternMsg, flowCntrlMsg);
			return rc;
		}

		/// <summary>This function performs an ISO14230 fast initialization sequence.</summary>
		/// <remarks>This function performs an ISO14230 fast initialization sequence.</remarks>
		/// <param name="channelId">- handle to the open communications channel</param>
		/// <param name="input">- start message to be transmitted to the vehicle network</param>
		/// <returns>response  - response upon a successful initialization</returns>
		public byte[] FastInit(int channelId, byte[] input)
		{
			J2534_v0404.PASSTHRU_MSG inMsg = PassThruMessage(input);
			J2534_v0404.PASSTHRU_MSG outMsg = PassThruMessage();
			LOGGER.Trace("Ioctl inMsg: " + ToString(inMsg));
			NativeLong ret = lib.PassThruIoctl(new NativeLong(channelId), new NativeLong(J2534Impl.IOCtl
				.FAST_INIT.value), inMsg.GetPointer(), outMsg.GetPointer());
			if (ret != J2534Impl.Status.NOERROR.value)
			{
				HandleError("PassThruIoctl", ret);
			}
			outMsg.Read();
			LOGGER.Trace("Ioctl outMsg: " + ToString(outMsg));
			byte[] response = new byte[outMsg.dataSize];
			System.Array.Copy(outMsg.data, 0, response, 0, outMsg.dataSize);
			return response;
		}

		/// <summary>This function reads the battery voltage on pin 16 of the J2534 interface.
		/// 	</summary>
		/// <remarks>This function reads the battery voltage on pin 16 of the J2534 interface.
		/// 	</remarks>
		/// <param name="deviceId">- handle to the PassThru device</param>
		/// <returns>battery voltage in VDC</returns>
		public double GetVbattery(int deviceId)
		{
			NativeLongByReference vBatt = new NativeLongByReference();
			NativeLong ret = lib.PassThruIoctl(new NativeLong(deviceId), new NativeLong(J2534Impl.IOCtl
				.READ_VBATT.value), null, vBatt.GetPointer());
			if (ret != J2534Impl.Status.NOERROR.value)
			{
				HandleError("PassThruIoctl", ret);
			}
			LOGGER.Trace("Ioctl result: " + vBatt.GetValue());
			double response = vBatt.GetValue() / 1000;
			return response;
		}

		/// <summary>Send a message through the existing communication channel to the vehicle.
		/// 	</summary>
		/// <remarks>Send a message through the existing communication channel to the vehicle.
		/// 	</remarks>
		/// <param name="channelId">- handle to the open communications channel</param>
		/// <param name="data">- data bytes to be transmitted to the vehicle network</param>
		/// <param name="timeout">- maximum time (in milliseconds) for write completion</param>
		public void WriteMsg(int channelId, byte[] data, long timeout, J2534Impl.TxFlags 
			flag)
		{
			J2534_v0404.PASSTHRU_MSG msg = PassThruMessage(flag, data);
			LOGGER.Trace("Write Msg: " + ToString(msg));
			NativeLongByReference numMsg = new NativeLongByReference();
			numMsg.SetValue(new NativeLong(1));
			NativeLong ret = lib.PassThruWriteMsgs(new NativeLong(channelId), msg.GetPointer(
				), numMsg, new NativeLong(timeout));
			if (ret != J2534Impl.Status.NOERROR.value)
			{
				HandleError("PassThruWriteMsgs", ret);
			}
		}

		/// <summary>Retrieve a message through the existing communication channel from the vehicle.
		/// 	</summary>
		/// <remarks>Retrieve a message through the existing communication channel from the vehicle.
		/// 	</remarks>
		/// <param name="channelId">- handle to the open communications channel</param>
		/// <param name="response">- data array to be populated with the vehicle network message
		/// 	</param>
		/// <param name="timeout">- maximum time (in milliseconds) for read completion</param>
		public void ReadMsg(int channelId, byte[] response, long timeout)
		{
			int index = 0;
			long end = Runtime.CurrentTimeMillis() + timeout;
			do
			{
				J2534_v0404.PASSTHRU_MSG msg = DoReadMsg(channelId);
				LOGGER.Trace("Read Msg: " + ToString(msg));
				if (!IsResponse(msg))
				{
					continue;
				}
				System.Array.Copy(msg.data, 0, response, index, msg.dataSize);
				index += msg.dataSize;
			}
			while (Runtime.CurrentTimeMillis() <= end && index < response.Length - 1);
		}

		/// <summary>Retrieve a message through the existing communication channel from the vehicle.
		/// 	</summary>
		/// <remarks>Retrieve a message through the existing communication channel from the vehicle.
		/// 	</remarks>
		/// <param name="channelId">- handle to the open communications channel</param>
		/// <param name="maxWait">- maximum time (in milliseconds) for read completion</param>
		/// <returns>bytes read from the vehicle network</returns>
		public byte[] ReadMsg(int channelId, long maxWait)
		{
			IList<byte[]> responses = new AList<byte[]>();
			long end = Runtime.CurrentTimeMillis() + maxWait;
			do
			{
				J2534_v0404.PASSTHRU_MSG msg = DoReadMsg(channelId);
				LOGGER.Trace("Read Msg: " + ToString(msg));
				if (IsResponse(msg))
				{
					responses.AddItem(Data(msg));
				}
				ThreadUtil.Sleep(2);
			}
			while (Runtime.CurrentTimeMillis() <= end);
			return Concat(responses);
		}

		/// <summary>
		/// Retrieve the indicated number of messages through the existing communication
		/// channel from the vehicle.
		/// </summary>
		/// <remarks>
		/// Retrieve the indicated number of messages through the existing communication
		/// channel from the vehicle. If the number of messages can not be read before the
		/// timeout expires, throw an exception.
		/// </remarks>
		/// <param name="channelId">- handle to the open communications channel</param>
		/// <param name="numMsg">- number of valid messages to retrieve</param>
		/// <returns>bytes read from the vehicle network</returns>
		/// <exception cref="J2534Exception">J2534Exception</exception>
		public byte[] ReadMsg(int channelId, int numMsg, long timeout)
		{
			if (loopback)
			{
				numMsg++;
			}
			IList<byte[]> responses = new AList<byte[]>();
			long end = Runtime.CurrentTimeMillis() + timeout;
			do
			{
				if (Runtime.CurrentTimeMillis() >= end)
				{
					string errString = string.Format("readMsg error: timeout expired waiting for %d more message(s)"
						, numMsg);
					throw new J2534Exception(errString);
				}
				J2534_v0404.PASSTHRU_MSG msg = DoReadMsg(channelId);
				LOGGER.Trace("Read Msg: " + ToString(msg));
				if (IsResponse(msg))
				{
					responses.AddItem(Data(msg));
					numMsg--;
				}
				ThreadUtil.Sleep(2);
			}
			while (numMsg != 0);
			return Concat(responses);
		}

		/// <summary>Stop the previously defined message filter by filter ID.</summary>
		/// <remarks>Stop the previously defined message filter by filter ID.</remarks>
		/// <param name="channelId">- handle to the open communications channel</param>
		/// <param name="msgId">- ID of the filter to stop</param>
		public void StopMsgFilter(int channelId, int msgId)
		{
			NativeLong ret = lib.PassThruStopMsgFilter(new NativeLong(channelId), new NativeLong
				(msgId));
			if (ret != J2534Impl.Status.NOERROR.value)
			{
				HandleError("PassThruStopMsgFilter", ret);
			}
		}

		/// <summary>Clear the buffers on the communications channel.</summary>
		/// <remarks>Clear the buffers on the communications channel.</remarks>
		/// <param name="channelId">- handle to the open communications channel</param>
		public void ClearBuffers(int channelId)
		{
			NativeLong ret = lib.PassThruIoctl(new NativeLong(channelId), new NativeLong(J2534Impl.IOCtl
				.CLEAR_TX_BUFFER.value), null, null);
			if (ret != J2534Impl.Status.NOERROR.value)
			{
				HandleError("PassThruIoctl (CLEAR_TX_BUFFER)", ret);
			}
			ret = lib.PassThruIoctl(new NativeLong(channelId), new NativeLong(J2534Impl.IOCtl
				.CLEAR_RX_BUFFER.value), null, null);
			if (ret != J2534Impl.Status.NOERROR.value)
			{
				HandleError("PassThruIoctl (CLEAR_RX_BUFFER)", ret);
			}
		}

		/// <summary>Disconnect a previously opened communications channel.</summary>
		/// <remarks>Disconnect a previously opened communications channel.</remarks>
		/// <param name="channelId">- handle to the open communications channel</param>
		public void Disconnect(int channelId)
		{
			NativeLong ret = lib.PassThruDisconnect(new NativeLong(channelId));
			if (ret != J2534Impl.Status.NOERROR.value)
			{
				HandleError("PassThruDisconnect", ret);
			}
		}

		/// <summary>Close the PassThru device by ID.</summary>
		/// <remarks>Close the PassThru device by ID.</remarks>
		/// <param name="deviceId">of PassThru device</param>
		public void Close(int deviceId)
		{
			NativeLong ret = lib.PassThruClose(new NativeLong(deviceId));
			if (ret != J2534Impl.Status.NOERROR.value)
			{
				HandleError("PassThruClose", ret);
			}
		}

		private byte[] Concat(IList<byte[]> responses)
		{
			int length = 0;
			foreach (byte[] response in responses)
			{
				length += response.Length;
			}
			byte[] result = new byte[length];
			int index = 0;
			foreach (byte[] response_1 in responses)
			{
				if (response_1.Length == 0)
				{
					continue;
				}
				System.Array.Copy(response_1, 0, result, index, response_1.Length);
				index += response_1.Length;
			}
			return result;
		}

		private string ToString(J2534_v0404.PASSTHRU_MSG msg)
		{
			byte[] bytes = new byte[msg.dataSize];
			System.Array.Copy(msg.data, 0, bytes, 0, bytes.Length);
			string str = string.Format("[protocolID=%d | rxStatus=0x%02x | txFlags=0x%02x | timestamp=0x%x |"
				 + " dataSize=%d | extraDataIndex=%d | data=%s]", msg.protocolID, msg.rxStatus, 
				msg.txFlags, msg.timestamp, msg.dataSize, msg.extraDataIndex, HexUtil.AsHex(bytes
				));
			return str;
		}

		private bool IsResponse(J2534_v0404.PASSTHRU_MSG msg)
		{
			if (msg.timestamp != 0)
			{
				switch (GetRxStatus(msg.rxStatus))
				{
					case J2534Impl.RxStatus.RX_INDICATION:
					{
						return true;
					}

					case J2534Impl.RxStatus.TX_LOOPBACK:
					{
						return loopback;
					}

					case J2534Impl.RxStatus.START_OF_MESSAGE:
					{
						return false;
					}

					case J2534Impl.RxStatus.RX_BREAK:
					{
						return false;
					}

					case J2534Impl.RxStatus.TX_INDICATION:
					{
						return false;
					}

					case J2534Impl.RxStatus.TX_DONE_LOOPBACK:
					{
						return false;
					}

					case J2534Impl.RxStatus.ISO15765_PADDING_ERROR:
					{
						return false;
					}

					case J2534Impl.RxStatus.ISO15765_ADDR_TYPE:
					{
						return false;
					}

					case J2534Impl.RxStatus.CAN_29BIT_ID:
					{
						return false;
					}

					case J2534Impl.RxStatus.UNDEFINED:
					{
						return false;
					}
				}
			}
			return false;
		}

		private J2534_v0404.PASSTHRU_MSG DoReadMsg(int channelId)
		{
			J2534_v0404.PASSTHRU_MSG msg = PassThruMessage();
			NativeLongByReference pNumMsgs = new NativeLongByReference();
			pNumMsgs.SetValue(new NativeLong(1));
			NativeLong status = lib.PassThruReadMsgs(new NativeLong(channelId), msg.GetPointer
				(), pNumMsgs, new NativeLong(50));
			if (status != J2534Impl.Status.NOERROR.value && status != J2534Impl.Status.ERR_TIMEOUT
				.value && status != J2534Impl.Status.ERR_BUFFER_EMPTY.value)
			{
				HandleError("PassThruReadMsgs", status);
			}
			msg.Read();
			return msg;
		}

		private ConfigItem[] ConfigItems(J2534_v0404.SCONFIG_LIST sConfigs)
		{
			J2534_v0404.SCONFIG.ByReference[] configs = (J2534_v0404.SCONFIG.ByReference[])sConfigs
				.configPtr.ToArray(sConfigs.numOfParams);
			ConfigItem[] items = new ConfigItem[configs.Length];
			for (int i = 0; i < configs.Length; i++)
			{
				configs[i].Read();
				items[i] = new ConfigItem(configs[i].parameter, configs[i].value);
			}
			return items;
		}

		private J2534_v0404.SCONFIG[] SConfigs(params ConfigItem[] items)
		{
			J2534_v0404.SCONFIG[] sConfigs = (J2534_v0404.SCONFIG[])new J2534_v0404.SCONFIG.ByReference
				().ToArray(items.Length);
			for (int i = 0; i < items.Length; i++)
			{
				sConfigs[i].parameter = new NativeLong(items[i].parameter);
				sConfigs[i].value = new NativeLong(items[i].value);
				if (items[i].parameter == J2534Impl.Config.LOOPBACK.value)
				{
					if (items[i].value == 1)
					{
						loopback = true;
					}
				}
			}
			//        for (SCONFIG sc : sConfigs) {
			//            sc.write();
			//            System.out.printf("%s%n", sc);
			//        }
			return sConfigs;
		}

		private J2534_v0404.SCONFIG[] SConfigs(params int[] parameters)
		{
			J2534_v0404.SCONFIG[] sConfigs = (J2534_v0404.SCONFIG[])new J2534_v0404.SCONFIG.ByReference
				().ToArray(parameters.Length);
			for (int i = 0; i < parameters.Length; i++)
			{
				sConfigs[i].parameter = new NativeLong(parameters[i]);
				sConfigs[i].value = new NativeLong(-1);
			}
			return sConfigs;
		}

		private J2534_v0404.SCONFIG_LIST SConfigList(J2534_v0404.SCONFIG[] sConfigs)
		{
			J2534_v0404.SCONFIG_LIST list = new J2534_v0404.SCONFIG_LIST();
			list.numOfParams = new NativeLong(sConfigs.Length);
			list.configPtr = (J2534_v0404.SCONFIG.ByReference)sConfigs[0];
			list.Write();
			//        System.out.printf("list:%n%s%n", list);
			return list;
		}

		private J2534_v0404.PASSTHRU_MSG PassThruMessage(params byte[] data)
		{
			J2534_v0404.PASSTHRU_MSG msg = PassThruMessage();
			msg.dataSize = new NativeLong(data.Length);
			System.Array.Copy(data, 0, msg.data, 0, data.Length);
			msg.Write();
			return msg;
		}

		private J2534_v0404.PASSTHRU_MSG PassThruMessage(J2534Impl.TxFlags flag, params byte
			[] data)
		{
			J2534_v0404.PASSTHRU_MSG msg = PassThruMessage(flag);
			msg.dataSize = new NativeLong(data.Length);
			System.Array.Copy(data, 0, msg.data, 0, data.Length);
			msg.Write();
			return msg;
		}

		private J2534_v0404.PASSTHRU_MSG PassThruMessage()
		{
			J2534_v0404.PASSTHRU_MSG msg = new J2534_v0404.PASSTHRU_MSG();
			msg.txFlags = new NativeLong(0);
			msg.protocolID = protocolID;
			return msg;
		}

		private J2534_v0404.PASSTHRU_MSG PassThruMessage(J2534Impl.TxFlags flag)
		{
			J2534_v0404.PASSTHRU_MSG msg = new J2534_v0404.PASSTHRU_MSG();
			msg.txFlags = new NativeLong(flag.value);
			msg.protocolID = protocolID;
			return msg;
		}

		private byte[] Data(J2534_v0404.PASSTHRU_MSG msg)
		{
			int length = msg.dataSize;
			byte[] data = new byte[length];
			System.Array.Copy(msg.data, 0, data, 0, length);
			return data;
		}

		private int SetMsgFilter(int channelId, int filterType, J2534_v0404.PASSTHRU_MSG 
			maskMsg, J2534_v0404.PASSTHRU_MSG patternMsg, J2534_v0404.PASSTHRU_MSG flowMsg)
		{
			NativeLongByReference msgId = new NativeLongByReference();
			msgId.SetValue(new NativeLong(0));
			NativeLong ret = lib.PassThruStartMsgFilter(new NativeLong(channelId), new NativeLong
				(filterType), maskMsg.GetPointer(), patternMsg.GetPointer(), flowMsg == null ? null
				 : flowMsg.GetPointer(), msgId);
			if (ret != J2534Impl.Status.NOERROR.value)
			{
				HandleError("PassThruStartMsgFilter", ret);
			}
			ClearBuffers(channelId);
			return msgId.GetValue();
		}

		/// <summary>
		/// Retrieve the text description for the most recent non-zero error
		/// and throw an exception.
		/// </summary>
		/// <remarks>
		/// Retrieve the text description for the most recent non-zero error
		/// and throw an exception.
		/// </remarks>
		/// <param name="operation">
		/// - string containing the name of the method for which
		/// the error occurred
		/// </param>
		/// <param name="status">- the method's numeric error value</param>
		/// <exception>
		/// J2534Exception
		/// on various non-zero return status
		/// </exception>
		/// <seealso cref="J2534_v0404">J2534_v0404</seealso>
		private static void HandleError(string operation, int status)
		{
			ByteBuffer error = ByteBuffer.Allocate(255);
			lib.PassThruGetLastError(error);
			string errString = string.Format("%s error [%d:%s], %s", operation, status, GetStatus
				(status), Native.ToString(((byte[])error.Array())));
			throw new J2534Exception(errString);
		}
	}
}

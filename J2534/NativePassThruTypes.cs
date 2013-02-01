using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace NateW.J2534
{
    public enum PassThruStatus : int
    {
        NoError = 0,
        ErrorNotSupported = 1,
        ErrorInvalidChannelID = 2,
        ErrorInvalidProtocolID = 3,
        ErrorNullParameter = 4,
        ErrorInvalidIoCtlValue = 5,
        ErrorInvalidFlags = 6,
        ErrorFailed = 7,
        ErrorDeviceNotConnected = 8,
        ErrorTimeout = 9,
        ErrorInvalidMessage = 10,
        ErrorInvalidTimeInterval = 11,
        ErrorExceededLimit = 12,
        ErrorInvalidMessageID = 13,
        ErrorDeviceInUse=14,
        ErrorInvalidIoCtlID = 15,
        ErrorBufferEmpty = 16,
        ErrorBufferFull = 17,
        ErrorBufferOverflow = 18,
        ErrorPinInvalid = 19,
        ErrorChannelInUse=20,
        ErrorMessageProtocolId=21,
        ErrorInvalidFilterId=22,
        ErrorNoFlowControl=23,
        ErrorNotUnique=24,
        ErrorInvalidBaudRate=25,
        ErrorInvalidDeviceID=26,
        ErrorInvalidDeviceID_OpenPort=32,// not documented, but returned by OpenPort20
    }

    public enum PassThruProtocol
    {
        Undefined = 0,
        J1850Vpw = 1,
        J1850Pwm = 2,
        Iso9141 = 3,
        Iso14320 = 4,
        Can = 5,
        Iso15765 = 6,
        Sci_A_Engine = 7,
        Sci_A_Trans = 8,
        Sci_B_Engine = 9,
        Sci_B_Trans = 10,
    }

    public enum PassThruConnectFlags
    {
        Iso9141KLineOnly = 4096,
        CanIDBoth = 2048,
        Iso9141NoChecksum = 512,
        Can29BitID = 256,
    }

    public enum PassThruBaudRate
    {
        Rate4800 = 4800,
    }

    public enum PassThruFilterType
    {
        Invalid = 0,
        Pass = 1,
        Block = 2,
        FlowControl = 3,
    }

    public enum PassThruIOControl
    {
        Invalid = 0,
        GetConfig = 1,
        SetConfig = 2,
        ReadVBatt = 3,
        FiveBaudInit = 4,
        FastInit = 5,
        Reserved_6 = 6,
        ClearTxBuffer = 7,
        ClearRxBuffer = 8,
        ClearPeriodicMessages = 9,
        ClearMessageFilter = 10,
        ClearFunctionMessageLookupTable = 11,
        AddToFunctionMessageLookupTable = 12,
        DeleteFromFunctionMessageLookupTable = 13,
        ReadProgrammingVoltage = 14,
    }

    [Flags]
    public enum PassThruTxFlags
    {
        None = 0,
    }

    [Flags]
    public enum PassThruRxStatus
    {
        /// <summary>
        /// No RxStatus flags set
        /// </summary>
        None = 0,

        /// <summary>
        /// If set, message was sent by the car; if not set, this is a loopback message.
        /// </summary>
        TxMessage = 1,

        /// <summary>
        /// If set, this is the first frame of a message; if not set, this is second-or-later frame.
        /// </summary>
        StartOfMessage = 2,

        /// <summary>
        /// If set, break was received; if not set, no break was received.  (J2610 and J1850 VPW only)
        /// </summary>
        RxBreak = 4,

        /// <summary>
        /// If set, indicates ISO 15765 TxDone.
        /// </summary>
        TxIndication = 8,

        /// <summary>
        /// If set, indicates that an ISO 15765 frame was received with less than 8 data bytes.
        /// </summary>
        Iso15765PaddingError = 16,

        Reserved_32,

        Reserved_64,

        /// <summary>
        /// If set, indicates than an extended address immediately follows the CAN ID.
        /// </summary>
        Iso15765AddressType = 128,

        /// <summary>
        /// If set, indicates a 29-bit CAN ID; if not set, indicates an 11-bit CAN ID.
        /// </summary>
        Can29BitId = 256,
    }

    public enum SetConfigurationParameter
    {
        FinalParam = 0,
        DataRate = 1,
        Unused2 = 2,
        Loopback = 3,
        NodeAddress = 4,
        NetworkLine = 5,
        P1Min = 6,
        P1Max = 7,
        P2Min = 8,
        P2Max = 9,
        P3Min = 0x0A,
        P3Max = 0x0B,
        P4Min = 0x0C,
        P4Max = 0x0D,
        W0 = 0x19, // ???
        W1 = 0x0E,
        W2 = 0x0F,
        W3 = 0x10,
        W4 = 0x11,
        W5 = 0x12,
        TIdle = 0x13,
        TIniL = 0x14,
        TWUp = 0x15,
        Parity = 0x16,
        BitSamplePoint = 0x17,
        SyncJumpWidth = 0x18,
        T1Max = 0x1A,
        T2Max = 0x1B,
        T3Max = 0x24, // ???
        T4Max = 0x1C,
        T5Max = 0x1D,
        Iso15765BlockSize = 0x1E,
        Iso15765SeparateTimeMinimum = 0x1F,
        Iso15765BlockSizeTransmit = 0x22,
        Iso15765SeparationTimeMinimum = 0x23,
        DataBits = 0x20,
        FiveBaudMod = 0x21,
        Iso15765WaitFrameTransferMax = 0x25,
    }

    [StructLayout(LayoutKind.Sequential)]
    public class PassThruMsg
    {
        [MarshalAs(UnmanagedType.U4)]
        public PassThruProtocol ProtocolID;
        
        [MarshalAs(UnmanagedType.U4)]
        public PassThruRxStatus RxStatus;
        
        [MarshalAs(UnmanagedType.U4)]
        public PassThruTxFlags TxFlags;
        public UInt32 Timestamp;
        public UInt32 DataSize;
        public UInt32 ExtraDataIndex;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst=4128)]
        public byte[] Data;

        public PassThruMsg()
        {
            this.ProtocolID = PassThruProtocol.Undefined;
            this.RxStatus = PassThruRxStatus.None;
            this.TxFlags = PassThruTxFlags.None;
            this.Timestamp = 0;
            this.ExtraDataIndex = 0;
            this.Data = new byte[4128];
            this.DataSize = (uint)this.Data.Length;
        }

        public PassThruMsg(PassThruProtocol protocol)
        {
            this.ProtocolID = protocol;
            this.RxStatus = PassThruRxStatus.None;
            this.TxFlags = PassThruTxFlags.None;
            this.Timestamp = 0;
            this.ExtraDataIndex = 0;
            this.Data = new byte[4128];
            this.DataSize = (uint) this.Data.Length;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SetConfigurationList : IDisposable
    {
        [MarshalAs(UnmanagedType.U4)]
        private UInt32 numberOfParameters;

        [MarshalAs(UnmanagedType.U4)]
        private IntPtr configurationArrayPointer;

        [MarshalAs(UnmanagedType.U4)] // Ignored
        private SetConfiguration[] configuration;
                
        private IntPtr thisPointer;

        public IntPtr Pointer
        {
            get
            {
                return this.thisPointer;
            }
        }

        public SetConfigurationList(SetConfiguration[] array)
        {
            this.configuration = array;
            this.numberOfParameters = (UInt32) array.Length;
            this.configurationArrayPointer = Marshal.AllocCoTaskMem(8 * this.configuration.Length);
            for (int i = 0; i < this.configuration.Length; i++)
            {
                //IntPtr temp = Marshal.AllocCoTaskMem(sizeof(SetConfiguration));
                Marshal.StructureToPtr(
                    this.configuration[i],
                    (IntPtr) ((int) this.configurationArrayPointer + (i * 8)),
                    false);
                //Marshal.Copy(
                //    temp, 
                //    0, 
                //    this.configurationArrayPointer + (i * sizeof(SetConfiguration)),
                //    sizeof(SetConfiguration));
                //Marshal.FreeCoTaskMem(temp);
            }

            this.thisPointer = Marshal.AllocCoTaskMem(8);
            Marshal.StructureToPtr(this.numberOfParameters, this.thisPointer, false);
            Marshal.StructureToPtr(this.configurationArrayPointer, (IntPtr)((int)this.thisPointer + 4), false);
        }

        public void Dispose()
        {
            Marshal.FreeCoTaskMem(this.configurationArrayPointer);
            Marshal.FreeCoTaskMem(this.thisPointer);
        }
    }

    [StructLayout(LayoutKind.Sequential, Size=8)]
    public struct SetConfiguration
    {
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 Parameter;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 Value;

        public SetConfiguration(SetConfigurationParameter parameter, UInt32 value)
        {
            this.Parameter = (UInt32) parameter;
            this.Value = value;
        }
    }
}

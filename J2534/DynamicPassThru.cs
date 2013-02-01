using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace NateW.J2534
{
    public class DynamicPassThru : IPassThru
    {
        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(String dllname);

        [DllImport("kernel32.dll")]
        private static extern IntPtr FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetProcAddress(IntPtr hModule, String procname);

        private string dllPath;
        private IntPtr dllHandle;

        private PassThruOpenDelegate passThruOpen;
        private PassThruCloseDelegate passThruClose;
        private PassThruConnectDelegate passThruConnect;
        private PassThruDisconnectDelegate passThruDisconnect;
        private PassThruReadMsgDelegate passThruReadMsg;
        private PassThruReadMsgsDelegate passThruReadMsgs;
        private PassThruWriteMsgDelegate passThruWriteMsg;
        private PassThruWriteMsgsDelegate passThruWriteMsgs;
        private PassThruStartPeriodicMsgDelegate passThruStartPeriodicMsg;
        private PassThruStopPeriodicMsgDelegate passThruStopPeriodicMsg;
        private PassThruStartMsgFilterDelegate passThruStartMsgFilter;
        private PassThruStopMsgFilterDelegate passThruStopMsgFilter;
        private PassThruReadVersionDelegate passThruReadVersion;
        private PassThruGetLastErrorDelegate passThruGetLastError;
        private PassThruIoctlDelegate passThruIoctl;
        
        private DynamicPassThru(string dllPath)
        {
            this.dllPath = dllPath;
            this.dllHandle = LoadLibrary(dllPath);

            this.passThruOpen = (PassThruOpenDelegate) this.GetDelegate(typeof(PassThruOpenDelegate), "PassThruOpen");
            this.passThruClose = (PassThruCloseDelegate) this.GetDelegate(typeof(PassThruCloseDelegate), "PassThruClose");
            this.passThruConnect = (PassThruConnectDelegate) this.GetDelegate(typeof(PassThruConnectDelegate), "PassThruConnect");
            this.passThruDisconnect = (PassThruDisconnectDelegate) this.GetDelegate(typeof(PassThruDisconnectDelegate), "PassThruDisconnect");
            this.passThruReadMsg = (PassThruReadMsgDelegate) this.GetDelegate(typeof(PassThruReadMsgDelegate), "PassThruReadMsgs");
            this.passThruReadMsgs = (PassThruReadMsgsDelegate) this.GetDelegate(typeof(PassThruReadMsgsDelegate), "PassThruReadMsgs");
            this.passThruWriteMsg = (PassThruWriteMsgDelegate) this.GetDelegate(typeof(PassThruWriteMsgDelegate), "PassThruWriteMsgs");
            this.passThruWriteMsgs = (PassThruWriteMsgsDelegate) this.GetDelegate(typeof(PassThruWriteMsgsDelegate), "PassThruWriteMsgs");
            this.passThruStartPeriodicMsg = (PassThruStartPeriodicMsgDelegate) this.GetDelegate(typeof(PassThruStartPeriodicMsgDelegate), "PassThruStartPeriodicMsg");
            this.passThruStopPeriodicMsg = (PassThruStopPeriodicMsgDelegate) this.GetDelegate(typeof(PassThruStopPeriodicMsgDelegate), "PassThruStopPeriodicMsg");
            this.passThruStartMsgFilter = (PassThruStartMsgFilterDelegate) this.GetDelegate(typeof(PassThruStartMsgFilterDelegate), "PassThruStartMsgFilter");
            this.passThruStopMsgFilter = (PassThruStopMsgFilterDelegate) this.GetDelegate(typeof(PassThruStopMsgFilterDelegate), "PassThruStopMsgFilter");
            this.passThruReadVersion = (PassThruReadVersionDelegate) this.GetDelegate(typeof(PassThruReadVersionDelegate), "PassThruReadVersion");
            this.passThruGetLastError = (PassThruGetLastErrorDelegate) this.GetDelegate(typeof(PassThruGetLastErrorDelegate), "PassThruGetLastError");
            this.passThruIoctl = (PassThruIoctlDelegate)this.GetDelegate(typeof(PassThruIoctlDelegate), "PassThruIoctl");
        }

        ~DynamicPassThru()
        {
            this.Dispose(false);
        }

        private Delegate GetDelegate(Type type, string name)
        {
            IntPtr procAddress = GetProcAddress(this.dllHandle, name);
            return Marshal.GetDelegateForFunctionPointer(procAddress, type);
        }

        public static DynamicPassThru GetInstance(string dllPath)
        {
            return new DynamicPassThru(dllPath);
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        #region Public methods

        public PassThruStatus PassThruOpen(string pName, out uint pDeviceId)
        {
            return (PassThruStatus) this.passThruOpen(pName, out pDeviceId);
        }

        public PassThruStatus PassThruClose(uint DeviceId)
        {
            return (PassThruStatus) this.passThruClose(DeviceId);
        }

        public PassThruStatus PassThruConnect(uint DeviceId, PassThruProtocol ProtocolId, PassThruConnectFlags Flags, PassThruBaudRate BaudRate, out uint pChannelID)
        {
            return (PassThruStatus) this.passThruConnect(DeviceId, (uint) ProtocolId, (uint) Flags, (uint) BaudRate, out pChannelID);
        }

        public PassThruStatus PassThruDisconnect(uint ChannelID)
        {
            return (PassThruStatus) this.passThruDisconnect(ChannelID);
        }

        public PassThruStatus PassThruReadMsg(uint ChannelID, PassThruMsg pMsg, uint Timeout)
        {
            UInt32 numMsgs = 1;
            return (PassThruStatus) this.passThruReadMsg(ChannelID, pMsg, ref numMsgs, Timeout);
        }

        public PassThruStatus PassThruReadMsgs(uint ChannelID, PassThruMsg[] pMsgs, ref uint pNumMsgs, uint Timeout)
        {
            return (PassThruStatus) this.passThruReadMsgs(ChannelID, pMsgs, ref pNumMsgs, Timeout);
        }

        public PassThruStatus PassThruWriteMsg(uint ChannelID, PassThruMsg pMsg, uint Timeout)
        {
            UInt32 numMsgs = 1;
            return (PassThruStatus) this.passThruWriteMsg(ChannelID, pMsg, ref numMsgs, Timeout);
        }

        public PassThruStatus PassThruWriteMsgs(uint ChannelID, PassThruMsg[] pMsg, ref uint pNumMsgs, uint Timeout)
        {
            return (PassThruStatus) this.passThruWriteMsgs(ChannelID, pMsg, ref pNumMsgs, Timeout);
        }

        public PassThruStatus PassThruStartPeriodicMsg(uint ChannelID, PassThruMsg pMsg, out uint pMsgId, uint TimeInterval)
        {
            return (PassThruStatus)  this.passThruStartPeriodicMsg(ChannelID, pMsg, out pMsgId, TimeInterval);
        }

        public PassThruStatus PassThruStopPeriodicMsg(uint ChannelID, uint MessageID)
        {
            return (PassThruStatus) this.passThruStopPeriodicMsg(ChannelID, MessageID);
        }

        public PassThruStatus PassThruStartMsgFilter(uint ChannelID, uint FilterType, PassThruMsg pMaskMsg, PassThruMsg pPatternMsg, PassThruMsg pFlowControlMsg, out uint pFilterID)
        {
            return (PassThruStatus) this.passThruStartMsgFilter(ChannelID, FilterType, pMaskMsg, pPatternMsg, pFlowControlMsg, out pFilterID);
        }

        public PassThruStatus PassThruStopMsgFilter(uint ChannelID, uint FilterID)
        {
            return (PassThruStatus) this.passThruStopMsgFilter(ChannelID, FilterID);
        }

        public PassThruStatus PassThruReadVersion(uint DeviceID, out string pFirmwareVersion, out string pDllVersion, out string pApiVersion)
        {
            return (PassThruStatus) this.passThruReadVersion(DeviceID, out pFirmwareVersion, out pDllVersion, out pApiVersion);
        }

        public PassThruStatus PassThruGetLastError(out string errorDescription)
        {
            byte[] message = new byte[80];
            PassThruStatus status = (PassThruStatus)this.passThruGetLastError(message);
            errorDescription = System.Text.Encoding.ASCII.GetString(message);
            return status;
        }

        public PassThruStatus PassThruIoctl(uint ChannelID, PassThruIOControl IoctlID, IntPtr pInput, IntPtr pOutput)
        {
            return (PassThruStatus) this.passThruIoctl(ChannelID, IoctlID, pInput, pOutput);
        }

        #endregion

        protected virtual void Dispose(bool disposing)
        {
            if (this.dllHandle != IntPtr.Zero)
            {
                FreeLibrary(dllHandle);
                this.dllHandle = IntPtr.Zero;
            }
        }

        #region Private delegate types

        /// <summary>
        /// Open a J2534 device
        /// </summary>
        /// <param name="pName">reserved for future use, must be null</param>
        /// <param name="pDeviceId">will be set to the id of the opened device</param>
        /// <returns>See Status enumeration</returns>
        private delegate Int32 PassThruOpenDelegate(string pName, out UInt32 pDeviceId);

        /// <summary>
        /// Close a J2534 device
        /// </summary>
        /// <param name="DeviceId">id of the device to close</param>
        /// <returns>See Status enumeration</returns>
        private delegate Int32 PassThruCloseDelegate(UInt32 DeviceId);

        /// <summary>
        /// Opens a communication channel
        /// </summary>
        /// <param name="DeviceId">Returned from PassThruOpen</param>
        /// <param name="ProtocolId">See Protocol enumeration</param>
        /// <param name="Flags">See ConnectFlags enumeration</param>
        /// <param name="BaudRate">See BaudRate enumeration</param>
        /// <param name="pChannelID">Will be set to the id of the opened channel</param>
        /// <returns>See Status enumeration</returns>
        private delegate Int32 PassThruConnectDelegate(
            UInt32 DeviceId,
            UInt32 ProtocolId,
            UInt32 Flags,
            UInt32 BaudRate,
            out UInt32 pChannelID);

        /// <summary>
        /// Closes a communication channel
        /// </summary>
        /// <param name="ChannelID">Channel ID provided by PassThruConnect</param>
        /// <returns>See Status enumeration</returns>
        private delegate Int32 PassThruDisconnectDelegate(UInt32 ChannelID);

        /// <summary>
        /// Read messages and indications from the receive buffer.
        /// </summary>
        /// <param name="ChannelID">Channel identifier returned from PassThruConnect</param>
        /// <param name="pMsg">Pointer to message structures</param>
        /// <param name="pNumMsgs">Indicates how many message structures have been provided; on return, indicates how many messages were received.</param>
        /// <param name="Timeout">Read timeout, in milliseconds.</param>
        /// <returns>See Status enumeration</returns>
        private delegate Int32 PassThruReadMsgDelegate(
            UInt32 ChannelID,
            [In][Out][MarshalAs(UnmanagedType.LPStruct)]
            PassThruMsg pMsg,
            ref UInt32 pNumMsgs,
            UInt32 Timeout);

        /// <summary>
        /// Read messages and indications from the receive buffer.
        /// </summary>
        /// <param name="ChannelID">Channel identifier returned from PassThruConnect</param>
        /// <param name="pMsg">Pointer to message structures</param>
        /// <param name="pNumMsgs">Indicates how many message structures have been provided; on return, indicates how many messages were received.</param>
        /// <param name="Timeout">Read timeout, in milliseconds.</param>
        /// <returns>See Status enumeration</returns>
        private delegate Int32 PassThruReadMsgsDelegate(
            UInt32 ChannelID,
            [Out]
            PassThruMsg[] pMsgs,
            ref UInt32 pNumMsgs,
            UInt32 Timeout);

        /// <summary>
        /// Send messages to the ECU.
        /// </summary>
        /// <remarks>DOES NOT MARSHAL CORRECTLY.</remarks>
        /// <param name="ChannelID">Channel identifier returned from PassThruConnect</param>
        /// <param name="pMsg">Pointer to message structures</param>
        /// <param name="pNumMsgs">Pointer to number of messages to send.  On return, will indicate how many messages were sent before timeout expired (if timeout is nonzero) or how many messages were enqueued (if timeout is zero).</param>
        /// <param name="Timeout">Write timeout, in milliseconds.</param>
        /// <returns>See Status enumeration</returns>
        private delegate Int32 PassThruWriteMsgDelegate(
            UInt32 ChannelID,
            PassThruMsg pMsg,
            ref UInt32 pNumMsgs,
            UInt32 Timeout);

        /// <summary>
        /// Send messages to the ECU.
        /// </summary>
        /// <remarks>DOES NOT MARSHAL CORRECTLY.</remarks>
        /// <param name="ChannelID">Channel identifier returned from PassThruConnect</param>
        /// <param name="pMsg">Pointer to message structures</param>
        /// <param name="pNumMsgs">Pointer to number of messages to send.  On return, will indicate how many messages were sent before timeout expired (if timeout is nonzero) or how many messages were enqueued (if timeout is zero).</param>
        /// <param name="Timeout">Write timeout, in milliseconds.</param>
        /// <returns>See Status enumeration</returns>
        private delegate Int32 PassThruWriteMsgsDelegate(
            UInt32 ChannelID,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)]
            PassThruMsg[] pMsg,
            ref UInt32 pNumMsgs,
            UInt32 Timeout);

        /// <summary>
        /// Immediately queue the given message, and re-send at the specified interval.
        /// </summary>
        /// <param name="ChannelID">Channel identifier returned from PassThruConnect</param>
        /// <param name="pMsg">Pointer to a single message structure</param>
        /// <param name="pMsgId">Pointer to periodic-message identifier assigned by the PassThru DLL</param>
        /// <param name="TimeInterval">Interval between the start of successive transmissions, in milliseconds.  Valid range is 5-65535.</param>
        /// <returns>See Status enumeration</returns>
        private delegate Int32 PassThruStartPeriodicMsgDelegate(
            UInt32 ChannelID,
            PassThruMsg pMsg,
            out UInt32 pMsgId,
            UInt32 TimeInterval);

        /// <summary>
        /// Stop the given periodic message.
        /// </summary>
        /// <param name="ChannelID">Channel identifier returned from PassThruConnect</param>
        /// <param name="MessageID">Periodic-message identifier returned by PassThruStartPeriodicMsg</param>
        /// <returns>See Status enumeration</returns>
        private delegate Int32 PassThruStopPeriodicMsgDelegate(
            UInt32 ChannelID,
            UInt32 MessageID);

        /// <summary>
        /// Apply a filter to incoming messages.
        /// </summary>
        /// <param name="ChannelID">Channel identifier returned from PassThruConnect</param>
        /// <param name="FilterType">See FilterType enumeration</param>
        /// <param name="pMaskMsg">This message will be bitwise-ANDed with incoming messages to mask irrelevant bits.</param>
        /// <param name="pPatternMsg">This message will be compared with the masked messsage; if equal the FilterType operation will be applied.</param>
        /// <param name="pFlowControlMsg">Must be null for Pass or Block filter types.  For FlowControl filters, points to the CAN ID used for segmented sends and receives.</param>
        /// <param name="pFilterID">Upon return, will be set with an ID for the newly applied filter.</param>
        /// <returns>See Status enumeration</returns>
        private delegate Int32 PassThruStartMsgFilterDelegate(
            UInt32 ChannelID,
            UInt32 FilterType,
            PassThruMsg pMaskMsg,
            PassThruMsg pPatternMsg,
            PassThruMsg pFlowControlMsg,
            out UInt32 pFilterID);

        /// <summary>
        /// Removes the given message filter.
        /// </summary>
        /// <param name="ChannelID">Channel identifier returned from PassThruConnect</param>
        /// <param name="FilterID">Filter identifier returned from PassThruStartMsgFilter</param>
        /// <returns>See Status enumeration</returns>
        private delegate Int32 PassThruStopMsgFilterDelegate(
            UInt32 ChannelID,
            UInt32 FilterID);

        /// <summary>
        /// Retreive version strings from the PassThru DLL.
        /// </summary>
        /// <param name="DeviceID">Device identifier returned from PassThruOpen</param>
        /// <param name="pFirmwareVersion">Firmware version string.  Allocate at least 80 characters.</param>
        /// <param name="pDllVersion">DLL version string.  Allocate at least 80 characters.</param>
        /// <param name="pApiVersion">API version string.  Allocate at least 80 characters.</param>
        /// <returns></returns>
        private delegate Int32 PassThruReadVersionDelegate(
            UInt32 DeviceID,
            [MarshalAs(UnmanagedType.LPStr, SizeConst = 80)]
            out string pFirmwareVersion,
            [MarshalAs(UnmanagedType.LPStr, SizeConst = 80)]
            out string pDllVersion,
            [MarshalAs(UnmanagedType.LPStr, SizeConst = 80)]
            out string pApiVersion);

        /// <summary>
        /// Retrieve error information regarding a previous PassThru API call.
        /// </summary>
        /// <param name="pErrorDescription">Pointer to error description buffer.  Allocate at least 80 characters.</param>
        /// <returns>See Status enumeration</returns>
        private delegate Int32 PassThruGetLastErrorDelegate(
            [Out][MarshalAs(UnmanagedType.LPArray, SizeConst = 80)]
            byte[] pErrorDescription);

        /// <summary>
        /// IO Control
        /// </summary>
        /// <param name="ChannelID">Channel identifier returned from PassThruConnect</param>
        /// <param name="IoctlID">See IOCtl enumeration</param>
        /// <param name="pInput">Pointer to input structure</param>
        /// <param name="pOutput">Pointer to output structure</param>
        /// <returns>See Status enumeration</returns>
        private delegate Int32 PassThruIoctlDelegate(
            UInt32 ChannelID,
            [MarshalAs(UnmanagedType.U4)]
            PassThruIOControl IoctlID,
            IntPtr pInput,
            IntPtr pOutput);

        #endregion

    }
}

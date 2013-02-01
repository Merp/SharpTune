using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace NateW.J2534
{
    public class PassThruDevice
    {
        /// <summary>
        /// Underlying implementation
        /// </summary>
        private IPassThru implementation;

        /// <summary>
        /// J2534 ID for this device
        /// </summary>
        private UInt32 deviceId;

        /// <summary>
        /// Private constructor to force consumers to use GetInstance
        /// </summary>
        private PassThruDevice(IPassThru implementation)
        {
            this.implementation = implementation;
        }

        /// <summary>
        /// Creates a PassThruDevice
        /// </summary>
        public static PassThruDevice GetInstance(IPassThru implementation)
        {
            return new PassThruDevice(implementation);
        }

        /// <summary>
        /// Open a J2534 device
        /// </summary>
        public void Open()
        {
            // Name is reserved, must be null.
            string name = null;

            PassThruStatus status = this.implementation.PassThruOpen(name, out this.deviceId);
            PassThruUtility.ThrowIfError(status);
        }

        /// <summary>
        /// Close a J2534 device
        /// </summary>
        public void Close()
        {
            PassThruStatus status = this.implementation.PassThruClose(this.deviceId);
            PassThruUtility.ThrowIfError(status);
        }

        /// <summary>
        /// Opens a communication channel
        /// </summary>
        /// <param name="protocolId">See Protocol enumeration</param>
        /// <param name="Flags">See ConnectFlags enumeration</param>
        /// <param name="BaudRate">See BaudRate enumeration</param>
        /// <param name="pChannelID">Will be set to the id of the opened channel</param>
        /// <returns>See Status enumeration</returns>
        public PassThruChannel OpenChannel(
            PassThruProtocol protocolId,
            PassThruConnectFlags flags,
            PassThruBaudRate baudRate)
        {
            UInt32 channelId;
            PassThruStatus status = this.implementation.PassThruConnect(
                this.deviceId,
                protocolId,
                flags, 
                baudRate, 
                out channelId);
            PassThruUtility.ThrowIfError(status);
            return PassThruChannel.GetInstance(this.implementation, channelId);
        }
        
        /// <summary>
        /// Retreive version strings from the PassThru DLL.
        /// </summary>
        /// <param name="DeviceID">Device identifier returned from PassThruOpen</param>
        /// <param name="firmwareVersion">Firmware version string.  Allocate at least 80 characters.</param>
        /// <param name="dllVersion">DLL version string.  Allocate at least 80 characters.</param>
        /// <param name="apiVersion">API version string.  Allocate at least 80 characters.</param>
        public void ReadVersion(
            out string firmwareVersion,
            out string dllVersion,
            out string apiVersion)
        {
            PassThruStatus status = this.implementation.PassThruReadVersion(
                this.deviceId, 
                out firmwareVersion, 
                out dllVersion, 
                out apiVersion);
            PassThruUtility.ThrowIfError(status);
        }
    }
}

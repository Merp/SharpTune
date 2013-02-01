using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace NateW.J2534
{
    public class PassThruChannel
    {
        /// <summary>
        /// Underlying implementation
        /// </summary>
        private IPassThru implementation;

        /// <summary>
        /// J2534 channel ID
        /// </summary>
        private UInt32 channelId;

        /// <summary>
        /// Private constructor to force consumers to use the factory method
        /// </summary>
        private PassThruChannel(IPassThru implementation, UInt32 channelId)
        {
            this.implementation = implementation;
            this.channelId = channelId;
        }

        /// <summary>
        /// Factory, should only be called from PassThruDevice.Connect
        /// </summary>
        internal static PassThruChannel GetInstance(IPassThru implementation, UInt32 channelId)
        {
            return new PassThruChannel(implementation, channelId);
        }

        /// <summary>
        /// Closes the communication channel
        /// </summary>
        public void Close()
        {
            PassThruStatus status = this.implementation.PassThruDisconnect(this.channelId);
            PassThruUtility.ThrowIfError(status);
        }

        /// <summary>
        /// Read messages and indications from the receive buffer.
        /// </summary>
        /// <param name="message">Pointer to message structures</param>
        /// <param name="timeout">Read timeout, in milliseconds.</param>
        /// <returns>True if a message was read, false if not.</returns>
        public bool ReadMessage(
            PassThruMsg message,
            TimeSpan timeout)
        {
            PassThruStatus status = this.implementation.PassThruReadMsg(
                this.channelId, 
                message,
                (UInt32) timeout.TotalMilliseconds);
            PassThruUtility.ThrowIfError(status);
            return true;
        }

        /// <summary>
        /// Send messages to the ECU.
        /// </summary>
        /// <remarks>
        /// This is a single-message hack to get around the marshaling problem w/ PassThruWriteMsgs.
        /// </remarks>
        /// <param name="messages">Pointer to message structures</param>
        /// <param name="timeout">Write timeout, in milliseconds.</param>
        public void WriteMessage(
            PassThruMsg message,
            TimeSpan timeout)
        {
            PassThruStatus status = this.implementation.PassThruWriteMsg(
                this.channelId,
                message,
                (UInt32)timeout.TotalMilliseconds);
            PassThruUtility.ThrowIfError(status);
        }

        /// <summary>
        /// Send messages to the ECU.
        /// </summary>
        /// <remarks>
        /// DOES NOT MARSHAL THE MESSAGE ARRAY CORRECTLY.
        /// </remarks>
        /// <param name="messages">Pointer to message structures</param>
        /// <param name="messageCount">Pointer to number of messages to send.  On return, will indicate how many messages were sent before timeout expired (if timeout is nonzero) or how many messages were enqueued (if timeout is zero).</param>
        /// <param name="timeout">Write timeout, in milliseconds.</param>
        public void WriteMessages(
            PassThruMsg[] messages,
            ref UInt32 messageCount,
            TimeSpan timeout)
        {
            PassThruStatus status = this.implementation.PassThruWriteMsgs(
                this.channelId, 
                messages,
                ref messageCount,
                (UInt32) timeout.TotalMilliseconds);
            PassThruUtility.ThrowIfError(status);
        }

        /// <summary>
        /// Immediately queue the given message, and re-send at the specified interval.
        /// </summary>
        /// <param name="message">Pointer to a single message structure</param>
        /// <param name="interval">Interval between the start of successive transmissions, in milliseconds.  Valid range is 5-65535.</param>
        /// <returns>message ID</returns>
        public UInt32 StartPeriodicMessage(
            PassThruMsg message,
            TimeSpan interval)
        {
            UInt32 messageId;
            PassThruStatus status = this.implementation.PassThruStartPeriodicMsg(
                this.channelId,
                message,
                out messageId,
                (UInt32) interval.TotalMilliseconds);
            PassThruUtility.ThrowIfError(status);
            return messageId;
        }

        /// <summary>
        /// Stop the given periodic message.
        /// </summary>
        /// <param name="messageId">Periodic-message identifier returned by StartPeriodicMessage</param>
        public void StopPeriodicMessage(
            UInt32 messageId)
        {
            PassThruStatus status = this.implementation.PassThruStopPeriodicMsg(this.channelId, messageId);
            PassThruUtility.ThrowIfError(status);
        }

        /// <summary>
        /// Apply a pass/block filter to incoming messages.
        /// </summary>
        /// <param name="filterType">See FilterType enumeration</param>
        /// <param name="maskMessage">This message will be bitwise-ANDed with incoming messages to mask irrelevant bits.</param>
        /// <param name="patternMessage">This message will be compared with the masked messsage; if equal the FilterType operation will be applied.</param>
        /// <param name="pFilterID">Upon return, will be set with an ID for the newly applied filter.</param>
        /// <returns>message filter ID</returns>
        public UInt32 StartMessageFilter(
            PassThruFilterType filterType,
            PassThruMsg maskMessage,
            PassThruMsg patternMessage)
        {
            UInt32 filterId;
            PassThruMsg flowControl = new PassThruMsg(PassThruProtocol.Iso9141);
            PassThruStatus status = this.implementation.PassThruStartMsgFilter(
                this.channelId, 
                (UInt32) filterType,
                maskMessage,
                patternMessage,
                flowControl,
                out filterId);
            PassThruUtility.ThrowIfError(status);
            return filterId;
        }

        /// <summary>
        /// Apply a flow-control filter to incoming messages.
        /// </summary>
        /// <param name="ChannelID">Channel identifier returned from PassThruConnect</param>
        /// <param name="FilterType">See FilterType enumeration</param>
        /// <param name="pMaskMsg">This message will be bitwise-ANDed with incoming messages to mask irrelevant bits.</param>
        /// <param name="pPatternMsg">This message will be compared with the masked messsage; if equal the FilterType operation will be applied.</param>
        /// <param name="pFlowControlMsg">Points to the CAN ID used for segmented sends and receives.</param>
        /// <param name="pFilterID">Upon return, will be set with an ID for the newly applied filter.</param>
        /// <returns>message filter ID</returns>
        public UInt32 StartFlowControlMessageFilter(
            PassThruMsg maskMessage,
            PassThruMsg patternMessage,
            PassThruMsg flowControlMessage)
        {
            UInt32 filterId;
            PassThruStatus status = this.implementation.PassThruStartMsgFilter(
                this.channelId, 
                (UInt32) PassThruFilterType.FlowControl,
                maskMessage,
                patternMessage,
                flowControlMessage,
                out filterId);
            PassThruUtility.ThrowIfError(status);
            return filterId;
        }
        
        /// <summary>
        /// Removes the given message filter.
        /// </summary>
        /// <param name="filterID">Filter identifier returned from PassThruStartMsgFilter</param>
        public void StopMessageFilter(
            UInt32 filterId)
        {
            PassThruStatus status = this.implementation.PassThruStopMsgFilter(this.channelId, filterId);
            PassThruUtility.ThrowIfError(status);
        }

        /// <summary>
        /// Sets up a connection for SSM
        /// </summary>
        public void InitializeSsm()
        {
            this.InitializeSsmIoCtl();
            this.InitializeSsmFilter();
        }

        /// <summary>
        /// Send the right IoCtls to set up an SSM connection
        /// </summary>
        private void InitializeSsmIoCtl()
        {
            SetConfiguration P1Max = new SetConfiguration(SetConfigurationParameter.P1Max, 2);
            SetConfiguration P3Min = new SetConfiguration(SetConfigurationParameter.P3Min, 0);
            SetConfiguration P4Min = new SetConfiguration(SetConfigurationParameter.P4Min, 0);
            SetConfiguration Loopback = new SetConfiguration(SetConfigurationParameter.Loopback, 1);
            SetConfiguration[] setConfigurationArray = new SetConfiguration[] { P1Max, P3Min, P4Min, Loopback };
            using (SetConfigurationList setConfigurationList = new SetConfigurationList(setConfigurationArray))
            {
                PassThruStatus status = this.implementation.PassThruIoctl(
                    this.channelId,
                    PassThruIOControl.SetConfig,
                    setConfigurationList.Pointer,
                    IntPtr.Zero);
                PassThruUtility.ThrowIfError(status);
            }
        }

        /// <summary>
        /// Set up the filters for an SSM connection
        /// </summary>
        private void InitializeSsmFilter()
        {
            PassThruMsg maskMsg = new PassThruMsg();
            maskMsg.DataSize = 1;

            PassThruMsg patternMsg = new PassThruMsg();
            patternMsg.DataSize = 1;

            // Might need to make this a private member?
            UInt32 filterId;

            // ErrorInvalidMessage w/ OP20
            PassThruStatus status = this.implementation.PassThruStartMsgFilter(
                this.channelId,
                (UInt32)PassThruFilterType.Pass,
                maskMsg,
                patternMsg,
                null,
                out filterId);
            PassThruUtility.ThrowIfError(status);
        }
    }
}

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace NateW.J2534
{
    /// <summary>
    /// Exception type for failed PassThru operations
    /// </summary>
    public class PassThruException : IOException
    {
        private PassThruStatus status;

        public PassThruStatus Status
        {
            get { return this.status; }
        }

        public PassThruException(PassThruStatus status) : base (status.ToString())
        {
            this.status = status;
        }
    }

    /// <summary>
    /// Utilities for PassThru stuff
    /// </summary>
    public static class PassThruUtility
    {
        /// <summary>
        /// Throw if the given PassThruStatus is anything but NoError
        /// </summary>
        internal static void ThrowIfError(PassThruStatus status)
        {
            if (status == PassThruStatus.NoError)
            {
                return;
            }

            throw new PassThruException(status);
        }
    }
}

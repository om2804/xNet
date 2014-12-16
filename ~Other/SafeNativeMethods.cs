using System;
using System.Runtime.InteropServices;
using System.Security;

namespace xNet
{
    [SuppressUnmanagedCodeSecurity]
    internal static class SafeNativeMethods
    {
        [Flags]
        internal enum InternetConnectionState
        {
            InternetConnectionModem = 0x1,
            InternetConnectionLan = 0x2,
            InternetConnectionProxy = 0x4,
            InternetRasInstalled = 0x10,
            InternetConnectionOffline = 0x20,
            InternetConnectionConfigured = 0x40
        }


        [DllImport("wininet.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern bool InternetGetConnectedState(
            ref InternetConnectionState lpdwFlags, int dwReserved);
    }
}
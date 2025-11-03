using System;
using Microsoft.Win32.SafeHandles;

namespace StormLibSharp.Native
{
    internal sealed class MpqArchiveSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public MpqArchiveSafeHandle(IntPtr handle)
            : base(true)
        {
            this.SetHandle(handle);
        }

        public MpqArchiveSafeHandle()
            : base(true) { }

        protected override bool ReleaseHandle()
        {
            return NativeMethods.SFileCloseArchive(this.handle);
        }
    }
}

using System;
using Microsoft.Win32.SafeHandles;

namespace CascLibSharp.Native
{
    internal class CascStorageSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public CascStorageSafeHandle()
            : base(true)
        {

        }

        public CascStorageSafeHandle(IntPtr handle)
            : this()
        {
            SetHandle(handle);
        }

        protected override bool ReleaseHandle()
        {
            var api = Api ?? CascApi.Instance;
            return api.CascCloseStorage!(DangerousGetHandle());
        }

        internal CascApi? Api
        {
            get;
            set;
        }
    }
}

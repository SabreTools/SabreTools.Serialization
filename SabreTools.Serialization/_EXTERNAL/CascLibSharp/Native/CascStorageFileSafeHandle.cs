using System;
using Microsoft.Win32.SafeHandles;

namespace CascLibSharp.Native
{
    internal class CascStorageFileSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public CascStorageFileSafeHandle()
            : base(true)
        {

        }

        public CascStorageFileSafeHandle(IntPtr handle)
            : this()
        {
            SetHandle(handle);
        }

        protected override bool ReleaseHandle()
        {
            var api = Api ?? CascApi.Instance;
            return api.CascCloseFile!(DangerousGetHandle());
        }

        internal CascApi? Api
        {
            get;
            set;
        }
    }
}

using System;
using Microsoft.Win32.SafeHandles;

namespace CascLibSharp.Native
{
    internal class CascFileEnumerationSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public CascFileEnumerationSafeHandle()
            : base(true) { }

        public CascFileEnumerationSafeHandle(IntPtr handle)
            : this()
        {
            SetHandle(handle);
        }

        protected override bool ReleaseHandle()
        {
            var api = Api ?? CascApi.Instance;
            return api.CascFindClose!(DangerousGetHandle());
        }

        internal CascApi? Api
        {
            get;
            set;
        }
    }
}

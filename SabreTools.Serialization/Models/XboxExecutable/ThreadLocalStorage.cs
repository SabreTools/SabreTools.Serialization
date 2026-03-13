namespace SabreTools.Data.Models.XboxExecutable
{
    /// <summary>
    /// XBox Executable thread-local storage
    /// </summary>
    /// <see href="https://www.caustik.com/cxbx/download/xbe.htm"/>
    public class ThreadLocalStorage
    {
        /// <summary>
        /// Address, after the .XBE is loaded into memory, of this .XBE's TLS Data.
        /// </summary>
        public uint DataStartAddress { get; set; }

        /// <summary>
        /// Address, after the .XBE is loaded into memory, of the end of this XBE's TLS Data.
        /// </summary>
        public uint DataEndAddress { get; set; }

        /// <summary>
        /// Address, after the .XBE is loaded into memory, of this XBE's TLS Index.
        /// </summary>
        public uint TLSIndexAddress { get; set; }

        /// <summary>
        /// Address, after the .XBE is loaded into memory, of this XBE's TLS Callback.
        /// </summary>
        public uint TLSCallbackAddress { get; set; }

        /// <summary>
        /// Size of Zero Fill
        /// </summary>
        public uint SizeOfZeroFill { get; set; }

        /// <summary>
        /// Various TLS characteristics.
        /// </summary>
        public uint Characteristics { get; set; }
    }
}

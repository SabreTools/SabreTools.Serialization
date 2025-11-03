using System;
using System.Runtime.InteropServices;

namespace CascLibSharp
{
    /// <summary>
    /// Represents a file found in a CASC container search.
    /// </summary>
    public class CascFoundFile
    {
#if NET20 || NET35 || NET40
        private readonly WeakReference _ownerContext;
#else
        private readonly WeakReference<CascStorageContext> _ownerContext;
#endif

        internal CascFoundFile(string? fileName, IntPtr plainName, byte[] encodingKey, CascLocales locales, long fileSize, CascStorageContext ownerContext)
        {
            FileName = fileName;
            PlainFileName = Marshal.PtrToStringAnsi(plainName);
            EncodingKey = encodingKey;
            Locales = locales;
            FileSize = fileSize;

#if NET20 || NET35 || NET40
            _ownerContext = new WeakReference(ownerContext);
#else
            _ownerContext = new WeakReference<CascStorageContext>(ownerContext);
#endif
        }

        /// <summary>
        /// Gets the full path to this file.
        /// </summary>
        public string? FileName { get; private set; }

        /// <summary>
        /// Gets the plain (no directory-qualified) file name of this file.
        /// </summary>
        public string? PlainFileName { get; private set; }

        /// <summary>
        /// Gets the CASC encoding key for this file.
        /// </summary>
        public byte[] EncodingKey { get; private set; }

        /// <summary>
        /// Gets the locales supported by this resource.
        /// </summary>
        public CascLocales Locales { get; private set; }

        /// <summary>
        /// Gets the length of the file in bytes.
        /// </summary>
        public long FileSize { get; private set; }

        /// <summary>
        /// Opens the found file for reading.
        /// </summary>
        /// <returns>A CascFileStream, which acts as a Stream for a CASC stored file.</returns>
        public CascFileStream Open()
        {
#if NET20 || NET35 || NET40
            CascStorageContext? context = _ownerContext.Target as CascStorageContext;
            if (context == null)
                throw new ObjectDisposedException("The owning context has been closed.");
#else
            if (!_ownerContext.TryGetTarget(out CascStorageContext? context) || context == null)
                throw new ObjectDisposedException("The owning context has been closed.");
#endif

            return context.OpenFileByEncodingKey(EncodingKey);
        }
    }
}

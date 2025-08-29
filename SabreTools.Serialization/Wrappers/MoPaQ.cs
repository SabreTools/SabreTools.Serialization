using System;
using System.IO;
using SabreTools.Serialization.Interfaces;
using SabreTools.Models.MoPaQ;
#if (NET452_OR_GREATER || NETCOREAPP) && (WINX86 || WINX64)
using StormLibSharp;
#endif

namespace SabreTools.Serialization.Wrappers
{
    public partial class MoPaQ : WrapperBase<Archive>, IExtractable
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "MoPaQ Archive";

        #endregion

        #region Constructors

        /// <inheritdoc/>
        /// <remarks>This should only be used for until MPQ parsing is fixed</remarks>
        public MoPaQ(byte[]? data, int offset)
            : base(new Archive(), data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
        /// <remarks>This should only be used for until MPQ parsing is fixed</remarks>
        public MoPaQ(Stream? data)
            : base(new Archive(), data)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
        public MoPaQ(Archive? model, byte[]? data, int offset)
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
        public MoPaQ(Archive? model, Stream? data)
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create a MoPaQ archive from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A MoPaQ archive wrapper on success, null on failure</returns>
        public static MoPaQ? Create(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null || data.Length == 0)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and use that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Create(dataStream);
        }

        /// <summary>
        /// Create a MoPaQ archive from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A MoPaQ archive wrapper on success, null on failure</returns>
        public static MoPaQ? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = Deserializers.MoPaQ.DeserializeStream(data);
                if (model == null)
                    return null;

                data.Seek(currentOffset, SeekOrigin.Begin);
                return new MoPaQ(model, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Extraction

        /// <inheritdoc/>
        public bool Extract(string outputDirectory, bool includeDebug)
        {
#if NET20 || NET35 || !(WINX86 || WINX64)
            Console.WriteLine("Extraction is not supported for this framework!");
            Console.WriteLine();
            return false;
#else
            try
            {
                if (Filename == null || !File.Exists(Filename))
                    return false;

                // Try to open the archive and listfile
                var mpqArchive = new MpqArchive(Filename, FileAccess.Read);
                string? listfile = null;
                MpqFileStream listStream = mpqArchive.OpenFile("(listfile)");

                // If we can't read the listfile, we just return
                if (!listStream.CanRead)
                    return false;

                // Read the listfile in for processing
                using (var sr = new StreamReader(listStream))
                {
                    listfile = sr.ReadToEnd();
                }

                // Split the listfile by newlines
                string[] listfileLines = listfile.Replace("\r\n", "\n").Split('\n');

                // Loop over each entry
                foreach (string sub in listfileLines)
                {
                    // Ensure directory separators are consistent
                    string filename = sub;
                    if (Path.DirectorySeparatorChar == '\\')
                        filename = filename.Replace('/', '\\');
                    else if (Path.DirectorySeparatorChar == '/')
                        filename = filename.Replace('\\', '/');

                    // Ensure the full output directory exists
                    filename = Path.Combine(outDir, filename);
                    var directoryName = Path.GetDirectoryName(filename);
                    if (directoryName != null && !Directory.Exists(directoryName))
                        Directory.CreateDirectory(directoryName);

                    // Try to write the data
                    try
                    {
                        mpqArchive.ExtractFile(sub, filename);
                    }
                    catch (System.Exception ex)
                    {
                        if (includeDebug) System.Console.WriteLine(ex);
                    }
                }

                return true;
            }
            catch (System.Exception ex)
            {
                if (includeDebug) System.Console.WriteLine(ex);
                return false;
            }
#endif
        }

        #endregion
    }
}

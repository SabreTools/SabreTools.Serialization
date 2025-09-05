using System;
using System.IO;
using SabreTools.IO.Extensions;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Wrappers
{
    public class WAD3 : WrapperBase<Models.WAD3.File>, IExtractable
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Half-Life Texture Package File (WAD3)";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="Models.WAD3.File.DirEntries"/>
        public Models.WAD3.DirEntry[]? DirEntries => Model.DirEntries;

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public WAD3(Models.WAD3.File? model, byte[]? data, int offset)
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
        public WAD3(Models.WAD3.File? model, Stream? data)
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create a WAD3 from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the WAD3</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A WAD3 wrapper on success, null on failure</returns>
        public static WAD3? Create(byte[]? data, int offset)
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
        /// Create a WAD3 from a Stream
        /// </summary>
        /// <param name="data">Stream representing the WAD3</param>
        /// <returns>An WAD3 wrapper on success, null on failure</returns>
        public static WAD3? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = Deserializers.WAD3.DeserializeStream(data);
                if (model == null)
                    return null;

                data.Seek(currentOffset, SeekOrigin.Begin);
                return new WAD3(model, data);
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
            // If we have no lumps
            if (DirEntries == null || DirEntries.Length == 0)
                return false;

            // Loop through and extract all lumps to the output
            bool allExtracted = true;
            for (int i = 0; i < DirEntries.Length; i++)
            {
                allExtracted &= ExtractLump(i, outputDirectory, includeDebug);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a lump from the WAD3 to an output directory by index
        /// </summary>
        /// <param name="index">Lump index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>True if the lump extracted, false otherwise</returns>
        public bool ExtractLump(int index, string outputDirectory, bool includeDebug)
        {
            // If we have no lumps
            if (DirEntries == null || DirEntries.Length == 0)
                return false;

            // If the lumps index is invalid
            if (index < 0 || index >= DirEntries.Length)
                return false;

            // Read the data -- TODO: Handle uncompressed lumps (see BSP.ExtractTexture)
            var lump = DirEntries[index];
            var data = ReadRangeFromSource((int)lump.Offset, (int)lump.Length);
            if (data == null)
                return false;

            // If we have an invalid output directory
            if (string.IsNullOrEmpty(outputDirectory))
                return false;

            // Ensure directory separators are consistent
            string filename = $"{lump.Name}.lmp";
            if (Path.DirectorySeparatorChar == '\\')
                filename = filename.Replace('/', '\\');
            else if (Path.DirectorySeparatorChar == '/')
                filename = filename.Replace('\\', '/');

            // Ensure the full output directory exists
            filename = Path.Combine(outputDirectory, filename);
            var directoryName = Path.GetDirectoryName(filename);
            if (directoryName != null && !Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);

            // Try to write the data
            try
            {
                // Open the output file for writing
                using Stream fs = File.OpenWrite(filename);
                fs.Write(data, 0, data.Length);
                fs.Flush();
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.Error.WriteLine(ex);
                return false;
            }

            return true;
        }

        #endregion
    }
}

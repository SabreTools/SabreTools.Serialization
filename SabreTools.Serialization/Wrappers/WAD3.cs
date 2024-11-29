using System.IO;

namespace SabreTools.Serialization.Wrappers
{
    public class WAD3 : WrapperBase<Models.WAD3.File>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Half-Life Texture Package File (WAD3)";

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
            if (data == null || data.Length == 0 || !data.CanRead)
                return null;

            try
            {
                var file = Deserializers.WAD3.DeserializeStream(data);
                if (file == null)
                    return null;

                return new WAD3(file, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Extraction

        /// <summary>
        /// Extract all lumps from the WAD3 to an output directory
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if all lumps extracted, false otherwise</returns>
        public bool ExtractAllLumps(string outputDirectory)
        {
            // If we have no lumps
            if (Model.DirEntries == null || Model.DirEntries.Length == 0)
                return false;

            // Loop through and extract all lumps to the output
            bool allExtracted = true;
            for (int i = 0; i < Model.DirEntries.Length; i++)
            {
                allExtracted &= ExtractLump(i, outputDirectory);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a lump from the WAD3 to an output directory by index
        /// </summary>
        /// <param name="index">Lump index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if the lump extracted, false otherwise</returns>
        public bool ExtractLump(int index, string outputDirectory)
        {
            // If we have no lumps
            if (Model.DirEntries == null || Model.DirEntries.Length == 0)
                return false;

            // If the lumps index is invalid
            if (index < 0 || index >= Model.DirEntries.Length)
                return false;

            // Get the lump
            var lump = Model.DirEntries[index];
            if (lump == null)
                return false;

            // Read the data -- TODO: Handle uncompressed lumps (see BSP.ExtractTexture)
            var data = ReadFromDataSource((int)lump.Offset, (int)lump.Length);
            if (data == null)
                return false;

            // Create the filename
            string filename = $"{lump.Name}.lmp";

            // If we have an invalid output directory
            if (string.IsNullOrEmpty(outputDirectory))
                return false;

            // Create the full output path
            filename = Path.Combine(outputDirectory, filename);

            // Ensure the output directory is created
            var directoryName = Path.GetDirectoryName(filename);
            if (directoryName != null)
                Directory.CreateDirectory(directoryName);

            // Try to write the data
            try
            {
                // Open the output file for writing
                using Stream fs = File.OpenWrite(filename);
                fs.Write(data, 0, data.Length);
            }
            catch
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}
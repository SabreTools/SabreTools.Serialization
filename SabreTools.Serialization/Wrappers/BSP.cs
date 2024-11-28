using System.IO;
using SabreTools.Models.BSP;

namespace SabreTools.Serialization.Wrappers
{
    public class BSP : WrapperBase<BspFile>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Half-Life Level (BSP)";

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public BSP(BspFile? model, byte[]? data, int offset)
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
        public BSP(BspFile? model, Stream? data)
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create a BSP from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the BSP</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A BSP wrapper on success, null on failure</returns>
        public static BSP? Create(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and use that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Create(dataStream);
        }

        /// <summary>
        /// Create a BSP from a Stream
        /// </summary>
        /// <param name="data">Stream representing the BSP</param>
        /// <returns>An BSP wrapper on success, null on failure</returns>
        public static BSP? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var file = Deserializers.BSP.DeserializeStream(data);
            if (file == null)
                return null;

            try
            {
                return new BSP(file, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Extraction

        /// <summary>
        /// Extract all lumps from the BSP to an output directory
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if all lumps extracted, false otherwise</returns>
        public bool ExtractAllLumps(string outputDirectory)
        {
            // If we have no lumps
            if (Model.Header?.Lumps == null || Model.Header.Lumps.Length == 0)
                return false;

            // Loop through and extract all lumps to the output
            bool allExtracted = true;
            for (int i = 0; i < Model.Header.Lumps.Length; i++)
            {
                allExtracted &= ExtractLump(i, outputDirectory);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a lump from the BSP to an output directory by index
        /// </summary>
        /// <param name="index">Lump index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if the lump extracted, false otherwise</returns>
        public bool ExtractLump(int index, string outputDirectory)
        {
            // If we have no lumps
            if (Model.Header?.Lumps == null || Model.Header.Lumps.Length == 0)
                return false;

            // If the lumps index is invalid
            if (index < 0 || index >= Model.Header.Lumps.Length)
                return false;

            // Get the lump
            var lump = Model.Header.Lumps[index];
            if (lump == null)
                return false;

            // Read the data
            var data = ReadFromDataSource(lump.Offset, lump.Length);
            if (data == null)
                return false;

            // Create the filename
            string filename = $"lump_{index}.bin";
            switch ((LumpType)index)
            {
                case LumpType.LUMP_ENTITIES:
                    filename = "entities.ent";
                    break;
                case LumpType.LUMP_TEXTURES:
                    filename = "texture_data.bin";
                    break;
            }

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
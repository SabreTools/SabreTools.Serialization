using System;
using System.IO;

namespace SabreTools.Wrappers
{
    public partial class NESCart : IWritable
    {
        /// <inheritdoc/>
        public bool Write(string outputPath, bool includeDebug)
        {
            // Ensure an output path
            if (string.IsNullOrEmpty(outputPath))
            {
                string outputFilename = Filename is null
                    ? (Guid.NewGuid().ToString() + ".nes")
                    : (Filename + ".new");
                outputPath = Path.GetFullPath(outputFilename);
            }

            // Check for invalid data
            if (Header is null || PrgRomData is null || PrgRomData.Length == 0)
            {
                if (includeDebug) Console.WriteLine("Model was invalid, cannot write!");
                return false;
            }

            // Create and use the writer
            var writer = new Serialization.Writers.NESCart { Debug = includeDebug };
            return writer.SerializeFile(Model, outputPath);
        }
    }
}

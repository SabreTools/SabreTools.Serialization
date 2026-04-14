using System;
using System.IO;

namespace SabreTools.Wrappers
{
    public partial class QD : IWritable
    {
        /// <inheritdoc/>
        public bool Write(string outputPath, bool includeDebug)
        {
            // Ensure an output path
            if (string.IsNullOrEmpty(outputPath))
            {
                string outputFilename = Filename is null
                    ? (Guid.NewGuid().ToString() + ".fds")
                    : (Filename + ".new");
                outputPath = Path.GetFullPath(outputFilename);
            }

            // Check for invalid data
            if (Model.Data is null || Model.Data.Length == 0)
            {
                if (includeDebug) Console.WriteLine("Model was invalid, cannot write!");
                return false;
            }

            // Create and use the writer
            var writer = new Serialization.Writers.QD { Debug = includeDebug };
            return writer.SerializeFile(Model, outputPath);
        }
    }
}

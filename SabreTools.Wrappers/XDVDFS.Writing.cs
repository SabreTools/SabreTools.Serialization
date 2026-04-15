using System;
using System.IO;
using SabreTools.Data.Models.XDVDFS;
using SabreTools.Numerics.Extensions;

namespace SabreTools.Wrappers
{
    public partial class XDVDFS : IWritable
    {
        /// <inheritdoc/>
        public bool Write(string outputDirectory, bool includeDebug)
        {
            // Get the base path
            string outputFilename = Filename is null
                ? Guid.NewGuid().ToString()
                : Path.GetFileName(Filename);
            outputFilename += ".xiso";
            string outputPath = Path.Combine(outputDirectory, outputFilename);

            var writer = new SabreTools.Serialization.Writers.XDVDFS();
            if (!writer.SerializeFile(Model, outputPath))
            {
                if (includeDebug) Console.WriteLine("Model was invalid, cannot write!");
                return false;
            }

            return true;
        }
    }
}

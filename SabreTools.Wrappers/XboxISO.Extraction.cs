using SabreTools.Data.Models.XboxISO;

namespace SabreTools.Wrappers
{
    public partial class XboxISO : IExtractable
    {
        /// <inheritdoc/>
        public bool Extract(string outputDirectory, bool includeDebug)
        {
            long initialOffset = _dataSource.Position;

            bool success = ExtractVideoPartition(outputDirectory, includeDebug);
            success |= ExtractGamePartition(outputDirectory, includeDebug);

            return success;
        }

        /// <summary>
        /// Extract all files from the Video ISO partition only
        /// </summary>
        public bool ExtractVideoPartition(string outputDirectory, bool includeDebug)
        {
            // Extract all files from the video partition
            var videoWrapper = new ISO9660(VideoPartition, _dataSource, initialOffset, _dataSource.Length);
            return videoWrapper?.Extract(outputDirectory, includeDebug) ?? false;
        }

        /// <summary>
        /// Extract all files from the XDVDFS (XISO) game partition only
        /// </summary>
        public bool ExtractGamePartition(string outputDirectory, bool includeDebug)
        {
            // Extract all files from the game partition
            var gameWrapper = new XDVDFS(GamePartition, _dataSource, initialOffset + Constants.XisoOffsets[XGDType], Constants.XisoLengths[XGDType]);
            return gameWrapper?.Extract(outputDirectory, includeDebug) ?? false;
        }
    }
}

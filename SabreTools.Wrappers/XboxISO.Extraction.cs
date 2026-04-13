using SabreTools.Data.Models.XboxISO;

namespace SabreTools.Wrappers
{
    public partial class XboxISO : IExtractable
    {
        /// <inheritdoc/>
        public bool Extract(string outputDirectory, bool includeDebug)
        {
            long initialOffset = _dataSource.Position;

            // Extract all files from the video partition
            var videoWrapper = new ISO9660(VideoPartition, _dataSource, initialOffset, _dataSource.Length);
            bool success = videoWrapper?.Extract(outputDirectory, includeDebug) ?? false;

            // Extract all files from the game partition
            var gameWrapper = new XDVDFS(GamePartition, _dataSource, initialOffset + Constants.XisoOffsets[XGDType], Constants.XisoLengths[XGDType]);
            success |= gameWrapper?.Extract(outputDirectory, includeDebug) ?? false;

            return success;
        }
    }
}

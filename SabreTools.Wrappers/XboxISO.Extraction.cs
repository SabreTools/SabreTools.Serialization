using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.Data.Models.XboxISO;

namespace SabreTools.Wrappers
{
    public partial class XboxISO : IExtractable
    {
        /// <inheritdoc/>
        public virtual bool Extract(string outputDirectory, bool includeDebug)
        {
            long initialOffset = _dataSource.Position;

            var videoWrapper = new SabreTools.Wrappers.ISO9660(VideoPartition, _dataSource, initialOffset, _dataSource.Length);
            bool success = videoWrapper?.Extract(outputDirectory, includeDebug) ?? false;

            var gameWrapper = new SabreTools.Wrappers.XDVDFS(GamePartition, _dataSource, initialOffset + Constants.XisoOffsets[XGDType], Constants.XisoLengths[XGDType]);
            success |= gameWrapper?.Extract(outputDirectory, includeDebug) ?? false;

            return success;
        }
    }
}

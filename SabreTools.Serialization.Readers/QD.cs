using System.IO;
using SabreTools.IO.Extensions;

namespace SabreTools.Serialization.Readers
{
    public class QD : BaseBinaryReader<Data.Models.NES.QD>
    {
        /// <inheritdoc/>
        public override Data.Models.NES.QD? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data is null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long initialOffset = data.Position;

                // Create a new QD file to fill
                var QD = new Data.Models.NES.QD();

                // If the size is invalid
                if ((data.Length - data.Position) == 0 || (data.Length - data.Position) % 65536 != 0)
                    return null;

                // Read the disk data
                QD.Data = data.ReadBytes((int)(data.Length - data.Position));

                return QD;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }
    }
}

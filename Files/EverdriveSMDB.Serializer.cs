using SabreTools.Models.EverdriveSMDB;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class EverdriveSMDB : IFileSerializer<MetadataFile>
    {
        /// <inheritdoc/>
        public bool Serialize(MetadataFile? obj, string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            using (var stream = new Streams.EverdriveSMDB().Serialize(obj))
            {
                if (stream == null)
                    return false;

                using (var fs = System.IO.File.OpenWrite(path))
                {
                    stream.CopyTo(fs);
                    return true;
                }
            }
        }
    }
}
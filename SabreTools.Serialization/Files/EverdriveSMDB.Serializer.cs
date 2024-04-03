using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class EverdriveSMDB : IFileSerializer<Models.EverdriveSMDB.MetadataFile>
    {
        /// <inheritdoc cref="IFileSerializer.SerializeImpl(T?, string?)"/>
        public static bool Serialize(Models.EverdriveSMDB.MetadataFile? obj, string? path)
        {
            var serializer = new EverdriveSMDB();
            return serializer.SerializeImpl(obj, path);
        }
        
        /// <inheritdoc/>
        public bool SerializeImpl(Models.EverdriveSMDB.MetadataFile? obj, string? path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            using var stream = new Streams.EverdriveSMDB().SerializeImpl(obj);
            if (stream == null)
                return false;

            using var fs = System.IO.File.OpenWrite(path);
            stream.CopyTo(fs);
            return true;
        }
    }
}
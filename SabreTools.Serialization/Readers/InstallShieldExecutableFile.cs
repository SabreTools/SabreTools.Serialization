using System.IO;
using SabreTools.Data.Models.InstallShieldExecutable;
using SabreTools.IO.Extensions;

namespace SabreTools.Serialization.Readers
{
    public class InstallShieldExecutableFile : BaseBinaryReader<ExtractableFile>
    {
        public override ExtractableFile? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the initial offset
                long initialOffset = data.Position;

                // Try to parse the header
                var header = ParseExtractableFileHeader(data);
                if (header == null)
                    return null;

                return header;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }        
        }
        
        private static ExtractableFile? ParseExtractableFileHeader(Stream? data)
        {
            var obj = new ExtractableFile();
            obj.Name = data.ReadNullTerminatedAnsiString();
            if (obj.Name == null)
                return null;
        
            obj.Path = data.ReadNullTerminatedAnsiString();
            if (obj.Path == null)
                return null;
        
            obj.Version = data.ReadNullTerminatedAnsiString();
            if (obj.Version == null)
                return null;
        
            var versionString = data.ReadNullTerminatedAnsiString();
            if (versionString == null || !int.TryParse(versionString, out int foundVersion))
                return null;
        
            obj.Length = (uint)foundVersion;
            if (obj.Name == null)
                return null;
        
            return obj;
        }
    }
}
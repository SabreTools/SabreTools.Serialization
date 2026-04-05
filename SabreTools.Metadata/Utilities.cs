using System.IO;

namespace SabreTools.Metadata
{
    /// <summary>
    /// Static utility functions used throughout the library
    /// </summary>
    public static class Utilities
    {
       /// <summary>
        /// Get if the given path has a valid DAT extension
        /// </summary>
        /// <param name="path">Path to check</param>
        /// <returns>True if the extension is valid, false otherwise</returns>
        public static bool HasValidDatExtension(string? path)
        {
            // If the path is null or empty, then we return false
            if (string.IsNullOrEmpty(path))
                return false;

            // Get the extension from the path, if possible
            string ext = Path.GetExtension(path).TrimStart('.').ToLowerInvariant();

            // Check against the list of known DAT extensions
            return ext switch
            {
                "csv" => true,
                "dat" => true,
                "json" => true,
                "md2" => true,
                "md4" => true,
                "md5" => true,
                "ripemd128" => true,
                "ripemd160" => true,
                "sfv" => true,
                "sha1" => true,
                "sha256" => true,
                "sha384" => true,
                "sha512" => true,
                "spamsum" => true,
                "ssv" => true,
                "tsv" => true,
                "txt" => true,
                "xml" => true,
                _ => false,
            };
        }
    }
}

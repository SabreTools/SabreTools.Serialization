namespace SabreTools.Wrappers
{
    public partial class NintendoDisc : IExtractable
    {
        /// <inheritdoc/>
        public bool Extract(string outputDirectory, bool includeDebug)
        {
            // TODO: Implement full FST-based file extraction.
            // GameCube: read FST at DiscHeader.FstOffset (no shift), enumerate file entries, copy raw data.
            // Wii: per-partition, read ticket at PartitionOffset+0x0, TMD at +0x2A4, decrypt title key,
            //      then read and decrypt each 0x8000-byte block (0x400 hash + 0x7C00 data) using AES-128-CBC,
            //      locate partition FST and extract files from decrypted stream.
            return false;
        }
    }
}

namespace SabreTools.Data.Models.Listrom
{
    /// <summary>
    /// ROMs required for driver "testdriver".
    /// Name                                   Size Checksum
    /// abcd.bin                               1024 CRC(00000000) SHA1(da39a3ee5e6b4b0d3255bfef95601890afd80709)
    /// efgh.bin                               1024 BAD CRC(00000000) SHA1(da39a3ee5e6b4b0d3255bfef95601890afd80709) BAD_DUMP
    /// ijkl.bin                               1024 NO GOOD DUMP KNOWN
    /// abcd                                        SHA1(da39a3ee5e6b4b0d3255bfef95601890afd80709)
    /// efgh                                        BAD SHA1(da39a3ee5e6b4b0d3255bfef95601890afd80709) BAD_DUMP
    /// ijkl                                        NO GOOD DUMP KNOWN
    /// </summary>
    public class Set
    {
        public string? Driver { get; set; }

        public string? Device { get; set; }

        public Row[]? Row { get; set; }
    }
}
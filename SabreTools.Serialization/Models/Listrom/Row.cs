namespace SabreTools.Serialization.Models.Listrom
{
    /// <summary>
    /// ROMs required for driver "testdriver".
    /// Name                                   Size Checksum
    /// abcd.bin                               1024 CRC(00000000) SHA1(da39a3ee5e6b4b0d3255bfef95601890afd80709)
    /// efgh.bin                               1024 BAD CRC(00000000) SHA1(da39a3ee5e6b4b0d3255bfef95601890afd80709) BAD_DUMP
    /// ijkl.bin                               1024 NO GOOD DUMP KNOWN
    /// abcd                                        MD5(d41d8cd98f00b204e9800998ecf8427e)
    /// abcd                                        SHA1(da39a3ee5e6b4b0d3255bfef95601890afd80709)
    /// efgh                                        BAD MD5(d41d8cd98f00b204e9800998ecf8427e) BAD_DUMP
    /// efgh                                        BAD SHA1(da39a3ee5e6b4b0d3255bfef95601890afd80709) BAD_DUMP
    /// ijkl                                        NO GOOD DUMP KNOWN
    /// </summary>
    public class Row
    {
        [SabreTools.Models.Required]
        public string? Name { get; set; }

        public string? Size { get; set; }

        public bool Bad { get; set; }

        public string? CRC { get; set; }

        public string? MD5 { get; set; }

        public string? SHA1 { get; set; }

        public bool NoGoodDumpKnown { get; set; }
    }
}
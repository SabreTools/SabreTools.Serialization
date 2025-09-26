namespace SabreTools.Serialization.Models.Hashfile
{
    /// <remarks>Hashfiles can only contain one type of hash at a time</remarks>
    public class Hashfile
    {
        public SFV[]? SFV { get; set; }

        public MD2[]? MD2 { get; set; }

        public MD4[]? MD4 { get; set; }

        public MD5[]? MD5 { get; set; }

        public RIPEMD128[]? RIPEMD128 { get; set; }

        public RIPEMD160[]? RIPEMD160 { get; set; }

        public SHA1[]? SHA1 { get; set; }

        public SHA256[]? SHA256 { get; set; }

        public SHA384[]? SHA384 { get; set; }

        public SHA512[]? SHA512 { get; set; }

        public SpamSum[]? SpamSum { get; set; }
    }
}
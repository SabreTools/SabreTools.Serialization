namespace SabreTools.Data.Models.InstallShieldArchiveV3
{
    public enum Attributes : byte
    {
        READONLY = 0x01,
        HIDDEN = 0x02,
        SYSTEM = 0x04,
        UNCOMPRESSED = 0x10,
        ARCHIVE = 0x20,
    }
}

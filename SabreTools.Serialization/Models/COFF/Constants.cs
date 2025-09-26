namespace SabreTools.Serialization.Models.COFF
{
    public static class Constants
    {
        /// <summary>
        /// Fixed size of <see cref="FileHeader"/> 
        /// </summary>
        public const int FileHeaderSize = 20;

        /// <summary>
        /// Fixed size of <see cref="SectionHeader"/> 
        /// </summary>
        public const int SectionHeaderSize = 40;

        /// <summary>
        /// Fixed size of <see cref="SymbolTableEntries.BaseEntry"/> 
        /// </summary>
        public const int SymbolTableEntrySize = 18;
    }
}
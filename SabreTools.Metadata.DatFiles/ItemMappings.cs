namespace SabreTools.Metadata.DatFiles
{
    /// <summary>
    /// Class used during deduplication
    /// </summary>
    public struct ItemMappings(DatItems.DatItem item, long machineId, long sourceId)
    {
        public DatItems.DatItem Item = item;
        public long MachineId = machineId;
        public long SourceId = sourceId;
    }
}

namespace SabreTools.Data.Models.ISO9660
{
    /// <summary>
    /// An int16 stored as int16-LSB followed by int16-MSB
    /// </summary>
    public class BothEndianInt16
    {
        public short LSB { get; set; }
        public int MSB { get; set; }
    }

    /// <summary>
    /// An int32 stored as int32-LSB followed by int32-MSB
    /// </summary>
    public class BothEndianInt32
    {
        public int LSB { get; set; }
        public int MSB { get; set; }
    }
}

namespace SabreTools.Data.Models.BSP
{
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/>
    public sealed class PhysCollideLump : Lump
    {
        /// <summary>
        /// Models
        /// </summary>
        public PhysModel[] Models { get; set; } = [];
    }
}

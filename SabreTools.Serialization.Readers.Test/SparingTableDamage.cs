namespace SabreTools.Serialization.Readers.Test
{
    /// <summary>
    /// Damage to the first copy of the Sparing Table, which then relocates the packets elsewhere
    /// </summary>
    public enum SparingTableDamage
    {
        None,
        Checksum,
        TagIdentifier,
        SparingIdentifier,
    }
}

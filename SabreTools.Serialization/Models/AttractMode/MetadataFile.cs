namespace SabreTools.Serialization.Models.AttractMode
{
    /// <summary>
    /// #Name;Title;Emulator;CloneOf;Year;Manufacturer;Category;Players;Rotation;Control;Status;DisplayCount;DisplayType;AltRomname;AltTitle;Extra;Buttons    /// </summary>
    /// </summary>
    public class MetadataFile
    {
        [SabreTools.Models.Required]
        public string[]? Header { get; set; }

        public Row[]? Row { get; set; }
    }
}
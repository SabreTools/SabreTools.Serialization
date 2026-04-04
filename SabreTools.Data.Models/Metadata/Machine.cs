namespace SabreTools.Data.Models.Metadata
{
    /// <summary>
    /// Format-agnostic representation of game, machine, and set data
    /// </summary>
    public class Machine : DictionaryBase
    {
        #region Properties

        public Adjuster[]? Adjuster { get; set; }

        public Archive[]? Archive { get; set; }

        public BiosSet[]? BiosSet { get; set; }

        public string? Board { get; set; }

        public string? Buttons { get; set; }

        public Chip[]? Chip { get; set; }

        public string? CloneOf { get; set; }

        public string? CloneOfId { get; set; }

        public string? Company { get; set; }

        public Configuration[]? Configuration { get; set; }

        public string? Control { get; set; }

        public string? CRC { get; set; }

        public string? Country { get; set; }

        public string? Description { get; set; }

        public string? Developer { get; set; }

        public Device[]? Device { get; set; }

        public DeviceRef[]? DeviceRef { get; set; }

        public DipSwitch[]? DipSwitch { get; set; }

        public string? DirName { get; set; }

        public Disk[]? Disk { get; set; }

        public Display[]? Display { get; set; }

        public string? DisplayCount { get; set; }

        public string? DisplayType { get; set; }

        public Driver? Driver { get; set; }

        public Dump[]? Dump { get; set; }

        public string? DuplicateID { get; set; }

        public string? Emulator { get; set; }

        public string? Enabled { get; set; }

        public string? Extra { get; set; }

        public string? Favorite { get; set; }

        public Feature[]? Feature { get; set; }

        public string? GenMSXID { get; set; }

        public string? Genre { get; set; }

        public string? Hash { get; set; }

        public string? History { get; set; }

        public string? Id { get; set; }

        public string? Im1CRC { get; set; }

        public string? Im2CRC { get; set; }

        public string? ImageNumber { get; set; }

        public Info[]? Info { get; set; }

        public Input? Input { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? IsBios { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? IsDevice { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? IsMechanical { get; set; }

        public string? Language { get; set; }

        public string? Location { get; set; }

        public string? Manufacturer { get; set; }

        public Media[]? Media { get; set; }

        public string? Name { get; set; }

        public string? Notes { get; set; }

        public Part[]? Part { get; set; }

        public string? PlayedCount { get; set; }

        public string? PlayedTime { get; set; }

        public string? Players { get; set; }

        public Port[]? Port { get; set; }

        public string? Publisher { get; set; }

        public RamOption[]? RamOption { get; set; }

        public string? Ratings { get; set; }

        public string? RebuildTo { get; set; }

        public string? RelatedTo { get; set; }

        public Release[]? Release { get; set; }

        public string? ReleaseNumber { get; set; }

        public Rom[]? Rom { get; set; }

        public string? RomOf { get; set; }

        public string? Rotation { get; set; }

        /// <remarks>(yes|partial|no) "no"</remarks>
        public Runnable? Runnable { get; set; }

        public Sample[]? Sample { get; set; }

        public string? SampleOf { get; set; }

        public string? SaveType { get; set; }

        public string? Score { get; set; }

        public SharedFeat[]? SharedFeat { get; set; }

        public Slot[]? Slot { get; set; }

        public SoftwareList[]? SoftwareList { get; set; }

        public Sound? Sound { get; set; }

        public string? Source { get; set; }

        public string? SourceFile { get; set; }

        public string? SourceRom { get; set; }

        public string? Status { get; set; }

        public string? Subgenre { get; set; }

        /// <remarks>(yes|partial|no) "yes"</remarks>
        public Supported? Supported { get; set; }

        public string? System { get; set; }

        public string? Tags { get; set; }

        public string? TitleID { get; set; }

        public string? Url { get; set; }

        public Video[]? Video { get; set; }

        public string? Year { get; set; }

        #endregion

        #region Keys

        /// <remarks>string, string[]</remarks>
        public const string CategoryKey = "category";

        /// <remarks>string, string[]</remarks>
        public const string CommentKey = "comment";

        #endregion
    }
}

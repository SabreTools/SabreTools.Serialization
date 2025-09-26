namespace SabreTools.Data.Models.RomCenter
{
    /// <remarks>¬-delimited</remarks>
    public class Rom
    {
        /// <remarks>0</remarks>
        public string? ParentName { get; set; }

        /// <remarks>1</remarks>
        public string? ParentDescription { get; set; }

        /// <remarks>2</remarks>
        public string? GameName { get; set; }

        /// <remarks>3</remarks>
        public string? GameDescription { get; set; }

        /// <remarks>4</remarks>
        public string? RomName { get; set; }

        /// <remarks>5</remarks>
        public string? RomCRC { get; set; }

        /// <remarks>6</remarks>
        public string? RomSize { get; set; }

        /// <remarks>7</remarks>
        public string? RomOf { get; set; }

        /// <remarks>8</remarks>
        public string? MergeName { get; set; }
    }
}
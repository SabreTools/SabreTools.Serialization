using System;

namespace SabreTools.Data.Models.Metadata
{
    /// <summary>
    /// Format-agnostic representation of game, machine, and set data
    /// </summary>
    public class Machine : DictionaryBase, ICloneable, IEquatable<Machine>
    {
        #region Properties

        public Adjuster[]? Adjuster { get; set; }

        public Archive[]? Archive { get; set; }

        public BiosSet[]? BiosSet { get; set; }

        public string? Board { get; set; }

        public string? Buttons { get; set; }

        public string[]? Category { get; set; }

        public Chip[]? Chip { get; set; }

        public string? CloneOf { get; set; }

        public string? CloneOfId { get; set; }

        public string[]? Comment { get; set; }

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

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new Machine();

            if (Adjuster is not null)
                obj.Adjuster = Array.ConvertAll(Adjuster, i => (Adjuster)i.Clone());
            if (Archive is not null)
                obj.Archive = Array.ConvertAll(Archive, i => (Archive)i.Clone());
            if (BiosSet is not null)
                obj.BiosSet = Array.ConvertAll(BiosSet, i => (BiosSet)i.Clone());
            obj.Board = Board;
            obj.Buttons = Buttons;
            if (Category is not null)
                obj.Category = Array.ConvertAll(Category, i => i);
            if (Chip is not null)
                obj.Chip = Array.ConvertAll(Chip, i => (Chip)i.Clone());
            obj.CloneOf = CloneOf;
            obj.CloneOfId = CloneOfId;
            if (Comment is not null)
                obj.Comment = Array.ConvertAll(Comment, i => i);
            obj.Company = Company;
            if (Configuration is not null)
                obj.Configuration = Array.ConvertAll(Configuration, i => (Configuration)i.Clone());
            obj.Control = Control;
            obj.CRC = CRC;
            obj.Country = Country;
            obj.Description = Description;
            obj.Developer = Developer;
            if (Device is not null)
                obj.Device = Array.ConvertAll(Device, i => (Device)i.Clone());
            if (DeviceRef is not null)
                obj.DeviceRef = Array.ConvertAll(DeviceRef, i => (DeviceRef)i.Clone());
            if (DipSwitch is not null)
                obj.DipSwitch = Array.ConvertAll(DipSwitch, i => (DipSwitch)i.Clone());
            obj.DirName = DirName;
            if (Disk is not null)
                obj.Disk = Array.ConvertAll(Disk, i => (Disk)i.Clone());
            if (Display is not null)
                obj.Display = Array.ConvertAll(Display, i => (Display)i.Clone());
            obj.DisplayCount = DisplayCount;
            obj.DisplayType = DisplayType;
            if (Driver is not null)
                obj.Driver = (Driver)Driver.Clone();
            if (Dump is not null)
                obj.Dump = Array.ConvertAll(Dump, i => (Dump)i.Clone());
            obj.DuplicateID = DuplicateID;
            obj.Emulator = Emulator;
            obj.Enabled = Enabled;
            obj.Extra = Extra;
            obj.Favorite = Favorite;
            if (Feature is not null)
                obj.Feature = Array.ConvertAll(Feature, i => (Feature)i.Clone());
            obj.GenMSXID = GenMSXID;
            obj.Genre = Genre;
            obj.Hash = Hash;
            obj.History = History;
            obj.Id = Id;
            obj.Im1CRC = Im1CRC;
            obj.Im2CRC = Im2CRC;
            obj.ImageNumber = ImageNumber;
            if (Info is not null)
                obj.Info = Array.ConvertAll(Info, i => (Info)i.Clone());
            if (Input is not null)
                obj.Input = (Input)Input.Clone();
            obj.IsBios = IsBios;
            obj.IsDevice = IsDevice;
            obj.IsMechanical = IsMechanical;
            obj.Language = Language;
            obj.Location = Location;
            obj.Manufacturer = Manufacturer;
            if (Media is not null)
                obj.Media = Array.ConvertAll(Media, i => (Media)i.Clone());
            obj.Name = Name;
            obj.Notes = Notes;
            if (Part is not null)
                obj.Part = Array.ConvertAll(Part, i => (Part)i.Clone());
            obj.PlayedCount = PlayedCount;
            obj.PlayedTime = PlayedTime;
            obj.Players = Players;
            if (Port is not null)
                obj.Port = Array.ConvertAll(Port, i => (Port)i.Clone());
            obj.Publisher = Publisher;
            if (RamOption is not null)
                obj.RamOption = Array.ConvertAll(RamOption, i => (RamOption)i.Clone());
            obj.Ratings = Ratings;
            obj.RebuildTo = RebuildTo;
            obj.RelatedTo = RelatedTo;
            if (Release is not null)
                obj.Release = Array.ConvertAll(Release, i => (Release)i.Clone());
            obj.ReleaseNumber = ReleaseNumber;
            if (Rom is not null)
                obj.Rom = Array.ConvertAll(Rom, i => (Rom)i.Clone());
            obj.RomOf = RomOf;
            obj.Rotation = Rotation;
            obj.Runnable = Runnable;
            if (Sample is not null)
                obj.Sample = Array.ConvertAll(Sample, i => (Sample)i.Clone());
            obj.SampleOf = SampleOf;
            obj.SaveType = SaveType;
            obj.Score = Score;
            if (SharedFeat is not null)
                obj.SharedFeat = Array.ConvertAll(SharedFeat, i => (SharedFeat)i.Clone());
            if (Slot is not null)
                obj.Slot = Array.ConvertAll(Slot, i => (Slot)i.Clone());
            if (SoftwareList is not null)
                obj.SoftwareList = Array.ConvertAll(SoftwareList, i => (SoftwareList)i.Clone());
            if (Sound is not null)
                obj.Sound = (Sound)Sound.Clone();
            obj.Source = Source;
            obj.SourceFile = SourceFile;
            obj.SourceRom = SourceRom;
            obj.Status = Status;
            obj.Subgenre = Subgenre;
            obj.Supported = Supported;
            obj.System = System;
            obj.Tags = Tags;
            obj.TitleID = TitleID;
            obj.Url = Url;
            if (Video is not null)
                obj.Video = Array.ConvertAll(Video, i => (Video)i.Clone());
            obj.Year = Year;

            return obj;
        }

        /// <inheritdoc/>
        public bool Equals(Machine? other)
        {
            // Null never matches
            if (other is null)
                return false;

            // Properties
            if ((Board is null) ^ (other.Board is null))
                return false;
            else if (Board is not null && !Board.Equals(other.Board, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Buttons is null) ^ (other.Buttons is null))
                return false;
            else if (Buttons is not null && !Buttons.Equals(other.Buttons, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((CloneOf is null) ^ (other.CloneOf is null))
                return false;
            else if (CloneOf is not null && !CloneOf.Equals(other.CloneOf, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((CloneOfId is null) ^ (other.CloneOfId is null))
                return false;
            else if (CloneOfId is not null && !CloneOfId.Equals(other.CloneOfId, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Company is null) ^ (other.Company is null))
                return false;
            else if (Company is not null && !Company.Equals(other.Company, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Control is null) ^ (other.Control is null))
                return false;
            else if (Control is not null && !Control.Equals(other.Control, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((CRC is null) ^ (other.CRC is null))
                return false;
            else if (CRC is not null && !CRC.Equals(other.CRC, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Country is null) ^ (other.Country is null))
                return false;
            else if (Country is not null && !Country.Equals(other.Country, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Description is null) ^ (other.Description is null))
                return false;
            else if (Description is not null && !Description.Equals(other.Description, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Developer is null) ^ (other.Developer is null))
                return false;
            else if (Developer is not null && !Developer.Equals(other.Developer, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((DirName is null) ^ (other.DirName is null))
                return false;
            else if (DirName is not null && !DirName.Equals(other.DirName, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((DisplayCount is null) ^ (other.DisplayCount is null))
                return false;
            else if (DisplayCount is not null && !DisplayCount.Equals(other.DisplayCount, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((DisplayType is null) ^ (other.DisplayType is null))
                return false;
            else if (DisplayType is not null && !DisplayType.Equals(other.DisplayType, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((DuplicateID is null) ^ (other.DuplicateID is null))
                return false;
            else if (DuplicateID is not null && !DuplicateID.Equals(other.DuplicateID, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Emulator is null) ^ (other.Emulator is null))
                return false;
            else if (Emulator is not null && !Emulator.Equals(other.Emulator, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Enabled is null) ^ (other.Enabled is null))
                return false;
            else if (Enabled is not null && !Enabled.Equals(other.Enabled, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Extra is null) ^ (other.Extra is null))
                return false;
            else if (Extra is not null && !Extra.Equals(other.Extra, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Favorite is null) ^ (other.Favorite is null))
                return false;
            else if (Favorite is not null && !Favorite.Equals(other.Favorite, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((GenMSXID is null) ^ (other.GenMSXID is null))
                return false;
            else if (GenMSXID is not null && !GenMSXID.Equals(other.GenMSXID, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Genre is null) ^ (other.Genre is null))
                return false;
            else if (Genre is not null && !Genre.Equals(other.Genre, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Hash is null) ^ (other.Hash is null))
                return false;
            else if (Hash is not null && !Hash.Equals(other.Hash, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((History is null) ^ (other.History is null))
                return false;
            else if (History is not null && !History.Equals(other.History, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Id is null) ^ (other.Id is null))
                return false;
            else if (Id is not null && !Id.Equals(other.Id, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Im1CRC is null) ^ (other.Im1CRC is null))
                return false;
            else if (Im1CRC is not null && !Im1CRC.Equals(other.Im1CRC, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Im2CRC is null) ^ (other.Im2CRC is null))
                return false;
            else if (Im2CRC is not null && !Im2CRC.Equals(other.Im2CRC, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((ImageNumber is null) ^ (other.ImageNumber is null))
                return false;
            else if (ImageNumber is not null && !ImageNumber.Equals(other.ImageNumber, StringComparison.OrdinalIgnoreCase))
                return false;

            if (IsBios != other.IsBios)
                return false;

            if (IsDevice != other.IsDevice)
                return false;

            if (IsMechanical != other.IsMechanical)
                return false;

            if ((Language is null) ^ (other.Language is null))
                return false;
            else if (Language is not null && !Language.Equals(other.Language, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Location is null) ^ (other.Location is null))
                return false;
            else if (Location is not null && !Location.Equals(other.Location, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Manufacturer is null) ^ (other.Manufacturer is null))
                return false;
            else if (Manufacturer is not null && !Manufacturer.Equals(other.Manufacturer, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Notes is null) ^ (other.Notes is null))
                return false;
            else if (Notes is not null && !Notes.Equals(other.Notes, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((PlayedCount is null) ^ (other.PlayedCount is null))
                return false;
            else if (PlayedCount is not null && !PlayedCount.Equals(other.PlayedCount, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((PlayedTime is null) ^ (other.PlayedTime is null))
                return false;
            else if (PlayedTime is not null && !PlayedTime.Equals(other.PlayedTime, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Players is null) ^ (other.Players is null))
                return false;
            else if (Players is not null && !Players.Equals(other.Players, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Publisher is null) ^ (other.Publisher is null))
                return false;
            else if (Publisher is not null && !Publisher.Equals(other.Publisher, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Ratings is null) ^ (other.Ratings is null))
                return false;
            else if (Ratings is not null && !Ratings.Equals(other.Ratings, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((RebuildTo is null) ^ (other.RebuildTo is null))
                return false;
            else if (RebuildTo is not null && !RebuildTo.Equals(other.RebuildTo, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((RelatedTo is null) ^ (other.RelatedTo is null))
                return false;
            else if (RelatedTo is not null && !RelatedTo.Equals(other.RelatedTo, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((ReleaseNumber is null) ^ (other.ReleaseNumber is null))
                return false;
            else if (ReleaseNumber is not null && !ReleaseNumber.Equals(other.ReleaseNumber, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((RomOf is null) ^ (other.RomOf is null))
                return false;
            else if (RomOf is not null && !RomOf.Equals(other.RomOf, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Rotation is null) ^ (other.Rotation is null))
                return false;
            else if (Rotation is not null && !Rotation.Equals(other.Rotation, StringComparison.OrdinalIgnoreCase))
                return false;

            if (Runnable != other.Runnable)
                return false;

            if ((SampleOf is null) ^ (other.SampleOf is null))
                return false;
            else if (SampleOf is not null && !SampleOf.Equals(other.SampleOf, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((SaveType is null) ^ (other.SaveType is null))
                return false;
            else if (SaveType is not null && !SaveType.Equals(other.SaveType, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Score is null) ^ (other.Score is null))
                return false;
            else if (Score is not null && !Score.Equals(other.Score, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Source is null) ^ (other.Source is null))
                return false;
            else if (Source is not null && !Source.Equals(other.Source, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((SourceFile is null) ^ (other.SourceFile is null))
                return false;
            else if (SourceFile is not null && !SourceFile.Equals(other.SourceFile, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((SourceRom is null) ^ (other.SourceRom is null))
                return false;
            else if (SourceRom is not null && !SourceRom.Equals(other.SourceRom, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Status is null) ^ (other.Status is null))
                return false;
            else if (Status is not null && !Status.Equals(other.Status, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Subgenre is null) ^ (other.Subgenre is null))
                return false;
            else if (Subgenre is not null && !Subgenre.Equals(other.Subgenre, StringComparison.OrdinalIgnoreCase))
                return false;

            if (Supported != other.Supported)
                return false;

            if ((System is null) ^ (other.System is null))
                return false;
            else if (System is not null && !System.Equals(other.System, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Tags is null) ^ (other.Tags is null))
                return false;
            else if (Tags is not null && !Tags.Equals(other.Tags, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((TitleID is null) ^ (other.TitleID is null))
                return false;
            else if (TitleID is not null && !TitleID.Equals(other.TitleID, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Url is null) ^ (other.Url is null))
                return false;
            else if (Url is not null && !Url.Equals(other.Url, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Year is null) ^ (other.Year is null))
                return false;
            else if (Year is not null && !Year.Equals(other.Year, StringComparison.OrdinalIgnoreCase))
                return false;

            // Sub-items
            if ((Driver is null) ^ (other.Driver is null))
                return false;
            else if (Driver is not null && other.Driver is not null && Driver.Equals(other.Driver))
                return false;

            if ((Input is null) ^ (other.Input is null))
                return false;
            else if (Input is not null && other.Input is not null && Input.Equals(other.Input))
                return false;

            if ((Sound is null) ^ (other.Sound is null))
                return false;
            else if (Sound is not null && other.Sound is not null && Sound.Equals(other.Sound))
                return false;

            // TODO: Figure out how to properly check arrays

            return true;
        }
    }
}

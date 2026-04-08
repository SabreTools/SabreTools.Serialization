
using System;

namespace SabreTools.Data.Models.Metadata
{
    /// <summary>
    /// Format-agnostic representation of metadata header data
    /// </summary>
    public class Header : ICloneable, IEquatable<Header>
    {
        #region Properties

        public string? Author { get; set; }

        /// <remarks>(none|split|merged|nonmerged|fullmerged|device|full) "split"</remarks>
        public MergingFlag BiosMode { get; set; }

        public string? Build { get; set; }

        /// TODO: This needs an internal model OR mapping to fields
        public OfflineList.CanOpen? CanOpen { get; set; }

        public string? Category { get; set; }

        public string? Comment { get; set; }

        public string? Date { get; set; }

        public string? DatVersion { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? Debug { get; set; }

        public string? Description { get; set; }

        public string? Email { get; set; }

        public string? EmulatorVersion { get; set; }

        public string? FileName { get; set; }

        /// <remarks>(none|split|merged|nonmerged|fullmerged|device|full) "split"</remarks>
        public MergingFlag ForceMerging { get; set; }

        /// <remarks>(obsolete|required|ignore) "obsolete"</remarks>
        public NodumpFlag ForceNodump { get; set; }

        /// <remarks>(zip|unzip) "zip"</remarks>
        public PackingFlag ForcePacking { get; set; }

        /// <remarks>(yes|no) "yes"</remarks>
        public bool? ForceZipping { get; set; }

        public string[]? HeaderRow { get; set; }

        public string? HeaderSkipper { get; set; }

        public string? Homepage { get; set; }

        public string? Id { get; set; }

        /// TODO: This needs an internal model OR mapping to fields
        public OfflineList.Images? Images { get; set; }

        public string? ImFolder { get; set; }

        /// TODO: This needs an internal model OR mapping to fields
        public OfflineList.Infos? Infos { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? LockBiosMode { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? LockRomMode { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? LockSampleMode { get; set; }

        public string? MameConfig { get; set; }

        public string? Name { get; set; }

        /// TODO: This needs an internal model OR mapping to fields
        public OfflineList.NewDat? NewDat { get; set; }

        public string? Notes { get; set; }

        public string? Plugin { get; set; }

        public string? RefName { get; set; }

        /// <remarks>(none|split|merged|nonmerged|fullmerged|device|full) "split"</remarks>
        public MergingFlag RomMode { get; set; }

        public string? RomTitle { get; set; }

        public string? RootDir { get; set; }

        /// <remarks>(none|split|merged|nonmerged|fullmerged|device|full) "split"</remarks>
        public MergingFlag SampleMode { get; set; }

        public string? SchemaLocation { get; set; }

        public string? ScreenshotsHeight { get; set; }

        public string? ScreenshotsWidth { get; set; }

        /// TODO: This needs an internal model OR mapping to fields
        public OfflineList.Search? Search { get; set; }

        public string? System { get; set; }

        public string? Timestamp { get; set; }

        public string? Type { get; set; }

        public string? Url { get; set; }

        public string? Version { get; set; }

        #endregion

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new Header();

            obj.Author = Author;
            obj.BiosMode = BiosMode;
            obj.Build = Build;
            obj.CanOpen = CanOpen;
            obj.Category = Category;
            obj.Comment = Comment;
            obj.Date = Date;
            obj.DatVersion = DatVersion;
            obj.Debug = Debug;
            obj.Description = Description;
            obj.Email = Email;
            obj.EmulatorVersion = EmulatorVersion;
            obj.FileName = FileName;
            obj.ForceMerging = ForceMerging;
            obj.ForceNodump = ForceNodump;
            obj.ForcePacking = ForcePacking;
            obj.ForceZipping = ForceZipping;
            obj.HeaderRow = HeaderRow;
            obj.HeaderSkipper = HeaderSkipper;
            obj.Homepage = Homepage;
            obj.Id = Id;
            obj.Images = Images;
            obj.ImFolder = ImFolder;
            obj.Infos = Infos;
            obj.LockBiosMode = LockBiosMode;
            obj.LockRomMode = LockRomMode;
            obj.LockSampleMode = LockSampleMode;
            obj.MameConfig = MameConfig;
            obj.Name = Name;
            obj.NewDat = NewDat;
            obj.Notes = Notes;
            obj.Plugin = Plugin;
            obj.RefName = RefName;
            obj.RomMode = RomMode;
            obj.RomTitle = RomTitle;
            obj.RootDir = RootDir;
            obj.SampleMode = SampleMode;
            obj.SchemaLocation = SchemaLocation;
            obj.ScreenshotsHeight = ScreenshotsHeight;
            obj.ScreenshotsWidth = ScreenshotsWidth;
            obj.Search = Search;
            obj.System = System;
            obj.Timestamp = Timestamp;
            obj.Type = Type;
            obj.Url = Url;
            obj.Version = Version;

            return obj;
        }

        /// <inheritdoc/>
        public bool Equals(Header? other)
        {
            // Null never matches
            if (other is null)
                return false;

            // Properties
            if ((Author is null) ^ (other.Author is null))
                return false;
            else if (Author is not null && !Author.Equals(other.Author, StringComparison.OrdinalIgnoreCase))
                return false;

            if (BiosMode != other.BiosMode)
                return false;

            if ((Build is null) ^ (other.Build is null))
                return false;
            else if (Build is not null && !Build.Equals(other.Build, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Category is null) ^ (other.Category is null))
                return false;
            else if (Category is not null && !Category.Equals(other.Category, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Comment is null) ^ (other.Comment is null))
                return false;
            else if (Comment is not null && !Comment.Equals(other.Comment, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Date is null) ^ (other.Date is null))
                return false;
            else if (Date is not null && !Date.Equals(other.Date, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((DatVersion is null) ^ (other.DatVersion is null))
                return false;
            else if (DatVersion is not null && !DatVersion.Equals(other.DatVersion, StringComparison.OrdinalIgnoreCase))
                return false;

            if (Debug != other.Debug)
                return false;

            if ((Description is null) ^ (other.Description is null))
                return false;
            else if (Description is not null && !Description.Equals(other.Description, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Email is null) ^ (other.Email is null))
                return false;
            else if (Email is not null && !Email.Equals(other.Email, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((EmulatorVersion is null) ^ (other.EmulatorVersion is null))
                return false;
            else if (EmulatorVersion is not null && !EmulatorVersion.Equals(other.EmulatorVersion, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((FileName is null) ^ (other.FileName is null))
                return false;
            else if (FileName is not null && !FileName.Equals(other.FileName, StringComparison.OrdinalIgnoreCase))
                return false;

            if (ForceMerging != other.ForceMerging)
                return false;

            if (ForceNodump != other.ForceNodump)
                return false;

            if (ForcePacking != other.ForcePacking)
                return false;

            if (ForceZipping != other.ForceZipping)
                return false;

            if ((HeaderSkipper is null) ^ (other.HeaderSkipper is null))
                return false;
            else if (HeaderSkipper is not null && !HeaderSkipper.Equals(other.HeaderSkipper, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Homepage is null) ^ (other.Homepage is null))
                return false;
            else if (Homepage is not null && !Homepage.Equals(other.Homepage, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Id is null) ^ (other.Id is null))
                return false;
            else if (Id is not null && !Id.Equals(other.Id, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((ImFolder is null) ^ (other.ImFolder is null))
                return false;
            else if (ImFolder is not null && !ImFolder.Equals(other.ImFolder, StringComparison.OrdinalIgnoreCase))
                return false;

            if (LockBiosMode != other.ForceZipping)
                return false;

            if (LockRomMode != other.LockRomMode)
                return false;

            if (LockSampleMode != other.LockSampleMode)
                return false;

            if ((MameConfig is null) ^ (other.MameConfig is null))
                return false;
            else if (MameConfig is not null && !MameConfig.Equals(other.MameConfig, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Name is null) ^ (other.Name is null))
                return false;
            else if (Name is not null && !Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Notes is null) ^ (other.Notes is null))
                return false;
            else if (Notes is not null && !Notes.Equals(other.Notes, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Plugin is null) ^ (other.Plugin is null))
                return false;
            else if (Plugin is not null && !Plugin.Equals(other.Plugin, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((RefName is null) ^ (other.RefName is null))
                return false;
            else if (RefName is not null && !RefName.Equals(other.RefName, StringComparison.OrdinalIgnoreCase))
                return false;

            if (RomMode != other.RomMode)
                return false;

            if ((RomTitle is null) ^ (other.RomTitle is null))
                return false;
            else if (RomTitle is not null && !RomTitle.Equals(other.RomTitle, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((RootDir is null) ^ (other.RootDir is null))
                return false;
            else if (RootDir is not null && !RootDir.Equals(other.RootDir, StringComparison.OrdinalIgnoreCase))
                return false;

            if (SampleMode != other.SampleMode)
                return false;

            if ((SchemaLocation is null) ^ (other.SchemaLocation is null))
                return false;
            else if (SchemaLocation is not null && !SchemaLocation.Equals(other.SchemaLocation, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((ScreenshotsHeight is null) ^ (other.ScreenshotsHeight is null))
                return false;
            else if (ScreenshotsHeight is not null && !ScreenshotsHeight.Equals(other.ScreenshotsHeight, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((ScreenshotsWidth is null) ^ (other.ScreenshotsWidth is null))
                return false;
            else if (ScreenshotsWidth is not null && !ScreenshotsWidth.Equals(other.ScreenshotsWidth, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((System is null) ^ (other.System is null))
                return false;
            else if (System is not null && !System.Equals(other.System, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Timestamp is null) ^ (other.Timestamp is null))
                return false;
            else if (Timestamp is not null && !Timestamp.Equals(other.Timestamp, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Type is null) ^ (other.Type is null))
                return false;
            else if (Type is not null && !Type.Equals(other.Type, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Url is null) ^ (other.Url is null))
                return false;
            else if (Url is not null && !Url.Equals(other.Url, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Version is null) ^ (other.Version is null))
                return false;
            else if (Version is not null && !Version.Equals(other.Version, StringComparison.OrdinalIgnoreCase))
                return false;

            // Sub-items
            // Header.CanOpen is intentionally skipped
            // Header.Images is intentionally skipped
            // Header.Infos is intentionally skipped
            // Header.NewDat is intentionally skipped
            // Header.Search is intentionally skipped

            // TODO: Figure out how to properly check arrays

            return true;
        }
    }
}

using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("archive"), XmlRoot("archive")]
    public class Archive : DatItem
    {
        #region Keys

        /// <remarks>string, No-Intro DB extension</remarks>
        public const string NumberKey = "number";

        /// <remarks>string, No-Intro DB extension</remarks>
        public const string CloneKey = "clone";

        /// <remarks>string, No-Intro DB extension</remarks>
        public const string RegParentKey = "regparent";

        /// <remarks>string, No-Intro DB extension</remarks>
        public const string MergeOfKey = "mergeof";

        /// <remarks>string, No-Intro DB extension</remarks>
        public const string MergeNameKey = "mergename";

        /// <remarks>string</remarks>
        public const string NameKey = "name";

        /// <remarks>string, No-Intro DB extension</remarks>
        public const string NameAltKey = "name_alt";

        /// <remarks>string, No-Intro DB extension</remarks>
        public const string RegionKey = "region";

        /// <remarks>string, No-Intro DB extension</remarks>
        public const string LanguagesKey = "languages";

        /// <remarks>byte, No-Intro DB extension</remarks>
        public const string ShowLangKey = "showlang";

        /// <remarks>string, No-Intro DB extension</remarks>
        public const string LangCheckedKey = "langchecked";

        /// <remarks>string, No-Intro DB extension</remarks>
        public const string Version1Key = "version1";

        /// <remarks>string, No-Intro DB extension</remarks>
        public const string Version2Key = "version2";

        /// <remarks>string, No-Intro DB extension</remarks>
        public const string DevStatusKey = "devstatus";

        /// <remarks>string, No-Intro DB extension</remarks>
        public const string AdditionalKey = "additional";

        /// <remarks>string, No-Intro DB extension</remarks>
        public const string Special1Key = "special1";

        /// <remarks>string, No-Intro DB extension</remarks>
        public const string Special2Key = "special2";

        /// <remarks>byte, No-Intro DB extension</remarks>
        public const string AltKey = "alt";

        /// <remarks>string, No-Intro DB extension</remarks>
        public const string GameId1Key = "gameid1";

        /// <remarks>string, No-Intro DB extension</remarks>
        public const string GameId2Key = "gameid2";

        /// <remarks>string, No-Intro DB extension</remarks>
        public const string DescriptionKey = "description";

        /// <remarks>byte, No-Intro DB extension</remarks>
        public const string BiosKey = "bios";

        /// <remarks>byte, No-Intro DB extension</remarks>
        public const string LicensedKey = "licensed";

        /// <remarks>byte, No-Intro DB extension</remarks>
        public const string PirateKey = "pirate";

        /// <remarks>byte, No-Intro DB extension</remarks>
        public const string PhysicalKey = "physical";

        /// <remarks>byte, No-Intro DB extension</remarks>
        public const string CompleteKey = "complete";

        /// <remarks>byte, No-Intro DB extension</remarks>
        public const string AdultKey = "adult";

        /// <remarks>byte, No-Intro DB extension</remarks>
        public const string DatKey = "dat";

        /// <remarks>byte, No-Intro DB extension</remarks>
        public const string ListedKey = "listed";

        /// <remarks>byte, No-Intro DB extension</remarks>
        public const string PrivateKey = "private";

        /// <remarks>string, No-Intro DB extension</remarks>
        public const string StickyNoteKey = "stickynote";

        /// <remarks>string, No-Intro DB extension</remarks>
        public const string DatterNoteKey = "datternote";

        /// <remarks>string, No-Intro DB extension</remarks>
        public const string CategoriesKey = "categories";

        #endregion

        public Archive() => ItemType = ItemType.Archive;
    }
}

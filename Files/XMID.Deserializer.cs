using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class XMID : IFileSerializer<Models.Xbox.XMID>
    {
        /// <inheritdoc/>
        /// <remarks>This treats the input path like a parseable string</remarks>
#if NET48
        public Models.Xbox.XMID Deserialize(string path)
#else
        public Models.Xbox.XMID? Deserialize(string? path)
#endif
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;

            string xmid = path.TrimEnd('\0');
            if (string.IsNullOrWhiteSpace(xmid))
                return null;

            return ParseXMID(xmid);
        }

        /// <summary>
        /// Parse an XGD2/3 XMID string
        /// </summary>
        /// <param name="xmidString">XMID string to attempt to parse</param>
        /// <returns>Filled XMID on success, null on error</returns>
#if NET48
        private static Models.Xbox.XMID ParseXMID(string xmidString)
#else
        private static Models.Xbox.XMID? ParseXMID(string? xmidString)
#endif
        {
            if (xmidString == null || xmidString.Length != 8)
                return null;

            var xmid = new Models.Xbox.XMID();

            xmid.PublisherIdentifier = xmidString.Substring(0, 2);
            if (string.IsNullOrEmpty(PublisherName(xmid)))
                return null;

            xmid.GameID = xmidString.Substring(2, 3);
            xmid.VersionNumber = xmidString.Substring(5, 2);
            xmid.RegionIdentifier = xmidString[7];
            if (InternalRegion(xmid) == null)
                return null;

            return xmid;
        }

        #region Helpers

        /// <summary>
        /// Human-readable name derived from the publisher identifier
        /// </summary>
#if NET48
        public static string PublisherName(Models.Xbox.XMID xmid) => GetPublisher(xmid.PublisherIdentifier);
#else
        public static string? PublisherName(Models.Xbox.XMID xmid) => GetPublisher(xmid.PublisherIdentifier);
#endif

        /// <summary>
        /// Internally represented region
        /// </summary>
#if NET48
        public static string InternalRegion(Models.Xbox.XMID xmid) => GetRegion(xmid.RegionIdentifier);
#else
        public static string? InternalRegion(Models.Xbox.XMID xmid) => GetRegion(xmid.RegionIdentifier);
#endif

        /// <summary>
        /// Get the full name of the publisher from the 2-character identifier
        /// </summary>
        /// <param name="publisherIdentifier">Case-sensitive 2-character identifier</param>
        /// <returns>Publisher name, if possible</returns>
        /// <see cref="https://xboxdevwiki.net/Xbe#Title_ID"/>
#if NET48
        private static string GetPublisher(string publisherIdentifier)
#else
        private static string? GetPublisher(string? publisherIdentifier)
#endif
        {
            switch (publisherIdentifier)
            {
                case "AC": return "Acclaim Entertainment";
                case "AH": return "ARUSH Entertainment";
                case "AQ": return "Aqua System";
                case "AS": return "ASK";
                case "AT": return "Atlus";
                case "AV": return "Activision";
                case "AY": return "Aspyr Media";
                case "BA": return "Bandai";
                case "BL": return "Black Box";
                case "BM": return "BAM! Entertainment";
                case "BR": return "Broccoli Co.";
                case "BS": return "Bethesda Softworks";
                case "BU": return "Bunkasha Co.";
                case "BV": return "Buena Vista Games";
                case "BW": return "BBC Multimedia";
                case "BZ": return "Blizzard";
                case "CC": return "Capcom";
                case "CK": return "Kemco Corporation"; // TODO: Confirm
                case "CM": return "Codemasters";
                case "CV": return "Crave Entertainment";
                case "DC": return "DreamCatcher Interactive";
                case "DX": return "Davilex";
                case "EA": return "Electronic Arts (EA)";
                case "EC": return "Encore inc";
                case "EL": return "Enlight Software";
                case "EM": return "Empire Interactive";
                case "ES": return "Eidos Interactive";
                case "FI": return "Fox Interactive";
                case "FS": return "From Software";
                case "GE": return "Genki Co.";
                case "GV": return "Groove Games";
                case "HE": return "Tru Blu (Entertainment division of Home Entertainment Suppliers)";
                case "HP": return "Hip games";
                case "HU": return "Hudson Soft";
                case "HW": return "Highwaystar";
                case "IA": return "Mad Catz Interactive";
                case "IF": return "Idea Factory";
                case "IG": return "Infogrames";
                case "IL": return "Interlex Corporation";
                case "IM": return "Imagine Media";
                case "IO": return "Ignition Entertainment";
                case "IP": return "Interplay Entertainment";
                case "IX": return "InXile Entertainment"; // TODO: Confirm
                case "JA": return "Jaleco";
                case "JW": return "JoWooD";
                case "KB": return "Kemco"; // TODO: Confirm
                case "KI": return "Kids Station Inc."; // TODO: Confirm
                case "KN": return "Konami";
                case "KO": return "KOEI";
                case "KU": return "Kobi and / or GAE (formerly Global A Entertainment)"; // TODO: Confirm
                case "LA": return "LucasArts";
                case "LS": return "Black Bean Games (publishing arm of Leader S.p.A.)";
                case "MD": return "Metro3D";
                case "ME": return "Medix";
                case "MI": return "Microïds";
                case "MJ": return "Majesco Entertainment";
                case "MM": return "Myelin Media";
                case "MP": return "MediaQuest"; // TODO: Confirm
                case "MS": return "Microsoft Game Studios";
                case "MW": return "Midway Games";
                case "MX": return "Empire Interactive"; // TODO: Confirm
                case "NK": return "NewKidCo";
                case "NL": return "NovaLogic";
                case "NM": return "Namco";
                case "OX": return "Oxygen Interactive";
                case "PC": return "Playlogic Entertainment";
                case "PL": return "Phantagram Co., Ltd.";
                case "RA": return "Rage";
                case "SA": return "Sammy";
                case "SC": return "SCi Games";
                case "SE": return "SEGA";
                case "SN": return "SNK";
                case "SS": return "Simon & Schuster";
                case "SU": return "Success Corporation";
                case "SW": return "Swing! Deutschland";
                case "TA": return "Takara";
                case "TC": return "Tecmo";
                case "TD": return "The 3DO Company (or just 3DO)";
                case "TK": return "Takuyo";
                case "TM": return "TDK Mediactive";
                case "TQ": return "THQ";
                case "TS": return "Titus Interactive";
                case "TT": return "Take-Two Interactive Software";
                case "US": return "Ubisoft";
                case "VC": return "Victor Interactive Software";
                case "VN": return "Vivendi Universal (just took Interplays publishing rights)"; // TODO: Confirm
                case "VU": return "Vivendi Universal Games";
                case "VV": return "Vivendi Universal Games"; // TODO: Confirm
                case "WE": return "Wanadoo Edition";
                case "WR": return "Warner Bros. Interactive Entertainment"; // TODO: Confirm
                case "XI": return "XPEC Entertainment and Idea Factory";
                case "XK": return "Xbox kiosk disk?"; // TODO: Confirm
                case "XL": return "Xbox special bundled or live demo disk?"; // TODO: Confirm
                case "XM": return "Evolved Games"; // TODO: Confirm
                case "XP": return "XPEC Entertainment";
                case "XR": return "Panorama";
                case "YB": return "YBM Sisa (South-Korea)";
                case "ZD": return "Zushi Games (formerly Zoo Digital Publishing)";
                default: return null;
            }
        }

        /// <summary>
        /// Determine the region based on the XGD serial character
        /// </summary>
        /// <param name="region">Character denoting the region</param>
        /// <returns>Region, if possible</returns>
#if NET48
        private static string GetRegion(char region)
#else
        private static string? GetRegion(char region)
#endif
        {
            switch (region)
            {
                case 'W': return "World";
                case 'A': return "USA";
                case 'J': return "Japan / Asia";
                case 'E': return "Europe";
                case 'K': return "USA / Japan";
                case 'L': return "USA / Europe";
                case 'H': return "Japan / Europe";
                default: return null;
            }
        }

        #endregion
    }
}
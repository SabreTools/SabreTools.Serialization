using System.Collections.Generic;

namespace SabreTools.Data.Models.Xbox
{
    /// <see href="https://xboxdevwiki.net/Xbe"/>
    /// <see href="http://wiki.redump.org/index.php?title=Xbox_Title_IDs"/>
    /// <see href="https://dbox.tools/publishers/"/>
    public static class Constants
    {
        /// <summary>
        /// Mapping of all Xbox 360 media subtypes to long names
        /// </summary>
        public static readonly Dictionary<char, string> MediaSubtypes = new()
        {
            { 'F', "XGD3" },
            { 'X', "XGD2" },
            { 'Z', "Games on Demand / Marketplace Demo" },
        };

        /// <summary>
        /// Mapping of all publisher 2-letter codes to long names
        /// </summary>
        public static readonly Dictionary<string, string> Publishers = new()
        {
            { "AB", "Ambush Reality" },
            { "AC", "Acclaim Entertainment" },
            { "AD", "Andamiro USA Corp." },
            { "AH", "Arush Entertainment" },
            { "AK", "Artdink" },
            { "AP", "Aquaplus" },
            { "AQ", "Aqua System" },
            { "AS", "ASK" },
            { "AT", "Atlus" },
            { "AV", "Activision" },
            { "AW", "Arc System Works" },
            { "AX", "Aksys Games" },
            { "AY", "Aspyr Media" },

            { "BA", "Bandai" },
            { "BB", "BigBen" },
            { "BD", "Bravado" },
            { "BE", "Blueside Inc." },
            { "BF", "Blind Ferret Entertainment" },
            { "BG", "BradyGames" },
            { "BH", "Blackhole" },
            { "BL", "Black Box" },
            { "BM", "BAM! Entertainment" },
            { "BR", "Broccoli Co." },
            { "BS", "Bethesda Softworks" },
            { "BT", "Brash Entertainment" },
            { "BU", "Bunkasha Co." },
            { "BV", "Buena Vista Games" },
            { "BW", "BBC Multimedia" },
            { "BZ", "Blizzard" },

            { "CC", "Capcom" },
            { "CK", "Kemco Corporation" }, // TODO: Confirm
            { "CM", "Codemasters" },
            { "CT", "CTO" },
            { "CV", "Crave Entertainment" },

            { "DC", "DreamCatcher Interactive" },
            { "DE", "Destineer" },
            { "DX", "Davilex" },

            { "EA", "Electronic Arts" },
            { "EC", "Encore Software" },
            { "EF", "E-Frontier" },
            { "EL", "Enlight Software" },
            { "EM", "Empire Interactive" },
            { "ES", "Eidos Interactive" },
            { "EV", "Evolved Games" },

            { "FE", "Focus Entertainment (formerly Focus Home Interactive)" }, // TODO: Confirm
            { "FI", "Fox Interactive" },
            { "FL", "Fluent Entertainment" },
            { "FO", "505 Games" },
            { "FS", "From Software" },

            { "GE", "Genki Co." },
            { "GF", "Gameloft" },
            { "GV", "Groove Games" },

            { "HE", "Tru Blu (Entertainment division of Home Entertainment Suppliers)" },
            { "HP", "Hip Games" },
            { "HU", "Hudson Soft" },
            { "HW", "Highwaystar" },

            { "IA", "Mad Catz Interactive" }, // TODO: Confirm
            { "IF", "Idea Factory" },
            { "IG", "Infogrames" },
            { "IL", "Interlex Corporation" },
            { "IM", "Imagine Media" },
            { "IO", "Ignition Entertainment" },
            { "IP", "Interplay Entertainment" },
            { "IX", "InXile Entertainment" },

            { "JA", "Jaleco Entertainment" },
            { "JW", "JoWooD Entertainment" },

            { "KA", "Konami Osaka / Major A" },
            { "KB", "Kemco" }, // TODO: Confirm
            { "KI", "Kids Station Inc." }, // TODO: Confirm
            { "KN", "Konami" },
            { "KO", "Koei" },
            { "KT", "Konami Tokyo" },
            { "KU", "Kobi and/or GAE (formerly Global A Entertainment)" }, // TODO: Confirm
            { "KY", "Kalypso" },

            { "LA", "LucasArts" },
            { "LS", "Black Bean Games (publishing arm of Leader S.p.A.)" }, // TODO: Confirm

            { "MD", "Metro3D" },
            { "ME", "Medix" },
            { "MI", "Microïds" }, // TODO: Confirm
            { "MJ", "Majesco Entertainment" },
            { "MM", "Myelin Media" },
            { "MP", "MediaQuest" }, // TODO: Confirm
            { "MS", "Microsoft Game Studios" },
            { "MW", "Midway Games" },
            { "MX", "Empire Interactive" }, // TODO: Confirm

            { "NK", "NewKidCo" },
            { "NL", "NovaLogic" },
            { "NM", "Namco" },

            { "OG", "O-Games" },
            { "OX", "Oxygen Interactive" },

            { "PC", "Playlogic Entertainment" },
            { "PL", "Phantagram Co., Ltd. / Playlogic Entertainment" }, // TODO: Confirm

            { "RA", "Rage" },

            { "SA", "Sammy" },
            { "SC", "SCi Games" },
            { "SE", "Sega" },
            { "SN", "SNK" },
            { "SP", "Southpeak Games" },
            { "SQ", "Square Enix" },
            { "SS", "Simon & Schuster" },
            { "ST", "Studio Nine" },
            { "SU", "Success Corporation" },
            { "SW", "Swing! Deutschland" },

            { "TA", "Takara" },
            { "TC", "Tecmo" },
            { "TD", "The 3DO Company" },
            { "TK", "Takuyo" },
            { "TM", "TDK Mediactive" },
            { "TQ", "THQ" },
            { "TS", "Titus Interactive" },
            { "TT", "Take-Two Interactive Software" },

            { "US", "Ubisoft" },

            { "VC", "Victor Interactive Software" },
            { "VG", "Valcon Games" },
            { "VN", "Vivendi Universal Games (Former Interplay)" }, // TODO: Confirm
            { "VU", "Vivendi Universal Games" },
            { "VV", "Vicarious Visions" },

            { "WE", "Wanadoo Edition" },
            { "WR", "Warner Bros. Interactive Entertainment" },

            { "XA", "Xbox Live Arcade" },
            { "XI", "Xicat Interactive / XPEC Entertainment and Idea Factory" }, // TODO: Confirm
            { "XK", "Xbox Kiosk" },
            { "XL", "Xbox Live" },
            { "XM", "Evolved Games" }, // TODO: Confirm
            { "XP", "XPEC Entertainment" },
            { "XR", "Panorama" }, // TODO: Confirm

            { "YB", "YBM Sisa (South-Korea)" },

            { "ZD", "Zushi Games (formerly Zoo Digital Publishing)" },

            { "" + (char)0x00 + (char)0x01, "Microsoft Downloadable Content" },
            { "" + (char)0x05 + (char)0x04, "Zuma's Revenge" },
            { "" + (char)0xAA + (char)0xAA, "Arcania - Gothic 4" },
            { "" + (char)0xFF + (char)0xED, "Microsoft Downloadable Content" },
            { "" + (char)0xFF + (char)0xFD, "Microsoft Downloadable Content" },
            { "" + (char)0xFF + (char)0xFE, "Microsoft Downloadable Content" },
            { "" + (char)0xFF + (char)0xFF, "Microsoft Downloadable Content" },
        };

        /// <summary>
        /// Mapping of all region 1-letter codes to long names
        /// </summary>
        public static readonly Dictionary<char, string> Regions = new()
        {
            { 'A', "USA" },
            { 'E', "Europe" },
            { 'H', "Japan / Europe" },
            { 'J', "Japan / Asia" },
            { 'K', "USA / Japan" },
            { 'L', "USA / Europe" },
            { 'W', "World" },
        };
    }
}

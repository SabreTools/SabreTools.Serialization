using System;
using System.Text;

namespace SabreTools.Serialization.ASN1
{
    /// <summary>
    /// Methods related to Object Identifiers (OID) and OID-IRI formatting
    /// </summary>
    public static partial class ObjectIdentifier
    {
        /// <summary>
        /// Parse an OID in separated-value notation into OID-IRI notation
        /// </summary>
        /// <param name="values">List of values to check against</param>
        /// <param name="index">Current index into the list</param>
        /// <returns>OID-IRI formatted string, if possible</returns>
        /// <see href="http://www.oid-info.com/index.htm"/>
        public static string? ParseOIDToOIDIRINotation(ulong[]? values)
        {
            // If we have an invalid set of values, we can't do anything
            if (values == null || values.Length == 0)
                return null;

            // Set the initial index
            int index = 0;

            // Get a string builder for the path
            var nameBuilder = new StringBuilder();

            // Try to parse the standard value
            string? standard = ParseOIDToOIDIRINotation(values, ref index);
            if (standard == null)
                return null;

            // Add the standard value to the output
            nameBuilder.Append(standard);

            // If we have no more items
            if (index == values.Length)
                return nameBuilder.ToString();

            // Add trailing items as just values
            nameBuilder.Append("/");

            // Get the remaining values in a new array
            var remainingValues = new ulong[values.Length - index];
            Array.Copy(values, index, remainingValues, 0, remainingValues.Length);

            // Convert the values and append to the builder
            var stringValues = Array.ConvertAll(remainingValues, v => v.ToString());
            nameBuilder.Append(string.Join("/", stringValues));

            // Create and return the string
            return nameBuilder.ToString();
        }

        /// <summary>
        /// Parse an OID in separated-value notation into OID-IRI notation
        /// </summary>
        /// <param name="values">List of values to check against</param>
        /// <param name="index">Current index into the list</param>
        /// <returns>OID-IRI formatted string, if possible</returns>
        /// <see href="http://www.oid-info.com/index.htm"/>
        private static string? ParseOIDToOIDIRINotation(ulong[]? values, ref int index)
        {
            // If we have an invalid set of values, we can't do anything
            if (values == null || values.Length == 0)
                return null;

            // If we have an invalid index, we can't do anything
            if (index < 0 || index >= values.Length)
                return null;

            #region Start

            var oidPath = string.Empty;
            switch (values[index++])
            {
                case 0: goto oid_0;
                case 1: goto oid_1;
                case 2: goto oid_2;
                default: return oidPath;
            }

        #endregion

        // itu-t, ccitt, itu-r
        #region 0.*

        oid_0:

            oidPath += "/ITU-T";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_0_0;
                case 2: return $"{oidPath}/Administration";
                case 3: return $"{oidPath}/Network-Operator";
                case 4: return $"{oidPath}/Identified-Organization";
                case 5: return "/ITU-R/R-Recommendation";
                case 9: return $"{oidPath}/Data";
                default: return $"{oidPath}/{values[index - 1]}";
            }
            ;

        // recommendation
        #region 0.0.*

        oid_0_0:

            oidPath += "/Recommendation";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/A",
                2 => $"{oidPath}/B",
                3 => $"{oidPath}/C",
                4 => $"{oidPath}/D",
                5 => $"{oidPath}/E",
                6 => $"{oidPath}/F",
                7 => $"{oidPath}/G",
                8 => $"{oidPath}/H",
                9 => $"{oidPath}/I",
                10 => $"{oidPath}/J",
                11 => $"{oidPath}/K",
                12 => $"{oidPath}/L",
                13 => $"{oidPath}/M",
                14 => $"{oidPath}/N",
                15 => $"{oidPath}/O",
                16 => $"{oidPath}/P",
                17 => $"{oidPath}/Q",
                18 => $"{oidPath}/R",
                19 => $"{oidPath}/S",
                20 => $"{oidPath}/T",
                21 => $"{oidPath}/U",
                22 => $"{oidPath}/V",
                24 => $"{oidPath}/X",
                25 => $"{oidPath}/Y",
                26 => $"{oidPath}/Z",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // iso
        #region 1.*

        oid_1:

            oidPath += "/ISO";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: return $"{oidPath}/Standard";
                case 1: return $"{oidPath}/Registration-Authority";
                case 2: goto oid_1_2;
                case 3: return $"{oidPath}/Identified-Organization";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // member-body
        #region 1.2.*

        oid_1_2:

            oidPath += "/Member-Body";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                36 => $"{oidPath}/AU",
                40 => $"{oidPath}/AT",
                56 => $"{oidPath}/BE",
                124 => $"{oidPath}/CA",
                156 => $"{oidPath}/CN",
                203 => $"{oidPath}/CZ",
                208 => $"{oidPath}/DK",
                246 => $"{oidPath}/FI",
                250 => $"{oidPath}/FR",
                276 => $"{oidPath}/DE",
                300 => $"{oidPath}/GR",
                344 => $"{oidPath}/HK",
                372 => $"{oidPath}/IE",
                392 => $"{oidPath}/JP",
                398 => $"{oidPath}/KZ",
                410 => $"{oidPath}/KR",
                498 => $"{oidPath}/MD",
                528 => $"{oidPath}/NL",
                566 => $"{oidPath}/NG",
                578 => $"{oidPath}/NO",
                616 => $"{oidPath}/PL",
                643 => $"{oidPath}/RU",
                702 => $"{oidPath}/SG",
                752 => $"{oidPath}/SE",
                804 => $"{oidPath}/UA",
                826 => $"{oidPath}/GB",
                840 => $"{oidPath}/US",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // joint-iso-itu-t, joint-iso-ccitt
        #region 2.*

        oid_2:

            oidPath += "/Joint-ISO-ITU-T";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: return "/ASN.1";
                case 16: oidPath = string.Empty; goto oid_2_16;
                case 17: return $"{oidPath}/Registration-Procedures";
                case 23: return $"{oidPath}/International-Organizations";
                case 25: goto oid_2_25;
                case 27: return "/Tag-Based";
                case 28: return $"{oidPath}/ITS";
                case 41: return "/BIP";
                case 42: oidPath = string.Empty; goto oid_2_42;
                case 48: oidPath = string.Empty; goto oid_2_48;
                case 49: oidPath = string.Empty; goto oid_2_49;
                case 50: return "/OIDResolutionSystem";
                case 51: return "/GS1";
                case 52: return $"{oidPath}/UAV";
                case 999: return $"{oidPath}/Example";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // country
        #region 2.16.*

        oid_2_16:

            oidPath += "/Country";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                4 => $"{oidPath}AF",
                8 => $"{oidPath}AL",
                12 => $"{oidPath}DZ",
                20 => $"{oidPath}AD",
                24 => $"{oidPath}AO",
                28 => $"{oidPath}AG",
                31 => $"{oidPath}AZ",
                32 => $"{oidPath}AR",
                36 => $"{oidPath}AU",
                40 => $"{oidPath}AT",
                44 => $"{oidPath}BS",
                48 => $"{oidPath}BH",
                50 => $"{oidPath}BD",
                51 => $"{oidPath}AM",
                52 => $"{oidPath}BB",
                56 => $"{oidPath}BE",
                60 => $"{oidPath}BM",
                64 => $"{oidPath}BT",
                68 => $"{oidPath}BO",
                70 => $"{oidPath}BA",
                72 => $"{oidPath}BW",
                76 => $"{oidPath}BR",
                84 => $"{oidPath}BZ",
                90 => $"{oidPath}SB",
                96 => $"{oidPath}BN",
                100 => $"{oidPath}BG",
                104 => $"{oidPath}MM",
                108 => $"{oidPath}BI",
                112 => $"{oidPath}BY",
                116 => $"{oidPath}KH",
                120 => $"{oidPath}CM",
                124 => $"{oidPath}CA",
                132 => $"{oidPath}CV",
                140 => $"{oidPath}CF",
                144 => $"{oidPath}LK",
                148 => $"{oidPath}TD",
                152 => $"{oidPath}CL",
                156 => $"{oidPath}CN",
                158 => $"{oidPath}TW",
                170 => $"{oidPath}CO",
                174 => $"{oidPath}KM",
                178 => $"{oidPath}CG",
                180 => $"{oidPath}CD",
                188 => $"{oidPath}CR",
                191 => $"{oidPath}HR",
                192 => $"{oidPath}CU",
                196 => $"{oidPath}CY",
                203 => $"{oidPath}CZ",
                204 => $"{oidPath}BJ",
                208 => $"{oidPath}DK",
                212 => $"{oidPath}DM",
                214 => $"{oidPath}DO",
                218 => $"{oidPath}EC",
                222 => $"{oidPath}SV",
                226 => $"{oidPath}GQ",
                231 => $"{oidPath}ET",
                232 => $"{oidPath}ER",
                233 => $"{oidPath}EE",
                242 => $"{oidPath}FJ",
                246 => $"{oidPath}FI",
                250 => $"{oidPath}FR",
                262 => $"{oidPath}DJ",
                266 => $"{oidPath}GA",
                268 => $"{oidPath}GE",
                270 => $"{oidPath}GM",
                275 => $"{oidPath}PS",
                276 => $"{oidPath}DE",
                288 => $"{oidPath}GH",
                296 => $"{oidPath}KI",
                300 => $"{oidPath}GR",
                308 => $"{oidPath}GD",
                320 => $"{oidPath}GT",
                324 => $"{oidPath}GN",
                328 => $"{oidPath}GY",
                332 => $"{oidPath}HT",
                336 => $"{oidPath}VA",
                340 => $"{oidPath}HN",
                344 => $"{oidPath}HK",
                348 => $"{oidPath}HU",
                352 => $"{oidPath}IS",
                356 => $"{oidPath}IN",
                360 => $"{oidPath}ID",
                364 => $"{oidPath}IR",
                368 => $"{oidPath}IQ",
                372 => $"{oidPath}IE",
                376 => $"{oidPath}IL",
                380 => $"{oidPath}IT",
                384 => $"{oidPath}CI",
                388 => $"{oidPath}JM",
                392 => $"{oidPath}JP",
                398 => $"{oidPath}KZ",
                400 => $"{oidPath}JO",
                404 => $"{oidPath}KE",
                408 => $"{oidPath}KP",
                410 => $"{oidPath}KR",
                414 => $"{oidPath}KW",
                417 => $"{oidPath}KG",
                418 => $"{oidPath}LA",
                422 => $"{oidPath}LB",
                426 => $"{oidPath}LS",
                428 => $"{oidPath}LV",
                430 => $"{oidPath}LR",
                434 => $"{oidPath}LY",
                438 => $"{oidPath}LI",
                440 => $"{oidPath}LT",
                442 => $"{oidPath}LU",
                450 => $"{oidPath}MG",
                454 => $"{oidPath}MW",
                458 => $"{oidPath}MY",
                462 => $"{oidPath}MV",
                466 => $"{oidPath}ML",
                470 => $"{oidPath}MT",
                478 => $"{oidPath}MR",
                480 => $"{oidPath}MU",
                484 => $"{oidPath}MX",
                492 => $"{oidPath}MC",
                496 => $"{oidPath}MN",
                498 => $"{oidPath}MD",
                499 => $"{oidPath}ME",
                504 => $"{oidPath}MA",
                508 => $"{oidPath}MZ",
                512 => $"{oidPath}OM",
                516 => $"{oidPath}NA",
                520 => $"{oidPath}NR",
                524 => $"{oidPath}NP",
                528 => $"{oidPath}NL",
                530 => $"{oidPath}AN",
                548 => $"{oidPath}VU",
                554 => $"{oidPath}NZ",
                558 => $"{oidPath}NI",
                562 => $"{oidPath}NE",
                566 => $"{oidPath}NG",
                578 => $"{oidPath}NO",
                583 => $"{oidPath}FM",
                584 => $"{oidPath}MH",
                585 => $"{oidPath}PW",
                586 => $"{oidPath}PK",
                591 => $"{oidPath}PA",
                598 => $"{oidPath}PG",
                600 => $"{oidPath}PY",
                604 => $"{oidPath}PE",
                608 => $"{oidPath}PH",
                616 => $"{oidPath}PL",
                620 => $"{oidPath}PT",
                624 => $"{oidPath}GW",
                626 => $"{oidPath}TL",
                634 => $"{oidPath}QA",
                642 => $"{oidPath}RO",
                643 => $"{oidPath}RU",
                646 => $"{oidPath}RW",
                659 => $"{oidPath}KN",
                662 => $"{oidPath}LC",
                670 => $"{oidPath}VC",
                674 => $"{oidPath}SM",
                678 => $"{oidPath}ST",
                682 => $"{oidPath}SA",
                686 => $"{oidPath}SN",
                688 => $"{oidPath}RS",
                690 => $"{oidPath}SC",
                694 => $"{oidPath}SL",
                702 => $"{oidPath}SG",
                703 => $"{oidPath}SK",
                704 => $"{oidPath}VN",
                705 => $"{oidPath}SI",
                706 => $"{oidPath}SO",
                710 => $"{oidPath}ZA",
                716 => $"{oidPath}ZW",
                724 => $"{oidPath}ES",
                728 => $"{oidPath}SS",
                729 => $"{oidPath}SD",
                740 => $"{oidPath}SR",
                748 => $"{oidPath}SZ",
                752 => $"{oidPath}SE",
                756 => $"{oidPath}CH",
                760 => $"{oidPath}SY",
                762 => $"{oidPath}TJ",
                764 => $"{oidPath}TH",
                768 => $"{oidPath}TG",
                776 => $"{oidPath}TO",
                780 => $"{oidPath}TT",
                784 => $"{oidPath}AE",
                788 => $"{oidPath}TN",
                792 => $"{oidPath}TR",
                795 => $"{oidPath}TM",
                798 => $"{oidPath}TV",
                800 => $"{oidPath}UG",
                804 => $"{oidPath}UA",
                807 => $"{oidPath}MK",
                818 => $"{oidPath}EG",
                826 => $"{oidPath}GB",
                834 => $"{oidPath}TZ",
                840 => $"{oidPath}US",
                854 => $"{oidPath}BF",
                858 => $"{oidPath}UY",
                860 => $"{oidPath}UZ",
                862 => $"{oidPath}VE",
                882 => $"{oidPath}WS",
                887 => $"{oidPath}YE",
                894 => $"{oidPath}ZM",
                _ => $"{oidPath}{values[index - 1]}",
            };

        #endregion

        // uuid [TODO: Requires 128-bit values]
        #region 2.25.*

        oid_2_25:

            oidPath += "/UUID";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/00000000-0000-0000-0000-000000000000",
                //case 288786655511405443130567505384701230: return $"{oidPath}/00379e48-0a2b-1085-b288-0002a5d5fd2e";
                //case 987895962269883002155146617097157934: return $"{oidPath}/00be4308-0c89-1085-8ea0-0002a5d5fd2e";
                //case 1858228783942312576083372383319475483: return $"{oidPath}/0165e1c0-a655-11e0-95b8-0002a5d5c51b";
                //case 2474299330026746002885628159579243803: return $"{oidPath}/01dc8860-25fb-11da-82b2-0002a5d5c51b";
                //case 3263645701162998421821186056373271854: return $"{oidPath}/02748e28-08c4-1085-b21d-0002a5d5fd2e";
                //case 3325839809379844461264382260940242222: return $"{oidPath}/02808890-0ad8-1085-9bdf-0002a5d5fd2e";
                // TODO: Left off at http://www.oid-info.com/get/2.25.3664154270495270126161055518190585115
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // telebiometrics
        #region 2.42.*

        oid_2_42:

            oidPath += "/Telebiometrics";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_2_42_0;
                case 1: goto oid_2_42_1;
                case 2: goto oid_2_42_2;
                case 3: goto oid_2_42_3;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // modules
        #region 2.42.0.*

        oid_2_42_0:

            oidPath += "/Modules";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_2_42_0_0;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // main
        #region 2.42.0.0.*

        oid_2_42_0_0:

            oidPath += "/Main_Module";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/Version1",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // tmm
        #region 2.42.1.*

        oid_2_42_1:

            oidPath += "/TMM";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_2_42_1_0;
                case 1: goto oid_2_42_1_1;
                case 2: goto oid_2_42_1_2;
                case 3: goto oid_2_42_1_3;
                case 4: return $"{oidPath}/Practitioners";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // modules
        #region 2.42.1.0.*

        oid_2_42_1_0:

            oidPath += "/Modules";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_2_42_1_0_0;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // main
        #region 2.42.1.0.0.*

        oid_2_42_1_0_0:

            oidPath += "/Main";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/First_Version",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // measures, metric
        #region 2.42.1.1.*

        oid_2_42_1_1:

            oidPath += "/Measures";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_42_1_1_1;
                case 2: return $"{oidPath}/Units";
                case 3: return $"{oidPath}";
                case 4: return $"{oidPath}/Conditions";
                case 5: goto oid_2_42_1_1_5;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // quantities
        #region 2.42.1.1.1.*

        oid_2_42_1_1_1:

            oidPath += "/Quantities";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/Physics",
                2 => $"{oidPath}/Chemistry",
                3 => $"{oidPath}/Biology",
                4 => $"{oidPath}/Culturology",
                5 => $"{oidPath}/Psychology",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // methods
        #region 2.42.1.1.5.*

        oid_2_42_1_1_5:

            oidPath += "/Methods";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/Physics",
                2 => $"{oidPath}/Chemistry",
                3 => $"{oidPath}/Biology",
                4 => $"{oidPath}/Culturology",
                5 => $"{oidPath}/Psychology",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // fields-of-study, scientific
        #region 2.42.1.2.*

        oid_2_42_1_2:

            oidPath += "/Fields_of_Study";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/Physics",
                2 => $"{oidPath}/Chemistry",
                3 => $"{oidPath}/Biology",
                4 => $"{oidPath}/Culturology",
                5 => $"{oidPath}/Psychology",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // modalities, sensory
        #region 2.42.1.3.*

        oid_2_42_1_3:

            oidPath += "/Modalities";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/Tango",
                2 => $"{oidPath}/Video",
                3 => $"{oidPath}/Audio",
                4 => $"{oidPath}/Chemo",
                5 => $"{oidPath}/Radio",
                6 => $"{oidPath}/Calor",
                7 => $"{oidPath}/Electro",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // human-physiology
        #region 2.42.2.*

        oid_2_42_2:

            oidPath += "/Human_Physiology";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_2_42_2_0;
                case 1: goto oid_2_42_2_1;
                case 2: return $"{oidPath}/Symbol_Combinations";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // modules
        #region 2.42.2.0.*

        oid_2_42_2_0:

            oidPath += "/Modules";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_2_42_2_0_0;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // main
        #region 2.42.2.0.0.*

        oid_2_42_2_0_0:

            oidPath += "/Main_Module";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/First_Version",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // symbols
        #region 2.42.2.1.*

        oid_2_42_2_1:

            oidPath += "/Symbols";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/Tango_in",
                2 => $"{oidPath}/Video_in",
                3 => $"{oidPath}/Audio_in",
                4 => $"{oidPath}/Chemo_in",
                5 => $"{oidPath}/Radio_in",
                6 => $"{oidPath}/Calor_in",
                7 => $"{oidPath}/Tango_out",
                8 => $"{oidPath}/Video_out",
                9 => $"{oidPath}/Audio_out",
                10 => $"{oidPath}/Chemo_out",
                11 => $"{oidPath}/Radio_out",
                12 => $"{oidPath}/Calor_out",
                13 => $"{oidPath}/Safe",
                14 => $"{oidPath}/Threshold",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // obj-cat, telehealth, e-health-protocol, th
        #region 2.42.3.*

        oid_2_42_3:

            oidPath += "/E_Health_Protocol";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_2_42_3_0;
                case 1: return $"{oidPath}/[Patient schemes]";
                case 2: return $"{oidPath}/[Medical staff schemes]";
                case 3: return $"{oidPath}/[Observer schemes]";
                case 4: return $"{oidPath}/[Pharmaceutical schemes]";
                case 5: return $"{oidPath}/[Laboratory schemes]";
                case 6: return $"{oidPath}/[Drug manufacturer schemes]";
                case 7: return $"{oidPath}/[Medical device schemes]";
                case 8: return $"{oidPath}/[Medical software schemes]";
                case 9: return $"{oidPath}/[Medical insurance schemes]";
                case 10: return $"{oidPath}/[Medical record schemes]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // obj-cat, telehealth, e-health-protocol, th
        #region 2.42.3.0.*

        oid_2_42_3_0:

            oidPath += "/Modules";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_2_42_3_0_0;
                case 1: goto oid_2_42_3_0_1;
                case 2: goto oid_2_42_3_0_2;
                case 3: goto oid_2_42_3_0_3;
                case 4: goto oid_2_42_3_0_4;
                case 5: goto oid_2_42_3_0_5;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // identification
        #region 2.42.3.0.0.*

        oid_2_42_3_0_0:

            oidPath += "/Identification";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/Version1",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // set-up
        #region 2.42.3.0.1.*

        oid_2_42_3_0_1:

            oidPath += "/Setup";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/Version1",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // send-and-ack
        #region 2.42.3.0.2.*

        oid_2_42_3_0_2:

            oidPath += "/Send-and-ack";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/Version1",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // command-response
        #region 2.42.3.0.3.*

        oid_2_42_3_0_3:

            oidPath += "/Command-response";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/Version1",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // quantity-and-units
        #region 2.42.3.0.4.*

        oid_2_42_3_0_4:

            oidPath += "/Quantities_And_Units";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/Version1",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // examples
        #region 2.42.3.0.5.*

        oid_2_42_3_0_5:

            oidPath += "/Examples";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/Command_Response",
                1 => $"{oidPath}/Data_Message",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        #endregion

        // cybersecurity
        #region 2.48.*

        oid_2_48:

            oidPath += "/Cybersecurity";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/Country",
                2 => $"{oidPath}/International-Org",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // alerting
        #region 2.49.*

        oid_2_49:

            oidPath += "/Alerting";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/WMO",
                _ => $"{oidPath}/{values[index - 1]}",
            };

            #endregion

            #endregion
        }
    }
}

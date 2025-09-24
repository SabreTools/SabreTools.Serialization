using System;
using System.Text;

#pragma warning disable CS0162 // Unreachable code detected
#pragma warning disable CS0164 // This label has not been referenced

namespace SabreTools.Serialization.ObjectIdentifier
{
    /// <summary>
    /// Methods related to Object Identifiers (OID) and OID-IRI formatting
    /// </summary>
    public static partial class Parser
    {
        /// <summary>
        /// Parse an OID in separated-value notation into modified OID-IRI notation
        /// </summary>
        /// <param name="values">List of values to check against</param>
        /// <returns>OID-IRI formatted string, if possible</returns>
        /// <remarks>
        /// If a value does not have a fully-descriptive name, it may be replaced by
        /// a string from the official description. As such, the output of this is
        /// not considered to be fully OID-IRI compliant.
        /// </remarks>
        /// <see href="https://oid-base.com/"/>
        public static string? ParseOIDToModifiedOIDIRI(ulong[]? values)
        {
            // If we have an invalid set of values, we can't do anything
            if (values == null || values.Length == 0)
                return null;

            // Set the initial index
            int index = 0;

            // Get a string builder for the path
            var nameBuilder = new StringBuilder();

            // Try to parse the standard value
            string? standard = ParseOIDToModifiedOIDIRI(values, ref index);
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
        /// Parse an OID in separated-value notation into modified OID-IRI notation
        /// </summary>
        /// <param name="values">List of values to check against</param>
        /// <param name="index">Current index into the list</param>
        /// <returns>OID-IRI formatted string, if possible</returns>
        /// <remarks>
        /// If a value does not have a fully-descriptive name, it may be replaced by
        /// a string from the official description. As such, the output of this is
        /// not considered to be fully OID-IRI compliant.
        /// </remarks>
        private static string? ParseOIDToModifiedOIDIRI(ulong[]? values, ref int index)
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
                case 1: return $"{oidPath}/[question]";
                case 2: goto oid_0_2;
                case 3: goto oid_0_3;
                case 4: goto oid_0_4;
                case 5: return "/ITU-R/R-Recommendation";
                case 9: goto oid_0_9;
                default: return $"{oidPath}/{values[index - 1]}";
            }
            ;

        // recommendation
        #region 0.0.*

        oid_0_0:

            oidPath += "/Recommendation";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: return $"{oidPath}/A";
                case 2: return $"{oidPath}/B";
                case 3: return $"{oidPath}/C";
                case 4: return $"{oidPath}/D";
                case 5: goto oid_0_0_5;
                case 6: return $"{oidPath}/F";
                case 7: goto oid_0_0_7;
                case 8: goto oid_0_0_8;
                case 9: goto oid_0_0_9;
                case 10: return $"{oidPath}/J";
                case 11: return $"{oidPath}/K";
                case 12: return $"{oidPath}/L";
                case 13: goto oid_0_0_13;
                case 14: return $"{oidPath}/N";
                case 15: return $"{oidPath}/O";
                case 16: return $"{oidPath}/P";
                case 17: goto oid_0_0_17;
                case 18: return $"{oidPath}/R";
                case 19: return $"{oidPath}/S";
                case 20: goto oid_0_0_20;
                case 21: return $"{oidPath}/U";
                case 22: goto oid_0_0_22;
                case 24: goto oid_0_0_24;
                case 25: return $"{oidPath}/Y";
                case 26: return $"{oidPath}/Z";
                case 59: return $"{oidPath}/[xcmJobZeroDummy]";
                case 74: return $"{oidPath}/[xcmSvcMonZeroDummy]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // e
        #region 0.0.5.*

        oid_0_0_5:

            oidPath += "/E";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 115: goto oid_0_0_5_115;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #region 0.0.5.115.*

        oid_0_0_5_115:

            oidPath += "/[Computerized directory assistance]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_0_0_5_115_1;
                case 2: goto oid_0_0_5_115_2;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #region 0.0.5.115.1.*

        oid_0_0_5_115_1:

            oidPath += "/[E115v1]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Version 1.00]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #region 0.0.5.115.2.*

        oid_0_0_5_115_2:

            oidPath += "/[E115v2]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Version 2.00]",
                1 => $"{oidPath}/[Version 2.01]",
                10 => $"{oidPath}/[Version 2.10]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        // g
        #region 0.0.7.*

        oid_0_0_7:

            oidPath += "";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 711: goto oid_0_0_7_711;
                case 719: goto oid_0_0_7_719;
                case 726: goto oid_0_0_7_726;
                case 774: goto oid_0_0_7_774;
                case 7221: goto oid_0_0_7_7221;
                case 7222: goto oid_0_0_7_7222;
                case 7761: goto oid_0_0_7_7761;
                case 85501: goto oid_0_0_7_85501;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // g711
        #region 0.0.7.711.*

        oid_0_0_7_711:

            oidPath += "/[G.711 series]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_0_0_7_711_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // dot
        #region 0.0.7.711.1.*

        oid_0_0_7_711_1:

            oidPath += "[G.711.x series of Recommendations]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_0_0_7_711_1_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // part1
        #region 0.0.7.711.1.1.*

        oid_0_0_7_711_1_1:

            oidPath += "/[Wideband embedded extension for G.711 pulse code modulation]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_0_0_7_711_1_1_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // generic-capabilities
        #region 0.0.7.711.1.1.1.*

        oid_0_0_7_711_1_1_1:

            oidPath += "/[Generic capabilities]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_0_0_7_711_1_1_1_0;
                case 1: goto oid_0_0_7_711_1_1_1_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // u-law
        #region 0.0.7.711.1.1.1.0.*

        oid_0_0_7_711_1_1_1_0:

            oidPath += "/[μ-law capability identifier]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[μ-law core capability identifier]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // a-law
        #region 0.0.7.711.1.1.1.1.*

        oid_0_0_7_711_1_1_1_1:

            oidPath += "/[a-law]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[a-law core capability identifier]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        #endregion

        #endregion

        // 719
        #region 0.0.7.719.*

        oid_0_0_7_719:

            oidPath += "/[Low-complexity, full-band audio coding for high-quality, conversational applications]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_0_0_7_719_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // generic-capabilities
        #region 0.0.7.719.1.*

        oid_0_0_7_719_1:

            oidPath += "/[Generic capabilities]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[capability]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // 726
        #region 0.0.7.726.*

        oid_0_0_7_726:

            oidPath += "/[40, 32, 24, 16 kbit/s Adaptive Differential Pulse Code Modulation (ADPCM)]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_0_0_7_726_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // generic-capabilities
        #region 0.0.7.726.1.*

        oid_0_0_7_726_1:

            oidPath += "/[generic capabilities]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Version 2003]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // sdhm, g774
        #region 0.0.7.774.*

        oid_0_0_7_774:

            oidPath += "/[Synchronous Digital Hierarchy (SDH)]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_0_0_7_774_0;
                case 1: goto oid_0_0_7_774_1;
                case 2: goto oid_0_0_7_774_2;
                case 127: goto oid_0_0_7_774_127;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // informationModel
        #region 0.0.7.774.0.*

        oid_0_0_7_774_0:

            oidPath += "/[Information model]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: return $"{oidPath}/[Standard-specific extensions to the allocation scheme]";
                case 2: goto oid_0_0_7_774_0_2;
                case 3: goto oid_0_0_7_774_0_3;
                case 4: return $"{oidPath}/[GDMO packages]";
                case 5: return $"{oidPath}/[Guidelines for the Definition of Managed Objects (GDMO) parameters]";
                case 6: goto oid_0_0_7_774_0_6;
                case 7: goto oid_0_0_7_774_0_7;
                case 8: return $"{oidPath}/[Guidelines for the Definition of Managed Objects (GDMO) attribute groups]";
                case 9: return $"{oidPath}/[Actions]";
                case 10: return $"{oidPath}/[Guidelines for the Definition of Managed Objects (GDMO) notifications]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // asn1Module
        #region 0.0.7.774.0.2.*

        oid_0_0_7_774_0_2:

            oidPath += "/[ASN.1 modules]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[SDH]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // managedObjectClass
        #region 0.0.7.774.0.3.*

        oid_0_0_7_774_0_3:

            oidPath += "/[Managed object classes]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                3 => $"{oidPath}/[au3CTPSource]",
                6 => $"{oidPath}/[au4CTPSource]",
                7 => $"{oidPath}/[augBidirectional]",
                8 => $"{oidPath}/[augSink]",
                9 => $"{oidPath}/[augSource]",
                10 => $"{oidPath}/[electricalSPITTPBidirectional]",
                11 => $"{oidPath}/[electricalSPITTPSink]",
                12 => $"{oidPath}/[electricalSPITTPSource]",
                13 => $"{oidPath}/[indirectAdaptorBidirectional]",
                14 => $"{oidPath}/[indirectAdaptorSink]",
                15 => $"{oidPath}/[indirectAdaptorSource]",
                16 => $"{oidPath}/[msCTPBidirectional]",
                17 => $"{oidPath}/[msCTPSink]",
                18 => $"{oidPath}/[msCTPSource]",
                19 => $"{oidPath}/[msDatacomCTPBidirectional]",
                20 => $"{oidPath}/[msDatacomCTPSink]",
                21 => $"{oidPath}/[msDatacomCTPSource]",
                22 => $"{oidPath}/[msOrderwireCTPBidirectional]",
                23 => $"{oidPath}/[msOrderwireCTPSink]",
                24 => $"{oidPath}/[msOrderwireCTPSource]",
                25 => $"{oidPath}/[msTTPBidirectional]",
                26 => $"{oidPath}/[msTTPSink]",
                27 => $"{oidPath}/[msTTPSource]",
                28 => $"{oidPath}/[opticalSPITTPBidirectional]",
                29 => $"{oidPath}/[opticalSPITTPSink]",
                30 => $"{oidPath}/[opticalSPITTPSource]",
                31 => $"{oidPath}/[rsCTPBidirectional]",
                32 => $"{oidPath}/[rsCTPSink]",
                33 => $"{oidPath}/[rsCTPSource]",
                34 => $"{oidPath}/[rsDatacomCTPBidirectional]",
                35 => $"{oidPath}/[rsDatacomCTPSink]",
                36 => $"{oidPath}/[rsDatacomCTPSource]",
                37 => $"{oidPath}/[rsOrderwireCTPBidirectional]",
                38 => $"{oidPath}/[rsOrderwireCTPSink]",
                39 => $"{oidPath}/[rsOrderwireCTPSource]",
                40 => $"{oidPath}/[rsTTPBidirectional]",
                41 => $"{oidPath}/[rsTTPSink]",
                42 => $"{oidPath}/[rsTTPSource]",
                43 => $"{oidPath}/[rsUserChannelCTPBidirectional]",
                44 => $"{oidPath}/[rsUserChannelCTPSink]",
                45 => $"{oidPath}/[rsUserChannelCTPSource]",
                46 => $"{oidPath}/[sdhNE]",
                49 => $"{oidPath}/[tu11CTPSource]",
                52 => $"{oidPath}/[tu12CTPSource]",
                55 => $"{oidPath}/[tu2CTPSource]",
                58 => $"{oidPath}/[tu3CTPSource]",
                59 => $"{oidPath}/[tug2Bidirectional]",
                60 => $"{oidPath}/[tug2Sink]",
                61 => $"{oidPath}/[tug2Source]",
                62 => $"{oidPath}/[tug3Bidirectional]",
                63 => $"{oidPath}/[tug3Sink]",
                64 => $"{oidPath}/[tug3Source]",
                67 => $"{oidPath}/[vc11TTPSource]",
                70 => $"{oidPath}/[vc12TTPSource]",
                73 => $"{oidPath}/[vc2TTPSource]",
                80 => $"{oidPath}/[vcnUserChannelCTPBidirectional]",
                81 => $"{oidPath}/[vcnUserChannelCTPSink]",
                82 => $"{oidPath}/[vcnUserChannelCTPSource]",
                83 => $"{oidPath}/[au3CTPBidirectionalR1]",
                84 => $"{oidPath}/[au3CTPSinkR1]",
                85 => $"{oidPath}/[au4CTPBidirectionalR1]",
                86 => $"{oidPath}/[au4CTPSinkR1]",
                87 => $"{oidPath}/[tu11CTPBidirectionalR1]",
                88 => $"{oidPath}/[tu11CTPSinkR1]",
                89 => $"{oidPath}/[tu12CTPBidirectionalR1]",
                90 => $"{oidPath}/[tu12CTPSinkR1]",
                91 => $"{oidPath}/[tu2CTPBidirectionalR1]",
                92 => $"{oidPath}/[tu2CTPSinkR1]",
                93 => $"{oidPath}/[tu3CTPBidirectionalR1]",
                94 => $"{oidPath}/[tu3CTPBidirectionalR1]",
                95 => $"{oidPath}/[vc11TTPBidirectionalR1]",
                96 => $"{oidPath}/[vc11TTPSinkR1]",
                97 => $"{oidPath}/[vc12TTPBidirectionalR1]",
                98 => $"{oidPath}/[vc12TTPSinkR1]",
                99 => $"{oidPath}/[vc2TTPBidirectionalR1]",
                100 => $"{oidPath}/[vc2TTPSinkR1]",
                101 => $"{oidPath}/[vc3TTPBidirectionalR1]",
                102 => $"{oidPath}/[vc3TTPSinkR1]",
                103 => $"{oidPath}/[vc3TTPSourceR1]",
                104 => $"{oidPath}/[vc4TTPBidirectionalR1]",
                105 => $"{oidPath}/[vc4TTPSinkR1]",
                106 => $"{oidPath}/[vc4TTPSourceR1]",
                107 => $"{oidPath}/[rsTTPTrailTraceBidirectional]",
                108 => $"{oidPath}/[rsTTPTrailTraceSink]",
                109 => $"{oidPath}/[rsTTPTrailTraceSource]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // nameBinding
        #region 0.0.7.774.0.6.*

        oid_0_0_7_774_0_6:

            oidPath += "/[Name bindings]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                4 => $"{oidPath}/[au3CTPSource-augBidirectional]",
                5 => $"{oidPath}/[au3CTPSource-augSource]",
                9 => $"{oidPath}/[au4CTPSource-augBidirectional]",
                10 => $"{oidPath}/[au4CTPSource-augSource]",
                11 => $"{oidPath}/[augBidirectional-msTTPBidirectional]",
                12 => $"{oidPath}/[augSink-msTTPSink]",
                13 => $"{oidPath}/[augSource-msTTPSource]",
                14 => $"{oidPath}/[electricalSPITTPBidirectional-sdhNE]",
                15 => $"{oidPath}/[electricalSPITTPSink-sdhNE]",
                16 => $"{oidPath}/[electricalSPITTPSource-sdhNE]",
                17 => $"{oidPath}/[msCTPBidirectional-rsTTPBidirectional]",
                18 => $"{oidPath}/[msCTPSink-rsTTPBidirectional]",
                19 => $"{oidPath}/[msCTPSink-rsTTPSink]",
                20 => $"{oidPath}/[msCTPSource-rsTTPBidirectional]",
                21 => $"{oidPath}/[msCTPSource-rsTTPSource]",
                22 => $"{oidPath}/[msDatacomCTPBidirectional-msTTPBidirectional]",
                23 => $"{oidPath}/[msDatacomCTPSink-msTTPBidirectional]",
                24 => $"{oidPath}/[msDatacomCTPSink-msTTPSink]",
                25 => $"{oidPath}/[msDatacomCTPSource-msTTPBidirectional]",
                26 => $"{oidPath}/[msDatacomCTPSource-msTTPSource]",
                27 => $"{oidPath}/[msOrderwireCTPBidirectional-msTTPBidirectional]",
                28 => $"{oidPath}/[msOrderwireCTPSink-msTTPBidirectional]",
                29 => $"{oidPath}/[msOrderwireCTPSink-msTTPSink]",
                30 => $"{oidPath}/[msOrderwireCTPSource-msTTPBidirectional]",
                31 => $"{oidPath}/[msOrderwireCTPSource-msTTPSource]",
                32 => $"{oidPath}/[msTTPBidirectional-sdhNE]",
                33 => $"{oidPath}/[msTTPSink-sdhNE]",
                34 => $"{oidPath}/[msTTPSource-sdhNE]",
                35 => $"{oidPath}/[opticalSPITTPBidirectional-sdhNE]",
                36 => $"{oidPath}/[opticalSPITTPSink-sdhNE]",
                37 => $"{oidPath}/[opticalSPITTPSource-sdhNE]",
                38 => $"{oidPath}/[rsCTPBidirectional-electricalSPITTPBidirectional]",
                39 => $"{oidPath}/[rsCTPSink-electricalSPITTPBidirectional]",
                40 => $"{oidPath}/[rsCTPSink-electricalSPITTPSink]",
                41 => $"{oidPath}/[rsCTPSource-electricalSPITTPBidirectional]",
                42 => $"{oidPath}/[rsCTPSource-electricalSPITTPSource]",
                43 => $"{oidPath}/[rsCTPBidirectional-opticalSPITTPBidirectional]",
                44 => $"{oidPath}/[rsCTPSink-opticalSPITTPBidirectional]",
                45 => $"{oidPath}/[rsCTPSink-opticalSPITTPSink]",
                46 => $"{oidPath}/[rsCTPSource-opticalSPITTPBidirectional]",
                47 => $"{oidPath}/[rsCTPSource-opticalSPITTPSource]",
                48 => $"{oidPath}/[rsDatacomCTPBidirectional-rsTTPBidirectional]",
                49 => $"{oidPath}/[rsDatacomCTPSink-rsTTPBidirectional]",
                50 => $"{oidPath}/[rsDatacomCTPSink-rsTTPSink]",
                51 => $"{oidPath}/[rsDatacomCTPSource-rsTTPBidirectional]",
                52 => $"{oidPath}/[rsDatacomCTPSource-rsTTPSource]",
                53 => $"{oidPath}/[rsOrderwireCTPBidirectional-rsTTPBidirectional]",
                54 => $"{oidPath}/[rsOrderwireCTPSink-rsTTPBidirectional]",
                55 => $"{oidPath}/[rsOrderwireCTPSink-rsTTPSink]",
                56 => $"{oidPath}/[rsOrderwireCTPSource-rsTTPBidirectional]",
                57 => $"{oidPath}/[rsOrderwireCTPSource-rsTTPSource]",
                58 => $"{oidPath}/[rsTTPBidirectional-sdhNE]",
                59 => $"{oidPath}/[rsTTPSink-sdhNE]",
                60 => $"{oidPath}/[rsTTPSource-sdhNE]",
                61 => $"{oidPath}/[rsUserChannelCTPBidirectional-rsTTPBidirectional]",
                62 => $"{oidPath}/[rsUserChannelCTPSink-rsTTPBidirectional]",
                63 => $"{oidPath}/[rsUserChannelCTPSink-rsTTPSink]",
                64 => $"{oidPath}/[rsUserChannelCTPSource-rsTTPBidirectional]",
                65 => $"{oidPath}/[rsUserChannelCTPSource-rsTTPSource]",
                69 => $"{oidPath}/[tu11CTPSource-tug2Bidirectional]",
                70 => $"{oidPath}/[tu11CTPSource-tug2Source]",
                74 => $"{oidPath}/[tu12CTPSource-tug2Bidirectional]",
                75 => $"{oidPath}/[tu12CTPSource-tug2Source]",
                79 => $"{oidPath}/[tu2CTPSource-tug2Bidirectional]",
                80 => $"{oidPath}/[tu2CTPSource-tug2Source]",
                84 => $"{oidPath}/[tu3CTPSource-tug3Bidirectional]",
                85 => $"{oidPath}/[tu3CTPSource-tug3Source]",
                86 => $"{oidPath}/[tug2Bidirectional-tug3Bidirectional]",
                87 => $"{oidPath}/[tug2Sink-tug3Sink]",
                88 => $"{oidPath}/[tug2Source-tug3Source]",
                97 => $"{oidPath}/[vc11TTPSource-sdhNE]",
                100 => $"{oidPath}/[vc12TTPSource-sdhNE]",
                103 => $"{oidPath}/[vc2TTPSource-sdhNE]",
                121 => $"{oidPath}/[au3CTPBidirectionalR1-augBidirectional]",
                122 => $"{oidPath}/[au3CTPSinkR1-augBidirectional]",
                123 => $"{oidPath}/[au3CTPSinkR1-augSink]",
                124 => $"{oidPath}/[au4CTPBidirectionalR1-augBidirectional]",
                125 => $"{oidPath}/[au4CTPSinkR1-augBidirectional]",
                126 => $"{oidPath}/[au4CTPSinkR1-augSink]",
                127 => $"{oidPath}/[tu11CTPBidirectionalR1-tug2Bidirectional]",
                128 => $"{oidPath}/[tu11CTPSinkR1-tug2Bidirectional]",
                129 => $"{oidPath}/[tu11CTPSinkR1-tug2Sink]",
                130 => $"{oidPath}/[tu12CTPBidirectionalR1-tug2Bidirectional]",
                131 => $"{oidPath}/[tu12CTPSinkR1-tug2Bidirectional]",
                132 => $"{oidPath}/[tu12CTPSinkR1-tug2Sink]",
                133 => $"{oidPath}/[tu2CTPBidirectionalR1-tug2Bidirectional]",
                134 => $"{oidPath}/[tu2CTPSinkR1-tug2Bidirectional]",
                135 => $"{oidPath}/[tu2CTPSinkR1-tug2Sink]",
                136 => $"{oidPath}/[tu3CTPBidirectionalR1-tug3Bidirectional]",
                137 => $"{oidPath}/[tu3CTPSinkR1-tug3Bidirectional]",
                138 => $"{oidPath}/[tu3CTPSinkR1-tug3Sink]",
                139 => $"{oidPath}/[tug2Bidirectional-vc3TTPBidirectionalR1]",
                140 => $"{oidPath}/[tug2Sink-vc3TTPSinkR1]",
                141 => $"{oidPath}/[tug2Source-vc3TTPSourceR1]",
                142 => $"{oidPath}/[tug3Bidirectional-vc4TTPBidirectionalR1]",
                143 => $"{oidPath}/[tug3Sink-vc4TTPSinkR1]",
                144 => $"{oidPath}/[tug3Source-vc4TTPSourceR1]",
                145 => $"{oidPath}/[vc11TTPBidirectionalR1-sdhNE]",
                146 => $"{oidPath}/[vc11TTPSinkR1-sdhNE]",
                147 => $"{oidPath}/[vc12TTPBidirectionalR1-sdhNE]",
                148 => $"{oidPath}/[vc12TTPSinkR1-sdhNE]",
                149 => $"{oidPath}/[vc2TTPBidirectionalR1-sdhNE]",
                150 => $"{oidPath}/[vc2TTPSinkR1-sdhNE]",
                151 => $"{oidPath}/[vc3TTPBidirectionalR1-sdhNE]",
                152 => $"{oidPath}/[vc3TTPSinkR1-sdhNE]",
                153 => $"{oidPath}/[vc3TTPSourceR1-sdhNE]",
                154 => $"{oidPath}/[vc4TTPBidirectionalR1-sdhNE]",
                155 => $"{oidPath}/[vc4TTPSinkR1-sdhNE]",
                156 => $"{oidPath}/[vc4TTPSourceR1-sdhNE]",
                157 => $"{oidPath}/[vcnUserChannelCTPBidirectional-vc3TTPBidirectionalR1]",
                158 => $"{oidPath}/[vcnUserChannelCTPSink-vc3TTPBidirectionalR1]",
                159 => $"{oidPath}/[vcnUserChannelCTPSink-vc3TTPSinkR1]",
                160 => $"{oidPath}/[vcnUserChannelCTPSource-vc3TTPBidirectionalR1]",
                161 => $"{oidPath}/[vcnUserChannelCTPSource-vc3TTPSourceR1]",
                162 => $"{oidPath}/[vcnUserChannelCTPBidirectional-vc4TTPBidirectionalR1]",
                163 => $"{oidPath}/[vcnUserChannelCTPSink-vc4TTPBidirectionalR1]",
                164 => $"{oidPath}/[vcnUserChannelCTPSink-vc4TTPSinkR1]",
                165 => $"{oidPath}/[vcnUserChannelCTPSource-vc4TTPBidirectionalR1]",
                166 => $"{oidPath}/[vcnUserChannelCTPSource-vc4TTPSourceR1]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // attribute
        #region 0.0.7.774.0.7.*

        oid_0_0_7_774_0_7:

            oidPath += "/[Attributes]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[electricalSPIPackage]",
                2 => $"{oidPath}/[au4CTPId]",
                3 => $"{oidPath}/[augId]",
                4 => $"{oidPath}/[c2SignalLabelExpected]",
                5 => $"{oidPath}/[c2SignalLabelReceive]",
                6 => $"{oidPath}/[c2SignalLabelSend]",
                7 => $"{oidPath}/[electricalSPITTPId]",
                8 => $"{oidPath}/[excessiveBERMtceInhibit]",
                10 => $"{oidPath}/[j1PathTraceExpected]",
                11 => $"{oidPath}/[j1PathTraceReceive]",
                12 => $"{oidPath}/[j1PathTraceSend]",
                13 => $"{oidPath}/[msCTPId]",
                14 => $"{oidPath}/[msDatacomCTPId]",
                15 => $"{oidPath}/[msOrderwireCTPId]",
                16 => $"{oidPath}/[msTTPId]",
                17 => $"{oidPath}/[opticalReach]",
                18 => $"{oidPath}/[opticalSPITTPId]",
                19 => $"{oidPath}/[opticalWavelength]",
                20 => $"{oidPath}/[pointerSinkType]",
                21 => $"{oidPath}/[pointerSourceType]",
                22 => $"{oidPath}/[rsCTPId]",
                23 => $"{oidPath}/[rsDatacomCTPId]",
                24 => $"{oidPath}/[rsOrderwireCTPId]",
                25 => $"{oidPath}/[rsTTPId]",
                26 => $"{oidPath}/[rsUserChannelCTPId]",
                27 => $"{oidPath}/[signalDegradeThreshold]",
                28 => $"{oidPath}/[stmLevel]",
                29 => $"{oidPath}/[tu11CTPId]",
                30 => $"{oidPath}/[tu12CTPId]",
                31 => $"{oidPath}/[tu2CTPId]",
                32 => $"{oidPath}/[tu3CTPId]",
                33 => $"{oidPath}/[tug2Id]",
                34 => $"{oidPath}/[tug3Id]",
                35 => $"{oidPath}/[v5SignalLabelExpected]",
                36 => $"{oidPath}/[v5SignalLabelReceive]",
                37 => $"{oidPath}/[v5SignalLabelSend]",
                38 => $"{oidPath}/[vc11TTPId]",
                39 => $"{oidPath}/[vc12TTPId]",
                40 => $"{oidPath}/[vc2TTPId]",
                41 => $"{oidPath}/[vc3TTPId]",
                42 => $"{oidPath}/[vc4TTPId]",
                43 => $"{oidPath}/[vcnUserChannelCTPId]",
                44 => $"{oidPath}/[trailTraceExpected]",
                45 => $"{oidPath}/[trailTraceReceive]",
                46 => $"{oidPath}/[trailTraceSend]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // protocolSupport
        #region 0.0.7.774.1.*

        oid_0_0_7_774_1:

            oidPath += "/[Protocol support]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // managementApplicationSupport
        #region 0.0.7.774.2.*

        oid_0_0_7_774_2:

            oidPath += "/[Management application support]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Standard-specific extensions to the allocation scheme]",
                1 => $"{oidPath}/[Functional unit packages]",
                2 => $"{oidPath}",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // dot, hyphen
        #region 0.0.7.774.127.*

        oid_0_0_7_774_127:

            oidPath += "/[Parts of Recommendation ITU-T G.774]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_0_0_7_774_127_1;
                case 2: goto oid_0_0_7_774_127_2;
                case 3: goto oid_0_0_7_774_127_3;
                case 4: goto oid_0_0_7_774_127_4;
                case 5: goto oid_0_0_7_774_127_5;
                case 6: goto oid_0_0_7_774_127_6;
                case 7: goto oid_0_0_7_774_127_7;
                case 8: goto oid_0_0_7_774_127_8;
                case 9: goto oid_0_0_7_774_127_9;
                case 10: goto oid_0_0_7_774_127_10;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // part1, pm
        #region 0.0.7.774.127.1.*

        oid_0_0_7_774_127_1:

            oidPath += "/[Recommendation ITU-T G.774.1]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_0_0_7_774_127_1_0;
                case 1: goto oid_0_0_7_774_127_1_1;
                case 2: goto oid_0_0_7_774_127_1_2;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // informationModel
        #region 0.0.7.774.127.1.0.*

        oid_0_0_7_774_127_1_0:

            oidPath += "/[Information model]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: return $"{oidPath}/[Standard specific extension]";
                case 1: return $"{oidPath}/[Functional unit packages]";
                case 2: goto oid_0_0_7_774_127_1_0_2;
                case 3: goto oid_0_0_7_774_127_1_0_3;
                case 4: goto oid_0_0_7_774_127_1_0_4;
                case 5: return $"{oidPath}";
                case 6: goto oid_0_0_7_774_127_1_0_6;
                case 7: goto oid_0_0_7_774_127_1_0_7;
                case 8: return $"{oidPath}/[Attribute groups]";
                case 9: return $"{oidPath}/[Actions]";
                case 10: return $"{oidPath}";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // asn1Module
        #region 0.0.7.774.127.1.0.2.*

        oid_0_0_7_774_127_1_0_2:

            oidPath += "/[ASN.1 modules]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[SDHPMASN1]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // managedObjectClass
        #region 0.0.7.774.127.1.0.3.*

        oid_0_0_7_774_127_1_0_3:

            oidPath += "/[Managed object classes]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[sdhCurrentData]",
                2 => $"{oidPath}/[rsCurrentData]",
                3 => $"{oidPath}/[rsCurrentDataTR]",
                4 => $"{oidPath}/[electricalSourceSPICurrentData]",
                5 => $"{oidPath}/[opticalSourceSPICurrentData]",
                6 => $"{oidPath}/[msCurrentData]",
                7 => $"{oidPath}/[msCurrentDataTR]",
                8 => $"{oidPath}/[protectionCurrentData]",
                9 => $"{oidPath}/[pathTerminationCurrentData]",
                10 => $"{oidPath}/[pathTerminationCurrentDataTR]",
                11 => $"{oidPath}/[msAdaptationCurrentData]",
                12 => $"{oidPath}/[rsHistoryData]",
                13 => $"{oidPath}/[electricalSPIHistoryData]",
                14 => $"{oidPath}/[opticalSPIHistoryData]",
                15 => $"{oidPath}/[msHistoryData]",
                16 => $"{oidPath}/[protectionHistoryData]",
                17 => $"{oidPath}/[pathTerminationHistoryData]",
                18 => $"{oidPath}/[msAdaptationHistoryData]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // package
        #region 0.0.7.774.127.1.0.4.*

        oid_0_0_7_774_127_1_0_4:

            oidPath += "/[Packages]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[cSESCurrentDataPackage]",
                2 => $"{oidPath}/[farEndCSESCurrentDataPackage]",
                3 => $"{oidPath}/[farEndCurrentDataPackage]",
                4 => $"{oidPath}/[farEndHistoryDataPackage]",
                5 => $"{oidPath}/[historyPackage]",
                6 => $"{oidPath}/[laserBiasCurrentDataPackage]",
                7 => $"{oidPath}/[laserBiasTideMarkPackage]",
                8 => $"{oidPath}/[laserTemperatureCurrentDataPackage]",
                9 => $"{oidPath}/[laserTemperatureTideMarkPackage]",
                10 => $"{oidPath}/[oFSCurrentDataPackage]",
                11 => $"{oidPath}/[oFSHistoryDataPackage]",
                12 => $"{oidPath}/[transmitPowerLevelCurrentDataPackage]",
                13 => $"{oidPath}/[transmitPowerLevelTideMarkPackage]",
                14 => $"{oidPath}/[thresholdResetPackage]",
                15 => $"{oidPath}/[uASCurrentDataPackage]",
                16 => $"{oidPath}/[uASHistoryDataPackage]",
                17 => $"{oidPath}/[unavailableTimeAlarmPackage]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // nameBinding
        #region 0.0.7.774.127.1.0.6.*

        oid_0_0_7_774_127_1_0_6:

            oidPath += "/[Name bindings]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[historyData-sdhCurrentData]",
                2 => $"{oidPath}/[msCurrentData-msTTPSink]",
                3 => $"{oidPath}/[msCurrentDataTR-msTTPSink]",
                4 => $"{oidPath}/[msCurrentData-protectedTTPSink]",
                5 => $"{oidPath}/[msCurrentDataTR-protectedTTPSink]",
                6 => $"{oidPath}/[protectionCurrentData-protectionUnit]",
                7 => $"{oidPath}/[rsCurrentData-rsTTPSink]",
                8 => $"{oidPath}/[rsCurrentDataTR-rsTTPSink]",
                9 => $"{oidPath}/[pathTerminationCurrentData-vc4TTPSink]",
                10 => $"{oidPath}/[pathTerminationCurrentData-vc3TTPSink]",
                11 => $"{oidPath}/[pathTerminationCurrentData-vc2TTPSink]",
                12 => $"{oidPath}/[pathTerminationCurrentData-vc12TTPSink]",
                13 => $"{oidPath}/[pathTerminationCurrentData-vc11TTPSink]",
                14 => $"{oidPath}/[pathTerminationCurrentDataTR-vc4TTPSink]",
                15 => $"{oidPath}/[pathTerminationCurrentDataTR-vc3TTPSink]",
                16 => $"{oidPath}/[pathTerminationCurrentDataTR-vc2TTPSink]",
                17 => $"{oidPath}/[pathTerminationCurrentDataTR-vc12TTPSink]",
                18 => $"{oidPath}/[pathTerminationCurrentDataTR-vc11TTPSink]",
                19 => $"{oidPath}/[electricalSourceSPICurrentData-electricalSPITTPSource]",
                20 => $"{oidPath}/[opticalSourceSPICurrentData-opticalSPITTPSource-electricalSPITTPSource]",
                21 => $"{oidPath}/[msAdaptationCurrentData-au4CTPSource]",
                22 => $"{oidPath}/[msAdaptationCurrentData-au3CTPSource]",
                23 => $"{oidPath}/[pathTerminationCurrentData-vc4TTPSinkR1]",
                24 => $"{oidPath}/[pathTerminationCurrentData-vc3TTPSinkR1]",
                25 => $"{oidPath}/[pathTerminationCurrentData-vc2TTPSinkR1]",
                26 => $"{oidPath}/[pathTerminationCurrentData-vc12TTPSinkR1]",
                27 => $"{oidPath}/[pathTerminationCurrentData-vc11TTPSinkR1]",
                28 => $"{oidPath}/[pathTerminationCurrentDataTR-vc4TTPSinkR1]",
                29 => $"{oidPath}/[pathTerminationCurrentDataTR-vc3TTPSinkR1]",
                30 => $"{oidPath}/[pathTerminationCurrentDataTR-vc2TTPSinkR1]",
                31 => $"{oidPath}/[pathTerminationCurrentDataTR-vc12TTPSinkR1]",
                32 => $"{oidPath}/[pathTerminationCurrentDataTR-vc11TTPSinkR1]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // attribute
        #region 0.0.7.774.127.1.0.7.*

        oid_0_0_7_774_127_1_0_7:

            oidPath += "/[Attributes]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[cSESEvent]",
                2 => $"{oidPath}/[eS]",
                3 => $"{oidPath}/[fEES]",
                4 => $"{oidPath}/[fEBBE]",
                5 => $"{oidPath}/[fECSESEvent]",
                6 => $"{oidPath}/[laserBias]",
                7 => $"{oidPath}/[laserBiasTideMarkMax]",
                8 => $"{oidPath}/[laserBiasTideMarkMin]",
                9 => $"{oidPath}/[laserTemperature]",
                10 => $"{oidPath}/[laserTemperatureTideMarkMax]",
                11 => $"{oidPath}/[laserTemperatureTideMarkMin]",
                12 => $"{oidPath}/[nCSES]",
                13 => $"{oidPath}/[bBE]",
                14 => $"{oidPath}/[oFS]",
                15 => $"{oidPath}/[pSC]",
                16 => $"{oidPath}/[pSD]",
                17 => $"{oidPath}/[sES]",
                18 => $"{oidPath}/[fESES]",
                19 => $"{oidPath}/[transmitPowerLevel]",
                20 => $"{oidPath}/[transmitPowerLevelTideMarkMax]",
                21 => $"{oidPath}/[transmitPowerLevelTideMarkMin]",
                22 => $"{oidPath}/[uAS]",
                23 => $"{oidPath}/[pJCHigh]",
                24 => $"{oidPath}/[pJCLow]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // protocolSupport
        #region 0.0.7.774.127.1.1.*

        oid_0_0_7_774_127_1_1:

            oidPath += "/[Protocol support]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // Management applications support
        #region 0.0.7.774.127.1.2.*

        oid_0_0_7_774_127_1_2:

            oidPath += "/[Management applications support]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Standard specific extension]",
                1 => $"{oidPath}/[Functional unit packages]",
                2 => $"{oidPath}",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // conf
        #region 0.0.7.774.127.2.*

        oid_0_0_7_774_127_2:

            oidPath += "[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_0_0_7_774_127_2_0;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // informationModel
        #region 0.0.7.774.127.2.0.*

        oid_0_0_7_774_127_2_0:

            oidPath += "/[Information model]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 2: goto oid_0_0_7_774_127_2_0_2;
                case 3: goto oid_0_0_7_774_127_2_0_3;
                case 5: goto oid_0_0_7_774_127_2_0_5;
                case 6: goto oid_0_0_7_774_127_2_0_6;
                case 9: goto oid_0_0_7_774_127_2_0_9;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // asn1Module
        #region 0.0.7.774.127.2.0.2.*

        oid_0_0_7_774_127_2_0_2:

            oidPath += "/[ASN.1 modules]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[SDHConfASN1]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // managedObjectClass
        #region 0.0.7.774.127.2.0.3.*

        oid_0_0_7_774_127_2_0_3:

            oidPath += "/[Managed object classes]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[modifiableAugBidirectional]",
                2 => $"{oidPath}/[modifiableAugSink]",
                3 => $"{oidPath}/[modifiableAugSource]",
                4 => $"{oidPath}/[modifiableTug3Bidirectional]",
                5 => $"{oidPath}/[modifiableTug3Sink]",
                6 => $"{oidPath}/[modifiableTug3Source]",
                7 => $"{oidPath}/[modifiableTug2Bidirectional]",
                8 => $"{oidPath}/[modifiableTug2Sink]",
                9 => $"{oidPath}/[modifiableTug2Source]",
                18 => $"{oidPath}/[modifiableVC2TTPSource]",
                21 => $"{oidPath}/[modifiableVC12TTPSource]",
                24 => $"{oidPath}/[modifiableVC11TTPSource]",
                25 => $"{oidPath}/[modifiableVC4TTPBidirectionalR1]",
                26 => $"{oidPath}/[modifiableVC4TTPSinkR1]",
                27 => $"{oidPath}/[modifiableVC4TTPSourceR1]",
                28 => $"{oidPath}/[modifiableVC3TTPBidirectionalR1]",
                29 => $"{oidPath}/[modifiableVC3TTPSinkR1]",
                30 => $"{oidPath}/[modifiableVC3TTPSourceR1]",
                31 => $"{oidPath}/[modifiableVC2TTPBidirectionalR1]",
                32 => $"{oidPath}/[modifiableVC2TTPSinkR1]",
                33 => $"{oidPath}/[modifiableVC12TTPBidirectionalR1]",
                34 => $"{oidPath}/[modifiableVC12TTPSinkR1]",
                35 => $"{oidPath}/[modifiableVC11TTPBidirectionalR1]",
                36 => $"{oidPath}/[modifiableVC11TTPSinkR1]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // parameter
        #region 0.0.7.774.127.2.0.5.*

        oid_0_0_7_774_127_2_0_5:

            oidPath += "/[Parameters]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[defineSDHStructureError]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // nameBinding
        #region 0.0.7.774.127.2.0.6.*

        oid_0_0_7_774_127_2_0_6:

            oidPath += "/[Name bindings]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                2 => $"{oidPath}/[au3CTPSource-augSource]",
                4 => $"{oidPath}/[au4CTPSource-augSource]",
                5 => $"{oidPath}/[augSink-msTTPSink]",
                6 => $"{oidPath}/[augSource-msTTPSource]",
                7 => $"{oidPath}/[electricalSPITTPSink-sdhNE]",
                8 => $"{oidPath}/[electricalSPITTPSource-sdhNE]",
                9 => $"{oidPath}/[msCTPSink-rsTTPSink]",
                10 => $"{oidPath}/[msCTPSource-rsTTPSource]",
                11 => $"{oidPath}/[msDatacomCTPSink-msTTPSink]",
                12 => $"{oidPath}/[msDatacomCTPSource-msTTPSource]",
                13 => $"{oidPath}/[msOrderwireCTPSink-msTTPSink]",
                14 => $"{oidPath}/[msOrderwireCTPSource-msTTPSource]",
                15 => $"{oidPath}/[msTTPSink-sdhNE]",
                16 => $"{oidPath}/[msTTPSource-sdhNE]",
                17 => $"{oidPath}/[opticalSPITTPSink-sdhNE]",
                18 => $"{oidPath}/[opticalSPITTPSource-sdhNE]",
                19 => $"{oidPath}/[rsCTPSink-electricalSPITTPSink]",
                20 => $"{oidPath}/[rsCTPSource-electricalSPITTPSource]",
                21 => $"{oidPath}/[rsCTPSink-opticalSPITTPSink]",
                22 => $"{oidPath}/[rsCTPSource-opticalSPITTPSource]",
                23 => $"{oidPath}/[rsDatacomCTPSink-rsTTPSink]",
                24 => $"{oidPath}/[rsDatacomCTPSource-rsTTPSource]",
                25 => $"{oidPath}/[rsOrderwireCTPSink-rsTTPSink]",
                26 => $"{oidPath}/[rsOrderwireCTPSource-rsTTPSource]",
                27 => $"{oidPath}/[rsTTPSink-sdhNE]",
                28 => $"{oidPath}/[rsTTPSource-sdhNE]",
                29 => $"{oidPath}/[rsUserChannelCTPSink-rsTTPSink]",
                30 => $"{oidPath}/[rsUserChannelCTPSource-rsTTPSource]",
                32 => $"{oidPath}/[tu11CTPSource-tug2Source]",
                34 => $"{oidPath}/[tu12CTPSource-tug2Source]",
                36 => $"{oidPath}/[tu2CTPSource-tug2Source]",
                38 => $"{oidPath}/[tu3CTPSource-tug3Source]",
                39 => $"{oidPath}/[tug2Sink-tug3Sink]",
                40 => $"{oidPath}/[tug2Source-tug3Source]",
                46 => $"{oidPath}/[vc11TTPSource-sdhNE]",
                48 => $"{oidPath}/[vc12TTPSource-sdhNE]",
                50 => $"{oidPath}/[vc2TTPSource-sdhNE]",
                59 => $"{oidPath}/[au3CTPSinkR1-augSink]",
                60 => $"{oidPath}/[au4CTPSinkR1-augSink]",
                61 => $"{oidPath}/[tu11CTPSinkR1-tug2Sink]",
                62 => $"{oidPath}/[tu12CTPSinkR1-tug2Sink]",
                63 => $"{oidPath}/[tu2CTPSinkR1-tug2Sink]",
                64 => $"{oidPath}/[tu3CTPSinkR1-tug3Sink]",
                65 => $"{oidPath}/[tug2Sink-vc3TTPSinkR1]",
                66 => $"{oidPath}/[tug2Source-vc3TTPSourceR1]",
                67 => $"{oidPath}/[tug3Sink-vc4TTPSinkR1]",
                68 => $"{oidPath}/[tug3Source-vc4TTPSourceR1]",
                69 => $"{oidPath}/[vc11TTPSinkR1-sdhNE]",
                70 => $"{oidPath}/[vc12TTPSinkR1-sdhNE]",
                71 => $"{oidPath}/[vc2TTPSinkR1-sdhNE]",
                72 => $"{oidPath}/[vc3TTPSinkR1-sdhNE]",
                73 => $"{oidPath}/[vc3TTPSourceR1-sdhNE]",
                74 => $"{oidPath}/[vc4TTPSinkR1-sdhNE]",
                75 => $"{oidPath}/[vc4TTPSourceR1-sdhNE]",
                76 => $"{oidPath}/[vcnUserChannelCTPSink-vc3TTPSinkR1]",
                77 => $"{oidPath}/[vcnUserChannelCTPSource-vc3TTPSourceR1]",
                78 => $"{oidPath}/[vcnUserChannelCTPSink-vc4TTPSinkR1]",
                79 => $"{oidPath}/[vcnUserChannelCTPSource-vc4TTPSourceR1]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // action
        #region 0.0.7.774.127.2.0.9.*

        oid_0_0_7_774_127_2_0_9:

            oidPath += "/[Action types]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[defineAUGStructure]",
                2 => $"{oidPath}/[defineVC4Structure]",
                3 => $"{oidPath}/[defineVC3Structure]",
                4 => $"{oidPath}/[defineTug3Structure]",
                5 => $"{oidPath}/[defineTug2Structure]",
                6 => $"{oidPath}/[defineClientType]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        // prot
        #region 0.0.7.774.127.3.*

        oid_0_0_7_774_127_3:

            oidPath += "/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_0_0_7_774_127_3_0;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // informationModel
        #region 0.0.7.774.127.3.0.*

        oid_0_0_7_774_127_3_0:

            oidPath += "/[Information model]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 2: goto oid_0_0_7_774_127_3_0_2;
                case 3: goto oid_0_0_7_774_127_3_0_3;
                case 4: goto oid_0_0_7_774_127_3_0_4;
                case 5: goto oid_0_0_7_774_127_3_0_5;
                case 6: goto oid_0_0_7_774_127_3_0_6;
                case 7: goto oid_0_0_7_774_127_3_0_7;
                case 9: goto oid_0_0_7_774_127_3_0_9;
                case 10: goto oid_0_0_7_774_127_3_0_10;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // asn1Module
        #region 0.0.7.774.127.3.0.2.*

        oid_0_0_7_774_127_3_0_2:

            oidPath += "/[ASN.1 modules]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[SDHProtASN1]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // managedObjectClass
        #region 0.0.7.774.127.3.0.3.*

        oid_0_0_7_774_127_3_0_3:

            oidPath += "/[Managed object classes]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[apsReportRecord]",
                2 => $"{oidPath}/[protectedTTPBidirectional]",
                3 => $"{oidPath}/[protectedTTPSink]",
                4 => $"{oidPath}/[protectedTTPSource]",
                5 => $"{oidPath}/[protectionGroup]",
                6 => $"{oidPath}/[protectionUnit]",
                7 => $"{oidPath}/[sdhMSProtectionGroup]",
                8 => $"{oidPath}/[sdhMSProtectionUnit]",
                9 => $"{oidPath}/[unprotectedCTPBidirectional]",
                10 => $"{oidPath}/[unprotectedCTPSink]",
                11 => $"{oidPath}/[unprotectedCTPSource]",
                12 => $"{oidPath}/[protectionGroupR1]",
                13 => $"{oidPath}/[sdhMSProtectionGroupR1]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // package
        #region 0.0.7.774.127.3.0.4.*

        oid_0_0_7_774_127_3_0_4:

            oidPath += "/[Packages]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[extraTrafficControlPkg]",
                2 => $"{oidPath}/[lastAttemptResultPkg]",
                3 => $"{oidPath}/[protectionSwitchExercisePkg]",
                4 => $"{oidPath}/[protectionMismatchStatusPkg]",
                5 => $"{oidPath}/[priorityPkg]",
                6 => $"{oidPath}/[sdhPriorityPkg]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // parameter
        #region 0.0.7.774.127.3.0.5.*

        oid_0_0_7_774_127_3_0_5:

            oidPath += "/[Parameters]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[invokeProtectionError]",
                2 => $"{oidPath}/[releaseProtectionError]",
                3 => $"{oidPath}/[protectionStatusParameter]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // nameBinding
        #region 0.0.7.774.127.3.0.6.*

        oid_0_0_7_774_127_3_0_6:

            oidPath += "/[Name bindings]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[protectedTTPBidirectional-sdhNE]",
                2 => $"{oidPath}/[protectedTTPSink-sdhNE]",
                3 => $"{oidPath}/[protectedTTPSource-sdhNE]",
                4 => $"{oidPath}/[protectionGroup-managedElement]",
                5 => $"{oidPath}/[augBidirectional-protectedTTPBidirectional]",
                6 => $"{oidPath}/[augSink-protectedTTPSink]",
                7 => $"{oidPath}/[augSource-protectedTTPSource]",
                8 => $"{oidPath}/[protectionUnit-protectionGroup]",
                9 => $"{oidPath}/[unprotectedCTPBidirectional-msTTPBidirectional]",
                10 => $"{oidPath}/[unprotectedCTPSink-msTTPSink]",
                11 => $"{oidPath}/[unprotectedCTPSource-msTTPSource]",
                12 => $"{oidPath}/[protectionGroupR1-managedElement]",
                13 => $"{oidPath}/[protectionUnit-protectionGroupR1]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // attribute
        #region 0.0.7.774.127.3.0.7.*

        oid_0_0_7_774_127_3_0_7:

            oidPath += "/[Attribute types]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[channelNumber]",
                2 => $"{oidPath}/[lastAttemptResult]",
                3 => $"{oidPath}/[priority]",
                4 => $"{oidPath}/[protectedTTPId]",
                5 => $"{oidPath}/[reportedProtectionUnit]",
                6 => $"{oidPath}/[protectionGroupId]",
                7 => $"{oidPath}/[protectionGroupType]",
                8 => $"{oidPath}/[protectionMismatchStatus]",
                9 => $"{oidPath}/[protectionStatus]",
                10 => $"{oidPath}/[protectionSwitchMode]",
                11 => $"{oidPath}/[protectionUnitId]",
                12 => $"{oidPath}/[protecting]",
                13 => $"{oidPath}/[reliableResourcePointer]",
                14 => $"{oidPath}/[revertive]",
                15 => $"{oidPath}/[sdhPriority]",
                16 => $"{oidPath}/[unprotectedCTPId]",
                17 => $"{oidPath}/[unreliableResourcePointer]",
                18 => $"{oidPath}/[waitToRestoreTime]",
                19 => $"{oidPath}/[notifiedProtectionUnit]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // action
        #region 0.0.7.774.127.3.0.9.*

        oid_0_0_7_774_127_3_0_9:

            oidPath += "/[Action types]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[invokeExercise]",
                2 => $"{oidPath}/[invokeProtection]",
                3 => $"{oidPath}/[releaseProtection]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // notification
        #region 0.0.7.774.127.3.0.10.*

        oid_0_0_7_774_127_3_0_10:

            oidPath += "/[Notifications]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[protectionSwitchReporting]",
                2 => $"{oidPath}/[protectionSwitchReportingR1]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        // sncp
        #region 0.0.7.774.127.4.*

        oid_0_0_7_774_127_4:

            oidPath += "/[Synchronous Digital Hierarchy (SDH) - Management of the subnetwork connection protection for the network element view]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_0_0_7_774_127_4_0;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // informationModel
        #region 0.0.7.774.127.4.0.*

        oid_0_0_7_774_127_4_0:

            oidPath += "/[Information model]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_0_0_7_774_127_4_0_0;
                case 2: goto oid_0_0_7_774_127_4_0_2;
                case 3: goto oid_0_0_7_774_127_4_0_3;
                case 4: goto oid_0_0_7_774_127_4_0_4;
                case 5: goto oid_0_0_7_774_127_4_0_5;
                case 6: goto oid_0_0_7_774_127_4_0_6;
                case 7: goto oid_0_0_7_774_127_4_0_7;
                case 9: goto oid_0_0_7_774_127_4_0_9;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // standardSpecificExtension
        #region 0.0.7.774.127.4.0.0.*

        oid_0_0_7_774_127_4_0_0:

            oidPath += "/[Standard specific extensions]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_0_0_7_774_127_4_0_0_0;
                case 1: return $"{oidPath}/[SubNetwork Connection Protection (SNCP) path trace mismatch criteria]";
                case 2: return $"{oidPath}/[SubNetwork Connection Protection (SNCP) excessive error criteria]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // sncpProtectionCriteria
        #region 0.0.7.774.127.4.0.0.0.*

        oid_0_0_7_774_127_4_0_0_0:

            oidPath += "/[sncpProtectionCriteria]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[sncpPathTraceMismatchCriteria]",
                2 => $"{oidPath}/[sncpExcessiveErrorCriteria]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // informationModel
        #region 0.0.7.774.127.4.0.2.*

        oid_0_0_7_774_127_4_0_2:

            oidPath += "/[ASN.1 modules]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[SDHSNCPASN1]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // managedObjectClass
        #region 0.0.7.774.127.4.0.3.*

        oid_0_0_7_774_127_4_0_3:

            oidPath += "/[Managed object classes]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[connectionProtectionGroup]",
                2 => $"{oidPath}/[connectionProtection]",
                3 => $"{oidPath}/[mpConnectionProtection]",
                4 => $"{oidPath}/[sncpFabric]",
                5 => $"{oidPath}/[connectionProtectionGroupR1]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // package
        #region 0.0.7.774.127.4.0.4.*

        oid_0_0_7_774_127_4_0_4:

            oidPath += "/[Packages]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[holdOffTimePackage]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // parameter
        #region 0.0.7.774.127.4.0.5.*

        oid_0_0_7_774_127_4_0_5:

            oidPath += "/[Parameters]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[switchStatusParameter]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // nameBinding
        #region 0.0.7.774.127.4.0.6.*

        oid_0_0_7_774_127_4_0_6:

            oidPath += "/[Name bindings]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[connectionProtection-connectionProtectionGroup]",
                2 => $"{oidPath}/[connectionProtectionGroup-sncpFabric]",
                3 => $"{oidPath}/[crossConnection-mpConnectionProtection]",
                4 => $"{oidPath}/[mpConnectionProtection-connectionProtectionGroup]",
                5 => $"{oidPath}/[crossConnection-sncpFabric]",
                6 => $"{oidPath}/[mpCrossConnection-sncpFabric]",
                7 => $"{oidPath}/[connectionProtection-connectionProtectionGroupR1]",
                8 => $"{oidPath}/[connectionProtectionGroupR1-sncpFabric]",
                9 => $"{oidPath}/[mpConnectionProtection-connectionProtectionGroupR1]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // attribute
        #region 0.0.7.774.127.4.0.7.*

        oid_0_0_7_774_127_4_0_7:

            oidPath += "/[Attribute types]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[holdOffTime]",
                2 => $"{oidPath}/[protectionCriteria]",
                3 => $"{oidPath}/[switchStatus]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // attribute
        #region 0.0.7.774.127.4.0.9.*

        oid_0_0_7_774_127_4_0_9:

            oidPath += "/[Action types]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[protectedConnect]",
                2 => $"{oidPath}/[protectUnprotect]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        // cs
        #region 0.0.7.774.127.5.*

        oid_0_0_7_774_127_5:

            oidPath += "/[Synchronous Digital Hierarchy (SDH) management of connection supervision functionality (HCS/LCS) for the network element view]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_0_0_7_774_127_5_0;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // informationModel
        #region 0.0.7.774.127.5.0.*

        oid_0_0_7_774_127_5_0:

            oidPath += "/[Information model]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 2: goto oid_0_0_7_774_127_5_0_2;
                case 3: goto oid_0_0_7_774_127_5_0_3;
                case 4: goto oid_0_0_7_774_127_5_0_4;
                case 6: goto oid_0_0_7_774_127_5_0_6;
                case 7: goto oid_0_0_7_774_127_5_0_7;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // asn1Module
        #region 0.0.7.774.127.5.0.2.*

        oid_0_0_7_774_127_5_0_2:

            oidPath += "/[ASN.1 modules]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[SDHCSASN1]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // managedObjectClass
        #region 0.0.7.774.127.5.0.3.*

        oid_0_0_7_774_127_5_0_3:

            oidPath += "/[Managed object classes]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                3 => $"{oidPath}/[au4SupervisedCTPSource]",
                6 => $"{oidPath}/[au3SupervisedCTPSource]",
                9 => $"{oidPath}/[tu3SupervisedCTPSource]",
                12 => $"{oidPath}/[tu2SupervisedCTPSource]",
                15 => $"{oidPath}/[tu12SupervisedCTPSource]",
                18 => $"{oidPath}/[tu11SupervisedCTPSource]",
                19 => $"{oidPath}/[au4SupervisedCTPBidirectionalR1]",
                20 => $"{oidPath}/[au4SupervisedCTPSinkR1]",
                21 => $"{oidPath}/[au3SupervisedCTPBidirectionalR1]",
                22 => $"{oidPath}/[au3SupervisedCTPSinkR1]",
                23 => $"{oidPath}/[tu3SupervisedCTPBidirectionalR1]",
                24 => $"{oidPath}/[tu3SupervisedCTPSinkR1]",
                25 => $"{oidPath}/[tu2SupervisedCTPBidirectionalR1]",
                26 => $"{oidPath}/[tu2SupervisedCTPSinkR1]",
                27 => $"{oidPath}/[tu12SupervisedCTPBidirectionalR1]",
                28 => $"{oidPath}/[tu12SupervisedCTPSinkR1]",
                29 => $"{oidPath}/[tu11SupervisedCTPBidirectionalR1]",
                30 => $"{oidPath}/[tu11SupervisedCTPSinkR1]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // package
        #region 0.0.7.774.127.5.0.4.*

        oid_0_0_7_774_127_5_0_4:

            oidPath += "/[Packages]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[vc11-2SupervisionBidirectionalPackage]",
                3 => $"{oidPath}/[vc11-2SupervisionSourcePackage]",
                4 => $"{oidPath}/[vc3-4SupervisionBidirectionalPackage]",
                6 => $"{oidPath}/[vc3-4SupervisionSourcePackage]",
                7 => $"{oidPath}/[vc11-2SupervisionSinkPackageR1]",
                8 => $"{oidPath}/[vc3-4SupervisionSinkPackageR1]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // nameBinding
        #region 0.0.7.774.127.5.0.6.*

        oid_0_0_7_774_127_5_0_6:

            oidPath += "/[Name bindings]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                7 => $"{oidPath}/[pathTerminationCurrentData-au4SupervisedCTPSinkR1]",
                8 => $"{oidPath}/[pathTerminationCurrentData-au3SupervisedCTPSinkR1]",
                9 => $"{oidPath}/[pathTerminationCurrentData-tu3SupervisedCTPSinkR1]",
                10 => $"{oidPath}/[pathTerminationCurrentData-tu2SupervisedCTPSinkR1]",
                11 => $"{oidPath}/[pathTerminationCurrentData-tu12SupervisedCTPSinkR1]",
                12 => $"{oidPath}/[pathTerminationCurrentData-tu11SupervisedCTPSinkR1]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // attribute
        #region 0.0.7.774.127.5.0.7.*

        oid_0_0_7_774_127_5_0_7:

            oidPath += "/[Attribute types]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[generatorEnabled]",
                2 => $"{oidPath}/[monitorActive]",
                3 => $"{oidPath}/[j1PathTraceReceive]",
                4 => $"{oidPath}/[j1PathTraceSend]",
                5 => $"{oidPath}/[j2PathTraceExpected]",
                6 => $"{oidPath}/[j2PathTraceReceive]",
                7 => $"{oidPath}/[j2PathTraceSend]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        // pmUni
        #region 0.0.7.774.127.6.*

        oid_0_0_7_774_127_6:

            oidPath += "/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_0_0_7_774_127_6_0;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // informationModel
        #region 0.0.7.774.127.6.0.*

        oid_0_0_7_774_127_6_0:

            oidPath += "}/[Information model]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 2: goto oid_0_0_7_774_127_6_0_2;
                case 3: goto oid_0_0_7_774_127_6_0_3;
                case 4: goto oid_0_0_7_774_127_6_0_4;
                case 6: goto oid_0_0_7_774_127_6_0_6;
                case 7: goto oid_0_0_7_774_127_6_0_7;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // asn1Module
        #region 0.0.7.774.127.6.0.2.*

        oid_0_0_7_774_127_6_0_2:

            oidPath += "/[ASN.1 modules]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[SDHPMUNIASN1]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // managedObjectClass
        #region 0.0.7.774.127.6.0.3.*

        oid_0_0_7_774_127_6_0_3:

            oidPath += "/[Managed object classes]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[sdhCurrentDataUnidirectional]",
                2 => $"{oidPath}/[msCurrentDataNearEnd]",
                3 => $"{oidPath}/[msCurrentDataNearEndTR]",
                4 => $"{oidPath}/[pathTerminationCurrentDataNearEnd]",
                5 => $"{oidPath}/[pathTerminationCurrentDataNearEndTR]",
                6 => $"{oidPath}/[msCurrentDataFarEnd]",
                7 => $"{oidPath}/[msCurrentDataFarEndTR]",
                8 => $"{oidPath}/[pathTerminationCurrentDataFarEnd]",
                9 => $"{oidPath}/[pathTerminationCurrentDataFarEndTR]",
                10 => $"{oidPath}/[msHistoryDataNearEnd]",
                11 => $"{oidPath}/[pathTerminationHistoryDataNearEnd]",
                12 => $"{oidPath}/[msHistoryDataFarEnd]",
                13 => $"{oidPath}/[pathTerminationHistoryDataFarEnd]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // package
        #region 0.0.7.774.127.6.0.4.*

        oid_0_0_7_774_127_6_0_4:

            oidPath += "/[Packages]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[nearEndUASCurrentDataPackage]",
                2 => $"{oidPath}/[farEndUASCurrentDataPackage]",
                3 => $"{oidPath}/[nearEndUASHistoryDataPackage]",
                4 => $"{oidPath}/[farEndUASHistoryDataPackage]",
                5 => $"{oidPath}/[failureCountsNearEndPackage]",
                6 => $"{oidPath}/[eSANearEndPackage]",
                7 => $"{oidPath}/[eSBNearEndPackage]",
                8 => $"{oidPath}/[failureCountsFarEndPackage]",
                9 => $"{oidPath}/[eSAFarEndPackage]",
                10 => $"{oidPath}/[eSBFarEndPackage]",
                11 => $"{oidPath}/[failureCountsNearEndHistoryDataPackage]",
                12 => $"{oidPath}/[eSANearEndHistoryDataPackage]",
                13 => $"{oidPath}/[eSBNearEndHistoryDataPackage]",
                14 => $"{oidPath}/[failureCountsFarEndHistoryDataPackage]",
                15 => $"{oidPath}/[eSAFarEndHistoryDataPackage]",
                16 => $"{oidPath}/[eSBFarEndHistoryDataPackage]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // nameBinding
        #region 0.0.7.774.127.6.0.6.*

        oid_0_0_7_774_127_6_0_6:

            oidPath += "/[Name bindings]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[msCurrentDataNearEnd-msTTPSink]",
                2 => $"{oidPath}/[msCurrentDataNearEndTR-msTTPSink]",
                13 => $"{oidPath}/[msCurrentDataFarEnd-msTTPSink]",
                14 => $"{oidPath}/[msCurrentDataFarEndTR-msTTPSink]",
                49 => $"{oidPath}/[pathTerminationCurrentDataNearEnd-vc4TTPSinkR1]",
                50 => $"{oidPath}/[pathTerminationCurrentDataNearEnd-vc3TTPSinkR1]",
                51 => $"{oidPath}/[pathTerminationCurrentDataNearEnd-vc2TTPSinkR1]",
                52 => $"{oidPath}/[pathTerminationCurrentDataNearEnd-vc12TTPSinkR1]",
                53 => $"{oidPath}/[pathTerminationCurrentDataNearEnd-vc11TTPSinkR1]",
                54 => $"{oidPath}/[pathTerminationCurrentDataNearEndTR-vc4TTPSinkR1]",
                55 => $"{oidPath}/[pathTerminationCurrentDataNearEndTR-vc3TTPSinkR1]",
                56 => $"{oidPath}/[pathTerminationCurrentDataNearEndTR-vc2TTPSinkR1]",
                57 => $"{oidPath}/[pathTerminationCurrentDataNearEndTR-vc12TTPSinkR1]",
                58 => $"{oidPath}/[pathTerminationCurrentDataNearEndTR-vc11TTPSinkR1]",
                59 => $"{oidPath}/[pathTerminationCurrentDataFarEnd-vc4TTPSinkR1]",
                60 => $"{oidPath}/[pathTerminationCurrentDataFarEnd-vc3TTPSinkR1]",
                61 => $"{oidPath}/[pathTerminationCurrentDataFarEnd-vc2TTPSinkR1]",
                62 => $"{oidPath}/[pathTerminationCurrentDataFarEnd-vc12TTPSinkR1]",
                63 => $"{oidPath}/[pathTerminationCurrentDataFarEnd-vc11TTPSinkR1]",
                64 => $"{oidPath}/[pathTerminationCurrentDataFarEndTR-vc4TTPSinkR1]",
                65 => $"{oidPath}/[pathTerminationCurrentDataFarEndTR-vc3TTPSinkR1]",
                66 => $"{oidPath}/[pathTerminationCurrentDataFarEndTR-vc2TTPSinkR1]",
                67 => $"{oidPath}/[pathTerminationCurrentDataFarEndTR-vc12TTPSinkR1]",
                68 => $"{oidPath}/[pathTerminationCurrentDataFarEndTR-vc11TTPSinkR1]",
                69 => $"{oidPath}/[pathTerminationCurrentDataNearEnd-au4SupervisedCTPSinkR1]",
                70 => $"{oidPath}/[pathTerminationCurrentDataNearEnd-au3SupervisedCTPSinkR1]",
                71 => $"{oidPath}/[pathTerminationCurrentDataNearEnd-tu3SupervisedCTPSinkR1]",
                72 => $"{oidPath}/[pathTerminationCurrentDataNearEnd-tu2SupervisedCTPSinkR1]",
                73 => $"{oidPath}/[pathTerminationCurrentDataNearEnd-tu12SupervisedCTPSinkR1]",
                74 => $"{oidPath}/[pathTerminationCurrentDataNearEnd-tu11SupervisedCTPSinkR1]",
                75 => $"{oidPath}/[pathTerminationCurrentDataFarEnd-au4SupervisedCTPSinkR1]",
                76 => $"{oidPath}/[pathTerminationCurrentDataFarEnd-au3SupervisedCTPSinkR1]",
                77 => $"{oidPath}/[pathTerminationCurrentDataFarEnd-tu3SupervisedCTPSinkR1]",
                78 => $"{oidPath}/[pathTerminationCurrentDataFarEnd-tu2SupervisedCTPSinkR1]",
                79 => $"{oidPath}/[pathTerminationCurrentDataFarEnd-tu12SupervisedCTPSinkR1]",
                80 => $"{oidPath}/[pathTerminationCurrentDataFarEnd-tu11SupervisedCTPSinkR1]",
                81 => $"{oidPath}/[pathTerminationCurrentDataNearEndTR-au4SupervisedCTPSinkR1]",
                82 => $"{oidPath}/[pathTerminationCurrentDataNearEndTR-au3SupervisedCTPSinkR1]",
                83 => $"{oidPath}/[pathTerminationCurrentDataNearEndTR-tu3SupervisedCTPSinkR1]",
                84 => $"{oidPath}/[pathTerminationCurrentDataNearEndTR-tu2SupervisedCTPSinkR1]",
                85 => $"{oidPath}/[pathTerminationCurrentDataNearEndTR-tu12SupervisedCTPSinkR1]",
                86 => $"{oidPath}/[pathTerminationCurrentDataNearEndTR-tu11SupervisedCTPSinkR1]",
                87 => $"{oidPath}/[pathTerminationCurrentDataFarEndTR-au4SupervisedCTPSinkR1]",
                88 => $"{oidPath}/[pathTerminationCurrentDataFarEndTR-au3SupervisedCTPSinkR1]",
                89 => $"{oidPath}/[pathTerminationCurrentDataFarEndTR-tu3SupervisedCTPSinkR1]",
                90 => $"{oidPath}/[pathTerminationCurrentDataFarEndTR-tu2SupervisedCTPSinkR1]",
                91 => $"{oidPath}/[pathTerminationCurrentDataFarEndTR-tu12SupervisedCTPSinkR1]",
                92 => $"{oidPath}/[pathTerminationCurrentDataFarEndTR-tu11SupervisedCTPSinkR1]",
                93 => $"{oidPath}/[msAdaptationCurrentData-au4CTPSource]",
                94 => $"{oidPath}/[msAdaptationCurrentData-au3CTPSource]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // attribute
        #region 0.0.7.774.127.6.0.7.*

        oid_0_0_7_774_127_6_0_7:

            oidPath += "/[Attribute types]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[nEUAS]",
                2 => $"{oidPath}/[fEUAS]",
                3 => $"{oidPath}/[fCNearEnd]",
                4 => $"{oidPath}/[eSANearEnd]",
                5 => $"{oidPath}/[eSBNearEnd]",
                6 => $"{oidPath}/[fCFarEnd]",
                7 => $"{oidPath}/[eSAFarEnd]",
                8 => $"{oidPath}/[eSBFarEnd]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        // ptl
        #region 0.0.7.774.127.7.*

        oid_0_0_7_774_127_7:

            oidPath += "/[Synchronous Digital Hierarchy (SDH) - Management of lower order path trace and interface labelling for the network element view]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_0_0_7_774_127_7_0;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // informationModel
        #region 0.0.7.774.127.7.0.*

        oid_0_0_7_774_127_7_0:

            oidPath += "/[Information model]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 2: goto oid_0_0_7_774_127_7_0_2;
                case 3: goto oid_0_0_7_774_127_7_0_3;
                case 4: return $"{oidPath}";
                case 5: return $"{oidPath}";
                case 6: return $"{oidPath}";
                case 7: return $"{oidPath}";
                case 9: return $"{oidPath}";
                case 10: return $"{oidPath}/[ANotifications]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // asn1Module
        #region 0.0.7.774.127.7.0.2.*

        oid_0_0_7_774_127_7_0_2:

            oidPath += "/[ASN.1 modules]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[SDHPTLASN1]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // managedObjectClass
        #region 0.0.7.774.127.7.0.3.*

        oid_0_0_7_774_127_7_0_3:

            oidPath += "/[Managed object classes]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[labelledElectricalSPITTPBidirectional]",
                2 => $"{oidPath}/[labelledElectricalSPITTPSink]",
                3 => $"{oidPath}/[labelledElectricalSPITTPSource]",
                4 => $"{oidPath}/[labelledOpticalSPITTPBidirectional]",
                5 => $"{oidPath}/[labelledOpticalSPITTPSink]",
                6 => $"{oidPath}/[labelledOpticalSPITTPSource]",
                7 => $"{oidPath}/[vc11PathTraceTTPBidirectional]",
                8 => $"{oidPath}/[vc11PathTraceTTPSink]",
                9 => $"{oidPath}/[vc11PathTraceTTPSource]",
                10 => $"{oidPath}/[vc12PathTraceTTPBidirectional]",
                11 => $"{oidPath}/[vc12PathTraceTTPSink]",
                12 => $"{oidPath}/[vc12PathTraceTTPSource]",
                13 => $"{oidPath}/[vc2PathTraceTTPBidirectional]",
                14 => $"{oidPath}/[vc2PathTraceTTPSink]",
                15 => $"{oidPath}/[vc2PathTraceTTPSource]",
                16 => $"{oidPath}/[modifiableVC2PathTraceTTPBidirectional]",
                17 => $"{oidPath}/[modifiableVC2PathTraceTTPSink]",
                18 => $"{oidPath}/[modifiableVC2PathTraceTTPSource]",
                19 => $"{oidPath}/[modifiableVC12PathTraceTTPBidirectional]",
                20 => $"{oidPath}/[modifiableVC12PathTraceTTPSink]",
                21 => $"{oidPath}/[modifiableVC12PathTraceTTPSource]",
                22 => $"{oidPath}/[modifiableVC11PathTraceTTPBidirectional]",
                23 => $"{oidPath}/[modifiableVC11PathTraceTTPSink]",
                24 => $"{oidPath}/[modifiableVC11PathTraceTTPSource]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        // sdhRadioIM
        #region 0.0.7.774.127.8.*

        oid_0_0_7_774_127_8:

            oidPath += "/[Synchronous Digital Hierarchy (SDH) - Management of radio-relay systems for the network element view]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_0_0_7_774_127_8_0;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // informationModel
        #region 0.0.7.774.127.8.0.*

        oid_0_0_7_774_127_8_0:

            oidPath += "/[Information model]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 2: goto oid_0_0_7_774_127_8_0_2;
                case 3: goto oid_0_0_7_774_127_8_0_3;
                case 4: goto oid_0_0_7_774_127_8_0_4;
                case 5: goto oid_0_0_7_774_127_8_0_5;
                case 6: goto oid_0_0_7_774_127_8_0_6;
                case 7: goto oid_0_0_7_774_127_8_0_7;
                case 9: goto oid_0_0_7_774_127_8_0_9;
                case 10: return $"{oidPath}/[Notifications]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // asn1Module
        #region 0.0.7.774.127.8.0.2.*

        oid_0_0_7_774_127_8_0_2:

            oidPath += "/[ASN.1 modules]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[SDHRadioTpASN1]",
                1 => $"{oidPath}/[SDHRadioProtASN1]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // managedObjectClass
        #region 0.0.7.774.127.8.0.3.*

        oid_0_0_7_774_127_8_0_3:

            oidPath += "/[Managed object classes]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[radioSPITTPBidirectional]",
                2 => $"{oidPath}/[radioSPITTPSink]",
                3 => $"{oidPath}/[radioSPITTPSource]",
                4 => $"{oidPath}/[sdhRadioProtectionGroup]",
                5 => $"{oidPath}/[sdhRadioProtectionUnit]",
                6 => $"{oidPath}/[msTcCTPBidirectional]",
                7 => $"{oidPath}/[msTcCTPSink]",
                8 => $"{oidPath}/[msTcCTPSource]",
                9 => $"{oidPath}/[msTcTTPBidirectional]",
                10 => $"{oidPath}/[msTcTTPSink]",
                11 => $"{oidPath}/[msTcTTPSource]",
                12 => $"{oidPath}/[au4HopcCTPBidirectional]",
                13 => $"{oidPath}/[au4HopcCTPSink]",
                14 => $"{oidPath}/[au4HopcCTPSource]",
                15 => $"{oidPath}/[vc4HopcTTPBidirectional]",
                16 => $"{oidPath}/[vc4HopcTTPSink]",
                17 => $"{oidPath}/[vc4HopcTTPSource]",
                18 => $"{oidPath}/[radioUnprotectedCTPBidirectional]",
                19 => $"{oidPath}/[radioUnprotectedCTPSink]",
                20 => $"{oidPath}/[radioUnprotectedCTPSource]",
                21 => $"{oidPath}/[radioProtectedTTPBidirectional]",
                22 => $"{oidPath}/[radioProtectedTTPSink]",
                23 => $"{oidPath}/[radioProtectedTTPSource]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // package
        #region 0.0.7.774.127.8.0.4.*

        oid_0_0_7_774_127_8_0_4:

            oidPath += "/[Packages]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                2 => $"{oidPath}/[atpcPackage]",
                3 => $"{oidPath}/[rxLOSNotificationPackage]",
                4 => $"{oidPath}/[demLOSNotificationPackage]",
                5 => $"{oidPath}/[txLOSNotificationPackage]",
                6 => $"{oidPath}/[modLOSNotificationPackage]",
                7 => $"{oidPath}/[exerciseOnOffPkg]",
                8 => $"{oidPath}/[singleExercisePkg]",
                9 => $"{oidPath}/[privilegedChannelPkg]",
                10 => $"{oidPath}/[radioHoldOffTimePkg]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // parameter
        #region 0.0.7.774.127.8.0.5.*

        oid_0_0_7_774_127_8_0_5:

            oidPath += "/[Parameters]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[radioProtectionStatusParameter]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // nameBinding
        #region 0.0.7.774.127.8.0.6.*

        oid_0_0_7_774_127_8_0_6:

            oidPath += "/[Name bindings]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[radioSPITTPSink-managedElement]",
                2 => $"{oidPath}/[radioSPITTPSink-managedElement]",
                3 => $"{oidPath}/[rsCTPSink-radioSPITTPSink]",
                4 => $"{oidPath}/[rsCTPSource-radioSPITTPSource]",
                5 => $"{oidPath}/[augSink-msTcTTPSink]",
                6 => $"{oidPath}/[augSource-msTcTTPSource]",
                7 => $"{oidPath}/[msTcCTPSink-rsTTPSink]",
                8 => $"{oidPath}/[msTcCTPSource-rsTTPSource]",
                9 => $"{oidPath}/[msTcTTPSink-sdhNE]",
                10 => $"{oidPath}/[msTcTTPSource-sdhNE]",
                11 => $"{oidPath}/[vc4HopcTTPSink-sdhNE]",
                12 => $"{oidPath}/[vc4HopcTTPSource-sdhNE]",
                13 => $"{oidPath}/[au4HopcCTPSink-augSink]",
                14 => $"{oidPath}/[au4HopcCTPSource-augSource]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // attribute
        #region 0.0.7.774.127.8.0.7.*

        oid_0_0_7_774_127_8_0_7:

            oidPath += "/[Attribute types]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[atpcImplemented]",
                2 => $"{oidPath}/[atpcEnabled]",
                3 => $"{oidPath}/[radioFrequency]",
                4 => $"{oidPath}/[radioSPITTPId]",
                5 => $"{oidPath}/[hitless]",
                6 => $"{oidPath}/[radioHoldOffTime]",
                7 => $"{oidPath}/[rpsSummaryStatus]",
                8 => $"{oidPath}/[exerciseOn]",
                9 => $"{oidPath}/[privilegedChannel]",
                10 => $"{oidPath}/[radioProtectionStatus]",
                11 => $"{oidPath}/[radioUnprotectedCTPId]",
                12 => $"{oidPath}/[radioProtectedTTPId]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // action
        #region 0.0.7.774.127.8.0.9.*

        oid_0_0_7_774_127_8_0_9:

            oidPath += "/[Action types]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[invokeRadioExercise]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        // protCoord
        #region 0.0.7.774.127.9.*

        oid_0_0_7_774_127_9:

            oidPath += "/[Synchronous Digital Hierarchy (SDH) - Configuration of linear multiplex-section protection for the network element view]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_0_0_7_774_127_9_0;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // informationModel
        #region 0.0.7.774.127.9.0.*

        oid_0_0_7_774_127_9_0:

            oidPath += "/[Information model]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 2: goto oid_0_0_7_774_127_9_0_2;
                case 3: goto oid_0_0_7_774_127_9_0_3;
                case 5: goto oid_0_0_7_774_127_9_0_5;
                case 6: goto oid_0_0_7_774_127_9_0_6;
                case 7: goto oid_0_0_7_774_127_9_0_7;
                case 9: goto oid_0_0_7_774_127_9_0_9;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // asn1Module
        #region 0.0.7.774.127.9.0.2.*

        oid_0_0_7_774_127_9_0_2:

            oidPath += "/[ASN.1 modules]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[SDHProtCoordASN1]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // managedObjectClass
        #region 0.0.7.774.127.9.0.3.*

        oid_0_0_7_774_127_9_0_3:

            oidPath += "/[Managed object classes]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[protectionCoordinator]",
                2 => $"{oidPath}/[sdhMSProtectionCoordinator]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // parameter
        #region 0.0.7.774.127.9.0.5.*

        oid_0_0_7_774_127_9_0_5:

            oidPath += "/[Parameters]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[mSPConfigurationError]",
                2 => $"{oidPath}/[mSPGroupConfigurationParameter]",
                3 => $"{oidPath}/[mSPUnitConfigurationParameter]",
                4 => $"{oidPath}/[protectionConfigurationError]",
                5 => $"{oidPath}/[removeProtectionError]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // nameBinding
        #region 0.0.7.774.127.9.0.6.*

        oid_0_0_7_774_127_9_0_6:

            oidPath += "/[Name bindings]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[protectionCoordinator-sdhNE]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // nameBinding
        #region 0.0.7.774.127.9.0.7.*

        oid_0_0_7_774_127_9_0_7:

            oidPath += "/[Attribute types]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[protectionCoordinatorId]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // action
        #region 0.0.7.774.127.9.0.9.*

        oid_0_0_7_774_127_9_0_9:

            oidPath += "/[Action types]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[dismissProtection]",
                2 => $"{oidPath}/[establishProtection]",
                3 => $"{oidPath}/[modifyProtection]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        // msspr
        #region 0.0.7.774.127.10.*

        oid_0_0_7_774_127_10:

            oidPath += "/[Synchronous Digital Hierarchy (SDH) Multiplex Section (MS) shared protection ring management for the network element view]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_0_0_7_774_127_10_0;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // informationModel
        #region 0.0.7.774.127.10.0.*

        oid_0_0_7_774_127_10_0:

            oidPath += "/[Information model]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_0_0_7_774_127_10_0_0;
                case 2: goto oid_0_0_7_774_127_10_0_2;
                case 3: goto oid_0_0_7_774_127_10_0_3;
                case 4: goto oid_0_0_7_774_127_10_0_4;
                case 5: goto oid_0_0_7_774_127_10_0_5;
                case 6: goto oid_0_0_7_774_127_10_0_6;
                case 7: goto oid_0_0_7_774_127_10_0_7;
                case 9: goto oid_0_0_7_774_127_10_0_9;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // standardSpecificExtension
        #region 0.0.7.774.127.10.0.0.*

        oid_0_0_7_774_127_10_0_0:

            oidPath += "/[Standard specific extensions]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_0_0_7_774_127_10_0_0_0;
                case 1: goto oid_0_0_7_774_127_10_0_0_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // mssprProtectionCriteria
        #region 0.0.7.774.127.10.0.0.0.*

        oid_0_0_7_774_127_10_0_0_0:

            oidPath += "/[mssprProtectionCriteria]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[mssprExcessiveErrorCriteria]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // msSPRProbableCause
        #region 0.0.7.774.127.10.0.0.1.*

        oid_0_0_7_774_127_10_0_0_1:

            oidPath += "/[msSPRProbableCause]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[msSPRDefaultKBytes]",
                2 => $"{oidPath}/[msSPRInconsistentAPSCodes]",
                3 => $"{oidPath}/[msSPRNodeIdMismatch]",
                4 => $"{oidPath}/[msSPRImproperAPSCodes]",
                5 => $"{oidPath}/[msSPRApsChannelProcessingFailure]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // asn1Module
        #region 0.0.7.774.127.10.0.2.*

        oid_0_0_7_774_127_10_0_2:

            oidPath += "/[ASN.1 modules]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[SDHMSSPRASN1]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // managedObjectClass
        #region 0.0.7.774.127.10.0.3.*

        oid_0_0_7_774_127_10_0_3:

            oidPath += "/[Managed object classes]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[auSquelchTable]",
                2 => $"{oidPath}/[msSPRProtectionGroup]",
                3 => $"{oidPath}/[msSPRProtectionUnit]",
                4 => $"{oidPath}/[nutTable]",
                5 => $"{oidPath}/[ripTable]",
                6 => $"{oidPath}/[sdhMSSPRProtectionCoordinator]",
                7 => $"{oidPath}/[sPRingManager]",
                8 => $"{oidPath}/[squelchTable]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // package
        #region 0.0.7.774.127.10.0.4.*

        oid_0_0_7_774_127_10_0_4:

            oidPath += "/[Packages]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[changeSPRConfigurationPkg]",
                2 => $"{oidPath}/[enhancedWtrSpanPkg]",
                3 => $"{oidPath}/[fourFiberPUPkg]",
                4 => $"{oidPath}/[manualSPRConfigurationPkg]",
                5 => $"{oidPath}/[restoreExtraTrafficPkg]",
                6 => $"{oidPath}/[wtrSpanPkg]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // parameter
        #region 0.0.7.774.127.10.0.5.*

        oid_0_0_7_774_127_10_0_5:

            oidPath += "/[Parameters]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[msSPRConfigurationError]",
                2 => $"{oidPath}/[msSPRLockoutTypeParameter]",
                3 => $"{oidPath}/[msSPRProtectionGroupConfigParameter]",
                4 => $"{oidPath}/[msSPRProtectionStatusParameter]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // nameBinding
        #region 0.0.7.774.127.10.0.6.*

        oid_0_0_7_774_127_10_0_6:

            oidPath += "/[Name bindings]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[auSquelchTable-squelchTable]",
                2 => $"{oidPath}/[nutTable-sPRingManager]",
                3 => $"{oidPath}/[ripTable-sPRingManager]",
                4 => $"{oidPath}/[sPRingManager-managedElement]",
                5 => $"{oidPath}/[squelchTable-sPRingManager]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // attribute
        #region 0.0.7.774.127.10.0.7.*

        oid_0_0_7_774_127_10_0_7:

            oidPath += "/[Attribute types]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[auTable]",
                2 => $"{oidPath}/[auNumber]",
                3 => $"{oidPath}/[currentSquelchingList]",
                4 => $"{oidPath}/[directionTable]",
                5 => $"{oidPath}/[enhancedWaitToRestoreTimeSpan]",
                6 => $"{oidPath}/[msSPRProtectionStatus]",
                7 => $"{oidPath}/[nodeNumber]",
                8 => $"{oidPath}/[nutChannelList]",
                9 => $"{oidPath}/[nutTableId]",
                10 => $"{oidPath}/[protectionGroupPointer]",
                11 => $"{oidPath}/[restoreExtraTraffic]",
                12 => $"{oidPath}/[ringId]",
                13 => $"{oidPath}/[ringMap]",
                14 => $"{oidPath}/[ringPU]",
                15 => $"{oidPath}/[ripChannelList]",
                16 => $"{oidPath}/[ripTableId]",
                17 => $"{oidPath}/[spanPU]",
                18 => $"{oidPath}/[sPRingApplication]",
                19 => $"{oidPath}/[sPRingManagerId]",
                20 => $"{oidPath}/[squelchTableId]",
                21 => $"{oidPath}/[waitToRestoreTimeSpan]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // action
        #region 0.0.7.774.127.10.0.9.*

        oid_0_0_7_774_127_10_0_9:

            oidPath += "/[Action types]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[changeSPRConfiguration]",
                2 => $"{oidPath}/[updateRipTable]",
                3 => $"{oidPath}/[updateSquelchTable]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        #endregion

        #endregion

        // g722-1, g7221
        #region 0.0.7.7221.*

        oid_0_0_7_7221:

            oidPath += "/[Coding at 24 and 32 kbit/s for hands-free operation in systems with low frame loss]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_0_0_7_7221_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // generic-capabilities
        #region 0.0.7.7221.1.*

        oid_0_0_7_7221_1:

            oidPath += "/[Generic capabilities]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: return $"{oidPath}/[Standard capability identifier]";
                case 2: goto oid_0_0_7_7221_1_2;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // generic-capabilities
        #region 0.0.7.7221.1.2.*

        oid_0_0_7_7221_1_2:

            oidPath += "/[Extended modes]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Extended mode capability]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        // 7222
        #region 0.0.7.7222.*

        oid_0_0_7_7222:

            oidPath += "/[Wideband coding of speech at around 16 kbit/s using Adaptive Multi-Rate Wideband (AMR-WB)]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_0_0_7_7222_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // 7222
        #region 0.0.7.7222.1.*

        oid_0_0_7_7222_1:

            oidPath += "/[Generic capabilities]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[First generic capability]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // g776-1, g7761
        #region 0.0.7.7761.*

        oid_0_0_7_7761:

            oidPath += "/[Managed objects for signal processing network elements]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 7: goto oid_0_0_7_7761_7;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // attribute
        #region 0.0.7.7761.7.*

        oid_0_0_7_7761_7:

            oidPath += "/[Attribute types]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                235 => $"{oidPath}/[facsimileDemodulation]",
                317 => $"{oidPath}/[facsimileDemodulationDS0]",
                318 => $"{oidPath}/[q50aSimpDLConAbcd]",
                319 => $"{oidPath}/[nssNotSupportedManufacturerCode]",
                320 => $"{oidPath}/[nssNotSupportedMachineCode]",
                321 => $"{oidPath}/[bcAssignment]",
                322 => $"{oidPath}/[alarmSeverityClassification]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // gntm
        #region 0.0.7.85501.*

        oid_0_0_7_85501:

            oidPath += "/[GDMO engineering viewpoint for the generic network level model]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_0_0_7_85501_0;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // informationModel
        #region 0.0.7.85501.0.*

        oid_0_0_7_85501_0:

            oidPath += "/[Information model]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 2: goto oid_0_0_7_85501_0_2;
                case 3: goto oid_0_0_7_85501_0_3;
                case 4: goto oid_0_0_7_85501_0_4;
                case 6: return $"{oidPath}";
                case 7: return $"{oidPath}/[Attribute types]";
                case 9: goto oid_0_0_7_85501_0_9;
                case 10: return $"{oidPath}/[Notifications]";
                case 12: goto oid_0_0_7_85501_0_12;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // asn1Modules
        #region 0.0.7.85501.0.2.*

        oid_0_0_7_85501_0_2:

            oidPath += "/[ASN.1 modules]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[G85501-ASN1TypeModule]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // managedObjectClass
        #region 0.0.7.85501.0.3.*

        oid_0_0_7_85501_0_3:

            oidPath += "/[Managed object classes]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[basicLayerNetworkDomain]",
                2 => $"{oidPath}/[basicSubNetwork]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // package
        #region 0.0.7.85501.0.4.*

        oid_0_0_7_85501_0_4:

            oidPath += "/[Packages]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[basicConnectionPerformerPackage]",
                2 => $"{oidPath}/[basicTrailHandlerPackage]",
                3 => $"{oidPath}/[logicalLinkEndHandlerPackage]",
                4 => $"{oidPath}/[logicalLinkHandlerPackage]",
                5 => $"{oidPath}/[topologicalLinkEndHandlerPackage]",
                6 => $"{oidPath}/[topologicalLinkHandlerPackage]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // action
        #region 0.0.7.85501.0.9.*

        oid_0_0_7_85501_0_9:

            oidPath += "/[Action types]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[associateNetworkTTPWithTopologicalLinkEnd]",
                2 => $"{oidPath}/[associateTrailWithTopologicalLink]",
                3 => $"{oidPath}/[disassociateNetworkTTPFromTopologicalLinkEnd]",
                4 => $"{oidPath}/[disassociateTrailFromTopologicalLink]",
                5 => $"{oidPath}/[establishLogicalLink]",
                6 => $"{oidPath}/[establishLogicalLinkAndEnds]",
                7 => $"{oidPath}/[establishTopologicalLink]",
                8 => $"{oidPath}/[establishTopologicalLinkAndEnds]",
                9 => $"{oidPath}/[releaseSnc]",
                10 => $"{oidPath}/[releaseTrail]",
                11 => $"{oidPath}/[removeLogicalLink]",
                12 => $"{oidPath}/[removeLogicalLinkAndEnds]",
                13 => $"{oidPath}/[removeTopologicalLink]",
                14 => $"{oidPath}/[removeTopologicalLinkAndEnds]",
                15 => $"{oidPath}/[setupSnc]",
                16 => $"{oidPath}/[setupTrail]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // specificError
        #region 0.0.7.85501.0.12.*

        oid_0_0_7_85501_0_12:

            oidPath += "/[Specific errors]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[aEndNetworkTPConnected]",
                2 => $"{oidPath}/[capacityProvisionned]",
                3 => $"{oidPath}/[consistencyFailure]",
                4 => $"{oidPath}/[failureToAssociate]",
                5 => $"{oidPath}/[failureToConnect]",
                6 => $"{oidPath}/[failureToCreateTopologicalLink]",
                7 => $"{oidPath}/[failureToDeleteLink]",
                8 => $"{oidPath}/[failureToDeleteTopologicalLinkEnd]",
                9 => $"{oidPath}/[failureToDisassociate]",
                10 => $"{oidPath}/[failureToRelease]",
                11 => $"{oidPath}/[failureToSetDirectionality]",
                13 => $"{oidPath}/[finalCapacitiesFailure]",
                14 => $"{oidPath}/[incorrectLink]",
                15 => $"{oidPath}/[incorrectLinkEnds]",
                16 => $"{oidPath}/[incorrectSubnetwork]",
                17 => $"{oidPath}/[incorrectSubnetworkTerminationPoints]",
                18 => $"{oidPath}/[initialCapacitiesFailure]",
                19 => $"{oidPath}/[invalidTrafficDescriptor]",
                20 => $"{oidPath}/[invalidTrail]",
                21 => $"{oidPath}/[invalidTransportServiceCharacteristics]",
                23 => $"{oidPath}/[linkAndTrailsNotCompatible]",
                24 => $"{oidPath}/[linkEndAndNetworkTTPsNotCompatible]",
                25 => $"{oidPath}/[linkConnectionsExisting]",
                26 => $"{oidPath}/[networkCTPsExisting]",
                27 => $"{oidPath}/[networkTTPAlreadyAssociated]",
                28 => $"{oidPath}/[networkTTPNotAssociated]",
                29 => $"{oidPath}/[networkTTPsInAEndAccessGroupConnected]",
                30 => $"{oidPath}/[networkTTPsInZEndAccessGroupConnected]",
                31 => $"{oidPath}/[networkTTPsNotPartOfLayerND]",
                32 => $"{oidPath}/[noSuchNetworkTTP]",
                33 => $"{oidPath}/[noSuchSnc]",
                34 => $"{oidPath}/[noSuchTrail]",
                35 => $"{oidPath}/[trailAlreadyAssociated]",
                36 => $"{oidPath}/[sncConnected]",
                37 => $"{oidPath}/[trailConnected]",
                38 => $"{oidPath}/[trailNotAssociated]",
                39 => $"{oidPath}/[unknownSnc]",
                40 => $"{oidPath}/[unknownTrail]",
                41 => $"{oidPath}/[userIdentifierNotUnique]",
                42 => $"{oidPath}/[wrongAEndDirectionality]",
                43 => $"{oidPath}/[wrongZEndDirectionality]",
                44 => $"{oidPath}/[zEndNetworkTPConnected]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        #endregion

        // h
        #region 0.0.8.*

        oid_0_0_8:

            oidPath += "/H";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 224: goto oid_0_0_8_224;
                case 230: goto oid_0_0_8_230;
                case 235: goto oid_0_0_8_235;
                case 239: goto oid_0_0_8_239;
                case 241: goto oid_0_0_8_241;
                case 245: goto oid_0_0_8_245;
                case 248: goto oid_0_0_8_248;
                case 249: goto oid_0_0_8_249;
                case 261: goto oid_0_0_8_261;
                case 263: goto oid_0_0_8_263;
                case 282: goto oid_0_0_8_282;
                case 283: goto oid_0_0_8_283;
                case 323: goto oid_0_0_8_323;
                case 324: goto oid_0_0_8_324;
                case 341: goto oid_0_0_8_341;
                //TODO: case 350: goto oid_0_0_8_350;
                //TODO: case 450: goto oid_0_0_8_450;
                //TODO: case 460: goto oid_0_0_8_460;
                //TODO: case 641: goto oid_0_0_8_641;
                //TODO: case 2250: goto oid_0_0_8_2250;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // h224
        #region 0.0.8.224.*

        oid_0_0_8_224:

            oidPath += "/[A real time control protocol for simplex applications using the H.221 LSD/HSD/HLP channels]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_0_0_8_224_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // generic-capabilities
        #region 0.0.8.224.1.*

        oid_0_0_8_224_1:

            oidPath += "/[Generic capability identifier]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[First generic capability]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // 230
        #region 0.0.8.230.*

        oid_0_0_8_230:

            oidPath += "/[Frame-synchronous control and indication signals for audiovisual systems]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                2 => $"{oidPath}/[Generic message]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // h235
        #region 0.0.8.235.*

        oid_0_0_8_235:

            oidPath += "/[Security and encryption for H-series (H.323 and other H.245-based) multimedia terminals]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_0_0_8_235_0;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // version
        #region 0.0.8.235.0.*

        oid_0_0_8_235_0:

            oidPath += "/[Versions]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_0_0_8_235_0_1;
                case 2: goto oid_0_0_8_235_0_2;
                case 3: goto oid_0_0_8_235_0_3;
                case 9: return $"{oidPath}/[End-to-end ClearToken carrying sendersID for verification at the recipient side]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // 1
        #region 0.0.8.235.0.1.*

        oid_0_0_8_235_0_1:

            oidPath += "/[Version 1]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Used in Procedure I for the CryptoToken-tokenOID, indicating that the hash includes all fields in the RAS/H.225.0 message (authentication and integrity)]",
                2 => $"{oidPath}/[Used in \"Procedure II\" for the CryptoToken-tokenOID indicating that the signature includes a subset of fields in the RAS/H.225.0 message (ClearToken) for authentication-only terminals without integrity]",
                3 => $"{oidPath}/[Used in \"Procedure II\" for the ClearToken-tokenOID indicating that the ClearToken is being used for end-to-end authentication/integrity]",
                4 => $"{oidPath}/[Used in \"Procedures II or III\" to indicate that certificate carries a Uniform Resource Locator (URL)]",
                5 => $"{oidPath}/[ClearToken being used for message authentication and integrity (used in \"Procedure I\" for the ClearToken-tokenOID)]",
                6 => $"{oidPath}/[Used in \"Procedure I\" indicating use of HMAC-SHA1-96]",
                7 => $"{oidPath}/[Used in \"Procedure II\" to indicate message authentication, integrity and non-repudiation]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // 2
        #region 0.0.8.235.0.2.*

        oid_0_0_8_235_0_2:

            oidPath += "/[Version 2]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Hash that includes all fields in the RAS/H.225.0 message (authentication and integrity) (used in \"Procedure I\" for the ClearToken-tokenOID)]",
                2 => $"{oidPath}/[Hash that includes all fields in the RAS/H.225.0 message (authentication and integrity) (used in \"Procedure I\" for the ClearToken-tokenOID)]",
                3 => $"{oidPath}/[Hash that includes all fields in the RAS/H.225.0 message (authentication and integrity) (used in \"Procedure I\" for the ClearToken-tokenOID)]",
                4 => $"{oidPath}/[Certificate that carries a Uniform Resource Locator (URL) in Procedures \"II\" or \"III\"]",
                5 => $"{oidPath}/[ClearToken being used for message authentication and integrity (used in \"Procedure I\" for the ClearToken-tokenOID)]",
                6 => $"{oidPath}/[Use of HMAC-SHA1-96 (used in \"Procedure I\" for the Algorithm OID)]",
                7 => $"{oidPath}/[Token used in \"Procedure II\" to indicate message authentication, integrity and non-repudiation]",
                8 => $"{oidPath}/[Anti-spamming using HMAC-SHA1-96]",
                9 => $"{oidPath}/[End-to-end ClearToken carrying sendersID for verification at the recipient side]",
                31 => $"{oidPath}/[Remote Authentication Dial-In User Service (RADIUS) challenge in the ClearToken]",
                32 => $"{oidPath}/[Remote Authentication Dial-In User Service (RADIUS) response (conveyed in the challenge field) in the ClearToken]",
                33 => $"{oidPath}/[Back-End Server (BES) default mode with a protected password in the ClearToken]",
                40 => $"{oidPath}/[Non-standard DH-group (Diffie-Hellman) explicitly provided]",
                43 => $"{oidPath}/[1024-bit bDH group]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // 3
        #region 0.0.8.235.0.3.*

        oid_0_0_8_235_0_3:

            oidPath += "/[Version 3]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Voice encryption using Triple-DES (168-bit) in outer-OFB mode and 1024-bit DH-group with 64-bit feedback]",
                2 => $"{oidPath}/[Used in \"Procedure II\" for the CryptoToken-tokenOID indicating that the signature includes a subset of fields in the RAS/H.225.0 message (ClearToken) for authentication-only terminals without integrity]",
                9 => $"{oidPath}/[End-to-end ClearToken carrying sendersID for verification at the recipient side]",
                12 => $"{oidPath}/[H.245 Dual Tone Multi-Frequency (DTMF) encryption with DES-56 in Enhanced Output FeedBack (EOFB) mode]",
                13 => $"{oidPath}/[H.245 Dual Tone Multi-Frequency (DTMF) encryption with 3DES-168 in Enhanced Output FeedBack (EOFB) mode]",
                14 => $"{oidPath}/[H.245 Dual Tone Multi-Frequency (DTMF) encryption with AES-128 in Enhanced Output FeedBack (EOFB) mode]",
                24 => $"{oidPath}/[Version 3 capability indicator in ClearToken during call signalling]",
                26 => $"{oidPath}/[Indicates the \"NULL encryption algorithm\"]",
                27 => $"{oidPath}/[Voice encryption using RC2-compatible (56 bit) or RC2-compatible in Enhanced Output FeedBack (EOFB) mode and 512-bit DH-group]",
                28 => $"{oidPath}/[Voice encryption using Data Encryption Standard (DES) (56 bit) in Enhanced Output FeedBack (EOFB) mode and 512-bit DH-group with 64-bit feedback]",
                29 => $"{oidPath}/[Voice encryption using Triple-DES (168 bit) in outer-EOFB mode (Enhanced Output FeedBack) and 1024-bit DH-group with 64-bit feedback]",
                30 => $"{oidPath}/[Voice encryption using Advanced Encryption Standard (AES) (128-bit) in Enhanced Output FeedBack (EOFB) mode and 1024-bit DH-group]",
                40 => $"{oidPath}/[Non-standard DH-group explicitly provided]",
                43 => $"{oidPath}/[1024-bit Diffie-Hellman (DH) group]",
                44 => $"{oidPath}/[1536-bit Diffie-Hellman (DH) group]",
                48 => $"{oidPath}/[Used in the Direct-Routed Call (DRC) procedure during \"GRQ/RRQ\" and \"GCF/RCF\" and \"ARQ\" message to let the EndPoint/GateKeeper (EP/GK) indicate support of Annex I]",
                49 => $"{oidPath}/[Used in the Direct-Routed Call (DRC) procedure for the ClearToken tokenOID indicating that the CLearToken holds an end-to-end key for the caller]",
                50 => $"{oidPath}/[Used in the Direct-Routed Call (DRC) procedure for the ClearToken tokenOID indicating that the ClearToken holds an end-to-end key for the callee]",
                51 => $"{oidPath}/[Used in the Direct-Routed Call (DRC) procedure for the keyDerivationOID within V3KeySyncMaterial to indicate the applied key derivation method in clause I.10 using the HMAC-SHA1 pseudo-random function]",
                71 => $"{oidPath}/[Indicates a baseline ClearToken for Rec. ITU-T H.235, Annex F in the context of this annex]",
                72 => $"{oidPath}/[Symmetric key distribution protocol using pre-shared symmetric keys and Keyed-Hashing for Message Authentication (HMACs, see IETF RFC 2104 and RFC 3830)]",
                73 => $"{oidPath}/[Diffie-Hellman key agreement protocol using pre-shared symmetric keys and Keyed-Hashing for Message Authentication (HMACs, see IETF RFC 2104 and RFC 3830)]",
                74 => $"{oidPath}/[(RSA-based) public-key distribution protocol using digital signatures (see IETF RFC 3830)]",
                75 => $"{oidPath}/[Diffie-Hellman key agreement protocol using digital signatures (see IETF RFC 3830)]",
                76 => $"{oidPath}/[Multimedia Internet KEYing (MIKEY) protocol family generally without indicating specifically any particular MIKEY key management protocol variant such as MIKEY-PS, MIKEY-DHHMAC, MIKEY-PK-SIGN or MIKEY-DH-SIGN]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        // h239
        #region 0.0.8.239.*

        oid_0_0_8_239:

            oidPath += "/[Role management and additional media channels for H.300-series terminals]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_0_0_8_239_1;
                case 2: return $"{oidPath}/[Generic message identifier]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // generic-capabilities
        #region 0.0.8.239.1.*

        oid_0_0_8_239_1:

            oidPath += "/[Generic capability identifier]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Control Capability]",
                2 => $"{oidPath}/[Extended Video Capability]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // h241
        #region 0.0.8.241.*

        oid_0_0_8_241:

            oidPath += "/[Extended video procedures and control signals for H.300-series terminals]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_0_0_8_241_0;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // specificVideoCodecCapabilities
        #region 0.0.8.241.0.*

        oid_0_0_8_241_0:

            oidPath += "/[Specific Video Codec Capabilities]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_0_0_8_241_0_0;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // h264
        #region 0.0.8.241.0.0.*

        oid_0_0_8_241_0_0:

            oidPath += "/[Rec. ITU-T H.264 transport for Rec. ITU-T H.323 systems]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_0_0_8_241_0_0_0;
                case 1: return $"{oidPath}/[Generic capability identifier]";
                case 2: return $"{oidPath}/[Set submode]";
                case 3: return $"{oidPath}/[Set Scalable Video Coding (SVC) mode capability]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // h264
        #region 0.0.8.241.0.0.0.*

        oid_0_0_8_241_0_0_0:

            oidPath += "/[Internet Protocol (IP) Packetisation]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Transport of Rec. ITU-T H.264 streams in Rec. ITU-T H.323 systems]",
                1 => $"{oidPath}/[ITU-T H.241/H.264 video protocol, IETF RFC 3984 non interleaved]",
                2 => $"{oidPath}/[ITU-T H.241/H.264 video protocol, IETF RFC 3984 interleaved]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        #endregion

        // h245
        #region 0.0.8.245.*

        oid_0_0_8_245:

            oidPath += "/[Control Protocol for multimedia communication]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_0_0_8_245_0;
                case 1: goto oid_0_0_8_245_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // version
        #region 0.0.8.245.0.*

        oid_0_0_8_245_0:

            oidPath += "/[Versions of ASN.1 module named Multimedia-System-Control]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: return $"{oidPath}/[Version 1]";
                case 2: return $"{oidPath}/[Version 2]";
                case 3: return $"{oidPath}/[Version 3]";
                case 4: return $"{oidPath}/[Version 4]";
                case 5: return $"{oidPath}/[Version 5]";
                case 6: return $"{oidPath}/[Version 6]";
                case 7: return $"{oidPath}/[Version 7]";
                case 8: return $"{oidPath}/[Version 8]";
                case 9: return $"{oidPath}/[Version 9]";
                case 10: return $"{oidPath}/[Version 10]";
                case 11: return $"{oidPath}/[Version 11]";
                case 12: return $"{oidPath}/[Version 12]";
                case 13: return $"{oidPath}/[Version 13]";
                case 14: goto oid_0_0_8_245_0_14;
                case 15: goto oid_0_0_8_245_0_15;
                case 16: goto oid_0_0_8_245_0_16;
                case 17: goto oid_0_0_8_245_0_17;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // 14
        #region 0.0.8.245.0.14.*

        oid_0_0_8_245_0_14:

            oidPath += "/[Version 14]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[MULTIMEDIA-SYTSTEM-CONTROL]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // 15
        #region 0.0.8.245.0.15.*

        oid_0_0_8_245_0_15:

            oidPath += "/[Version 15]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[MULTIMEDIA-SYTSTEM-CONTROL]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // 16
        #region 0.0.8.245.0.16.*

        oid_0_0_8_245_0_16:

            oidPath += "/[Version 16]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[MULTIMEDIA-SYTSTEM-CONTROL]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // 17
        #region 0.0.8.245.0.17.*

        oid_0_0_8_245_0_17:

            oidPath += "/[Version 17]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[MULTIMEDIA-SYTSTEM-CONTROL]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // generic-capabilities
        #region 0.0.8.245.1.*

        oid_0_0_8_245_1:

            oidPath += "/[Generic capabilities]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_0_0_8_245_1_0;
                case 1: goto oid_0_0_8_245_1_1;
                case 2: goto oid_0_0_8_245_1_2;
                case 3: goto oid_0_0_8_245_1_3;
                case 4: return $"{oidPath}/[Generic multiplex capabilities]";
                case 5: return $"{oidPath}/[Generic user-input capabilities]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // video
        #region 0.0.8.245.1.0.*

        oid_0_0_8_245_1_0:

            oidPath += "/[Video]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Generic capability for ISO/IEC 14496-2]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // audio
        #region 0.0.8.245.1.1.*

        oid_0_0_8_245_1_1:

            oidPath += "/[Audio]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Generic capability for ISO/IEC 14496-3]",
                1 => $"{oidPath}/[Generic capability for the Global System for Mobile (GSM) Adaptive Multi rate speech codec]",
                2 => $"{oidPath}/[Used to indicate the generic capability for the Algebraic Code-Excited Linear Prediction (ACELP) voice codec specified in TIA/EIA/ANSI IS-136]",
                3 => $"{oidPath}/[Used to indicate the generic capability for the TIA/EIA/ANSI IS-136 \"US1\" voice codec]",
                4 => $"{oidPath}/[Generic capability for the TIA/EIA IS-127 Enhanced Variable Rate Codec]",
                5 => $"{oidPath}/[Used to indicate the generic capability for the ISO/IEC 13818-7 standard]",
                6 => $"{oidPath}/[Generic capability for International Telecommunication Union - Radiocommunication sector (ITU-R) BS.1196 as well as IETF RFC 3389 (deprecated)]",
                7 => $"{oidPath}/[Generic capability for L-16 sample base variable rate linear 16-bit codec as defined in IETF RFC 1890]",
                8 => $"{oidPath}/[Bounded audio stream capability]",
                9 => $"{oidPath}/[Generic capability for the Global System for Mobile communications (GSM) Adaptive Multi Rate Narrow Band (AMR-NB) codec (defined in Rec. ITU-T H.245, Annex R)]",
                10 => $"{oidPath}/[Generic capability for the Global System for Mobile communications (GSM) Adaptive Multi Rate Wide Band (AMR-WB) codec (defined in Rec. ITU-T H.245, Annex R)]",
                11 => $"{oidPath}/[Used to indicate the generic capability for the Internet Low Bit Rate Codec (iLBC) (defined in ITU-T H.245, Annex S)]",
                12 => $"{oidPath}/[Used to indicate the generic capability for ITU-R BS.1196 (defined in ITU-T H.245, Annex M)]",
                13 => $"{oidPath}/[Used to indicate the generic capability for signalling comfort noise as specified in RFC 3389 (defined in ITU-T H.245, Annex N)]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // data
        #region 0.0.8.245.1.2.*

        oid_0_0_8_245_1_2:

            oidPath += "/[Data]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Generic capability for ISO/IEC 14496-1]",
                1 => $"{oidPath}/[Used to indicate the generic capability for Nx64 clear channel data transmission (documented in ITU-T H.245, Annex Q)]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // control
        #region 0.0.8.245.1.3.*

        oid_0_0_8_245_1_3:

            oidPath += "/[Control]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Generic capability for logical channel bit rate management]",
                1 => $"{oidPath}/[DynamicPayloadType Replacement (generic capability to replace the dynamic payload type value signalled in open logical channel connection requests with the value signalled in the corresponding open logical channel acknowledgements)]",
                2 => $"{oidPath}/[Generic capability related to support of different versions of the Internet Protocol (IP) for open logical channel signaling]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        // h248
        #region 0.0.8.248.*

        oid_0_0_8_248:

            oidPath += "/[Gateway control protocol]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_0_0_8_248_0;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // modules
        #region 0.0.8.248.0.*

        oid_0_0_8_248_0:

            oidPath += "/[ASN.1 modules]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_0_0_8_248_0_0;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // media-gateway-control
        #region 0.0.8.248.0.0.*

        oid_0_0_8_248_0_0:

            oidPath += "/[MEDIA-GATEWAY-CONTROL]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                2 => $"{oidPath}/[Version 2]",
                3 => $"{oidPath}/[Version 3]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        // 249 
        #region 0.0.8.249.*

        oid_0_0_8_249:

            oidPath += "/[Recommendation ITU-T H.249]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[navigation-key]",
                2 => $"{oidPath}/[soft-keys]",
                3 => $"{oidPath}/[pointing-device]",
                4 => $"{oidPath}/[modal-interface]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // 261 
        #region 0.0.8.261.*

        oid_0_0_8_261:

            oidPath += "/[Video codec for audiovisual services at p x 64 kbits]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_0_0_8_261_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // generic-capabilities 
        #region 0.0.8.261.1.*

        oid_0_0_8_261_1:

            oidPath += "/[Generic capabilities]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Video codec]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // h263
        #region 0.0.8.263.*

        oid_0_0_8_263:

            oidPath += "/[Video coding for low bit rate communication]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_0_0_8_263_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // generic-capabilities
        #region 0.0.8.263.1.*

        oid_0_0_8_263_1:

            oidPath += "/[Generic capabilities]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[First generic capability]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // h282
        #region 0.0.8.282.*

        oid_0_0_8_282:

            oidPath += "/[Remote device control protocol for multimedia applications]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_0_0_8_282_0;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // version
        #region 0.0.8.282.0.*

        oid_0_0_8_282_0:

            oidPath += "/[Versions of Recommendation ITU-T H.282]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[RDC-PROTOCOL]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // h283
        #region 0.0.8.283.*

        oid_0_0_8_283:

            oidPath += "/[Remote device control logical channel transport]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_0_0_8_283_0;
                case 1: goto oid_0_0_8_283_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // version
        #region 0.0.8.283.0.*

        oid_0_0_8_283_0:

            oidPath += "/[Versions of Recommendation ITU-T H.283]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[LCT-PROTOCOL]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // generic-capabilities
        #region 0.0.8.283.1.*

        oid_0_0_8_283_1:

            oidPath += "/[Generic Capability]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[First generic capability]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // 323
        #region 0.0.8.323.*

        oid_0_0_8_323:

            oidPath += "/[Packet-based multimedia communications systems]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_0_0_8_323_0;
                case 1: goto oid_0_0_8_323_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // main
        #region 0.0.8.323.0.*

        oid_0_0_8_323_0:

            oidPath += "/[Main part]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_0_0_8_323_0_0;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // generic-capabilities
        #region 0.0.8.323.0.0.*

        oid_0_0_8_323_0_0:

            oidPath += "/[Generic capabilities]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[singleTransmitterMulticast]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // annex
        #region 0.0.8.323.1.*

        oid_0_0_8_323_1:

            oidPath += "/[Annexes]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 7: goto oid_0_0_8_323_1_7;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // g
        #region 0.0.8.323.1.7.*

        oid_0_0_8_323_1_7:

            oidPath += "/[Annex G]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[T140Data and T140Audio]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        // h324
        #region 0.0.8.324.*

        oid_0_0_8_324:

            oidPath += "/[Terminal for low bit-rate multimedia communication]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_0_0_8_324_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // generic-capabilities
        #region 0.0.8.324.1.*

        oid_0_0_8_324_1:

            oidPath += "/[Generic capabilities]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: return $"{oidPath}/[http]";
                case 1: return $"{oidPath}/[sessionResetCapability]";
                case 2: goto oid_0_0_8_324_1_2;
                case 3: return $"{oidPath}/[textConversationCapability]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // mona
        #region 0.0.8.324.1.2.*

        oid_0_0_8_324_1_2:

            oidPath += "/[Media-Oriented Negotiation Acceleration (MONA)]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Mean Opinion Score (MOS)]",
                2 => $"{oidPath}/[Mean Opinion Score (MOS) Ack]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        // h341
        #region 0.0.8.341.*

        oid_0_0_8_341:

            oidPath += "/[Multimedia management information base]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_0_0_8_341_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // mib, mmRoot
        #region 0.0.8.341.1.*

        oid_0_0_8_341_1:

            oidPath += "/[Management Information Base (MIB)]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_0_0_8_341_1_0;
                case 1: goto oid_0_0_8_341_1_1;
                //TODO: case 2: goto oid_0_0_8_341_1_2;
                //TODO: case 3: goto oid_0_0_8_341_1_3;
                //TODO: case 4: goto oid_0_0_8_341_1_4;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // mmRoot
        #region 0.0.8.341.1.0.*

        oid_0_0_8_341_1_0:

            oidPath += "/[mmRoot]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[multimediaMibTC]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // mmH323Root
        #region 0.0.8.341.1.1.*

        oid_0_0_8_341_1_1:

            oidPath += "/[mmH323Root]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_0_0_8_341_1_1_1;
                case 2: goto oid_0_0_8_341_1_1_2;
                case 3: goto oid_0_0_8_341_1_1_3;
                //TODO: case 4: goto oid_0_0_8_341_1_1_4;
                //TODO: case 5: goto oid_0_0_8_341_1_1_5;
                //TODO: case 6: goto oid_0_0_8_341_1_1_6;
                default: return $"{oidPath}{values[index - 1]}";
            }

        // h225CallSignaling
        #region 0.0.8.341.1.1.1.*

        oid_0_0_8_341_1_1_1:

            oidPath += "/[h225CallSignaling]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_0_0_8_341_1_1_1_1;
                case 2: goto oid_0_0_8_341_1_1_1_2;
                case 3: goto oid_0_0_8_341_1_1_1_3;
                case 4: goto oid_0_0_8_341_1_1_1_4;
                case 5: goto oid_0_0_8_341_1_1_1_5;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // callSignalConfig
        #region 0.0.8.341.1.1.1.1.*

        oid_0_0_8_341_1_1_1_1:

            oidPath += "/[callSignalConfig]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_0_0_8_341_1_1_1_1_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // callSignalConfigTable
        #region 0.0.8.341.1.1.1.1.1.*

        oid_0_0_8_341_1_1_1_1_1:

            oidPath += "/[callSignalConfigTable]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_0_0_8_341_1_1_1_1_1_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // callSignalConfigEntry
        #region 0.0.8.341.1.1.1.1.1.1.*

        oid_0_0_8_341_1_1_1_1_1_1:

            oidPath += "/[callSignalConfigEntry]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[callSignalConfigMaxConnections]",
                2 => $"{oidPath}/[callSignalConfigAvailableConnections]",
                3 => $"{oidPath}/[callSignalConfigT303]",
                4 => $"{oidPath}/[callSignalConfigT301]",
                5 => $"{oidPath}/[callSignalConfigEnableNotifications]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        // callSignalStats
        #region 0.0.8.341.1.1.1.2.*

        oid_0_0_8_341_1_1_1_2:

            oidPath += "/[callSignalStats]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_0_0_8_341_1_1_1_2_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // callSignalStatsTable
        #region 0.0.8.341.1.1.1.2.1.*

        oid_0_0_8_341_1_1_1_2_1:

            oidPath += "/[callSignalStatsTable]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_0_0_8_341_1_1_1_2_1_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // callSignalStatsEntry
        #region 0.0.8.341.1.1.1.2.1.1.*

        oid_0_0_8_341_1_1_1_2_1_1:

            oidPath += "/[callSignalStatsEntry]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[callSignalStatsCallConnectionsIn]",
                2 => $"{oidPath}/[callSignalStatsCallConnectionsOut]",
                3 => $"{oidPath}/[callSignalStatsAlertingMsgsIn]",
                4 => $"{oidPath}/[callSignalStatsAlertingMsgsOut]",
                5 => $"{oidPath}/[callSignalStatsCallProceedingsIn]",
                6 => $"{oidPath}/[callSignalStatsCallProceedingsOut]",
                7 => $"{oidPath}/[callSignalStatsSetupMsgsIn]",
                8 => $"{oidPath}/[callSignalStatsSetupMsgsOut]",
                9 => $"{oidPath}/[callSignalStatsSetupAckMsgsIn]",
                10 => $"{oidPath}/[callSignalStatsSetupAckMsgsOut]",
                11 => $"{oidPath}/[callSignalStatsProgressMsgsIn]",
                12 => $"{oidPath}/[callSignalStatsProgressMsgsOut]",
                13 => $"{oidPath}/[callSignalStatsReleaseCompleteMsgsIn]",
                14 => $"{oidPath}/[callSignalStatsReleaseCompleteMsgsOut]",
                15 => $"{oidPath}/[callSignalStatsStatusMsgsIn]",
                16 => $"{oidPath}/[callSignalStatsStatusMsgsOut]",
                17 => $"{oidPath}/[callSignalStatsStatusInquiryMsgsIn]",
                18 => $"{oidPath}/[callSignalStatsStatusInquiryMsgsOut]",
                19 => $"{oidPath}/[callSignalStatsFacilityMsgsIn]",
                20 => $"{oidPath}/[callSignalStatsFacilityMsgsOut]",
                21 => $"{oidPath}/[callSignalStatsInfoMsgsIn]",
                22 => $"{oidPath}/[callSignalStatsInfoMsgsOut]",
                23 => $"{oidPath}/[callSignalStatsNotifyMsgsIn]",
                24 => $"{oidPath}/[callSignalStatsNotifyMsgsOut]",
                25 => $"{oidPath}/[callSignalStatsAverageCallDuration]",
                26 => $"{oidPath}/[callSignalStatsCallConnections]",
                27 => $"{oidPath}/[callSignalStatsAlertingMsgs]",
                28 => $"{oidPath}/[callSignalStatsCallProceedings]",
                29 => $"{oidPath}/[callSignalStatsSetupMsgs]",
                30 => $"{oidPath}/[callSignalStatsSetupAckMsgs]",
                31 => $"{oidPath}/[callSignalStatsProgressMsgs]",
                32 => $"{oidPath}/[callSignalStatsReleaseCompleteMsgs]",
                33 => $"{oidPath}/[callSignalStatsStatusMsgs]",
                34 => $"{oidPath}/[callSignalStatsStatusInquiryMsgs]",
                35 => $"{oidPath}/[callSignalStatsFacilityMsgs]",
                36 => $"{oidPath}/[callSignalStatsInfoMsgs]",
                37 => $"{oidPath}/[callSignalStatsNotifyMsgs]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        // connections
        #region 0.0.8.341.1.1.1.3.*

        oid_0_0_8_341_1_1_1_3:

            oidPath += "/[connections]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: return $"{oidPath}/[connectionsActiveConnections]";
                case 2: goto oid_0_0_8_341_1_1_1_3_2;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // connectionsTable
        #region 0.0.8.341.1.1.1.3.2.*

        oid_0_0_8_341_1_1_1_3_2:

            oidPath += "/[connectionsTable]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_0_0_8_341_1_1_1_3_2_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // connectionsTableEntry
        #region 0.0.8.341.1.1.1.3.2.1.*

        oid_0_0_8_341_1_1_1_3_2_1:

            oidPath += "/[connectionsTableEntry]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[connectionsSrcTransporTAddressTag]",
                2 => $"{oidPath}/[connectionsSrcTransporTAddress]",
                3 => $"{oidPath}/[connectionsCallIdentifier]",
                4 => $"{oidPath}/[connectionsRole]",
                5 => $"{oidPath}/[connectionsState]",
                6 => $"{oidPath}/[connectionsDestTransporTAddressTag]",
                7 => $"{oidPath}/[connectionsDestTransporTAddress]",
                8 => $"{oidPath}/[connectionsDestAliasTag]",
                9 => $"{oidPath}/[connectionsDestAlias]",
                10 => $"{oidPath}/[connectionsSrcH245SigTransporTAddressTag]",
                11 => $"{oidPath}/[connectionsSrcH245SigTransporTAddress]",
                12 => $"{oidPath}/[connectionsDestH245SigTransporTAddressTag]",
                13 => $"{oidPath}/[connectionsDestH245SigTransporTAddress]",
                14 => $"{oidPath}/[connectionsConfId]",
                15 => $"{oidPath}/[connectionsCalledPartyNumber]",
                16 => $"{oidPath}/[connectionsDestXtraCallingNumber1]",
                17 => $"{oidPath}/[connectionsDestXtraCallingNumber2]",
                18 => $"{oidPath}/[connectionsDestXtraCallingNumber3]",
                19 => $"{oidPath}/[connectionsDestXtraCallingNumber4]",
                20 => $"{oidPath}/[connectionsDestXtraCallingNumber5]",
                21 => $"{oidPath}/[connectionsFastCall]",
                22 => $"{oidPath}/[connectionsSecurity]",
                23 => $"{oidPath}/[connectionsH245Tunneling]",
                24 => $"{oidPath}/[connectionsCanOverlapSend]",
                25 => $"{oidPath}/[connectionsCRV]",
                26 => $"{oidPath}/[connectionsCallType]",
                27 => $"{oidPath}/[connectionsRemoteExtensionAddress]",
                28 => $"{oidPath}/[connectionsExtraCRV1]",
                29 => $"{oidPath}/[connectionsExtraCRV2]",
                30 => $"{oidPath}/[connectionsConnectionStartTime]",
                31 => $"{oidPath}/[connectionsEndpointType]",
                32 => $"{oidPath}/[connectionsReleaseCompleteReason]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        // 4
        #region 0.0.8.341.1.1.1.4.*

        oid_0_0_8_341_1_1_1_4:

            oidPath += "/[???]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_0_0_8_341_1_1_1_4_0;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // callSignalEvents
        #region 0.0.8.341.1.1.1.4.0.*

        oid_0_0_8_341_1_1_1_4_0:

            oidPath += "/[callSignalEvents]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[callReleaseComplete]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // callSignalingMIBConformance
        #region 0.0.8.341.1.1.1.5.*

        oid_0_0_8_341_1_1_1_5:

            oidPath += "/[callSignalingMIBConformance]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_0_0_8_341_1_1_1_5_1;
                case 2: return $"{oidPath}/[callSignalingMIBCompliance]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // callSignalingMIBGroups
        #region 0.0.8.341.1.1.1.5.1.*

        oid_0_0_8_341_1_1_1_5_1:

            oidPath += "/[callSignalingMIBConformance]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[callSignalConfigGroup]",
                2 => $"{oidPath}/[callSignalStatsGroup]",
                3 => $"{oidPath}/[connectionsGroup]",
                4 => $"{oidPath}/[callSignalEventsGroup]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        // ras
        #region 0.0.8.341.1.1.2.*

        oid_0_0_8_341_1_1_2:

            oidPath += "/[ras]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_0_0_8_341_1_1_2_1;
                case 2: goto oid_0_0_8_341_1_1_2_2;
                case 3: goto oid_0_0_8_341_1_1_2_3;
                case 4: goto oid_0_0_8_341_1_1_2_4;
                case 5: goto oid_0_0_8_341_1_1_2_5;
                case 6: goto oid_0_0_8_341_1_1_2_6;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // rasConfiguration
        #region 0.0.8.341.1.1.2.1.*

        oid_0_0_8_341_1_1_2_1:

            oidPath += "/[rasConfiguration]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_0_0_8_341_1_1_2_1_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // rasConfigurationTable
        #region 0.0.8.341.1.1.2.1.1.*

        oid_0_0_8_341_1_1_2_1_1:

            oidPath += "/[rasConfigurationTable]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_0_0_8_341_1_1_2_1_1_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // rasConfigurationTableEntry
        #region 0.0.8.341.1.1.2.1.1.1.*

        oid_0_0_8_341_1_1_2_1_1_1:

            oidPath += "/[rasConfigurationTableEntry]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[rasConfigurationGatekeeperIdentifier]",
                2 => $"{oidPath}/[rasConfigurationTimer]",
                3 => $"{oidPath}/[rasConfigurationMaxNumberOfRetries]",
                4 => $"{oidPath}/[rasConfigurationGatekeeperDiscoveryAddressTag]",
                5 => $"{oidPath}/[rasConfigurationGatekeeperDiscoveryAddress]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        // rasRegistration
        #region 0.0.8.341.1.1.2.2.*

        oid_0_0_8_341_1_1_2_2:

            oidPath += "/[rasRegistration]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_0_0_8_341_1_1_2_2_1;
                case 2: goto oid_0_0_8_341_1_1_2_2_2;
                case 3: goto oid_0_0_8_341_1_1_2_2_3;
                case 4: goto oid_0_0_8_341_1_1_2_2_4;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // rasRegistrationTable
        #region 0.0.8.341.1.1.2.2.1.*

        oid_0_0_8_341_1_1_2_2_1:

            oidPath += "/[rasRegistrationTable]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_0_0_8_341_1_1_2_2_1_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // rasRegistrationTableEntry
        #region 0.0.8.341.1.1.2.2.1.1.*

        oid_0_0_8_341_1_1_2_2_1_1:

            oidPath += "/[rasRegistrationTableEntry]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[rasRegistrationCallSignallingAddressTag]",
                2 => $"{oidPath}/[rasRegistrationCallSignallingAddress]",
                3 => $"{oidPath}/[rasRegistrationSrcRasAddressTag]",
                4 => $"{oidPath}/[rasRegistrationSrcRasAddress]",
                5 => $"{oidPath}/[rasRegistrationIsGatekeeper]",
                6 => $"{oidPath}/[rasRegistrationGatekeeperId]",
                7 => $"{oidPath}/[rasRegistrationEndpointId]",
                8 => $"{oidPath}/[rasRegistrationEncryption]",
                9 => $"{oidPath}/[rasRegistrationWillSupplyUUIE]",
                10 => $"{oidPath}/[rasRegistrationIntegrityCheckValue]",
                11 => $"{oidPath}/[rasRegistrationTableNumberOfAliases]",
                12 => $"{oidPath}/[rasRegistrationTableRowStatus]",
                13 => $"{oidPath}/[rasRegistrationEndpointType]",
                14 => $"{oidPath}/[rasRegistrationPregrantedARQ]",
                15 => $"{oidPath}/[rasRegistrationIsregisteredByRRQ]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // rasRegistrationAliasTable
        #region 0.0.8.341.1.1.2.2.2.*

        oid_0_0_8_341_1_1_2_2_2:

            oidPath += "/[rasRegistrationAliasTable]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_0_0_8_341_1_1_2_2_2_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // rasRegistrationAliasTableEntry
        #region 0.0.8.341.1.1.2.2.2.1.*

        oid_0_0_8_341_1_1_2_2_2_1:

            oidPath += "/[rasRegistrationAliasTableEntry]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[rasRegistrationAliasTableIndex]",
                2 => $"{oidPath}/[rasRegistrationAliasTag]",
                3 => $"{oidPath}/[rasRegistrationAlias]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // rasRegistrationRasAddressTable
        #region 0.0.8.341.1.1.2.2.3.*

        oid_0_0_8_341_1_1_2_2_3:

            oidPath += "/[rasRegistrationRasAddressTable]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_0_0_8_341_1_1_2_2_3_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // rasRegistrationRasAddressTableEntry
        #region 0.0.8.341.1.1.2.2.3.1.*

        oid_0_0_8_341_1_1_2_2_3_1:

            oidPath += "/[rasRegistrationRasAddressTableEntry]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[rasRegistrationRasAddressTableIndex]",
                2 => $"{oidPath}/[rasRegistrationAdditionalSrcRasAddressTag]",
                3 => $"{oidPath}/[rasRegistrationAdditionalSrcRasAddress]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // rasRegistrationCallSignalingAddressTable
        #region 0.0.8.341.1.1.2.2.4.*

        oid_0_0_8_341_1_1_2_2_4:

            oidPath += "/[rasRegistrationCallSignalingAddressTable]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_0_0_8_341_1_1_2_2_4_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // rasRegistrationCallSignalingAddressTableEntry
        #region 0.0.8.341.1.1.2.2.4.1.*

        oid_0_0_8_341_1_1_2_2_4_1:

            oidPath += "/[rasRegistrationCallSignalingAddressTableEntry]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[rasRegistrationCallSignalingAddressTableIndex]",
                2 => $"{oidPath}/[rasRegistrationAdditionalCallSignalingAddressTag]",
                3 => $"{oidPath}/[rasRegistrationAdditionalCallSignalingAddress]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        // rasAdmission
        #region 0.0.8.341.1.1.2.3.*

        oid_0_0_8_341_1_1_2_3:

            oidPath += "/[rasAdmission]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_0_0_8_341_1_1_2_3_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // rasAdmissionTable
        #region 0.0.8.341.1.1.2.3.1.*

        oid_0_0_8_341_1_1_2_3_1:

            oidPath += "/[rasAdmissionTable]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_0_0_8_341_1_1_2_3_1_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // rasAdmissionTableEntry
        #region 0.0.8.341.1.1.2.3.1.1.*

        oid_0_0_8_341_1_1_2_3_1_1:

            oidPath += "/[rasAdmissionTableEntry]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[rasAdmissionSrcCallSignallingAddressTag]",
                2 => $"{oidPath}/[rasAdmissionSrcCallSignallingAddress]",
                3 => $"{oidPath}/[rasAdmissionDestCallSignallingAddressTag]",
                4 => $"{oidPath}/[rasAdmissionDestCallSignallingAddress]",
                5 => $"{oidPath}/[rasAdmissionCallIdentifier]",
                6 => $"{oidPath}/[rasAdmissionConferenceId]",
                7 => $"{oidPath}/[rasAdmissionRasAddressTag]",
                8 => $"{oidPath}/[rasAdmissionRasAddress]",
                9 => $"{oidPath}/[rasAdmissionCRV]",
                10 => $"{oidPath}/[rasAdmissionIsGatekeeper]",
                11 => $"{oidPath}/[rasAdmissionSrcAliasAddressTag]",
                12 => $"{oidPath}/[rasAdmissionSrcAliasAddress]",
                13 => $"{oidPath}/[rasAdmissionDestAliasAddressTag]",
                14 => $"{oidPath}/[rasAdmissionDestAliasAddress]",
                15 => $"{oidPath}/[rasAdmissionAnswerCallIndicator]",
                16 => $"{oidPath}/[rasAdmissionTime]",
                17 => $"{oidPath}/[rasAdmissionEndpointId]",
                18 => $"{oidPath}/[rasAdmissionBandwidth]",
                19 => $"{oidPath}/[rasAdmissionIRRFrequency]",
                20 => $"{oidPath}/[rasAdmissionCallType]",
                21 => $"{oidPath}/[rasAdmissionCallModel]",
                22 => $"{oidPath}/[rasAdmissionSrcHandlesBandwidth]",
                23 => $"{oidPath}/[rasAdmissionDestHandlesBandwidth]",
                24 => $"{oidPath}/[rasAdmissionSecurity]",
                25 => $"{oidPath}/[rasAdmissionSrcWillSupplyUUIE]",
                26 => $"{oidPath}/[rasAdmissionDestWillSupplyUUIE]",
                27 => $"{oidPath}/[rasAdmissionTableRowStatus]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        // rasStats
        #region 0.0.8.341.1.1.2.4.*

        oid_0_0_8_341_1_1_2_4:

            oidPath += "/[rasStats]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_0_0_8_341_1_1_2_4_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // rasStatsTable
        #region 0.0.8.341.1.1.2.4.1.*

        oid_0_0_8_341_1_1_2_4_1:

            oidPath += "/[rasStatsTable]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_0_0_8_341_1_1_2_4_1_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // rasStatsTableEntry
        #region 0.0.8.341.1.1.2.4.1.1.*

        oid_0_0_8_341_1_1_2_4_1_1:

            oidPath += "/[rasStatsTableEntry]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[rasStatsGatekeeperConfirms]",
                2 => $"{oidPath}/[rasStatsGatekeeperRejects]",
                3 => $"{oidPath}/[rasStatsRegistrationConfirms]",
                4 => $"{oidPath}/[rasStatsRegistrationRejects]",
                5 => $"{oidPath}/[rasStatsUnregistrationConfirms]",
                6 => $"{oidPath}/[rasStatsUnregistrationRejects]",
                7 => $"{oidPath}/[rasStatsAdmissionConfirms]",
                8 => $"{oidPath}/[rasStatsAdmissionRejects]",
                9 => $"{oidPath}/[rasStatsBandwidthConfirms]",
                10 => $"{oidPath}/[rasStatsBandwidthRejects]",
                11 => $"{oidPath}/[rasStatsDisengageConfirms]",
                12 => $"{oidPath}/[rasStatsDisengageRejects]",
                13 => $"{oidPath}/[rasStatsLocationConfirms]",
                14 => $"{oidPath}/[rasStatsLocationRejects]",
                15 => $"{oidPath}/[rasStatsInfoRequests]",
                16 => $"{oidPath}/[rasStatsInfoRequestResponses]",
                17 => $"{oidPath}/[rasStatsnonStandardMessages]",
                18 => $"{oidPath}/[rasStatsUnknownMessages]",
                19 => $"{oidPath}/[rasStatsRequestInProgress]",
                20 => $"{oidPath}/[rasStatsResourceAvailabilityIndicator]",
                21 => $"{oidPath}/[rasStatsResourceAvailabilityConfirm]",
                22 => $"{oidPath}/[rasStatsRegisteredEndpointsNo]",
                23 => $"{oidPath}/[rasStatsAdmittedEndpointsNo]",
                24 => $"{oidPath}/[rasStatsINAKs]",
                25 => $"{oidPath}/[rasStatsIACKs]",
                26 => $"{oidPath}/[rasStatsGkRoutedCalls]",
                27 => $"{oidPath}/[rasStatsResourceAvailabilityIndications]",
                28 => $"{oidPath}/[rasStatsResourceAvailabilityConfirmations]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        // ???
        #region 0.0.8.341.1.1.2.5.*

        oid_0_0_8_341_1_1_2_5:

            oidPath += "/[???]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_0_0_8_341_1_1_2_5_0;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // rasEvents
        #region 0.0.8.341.1.1.2.5.0.*

        oid_0_0_8_341_1_1_2_5_0:

            oidPath += "/[rasEvents]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[lastArjReason]",
                2 => $"{oidPath}/[lastArjRasAddressTag]",
                3 => $"{oidPath}/[lastArjRasAddress]",
                4 => $"{oidPath}/[admissionReject]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // rasMIBConformance
        #region 0.0.8.341.1.1.2.6.*

        oid_0_0_8_341_1_1_2_6:

            oidPath += "/[rasMIBConformance]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_0_0_8_341_1_1_2_6_1;
                case 2: return $"{oidPath}/[rasMIBCompliance]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // rasMIBGroups
        #region 0.0.8.341.1.1.2.6.1.*

        oid_0_0_8_341_1_1_2_6_1:

            oidPath += "/[rasMIBGroups]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[rasConfigurationGroup]",
                2 => $"{oidPath}/[rasRegistrationGroup]",
                3 => $"{oidPath}/[rasAdmissionGroup]",
                4 => $"{oidPath}/[rasStatsGroup]",
                5 => $"{oidPath}/[rasEventsGroup]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        // h323Terminal
        #region 0.0.8.341.1.1.3.*

        oid_0_0_8_341_1_1_3:

            oidPath += "/[h323Terminal]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                // TODO: case 1: goto oid_0_0_8_341_1_1_3_1;
                // TODO: case 2: goto oid_0_0_8_341_1_1_3_2;
                // TODO: case 3: goto oid_0_0_8_341_1_1_3_3;
                // TODO: case 5: goto oid_0_0_8_341_1_1_3_5;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        #endregion

        #endregion

        #endregion

        #endregion

        // i
        #region 0.0.9.*

        oid_0_0_9:

            oidPath += "/I";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 751: goto oid_0_0_9_751;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // atmm, i751
        #region 0.0.9.751.*

        oid_0_0_9_751:

            oidPath += "/[Asynchronous transfer mode management of the network element view]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_0_0_9_751_0;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // informationModel
        #region 0.0.9.751.0.*

        oid_0_0_9_751_0:

            oidPath += "/[Information model]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                //TODO: case 0: goto oid_0_0_9_751_0_0;
                //TODO: case 2: goto oid_0_0_9_751_0_2;
                //TODO: case 3: goto oid_0_0_9_751_0_3;
                //TODO: case 4: goto oid_0_0_9_751_0_4;
                case 5: return $"{oidPath}";
                //TODO: case 6: goto oid_0_0_9_751_0_6;
                //TODO: case 7: goto oid_0_0_9_751_0_7;
                //TODO: case 9: goto oid_0_0_9_751_0_9;
                case 10: return $"{oidPath}/[Notifications]";
                case 11: return $"{oidPath}/[Relationship classes]";
                //TODO: case 12: goto oid_0_0_9_751_0_12;
                default: return $"{oidPath}/{values[index - 1]}";
            }
            ;

        #endregion

        #endregion

        #endregion

        // m
        #region 0.0.13.*

        oid_0_0_13:

            oidPath += "/M";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                //TODO: case 3100: goto oid_0_0_13_3100;
                //TODO: case 3108: goto oid_0_0_13_3108;
                //TODO: case 3611: goto oid_0_0_13_3611;
                //TODO: case 3640: goto oid_0_0_13_3640;
                //TODO: case 3641: goto oid_0_0_13_3641;
                //TODO: case 3650: goto oid_0_0_13_3650;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // q
        #region 0.0.17.*

        oid_0_0_17:

            oidPath += "/Q";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                //TODO: case 733: goto oid_0_0_17_733;
                //TODO: case 736: goto oid_0_0_17_736;
                //TODO: case 751: goto oid_0_0_17_751;
                //TODO: case 753: goto oid_0_0_17_753;
                //TODO: case 754: goto oid_0_0_17_754;
                //TODO: case 755: goto oid_0_0_17_755;
                //TODO: case 763: goto oid_0_0_17_763;
                //TODO: case 765: goto oid_0_0_17_765;
                //TODO: case 773: goto oid_0_0_17_773;
                //TODO: case 775: goto oid_0_0_17_775;
                //TODO: case 813: goto oid_0_0_17_813;
                //TODO: case 814: goto oid_0_0_17_814;
                //TODO: case 815: goto oid_0_0_17_815;
                //TODO: case 821: goto oid_0_0_17_821;
                //TODO: case 822: goto oid_0_0_17_822;
                //TODO: case 823: goto oid_0_0_17_823;
                //TODO: case 824: goto oid_0_0_17_824;
                //TODO: case 825: goto oid_0_0_17_825;
                //TODO: case 826: goto oid_0_0_17_826;
                //TODO: case 831: goto oid_0_0_17_831;
                //TODO: case 832: goto oid_0_0_17_832;
                //TODO: case 835: goto oid_0_0_17_835;
                //TODO: case 836: goto oid_0_0_17_836;
                //TODO: case 860: goto oid_0_0_17_860;
                //TODO: case 932: goto oid_0_0_17_932;
                //TODO: case 941: goto oid_0_0_17_941;
                //TODO: case 950: goto oid_0_0_17_950;
                //TODO: case 951: goto oid_0_0_17_951;
                //TODO: case 952: goto oid_0_0_17_952;
                //TODO: case 953: goto oid_0_0_17_953;
                //TODO: case 954: goto oid_0_0_17_954;
                //TODO: case 955: goto oid_0_0_17_955;
                //TODO: case 956: goto oid_0_0_17_956;
                //TODO: case 957: goto oid_0_0_17_957;
                //TODO: case 1218: goto oid_0_0_17_1218;
                //TODO: case 1228: goto oid_0_0_17_1228;
                //TODO: case 1238: goto oid_0_0_17_1238;
                //TODO: case 1248: goto oid_0_0_17_1248;
                //TODO: case 1400: goto oid_0_0_17_1400;
                //TODO: case 1551: goto oid_0_0_17_1551;
                //TODO: case 1831: goto oid_0_0_17_1831;
                //TODO: case 2724: goto oid_0_0_17_2724;
                //TODO: case 2751: goto oid_0_0_17_2751;
                //TODO: case 2932: goto oid_0_0_17_2932;
                //TODO: case 2964: goto oid_0_0_17_2964;
                //TODO: case 2981: goto oid_0_0_17_2981;
                //TODO: case 2984: goto oid_0_0_17_2984;
                //TODO: case 3303: goto oid_0_0_17_3303;
                //TODO: case 3304: goto oid_0_0_17_3304;
                //TODO: case 3308: goto oid_0_0_17_3308;
                case 8361: return $"{oidPath}/[Specifications of Signalling System No. 7 -- Q3 interface]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // t
        #region 0.0.20.*

        oid_0_0_20:

            oidPath += "/T";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                //TODO: case 43: goto oid_0_0_20_43;
                //TODO: case 123: goto oid_0_0_20_123;
                //TODO: case 124: goto oid_0_0_20_124;
                //TODO: case 126: goto oid_0_0_20_126;
                //TODO: case 127: goto oid_0_0_20_127;
                //TODO: case 128: goto oid_0_0_20_128;
                //TODO: case 134: goto oid_0_0_20_134;
                //TODO: case 135: goto oid_0_0_20_135;
                //TODO: case 136: goto oid_0_0_20_136;
                //TODO: case 137: goto oid_0_0_20_137;
                case 330: return $"{oidPath}/[TLMAAbsService]";
                //TODO: case 433: goto oid_0_0_20_433;
                //TODO: case 434: goto oid_0_0_20_434;
                //TODO: case 435: goto oid_0_0_20_435;
                //TODO: case 436: goto oid_0_0_20_436;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // v
        #region 0.0.22.*

        oid_0_0_22:

            oidPath += "/V";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                //TODO: case 43: goto oid_0_0_22_150;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // x
        #region 0.0.24.*

        oid_0_0_24:

            oidPath += "/X";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                //TODO: case 162: goto oid_0_0_24_162;
                //TODO: case 754: goto oid_0_0_24_754;
                //TODO: case 790: goto oid_0_0_24_790;
                //TODO: case 792: goto oid_0_0_24_792;
                //TODO: case 894: goto oid_0_0_24_894;
                //TODO: case 1084: goto oid_0_0_24_1084;
                //TODO: case 1089: goto oid_0_0_24_1089;
                //TODO: case 1125: goto oid_0_0_24_1125;
                //TODO: case 1243: goto oid_0_0_24_1243;
                //TODO: case 1303: goto oid_0_0_24_1303;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        #endregion

        // administration
        #region 0.2.*

        oid_0_2:

            oidPath += "/Administration";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 202: return $"{oidPath}/[Greece]";
                case 204:
                case 205: return $"{oidPath}/[Kingdom of the Netherlands]";
                case 206: return $"{oidPath}/[Belgium]";
                case 208:
                case 209:
                case 210:
                case 211: return $"{oidPath}/[France]";
                case 212: return $"{oidPath}/[Principality of Monaco]";
                case 213: return $"{oidPath}/[Principality of ANDORRA]";
                case 214:
                case 215: return $"{oidPath}/[Spain]";
                case 216: return $"{oidPath}/[Hungary]";
                case 218: return $"{oidPath}/[Bosnia and Herzegovina]";
                case 219: return $"{oidPath}/[Republic of CROATIA]";
                case 220: return $"{oidPath}/[Republic of Serbia]";
                case 222:
                case 223:
                case 224: return $"{oidPath}/[Italy]";
                case 225: return $"{oidPath}/[Vatican City State]";
                case 226: return $"{oidPath}/[Romania]";
                //TODO: case 228: goto oid_0_2_228;
                case 229: return $"{oidPath}/[Confederation of Switzerland]";
                case 230: return $"{oidPath}/[Czech Republic]";
                case 231: return $"{oidPath}/[Slovakia]";
                case 232:
                case 233: return $"{oidPath}/[Austria]";
                case 234:
                case 235:
                case 236:
                case 237: return $"{oidPath}/[United Kingdom of Great Britain and Northern Ireland]";
                case 238:
                case 239: return $"{oidPath}/[Denmark]";
                case 240: return $"{oidPath}/[Sweden]";
                //TODO: case 242: goto oid_0_2_242;
                case 243: return $"{oidPath}/[Norway]";
                case 244: return $"{oidPath}/[Finland]";
                case 246: return $"{oidPath}/[Republic of LITHUANIA]";
                case 247: return $"{oidPath}/[Republic of Latvia]";
                case 248: return $"{oidPath}/[Republic of ESTONIA]";
                case 250:
                case 251: return $"{oidPath}/[Russian Federation]";
                case 255: return $"{oidPath}/[Ukraine]";
                case 257: return $"{oidPath}/[Republic of Belarus]";
                case 259: return $"{oidPath}/[Republic of Moldova]";
                case 260:
                case 261: return $"{oidPath}/[Republic of Poland]";
                //TODO: case 262: goto oid_0_2_262;
                case 263:
                case 264:
                case 265: return $"{oidPath}/[Federal Republic of Germany]";
                case 266: return $"{oidPath}/[Gibraltar]";
                case 268:
                case 269: return $"{oidPath}/[Portugal]";
                case 270: return $"{oidPath}/[Luxembourg]";
                case 272: return $"{oidPath}/[Ireland]";
                case 274: return $"{oidPath}/[Iceland]";
                case 276: return $"{oidPath}/[Republic of Albania]";
                case 278: return $"{oidPath}/[Malta]";
                case 280: return $"{oidPath}/[Republic of Cyprus]";
                case 282: return $"{oidPath}/[Georgia]";
                case 283: return $"{oidPath}/[Republic of ARMENIA]";
                case 284: return $"{oidPath}/[Republic of Bulgaria]";
                case 286: return $"{oidPath}/[Turkey]";
                case 288: return $"{oidPath}/[Faroe Islands]";
                case 290: return $"{oidPath}/[Greenland]";
                case 292: return $"{oidPath}/[Republic of San Marino]";
                case 293: return $"{oidPath}/[Republic of SLOVENIA]";
                case 294: return $"{oidPath}/[The Former Yugoslav Republic of Macedonia]";
                case 295: return $"{oidPath}/[Principality of Liechtenstein]";
                case 297: return $"{oidPath}/[Montenegro]";
                case 302:
                case 303: return $"{oidPath}/[Canada]";
                case 308: return $"{oidPath}/[Saint Pierre and Miquelon (Collectivité territoriale de la République française)]";
                case 310:
                case 311:
                case 312:
                case 313:
                case 314:
                case 315:
                case 316: return $"{oidPath}/[United States of America]";
                case 330: return $"{oidPath}/[Puerto Rico]";
                case 332: return $"{oidPath}/[United States Virgin Islands]";
                case 334:
                case 335: return $"{oidPath}/[Mexico]";
                case 338: return $"{oidPath}/[Jamaica]";
                case 340: return $"{oidPath}/[French Department of Guadeloupe and French Department of Martinique]";
                case 342: return $"{oidPath}/[Barbados]";
                case 344: return $"{oidPath}/[Antigua and Barbuda]";
                case 346: return $"{oidPath}/[Cayman Islands]";
                case 348: return $"{oidPath}/[British Virgin Islands]";
                case 350: return $"{oidPath}/[Bermuda]";
                case 352: return $"{oidPath}/[Grenada]";
                case 354: return $"{oidPath}/[Montserrat]";
                case 356: return $"{oidPath}/[Saint Kitts and Nevis]";
                case 358: return $"{oidPath}/[Saint Lucia]";
                case 360: return $"{oidPath}/[Saint Vincent and the Grenadines]";
                case 362: return $"{oidPath}/[Netherlands Antilles]";
                case 363: return $"{oidPath}/[Aruba]";
                case 364: return $"{oidPath}/[Commonwealth of the Bahamas]";
                case 365: return $"{oidPath}/[Anguilla]";
                case 366: return $"{oidPath}/[Commonwealth of Dominica]";
                case 368: return $"{oidPath}/[Cuba]";
                case 370: return $"{oidPath}/[Dominican Republic]";
                case 372: return $"{oidPath}/[Republic of Haiti]";
                case 374: return $"{oidPath}/[Trinidad and Tobago]";
                case 376: return $"{oidPath}/[Turks and Caicos Islands]";
                case 400: return $"{oidPath}/[Azerbaijani Republic]";
                case 401: return $"{oidPath}/[Republic of KAZAKHSTAN]";
                case 404: return $"{oidPath}/[Republic of India]";
                case 410:
                case 411: return $"{oidPath}/[Islamic Republic of Pakistan]";
                case 412: return $"{oidPath}/[Afghanistan]";
                case 413: return $"{oidPath}/[Democratic Scialist Republic of Sri Lanka]";
                case 414: return $"{oidPath}/[Union of MYANMAR]";
                case 415: return $"{oidPath}/[Lebanon]";
                case 416: return $"{oidPath}/[Hashemite Kingdom of Jordan]";
                case 417: return $"{oidPath}/[Syrian Arab Republic]";
                case 418: return $"{oidPath}/[Republic of Iraq]";
                case 419: return $"{oidPath}/[State of Kuwait]";
                case 420: return $"{oidPath}/[Kingdom of Saudi Arabia]";
                case 421: return $"{oidPath}/[Republic of Yemen]";
                case 422: return $"{oidPath}/[Sultanate of Oman]";
                case 423: return $"{oidPath}/[Reserved]";
                case 424: return $"{oidPath}/[United Arab Emirates]";
                case 425: return $"{oidPath}/[State of Israel]";
                case 426: return $"{oidPath}/[Kingdom of Bahrain]";
                case 427: return $"{oidPath}/[State of Qatar]";
                case 428: return $"{oidPath}/[Mongolia]";
                case 429: return $"{oidPath}/[Nepal]";
                case 430: return $"{oidPath}/[United Arab Emirates (Abu Dhabi)]";
                case 431: return $"{oidPath}/[United Arab Emirates (Dubai)]";
                case 432: return $"{oidPath}/[Islamic Republic of Iran]";
                case 434: return $"{oidPath}/[Republic of UZBEKISTAN]";
                case 436: return $"{oidPath}/[Republic of Tajikistan]";
                case 437: return $"{oidPath}/[Kyrgyz Republic]";
                case 438: return $"{oidPath}/[Turkmenistan]";
                //TODO: case 440: goto oid_0_2_440;
                case 441:
                case 442:
                case 443: return $"{oidPath}/[Japan]";
                //TODO: case 450: goto oid_0_2_450;
                case 452: return $"{oidPath}/[Viet Nam]";
                case 453:
                case 454: return $"{oidPath}/[Hong Kong, China]";
                case 455: return $"{oidPath}/[Macau, China]";
                case 456: return $"{oidPath}/[Kingdom of Cambodia]";
                case 457: return $"{oidPath}/[Lao People's Democratic Republic]";
                case 460: return $"{oidPath}/[People's Republic of China]";
                case 466: return $"{oidPath}/[Taiwan, Province of China]";
                case 467: return $"{oidPath}/[Democratic People's Republic of Korea]";
                case 470: return $"{oidPath}/[The People's Republic of Bangladesh]";
                case 472: return $"{oidPath}/[Republic of MALDIVES]";
                case 480:
                case 481: return $"{oidPath}/[Republic of Korea]";
                case 502: return $"{oidPath}/[Malaysia]";
                case 505: return $"{oidPath}/[AUSTRALIA]";
                case 510: return $"{oidPath}/[Republic of INDONESIA]";
                case 515: return $"{oidPath}/[Republic of the Philippines]";
                case 520: return $"{oidPath}/[Thailand]";
                case 525:
                case 526: return $"{oidPath}/[Republic of Singapore]";
                case 528: return $"{oidPath}/[Brunei Darussalam]";
                case 530: return $"{oidPath}/[New Zealand]";
                case 534: return $"{oidPath}/[Commonwealth of the Northern Mariana Islands]";
                case 535: return $"{oidPath}/[Guam]";
                case 536: return $"{oidPath}/[Republic of Nauru]";
                case 537: return $"{oidPath}/[Papua New Guinea]";
                case 539: return $"{oidPath}/[Kingdom of Tonga]";
                case 540: return $"{oidPath}/[Solomon Islands]";
                case 541: return $"{oidPath}/[Republic of Vanuatu]";
                case 542: return $"{oidPath}/[Republic of Fiji]";
                case 543: return $"{oidPath}/[Wallis and Futuna (French Overseas Territory)]";
                case 544: return $"{oidPath}/[American Samoa]";
                case 545: return $"{oidPath}/[Republic of Kiribati]";
                case 546: return $"{oidPath}/[New Caledonia (French Overseas Territory)]";
                case 547: return $"{oidPath}/[French Polynesia (French Overseas Territory)]";
                case 548: return $"{oidPath}/[Cook Islands]";
                case 549: return $"{oidPath}/[Independent State of Samoa]";
                case 550: return $"{oidPath}/[Federated States of Micronesia]";
                case 602: return $"{oidPath}/[Arab Republic of Egypt]";
                case 603: return $"{oidPath}/[People's Democratic Republic of Algeria]";
                case 604: return $"{oidPath}/[Kingdom of Morocco]";
                case 605: return $"{oidPath}/[Tunisia]";
                case 606: return $"{oidPath}/[Socialist People's Libyan Arab Jamahiriya]";
                case 607: return $"{oidPath}/[The Republic of the Gambia]";
                case 608: return $"{oidPath}/[Republic of Senegal]";
                case 609: return $"{oidPath}/[Islamic Republic of Mauritania]";
                case 610: return $"{oidPath}/[Republic of Mali]";
                case 611: return $"{oidPath}/[Republic of Guinea]";
                case 612: return $"{oidPath}/[Republic of Côte d'Ivoire]";
                case 613: return $"{oidPath}/[Burkina Faso]";
                case 614: return $"{oidPath}/[Republic of the Niger]";
                case 615: return $"{oidPath}/[Togolese Republic]";
                case 616: return $"{oidPath}/[Republic of Benin]";
                case 617: return $"{oidPath}/[Republic of Mauritius]";
                case 618: return $"{oidPath}/[Republic of Liberia]";
                case 619: return $"{oidPath}/[Sierra Leone]";
                case 620: return $"{oidPath}/[Ghana]";
                case 621: return $"{oidPath}/[Federal Republic of Nigeria]";
                case 622: return $"{oidPath}/[Republic of Chad]";
                case 623: return $"{oidPath}/[Central African Republic]";
                case 624: return $"{oidPath}/[Republic of Cameroon]";
                case 625: return $"{oidPath}/[Republic of Cape Verde]";
                case 626: return $"{oidPath}/[Democratic Republic of Sao Tome and Principe]";
                case 627: return $"{oidPath}/[Equatorial Guinea]";
                case 628: return $"{oidPath}/[Gabon]";
                case 629: return $"{oidPath}/[Republic of the Congo]";
                case 630: return $"{oidPath}/[Democratic Republic of the Congo]";
                case 631: return $"{oidPath}/[Republic of Angola]";
                case 632: return $"{oidPath}/[Republic of Guinea-Bissau]";
                case 633: return $"{oidPath}/[Republic of Seychelles]";
                case 634: return $"{oidPath}/[Republic of the Sudan]";
                case 635: return $"{oidPath}/[Republic of Rwanda]";
                case 636: return $"{oidPath}/[Federal Democratic Republic of Ethiopia]";
                case 637: return $"{oidPath}/[Somali Democratic Republic]";
                case 638: return $"{oidPath}/[Republic of Djibouti]";
                case 639: return $"{oidPath}/[Republic of Kenya]";
                case 640: return $"{oidPath}/[United Republic of Tanzania]";
                case 641: return $"{oidPath}/[Republic of Uganda]";
                case 642: return $"{oidPath}/[Republic of Burundi]";
                case 643: return $"{oidPath}/[Republic of Mozambique]";
                case 645: return $"{oidPath}/[Republic of Zambia]";
                case 646: return $"{oidPath}/[Republic of Madagascar]";
                case 647: return $"{oidPath}/[French Departments and Territories in the Indian Ocean]";
                case 648: return $"{oidPath}/[Republic of Zimbabwe]";
                case 649: return $"{oidPath}/[Republic of Namibia]";
                case 650: return $"{oidPath}/[Malawi]";
                case 651: return $"{oidPath}/[Kingdom of Lesotho]";
                case 652: return $"{oidPath}/[Republic of Botswana]";
                case 653: return $"{oidPath}/[Eswatini (formerly, Kingdom of Swaziland)]";
                case 654: return $"{oidPath}/[Union of the Comoros]";
                case 655: return $"{oidPath}/[Republic of South Africa]";
                case 658: return $"{oidPath}/[Eritrea]";
                case 702: return $"{oidPath}/[Belize]";
                case 704: return $"{oidPath}/[Republic of Guatemala]";
                case 706: return $"{oidPath}/[Republic of El Salvador]";
                case 708: return $"{oidPath}/[Republic of Honduras]";
                case 710: return $"{oidPath}/[Nicaragua]";
                case 712: return $"{oidPath}/[Costa Rica]";
                case 714: return $"{oidPath}/[Republic of Panama]";
                case 716: return $"{oidPath}/[Peru]";
                case 722: return $"{oidPath}/[ARGENTINE Republic]";
                case 724:
                case 725: return $"{oidPath}/[Federative Republic of Brazil]";
                case 730: return $"{oidPath}/[Chile]";
                case 732: return $"{oidPath}/[Republic of Colombia]";
                case 734: return $"{oidPath}/[Bolivarian Republic of Venezuela]";
                case 736: return $"{oidPath}/[Republic of Bolivia]";
                case 738: return $"{oidPath}/[Guyana]";
                case 740: return $"{oidPath}/[Ecuador]";
                case 742: return $"{oidPath}/[French Department of Guiana]";
                case 744: return $"{oidPath}/[Republic of PARAGUAY]";
                case 746: return $"{oidPath}/[Republic of Suriname]";
                case 748: return $"{oidPath}/[Eastern Republic of Uruguay]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // network-operator
        #region 0.3.*

        oid_0_3:

            oidPath += "/Network-Operator";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1111: return $"{oidPath}/[INMARSAT: Atlantic Ocean-East]";
                case 1112: return $"{oidPath}/[INMARSAT: Pacific Ocean]";
                case 1113: return $"{oidPath}/[INMARSAT: Indian Ocean]";
                case 1114: return $"{oidPath}/[INMARSAT: Atlantic Ocean-West]";
                case 2023: return $"{oidPath}/[Greece: Packet Switched Public Data Network (HELLASPAC)]";
                case 2027: return $"{oidPath}/[Greece: LAN-NET]";
                case 2041: return $"{oidPath}/[Netherlands: Datanet 1 X.25 access]";
                case 2044: return $"{oidPath}/[Netherlands: Unisource / Unidata]";
                case 2046: return $"{oidPath}/[Netherlands: Unisource / \"VPNS\"]";
                case 2052: return $"{oidPath}/[Netherlands: Naamloze Vennootschap (NV) CasTel]";
                case 2053: return $"{oidPath}/[Netherlands: Global One Communications BV]";
                case 2055: return $"{oidPath}/[Netherlands: Rabofacet BV]";
                case 2057: return $"{oidPath}/[Netherlands: Trionet v.o.f.]";
                case 2062: return $"{oidPath}/[Belgium: Réseau de transmission de données à commutation par paquets, Data Communication Service (DCS)]";
                case 2064: return $"{oidPath}/[Belgium: CODENET]";
                case 2065: return $"{oidPath}/[Belgium: Code utilisé au niveau national pour le réseau Data Communication Service (DCS)]";
                case 2066: return $"{oidPath}/[Belgium: Unisource Belgium X.25 Service (code canceled)]";
                case 2067: return $"{oidPath}/[Belgium: MOBISTAR]";
                case 2068: return $"{oidPath}/[Belgium: Accès au réseau Data Communication Service (DCS) via le réseau telex commuté national]";
                case 2069: return $"{oidPath}/[Belgium: Acces au reseau DCS via le reseau telephonique commute national]";
                case 2080: return $"{oidPath}/[France: Réseau de transmission de données à commutation par paquets \"TRANSPAC\"]";
                case 2081: return $"{oidPath}/[France: Noeud de transit international]";
                case 2082: return $"{oidPath}/[France: Grands services publics]";
                case 2083: return $"{oidPath}/[France: Administrations]";
                case 2084: return $"{oidPath}/[France: Air France]";
                case 2085: return $"{oidPath}/[France: \"SIRIS\"]";
                case 2086: return $"{oidPath}/[France: BT France]";
                case 2089: return $"{oidPath}/[France: Interconnexion entre le réseau public de transmission de données Transpac et d'autres réseaux publics français, pour des services offerts en mode synchrone]";
                case 2135: return $"{oidPath}/[Andorra: ANDORPAC]";
                case 2140: return $"{oidPath}/[Spain: Administracion Publica]";
                case 2141: return $"{oidPath}/[Spain: Nodo internacional de datos]";
                case 2142: return $"{oidPath}/[Spain: RETEVISION]";
                case 2145: return $"{oidPath}/[Spain: Red IBERPAC]";
                case 2147: return $"{oidPath}/[Spain: France Telecom Redes y Servicios]";
                case 2149: return $"{oidPath}/[Spain: MegaRed]";
                case 2161: return $"{oidPath}/[Hungary: Packet Switched Data Service]";
                case 2180: return $"{oidPath}/[Bosnia and Herzegovina: \"BIHPAK\"]";
                case 2191: return $"{oidPath}/[Croatia: CROAPAK (Croatian Packet Switching Data Network)]";
                case 2201: return $"{oidPath}/[Serbia and Montenegro: YUgoslav PACket (YUPAC) switched public data network]";
                case 2221: return $"{oidPath}/[Italy: Rete Telex-Dati (Amministrazione P.T. / national)]";
                case 2222: return $"{oidPath}/[Italy: \"ITAPAC\" X.25]";
                case 2223: return $"{oidPath}/[Italy: Packet Network (PAN)]";
                case 2226: return $"{oidPath}/[Italy: \"ITAPAC\" - X.32 Public Switched Telephone Network (Public Switched Telephone Network (PSTN)), X.28, D channel]";
                case 2227: return $"{oidPath}/[Italy: \"ITAPAC International\"]";
                case 2233: return $"{oidPath}/[Italy: \"ALBADATA X.25\"]";
                case 2234: return $"{oidPath}/[Italy: Trasmissione dati a commutazione di pacchetto X.25 (UNISOURCE Italia S.p.A.)]";
                case 2235: return $"{oidPath}/[Italy: Trasmissione dati a commutazione di pacchetto X.25 (INFOSTRADA S.p.A.)]";
                case 2236: return $"{oidPath}/[Italy: Trasmissione dati a commutazione di pacchetto X.25 (WIND Telecomunicazioni S.p.A.)]";
                case 2237: return $"{oidPath}/[Italy: Trasmissione dati a commutazione di pacchetto X.25 (Atlanet S.p.A.)]";
                case 2250: return $"{oidPath}/[Vatican: Packet Switching Data Network (PSDN) of Vatican City State]";
                case 2260: return $"{oidPath}/[Romania: \"ROMPAC\"]";
                case 2280: return $"{oidPath}/[Switzerland: ISDNPac]";
                case 2282: return $"{oidPath}/[Switzerland: Transpac-CH]";
                case 2283: return $"{oidPath}/[Switzerland: Bebbicel]";
                case 2284: return $"{oidPath}/[Switzerland: Telepac]";
                case 2285: return $"{oidPath}/[Switzerland: Telepac (acces de reseaux prives)]";
                case 2286: return $"{oidPath}/[Switzerland: DataRail]";
                case 2287: return $"{oidPath}/[Switzerland: Spack]";
                case 2301: return $"{oidPath}/[Czech Republic: \"NEXTEL\"]";
                case 2302: return $"{oidPath}/[Czech Republic: Aliatel (code canceled)]";
                case 2311: return $"{oidPath}/[Slovakia: EuroTel]";
                case 2322: return $"{oidPath}/[Austria: Dataswitch (DATAKOM)]";
                case 2329: return $"{oidPath}/[Austria: Radausdata (DATAKOM)]";
                case 2340: return $"{oidPath}/[United Kingdom: British Telecommunications plc (BT)]";
                case 2341: return $"{oidPath}/[United Kingdom: International Packet Switching Service (IPSS)]";
                case 2342: return $"{oidPath}/[United Kingdom: Packet Switched Service (PSS)]";
                case 2343:
                case 2344: return $"{oidPath}/[United Kingdom: British Telecommunications plc (BT) Concert Packet Network]";
                case 2347:
                case 2348: return $"{oidPath}/[United Kingdom: British Telecommunications plc (BT)]";
                case 2349: return $"{oidPath}/[United Kingdom: Barclays Technology Services]";
                case 2350:
                case 2351: return $"{oidPath}/[United Kingdom: C&W X.25 Service, international packet gateway]";
                case 2352: return $"{oidPath}/[United Kingdom: Kingston Communications (Hull) Plc.]";
                case 2353: return $"{oidPath}/[United Kingdom: Vodaphone, Packet Network Service]";
                case 2354: return $"{oidPath}/[United Kingdom: Nomura Computer Systems Europe Ltd. (NCC-E)]";
                case 2355: return $"{oidPath}/[United Kingdom: \"JAIS Europe Ltd.\"]";
                case 2357: return $"{oidPath}/[United Kingdom: \"FEDEX UK\"]";
                case 2358: return $"{oidPath}/[United Kingdom: Reuters]";
                case 2359: return $"{oidPath}/[United Kingdom: British Telecommunications plc (BT)]";
                case 2360: return $"{oidPath}/[United Kingdom: AT&T \"ISTEL\"]";
                case 2370: return $"{oidPath}/[United Kingdom: GlobalOne (France Telecom)]";
                case 2378: return $"{oidPath}/[United Kingdom: Racal Telecom]";
                case 2380: return $"{oidPath}/[Denmark: Tele Danmark A/S]";
                case 2381: return $"{oidPath}/[Denmark: \"DATEX\" (Circuit switched network)]";
                case 2382:
                case 2383: return $"{oidPath}/[Denmark: \"DATAPAK\", packet switched network]";
                case 2384: return $"{oidPath}/[Denmark: Transpac]";
                case 2385: return $"{oidPath}/[Denmark: SONOFON Global System for Mobile communications (GSM)]";
                case 2401: return $"{oidPath}/[Sweden: Datex (Circuit Switched Public Data Network) - TeliaSonera AB (code canceled)]";
                case 2402: return $"{oidPath}/[Sweden: WM-data Infrastructur (code canceled)]";
                case 2403: return $"{oidPath}/[Sweden: Datapak (Packet Switched Public Data Network) - TeliaSonera AB]";
                case 2406: return $"{oidPath}/[Sweden: Flex25 (Public Packet Switched Data Network)]";
                case 2407: return $"{oidPath}/[Sweden: Private X.25 Networks (DNIC allocated for a group of private networks) - TeliaSonera AB]";
                case 2408: return $"{oidPath}/[Sweden: TRANSPAC Scandinavia AB (code canceled)]";
                case 2421: return $"{oidPath}/[Norway: \"DATEX\" Circuit Switched Data Network (CSDN)]";
                case 2422: return $"{oidPath}/[Norway: DATAPAK (Packet Switched Network, PSDN)]";
                case 2429: return $"{oidPath}/[Norway: Shared by private data networks for Private Network Identification Code (PNIC) allocation]";
                case 2442: return $"{oidPath}/[Finland: Datapak]";
                case 2443: return $"{oidPath}/[Finland: Finpak (Packet Switched Data Network, PSDN) of Helsinki Telephone Company Ltd.]";
                case 2444: return $"{oidPath}/[Finland: Telia Finland Ltd.]";
                case 2462: return $"{oidPath}/[Lithuania: Vilnius DATAPAK]";
                case 2463: return $"{oidPath}/[Lithuania: Omnitel]";
                case 2471: return $"{oidPath}/[Latvia: Latvia Public Packed Switched Data Network]";
                case 2472: return $"{oidPath}/[Latvia: Tele2]";
                case 2473: return $"{oidPath}/[Latvia: Telekom Baltija]";
                case 2474: return $"{oidPath}/[Latvia: \"MDBA\"]";
                case 2475: return $"{oidPath}/[Latvia: Rigatta]";
                case 2476: return $"{oidPath}/[Latvia: Rixtel]";
                case 2477: return $"{oidPath}/[Latvia: Advem]";
                case 2478: return $"{oidPath}/[Latvia: \"AWA\" Baltic]";
                case 2480: return $"{oidPath}/[Estonia: \"ESTPAK\"]";
                case 2500: return $"{oidPath}/[Russian Federation: Rospack-RT]";
                case 2501: return $"{oidPath}/[Russian Federation: \"SPRINT\" Networks]";
                case 2502: return $"{oidPath}/[Russian Federation: \"IASNET\"]";
                case 2503: return $"{oidPath}/[Russian Federation: \"MMTEL\"]";
                case 2504: return $"{oidPath}/[Russian Federation: INFOTEL]";
                case 2506: return $"{oidPath}/[Russian Federation: \"ROSNET\"]";
                case 2507: return $"{oidPath}/[Russian Federation: ISTOK-K]";
                case 2508: return $"{oidPath}/[Russian Federation: TRANSINFORM]";
                case 2509: return $"{oidPath}/[Russian Federation: LENFINCOM]";
                case 2510: return $"{oidPath}/[Russian Federation: SOVAMNET]";
                case 2511: return $"{oidPath}/[Russian Federation: EDITRANS]";
                case 2512: return $"{oidPath}/[Russian Federation: \"TECOS\"]";
                case 2513: return $"{oidPath}/[Russian Federation: \"PTTNET\"]";
                case 2514: return $"{oidPath}/[Russian Federation: \"BCLNET\"]";
                case 2515: return $"{oidPath}/[Russian Federation: \"SPTNET\"]";
                case 2516: return $"{oidPath}/[Russian Federation: \"AS\" Sirena-3 data communication system]";
                case 2517: return $"{oidPath}/[Russian Federation: TELSYCOM]";
                case 2550: return $"{oidPath}/[Ukraine: UkrPack]";
                case 2551: return $"{oidPath}/[Ukraine: bkcNET]";
                case 2555: return $"{oidPath}/[Ukraine: \"GTNET\"]";
                case 2556: return $"{oidPath}/[Ukraine: UkrPack]";
                case 2570: return $"{oidPath}/[Belarus: \"BELPAK\"]";
                case 2601: return $"{oidPath}/[Poland: \"POLPAK\"]";
                case 2602: return $"{oidPath}/[Poland: \"NASK\" (code canceled)]";
                case 2603: return $"{oidPath}/[Poland: TELBANK]";
                case 2604: return $"{oidPath}/[Poland: \"POLPAK -T\"]";
                case 2605: return $"{oidPath}/[Poland: \"PKONET\" (code canceled)]";
                case 2606: return $"{oidPath}/[Poland: Shared by a number of data networks (code canceled)]";
                case 2607: return $"{oidPath}/[Poland: \"CUPAK\"]";
                case 2621: return $"{oidPath}/[Germany: ISDN/X.25]";
                case 2622: return $"{oidPath}/[Germany: Circuit Switched Data Service (DATEX-L)]";
                case 2624: return $"{oidPath}/[Germany: Packet Switched Data Service (DATEX-P)]";
                case 2625: return $"{oidPath}/[Germany: Satellite Services]";
                case 2627: return $"{oidPath}/[Germany: Teletex]";
                case 2629: return $"{oidPath}/[Germany: D2-Mannesmann]";
                case 2631: return $"{oidPath}/[Germany: CoNetP]";
                case 2632: return $"{oidPath}/[Germany: \"RAPNET\"]";
                case 2633: return $"{oidPath}/[Germany: \"DPS\"]";
                case 2634: return $"{oidPath}/[Germany: EkoNet]";
                case 2636: return $"{oidPath}/[Germany: ARCOR/PSN-1]";
                case 2640: return $"{oidPath}/[Germany: DETECON]";
                case 2641: return $"{oidPath}/[Germany: \"SCN\"]";
                case 2642: return $"{oidPath}/[Germany: \"INFO AG NWS\"]";
                case 2644: return $"{oidPath}/[Germany: \"IDNS\"]";
                case 2645: return $"{oidPath}/[Germany: ARCOR/otelo-net1]";
                case 2646: return $"{oidPath}/[Germany: EuroDATA]";
                case 2647: return $"{oidPath}/[Germany: ARCOR/otelo-net2]";
                case 2648: return $"{oidPath}/[Germany: SNSPac]";
                case 2649: return $"{oidPath}/[Germany: \"MMONET\"]";
                case 2651: return $"{oidPath}/[Germany: WestLB X.25 Net]";
                case 2652: return $"{oidPath}/[Germany: PSN/FSINFOSYSBW]";
                case 2653: return $"{oidPath}/[Germany: ARCOR/PSN-2]";
                case 2654: return $"{oidPath}/[Germany: \"TNET\"]";
                case 2655: return $"{oidPath}/[Germany: ISIS_DUS]";
                case 2656: return $"{oidPath}/[Germany: \"RWE TELPAC\"]";
                case 2657: return $"{oidPath}/[Germany: DTN/AutoF FmNLw]";
                case 2658: return $"{oidPath}/[Germany: \"DRENET\"]";
                case 2659: return $"{oidPath}/[Germany: GCN (Geno Communication Network)]";
                case 2680: return $"{oidPath}/[Portugal: PrimeNet]";
                case 2681: return $"{oidPath}/[Portugal: OniSolutions -Infocomunicacies, S.A.]";
                case 2682: return $"{oidPath}/[Portugal: CPRM-Marconi]";
                case 2683: return $"{oidPath}/[Portugal: Eastecnica, Electronica e Tecnica, S.A.]";
                case 2684: return $"{oidPath}/[Portugal: PrimeNet]";
                case 2685: return $"{oidPath}/[Portugal: Global One - Comunicacies, S.A.]";
                case 2686: return $"{oidPath}/[Portugal: \"HLC\", Telecomunicacies & Multimedia, S.A.]";
                case 2687: return $"{oidPath}/[Portugal: Jazztel Portugal - Servicos de Telecomunicacies, S.A.]";
                case 2702: return $"{oidPath}/[Luxembourg: CODENET]";
                case 2703: return $"{oidPath}/[Luxembourg: Regional \"ATS\" Packet switched NETwork (RAPNET)]";
                case 2704: return $"{oidPath}/[Luxembourg: \"LUXPAC\" (réseau de transmission de données à commutation par paquets)]";
                case 2705: return $"{oidPath}/[Luxembourg: \"LUXNET\" (interconnection avec le réseau public de transmission de données)]";
                case 2709: return $"{oidPath}/[Luxembourg: \"LUXPAC\" (accès X.28 et X.32 au réseau téléphonique commuté)]";
                case 2721: return $"{oidPath}/[Ireland: International Packet Switched Service]";
                case 2723: return $"{oidPath}/[Ireland: EURONET]";
                case 2724: return $"{oidPath}/[Ireland: \"EIRPAC\" (Packet Switched Data Networks)]";
                case 2728: return $"{oidPath}/[Ireland: PostNET (PostGEM Packet Switched Data Network)]";
                case 2740: return $"{oidPath}/[Iceland: ISPAK/ICEPAC]";
                case 2782: return $"{oidPath}/[Malta: MALTAPAC (Packet Switching Service)]";
                case 2802: return $"{oidPath}/[Cyprus: CYTAPAC - Public Switched Data Network (PSDN), subscribers with direct access]";
                case 2808: return $"{oidPath}/[Cyprus: CYTAPAC - Public Switched Data Network (PSDN), subscribers with access via telex]";
                case 2809: return $"{oidPath}/[Cyprus: CYTAPAC - Public Switched Data Network (PSDN), subscribers with access via Public Switched Telephone Network (Public Switched Telephone Network (PSTN)) - X.28, X.32]";
                case 2821: return $"{oidPath}/[Georgia: IBERIAPAC]";
                case 2830: return $"{oidPath}/[Armenia: ArmPac]";
                case 2860: return $"{oidPath}/[Turkey: TELETEX]";
                case 2861: return $"{oidPath}/[Turkey: DATEX-L]";
                case 2863: return $"{oidPath}/[Turkey: TURkish PAcKet switched data network (TURPAK)]";
                case 2864: return $"{oidPath}/[Turkey: \"TURPAK\"]";
                case 2881: return $"{oidPath}/[Faroe Islands: FAROEPAC]";
                case 2901: return $"{oidPath}/[Greenland: DATAPAK (Packet Switched Network)]";
                case 2922: return $"{oidPath}/[San Marino: X-Net \"SMR\"]";
                case 2931: return $"{oidPath}/[Slovenia: SIPAX.25]";
                case 2932: return $"{oidPath}/[Slovenia: SIPAX.25 access through Integrated Services Digital Network (ISDN) (code canceled)]";
                case 2940: return $"{oidPath}/[The Former Yugoslav Republic of Macedonia: \"MAKPAK\"]";
                case 3020: return $"{oidPath}/[Canada: Telecom Canada Datapak Network]";
                case 3021: return $"{oidPath}/[Canada: Telecom Canada Public Switched Telephone Network (Public Switched Telephone Network (PSTN)) Access]";
                case 3022: return $"{oidPath}/[Canada: Stentor Private Packet Switched Data Network Gateway]";
                case 3023: return $"{oidPath}/[Canada: Stentor Integrated Services Digital Network (ISDN) identification]";
                case 3024: return $"{oidPath}/[Canada: Teleglobe Canada - Globedat-C Circuit Switched Data Transmission]";
                case 3025: return $"{oidPath}/[Canada: Teleglobe Canada - Globedat-P Packed Switched Data Transmission]";
                case 3026: return $"{oidPath}/[Canada: AT&T Canada Long Distance Services - FasPac]";
                case 3028: return $"{oidPath}/[Canada: AT&T Canada Long Distance Services - Packet Switched Public Data Network (PSPDN)]";
                case 3036: return $"{oidPath}/[Canada: Sprint Canada Frame Relay Service - Packet-Switched Network]";
                case 3037: return $"{oidPath}/[Canada: \"TMI Communications\", Limited Partnership - Mobile Data Service (MDS)]";
                case 3038: return $"{oidPath}/[Canada: Canada Post - POSTpac - X.25 Packet Switched Data Network]";
                case 3039: return $"{oidPath}/[Canada: Telesat Canada - Anikom 200]";
                case 3101: return $"{oidPath}/[United States: PTN-1 Western Union Packet Switching Network]";
                case 3102: return $"{oidPath}/[United States: \"MCI\" Public Data Network (ResponseNet)]";
                case 3103: return $"{oidPath}/[United States: \"ITT UDTS\" Network]";
                case 3104: return $"{oidPath}/[United States: MCI Public Data Network (International Gateway)]";
                case 3105: return $"{oidPath}/[United States: \"WUI\" Leased Channel Network]";
                case 3106: return $"{oidPath}/[United States: Tymnet Network]";
                case 3107: return $"{oidPath}/[United States: \"ITT\" Datel Network]";
                case 3108: return $"{oidPath}/[United States: ITT Short Term Voice/Data Transmission Network]";
                case 3109: return $"{oidPath}/[United States: \"RCAG DATEL II\"]";
                case 3110: return $"{oidPath}/[United States: Telenet Communications Corporation]";
                case 3111: return $"{oidPath}/[United States: \"RCAG DATEL I\" (Switched Alternate Voice-Data Service)]";
                case 3112: return $"{oidPath}/[United States: Western Union Teletex Service]";
                case 3113: return $"{oidPath}/[United States: \"RCAG\" Remote Global Computer Access Service (Low Speed)]";
                case 3114: return $"{oidPath}/[United States: Western Union Infomaster]";
                case 3115: return $"{oidPath}/[United States: Graphnet Interactive Network]";
                case 3116: return $"{oidPath}/[United States: Graphnet Store and Forward Network]";
                case 3117: return $"{oidPath}/[United States: \"WUI\" Telex Network]";
                case 3118: return $"{oidPath}/[United States: Graphnet Data Network]";
                case 3119: return $"{oidPath}/[United States: \"TRT\" International Packet Switched Service (IPSS)]";
                case 3120: return $"{oidPath}/[United States: \"ITT\" Low Speed Network]";
                case 3121: return $"{oidPath}/[United States: \"FTCC\" Circuit Switched Network]";
                case 3122: return $"{oidPath}/[United States: FTCC Telex]";
                case 3123: return $"{oidPath}/[United States: FTCC Domestic Packet Switched Transmission (PST) Service]";
                case 3124: return $"{oidPath}/[United States: FTCC International PST Service]";
                case 3125: return $"{oidPath}/[United States: \"UNINET\"]";
                case 3126: return $"{oidPath}/[United States: \"ADP\" Autonet]";
                case 3127: return $"{oidPath}/[United States: \"GTE\" Telenet Communications Corporation]";
                case 3128: return $"{oidPath}/[United States: \"TRT\" Mail/Telex Network]";
                case 3129: return $"{oidPath}/[United States: \"TRT\" Circuit Switch Data (\"ICSS\")]";
                case 3130: return $"{oidPath}/[United States: TRT Digital Data Network]";
                case 3131: return $"{oidPath}/[United States: \"RCAG\" Telex Network]";
                case 3132: return $"{oidPath}/[United States: Compuserve Network Services]";
                case 3133: return $"{oidPath}/[United States: \"RCAG XNET\" Service]";
                case 3134: return $"{oidPath}/[United States: AT&T/ACCUNET Packet Switched Capability]";
                case 3135: return $"{oidPath}/[United States: ALASCOM/ALASKANET Service]";
                case 3136: return $"{oidPath}/[United States: Geisco Data Network]";
                case 3137: return $"{oidPath}/[United States: International Information Network Services - INFONET Service]";
                case 3138: return $"{oidPath}/[United States: Fedex International Transmission Corporation - International Document Transmission Service]";
                case 3139: return $"{oidPath}/[United States: \"KDD America\", Inc. - Public Data Network]";
                case 3140: return $"{oidPath}/[United States: Southern New England Telephone Company - Public Packet Network]";
                case 3141: return $"{oidPath}/[United States: Bell Atlantic Telephone Companies - Advance Service]";
                case 3142: return $"{oidPath}/[United States: Bellsouth Corporation - Pulselink Service]";
                case 3143: return $"{oidPath}/[United States: Ameritech Operating Companies - Public Packet Data Networks]";
                case 3144: return $"{oidPath}/[United States: Nynex Telephone Companies - Nynex Infopath Service]";
                case 3145: return $"{oidPath}/[United States: Pacific Telesis Public Packet Switching Service]";
                case 3146: return $"{oidPath}/[United States: Southwestern Bell Telephone Co. - Microlink \"II\" Public Packet Switching Service]";
                case 3147: return $"{oidPath}/[United States: U.S. West, Inc. - Public Packet Switching Service]";
                case 3148: return $"{oidPath}/[United States: United States Telephone Association - to be shared by local exchange telephone companies]";
                case 3149: return $"{oidPath}/[United States: Cable & Wireless Communications, Inc. - Public Data Network]";
                case 3150: return $"{oidPath}/[United States: Globenet, Inc. - Globenet Network Packet Switching Service]";
                case 3151: return $"{oidPath}/[United States: Data America Corporation - Data America Network]";
                case 3152: return $"{oidPath}/[United States: \"GTE\" Hawaiian Telephone Company, Inc. - Public Data Network]";
                case 3153: return $"{oidPath}/[United States: \"JAIS USA-NET\" Public Packet Switching Service]";
                case 3154: return $"{oidPath}/[United States: Nomura Computer Systems America, Inc. - \"NCC-A VAN\" public packet switching service]";
                case 3155: return $"{oidPath}/[United States: Aeronautical Radio, Inc. - GLOBALINK]";
                case 3156: return $"{oidPath}/[United States: American Airlines, Inc. - \"AANET\"]";
                case 3157: return $"{oidPath}/[United States: \"COMSAT\" Mobile Communications - \"C-LINK\"]";
                case 3158: return $"{oidPath}/[United States: Schlumberger Information NETwork (SINET)]";
                case 3159: return $"{oidPath}/[United States: Westinghouse Communications - Westinghouse Packet Network]";
                case 3160: return $"{oidPath}/[United States: Network Users Group, Ltd. - \"WDI NET\" packet]";
                case 3161: return $"{oidPath}/[United States: United States Department of State, Diplomatic Telecommunications Service]";
                case 3162: return $"{oidPath}/[United States: Transaction Network Services (TNS), Inc. -- Public packet-switched network]";
                case 3166: return $"{oidPath}/[United States: U.S. Department of Treasury Wide Area Data Network]";
                case 3168: return $"{oidPath}/[United States: \"BT\" North America packet-switched data network]";
                case 3169: return $"{oidPath}/[United States: Tenzing Communications Inc. - Inflight Network]";
                case 3302: return $"{oidPath}/[Puerto Rico: Asynchronous Transfer Mode (ATM) Broadband Network]";
                case 3303: return $"{oidPath}/[Puerto Rico: TDNet Puerto Rico]";
                case 3340: return $"{oidPath}/[Mexico: \"TELEPAC\"]";
                case 3341: return $"{oidPath}/[Mexico: \"UNITET\"]";
                case 3342: return $"{oidPath}/[Mexico: IUSANET]";
                case 3343: return $"{oidPath}/[Mexico: \"TEI\"]";
                case 3344: return $"{oidPath}/[Mexico: \"OPTEL\"]";
                case 3345: return $"{oidPath}/[Mexico: TELNORPAC]";
                case 3346: return $"{oidPath}/[Mexico: \"TYMPAQ\"]";
                case 3347: return $"{oidPath}/[Mexico: SINFRARED]";
                case 3348: return $"{oidPath}/[Mexico: INTERVAN]";
                case 3349: return $"{oidPath}/[Mexico: INTELCOMNET]";
                case 3350: return $"{oidPath}/[Mexico: AVANTEL, S.A.]";
                case 3351: return $"{oidPath}/[Mexico: ALESTRA, S. de R.L. de C.V.]";
                case 3422: return $"{oidPath}/[Barbados: CARIBNET]";
                case 3423: return $"{oidPath}/[Barbados: International Database Access Service (IDAS)]";
                case 3443: return $"{oidPath}/[Antigua and Barbuda: Antigua Packet Switched Service]";
                case 3463: return $"{oidPath}/[Cayman Islands: Cable and Wireless Packet Switching Node]";
                case 3502: return $"{oidPath}/[Bermuda: Cable and Wireless Data Communications Node]";
                case 3503: return $"{oidPath}/[Bermuda: Cable and Wireless Packet Switched Node]";
                case 3522: return $"{oidPath}/[Grenada: CARIBNET]";
                case 3620: return $"{oidPath}/[Netherlands Antilles: Telematic Network]";
                case 3621: return $"{oidPath}/[Netherlands Antilles: Datanet Curacao]";
                case 3680: return $"{oidPath}/[Cuba: Servicios de informacion por conmutacion de paquetes del \"IDICT\"]";
                case 3706: return $"{oidPath}/[Dominican Republic: All America Cables and Radio Inc.]";
                case 3740: return $"{oidPath}/[Trinidad and Tobago: \"TEXDAT\"]";
                case 3745: return $"{oidPath}/[Trinidad and Tobago: DATANETT]";
                case 3763:
                case 3764: return $"{oidPath}/[Turks and Caicos Islands: Cable and wireless packet switched node]";
                case 4001: return $"{oidPath}/[Azerbaijan: AZPAK (AZerbaijan public PAcKet switched data network)]";
                case 4002: return $"{oidPath}/[Azerbaijan: \"AzEuroTel\" Joint Venture]";
                case 4010: return $"{oidPath}/[Kazakhstan: KazNet X.25]";
                case 4011: return $"{oidPath}/[Kazakhstan: BankNet X.25]";
                case 4041: return $"{oidPath}/[India: \"RABMN\"]";
                case 4042: return $"{oidPath}/[India: International Gateway Packet Switching System (GPSS)]";
                case 4043: return $"{oidPath}/[India: \"INET\" (Packet Switched Public Data Network)]";
                case 4045: return $"{oidPath}/[India: HVnet]";
                case 4046: return $"{oidPath}/[India: Shared Data Network Identification Code (DNIC) for \"VSAT\" based private data networks]";
                case 4101: return $"{oidPath}/[Pakistan: TRANSLINK]";
                case 4132: return $"{oidPath}/[Sri Lanka: Lanka Communication Services (Pvt) Limited]";
                case 4133: return $"{oidPath}/[Sri Lanka: Electroteks (Pvt) Limited]";
                case 4141: return $"{oidPath}/[Myanmar: MYANMARP]";
                case 4155: return $"{oidPath}/[Lebanon: Reseau public de transmission de donnees par paquets]";
                case 4195: return $"{oidPath}/[Kuwait: Qualitynet]";
                case 4201: return $"{oidPath}/[Saudi Arabia: ALWASEET - Public Packet Switched Data Network]";
                case 4241: return $"{oidPath}/[United Arab Emirates: \"EMDAN\" Teletex Network]";
                case 4243: return $"{oidPath}/[United Arab Emirates: \"EMDAN\" X.25 and X.28 Terminals]";
                case 4251: return $"{oidPath}/[Israel: ISRANET]";
                case 4260: return $"{oidPath}/[Bahrain: Batelco Global System for Mobile communications (GSM) Service]";
                case 4262: return $"{oidPath}/[Bahrain: Bahrain MAnaged DAta Network (MADAN)]";
                case 4263: return $"{oidPath}/[Bahrain: Batelco Packet Switched Node]";
                case 4271: return $"{oidPath}/[Qatar: \"DOHPAK\"]";
                case 4290: return $"{oidPath}/[Nepal: NEPal PAcKet switched public data network (NEPPAK)]";
                case 4321: return $"{oidPath}/[Islamic Republic of Iran: IranPac]";
                case 4341: return $"{oidPath}/[Uzbekistan: UzPAK]";
                case 4400: return $"{oidPath}/[Japan: GLOBALNET (Network of the Global \"VAN\" Japan Incorporation)]";
                //TODO: case 4401: goto oid_0_3_4401;
                case 4402: return $"{oidPath}/[Japan: NEC-NET (NEC Corporation)]";
                case 4403: return $"{oidPath}/[Japan: \"JENSNET\" (\"JENS Corporation\")]";
                case 4404: return $"{oidPath}/[Japan: JAIS-NET (Japan Research Institute Ltd.)]";
                case 4405: return $"{oidPath}/[Japan: NCC-VAN (NRI Co., Ltd.)]";
                case 4406: return $"{oidPath}/[Japan: TYMNET-Japan (Japan TELECOM COMMUNICATIONS SERVICES CO., LTD.)]";
                case 4407: return $"{oidPath}/[Japan: International High Speed Switched Data Transmission Network (\"KDDI\") (code canceled)]";
                case 4408: return $"{oidPath}/[Japan: International Packet Switched Data Transmission Network (\"KDDI\") (code canceled)]";
                case 4412: return $"{oidPath}/[Japan: Sprintnet (Global One Communications, INC.)]";
                case 4413: return $"{oidPath}/[Japan: \"KYODO NET\" (\"UNITED NET\" Corp)]";
                case 4415: return $"{oidPath}/[Japan: \"FENICS\" (Fujitsu Limited)]";
                case 4416: return $"{oidPath}/[Japan: \"HINET\" (Hitachi Information Network, Ltd.)]";
                case 4417: return $"{oidPath}/[Japan: TIS-Net (TOYO Information Systems Co., Ltd.)]";
                case 4418: return $"{oidPath}/[Japan: TG-VAN (TOSHIBA Corporation)]";
                case 4420: return $"{oidPath}/[Japan: Pana-Net (Matsushita Electric Industrial Co. Ltd.)]";
                case 4421: return $"{oidPath}/[Japan: \"DDX-P\" (Nippon Telegraph and Telephone (NTT) Communications Corporation) (code canceled)]";
                case 4422: return $"{oidPath}/[Japan: CTC-P (CHUBU TELECOMMUNICATIONS CO., INC.)]";
                case 4423: return $"{oidPath}/[Japan: \"JENSNET\" (\"JENS Corporation\")]";
                case 4424: return $"{oidPath}/[Japan: \"SITA\" Network]";
                case 4425: return $"{oidPath}/[Japan: Global Managed Data Service (Cable & Wireless IDC-Si)]";
                case 4426: return $"{oidPath}/[Japan: \"ECHO-NET\" (Hitachi Information Systems Ltd.)]";
                case 4427: return $"{oidPath}/[Japan: U-net (Nihon Unysys Information Systems Ltd.)]";
                case 4500: return $"{oidPath}/[Republic of Korea: HiNET-P (Korea Telecom)]";
                case 4501: return $"{oidPath}/[Republic of Korea: DACOM-NET]";
                case 4502: return $"{oidPath}/[Republic of Korea: \"CSDN\" (only assigned to Teletex)]";
                case 4538: return $"{oidPath}/[Hong Kong, China: Cable & Wireless Regional Businesses (Hong Kong) Limited]";
                case 4540: return $"{oidPath}/[Hong Kong, China: Public Switched Document Transfer Service]";
                case 4541: return $"{oidPath}/[Hong Kong, China: Hutchison Global Crossing Limited]";
                case 4542: return $"{oidPath}/[Hong Kong, China: INTELPAK (code canceled)]";
                case 4543: return $"{oidPath}/[Hong Kong, China: New T&T]";
                case 4545: return $"{oidPath}/[Hong Kong, China: Datapak]";
                case 4546: return $"{oidPath}/[Hong Kong, China: iAsiaWorks (Hong Kong) Service]";
                case 4547: return $"{oidPath}/[Hong Kong, China: New World Telephone Limited]";
                case 4548: return $"{oidPath}/[Hong Kong, China: \"KDD\" Telecomet Hong Kong Ltd.]";
                case 4550: return $"{oidPath}/[Macau: \"MACAUPAC\"]";
                case 4601: return $"{oidPath}/[China: Teletex and low speed data network]";
                case 4603: return $"{oidPath}/[China: \"CHINAPAC\"]";
                case 4604: return $"{oidPath}/[China: Reserved for public mobile data service]";
                case 4605: return $"{oidPath}/[China: Public data network]";
                case 4606:
                case 4607:
                case 4608: return $"{oidPath}/[China: Dedicated network]";
                case 4609: return $"{oidPath}/[China: China Railcom \"PAC\"]";
                case 4720: return $"{oidPath}/[Maldives: DATANET (Maldives Packet Switching Service)]";
                case 5020: return $"{oidPath}/[Malaysia: \"COINS\" Global Frame Relay]";
                case 5021: return $"{oidPath}/[Malaysia: Malaysian Public Packet Switched Public Data Network (\"MAYPAC\")]";
                case 5023: return $"{oidPath}/[Malaysia: Corporate Information Networks]";
                case 5024: return $"{oidPath}/[Malaysia: ACASIA-ASEAN Managed Overlay Network]";
                case 5026: return $"{oidPath}/[Malaysia: Mutiara Frame Relay Network]";
                case 5027: return $"{oidPath}/[Malaysia: Mobile Public Data Network (WAVENET)]";
                case 5028: return $"{oidPath}/[Malaysia: Global Management Data Services (GMDS)]";
                case 5052: return $"{oidPath}/[Australia: Telstra Corporation Ltd. - AUSTPAC packet switching network]";
                case 5053: return $"{oidPath}/[Australia: Telstra Corporation Ltd. - AUSTPAC International]";
                case 5057: return $"{oidPath}/[Australia: Australian Private Networks]";
                case 5101: return $"{oidPath}/[Indonesia: Sambungan Komunikasi Data Paket (SKDP) Packet Switched Service]";
                case 5151: return $"{oidPath}/[Philippines: \"CWI DATANET\" - Capitol Wireless, Inc. (CAPWIRE)]";
                case 5152: return $"{oidPath}/[Philippines: Philippine Global Communications, Inc. (PHILCOM)]";
                case 5154: return $"{oidPath}/[Philippines: Globe-Mackay Cable and Radio corp. (GMCR)]";
                case 5156: return $"{oidPath}/[Philippines: Eastern Telecommunications Philippines, Inc. (ETPI)]";
                case 5157: return $"{oidPath}/[Philippines: DATAPAC]";
                case 5202: return $"{oidPath}/[Thailand: THAIPAK 2 - Value Added Public Packet Switched Data Network]";
                case 5203: return $"{oidPath}/[Thailand: \"CAT\" Store and Forward Fax Network]";
                case 5209: return $"{oidPath}/[Thailand: \"TOT\" Integrated Services Digital Network (ISDN)]";
                case 5250: return $"{oidPath}/[Singapore: International telephone prefix]";
                case 5251: return $"{oidPath}/[Singapore: Inmarsat service]";
                case 5252: return $"{oidPath}/[Singapore: TELEPAC (Public Packet Switching Data Network)]";
                case 5253: return $"{oidPath}/[Singapore: High speed data/long packet service]";
                case 5254:
                case 5255: return $"{oidPath}/[Singapore: Public Data Network]";
                case 5257: return $"{oidPath}/[Singapore: Integrated Services Digital Network (ISDN) packet switching service]";
                case 5258: return $"{oidPath}/[Singapore: Telex]";
                case 5259: return $"{oidPath}/[Singapore: Public Switched Telephone Network (PSTN) access (dial-in/out)]";
                case 5301: return $"{oidPath}/[New Zealand: \"PACNET\" Packet Switching Network]";
                case 5351: return $"{oidPath}/[Guam: The Pacific Connection, Inc. - Pacnet Public Packet Switching Service]";
                case 5390: return $"{oidPath}/[Tonga: TONGAPAK]";
                case 5400: return $"{oidPath}/[Solomon Islands: Datanet]";
                case 5410: return $"{oidPath}/[Vanuatu: Vanuatu International Access for PACkets (VIAPAC)]";
                case 5420: return $"{oidPath}/[Fiji: \"FIJPAK\"]";
                case 5421: return $"{oidPath}/[Fiji: FIJINET]";
                case 5460: return $"{oidPath}/[New Caledonia: Transpac - Nouvelle Calédonie et opérateur public local]";
                case 5470: return $"{oidPath}/[French Polynesia: Transpac - Polynésie et opérateur public local]";
                case 5501: return $"{oidPath}/[Micronesia: \"FSMTC\" Packet Switched Network]";
                case 6026: return $"{oidPath}/[Egypt: \"EGYPTNET\"]";
                case 6030: return $"{oidPath}/[Algeria: \"DZ PAC\" (Réseau public de données à commutation par paquets)]";
                case 6041: return $"{oidPath}/[Morocco: MAGHRIPAC]";
                case 6042: return $"{oidPath}/[Morocco: MAGHRIPAC X.32]";
                case 6049: return $"{oidPath}/[Morocco: MAGHRIPAC \"RTC PAD\"]";
                case 6070: return $"{oidPath}/[Gambia: \"GAMNET\"]";
                case 6081: return $"{oidPath}/[Senegal: \"SENPAC\"]";
                case 6122: return $"{oidPath}/[Côte d'Ivoire: SYTRANPAC]";
                case 6132: return $"{oidPath}/[Burkina Faso: FASOPAC]";
                case 6202: return $"{oidPath}/[Ghana: DATATEL]";
                case 6222: return $"{oidPath}/[Chad: TCHADPAC]";
                case 6242: return $"{oidPath}/[Cameroon: \"CAMPAC\"]";
                case 6255: return $"{oidPath}/[Cape Verde: \"CVDATA\"]";
                case 6280: return $"{oidPath}/[Gabon: GABONPAC (Réseau de transmission de données à commutation par paquets)]";
                case 6282: return $"{oidPath}/[Gabon: GABONPAC2]";
                case 6315: return $"{oidPath}/[Angola: ANGOPAC]";
                case 6331: return $"{oidPath}/[Seychelles: Infolink]";
                case 6390: return $"{oidPath}/[Kenya: \"KENPAC\" - Telkom Kenya Ltd.]";
                case 6435: return $"{oidPath}/[Mozambique: \"COMPAC\" (Packet Switching Public Data Network)]";
                case 6451: return $"{oidPath}/[Zambia: \"ZAMPAK\"]";
                case 6460: return $"{oidPath}/[Madagascar: INFOPAC]";
                case 6484: return $"{oidPath}/[Zimbabwe: \"ZIMNET\"]";
                case 6490: return $"{oidPath}/[Namibia: \"SWANET\" (Public Packet Switched Network)]";
                case 6550: return $"{oidPath}/[South Africa: Saponet - P]";
                case 7080: return $"{oidPath}/[Honduras: HONDUPAQ]";
                case 7100: return $"{oidPath}/[Nicaragua: NicaPac]";
                case 7120: return $"{oidPath}/[Costa Rica: RACSADATOS]";
                case 7141: return $"{oidPath}/[Panama: Red de transmision de datos con conmutacion de paquetes (INTELPAQ)]";
                case 7144: return $"{oidPath}/[Panama: \"CWP\" data network]";
                case 7160: return $"{oidPath}/[Peru: \"MEGANET\" (\"PERUNET\")]";
                case 7161: return $"{oidPath}/[Peru: \"MEGANET\"]";
                case 7221: return $"{oidPath}/[Argentina: Nodo Internacional de Datos - TELINTAR]";
                case 7222: return $"{oidPath}/[Argentina: \"ARPAC\" (\"ENTEL\")]";
                case 7223: return $"{oidPath}/[Argentina: EASYGATE (\"ATT\")]";
                case 7240: return $"{oidPath}/[Brazil: International Packet Switching Data Communication Service (INTERDATA)]";
                case 7241: return $"{oidPath}/[Brazil: National Packet Switching Data Communication Service (\"RENPAC\")]";
                case 7242: return $"{oidPath}/[Brazil: \"RIOPAC\"]";
                case 7243: return $"{oidPath}/[Brazil: MINASPAC]";
                case 7244: return $"{oidPath}/[Brazil: TRANSPAC]";
                case 7245: return $"{oidPath}/[Brazil: Fac Simile Service (DATA FAX)]";
                case 7246: return $"{oidPath}/[Brazil: Brazilian private networks]";
                case 7247: return $"{oidPath}/[Brazil: \"DATASAT BI\"]";
                case 7251: return $"{oidPath}/[Brazil: S.PPAC]";
                case 7252: return $"{oidPath}/[Brazil: \"TELEST\" Public packet data network]";
                case 7253: return $"{oidPath}/[Brazil: TELEMIG Public Switched Packet Data Network]";
                case 7254: return $"{oidPath}/[Brazil: \"PACPAR\"]";
                case 7255: return $"{oidPath}/[Brazil: CRT/CTMR]";
                case 7256: return $"{oidPath}/[Brazil: Western and Midwestern Public Switched Packet Data Network]";
                case 7257: return $"{oidPath}/[Brazil: TELEBAHIA and TELERGIPE Public Switched Packet Data Network]";
                case 7258: return $"{oidPath}/[Brazil: Northeastern Public Switched Packet Data Network]";
                case 7259: return $"{oidPath}/[Brazil: Northern Public Switched Packet Data Network]";
                case 7302: return $"{oidPath}/[Chile: Red nacional de transmision de datos]";
                case 7321: return $"{oidPath}/[Colombia: Red de Alta Velocidad]";
                case 7380: return $"{oidPath}/[Guyana: \"GT&T PAC\"]";
                case 7440: return $"{oidPath}/[Paraguay: PARABAN]";
                case 7447: return $"{oidPath}/[Paraguay: ANTELPAC]";
                case 7448: return $"{oidPath}/[Paraguay: PARAPAQ]";
                case 7482: return $"{oidPath}/[Uruguay: \"URUPAC\" - Servicio publico de transmision de datos con conmutacion de paquetes]";
                case 7488: return $"{oidPath}/[Uruguay: URUPAC - Interfuncionamiento con la red telex]";
                case 7489: return $"{oidPath}/[Uruguay: URUPAC - Interfuncionamiento con la red telefonica]";
                case 23030: return $"{oidPath}/[Czech Republic: \"G-NET\" (code canceled)]";
                case 23040:
                case 23041:
                case 23042:
                case 23043:
                case 23044: return $"{oidPath}/[Czech Republic: RadioNET]";
                case 41362: return $"{oidPath}/[Sri Lanka: \"MTT\" Network (Pvt) Limited]";
                case 41363: return $"{oidPath}/[Sri Lanka: \"DPMC\" Electronics (Pvt) Limited]";
                case 260621: return $"{oidPath}/[Poland: DATACOM]";
                case 260622: return $"{oidPath}/[Poland: \"MNI\"]";
                case 260641: return $"{oidPath}/[Poland: \"PAGI\"]";
                case 260642: return $"{oidPath}/[Poland: Crowley Data Poland]";
                case 260651: return $"{oidPath}/[Poland: MEDIATEL]";
                case 260661: return $"{oidPath}/[Poland: \"KOLPAK\"]";
                case 260662: return $"{oidPath}/[Poland: Energis Polska]";
                case 260672: return $"{oidPath}/[Poland: Virtual Private Network (VPN) Service]";
                case 260681: return $"{oidPath}/[Poland: Exatel]";
                case 260691: return $"{oidPath}/[Poland: \"NETIA\"]";
                case 460200:
                case 460201:
                case 460202:
                case 460203:
                case 460204:
                case 460205:
                case 460206:
                case 460207: return $"{oidPath}/[China: \"CAAC\" privileged data network]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // identified-organization
        #region 0.4.*

        oid_0_4:

            oidPath += "/Identified-Organization";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                //TODO: case 0: goto oid_0_4_0;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // data
        #region 0.9.*

        oid_0_9:

            oidPath += "/Data";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                //TODO: case 0: goto oid_0_9_0;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        #endregion

        // iso
        #region 1.*

        oid_1:

            oidPath += "/ISO";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_1_0;
                case 1: goto oid_1_1;
                case 2: goto oid_1_2;
                case 3: goto oid_1_3;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // standard
        #region 1.0.*

        oid_1_0:

            oidPath += "/Standard";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                //TODO: case 639: goto oid_1_0_639;
                //TODO: case 1087: goto oid_1_0_1087;
                //TODO: case 2022: goto oid_1_0_2022;
                //TODO: case 2382: goto oid_1_0_2382;
                //TODO: case 3166: goto oid_1_0_3166;
                case 4217: return $"{oidPath}/[Currency Codes]";
                //TODO: case 4426: goto oid_1_0_4426;
                //TODO: case 4922: goto oid_1_0_4922;
                case 5218: return $"{oidPath}/[Information technology -- Codes for the representation of human sexes]";
                case 6523: return $"{oidPath}/[Information technology -- Structure for the identification of organizations and organization parts]";
                //TODO: case 7498: goto oid_1_0_7498;
                //TODO: case 7816: goto oid_1_0_7816;
                //TODO: case 8571: goto oid_1_0_8571;
                case 8601: return $"{oidPath}/[Data elements and interchange formats -- Information interchange -- Representation of dates and times]";
                //TODO: case 8802: goto oid_1_0_8802;
                //TODO: case 9040: goto oid_1_0_9040;
                //TODO: case 9041: goto oid_1_0_9041;
                //TODO: case 9069: goto oid_1_0_9069;
                case 9362: return $"{oidPath}/[Banking -- Banking telecommunication messages -- Business Identifier Code (BIC)]";
                //TODO: case 9506: goto oid_1_0_9506;
                //TODO: case 9596: goto oid_1_0_9596;
                //TODO: case 9796: goto oid_1_0_9796;
                //TODO: case 9797: goto oid_1_0_9797;
                //TODO: case 9798: goto oid_1_0_9798;
                //TODO: case 9834: goto oid_1_0_9834;
                //TODO: case 9979: goto oid_1_0_9979;
                //TODO: case 9992: goto oid_1_0_9992;
                //TODO: case 10021: goto oid_1_0_10021;
                //TODO: case 10116: goto oid_1_0_10116;
                //TODO: case 10118: goto oid_1_0_10118;
                //TODO: case 10161: goto oid_1_0_10161;
                //TODO: case 10166: goto oid_1_0_10166;
                case 10374: return $"{oidPath}/[Freight containers -- Automatic identification]";
                //TODO: case 10646: goto oid_1_0_10646;
                //TODO: case 10746: goto oid_1_0_10746;
                case 10891: return $"{oidPath}/[Freight containers -- Radio frequency identification (RFID) -- Licence plate tag]";
                //TODO: case 11188: goto oid_1_0_11188;
                case 11404: return $"{oidPath}/[Information technology -- Programming languages, their environments and system software interfaces -- Language-independent datatypes]";
                //TODO: case 11578: goto oid_1_0_11578;
                //TODO: case 11582: goto oid_1_0_11582;
                //TODO: case 11770: goto oid_1_0_11770;
                //TODO: case 12813: goto oid_1_0_12813;
                //TODO: case 12855: goto oid_1_0_12855;
                //TODO: case 13141: goto oid_1_0_13141;
                case 13616: return $"{oidPath}/[Financial services -- International Bank Account Number (IBAN)]";
                //TODO: case 13868: goto oid_1_0_13868;
                //TODO: case 13869: goto oid_1_0_13869;
                //TODO: case 13870: goto oid_1_0_13870;
                //TODO: case 13873: goto oid_1_0_13873;
                //TODO: case 13874: goto oid_1_0_13874;
                //TODO: case 13888: goto oid_1_0_13888;
                //TODO: case 14813: goto oid_1_0_14813;
                //TODO: case 14816: goto oid_1_0_14816;
                //TODO: case 14823: goto oid_1_0_14823;
                //TODO: case 14843: goto oid_1_0_14843;
                //TODO: case 14844: goto oid_1_0_14844;
                //TODO: case 14846: goto oid_1_0_14846;
                //TODO: case 14888: goto oid_1_0_14888;
                //TODO: case 14906: goto oid_1_0_14906;
                //TODO: case 15050: goto oid_1_0_15050;
                //TODO: case 15052: goto oid_1_0_15052;
                //TODO: case 15054: goto oid_1_0_15054;
                //TODO: case 15118: goto oid_1_0_15118;
                //TODO: case 15418: goto oid_1_0_15418;
                //TODO: case 15429: goto oid_1_0_15429;
                //TODO: case 15431: goto oid_1_0_15431;
                //TODO: case 15433: goto oid_1_0_15433;
                case 15434: return $"{oidPath}/[Transfer Syntax for High Capacity data carrier]";
                //TODO: case 15459: goto oid_1_0_15459;
                //TODO: case 15506: goto oid_1_0_15506;
                //TODO: case 15507: goto oid_1_0_15507;
                //TODO: case 15628: goto oid_1_0_15628;
                //TODO: case 15772: goto oid_1_0_15772;
                //TODO: case 15946: goto oid_1_0_15946;
                //TODO: case 15961: goto oid_1_0_15961;
                //TODO: case 15992: goto oid_1_0_15992;
                //TODO: case 16460: goto oid_1_0_16460;
                //TODO: case 16785: goto oid_1_0_16785;
                //TODO: case 17090: goto oid_1_0_17090;
                //TODO: case 17262: goto oid_1_0_17262;
                //TODO: case 17264: goto oid_1_0_17264;
                //TODO: case 17419: goto oid_1_0_17419;
                //TODO: case 17423: goto oid_1_0_17423;
                //TODO: case 17429: goto oid_1_0_17429;
                //TODO: case 17515: goto oid_1_0_17515;
                //TODO: case 17573: goto oid_1_0_17573;
                //TODO: case 17575: goto oid_1_0_17575;
                //TODO: case 17876: goto oid_1_0_17876;
                //TODO: case 17878: goto oid_1_0_17878;
                //TODO: case 17922: goto oid_1_0_17922;
                //TODO: case 18013: goto oid_1_0_18013;
                //TODO: case 18014: goto oid_1_0_18014;
                case 18031: return $"{oidPath}/[Information technology -- Security techniques -- Random bit generation]";
                case 18032: return $"{oidPath}/[Information technology -- Security techniques -- Prime number generation]";
                //TODO: case 18033: goto oid_1_0_18033;
                //TODO: case 18370: goto oid_1_0_18370;
                //TODO: case 18750: goto oid_1_0_18750;
                //TODO: case 19079: goto oid_1_0_19079;
                //TODO: case 19091: goto oid_1_0_19091;
                //TODO: case 19321: goto oid_1_0_19321;
                //TODO: case 19460: goto oid_1_0_19460;
                //TODO: case 19592: goto oid_1_0_19592;
                //TODO: case 19772: goto oid_1_0_19772;
                //TODO: case 19785: goto oid_1_0_19785;
                //TODO: case 19794: goto oid_1_0_19794;
                //TODO: case 20008: goto oid_1_0_20008;
                //TODO: case 20009: goto oid_1_0_20009;
                case 20022: return $"{oidPath}/[Universal Financial Industry message scheme]";
                //TODO: case 20248: goto oid_1_0_20248;
                //TODO: case 20684: goto oid_1_0_20684;
                //TODO: case 20828: goto oid_1_0_20828;
                //TODO: case 21000: goto oid_1_0_21000;
                //TODO: case 21091: goto oid_1_0_21091;
                //TODO: case 21177: goto oid_1_0_21177;
                //TODO: case 21184: goto oid_1_0_21184;
                //TODO: case 21185: goto oid_1_0_21185;
                //TODO: case 21192: goto oid_1_0_21192;
                //TODO: case 21193: goto oid_1_0_21193;
                //TODO: case 21210: goto oid_1_0_21210;
                //TODO: case 21215: goto oid_1_0_21215;
                //TODO: case 21218: goto oid_1_0_21218;
                //TODO: case 21407: goto oid_1_0_21407;
                //TODO: case 21889: goto oid_1_0_21889;
                //TODO: case 22418: goto oid_1_0_22418;
                //TODO: case 22895: goto oid_1_0_22895;
                //TODO: case 23264: goto oid_1_0_23264;
                //TODO: case 24102: goto oid_1_0_24102;
                case 24531: return $"{oidPath}/[Intelligent Transport Systems (ITS) -- System architecture, taxonomy and terminology -- Using eXtensible Markup Language (XML) in ITS standards, data registries and data dictionaries]";
                //TODO: case 24534: goto oid_1_0_24534;
                //TODO: case 24727: goto oid_1_0_24727;
                //TODO: case 24753: goto oid_1_0_24753;
                //TODO: case 24761: goto oid_1_0_24761;
                case 24787: return $"{oidPath}/[Information technology -- Identification cards -- On-card biometric comparison]";
                //TODO: case 29150: goto oid_1_0_29150;
                //TODO: case 29192: goto oid_1_0_29192;
                //TODO: case 29281: goto oid_1_0_29281;
                //TODO: case 30107: goto oid_1_0_30107;
                //TODO: case 39794: goto oid_1_0_39794;
                //TODO: case 62351: goto oid_1_0_62351;
                //TODO: case 62379: goto oid_1_0_62379;
                //TODO: case 62439: goto oid_1_0_62439;
                //TODO: case 63047: goto oid_1_0_63047;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // registration-authority
        #region 1.1.*

        oid_1_1:

            oidPath += "/Registration-Authority";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: return $"{oidPath}/[reserved]";
                case 2: return $"{oidPath}/[document-type]";
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10: return $"{oidPath}/[reserved]";
                case 2108: return $"{oidPath}/[Information and documentation -- International Standard Book Numbering (ISBN)]";
                //TODO: case 2375: goto oid_1_1_2375;
                //TODO: case 10036: goto oid_1_1_10036;
                //TODO: case 19785: goto oid_1_1_19785;
                //TODO: case 24727: goto oid_1_1_24727;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // member-body
        #region 1.2.*

        oid_1_2:

            oidPath += "/Member-Body";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                //TODO: case 36: goto oid_1_2_36;
                //TODO: case 40: goto oid_1_2_40;
                //TODO: case 56: goto oid_1_2_56;
                case 124: return $"{oidPath}";
                //TODO: case 156: goto oid_1_2_156;
                //TODO: case 203: goto oid_1_2_203;
                //TODO: case 208: goto oid_1_2_208;
                //TODO: case 246: goto oid_1_2_246;
                //TODO: case 250: goto oid_1_2_250;
                //TODO: case 276: goto oid_1_2_276;
                case 280: return $"{oidPath}/[Germany: Bundesrepublik Deutschland]";
                //TODO: case 300: goto oid_1_2_300;
                case 344: return $"{oidPath}";
                //TODO: case 372: goto oid_1_2_372;
                //TODO: case 392: goto oid_1_2_392;
                case 398: return $"{oidPath}/KZ";
                //TODO: case 410: goto oid_1_2_410;
                //TODO: case 498: goto oid_1_2_498;
                //TODO: case 528: goto oid_1_2_528;
                case 566: return $"{oidPath}/NG";
                //TODO: case 578: goto oid_1_2_578;
                //TODO: case 616: goto oid_1_2_616;
                //TODO: case 643: goto oid_1_2_643;
                //TODO: case 702: goto oid_1_2_702;
                //TODO: case 752: goto oid_1_2_752;
                //TODO: case 804: goto oid_1_2_804;
                //TODO: case 826: goto oid_1_2_826;
                //TODO: case 840: goto oid_1_2_840;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // identified-organization
        #region 1.3.*

        oid_1_3:

            oidPath += "/Identified-Organization";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: return $"{oidPath}/[Not assigned]";
                case 2: return $"{oidPath}/[Système d'Information et Répertoire des ENtreprises et des Etablissements (SIRENE)]";
                case 3: return $"{oidPath}/[Codification numérique des établissements financiers en Belgique]";
                case 4: return $"{oidPath}/[National Bureau of Standards (NBS) Open System Interconnection NETwork (OSINET)]";
                case 5: return $"{oidPath}/[United States Federal Government Open System interconnection NETwork (GOSNET)]";
                case 6: return $"{oidPath}/[\"DODNET\": Open System Interconnection (OSI) network for the Department of Defense (DoD)]";
                case 7: return $"{oidPath}/[Organisationsnummer]";
                case 8: return $"{oidPath}/[Le Numéro national]";
                case 9: return $"{oidPath}/[Système d'Identification du Répertoire des ETablissements (SIRET) codes]";
                case 10: return $"{oidPath}/[Organizational identifiers for structured names under ISO 9541-2]";
                case 11: return $"{oidPath}/[OSI-based amateur radio organizations, network objects and application services]";
                //TODO: case 12: goto oid_1_3_12;
                case 13: return $"{oidPath}/[Code assigned by the German Automotive Association to companies operating file transfer stations using Odette File Transfer Protocol (OFTP) (formerly, \"VSA\" File Transfer Protocol (FTP) code)]";
                //TODO: case 14: goto oid_1_3_14;
                //TODO: case 15: goto oid_1_3_15;
                //TODO: case 16: goto oid_1_3_16;
                case 17: return $"{oidPath}/[COMMON LANGUAGE]";
                //TODO: case 18: goto oid_1_3_18;
                case 19: return $"{oidPath}/[Air Transport Industry Services Communications Network]";
                case 20: return $"{oidPath}/[European Laboratory for Particle Physics \"CERN\"]";
                case 21: return $"{oidPath}/[Society for Worldwide Interbank Financial Telecommunication (SWIFT)]";
                //TODO: case 22: goto oid_1_3_22;
                case 23: return $"{oidPath}/[Nordic University and Research Network: NORDUnet]";
                case 24: return $"{oidPath}/[Digital Equipment Corporation (DEC)]";
                case 25: return $"{oidPath}/[OSI Asia-Oceania Workshop (AOW)]";
                //TODO: case 26: goto oid_1_3_26;
                //TODO: case 27: goto oid_1_3_27;
                case 28: return $"{oidPath}/[Organisation for Data Exchange through TeleTransmission in Europe (ODETTE)]";
                case 29: return $"{oidPath}/[The all-union classifier of enterprises and organizations]";
                case 30: return $"{oidPath}/[AT&T/OSI network]";
                case 31: return $"{oidPath}/[AT&T/Electronic Data Interchange (EDI) partner identification code]";
                case 32: return $"{oidPath}/[Telecom Australia]";
                case 33: return $"{oidPath}/[S G Warburg Group Management Ltd OSI Internetwork]";
                case 34: return $"{oidPath}/[Reuter open address standard]";
                case 35: return $"{oidPath}/[British Petroleum Ltd]";
                //TODO: case 36: goto oid_1_3_36;
                case 37: return $"{oidPath}/[LY-tunnus]";
                case 38: return $"{oidPath}/[The Australian Government Open Systems Interconnection Profile (GOSIP) network]";
                case 39: return $"{oidPath}/[\"OZDOD DEFNET\": Australian Department Of Defence (DOD) OSI network]";
                case 40: return $"{oidPath}/[Unilever Group Companies]";
                case 41: return $"{oidPath}/[Citicorp Global Information Network (CGIN)]";
                case 42: return $"{oidPath}/[Deutsche BundesPost (DBP) Telekom]";
                case 43: return $"{oidPath}/[HydroNETT]";
                case 44: return $"{oidPath}/[Thai Industrial Standards Institute (TISI)]";
                case 45: return $"{oidPath}/[\"ICI\" Company]";
                case 46: return $"{oidPath}/[Philips FUNction LOCations (FUNLOC)]";
                case 47: return $"{oidPath}/[Bull \"ODI\"/Distributed System Architecture (DSA)/Unix network]";
                case 48: return $"{oidPath}/[\"OSINZ\"]";
                case 49: return $"{oidPath}/[Auckland Area Health]";
                case 50: return $"{oidPath}/[Firmenich]";
                case 51: return $"{oidPath}/[\"AGFA-DIS\"]";
                case 52: return $"{oidPath}/[Society of Motion Picture and Television Engineers (SMPTE)]";
                case 53: return $"{oidPath}/[Migros_Network M_NETOPZ]";
                case 54: return $"{oidPath}/[Pfizer]";
                case 55: return $"{oidPath}/[Energy Net]";
                case 56: return $"{oidPath}/[Nokia]";
                case 57: return $"{oidPath}/[Saint Gobain]";
                case 58: return $"{oidPath}/[Siemens Corporate Network (SCN)]";
                case 59: return $"{oidPath}/[\"DANZNET\"]";
                case 60: return $"{oidPath}/[Dun & Bradstreet Data Universal Numbering System (D-U-N-S)]";
                case 61: return $"{oidPath}/[\"SOFFEX\" OSI]";
                case 62: return $"{oidPath}/[Koninklijke \"PTT\" Nederland (KPN) \"OVN\" (operator fixed networks)]";
                case 63: return $"{oidPath}/[AscomOSINet]";
                case 64: return $"{oidPath}/[Uniform Transport Code (UTC)]";
                case 65: return $"{oidPath}/[Solvay Group]";
                case 66: return $"{oidPath}/[Roche Corporate Network]";
                case 67: return $"{oidPath}/[Zellweger]";
                case 68: return $"{oidPath}";
                case 69: return $"{oidPath}/[SITA (Société Internationale de Télécommunications Aéronautiques)]";
                case 70: return $"{oidPath}/[DaimlerChrysler Corporate Network (DCCN)]";
                case 71: return $"{oidPath}/[LEGOnet]";
                case 72: return $"{oidPath}/[Navistar]";
                case 73: return $"{oidPath}/[Formatted Asynchronous Transfer Mode (ATM) address]";
                case 74: return $"{oidPath}/[\"ARINC\"]";
                case 75: return $"{oidPath}/[Alcanet (Alcatel-Alsthom vorporate network)]";
                //TODO: case 76: goto oid_1_3_76;
                case 77: return $"{oidPath}/[Sistema Italiano di Indirizzamento di Reti OSI Gestito da \"UNINFO\"]";
                case 78: return $"{oidPath}/[Mitel terminal or switching equipment]";
                case 79: return $"{oidPath}/[Asynchronous Transfer Mode (ATM) Forum]";
                case 80: return $"{oidPath}/[UK national health service scheme (Electronic Data Interchange Registration Authorities (EDIRA) compliant)]";
                case 81: return $"{oidPath}/[International Network Service Access Point (NSAP)]";
                case 82: return $"{oidPath}/[Norwegian Telecommunications Authority (NTA)]";
                case 83: return $"{oidPath}/[Advanced Telecommunications Modules Limited Corporate Network]";
                case 84: return $"{oidPath}/[Athens Chamber of Commerce & Industry Scheme]";
                case 85: return $"{oidPath}/[Swisskey certificate authority coding system]";
                case 86: return $"{oidPath}/[United States Council for International Business (USCIB)]";
                case 87: return $"{oidPath}/[National Federation of Chambers of Commerce & Industry of Belgium Scheme]";
                case 88: return $"{oidPath}/[Global Location Number (GLN) (previously, European Article Number (EAN) location code)]";
                case 89: return $"{oidPath}/[Association of British Chambers of Commerce Ltd. scheme]";
                case 90: return $"{oidPath}/[Internet Protocol (IP) addressing]";
                case 91: return $"{oidPath}/[Cisco Systems / Open Systems Interconnection (OSI) network]";
                case 92: return $"{oidPath}/[Not to be assigned]";
                case 93: return $"{oidPath}/[Revenue Canada Business Number (BN) registration]";
                case 94: return $"{oidPath}/[Deutscher Industrie- und HandelsTag (DIHT) scheme]";
                case 95: return $"{oidPath}/[Hewlett-Packard (HP) Company internal Asynchronous Transfer Mode (ATM) network]";
                case 96: return $"{oidPath}/[Danish Chamber of Commerce & Industry]";
                case 97: return $"{oidPath}/[\"FTI\" - Ediforum Italia]";
                case 98: return $"{oidPath}/[Chamber of Commerce Tel Aviv-Jaffa]";
                case 99: return $"{oidPath}/[Siemens Supervisory Systems Network]";
                case 100: return $"{oidPath}/[PNG_ICD scheme]";
                //TODO: case 101: goto oid_1_3_101;
                case 102: return $"{oidPath}/[\"HEAG\" holding group]";
                case 103: return $"{oidPath}/[Reserved for later allocation]";
                case 104: return $"{oidPath}/[British Telecommunications plc (BT)]";
                case 105: return $"{oidPath}/[Portuguese Chamber of Commerce and Industry]";
                case 106: return $"{oidPath}/[Vereniging van Kamers van Koophandel en Fabrieken in Nederland]";
                case 107: return $"{oidPath}/[Association of Swedish Chambers of Commerce and Industry]";
                case 108: return $"{oidPath}/[Australian Chambers of Commerce and Industry]";
                case 109: return $"{oidPath}/[BellSouth Asynchronous Transfer Mode (ATM) End System Address (AESA)]";
                case 110: return $"{oidPath}/[Bell Atlantic]";
                //TODO: case 111: goto oid_1_3_111;
                //TODO: case 112: goto oid_1_3_112;
                case 113: return $"{oidPath}/[OriginNet]";
                //TODO: case 114: goto oid_1_3_114;
                case 115: return $"{oidPath}/[Pacific Bell data communications network]";
                case 116: return $"{oidPath}/[Postal Security Services (PSS)]";
                case 117: return $"{oidPath}/[Stentor]";
                case 118: return $"{oidPath}/[ATM-Network ZN\"96]";
                case 119: return $"{oidPath}/[\"MCI\" OSI network]";
                case 120: return $"{oidPath}/[Advantis]";
                case 121: return $"{oidPath}/[Affable Software data interchange codes]";
                case 122: return $"{oidPath}/[BB-DATA GmbH]";
                case 123: return $"{oidPath}/[Badische Anilin und Soda Fabrik (BASF) company Asynchronous Transfer Mode (ATM) network]";
                //TODO: case 124: goto oid_1_3_124;
                case 125: return $"{oidPath}/[Henkel Corporate Network (H-Net)]";
                case 126: return $"{oidPath}/[\"GTE\" OSI network]";
                case 127: return $"{oidPath}/[Allianz Technology]";
                case 128: return $"{oidPath}/[\"BCNR\" (Swiss clearing bank number)]";
                case 129: return $"{oidPath}/[Telekurs Business Partner Identification (BPI)]";
                //TODO: case 130: goto oid_1_3_130;
                case 131: return $"{oidPath}/[Code for the Identification of National Organizations]";
                //TODO: case 132: goto oid_1_3_132;
                //TODO: case 133: goto oid_1_3_133;
                case 134: return $"{oidPath}/[Infonet Services Corporation]";
                case 135: return $"{oidPath}/[Societa Interbancaria per l'Automazione (SIA) S.p.A.]";
                case 136: return $"{oidPath}/[Cable & Wireless global Asynchronous Transfer Mode (ATM) end-system address plan]";
                case 137: return $"{oidPath}/[Global One Asynchronous Transfer Mode (ATM) End System Address (AESA) scheme]";
                case 138: return $"{oidPath}/[France Telecom Asynchronous Transfer Mode (ATM) End System Address (AESA) plan]";
                case 139: return $"{oidPath}/[Savvis Communications Asynchronous Transfer Mode (ATM) End System Address (AESA)]";
                case 140: return $"{oidPath}/[Toshiba Organizations, Partners, And Suppliers (TOPAS) code]";
                case 141: return $"{oidPath}/[North Atlantic Treaty Organization (NATO) Commercial And Government Entity (NCAGE) system, a.k.a. NCAGE NATO Code of manufacturers]";
                case 142: return $"{oidPath}/[\"SECETI S.p.A.\"]";
                case 143: return $"{oidPath}/[EINESTEINet AG]";
                case 144: return $"{oidPath}/[Department of Defense Activity Address Code (DoDAAC)]";
                case 145: return $"{oidPath}/[Direction Générale de la Comptabilité Publique (DGCP) administrative accounting identification scheme]";
                case 146: return $"{oidPath}/[Direction Générale des Impots (DGI)]";
                case 147: return $"{oidPath}/[Standard company code (partner identification code) registered with \"JIPDEC\"]";
                case 148: return $"{oidPath}/[International Telecommunication Union (ITU) Data Network Identification Codes (DNIC)]";
                case 149: return $"{oidPath}/[Global Business Identifier (GBI)]";
                case 150: return $"{oidPath}/[Madge Networks Ltd Asynchronous Transfer Mode (ATM) addressing scheme]";
                case 151: return $"{oidPath}/[Australian Business Number (ABN) scheme]";
                case 152: return $"{oidPath}/[Electronic Data Interchange Registration Authorities (EDIRA) scheme identifier code]";
                case 153: return $"{oidPath}/[Concert Global network services Asynchronous Transfer Mode (ATM) End System Address (AESA)]";
                //TODO: case 154: goto oid_1_3_154;
                case 155: return $"{oidPath}/[Global Crossing Asynchronous Transfer Mode (ATM) End System Address (AESA)]";
                case 156: return $"{oidPath}/[\"AUNA\"]";
                case 157: return $"{oidPath}/[Informatie en communicatie Technologie Organisatie (ITO) Drager Net]";
                //TODO: case 158: goto oid_1_3_158;
                //TODO: case 159: goto oid_1_3_159;
                case 160: return $"{oidPath}/[GS1 Global Trade Item Number (GTIN)]";
                case 161: return $"{oidPath}/[Electronic Commerce Code Management Association (ECCMA) open technical dictionary]";
                //TODO: case 162: goto oid_1_3_162;
                case 163: return $"{oidPath}/[United States Environmental Protection Agency (US-EPA) facilities]";
                case 164: return $"{oidPath}/[\"TELUS\" Corporation Asynchronous Transfer Mode (ATM) End System Address (AESA) addressing scheme for ATM Private Network-to-Network Interface (PNNI) implementation]";
                case 165: return $"{oidPath}/[\"FIEIE\"]";
                case 166: return $"{oidPath}/[Swissguide]";
                case 167: return $"{oidPath}/[Priority Telecom Asynchronous Transfer Mode (ATM) End System Address (AESA) plan]";
                case 168: return $"{oidPath}/[Vodafone Ireland]";
                case 169: return $"{oidPath}/[Swiss Federal Business Identification Number]";
                case 170: return $"{oidPath}/[Teikoku Company Code]";
                //TODO: case 171: goto oid_1_3_171;
                case 172: return $"{oidPath}/[Project Group \"Lists of properties\" (PROLIST®)]";
                case 173: return $"{oidPath}/[eCl@ss]";
                case 174: return $"{oidPath}/[StepNexus]";
                case 175: return $"{oidPath}/[Siemens AG]";
                case 176: return $"{oidPath}/[Paradine GmbH]";
                //TODO: case 177: goto oid_1_3_177;
                case 178: return $"{oidPath}/[Route1's MobiNET]";
                //TODO: case 179: goto oid_1_3_179;
                case 180: return $"{oidPath}/[Lithuanian military Public Key Infrastructure (PKI)]";
                case 183: return $"{oidPath}/[Unique IDentification Business (UIDB) number]";
                case 184: return $"{oidPath}/[\"DIGSTORG\"]";
                case 185: return $"{oidPath}/[Perceval Object Code (POC)]";
                case 186: return $"{oidPath}/[TrustPoint]";
                case 187: return $"{oidPath}/[Amazon Unique Identification Scheme (AUIS)]";
                case 188: return $"{oidPath}/[Corporate number of the social security and tax number system of Japan]";
                case 189: return $"{oidPath}/[European Business IDentifier (EBID)]";
                case 190: return $"{oidPath}/[Organisatie Identificatie Nummer (OIN)]";
                case 191: return $"{oidPath}/[Company code (Estonia)]";
                case 192: return $"{oidPath}/[Organisasjonsnummer, Norway]";
                case 193: return $"{oidPath}/[UBL.BE party identifier]";
                case 194: return $"{oidPath}/[\"KOIOS\" Open Technical Dictionary (OTD)]";
                case 195: return $"{oidPath}/[Singapore nationwide e-invoice framework]";
                case 196: return $"{oidPath}/[Íslensk kennitala]";
                case 197: return $"{oidPath}/[Reserved]";
                case 198: return $"{oidPath}/[ERSTORG]";
                case 199: return $"{oidPath}/[Legal Entity Identifier (LEI)]";
                case 200: return $"{oidPath}/[Legal entity code (Lithuania)]";
                case 201: return $"{oidPath}/[Codice Univoco Unità Organizzativa iPA]";
                case 202: return $"{oidPath}/[Indirizzo di Posta Elettronica Certificata]";
                case 203: return $"{oidPath}/[eDelivery network participant identifier]";
                case 204: return $"{oidPath}/[Leitweg-ID]";
                case 205: return $"{oidPath}/[CODDEST]";
                case 206: return $"{oidPath}/[Registre du Commerce et de l'Industrie (RCI), Monaco]";
                case 207: return $"{oidPath}/[Pilog Ontology Codification Identifier (POCI)]";
                case 208: return $"{oidPath}/[Numéro d'entreprise / Ondernemingsnummer / Unternehmensnummer, Belgium]";
                case 209: return $"{oidPath}";
                case 210: return $"{oidPath}/[Codice fiscale]";
                case 211: return $"{oidPath}/[Partita iva]";
                case 212: return $"{oidPath}/[Finnish Organization Identifier]";
                case 213: return $"{oidPath}/[Finnish organization value add tax identifier]";
                case 214: return $"{oidPath}/[Tradeplace TradePI (Product Information) standard]";
                case 215: return $"{oidPath}/[Net service identifier]";
                case 216: return $"{oidPath}/[OVTcode]";
                case 9900:
                case 9991:
                case 9992:
                case 9993:
                case 9994:
                case 9995:
                case 9996:
                case 9997:
                case 9998:
                case 9999: return $"{oidPath}/[Reserved]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        #endregion

        // joint-iso-itu-t, joint-iso-ccitt
        #region 2.*

        oid_2:

            oidPath += "/Joint-ISO-ITU-T";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: return $"{oidPath}/[Presentation layer service and protocol]";
                case 1: oidPath = string.Empty; goto oid_2_1;
                case 2: goto oid_2_2;
                case 3: goto oid_2_3;
                case 4: goto oid_2_4;
                case 5: goto oid_2_5;
                case 6: goto oid_2_6;
                case 7: goto oid_2_7;
                case 8: goto oid_2_8;
                case 9: goto oid_2_9;
                case 10: goto oid_2_10;
                case 11: goto oid_2_11;
                case 12: goto oid_2_12;
                case 13: goto oid_2_13;
                case 14: goto oid_2_14;
                case 15: goto oid_2_15;
                case 16: oidPath = string.Empty; goto oid_2_16;
                case 17: goto oid_2_17;
                case 18: goto oid_2_18;
                case 19: goto oid_2_19;
                case 20: goto oid_2_20;
                case 21: goto oid_2_21;
                case 22: goto oid_2_22;
                case 23: oidPath = string.Empty; goto oid_2_23;
                case 24: goto oid_2_24;
                case 25: oidPath = string.Empty; goto oid_2_25;
                case 26: goto oid_2_26;
                case 27: oidPath = string.Empty; goto oid_2_27;
                case 28: goto oid_2_28;
                case 40: goto oid_2_40;
                case 41: oidPath = string.Empty; goto oid_2_41;
                case 42: oidPath = string.Empty; goto oid_2_42;
                case 48: oidPath = string.Empty; goto oid_2_48;
                case 49: oidPath = string.Empty; goto oid_2_49;
                case 50: oidPath = string.Empty; goto oid_2_50;
                case 51: oidPath = string.Empty; goto oid_2_51;
                case 52: goto oid_2_52;
                case 999: return $"{oidPath}/Example";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // asn1
        #region 2.1.*

        oid_2_1:

            oidPath += "/ASN.1";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                //TODO: case 0: goto oid_2_1_0;
                case 1: return $"{oidPath}/[Basic Encoding Rules (BER)]";
                //TODO: case 2: goto oid_2_1_2;
                //TODO: case 3: goto oid_2_1_3;
                //TODO: case 4: goto oid_2_1_4;
                //TODO: case 5: goto oid_2_1_5;
                //TODO: case 6: goto oid_2_1_6;
                case 7: return $"{oidPath}/[Javascript object notation Encoding Rules (JER)]";
                //TODO: case 8: goto oid_2_1_8;
                case 123: return $"{oidPath}/[Examples]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // association-control
        #region 2.2.*

        oid_2_2:

            oidPath += "/[Association Control Service Element (ACSE)]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                //TODO: case 0: goto oid_2_2_0;
                //TODO: case 1: goto oid_2_2_1;
                //TODO: case 2: goto oid_2_2_2;
                //TODO: case 3: goto oid_2_2_3;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // reliable-transfer
        #region 2.3.*

        oid_2_3:

            oidPath += "/[Reliable transfer service element]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Reliable-Transfer-APDU]",
                1 => $"{oidPath}/[Reliable Transfer Service Element (RTSE) identifier]",
                2 => $"{oidPath}/[Abstract syntaxes]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // remote-operations
        #region 2.4.*

        oid_2_4:

            oidPath += "/[Remote Operations Service Element (ROSE)]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: return $"{oidPath}/[Remote-Operation-Notation]";
                case 1: return $"{oidPath}/[Remote-Operations-APDUs]";
                case 2: return $"{oidPath}/[Remote-Operations-Notation-extension]";
                case 3: return $"{oidPath}/[Application Service Element (ASE) identifier]";
                case 4: return $"{oidPath}/[Association Control Service Element (ACSE)]";
                //TODO: case 5: goto oid_2_4_5;
                //TODO: case 6: goto oid_2_4_6;
                //TODO: case 7: goto oid_2_4_7;
                //TODO: case 8: goto oid_2_4_8;
                //TODO: case 9: goto oid_2_4_9;
                //TODO: case 10: goto oid_2_4_10;
                //TODO: case 11: goto oid_2_4_11;
                //TODO: case 12: goto oid_2_4_12;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // ds, directory
        #region 2.5.*

        oid_2_5:

            oidPath += "/[Directory services]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                //TODO: case 1: goto oid_2_5_1;
                case 2: return $"{oidPath}/[Directory service elements]";
                //TODO: case 3: goto oid_2_5_3;
                //TODO: case 4: goto oid_2_5_4;
                //TODO: case 5: goto oid_2_5_5;
                //TODO: case 6: goto oid_2_5_6;
                case 7: return $"{oidPath}/[X.500 attribute sets]";
                //TODO: case 8: goto oid_2_5_8;
                //TODO: case 9: goto oid_2_5_9;
                //TODO: case 12: goto oid_2_5_12;
                //TODO: case 13: goto oid_2_5_13;
                //TODO: case 14: goto oid_2_5_14;
                //TODO: case 15: goto oid_2_5_15;
                case 16: return $"{oidPath}/[Groups]";
                //TODO: case 17: goto oid_2_5_17;
                //TODO: case 18: goto oid_2_5_18;
                //TODO: case 19: goto oid_2_5_19;
                //TODO: case 20: goto oid_2_5_20;
                //TODO: case 21: goto oid_2_5_21;
                //TODO: case 23: goto oid_2_5_23;
                //TODO: case 24: goto oid_2_5_24;
                //TODO: case 25: goto oid_2_5_25;
                //TODO: case 26: goto oid_2_5_26;
                //TODO: case 27: goto oid_2_5_27;
                //TODO: case 28: goto oid_2_5_28;
                //TODO: case 29: goto oid_2_5_29;
                //TODO: case 30: goto oid_2_5_30;
                //TODO: case 31: goto oid_2_5_31;
                //TODO: case 32: goto oid_2_5_32;
                //TODO: case 33: goto oid_2_5_33;
                //TODO: case 34: goto oid_2_5_34;
                //TODO: case 35: goto oid_2_5_35;
                case 36: return $"{oidPath}/[Matching restrictions]";
                //TODO: case 37: goto oid_2_5_37;
                case 38: return $"{oidPath}/[Key purposes]";
                //TODO: case 39: goto oid_2_5_39;
                //TODO: case 40: goto oid_2_5_40;
                //TODO: case 41: goto oid_2_5_41;
                //TODO: case 42: goto oid_2_5_42;
                //TODO: case 43: goto oid_2_5_43;
                //TODO: case 44: goto oid_2_5_44;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // mhs, mhs-motis
        #region 2.6.*

        oid_2_6:

            oidPath += "/[Message Handling System (MHS)]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                //TODO: case 0: goto oid_2_6_0;
                //TODO: case 1: goto oid_2_6_1;
                //TODO: case 2: goto oid_2_6_2;
                //TODO: case 3: goto oid_2_6_3;
                //TODO: case 4: goto oid_2_6_4;
                //TODO: case 5: goto oid_2_6_5;
                //TODO: case 6: goto oid_2_6_6;
                //TODO: case 7: goto oid_2_6_7;
                //TODO: case 8: goto oid_2_6_8;
                //TODO: case 9: goto oid_2_6_9;
                //TODO: case 10: goto oid_2_6_10;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // ccr
        #region 2.7.*

        oid_2_7:

            oidPath += "/[Commitment, Concurrency and Recovery (CCR) service and protocol]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                //TODO: case 1: goto oid_2_7_1;
                //TODO: case 2: goto oid_2_7_2;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // oda
        #region 2.8.*

        oid_2_8:

            oidPath += "/[Open Document Architecture (ODA)]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                //TODO: case 0: goto oid_2_8_0;
                //TODO: case 1: goto oid_2_8_1;
                //TODO: case 2: goto oid_2_8_2;
                //TODO: case 3: goto oid_2_8_3;
                case 4: return $"{oidPath}/[Identification of a document application profile]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // ms, osi-management
        #region 2.9.*

        oid_2_9:

            oidPath += "/[OSI network management]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                //TODO: case 0: goto oid_2_9_0;
                //TODO: case 1: goto oid_2_9_1;
                //TODO: case 2: goto oid_2_9_2;
                //TODO: case 3: goto oid_2_9_3;
                //TODO: case 4: goto oid_2_9_4;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // transaction-processing
        #region 2.10.*

        oid_2_10:

            oidPath += "/[Transaction processing]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                //TODO: case 0: goto oid_2_10_0;
                //TODO: case 1: goto oid_2_10_1;
                //TODO: case 2: goto oid_2_10_2;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // dor, distinguished-object-reference
        #region 2.11.*

        oid_2_11:

            oidPath += "/[Information technology -- Text and office systems -- Distributed-office-applications model -- Part 2: Distinguished-object-reference and associated procedures]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: return $"{oidPath}/[DOR-definition]";
                case 1: return $"{oidPath}/[Abstract syntax of \"distinguished-object-reference\"]";
                //TODO: case 2: goto oid_2_11_2;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // reference-data-transfer, rdt
        #region 2.12.*

        oid_2_12:

            oidPath += "/[Referenced Data Transfer (RDT)]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                //TODO: case 0: goto oid_2_12_0;
                //TODO: case 1: goto oid_2_12_1;
                //TODO: case 2: goto oid_2_12_2;
                //TODO: case 3: goto oid_2_12_3;
                //TODO: case 4: goto oid_2_12_4;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // network-layer, network-layer-management
        #region 2.13.*

        oid_2_13:

            oidPath += "/[Network layer management]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                //TODO: case 0: goto oid_2_13_0;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // transport-layer, transport-layer-management
        #region 2.14.*

        oid_2_14:

            oidPath += "/[Transport layer management]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                //TODO: case 0: goto oid_2_14_0;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // datalink-layer, datalink-layer-management, datalink-layer-management-information
        #region 2.15.*

        oid_2_15:

            oidPath += "/[OSI data link layer management]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                //TODO: case 0: goto oid_2_15_0;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // country
        #region 2.16.*

        oid_2_16:

            oidPath += "/Country";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 4: return $"{oidPath}/AF";
                case 8: return $"{oidPath}/AL";
                case 12: goto oid_2_16_12;
                case 20: goto oid_2_16_20;
                case 24: return $"{oidPath}/AO";
                case 28: return $"{oidPath}/AG";
                case 31: return $"{oidPath}/AZ";
                case 32: goto oid_2_16_32;
                case 36: return $"{oidPath}/AU";
                case 40: return $"{oidPath}/AT";
                case 44: return $"{oidPath}/BS";
                case 48: return $"{oidPath}/BH";
                case 50: goto oid_2_16_50;
                case 51: return $"{oidPath}/AM";
                case 52: return $"{oidPath}/BB";
                case 56: return $"{oidPath}/BE";
                case 60: goto oid_2_16_60;
                case 64: return $"{oidPath}/BT";
                case 68: return $"{oidPath}/BO";
                case 70: return $"{oidPath}/BA";
                case 72: return $"{oidPath}/BW";
                case 76: goto oid_2_16_76;
                case 84: return $"{oidPath}/BZ";
                case 90: return $"{oidPath}/SB";
                case 96: return $"{oidPath}/BN";
                case 100: return $"{oidPath}/BG";
                case 104: return $"{oidPath}/MM";
                case 108: return $"{oidPath}/BI";
                case 112: return $"{oidPath}/BY";
                case 116: return $"{oidPath}/KH";
                case 120: return $"{oidPath}/CM";
                case 124: goto oid_2_16_124;
                case 132: return $"{oidPath}/CV";
                case 140: return $"{oidPath}/CF";
                case 144: goto oid_2_16_144;
                case 148: return $"{oidPath}/TD";
                case 152: return $"{oidPath}/CL";
                case 156: goto oid_2_16_156;
                case 158: goto oid_2_16_158;
                case 170: return $"{oidPath}/CO";
                case 174: return $"{oidPath}/KM";
                case 178: return $"{oidPath}/CG";
                case 180: return $"{oidPath}/CD";
                case 188: return $"{oidPath}/CR";
                case 191: goto oid_2_16_191;
                case 192: return $"{oidPath}/CU";
                case 196: return $"{oidPath}/CY";
                case 203: return $"{oidPath}/CZ";
                case 204: return $"{oidPath}/BJ";
                case 208: return $"{oidPath}/DK";
                case 212: return $"{oidPath}/DM";
                case 214: return $"{oidPath}/DO";
                case 218: goto oid_2_16_218;
                case 222: return $"{oidPath}/SV";
                case 226: return $"{oidPath}/GQ";
                case 231: return $"{oidPath}/ET";
                case 232: return $"{oidPath}/ER";
                case 233: return $"{oidPath}/EE";
                case 242: return $"{oidPath}/FJ";
                case 246: return $"{oidPath}/FI";
                case 250: return $"{oidPath}/FR";
                case 262: return $"{oidPath}/DJ";
                case 266: return $"{oidPath}/GA";
                case 268: return $"{oidPath}/GE";
                case 270: return $"{oidPath}/GM";
                case 275: return $"{oidPath}/PS";
                case 276: goto oid_2_16_276;
                case 288: return $"{oidPath}/GH";
                case 296: return $"{oidPath}/KI";
                case 300: return $"{oidPath}/GR";
                case 308: return $"{oidPath}/GD";
                case 320: return $"{oidPath}/GT";
                case 324: return $"{oidPath}/GN";
                case 328: return $"{oidPath}/GY";
                case 332: return $"{oidPath}/HT";
                case 336: return $"{oidPath}/VA";
                case 340: goto oid_2_16_340;
                case 344: goto oid_2_16_344;
                case 348: return $"{oidPath}/HU";
                case 352: goto oid_2_16_352;
                case 356: goto oid_2_16_356;
                case 360: return $"{oidPath}/ID";
                case 364: goto oid_2_16_364;
                case 368: return $"{oidPath}/IQ";
                case 372: return $"{oidPath}/IE";
                case 376: return $"{oidPath}/IL";
                case 380: goto oid_2_16_380;
                case 384: return $"{oidPath}/CI";
                case 388: goto oid_2_16_388;
                case 392: return $"{oidPath}/JP";
                case 398: return $"{oidPath}/KZ";
                case 400: return $"{oidPath}/JO";
                case 404: return $"{oidPath}/KE";
                case 408: return $"{oidPath}/KP";
                case 410: return $"{oidPath}/KR";
                case 414: return $"{oidPath}/KW";
                case 417: return $"{oidPath}/KG";
                case 418: return $"{oidPath}/LA";
                case 422: return $"{oidPath}/LB";
                case 426: return $"{oidPath}/LS";
                case 428: return $"{oidPath}/LV";
                case 430: return $"{oidPath}/LR";
                case 434: return $"{oidPath}/LY";
                case 438: return $"{oidPath}/LI";
                case 440: return $"{oidPath}/LT";
                case 442: return $"{oidPath}/LU";
                case 450: return $"{oidPath}/MG";
                case 454: return $"{oidPath}/MW";
                case 458: return $"{oidPath}/MY";
                case 462: return $"{oidPath}/MV";
                case 466: return $"{oidPath}/ML";
                case 470: goto oid_2_16_470;
                case 478: return $"{oidPath}/MR";
                case 480: return $"{oidPath}/MU";
                case 484: return $"{oidPath}/MX";
                case 492: return $"{oidPath}/MC";
                case 496: return $"{oidPath}/MN";
                case 498: return $"{oidPath}/MD";
                case 499: return $"{oidPath}/ME";
                case 504: return $"{oidPath}/MA";
                case 508: return $"{oidPath}/MZ";
                case 512: return $"{oidPath}/OM";
                case 516: return $"{oidPath}/NA";
                case 520: return $"{oidPath}/NR";
                case 524: return $"{oidPath}/NP";
                case 528: goto oid_2_16_528;
                case 530: return $"{oidPath}/AN";
                case 548: return $"{oidPath}/VU";
                case 554: goto oid_2_16_554;
                case 558: return $"{oidPath}/NI";
                case 562: return $"{oidPath}/NE";
                case 566: return $"{oidPath}/NG";
                case 578: goto oid_2_16_578;
                case 583: return $"{oidPath}/FM";
                case 584: return $"{oidPath}/MH";
                case 585: return $"{oidPath}/PW";
                case 586: return $"{oidPath}/PK";
                case 591: goto oid_2_16_591;
                case 598: return $"{oidPath}/PG";
                case 600: return $"{oidPath}/PY";
                case 604: return $"{oidPath}/PE";
                case 608: return $"{oidPath}/PH";
                case 616: return $"{oidPath}/PL";
                case 620: goto oid_2_16_620;
                case 624: return $"{oidPath}/GW";
                case 626: return $"{oidPath}/TL";
                case 634: return $"{oidPath}/QA";
                case 642: return $"{oidPath}/RO";
                case 643: return $"{oidPath}/RU";
                case 646: return $"{oidPath}/RW";
                case 659: return $"{oidPath}/KN";
                case 662: return $"{oidPath}/LC";
                case 670: return $"{oidPath}/VC";
                case 674: return $"{oidPath}/SM";
                case 678: return $"{oidPath}/ST";
                case 682: goto oid_2_16_682;
                case 686: return $"{oidPath}/SN";
                case 688: return $"{oidPath}/RS";
                case 690: return $"{oidPath}/SC";
                case 694: return $"{oidPath}/SL";
                case 702: return $"{oidPath}/SG";
                case 703: return $"{oidPath}/SK";
                case 704: return $"{oidPath}/VN";
                case 705: return $"{oidPath}/SI";
                case 706: return $"{oidPath}/SO";
                case 710: return $"{oidPath}/ZA";
                case 716: return $"{oidPath}/ZW";
                case 724: goto oid_2_16_724;
                case 728: return $"{oidPath}/SS";
                case 729: return $"{oidPath}/SD";
                case 736: return $"{oidPath}/[Sudan (old code \"retired\")";
                case 740: return $"{oidPath}/SR";
                case 748: return $"{oidPath}/SZ";
                case 752: return $"{oidPath}/SE";
                case 756: goto oid_2_16_756;
                case 760: return $"{oidPath}/SY";
                case 762: return $"{oidPath}/TJ";
                case 764: goto oid_2_16_764;
                case 768: return $"{oidPath}/TG";
                case 776: return $"{oidPath}/TO";
                case 780: return $"{oidPath}/TT";
                case 784: goto oid_2_16_784;
                case 788: goto oid_2_16_788;
                case 792: goto oid_2_16_792;
                case 795: return $"{oidPath}/TM";
                case 798: return $"{oidPath}/TV";
                case 800: return $"{oidPath}/UG";
                case 804: return $"{oidPath}/UA";
                case 807: return $"{oidPath}/MK";
                case 818: return $"{oidPath}/EG";
                case 826: return $"{oidPath}/GB";
                case 834: return $"{oidPath}/TZ";
                case 840: goto oid_2_16_840;
                case 854: return $"{oidPath}/BF";
                case 858: goto oid_2_16_858;
                case 860: return $"{oidPath}/UZ";
                case 862: return $"{oidPath}/VE";
                case 882: return $"{oidPath}/WS";
                case 886: goto oid_2_16_886;
                case 887: return $"{oidPath}/YE";
                case 891: return $"{oidPath}/[Serbia and Montenegro (code not in current use)]";
                case 894: return $"{oidPath}/ZM";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // dz
        #region 2.16.12.*

        oid_2_16_12:

            oidPath += "/DZ";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: return $"{oidPath}/[Public sector institutions]";
                case 2: return $"{oidPath}/[Private sector institutions]";
                case 3: goto oid_2_16_12_3;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // certification-authority
        #region 2.16.12.3.*

        oid_2_16_12_3:

            oidPath += "/[Electronic certification]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_16_12_3_1;
                case 2: return $"{oidPath}/[Governmental authority]";
                case 3: return $"{oidPath}/[Economic authority]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // national-authority
        #region 2.16.12.3.1.*

        oid_2_16_12_3_1:

            oidPath += "/[National authority]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_16_12_3_1_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // 1
        #region 2.16.12.3.1.1.*

        oid_2_16_12_3_1_1:

            oidPath += "/[Authority Public-Key Infrastructure (PKI)]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: return $"{oidPath}/[Certification policy]";
                case 2: return $"{oidPath}/[Certification practice statement]";
                case 3: return $"{oidPath}/[Timestamping policy]";
                case 4: return $"{oidPath}/[Signature policy]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        #endregion

        #endregion

        #endregion

        // ad
        #region 2.16.20.*

        oid_2_16_20:

            oidPath += "/AD";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_16_20_1;
                case 2: goto oid_2_16_20_2;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // organitzacions
        #region 2.16.20.1.*

        oid_2_16_20_1:

            oidPath += "/[Organitzacions públiques, parapúbliques o privades]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_16_20_1_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // gov
        #region 2.16.20.1.1.*

        oid_2_16_20_1_1:

            oidPath += "/[Organitzacions públiques, parapúbliques o privades]/[Govern d'Andorra]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Secretaria General de Govern]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // regulacions
        #region 2.16.20.2.*

        oid_2_16_20_2:

            oidPath += "/[Polítiques i Estàndards de l'Administració General]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_16_20_2_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // serveisconfianca
        #region 2.16.20.2.1.*

        oid_2_16_20_2_1:

            oidPath += "/[Serveis de confiança]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_16_20_2_1_1;
                case 2: goto oid_2_16_20_2_1_2;
                case 3: goto oid_2_16_20_2_1_3;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // criptografia
        #region 2.16.20.2.1.1.*

        oid_2_16_20_2_1_1:

            oidPath += "/[Polítiques de Seguretat Criptogràfica]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Estàndards de Seguretat Criptogràfica]",
                2 => $"{oidPath}/[Guies de Seguretat Criptogràfica]",
                3 => $"{oidPath}/[Procediments operatius de Seguretat Criptogràfica]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // keymanagement
        #region 2.16.20.2.1.2.*

        oid_2_16_20_2_1_2:

            oidPath += "/[Política General de gestió de Claus]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}[Estàndards de gestió de claus]",
                2 => $"{oidPath}[Guies de Gestió de claus]",
                3 => $"{oidPath}[Procediments operatius de gestió de claus]",
                _ => $"{oidPath}{values[index - 1]}",
            };

        #endregion

        // cert
        #region 2.16.20.2.1.3.*

        oid_2_16_20_2_1_3:

            oidPath += "/[Política general de Certificació digital]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_16_20_2_1_3_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // std
        #region 2.16.20.2.1.3.1.*

        oid_2_16_20_2_1_3_1:

            oidPath += "/[Estàndards de certificació digital]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_16_20_2_1_3_1_1;
                case 2: return $"{oidPath}/[Segell empresa]";
                case 4308: return $"{oidPath}/[Persona física al servei organitzacio]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // personafisica
        #region 2.16.20.2.1.3.1.1.*

        oid_2_16_20_2_1_3_1_1:

            oidPath += "/[Certificats tipus de persona física]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Certificats tipus de persona física de nivell 1]",
                3 => $"{oidPath}/[Certificats tipus de persona física de nivell 3]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        #endregion

        #endregion

        #endregion

        // ar
        #region 2.16.32.*

        oid_2_16_32:

            oidPath += "/AR";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                //TODO: case 0: return $"{oidPath}/[XXXXX]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // bd
        #region 2.16.50.*

        oid_2_16_50:

            oidPath += "/BD";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // bm
        #region 2.16.60.*

        oid_2_16_60:

            oidPath += "/BM";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 2: return $"{oidPath}/[XXXXX]";
                //TODO: case 3: return $"{oidPath}/[XXXXX]";
                //TODO: case 7: return $"{oidPath}/[XXXXX]";
                //TODO: case 8: return $"{oidPath}/[XXXXX]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // br
        #region 2.16.76.*

        oid_2_16_76:

            oidPath += "/BR";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 2: return $"{oidPath}/[XXXXX]";
                //TODO: case 3: return $"{oidPath}/[XXXXX]";
                //TODO: case 4: return $"{oidPath}/[XXXXX]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // ca
        #region 2.16.124.*

        oid_2_16_124:

            oidPath += "/CA";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 2: return $"{oidPath}/[XXXXX]";
                //TODO: case 3: return $"{oidPath}/[XXXXX]";
                //TODO: case 4: return $"{oidPath}/[XXXXX]";
                //TODO: case 5: return $"{oidPath}/[XXXXX]";
                //TODO: case 6: return $"{oidPath}/[XXXXX]";
                //TODO: case 7: return $"{oidPath}/[XXXXX]";
                //TODO: case 8: return $"{oidPath}/[XXXXX]";
                //TODO: case 9: return $"{oidPath}/[XXXXX]";
                //TODO: case 10: return $"{oidPath}/[XXXXX]";
                //TODO: case 11: return $"{oidPath}/[XXXXX]";
                //TODO: case 12: return $"{oidPath}/[XXXXX]";
                //TODO: case 13: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // lk
        #region 2.16.144.*

        oid_2_16_144:

            oidPath += "/LK";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                //TODO: case 1: return $"{oidPath}[XXXXX]";
                default: return $"{oidPath}{values[index - 1]}";
            }

        #endregion

        // cn, chn
        #region 2.16.156.*

        oid_2_16_156:

            oidPath += "/CN";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // tw
        #region 2.16.158.*

        oid_2_16_158:

            oidPath += "/TW";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                //TODO: case 168: return $"{oidPath}/[XXXXX]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // hr
        #region 2.16.191.*

        oid_2_16_191:

            oidPath += "/HR";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                //TODO: case 10: return $"{oidPath}/[XXXXX]";
                //TODO: case 20: return $"{oidPath}/[XXXXX]";
                //TODO: case 100: return $"{oidPath}/[XXXXX]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // ec
        #region 2.16.218.*

        oid_2_16_218:

            oidPath += "/EC";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                //TODO: case 0: return $"{oidPath}/[XXXXX]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // de
        #region 2.16.276.*

        oid_2_16_276:

            oidPath += "/DE";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // hn
        #region 2.16.340.*

        oid_2_16_340:

            oidPath += "/HN";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // hk
        #region 2.16.344.*

        oid_2_16_344:

            oidPath += "/HK";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 2: return $"{oidPath}/[XXXXX]";
                //TODO: case 8: return $"{oidPath}/[XXXXX]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // is
        #region 2.16.352.*

        oid_2_16_352:

            oidPath += "/IS";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // in
        #region 2.16.356.*

        oid_2_16_356:

            oidPath += "/IN";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                //TODO: case 100: return $"{oidPath}/[XXXXX]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // ir
        #region 2.16.364.*

        oid_2_16_364:

            oidPath += "/IR";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                //TODO: case 102: return $"{oidPath}/[XXXXX]";
                //TODO: case 103: return $"{oidPath}/[XXXXX]";
                //TODO: case 105: return $"{oidPath}/[XXXXX]";
                //TODO: case 2489: return $"{oidPath}/[XXXXX]";
                //TODO: case 4020: return $"{oidPath}/[XXXXX]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // it
        #region 2.16.380.*

        oid_2_16_380:

            oidPath += "/IT";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 2: return $"{oidPath}/[XXXXX]";
                //TODO: case 7: return $"{oidPath}/[XXXXX]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // jm
        #region 2.16.388.*

        oid_2_16_388:

            oidPath += "/JM";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // mt
        #region 2.16.470.*

        oid_2_16_470:

            oidPath += "/MT";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 2: return $"{oidPath}/[XXXXX]";
                //TODO: case 3: return $"{oidPath}/[XXXXX]";
                //TODO: case 4: return $"{oidPath}/[XXXXX]";
                //TODO: case 5: return $"{oidPath}/[XXXXX]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // nl, nederland
        #region 2.16.528.*

        oid_2_16_528:

            oidPath += "/NL";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // nz
        #region 2.16.554.*

        oid_2_16_554:

            oidPath += "/NZ";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                //TODO: case 101: return $"{oidPath}/[XXXXX]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // no
        #region 2.16.578.*

        oid_2_16_578:

            oidPath += "/NO";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 2: return $"{oidPath}/[XXXXX]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // pa
        #region 2.16.591.*

        oid_2_16_591:

            oidPath += "/PA";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 2: return $"{oidPath}/[XXXXX]";
                //TODO: case 3: return $"{oidPath}/[XXXXX]";
                //TODO: case 4: return $"{oidPath}/[XXXXX]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // pt
        #region 2.16.620.*

        oid_2_16_620:

            oidPath += "/PT";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                //TODO: case 2: return $"{oidPath}/[XXXXX]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // sa
        #region 2.16.682.*

        oid_2_16_682:

            oidPath += "/SA";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 2000: return $"{oidPath}/[XXXXX]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // es
        #region 2.16.724.*

        oid_2_16_724:

            oidPath += "/ES";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 2: return $"{oidPath}/[XXXXX]";
                //TODO: case 3: return $"{oidPath}/[XXXXX]";
                //TODO: case 4: return $"{oidPath}/[XXXXX]";
                //TODO: case 5: return $"{oidPath}/[XXXXX]";
                //TODO: case 6: return $"{oidPath}/[XXXXX]";
                //TODO: case 7: return $"{oidPath}/[XXXXX]";
                //TODO: case 8: return $"{oidPath}/[XXXXX]";
                //TODO: case 9: return $"{oidPath}/[XXXXX]";
                //TODO: case 10: return $"{oidPath}/[XXXXX]";
                //TODO: case 11: return $"{oidPath}/[XXXXX]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // ch
        #region 2.16.756.*

        oid_2_16_756:

            oidPath += "/CH";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 2: return $"{oidPath}/[XXXXX]";
                //TODO: case 3: return $"{oidPath}/[XXXXX]";
                //TODO: case 4: return $"{oidPath}/[XXXXX]";
                //TODO: case 5: return $"{oidPath}/[XXXXX]";
                //TODO: case 6: return $"{oidPath}/[XXXXX]";
                //TODO: case 10: return $"{oidPath}/[XXXXX]";
                //TODO: case 11: return $"{oidPath}/[XXXXX]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // th
        #region 2.16.764.*

        oid_2_16_764:

            oidPath += "/TH";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // ae
        #region 2.16.784.*

        oid_2_16_784:

            oidPath += "/AE";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 2: return $"{oidPath}/[XXXXX]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // tn
        #region 2.16.788.*

        oid_2_16_788:

            oidPath += "/TN";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                //TODO: case 0: return$"{oidPath}/[XXXXX]";
                //TODO: case 1: return$"{oidPath}/[XXXXX]";
                //TODO: case 2: return$"{oidPath}/[XXXXX]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // tr
        #region 2.16.792.*

        oid_2_16_792:

            oidPath += "/TR";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                //TODO: case 0: return $"{oidPath}/[XXXXX]";
                //TODO: case 1: return $"{oidPath}/[XXXXX]";
                //TODO: case 2: return $"{oidPath}/[XXXXX]";
                //TODO: case 3: return $"{oidPath}/[XXXXX]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // us
        #region 2.16.840.*

        oid_2_16_840:

            oidPath += "/US";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_16_840_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // organization
        #region 2.16.840.1.*

        oid_2_16_840_1:

            oidPath += "/[organization]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 101: goto oid_2_16_840_1_101;
                case 113531: return $"{oidPath}/[Control Data Corporation (CDC)]";
                case 113542: return $"{oidPath}/[TRW Inc.]";
                case 113545: return $"{oidPath}/[AT&T Canada]";
                case 113560: return $"{oidPath}/[Columbia University in the City of New York]";
                case 113564: return $"{oidPath}/[Eastman Kodak Company]";
                case 113593: return $"{oidPath}/[University of Minnesota]";
                case 113669: return $"{oidPath}/[Merge Technologies]";
                case 113678: goto oid_2_16_840_1_113678;
                case 113694: goto oid_2_16_840_1_113694;
                case 113719: goto oid_2_16_840_1_113719;
                case 113730: goto oid_2_16_840_1_113730;
                case 113731: return $"{oidPath}/[CertCo]";
                case 113732: return $"{oidPath}/[Television Computer, Inc. / Hyperstamps]";
                case 113733: goto oid_2_16_840_1_113733;
                case 113735: return $"{oidPath}/[BMC Software, Inc]";
                case 113741: goto oid_2_16_840_1_113741;
                case 113762: return $"{oidPath}/[National Institutes of Health (NIH)]";
                case 113793: return $"{oidPath}/[Motorola Inc.]";
                case 113818: return $"{oidPath}/[Cylink Corporation]";
                case 113839: return $"{oidPath}/[IdenTrust]";
                case 113883: goto oid_2_16_840_1_113883;
                case 113894: return $"{oidPath}/[Oracle Corporation]";
                case 113903: return $"{oidPath}/[Citigroup]";
                case 113937: return $"{oidPath}/[Calvin College]";
                case 113938: return $"{oidPath}/[Equifax, Inc.]";
                case 113983: return $"{oidPath}/[Bank of America]";
                case 113992: return $"{oidPath}/[Mississippi State University]";
                //TODO: case 113995: goto oid_2_16_840_1_113995;
                case 113996: return $"{oidPath}/[Equifax Secure, Inc.]";
                case 114003: return $"{oidPath}/AlphaTrust-Corporation";
                //TODO: case 114027: goto oid_2_16_840_1_114027;
                case 114028: return $"{oidPath}/[Entrust]";
                case 114060: return $"{oidPath}/[Siemens Medical Solutions Health Services]";
                case 114102: return $"{oidPath}/[CyberSafe Corporation]";
                case 114160: return $"{oidPath}/[marchFIRST]";
                //TODO: case 114171: goto oid_2_16_840_1_114171;
                case 114187: return $"{oidPath}/[Avaya, Inc.]";
                //TODO: case 114222: goto oid_2_16_840_1_114222;
                case 114223: return $"{oidPath}/[ANSI C12.22 application titles (ApTitle)]";
                case 114273: return $"{oidPath}/[State of Illinois]";
                case 114274: return $"{oidPath}/[American College of Radiology]";
                //TODO: case 114334: goto oid_2_16_840_1_114334;
                case 114360: return $"{oidPath}/[MIB Group, Inc.]";
                //TODO: case 114404: goto oid_2_16_840_1_114404;
                //TODO: case 114412: goto oid_2_16_840_1_114412;
                case 114413: return $"{oidPath}/[Starfield Technologies, LLC, part of Go Daddy (Go Daddy Operating Company, LLC; The Go Daddy Group, Inc.)]";
                case 114425: return $"{oidPath}/[InterComputer Network]";
                case 114426: return $"{oidPath}/[InterComputer Corporation]";
                case 114453: return $"{oidPath}/[Penango, Inc.]";
                case 114459: return $"{oidPath}/[Corepoint Health, LLC]";
                //TODO: case 114505: goto oid_2_16_840_1_114505;
                //TODO: case 114519: goto oid_2_16_840_1_114519;
                //TODO: case 114547: goto oid_2_16_840_1_114547;
                case 114549: return $"{oidPath}/[MaxMD Maxsignatures]";
                //TODO: case 114569: goto oid_2_16_840_1_114569;
                //TODO: case 114572: goto oid_2_16_840_1_114572;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // gov
        #region 2.16.840.1.101.*

        oid_2_16_840_1_101:

            oidPath += "/[US government]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 2: goto oid_2_16_840_1_101_2;
                case 3: goto oid_2_16_840_1_101_3;
                case 10: goto oid_2_16_840_1_101_10;
                case 100: goto oid_2_16_840_1_101_100;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // dod
        #region 2.16.840.1.101.2.*

        oid_2_16_840_1_101_2:

            oidPath += "/[Department of Defense (DoD)]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_16_840_1_101_2_1;
                case 2: goto oid_2_16_840_1_101_2_2;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // infosec
        #region 2.16.840.1.101.2.1.*

        oid_2_16_840_1_101_2_1:

            oidPath += "/[Information security]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_2_16_840_1_101_2_1_0;
                case 1: goto oid_2_16_840_1_101_2_1_1;
                case 2: goto oid_2_16_840_1_101_2_1_2;
                case 3: goto oid_2_16_840_1_101_2_1_3;
                case 4: goto oid_2_16_840_1_101_2_1_4;
                case 5: goto oid_2_16_840_1_101_2_1_5;
                case 6: goto oid_2_16_840_1_101_2_1_6;
                case 7: goto oid_2_16_840_1_101_2_1_7;
                case 8: goto oid_2_16_840_1_101_2_1_8;
                case 10: goto oid_2_16_840_1_101_2_1_10;
                case 11: goto oid_2_16_840_1_101_2_1_11;
                case 12: goto oid_2_16_840_1_101_2_1_12;
                case 16: goto oid_2_16_840_1_101_2_1_16;
                case 20: goto oid_2_16_840_1_101_2_1_20;
                case 21: goto oid_2_16_840_1_101_2_1_21;
                case 22: goto oid_2_16_840_1_101_2_1_22;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // modules
        #region 2.16.840.1.101.2.1.0.*

        oid_2_16_840_1_101_2_1_0:

            oidPath += "/[ASN.1 modules]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                3 => $"{oidPath}/[MSPDirectoryAdditions]",
                20 => $"{oidPath}/[InformationSecurityLabelModule]",
                30 => $"{oidPath}/[TAMP-Protocol-v2]",
                31 => $"{oidPath}/[TAMP-Protocol-v2-88]",
                33 => $"{oidPath}/[TrustAnchorInfoModule]",
                37 => $"{oidPath}/[TrustAnchorInfoModule-88]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // algorithms
        #region 2.16.840.1.101.2.1.1.*

        oid_2_16_840_1_101_2_1_1:

            oidPath += "/[Algorithms]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[id-sdnsSignatureAlgorithm]",
                2 => $"{oidPath}/[id-mosaicSignatureAlgorithm]",
                3 => $"{oidPath}/[id-sdnsConfidentialityAlgorithm]",
                4 => $"{oidPath}/[id-mosaicConfidentialityAlgorithm]",
                5 => $"{oidPath}/[id-sdnsIntegrityAlgorithm]",
                6 => $"{oidPath}/[id-mosaicIntegrityAlgorithm]",
                7 => $"{oidPath}/[id-sdnsTokenProtectionAlgorithm]",
                8 => $"{oidPath}/[id-mosaicTokenProtectionAlgorithm]",
                9 => $"{oidPath}/[id-sdnsKeyManagementAlgorithm]",
                10 => $"{oidPath}/[id-mosaicKeyManagementAlgorithm]",
                11 => $"{oidPath}/[id-sdnsKMandSigAlgorithms]",
                12 => $"{oidPath}/[id-mosaicKMandSigAlgorithms]",
                13 => $"{oidPath}/[id-SuiteASignatureAlgorithm]",
                14 => $"{oidPath}/[id-SuiteAConfidentialityAlgorithm]",
                15 => $"{oidPath}/[id-SuiteAIntegrityAlgorithm]",
                16 => $"{oidPath}/[id-SuiteATokenProtectionAlgorithm]",
                17 => $"{oidPath}/[id-SuiteAKeyManagementAlgorithm]",
                18 => $"{oidPath}/[id-SuiteAKMandSigAlgorithms]",
                19 => $"{oidPath}/[id-mosaicUpdatedSigAlgorithm]",
                20 => $"{oidPath}/[id-mosaicKMandUpdSigAlgorithms]",
                21 => $"{oidPath}/[id-mosaicUpdatedIntegAlgorithm]",
                22 => $"{oidPath}/[id-keyExchangeAlgorithm]",
                23 => $"{oidPath}/[id-fortezzaWrap80Algorithm]",
                24 => $"{oidPath}/[id-kEAKeyEncryptionAlgorithm]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // formats
        #region 2.16.840.1.101.2.1.2.*

        oid_2_16_840_1_101_2_1_2:

            oidPath += "/[Formats]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: return $"{oidPath}/[id-rfc822-message-format]";
                case 2: return $"{oidPath}/[id-empty-content]";
                case 3: return $"{oidPath}/[US DMS ACP 120 content type]";
                case 42: return $"{oidPath}/[id-msp-rev3-content-type]";
                case 48: return $"{oidPath}/[id-msp-content-type]";
                case 49: return $"{oidPath}/[id-msp-rekey-agent-protocol]";
                case 50: return $"{oidPath}/[mspMMP]";
                case 66: return $"{oidPath}/[mspRev3-1ContentType]";
                case 72: return $"{oidPath}/[forwarded-MSP-message-body-part]";
                case 73: return $"{oidPath}/[mspForwardedMessageParameters]";
                case 74: return $"{oidPath}/[forwarded-csp-msg-body-part]";
                case 75: return $"{oidPath}/[csp-forwarded-message-parameters-id]";
                case 76: return $"{oidPath}/[mspMMP2]";
                case 77: goto oid_2_16_840_1_101_2_1_2_77;
                case 78: goto oid_2_16_840_1_101_2_1_2_78;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // id-tamp
        #region 2.16.840.1.101.2.1.2.77.*

        oid_2_16_840_1_101_2_1_2_77:

            oidPath += "/[id-tamp]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[id-ct-TAMP-statusQuery]",
                2 => $"{oidPath}/[id-ct-TAMP-statusResponse]",
                3 => $"{oidPath}/[id-ct-TAMP-update]",
                4 => $"{oidPath}/[id-ct-TAMP-updateConfirm]",
                5 => $"{oidPath}/[id-ct-TAMP-apexUpdate]",
                6 => $"{oidPath}/[id-ct-TAMP-apexUpdateConfirm]",
                7 => $"{oidPath}/[id-ct-TAMP-communityUpdate]",
                8 => $"{oidPath}/[id-ct-TAMP-communityUpdateConfirm]",
                9 => $"{oidPath}/[id-ct-TAMP-error]",
                10 => $"{oidPath}/[id-ct-TAMP-seqNumAdjust]",
                11 => $"{oidPath}/[id-ct-TAMP-seqNumAdjustConfirm]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // key-package-content-types
        #region 2.16.840.1.101.2.1.2.78.*

        oid_2_16_840_1_101_2_1_2_78:

            oidPath += "/[Key package content types]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                2 => $"{oidPath}/[id-ct-KP-encryptedKeyPkg]",
                3 => $"{oidPath}/[id-ct-KP-keyPackageReceipt]",
                5 => $"{oidPath}/[id-ct-KP-aKeyPackage]",
                6 => $"{oidPath}/[id-ct-KP-keyPackageError]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // policy
        #region 2.16.840.1.101.2.1.3.*

        oid_2_16_840_1_101_2_1_3:

            oidPath += "/[Policy]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: return $"{oidPath}/[id-sdns-security-policy-id]";
                case 2: return $"{oidPath}/[id-sdns-prbac-id]";
                case 3: return $"{oidPath}/[id-mosaic-prbac-id]";
                case 10: goto oid_2_16_840_1_101_2_1_3_10;
                case 11: goto oid_2_16_840_1_101_2_1_3_11;
                case 12: return $"{oidPath}/[defaultSecurityPolicy]";
                case 13: goto oid_2_16_840_1_101_2_1_3_13;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // 10
        #region 2.16.840.1.101.2.1.3.10.*

        oid_2_16_840_1_101_2_1_3_10:

            oidPath += "/[siSecurityPolicy]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[siNASP]",
                1 => $"{oidPath}/[siELCO]",
                2 => $"{oidPath}/[siTK]",
                3 => $"{oidPath}/[siDSAP]",
                4 => $"{oidPath}/[siSSSS]",
                5 => $"{oidPath}/[siDNASP]",
                6 => $"{oidPath}/[siBYEMAN]",
                7 => $"{oidPath}/[siREL-US]",
                8 => $"{oidPath}/[siREL-AUS]",
                9 => $"{oidPath}/[siREL-CAN]",
                10 => $"{oidPath}/[siREL-UK]",
                11 => $"{oidPath}/[siREL-NZ]",
                12 => $"{oidPath}/[siGeneric]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // 11
        #region 2.16.840.1.101.2.1.3.11.*

        oid_2_16_840_1_101_2_1_3_11:

            oidPath += "/[Genser security policy]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: return $"{oidPath}/[genserNations]";
                case 1: return $"{oidPath}/[genserComsec]";
                case 2: return $"{oidPath}/[genserAcquisition]";
                case 3: goto oid_2_16_840_1_101_2_1_3_11_3;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // 3
        #region 2.16.840.1.101.2.1.3.11.3.*

        oid_2_16_840_1_101_2_1_3_11_3:

            oidPath += "/[genserSecurityCategories]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[genserTagSetName]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // 13
        #region 2.16.840.1.101.2.1.3.13.*

        oid_2_16_840_1_101_2_1_3_13:

            oidPath += "/[capcoMarkings]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_2_16_840_1_101_2_1_3_13_0;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // 0
        #region 2.16.840.1.101.2.1.3.13.0.*

        oid_2_16_840_1_101_2_1_3_13_0:

            oidPath += "/[capcoSecurityCategories]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[capcoTagSetName1]",
                2 => $"{oidPath}/[capcoTagSetName2]",
                3 => $"{oidPath}/[capcoTagSetName3]",
                4 => $"{oidPath}/[capcoTagSetName4]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        // id-object-classes
        #region 2.16.840.1.101.2.1.4.*

        oid_2_16_840_1_101_2_1_4:

            oidPath += "/[Object classes]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[id-msp-user-sdns]",
                2 => $"{oidPath}/[id-mail-list]",
                3 => $"{oidPath}/[id-dsa-sdns]",
                4 => $"{oidPath}/[id-ca-sdns]",
                5 => $"{oidPath}/[id-crls-sdns]",
                6 => $"{oidPath}/[id-msp-user-mosaic]",
                7 => $"{oidPath}/[id-dsa-mosaic]",
                8 => $"{oidPath}/[id-ca-mosaic]",
                9 => $"{oidPath}/[id-krl-mosaic]",
                10 => $"{oidPath}/[id-strong-auth-user-sdns]",
                11 => $"{oidPath}/[id-strong-auth-user-mosaic]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // attributes, id-attributes
        #region 2.16.840.1.101.2.1.5.*

        oid_2_16_840_1_101_2_1_5:

            oidPath += "/[Attributes]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[id-sdnsKeyManagementCertificate]",
                2 => $"{oidPath}/[id-sdnsUserSignatureCertificate]",
                3 => $"{oidPath}/[id-sdnsKMandSigCertificate]",
                4 => $"{oidPath}/[id-mosaicKeyManagementCertificate]",
                5 => $"{oidPath}/[id-mosaicKMandSigCertificate]",
                6 => $"{oidPath}/[id-mosaicUserSignatureCertificate]",
                7 => $"{oidPath}/[id-mosaicCASignatureCertificate]",
                8 => $"{oidPath}/[id-sdnsCASignatureCertificate]",
                10 => $"{oidPath}/[id-auxiliaryVector]",
                11 => $"{oidPath}/[id-mlReceiptPolicy]",
                12 => $"{oidPath}/[id-mlMembership]",
                13 => $"{oidPath}/[id-mlAdministrators]",
                14 => $"{oidPath}/[id-mlid]",
                20 => $"{oidPath}/[id-janUKMs]",
                21 => $"{oidPath}/[id-febUKMs]",
                22 => $"{oidPath}/[id-marUKMs]",
                23 => $"{oidPath}/[id-aprUKMs]",
                24 => $"{oidPath}/[id-mayUKMs]",
                25 => $"{oidPath}/[id-junUKMs]",
                26 => $"{oidPath}/[id-julUKMs]",
                27 => $"{oidPath}/[id-augUKMs]",
                28 => $"{oidPath}/[id-sepUKMs]",
                29 => $"{oidPath}/[id-octUKMs]",
                30 => $"{oidPath}/[id-novUKMs]",
                31 => $"{oidPath}/[id-decUKMs]",
                40 => $"{oidPath}/[id-metaSDNScrl]",
                41 => $"{oidPath}/[id-sdnsCRL]",
                42 => $"{oidPath}/[id-metaSDNSsignatureCRL]",
                43 => $"{oidPath}/[id-SDNSsignatureCRL]",
                44 => $"{oidPath}/[id-sdnsCertificateRevocationList]",
                45 => $"{oidPath}/[id-mosaicCertificateRevocationList]",
                46 => $"{oidPath}/[id-mosaicKRL]",
                47 => $"{oidPath}/[id-mlExemptedAddressProcessor]",
                48 => $"{oidPath}/[id-snsGuardGateway]",
                49 => $"{oidPath}/[id-algorithmsSupported]",
                50 => $"{oidPath}/[id-suiteAKeyManagementCertificate]",
                51 => $"{oidPath}/[id-suiteAKMandSigCertificate]",
                52 => $"{oidPath}/[id-suiteAUserSignatureCertificate]",
                53 => $"{oidPath}/[prbacInfo]",
                54 => $"{oidPath}/[prbacCAConstraints]",
                55 => $"{oidPath}/[sigOrKMPrivileges]",
                56 => $"{oidPath}/[commPrivileges]",
                57 => $"{oidPath}/[labeledAttribute]",
                58 => $"{oidPath}/[policyInformationFile]",
                59 => $"{oidPath}/[DMS Security Policy Information File (SPIF) attribute]",
                60 => $"{oidPath}/[cAClearanceConstraint]",
                63 => $"{oidPath}/[id-aa-TAMP-transitionalPublicKeyDecryptKey]",
                65 => $"{oidPath}/[id-aa-KP-keyPkgIdAndReceiptReq]",
                66 => $"{oidPath}/[id-aa-KP-contentDecryptKeyID]",
                68 => $"{oidPath}/[id-clearanceSponsor]",
                69 => $"{oidPath}/[id-deviceOwner]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // id-attribute-syntax
        #region 2.16.840.1.101.2.1.6.*

        oid_2_16_840_1_101_2_1_6:

            oidPath += "/[Attribute syntaxes]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[id-monthlyUKMsyntax]",
                2 => $"{oidPath}/[id-mLReceiptPolicy]",
                3 => $"{oidPath}/[id-oRNameListSyntax]",
                4 => $"{oidPath}/[id-kmidSyntax]",
                5 => $"{oidPath}/[id-cRLinfoSyntax]",
                6 => $"{oidPath}/[id-cAcrlSyntax]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // 7
        #region 2.16.840.1.101.2.1.7.*

        oid_2_16_840_1_101_2_1_7:

            oidPath += "/[Extensions]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_16_840_1_101_2_1_7_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // 1
        #region 2.16.840.1.101.2.1.7.1.*

        oid_2_16_840_1_101_2_1_7_1:

            oidPath += "/[cspExtns]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[cspCsExtn]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // id-security-categories
        #region 2.16.840.1.101.2.1.8.*

        oid_2_16_840_1_101_2_1_8:

            oidPath += "/[Security categories]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: return $"{oidPath}/[mISSISecurityCategories]";
                case 2: return $"{oidPath}/[standardSecurityLabelPrivileges]";
                case 3: goto oid_2_16_840_1_101_2_1_8_3;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // commonSecurityCategoriesSyntaxes
        #region 2.16.840.1.101.2.1.8.3.*

        oid_2_16_840_1_101_2_1_8_3:

            oidPath += "/[Common security categories syntaxes]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[restrictiveAttributes]",
                1 => $"{oidPath}/[enumeratedPermissiveAttributes]",
                2 => $"{oidPath}/[permissiveAttributes]",
                3 => $"{oidPath}/[informativeAttributes]",
                4 => $"{oidPath}/[enumeratedRestrictiveAttributes]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // 10
        #region 2.16.840.1.101.2.1.10.*

        oid_2_16_840_1_101_2_1_10:

            oidPath += "/[Privileges]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[sigPrivileges]",
                2 => $"{oidPath}/[kmPrivileges]",
                3 => $"{oidPath}/[namedTagSetPrivilege]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // 11
        #region 2.16.840.1.101.2.1.11.*

        oid_2_16_840_1_101_2_1_11:

            oidPath += "/[United States Department of Defense (DoD) Public Key Infrastructure (PKI) certificate policies, covering levels of assurance]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[ukDemo]",
                2 => $"{oidPath}/[usDODClass2]",
                3 => $"{oidPath}/[usMediumPilot]",
                4 => $"{oidPath}/[usDODClass4]",
                5 => $"{oidPath}/[usDODClass3]",
                6 => $"{oidPath}/[usDODClass5]",
                9 => $"{oidPath}/[id-US-dod-mediumHardware]",
                19 => $"{oidPath}/[id-US-dod-mediumHardware-2048]",
                39 => $"{oidPath}/[id-US-dod-medium-112]",
                42 => $"{oidPath}/[id-US-dod-mediumHardware-112]",
                59 => $"{oidPath}/[id-US-dod-admin]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // 12
        #region 2.16.840.1.101.2.1.12.*

        oid_2_16_840_1_101_2_1_12:

            oidPath += "/[Security policy]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_2_16_840_1_101_2_1_12_0;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // 0
        #region 2.16.840.1.101.2.1.12.0.*

        oid_2_16_840_1_101_2_1_12_0:

            oidPath += "/[testSecurityPolicy]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_16_840_1_101_2_1_12_0_1;
                case 2: goto oid_2_16_840_1_101_2_1_12_0_2;
                case 3: goto oid_2_16_840_1_101_2_1_12_0_3;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // 1
        #region 2.16.840.1.101.2.1.12.0.1.*

        oid_2_16_840_1_101_2_1_12_0_1:

            oidPath += "/[tsp1]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_2_16_840_1_101_2_1_12_0_1_0;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // 0
        #region 2.16.840.1.101.2.1.12.0.1.0.*

        oid_2_16_840_1_101_2_1_12_0_1_0:

            oidPath += "/[tsp1SecurityCategories]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[tsp1TagSetZero]",
                1 => $"{oidPath}/[tsp1TagSetOne]",
                2 => $"{oidPath}/[tsp1TagSetTwo]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // 2
        #region 2.16.840.1.101.2.1.12.0.2.*

        oid_2_16_840_1_101_2_1_12_0_2:

            oidPath += "/[tsp2]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_2_16_840_1_101_2_1_12_0_2_0;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // 0
        #region 2.16.840.1.101.2.1.12.0.2.0.*

        oid_2_16_840_1_101_2_1_12_0_2_0:

            oidPath += "/[tsp2SecurityCategories]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[tsp2TagSetZero]",
                1 => $"{oidPath}/[tsp2TagSetOne]",
                2 => $"{oidPath}/[tsp2TagSetTwo]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // 3
        #region 2.16.840.1.101.2.1.12.0.3.*

        oid_2_16_840_1_101_2_1_12_0_3:

            oidPath += "/[kafka]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_2_16_840_1_101_2_1_12_0_3_0;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // 0
        #region 2.16.840.1.101.2.1.12.0.3.0.*

        oid_2_16_840_1_101_2_1_12_0_3_0:

            oidPath += "/[kafkaSecurityCategories]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[kafkaTagSetName1]",
                2 => $"{oidPath}/[kafkaTagSetName2]",
                3 => $"{oidPath}/[kafkaTagSetName3]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        #endregion

        // sir-name-types
        #region 2.16.840.1.101.2.1.16.*

        oid_2_16_840_1_101_2_1_16:

            oidPath += "/[Source Intermediary Recipient (SIR) name types]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[id-dn]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // id-infosec-contentTypes
        #region 2.16.840.1.101.2.1.20.*

        oid_2_16_840_1_101_2_1_20:

            oidPath += "/[Content types]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[id-ct-keyPackage]",
                2 => $"{oidPath}/[id-ct-encryptedKeyPkg]",
                3 => $"{oidPath}/[id-ct-keyPackageReceipt]",
                4 => $"{oidPath}/[id-ct-keyPackageError]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // id-infosec-kmProperties
        #region 2.16.840.1.101.2.1.21.*

        oid_2_16_840_1_101_2_1_21:

            oidPath += "/[KM types]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[id-kmp-keyAlgorithm]",
                2 => $"{oidPath}/[id-kmp-certType]",
                3 => $"{oidPath}/[id-kmp-TSECNomenclature]",
                4 => $"{oidPath}/[id-kmp-keyPurposeAndUse]",
                5 => $"{oidPath}/[id-kmp-keyDistPeriod]",
                6 => $"{oidPath}/[id-kmp-keyValidityPeriod]",
                7 => $"{oidPath}/[id-kmp-keyDuration]",
                8 => $"{oidPath}/[id-kmp-classification]",
                9 => $"{oidPath}/[id-kmp-keyPkgReceivers]",
                10 => $"{oidPath}/[id-kmp-splitID]",
                11 => $"{oidPath}/[id-kmp-keyPkgType]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // id-infosec-certTypes
        #region 2.16.840.1.101.2.1.22.*

        oid_2_16_840_1_101_2_1_22:

            oidPath += "/[Certification types]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[id-certType-X509]",
                2 => $"{oidPath}/[id-certType-FIREFLY]",
                3 => $"{oidPath}/[id-certType-EnhancedFIREFLY]",
                4 => $"{oidPath}/[id-certType-MAYFLY]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // id-ds
        #region 2.16.840.1.101.2.2.*

        oid_2_16_840_1_101_2_2:

            oidPath += "/[Rec. ITU-T X.500 Directory information]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_16_840_1_101_2_2_1;
                case 3: goto oid_2_16_840_1_101_2_2_3;
                case 4: goto oid_2_16_840_1_101_2_2_4;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // attributeType
        #region 2.16.840.1.101.2.2.1.*

        oid_2_16_840_1_101_2_2_1:

            oidPath += "/[attributeType]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                3 => $"{oidPath}/[Alternate recipient]",
                4 => $"{oidPath}/[Associated organization]",
                5 => $"{oidPath}/[Associated address list]",
                6 => $"{oidPath}/[Associated PLA]",
                8 => $"{oidPath}/[Address list type]",
                45 => $"{oidPath}/[Release authority name]",
                46 => $"{oidPath}/[Action address]",
                47 => $"{oidPath}/[Additional addressees]",
                48 => $"{oidPath}/[Additional second party addressees]",
                49 => $"{oidPath}/[Alias pointer]",
                50 => $"{oidPath}/[Allowable originators]",
                51 => $"{oidPath}/[cognizantAuthority]",
                52 => $"{oidPath}/[community]",
                53 => $"{oidPath}/[dodaac]",
                54 => $"{oidPath}/[dualRoute]",
                55 => $"{oidPath}/[effectiveDate]",
                56 => $"{oidPath}/[entryClassification]",
                57 => $"{oidPath}/[expirationDate]",
                58 => $"{oidPath}/[hostOrganizationalPLA]",
                60 => $"{oidPath}/[lastRecapDate]",
                61 => $"{oidPath}/[listPointer]",
                62 => $"{oidPath}/[lmf]",
                63 => $"{oidPath}/[longTitle]",
                67 => $"{oidPath}/[nameClassification]",
                68 => $"{oidPath}/[nationality]",
                69 => $"{oidPath}/[navcompars]",
                70 => $"{oidPath}/[plaName]",
                71 => $"{oidPath}/[plaAddressees]",
                72 => $"{oidPath}/[plaReplace]",
                73 => $"{oidPath}/[primarySpelling]",
                74 => $"{oidPath}/[publish]",
                75 => $"{oidPath}/[recapDueDate]",
                76 => $"{oidPath}/[remarks]",
                77 => $"{oidPath}/[rI]",
                78 => $"{oidPath}/[rIClassification]",
                79 => $"{oidPath}/[rIInfo]",
                80 => $"{oidPath}/[secondPartyAddressees]",
                81 => $"{oidPath}/[section]",
                82 => $"{oidPath}/[serviceOrAgency]",
                83 => $"{oidPath}/[sHD]",
                84 => $"{oidPath}/[shortTitle]",
                85 => $"{oidPath}/[sigad]",
                86 => $"{oidPath}/[spot]",
                87 => $"{oidPath}/[tARE]",
                96 => $"{oidPath}/[tCC]",
                97 => $"{oidPath}/[tRC]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // objectClass
        #region 2.16.840.1.101.2.2.3.*

        oid_2_16_840_1_101_2_2_3:

            oidPath += "/[objectClass]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                22 => $"{oidPath}/[alternateSpellingPLA]",
                26 => $"{oidPath}/[plaData]",
                28 => $"{oidPath}/[cadPLA]",
                34 => $"{oidPath}/[organizationalPLA]",
                35 => $"{oidPath}/[plaCollective]",
                37 => $"{oidPath}/[routingIndicator]",
                38 => $"{oidPath}/[sigintPLA]",
                39 => $"{oidPath}/[sIPLA]",
                41 => $"{oidPath}/[taskForcePLA]",
                42 => $"{oidPath}/[tenantPLA]",
                47 => $"{oidPath}/[pla]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // nameForm
        #region 2.16.840.1.101.2.2.4.*

        oid_2_16_840_1_101_2_2_4:

            oidPath += "/[nameForm]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                4 => $"{oidPath}/[alternateSpellingPLANameForm]",
                6 => $"{oidPath}/[cadPLANameForm]",
                9 => $"{oidPath}/[mLANameForm]",
                12 => $"{oidPath}/[organizationalPLANameForm]",
                13 => $"{oidPath}/[plaCollectiveNameForm]",
                14 => $"{oidPath}/[releaseAuthorityNameForm]",
                15 => $"{oidPath}/[routingIndicatorNameForm]",
                16 => $"{oidPath}/[sigintPLANameForm]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        // csor
        #region 2.16.840.1.101.3.*

        oid_2_16_840_1_101_3:

            oidPath += "/[Computer Security Objects Register (CSOR)]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: return $"{oidPath}";
                case 2: goto oid_2_16_840_1_101_3_2;
                case 3: goto oid_2_16_840_1_101_3_3;
                case 4: goto oid_2_16_840_1_101_3_4;
                case 6: goto oid_2_16_840_1_101_3_6;
                case 9: goto oid_2_16_840_1_101_3_9;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // pki
        #region 2.16.840.1.101.3.2.*

        oid_2_16_840_1_101_3_2:

            oidPath += "/[Public Key Infrastructure (PKI)]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_16_840_1_101_3_2_1;
                case 2: goto oid_2_16_840_1_101_3_2_2;
                case 3: goto oid_2_16_840_1_101_3_2_3;
                case 4: return $"{oidPath}/[keyrecoveryschemes]";
                case 5: return $"{oidPath}/[krapola]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // csor-certpolicy
        #region 2.16.840.1.101.3.2.1.*

        oid_2_16_840_1_101_3_2_1:

            oidPath += "/[National Institute of Standards and Technology (NIST) Computer Security Objects Register (CSOR) certificate policy]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_16_840_1_101_3_2_1_1;
                case 2: return $"{oidPath}/[Patent and Trademark Office (PTO) policies]";
                case 3: goto oid_2_16_840_1_101_3_2_1_3;
                case 4: goto oid_2_16_840_1_101_3_2_1_4;
                case 5: goto oid_2_16_840_1_101_3_2_1_5;
                case 6: goto oid_2_16_840_1_101_3_2_1_6;
                case 7: goto oid_2_16_840_1_101_3_2_1_7;
                case 8: goto oid_2_16_840_1_101_3_2_1_8;
                case 9: goto oid_2_16_840_1_101_3_2_1_9;
                case 48: goto oid_2_16_840_1_101_3_2_1_48;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // aces
        #region 2.16.840.1.101.3.2.1.1.*

        oid_2_16_840_1_101_3_2_1_1:

            oidPath += "/[Access Certificates for Electronic Services (ACES)]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Certification Authorities]",
                2 => $"{oidPath}/[Identity]",
                3 => $"{oidPath}/[Business Representative]",
                4 => $"{oidPath}/[Qualified Relying Party Application]",
                5 => $"{oidPath}/[Extended validation certificate]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // fbca-policies
        #region 2.16.840.1.101.3.2.1.3.*

        oid_2_16_840_1_101_3_2_1_3:

            oidPath += "/[Federal Bridge Certification Authority (FBCA) policies]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[fpki-certpcy-rudimentaryAssurance]",
                2 => $"{oidPath}/[fpki-certpcy-basicAssurance]",
                3 => $"{oidPath}/[fpki-certpcy-mediumAssurance]",
                4 => $"{oidPath}/[fpki-certpcy-highAssurance]",
                5 => $"{oidPath}/[fpki-certpcy-testAssurance]",
                6 => $"{oidPath}/[fpki-common-policy]",
                7 => $"{oidPath}/[id-fpki-common-hardware]",
                8 => $"{oidPath}/[id-fpki-common-devices]",
                12 => $"{oidPath}/[id-fpki-certpcy-mediumHardware]",
                13 => $"{oidPath}/[id-fpki-common-authentication]",
                14 => $"{oidPath}/[id-fpki-certpcy-medium-CBP]",
                15 => $"{oidPath}/[id-fpki-certpcy-mediumHW-CBP]",
                16 => $"{oidPath}/[id-fpki-common-High]",
                17 => $"{oidPath}/[id-fpki-common-cardAuth]",
                21 => $"{oidPath}/[fpki-SHA1-medium-CBP]",
                22 => $"{oidPath}/[fpki-SHA1-mediumHW-CBP]",
                23 => $"{oidPath}/[fpki-SHA1-policy]",
                24 => $"{oidPath}/[fpki-SHA1-hardware]",
                36 => $"{oidPath}/[id-fpki-common-devicesHardware]",
                39 => $"{oidPath}/[id-fpki-common-piv-contentSigning]",
                40 => $"{oidPath}/[id-fpki-common-derived-pivAuth]",
                41 => $"{oidPath}/[id-fpki-common-derived-pivAuth-hardware]",
                45 => $"{oidPath}/[id-fpki-common-pivi-authentication]",
                46 => $"{oidPath}/[id-fpki-common-pivi-cardAuth]",
                47 => $"{oidPath}/[id-fpki-common-pivi-contentSigning]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // nist-policies
        #region 2.16.840.1.101.3.2.1.4.*

        oid_2_16_840_1_101_3_2_1_4:

            oidPath += "/[National Institute of Standards and Technology (NIST) policies]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Certificate Policy 1]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // treasury-policies
        #region 2.16.840.1.101.3.2.1.5.*

        oid_2_16_840_1_101_3_2_1_5:

            oidPath += "/[Treasury policies]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Certificate Policy 1]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // state-policies
        #region 2.16.840.1.101.3.2.1.6.*

        oid_2_16_840_1_101_3_2_1_6:

            oidPath += "/[State Department policies]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[state-basic]",
                2 => $"{oidPath}/[state-low]",
                3 => $"{oidPath}/[state-moderate]",
                4 => $"{oidPath}/[state-high]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // fdic-policies
        #region 2.16.840.1.101.3.2.1.7.*

        oid_2_16_840_1_101_3_2_1_7:

            oidPath += "/[Federal Deposit Insurance Corporation (FDIC) policies]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[fdic-basic]",
                2 => $"{oidPath}/[fdic-low]",
                3 => $"{oidPath}/[fdic-moderate]",
                4 => $"{oidPath}/[fdic-high]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // usda-nfo-policies
        #region 2.16.840.1.101.3.2.1.8.*

        oid_2_16_840_1_101_3_2_1_8:

            oidPath += "/[usda-nfo-policies]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[basicAssurance]",
                2 => $"{oidPath}/[mediumAssurance]",
                3 => $"{oidPath}/[highAssurance]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // dea-policies
        #region 2.16.840.1.101.3.2.1.9.*

        oid_2_16_840_1_101_3_2_1_9:

            oidPath += "/[dea-policies]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[dea-policy1]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // csor-test-policies
        #region 2.16.840.1.101.3.2.1.48.*

        oid_2_16_840_1_101_3_2_1_48:

            oidPath += "/[Test certificate policy to support Public-Key Infrastructure (PKI) pilots and testing]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[test1]",
                2 => $"{oidPath}/[test2]",
                3 => $"{oidPath}/[test3]",
                4 => $"{oidPath}/[test4]",
                5 => $"{oidPath}/[test5]",
                6 => $"{oidPath}/[test6]",
                7 => $"{oidPath}/[test7]",
                8 => $"{oidPath}/[test8]",
                9 => $"{oidPath}/[test9]",
                10 => $"{oidPath}/[test10]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // 2
        #region 2.16.840.1.101.3.2.2.*

        oid_2_16_840_1_101_3_2_2:

            oidPath += "/[CSOR GAK extended key usage GAK]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[kRAKey]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // extensions
        #region 2.16.840.1.101.3.2.3.*

        oid_2_16_840_1_101_3_2_3:

            oidPath += "/[Computer Security Objects Register (CSOR) Governmental Accessed Key (GAK) extensions]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[kRTechnique]",
                2 => $"{oidPath}/[kRecoveryCapable]",
                3 => $"{oidPath}/[kR]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // iosp
        #region 2.16.840.1.101.3.3.*

        oid_2_16_840_1_101_3_3:

            oidPath += "/[Information Object Security Project (IOSP)]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_2_16_840_1_101_3_3_0;
                case 1: goto oid_2_16_840_1_101_3_3_1;
                case 2: goto oid_2_16_840_1_101_3_3_2;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // components
        #region 2.16.840.1.101.3.3.0.*

        oid_2_16_840_1_101_3_3_0:

            oidPath += "/[Components]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: return $"{oidPath}/[dataComponent]";
                case 1: return $"{oidPath}/[accessControlComponent]";
                case 2: return $"{oidPath}/[confidentialityComponent]";
                case 3: return $"{oidPath}/[signatureComponent]";
                case 4: return $"{oidPath}/[keyManagementComponent]";
                case 5: goto oid_2_16_840_1_101_3_3_0_5;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // annotationComponent
        #region 2.16.840.1.101.3.3.0.5.*

        oid_2_16_840_1_101_3_3_0_5:

            oidPath += "/[annotationComponent]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[linear]",
                1 => $"{oidPath}/[rowcol]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // dataType
        #region 2.16.840.1.101.3.3.1.*

        oid_2_16_840_1_101_3_3_1:

            oidPath += "/[Data type]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: return $"{oidPath}/[pemData]";
                case 2: return $"{oidPath}/[mimeData]";
                case 3: return $"{oidPath}/[hashData]";
                case 4: return $"{oidPath}/[protectedComponent]";
                case 5: return $"{oidPath}/[binaryData]";
                case 6: return $"{oidPath}/[ia5Data]";
                case 7: return $"{oidPath}/[iosComponentList]";
                case 8: goto oid_2_16_840_1_101_3_3_1_8;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // tokenData
        #region 2.16.840.1.101.3.3.1.8.*

        oid_2_16_840_1_101_3_3_1_8:

            oidPath += "/[tokenData]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[publicKeyToken]",
                1 => $"{oidPath}/[symmetricKey]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // pki
        #region 2.16.840.1.101.3.3.2.*

        oid_2_16_840_1_101_3_3_2:

            oidPath += "/[Public Key Infrastructure (PKI)]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: return $"{oidPath}/[cert-policy]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        #endregion

        // nistAlgorithms
        #region 2.16.840.1.101.3.4.*

        oid_2_16_840_1_101_3_4:

            oidPath += "/[National Institute of Standards and Technology (NIST) algorithms]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_2_16_840_1_101_3_4_0;
                case 1: goto oid_2_16_840_1_101_3_4_1;
                case 2: goto oid_2_16_840_1_101_3_4_2;
                case 3: goto oid_2_16_840_1_101_3_4_3;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // modules
        #region 2.16.840.1.101.3.4.0.*

        oid_2_16_840_1_101_3_4_0:

            oidPath += "/[ASN.1 modules]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[AES]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // aes
        #region 2.16.840.1.101.3.4.1.*

        oid_2_16_840_1_101_3_4_1:

            oidPath += "/[Advanced Encryption Standard (AES)]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[aes128-ECB]",
                2 => $"{oidPath}/[aes128-CBC-PAD]",
                3 => $"{oidPath}/[aes128-OFB]",
                4 => $"{oidPath}/[aes128-CFB]",
                5 => $"{oidPath}/[aes128-wrap]",
                6 => $"{oidPath}/[aes128-GCM]",
                7 => $"{oidPath}/[aes128-CCM]",
                8 => $"{oidPath}/[aes128-wrap-pad]",
                9 => $"{oidPath}/[aes128-GMAC]",
                21 => $"{oidPath}/[aes192-ECB]",
                22 => $"{oidPath}/[aes192-CBC-PAD]",
                23 => $"{oidPath}/[aes192-OFB]",
                24 => $"{oidPath}/[aes192-CFB]",
                25 => $"{oidPath}/[aes192-wrap]",
                26 => $"{oidPath}/[aes192-GCM]",
                27 => $"{oidPath}/[aes192-CCM]",
                28 => $"{oidPath}/[aes192-wrap-pad]",
                29 => $"{oidPath}/[aes192-GMAC]",
                41 => $"{oidPath}/[aes256-ECB]",
                42 => $"{oidPath}/[aes256-CBC-PAD]",
                43 => $"{oidPath}/[aes256-OFB]",
                44 => $"{oidPath}/[aes256-CFB]",
                45 => $"{oidPath}/[aes256-wrap]",
                46 => $"{oidPath}/[aes256-GCM]",
                47 => $"{oidPath}/[aes256-CCM]",
                48 => $"{oidPath}/[aes256-wrap-pad]",
                49 => $"{oidPath}/[aes256-GMAC]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // hashalgs, hashAlgs
        #region 2.16.840.1.101.3.4.2.*

        oid_2_16_840_1_101_3_4_2:

            oidPath += "/[NIST-SHA2]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[sha256]",
                2 => $"{oidPath}/[sha384]",
                3 => $"{oidPath}/[sha512]",
                4 => $"{oidPath}/[sha224]",
                5 => $"{oidPath}/[sha512-224]",
                6 => $"{oidPath}/[sha512-256]",
                7 => $"{oidPath}/[sha3-224]",
                8 => $"{oidPath}/[sha3-256]",
                9 => $"{oidPath}/[sha3-384]",
                10 => $"{oidPath}/[sha3-512]",
                11 => $"{oidPath}/[shake128]",
                12 => $"{oidPath}/[shake256]",
                13 => $"{oidPath}/[hmacWithSHA3-224]",
                14 => $"{oidPath}/[hmacWithSHA3-256]",
                15 => $"{oidPath}/[hmacWithSHA3-384]",
                16 => $"{oidPath}/[hmacWithSHA3-512]",
                17 => $"{oidPath}/[shake128-len]",
                18 => $"{oidPath}/[shake256-len]",
                19 => $"{oidPath}/[KmacWithSHAKE128]",
                20 => $"{oidPath}/[KmacWithSHAKE256]",
                21 => $"{oidPath}/[KMACXOF128]",
                22 => $"{oidPath}/[KACXOF256]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // sigAlgs, id-dsa-with-sha2
        #region 2.16.840.1.101.3.4.3.*

        oid_2_16_840_1_101_3_4_3:

            oidPath += "/[Signature algorithms]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[dsa-with-sha224]",
                2 => $"{oidPath}/[dsa-with-sha256]",
                3 => $"{oidPath}/[dsa-with-sha384]",
                4 => $"{oidPath}/[dsa-with-sha512]",
                10 => $"{oidPath}/[ecdsa-with-sha3-256]",
                13 => $"{oidPath}/[rsassa-pkcs1-v1-5-with-sha3-224]",
                14 => $"{oidPath}/[rsassa-pkcs1-v1-5-with-sha3-256]",
                15 => $"{oidPath}/[rsassa-pkcs1-v1-5-with-sha3-384]",
                16 => $"{oidPath}/[rsassa-pkcs1-v1-5-with-sha3-512]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // 6
        #region 2.16.840.1.101.3.6.*

        oid_2_16_840_1_101_3_6:

            oidPath += "/[GeneralName otherName type-ids]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                6 => $"{oidPath}/[Federal Agency Smart Credential Number (FASC-N)]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // biometrics
        #region 2.16.840.1.101.3.9.*

        oid_2_16_840_1_101_3_9:

            oidPath += "/[National Institute of Standards and Technology (NIST) Biometric Clients Lab]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: return $"{oidPath}/[Smartcards and biometrics in relation to identity management]";
                case 1: return $"{oidPath}/[Multimodal Biometric Application Resource Kit (MBARK)]";
                case 2: goto oid_2_16_840_1_101_3_9_2;
                case 3: goto oid_2_16_840_1_101_3_9_3;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // nbcl
        #region 2.16.840.1.101.3.9.2.*

        oid_2_16_840_1_101_3_9_2:

            oidPath += "/[NIST Biometric Clients Laboratory]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Certificate Authority]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // wsbd
        #region 2.16.840.1.101.3.9.3.*

        oid_2_16_840_1_101_3_9_3:

            oidPath += "/[Web Services for Biometric Devices (WSBD)]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Version 1]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        // us-government-org
        #region 2.16.840.1.101.10.*

        oid_2_16_840_1_101_10:

            oidPath += "/[U.S. General Services Administration]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 2: goto oid_2_16_840_1_101_10_2;
                case 51: return $"{oidPath}/[Department of Justice]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // treasury
        #region 2.16.840.1.101.10.2.*

        oid_2_16_840_1_101_10_2:

            oidPath += "/[Department of the Treasury]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: return $"{oidPath}/[Secret Service]";
                case 15: goto oid_2_16_840_1_101_10_2_15;
                case 16: return $"{oidPath}/[Financial Management Service]";
                case 17: return $"{oidPath}/[Fiscal service]";
                case 18: goto oid_2_16_840_1_101_10_2_18;
                case 19: return $"{oidPath}/[Common Approach to Identity Assurance (CAIA)]";
                case 30: goto oid_2_16_840_1_101_10_2_30;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // bpd
        #region 2.16.840.1.101.10.2.15.*

        oid_2_16_840_1_101_10_2_15:

            oidPath += "/[Public Debt]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_16_840_1_101_10_2_15_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // directory
        #region 2.16.840.1.101.10.2.15.1.*

        oid_2_16_840_1_101_10_2_15_1:

            oidPath += "/[Directory objects]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[ObjectClass definitions]",
                2 => $"{oidPath}/[Attribute definitions]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // bfs
        #region 2.16.840.1.101.10.2.18.*

        oid_2_16_840_1_101_10_2_18:

            oidPath += "/[Bureau of the Fiscal Service]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_16_840_1_101_10_2_18_1;
                case 2: goto oid_2_16_840_1_101_10_2_18_2;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // ldap
        #region 2.16.840.1.101.10.2.18.1.*

        oid_2_16_840_1_101_10_2_18_1:

            oidPath += "/[Lightweight Directory Access Protocol (LDAP) schema objects]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}",
                2 => $"{oidPath}",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // pki
        #region 2.16.840.1.101.10.2.18.2.*

        oid_2_16_840_1_101_10_2_18_2:

            oidPath += "/[Public Key Infrastructure (PKI)]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Server-based Certificate Validation Protocol (SCVP) policies]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // 30
        #region 2.16.840.1.101.10.2.30.*

        oid_2_16_840_1_101_10_2_30:

            oidPath += "/[Internal revenue service]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_16_840_1_101_10_2_30_1;
                case 2: goto oid_2_16_840_1_101_10_2_30_2;
                case 3: return $"{oidPath}/[Directory name forms]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // 1
        #region 2.16.840.1.101.10.2.30.1.*

        oid_2_16_840_1_101_10_2_30_1:

            oidPath += "/[Directory attributes]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_16_840_1_101_10_2_30_1_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // 1
        #region 2.16.840.1.101.10.2.30.1.1.*

        oid_2_16_840_1_101_10_2_30_1_1:

            oidPath += "/[Dimensional Model]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[organizationalUnitDN]",
                2 => $"{oidPath}/[localityDN]",
                3 => $"{oidPath}/[personDN]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // 2
        #region 2.16.840.1.101.10.2.30.2.*

        oid_2_16_840_1_101_10_2_30_2:

            oidPath += "/[Directory object classes]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_16_840_1_101_10_2_30_2_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // 1
        #region 2.16.840.1.101.10.2.30.2.1.*

        oid_2_16_840_1_101_10_2_30_2_1:

            oidPath += "/[Dimensional Model]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[dimensionalModelAuxClass]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        #endregion

        #endregion

        // 100
        #region 2.16.840.1.101.100.*

        oid_2_16_840_1_101_100:

            oidPath += "/[Government-wide programs]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}",
                2 => $"{oidPath}",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // lotus
        #region 2.16.840.1.113678.*

        oid_2_16_840_1_113678:

            oidPath += "/[Lotus Corporation]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_16_840_1_113678_1;
                case 2: goto oid_2_16_840_1_113678_2;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // desktop-apps
        #region 2.16.840.1.113678.1.*

        oid_2_16_840_1_113678_1:

            oidPath += "/[Lotus Corporation desktop products]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_16_840_1_113678_1_1;
                case 2: goto oid_2_16_840_1_113678_1_2;
                case 3: goto oid_2_16_840_1_113678_1_3;
                case 4: goto oid_2_16_840_1_113678_1_4;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // 1
        #region 2.16.840.1.113678.1.1.*

        oid_2_16_840_1_113678_1_1:

            oidPath += "/[Lotus 1-2-3]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Attachment type for a Rec. ITU-T X.400 file transfer Bodypart containing a Lotus 1-2-3 file]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // amipro
        #region 2.16.840.1.113678.1.2.*

        oid_2_16_840_1_113678_1_2:

            oidPath += "/[Lotus AMIPro]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Attachment type for an X.400 File Transfer Bodypart containing a Lotus AMIPro Word Processing file]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // freelance
        #region 2.16.840.1.113678.1.3.*

        oid_2_16_840_1_113678_1_3:

            oidPath += "/[Lotus Freelance]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Attachment type for an X.400 File Transfer Bodypart containing a Lotus Freelance presentation graphics file]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // approach
        #region 2.16.840.1.113678.1.4.*

        oid_2_16_840_1_113678_1_4:

            oidPath += "/[Lotus Approach]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Attachment type for an X.400 File Transfer Bodypart containing a Lotus Apporach data base file]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // communications-apps
        #region 2.16.840.1.113678.2.*

        oid_2_16_840_1_113678_2:

            oidPath += "/[communications-apps]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 2: goto oid_2_16_840_1_113678_2_2;
                case 4: goto oid_2_16_840_1_113678_2_4;
                case 5: goto oid_2_16_840_1_113678_2_5;
                case 6: goto oid_2_16_840_1_113678_2_6;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // notes
        #region 2.16.840.1.113678.2.2.*

        oid_2_16_840_1_113678_2_2:

            oidPath += "/[Lotus Notes]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_16_840_1_113678_2_2_1;
                case 2: goto oid_2_16_840_1_113678_2_2_2;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // notes
        #region 2.16.840.1.113678.2.2.1.*

        oid_2_16_840_1_113678_2_2_1:

            oidPath += "/[files]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Attachment type for an X.400 File Transfer Bodypart containing a Lotus Notes groupware file (linked in singleNoteDB)]",
                2 => $"{oidPath}/[Attachment type for an X.400 File Transfer Bodypart containing a Lotus Notes groupware file (delinked in singleNoteDB)]",
                3 => $"{oidPath}/[Attachment type for an X.400 File Transfer Bodypart containing a Lotus Notes groupware file as file attachment]",
                4 => $"{oidPath}/[Lotus Notes groupware file attachment (rtf)]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // 2
        #region 2.16.840.1.113678.2.2.2.*

        oid_2_16_840_1_113678_2_2_2:

            oidPath += "/[Lotus Domino Directory Service]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[LDAP Object Classes]",
                2 => $"{oidPath}/[LDAP Attribute Types]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // vip
        #region 2.16.840.1.113678.2.4.*

        oid_2_16_840_1_113678_2_4:

            oidPath += "/[Lotus VIP]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Attachment type for an X.400 File Transfer Bodypart containing a Lotus ViP application development file]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // forms
        #region 2.16.840.1.113678.2.5.*

        oid_2_16_840_1_113678_2_5:

            oidPath += "[Lotus Forms]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Attachment type for an X.400 File Transfer Bodypart containing a Lotus Forms form definition file]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // organizer
        #region 2.16.840.1.113678.2.6.*

        oid_2_16_840_1_113678_2_6:

            oidPath += "/[Lotus Organizer]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Attachment type for a Rec. ITU-T X.400 File Transfer Bodypart containing a Lotus Organizer calendaring file]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        // 113694
        #region 2.16.840.1.113694.*

        oid_2_16_840_1_113694:

            oidPath += "/[Electronic Messaging Association (EMA)]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_16_840_1_113694_1;
                case 2: goto oid_2_16_840_1_113694_2;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // 1
        #region 2.16.840.1.113694.1.*

        oid_2_16_840_1_113694_1:

            oidPath += "/[committees]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_16_840_1_113694_1_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // 1
        #region 2.16.840.1.113694.1.1.*

        oid_2_16_840_1_113694_1_1:

            oidPath += "/[Messaging Management Committee]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_16_840_1_113694_1_1_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // 1
        #region 2.16.840.1.113694.1.1.1.*

        oid_2_16_840_1_113694_1_1_1:

            oidPath += "/[Messaging Management Technical Sub-Committee]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_16_840_1_113694_1_1_1_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // 1
        #region 2.16.840.1.113694.1.1.1.1.*

        oid_2_16_840_1_113694_1_1_1_1:

            oidPath += "/[EMA Dynamic Monitoring]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[EMA Dynamic Monitoring MTA]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        #endregion

        // 2
        #region 2.16.840.1.113694.2.*

        oid_2_16_840_1_113694_2:

            oidPath += "/[Defined objects]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 2: goto oid_2_16_840_1_113694_2_2;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // 2
        #region 2.16.840.1.113694.2.2.*

        oid_2_16_840_1_113694_2_2:

            oidPath += "/[Rec. ITU-T X.400 messaging]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_16_840_1_113694_2_2_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // attachment
        #region 2.16.840.1.113694.2.2.1.*

        oid_2_16_840_1_113694_2_2_1:

            oidPath += "/[Rec. ITU-T X.400 message attachments]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Unknown attachment]",
                2 => $"{oidPath}/[UUencoded attachment]",
                3 => $"{oidPath}/[Unknown text attachment]",
                4 => $"{oidPath}/[Attachment containing a digital image in Graphics Interchange Format (GIF)]",
                5 => $"{oidPath}/[Attachment containing a digital image in Tagged Image File Format (TIFF)]",
                6 => $"{oidPath}/[Attachment containing a compressed digital image in Joint Photographic Experts Group (JPEG) format]",
                7 => $"{oidPath}/[Attachment containing a digital image in PiCture eXchange (PCX) format]",
                8 => $"{oidPath}/[Attachment containing a digital image in PICT format]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        #endregion

        // novell
        #region 2.16.840.1.113719.*

        oid_2_16_840_1_113719:

            oidPath += "/[Novell, Inc]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_16_840_1_113719_1;
                case 2: goto oid_2_16_840_1_113719_2;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // applications
        #region 2.16.840.1.113719.1.*

        oid_2_16_840_1_113719_1:

            oidPath += "/[Applications]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 9: goto oid_2_16_840_1_113719_1_9;
                case 39: goto oid_2_16_840_1_113719_1_39;
                case 42: goto oid_2_16_840_1_113719_1_42;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // pki
        #region 2.16.840.1.113719.1.9.*

        oid_2_16_840_1_113719_1_9:

            oidPath += "/[Public key Infrastructure]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 4: goto oid_2_16_840_1_113719_1_9_4;
                case 5: return $"{oidPath}/[pkiAttributeSyntax]";
                case 6: return $"{oidPath}/[pkiObjectClass]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // pkiAttributeType
        #region 2.16.840.1.113719.1.9.4.*

        oid_2_16_840_1_113719_1_9_4:

            oidPath += "/[pkiAttributeType]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[securityAttributes]",
                2 => $"{oidPath}/[relianceLimit]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // sas
        #region 2.16.840.1.113719.1.39.*

        oid_2_16_840_1_113719_1_39:

            oidPath += "/[Secure Authentication Service (SAS)]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                43 => $"{oidPath}/[Novell Secure Password Manager (NSPM)]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // nmas
        #region 2.16.840.1.113719.1.42.*

        oid_2_16_840_1_113719_1_42:

            oidPath += "/[Novell Modular Authentication Service® (NMAS)]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 100: goto oid_2_16_840_1_113719_1_42_100;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // 100
        #region 2.16.840.1.113719.1.42.100.*

        oid_2_16_840_1_113719_1_42_100:

            oidPath += "/[???]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[NMASLDAP_PUT_LOGIN_CONFIG_REQUEST]",
                2 => $"{oidPath}/[NMASLDAP_PUT_LOGIN_CONFIG_RESPONSE]",
                3 => $"{oidPath}/[NMASLDAP_GET_LOGIN_CONFIG_REQUEST]",
                4 => $"{oidPath}/[NMASLDAP_GET_LOGIN_CONFIG_RESPONSE]",
                5 => $"{oidPath}/[NMASLDAP_DELETE_LOGIN_CONFIG_REQUEST]",
                6 => $"{oidPath}/[NMASLDAP_DELETE_LOGIN_CONFIG_RESPONSE]",
                7 => $"{oidPath}/[NMASLDAP_PUT_LOGIN_SECRET_REQUEST]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        // subregistry
        #region 2.16.840.1.113719.2.*

        oid_2_16_840_1_113719_2:

            oidPath += "/[External applications]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                124 => $"{oidPath}/[Cyvaned Systems]",
                205 => $"{oidPath}/[MUS a.s.]",
                225 => $"{oidPath}/[Epicentric, Inc]",
                247 => $"{oidPath}/[Supposed to be assigned by Novell to gid GmbH for extension of LDAP/eDirectory classes and attributes, but no offical registration was found]",
                279 => $"{oidPath}/[TEKWorx Limited]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // netscape
        #region 2.16.840.1.113730.*

        oid_2_16_840_1_113730:

            oidPath += "/[Netscape Communications Corp.]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_16_840_1_113730_1;
                case 2: goto oid_2_16_840_1_113730_2;
                case 3: goto oid_2_16_840_1_113730_3;
                case 4: goto oid_2_16_840_1_113730_4;
                case 5: return $"{oidPath}/[Certificate server]";
                case 6: goto oid_2_16_840_1_113730_6;
                case 7: goto oid_2_16_840_1_113730_7;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // cert-ext
        #region 2.16.840.1.113730.1.*

        oid_2_16_840_1_113730_1:

            oidPath += "/[Netscape certificate extensions within Rec. ITU-T X.509 version 3 certificates]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: return $"{oidPath}/[cert-type]";
                case 2: return $"{oidPath}/[base-url]";
                case 3: return $"{oidPath}/[revocation-url]";
                case 4: return $"{oidPath}/[ca-revocation-url]";
                case 5: return $"{oidPath}/[ca-crl-url]";
                case 6: return $"{oidPath}/[ca-cert-url]";
                case 7: return $"{oidPath}/[renewal-url]";
                case 8: return $"{oidPath}/[ca-policy-url]";
                case 9: return $"{oidPath}/[homepage-url]";
                case 10: return $"{oidPath}/[entity-logo]";
                case 11: return $"{oidPath}/[user-picture]";
                case 12: return $"{oidPath}/[ssl-server-name]";
                case 13: return $"{oidPath}/[comment]";
                case 14: return $"{oidPath}/[lost-password-url]";
                case 15: return $"{oidPath}/[cert-renewal-time]";
                case 16: goto oid_2_16_840_1_113730_1_16;
                case 17: return $"{oidPath}/[cert-scope-of-use]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // aia
        #region 2.16.840.1.113730.1.16.*

        oid_2_16_840_1_113730_1_16:

            oidPath += "/[aia]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[cert-renewal]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // data-type
        #region 2.16.840.1.113730.2.*

        oid_2_16_840_1_113730_2:

            oidPath += "/[Netscape data types]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[gif]",
                2 => $"{oidPath}/[jpeg]",
                3 => $"{oidPath}/[url]",
                4 => $"{oidPath}/[html]",
                5 => $"{oidPath}/[cert-sequence]",
                6 => $"{oidPath}/[netscape-cert-url]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // directory
        #region 2.16.840.1.113730.3.*

        oid_2_16_840_1_113730_3:

            oidPath += "/[Netscape Lightweight Directory Access Protocol (LDAP)]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_16_840_1_113730_3_1;
                case 2: goto oid_2_16_840_1_113730_3_2;
                case 3: goto oid_2_16_840_1_113730_3_3;
                case 4: goto oid_2_16_840_1_113730_3_4;
                case 5: goto oid_2_16_840_1_113730_3_5;
                case 6: goto oid_2_16_840_1_113730_3_6;
                case 8: return $"{oidPath}/[Identity, Policy, Audit (IPA)]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // 1
        #region 2.16.840.1.113730.3.1.*

        oid_2_16_840_1_113730_3_1:

            oidPath += "/[attributes]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[carLicense]",
                2 => $"{oidPath}/[departmentNumber]",
                3 => $"{oidPath}/[employeeNumber]",
                4 => $"{oidPath}/[employeeType]",
                5 => $"{oidPath}/[changeNumber]",
                6 => $"{oidPath}/[targetDN]",
                7 => $"{oidPath}/[changeType]",
                8 => $"{oidPath}/[changes]",
                9 => $"{oidPath}/[newRdn]",
                10 => $"{oidPath}/[deleteOldRdn]",
                11 => $"{oidPath}/[newSuperior]",
                12 => $"{oidPath}/[mailAccessDomain]",
                14 => $"{oidPath}/[mailAutoReplyMode]",
                15 => $"{oidPath}/[mailAutoReplyText]",
                17 => $"{oidPath}/[mailForwardingAddress]",
                18 => $"{oidPath}/[mailHost]",
                34 => $"{oidPath}/[ref]",
                35 => $"{oidPath}/[changeLog]",
                36 => $"{oidPath}/[nsLicensedFor]",
                39 => $"{oidPath}/[preferredLanguage]",
                40 => $"{oidPath}/[userSMIMECertificate]",
                47 => $"{oidPath}/[mailRoutingAddress]",
                55 => $"{oidPath}/[aci]",
                77 => $"{oidPath}/[changeTime]",
                198 => $"{oidPath}/[memberURL]",
                216 => $"{oidPath}/[userPKCS12]",
                241 => $"{oidPath}/[displayName]",
                542 => $"{oidPath}/[nsUniqueId]",
                692 => $"{oidPath}/[inetUserStatus]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // 2
        #region 2.16.840.1.113730.3.2.*

        oid_2_16_840_1_113730_3_2:

            oidPath += "/[Object classes]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[changeLogEntry]",
                2 => $"{oidPath}/[inetOrgPerson]",
                6 => $"{oidPath}/[referral]",
                33 => $"{oidPath}/[groupOfURLs]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // 3
        #region 2.16.840.1.113730.3.3.*

        oid_2_16_840_1_113730_3_3:

            oidPath += "/[Matching rules]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                2 => $"{oidPath}/[Locales]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // 4
        #region 2.16.840.1.113730.3.4.*

        oid_2_16_840_1_113730_3_4:

            oidPath += "/[Version 3 controls]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                2 => $"{oidPath}/[manageDsaIT]",
                3 => $"{oidPath}/[Persistent search]",
                4 => $"{oidPath}/[Netscape password expired]",
                5 => $"{oidPath}/[Netscape password expiring]",
                6 => $"{oidPath}/[Netscape NT synchronization client]",
                7 => $"{oidPath}/[Entry change notification]",
                8 => $"{oidPath}/[Transaction ID request]",
                9 => $"{oidPath}/[Virtual List View (VLV) providing partial results to a search rather than returning all resulting entries at once]",
                10 => $"{oidPath}/[Virtual List View (VLV) response]",
                11 => $"{oidPath}/[Transaction ID response]",
                12 => $"{oidPath}/[Proxied authorization (old specification) allowing the client to assume another identity for the duration of a request]",
                13 => $"{oidPath}/[iPlanet directory server replication update information]",
                14 => $"{oidPath}/[Search on specific database]",
                15 => $"{oidPath}/[Authentication response]",
                16 => $"{oidPath}/[Authentication identity request]",
                17 => $"{oidPath}/[Real attribute only]",
                18 => $"{oidPath}/[Proxied authorization (new version 2 specification) allowing the client to assume another identity for the duration of a request]",
                19 => $"{oidPath}/[Virtual attributes only]",
                999 => $"{oidPath}/[iPlanet Replication Modrdn Extra Mods]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // 5
        #region 2.16.840.1.113730.3.5.*

        oid_2_16_840_1_113730_3_5:

            oidPath += "/[Version 3 extended operations]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Transaction request]",
                2 => $"{oidPath}/[Transaction response]",
                3 => $"{oidPath}/[iPlanet start replication request]",
                4 => $"{oidPath}/[iPlanet replication response]",
                5 => $"{oidPath}/[iPlanet end replication request]",
                6 => $"{oidPath}/[iPlanet replication entry request]",
                7 => $"{oidPath}/[iPlanet bulk import start]",
                8 => $"{oidPath}/[iPlanet bulk import finished]",
                9 => $"{oidPath}/[iPlanet digest authentication calculation]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // 6
        #region 2.16.840.1.113730.3.6.*

        oid_2_16_840_1_113730_3_6:

            oidPath += "/[Miscellaneous]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[iPlanet incremental update replication protocol identifier]",
                2 => $"{oidPath}/[iPlanet total update replication protocol identifier]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // policy
        #region 2.16.840.1.113730.4.*

        oid_2_16_840_1_113730_4:

            oidPath += "/[Netscape policy types]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Netscape Server Gated Crypto (SGC)]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // algs
        #region 2.16.840.1.113730.6.*

        oid_2_16_840_1_113730_6:

            oidPath += "/[Algorithm identifiers]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Netscape S/MIME KEA]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // name-components
        #region 2.16.840.1.113730.7.*

        oid_2_16_840_1_113730_7:

            oidPath += "/[Name components]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[nickname]",
                2 => $"{oidPath}/[aol-screenname]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // digicert, verisign, symantec
        #region 2.16.840.1.113733.*

        oid_2_16_840_1_113733:

            oidPath += "/[DigiCert, Inc (previously, Symantec Corporation and Verisign, Inc.)]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_16_840_1_113733_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // pki
        #region 2.16.840.1.113733.1.*

        oid_2_16_840_1_113733_1:

            oidPath += "/[Public-Key Infrastructure (PKI)]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 6: goto oid_2_16_840_1_113733_1_6;
                case 7: goto oid_2_16_840_1_113733_1_7;
                case 8: goto oid_2_16_840_1_113733_1_8;
                case 9: goto oid_2_16_840_1_113733_1_9;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // extensions
        #region 2.16.840.1.113733.1.6.*

        oid_2_16_840_1_113733_1_6:

            oidPath += "/[VeriSign defined certificate extension sub tree]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                3 => $"{oidPath}/[Unknown Verisign extension]",
                6 => $"{oidPath}/[Unknown Verisign extension]",
                7 => $"{oidPath}/[VeriSign serial number rollover class]",
                11 => $"{oidPath}/[verisignOnsiteJurisdictionHash]",
                13 => $"{oidPath}/[Unknown Verisign VPN extension]",
                15 => $"{oidPath}/[verisignServerID]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // policies
        #region 2.16.840.1.113733.1.7.*

        oid_2_16_840_1_113733_1_7:

            oidPath += "/[Policy documentation]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 9: return $"{oidPath}/[British Telecommunications plc (BT) trust services relying third party charters]";
                case 21: return $"{oidPath}/[Policy identifier]";
                case 23: goto oid_2_16_840_1_113733_1_7_23;
                case 46: goto oid_2_16_840_1_113733_1_7_46;
                case 48: goto oid_2_16_840_1_113733_1_7_48;
                case 54: return $"{oidPath}/[Symantec Reserved certificate policy (Symantec/id-CABF-OVandDVvalidation)]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // vtn-cp
        #region 2.16.840.1.113733.1.7.23.*

        oid_2_16_840_1_113733_1_7_23:

            oidPath += "/[VeriSign Trust Network - Certificate Policies]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: return $"{oidPath}/[Certificate Policy (CP) for class 1 certificates]";
                case 2: return $"{oidPath}/[Certificate Policy (CP) for class 2 certificates]";
                case 3: goto oid_2_16_840_1_113733_1_7_23_3;
                case 4: return $"{oidPath}/[Certificate Policy (CP) for class 4 certificates]";
                case 6: return $"{oidPath}/[Verisign Certification Policy for Extended Validation (EV) certificates]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // class3
        #region 2.16.840.1.113733.1.7.23.3.*

        oid_2_16_840_1_113733_1_7_23_3:

            oidPath += "/[Certificate Policy (CP) for class 3 certificates]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_16_840_1_113733_1_7_23_3_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // class3
        #region 2.16.840.1.113733.1.7.23.3.1.*

        oid_2_16_840_1_113733_1_7_23_3_1:

            oidPath += "/[Non-federal Certification Practice Statement (CPS)]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                6 => $"{oidPath}/[non-federal-SSP-Medium]",
                7 => $"{oidPath}/[non-federal-SSP-MediumHardware]",
                8 => $"{oidPath}/[non-federal-SSP-Devices]",
                13 => $"{oidPath}/[non-federal-SSP-Auth]",
                14 => $"{oidPath}/[non-federal-SSP-Medium-CBP]",
                15 => $"{oidPath}/[non-federal-SSP-MediumHardware-CBP]",
                23 => $"{oidPath}/[class-3-VTN-SSP-Medium-SHA1]",
                24 => $"{oidPath}/[class-3-VTN-SSP-MediumHardware-SHA1]",
                25 => $"{oidPath}/[class-3-VTN-SSP-Devices-SHA1]",
                26 => $"{oidPath}/[class-3-VTN-SSP-PIV-I-Auth-SHA1]",
                27 => $"{oidPath}/[class-3-VTN-SSP-PIV-I-CardAuth-SHA1]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        // cis
        #region 2.16.840.1.113733.1.7.46.*

        oid_2_16_840_1_113733_1_7_46:

            oidPath += "/[Certificate Interoperability Service (CIS) supplemental policies]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Type 1 policy]",
                2 => $"{oidPath}/[Type 2 policy]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // 48
        #region 2.16.840.1.113733.1.7.48.*

        oid_2_16_840_1_113733_1_7_48:

            oidPath += "/[Thawte Premium Server Certification Authority]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Thawte Extended Validation (EV) Certification Practice Statement (CPS) v. 3.3]",
                2 => $"{oidPath}/[Thawte certificates (without Extended Validation)]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // 8
        #region 2.16.840.1.113733.1.8.*

        oid_2_16_840_1_113733_1_8:

            oidPath += "/[Server Gated Crypto (SGC) identifier for Certification Authority (CA) certificates]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[VeriSign Server Gated Crypto (SGC)]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // attributes
        #region 2.16.840.1.113733.1.9.*

        oid_2_16_840_1_113733_1_9:

            oidPath += "/[Attributes]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                2 => $"{oidPath}/[messageType]",
                3 => $"{oidPath}/[pkiStatus]",
                4 => $"{oidPath}/[failInfo]",
                5 => $"{oidPath}/[senderNonce]",
                6 => $"{oidPath}/[recipientNonce]",
                7 => $"{oidPath}/[transactionID]",
                8 => $"{oidPath}/[extensionReq]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        // intel
        #region 2.16.840.1.113741.*

        oid_2_16_840_1_113741:

            oidPath += "/[Intel Corporation]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 2: goto oid_2_16_840_1_113741_2;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // cdsa-security
        #region 2.16.840.1.113741.2.*

        oid_2_16_840_1_113741_2:

            oidPath += "/[Common Data Security Architecture (CDSA) security]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_16_840_1_113741_2_1;
                case 2: goto oid_2_16_840_1_113741_2_2;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // formats
        #region 2.16.840.1.113741.2.1.*

        oid_2_16_840_1_113741_2_1:

            oidPath += "/[formats]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_16_840_1_113741_2_1_1;
                case 4: goto oid_2_16_840_1_113741_2_1_4;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // 1
        #region 2.16.840.1.113741.2.1.1.*

        oid_2_16_840_1_113741_2_1_1:

            oidPath += "/[INTEL_X509V3_CERT_R08]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[X509V3TbsCertificate]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // bundle
        #region 2.16.840.1.113741.2.1.4.*

        oid_2_16_840_1_113741_2_1_4:

            oidPath += "/[Bundles]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[INTEL_CERT_AND_PRIVATE_KEY_2_0]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // algs
        #region 2.16.840.1.113741.2.2.*

        oid_2_16_840_1_113741_2_2:

            oidPath += "/[Algorithms]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                5 => $"{oidPath}/[Security algorithms]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        // hl7
        #region 2.16.840.1.113883.*

        oid_2_16_840_1_113883:

            oidPath += "/[Health Level 7 (HL7), Inc.]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_16_840_1_113883_1;
                case 2: goto oid_2_16_840_1_113883_2;
                //TODO: case 3: goto oid_2_16_840_1_113883_3;
                //TODO: case 4: goto oid_2_16_840_1_113883_4;
                //TODO: case 5: goto oid_2_16_840_1_113883_5;
                //TODO: case 6: goto oid_2_16_840_1_113883_6;
                //TODO: case 7: goto oid_2_16_840_1_113883_7;
                //TODO: case 8: goto oid_2_16_840_1_113883_8;
                //TODO: case 9: goto oid_2_16_840_1_113883_9;
                //TODO: case 10: goto oid_2_16_840_1_113883_10;
                //TODO: case 11: goto oid_2_16_840_1_113883_11;
                //TODO: case 12: goto oid_2_16_840_1_113883_12;
                //TODO: case 13: goto oid_2_16_840_1_113883_13;
                //TODO: case 14: goto oid_2_16_840_1_113883_14;
                //TODO: case 15: goto oid_2_16_840_1_113883_15;
                //TODO: case 17: goto oid_2_16_840_1_113883_17;
                //TODO: case 18: goto oid_2_16_840_1_113883_18;
                //TODO: case 19: goto oid_2_16_840_1_113883_19;
                //TODO: case 21: goto oid_2_16_840_1_113883_21;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // internalHL7objects
        #region 2.16.840.1.113883.1.*

        oid_2_16_840_1_113883_1:

            oidPath += "/[Internal Objects]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: return $"{oidPath}/[OID registered objects]";
                case 2: return $"{oidPath}/[Common Message Element Types (CMETs)]";
                case 3: return $"{oidPath}/[Refined Message Information Models (RMIMs)]";
                case 4: return $"{oidPath}/[RIM Classes]";
                case 5: return $"{oidPath}/[RIM Attributes]";
                case 6: return $"{oidPath}/[Interactions]";
                case 7: goto oid_2_16_840_1_113883_1_7;
                case 8: return $"{oidPath}/[BRIDG Domain Access Model]";
                case 9: return $"{oidPath}/[International V3 Release]";
                case 11: goto oid_2_16_840_1_113883_1_11;
                case 18: return $"{oidPath}/[Trigger event]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // structured-Document-HMDs
        #region 2.16.840.1.113883.1.7.*

        oid_2_16_840_1_113883_1_7:

            oidPath += "/[Hierarchical Message Descriptions (HMDs) for balloted Structured Documents releases]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Clinical Document Architecture (CDA) release 1]",
                2 => $"{oidPath}/[Clinical Document Architecture (CDA) release 2]",
                3 => $"{oidPath}/[Hierarchical Message Description (HMD) for Structured Product Labeling (SPL) Release 1]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // harmonizationValueSets
        #region 2.16.840.1.113883.1.11.*

        oid_2_16_840_1_113883_1_11:

            oidPath += "/[V3 Harmonization Value Sets]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: return $"{oidPath}/[Administrative gender]";
                case 20: goto oid_2_16_840_1_113883_1_11_20;
                case 78: return $"{oidPath}/[Result normalcy status]";
                case 10228: return $"{oidPath}/[Confidentiality]";
                case 10416: return $"{oidPath}/[Financially responsible party type]";
                case 11526: return $"{oidPath}/[Language]";
                case 12212: return $"{oidPath}/[Marital status]";
                case 12249: return $"{oidPath}/[LanguageAbilityMode]";
                case 14914: return $"{oidPath}/[Race]";
                case 15836: return $"{oidPath}/[Ethnicity]";
                case 18877: return $"{oidPath}/[Coverage role type]";
                case 19185: return $"{oidPath}/[Religious affiliation]";
                case 19563: return $"{oidPath}/[Personal relationship role type]";
                case 19579: return $"{oidPath}/[Family member]";
                case 19717: return $"{oidPath}/[No immunization reason]";
                case 159331: return $"{oidPath}/[actStatus-incorrect]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // 20
        #region 2.16.840.1.113883.1.11.20.*

        oid_2_16_840_1_113883_1_11_20:

            oidPath += "/[SDTC]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                2 => $"{oidPath}/[Advance directive type]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        // affiliate
        #region 2.16.840.1.113883.2.*

        oid_2_16_840_1_113883_2:

            oidPath += "/[Affiliate organizations]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_16_840_1_113883_2_1;
                case 2: return $"{oidPath}/[Japan]";
                //TODO: case 3: goto oid_2_16_840_1_113883_2_3;
                //TODO: case 4: goto oid_2_16_840_1_113883_2_4;
                case 5: return $"{oidPath}/[Switzerland]";
                //TODO: case 6: goto oid_2_16_840_1_113883_2_6;
                case 7: return $"{oidPath}/[Croatia]";
                case 8: return $"{oidPath}/[France Harmoniser et PRomouvoir l'Informatique Médicale (HPRIM)]";
                case 9: return $"{oidPath}/[Italy]";
                case 10: return $"{oidPath}/[Argentina]";
                case 11: return $"{oidPath}/[Lithuania]";
                case 13: return $"{oidPath}/[National Council for Prescription Drug Programs (NCPDP) standard product billing code of NCPDP field Unit of Measure (600-28)]";
                case 14: return $"{oidPath}/[Uruguay]";
                case 15: return $"{oidPath}/[Malaysia]";
                case 16: return $"{oidPath}/[Austria (formerly Anwendergruppe Österreich)]";
                case 17: return $"{oidPath}/[Columbia]";
                case 18: return $"{oidPath}/[New Zealand]";
                //TODO: case 19: goto oid_2_16_840_1_113883_2_19;
                //TODO: case 20: goto oid_2_16_840_1_113883_2_20;
                //TODO: case 21: goto oid_2_16_840_1_113883_2_21;
                //TODO: case 22: goto oid_2_16_840_1_113883_2_22;
                //TODO: case 23: goto oid_2_16_840_1_113883_2_23;
                //TODO: case 24: goto oid_2_16_840_1_113883_2_24;
                case 25: return $"{oidPath}/[Greece]";
                case 26: return $"{oidPath}/[India]";
                case 27: return $"{oidPath}/[Ireland]";
                case 28: return $"{oidPath}/[Korea]";
                case 29: return $"{oidPath}/[Mexico]";
                case 30: return $"{oidPath}/[Romania]";
                case 31: return $"{oidPath}/[Singapore]";
                case 32: return $"{oidPath}/[Sweden]";
                case 33: return $"{oidPath}/[Taiwan]";
                case 34: return $"{oidPath}/[Turkey]";
                case 35: return $"{oidPath}/[Russia]";
                case 36: return $"{oidPath}/[Pakistan]";
                case 37: return $"{oidPath}/[Bosnia and Herzegovina]";
                case 38: return $"{oidPath}/[Mexico]";
                case 39: return $"{oidPath}/[Luxembourg]";
                //TODO: case 40: goto oid_2_16_840_1_113883_2_40;
                case 41: return $"{oidPath}/[Hong Kong]";
                case 42: return $"{oidPath}/[Norway]";
                case 43: return $"{oidPath}/[Puerto Rico]";
                case 44: return $"{oidPath}/[Philippines]";
                case 45: return $"{oidPath}/[Malaysia]";
                case 46: return $"{oidPath}/[Slovenia]";
                case 47: return $"{oidPath}/[Serbia]";
                case 48: return $"{oidPath}/[Poland]";
                case 49: return $"{oidPath}/[Ukraine]";
                case 50: return $"{oidPath}/[Belgium]";
                case 51: return $"{oidPath}/[Europe]";
                case 52: return $"{oidPath}/[Portugal]";
                //TODO: case 86: goto oid_2_16_840_1_113883_2_86;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // 1
        #region 2.16.840.1.113883.2.1.*

        oid_2_16_840_1_113883_2_1:

            oidPath += "/[UK]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 3: goto oid_2_16_840_1_113883_2_1_3;
                //TODO: case 4: goto oid_2_16_840_1_113883_2_1_4;
                case 5: return $"{oidPath}/[Reserved for future use]";
                //TODO: case 6: goto oid_2_16_840_1_113883_2_1_6;
                case 7: return $"{oidPath}/[National Patient Safety Agency (NPSA) patient safety]";
                //TODO: case 8: goto oid_2_16_840_1_113883_2_1_8;
                case 9: return $"{oidPath}/[v2 vocabularies]";
                case 10: return $"{oidPath}/[National Health Service (NHS) Scotland]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // 3
        #region 2.16.840.1.113883.2.1.3.*

        oid_2_16_840_1_113883_2_1_3:

            oidPath += "/[UK coding systems]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: return $"{oidPath}/[IDX]";
                //TODO: case 2: goto oid_2_16_840_1_113883_2_1_3_2;
                case 3: return $"{oidPath}/[UK Biobank]";
                //TODO: case 4: goto oid_2_16_840_1_113883_2_1_3_4;
                case 7: return $"{oidPath}/[iSoft PLC]";
                case 8: return $"{oidPath}/[National Institute of Health Research (NIHR)]";
                case 9: return $"{oidPath}/[Sintero]";
                case 10: return $"{oidPath}/[Regional Health and Social Care in Northern Ireland]";
                case 11: return $"{oidPath}/[Imperial College London]";
                case 12: return $"{oidPath}/[Great Ormond Street Hospital for Children National Health Service (NHS) Trust]";
                case 13: return $"{oidPath}/[Ashford & St Peter's Hospitals National Health Service (NHS) Foundation Trust]";
                case 14: return $"{oidPath}/[Brighton & Sussex University Hospitals National Health Service (NHS) Trust]";
                case 15: return $"{oidPath}/[East Sussex Healthcare National Health Service (NHS) Trust]";
                case 16: return $"{oidPath}/[Queen Victoria Hospital National Health Service (NHS) Foundation Trust]";
                case 17: return $"{oidPath}/[Royal Surrey County Hospital National Health Service (NHS) Foundation Trust]";
                case 18: return $"{oidPath}/[Western Sussex Hospitals National Health Service (NHS) Trust]";
                case 19: return $"{oidPath}/[Isle Of Wight National Health Service (NHS) Trust]";
                case 20: return $"{oidPath}/[Southern Health National Health Service (NHS) Foundation Trust]";
                case 21: return $"{oidPath}/[University Hospital Southampton National Health Service (NHS) Foundation Trust]";
                case 22: return $"{oidPath}/[Salisbury National Health Service (NHS) Foundation Trust]";
                case 23: return $"{oidPath}/[Portsmouth Hospitals Trust]";
                case 24: return $"{oidPath}/[Burnbank Systems Ltd]";
                case 25: return $"{oidPath}/[National Health Service (NHS) Lothian]";
                case 26: return $"{oidPath}/[City of Edinburgh Council]";
                case 27: return $"{oidPath}/[East Lothian Council]";
                case 28: return $"{oidPath}/[Mid Lothian Council]";
                case 29: return $"{oidPath}/[West Lothian Council]";
                case 30: return $"{oidPath}/[South Essex Partnership University National Health Service (NHS) Foundation Trust]";
                case 31: return $"{oidPath}/[Lancashire's Patient Record Exchange Service - Health Information Exchange Platform for Lancashire]";
                case 32: return $"{oidPath}/[West Suffolk Hospital National Health Service (NHS) Foundation Trust]";
                case 33: return $"{oidPath}/[Connecting Care (requested by Orion Health Ltd)]";
                case 34: return $"{oidPath}/[University College London Hospitals National Health Service (NHS) Foundation Trust]";
                case 35: return $"{oidPath}/[Tameside Hospital National Health Service (NHS) Foundation Trust (where RMP is the Trust's nationally recognised code)]";
                case 36: return $"{oidPath}/[Royal Marsden Hospital Foundation Trust]";
                case 37: return $"{oidPath}/[Lewisham and Greenwich National Health Service (NHS) Trust]";
                case 38: return $"{oidPath}/[West Middlesex Hospital]";
                case 39: return $"{oidPath}/[Chelsea and Westminster Hospital]";
                case 40: return $"{oidPath}/[Guy's And St Thomas' National Health Service (NHS) Foundation Trust]";
                case 41: return $"{oidPath}/[Hillingdon Hospitals National Health Service (NHS) Foundation Trust]";
                case 42: return $"{oidPath}/[Imperial College Healthcare National Health Service (NHS) Trust]";
                case 43: return $"{oidPath}/[Essex Partnership University National Health Service (NHS) Foundation Trust]";
                case 44: return $"{oidPath}/[County Durham and Darlington National Health Service (NHS) Foundation Trust]";
                case 45: return $"{oidPath}/[Milton Keynes University Hospital National Health Service (NHS) Foundation Trust]";
                case 46: return $"{oidPath}/[National Health Service (NHS) England London Region]";
                case 47: return $"{oidPath}/[Sandwell & West Birmingham Hospitals National Health Service (NHS) Trust]";
                case 48: return $"{oidPath}/[Croydon Health Services National Health Service (NHS) Trust]";
                case 49: return $"{oidPath}/[Alder Hey Children's National Health Service (NHS) Foundation Trust]";
                case 50: return $"{oidPath}/[Royal Liverpool and Broadgreen University Hospitals Trust]";
                case 51: return $"{oidPath}/[Liverpool Heart & Chest Hospital National Health Service (NHS) Trust]";
                case 52: return $"{oidPath}/[Merseycare National Health Service (NHS) Trust]";
                case 53: return $"{oidPath}/[Kingston Hospital National Health Service (NHS) Foundation Trust]";
                case 54: return $"{oidPath}/[The Clatterbridge Cancer Centre National Health Service (NHS) Foundation Trust]";
                case 55: return $"{oidPath}/[St Helens and Knowsley Teaching Hospitals National Health Service (NHS) Trust]";
                case 56: return $"{oidPath}/[Epsom and St Helier University Hospitals National Health Service (NHS) Trust]";
                case 57: return $"{oidPath}/[OUH National Health Service (NHS) Foundation Trust]";
                case 58: return $"{oidPath}/[London North West University Healthcare National Health Service (NHS) Trust]";
                case 59: return $"{oidPath}/[InHealth Group]";
                case 60: return $"{oidPath}/[Dorset Care Record Partnership]";
                case 61: return $"{oidPath}/[East Lancashire Hospitals National Health Service (NHS) Trust]";
                case 62: return $"{oidPath}/[Newcastle Hospitals National Health Service (NHS) Foundation Trust interoperability for Great North Care Record]";
                case 63: return $"{oidPath}/[South Tyneside and Sunderland National Health Service (NHS) Foundation Trust]";
                case 64: return $"{oidPath}/[Liverpool Women's National Health Service (NHS) Foundation Trust]";
                case 65: return $"{oidPath}/[Sunderland City Council interoperability for Great North Care Record]";
                case 66: return $"{oidPath}/[North Tees and Hartlepool National Health Service (NHS) Foundation Trust]";
                case 67: return $"{oidPath}/[Gateshead Health National Health Service (NHS) Foundation Trust]";
                case 68: return $"{oidPath}/[North West London Radiology Network]";
                case 69: return $"{oidPath}/[Aintree University Hospital National Health Service (NHS) Foundation Trust]";
                case 70: return $"{oidPath}/[South Tees Hospitals National Health Service (NHS) Foundation Trust]";
                case 71: return $"{oidPath}/[Warrington & Halton Teaching Hospital National Health Service (NHS) Foundation Trust]";
                case 72: return $"{oidPath}/[Tees, Esk and Wear Valleys National Health Service (NHS) Foundation Trust]";
                case 73: return $"{oidPath}/[Greater Manchester Combined Authority, Health and Social Care]";
                case 74: return $"{oidPath}/[Chelsea and Westminster Hospital National Health Service (NHS) Foundation Trust]";
                case 75: return $"{oidPath}/[North West Boroughs Healthcare National Health Service (NHS) Foundation Trust]";
                case 76: return $"{oidPath}/[Mid and South Essex National Health Service (NHS) Foundation Trust]";
                // TODO: Left off at https://oid-base.com/get/2.16.840.1.113883.2.1.3.76
                // case 1: return $"{oidPath}/[XXXXX]";
                // case 1: return $"{oidPath}/[XXXXX]";
                // case 1: return $"{oidPath}/[XXXXX]";
                // case 1: return $"{oidPath}/[XXXXX]";
                // case 1: return $"{oidPath}/[XXXXX]";
                // case 1: return $"{oidPath}/[XXXXX]";
                // case 1: return $"{oidPath}/[XXXXX]";
                // case 1: return $"{oidPath}/[XXXXX]";
                // case 1: return $"{oidPath}/[XXXXX]";
                // case 1: return $"{oidPath}/[XXXXX]";
                // case 1: return $"{oidPath}/[XXXXX]";
                // case 1: return $"{oidPath}/[XXXXX]";
                // case 1: return $"{oidPath}/[XXXXX]";
                // case 1: return $"{oidPath}/[XXXXX]";
                // case 1: return $"{oidPath}/[XXXXX]";
                // case 1: return $"{oidPath}/[XXXXX]";
                // case 1: return $"{oidPath}/[XXXXX]";
                // case 1: return $"{oidPath}/[XXXXX]";
                // case 1: return $"{oidPath}/[XXXXX]";
                // case 1: return $"{oidPath}/[XXXXX]";
                // case 1: return $"{oidPath}/[XXXXX]";
                // case 1: return $"{oidPath}/[XXXXX]";
                // case 1: return $"{oidPath}/[XXXXX]";
                // case 1: return $"{oidPath}/[XXXXX]";
                // case 1: return $"{oidPath}/[XXXXX]";
                // case 1: return $"{oidPath}/[XXXXX]";
                // case 1: return $"{oidPath}/[XXXXX]";
                // case 1: return $"{oidPath}/[XXXXX]";
                // case 1: return $"{oidPath}/[XXXXX]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        #endregion

        #endregion

        #endregion

        #endregion

        #endregion

        // uy
        #region 2.16.858.*

        oid_2_16_858:

            oidPath += "/UY";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_2_16_858_0;
                case 1: return $"{oidPath}/[Personas dentro del territorio uruguayo]";
                case 2: return $"{oidPath}/[Todo tangible o intangible, técnicamente viable de ser identificado como unidad, capaz de constituir grupos y por ende de contabilizarse]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // uy
        #region 2.16.858.0.*

        oid_2_16_858_0:

            oidPath += "/[Todas las organizaciones públicas y privadas]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Instituciones Públicas del Estado Uruguayo]",
                1 => $"{oidPath}/[Instituciones Públicas que no pertenecen al Estado Uruguayo]",
                2 => $"{oidPath}/[Empresas u organizaciones privadas de todo tipo]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // 886
        #region 2.16.886.*

        oid_2_16_886:

            oidPath += "/[Yemen (code not in current use)]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_16_886_1;
                case 2: return $"{oidPath}/[Computer & Communications Research Lab. of Industrial Technology Research Institute]";
                case 101: goto oid_2_16_886_101;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // illegal
        #region 2.16.886.1.*

        oid_2_16_886_1:

            oidPath += "/[Chunghaw Telecom co.]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_16_886_1_1;
                case 2: goto oid_2_16_886_1_2;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // illegal, id
        #region 2.16.886.1.1.*

        oid_2_16_886_1_1:

            oidPath += "/[Legal entities]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Personal Identification Number]",
                2 => $"{oidPath}/[Private organization ID registered in Taiwan]",
                3 => $"{oidPath}/[Public organization ID registered in Taiwan]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // cp-illegal, cp
        #region 2.16.886.1.2.*

        oid_2_16_886_1_2:

            oidPath += "/[Policies that Chunghwa Telecom would make public]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Taiwan Government Root Certificate Authority (GRCA) policies]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // illegal-gov, gov
        #region 2.16.886.101.*

        oid_2_16_886_101:

            oidPath += "/[Government root certification authority of Taiwan]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_2_16_886_101_0;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // gpki-illegal, gpki
        #region 2.16.886.101.0.*

        oid_2_16_886_101_0:

            oidPath += "/[Government Public Key Infrastructure (PKI)]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 3: goto oid_2_16_886_101_0_3;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // certpolicy-illegal, certpolicy
        #region 2.16.886.101.0.3.*

        oid_2_16_886_101_0_3:

            oidPath += "/[Certificate policy]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[testAssurance-illegal]",
                1 => $"{oidPath}/[class1Assurance-illegal]",
                2 => $"{oidPath}/[class2Assurance-illegal]",
                3 => $"{oidPath}/[class3Assurance-illegal]",
                4 => $"{oidPath}/[class4Assurance-illegal]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        #endregion

        #endregion

        // registration-procedures
        #region 2.17.*

        oid_2_17:

            oidPath += "/Registration-Procedures";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_17_1;
                case 2: goto oid_2_17_2;
                case 3: return $"{oidPath}/[Registration procedures for the registration authority of international ASN.1 names]";
                case 5: return $"{oidPath}/[Registration procedures for the registration authority of international ADministration Management Domain (ADMD) alphanumeric names and international PRivate Management Domain (PRMD) alphanumeric names for Originator/Recipient (O/R) Rec. ITU-T X.400 addresses]";
                case 6: return $"{oidPath}/[Registration procedures for the registration authority of international organization alphanumeric names for use in Rec. ITU-T X.500 directory distinguished names]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // module
        #region 2.17.1.*

        oid_2_17_1:

            oidPath += "/[ASN.1 modules]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: return $"{oidPath}/[OidDirectoryNameDef]";
                case 2: goto oid_2_17_1_2;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // directory-defs
        #region 2.17.1.2.*

        oid_2_17_1_2:

            oidPath += "/[Information objects defined in ASN.1 module named OidDirectoryNameDef]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[oidC1]",
                1 => $"{oidPath}/[oidC2]",
                2 => $"{oidPath}/[oidC]",
                3 => $"{oidPath}/[oidRoot]",
                4 => $"{oidPath}/[oidRootNf]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // document-types
        #region 2.17.2.*

        oid_2_17_2:

            oidPath += "/[Document types]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                3 => $"{oidPath}/[Third registered instance of the Document Type information object as described in clause A.4 of ISO/IEC 9834-2]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // physical-layer, physical-layer-management
        #region 2.18.*

        oid_2_18:

            oidPath += "/[Physical layer management]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_2_18_0;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // management
        #region 2.18.0.*

        oid_2_18_0:

            oidPath += $"{oidPath}";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: return $"{oidPath}/[standardSpecificExtension]";
                case 2: goto oid_2_18_0_2;
                case 3: goto oid_2_18_0_3;
                case 4: return $"{oidPath}/[package]";
                case 5: goto oid_2_18_0_5;
                case 6: goto oid_2_18_0_6;
                case 7: goto oid_2_18_0_7;
                case 8: return $"{oidPath}/[attributeGroup]";
                case 9: return $"{oidPath}/[action]";
                case 10: return $"{oidPath}/[notification]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // asn1Module
        #region 2.18.0.2.*

        oid_2_18_0_2:

            oidPath += "/[ASN.1 modules]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[PHLM]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // objectClass
        #region 2.18.0.3.*

        oid_2_18_0_3:

            oidPath += "/[Object classes]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[physicalSubsystem]",
                2 => $"{oidPath}/[physicalEntity]",
                3 => $"{oidPath}/[physicalSAP]",
                4 => $"{oidPath}/[dataCircuit]",
                5 => $"{oidPath}/[physicalConnection]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // parameter
        #region 2.18.0.5.*

        oid_2_18_0_5:

            oidPath += "/[Parameters]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[bitErrorThresholdReached]",
                2 => $"{oidPath}/[connectionError]",
                3 => $"{oidPath}/[connectionEstablished]",
                4 => $"{oidPath}/[lossOfSignal]",
                5 => $"{oidPath}/[lossOfSynchronization]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // nameBinding
        #region 2.18.0.6.*

        oid_2_18_0_6:

            oidPath += "/[Name bindings]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[physicalSubsystem-system]",
                2 => $"{oidPath}/[physicalEntity-physicalSubsystem-Management]",
                3 => $"{oidPath}/[physicalSAP-physicalSubsystem]",
                4 => $"{oidPath}/[dataCircuit-physicalEntity]",
                5 => $"{oidPath}/[physicalConnection-dataCircuit]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // attribute
        #region 2.18.0.7.*

        oid_2_18_0_7:

            oidPath += "/[Attributes]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[physicalEntityTitles]",
                2 => $"{oidPath}/[bitErrorsThreshold]",
                3 => $"{oidPath}/[dataCircuitType]",
                4 => $"{oidPath}/[physicalInterfaceStandard]",
                5 => $"{oidPath}/[physicalInterfaceType]",
                6 => $"{oidPath}/[physicalMediaNames]",
                7 => $"{oidPath}/[synchronizationMode]",
                8 => $"{oidPath}/[transmissionCoding]",
                9 => $"{oidPath}/[transmissionMode]",
                10 => $"{oidPath}/[transmissionRate]",
                11 => $"{oidPath}/[endpointIdentifier]",
                12 => $"{oidPath}/[portNumber]",
                13 => $"{oidPath}/[bitErrorsReceived]",
                14 => $"{oidPath}/[bitErrorsTransmitted]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        // mheg
        #region 2.19.*

        oid_2_19:

            oidPath += "/[Multimedia and Hypermedia information coding Expert Group (MHEG)]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_19_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // version
        #region 2.19.1.*

        oid_2_19_1:

            oidPath += "/[Versions]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                9 => $"{oidPath}/[ISOMHEG-ud]",
                11 => $"{oidPath}/[ISOMHEG-sir]",
                17 => $"{oidPath}/[ISO13522-MHEG-5]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // genericULS, generic-upper-layers-security, guls
        #region 2.20.*

        oid_2_20:

            oidPath += "/[Generic Upper Layers Security (GULS)]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_20_1;
                case 2: return $"{oidPath}/[General transfer syntax]";
                case 3: goto oid_2_20_3;
                case 4: goto oid_2_20_4;
                case 5: goto oid_2_20_5;
                case 7: goto oid_2_20_7;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // modules
        #region 2.20.1.*

        oid_2_20_1:

            oidPath += "/[ASN.1 modules]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[ObjectIdentifiers]",
                1 => $"{oidPath}/[Notation]",
                2 => $"{oidPath}/[GulsSecurityExchanges]",
                3 => $"{oidPath}/[GulsSecurityTransformations]",
                4 => $"{oidPath}/[DirectoryProtectionMappings]",
                5 => $"{oidPath}/[GULSProtectionMappings]",
                6 => $"{oidPath}/[SeseAPDUs]",
                7 => $"{oidPath}/[GenericProtectingTransferSyntax]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // specificTransferSyntax
        #region 2.20.3.*

        oid_2_20_3:

            oidPath += "/[Specific transfer syntax]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: return $"{oidPath}/[Basic Encoding Rules (BER)]";
                case 2: goto oid_2_20_3_2;
                case 3: goto oid_2_20_3_3;
                case 5: goto oid_2_20_3_5;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // ber-derived
        #region 2.20.3.2.*

        oid_2_20_3_2:

            oidPath += "/[Canonical Encoding Rules (CER) and Distinguished Encoding Rules (DER) as variants of the Basic Encoding Rules (BER)]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Canonical Encoding Rules (CER)]",
                1 => $"{oidPath}/[Distinguished Encoding Rules (DER)]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // packed-encoding
        #region 2.20.3.3.*

        oid_2_20_3_3:

            oidPath += "/[Packed Encoding Rules (PER)]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Basic variant]",
                1 => $"{oidPath}/[Canonical variant]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // packed-encoding
        #region 2.20.3.5.*

        oid_2_20_3_5:

            oidPath += "/[EXtensible Markup Language (XML) Encoding Rules (XER)]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Basic variant]",
                1 => $"{oidPath}/[Canonical variant]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // securityExchanges
        #region 2.20.4.*

        oid_2_20_4:

            oidPath += "/[Security exchanges]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[dirAuthenticationOneWay]",
                2 => $"{oidPath}/[dirAuthenticationTwoWay]",
                3 => $"{oidPath}/[simpleNegotiationSE]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // securityTransformations
        #region 2.20.5.*

        oid_2_20_5:

            oidPath += "/[Security transformations]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[dirEncryptedTransformation]",
                2 => $"{oidPath}/[dirSignedTransformation]",
                3 => $"{oidPath}/[dirSignatureTransformation]",
                4 => $"{oidPath}/[gulsSignedTransformation]",
                5 => $"{oidPath}/[gulsSignatureTransformation]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // application-contexts
        #region 2.20.7.*

        oid_2_20_7:

            oidPath += "/[Application contexts]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Basic]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // transport-layer-security-protocol
        #region 2.21.*

        oid_2_21:

            oidPath += "/[Transport layer security protocol]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_21_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // sa-p-kte
        #region 2.21.1.*

        oid_2_21_1:

            oidPath += "/[Security Association Protocol Type]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Exponential Key Exchange (EKE)]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // network-layer-security-protocol
        #region 2.22.*

        oid_2_22:

            oidPath += "/[Network layer security protocol]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_22_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // sa-p-kte
        #region 2.22.1.*

        oid_2_22_1:

            oidPath += "/[Security Association Protocol Type]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Exponential Key Exchange (EKE)]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // international-organizations
        #region 2.23.*

        oid_2_23:

            oidPath += "/International-Organizations";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 42: goto oid_2_23_42;
                case 43: goto oid_2_23_43;
                case 128: return $"{oidPath}/[Teleglobe, Inc.]";
                case 129: return $"{oidPath}/[Key Recovery Alliance]";
                case 130: return $"{oidPath}/[Object Management Group]";
                case 131: return $"{oidPath}/[Visa International]";
                case 132: return $"{oidPath}/[Comprehensive nuclear-Test-Ban Treaty Organization (CTBTO) Public-Key Infrastructure (PKI)]";
                case 133: goto oid_2_23_133;
                case 134: return $"{oidPath}/[Ceska Posta s.p.]";
                case 135: return $"{oidPath}/[\"HBOS Plc\"]";
                case 136: goto oid_2_23_136;
                case 137: return $"{oidPath}/[Comrad Medical Systems]";
                case 138: return $"{oidPath}/[International Atomic Energy Agency (IAEA)]";
                case 139: return $"{oidPath}/[British Sky Broadcasting Group]";
                case 140: goto oid_2_23_140;
                case 141: return $"{oidPath}/[\"WAC\" Application Services Ltd.]";
                case 143: goto oid_2_23_143;
                case 144: return $"{oidPath}/[Directorate General of The General Security of Lebanon]";
                case 146: goto oid_2_23_146;
                case 147: return $"{oidPath}/[Peripheral Component Interconnect Special Interest Group (PCI-SIG) component measurement and authorization]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // set
        #region 2.23.42.*

        oid_2_23_42:

            oidPath += "/[Secure Electronic Transactions (SET)]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_2_23_42_0;
                case 1: goto oid_2_23_42_1;
                case 2: goto oid_2_23_42_2;
                case 3: goto oid_2_23_42_3;
                case 4: return $"{oidPath}/[algorithm]";
                case 5: goto oid_2_23_42_5;
                case 6: goto oid_2_23_42_6;
                case 7: goto oid_2_23_42_7;
                case 8: goto oid_2_23_42_8;
                case 9: goto oid_2_23_42_9;
                case 10: goto oid_2_23_42_10;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // contentType
        #region 2.23.42.0.*

        oid_2_23_42_0:

            oidPath += "/[contentType]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[id-set-content-PANData]",
                1 => $"{oidPath}/[id-set-content-PANToken]",
                2 => $"{oidPath}/[id-set-content-PANOnly]",
                3 => $"{oidPath}/[id-set-content-OIData]",
                4 => $"{oidPath}/[id-set-content-PI]",
                5 => $"{oidPath}/[id-set-content-PIData]",
                6 => $"{oidPath}/[id-set-content-PIDataUnsigned]",
                7 => $"{oidPath}/[id-set-content-HODInput]",
                8 => $"{oidPath}/[id-set-content-AuthResBaggage]",
                9 => $"{oidPath}/[id-set-content-AuthRevReqBaggage]",
                10 => $"{oidPath}/[id-set-content-AuthRevResBaggage]",
                11 => $"{oidPath}/[id-set-content-CapTokenSeq]",
                12 => $"{oidPath}/[id-set-content-PInitResData]",
                13 => $"{oidPath}/[id-set-content-PI-TBS]",
                14 => $"{oidPath}/[id-set-content-PResData]",
                15 => $"{oidPath}/[id-set-content-InqReqData]",
                16 => $"{oidPath}/[id-set-content-AuthReqTBS]",
                17 => $"{oidPath}/[id-set-content-AuthResTBS]",
                18 => $"{oidPath}/[id-set-content-AuthResTBSXOID]",
                19 => $"{oidPath}/[id-set-content-AuthTokenTBS]",
                20 => $"{oidPath}/[id-set-content-CapTokenData]",
                21 => $"{oidPath}/[id-set-content-CapTokenTBSOID]",
                22 => $"{oidPath}/[id-set-content-AcqCardCodeMsg]",
                23 => $"{oidPath}/[id-set-content-AuthRevReqTBS]",
                24 => $"{oidPath}/[id-set-content-AuthRevResData]",
                25 => $"{oidPath}/[id-set-content-AuthRevResTBS]",
                26 => $"{oidPath}/[id-set-content-CapReqTBS]",
                27 => $"{oidPath}/[id-set-content-CapReqTBSX]",
                28 => $"{oidPath}/[id-set-content-CapResData]",
                29 => $"{oidPath}/[id-set-content-CapRevReqTBS]",
                30 => $"{oidPath}/[id-set-content-CapRevReqTBSX]",
                31 => $"{oidPath}/[id-set-content-CapRevResData]",
                32 => $"{oidPath}/[id-set-content-CredReqTBS]",
                33 => $"{oidPath}/[id-set-content-CredReqTBSXOID]",
                34 => $"{oidPath}/[id-set-content-CredResDataOID]",
                35 => $"{oidPath}/[id-set-content-CredRevReqTBS]",
                36 => $"{oidPath}/[id-set-content-CredRevReqTBSX]",
                37 => $"{oidPath}/[id-set-content-CredRevResData]",
                38 => $"{oidPath}/[id-set-content-PCertReqData]",
                39 => $"{oidPath}/[id-set-content-PCertResTBSOID]",
                40 => $"{oidPath}/[id-set-content-BatchAdminReqData]",
                41 => $"{oidPath}/[id-set-content-BatchAdminResData]",
                42 => $"{oidPath}/[id-set-content-CardCInitResTBS]",
                43 => $"{oidPath}/[id-set-content-AqCInitResTBSOID]",
                44 => $"{oidPath}/[id-set-content-RegFormResTBS]",
                45 => $"{oidPath}/[id-set-content-CertReqDataOID]",
                46 => $"{oidPath}/[id-set-content-CertReqTBS]",
                47 => $"{oidPath}/[id-set-content-CertResDataOID]",
                48 => $"{oidPath}/[id-set-content-CertInqReqTBS]",
                49 => $"{oidPath}/[id-set-content-ErrorTBS]",
                50 => $"{oidPath}/[id-set-content-PIDualSignedTBE]",
                51 => $"{oidPath}/[id-set-content-PIUnsignedTBE]",
                52 => $"{oidPath}/[id-set-content-AuthReqTBE]",
                53 => $"{oidPath}/[id-set-content-AuthResTBE]",
                54 => $"{oidPath}/[id-set-content-AuthResTBEX]",
                55 => $"{oidPath}/[id-set-content-AuthTokenTBE]",
                56 => $"{oidPath}/[id-set-content-CapTokenTBEOID]",
                57 => $"{oidPath}/[id-set-content-CapTokenTBEX]",
                58 => $"{oidPath}/[id-set-content-AcqCardCodeMsgTBE]",
                59 => $"{oidPath}/[id-set-content-AuthRevReqTBE]",
                60 => $"{oidPath}/[id-set-content-AuthRevResTBE]",
                61 => $"{oidPath}/[id-set-content-AuthRevResTBEB]",
                62 => $"{oidPath}/[id-set-content-CapReqTBE]",
                63 => $"{oidPath}/[id-set-content-CapReqTBEX]",
                64 => $"{oidPath}/[id-set-content-CapResTBE]",
                65 => $"{oidPath}/[id-set-content-CapRevReqTBE]",
                66 => $"{oidPath}/[id-set-content-CapRevReqTBEX]",
                67 => $"{oidPath}/[id-set-content-CapRevResTBE]",
                68 => $"{oidPath}/[id-set-content-CredReqTBE]",
                69 => $"{oidPath}/[id-set-content-CredReqTBEXOID]",
                70 => $"{oidPath}/[id-set-content-CredResTBE]",
                71 => $"{oidPath}/[id-set-content-CredRevReqTBE]",
                72 => $"{oidPath}/[id-set-content-CredRevReqTBEX]",
                73 => $"{oidPath}/[id-set-content-CredRevResTBE]",
                74 => $"{oidPath}/[id-set-content-BatchAdminReqTBE]",
                75 => $"{oidPath}/[id-set-content-BatchAdminResTBE]",
                76 => $"{oidPath}/[id-set-content-RegFormReqTBE]",
                77 => $"{oidPath}/[id-set-content-CertReqTBE]",
                78 => $"{oidPath}/[id-set-content-CertReqTBEX]",
                79 => $"{oidPath}/[id-set-content-CertResTBE]",
                80 => $"{oidPath}/[id-set-content-CRLNotificationTBS]",
                81 => $"{oidPath}/[id-set-content-CRLNotificationResTBS]",
                82 => $"{oidPath}/[id-set-content-BCIDistributionTBS]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // msgExt
        #region 2.23.42.1.*

        oid_2_23_42_1:

            oidPath += "/[msgExt]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[setext-genCrypt]",
                3 => $"{oidPath}/[setext-miAuth]",
                4 => $"{oidPath}/[setext-pinSecure]",
                5 => $"{oidPath}/[setext-pinAny]",
                7 => $"{oidPath}/[setext-track2]",
                8 => $"{oidPath}/[setext-cv]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // field
        #region 2.23.42.2.*

        oid_2_23_42_2:

            oidPath += "/[field]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[fullName]",
                1 => $"{oidPath}/[givenName]",
                2 => $"{oidPath}/[familyName]",
                3 => $"{oidPath}/[birthFamilyName]",
                4 => $"{oidPath}/[placeName]",
                5 => $"{oidPath}/[identificationNumber]",
                6 => $"{oidPath}/[month]",
                7 => $"{oidPath}/[date]",
                8 => $"{oidPath}/[address]",
                9 => $"{oidPath}/[telephone]",
                10 => $"{oidPath}/[amount]",
                11 => $"{oidPath}/[accountNumber]",
                12 => $"{oidPath}/[passPhrase]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // attribute
        #region 2.23.42.3.*

        oid_2_23_42_3:

            oidPath += "/[attribute]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_2_23_42_3_0;
                case 1: return $"{oidPath}/[setAttr-PGWYcap]";
                case 2: goto oid_2_23_42_3_2;
                case 3: goto oid_2_23_42_3_3;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // 0
        #region 2.23.42.3.0.*

        oid_2_23_42_3_0:

            oidPath += "/[cert]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[rootKeyThumb]",
                1 => $"{oidPath}/[additionalPolicy]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // 2
        #region 2.23.42.3.2.*

        oid_2_23_42_3_2:

            oidPath += "/[setAttr-TokenType]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[setAttr-Token-EMV]",
                2 => $"{oidPath}/[setAttr-Token-B0Prime]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // 3
        #region 2.23.42.3.3.*

        oid_2_23_42_3_3:

            oidPath += "/[setAttr-IssCap]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 3: goto oid_2_23_42_3_3_3;
                case 4: goto oid_2_23_42_3_3_4;
                case 5: goto oid_2_23_42_3_3_5;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // 3
        #region 2.23.42.3.3.3.*

        oid_2_23_42_3_3_3:

            oidPath += "/[setAttr-IssCap-CVM]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[setAttr-GenCryptgrm]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // 4
        #region 2.23.42.3.3.4.*

        oid_2_23_42_3_3_4:

            oidPath += "/[setAttr-IssCap-T2]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[setAttr-T2Enc]",
                2 => $"{oidPath}/[setAttr-T2cleartxt]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // 5
        #region 2.23.42.3.3.5.*

        oid_2_23_42_3_3_5:

            oidPath += "/[setAttr-IssCap-Sig]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[setAttr-TokICCsig]",
                2 => $"{oidPath}/[setAttr-SecDevSig]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        // policy
        #region 2.23.42.5.*

        oid_2_23_42_5:

            oidPath += "/[policy]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[root]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // module
        #region 2.23.42.6.*

        oid_2_23_42_6:

            oidPath += "/[ASN.1 modules]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[SetMessage]",
                1 => $"{oidPath}/[SetCertMsgs]",
                2 => $"{oidPath}/[SetPayMsgs]",
                4 => $"{oidPath}/[SetCertificateExtensions]",
                8 => $"{oidPath}/[SetMarketData]",
                9 => $"{oidPath}/[SetPKCS10]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // 7
        #region 2.23.42.7.*

        oid_2_23_42_7:

            oidPath += "/[certExt]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[hashedRootKey]",
                1 => $"{oidPath}/[certificateType]",
                2 => $"{oidPath}/[merchantData]",
                3 => $"{oidPath}/[cardCertRequired]",
                4 => $"{oidPath}/[tunneling]",
                5 => $"{oidPath}/[setExtensions]",
                6 => $"{oidPath}/[setQualifier]",
                7 => $"{oidPath}/[setCext-PGWYcapabilities]",
                8 => $"{oidPath}/[setCext-TokenIdentifier]",
                9 => $"{oidPath}/[setCext-Track2Data]",
                10 => $"{oidPath}/[setCext-TokenType]",
                11 => $"{oidPath}/[setCext-IssuerCapabilities]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // 8
        #region 2.23.42.8.*

        oid_2_23_42_8:

            oidPath += "/[brand]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[International Air Transport Association (IATA)-Air Transport Association (ATA)]",
                4 => $"{oidPath}/[VISA]",
                5 => $"{oidPath}/[MasterCard]",
                30 => $"{oidPath}/[Diners]",
                34 => $"{oidPath}/[AmericanExpress]",
                35 => $"{oidPath}/[JCB]",
                6011 => $"{oidPath}/[Novus]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // vendor, set-vendors
        #region 2.23.42.9.*

        oid_2_23_42_9:

            oidPath += "/[Registered vendors]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: return $"{oidPath}/[GlobeSet]";
                case 1: return $"{oidPath}/[IBM]";
                case 2: return $"{oidPath}/[CyberCash]";
                case 3: return $"{oidPath}/[Terisa]";
                case 4: return $"{oidPath}/[RSADSI]";
                case 5: return $"{oidPath}/[VeriFone]";
                case 6: return $"{oidPath}/[TrinTech]";
                case 7: return $"{oidPath}/[BankGate]";
                case 8: return $"{oidPath}/[GTE]";
                case 9: return $"{oidPath}/[CompuSource]";
                case 10: goto oid_2_23_42_9_10;
                case 11: goto oid_2_23_42_9_11;
                case 12: return $"{oidPath}/[OSS]";
                case 13: return $"{oidPath}/[TenthMountain]";
                case 14: return $"{oidPath}/[Antares]";
                case 15: return $"{oidPath}/[ECC]";
                case 16: return $"{oidPath}/[Maithean]";
                case 17: return $"{oidPath}/[Netscape]";
                case 18: return $"{oidPath}/[Verisign]";
                case 19: return $"{oidPath}/[BlueMoney]";
                case 20: return $"{oidPath}/[Lacerte]";
                case 21: return $"{oidPath}/[Fujitsu]";
                case 22: return $"{oidPath}/[eLab]";
                case 23: return $"{oidPath}/[Entrust]";
                case 24: return $"{oidPath}/[VIAnet]";
                case 25: return $"{oidPath}/[III]";
                case 26: return $"{oidPath}/[OpenMarket]";
                case 27: return $"{oidPath}/[Lexem]";
                case 28: return $"{oidPath}/[Intertrader]";
                case 29: return $"{oidPath}/[Persimmon]";
                case 30: return $"{oidPath}/[NABLE]";
                case 31: return $"{oidPath}/[Espace-net]";
                case 32: return $"{oidPath}/[Hitachi]";
                case 33: return $"{oidPath}/[Microsoft]";
                case 34: return $"{oidPath}/[NEC]";
                case 35: return $"{oidPath}/[Mitsubishi]";
                case 36: return $"{oidPath}/[NCR]";
                case 37: return $"{oidPath}/[e-COMM]";
                case 38: return $"{oidPath}/[Gemplus]";
                case 39: return $"{oidPath}/[SKCC]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // griffin
        #region 2.23.42.9.10.*

        oid_2_23_42_9_10:

            oidPath += "/[Griffin Consulting]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_23_42_9_10_1;
                case 2: goto oid_2_23_42_9_10_2;
                case 3: goto oid_2_23_42_9_10_3;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // modules
        #region 2.23.42.9.10.1.*

        oid_2_23_42_9_10_1:

            oidPath += "/[ASN.1 modules]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[OID-Registry]",
                1 => $"{oidPath}/[armenians]",
                2 => $"{oidPath}/[bhebrew]",
                3 => $"{oidPath}/[ctrl646]",
                4 => $"{oidPath}/[currency]",
                5 => $"{oidPath}/[dingbats]",
                6 => $"{oidPath}/[genpunc]",
                7 => $"{oidPath}/[katakana]",
                8 => $"{oidPath}/[misctech]",
                9 => $"{oidPath}/[ocr]",
                10 => $"{oidPath}/[telegraph]",
                11 => $"{oidPath}/[eyeExamples]",
                12 => $"{oidPath}/[cherokee]",
                13 => $"{oidPath}/[ethiopic]",
                14 => $"{oidPath}/[khmer]",
                15 => $"{oidPath}/[mongolian]",
                16 => $"{oidPath}/[ogham]",
                17 => $"{oidPath}/[runic]",
                18 => $"{oidPath}/[x942]",
                19 => $"{oidPath}/[cmp]",
                20 => $"{oidPath}/[biometricObject]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // examples
        #region 2.23.42.9.10.2.*

        oid_2_23_42_9_10_2:

            oidPath += "/[ASN.1 examples]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_2_23_42_9_10_2_0;
                case 1: goto oid_2_23_42_9_10_2_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // extKeyUsage
        #region 2.23.42.9.10.2.0.*

        oid_2_23_42_9_10_2_0:

            oidPath += "/[extKeyUsage]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[extKeyUsage-Ex1]",
                2 => $"{oidPath}/[extKeyUsage-Ex2]",
                3 => $"{oidPath}/[extKeyUsage-Ex3]",
                4 => $"{oidPath}/[extKeyUsage-Ex4]",
                5 => $"{oidPath}/[extKeyUsage-Ex5]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // certificatePolicies
        #region 2.23.42.9.10.2.1.*

        oid_2_23_42_9_10_2_1:

            oidPath += "/[certificatePolicies]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[certificatePolicies-Ex1]",
                2 => $"{oidPath}/[certificatePolicies-Ex2]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // business
        #region 2.23.42.9.10.3.*

        oid_2_23_42_9_10_3:

            oidPath += "/[business]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_2_23_42_9_10_3_0;
                case 1: return $"{oidPath}/[Viatec]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // tecsec
        #region 2.23.42.9.10.3.0.*

        oid_2_23_42_9_10_3_0:

            oidPath += "/[Tecsec]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 2: goto oid_2_23_42_9_10_3_0_2;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // cms
        #region 2.23.42.9.10.3.0.2.*

        oid_2_23_42_9_10_3_0_2:

            oidPath += "/[Cryptographic messages syntaxes]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 2: goto oid_2_23_42_9_10_3_0_2_2;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // header
        #region 2.23.42.9.10.3.0.2.2.*

        oid_2_23_42_9_10_3_0_2_2:

            oidPath += "/[Header]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: return $"{oidPath}/[Ivec]";
                case 2: return $"{oidPath}/[Secryptm]";
                case 3: return $"{oidPath}/[Filelength]";
                case 4: return $"{oidPath}/[Filehash]";
                case 5: return $"{oidPath}/[Filename]";
                case 6: return $"{oidPath}/[Domainlist]";
                case 7: return $"{oidPath}/[Accessgrouplist]";
                case 8: return $"{oidPath}/[Issuer]";
                case 9: return $"{oidPath}/[Credentiallist]";
                case 10: return $"{oidPath}/[SignKey]";
                case 11: return $"{oidPath}/[KeyUsage]";
                case 12: goto oid_2_23_42_9_10_3_0_2_2_12;
                case 13: return $"{oidPath}/[FavoriteName]";
                case 14: return $"{oidPath}/[DataSignature]";
                case 15: return $"{oidPath}/[BlockSize]";
                case 16: return $"{oidPath}/[DataFormat]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // 12
        #region 2.23.42.9.10.3.0.2.2.12.*

        oid_2_23_42_9_10_3_0_2_2_12:

            oidPath += "/[BitSpray]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[BitSprayMeta]",
                2 => $"{oidPath}/[BitSprayShares]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        #endregion

        #endregion

        #endregion

        // 11
        #region 2.23.42.9.11.*

        oid_2_23_42_9_11:

            oidPath += "/[Certicom]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 4: goto oid_2_23_42_9_11_4;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // 4
        #region 2.23.42.9.11.4.*

        oid_2_23_42_9_11_4:

            oidPath += "/[algorithms]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_2_23_42_9_11_4_0;
                case 1: goto oid_2_23_42_9_11_4_1;
                case 2: goto oid_2_23_42_9_11_4_2;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // 0
        #region 2.23.42.9.11.4.0.*

        oid_2_23_42_9_11_4_0:

            oidPath += "/[Elliptic Curve Encryption Scheme (ECES)]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[ecesOAEPEncryptionSET]",
                1 => $"{oidPath}/[ecesEncryption]",
                10 => $"{oidPath}/[cryptECESec131a01]",
                11 => $"{oidPath}/[cryptECESec163a01]",
                12 => $"{oidPath}/[cryptECESec239a01]",
                13 => $"{oidPath}/[cryptECESec131b01]",
                14 => $"{oidPath}/[cryptECESec155b01]",
                15 => $"{oidPath}/[cryptECESec163b01]",
                16 => $"{oidPath}/[cryptECESec191b01]",
                17 => $"{oidPath}/[cryptECESec210b01]",
                18 => $"{oidPath}/[cryptECESec239b01]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // 1
        #region 2.23.42.9.11.4.1.*

        oid_2_23_42_9_11_4_1:

            oidPath += "/[Elliptic Curve Digital Signature Algorithm (ECDSA)]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[ecdsaWithSHA-1]",
                10 => $"{oidPath}/[sigECDSAec131a01]",
                11 => $"{oidPath}/[sigECDSAec163a01]",
                12 => $"{oidPath}/[sigECDSAec239a01]",
                13 => $"{oidPath}/[sigECDSAec131b01]",
                14 => $"{oidPath}/[sigECDSAec155b01]",
                15 => $"{oidPath}/[sigECDSAec163b01]",
                16 => $"{oidPath}/[sigECDSAec191b01]",
                17 => $"{oidPath}/[sigECDSAec210b01]",
                18 => $"{oidPath}/[sigECDSAec239b01]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // 2
        #region 2.23.42.9.11.4.2.*

        oid_2_23_42_9_11_4_2:

            oidPath += "/[Elliptic Curve Nyberg-Rueppel Algorithms (ECNRAs)]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                10 => $"{oidPath}/[sigECNRAec131a01]",
                11 => $"{oidPath}/[sigECNRAec163a01]",
                12 => $"{oidPath}/[sigECNRAec239a01]",
                13 => $"{oidPath}/[sigECNRAec131b01]",
                14 => $"{oidPath}/[sigECNRAec155b01]",
                15 => $"{oidPath}/[sigECNRAec163b01]",
                16 => $"{oidPath}/[sigECNRAec191b01]",
                17 => $"{oidPath}/[sigECNRAec210b01]",
                18 => $"{oidPath}/[sigECNRAec239b01]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        #endregion

        // 10
        #region 2.23.42.10.*

        oid_2_23_42_10:

            oidPath += "/[national]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                192 => $"{oidPath}/[Japan]",
                392 => $"{oidPath}/[Japan]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // wap
        #region 2.23.43.*

        oid_2_23_43:

            oidPath += "/[Open Mobile Alliance (OMA)]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: return $"{oidPath}/[Modules]";
                case 1: goto oid_2_23_43_1;
                case 2: goto oid_2_23_43_2;
                case 3: goto oid_2_23_43_3;
                case 4: goto oid_2_23_43_4;
                case 5: goto oid_2_23_43_5;
                case 6: goto oid_2_23_43_6;
                case 7: goto oid_2_23_43_7;
                case 8: goto oid_2_23_43_8;
                case 9: goto oid_2_23_43_9;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // wap-wsg
        #region 2.23.43.1.*

        oid_2_23_43_1:

            oidPath += "/[Wireless Application Protocol (WAP) Security Group (WSG)]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_23_43_1_1;
                case 2: goto oid_2_23_43_1_2;
                case 3: return $"{oidPath}/[wap-wsg-wimpath]";
                case 4: goto oid_2_23_43_1_4;
                case 5: goto oid_2_23_43_1_5;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // wap-wsg-idm-se
        #region 2.23.43.1.1.*

        oid_2_23_43_1_1:

            oidPath += "/[wap-wsg-idm-se]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[wap-wsg-idm-se-wtlsrsa]",
                2 => $"{oidPath}/[wap-wsg-idm-se-wimgenericrsa]",
                3 => $"{oidPath}/[wap-wsg-idm-se-wtlsecdh]",
                4 => $"{oidPath}/[wap-wsg-idm-se-wimgenericecc]",
                5 => $"{oidPath}/[wap-wsg-idm-se-tlsrsa]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // wap-wsg-idm-file
        #region 2.23.43.1.2.*

        oid_2_23_43_1_2:

            oidPath += "/[wap-wsg-idm-file]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[wap-wsg-idm-file-peer]",
                2 => $"{oidPath}/[wap-wsg-idm-file-session]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // wap-wsg-idm-ecid
        #region 2.23.43.1.4.*

        oid_2_23_43_1_4:

            oidPath += "/[wap-wsg-idm-ecid]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[wap-wsg-idm-ecid-wtls1]",
                3 => $"{oidPath}/[wap-wsg-idm-ecid-wtls3]",
                4 => $"{oidPath}/[wap-wsg-idm-ecid-wtls4]",
                5 => $"{oidPath}/[wap-wsg-idm-ecid-wtls5]",
                6 => $"{oidPath}/[wap-wsg-idm-ecid-wtls6]",
                7 => $"{oidPath}/[wap-wsg-idm-ecid-wtls7]",
                8 => $"{oidPath}/[wap-wsg-idm-ecid-wtls8]",
                9 => $"{oidPath}/[wap-wsg-idm-ecid-wtls9]",
                10 => $"{oidPath}/[wap-wsg-idm-ecid-wtls10]",
                11 => $"{oidPath}/[wap-wsg-idm-ecid-wtls11]",
                12 => $"{oidPath}/[wap-wsg-idm-ecid-wtls12]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // wap-signedContent-indications
        #region 2.23.43.1.5.*

        oid_2_23_43_1_5:

            oidPath += "/[wap-signedContent-indications]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[wap-implicitIndication]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // wap-at
        #region 2.23.43.2.*

        oid_2_23_43_2:

            oidPath += "/[Wireless Application Protocol (WAP) AT]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[wap-at-certificateURL]",
                2 => $"{oidPath}/[id-keygen-assertion]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // wap-ce
        #region 2.23.43.3.*

        oid_2_23_43_3:

            oidPath += "/[Wireless Application Protocol (WAP) CE]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[wap-ce-domainInformation]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // wap-oc
        #region 2.23.43.4.*

        oid_2_23_43_4:

            oidPath += "/[Wireless Application Protocol (WAP) OC]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[wap-oc-wapEntity]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // wap-provisioning
        #region 2.23.43.5.*

        oid_2_23_43_5:

            oidPath += "/[Wireless Application Protocol (WAP) Provisioning]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[bootstrap]",
                2 => $"{oidPath}/[config1]",
                3 => $"{oidPath}/[config2]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // oma-drm
        #region 2.23.43.6.*

        oid_2_23_43_6:

            oidPath += "/[Open Mobile Alliance (OMA) DRM]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_23_43_6_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // oma-kp
        #region 2.23.43.6.1.*

        oid_2_23_43_6_1:

            oidPath += "/[oma-kp]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[oma-kp-rightsIssuer]",
                2 => $"{oidPath}/[oma-kp-drmAgent]",
                3 => $"{oidPath}/[oma-kp-srmAgent]",
                4 => $"{oidPath}/[oma-kp-sceDrmAgent]",
                5 => $"{oidPath}/[oma-kp-sceRenderSource]",
                6 => $"{oidPath}/[oma-kp-sceRenderAgent]",
                7 => $"{oidPath}/[oma-kp-localRightsManagerDevice]",
                8 => $"{oidPath}/[oma-kp-localRightsManagerDomain]",
                9 => $"{oidPath}/[oma-kp-domainAuthority]",
                10 => $"{oidPath}/[oma-kp-domainEnforcementAgentLocal]",
                11 => $"{oidPath}/[oma-kp-domainEnforcementAgentNetwork]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // oma-dm
        #region 2.23.43.7.*

        oid_2_23_43_7:

            oidPath += "/[Open Mobile Alliance (OMA) DM]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[dm-bootstrap]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // oma-bcast
        #region 2.23.43.8.*

        oid_2_23_43_8:

            oidPath += "/[BCAST]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[oma-bcast-spcp]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // oma-lwm2m
        #region 2.23.43.9.*

        oid_2_23_43_9:

            oidPath += "/[Lightweight Machine-to-Machine (M2M)]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[lwm2m-bootstrap]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // 133
        #region 2.23.133.*

        oid_2_23_133:

            oidPath += "/[Trusted Computing Group (TCG)]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: return $"{oidPath}/[tcg-tcpaSpecVersion]";
                case 2: goto oid_2_23_133_2;
                case 3: goto oid_2_23_133_3;
                case 4: goto oid_2_23_133_4;
                case 5: goto oid_2_23_133_5;
                case 6: goto oid_2_23_133_6;
                case 8: goto oid_2_23_133_8;
                case 17: goto oid_2_23_133_17;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // tcg-attribute
        #region 2.23.133.2.*

        oid_2_23_133_2:

            oidPath += "/[tcg-attribute]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[tcg-at-tpmManufacturer]",
                2 => $"{oidPath}/[tcg-at-tpmModel]",
                3 => $"{oidPath}/[tcg-at-tpmVersion]",
                4 => $"{oidPath}/[tcg-at-platformManufacturer]",
                5 => $"{oidPath}/[tcg-at-platformModel]",
                6 => $"{oidPath}/[tcg-at-platformVersion]",
                7 => $"{oidPath}/[tcg-at-componentManufacturer]",
                8 => $"{oidPath}/[tcg-at-componentModel]",
                9 => $"{oidPath}/[tcg-at-componentVersion]",
                10 => $"{oidPath}/[tcg-at-securityQualities]",
                11 => $"{oidPath}/[tcg-at-tpmProtectionProfile]",
                12 => $"{oidPath}/[tcg-at-tpmSecurityTarget]",
                13 => $"{oidPath}/[tcg-at-tbbProtectionProfile]",
                14 => $"{oidPath}/[tcg-at-tbbSecurityTarget]",
                15 => $"{oidPath}/[tcg-at-tpmIdLabel]",
                16 => $"{oidPath}/[tcg-at-tpmSpecification]",
                17 => $"{oidPath}/[tcg-at-tcgPlatformSpecification]",
                18 => $"{oidPath}/[tcg-at-tpmSecurityAssertions]",
                19 => $"{oidPath}/[tcg-at-tbbSecurityAssertions]",
                23 => $"{oidPath}/[tcg-at-tcgCredentialSpecification]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // tcg-protocol
        #region 2.23.133.3.*

        oid_2_23_133_3:

            oidPath += "/[tcg-protocol]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[tcg-prt-tpmIdProtocol]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // tcg-algorithm
        #region 2.23.133.4.*

        oid_2_23_133_4:

            oidPath += "/[tcg-algorithm]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[tcg-algorithm-null]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // tcg-platformClass
        #region 2.23.133.5.*

        oid_2_23_133_5:

            oidPath += "/[tcg-platformClass]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_23_133_5_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // tcg-common
        #region 2.23.133.5.1.*

        oid_2_23_133_5_1:

            oidPath += "/[tcg-common]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: return $"{oidPath}/[tcg-at-platformManufacturerStr]";
                case 2: return $"{oidPath}/[tcg-at-platformManufacturerId]";
                case 3: return $"{oidPath}/[tcg-at-platformConfigUri]";
                case 4: return $"{oidPath}/[tcg-at-platformModel]";
                case 5: return $"{oidPath}/[tcg-at-platformVersion]";
                case 6: return $"{oidPath}/[tcg-at-platformSerial]";
                case 7: goto oid_2_23_133_5_1_7;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // tcg-at-platformConfiguration
        #region 2.23.133.5.1.7.*

        oid_2_23_133_5_1_7:

            oidPath += "/[tcg-at-platformConfiguration]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[tcg-at-platformConfiguration-v1]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        // tcg-ce
        #region 2.23.133.6.*

        oid_2_23_133_6:

            oidPath += "/[Certificate Extensions (CE)]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                2 => $"{oidPath}/[tcg-ce-relevantCredentials]",
                3 => $"{oidPath}/[tcg-ce-relevantManifests]",
                4 => $"{oidPath}/[tcg-ce-virtualPlatformAttestationService]",
                5 => $"{oidPath}/[tcg-ce-migrationControllerAttestationService]",
                6 => $"{oidPath}/[tcg-ce-migrationControllerRegistrationService]",
                7 => $"{oidPath}/[tcg-ce-virtualPlatformBackupService]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // tcg-kp
        #region 2.23.133.8.*

        oid_2_23_133_8:

            oidPath += "/[Key Purposes (KPs)]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[tcg-kp-EKCertificate]",
                2 => $"{oidPath}/[tcg-kp-PlatformAttributeCertificate]",
                3 => $"{oidPath}/[tcg-kp-AIKCertificate]",
                4 => $"{oidPath}/[tcg-kp-PlatformKeyCertificate]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // tcg-address
        #region 2.23.133.17.*

        oid_2_23_133_17:

            oidPath += "/[tcg-address]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[tcg-address-ethernetmac]",
                2 => $"{oidPath}/[tcg-address-wlanmac]",
                3 => $"{oidPath}/[tcg-address-bluetoothmac]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // icao
        #region 2.23.136.*

        oid_2_23_136:

            oidPath += "/[International Civil Aviation Organization (ICAO)]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_23_136_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // mrtd
        #region 2.23.136.1.*

        oid_2_23_136_1:

            oidPath += "/[Machine Readable Travel Document (MRTD)]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_23_136_1_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // security
        #region 2.23.136.1.1.*

        oid_2_23_136_1_1:

            oidPath += "/[Security]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: return $"{oidPath}/[LDSSecurityObject]";
                case 2: return $"{oidPath}/[cscaMasterList]";
                case 3: return $"{oidPath}/[cscaMasterListSigningKey]";
                case 4: return $"{oidPath}/[documentTypeList]";
                case 5: return $"{oidPath}/[aaProtocolObject]";
                case 6: goto oid_2_23_136_1_1_6;
                case 7: return $"{oidPath}/[deviationList]";
                case 8: return $"{oidPath}/[deviationListSigningKey]";
                case 9: goto oid_2_23_136_1_1_9;
                case 10: goto oid_2_23_136_1_1_10;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // extensions
        #region 2.23.136.1.1.6.*

        oid_2_23_136_1_1_6:

            oidPath += "/[extensions]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[nameChange]",
                2 => $"{oidPath}/[documentTypeList]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // lds2
        #region 2.23.136.1.1.9.*

        oid_2_23_136_1_1_9:

            oidPath += "/[Logical Data Structure (LDS), version 2.0]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_23_136_1_1_9_1;
                case 2: goto oid_2_23_136_1_1_9_2;
                case 3: goto oid_2_23_136_1_1_9_3;
                case 8: goto oid_2_23_136_1_1_9_8;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // travelRecords
        #region 2.23.136.1.1.9.1.*

        oid_2_23_136_1_1_9_1:

            oidPath += "/[travelRecords]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[application]",
                2 => $"{oidPath}/[access]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // visaRecords
        #region 2.23.136.1.1.9.2.*

        oid_2_23_136_1_1_9_2:

            oidPath += "/[visaRecords]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[application]",
                2 => $"{oidPath}/[access]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // additionalBiometrics
        #region 2.23.136.1.1.9.3.*

        oid_2_23_136_1_1_9_3:

            oidPath += "/[additionalBiometrics]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[application]",
                2 => $"{oidPath}/[access]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // lds2Signer
        #region 2.23.136.1.1.9.8.*

        oid_2_23_136_1_1_9_8:

            oidPath += "/[lds2Signer]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[tsSigner]",
                2 => $"{oidPath}/[vSigner]",
                3 => $"{oidPath}/[bSigner]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // security
        #region 2.23.136.1.1.10.*

        oid_2_23_136_1_1_10:

            oidPath += "/[Single Point Of Contact (SPOC)]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[spocClient]",
                2 => $"{oidPath}/[spocServer]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        #endregion

        // ca-browser-forum
        #region 2.23.140.*

        oid_2_23_140:

            oidPath += "/[CA/Browser Forum]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_23_140_1;
                case 2: goto oid_2_23_140_2;
                case 3: goto oid_2_23_140_3;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // certificate-policies
        #region 2.23.140.1.*

        oid_2_23_140_1:

            oidPath += "/[Certificate policies]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: return $"{oidPath}/[ev-guidelines]";
                case 2: goto oid_2_23_140_1_2;
                case 3: return $"{oidPath}/[extended-validation-codesigning]";
                case 4: goto oid_2_23_140_1_4;
                case 5: return $"{oidPath}/[smime]";
                case 31: return $"{oidPath}/[onion-EV]";
                default: return $"{oidPath}/{values[index - 1]}";
            }


        // baseline-requirements
        #region 2.23.140.1.2.*

        oid_2_23_140_1_2:

            oidPath += "/[baseline-requirements]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[domain-validated]",
                2 => $"{oidPath}/[organization-validated]",
                3 => $"{oidPath}/[individual-validated]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // code-signing-requirements
        #region 2.23.140.1.4.*

        oid_2_23_140_1_4:

            oidPath += "/[code-signing-requirements]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[code-signing]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // certificate-extensions
        #region 2.23.140.2.*

        oid_2_23_140_2:

            oidPath += "/[Certificate extensions]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Test certificate]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // certificate-extensions
        #region 2.23.140.3.*

        oid_2_23_140_3:

            oidPath += "/[Certificate extensions]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[cabforganization-identifier]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // tca, simalliance
        #region 2.23.143.*

        oid_2_23_143:

            oidPath += "/[Trusted Connectivity Alliance]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_23_143_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // euicc-profile
        #region 2.23.143.1.*

        oid_2_23_143_1:

            oidPath += "/[Embedded UICC (eUICC) profile package specification]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_23_143_1_1;
                case 2: goto oid_2_23_143_1_2;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // spec-version
        #region 2.23.143.1.1.*

        oid_2_23_143_1_1:

            oidPath += "/[Specification versions]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Version 1]",
                2 => $"{oidPath}/[Version 2]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // template
        #region 2.23.143.1.2.*

        oid_2_23_143_1_2:

            oidPath += "/[Templates]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: return $"{oidPath}/[mf]";
                case 2: return $"{oidPath}/[cd]";
                case 3: return $"{oidPath}/[telecom]";
                case 4: return $"{oidPath}/[usim]";
                case 5: return $"{oidPath}/[opt-usim]";
                case 6: return $"{oidPath}/[phonebook]";
                case 7: return $"{oidPath}/[gsm-access]";
                case 8: return $"{oidPath}/[isim]";
                case 9: return $"{oidPath}/[opt-isim]";
                case 10: return $"{oidPath}/[csim]";
                case 11: goto oid_2_23_143_1_2_11;
                case 12: return $"{oidPath}/[eap]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // template
        #region 2.23.143.1.2.11.*

        oid_2_23_143_1_2_11:

            oidPath += "/[opt-csim]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                13 => $"{oidPath}/[df-5gs]",
                14 => $"{oidPath}/[df-saip]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        #endregion

        // gsma
        #region 2.23.146.*

        oid_2_23_146:

            oidPath += "/[GSMA]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_23_146_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // rsp
        #region 2.23.146.1.*

        oid_2_23_146_1:

            oidPath += "/[Remote Subscriber identity module Provisioning (RSP)]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_23_146_1_1;
                case 2: goto oid_2_23_146_1_2;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // spec-version
        #region 2.23.146.1.1.*

        oid_2_23_146_1_1:

            oidPath += "/[spec-version]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                2 => $"{oidPath}/[Version 2]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // cert-objects
        #region 2.23.146.1.2.*

        oid_2_23_146_1_2:

            oidPath += "/[id-rsp-cert-objects]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_2_23_146_1_2_0;
                case 1: goto oid_2_23_146_1_2_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // id-rspExt
        #region 2.23.146.1.2.0.*

        oid_2_23_146_1_2_0:

            oidPath += "/[id-rspExt]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[id-rsp-expDate]",
                2 => $"{oidPath}/[id-rsp-totalPartialCrlNumber]",
                3 => $"{oidPath}/[id-rsp-partialCrlNumber]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // id-rspExt
        #region 2.23.146.1.2.1.*

        oid_2_23_146_1_2_1:

            oidPath += "/[id-rspRole]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[id-rspRole-ci]",
                1 => $"{oidPath}/[id-rspRole-euicc]",
                2 => $"{oidPath}/[id-rspRole-eum]",
                3 => $"{oidPath}/[id-rspRole-dp-tls]",
                4 => $"{oidPath}/[id-rspRole-dp-auth]",
                5 => $"{oidPath}/[id-rspRole-dp-pb]",
                6 => $"{oidPath}/[id-rspRole-ds-tls]",
                7 => $"{oidPath}/[id-rspRole-ds-auth]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        #endregion

        #endregion

        // sios
        #region 2.24.*

        oid_2_24:

            oidPath += "/[Security Information Objects (SIOs) for access control]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_2_24_0;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // specification
        #region 2.24.0.*

        oid_2_24_0:

            oidPath += "/[Specification]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_2_24_0_0;
                case 1: goto oid_2_24_0_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // modules
        #region 2.24.0.0.*

        oid_2_24_0_0:

            oidPath += "/[ASN.1 modules]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[SIOsAccessControl-MODULE]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // securityLabels
        #region 2.24.0.1.*

        oid_2_24_0_1:

            oidPath += "/[Security labels]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Confidentiality label]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        // uuid [TODO: Requires 128-bit values]
        #region 2.25.*

        oid_2_25:

            oidPath += "/UUID";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: return $"{oidPath}/00000000-0000-0000-0000-000000000000";
                //case 288786655511405443130567505384701230: return $"{oidPath}/00379e48-0a2b-1085-b288-0002a5d5fd2e";
                //case 987895962269883002155146617097157934: return $"{oidPath}/00be4308-0c89-1085-8ea0-0002a5d5fd2e";
                //case 1858228783942312576083372383319475483: return $"{oidPath}/0165e1c0-a655-11e0-95b8-0002a5d5c51b";
                //case 2474299330026746002885628159579243803: return $"{oidPath}/01dc8860-25fb-11da-82b2-0002a5d5c51b";
                //case 3263645701162998421821186056373271854: return $"{oidPath}/02748e28-08c4-1085-b21d-0002a5d5fd2e";
                //case 3325839809379844461264382260940242222: return $"{oidPath}/02808890-0ad8-1085-9bdf-0002a5d5fd2e";
                // TODO: Left off at https://oid-base.com/cgi-bin/display?oid=2.25.3664154270495270126161055518190585115
                default: return $"{oidPath}/{values[index - 1]}";
            }

        #endregion

        // odp
        #region 2.26.*

        oid_2_26:

            oidPath += "/[Information technology -- Open Distributed Processing (ODP)]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_2_26_0;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // trader
        #region 2.26.0.*

        oid_2_26_0:

            oidPath += "/[Open Distributed Processing -- Trading Function: Provision of trading function using OSI Directory service]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 2: goto oid_2_26_0_2;
                case 4: goto oid_2_26_0_4;
                case 6: goto oid_2_26_0_6;
                case 13: goto oid_2_26_0_13;
                case 15: goto oid_2_26_0_15;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // asn1Modules
        #region 2.26.0.2.*

        oid_2_26_0_2:

            oidPath += "/[ASN.1 modules]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[TraderDefinitions]",
                1 => $"{oidPath}/[PrinterServiceOfferDefinitions]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // id-trader-at
        #region 2.26.0.4.*

        oid_2_26_0_4:

            oidPath += "/[id-trader-at]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 6: return $"{oidPath}/[id-trader-at-commonName]";
                case 100: goto oid_2_26_0_4_100;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // id-trader-at-so
        #region 2.26.0.4.100.*

        oid_2_26_0_4_100:

            oidPath += "/[id-trader-at-so]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                2 => $"{oidPath}/[id-trader-at-so-locationBlg]",
                4 => $"{oidPath}/[id-trader-at-so-langSupp]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // id-trader-oc
        #region 2.26.0.6.*

        oid_2_26_0_6:

            oidPath += "/[id-trader-oc]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: return $"{oidPath}/[id-trader-oc-traderEntry]";
                case 1: goto oid_2_26_0_6_1;
                case 2: return $"{oidPath}/[id-trader-oc-proxyOffer]";
                case 3: return $"{oidPath}/[id-trader-oc-traderLink]";
                case 4: return $"{oidPath}/[id-trader-oc-traderPolicy]";
                case 5: return $"{oidPath}/[id-trader-oc-interfaceEntry]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // id-trader-oc-serviceOffer
        #region 2.26.0.6.1.*

        oid_2_26_0_6_1:

            oidPath += "/[id-trader-oc-serviceOffer]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[id-trader-oc-serviceOffer-printer]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // id-trader-mr
        #region 2.26.0.13.*

        oid_2_26_0_13:

            oidPath += "/[id-trader-mr]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[id-trader-mr-policySpecificationMatch]",
                2 => $"{oidPath}/[id-trader-mr-dynamicPropValueMatch]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // id-trader-mr
        #region 2.26.0.15.*

        oid_2_26_0_15:

            oidPath += "/[id-trader-nf]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[id-trader-nf-serviceOffer]",
                2 => $"{oidPath}/[id-trader-nf-traderLink]",
                3 => $"{oidPath}/[id-trader-nf-traderPolicy]",
                4 => $"{oidPath}/[id-trader-nf-proxyOffer]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        // tag-based, nid
        #region 2.27.*

        oid_2_27:

            oidPath += "/Tag-Based";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[mCode, micro-mCode and mini-mCode for mobile RFID services]",
                2 => $"{oidPath}/[\"ucode\" identification scheme]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // its
        #region 2.28.*

        oid_2_28:

            oidPath += "/ITS";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_2_28_0;
                case 3: return $"{oidPath}/[fieldDevice]";
                case 4: return $"{oidPath}/[fdVms]";
                case 5: return $"{oidPath}/[Graphic Data Dictionary (GDD) codes]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // its-misc
        #region 2.28.0.*

        oid_2_28_0:

            oidPath += "/[Intelligent Transport Systems (ITS) data concepts that do not relate to internationally standardized object classes]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: return $"{oidPath}/[ISO member bodies]";
                case 2: return $"{oidPath}/[Standard development organizations]";
                case 3: goto oid_2_28_0_3;
                case 4: goto oid_2_28_0_4;
                case 50: return $"{oidPath}/[Private entities, such as corporations]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // value-domains
        #region 2.28.0.3.*

        oid_2_28_0_3:

            oidPath += "/[Internationally standardized value domains that may be used by multiple Intelligent Transport Systems (ITS) standards]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Module containing all value domain objects]",
                1 => $"{oidPath}/[Amount value domain]",
                2 => $"{oidPath}/[Binary value domain]",
                3 => $"{oidPath}/[Code value domain]",
                4 => $"{oidPath}/[Date value domain]",
                5 => $"{oidPath}/[Datetime value domain]",
                6 => $"{oidPath}/[Duration value domain]",
                7 => $"{oidPath}/[Identifier value domain]",
                8 => $"{oidPath}/[OID value domain]",
                9 => $"{oidPath}/[Indicator value domain]",
                10 => $"{oidPath}/[Measure value domain]",
                11 => $"{oidPath}/[Name value domain]",
                12 => $"{oidPath}/[Numeric value domain]",
                13 => $"{oidPath}/[Percent value domain]",
                14 => $"{oidPath}/[Quantity value domain]",
                15 => $"{oidPath}/[Rate value domain]",
                16 => $"{oidPath}/[Text value domain]",
                17 => $"{oidPath}/[Time value domain]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // dialogues
        #region 2.28.0.4.*

        oid_2_28_0_4:

            oidPath += "/[Dialogues according to the Intelligent Transportation Systems (ITS) service domain to which they most closely relate]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Traveller info service domain]",
                2 => $"{oidPath}/[Traffic management service domain]",
                3 => $"{oidPath}/[Vehicle services service domain]",
                4 => $"{oidPath}/[Freight transport service domain]",
                5 => $"{oidPath}/[Public transport service domain]",
                6 => $"{oidPath}/[Emergency service service domain]",
                7 => $"{oidPath}/[Electronic payment service domain]",
                8 => $"{oidPath}/[Personal safety service domain]",
                9 => $"{oidPath}/[Environmental monitoring service domain]",
                10 => $"{oidPath}/[Disaster management service domain]",
                11 => $"{oidPath}/[National security service domain]",
                12 => $"{oidPath}/[Data management service domain]",
                13 => $"{oidPath}/[Performance management service domain]",
                14 => $"{oidPath}/[Cooperative Intelligent Transport Systems (ITS)]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        // upu
        #region 2.40.*

        oid_2_40:

            oidPath += "/[Universal Postal Union (UPU)]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_2_40_0;
                case 2: return $"{oidPath}/[Member bodies (postal administrations)]";
                case 3: goto oid_2_40_3;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // standard
        #region 2.40.0.*

        oid_2_40_0:

            oidPath += "/[Universal Postal Union (UPU) standards]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                25 => $"{oidPath}/[Data constructs for the communication of information on postal items, batches and receptacles]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // identified-organization
        #region 2.40.3.*

        oid_2_40_3:

            oidPath += "/[Data content related to standards produced by other identified organizations]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[International Organization for Standardization (ISO)]",
                1 => $"{oidPath}/[International Electrotechnical Commission (IEC)]",
                2 => $"{oidPath}/[United Nations (UN)]",
                3 => $"{oidPath}/[Association Connecting Electronics Industries (IPC)]",
                4 => $"{oidPath}/[International Telecommunication Union (ITU)]",
                5 => $"{oidPath}/[European Telecommunications Standards Institute (ETSI)]",
                6 => $"{oidPath}/[Federal Communications Commission (FCC)]",
                7 => $"{oidPath}/[American National Standards Institute (ANSI)]",
                8 => $"{oidPath}/[United Nations Directories for Electronic Data Interchange for Administration, Commerce and Transport (EDIFACT)]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // bip
        #region 2.41.*

        oid_2_41:

            oidPath += "/BIP";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_2_41_0;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // modules
        #region 2.41.0.*

        oid_2_41_0:

            oidPath += "/[ASN.1 modules]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_2_41_0_0;
                case 1: goto oid_2_41_0_1;
                case 2: goto oid_2_41_0_2;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // bip
        #region 2.41.0.0.*

        oid_2_41_0_0:

            oidPath += "/[Bip]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Version 1]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // bip-tcpip
        #region 2.41.0.1.*

        oid_2_41_0_1:

            oidPath += "/[BIP-TCPIP]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Version 1]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // bip-discovery
        #region 2.41.0.2.*

        oid_2_41_0_2:

            oidPath += "/[BIP-DISCOVERY]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Version 1]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

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
                case 10: goto oid_2_42_10;
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
                case 4: goto oid_2_42_1_4;
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
                case 3: goto oid_2_42_1_1_3;
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

        // symbols
        #region 2.42.1.1.3.*

        oid_2_42_1_1_3:

            oidPath += "/Symbols";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_42_1_1_3_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // physics
        #region 2.42.1.1.3.1.*

        oid_2_42_1_1_3_1:

            oidPath += "/[Symbols related to physics]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[1]",
                2 => $"{oidPath}/[m]",
                3 => $"{oidPath}/[m⁻¹]",
                4 => $"{oidPath}/[m²]",
                5 => $"{oidPath}/[m³]",
                6 => $"{oidPath}/[l, L]",
                7 => $"{oidPath}/[rad]",
                8 => $"{oidPath}/[°]",
                9 => $"{oidPath}/[′]",
                10 => $"{oidPath}/[″]",
                11 => $"{oidPath}/[sr]",
                12 => $"{oidPath}/[s]",
                13 => $"{oidPath}/[min]",
                14 => $"{oidPath}/[h]",
                15 => $"{oidPath}/[d]",
                16 => $"{oidPath}/[m/s]",
                17 => $"{oidPath}/[km/h]",
                18 => $"{oidPath}/[m/s²]",
                19 => $"{oidPath}/[rad/s]",
                20 => $"{oidPath}/[r/s]",
                21 => $"{oidPath}/[r/min]",
                22 => $"{oidPath}/[r/h]",
                23 => $"{oidPath}/[rad/s²]",
                24 => $"{oidPath}/[r]",
                25 => $"{oidPath}/[Hz]",
                26 => $"{oidPath}/[min⁻¹]",
                27 => $"{oidPath}/[rad/s]",
                28 => $"{oidPath}/[rad/m]",
                29 => $"{oidPath}/[Np]",
                30 => $"{oidPath}/[B]",
                31 => $"{oidPath}/[dB]",
                32 => $"{oidPath}/[kg]",
                33 => $"{oidPath}/[kg/m³]",
                34 => $"{oidPath}/[m³/kg]",
                35 => $"{oidPath}/[kg/m]",
                36 => $"{oidPath}/[kg/m²]",
                37 => $"{oidPath}/[kg·m²]",
                38 => $"{oidPath}/[N]",
                39 => $"{oidPath}/[J]",
                40 => $"{oidPath}/[W]",
                41 => $"{oidPath}/[kg·m/s]",
                42 => $"{oidPath}/[N·s]",
                43 => $"{oidPath}/[kg·m²·s¹]",
                44 => $"{oidPath}/[N·m]",
                45 => $"{oidPath}/[N·m·s]",
                46 => $"{oidPath}/[Pa]",
                47 => $"{oidPath}/[bar]",
                48 => $"{oidPath}/[atm]",
                49 => $"{oidPath}/[mmHg]",
                50 => $"{oidPath}/[N/m²]",
                51 => $"{oidPath}/[m²/N]",
                52 => $"{oidPath}/[m⁴]",
                53 => $"{oidPath}/[Pa·s]",
                54 => $"{oidPath}/[m²/s]",
                55 => $"{oidPath}/[N/m]",
                56 => $"{oidPath}/[kg/s]",
                57 => $"{oidPath}/[m³/s]",
                58 => $"{oidPath}/[l/s]",
                59 => $"{oidPath}/[W/m]",
                60 => $"{oidPath}/[lm/W]",
                61 => $"{oidPath}/[lm]",
                62 => $"{oidPath}/[lm·s]",
                63 => $"{oidPath}/[lm·h]",
                64 => $"{oidPath}/[cd]",
                65 => $"{oidPath}/[lx]",
                66 => $"{oidPath}/[cd/m²]",
                67 => $"{oidPath}/[lm/m²]",
                68 => $"{oidPath}/[lx·s]",
                69 => $"{oidPath}/[lx·h]",
                70 => $"{oidPath}/[oct]",
                71 => $"{oidPath}/[c]",
                72 => $"{oidPath}/[dec]",
                73 => $"{oidPath}/[J/m³]",
                74 => $"{oidPath}/[W/m²]",
                75 => $"{oidPath}/[Pa²·s]",
                76 => $"{oidPath}/[Pa·s/m]",
                77 => $"{oidPath}/[Pa·s/m³]",
                78 => $"{oidPath}/[N·s/m]",
                79 => $"{oidPath}/[Da, u]",
                80 => $"{oidPath}/[s⁻¹]",
                81 => $"{oidPath}/[Bq]",
                82 => $"{oidPath}/[Bq/kg]",
                83 => $"{oidPath}/[Bq/m³]",
                84 => $"{oidPath}/[Bq/m²]",
                85 => $"{oidPath}/[m⁻²]",
                86 => $"{oidPath}/[m⁻²/s]",
                87 => $"{oidPath}/[m²/kg]",
                88 => $"{oidPath}/[m⁻³]",
                89 => $"{oidPath}/[eV]",
                90 => $"{oidPath}/[Gy]",
                91 => $"{oidPath}/[Sv]",
                92 => $"{oidPath}/[C/kg]",
                93 => $"{oidPath}/[C/(kg·s)]",
                94 => $"{oidPath}/[Gy/s]",
                95 => $"{oidPath}/[J/m]",
                96 => $"{oidPath}/[eV/m]",
                97 => $"{oidPath}/[K]",
                98 => $"{oidPath}/[°C]",
                99 => $"{oidPath}/[K⁻¹]",
                100 => $"{oidPath}/[Pa/K]",
                101 => $"{oidPath}/[Pa⁻¹]",
                102 => $"{oidPath}/[W/m²]",
                103 => $"{oidPath}/[W/(m·K)]",
                104 => $"{oidPath}/[W/(m²·K)]",
                105 => $"{oidPath}/[m²·K/W]",
                106 => $"{oidPath}/[K/W]",
                107 => $"{oidPath}/[W/K]",
                108 => $"{oidPath}/[m²/s]",
                109 => $"{oidPath}/[J/K]",
                110 => $"{oidPath}/[J/(kg·K)]",
                111 => $"{oidPath}/[A]",
                112 => $"{oidPath}/[C]",
                113 => $"{oidPath}/[A/m²]",
                114 => $"{oidPath}/[V/m]",
                115 => $"{oidPath}/[V]",
                116 => $"{oidPath}/[Ω]",
                117 => $"{oidPath}/[S]",
                118 => $"{oidPath}/[F]",
                119 => $"{oidPath}/[H]",
                120 => $"{oidPath}/[W·h]",
                121 => $"{oidPath}/[T]",
                122 => $"{oidPath}/[Wb]",
                123 => $"{oidPath}/[A/m]",
                124 => $"{oidPath}/[H/m]",
                125 => $"{oidPath}/[Wb/m]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

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

        // practitioners
        #region 2.42.1.4.*

        oid_2_42_1_4:

            oidPath += "/Practitioners";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Medical physicist]",
                2 => $"{oidPath}/[Radiologist]",
                3 => $"{oidPath}/[Radiation protection expert]",
                4 => $"{oidPath}/[Medical imaging and therapeutic equipment technician]",
                5 => $"{oidPath}/[Radiographer]",
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
                case 2: goto oid_2_42_2_2;
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

        // symbol-combinations
        #region 2.42.2.2.*

        oid_2_42_2_2:

            oidPath += "/Symbol_Combinations";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                2307 => $"{oidPath}/[id-comb2307]",
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

        // modules
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
                case 6: goto oid_2_42_3_0_6;
                case 10: goto oid_2_42_3_0_10;
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

        // pbact-access
        #region 2.42.3.0.6.*

        oid_2_42_3_0_6:

            oidPath += "/[Pbact-access]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Version 1]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // telehealth
        #region 2.42.3.0.10.*

        oid_2_42_3_0_10:

            oidPath += "/[Identifiers for the Rec. ITU-T X.1080 series in general]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_2_42_3_0_10_0;
                case 1: return $"{oidPath}/[Rec. ITU-T X.1080.1]";
                case 2: return $"{oidPath}/[Rec. ITU-T X.1080.2]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // part0
        #region 2.42.3.0.10.0.*

        oid_2_42_3_0_10_0:

            oidPath += "/[Rec. ITU-T X.1080.0]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: return $"{oidPath}";
                case 1: goto oid_2_42_3_0_10_0_1;
                case 2: goto oid_2_42_3_0_10_0_2;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // cmsCont
        #region 2.42.3.0.10.0.1.*

        oid_2_42_3_0_10_0_1:

            oidPath += "/[Cryptographic Message Syntax (CMS) content types for the privilege assignment protocol and for the privilege assessment protocol]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[privAssignRequest]",
                2 => $"{oidPath}/[privAssignResult]",
                3 => $"{oidPath}/[readRequest]",
                4 => $"{oidPath}/[readResult]",
                5 => $"{oidPath}/[compareRequest]",
                6 => $"{oidPath}/[compareResult]",
                7 => $"{oidPath}/[addRequest]",
                8 => $"{oidPath}/[addResult]",
                9 => $"{oidPath}/[deleteRequest]",
                10 => $"{oidPath}/[deleteResult]",
                11 => $"{oidPath}/[modifyRequest]",
                12 => $"{oidPath}/[modifyResult]",
                13 => $"{oidPath}/[renameRequest]",
                14 => $"{oidPath}/[renameResult]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // prAttr
        #region 2.42.3.0.10.0.2.*

        oid_2_42_3_0_10_0_2:

            oidPath += "/[Attribute types used for assigning privileges]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[accessSer]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        #endregion

        #endregion

        // thprot
        #region 2.42.10.*

        oid_2_42_10:

            oidPath += "/[e-Health and world-wide telemedicines]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_2_42_10_0;
                case 1: goto oid_2_42_10_1;
                case 2: goto oid_2_42_10_2;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // part0
        #region 2.42.10.0.*

        oid_2_42_10_0:

            oidPath += "/[Access control for telebiometrics data protection]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_2_42_10_0_0;
                case 1: goto oid_2_42_10_0_1;
                case 2: goto oid_2_42_10_0_2;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // module
        #region 2.42.10.0.0.*

        oid_2_42_10_0_0:

            oidPath += "/[ASN.1 modules]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_42_10_0_0_1;
                case 2: goto oid_2_42_10_0_0_2;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // cmsProfile
        #region 2.42.10.0.0.1.*

        oid_2_42_10_0_0_1:

            oidPath += "/[CONTENT-TYPE]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Version 1]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // pbact-access
        #region 2.42.10.0.0.2.*

        oid_2_42_10_0_0_2:

            oidPath += "/[Pbact-access]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Version 1]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // cmsCont
        #region 2.42.10.0.1.*

        oid_2_42_10_0_1:

            oidPath += "/[Cryptographic Message Syntax (CMS) content types]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Privilege assignment request content type]",
                2 => $"{oidPath}/[Privilege assignment result content type]",
                3 => $"{oidPath}/[Read operation content type]",
                4 => $"{oidPath}/[Read result content type]",
                5 => $"{oidPath}/[Compare request content type]",
                6 => $"{oidPath}/[Compare result content type]",
                7 => $"{oidPath}/[Add request content type]",
                8 => $"{oidPath}/[Add result content type]",
                9 => $"{oidPath}/[Delete request content type]",
                10 => $"{oidPath}/[Delete result content type]",
                11 => $"{oidPath}/[Modify request content type]",
                12 => $"{oidPath}/[Modify result content type]",
                13 => $"{oidPath}/[Rename request content type]",
                14 => $"{oidPath}/[Rename result content type]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // cmsCont
        #region 2.42.10.0.2.*

        oid_2_42_10_0_2:

            oidPath += "/[Attribute types for carrying privilege definitions]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Access service attribute syntax]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // part1
        #region 2.42.10.1.*

        oid_2_42_10_1:

            oidPath += "/[e-Health and world-wide telemedicines - Generic telecommunication protocol]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_2_42_10_1_0;
                case 1: goto oid_2_42_10_1_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // module
        #region 2.42.10.1.0.*

        oid_2_42_10_1_0:

            oidPath += "/[ASN.1 modules]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_2_42_10_1_0_0;
                case 1: goto oid_2_42_10_1_0_1;
                case 2: goto oid_2_42_10_1_0_2;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // oids
        #region 2.42.10.1.0.0.*

        oid_2_42_10_1_0_0:

            oidPath += "/[Telebiometrics]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Version 1]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // hCommen
        #region 2.42.10.1.0.1.*

        oid_2_42_10_1_0_1:

            oidPath += "/[E-health-common]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Version 1]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // id-telehealth
        #region 2.42.10.1.0.2.*

        oid_2_42_10_1_0_2:

            oidPath += "/[E-health-identification]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Version 1]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // cms-content
        #region 2.42.10.1.1.*

        oid_2_42_10_1_1:

            oidPath += "/[Cryptographic Message Syntax (CMS) content types]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Content type for the establishment of a session]",
                2 => $"{oidPath}/[Content type for accepting a session]",
                3 => $"{oidPath}/[Content type for reporting an error during session establishment]",
                4 => $"{oidPath}/[Content type for the initiation of a session termination]",
                5 => $"{oidPath}/[Content type for the completion of a session termination]",
                6 => $"{oidPath}/[Content type for reporting a session termination error]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // part2
        #region 2.42.10.2.*

        oid_2_42_10_2:

            oidPath += "/[Biology-to-machine protocol]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_2_42_10_2_0;
                case 1: goto oid_2_42_10_2_1;
                case 2: goto oid_2_42_10_2_2;
                case 3: goto oid_2_42_10_2_3;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // modules
        #region 2.42.10.2.0.*

        oid_2_42_10_2_0:

            oidPath += "/[ASN.1 modules]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_2_42_10_2_0_0;
                case 1: goto oid_2_42_10_2_0_1;
                case 2: goto oid_2_42_10_2_0_2;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // b2m
        #region 2.42.10.2.0.0.*

        oid_2_42_10_2_0_0:

            oidPath += "/[Biology-to-Machine]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Version 1]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // monitor-types
        #region 2.42.10.2.0.1.*

        oid_2_42_10_2_0_1:

            oidPath += "[Monitor types]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_2_42_10_2_0_1_0;
                case 1: goto oid_2_42_10_2_0_1_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // monitors
        #region 2.42.10.2.0.1.0.*

        oid_2_42_10_2_0_1_0:

            oidPath += "/[MonitorTypes]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Version 1]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // moving-detect
        #region 2.42.10.2.0.1.1.*

        oid_2_42_10_2_0_1_1:

            oidPath += "/[Moving-detection]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Version 1]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // infoObjects
        #region 2.42.10.2.0.2.*

        oid_2_42_10_2_0_2:

            oidPath += "/[InfoObjects]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Version 1]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // monitor-type
        #region 2.42.10.2.1.*

        oid_2_42_10_2_1:

            oidPath += "/[Monitor types]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[moving-detect]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // unit
        #region 2.42.10.2.2.*

        oid_2_42_10_2_2:

            oidPath += "/[Unit type specifications]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[id-un-metre]",
                2 => $"{oidPath}/[id-un-kilogram]",
                3 => $"{oidPath}/[id-un-second]",
                4 => $"{oidPath}/[id-un-ampere]",
                5 => $"{oidPath}/[id-un-kelvin]",
                6 => $"{oidPath}/[id-un-mole]",
                7 => $"{oidPath}/[id-un-candela]",
                8 => $"{oidPath}/[id-un-hertz]",
                9 => $"{oidPath}/[id-un-newton]",
                10 => $"{oidPath}/[id-un-pascal]",
                11 => $"{oidPath}/[id-un-joule]",
                12 => $"{oidPath}/[id-un-watt]",
                13 => $"{oidPath}/[id-un-coulomb]",
                14 => $"{oidPath}/[id-un-volt]",
                15 => $"{oidPath}/[id-un-farad]",
                16 => $"{oidPath}/[id-un-ohm]",
                17 => $"{oidPath}/[id-un-siemens]",
                18 => $"{oidPath}/[id-un-weber]",
                19 => $"{oidPath}/[id-un-tesla]",
                20 => $"{oidPath}/[id-un-henry]",
                21 => $"{oidPath}/[id-un-degreeCelsius]",
                22 => $"{oidPath}/[id-un-lumen]",
                23 => $"{oidPath}/[id-un-lux]",
                24 => $"{oidPath}/[id-un-becquerel]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // gen-info
        #region 2.42.10.2.3.*

        oid_2_42_10_2_3:

            oidPath += "/[General information type specifications]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[id-gi-surname]",
                2 => $"{oidPath}/[id-gi-givenName]",
                3 => $"{oidPath}/[id-gi-initials]",
                4 => $"{oidPath}/[id-gi-generationQualifier]",
                5 => $"{oidPath}/[id-gi-serialNumber]",
                6 => $"{oidPath}/[id-gi-pseudonym]",
                7 => $"{oidPath}/[id-gi-uri]",
                8 => $"{oidPath}/[id-gi-urn]",
                9 => $"{oidPath}/[id-gi-url]",
                10 => $"{oidPath}/[id-gi-dnsName]",
                11 => $"{oidPath}/[id-gi-email]",
                12 => $"{oidPath}/[id-gi-countryName]",
                13 => $"{oidPath}/[id-gi-countryCode3c]",
                14 => $"{oidPath}/[id-gi-localityName]",
                15 => $"{oidPath}/[id-gi-streetAddress]",
                16 => $"{oidPath}/[id-gi-houseIdentifier]",
                17 => $"{oidPath}/[id-gi-utmCoordinates]",
                18 => $"{oidPath}/[id-gi-organizationName]",
                19 => $"{oidPath}/[id-gi-organizationalUnitName]",
                20 => $"{oidPath}/[id-gi-title]",
                21 => $"{oidPath}/[id-gi-organizationIdentifier]",
                22 => $"{oidPath}/[id-gi-description]",
                23 => $"{oidPath}/[id-gi-businessCategory]",
                24 => $"{oidPath}/[id-gi-postalCode]",
                25 => $"{oidPath}/[id-gi-postOfficeBox]",
                26 => $"{oidPath}/[id-gi-telephoneNumber]",
                27 => $"{oidPath}/[id-gi-mobileNumber]",
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
            switch (values[index++])
            {
                case 0: goto oid_2_49_0;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // wmo
        #region 2.49.0.*

        oid_2_49_0:

            oidPath += "/WMO";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_2_49_0_0;
                case 1: return $"{oidPath}/[Alerting messages of countries]";
                case 2: goto oid_2_49_0_2;
                case 3: goto oid_2_49_0_3;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // authority
        #region 2.49.0.0.*

        oid_2_49_0_0:

            oidPath += "/[Alerting authorities of countries]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 4: goto oid_2_49_0_0_4;
                case 8: goto oid_2_49_0_0_8;
                case 12: goto oid_2_49_0_0_12;
                case 20: goto oid_2_49_0_0_20;
                case 24: goto oid_2_49_0_0_24;
                case 28: goto oid_2_49_0_0_28;
                case 31: goto oid_2_49_0_0_31;
                case 32: goto oid_2_49_0_0_32;
                case 36: goto oid_2_49_0_0_36;
                case 40: goto oid_2_49_0_0_40;
                case 44: goto oid_2_49_0_0_44;
                case 48: goto oid_2_49_0_0_48;
                case 50: goto oid_2_49_0_0_50;
                case 51: goto oid_2_49_0_0_51;
                case 52: goto oid_2_49_0_0_52;
                case 56: goto oid_2_49_0_0_56;
                case 60: return $"{oidPath}/[Alerting authorities of Bermuda]";
                case 64: goto oid_2_49_0_0_64;
                case 68: goto oid_2_49_0_0_68;
                case 70: goto oid_2_49_0_0_70;
                case 72: goto oid_2_49_0_0_72;
                case 76: goto oid_2_49_0_0_76;
                case 84: goto oid_2_49_0_0_84;
                case 90: goto oid_2_49_0_0_90;
                case 92: goto oid_2_49_0_0_92;
                case 96: goto oid_2_49_0_0_96;
                case 100: goto oid_2_49_0_0_100;
                case 104: goto oid_2_49_0_0_104;
                case 108: goto oid_2_49_0_0_108;
                case 112: goto oid_2_49_0_0_112;
                case 116: goto oid_2_49_0_0_116;
                case 120: goto oid_2_49_0_0_120;
                case 124: goto oid_2_49_0_0_124;
                case 132: goto oid_2_49_0_0_132;
                case 136: goto oid_2_49_0_0_136;
                case 140: goto oid_2_49_0_0_140;
                case 144: goto oid_2_49_0_0_144;
                case 148: goto oid_2_49_0_0_148;
                case 152: goto oid_2_49_0_0_152;
                case 156: goto oid_2_49_0_0_156;
                case 170: goto oid_2_49_0_0_170;
                case 174: goto oid_2_49_0_0_174;
                case 178: goto oid_2_49_0_0_178;
                case 180: goto oid_2_49_0_0_180;
                case 184: goto oid_2_49_0_0_184;
                case 188: goto oid_2_49_0_0_188;
                case 191: goto oid_2_49_0_0_191;
                case 192: goto oid_2_49_0_0_192;
                case 196: goto oid_2_49_0_0_196;
                case 203: goto oid_2_49_0_0_203;
                case 204: goto oid_2_49_0_0_204;
                case 208: goto oid_2_49_0_0_208;
                case 212: goto oid_2_49_0_0_212;
                case 214: goto oid_2_49_0_0_214;
                case 218: goto oid_2_49_0_0_218;
                case 222: goto oid_2_49_0_0_222;
                case 231: goto oid_2_49_0_0_231;
                case 232: goto oid_2_49_0_0_232;
                case 233: goto oid_2_49_0_0_233;
                case 242: goto oid_2_49_0_0_242;
                case 246: goto oid_2_49_0_0_246;
                case 250: goto oid_2_49_0_0_250;
                case 258: goto oid_2_49_0_0_258;
                case 262: goto oid_2_49_0_0_262;
                case 266: goto oid_2_49_0_0_266;
                case 268: goto oid_2_49_0_0_268;
                case 270: goto oid_2_49_0_0_270;
                case 276: goto oid_2_49_0_0_276;
                case 288: goto oid_2_49_0_0_288;
                case 296: goto oid_2_49_0_0_296;
                case 300: goto oid_2_49_0_0_300;
                case 308: goto oid_2_49_0_0_308;
                case 320: goto oid_2_49_0_0_320;
                case 324: goto oid_2_49_0_0_324;
                case 328: goto oid_2_49_0_0_328;
                case 332: goto oid_2_49_0_0_332;
                case 340: goto oid_2_49_0_0_340;
                case 344: goto oid_2_49_0_0_344;
                case 348: goto oid_2_49_0_0_348;
                case 352: goto oid_2_49_0_0_352;
                case 356: goto oid_2_49_0_0_356;
                case 360: goto oid_2_49_0_0_360;
                case 364: goto oid_2_49_0_0_364;
                case 368: goto oid_2_49_0_0_368;
                case 372: goto oid_2_49_0_0_372;
                case 376: goto oid_2_49_0_0_376;
                case 380: goto oid_2_49_0_0_380;
                case 384: goto oid_2_49_0_0_384;
                case 388: goto oid_2_49_0_0_388;
                case 392: goto oid_2_49_0_0_392;
                case 398: goto oid_2_49_0_0_398;
                case 400: goto oid_2_49_0_0_400;
                case 404: goto oid_2_49_0_0_404;
                case 408: goto oid_2_49_0_0_408;
                case 410: goto oid_2_49_0_0_410;
                case 414: goto oid_2_49_0_0_414;
                case 417: goto oid_2_49_0_0_417;
                case 418: goto oid_2_49_0_0_418;
                case 422: goto oid_2_49_0_0_422;
                case 426: goto oid_2_49_0_0_426;
                case 428: goto oid_2_49_0_0_428;
                case 430: goto oid_2_49_0_0_430;
                case 434: goto oid_2_49_0_0_434;
                case 440: goto oid_2_49_0_0_440;
                case 442: goto oid_2_49_0_0_442;
                case 446: goto oid_2_49_0_0_446;
                case 450: goto oid_2_49_0_0_450;
                case 454: goto oid_2_49_0_0_454;
                case 458: goto oid_2_49_0_0_458;
                case 462: goto oid_2_49_0_0_462;
                case 466: goto oid_2_49_0_0_466;
                case 470: goto oid_2_49_0_0_470;
                case 478: goto oid_2_49_0_0_478;
                case 480: goto oid_2_49_0_0_480;
                case 484: goto oid_2_49_0_0_484;
                case 492: goto oid_2_49_0_0_492;
                case 496: goto oid_2_49_0_0_496;
                case 498: goto oid_2_49_0_0_498;
                case 499: goto oid_2_49_0_0_499;
                case 500: goto oid_2_49_0_0_500;
                case 504: goto oid_2_49_0_0_504;
                case 508: goto oid_2_49_0_0_508;
                case 512: goto oid_2_49_0_0_512;
                case 516: goto oid_2_49_0_0_516;
                case 524: goto oid_2_49_0_0_524;
                case 528: goto oid_2_49_0_0_528;
                case 530: goto oid_2_49_0_0_530;
                case 540: goto oid_2_49_0_0_540;
                case 548: goto oid_2_49_0_0_548;
                case 554: goto oid_2_49_0_0_554;
                case 558: goto oid_2_49_0_0_558;
                case 562: goto oid_2_49_0_0_562;
                case 566: goto oid_2_49_0_0_566;
                case 570: goto oid_2_49_0_0_570;
                case 578: goto oid_2_49_0_0_578;
                case 583: goto oid_2_49_0_0_583;
                case 585: goto oid_2_49_0_0_585;
                case 586: goto oid_2_49_0_0_586;
                case 591: goto oid_2_49_0_0_591;
                case 598: goto oid_2_49_0_0_598;
                case 600: goto oid_2_49_0_0_600;
                case 604: goto oid_2_49_0_0_604;
                case 608: goto oid_2_49_0_0_608;
                case 616: goto oid_2_49_0_0_616;
                case 620: goto oid_2_49_0_0_620;
                case 624: goto oid_2_49_0_0_624;
                case 626: goto oid_2_49_0_0_626;
                case 634: goto oid_2_49_0_0_634;
                case 642: goto oid_2_49_0_0_642;
                case 643: goto oid_2_49_0_0_643;
                case 646: goto oid_2_49_0_0_646;
                case 660: goto oid_2_49_0_0_660;
                case 662: goto oid_2_49_0_0_662;
                case 670: goto oid_2_49_0_0_670;
                case 678: goto oid_2_49_0_0_678;
                case 682: goto oid_2_49_0_0_682;
                case 686: goto oid_2_49_0_0_686;
                case 688: goto oid_2_49_0_0_688;
                case 690: goto oid_2_49_0_0_690;
                case 694: goto oid_2_49_0_0_694;
                case 702: goto oid_2_49_0_0_702;
                case 703: goto oid_2_49_0_0_703;
                case 704: goto oid_2_49_0_0_704;
                case 705: goto oid_2_49_0_0_705;
                case 706: goto oid_2_49_0_0_706;
                case 710: goto oid_2_49_0_0_710;
                case 716: goto oid_2_49_0_0_716;
                case 724: goto oid_2_49_0_0_724;
                case 728: goto oid_2_49_0_0_728;
                case 729: goto oid_2_49_0_0_729;
                case 736: return $"{oidPath}";
                case 740: goto oid_2_49_0_0_740;
                case 748: goto oid_2_49_0_0_748;
                case 752: goto oid_2_49_0_0_752;
                case 756: goto oid_2_49_0_0_756;
                case 760: goto oid_2_49_0_0_760;
                case 762: goto oid_2_49_0_0_762;
                case 764: goto oid_2_49_0_0_764;
                case 768: goto oid_2_49_0_0_768;
                case 776: goto oid_2_49_0_0_776;
                case 780: goto oid_2_49_0_0_780;
                case 784: goto oid_2_49_0_0_784;
                case 788: goto oid_2_49_0_0_788;
                case 792: goto oid_2_49_0_0_792;
                case 795: goto oid_2_49_0_0_795;
                case 796: goto oid_2_49_0_0_796;
                case 798: goto oid_2_49_0_0_798;
                case 800: goto oid_2_49_0_0_800;
                case 804: goto oid_2_49_0_0_804;
                case 807: goto oid_2_49_0_0_807;
                case 818: goto oid_2_49_0_0_818;
                case 826: goto oid_2_49_0_0_826;
                case 834: goto oid_2_49_0_0_834;
                case 840: goto oid_2_49_0_0_840;
                case 854: goto oid_2_49_0_0_854;
                case 858: goto oid_2_49_0_0_858;
                case 860: goto oid_2_49_0_0_860;
                case 862: goto oid_2_49_0_0_862;
                case 882: goto oid_2_49_0_0_882;
                case 887: goto oid_2_49_0_0_887;
                case 894: goto oid_2_49_0_0_894;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // af
        #region 2.49.0.0.4.*

        oid_2_49_0_0_4:

            oidPath += "/[Alerting authorities of Afghanistan]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Afghan Meteorological Authority]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // al
        #region 2.49.0.0.8.*

        oid_2_49_0_0_8:

            oidPath += "/[Alerting authorities of Albania]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Hydrometeorological Institute]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // dz
        #region 2.49.0.0.12.*

        oid_2_49_0_0_12:

            oidPath += "/[Alerting authorities of Algeria]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Office National de la Météorologie]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // ad
        #region 2.49.0.0.20.*

        oid_2_49_0_0_20:

            oidPath += "/[Alerting authorities of Andorra]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Servei Meteorològic d'Andorra]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // ao
        #region 2.49.0.0.24.*

        oid_2_49_0_0_24:

            oidPath += "/[Alerting authorities of Angola]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Instituto Nacional de Hidrometeorología e Geofísica]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // ag
        #region 2.49.0.0.28.*

        oid_2_49_0_0_28:

            oidPath += "/[Alerting authorities of Antigua and Barbuda]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Meteorological Services]",
                78862 => $"{oidPath}/[Antigua and Barbuda Meteorological Service]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // az
        #region 2.49.0.0.31.*

        oid_2_49_0_0_31:

            oidPath += "/[Alerting authorities of Azerbaijan]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Hydrometeorological Institute of the Ministry of Ecology and Natural Resources]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // ar
        #region 2.49.0.0.32.*

        oid_2_49_0_0_32:

            oidPath += "/[Alerting authorities of Argentina]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Servicio Meteorologico Nacional]",
                1 => $"{oidPath}/[Instituto Nacional del Agua]",
                2 => $"{oidPath}/[Servicio de Hidrografía Naval - Ministerio de Defensa]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // au
        #region 2.49.0.0.36.*

        oid_2_49_0_0_36:

            oidPath += "/[Alerting authorities of Australia]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Bureau of Meteorology]",
                1 => $"{oidPath}/[Hydrological Services Program]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // at
        #region 2.49.0.0.40.*

        oid_2_49_0_0_40:

            oidPath += "/[Alerting authorities of Austria]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Central Institute for Meteorology and Geodynamics]",
                1 => $"{oidPath}/[Abteilung Wasserhaushalt]",
                5 => $"{oidPath}/[Amt der Vorarlberger Landesregierung. Wasserwirtschaft]",
                6 => $"{oidPath}/[Hydrographischer Dienst Tirol]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // bs
        #region 2.49.0.0.44.*

        oid_2_49_0_0_44:

            oidPath += "/[Alerting authorities of Bahamas]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Department of Meteorology]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // bh
        #region 2.49.0.0.48.*

        oid_2_49_0_0_48:

            oidPath += "/[Alerting authorities of Bahrain]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Bahrain Meteorological Service]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // bd
        #region 2.49.0.0.50.*

        oid_2_49_0_0_50:

            oidPath += "/[Alerting authorities of Bangladesh]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Bangladesh Meteorological Department]",
                1 => $"{oidPath}/[Bangladesh Water Development Board (BWDB)]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // bd
        #region 2.49.0.0.51.*

        oid_2_49_0_0_51:

            oidPath += "/[Alerting authorities of Armenia]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Armenian State Hydrometeorological and Monitoring Service]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // bb
        #region 2.49.0.0.52.*

        oid_2_49_0_0_52:

            oidPath += "/[Alerting authorities of Barbados]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Meteorological Services]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // be
        #region 2.49.0.0.56.*

        oid_2_49_0_0_56:

            oidPath += "/[Alerting authorities of Belgium]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Institut Royal Météorologique]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // bt
        #region 2.49.0.0.64.*

        oid_2_49_0_0_64:

            oidPath += "/[Alerting authorities of Bhutan]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Council for Renewable Natural Resources Research]",
                1 => $"{oidPath}/[Department of Hydromet Services]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // bo
        #region 2.49.0.0.68.*

        oid_2_49_0_0_68:

            oidPath += "/[Alerting authorities of Bolivia]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Servicio Nacional de Meteorología e Hidrología]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // ba
        #region 2.49.0.0.70.*

        oid_2_49_0_0_70:

            oidPath += "/[Alerting authorities of Bosnia and Herzegovina]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Federal Hydrometeorological Institute of Federation of Bosnia and Herzegovina]",
                1 => $"{oidPath}/[Republic Hydrometeorological Service of Republic of Srpska]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // bw
        #region 2.49.0.0.72.*

        oid_2_49_0_0_72:

            oidPath += "/[Alerting authorities of Botswana]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Botswana Meteorological Services]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // br
        #region 2.49.0.0.76.*

        oid_2_49_0_0_76:

            oidPath += "/[Alerting authorities of Brazil]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Instituto Nacional de Meteorologia - INMET]",
                1 => $"{oidPath}/[Universidade de Brasília - Observatório Sismológico]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // bz
        #region 2.49.0.0.84.*

        oid_2_49_0_0_84:

            oidPath += "/[Alerting authorities of Belize]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[National Meteorological Service]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // sb
        #region 2.49.0.0.90.*

        oid_2_49_0_0_90:

            oidPath += "/[Alerting authorities of Solomon Islands]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Solomon Islands Meteorological Service]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // vg
        #region 2.49.0.0.92.*

        oid_2_49_0_0_92:

            oidPath += "/[Alerting authorities of British Virgin Islands]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Department of Disaster Management]",
                1 => $"{oidPath}/[Caribbean Meteorological Organization]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // bn
        #region 2.49.0.0.96.*

        oid_2_49_0_0_96:

            oidPath += "/[Alerting authorities of Brunei Darussalam]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Brunei Meteorological Service]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // bg
        #region 2.49.0.0.100.*

        oid_2_49_0_0_100:

            oidPath += "/[Alerting authorities of Bulgaria]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[National Institute of Meteorology and Hydrology]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // mm
        #region 2.49.0.0.104.*

        oid_2_49_0_0_104:

            oidPath += "/[Alerting authorities of Myanmar]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Department of Meteorology and Hydrology]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // bi
        #region 2.49.0.0.108.*

        oid_2_49_0_0_108:

            oidPath += "/[Alerting authorities of Burundi]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Institut Géographique du Burundi]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // by
        #region 2.49.0.0.112.*

        oid_2_49_0_0_112:

            oidPath += "/[Alerting authorities of Belarus]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Department of Hydrometeorology]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // kh
        #region 2.49.0.0.116.*

        oid_2_49_0_0_116:

            oidPath += "/[Alerting authorities of Cambodia]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Department of Meteorology (DOM)]",
                1 => $"{oidPath}/[Ministry of Water Resources and Meteorology]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // cm
        #region 2.49.0.0.120.*

        oid_2_49_0_0_120:

            oidPath += "/[Alerting authorities of Cameroon]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Direction de la Météorologie Nationale]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // ca
        #region 2.49.0.0.124.*

        oid_2_49_0_0_124:

            oidPath += "/[Alerting authorities of Canada]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Meteorological Service of Canada]",
                1 => $"{oidPath}/[Natural Resources Canada]",
                2 => $"{oidPath}/[Alberta Emergency Management Agency (Government of Alberta, Ministry of Municipal Affairs)]",
                3 => $"{oidPath}/[Ministère de la Sécurité publique du Québec]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // cv
        #region 2.49.0.0.132.*

        oid_2_49_0_0_132:

            oidPath += "/[Alerting authorities of Cape Verde]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Instituto Nacional de Meteorologia e Geophísica]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // ky
        #region 2.49.0.0.136.*

        oid_2_49_0_0_136:

            oidPath += "/[Alerting authorities of Cayman Islands]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Hazard Management Cayman Islands]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // cf
        #region 2.49.0.0.140.*

        oid_2_49_0_0_140:

            oidPath += "/[Alerting authorities of Central African Republic]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Direction Générale de l'Aviation Civile et de la Météorologie]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // lk
        #region 2.49.0.0.144.*

        oid_2_49_0_0_144:

            oidPath += "/[Alerting authorities of Sri Lanka]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Department of Meteorology]",
                1 => $"{oidPath}/[Hydrology Division, Department of Irrigation]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // td
        #region 2.49.0.0.148.*

        oid_2_49_0_0_148:

            oidPath += "/[Alerting authorities of Chad]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Direction des Ressources en Eau et de la Météorologie]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // cl
        #region 2.49.0.0.152.*

        oid_2_49_0_0_152:

            oidPath += "/[Alerting authorities of Chile]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Direccion Meteorologica de Chile]",
                1 => $"{oidPath}/[Dirección General de Aguas]",
                3 => $"{oidPath}/[Servicio Hidrográfico y Oceanográfico de la Armada]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // cn
        #region 2.49.0.0.156.*

        oid_2_49_0_0_156:

            oidPath += "/[Alerting authorities of China]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[China Meteorological Administration]",
                1 => $"{oidPath}/[Ministry of Water Resources]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // co
        #region 2.49.0.0.170.*

        oid_2_49_0_0_170:

            oidPath += "/[Alerting authorities of Colombia]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Instituto de Hidrología, Meteorología y Estudios Ambientales]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // km
        #region 2.49.0.0.174.*

        oid_2_49_0_0_174:

            oidPath += "/[Alerting authorities of Comoros]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Direction de la Météorologie Nationale]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // cg
        #region 2.49.0.0.178.*

        oid_2_49_0_0_178:

            oidPath += "/[Alerting authorities of Congo]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Direction de la Météorologie]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // cd
        #region 2.49.0.0.180.*

        oid_2_49_0_0_180:

            oidPath += "/[Alerting authorities of Democratic Republic of the Congo]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Agence Nationale de la Météorologie et de Télédétection par Satellite]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // cd
        #region 2.49.0.0.184.*

        oid_2_49_0_0_184:

            oidPath += "/[Alerting authorities of Cook Islands]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Cook Islands Meteorological Service]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // cr
        #region 2.49.0.0.188.*

        oid_2_49_0_0_188:

            oidPath += "/[Alerting authorities of Costa Rica]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Instituto Meteorologico Nacional]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // hr
        #region 2.49.0.0.191.*

        oid_2_49_0_0_191:

            oidPath += "/[Alerting authorities of Croatia]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Meteorological and Hydrological Service]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // cu
        #region 2.49.0.0.192.*

        oid_2_49_0_0_192:

            oidPath += "/[Alerting authorities of Cuba]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Instituto de Meteorología]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // cy
        #region 2.49.0.0.196.*

        oid_2_49_0_0_196:

            oidPath += "/[Alerting authorities of Cyprus]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Meteorological Service]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // cz
        #region 2.49.0.0.203.*

        oid_2_49_0_0_203:

            oidPath += "/[Alerting authorities of Czech Republic]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Czech Hydrometeorological Institute]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // bj
        #region 2.49.0.0.204.*

        oid_2_49_0_0_204:

            oidPath += "/[Alerting authorities of Benin]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Service Météorologique National]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // dk
        #region 2.49.0.0.208.*

        oid_2_49_0_0_208:

            oidPath += "/[Alerting authorities of Denmark]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Danish Meteorological Institute]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // dm
        #region 2.49.0.0.212.*

        oid_2_49_0_0_212:

            oidPath += "/[Alerting authorities of Dominica]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Dominica Meteorological Services]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // dm
        #region 2.49.0.0.214.*

        oid_2_49_0_0_214:

            oidPath += "/[Alerting authorities of Dominican Republic]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Oficina Nacional de Meteorología]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // ec
        #region 2.49.0.0.218.*

        oid_2_49_0_0_218:

            oidPath += "/[Alerting authorities of Ecuador]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Instituto Nacional de Meteorología e Hidrología]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // sv
        #region 2.49.0.0.222.*

        oid_2_49_0_0_222:

            oidPath += "/[Alerting authorities of El Salvador]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Servicio Nacional de Estudios Territoriales]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // et
        #region 2.49.0.0.231.*

        oid_2_49_0_0_231:

            oidPath += "/[Alerting authorities of Ethiopia]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[National Meteorological Agency]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // er
        #region 2.49.0.0.232.*

        oid_2_49_0_0_232:

            oidPath += "/[Alerting authorities of Eritrea]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Civil Aviation Authority]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // ee
        #region 2.49.0.0.233.*

        oid_2_49_0_0_233:

            oidPath += "/[Alerting authorities of Estonia]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Estonian Meteorological and Hydrological Institute]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // fj
        #region 2.49.0.0.242.*

        oid_2_49_0_0_242:

            oidPath += "/[Alerting authorities of Fiji]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Fiji Meteorological Service]",
                1 => $"{oidPath}/[Suva Water Supplies]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // fi
        #region 2.49.0.0.246.*

        oid_2_49_0_0_246:

            oidPath += "/[Alerting authorities of Finland]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Finnish Meteorological Institute]",
                1 => $"{oidPath}/[Finnish Environment Institute]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // fr
        #region 2.49.0.0.250.*

        oid_2_49_0_0_250:

            oidPath += "/[Alerting authorities of France]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Météo-France]",
                1 => $"{oidPath}/[Service Central d'Hydrométéorologie et d'Appui à la Prévision des Inondations]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // pf
        #region 2.49.0.0.258.*

        oid_2_49_0_0_258:

            oidPath += "/[Alerting authorities of French Polynesia]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Météo-France]",
                1 => $"{oidPath}/[DIRECTION DE LA DEFENSE ET DE LA PROTECTION CIVILE]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // dj
        #region 2.49.0.0.262.*

        oid_2_49_0_0_262:

            oidPath += "/[Alerting authorities of Djibouti]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Service de la Météorologie]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // ga
        #region 2.49.0.0.266.*

        oid_2_49_0_0_266:

            oidPath += "/[Alerting authorities of Gabon]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Cabinet du Ministre des Transports]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // ge
        #region 2.49.0.0.268.*

        oid_2_49_0_0_268:

            oidPath += "/[Alerting authorities of Georgia]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Department of Hydrometeorology]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // gm
        #region 2.49.0.0.270.*

        oid_2_49_0_0_270:

            oidPath += "/[Alerting authorities of Gambia]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Department of Water Resources]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // de
        #region 2.49.0.0.276.*

        oid_2_49_0_0_276:

            oidPath += "/[Alerting authorities of Germany]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Deutscher Wetterdienst]",
                1 => $"{oidPath}/[Federal Institute of Hydrology]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // gh
        #region 2.49.0.0.288.*

        oid_2_49_0_0_288:

            oidPath += "/[Alerting authorities of Ghana]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Ghana Meteorological Services Department]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // ki
        #region 2.49.0.0.296.*

        oid_2_49_0_0_296:

            oidPath += "/[Alerting authorities of Kiribati]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Kiribati Meteorological Service]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // gr
        #region 2.49.0.0.300.*

        oid_2_49_0_0_300:

            oidPath += "/[Alerting authorities of Greece]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Hellenic National Meteorological Service]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // gd
        #region 2.49.0.0.308.*

        oid_2_49_0_0_308:

            oidPath += "/[Alerting authorities of Grenada]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Caribbean Meteorological Organization]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // gt
        #region 2.49.0.0.320.*

        oid_2_49_0_0_320:

            oidPath += "/[Alerting authorities of Guatemala]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Instituto Nacional de Sismología, Vulcanología, Meteorología e Hidrología (INSIVUMEH)]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // gn
        #region 2.49.0.0.324.*

        oid_2_49_0_0_324:

            oidPath += "/[Alerting authorities of Guinea]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Direction Nationale de la Météorologie]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // gy
        #region 2.49.0.0.328.*

        oid_2_49_0_0_328:

            oidPath += "/[Alerting authorities of Guyana]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Hydrometeorological Service]",
                1 => $"{oidPath}/[Civil Defence Commission]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // ht
        #region 2.49.0.0.332.*

        oid_2_49_0_0_332:

            oidPath += "/[Alerting authorities of Haiti]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Haiti Weather]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // hn
        #region 2.49.0.0.340.*

        oid_2_49_0_0_340:

            oidPath += "/[Alerting authorities of Honduras]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Servicio Meteorologico Nacional]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // hk
        #region 2.49.0.0.344.*

        oid_2_49_0_0_344:

            oidPath += "/[Alerting authorities of Hong Kong, China]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Hong Kong Observatory]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // hu
        #region 2.49.0.0.348.*

        oid_2_49_0_0_348:

            oidPath += "/[Alerting authorities of Hungary]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Hungarian Meteorological Service]",
                1 => $"{oidPath}/[VITUKI]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // is
        #region 2.49.0.0.352.*

        oid_2_49_0_0_352:

            oidPath += "/[Alerting authorities of Iceland]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Icelandic Meteorological Office]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // in
        #region 2.49.0.0.356.*

        oid_2_49_0_0_356:

            oidPath += "/[Alerting authorities of India]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[India Meteorological Department]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // id
        #region 2.49.0.0.360.*

        oid_2_49_0_0_360:

            oidPath += "/[Alerting authorities of Indonesia]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Meteorological and Geophysical Agency]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // ir
        #region 2.49.0.0.364.*

        oid_2_49_0_0_364:

            oidPath += "/[Alerting authorities of Islamic Republic of Iran]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Islamic Republic of Iran Meteorological Organization]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // iq
        #region 2.49.0.0.368.*

        oid_2_49_0_0_368:

            oidPath += "/[Alerting authorities of Iraq]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Iraqi Meteorological Organization]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // ie
        #region 2.49.0.0.372.*

        oid_2_49_0_0_372:

            oidPath += "/[Alerting authorities of Ireland]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Met Eireann - Irish Meteorological Service]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // il
        #region 2.49.0.0.376.*

        oid_2_49_0_0_376:

            oidPath += "/[Alerting authorities of Israel]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Israel Meteorological Service]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // it
        #region 2.49.0.0.380.*

        oid_2_49_0_0_380:

            oidPath += "/[Alerting authorities of Italy]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Italian Civil Protecion in cooperation with italian region civil protecion structures]",
                2 => $"{oidPath}/[Ministry of Interior - Department of firefighters, public rescue and civil defense]",
                3 => $"{oidPath}/[National Centre for Aeronautical Meteorology and Climatology]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // ci
        #region 2.49.0.0.384.*

        oid_2_49_0_0_384:

            oidPath += "/[Alerting authorities of Cote d'Ivoire]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Direction de la Météorologie Nationale]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // jm
        #region 2.49.0.0.388.*

        oid_2_49_0_0_388:

            oidPath += "/[Alerting authorities of Jamaica]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Meteorological Service]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // jp
        #region 2.49.0.0.392.*

        oid_2_49_0_0_392:

            oidPath += "/[Alerting authorities of Japan]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Japan Meteorological Agency (JMA)]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // kz
        #region 2.49.0.0.398.*

        oid_2_49_0_0_398:

            oidPath += "/[Alerting authorities of Kazakhstan]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Kazhydromet]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // jo
        #region 2.49.0.0.400.*

        oid_2_49_0_0_400:

            oidPath += "/[Alerting authorities of Jordan]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Jordan Meteorological Department]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // ke
        #region 2.49.0.0.404.*

        oid_2_49_0_0_404:

            oidPath += "/[Alerting authorities of Kenya]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Kenya Meteorological Department]",
                1 => $"{oidPath}/[Ministry of Water and Irrigation]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // kp
        #region 2.49.0.0.408.*

        oid_2_49_0_0_408:

            oidPath += "/[Alerting authorities of Democratic Peoples Republic of Korea]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[State Hydrometeorological Administration]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // kr
        #region 2.49.0.0.410.*

        oid_2_49_0_0_410:

            oidPath += "/[Alerting authorities of Republic of Korea]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Korea Meteorological Administration]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // kw
        #region 2.49.0.0.414.*

        oid_2_49_0_0_414:

            oidPath += "/[Alerting authorities of Kuwait]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Meteorological Department]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // kg
        #region 2.49.0.0.417.*

        oid_2_49_0_0_417:

            oidPath += "/[Alerting authorities of Kyrgyzstan]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Main Hydrometeorological Administration]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // la
        #region 2.49.0.0.418.*

        oid_2_49_0_0_418:

            oidPath += "/[Alerting authorities of Lao People's Democratic Republic]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Department of Meteorology and Hydrology]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // lb
        #region 2.49.0.0.422.*

        oid_2_49_0_0_422:

            oidPath += "/[Alerting authorities of Lebanon]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Service Météorologique]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // lb
        #region 2.49.0.0.426.*

        oid_2_49_0_0_426:

            oidPath += "/[Alerting authorities of Lesotho]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Lesotho Meteorological Services]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // lv
        #region 2.49.0.0.428.*

        oid_2_49_0_0_428:

            oidPath += "/[Alerting authorities of Latvia]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Latvian Environment, Geology and Meteorology Centre (LEGMC)]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // lr
        #region 2.49.0.0.430.*

        oid_2_49_0_0_430:

            oidPath += "/[Alerting authorities of Liberia]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Ministry of Transport]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // ly
        #region 2.49.0.0.434.*

        oid_2_49_0_0_434:

            oidPath += "/[Alerting authorities of Libyan Arab Jamahiriya]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Libyan National Meteorological Centre]",
                1 => $"{oidPath}/[General Water Authority]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // lt
        #region 2.49.0.0.440.*

        oid_2_49_0_0_440:

            oidPath += "/[Alerting authorities of Lithuania]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Lithuanian Hydrometeorological Service]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // lu
        #region 2.49.0.0.442.*

        oid_2_49_0_0_442:

            oidPath += "/[Alerting authorities of Luxembourg]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Administration de l'Aéroport de Luxembourg]",
                1 => $"{oidPath}/[Administration de la Gestion de l'Eau]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // mo
        #region 2.49.0.0.446.*

        oid_2_49_0_0_446:

            oidPath += "/[Alerting authorities of Macao, China]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Meteorological and Geophysical Bureau]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // mg
        #region 2.49.0.0.450.*

        oid_2_49_0_0_450:

            oidPath += "/[Alerting authorities of Madagascar]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Direction Générale de la Météorologie]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // mw
        #region 2.49.0.0.454.*

        oid_2_49_0_0_454:

            oidPath += "/[Alerting authorities of Malawi]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Malawi Meteorological Services]",
                1 => $"{oidPath}/[Ministry of Water Department]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // my
        #region 2.49.0.0.458.*

        oid_2_49_0_0_458:

            oidPath += "/[Alerting authorities of Malaysia]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Malaysian Meteorological Department]",
                1 => $"{oidPath}/[Department of Irrigation and Drainage]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // mv
        #region 2.49.0.0.462.*

        oid_2_49_0_0_462:

            oidPath += "/[Alerting authorities of Maldives]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Department of Meteorology]",
                1 => $"{oidPath}/[National Disaster Management Centre]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // ml
        #region 2.49.0.0.466.*

        oid_2_49_0_0_466:

            oidPath += "/[Alerting authorities of Mali]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Direction Nationale de la Météorologie du Mali]",
                1 => $"{oidPath}/[Direction Nationale de l'Hydraulique]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // mt
        #region 2.49.0.0.470.*

        oid_2_49_0_0_470:

            oidPath += "/[Alerting authorities of Malta]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Meteorological Office]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // mr
        #region 2.49.0.0.478.*

        oid_2_49_0_0_478:

            oidPath += "/[Alerting authorities of Mauritania]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Office National de la Météorologie]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // mu
        #region 2.49.0.0.480.*

        oid_2_49_0_0_480:

            oidPath += "/[Alerting authorities of Mauritius]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Mauritius Meteorological Services]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // mx
        #region 2.49.0.0.484.*

        oid_2_49_0_0_484:

            oidPath += "/[Alerting authorities of Mexico]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Servicio Meteorológico Nacional]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // mc
        #region 2.49.0.0.492.*

        oid_2_49_0_0_492:

            oidPath += "/[Alerting authorities of Monaco]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Mission Permanente de la Principauté de Monaco]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // mn
        #region 2.49.0.0.496.*

        oid_2_49_0_0_496:

            oidPath += "/[Alerting authorities of Mongolia]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[National Agency For Meteorology, Hydrology and Environment Monitoring]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // md
        #region 2.49.0.0.498.*

        oid_2_49_0_0_498:

            oidPath += "/[Alerting authorities of Republic of Moldova]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[State Hydrometeorological Service]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // me
        #region 2.49.0.0.499.*

        oid_2_49_0_0_499:

            oidPath += "/[Alerting authorities of Montenegro]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Institute of Hydrometeorology and Seismology of Montenegro]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // me
        #region 2.49.0.0.500.*

        oid_2_49_0_0_500:

            oidPath += "/[Alerting authorities of Montserrat]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Disaster Management Coordination Agency]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // ma
        #region 2.49.0.0.504.*

        oid_2_49_0_0_504:

            oidPath += "/[Alerting authorities of Morocco]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Direction de la Météorologie Natinale]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // mz
        #region 2.49.0.0.508.*

        oid_2_49_0_0_508:

            oidPath += "/[Alerting authorities of Mozambique]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Instituto Nacional de Meteorologia]",
                1 => $"{oidPath}/[Direcccion Nacional de Aqua]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // om
        #region 2.49.0.0.512.*

        oid_2_49_0_0_512:

            oidPath += "/[Alerting authorities of Oman]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Directorate General of Meteorology and Air Navigation]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // na
        #region 2.49.0.0.516.*

        oid_2_49_0_0_516:

            oidPath += "/[Alerting authorities of Namibia]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Namibia Meteorological Service]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // na
        #region 2.49.0.0.524.*

        oid_2_49_0_0_524:

            oidPath += "/[Alerting authorities of Nepal]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Department of Hydrology and Meteorology]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // nl
        #region 2.49.0.0.528.*

        oid_2_49_0_0_528:

            oidPath += "/[Alerting authorities of Netherlands]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Royal Netherlands Meteorological Institute]",
                1 => $"{oidPath}/[Wageningen University and Research Centre]",
                2 => $"{oidPath}/[Rijkswaterstaat]",
                3 => $"{oidPath}/[Aruba Meteorological Department]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // an
        #region 2.49.0.0.530.*

        oid_2_49_0_0_530:

            oidPath += "/[Alerting authorities of Curacao and Sint Maarten]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Meteorological Department Curacao]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // mf
        #region 2.49.0.0.540.*

        oid_2_49_0_0_540:

            oidPath += "/[Alerting authorities of New Caledonia]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Météo France]",
                1 => $"{oidPath}/[Securite civile de la Nouvelle Caledonie]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // vu
        #region 2.49.0.0.548.*

        oid_2_49_0_0_548:

            oidPath += "/[Alerting authorities of Vanuatu]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Vanuatu Meteorological Services]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // nz
        #region 2.49.0.0.554.*

        oid_2_49_0_0_554:

            oidPath += "/[Alerting authorities of New Zealand]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: return $"{oidPath}/[Meteorological Service of New Zealand Limited]";
                case 1: return $"{oidPath}/[Institute of Geological & Nuclear Sciences (GNS) Ltd., trading as GNS Science]";
                case 2: goto oid_2_49_0_0_554_2;
                case 3: return $"{oidPath}/[New Zealand Ministry of Health]";
                case 4: return $"{oidPath}/[Fire and Emergency New Zealand]";
                case 5: return $"{oidPath}/[New Zealand Police]";
                case 6: return $"{oidPath}/[New Zealand Transport Agency]";
                case 7: return $"{oidPath}/[Ministry for Primary Industries]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // 2
        #region 2.49.0.0.554.2.*

        oid_2_49_0_0_554_2:

            oidPath += "/[National Emergency Management Agency]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Northland Civil Defence Emergency Management (CDEM) group]",
                2 => $"{oidPath}/[Auckland Civil Defence Emergency Management (CDEM) group]",
                3 => $"{oidPath}/[Waikato Civil Defence Emergency Management (CDEM) group]",
                4 => $"{oidPath}/[Bay of Plenty Civil Defence Emergency Management (CDEM) group]",
                5 => $"{oidPath}/[Gisborne Civil Defence Emergency Management (CDEM) group]",
                6 => $"{oidPath}/[Hawkes Bay Civil Defence Emergency Management (CDEM) group]",
                7 => $"{oidPath}/[Manawatu Whanganui Civil Defence Emergency Management (CDEM) group]",
                8 => $"{oidPath}/[Taranaki Civil Defence Emergency Management (CDEM) group]",
                9 => $"{oidPath}/[Wellington Civil Defence Emergency Management (CDEM) group]",
                10 => $"{oidPath}/[Nelson Tasman Civil Defence Emergency Management (CDEM) group]",
                11 => $"{oidPath}/[Marlborough Civil Defence Emergency Management (CDEM) group]",
                12 => $"{oidPath}/[Canterbury Civil Defence Emergency Management (CDEM) group]",
                13 => $"{oidPath}/[West Coast Civil Defence Emergency Management (CDEM) group]",
                14 => $"{oidPath}/[Otago Civil Defence Emergency Management (CDEM) group]",
                15 => $"{oidPath}/[Southland Civil Defence Emergency Management (CDEM) group]",
                16 => $"{oidPath}/[Chatham Islands Civil Defence Emergency Management (CDEM) group]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // ni
        #region 2.49.0.0.558.*

        oid_2_49_0_0_558:

            oidPath += "/[Alerting authorities of Nicaragua]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Dirección General de Meteorología]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // ne
        #region 2.49.0.0.562.*

        oid_2_49_0_0_562:

            oidPath += "/[Alerting authorities of Niger]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Direction de la Météorologie Nationale]",
                1 => $"{oidPath}/[Ministère des Ressources en Eau]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // ng
        #region 2.49.0.0.566.*

        oid_2_49_0_0_566:

            oidPath += "/[Alerting authorities of Nigeria]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Nigerian Meteorological Agency]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // nu
        #region 2.49.0.0.570.*

        oid_2_49_0_0_570:

            oidPath += "/[Alerting authorities of Niue]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Niue Meteorological Service]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // no
        #region 2.49.0.0.578.*

        oid_2_49_0_0_578:

            oidPath += "/[Alerting authorities of Norway]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Norwegian Meteorological Institute]",
                1 => $"{oidPath}/[Norwegian Water Resources and Energy Directorate]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // fm
        #region 2.49.0.0.583.*

        oid_2_49_0_0_583:

            oidPath += "/[Alerting authorities of Federated States of Micronesia]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[FSM Weather Station]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // pw
        #region 2.49.0.0.585.*

        oid_2_49_0_0_585:

            oidPath += "/[Alerting authorities of Palau]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Palau Weather Service Office]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // pk
        #region 2.49.0.0.586.*

        oid_2_49_0_0_586:

            oidPath += "/[Alerting authorities of Pakistan]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Pakistan Meteorological Department]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // pa
        #region 2.49.0.0.591.*

        oid_2_49_0_0_591:

            oidPath += "/[Alerting authorities of Panama]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Hidrometeorología]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // pg
        #region 2.49.0.0.598.*

        oid_2_49_0_0_598:

            oidPath += "/[Alerting authorities of Papua New Guinea]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Papua New Guinea Meteorological Service]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // py
        #region 2.49.0.0.600.*

        oid_2_49_0_0_600:

            oidPath += "/[Alerting authorities of Paraguay]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Dirección de Meteorología e Hidrología]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // pe
        #region 2.49.0.0.604.*

        oid_2_49_0_0_604:

            oidPath += "/[Alerting authorities of Peru]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Servicio Nacional de Meteorologia e Hidrologia]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // ph
        #region 2.49.0.0.608.*

        oid_2_49_0_0_608:

            oidPath += "/[Alerting authorities of Philippines]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Philippine Atmospheric Geophysical and Astronomical Services Administration]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // pl
        #region 2.49.0.0.616.*

        oid_2_49_0_0_616:

            oidPath += "/[Alerting authorities of Poland]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Institute of Meteorology and Water Management]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // pt
        #region 2.49.0.0.620.*

        oid_2_49_0_0_620:

            oidPath += "/[Alerting authorities of Portugal]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Instituto Português do Mar e da Atmosfera, I.P.]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // gw
        #region 2.49.0.0.624.*

        oid_2_49_0_0_624:

            oidPath += "/[Alerting authorities of Guinea-Bissau]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Météorologie de Guinée Bissau]",
                1 => $"{oidPath}/[Direcção-General dos Recursos Hidrico]",
                3 => $"{oidPath}/[The epedimologique Health Service of Guinea-Bissau]",
                4 => $"{oidPath}/[Ministere of the health of the Guinea-Bissau]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // tl
        #region 2.49.0.0.626.*

        oid_2_49_0_0_626:

            oidPath += "/[Alerting authorities of Timor-Leste]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Dirrecão Nacional Meteorologia e Geofisica]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // qa
        #region 2.49.0.0.634.*

        oid_2_49_0_0_634:

            oidPath += "/[Alerting authorities of Qatar]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Civil Aviation Authority]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // ro
        #region 2.49.0.0.642.*

        oid_2_49_0_0_642:

            oidPath += "/[Alerting authorities of Romania]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[National Meteorological Administration]",
                1 => $"{oidPath}/[National Institute of Hydrology and Water Management]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // ru
        #region 2.49.0.0.643.*

        oid_2_49_0_0_643:

            oidPath += "/[Alerting authorities of Russian Federation]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Russian Federal Service for Hydrometeorology and Environmental Monitoring]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // rw
        #region 2.49.0.0.646.*

        oid_2_49_0_0_646:

            oidPath += "/[Alerting authorities of Rwanda]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Rwanda Meteorological Service]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // ai
        #region 2.49.0.0.660.*

        oid_2_49_0_0_660:

            oidPath += "/[Alerting authorities of Anguilla]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Disaster Management Anguilla]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // lc
        #region 2.49.0.0.662.*

        oid_2_49_0_0_662:

            oidPath += "/[Alerting authorities of Saint Lucia]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Meteorological Services]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // vc
        #region 2.49.0.0.670.*

        oid_2_49_0_0_670:

            oidPath += "/[Alerting authorities of Saint Vincent and the Grenadines]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Caribbean Meteorological Organization]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // st
        #region 2.49.0.0.678.*

        oid_2_49_0_0_678:

            oidPath += "/[Alerting authorities of Sao Tome and Principe]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Institut National de Météorologie]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // sa
        #region 2.49.0.0.682.*

        oid_2_49_0_0_682:

            oidPath += "/[Alerting authorities of Saudi Arabia]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Presidency of Meteorology and Environment]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // sn
        #region 2.49.0.0.686.*

        oid_2_49_0_0_686:

            oidPath += "/[Alerting authorities of Senegal]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Direction de la Meteorologie Nationale]",
                1 => $"{oidPath}/[Direction de l'Hydraulique Rurale et du Réseau Hydrographique National]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // rs
        #region 2.49.0.0.688.*

        oid_2_49_0_0_688:

            oidPath += "/[Alerting authorities of Serbia]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Republic Hydrometeorological Service of Serbia]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // sc
        #region 2.49.0.0.690.*

        oid_2_49_0_0_690:

            oidPath += "/[Alerting authorities of Seychelles]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[National Meteorological Services]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // sl
        #region 2.49.0.0.694.*

        oid_2_49_0_0_694:

            oidPath += "/[Alerting authorities of Sierra Leone]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Meteorological Department]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // sg
        #region 2.49.0.0.702.*

        oid_2_49_0_0_702:

            oidPath += "/[Alerting authorities of Singapore]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Meteorological Services Singapore]",
                1 => $"{oidPath}/[National Environment Agency]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // sk
        #region 2.49.0.0.703.*

        oid_2_49_0_0_703:

            oidPath += "/[Alerting authorities of Slovakia]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Slovak Hydrometeorological Institute]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // vn
        #region 2.49.0.0.704.*

        oid_2_49_0_0_704:

            oidPath += "/[Alerting authorities of Socialist Republic of Viet Nam]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Hydrometeorological Service]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // si
        #region 2.49.0.0.705.*

        oid_2_49_0_0_705:

            oidPath += "/[Alerting authorities of Slovenia]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Slovenian Environment Agency - ARSO]",
                1 => $"{oidPath}/[National Meteorological Service (ARSO/meteo.si - Slovenian Environment Agency/Meteorological Office)]",
                2 => $"{oidPath}/[National Hydrological Service (ARSO/hydro.si - Slovenian Environment Agency/Hydrology and State of the Environment Office)]",
                3 => $"{oidPath}/[National Seismological Service (ARSO - Slovenian Environment Agency/Seismology and Geology Office)]",
                10 => $"{oidPath}/[Civil Protection and Disaster Relief Administration of the Republic of Slovenia (\"URSZR\")]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // so
        #region 2.49.0.0.706.*

        oid_2_49_0_0_706:

            oidPath += "/[Alerting authorities of Somalia]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Permanent Mission of Somalia]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // za
        #region 2.49.0.0.710.*

        oid_2_49_0_0_710:

            oidPath += "/[Alerting authorities of South Africa]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[South African Weather Service]",
                1 => $"{oidPath}/[Department of Water Affairs and Forestry]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // za
        #region 2.49.0.0.716.*

        oid_2_49_0_0_716:

            oidPath += "/[Alerting authorities of Zimbabwe]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Zimbabwe Meteorological Services Department]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // es
        #region 2.49.0.0.724.*

        oid_2_49_0_0_724:

            oidPath += "/[Alerting authorities of Spain]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Agencia Estatal de Meteorología]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // ss
        #region 2.49.0.0.728.*

        oid_2_49_0_0_728:

            oidPath += "/[Alerting authorities of South Sudan]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[South Sudan Weather Service]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // sd
        #region 2.49.0.0.729.*

        oid_2_49_0_0_729:

            oidPath += "/[Alerting authorities of Sudan]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Sudan Meteorological Authority]",
                1 => $"{oidPath}/[Nile Waters Department]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // sr
        #region 2.49.0.0.740.*

        oid_2_49_0_0_740:

            oidPath += "/[Alerting authorities of Suriname]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Meteorological Service]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // sr
        #region 2.49.0.0.748.*

        oid_2_49_0_0_748:

            oidPath += "/[Alerting authorities of Eswatini (formerly, Kingdom of Swaziland)]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Swaziland Meteorological Services]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // se
        #region 2.49.0.0.752.*

        oid_2_49_0_0_752:

            oidPath += "/[Alerting authorities of Sweden]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Swedish Meteorological and Hydrological Institute]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // ch
        #region 2.49.0.0.756.*

        oid_2_49_0_0_756:

            oidPath += "/[Alerting authorities of Switzerland]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[MeteoSwiss, Bundesamt für Meteorologie und Klimatologie]",
                1 => $"{oidPath}/[Federal Office for the Environment, Bundesamt für Umwelt]",
                2 => $"{oidPath}/[Swiss Seismological Service, Schweizerischer Erdbebendienst]",
                3 => $"{oidPath}/[WSL Institute for Snow and Avalanche Research SLF, WSL-Institut für Schnee- und Lawinenforschung SLF]",
                4 => $"{oidPath}/[Federal Office for Civil Protection, National Emergency Operation Centre, Nationale Alarmzentrale, Bundesamt für Bevölkerungsschutz]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // sy
        #region 2.49.0.0.760.*

        oid_2_49_0_0_760:

            oidPath += "/[Alerting authorities of Syrian Arab Republic]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Ministry of Defence Meteorological Department]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // tj
        #region 2.49.0.0.762.*

        oid_2_49_0_0_762:

            oidPath += "/[Alerting authorities of Tajikistan]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Main Administration of Hydrometeorology and Monitoring of the Environment]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // th
        #region 2.49.0.0.764.*

        oid_2_49_0_0_764:

            oidPath += "/[Alerting authorities of Thailand]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Thai Meteorological Department]",
                1 => $"{oidPath}/[National Disaster Warning Center (NDWC)]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // tg
        #region 2.49.0.0.768.*

        oid_2_49_0_0_768:

            oidPath += "/[Alerting authorities of Togo]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Direction de la Météorologie Nationale]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // to
        #region 2.49.0.0.776.*

        oid_2_49_0_0_776:

            oidPath += "/[Alerting authorities of Tonga]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Tonga Meteorological Service]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // tt
        #region 2.49.0.0.780.*

        oid_2_49_0_0_780:

            oidPath += "/[Alerting authorities of Trinidad and Tobago]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Meteorological Service]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // ae
        #region 2.49.0.0.784.*

        oid_2_49_0_0_784:

            oidPath += "/[Alerting authorities of United Arab Emirates]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Meteorological Department]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // tn
        #region 2.49.0.0.788.*

        oid_2_49_0_0_788:

            oidPath += "/[Alerting authorities of Tunisia]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[National Institute of Meteorology]",
                1 => $"{oidPath}/[Direction Nationale de la Gestion des Ressources en Eau]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // tr
        #region 2.49.0.0.792.*

        oid_2_49_0_0_792:

            oidPath += "/[Alerting authorities of Turkey]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Turkish State Meteorological Service]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // tm
        #region 2.49.0.0.795.*

        oid_2_49_0_0_795:

            oidPath += "/[Alerting authorities of Turkmenistan]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Administration of Hydrometeorology]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // tc
        #region 2.49.0.0.796.*

        oid_2_49_0_0_796:

            oidPath += "/[Alerting authorities of Turks and Caicos Islands]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Department of Disaster Management and Emergencies]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // tv
        #region 2.49.0.0.798.*

        oid_2_49_0_0_798:

            oidPath += "/[Alerting authorities of Tuvalu]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Tuvalu Met Service]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // ug
        #region 2.49.0.0.800.*

        oid_2_49_0_0_800:

            oidPath += "/[Alerting authorities of Uganda]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Department of Meteorology]",
                1 => $"{oidPath}/[Directorate for Water Development]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // ua
        #region 2.49.0.0.804.*

        oid_2_49_0_0_804:

            oidPath += "/[Alerting authorities of Ukraine]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[State Hydrometeorological Service]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // mk
        #region 2.49.0.0.807.*

        oid_2_49_0_0_807:

            oidPath += "/[Alerting authorities of The Former Yugoslav Republic of Macedonia]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Republic Hydrometeorological Organization]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // eg
        #region 2.49.0.0.818.*

        oid_2_49_0_0_818:

            oidPath += "/[Alerting authorities of Egypt]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Egyptian Meteorological Authority]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // gb
        #region 2.49.0.0.826.*

        oid_2_49_0_0_826:

            oidPath += "/[Alerting authorities of United Kingdom of Great Britain and Northern Ireland]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Met Office]",
                1 => $"{oidPath}/[Centre for Ecology and Hydrology]",
                2 => $"{oidPath}/[Caribbean Meteorological Organization]",
                3 => $"{oidPath}/[Bermuda Weather Service]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // tz
        #region 2.49.0.0.834.*

        oid_2_49_0_0_834:

            oidPath += "/[Alerting authorities of United Republic of Tanzania]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Tanzania Meteorological Agency]",
                1 => $"{oidPath}/[PMO-Disaster management department]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // us
        #region 2.49.0.0.840.*

        oid_2_49_0_0_840:

            oidPath += "/[Alerting authorities of United States of America]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[National Oceanic and Atmospheric Administration]",
                1 => $"{oidPath}/[National Oceanic and Atmospheric Administration (NOAA), National Tsunami Warning Center]",
                2 => $"{oidPath}/[United States Geological Survey, Earthquakes]",
                3 => $"{oidPath}/[Environmental Protection Agency, Air Quality Alerts]",
                4 => $"{oidPath}/[Federal Emergency Management Agency, Integrated Public Alert and Warning System]",
                5 => $"{oidPath}/[United States Geological Survey, Volcano Hazards Program]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // bf
        #region 2.49.0.0.854.*

        oid_2_49_0_0_854:

            oidPath += "/[Alerting authorities of Burkina Faso]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Direction de la Météorologie]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // uy
        #region 2.49.0.0.858.*

        oid_2_49_0_0_858:

            oidPath += "/[Alerting authorities of Uruguay]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Direccion Nacional de Meteorologia]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // uz
        #region 2.49.0.0.860.*

        oid_2_49_0_0_860:

            oidPath += "/[Alerting authorities of Uzbekistan]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Uzhydromet]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // ve
        #region 2.49.0.0.862.*

        oid_2_49_0_0_862:

            oidPath += "/[Alerting authorities of Venezuela]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Servicio de Meteorologia de la Aviacion]",
                1 => $"{oidPath}/[Dirección de Meteorología e Hidrología - Ministerio del Ambiente]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // ws
        #region 2.49.0.0.882.*

        oid_2_49_0_0_882:

            oidPath += "/[Alerting authorities of Samoa]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Samoa Meteorology Division]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // ye
        #region 2.49.0.0.887.*

        oid_2_49_0_0_887:

            oidPath += "/[Alerting authorities of Republic of Yemen]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Yemen Meteorological Service]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // zm
        #region 2.49.0.0.894.*

        oid_2_49_0_0_894:

            oidPath += "/[Alerting authorities of Zambia]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Zambia Meteorological Department]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // org
        #region 2.49.0.2.*

        oid_2_49_0_2:

            oidPath += "/[Alerting authorities of other organizations]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[EUMETNET]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // org-msg
        #region 2.49.0.3.*

        oid_2_49_0_3:

            oidPath += "/[Alerting messages of other organizations]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                0 => $"{oidPath}/[Alert messages issued by EUMETNET]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        // ors
        #region 2.50.*

        oid_2_50:

            oidPath += "/OIDResolutionSystem";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_2_50_0;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // modules
        #region 2.50.0.*

        oid_2_50_0:

            oidPath += "/[ASN.1 modules]";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 0: goto oid_2_50_0_0;
                case 1: goto oid_2_50_0_1;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // cinf
        #region 2.50.0.0.*

        oid_2_50_0_0:

            oidPath += "/[CINF-module]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Version 1]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // rinf
        #region 2.50.0.1.*

        oid_2_50_0_1:

            oidPath += "/[RINF-module]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Version 1]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        #endregion

        // gs1
        #region 2.51.*

        oid_2_51:

            oidPath += "/GS1";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: goto oid_2_51_1;
                case 2: goto oid_2_51_2;
                case 3: return $"{oidPath}/[GS1 business data]";
                case 4: return $"{oidPath}/[GS1 technical data]";
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // 1
        #region 2.51.1.*

        oid_2_51_1:

            oidPath += "/[GS1 identification keys]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Global Trade Item Number (the GS1 Identification Key used to identify trade items)]",
                2 => $"{oidPath}/[Serial Shipping Container Code (GS1 Identification Key used to identify logistics units)]",
                3 => $"{oidPath}/[Global Location Number (GS1 Identification Key used to identify physical locations or parties)]",
                4 => $"{oidPath}/[Global Returnable Asset Identifier (GS1 Identification Key used to identify Returnable Assets)]",
                5 => $"{oidPath}/[Global Individual Asset Identifier (GS1 Identification Key used to identify an Individual Asset)]",
                6 => $"{oidPath}/[Global Document Type Identifier (GS1 Identification Key used to identify a document type)]",
                7 => $"{oidPath}/[Global Service Relation Number (a non-significant number used to identify the relationship between an organization offering services and the individual entities providing or benefiting from the services)]",
                8 => $"{oidPath}/[Global Shipment Identification Number (GSIN)]",
                9 => $"{oidPath}/[Global Identification Number for Consignment (GS1 Identification Key used to identify a logical grouping of logistic or transport units that are assembled to be transported under one transport document, e.g., HWB)]",
                10 => $"{oidPath}/[Global Coupon Number (GS1 Identification Key that provides a globally unique identification for a coupon, with an optional serial number)]",
                11 => $"{oidPath}/[Component or Part IDentifier (CPID)]",
                12 => $"{oidPath}/[Global Model Number (GMN) used to identify a product model]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // 2
        #region 2.51.2.*

        oid_2_51_2:

            oidPath += "/[GS1 supplementary data]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                10 => $"{oidPath}/[Number that associates an item with information the manufacturer considers relevant for traceability purposes]",
                21 => $"{oidPath}/[Numeric or alphanumeric code assigned to an individual instance of an entity for its lifetime]",
                254 => $"{oidPath}/[Alphanumeric extension component used to identify internal physical locations within a location]",
                8011 => $"{oidPath}/[Numeric identifier assigned to an individual instance of a component/part]",
                8019 => $"{oidPath}/[Numeric identifier allowing to distinguish different encounters during a service relationship]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        #endregion

        // uav
        #region 2.52.*

        oid_2_52:

            oidPath += "/UAV";
            if (index == values.Length) return oidPath;
            switch (values[index++])
            {
                case 1: return $"{oidPath}/[Unmanned Aerial Vehicle (UAV) devices]";
                case 2: return $"{oidPath}/[Ground control stations]";
                case 3: return $"{oidPath}/[Monitoring devices]";
                case 4: goto oid_2_52_4;
                case 5: return $"{oidPath}/[Security modules]";
                case 6: goto oid_2_52_6;
                default: return $"{oidPath}/{values[index - 1]}";
            }

        // 4
        #region 2.52.4.*

        oid_2_52_4:

            oidPath += "/[Data modules for the full life-cycle management of Unmanned Aerial Vehicles (UAVs)]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Data modules of a manufacturing system]",
                2 => $"{oidPath}/[Data modules of a sales and logistical system]",
                3 => $"{oidPath}/[Data modules of a repairing system]",
                4 => $"{oidPath}/[Data modules of a scrapping system]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

        #endregion

        // 6
        #region 2.52.6.*

        oid_2_52_6:

            oidPath += "/[Data modules for identity recognition of Unmanned Aerial Vehicles (UAVs)]";
            if (index == values.Length) return oidPath;
            return values[index++] switch
            {
                1 => $"{oidPath}/[Data modules of Unmanned Aerial Vehicle (UAV) systems]",
                2 => $"{oidPath}/[Data modules of Unmanned Aerial Vehicle (UAV) Ground Control Station (GCS) systems]",
                3 => $"{oidPath}/[Data modules of Unmanned Aerial Vehicle (UAV) Monitoring and Control Station/server (MCS) systems]",
                4 => $"{oidPath}/[Data modules of Unmanned Aerial Vehicle (UAV) monitoring cloud systems]",
                _ => $"{oidPath}/{values[index - 1]}",
            };

            #endregion

            #endregion

            #endregion
        }
    }
}

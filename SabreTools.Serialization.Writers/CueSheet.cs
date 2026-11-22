using System;
using System.IO;
using System.Text;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.CueSheets;
using SabreTools.IO.Extensions;

#pragma warning disable CA1510 // Use ArgumentNullException throw helper
namespace SabreTools.Serialization.Writers
{
    public class CueSheet : BaseBinaryWriter<Data.Models.CueSheets.CueSheet>
    {
        /// <inheritdoc/>
        public override Stream? SerializeStream(Data.Models.CueSheets.CueSheet? obj)
        {
            // If the cuesheet is null
            if (obj is null)
                return null;

            // If we don't have any files, it's invalid
            if (obj?.Files is null)
                throw new InvalidDataException(nameof(obj.Files));
            else if (obj.Files.Length == 0)
                throw new ArgumentException("No files provided to write");

            // Setup the writer and output
            var stream = new MemoryStream();
#if NET20 || NET35 || NET40
            var writer = new StreamWriter(stream, new UTF8Encoding(false), 1024);
#else
            var writer = new StreamWriter(stream, new UTF8Encoding(false), 1024, true);
#endif

            // Write the file
            WriteCueSheet(obj, writer);
            writer.Flush();

            // Return the stream
            stream.SeekIfPossible(0, SeekOrigin.Begin);
            return stream;
        }

        /// <summary>
        /// Write the cuesheet out to a stream
        /// </summary>
        /// <param name="cueSheet">CueSheet to write</param>
        /// <param name="sw">StreamWriter to write to</param>
        private static void WriteCueSheet(Data.Models.CueSheets.CueSheet cueSheet, StreamWriter sw)
        {
            // If we don't have any files, it's invalid
            if (cueSheet.Files is null)
                throw new InvalidDataException(nameof(cueSheet.Files));
            else if (cueSheet.Files.Length == 0)
                throw new ArgumentException("No files provided to write");

            if (!string.IsNullOrEmpty(cueSheet.Catalog))
                sw.WriteLine($"CATALOG {cueSheet.Catalog}");

            if (!string.IsNullOrEmpty(cueSheet.CdTextFile))
                sw.WriteLine($"CDTEXTFILE {cueSheet.CdTextFile}");

            if (!string.IsNullOrEmpty(cueSheet.Performer))
                sw.WriteLine($"PERFORMER \"{cueSheet.Performer}\"");

            if (!string.IsNullOrEmpty(cueSheet.Songwriter))
                sw.WriteLine($"SONGWRITER \"{cueSheet.Songwriter}\"");

            if (!string.IsNullOrEmpty(cueSheet.Title))
                sw.WriteLine($"TITLE \"{cueSheet.Title}\"");

            var remarks = new RemarkState();
            WriteRemarkLines(cueSheet.Remarks, sw, remarks);
            foreach (var cueFile in cueSheet.Files)
            {
                WriteCueFile(cueFile, sw, remarks);
            }
        }

        /// <summary>
        /// Write the FILE out to a stream
        /// </summary>
        /// <param name="cueFile">CueFile to write</param>
        /// <param name="sw">StreamWriter to write to</param>
        /// <param name="remarks">Session and density area from the REM lines written so far</param>
        private static void WriteCueFile(CueFile? cueFile, StreamWriter sw, RemarkState remarks)
        {
            // If we don't have any tracks, it's invalid
            if (cueFile?.Tracks is null)
                throw new InvalidDataException(nameof(cueFile.Tracks));

            // A file without tracks needs the indices of the last track of the previous file
            CueIndex[] indices = cueFile.Indices ?? [];
            if (cueFile.Tracks.Length == 0 && indices.Length == 0)
                throw new ArgumentException("No tracks provided to write");

            // A session or density area that starts with the file is noted before the FILE line,
            // or after the REM lines of the file if it has any, as they could note it themselves
            if ((cueFile.Remarks is null || cueFile.Remarks.Length == 0) && cueFile.Tracks.Length > 0)
                WriteRemarks(cueFile.Tracks[0], sw, remarks);

            sw.WriteLine($"FILE \"{cueFile.FileName}\" {FromFileType(cueFile.FileType)}");
            WriteRemarkLines(cueFile.Remarks, sw, remarks);

            // Indices of the last track of the previous file come before the first track of this file
            foreach (var index in indices)
            {
                WriteCueIndex(index, sw, remarks);
            }

            foreach (var track in cueFile.Tracks)
            {
                WriteRemarks(track, sw, remarks);
                WriteCueTrack(track, sw, remarks);
            }
        }

        /// <summary>
        /// Session and density area from the REM lines written so far
        /// </summary>
        private sealed class RemarkState
        {
            /// <summary>
            /// Session of the last REM SESSION line
            /// </summary>
            public int Session { get; set; }

            /// <summary>
            /// Density area of the last REM SINGLE-DENSITY AREA or REM HIGH-DENSITY AREA line
            /// </summary>
            public CueDensityArea DensityArea { get; set; }
        }

        /// <summary>
        /// Write REM lines of the model and read their session or density area like the reader
        /// </summary>
        /// <param name="lines">Text after the REM keyword of each line</param>
        /// <param name="sw">StreamWriter to write to</param>
        /// <param name="remarks">Session and density area from the REM lines written so far</param>
        private static void WriteRemarkLines(string[]? lines, StreamWriter sw, RemarkState remarks)
        {
            if (lines is null)
                return;

            foreach (string remark in lines)
            {
                sw.WriteLine(remark.Length == 0 ? "REM" : $"REM {remark}");

                int? session = CueSheetExtensions.GetSession(remark);
                if (session is not null)
                    remarks.Session = session.Value;

                var densityArea = CueSheetExtensions.GetDensityArea(remark);
                if (densityArea != CueDensityArea.None)
                    remarks.DensityArea = densityArea;
            }
        }

        /// <summary>
        /// Write the REM lines for a track that starts a new session or density area
        /// </summary>
        /// <param name="cueTrack">CueTrack to check</param>
        /// <param name="sw">StreamWriter to write to</param>
        /// <param name="remarks">Session and density area from the REM lines written so far</param>
        private static void WriteRemarks(CueTrack? cueTrack, StreamWriter sw, RemarkState remarks)
        {
            // A missing track is reported by WriteCueTrack
            if (cueTrack is null)
                return;

            if (cueTrack.Session != remarks.Session)
            {
                remarks.Session = cueTrack.Session;
                sw.WriteLine($"REM SESSION {remarks.Session:D2}");
            }

            if (cueTrack.DensityArea != remarks.DensityArea)
            {
                remarks.DensityArea = cueTrack.DensityArea;
                switch (remarks.DensityArea)
                {
                    case CueDensityArea.SingleDensity:
                        sw.WriteLine("REM SINGLE-DENSITY AREA");
                        break;

                    case CueDensityArea.HighDensity:
                        sw.WriteLine("REM HIGH-DENSITY AREA");
                        break;

                    case CueDensityArea.None:
                    default:
                        // No REM line ends a density area
                        break;
                }
            }
        }

        /// <summary>
        /// Write the TRACK out to a stream
        /// </summary>
        /// <param name="cueFile">CueFile to write</param>
        /// <param name="sw">StreamWriter to write to</param>
        /// <param name="remarks">Session and density area from the REM lines written so far</param>
        private static void WriteCueTrack(CueTrack? cueTrack, StreamWriter sw, RemarkState remarks)
        {
            // If we don't have any indices, it's invalid
            if (cueTrack?.Indices is null)
                throw new InvalidDataException(nameof(cueTrack.Indices));
            else if (cueTrack.Indices.Length == 0)
                throw new ArgumentException("No indices provided to write");

            sw.WriteLine($"  TRACK {cueTrack.Number:D2} {FromDataType(cueTrack.DataType)}");

            if (cueTrack.Flags != 0)
                sw.WriteLine($"    FLAGS {FromFlags(cueTrack.Flags)}");

            if (!string.IsNullOrEmpty(cueTrack.ISRC))
                sw.WriteLine($"    ISRC {cueTrack.ISRC}");

            if (!string.IsNullOrEmpty(cueTrack.Performer))
                sw.WriteLine($"    PERFORMER \"{cueTrack.Performer}\"");

            if (!string.IsNullOrEmpty(cueTrack.Songwriter))
                sw.WriteLine($"    SONGWRITER \"{cueTrack.Songwriter}\"");

            if (!string.IsNullOrEmpty(cueTrack.Title))
                sw.WriteLine($"    TITLE \"{cueTrack.Title}\"");

            WriteRemarkLines(cueTrack.Remarks, sw, remarks);

            if (cueTrack.PreGap is not null)
                WritePreGap(cueTrack.PreGap, sw);

            foreach (var index in cueTrack.Indices)
            {
                WriteCueIndex(index, sw, remarks);
            }

            if (cueTrack.PostGap is not null)
                WritePostGap(cueTrack.PostGap, sw);
        }

        /// <summary>
        /// Write the PREGAP out to a stream
        /// </summary>
        /// <param name="preGap">PreGap to write</param>
        /// <param name="sw">StreamWriter to write to</param>
        private static void WritePreGap(PreGap preGap, StreamWriter sw)
        {
            sw.WriteLine($"    PREGAP {preGap.Minutes:D2}:{preGap.Seconds:D2}:{preGap.Frames:D2}");
        }

        /// <summary>
        /// Write the INDEX out to a stream
        /// </summary>
        /// <param name="cueIndex">CueIndex to write</param>
        /// <param name="sw">StreamWriter to write to</param>
        /// <param name="remarks">Session and density area from the REM lines written so far</param>
        private static void WriteCueIndex(CueIndex? cueIndex, StreamWriter sw, RemarkState remarks)
        {
            if (cueIndex is null)
                throw new ArgumentNullException(nameof(cueIndex));

            sw.WriteLine($"    INDEX {cueIndex.Index:D2} {cueIndex.Minutes:D2}:{cueIndex.Seconds:D2}:{cueIndex.Frames:D2}");
            WriteRemarkLines(cueIndex.Remarks, sw, remarks);
        }

        /// <summary>
        /// Write the POSTGAP out to a stream
        /// </summary>
        /// <param name="postGap">PostGap to write</param>
        /// <param name="sw">StreamWriter to write to</param>
        private static void WritePostGap(PostGap postGap, StreamWriter sw)
        {
            sw.WriteLine($"    POSTGAP {postGap.Minutes:D2}:{postGap.Seconds:D2}:{postGap.Frames:D2}");
        }

        #region Helpers

        /// <summary>
        /// Get the string from a given file type
        /// </summary>
        /// <param name="fileType">CueFileType to get value from</param>
        /// <returns>String, if possible (default BINARY)</returns>
        private static string FromFileType(CueFileType fileType)
        {
            return fileType switch
            {
                CueFileType.BINARY => "BINARY",
                CueFileType.MOTOROLA => "MOTOROLA",
                CueFileType.AIFF => "AIFF",
                CueFileType.WAVE => "WAVE",
                CueFileType.MP3 => "MP3",
                _ => string.Empty,
            };
        }

        /// <summary>
        /// Get the string from a given data type
        /// </summary>
        /// <param name="dataType">CueTrackDataType to get value from</param>
        /// <returns>string, if possible</returns>
        private static string FromDataType(CueTrackDataType dataType)
        {
            return dataType switch
            {
                CueTrackDataType.AUDIO => "AUDIO",
                CueTrackDataType.CDG => "CDG",
                CueTrackDataType.MODE1_2048 => "MODE1/2048",
                CueTrackDataType.MODE1_2352 => "MODE1/2352",
                CueTrackDataType.MODE2_2336 => "MODE2/2336",
                CueTrackDataType.MODE2_2352 => "MODE2/2352",
                CueTrackDataType.CDI_2336 => "CDI/2336",
                CueTrackDataType.CDI_2352 => "CDI/2352",
                _ => string.Empty,
            };
        }

        /// <summary>
        /// Get the string value for a set of track flags
        /// </summary>
        /// <param name="flags">CueTrackFlag to get value from</param>
        /// <returns>String value representing the CueTrackFlag, if possible</returns>
        private static string FromFlags(CueTrackFlag flags)
        {
            string outputFlagString = string.Empty;

#if NET20 || NET35
            if ((flags & CueTrackFlag.DCP) != 0)
#else
            if (flags.HasFlag(CueTrackFlag.DCP))
#endif
                outputFlagString += "DCP ";

#if NET20 || NET35
            if ((flags & CueTrackFlag.FourCH) != 0)
#else
            if (flags.HasFlag(CueTrackFlag.FourCH))
#endif
                outputFlagString += "4CH ";

#if NET20 || NET35
            if ((flags & CueTrackFlag.PRE) != 0)
#else
            if (flags.HasFlag(CueTrackFlag.PRE))
#endif
                outputFlagString += "PRE ";

#if NET20 || NET35
            if ((flags & CueTrackFlag.SCMS) != 0)
#else
            if (flags.HasFlag(CueTrackFlag.SCMS))
#endif
                outputFlagString += "SCMS ";

#if NET20 || NET35
            if ((flags & CueTrackFlag.DATA) != 0)
#else
            if (flags.HasFlag(CueTrackFlag.DATA))
#endif
                outputFlagString += "DATA ";

            return outputFlagString.Trim();
        }

        #endregion
    }
}

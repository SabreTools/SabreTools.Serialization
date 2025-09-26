using System;
using System.IO;
using System.Text;
using SabreTools.Data.Models.CueSheets;

namespace SabreTools.Serialization.Writers
{
    public class CueSheet : BaseBinaryWriter<Data.Models.CueSheets.CueSheet>
    {
        /// <inheritdoc/>
        public override Stream? SerializeStream(Data.Models.CueSheets.CueSheet? obj)
        {
            // If the cuesheet is null
            if (obj == null)
                return null;

            // If we don't have any files, it's invalid
            if (obj?.Files == null)
                throw new ArgumentNullException(nameof(obj.Files));
            else if (obj.Files.Length == 0)
                throw new ArgumentException("No files provided to write");

            // Setup the writer and output
            var stream = new MemoryStream();
#if NET20 || NET35 || NET40
            var writer = new StreamWriter(stream, Encoding.ASCII, 1024);
#else
            var writer = new StreamWriter(stream, Encoding.ASCII, 1024, true);
#endif

            // Write the file
            WriteCueSheet(obj, writer);

            // Return the stream
            stream.Seek(0, SeekOrigin.Begin);
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
            if (cueSheet.Files == null)
                throw new ArgumentNullException(nameof(cueSheet.Files));
            else if (cueSheet.Files.Length == 0)
                throw new ArgumentException("No files provided to write");

            if (!string.IsNullOrEmpty(cueSheet.Catalog))
                sw.WriteLine($"CATALOG {cueSheet.Catalog}");

            if (!string.IsNullOrEmpty(cueSheet.CdTextFile))
                sw.WriteLine($"CDTEXTFILE {cueSheet.CdTextFile}");

            if (!string.IsNullOrEmpty(cueSheet.Performer))
                sw.WriteLine($"PERFORMER {cueSheet.Performer}");

            if (!string.IsNullOrEmpty(cueSheet.Songwriter))
                sw.WriteLine($"SONGWRITER {cueSheet.Songwriter}");

            if (!string.IsNullOrEmpty(cueSheet.Title))
                sw.WriteLine($"TITLE {cueSheet.Title}");

            foreach (var cueFile in cueSheet.Files)
            {
                WriteCueFile(cueFile, sw);
            }
        }

        /// <summary>
        /// Write the FILE out to a stream
        /// </summary>
        /// <param name="cueFile">CueFile to write</param>
        /// <param name="sw">StreamWriter to write to</param>
        private static void WriteCueFile(CueFile? cueFile, StreamWriter sw)
        {
            // If we don't have any tracks, it's invalid
            if (cueFile?.Tracks == null)
                throw new ArgumentNullException(nameof(cueFile.Tracks));
            else if (cueFile.Tracks.Length == 0)
                throw new ArgumentException("No tracks provided to write");

            sw.WriteLine($"FILE \"{cueFile.FileName}\" {FromFileType(cueFile.FileType)}");

            foreach (var track in cueFile.Tracks)
            {
                WriteCueTrack(track, sw);
            }
        }

        /// <summary>
        /// Write the TRACK out to a stream
        /// </summary>
        /// <param name="cueFile">CueFile to write</param>
        /// <param name="sw">StreamWriter to write to</param>
        private static void WriteCueTrack(CueTrack? cueTrack, StreamWriter sw)
        {
            // If we don't have any indices, it's invalid
            if (cueTrack?.Indices == null)
                throw new ArgumentNullException(nameof(cueTrack.Indices));
            else if (cueTrack.Indices.Length == 0)
                throw new ArgumentException("No indices provided to write");

            sw.WriteLine($"  TRACK {cueTrack.Number:D2} {FromDataType(cueTrack.DataType)}");

            if (cueTrack.Flags != 0)
                sw.WriteLine($"    FLAGS {FromFlags(cueTrack.Flags)}");

            if (!string.IsNullOrEmpty(cueTrack.ISRC))
                sw.WriteLine($"    ISRC {cueTrack.ISRC}");

            if (!string.IsNullOrEmpty(cueTrack.Performer))
                sw.WriteLine($"    PERFORMER {cueTrack.Performer}");

            if (!string.IsNullOrEmpty(cueTrack.Songwriter))
                sw.WriteLine($"    SONGWRITER {cueTrack.Songwriter}");

            if (!string.IsNullOrEmpty(cueTrack.Title))
                sw.WriteLine($"    TITLE {cueTrack.Title}");

            if (cueTrack.PreGap != null)
                WritePreGap(cueTrack.PreGap, sw);

            foreach (var index in cueTrack.Indices)
            {
                WriteCueIndex(index, sw);
            }

            if (cueTrack.PostGap != null)
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
        private static void WriteCueIndex(CueIndex? cueIndex, StreamWriter sw)
        {
            if (cueIndex == null)
                throw new ArgumentNullException(nameof(cueIndex));

            sw.WriteLine($"    INDEX {cueIndex.Index:D2} {cueIndex.Minutes:D2}:{cueIndex.Seconds:D2}:{cueIndex.Frames:D2}");
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

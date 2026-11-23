using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.CueSheets;

namespace SabreTools.Serialization.Readers
{
    public class CueSheet : BaseBinaryReader<Data.Models.CueSheets.CueSheet>
    {
        /// <inheritdoc/>
        public override Data.Models.CueSheets.CueSheet? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data is null || !data.CanRead)
                return null;

            try
            {
                // Setup the reader and output
                var reader = new StreamReader(data, GetEncoding(data));
                var cueSheet = new Data.Models.CueSheets.CueSheet();
                var cueFiles = new List<CueFile>();
                int session = 0;
                var densityArea = CueDensityArea.None;
                List<string> sheetRemarks = [];

                // Read the next line from the input
                string? lastLine = null;
                while (lastLine is not null || !reader.EndOfStream)
                {
                    string? line = lastLine ?? reader.ReadLine();
                    lastLine = null;

                    // If we have a null line, break from the loop
                    if (line is null)
                        break;

                    // If we have an empty line, we skip
                    var splitLine = CueSheetExtensions.SplitLine(line);
                    if (splitLine.Length == 0)
                        continue;

                    switch (splitLine[0])
                    {
                        // Read comments
                        case "REM":
                            string remark = GetRemark(line);
                            CueSheetExtensions.ReadRemark(remark, ref session, ref densityArea);
                            sheetRemarks.Add(remark);
                            break;

                        // Read MCN
                        case "CATALOG":
                            if (splitLine.Length < 2)
                                throw new FormatException($"CATALOG line malformed: {line}");

                            cueSheet.Catalog = splitLine[1].Trim('"');
                            break;

                        // Read external CD-Text file path
                        case "CDTEXTFILE":
                            if (splitLine.Length < 2)
                                throw new FormatException($"CDTEXTFILE line malformed: {line}");

                            cueSheet.CdTextFile = splitLine[1].Trim('"');
                            break;

                        // Read CD-Text enhanced performer
                        case "PERFORMER":
                            cueSheet.Performer = GetTextValue(line, splitLine[0], reader, out lastLine);
                            break;

                        // Read CD-Text enhanced songwriter
                        case "SONGWRITER":
                            cueSheet.Songwriter = GetTextValue(line, splitLine[0], reader, out lastLine);
                            break;

                        // Read CD-Text enhanced title
                        case "TITLE":
                            cueSheet.Title = GetTextValue(line, splitLine[0], reader, out lastLine);
                            break;

                        // Read file information
                        case "FILE":
                            if (splitLine.Length < 3)
                                throw new FormatException($"FILE line malformed: {line}");

                            var file = CreateCueFile(splitLine[1], splitLine[2], reader, ref session, ref densityArea, out lastLine);
                            if (file == default)
                                throw new FormatException($"FILE line malformed: {line}");

                            cueFiles.Add(file);
                            break;

                        default:
                            // TODO: Log invalid values
                            break;
                    }
                }

                if (cueFiles.Count == 0)
                    return null;

                cueSheet.Remarks = [.. sheetRemarks];
                cueSheet.Files = [.. cueFiles];
                return cueSheet;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <summary>
        /// Fill a FILE from an array of lines
        /// </summary>
        /// <param name="fileName">File name to set</param>
        /// <param name="fileType">File type to set</param>
        /// <param name="reader">StreamReader to get lines from</param>
        /// <param name="session">Session from the REM lines read so far</param>
        /// <param name="densityArea">Density area from the REM lines read so far</param>
        private static CueFile? CreateCueFile(string fileName, string fileType, StreamReader reader, ref int session, ref CueDensityArea densityArea, out string? lastLine)
        {
            // Check the required parameters
            if (reader is null || reader.BaseStream.Length == 0 || !reader.BaseStream.CanRead)
                throw new ArgumentNullException(nameof(reader));

            // Create the holding objects
            lastLine = null;
            var cueTracks = new List<CueTrack>();
            List<string> fileRemarks = [];

            while (lastLine is not null || !reader.EndOfStream)
            {
                string? line = lastLine ?? reader.ReadLine();
                lastLine = null;

                // If we have a null line, break from the loop
                if (line is null)
                    break;

                // If we have an empty line, we skip
                var splitLine = CueSheetExtensions.SplitLine(line);
                if (splitLine.Length == 0)
                    continue;

                switch (splitLine[0])
                {
                    // Read comments
                    case "REM":
                        string remark = GetRemark(line);
                        CueSheetExtensions.ReadRemark(remark, ref session, ref densityArea);
                        fileRemarks.Add(remark);
                        break;

                    // Read track information
                    case "TRACK":
                        if (splitLine.Length < 3)
                            throw new FormatException($"TRACK line malformed: {line}");

                        var track = CreateCueTrack(splitLine[1], splitLine[2], reader, ref session, ref densityArea, out lastLine);
                        if (track == default)
                            throw new FormatException($"TRACK line malformed: {line}");

                        cueTracks.Add(track);
                        break;

                    // Next file found, return
                    case "FILE":
                        lastLine = line;
                        if (cueTracks.Count == 0)
                            return null;

                        return new CueFile
                        {
                            FileName = fileName.Trim('"'),
                            FileType = GetFileType(fileType),
                            Remarks = [.. fileRemarks],
                            Tracks = [.. cueTracks],
                        };

                    // Default means return
                    default:
                        lastLine = line;
                        if (cueTracks.Count == 0)
                            return null;

                        return new CueFile
                        {
                            FileName = fileName.Trim('"'),
                            FileType = GetFileType(fileType),
                            Remarks = [.. fileRemarks],
                            Tracks = [.. cueTracks],
                        };
                }
            }

            if (cueTracks.Count == 0)
                return null;

            return new CueFile
            {
                FileName = fileName.Trim('"'),
                FileType = GetFileType(fileType),
                Remarks = [.. fileRemarks],
                Tracks = [.. cueTracks],
            };
        }

        /// <summary>
        /// Fill a TRACK from an array of lines
        /// </summary>
        /// <param name="number">Number to set</param>
        /// <param name="dataType">Data type to set</param>
        /// <param name="reader">StreamReader to get lines from</param>
        /// <param name="session">Session from the REM lines read so far</param>
        /// <param name="densityArea">Density area from the REM lines read so far</param>
        private static CueTrack? CreateCueTrack(string number, string dataType, StreamReader reader, ref int session, ref CueDensityArea densityArea, out string? lastLine)
        {
            // Check the required parameters
            if (reader is null || reader.BaseStream.Length == 0 || !reader.BaseStream.CanRead)
                throw new ArgumentNullException(nameof(reader));
            if (reader.EndOfStream)
                throw new IndexOutOfRangeException();

            // Set the current fields
            if (!int.TryParse(number, out int parsedNumber))
                throw new ArgumentException($"Number was not a number: {number}");
            else if (parsedNumber < 1 || parsedNumber > 99)
                throw new IndexOutOfRangeException($"Index must be between 1 and 99: {parsedNumber}");

            // Create the holding objects
            lastLine = null;
            var cueTrack = new CueTrack();
            var cueIndices = new List<CueIndex>();
            List<string> cueRemarks = [];

            cueTrack.Number = parsedNumber;
            cueTrack.DataType = GetDataType(dataType);
            cueTrack.Session = session;
            cueTrack.DensityArea = densityArea;

            while (lastLine is not null || !reader.EndOfStream)
            {
                string? line = lastLine ?? reader.ReadLine();
                lastLine = null;

                // If we have a null line, break from the loop
                if (line is null)
                    break;

                // If we have an empty line, we skip
                var splitLine = CueSheetExtensions.SplitLine(line);
                if (splitLine.Length == 0)
                    continue;

                switch (splitLine[0])
                {
                    // Read comments
                    case "REM":
                        string remark = GetRemark(line);
                        CueSheetExtensions.ReadRemark(remark, ref session, ref densityArea);
                        cueRemarks.Add(remark);
                        break;

                    // Read flag information
                    case "FLAGS":
                        if (splitLine.Length < 2)
                            throw new FormatException($"FLAGS line malformed: {line}");

                        cueTrack.Flags = GetFlags([.. splitLine]);
                        break;

                    // Read International Standard Recording Code
                    case "ISRC":
                        if (splitLine.Length < 2)
                            throw new FormatException($"ISRC line malformed: {line}");

                        cueTrack.ISRC = splitLine[1].Trim('"');
                        break;

                    // Read CD-Text enhanced performer
                    case "PERFORMER":
                        cueTrack.Performer = GetTextValue(line, splitLine[0], reader, out lastLine);
                        break;

                    // Read CD-Text enhanced songwriter
                    case "SONGWRITER":
                        cueTrack.Songwriter = GetTextValue(line, splitLine[0], reader, out lastLine);
                        break;

                    // Read CD-Text enhanced title
                    case "TITLE":
                        cueTrack.Title = GetTextValue(line, splitLine[0], reader, out lastLine);
                        break;

                    // Read pregap information
                    case "PREGAP":
                        if (splitLine.Length < 2)
                            throw new FormatException($"PREGAP line malformed: {line}");

                        var pregap = CreatePreGap(splitLine[1]);
                        if (pregap == default)
                            throw new FormatException($"PREGAP line malformed: {line}");

                        cueTrack.PreGap = pregap;
                        break;

                    // Read index information
                    case "INDEX":
                        if (splitLine.Length < 3)
                            throw new FormatException($"INDEX line malformed: {line}");

                        var index = CreateCueIndex(splitLine[1], splitLine[2]);
                        if (index == default)
                            throw new FormatException($"INDEX line malformed: {line}");

                        SetRemarks(cueTrack, cueIndices, cueRemarks);
                        cueIndices.Add(index);
                        break;

                    // Read postgap information
                    case "POSTGAP":
                        if (splitLine.Length < 2)
                            throw new FormatException($"POSTGAP line malformed: {line}");

                        var postgap = CreatePostGap(splitLine[1]);
                        if (postgap == default)
                            throw new FormatException($"POSTGAP line malformed: {line}");

                        cueTrack.PostGap = postgap;
                        break;

                    // Next track or file found, return
                    case "TRACK":
                    case "FILE":
                        lastLine = line;
                        SetRemarks(cueTrack, cueIndices, cueRemarks);
                        cueTrack.Indices = [.. cueIndices];
                        return cueTrack;

                    // Default means return
                    default:
                        lastLine = line;
                        SetRemarks(cueTrack, cueIndices, cueRemarks);
                        cueTrack.Indices = [.. cueIndices];
                        return cueTrack;
                }
            }

            SetRemarks(cueTrack, cueIndices, cueRemarks);
            if (cueIndices.Count > 0)
                cueTrack.Indices = [.. cueIndices];

            return cueTrack;
        }

        /// <summary>
        /// Create a PREGAP from a mm:ss:ff length
        /// </summary>
        /// <param name="length">String to get length information from</param>
        private static PreGap CreatePreGap(string length)
        {
            // Ignore empty lines
            if (string.IsNullOrEmpty(length))
                throw new ArgumentException("Length was null or whitespace");

            // Ignore lines that don't contain the correct information
            if (length!.Length != 8)
                throw new FormatException($"Length was not in a recognized format: {length}");

            // Split the line
            string[] splitLength = length.Split(':');
            if (splitLength.Length != 3)
                throw new FormatException($"Length was not in a recognized format: {length}");

            // Parse the lengths
            int[] lengthSegments = new int[3];

            // Minutes
            if (!int.TryParse(splitLength[0], out lengthSegments[0]))
                throw new FormatException($"Minutes segment was not a number: {splitLength[0]}");
            else if (lengthSegments[0] < 0)
                throw new IndexOutOfRangeException($"Minutes segment must be 0 or greater: {lengthSegments[0]}");

            // Seconds
            if (!int.TryParse(splitLength[1], out lengthSegments[1]))
                throw new FormatException($"Seconds segment was not a number: {splitLength[1]}");
            else if (lengthSegments[1] < 0 || lengthSegments[1] > 60)
                throw new IndexOutOfRangeException($"Seconds segment must be between 0 and 60: {lengthSegments[1]}");

            // Frames
            if (!int.TryParse(splitLength[2], out lengthSegments[2]))
                throw new FormatException($"Frames segment was not a number: {splitLength[2]}");
            else if (lengthSegments[2] < 0 || lengthSegments[2] > 75)
                throw new IndexOutOfRangeException($"Frames segment must be between 0 and 75: {lengthSegments[2]}");

            // Set the values
            var preGap = new PreGap
            {
                Minutes = lengthSegments[0],
                Seconds = lengthSegments[1],
                Frames = lengthSegments[2],
            };
            return preGap;
        }

        /// <summary>
        /// Fill a INDEX from an array of lines
        /// </summary>
        /// <param name="index">Index to set</param>
        /// <param name="startTime">Start time to set</param>
        private static CueIndex CreateCueIndex(string index, string startTime)
        {
            // Set the current fields
            if (!int.TryParse(index, out int parsedIndex))
                throw new ArgumentException($"Index was not a number: {index}");
            else if (parsedIndex < 0 || parsedIndex > 99)
                throw new IndexOutOfRangeException($"Index must be between 0 and 99: {parsedIndex}");

            // Ignore empty lines
            if (string.IsNullOrEmpty(startTime))
                throw new ArgumentException("Start time was null or whitespace");

            // Ignore lines that don't contain the correct information
            if (startTime!.Length != 8)
                throw new FormatException($"Start time was not in a recognized format: {startTime}");

            // Split the line
            string[] splitTime = startTime.Split(':');
            if (splitTime.Length != 3)
                throw new FormatException($"Start time was not in a recognized format: {startTime}");

            // Parse the lengths
            int[] lengthSegments = new int[3];

            // Minutes
            if (!int.TryParse(splitTime[0], out lengthSegments[0]))
                throw new FormatException($"Minutes segment was not a number: {splitTime[0]}");
            else if (lengthSegments[0] < 0)
                throw new IndexOutOfRangeException($"Minutes segment must be 0 or greater: {lengthSegments[0]}");

            // Seconds
            if (!int.TryParse(splitTime[1], out lengthSegments[1]))
                throw new FormatException($"Seconds segment was not a number: {splitTime[1]}");
            else if (lengthSegments[1] < 0 || lengthSegments[1] > 60)
                throw new IndexOutOfRangeException($"Seconds segment must be between 0 and 60: {lengthSegments[1]}");

            // Frames
            if (!int.TryParse(splitTime[2], out lengthSegments[2]))
                throw new FormatException($"Frames segment was not a number: {splitTime[2]}");
            else if (lengthSegments[2] < 0 || lengthSegments[2] > 75)
                throw new IndexOutOfRangeException($"Frames segment must be between 0 and 75: {lengthSegments[2]}");

            // Set the values
            var cueIndex = new CueIndex
            {
                Index = parsedIndex,
                Minutes = lengthSegments[0],
                Seconds = lengthSegments[1],
                Frames = lengthSegments[2],
            };
            return cueIndex;
        }

        /// <summary>
        /// Create a POSTGAP from a mm:ss:ff length
        /// </summary>
        /// <param name="length">String to get length information from</param>
        private static PostGap CreatePostGap(string length)
        {
            // Ignore empty lines
            if (string.IsNullOrEmpty(length))
                throw new ArgumentException("Length was null or whitespace");

            // Ignore lines that don't contain the correct information
            if (length!.Length != 8)
                throw new FormatException($"Length was not in a recognized format: {length}");

            // Split the line
            string[] splitLength = length.Split(':');
            if (splitLength.Length != 3)
                throw new FormatException($"Length was not in a recognized format: {length}");

            // Parse the lengths
            int[] lengthSegments = new int[3];

            // Minutes
            if (!int.TryParse(splitLength[0], out lengthSegments[0]))
                throw new FormatException($"Minutes segment was not a number: {splitLength[0]}");
            else if (lengthSegments[0] < 0)
                throw new IndexOutOfRangeException($"Minutes segment must be 0 or greater: {lengthSegments[0]}");

            // Seconds
            if (!int.TryParse(splitLength[1], out lengthSegments[1]))
                throw new FormatException($"Seconds segment was not a number: {splitLength[1]}");
            else if (lengthSegments[1] < 0 || lengthSegments[1] > 60)
                throw new IndexOutOfRangeException($"Seconds segment must be between 0 and 60: {lengthSegments[1]}");

            // Frames
            if (!int.TryParse(splitLength[2], out lengthSegments[2]))
                throw new FormatException($"Frames segment was not a number: {splitLength[2]}");
            else if (lengthSegments[2] < 0 || lengthSegments[2] > 75)
                throw new IndexOutOfRangeException($"Frames segment must be between 0 and 75: {lengthSegments[2]}");

            // Set the values
            var postGap = new PostGap
            {
                Minutes = lengthSegments[0],
                Seconds = lengthSegments[1],
                Frames = lengthSegments[2],
            };
            return postGap;
        }

        #region Helpers

        /// <summary>
        /// Get the encoding of a cue sheet: UTF-8, or ISO 8859-1 if the bytes are not valid UTF-8
        /// </summary>
        /// <remarks>DiscImageCreator writes CD-Text as the bytes on the disc, which can be ISO 8859-1 (character code 0x00)</remarks>
        private static Encoding GetEncoding(Stream data)
        {
            long position = data.Position;
            var encoding = new UTF8Encoding(false, true);
            var decoder = encoding.GetDecoder();
            byte[] bytes = new byte[4096];
            char[] chars = new char[encoding.GetMaxCharCount(bytes.Length)];
            try
            {
                // The decoder keeps a sequence that is split between two reads
                int read;
                while ((read = data.Read(bytes, 0, bytes.Length)) > 0)
                {
                    decoder.GetChars(bytes, 0, read, chars, 0, false);
                }

                // An incomplete sequence at the end is not valid UTF-8 either
                decoder.GetChars(bytes, 0, 0, chars, 0, true);
                return Encoding.UTF8;
            }
            catch (DecoderFallbackException)
            {
                return Encoding.GetEncoding(28591);
            }
            finally
            {
                data.Seek(position, SeekOrigin.Begin);
            }
        }

        /// <summary>
        /// Get the text of a REM line: the rest of the line after the keyword
        /// </summary>
        private static string GetRemark(string line)
        {
            return line.TrimStart().Substring("REM".Length).Trim();
        }

        /// <summary>
        /// Set the REM lines read since the TRACK line or the last INDEX line on the track or on that index
        /// </summary>
        /// <param name="cueTrack">Track that is read</param>
        /// <param name="cueIndices">Indices of the track read so far</param>
        /// <param name="cueRemarks">REM lines read since the TRACK line or the last INDEX line, cleared afterwards</param>
        private static void SetRemarks(CueTrack cueTrack, List<CueIndex> cueIndices, List<string> cueRemarks)
        {
            if (cueIndices.Count == 0)
                cueTrack.Remarks = [.. cueRemarks];
            else
                cueIndices[cueIndices.Count - 1].Remarks = [.. cueRemarks];

            cueRemarks.Clear();
        }

        /// <summary>
        /// Get the value of a CD-Text line: the rest of the line after the keyword, without one pair of enclosing quotes
        /// </summary>
        /// <param name="line">Line with the keyword</param>
        /// <param name="keyword">Keyword of the line</param>
        /// <param name="reader">StreamReader to get the following lines of a value from</param>
        /// <param name="nextLine">Line after the value that was read to find its end, if any</param>
        /// <remarks>
        /// Values can contain quotes themselves, e.g. TITLE "Symphony No. 9 "From the New World"". They can also
        /// contain newlines, so a value that starts with a quote and does not end with one on its line goes on over
        /// the following lines up to the next command.
        /// </remarks>
        private static string GetTextValue(string line, string keyword, StreamReader reader, out string? nextLine)
        {
            nextLine = null;
            string value = line.TrimStart().Substring(keyword.Length).Trim();
            if (value.Length == 0)
                throw new FormatException($"{keyword} line malformed: {line}");

            if (value[0] == '"' && (value.Length == 1 || value[value.Length - 1] != '"'))
            {
                var builder = new StringBuilder(line.TrimStart().Substring(keyword.Length));
                while (!reader.EndOfStream)
                {
                    string next = reader.ReadLine() ?? string.Empty;
                    var splitLine = CueSheetExtensions.SplitLine(next);
                    if (splitLine.Length > 0 && Array.IndexOf(Constants.Commands, splitLine[0]) > -1)
                    {
                        nextLine = next;
                        break;
                    }

                    builder.Append('\n').Append(next);
                }

                value = builder.ToString().Trim();
            }

            if (value.Length > 1 && value[0] == '"' && value[value.Length - 1] == '"')
                value = value.Substring(1, value.Length - 2);

            return value;
        }

        /// <summary>
        /// Get the file type from a given string
        /// </summary>
        /// <param name="fileType">String to get value from</param>
        /// <returns>CueFileType, if possible</returns>
        private static CueFileType GetFileType(string? fileType)
        {
            return (fileType?.ToLowerInvariant()) switch
            {
                "binary" => CueFileType.BINARY,
                "motorola" => CueFileType.MOTOROLA,
                "aiff" => CueFileType.AIFF,
                "wave" => CueFileType.WAVE,
                "mp3" => CueFileType.MP3,
                _ => CueFileType.BINARY,
            };
        }

        /// <summary>
        /// Get the data type from a given string
        /// </summary>
        /// <param name="dataType">String to get value from</param>
        /// <returns>CueTrackDataType, if possible (default AUDIO)</returns>
        private static CueTrackDataType GetDataType(string? dataType)
        {
            return (dataType?.ToLowerInvariant()) switch
            {
                "audio" => CueTrackDataType.AUDIO,
                "cdg" => CueTrackDataType.CDG,
                "mode1/2048" => CueTrackDataType.MODE1_2048,
                "mode1/2352" => CueTrackDataType.MODE1_2352,
                "mode2/2336" => CueTrackDataType.MODE2_2336,
                "mode2/2352" => CueTrackDataType.MODE2_2352,
                "cdi/2336" => CueTrackDataType.CDI_2336,
                "cdi/2352" => CueTrackDataType.CDI_2352,
                _ => CueTrackDataType.AUDIO,
            };
        }

        /// <summary>
        /// Get the flag value for an array of strings
        /// </summary>
        /// <param name="flagStrings">Possible flags as strings</param>
        /// <returns>CueTrackFlag value representing the strings, if possible</returns>
        private static CueTrackFlag GetFlags(string[]? flagStrings)
        {
            CueTrackFlag flag = 0;
            if (flagStrings is null)
                return flag;

            foreach (string? flagString in flagStrings)
            {
                switch (flagString?.ToLowerInvariant())
                {
                    case "flags":
                        // No-op since this is the start of the line
                        break;

                    case "dcp":
                        flag |= CueTrackFlag.DCP;
                        break;

                    case "4ch":
                        flag |= CueTrackFlag.FourCH;
                        break;

                    case "pre":
                        flag |= CueTrackFlag.PRE;
                        break;

                    case "scms":
                        flag |= CueTrackFlag.SCMS;
                        break;

                    case "data":
                        flag |= CueTrackFlag.DATA;
                        break;

                    default:
                        // TODO: Log invalid values
                        break;
                }
            }

            return flag;
        }

        #endregion
    }
}

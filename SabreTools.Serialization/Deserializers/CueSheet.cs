using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using SabreTools.IO.Extensions;
using SabreTools.Models.CueSheets;

namespace SabreTools.Serialization.Deserializers
{
    public class CueSheet : BaseBinaryDeserializer<Models.CueSheets.CueSheet>
    {
        /// <inheritdoc/>
        public override Models.CueSheets.CueSheet? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            // If the offset is out of bounds
            if (data.Position < 0 || data.Position >= data.Length)
                return null;

            // Cache the current offset
            int initialOffset = (int)data.Position;

            // Create a new cuesheet to fill
            var cueSheet = new Models.CueSheets.CueSheet();
            var cueFiles = new List<CueFile>();

            // Read the next line from the input
            string? lastLine = null;
            while (true)
            {
                string? line = lastLine ?? ReadUntilNewline(data);
                lastLine = null;

                // If we have a null line, break from the loop
                if (line == null)
                    break;

                // If we have an empty line, we skip
                if (string.IsNullOrEmpty(line))
                    continue;

                // http://stackoverflow.com/questions/554013/regular-expression-to-split-on-spaces-unless-in-quotes
                var matchCol = Regex.Matches(line, @"[^\s""]+|""[^""]*""");
                var splitLine = new List<string>();
                foreach (Match? match in matchCol)
                {
                    if (match != null)
                        splitLine.Add(match.Groups[0].Value);
                }

                switch (splitLine[0])
                {
                    // Read comments
                    case "REM":
                        // We ignore all comments for now
                        break;

                    // Read MCN
                    case "CATALOG":
                        if (splitLine.Count < 2)
                            throw new FormatException($"CATALOG line malformed: {line}");

                        cueSheet.Catalog = splitLine[1].Trim('"');
                        break;

                    // Read external CD-Text file path
                    case "CDTEXTFILE":
                        if (splitLine.Count < 2)
                            throw new FormatException($"CDTEXTFILE line malformed: {line}");

                        cueSheet.CdTextFile = splitLine[1].Trim('"');
                        break;

                    // Read CD-Text enhanced performer
                    case "PERFORMER":
                        if (splitLine.Count < 2)
                            throw new FormatException($"PERFORMER line malformed: {line}");

                        cueSheet.Performer = splitLine[1].Trim('"');
                        break;

                    // Read CD-Text enhanced songwriter
                    case "SONGWRITER":
                        if (splitLine.Count < 2)
                            throw new FormatException($"SONGWRITER line malformed: {line}");

                        cueSheet.Songwriter = splitLine[1].Trim('"');
                        break;

                    // Read CD-Text enhanced title
                    case "TITLE":
                        if (splitLine.Count < 2)
                            throw new FormatException($"TITLE line malformed: {line}");

                        cueSheet.Title = splitLine[1].Trim('"');
                        break;

                    // Read file information
                    case "FILE":
                        if (splitLine.Count < 3)
                            throw new FormatException($"FILE line malformed: {line}");

                        var file = CreateCueFile(splitLine[1], splitLine[2], data, out lastLine);
                        if (file == default)
                            throw new FormatException($"FILE line malformed: {line}");

                        cueFiles.Add(file);
                        break;
                }
            }

            cueSheet.Files = [.. cueFiles];
            return cueSheet;
        }

        /// <summary>
        /// Fill a FILE from an array of lines
        /// </summary>
        /// <param name="fileName">File name to set</param>
        /// <param name="fileType">File type to set</param>
        /// <param name="data">Stream to pull from</param>
        private static CueFile? CreateCueFile(string fileName, string fileType, Stream data, out string? lastLine)
        {
            // Check the required parameters
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                throw new ArgumentNullException(nameof(data));
            if (data.Position < 0 || data.Position >= data.Length)
                throw new IndexOutOfRangeException();

            // Create the holding objects
            lastLine = null;
            var cueFile = new CueFile();
            var cueTracks = new List<CueTrack>();

            // Set the current fields
            cueFile.FileName = fileName.Trim('"');
            cueFile.FileType = GetFileType(fileType);

            while (true)
            {
                string? line = lastLine ?? ReadUntilNewline(data);
                lastLine = null;

                // If we have a null line, break from the loop
                if (line == null)
                    break;

                // If we have an empty line, we skip
                if (string.IsNullOrEmpty(line))
                    continue;

                // http://stackoverflow.com/questions/554013/regular-expression-to-split-on-spaces-unless-in-quotes
                var matchCol = Regex.Matches(line, @"[^\s""]+|""[^""]*""");
                var splitLine = new List<string>();
                foreach (Match? match in matchCol)
                {
                    if (match != null)
                        splitLine.Add(match.Groups[0].Value);
                }

                switch (splitLine[0])
                {
                    // Read comments
                    case "REM":
                        // We ignore all comments for now
                        break;

                    // Read track information
                    case "TRACK":
                        if (splitLine.Count < 3)
                            throw new FormatException($"TRACK line malformed: {line}");

                        var track = CreateCueTrack(splitLine[1], splitLine[2], data, out lastLine);
                        if (track == default)
                            throw new FormatException($"TRACK line malformed: {line}");

                        cueTracks.Add(track);
                        break;

                    // Next file found, return
                    case "FILE":
                        lastLine = line;
                        cueFile.Tracks = [.. cueTracks];
                        return cueFile;

                    // Default means return
                    default:
                        lastLine = line;
                        cueFile.Tracks = [.. cueTracks];
                        return cueFile;
                }
            }

            cueFile.Tracks = [.. cueTracks];
            return cueFile;
        }

        /// <summary>
        /// Fill a TRACK from an array of lines
        /// </summary>
        /// <param name="number">Number to set</param>
        /// <param name="dataType">Data type to set</param>
        /// <param name="data">Stream to pull from</param>
        private static CueTrack? CreateCueTrack(string number, string dataType, Stream data, out string? lastLine)
        {
            // Check the required parameters
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                throw new ArgumentNullException(nameof(data));
            if (data.Position < 0 || data.Position >= data.Length)
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

            cueTrack.Number = parsedNumber;
            cueTrack.DataType = GetDataType(dataType);

            while (true)
            {
                string? line = lastLine ?? ReadUntilNewline(data);
                lastLine = null;

                // If we have a null line, break from the loop
                if (line == null)
                    break;

                // If we have an empty line, we skip
                if (string.IsNullOrEmpty(line))
                    continue;

                // http://stackoverflow.com/questions/554013/regular-expression-to-split-on-spaces-unless-in-quotes
                var matchCol = Regex.Matches(line, @"[^\s""]+|""[^""]*""");
                var splitLine = new List<string>();
                foreach (Match? match in matchCol)
                {
                    if (match != null)
                        splitLine.Add(match.Groups[0].Value);
                }
                switch (splitLine[0])
                {
                    // Read comments
                    case "REM":
                        // We ignore all comments for now
                        break;

                    // Read flag information
                    case "FLAGS":
                        if (splitLine.Count < 2)
                            throw new FormatException($"FLAGS line malformed: {line}");

                        cueTrack.Flags = GetFlags([.. splitLine]);
                        break;

                    // Read International Standard Recording Code
                    case "ISRC":
                        if (splitLine.Count < 2)
                            throw new FormatException($"ISRC line malformed: {line}");

                        cueTrack.ISRC = splitLine[1].Trim('"');
                        break;

                    // Read CD-Text enhanced performer
                    case "PERFORMER":
                        if (splitLine.Count < 2)
                            throw new FormatException($"PERFORMER line malformed: {line}");

                        cueTrack.Performer = splitLine[1].Trim('"');
                        break;

                    // Read CD-Text enhanced songwriter
                    case "SONGWRITER":
                        if (splitLine.Count < 2)
                            throw new FormatException($"SONGWRITER line malformed: {line}");

                        cueTrack.Songwriter = splitLine[1].Trim('"');
                        break;

                    // Read CD-Text enhanced title
                    case "TITLE":
                        if (splitLine.Count < 2)
                            throw new FormatException($"TITLE line malformed: {line}");

                        cueTrack.Title = splitLine[1].Trim('"');
                        break;

                    // Read pregap information
                    case "PREGAP":
                        if (splitLine.Count < 2)
                            throw new FormatException($"PREGAP line malformed: {line}");

                        var pregap = CreatePreGap(splitLine[1]);
                        if (pregap == default)
                            throw new FormatException($"PREGAP line malformed: {line}");

                        cueTrack.PreGap = pregap;
                        break;

                    // Read index information
                    case "INDEX":
                        if (splitLine.Count < 3)
                            throw new FormatException($"INDEX line malformed: {line}");

                        var index = CreateCueIndex(splitLine[1], splitLine[2]);
                        if (index == default)
                            throw new FormatException($"INDEX line malformed: {line}");

                        cueIndices.Add(index);
                        break;

                    // Read postgap information
                    case "POSTGAP":
                        if (splitLine.Count < 2)
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
                        cueTrack.Indices = [.. cueIndices];
                        return cueTrack;

                    // Default means return
                    default:
                        lastLine = line;
                        cueTrack.Indices = [.. cueIndices];
                        return cueTrack;
                }
            }

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
        private static CueTrackFlag GetFlags(string?[]? flagStrings)
        {
            CueTrackFlag flag = 0;
            if (flagStrings == null)
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
                }
            }

            return flag;
        }

        /// <summary>
        /// Read a string until a newline value is found
        /// </summary>
        /// <param name="data">Stream to pull from</param>
        /// <returns>The next line from the data on success, null on error</returns>
        private static string? ReadUntilNewline(Stream data)
        {
            // Validate the input
            if (data.Length == 0 || data.Position < 0 || data.Position >= data.Length)
                return null;

            // Read characters either until a newline or end of file
            var lineBuilder = new StringBuilder();
            while (data.Position < data.Length)
            {
                char c = data.ReadChar();

                // Handle Windows and old Mac line endings
                if (c == '\r')
                {
                    // Handle premature end of stream
                    if (data.Position >= data.Length)
                        break;

                    // Handle Windows line endings
                    c = data.ReadChar();
                    if (c == '\n')
                        break;

                    // Seek backward if we had old Mac line endings instead
                    data.Seek(-1, SeekOrigin.Current);
                    break;
                }

                // Handle standard line endings
                if (c == '\n')
                    break;

                // Handle all other characters
                lineBuilder.Append(c);
            }

            // Return the line without trailing newline
            return lineBuilder.ToString();
        }

        #endregion
    }
}

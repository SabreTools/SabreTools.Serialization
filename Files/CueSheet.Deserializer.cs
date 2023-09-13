using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using SabreTools.Models.CueSheets;

namespace SabreTools.Serialization.Files
{
    public partial class CueSheet : IFileSerializer<Models.CueSheets.CueSheet>
    {
        /// <inheritdoc/>
#if NET48
        public Models.CueSheets.CueSheet Deserialize(string path)
#else
        public Models.CueSheets.CueSheet? Deserialize(string? path)
#endif
        {
            // Check that the file exists
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                return null;

            // Check the extension
            string ext = Path.GetExtension(path).TrimStart('.');
            if (!string.Equals(ext, "cue", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(ext, "txt", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            // Create the holding objects
            var cueSheet = new Models.CueSheets.CueSheet();
            var cueFiles = new List<CueFile>();

            // Open the file and begin reading
            string[] cueLines = File.ReadAllLines(path);
            for (int i = 0; i < cueLines.Length; i++)
            {
                string line = cueLines[i].Trim();

                // http://stackoverflow.com/questions/554013/regular-expression-to-split-on-spaces-unless-in-quotes
                string[] splitLine = Regex
                    .Matches(line, @"[^\s""]+|""[^""]*""")
                    .Cast<Match>()
                    .Select(m => m.Groups[0].Value)
                    .ToArray();

                // If we have an empty line, we skip
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                switch (splitLine[0])
                {
                    // Read comments
                    case "REM":
                        // We ignore all comments for now
                        break;

                    // Read MCN
                    case "CATALOG":
                        if (splitLine.Length < 2)
                            throw new FormatException($"CATALOG line malformed: {line}");

                        cueSheet.Catalog = splitLine[1];
                        break;

                    // Read external CD-Text file path
                    case "CDTEXTFILE":
                        if (splitLine.Length < 2)
                            throw new FormatException($"CDTEXTFILE line malformed: {line}");

                        cueSheet.CdTextFile = splitLine[1];
                        break;

                    // Read CD-Text enhanced performer
                    case "PERFORMER":
                        if (splitLine.Length < 2)
                            throw new FormatException($"PERFORMER line malformed: {line}");

                        cueSheet.Performer = splitLine[1];
                        break;

                    // Read CD-Text enhanced songwriter
                    case "SONGWRITER":
                        if (splitLine.Length < 2)
                            throw new FormatException($"SONGWRITER line malformed: {line}");

                        cueSheet.Songwriter = splitLine[1];
                        break;

                    // Read CD-Text enhanced title
                    case "TITLE":
                        if (splitLine.Length < 2)
                            throw new FormatException($"TITLE line malformed: {line}");

                        cueSheet.Title = splitLine[1];
                        break;

                    // Read file information
                    case "FILE":
                        if (splitLine.Length < 3)
                            throw new FormatException($"FILE line malformed: {line}");

                        var file = CreateCueFile(splitLine[1], splitLine[2], cueLines, ref i);
                        if (file == default)
                            throw new FormatException($"FILE line malformed: {line}");

                        cueFiles.Add(file);
                        break;
                }
            }

            cueSheet.Files = cueFiles.ToArray();
            return cueSheet;
        }

        /// <summary>
        /// Fill a FILE from an array of lines
        /// </summary>
        /// <param name="fileName">File name to set</param>
        /// <param name="fileType">File type to set</param>
        /// <param name="cueLines">Lines array to pull from</param>
        /// <param name="i">Reference to index in array</param>
#if NET48
        private static CueFile CreateCueFile(string fileName, string fileType, string[] cueLines, ref int i)
#else
        private static CueFile? CreateCueFile(string fileName, string fileType, string[]? cueLines, ref int i)
#endif
        {
            // Check the required parameters
            if (cueLines == null)
                throw new ArgumentNullException(nameof(cueLines));
            else if (i < 0 || i > cueLines.Length)
                throw new IndexOutOfRangeException();

            // Create the holding objects
            var cueFile = new CueFile();
            var cueTracks = new List<CueTrack>();

            // Set the current fields
            cueFile.FileName = fileName.Trim('"');
            cueFile.FileType = GetFileType(fileType);

            // Increment to start
            i++;

            for (; i < cueLines.Length; i++)
            {
                string line = cueLines[i].Trim();
                string[] splitLine = line.Split(' ');

                // If we have an empty line, we skip
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                switch (splitLine[0])
                {
                    // Read comments
                    case "REM":
                        // We ignore all comments for now
                        break;

                    // Read track information
                    case "TRACK":
                        if (splitLine.Length < 3)
                            throw new FormatException($"TRACK line malformed: {line}");

                        var track = CreateCueTrack(splitLine[1], splitLine[2], cueLines, ref i);
                        if (track == default)
                            throw new FormatException($"TRACK line malformed: {line}");

                        cueTracks.Add(track);
                        break;

                    // Default means return
                    default:
                        i--;
                        return null;
                }
            }

            cueFile.Tracks = cueTracks.ToArray();
            return cueFile;
        }

        /// <summary>
        /// Fill a TRACK from an array of lines
        /// </summary>
        /// <param name="number">Number to set</param>
        /// <param name="dataType">Data type to set</param>
        /// <param name="cueLines">Lines array to pull from</param>
        /// <param name="i">Reference to index in array</param>
#if NET48
        private static CueTrack CreateCueTrack(string number, string dataType, string[] cueLines, ref int i)
#else
        private static CueTrack? CreateCueTrack(string number, string dataType, string[]? cueLines, ref int i)
#endif
        {
            // Check the required parameters
            if (cueLines == null)
                throw new ArgumentNullException(nameof(cueLines));
            else if (i < 0 || i > cueLines.Length)
                throw new IndexOutOfRangeException();

            // Set the current fields
            if (!int.TryParse(number, out int parsedNumber))
                throw new ArgumentException($"Number was not a number: {number}");
            else if (parsedNumber < 1 || parsedNumber > 99)
                throw new IndexOutOfRangeException($"Index must be between 1 and 99: {parsedNumber}");

            // Create the holding objects
            var cueTrack = new CueTrack();
            var cueIndices = new List<CueIndex>();

            cueTrack.Number = parsedNumber;
            cueTrack.DataType = GetDataType(dataType);

            // Increment to start
            i++;

            for (; i < cueLines.Length; i++)
            {
                string line = cueLines[i].Trim();
                string[] splitLine = line.Split(' ');

                // If we have an empty line, we skip
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                switch (splitLine[0])
                {
                    // Read comments
                    case "REM":
                        // We ignore all comments for now
                        break;

                    // Read flag information
                    case "FLAGS":
                        if (splitLine.Length < 2)
                            throw new FormatException($"FLAGS line malformed: {line}");

                        cueTrack.Flags = GetFlags(splitLine);
                        break;

                    // Read International Standard Recording Code
                    case "ISRC":
                        if (splitLine.Length < 2)
                            throw new FormatException($"ISRC line malformed: {line}");

                        cueTrack.ISRC = splitLine[1];
                        break;

                    // Read CD-Text enhanced performer
                    case "PERFORMER":
                        if (splitLine.Length < 2)
                            throw new FormatException($"PERFORMER line malformed: {line}");

                        cueTrack.Performer = splitLine[1];
                        break;

                    // Read CD-Text enhanced songwriter
                    case "SONGWRITER":
                        if (splitLine.Length < 2)
                            throw new FormatException($"SONGWRITER line malformed: {line}");

                        cueTrack.Songwriter = splitLine[1];
                        break;

                    // Read CD-Text enhanced title
                    case "TITLE":
                        if (splitLine.Length < 2)
                            throw new FormatException($"TITLE line malformed: {line}");

                        cueTrack.Title = splitLine[1];
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

                    // Default means return
                    default:
                        i--;
                        return null;
                }
            }

            cueTrack.Indices = cueIndices.ToArray();
            return cueTrack;
        }

        /// <summary>
        /// Create a PREGAP from a mm:ss:ff length
        /// </summary>
        /// <param name="length">String to get length information from</param>
#if NET48
        private static PreGap CreatePreGap(string length)
#else
        private static PreGap CreatePreGap(string? length)
#endif
        {
            // Ignore empty lines
            if (string.IsNullOrWhiteSpace(length))
                throw new ArgumentException("Length was null or whitespace");

            // Ignore lines that don't contain the correct information
            if (length.Length != 8 || length.Count(c => c == ':') != 2)
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
#if NET48
        private static CueIndex CreateCueIndex(string index, string startTime)
#else
        private static CueIndex CreateCueIndex(string? index, string? startTime)
#endif
        {
            // Set the current fields
            if (!int.TryParse(index, out int parsedIndex))
                throw new ArgumentException($"Index was not a number: {index}");
            else if (parsedIndex < 0 || parsedIndex > 99)
                throw new IndexOutOfRangeException($"Index must be between 0 and 99: {parsedIndex}");

            // Ignore empty lines
            if (string.IsNullOrWhiteSpace(startTime))
                throw new ArgumentException("Start time was null or whitespace");

            // Ignore lines that don't contain the correct information
            if (startTime.Length != 8 || startTime.Count(c => c == ':') != 2)
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
#if NET48
        private static PostGap CreatePostGap(string length)
#else
        private static PostGap CreatePostGap(string? length)
#endif
        {
            // Ignore empty lines
            if (string.IsNullOrWhiteSpace(length))
                throw new ArgumentException("Length was null or whitespace");

            // Ignore lines that don't contain the correct information
            if (length.Length != 8 || length.Count(c => c == ':') != 2)
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
#if NET48
        private static CueFileType GetFileType(string fileType)
#else
        private static CueFileType GetFileType(string? fileType)
#endif
        {
            switch (fileType?.ToLowerInvariant())
            {
                case "binary":
                    return CueFileType.BINARY;

                case "motorola":
                    return CueFileType.MOTOROLA;

                case "aiff":
                    return CueFileType.AIFF;

                case "wave":
                    return CueFileType.WAVE;

                case "mp3":
                    return CueFileType.MP3;

                default:
                    return CueFileType.BINARY;
            }
        }

        /// <summary>
        /// Get the data type from a given string
        /// </summary>
        /// <param name="dataType">String to get value from</param>
        /// <returns>CueTrackDataType, if possible (default AUDIO)</returns>
#if NET48
        private static CueTrackDataType GetDataType(string dataType)
#else
        private static CueTrackDataType GetDataType(string? dataType)
#endif
        {
            switch (dataType?.ToLowerInvariant())
            {
                case "audio":
                    return CueTrackDataType.AUDIO;

                case "cdg":
                    return CueTrackDataType.CDG;

                case "mode1/2048":
                    return CueTrackDataType.MODE1_2048;

                case "mode1/2352":
                    return CueTrackDataType.MODE1_2352;

                case "mode2/2336":
                    return CueTrackDataType.MODE2_2336;

                case "mode2/2352":
                    return CueTrackDataType.MODE2_2352;

                case "cdi/2336":
                    return CueTrackDataType.CDI_2336;

                case "cdi/2352":
                    return CueTrackDataType.CDI_2352;

                default:
                    return CueTrackDataType.AUDIO;
            }
        }

        /// <summary>
        /// Get the flag value for an array of strings
        /// </summary>
        /// <param name="flagStrings">Possible flags as strings</param>
        /// <returns>CueTrackFlag value representing the strings, if possible</returns>
#if NET48
        private static CueTrackFlag GetFlags(string[] flagStrings)
#else
        private static CueTrackFlag GetFlags(string?[]? flagStrings)
#endif
        {
            CueTrackFlag flag = 0;
            if (flagStrings == null)
                return flag;

#if NET48
            foreach (string flagString in flagStrings)
#else
            foreach (string? flagString in flagStrings)
#endif
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

        #endregion
    }
}
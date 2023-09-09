using System.IO;
using System.Linq;
using System.Text;
using SabreTools.IO.Writers;
using SabreTools.Models.RomCenter;

namespace SabreTools.Serialization.Streams
{
    public partial class RomCenter : IStreamSerializer<MetadataFile>
    {
        /// <inheritdoc/>
#if NET48
        public Stream Serialize(MetadataFile obj)
#else
        public Stream? Serialize(MetadataFile? obj)
#endif
        {
            // If the metadata file is null
            if (obj == null)
                return null;

            // Setup the writer and output
            var stream = new MemoryStream();
            var writer = new IniWriter(stream, Encoding.UTF8);

            // Write out the credits section
            WriteCredits(obj.Credits, writer);

            // Write out the dat section
            WriteDat(obj.Dat, writer);

            // Write out the emulator section
            WriteEmulator(obj.Emulator, writer);

            // Write out the games
            WriteGames(obj.Games, writer);

            // Return the stream
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        /// <summary>
        /// Write credits information to the current writer
        /// </summary>
        /// <param name="credits">Credits object representing the credits information</param>
        /// <param name="writer">IniWriter representing the output</param>
#if NET48
        private static void WriteCredits(Credits credits, IniWriter writer)
#else
        private static void WriteCredits(Credits? credits, IniWriter writer)
#endif
        {
            // If the credits information is missing, we can't do anything
            if (credits == null)
                return;

            writer.WriteSection("credits");

            writer.WriteKeyValuePair("author", credits.Author);
            writer.WriteKeyValuePair("version", credits.Version);
            writer.WriteKeyValuePair("email", credits.Email);
            writer.WriteKeyValuePair("homepage", credits.Homepage);
            writer.WriteKeyValuePair("url", credits.Url);
            writer.WriteKeyValuePair("date", credits.Date);
            writer.WriteKeyValuePair("comment", credits.Comment);
            writer.WriteLine();

            writer.Flush();
        }

        /// <summary>
        /// Write dat information to the current writer
        /// </summary>
        /// <param name="dat">Dat object representing the dat information</param>
        /// <param name="writer">IniWriter representing the output</param>
#if NET48
        private static void WriteDat(Dat dat, IniWriter writer)
#else
        private static void WriteDat(Dat? dat, IniWriter writer)
#endif
        {
            // If the dat information is missing, we can't do anything
            if (dat == null)
                return;

            writer.WriteSection("dat");

            writer.WriteKeyValuePair("version", dat.Version);
            writer.WriteKeyValuePair("plugin", dat.Plugin);
            writer.WriteKeyValuePair("split", dat.Split);
            writer.WriteKeyValuePair("merge", dat.Merge);
            writer.WriteLine();

            writer.Flush();
        }

        /// <summary>
        /// Write emulator information to the current writer
        /// </summary>
        /// <param name="emulator">Emulator object representing the emulator information</param>
        /// <param name="writer">IniWriter representing the output</param>
#if NET48
        private static void WriteEmulator(Emulator emulator, IniWriter writer)
#else
        private static void WriteEmulator(Emulator? emulator, IniWriter writer)
#endif
        {
            // If the emulator information is missing, we can't do anything
            if (emulator == null)
                return;

            writer.WriteSection("emulator");

            writer.WriteKeyValuePair("refname", emulator.RefName);
            writer.WriteKeyValuePair("version", emulator.Version);
            writer.WriteLine();

            writer.Flush();
        }

        /// <summary>
        /// Write games information to the current writer
        /// </summary>
        /// <param name="games">Games object representing the games information</param>
        /// <param name="writer">IniWriter representing the output</param>
#if NET48
        private static void WriteGames(Games games, IniWriter writer)
#else
        private static void WriteGames(Games? games, IniWriter writer)
#endif
        {
            // If the games information is missing, we can't do anything
            if (games?.Rom == null || !games.Rom.Any())
                return;

            writer.WriteSection("games");

            foreach (var rom in games.Rom)
            {
                var romBuilder = new StringBuilder();

                romBuilder.Append('¬');
                romBuilder.Append(rom.ParentName);
                romBuilder.Append('¬');
                romBuilder.Append(rom.ParentDescription);
                romBuilder.Append('¬');
                romBuilder.Append(rom.GameName);
                romBuilder.Append('¬');
                romBuilder.Append(rom.GameDescription);
                romBuilder.Append('¬');
                romBuilder.Append(rom.RomName);
                romBuilder.Append('¬');
                romBuilder.Append(rom.RomCRC);
                romBuilder.Append('¬');
                romBuilder.Append(rom.RomSize);
                romBuilder.Append('¬');
                romBuilder.Append(rom.RomOf);
                romBuilder.Append('¬');
                romBuilder.Append(rom.MergeName);
                romBuilder.Append('¬');
                romBuilder.Append('\n');

                writer.WriteString(romBuilder.ToString());
                writer.Flush();
            }

            writer.WriteLine();
            writer.Flush();
        }
    }
}
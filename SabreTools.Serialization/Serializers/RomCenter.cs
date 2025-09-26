using System.IO;
using System.Text;
using SabreTools.Data.Models.RomCenter;
using SabreTools.IO.Writers;

namespace SabreTools.Serialization.Writers
{
    public class RomCenter : BaseBinaryWriter<MetadataFile>
    {
        /// <inheritdoc/>
        public override Stream? SerializeStream(MetadataFile? obj)
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
        private static void WriteCredits(Credits? credits, IniWriter writer)
        {
            // If the credits information is missing, we can't do anything
            if (credits == null)
                return;

            writer.WriteSection("CREDITS");

            if (credits.Author != null)
                writer.WriteKeyValuePair("Author", credits.Author);
            if (credits.Version != null)
                writer.WriteKeyValuePair("Version", credits.Version);
            if (credits.Email != null)
                writer.WriteKeyValuePair("Email", credits.Email);
            if (credits.Homepage != null)
                writer.WriteKeyValuePair("Homepage", credits.Homepage);
            if (credits.Url != null)
                writer.WriteKeyValuePair("Url", credits.Url);
            if (credits.Date != null)
                writer.WriteKeyValuePair("Date", credits.Date);
            if (credits.Comment != null)
                writer.WriteKeyValuePair("Comment", credits.Comment);
            writer.WriteLine();

            writer.Flush();
        }

        /// <summary>
        /// Write dat information to the current writer
        /// </summary>
        /// <param name="dat">Dat object representing the dat information</param>
        /// <param name="writer">IniWriter representing the output</param>
        private static void WriteDat(Dat? dat, IniWriter writer)
        {
            // If the dat information is missing, we can't do anything
            if (dat == null)
                return;

            writer.WriteSection("DAT");

            if (dat.Version != null)
                writer.WriteKeyValuePair("Version", dat.Version);
            if (dat.Plugin != null)
                writer.WriteKeyValuePair("Plugin", dat.Plugin);
            if (dat.Split != null)
                writer.WriteKeyValuePair("Split", dat.Split);
            if (dat.Merge != null)
                writer.WriteKeyValuePair("Merge", dat.Merge);
            writer.WriteLine();

            writer.Flush();
        }

        /// <summary>
        /// Write emulator information to the current writer
        /// </summary>
        /// <param name="emulator">Emulator object representing the emulator information</param>
        /// <param name="writer">IniWriter representing the output</param>
        private static void WriteEmulator(Emulator? emulator, IniWriter writer)
        {
            // If the emulator information is missing, we can't do anything
            if (emulator == null)
                return;

            writer.WriteSection("EMULATOR");

            if (emulator.RefName != null)
                writer.WriteKeyValuePair("refname", emulator.RefName);
            if (emulator.Version != null)
                writer.WriteKeyValuePair("version", emulator.Version);
            writer.WriteLine();

            writer.Flush();
        }

        /// <summary>
        /// Write games information to the current writer
        /// </summary>
        /// <param name="games">Games object representing the games information</param>
        /// <param name="writer">IniWriter representing the output</param>
        private static void WriteGames(Games? games, IniWriter writer)
        {
            // If the games information is missing, we can't do anything
            if (games?.Rom == null || games.Rom.Length == 0)
                return;

            writer.WriteSection("GAMES");

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

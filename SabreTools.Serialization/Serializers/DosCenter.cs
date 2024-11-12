using System.IO;
using System.Linq;
using System.Text;
using SabreTools.IO.Writers;
using SabreTools.Models.DosCenter;

namespace SabreTools.Serialization.Serializers
{
    public class DosCenter : BaseBinarySerializer<MetadataFile>
    {
        /// <inheritdoc/>
        public override Stream? Serialize(MetadataFile? obj)
        {
            // If the metadata file is null
            if (obj == null)
                return null;

            // Setup the writer and output
            var stream = new MemoryStream();
            var writer = new ClrMameProWriter(stream, Encoding.UTF8)
            {
                Quotes = false,
            };

            // Write the header, if it exists
            WriteHeader(obj.DosCenter, writer);

            // Write out the games, if they exist
            WriteGames(obj.Game, writer);

            // Return the stream
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        /// <summary>
        /// Write header information to the current writer
        /// </summary>
        /// <param name="header">DosCenter representing the header information</param>
        /// <param name="writer">ClrMameProWriter representing the output</param>
        private static void WriteHeader(Models.DosCenter.DosCenter? header, ClrMameProWriter writer)
        {
            // If the header information is missing, we can't do anything
            if (header == null)
                return;

            writer.WriteStartElement("DOSCenter");

            writer.WriteOptionalStandalone("Name:", header.Name);
            writer.WriteOptionalStandalone("Description:", header.Description);
            writer.WriteOptionalStandalone("Version:", header.Version);
            writer.WriteOptionalStandalone("Date:", header.Date);
            writer.WriteOptionalStandalone("Author:", header.Author);
            writer.WriteOptionalStandalone("Homepage:", header.Homepage);
            writer.WriteOptionalStandalone("Comment:", header.Comment);

            writer.WriteEndElement(); // doscenter
            writer.Flush();
        }

        /// <summary>
        /// Write games information to the current writer
        /// </summary>
        /// <param name="games">Array of Game objects representing the games information</param>
        /// <param name="writer">ClrMameProWriter representing the output</param>
        private static void WriteGames(Game[]? games, ClrMameProWriter writer)
        {
            // If the games information is missing, we can't do anything
            if (games == null || games.Length == 0)
                return;

            // Loop through and write out the games
            foreach (var game in games)
            {
                WriteGame(game, writer);
                writer.Flush();
            }
        }

        /// <summary>
        /// Write game information to the current writer
        /// </summary>
        /// <param name="game">Game object representing the game information</param>
        /// <param name="writer">ClrMameProWriter representing the output</param>
        private static void WriteGame(Game game, ClrMameProWriter writer)
        {
            // If the game information is missing, we can't do anything
            if (game == null)
                return;

            writer.WriteStartElement("game");

            // Write the standalone values
            writer.WriteRequiredStandalone("name", game.Name, throwOnError: true);

            // Write the item values
            WriteFiles(game.File, writer);

            writer.WriteEndElement(); // game
        }

        /// <summary>
        /// Write files information to the current writer
        /// </summary>
        /// <param name="files">Array of File objects to write</param>
        /// <param name="writer">ClrMameProWriter representing the output</param>
        private static void WriteFiles(Models.DosCenter.File[]? files, ClrMameProWriter writer)
        {
            // If the array is missing, we can't do anything
            if (files == null)
                return;

            foreach (var file in files)
            {
                writer.WriteStartElement("file");

                writer.WriteRequiredAttributeString("name", file.Name, throwOnError: true);
                writer.WriteRequiredAttributeString("size", file.Size, throwOnError: true);
                writer.WriteOptionalAttributeString("date", file.Date);
                writer.WriteRequiredAttributeString("crc", file.CRC?.ToUpperInvariant(), throwOnError: true);

                writer.WriteEndElement(); // file
            }
        }
    }
}
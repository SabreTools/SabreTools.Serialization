using System.IO;
using System.Text;
using SabreTools.IO.Writers;
using SabreTools.Models.ClrMamePro;

namespace SabreTools.Serialization.Serializers
{
    public class ClrMamePro : BaseBinarySerializer<MetadataFile>
    {
        #region IByteSerializer

        /// <inheritdoc cref="Interfaces.IByteSerializer.SerializeArray(T?)"/>
        public static byte[]? SerializeBytes(MetadataFile? obj, bool quotes = false)
        {
            var serializer = new ClrMamePro();
            return serializer.SerializeArray(obj, quotes);
        }

        /// <inheritdoc/>
        public override byte[]? SerializeArray(MetadataFile? obj)
            => SerializeArray(obj, false);

        /// <inheritdoc/>
        public byte[]? SerializeArray(MetadataFile? obj, bool quotes)
        {
            using var stream = SerializeStream(obj, quotes);
            if (stream == null)
                return null;

            byte[] bytes = new byte[stream.Length];
            int read = stream.Read(bytes, 0, bytes.Length);
            return bytes;
        }

        #endregion

        #region IFileSerializer

        /// <inheritdoc cref="IFileSerializer.Serialize(T?, string?)"/>
        public static bool SerializeFile(MetadataFile? obj, string? path, bool quotes = true)
        {
            var serializer = new ClrMamePro();
            return serializer.Serialize(obj, path, quotes);
        }

        /// <inheritdoc/>
        public override bool Serialize(MetadataFile? obj, string? path)
            => Serialize(obj, path, true);

        /// <inheritdoc cref="Serialize(MetadataFile, string)"/>
        public bool Serialize(MetadataFile? obj, string? path, bool quotes)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            using var stream = SerializeStream(obj, quotes);
            if (stream == null)
                return false;

            using var fs = File.OpenWrite(path);
            stream.CopyTo(fs);
            return true;
        }

        #endregion

        #region IStreamSerializer

        /// <inheritdoc cref="IStreamSerializer.Serialize(T?)"/>
        public static Stream? SerializeStream(MetadataFile? obj, bool quotes = true)
        {
            var serializer = new ClrMamePro();
            return serializer.Serialize(obj, quotes);
        }

        /// <inheritdoc/>
        public override Stream? Serialize(MetadataFile? obj)
            => Serialize(obj, true);

        /// <inheritdoc cref="Serialize(MetadataFile)"/>
        public Stream? Serialize(MetadataFile? obj, bool quotes)
        {
            // If the metadata file is null
            if (obj == null)
                return null;

            // Setup the writer and output
            var stream = new MemoryStream();
            var writer = new ClrMameProWriter(stream, Encoding.UTF8) { Quotes = quotes };

            // Write the header, if it exists
            WriteHeader(obj.ClrMamePro, writer);

            // Write out the games, if they exist
            WriteGames(obj.Game, writer);

            // Return the stream
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        /// <summary>
        /// Write header information to the current writer
        /// </summary>
        /// <param name="header">ClrMamePro representing the header information</param>
        /// <param name="writer">ClrMameProWriter representing the output</param>
        private static void WriteHeader(Models.ClrMamePro.ClrMamePro? header, ClrMameProWriter writer)
        {
            // If the header information is missing, we can't do anything
            if (header == null)
                return;

            writer.WriteStartElement("clrmamepro");

            writer.WriteOptionalStandalone("name", header.Name);
            writer.WriteOptionalStandalone("description", header.Description);
            writer.WriteOptionalStandalone("rootdir", header.RootDir);
            writer.WriteOptionalStandalone("category", header.Category);
            writer.WriteOptionalStandalone("version", header.Version);
            writer.WriteOptionalStandalone("date", header.Date);
            writer.WriteOptionalStandalone("author", header.Author);
            writer.WriteOptionalStandalone("homepage", header.Homepage);
            writer.WriteOptionalStandalone("url", header.Url);
            writer.WriteOptionalStandalone("comment", header.Comment);
            writer.WriteOptionalStandalone("header", header.Header);
            writer.WriteOptionalStandalone("type", header.Type);
            writer.WriteOptionalStandalone("forcemerging", header.ForceMerging);
            writer.WriteOptionalStandalone("forcezipping", header.ForceZipping);
            writer.WriteOptionalStandalone("forcepacking", header.ForcePacking);

            writer.WriteEndElement(); // clrmamepro
            writer.Flush();
        }

        /// <summary>
        /// Write games information to the current writer
        /// </summary>
        /// <param name="games">Array of GameBase objects representing the games information</param>
        /// <param name="writer">ClrMameProWriter representing the output</param>
        private static void WriteGames(GameBase[]? games, ClrMameProWriter writer)
        {
            // If the games information is missing, we can't do anything
            if (games == null || games.Length == 0)
                return;

            // Loop through and write out the games
            foreach (var game in games)
            {
                if (game == null)
                    continue;

                WriteGame(game, writer);
                writer.Flush();
            }
        }

        /// <summary>
        /// Write game information to the current writer
        /// </summary>
        /// <param name="game">GameBase object representing the game information</param>
        /// <param name="writer">ClrMameProWriter representing the output</param>
        private static void WriteGame(GameBase game, ClrMameProWriter writer)
        {
            // If the game information is missing, we can't do anything
            if (game == null)
                return;

            switch (game)
            {
                case Game:
                    writer.WriteStartElement("game");
                    break;
                case Machine:
                    writer.WriteStartElement("machine");
                    break;
                case Resource:
                    writer.WriteStartElement("resource");
                    break;
                case Set:
                    writer.WriteStartElement(name: "set");
                    break;
            }

            // Write the standalone values
            writer.WriteRequiredStandalone("name", game.Name, throwOnError: true);
            writer.WriteOptionalStandalone("description", game.Description);
            writer.WriteOptionalStandalone("year", game.Year);
            writer.WriteOptionalStandalone("manufacturer", game.Manufacturer);
            writer.WriteOptionalStandalone("category", game.Category);
            writer.WriteOptionalStandalone("cloneof", game.CloneOf);
            writer.WriteOptionalStandalone("romof", game.RomOf);
            writer.WriteOptionalStandalone("sampleof", game.SampleOf);

            // Write the item values
            WriteReleases(game.Release, writer);
            WriteBiosSets(game.BiosSet, writer);
            WriteRoms(game.Rom, writer);
            WriteDisks(game.Disk, writer);
            WriteMedia(game.Media, writer);
            WriteSamples(game.Sample, writer);
            WriteArchives(game.Archive, writer);
            WriteChips(game.Chip, writer);
            WriteVideos(game.Video, writer);
            WriteSound(game.Sound, writer);
            WriteInput(game.Input, writer);
            WriteDipSwitches(game.DipSwitch, writer);
            WriteDriver(game.Driver, writer);

            writer.WriteEndElement(); // game, machine, resource, set
        }

        /// <summary>
        /// Write releases information to the current writer
        /// </summary>
        /// <param name="releases">Array of Release objects to write</param>
        /// <param name="writer">ClrMameProWriter representing the output</param>
        private static void WriteReleases(Release[]? releases, ClrMameProWriter writer)
        {
            // If the array is missing, we can't do anything
            if (releases == null)
                return;

            foreach (var release in releases)
            {
                writer.WriteStartElement("release");
                writer.WriteRequiredAttributeString("name", release.Name, throwOnError: true);
                writer.WriteRequiredAttributeString("region", release.Region, throwOnError: true);
                writer.WriteOptionalAttributeString("language", release.Language);
                writer.WriteOptionalAttributeString("date", release.Date);
                writer.WriteOptionalAttributeString("default", release.Default);
                writer.WriteEndElement(); // release
            }
        }

        /// <summary>
        /// Write biossets information to the current writer
        /// </summary>
        /// <param name="biossets">Array of BiosSet objects to write</param>
        /// <param name="writer">ClrMameProWriter representing the output</param>
        private static void WriteBiosSets(BiosSet[]? biossets, ClrMameProWriter writer)
        {
            // If the array is missing, we can't do anything
            if (biossets == null)
                return;

            foreach (var biosset in biossets)
            {
                writer.WriteStartElement("biosset");
                writer.WriteRequiredAttributeString("name", biosset.Name, throwOnError: true);
                writer.WriteRequiredAttributeString("description", biosset.Description, throwOnError: true);
                writer.WriteOptionalAttributeString("default", biosset.Default);
                writer.WriteEndElement(); // release
            }
        }

        /// <summary>
        /// Write roms information to the current writer
        /// </summary>
        /// <param name="roms">Array of Rom objects to write</param>
        /// <param name="writer">ClrMameProWriter representing the output</param>
        private static void WriteRoms(Rom[]? roms, ClrMameProWriter writer)
        {
            // If the array is missing, we can't do anything
            if (roms == null)
                return;

            foreach (var rom in roms)
            {
                writer.WriteStartElement("rom");
                writer.WriteRequiredAttributeString("name", rom.Name, throwOnError: true);
                writer.WriteRequiredAttributeString("size", rom.Size, throwOnError: true);
                writer.WriteOptionalAttributeString("crc", rom.CRC);
                writer.WriteOptionalAttributeString("md5", rom.MD5);
                writer.WriteOptionalAttributeString("sha1", rom.SHA1);
                writer.WriteOptionalAttributeString("sha256", rom.SHA256);
                writer.WriteOptionalAttributeString("sha384", rom.SHA384);
                writer.WriteOptionalAttributeString("sha512", rom.SHA512);
                writer.WriteOptionalAttributeString("spamsum", rom.SpamSum);
                writer.WriteOptionalAttributeString("xxh3_64", rom.xxHash364);
                writer.WriteOptionalAttributeString("xxh3_128", rom.xxHash3128);
                writer.WriteOptionalAttributeString("merge", rom.Merge);
                writer.WriteOptionalAttributeString("status", rom.Status);
                writer.WriteOptionalAttributeString("region", rom.Region);
                writer.WriteOptionalAttributeString("flags", rom.Flags);
                writer.WriteOptionalAttributeString("offs", rom.Offs);
                writer.WriteOptionalAttributeString("serial", rom.Serial);
                writer.WriteOptionalAttributeString("header", rom.Header);
                writer.WriteOptionalAttributeString("date", rom.Date);
                writer.WriteOptionalAttributeString("inverted", rom.Inverted);
                writer.WriteOptionalAttributeString("mia", rom.MIA);
                writer.WriteEndElement(); // rom
            }
        }

        /// <summary>
        /// Write disks information to the current writer
        /// </summary>
        /// <param name="disks">Array of Disk objects to write</param>
        /// <param name="writer">ClrMameProWriter representing the output</param>
        private static void WriteDisks(Disk[]? disks, ClrMameProWriter writer)
        {
            // If the array is missing, we can't do anything
            if (disks == null)
                return;

            foreach (var disk in disks)
            {
                writer.WriteStartElement("disk");
                writer.WriteRequiredAttributeString("name", disk.Name, throwOnError: true);
                writer.WriteOptionalAttributeString("md5", disk.MD5);
                writer.WriteOptionalAttributeString("sha1", disk.SHA1);
                writer.WriteOptionalAttributeString("merge", disk.Merge);
                writer.WriteOptionalAttributeString("status", disk.Status);
                writer.WriteOptionalAttributeString("flags", disk.Flags);
                writer.WriteEndElement(); // disk
            }
        }

        /// <summary>
        /// Write medias information to the current writer
        /// </summary>
        /// <param name="medias">Array of Media objects to write</param>
        /// <param name="writer">ClrMameProWriter representing the output</param>
        private static void WriteMedia(Media[]? medias, ClrMameProWriter writer)
        {
            // If the array is missing, we can't do anything
            if (medias == null)
                return;

            foreach (var media in medias)
            {
                writer.WriteStartElement("media");
                writer.WriteRequiredAttributeString("name", media.Name, throwOnError: true);
                writer.WriteOptionalAttributeString("md5", media.MD5);
                writer.WriteOptionalAttributeString("sha1", media.SHA1);
                writer.WriteOptionalAttributeString("sha256", media.SHA256);
                writer.WriteOptionalAttributeString("spamsum", media.SpamSum);
                writer.WriteEndElement(); // media
            }
        }

        /// <summary>
        /// Write samples information to the current writer
        /// </summary>
        /// <param name="samples">Array of Sample objects to write</param>
        /// <param name="writer">ClrMameProWriter representing the output</param>
        private static void WriteSamples(Sample[]? samples, ClrMameProWriter writer)
        {
            // If the array is missing, we can't do anything
            if (samples == null)
                return;

            foreach (var sample in samples)
            {
                writer.WriteStartElement("sample");
                writer.WriteRequiredAttributeString("name", sample.Name, throwOnError: true);
                writer.WriteEndElement(); // sample
            }
        }

        /// <summary>
        /// Write archives information to the current writer
        /// </summary>
        /// <param name="archives">Array of Archive objects to write</param>
        /// <param name="writer">ClrMameProWriter representing the output</param>
        private static void WriteArchives(Archive[]? archives, ClrMameProWriter writer)
        {
            // If the array is missing, we can't do anything
            if (archives == null)
                return;

            foreach (var archive in archives)
            {
                writer.WriteStartElement("archive");
                writer.WriteRequiredAttributeString("name", archive.Name, throwOnError: true);
                writer.WriteEndElement(); // archive
            }
        }

        /// <summary>
        /// Write chips information to the current writer
        /// </summary>
        /// <param name="chips">Array of Chip objects to write</param>
        /// <param name="writer">ClrMameProWriter representing the output</param>
        private static void WriteChips(Chip[]? chips, ClrMameProWriter writer)
        {
            // If the array is missing, we can't do anything
            if (chips == null)
                return;

            foreach (var chip in chips)
            {
                writer.WriteStartElement("chip");
                writer.WriteRequiredAttributeString("type", chip.Type, throwOnError: true);
                writer.WriteRequiredAttributeString("name", chip.Name, throwOnError: true);
                writer.WriteOptionalAttributeString("flags", chip.Flags);
                writer.WriteOptionalAttributeString("clock", chip.Clock);
                writer.WriteEndElement(); // chip
            }
        }

        /// <summary>
        /// Write video information to the current writer
        /// </summary>
        /// <param name="videos">Array of Video objects to write</param>
        /// <param name="writer">ClrMameProWriter representing the output</param>
        private static void WriteVideos(Video[]? videos, ClrMameProWriter writer)
        {
            // If the item is missing, we can't do anything
            if (videos == null)
                return;

            foreach (var video in videos)
            {
                writer.WriteStartElement("video");
                writer.WriteRequiredAttributeString("screen", video.Screen, throwOnError: true);
                writer.WriteRequiredAttributeString("orientation", video.Orientation, throwOnError: true);
                writer.WriteOptionalAttributeString("x", video.X);
                writer.WriteOptionalAttributeString("y", video.Y);
                writer.WriteOptionalAttributeString("aspectx", video.AspectX);
                writer.WriteOptionalAttributeString("aspecty", video.AspectY);
                writer.WriteOptionalAttributeString("freq", video.Freq);
                writer.WriteEndElement(); // video
            }
        }

        /// <summary>
        /// Write sound information to the current writer
        /// </summary>
        /// <param name="sound">Sound object to write</param>
        /// <param name="writer">ClrMameProWriter representing the output</param>
        private static void WriteSound(Sound? sound, ClrMameProWriter writer)
        {
            // If the item is missing, we can't do anything
            if (sound == null)
                return;

            writer.WriteStartElement("sound");
            writer.WriteRequiredAttributeString("channels", sound.Channels, throwOnError: true);
            writer.WriteEndElement(); // sound
        }

        /// <summary>
        /// Write input information to the current writer
        /// </summary>
        /// <param name="input">Input object to write</param>
        /// <param name="writer">ClrMameProWriter representing the output</param>
        private static void WriteInput(Input? input, ClrMameProWriter writer)
        {
            // If the item is missing, we can't do anything
            if (input == null)
                return;

            writer.WriteStartElement("input");
            writer.WriteRequiredAttributeString("players", input.Players, throwOnError: true);
            writer.WriteOptionalAttributeString("control", input.Control);
            writer.WriteRequiredAttributeString("buttons", input.Buttons, throwOnError: true);
            writer.WriteOptionalAttributeString("coins", input.Coins);
            writer.WriteOptionalAttributeString("tilt", input.Tilt);
            writer.WriteOptionalAttributeString("service", input.Service);
            writer.WriteEndElement(); // input
        }

        /// <summary>
        /// Write dipswitches information to the current writer
        /// </summary>
        /// <param name="dipswitches">Array of DipSwitch objects to write</param>
        /// <param name="writer">ClrMameProWriter representing the output</param>
        private static void WriteDipSwitches(DipSwitch[]? dipswitches, ClrMameProWriter writer)
        {
            // If the array is missing, we can't do anything
            if (dipswitches == null)
                return;

            foreach (var dipswitch in dipswitches)
            {
                writer.WriteStartElement("dipswitch");
                writer.WriteRequiredAttributeString("name", dipswitch.Name, throwOnError: true);
                foreach (var entry in dipswitch.Entry ?? [])
                {
                    writer.WriteRequiredAttributeString("entry", entry);
                }
                writer.WriteOptionalAttributeString("default", dipswitch.Default);
                writer.WriteEndElement(); // dipswitch
            }
        }

        /// <summary>
        /// Write driver information to the current writer
        /// </summary>
        /// <param name="driver">Driver object to write</param>
        /// <param name="writer">ClrMameProWriter representing the output</param>
        private static void WriteDriver(Driver? driver, ClrMameProWriter writer)
        {
            // If the item is missing, we can't do anything
            if (driver == null)
                return;

            writer.WriteStartElement("driver");
            writer.WriteRequiredAttributeString("status", driver.Status, throwOnError: true);
            writer.WriteOptionalAttributeString("color", driver.Color); // TODO: Probably actually required
            writer.WriteOptionalAttributeString("sound", driver.Sound); // TODO: Probably actually required
            writer.WriteOptionalAttributeString("palettesize", driver.PaletteSize);
            writer.WriteOptionalAttributeString("blit", driver.Blit);
            writer.WriteEndElement(); // driver
        }

        #endregion
    }
}

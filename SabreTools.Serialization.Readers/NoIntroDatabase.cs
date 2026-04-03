using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.NoIntroDatabase;

namespace SabreTools.Serialization.Readers
{
    public class NoIntroDatabase : BaseBinaryReader<Datafile>
    {
        /// <inheritdoc/>
        public override Datafile? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data is null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long initialOffset = data.Position;

                // Create the XmlTextReader
                var reader = new XmlTextReader(data);
                reader.WhitespaceHandling = WhitespaceHandling.None;

                // Parse the XML, if possible
                Datafile? datafile = null;
                while (reader.Read())
                {
                    // An ending element means exit
                    if (reader.NodeType == XmlNodeType.EndElement)
                        break;

                    // Only process starting elements
                    if (!reader.IsStartElement())
                        continue;

                    switch (reader.Name)
                    {
                        case "datafile":
                            if (datafile is not null && Debug)
                                Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                            datafile = ParseDatafile(reader);
                            break;

                        default:
                            if (Debug) Console.Error.WriteLine($"Element '{reader.Name}' is not recognized");
                            break;
                    }
                }

                return datafile;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <summary>
        /// Parse from an XmlTextReader into a Datafile
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Datafile on success, null on error</returns>
        public Datafile ParseDatafile(XmlTextReader reader)
        {
            var obj = new Datafile();

            List<Game> games = [];
            while (reader.Read())
            {
                // An ending element means exit
                if (reader.NodeType == XmlNodeType.EndElement)
                    break;

                // Only process starting elements
                if (!reader.IsStartElement())
                    continue;

                switch (reader.Name)
                {
                    case "game":
                        var game = ParseGame(reader);
                        if (game is not null)
                            games.Add(game);

                        break;

                    default:
                        if (Debug) Console.Error.WriteLine($"Element '{reader.Name}' is not recognized");
                        break;
                }
            }

            if (games.Count > 0)
                obj.Game = [.. games];

            return obj;
        }

        #region Items

        /// <summary>
        /// Parse from an XmlTextReader into a Archive
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Archive on success, null on error</returns>
        public Archive ParseArchive(XmlTextReader reader)
        {
            var obj = new Archive();

            obj.Number = reader.GetAttribute("number");
            obj.Clone = reader.GetAttribute("clone");
            obj.RegParent = reader.GetAttribute("regparent");
            obj.MergeOf = reader.GetAttribute("mergeof");
            obj.MergeName = reader.GetAttribute("mergename");
            obj.Name = reader.GetAttribute("name");
            obj.NameAlt = reader.GetAttribute("name_alt");
            obj.Region = reader.GetAttribute("region");
            obj.Languages = reader.GetAttribute("languages");
            obj.ShowLang = reader.GetAttribute("showlang");
            obj.LangChecked = reader.GetAttribute("langchecked");
            obj.Version1 = reader.GetAttribute("version1");
            obj.Version2 = reader.GetAttribute("version2");
            obj.DevStatus = reader.GetAttribute("devstatus");
            obj.Additional = reader.GetAttribute("additional");
            obj.Special1 = reader.GetAttribute("special1");
            obj.Special2 = reader.GetAttribute("special2");
            obj.Alt = reader.GetAttribute("alt");
            obj.GameId1 = reader.GetAttribute("gameid1");
            obj.GameId2 = reader.GetAttribute("gameid2");
            obj.Description = reader.GetAttribute("description");
            obj.Bios = reader.GetAttribute("bios");
            obj.Licensed = reader.GetAttribute("licensed");
            obj.Pirate = reader.GetAttribute("pirate");
            obj.Physical = reader.GetAttribute("physical");
            obj.Complete = reader.GetAttribute("complete");
            obj.Adult = reader.GetAttribute("adult");
            obj.Dat = reader.GetAttribute("dat");
            obj.Listed = reader.GetAttribute("listed");
            obj.Private = reader.GetAttribute("private");
            obj.StickyNote = reader.GetAttribute("sticky_note");
            obj.DatterNote = reader.GetAttribute("datter_note");
            obj.Categories = reader.GetAttribute("categories");

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a File
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled File on success, null on error</returns>
        public Data.Models.NoIntroDatabase.File ParseFile(XmlTextReader reader)
        {
            var obj = new Data.Models.NoIntroDatabase.File();

            obj.Id = reader.GetAttribute("id");
            obj.AppendToSourceId = reader.GetAttribute("append_to_source_id");
            obj.ForceName = reader.GetAttribute("forcename");
            obj.ForceSceneName = reader.GetAttribute("forcescenename");
            obj.EmptyDir = reader.GetAttribute("emptydir");
            obj.Extension = reader.GetAttribute("extension");
            obj.Item = reader.GetAttribute("item");
            obj.Date = reader.GetAttribute("date");
            obj.Format = reader.GetAttribute("format");
            obj.Note = reader.GetAttribute("note");
            obj.Filter = reader.GetAttribute("filter");
            obj.Version = reader.GetAttribute("version");
            obj.UpdateType = reader.GetAttribute("update_type");
            obj.Size = reader.GetAttribute("size");
            obj.CRC32 = reader.GetAttribute("crc32");
            obj.MD5 = reader.GetAttribute("md5");
            obj.SHA1 = reader.GetAttribute("sha1");
            obj.SHA256 = reader.GetAttribute("sha256");
            obj.Serial = reader.GetAttribute("serial");
            obj.Header = reader.GetAttribute("header");
            obj.Bad = reader.GetAttribute("bad");
            obj.MIA = reader.GetAttribute("mia");
            obj.Unique = reader.GetAttribute("unique");
            obj.MergeName = reader.GetAttribute("mergename");
            obj.UniqueAttachment = reader.GetAttribute("unique_attachment");

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a Game
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Game on success, null on error</returns>
        public Game ParseGame(XmlTextReader reader)
        {
            var obj = new Game();

            obj.Name = reader.GetAttribute("name");

            List<Media> medias = [];
            List<Source> sources = [];
            List<Release> releases = [];

            reader.Read();
            while (!reader.EOF)
            {
                // An ending element means exit
                if (reader.NodeType == XmlNodeType.EndElement)
                    break;

                // Only process starting elements
                if (!reader.IsStartElement())
                    continue;

                switch (reader.Name)
                {
                    case "archive":
                        if (obj.Archive is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Archive = ParseArchive(reader);
                        reader.Skip();
                        break;
                    case "media":
                        var media = ParseMedia(reader);
                        if (media is not null)
                            medias.Add(media);

                        reader.Skip();
                        break;
                    case "source":
                        var source = ParseSource(reader);
                        if (source is not null)
                            sources.Add(source);

                        reader.Skip();
                        break;
                    case "release":
                        var release = ParseRelease(reader);
                        if (release is not null)
                            releases.Add(release);

                        reader.Skip();
                        break;

                    default:
                        if (Debug) Console.Error.WriteLine($"Element '{reader.Name}' is not recognized");
                        reader.Skip();
                        break;
                }
            }

            if (medias.Count > 0)
                obj.Media = [.. medias];
            if (sources.Count > 0)
                obj.Source = [.. sources];
            if (releases.Count > 0)
                obj.Release = [.. releases];

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a Media
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Media on success, null on error</returns>
        public Media ParseMedia(XmlTextReader reader)
        {
            var obj = new Media();

            // This item is empty

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a Release
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Release on success, null on error</returns>
        public Release ParseRelease(XmlTextReader reader)
        {
            var obj = new Release();

            List<Data.Models.NoIntroDatabase.File> files = [];

            reader.Read();
            while (!reader.EOF)
            {
                // An ending element means exit
                if (reader.NodeType == XmlNodeType.EndElement)
                    break;

                // Only process starting elements
                if (!reader.IsStartElement())
                    continue;

                switch (reader.Name)
                {
                    case "details":
                        if (obj.Details is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Details = ParseReleaseDetails(reader);
                        reader.Skip();
                        break;
                    case "serials":
                        if (obj.Serials is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Serials = ParseSerials(reader);
                        reader.Skip();
                        break;
                    case "file":
                        var file = ParseFile(reader);
                        if (file is not null)
                            files.Add(file);

                        reader.Skip();
                        break;

                    default:
                        if (Debug) Console.Error.WriteLine($"Element '{reader.Name}' is not recognized");
                        reader.Skip();
                        break;
                }
            }

            if (files.Count > 0)
                obj.File = [.. files];

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a ReleaseDetails
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled ReleaseDetails on success, null on error</returns>
        public ReleaseDetails ParseReleaseDetails(XmlTextReader reader)
        {
            var obj = new ReleaseDetails();

            obj.Id = reader.GetAttribute("id");
            obj.AppendToNumber = reader.GetAttribute("append_to_number");
            obj.Date = reader.GetAttribute("date");
            obj.OriginalFormat = reader.GetAttribute("originalformat");
            obj.Group = reader.GetAttribute("group");
            obj.DirName = reader.GetAttribute("dirname");
            obj.NfoName = reader.GetAttribute("nfoname");
            obj.NfoSize = reader.GetAttribute("nfosize");
            obj.NfoCRC = reader.GetAttribute("nfocrc");
            obj.ArchiveName = reader.GetAttribute("archivename");
            obj.RomInfo = reader.GetAttribute("rominfo");
            obj.Category = reader.GetAttribute("category");
            obj.Comment = reader.GetAttribute("comment");
            obj.Tool = reader.GetAttribute("tool");
            obj.Region = reader.GetAttribute("region");
            obj.Origin = reader.GetAttribute("origin");

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a Serials
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Serials on success, null on error</returns>
        public Serials ParseSerials(XmlTextReader reader)
        {
            var obj = new Serials();

            obj.MediaSerial1 = reader.GetAttribute("media_serial1");
            obj.MediaSerial2 = reader.GetAttribute("media_serial2");
            obj.MediaSerial3 = reader.GetAttribute("media_serial3");
            obj.PCBSerial = reader.GetAttribute("pcb_serial");
            obj.RomChipSerial1 = reader.GetAttribute("romchip_serial1");
            obj.RomChipSerial2 = reader.GetAttribute("romchip_serial2");
            obj.LockoutSerial = reader.GetAttribute("lockout_serial");
            obj.SaveChipSerial = reader.GetAttribute("savechip_serial");
            obj.ChipSerial = reader.GetAttribute("chip_serial");
            obj.BoxSerial = reader.GetAttribute("box_serial");
            obj.MediaStamp = reader.GetAttribute("mediastamp");
            obj.BoxBarcode = reader.GetAttribute("box_barcode");
            obj.DigitalSerial1 = reader.GetAttribute("digital_serial1");
            obj.DigitalSerial2 = reader.GetAttribute("digital_serial2");

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a Source
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Source on success, null on error</returns>
        public Source ParseSource(XmlTextReader reader)
        {
            var obj = new Source();

            List<Data.Models.NoIntroDatabase.File> files = [];

            reader.Read();
            while (!reader.EOF)
            {
                // An ending element means exit
                if (reader.NodeType == XmlNodeType.EndElement)
                    break;

                // Only process starting elements
                if (!reader.IsStartElement())
                    continue;

                switch (reader.Name)
                {
                    case "details":
                        if (obj.Details is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Details = ParseSourceDetails(reader);
                        reader.Skip();
                        break;
                    case "serials":
                        if (obj.Serials is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Serials = ParseSerials(reader);
                        reader.Skip();
                        break;
                    case "file":
                        var file = ParseFile(reader);
                        if (file is not null)
                            files.Add(file);

                        reader.Skip();
                        break;

                    default:
                        if (Debug) Console.Error.WriteLine($"Element '{reader.Name}' is not recognized");
                        reader.Skip();
                        break;
                }
            }

            if (files.Count > 0)
                obj.File = [.. files];

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a SourceDetails
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled SourceDetails on success, null on error</returns>
        public SourceDetails ParseSourceDetails(XmlTextReader reader)
        {
            var obj = new SourceDetails();

            obj.Id = reader.GetAttribute("id");
            obj.AppendToNumber = reader.GetAttribute("append_to_number");
            obj.Section = reader.GetAttribute("section");
            obj.RomInfo = reader.GetAttribute("rominfo");
            obj.DumpDate = reader.GetAttribute("d_date");
            obj.DumpDateInfo = reader.GetAttribute("d_date_info").AsYesNo();
            obj.ReleaseDate = reader.GetAttribute("r_date");
            obj.ReleaseDateInfo = reader.GetAttribute("r_date_info").AsYesNo();
            obj.Dumper = reader.GetAttribute("dumper");
            obj.Project = reader.GetAttribute("project");
            obj.OriginalFormat = reader.GetAttribute("originalformat");
            obj.Nodump = reader.GetAttribute("nodump").AsYesNo();
            obj.Tool = reader.GetAttribute("tool");
            obj.Origin = reader.GetAttribute("origin");
            obj.Comment1 = reader.GetAttribute("comment1");
            obj.Comment2 = reader.GetAttribute("comment2");
            obj.Link1 = reader.GetAttribute("link1");
            obj.Link1Public = reader.GetAttribute("link1_public").AsYesNo();
            obj.Link2 = reader.GetAttribute("link2");
            obj.Link2Public = reader.GetAttribute("link2_public").AsYesNo();
            obj.Link3 = reader.GetAttribute("link3");
            obj.Link3Public = reader.GetAttribute("link3_public").AsYesNo();
            obj.Region = reader.GetAttribute("region");
            obj.MediaTitle = reader.GetAttribute("media_title");

            return obj;
        }

        #endregion
    }
}

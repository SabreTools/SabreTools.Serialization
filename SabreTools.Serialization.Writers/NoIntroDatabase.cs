using System.IO;
using System.Text;
using System.Xml;
using SabreTools.Data.Models.NoIntroDatabase;
using SabreTools.IO.Extensions;
using SabreTools.Text.Extensions;

namespace SabreTools.Serialization.Writers
{
    public class NoIntroDatabase : BaseBinaryWriter<Datafile>
    {
        /// <inheritdoc/>
        public override Stream? SerializeStream(Datafile? obj)
        {
            // If the metadata file is null
            if (obj is null)
                return null;

            // Setup the writer and output
            var stream = new MemoryStream();
            var writer = new XmlTextWriter(stream, Encoding.UTF8)
            {
                Formatting = Formatting.Indented,
                IndentChar = '\t',
                Indentation = 1
            };
            writer.Settings?.CheckCharacters = false;
            writer.Settings?.NewLineChars = "\n";

            // Write document start
            writer.WriteStartDocument();

            // Write the SoftwareDb, if it exists
            WriteDatafile(obj, writer);
            writer.Flush();

            // Return the stream
            stream.SeekIfPossible(0, SeekOrigin.Begin);
            return stream;
        }

        /// <summary>
        /// Write a Datafile to an XmlTextWriter
        /// </summary>
        /// <param name="obj">Datafile to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteDatafile(Datafile obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("datafile");

            if (obj.Game is not null && obj.Game.Length > 0)
            {
                foreach (var game in obj.Game)
                {
                    WriteGame(game, writer);
                }
            }

            writer.WriteEndElement();
        }

        #region Items

        /// <summary>
        /// Write a Archive to an XmlTextWriter
        /// </summary>
        /// <param name="obj">Archive to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteArchive(Archive obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("archive");

            writer.WriteOptionalAttributeString("number", obj.Number);
            writer.WriteOptionalAttributeString("clone", obj.Clone);
            writer.WriteOptionalAttributeString("regparent", obj.RegParent);
            writer.WriteOptionalAttributeString("mergeof", obj.MergeOf);
            writer.WriteOptionalAttributeString("mergename", obj.MergeName);
            writer.WriteOptionalAttributeString("name", obj.Name);
            writer.WriteOptionalAttributeString("name_alt", obj.NameAlt);
            writer.WriteOptionalAttributeString("region", obj.Region);
            writer.WriteOptionalAttributeString("languages", obj.Languages);
            writer.WriteOptionalAttributeString("showlang", obj.ShowLang);
            writer.WriteOptionalAttributeString("langchecked", obj.LangChecked);
            writer.WriteOptionalAttributeString("version1", obj.Version1);
            writer.WriteOptionalAttributeString("version2", obj.Version2);
            writer.WriteOptionalAttributeString("devstatus", obj.DevStatus);
            writer.WriteOptionalAttributeString("additional", obj.Additional);
            writer.WriteOptionalAttributeString("special1", obj.Special1);
            writer.WriteOptionalAttributeString("special2", obj.Special2);
            writer.WriteOptionalAttributeString("alt", obj.Alt);
            writer.WriteOptionalAttributeString("gameid1", obj.GameId1);
            writer.WriteOptionalAttributeString("gameid2", obj.GameId2);
            writer.WriteOptionalAttributeString("description", obj.Description);
            writer.WriteOptionalAttributeString("bios", obj.Bios);
            writer.WriteOptionalAttributeString("licensed", obj.Licensed);
            writer.WriteOptionalAttributeString("pirate", obj.Pirate);
            writer.WriteOptionalAttributeString("physical", obj.Physical);
            writer.WriteOptionalAttributeString("complete", obj.Complete);
            writer.WriteOptionalAttributeString("adult", obj.Adult);
            writer.WriteOptionalAttributeString("dat", obj.Dat);
            writer.WriteOptionalAttributeString("listed", obj.Listed);
            writer.WriteOptionalAttributeString("private", obj.Private);
            writer.WriteOptionalAttributeString("sticky_note", obj.StickyNote);
            writer.WriteOptionalAttributeString("datter_note", obj.DatterNote);
            writer.WriteOptionalAttributeString("categories", obj.Categories);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a File to an XmlTextWriter
        /// </summary>
        /// <param name="obj">File to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteFile(Data.Models.NoIntroDatabase.File obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("file");

            writer.WriteOptionalAttributeString("id", obj.Id);
            writer.WriteOptionalAttributeString("append_to_source_id", obj.AppendToSourceId);
            writer.WriteOptionalAttributeString("forcename", obj.ForceName);
            writer.WriteOptionalAttributeString("forcescenename", obj.ForceSceneName);
            writer.WriteOptionalAttributeString("emptydir", obj.EmptyDir);
            writer.WriteOptionalAttributeString("extension", obj.Extension);
            writer.WriteOptionalAttributeString("item", obj.Item);
            writer.WriteOptionalAttributeString("date", obj.Date);
            writer.WriteOptionalAttributeString("format", obj.Format);
            writer.WriteOptionalAttributeString("note", obj.Note);
            writer.WriteOptionalAttributeString("filter", obj.Filter);
            writer.WriteOptionalAttributeString("version", obj.Version);
            writer.WriteOptionalAttributeString("update_type", obj.UpdateType);
            writer.WriteOptionalAttributeString("size", obj.Size);
            writer.WriteOptionalAttributeString("crc32", obj.CRC32);
            writer.WriteOptionalAttributeString("md5", obj.MD5);
            writer.WriteOptionalAttributeString("sha1", obj.SHA1);
            writer.WriteOptionalAttributeString("sha256", obj.SHA256);
            writer.WriteOptionalAttributeString("serial", obj.Serial);
            writer.WriteOptionalAttributeString("header", obj.Header);
            writer.WriteOptionalAttributeString("bad", obj.Bad);
            writer.WriteOptionalAttributeString("mia", obj.MIA);
            writer.WriteOptionalAttributeString("unique", obj.Unique);
            writer.WriteOptionalAttributeString("mergename", obj.MergeName);
            writer.WriteOptionalAttributeString("unique_attachment", obj.UniqueAttachment);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a Game to an XmlTextWriter
        /// </summary>
        /// <param name="obj">Game to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteGame(Game obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("game");

            writer.WriteOptionalAttributeString("name", obj.Name);

            if (obj.Archive is not null)
                WriteArchive(obj.Archive, writer);

            if (obj.Media is not null && obj.Media.Length > 0)
            {
                foreach (var media in obj.Media)
                {
                    WriteMedia(media, writer);
                }
            }

            if (obj.Source is not null && obj.Source.Length > 0)
            {
                foreach (var source in obj.Source)
                {
                    WriteSource(source, writer);
                }
            }

            if (obj.Release is not null && obj.Release.Length > 0)
            {
                foreach (var release in obj.Release)
                {
                    WriteRelease(release, writer);
                }
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a Media to an XmlTextWriter
        /// </summary>
        /// <param name="obj">Media to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteMedia(Media obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("media");

            // This item is empty

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a Release to an XmlTextWriter
        /// </summary>
        /// <param name="obj">Release to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteRelease(Release obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("release");

            if (obj.Details is not null)
                WriteReleaseDetails(obj.Details, writer);

            if (obj.Serials is not null)
                WriteSerials(obj.Serials, writer);

            if (obj.File is not null && obj.File.Length > 0)
            {
                foreach (var file in obj.File)
                {
                    WriteFile(file, writer);
                }
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a ReleaseDetails to an XmlTextWriter
        /// </summary>
        /// <param name="obj">ReleaseDetails to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteReleaseDetails(ReleaseDetails obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("details");

            writer.WriteOptionalAttributeString("id", obj.Id);
            writer.WriteOptionalAttributeString("append_to_number", obj.AppendToNumber);
            writer.WriteOptionalAttributeString("date", obj.Date);
            writer.WriteOptionalAttributeString("originalformat", obj.OriginalFormat);
            writer.WriteOptionalAttributeString("group", obj.Group);
            writer.WriteOptionalAttributeString("dirname", obj.DirName);
            writer.WriteOptionalAttributeString("nfoname", obj.NfoName);
            writer.WriteOptionalAttributeString("nfosize", obj.NfoSize);
            writer.WriteOptionalAttributeString("nfocrc", obj.NfoCRC);
            writer.WriteOptionalAttributeString("archivename", obj.ArchiveName);
            writer.WriteOptionalAttributeString("rominfo", obj.RomInfo);
            writer.WriteOptionalAttributeString("category", obj.Category);
            writer.WriteOptionalAttributeString("comment", obj.Comment);
            writer.WriteOptionalAttributeString("tool", obj.Tool);
            writer.WriteOptionalAttributeString("region", obj.Region);
            writer.WriteOptionalAttributeString("origin", obj.Origin);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a Serials to an XmlTextWriter
        /// </summary>
        /// <param name="obj">Serials to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteSerials(Serials obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("serials");

            writer.WriteOptionalAttributeString("media_serial1", obj.MediaSerial1);
            writer.WriteOptionalAttributeString("media_serial2", obj.MediaSerial2);
            writer.WriteOptionalAttributeString("media_serial3", obj.MediaSerial3);
            writer.WriteOptionalAttributeString("pcb_serial", obj.PCBSerial);
            writer.WriteOptionalAttributeString("romchip_serial1", obj.RomChipSerial1);
            writer.WriteOptionalAttributeString("romchip_serial2", obj.RomChipSerial2);
            writer.WriteOptionalAttributeString("lockout_serial", obj.LockoutSerial);
            writer.WriteOptionalAttributeString("savechip_serial", obj.SaveChipSerial);
            writer.WriteOptionalAttributeString("chip_serial", obj.ChipSerial);
            writer.WriteOptionalAttributeString("box_serial", obj.BoxSerial);
            writer.WriteOptionalAttributeString("mediastamp", obj.MediaStamp);
            writer.WriteOptionalAttributeString("box_barcode", obj.BoxBarcode);
            writer.WriteOptionalAttributeString("digital_serial1", obj.DigitalSerial1);
            writer.WriteOptionalAttributeString("digital_serial2", obj.DigitalSerial2);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a Source to an XmlTextWriter
        /// </summary>
        /// <param name="obj">Source to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteSource(Source obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("source");

            if (obj.Details is not null)
                WriteSourceDetails(obj.Details, writer);

            if (obj.Serials is not null)
                WriteSerials(obj.Serials, writer);

            if (obj.File is not null && obj.File.Length > 0)
            {
                foreach (var file in obj.File)
                {
                    WriteFile(file, writer);
                }
            }

             writer.WriteEndElement();
        }

        /// <summary>
        /// Write a SourceDetails to an XmlTextWriter
        /// </summary>
        /// <param name="obj">SourceDetails to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteSourceDetails(SourceDetails obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("details");

            writer.WriteOptionalAttributeString("id", obj.Id);
            writer.WriteOptionalAttributeString("append_to_number", obj.AppendToNumber);
            writer.WriteOptionalAttributeString("section", obj.Section);
            writer.WriteOptionalAttributeString("rominfo", obj.RomInfo);
            writer.WriteOptionalAttributeString("d_date", obj.DumpDate);
            writer.WriteOptionalAttributeString("d_date_info", obj.DumpDateInfo);
            writer.WriteOptionalAttributeString("r_date", obj.ReleaseDate);
            writer.WriteOptionalAttributeString("r_date_info", obj.ReleaseDateInfo);
            writer.WriteOptionalAttributeString("dumper", obj.Dumper);
            writer.WriteOptionalAttributeString("project", obj.Project);
            writer.WriteOptionalAttributeString("originalformat", obj.OriginalFormat);
            writer.WriteOptionalAttributeString("nodump", obj.Nodump);
            writer.WriteOptionalAttributeString("tool", obj.Tool);
            writer.WriteOptionalAttributeString("origin", obj.Origin);
            writer.WriteOptionalAttributeString("comment1", obj.Comment1);
            writer.WriteOptionalAttributeString("comment2", obj.Comment2);
            writer.WriteOptionalAttributeString("link1", obj.Link1);
            writer.WriteOptionalAttributeString("link1_public", obj.Link1Public);
            writer.WriteOptionalAttributeString("link2", obj.Link2);
            writer.WriteOptionalAttributeString("link2_public", obj.Link2Public);
            writer.WriteOptionalAttributeString("link3", obj.Link3);
            writer.WriteOptionalAttributeString("link3_public", obj.Link3Public);
            writer.WriteOptionalAttributeString("region", obj.Region);
            writer.WriteOptionalAttributeString("media_title", obj.MediaTitle);

            writer.WriteEndElement();
        }

        #endregion
    }
}

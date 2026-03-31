using System.IO;
using System.Text;
using System.Xml;
using SabreTools.Data.Models.Logiqx;
using SabreTools.IO.Extensions;
using SabreTools.Text.Extensions;

namespace SabreTools.Serialization.Writers
{
    public class Logiqx : BaseBinaryWriter<Datafile>
    {
        #region Constants

        /// <summary>
        /// name field for DOCTYPE
        /// </summary>
        public const string DocTypeName = "datafile";

        /// <summary>
        /// pubid field for DOCTYPE
        /// </summary>
        public const string DocTypePubId = "-//Logiqx//DTD ROM Management Datafile//EN";

        /// <summary>
        /// sysid field for DOCTYPE
        /// </summary>
        public const string DocTypeSysId = "http://www.logiqx.com/Dats/datafile.dtd";

        /// <summary>
        /// subset field for DOCTYPE
        /// </summary>
        public const string? DocTypeSubset = null;

        /// <summary>
        /// xmlns:xsi field for datafile
        /// </summary>
        public const string? DatafileXmlnsXsi = "http://www.w3.org/2001/XMLSchema-instance";

        /// <summary>
        /// xsi:schemaLocation field for datafile
        /// </summary>
        public const string? DatafileXsiSchemaLocation = "https://datomatic.no-intro.org/stuff https://datomatic.no-intro.org/stuff/schema_nointro_datfile_v3.xsd";

        #endregion

        /// <inheritdoc/>
        public override Stream? SerializeStream(Datafile? obj)
        {
            // If the metadata file is null
            if (obj is null)
                return null;

            // Setup the writer and output
            var stream = new MemoryStream();
            var writer = new XmlTextWriter(stream, Encoding.UTF8);
            writer.Formatting = Formatting.Indented;

            // Write document start
            writer.WriteStartDocument();

            // Write document type
            writer.WriteDocType(DocTypeName, DocTypePubId, DocTypeSysId, DocTypeSubset);

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

            writer.WriteOptionalAttributeString("build", obj.Build);
            writer.WriteOptionalAttributeString("debug", obj.Debug);

            // TODO: Fix schema location writing
            // writer.WriteOptionalAttributeString("schemaLocation", obj.SchemaLocation);

            if (obj.Header is not null)
                WriteHeader(obj.Header, writer);

            if (obj.Game is not null && obj.Game.Length > 0)
            {
                foreach (var gameBase in obj.Game)
                {
                    WriteGameBase(gameBase, writer);
                }
            }

            if (obj.Dir is not null && obj.Dir.Length > 0)
            {
                foreach (var dir in obj.Dir)
                {
                    WriteDir(dir, writer);
                }
            }

            writer.WriteEndElement();
        }

        #region Header

        /// <summary>
        /// Write a ClrMamePro to an XmlTextWriter
        /// </summary>
        /// <param name="obj">ClrMamePro to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteClrMamePro(Data.Models.Logiqx.ClrMamePro obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("clrmamepro");

            writer.WriteOptionalAttributeString("header", obj.Header);
            writer.WriteOptionalAttributeString("forcemerging", obj.ForceMerging);
            writer.WriteOptionalAttributeString("forcenodump", obj.ForceNodump);
            writer.WriteOptionalAttributeString("forcepacking", obj.ForcePacking);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a Header to an XmlTextWriter
        /// </summary>
        /// <param name="obj">Header to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteHeader(Header obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("header");

            writer.WriteOptionalElementString("id", obj.Id);
            writer.WriteRequiredElementString("name", obj.Name);
            writer.WriteRequiredElementString("description", obj.Description);
            writer.WriteOptionalElementString("rootdir", obj.RootDir);
            writer.WriteOptionalElementString("category", obj.Category);
            writer.WriteRequiredElementString("version", obj.Version);
            writer.WriteOptionalElementString("date", obj.Date);
            writer.WriteRequiredElementString("author", obj.Author);
            writer.WriteOptionalElementString("email", obj.Email);
            writer.WriteOptionalElementString("homepage", obj.Homepage);
            writer.WriteOptionalElementString("url", obj.Url);
            writer.WriteOptionalElementString("comment", obj.Comment);
            writer.WriteOptionalElementString("type", obj.Type);

            if (obj.ClrMamePro is not null)
                WriteClrMamePro(obj.ClrMamePro, writer);

            if (obj.RomCenter is not null)
                WriteRomCenter(obj.RomCenter, writer);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a RomCenter to an XmlTextWriter
        /// </summary>
        /// <param name="obj">RomCenter to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteRomCenter(Data.Models.Logiqx.RomCenter obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("romcenter");

            writer.WriteOptionalAttributeString("plugin", obj.Plugin);
            writer.WriteOptionalAttributeString("rommode", obj.RomMode);
            writer.WriteOptionalAttributeString("biosmode", obj.BiosMode);
            writer.WriteOptionalAttributeString("samplemode", obj.SampleMode);
            writer.WriteOptionalAttributeString("lockrommode", obj.LockRomMode);
            writer.WriteOptionalAttributeString("lockbiosmode", obj.LockBiosMode);
            writer.WriteOptionalAttributeString("locksamplemode", obj.LockSampleMode);

            writer.WriteEndElement();
        }

        #endregion

        #region Items

        /// <summary>
        /// Write a Archive to an XmlTextWriter
        /// </summary>
        /// <param name="obj">Archive to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteArchive(Archive obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("archive");

            writer.WriteRequiredAttributeString("name", obj.Name);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a BiosSet to an XmlTextWriter
        /// </summary>
        /// <param name="obj">BiosSet to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteBiosSet(BiosSet obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("biosset");

            writer.WriteRequiredAttributeString("name", obj.Name);
            writer.WriteRequiredAttributeString("description", obj.Description);
            writer.WriteOptionalAttributeString("default", obj.Default);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a DeviceRef to an XmlTextWriter
        /// </summary>
        /// <param name="obj">DeviceRef to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteDeviceRef(DeviceRef obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("device_ref");

            writer.WriteRequiredAttributeString("name", obj.Name);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a Dir to an XmlTextWriter
        /// </summary>
        /// <param name="obj">Dir to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteDir(Dir obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("dir");

            writer.WriteRequiredAttributeString("name", obj.Name);

            if (obj.Subdir is not null && obj.Subdir.Length > 0)
            {
                foreach (var subdir in obj.Subdir)
                {
                    WriteDir(subdir, writer);
                }
            }

            if (obj.Game is not null && obj.Game.Length > 0)
            {
                foreach (var game in obj.Game)
                {
                    WriteGameBase(game, writer);
                }
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a Disk to an XmlTextWriter
        /// </summary>
        /// <param name="obj">Disk to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteDisk(Disk obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("disk");

            writer.WriteRequiredAttributeString("name", obj.Name);
            writer.WriteOptionalAttributeString("md5", obj.MD5);
            writer.WriteOptionalAttributeString("sha1", obj.SHA1);
            writer.WriteOptionalAttributeString("merge", obj.Merge);
            writer.WriteOptionalAttributeString("status", obj.Status);
            writer.WriteOptionalAttributeString("region", obj.Region);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a Driver to an XmlTextWriter
        /// </summary>
        /// <param name="obj">Driver to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteDriver(Driver obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("driver");

            writer.WriteRequiredAttributeString("status", obj.Status);
            writer.WriteRequiredAttributeString("emulation", obj.Emulation);
            writer.WriteRequiredAttributeString("cocktail", obj.Cocktail);
            writer.WriteRequiredAttributeString("savestate", obj.SaveState);
            writer.WriteOptionalAttributeString("requiresartwork", obj.RequiresArtwork);
            writer.WriteOptionalAttributeString("unofficial", obj.Unofficial);
            writer.WriteOptionalAttributeString("nosoundhardware", obj.NoSoundHardware);
            writer.WriteOptionalAttributeString("incomplete", obj.Incomplete);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a GameBase to an XmlTextWriter
        /// </summary>
        /// <param name="obj">GameBase to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteGameBase(GameBase obj, XmlTextWriter writer)
        {
            if (obj is Game)
                writer.WriteStartElement("game");
            else if (obj is Machine)
                writer.WriteStartElement("machine");
            else
                return;

            writer.WriteRequiredAttributeString("name", obj.Name);
            writer.WriteOptionalAttributeString("sourcefile", obj.SourceFile);
            writer.WriteOptionalAttributeString("isbios", obj.IsBios);
            writer.WriteOptionalAttributeString("isdevice", obj.IsDevice);
            writer.WriteOptionalAttributeString("ismechanical", obj.IsMechanical);
            writer.WriteOptionalAttributeString("cloneof", obj.CloneOf);
            writer.WriteOptionalAttributeString("romof", obj.RomOf);
            writer.WriteOptionalAttributeString("sampleof", obj.SampleOf);
            writer.WriteOptionalAttributeString("board", obj.Board);
            writer.WriteOptionalAttributeString("rebuildto", obj.RebuildTo);
            writer.WriteOptionalAttributeString("id", obj.Id);
            writer.WriteOptionalAttributeString("cloneofid", obj.CloneOfId);
            writer.WriteOptionalAttributeString("runnable", obj.Runnable);

            if (obj.Comment is not null && obj.Comment.Length > 0)
            {
                foreach (var comment in obj.Comment)
                {
                    writer.WriteRequiredElementString("comment", comment);
                }
            }

            writer.WriteRequiredElementString("description", obj.Description);
            writer.WriteOptionalElementString("year", obj.Year);
            writer.WriteOptionalElementString("manufacturer", obj.Manufacturer);
            writer.WriteOptionalElementString("publisher", obj.Publisher);

            if (obj.Category is not null && obj.Category.Length > 0)
            {
                foreach (var category in obj.Category)
                {
                    writer.WriteRequiredElementString("category", category);
                }
            }

            if (obj.Trurip is not null)
                WriteTrurip(obj.Trurip, writer);

            if (obj.Release is not null && obj.Release.Length > 0)
            {
                foreach (var release in obj.Release)
                {
                    WriteRelease(release, writer);
                }
            }

            if (obj.BiosSet is not null && obj.BiosSet.Length > 0)
            {
                foreach (var biosSet in obj.BiosSet)
                {
                    WriteBiosSet(biosSet, writer);
                }
            }

            if (obj.Rom is not null && obj.Rom.Length > 0)
            {
                foreach (var rom in obj.Rom)
                {
                    WriteRom(rom, writer);
                }
            }

            if (obj.Disk is not null && obj.Disk.Length > 0)
            {
                foreach (var disk in obj.Disk)
                {
                    WriteDisk(disk, writer);
                }
            }

            if (obj.Media is not null && obj.Media.Length > 0)
            {
                foreach (var media in obj.Media)
                {
                    WriteMedia(media, writer);
                }
            }

            if (obj.DeviceRef is not null && obj.DeviceRef.Length > 0)
            {
                foreach (var deviceRef in obj.DeviceRef)
                {
                    WriteDeviceRef(deviceRef, writer);
                }
            }

            if (obj.Sample is not null && obj.Sample.Length > 0)
            {
                foreach (var sample in obj.Sample)
                {
                    WriteSample(sample, writer);
                }
            }

            if (obj.Archive is not null && obj.Archive.Length > 0)
            {
                foreach (var archive in obj.Archive)
                {
                    WriteArchive(archive, writer);
                }
            }

            if (obj.Driver is not null)
                WriteDriver(obj.Driver, writer);

            if (obj.SoftwareList is not null && obj.SoftwareList.Length > 0)
            {
                foreach (var softwareList in obj.SoftwareList)
                {
                    WriteSoftwareList(softwareList, writer);
                }
            }

            writer.WriteOptionalElementString("url", obj.Url);
            writer.WriteOptionalElementString("hash", obj.Hash);

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

            writer.WriteRequiredAttributeString("name", obj.Name);
            writer.WriteOptionalAttributeString("md5", obj.MD5);
            writer.WriteOptionalAttributeString("sha1", obj.SHA1);
            writer.WriteOptionalAttributeString("sha256", obj.SHA256);
            writer.WriteOptionalAttributeString("spamsum", obj.SpamSum);

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

            writer.WriteRequiredAttributeString("name", obj.Name);
            writer.WriteRequiredAttributeString("region", obj.Region);
            writer.WriteOptionalAttributeString("language", obj.Language);
            writer.WriteOptionalAttributeString("date", obj.Date);
            writer.WriteOptionalAttributeString("default", obj.Default);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a Rom to an XmlTextWriter
        /// </summary>
        /// <param name="obj">Rom to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteRom(Rom obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("rom");

            writer.WriteRequiredAttributeString("name", obj.Name);
            writer.WriteRequiredAttributeString("size", obj.Size);
            writer.WriteOptionalAttributeString("crc16", obj.CRC16);
            writer.WriteOptionalAttributeString("crc", obj.CRC);
            writer.WriteOptionalAttributeString("crc64", obj.CRC64);
            writer.WriteOptionalAttributeString("md2", obj.MD2);
            writer.WriteOptionalAttributeString("md4", obj.MD4);
            writer.WriteOptionalAttributeString("md5", obj.MD5);
            writer.WriteOptionalAttributeString("ripemd128", obj.RIPEMD128);
            writer.WriteOptionalAttributeString("ripemd160", obj.RIPEMD160);
            writer.WriteOptionalAttributeString("sha1", obj.SHA1);
            writer.WriteOptionalAttributeString("sha256", obj.SHA256);
            writer.WriteOptionalAttributeString("sha384", obj.SHA384);
            writer.WriteOptionalAttributeString("sha512", obj.SHA512);
            writer.WriteOptionalAttributeString("spamsum", obj.SpamSum);
            writer.WriteOptionalAttributeString("xxh3_64", obj.xxHash364);
            writer.WriteOptionalAttributeString("xxh3_128", obj.xxHash3128);
            writer.WriteOptionalAttributeString("merge", obj.Merge);
            writer.WriteOptionalAttributeString("status", obj.Status);
            writer.WriteOptionalAttributeString("serial", obj.Serial);
            writer.WriteOptionalAttributeString("header", obj.Header);
            writer.WriteOptionalAttributeString("date", obj.Date);
            writer.WriteOptionalAttributeString("inverted", obj.Inverted);
            writer.WriteOptionalAttributeString("mia", obj.MIA);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a Sample to an XmlTextWriter
        /// </summary>
        /// <param name="obj">Sample to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteSample(Sample obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("sample");

            writer.WriteRequiredAttributeString("name", obj.Name);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a SoftwareList to an XmlTextWriter
        /// </summary>
        /// <param name="obj">SoftwareList to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteSoftwareList(Data.Models.Logiqx.SoftwareList obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("softwarelist");

            writer.WriteRequiredAttributeString("tag", obj.Tag);
            writer.WriteRequiredAttributeString("name", obj.Name);
            writer.WriteRequiredAttributeString("status", obj.Status);
            writer.WriteOptionalAttributeString("filter", obj.Filter);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a Trurip to an XmlTextWriter
        /// </summary>
        /// <param name="obj">Trurip to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteTrurip(Trurip obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("trurip");

            writer.WriteOptionalElementString("titleid", obj.TitleID);
            writer.WriteOptionalElementString("publisher", obj.Publisher);
            writer.WriteOptionalElementString("developer", obj.Developer);
            writer.WriteOptionalElementString("year", obj.Year);
            writer.WriteOptionalElementString("genre", obj.Genre);
            writer.WriteOptionalElementString("subgenre", obj.Subgenre);
            writer.WriteOptionalElementString("ratings", obj.Ratings);
            writer.WriteOptionalElementString("score", obj.Score);
            writer.WriteOptionalElementString("players", obj.Players);
            writer.WriteOptionalElementString("enabled", obj.Enabled);
            writer.WriteOptionalElementString("crc", obj.CRC);
            writer.WriteOptionalElementString("source", obj.Source);
            writer.WriteOptionalElementString("cloneof", obj.CloneOf);
            writer.WriteOptionalElementString("relatedto", obj.RelatedTo);

            writer.WriteEndElement();
        }

        #endregion
    }
}

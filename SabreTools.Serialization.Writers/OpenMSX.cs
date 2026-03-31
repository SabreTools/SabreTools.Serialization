using System.IO;
using System.Text;
using System.Xml;
using SabreTools.Data.Models.OpenMSX;
using SabreTools.IO.Extensions;
using SabreTools.Text.Extensions;

namespace SabreTools.Serialization.Writers
{
    public class OpenMSX : BaseBinaryWriter<SoftwareDb>
    {
        #region Constants

        /// <summary>
        /// name field for DOCTYPE
        /// </summary>
        public const string DocTypeName = "softwaredb";

        /// <summary>
        /// pubid field for DOCTYPE
        /// </summary>
        public const string? DocTypePubId = null;

        /// <summary>
        /// sysid field for DOCTYPE
        /// </summary>
        public const string DocTypeSysId = "softwaredb1.dtd";

        /// <summary>
        /// subset field for DOCTYPE
        /// </summary>
        public const string? DocTypeSubset = null;

        #endregion

        /// <inheritdoc/>
        public override Stream? SerializeStream(SoftwareDb? obj)
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

            // Write document type
            writer.WriteDocType(DocTypeName, DocTypePubId, DocTypeSysId, DocTypeSubset);

            // Write the SoftwareDb, if it exists
            WriteSoftwareDb(obj, writer);
            writer.Flush();

            // Return the stream
            stream.SeekIfPossible(0, SeekOrigin.Begin);
            return stream;
        }

        /// <summary>
        /// Write a SoftwareDb to an XmlTextWriter
        /// </summary>
        /// <param name="obj">SoftwareDb to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteSoftwareDb(SoftwareDb obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("softwaredb");

            writer.WriteOptionalAttributeString("timestamp", obj.Timestamp);

            if (obj.Software is not null && obj.Software.Length > 0)
            {
                foreach (var software in obj.Software)
                {
                    WriteSoftware(software, writer);
                }
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a Dump to an XmlTextWriter
        /// </summary>
        /// <param name="obj">Dump to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteDump(Dump obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("dump");

            if (obj.Original is not null)
                WriteOriginal(obj.Original, writer);

            if (obj.Rom is not null)
                WriteRomBase(obj.Rom, writer);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a Original to an XmlTextWriter
        /// </summary>
        /// <param name="obj">Original to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteOriginal(Original obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("original");

            writer.WriteOptionalAttributeString("value", obj.Value);

            if (obj.Content is not null)
                writer.WriteRaw(obj.Content);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a RomBase to an XmlTextWriter
        /// </summary>
        /// <param name="obj">RomBase to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteRomBase(RomBase obj, XmlTextWriter writer)
        {
            if (obj is MegaRom)
                writer.WriteStartElement("megarom");
            else if (obj is Rom)
                writer.WriteStartElement("rom");
            else if (obj is SCCPlusCart)
                writer.WriteStartElement("sccpluscart");
            else
                return;

            writer.WriteOptionalElementString("start", obj.Start);
            writer.WriteOptionalElementString("type", obj.Type);
            writer.WriteOptionalElementString("hash", obj.Hash);
            writer.WriteOptionalElementString("remark", obj.Remark);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a Software to an XmlTextWriter
        /// </summary>
        /// <param name="obj">Software to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteSoftware(Software obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("software");

            writer.WriteRequiredElementString("title", obj.Title);
            writer.WriteOptionalElementString("genmsxid", obj.GenMSXID);
            writer.WriteRequiredElementString("system", obj.System);
            writer.WriteRequiredElementString("company", obj.Company);
            writer.WriteRequiredElementString("year", obj.Year);
            writer.WriteRequiredElementString("country", obj.Country);

            if (obj.Dump is not null && obj.Dump.Length > 0)
            {
                foreach (var dump in obj.Dump)
                {
                    WriteDump(dump, writer);
                }
            }

            writer.WriteEndElement();
        }
    }
}

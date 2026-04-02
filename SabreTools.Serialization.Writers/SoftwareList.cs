using System.IO;
using System.Text;
using System.Xml;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.SoftwareList;
using SabreTools.IO.Extensions;
using SabreTools.Text.Extensions;

namespace SabreTools.Serialization.Writers
{
    public class SoftwareList : BaseBinaryWriter<Data.Models.SoftwareList.SoftwareList>
    {
        #region Constants

        /// <summary>
        /// name field for DOCTYPE
        /// </summary>
        public const string DocTypeName = "softwarelist";

        /// <summary>
        /// pubid field for DOCTYPE
        /// </summary>
        public const string? DocTypePubId = null;

        /// <summary>
        /// sysid field for DOCTYPE
        /// </summary>
        public const string DocTypeSysId = "softwarelist.dtd";

        /// <summary>
        /// subset field for DOCTYPE
        /// </summary>
        public const string? DocTypeSubset = null;

        #endregion

        /// <inheritdoc/>
        public override Stream? SerializeStream(Data.Models.SoftwareList.SoftwareList? obj)
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
            WriteSoftwareList(obj, writer);
            writer.Flush();

            // Return the stream
            stream.SeekIfPossible(0, SeekOrigin.Begin);
            return stream;
        }

        /// <summary>
        /// Write a SoftwareList to an XmlTextWriter
        /// </summary>
        /// <param name="obj">SoftwareList to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteSoftwareList(Data.Models.SoftwareList.SoftwareList obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("softwarelist");

            writer.WriteRequiredAttributeString("name", obj.Name);
            writer.WriteOptionalAttributeString("description", obj.Description);

            writer.WriteOptionalElementString("notes", obj.Notes);

            if (obj.Software is not null && obj.Software.Length > 0)
            {
                foreach (var software in obj.Software)
                {
                    WriteSoftware(software, writer);
                }
            }

            writer.WriteEndElement();
        }

        #region Items

        /// <summary>
        /// Write a DataArea to an XmlTextWriter
        /// </summary>
        /// <param name="obj">DataArea to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteDataArea(DataArea obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("dataarea");

            writer.WriteRequiredAttributeString("name", obj.Name);
            writer.WriteRequiredAttributeString("size", obj.Size);
            writer.WriteOptionalAttributeString("width", obj.Width);
            writer.WriteOptionalAttributeString("endianness", obj.Endianness?.AsStringValue());

            if (obj.Rom is not null && obj.Rom.Length > 0)
            {
                foreach (var rom in obj.Rom)
                {
                    WriteRom(rom, writer);
                }
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a DipSwitch to an XmlTextWriter
        /// </summary>
        /// <param name="obj">DipSwitch to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteDipSwitch(DipSwitch obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("dipswitch");

            writer.WriteRequiredAttributeString("name", obj.Name);
            writer.WriteRequiredAttributeString("tag", obj.Tag);
            writer.WriteOptionalAttributeString("mask", obj.Mask);

            if (obj.DipValue is not null && obj.DipValue.Length > 0)
            {
                foreach (var dipValue in obj.DipValue)
                {
                    WriteDipValue(dipValue, writer);
                }
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a DipValue to an XmlTextWriter
        /// </summary>
        /// <param name="obj">DipValue to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteDipValue(DipValue obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("dipvalue");

            writer.WriteRequiredAttributeString("name", obj.Name);
            writer.WriteRequiredAttributeString("value", obj.Value);
            writer.WriteOptionalAttributeString("default", obj.Default.FromYesNo());

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
            writer.WriteOptionalAttributeString("status", obj.Status?.AsStringValue());
            writer.WriteOptionalAttributeString("writable", obj.Writeable.FromYesNo());

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a DiskArea to an XmlTextWriter
        /// </summary>
        /// <param name="obj">DiskArea to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteDiskArea(DiskArea obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("diskarea");

            writer.WriteRequiredAttributeString("name", obj.Name);

            if (obj.Disk is not null && obj.Disk.Length > 0)
            {
                foreach (var disk in obj.Disk)
                {
                    WriteDisk(disk, writer);
                }
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a Feature to an XmlTextWriter
        /// </summary>
        /// <param name="obj">Feature to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteFeature(Feature obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("feature");

            writer.WriteRequiredAttributeString("name", obj.Name);
            writer.WriteOptionalAttributeString("value", obj.Value);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a Info to an XmlTextWriter
        /// </summary>
        /// <param name="obj">Info to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteInfo(Info obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("info");

            writer.WriteRequiredAttributeString("name", obj.Name);
            writer.WriteOptionalAttributeString("value", obj.Value);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a Part to an XmlTextWriter
        /// </summary>
        /// <param name="obj">Part to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WritePart(Part obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("part");

            writer.WriteRequiredAttributeString("name", obj.Name);
            writer.WriteRequiredAttributeString("interface", obj.Interface);

            if (obj.Feature is not null && obj.Feature.Length > 0)
            {
                foreach (var feature in obj.Feature)
                {
                    WriteFeature(feature, writer);
                }
            }

            if (obj.DataArea is not null && obj.DataArea.Length > 0)
            {
                foreach (var dataArea in obj.DataArea)
                {
                    WriteDataArea(dataArea, writer);
                }
            }

            if (obj.DiskArea is not null && obj.DiskArea.Length > 0)
            {
                foreach (var diskArea in obj.DiskArea)
                {
                    WriteDiskArea(diskArea, writer);
                }
            }

            if (obj.DipSwitch is not null && obj.DipSwitch.Length > 0)
            {
                foreach (var dipSwitch in obj.DipSwitch)
                {
                    WriteDipSwitch(dipSwitch, writer);
                }
            }

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
            writer.WriteOptionalAttributeString("size", obj.Size);
            writer.WriteOptionalAttributeString("length", obj.Length);
            writer.WriteOptionalAttributeString("crc", obj.CRC);
            writer.WriteOptionalAttributeString("sha1", obj.SHA1);
            writer.WriteOptionalAttributeString("offset", obj.Offset);
            writer.WriteOptionalAttributeString("value", obj.Value);
            writer.WriteOptionalAttributeString("status", obj.Status?.AsStringValue());
            writer.WriteOptionalAttributeString("loadflag", obj.LoadFlag?.AsStringValue());

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a SharedFeat to an XmlTextWriter
        /// </summary>
        /// <param name="obj">SharedFeat to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteSharedFeat(SharedFeat obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("sharedfeat");

            writer.WriteRequiredAttributeString("name", obj.Name);
            writer.WriteOptionalAttributeString("value", obj.Value);

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

            writer.WriteRequiredAttributeString("name", obj.Name);
            writer.WriteOptionalAttributeString("cloneof", obj.CloneOf);
            writer.WriteOptionalAttributeString("supported", obj.Supported?.AsStringValue());

            writer.WriteRequiredElementString("description", obj.Description);
            writer.WriteRequiredElementString("year", obj.Year);
            writer.WriteRequiredElementString("publisher", obj.Publisher);
            writer.WriteOptionalElementString("notes", obj.Notes);

            if (obj.Info is not null && obj.Info.Length > 0)
            {
                foreach (var info in obj.Info)
                {
                    WriteInfo(info, writer);
                }
            }

            if (obj.SharedFeat is not null && obj.SharedFeat.Length > 0)
            {
                foreach (var sharedFeat in obj.SharedFeat)
                {
                    WriteSharedFeat(sharedFeat, writer);
                }
            }

            if (obj.Part is not null && obj.Part.Length > 0)
            {
                foreach (var part in obj.Part)
                {
                    WritePart(part, writer);
                }
            }

            writer.WriteEndElement();
        }

        #endregion
    }
}

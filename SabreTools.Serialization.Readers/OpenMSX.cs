using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.OpenMSX;

namespace SabreTools.Serialization.Readers
{
    public class OpenMSX : BaseBinaryReader<SoftwareDb>
    {
        /// <inheritdoc/>
        public override SoftwareDb? Deserialize(Stream? data)
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
                SoftwareDb? softwareDb = null;
                while (reader.Read())
                {
                    // Comments have to be skipped
                    if (reader.NodeType == XmlNodeType.Comment)
                        continue;

                    // An ending element means exit
                    if (reader.NodeType == XmlNodeType.EndElement)
                        break;

                    // Only process starting elements
                    if (!reader.IsStartElement())
                        continue;

                    switch (reader.Name)
                    {
                        case "softwaredb":
                            if (softwareDb is not null && Debug)
                                Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                            softwareDb = ParseSoftwareDb(reader);
                            break;

                        default:
                            if (Debug) Console.Error.WriteLine($"Element '{reader.Name}' is not recognized");
                            break;
                    }
                }

                return softwareDb;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <summary>
        /// Parse from an XmlTextReader into a SoftwareDb
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled SoftwareDb on success, null on error</returns>
        public SoftwareDb ParseSoftwareDb(XmlTextReader reader)
        {
            var obj = new SoftwareDb();

            obj.Timestamp = reader.GetAttribute("timestamp");

            // Handle empty elements
            if (reader.IsEmptyElement)
                return obj;

            List<Software> softwares = [];

            reader.Read();
            while (!reader.EOF)
            {
                // Comments have to be skipped
                if (reader.NodeType == XmlNodeType.Comment)
                {
                    reader.Skip();
                    continue;
                }

                // An ending element means exit
                if (reader.NodeType == XmlNodeType.EndElement)
                    break;

                // Only process starting elements
                if (!reader.IsStartElement())
                {
                    reader.Skip();
                    continue;
                }

                switch (reader.Name)
                {
                    case "software":
                        var software = ParseSoftware(reader);
                        if (software is not null)
                            softwares.Add(software);

                        reader.Skip();
                        break;

                    default:
                        if (Debug) Console.Error.WriteLine($"Element '{reader.Name}' is not recognized");
                        reader.Skip();
                        break;
                }
            }

            if (softwares.Count > 0)
                obj.Software = [.. softwares];

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a Dump
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Dump on success, null on error</returns>
        public Dump ParseDump(XmlTextReader reader)
        {
            var obj = new Dump();

            // Handle empty elements
            if (reader.IsEmptyElement)
                return obj;

            reader.Read();
            while (!reader.EOF)
            {
                // Comments have to be skipped
                if (reader.NodeType == XmlNodeType.Comment)
                {
                    reader.Skip();
                    continue;
                }

                // An ending element means exit
                if (reader.NodeType == XmlNodeType.EndElement)
                    break;

                // Only process starting elements
                if (!reader.IsStartElement())
                {
                    reader.Skip();
                    continue;
                }

                switch (reader.Name)
                {
                    case "boot":
                        if (obj.Boot is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Boot = reader.ReadElementContentAsString();
                        break;
                    case "original":
                        if (obj.Original is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Original = ParseOriginal(reader);
                        break;
                    case "rom":
                    case "megarom":
                    case "sccpluscart":
                        if (obj.Rom is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Rom = ParseRomBase(reader);
                        reader.Skip();
                        break;

                    default:
                        if (Debug) Console.Error.WriteLine($"Element '{reader.Name}' is not recognized");
                        reader.Skip();
                        break;
                }
            }

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a Original
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Original on success, null on error</returns>
        public Original ParseOriginal(XmlTextReader reader)
        {
            var obj = new Original();

            obj.Value = reader.GetAttribute("value").AsYesNo();
            obj.Content = reader.ReadElementContentAsString();

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a RomBase
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled RomBase on success, null on error</returns>
        public RomBase? ParseRomBase(XmlTextReader reader)
        {
            RomBase obj;
            if (reader.Name == "rom")
                obj = new Rom();
            else if (reader.Name == "megarom")
                obj = new MegaRom();
            else if (reader.Name == "sccpluscart")
                obj = new SCCPlusCart();
            else
                return null;

            // Handle empty elements
            if (reader.IsEmptyElement)
                return obj;

            reader.Read();
            while (!reader.EOF)
            {
                // Comments have to be skipped
                if (reader.NodeType == XmlNodeType.Comment)
                {
                    reader.Skip();
                    continue;
                }

                // An ending element means exit
                if (reader.NodeType == XmlNodeType.EndElement)
                    break;

                // Only process starting elements
                if (!reader.IsStartElement())
                {
                    reader.Skip();
                    continue;
                }

                switch (reader.Name)
                {
                    case "start":
                        if (obj.Start is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Start = reader.ReadElementContentAsString();
                        break;
                    case "type":
                        if (obj.Type is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Type = reader.ReadElementContentAsString();
                        break;
                    case "hash":
                        if (obj.Hash is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Hash = reader.ReadElementContentAsString();
                        break;
                    case "remark":
                        if (obj.Remark is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Remark = reader.ReadElementContentAsString();
                        break;

                    default:
                        if (Debug) Console.Error.WriteLine($"Element '{reader.Name}' is not recognized");
                        reader.Skip();
                        break;
                }
            }

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a Software
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Software on success, null on error</returns>
        public Software ParseSoftware(XmlTextReader reader)
        {
            var obj = new Software();

            // Handle empty elements
            if (reader.IsEmptyElement)
                return obj;

            List<Dump> dumps = [];

            reader.Read();
            while (!reader.EOF)
            {
                // Comments have to be skipped
                if (reader.NodeType == XmlNodeType.Comment)
                {
                    reader.Skip();
                    continue;
                }

                // An ending element means exit
                if (reader.NodeType == XmlNodeType.EndElement)
                    break;

                // Only process starting elements
                if (!reader.IsStartElement())
                {
                    reader.Skip();
                    continue;
                }

                switch (reader.Name)
                {
                    case "title":
                        if (obj.Title is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Title = reader.ReadElementContentAsString();
                        break;
                    case "genmsxid":
                        if (obj.GenMSXID is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.GenMSXID = reader.ReadElementContentAsString();
                        break;
                    case "system":
                        if (obj.System is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.System = reader.ReadElementContentAsString();
                        break;
                    case "company":
                        if (obj.Company is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Company = reader.ReadElementContentAsString();
                        break;
                    case "year":
                        if (obj.Year is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Year = reader.ReadElementContentAsString();
                        break;
                    case "country":
                        if (obj.Country is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Country = reader.ReadElementContentAsString();
                        break;
                    case "dump":
                        var dump = ParseDump(reader);
                        if (dump is not null)
                            dumps.Add(dump);

                        reader.Skip();
                        break;

                    default:
                        if (Debug) Console.Error.WriteLine($"Element '{reader.Name}' is not recognized");
                        reader.Skip();
                        break;
                }
            }

            if (dumps.Count > 0)
                obj.Dump = [.. dumps];

            return obj;
        }
    }
}

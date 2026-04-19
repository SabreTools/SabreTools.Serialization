using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.SoftwareList;
using SabreTools.Text.Extensions;

namespace SabreTools.Serialization.Readers
{
    public class SoftwareList : BaseBinaryReader<Data.Models.SoftwareList.SoftwareList>
    {
        /// <inheritdoc/>
        public override Data.Models.SoftwareList.SoftwareList? Deserialize(Stream? data)
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
                Data.Models.SoftwareList.SoftwareList? softwareList = null;
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
                        case "softwarelist":
                            if (softwareList is not null && Debug)
                                Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                            softwareList = ParseSoftwareList(reader);
                            break;

                        default:
                            if (Debug) Console.Error.WriteLine($"Element '{reader.Name}' is not recognized");
                            break;
                    }
                }

                return softwareList;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <summary>
        /// Parse from an XmlTextReader into a SoftwareList
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled SoftwareList on success, null on error</returns>
        public Data.Models.SoftwareList.SoftwareList ParseSoftwareList(XmlTextReader reader)
        {
            var obj = new Data.Models.SoftwareList.SoftwareList();

            obj.Name = reader.GetAttribute("name");
            obj.Description = reader.GetAttribute("description");

            // Handle empty elements
            if (reader.IsEmptyElement)
                return obj;

            List<Software> softwares = [];

            reader.Read();
            while (!reader.EOF)
            {
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
                    case "notes":
                        if (obj.Notes is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Notes = reader.ReadElementContentAsString();
                        break;
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
        /// Parse from an XmlTextReader into a DataArea
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled DataArea on success, null on error</returns>
        public DataArea ParseDataArea(XmlTextReader reader)
        {
            var obj = new DataArea();

            obj.Name = reader.GetAttribute("name");
            obj.Size = NumberHelper.ConvertToInt64(reader.GetAttribute("size"));
            obj.Width = reader.GetAttribute("width").AsWidth();
            obj.Endianness = reader.GetAttribute("endianness").AsEndianness();

            // Handle empty elements
            if (reader.IsEmptyElement)
                return obj;

            List<Rom> roms = [];

            reader.Read();
            while (!reader.EOF)
            {
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
                    case "rom":
                        var rom = ParseRom(reader);
                        if (rom is not null)
                            roms.Add(rom);

                        reader.Skip();
                        break;

                    default:
                        if (Debug) Console.Error.WriteLine($"Element '{reader.Name}' is not recognized");
                        reader.Skip();
                        break;
                }
            }

            if (roms.Count > 0)
                obj.Rom = [.. roms];

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a DipSwitch
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled DipSwitch on success, null on error</returns>
        public DipSwitch ParseDipSwitch(XmlTextReader reader)
        {
            var obj = new DipSwitch();

            obj.Name = reader.GetAttribute("name");
            obj.Tag = reader.GetAttribute("tag");
            obj.Mask = reader.GetAttribute("mask");

            // Handle empty elements
            if (reader.IsEmptyElement)
                return obj;

            List<DipValue> dipValues = [];

            reader.Read();
            while (!reader.EOF)
            {
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
                    case "dipvalue":
                        var dipValue = ParseDipValue(reader);
                        if (dipValue is not null)
                            dipValues.Add(dipValue);

                        reader.Skip();
                        break;

                    default:
                        if (Debug) Console.Error.WriteLine($"Element '{reader.Name}' is not recognized");
                        reader.Skip();
                        break;
                }
            }

            if (dipValues.Count > 0)
                obj.DipValue = [.. dipValues];

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a DipValue
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled DipValue on success, null on error</returns>
        public DipValue ParseDipValue(XmlTextReader reader)
        {
            var obj = new DipValue();

            obj.Name = reader.GetAttribute("name");
            obj.Value = reader.GetAttribute("value");
            obj.Default = reader.GetAttribute("default").AsYesNo();

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a Disk
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Disk on success, null on error</returns>
        public Disk ParseDisk(XmlTextReader reader)
        {
            var obj = new Disk();

            obj.Name = reader.GetAttribute("name");
            obj.MD5 = reader.GetAttribute("md5");
            obj.SHA1 = reader.GetAttribute("sha1");
            obj.Status = reader.GetAttribute("status").AsItemStatus();
            obj.Writeable = reader.GetAttribute("writable").AsYesNo();

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a DiskArea
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled DiskArea on success, null on error</returns>
        public DiskArea ParseDiskArea(XmlTextReader reader)
        {
            var obj = new DiskArea();

            obj.Name = reader.GetAttribute("name");

            // Handle empty elements
            if (reader.IsEmptyElement)
                return obj;

            List<Disk> disks = [];

            reader.Read();
            while (!reader.EOF)
            {
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
                    case "disk":
                        var disk = ParseDisk(reader);
                        if (disk is not null)
                            disks.Add(disk);

                        reader.Skip();
                        break;

                    default:
                        if (Debug) Console.Error.WriteLine($"Element '{reader.Name}' is not recognized");
                        reader.Skip();
                        break;
                }
            }

            if (disks.Count > 0)
                obj.Disk = [.. disks];

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a Feature
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Feature on success, null on error</returns>
        public Feature ParseFeature(XmlTextReader reader)
        {
            var obj = new Feature();

            obj.Name = reader.GetAttribute("name");
            obj.Value = reader.GetAttribute("value");

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a Info
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Info on success, null on error</returns>
        public Info ParseInfo(XmlTextReader reader)
        {
            var obj = new Info();

            obj.Name = reader.GetAttribute("name");
            obj.Value = reader.GetAttribute("value");

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a Part
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Part on success, null on error</returns>
        public Part ParsePart(XmlTextReader reader)
        {
            var obj = new Part();

            obj.Name = reader.GetAttribute("name");
            obj.Interface = reader.GetAttribute("interface");

            // Handle empty elements
            if (reader.IsEmptyElement)
                return obj;

            List<Feature> features = [];
            List<DataArea> dataAreas = [];
            List<DiskArea> diskAreas = [];
            List<DipSwitch> dipSwitches = [];

            reader.Read();
            while (!reader.EOF)
            {
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
                    case "feature":
                        var feature = ParseFeature(reader);
                        if (feature is not null)
                            features.Add(feature);

                        reader.Skip();
                        break;
                    case "dataarea":
                        var dataArea = ParseDataArea(reader);
                        if (dataArea is not null)
                            dataAreas.Add(dataArea);

                        reader.Skip();
                        break;
                    case "diskarea":
                        var diskArea = ParseDiskArea(reader);
                        if (diskArea is not null)
                            diskAreas.Add(diskArea);

                        reader.Skip();
                        break;
                    case "dipswitch":
                        var dipSwitch = ParseDipSwitch(reader);
                        if (dipSwitch is not null)
                            dipSwitches.Add(dipSwitch);

                        reader.Skip();
                        break;

                    default:
                        if (Debug) Console.Error.WriteLine($"Element '{reader.Name}' is not recognized");
                        reader.Skip();
                        break;
                }
            }

            if (features.Count > 0)
                obj.Feature = [.. features];
            if (dataAreas.Count > 0)
                obj.DataArea = [.. dataAreas];
            if (diskAreas.Count > 0)
                obj.DiskArea = [.. diskAreas];
            if (dipSwitches.Count > 0)
                obj.DipSwitch = [.. dipSwitches];

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a Rom
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Rom on success, null on error</returns>
        public Rom ParseRom(XmlTextReader reader)
        {
            var obj = new Rom();

            obj.Name = reader.GetAttribute("name");
            obj.Size = NumberHelper.ConvertToInt64(reader.GetAttribute("size"));
            obj.Length = reader.GetAttribute("length");
            obj.CRC = reader.GetAttribute("crc");
            obj.SHA1 = reader.GetAttribute("sha1");
            obj.Offset = reader.GetAttribute("offset");
            obj.Value = reader.GetAttribute("value");
            obj.Status = reader.GetAttribute("status").AsItemStatus();
            obj.LoadFlag = reader.GetAttribute("loadflag").AsLoadFlag();

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a SharedFeat
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled SharedFeat on success, null on error</returns>
        public SharedFeat ParseSharedFeat(XmlTextReader reader)
        {
            var obj = new SharedFeat();

            obj.Name = reader.GetAttribute("name");
            obj.Value = reader.GetAttribute("value");

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

            obj.Name = reader.GetAttribute("name");
            obj.CloneOf = reader.GetAttribute("cloneof");
            obj.Supported = reader.GetAttribute("supported")?.AsSupported();

            // Handle empty elements
            if (reader.IsEmptyElement)
                return obj;

            List<Info> infos = [];
            List<SharedFeat> sharedFeats = [];
            List<Part> parts = [];

            reader.Read();
            while (!reader.EOF)
            {
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
                    case "description":
                        if (obj.Description is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Description = reader.ReadElementContentAsString();
                        break;
                    case "year":
                        if (obj.Year is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Year = reader.ReadElementContentAsString();
                        break;
                    case "publisher":
                        if (obj.Publisher is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Publisher = reader.ReadElementContentAsString();
                        break;
                    case "notes":
                        if (obj.Notes is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Notes = reader.ReadElementContentAsString();
                        break;
                    case "info":
                        var info = ParseInfo(reader);
                        if (info is not null)
                            infos.Add(info);

                        reader.Skip();
                        break;
                    case "sharedfeat":
                        var sharedFeat = ParseSharedFeat(reader);
                        if (sharedFeat is not null)
                            sharedFeats.Add(sharedFeat);

                        reader.Skip();
                        break;
                    case "part":
                        var part = ParsePart(reader);
                        if (part is not null)
                            parts.Add(part);

                        reader.Skip();
                        break;

                    default:
                        if (Debug) Console.Error.WriteLine($"Element '{reader.Name}' is not recognized");
                        reader.Skip();
                        break;
                }
            }

            if (infos.Count > 0)
                obj.Info = [.. infos];
            if (sharedFeats.Count > 0)
                obj.SharedFeat = [.. sharedFeats];
            if (parts.Count > 0)
                obj.Part = [.. parts];

            return obj;
        }
    }
}

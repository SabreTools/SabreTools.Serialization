using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.Logiqx;

namespace SabreTools.Serialization.Readers
{
    public class Logiqx : BaseBinaryReader<Datafile>
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

            obj.Build = reader.GetAttribute("build");
            obj.Debug = reader.GetAttribute("debug").AsYesNo();

            // TODO: Fix this based on No-Intro DATs
            // obj.SchemaLocation = reader.GetAttribute("schemaLocation");

            List<GameBase> games = [];
            List<Dir> dirs = [];
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
                    case "header":
                        if (obj.Header is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Header = ParseHeader(reader);
                        break;
                    case "game":
                    case "machine":
                        var game = ParseGameBase(reader);
                        if (game is not null)
                            games.Add(game);

                        break;
                    case "dir":
                        var dir = ParseDir(reader);
                        if (dir is not null)
                            dirs.Add(dir);

                        break;

                    default:
                        if (Debug) Console.Error.WriteLine($"Element '{reader.Name}' is not recognized");
                        break;
                }
            }

            if (games.Count > 0)
                obj.Game = [.. games];
            if (dirs.Count > 0)
                obj.Dir = [.. dirs];

            return obj;
        }

        #region Header

        /// <summary>
        /// Parse from an XmlTextReader into a ClrMamePro
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled ClrMamePro on success, null on error</returns>
        public Data.Models.Logiqx.ClrMamePro ParseClrMamePro(XmlTextReader reader)
        {
            var obj = new Data.Models.Logiqx.ClrMamePro();

            obj.Header = reader.GetAttribute("header");
            obj.ForceMerging = reader.GetAttribute("forcemerging");
            obj.ForceNodump = reader.GetAttribute("forcenodump");
            obj.ForcePacking = reader.GetAttribute("forcepacking");

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a Header
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Header on success, null on error</returns>
        public Header ParseHeader(XmlTextReader reader)
        {
            var obj = new Header();

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
                    case "id":
                        if (obj.Id is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Id = reader.ReadElementContentAsString();
                        break;
                    case "name":
                        if (obj.Name is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Name = reader.ReadElementContentAsString();
                        break;
                    case "description":
                        if (obj.Description is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Description = reader.ReadElementContentAsString();
                        break;
                    case "rootdir":
                        if (obj.RootDir is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.RootDir = reader.ReadElementContentAsString();
                        break;
                    case "category":
                        if (obj.Category is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Category = reader.ReadElementContentAsString();
                        break;
                    case "version":
                        if (obj.Version is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Version = reader.ReadElementContentAsString();
                        break;
                    case "date":
                        if (obj.Date is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Date = reader.ReadElementContentAsString();
                        break;
                    case "author":
                        if (obj.Author is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Author = reader.ReadElementContentAsString();
                        break;
                    case "email":
                        if (obj.Email is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Email = reader.ReadElementContentAsString();
                        break;
                    case "homepage":
                        if (obj.Homepage is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Homepage = reader.ReadElementContentAsString();
                        break;
                    case "url":
                        if (obj.Url is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Url = reader.ReadElementContentAsString();
                        break;
                    case "comment":
                        if (obj.Comment is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Comment = reader.ReadElementContentAsString();
                        break;
                    case "type":
                        if (obj.Type is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Type = reader.ReadElementContentAsString();
                        break;
                    case "clrmamepro":
                        if (obj.ClrMamePro is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.ClrMamePro = ParseClrMamePro(reader);
                        reader.Skip();
                        break;
                    case "romcenter":
                        if (obj.RomCenter is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.RomCenter = ParseRomCenter(reader);
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
        /// Parse from an XmlTextReader into a RomCenter
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled RomCenter on success, null on error</returns>
        public Data.Models.Logiqx.RomCenter ParseRomCenter(XmlTextReader reader)
        {
            var obj = new Data.Models.Logiqx.RomCenter();

            obj.Plugin = reader.GetAttribute("plugin");
            obj.RomMode = reader.GetAttribute("rommode");
            obj.BiosMode = reader.GetAttribute("biosmode");
            obj.SampleMode = reader.GetAttribute("samplemode");
            obj.LockRomMode = reader.GetAttribute("lockrommode").AsYesNo();
            obj.LockBiosMode = reader.GetAttribute("lockbiosmode").AsYesNo();
            obj.LockSampleMode = reader.GetAttribute("locksamplemode").AsYesNo();

            return obj;
        }

        #endregion

        #region Items

        /// <summary>
        /// Parse from an XmlTextReader into a Archive
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Archive on success, null on error</returns>
        public Archive ParseArchive(XmlTextReader reader)
        {
            var obj = new Archive();

            obj.Name = reader.GetAttribute("name");

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a BiosSet
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled BiosSet on success, null on error</returns>
        public BiosSet ParseBiosSet(XmlTextReader reader)
        {
            var obj = new BiosSet();

            obj.Name = reader.GetAttribute("name");
            obj.Description = reader.GetAttribute("description");
            obj.Default = reader.GetAttribute("default").AsYesNo();

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a DeviceRef
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled DeviceRef on success, null on error</returns>
        public DeviceRef ParseDeviceRef(XmlTextReader reader)
        {
            var obj = new DeviceRef();

            obj.Name = reader.GetAttribute("name");

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a Dir
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Dir on success, null on error</returns>
        public Dir ParseDir(XmlTextReader reader)
        {
            var obj = new Dir();

            obj.Name = reader.GetAttribute("name");

            List<Dir> subdirs = [];
            List<GameBase> games = [];

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
                    case "dir":
                        var dir = ParseDir(reader);
                        if (dir is not null)
                            subdirs.Add(dir);

                        break;
                    case "game":
                    case "machine":
                        var game = ParseGameBase(reader);
                        if (game is not null)
                            games.Add(game);

                        break;

                    default:
                        if (Debug) Console.Error.WriteLine($"Element '{reader.Name}' is not recognized");
                        break;
                }
            }

            if (subdirs.Count > 0)
                obj.Subdir = [.. subdirs];
            if (games.Count > 0)
                obj.Game = [.. games];

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
            obj.Merge = reader.GetAttribute("merge");
            obj.Status = reader.GetAttribute("status");
            obj.Region = reader.GetAttribute("region");

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a Driver
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Driver on success, null on error</returns>
        public Driver ParseDriver(XmlTextReader reader)
        {
            var obj = new Driver();

            obj.Status = reader.GetAttribute("status");
            obj.Emulation = reader.GetAttribute("emulation");
            obj.Cocktail = reader.GetAttribute("cocktail");
            obj.SaveState = reader.GetAttribute("savestate");
            obj.RequiresArtwork = reader.GetAttribute("requiresartwork").AsYesNo();
            obj.Unofficial = reader.GetAttribute("unofficial").AsYesNo();
            obj.NoSoundHardware = reader.GetAttribute("nosoundhardware").AsYesNo();
            obj.Incomplete = reader.GetAttribute("incomplete").AsYesNo();

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a GameBase
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled GameBase on success, null on error</returns>
        public GameBase? ParseGameBase(XmlTextReader reader)
        {
            GameBase obj;
            if (reader.Name == "game")
                obj = new Game();
            else if (reader.Name == "machine")
                obj = new Machine();
            else
                return null;

            obj.Name = reader.GetAttribute("name");
            obj.SourceFile = reader.GetAttribute("sourcefile");
            obj.IsBios = reader.GetAttribute("isbios").AsYesNo();
            obj.IsDevice = reader.GetAttribute("isdevice").AsYesNo();
            obj.IsMechanical = reader.GetAttribute("ismechanical").AsYesNo();
            obj.CloneOf = reader.GetAttribute("cloneof");
            obj.RomOf = reader.GetAttribute("romof");
            obj.SampleOf = reader.GetAttribute("sampleof");
            obj.Board = reader.GetAttribute("board");
            obj.RebuildTo = reader.GetAttribute("rebuildto");
            obj.Id = reader.GetAttribute("id");
            obj.CloneOfId = reader.GetAttribute("cloneofid");
            obj.Runnable = reader.GetAttribute("runnable").AsYesNo();

            List<string> comments = [];
            List<string> categories = [];
            List<Release> releases = [];
            List<BiosSet> biosSets = [];
            List<Rom> roms = [];
            List<Disk> disks = [];
            List<Media> medias = [];
            List<DeviceRef> deviceRefs = [];
            List<Sample> samples = [];
            List<Archive> archives = [];
            List<Data.Models.Logiqx.SoftwareList> softwareLists = [];

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
                    case "comment":
                        var comment = reader.ReadElementContentAsString();
                        if (comment is not null)
                            comments.Add(comment);

                        break;
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
                    case "manufacturer":
                        if (obj.Manufacturer is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Manufacturer = reader.ReadElementContentAsString();
                        break;
                    case "publisher":
                        if (obj.Publisher is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Publisher = reader.ReadElementContentAsString();
                        break;
                    case "category":
                        var category = reader.ReadElementContentAsString();
                        if (category is not null)
                            categories.Add(category);

                        break;
                    case "trurip":
                        if (obj.Trurip is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Trurip = ParseTrurip(reader);
                        reader.Skip();
                        break;
                    case "release":
                        var release = ParseRelease(reader);
                        if (release is not null)
                            releases.Add(release);

                        reader.Skip();
                        break;
                    case "biosset":
                        var biosSet = ParseBiosSet(reader);
                        if (biosSet is not null)
                            biosSets.Add(biosSet);

                        reader.Skip();
                        break;
                    case "rom":
                        var rom = ParseRom(reader);
                        if (rom is not null)
                            roms.Add(rom);

                        reader.Skip();
                        break;
                    case "disk":
                        var disk = ParseDisk(reader);
                        if (disk is not null)
                            disks.Add(disk);

                        reader.Skip();
                        break;
                    case "media":
                        var media = ParseMedia(reader);
                        if (media is not null)
                            medias.Add(media);

                        reader.Skip();
                        break;
                    case "device_ref":
                        var deviceRef = ParseDeviceRef(reader);
                        if (deviceRef is not null)
                            deviceRefs.Add(deviceRef);

                        reader.Skip();
                        break;
                    case "sample":
                        var sample = ParseSample(reader);
                        if (sample is not null)
                            samples.Add(sample);

                        reader.Skip();
                        break;
                    case "archive":
                        var archive = ParseArchive(reader);
                        if (archive is not null)
                            archives.Add(archive);

                        reader.Skip();
                        break;
                    case "driver":
                        if (obj.Driver is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Driver = ParseDriver(reader);
                        reader.Skip();
                        break;
                    case "softwarelist":
                        var softwareList = ParseSoftwareList(reader);
                        if (softwareList is not null)
                            softwareLists.Add(softwareList);

                        reader.Skip();
                        break;
                    case "url":
                        if (obj.Url is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Url = reader.ReadElementContentAsString();
                        break;
                    case "hash":
                        if (obj.Hash is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Hash = reader.ReadElementContentAsString();
                        break;

                    default:
                        if (Debug) Console.Error.WriteLine($"Element '{reader.Name}' is not recognized");
                        reader.Skip();
                        break;
                }
            }

            if (comments.Count > 0)
                obj.Comment = [.. comments];
            if (categories.Count > 0)
                obj.Category = [.. categories];
            if (releases.Count > 0)
                obj.Release = [.. releases];
            if (biosSets.Count > 0)
                obj.BiosSet = [.. biosSets];
            if (roms.Count > 0)
                obj.Rom = [.. roms];
            if (disks.Count > 0)
                obj.Disk = [.. disks];
            if (medias.Count > 0)
                obj.Media = [.. medias];
            if (deviceRefs.Count > 0)
                obj.DeviceRef = [.. deviceRefs];
            if (samples.Count > 0)
                obj.Sample = [.. samples];
            if (archives.Count > 0)
                obj.Archive = [.. archives];
            if (softwareLists.Count > 0)
                obj.SoftwareList = [.. softwareLists];

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

            obj.Name = reader.GetAttribute("name");
            obj.MD5 = reader.GetAttribute("md5");
            obj.SHA1 = reader.GetAttribute("sha1");
            obj.SHA256 = reader.GetAttribute("sha256");
            obj.SpamSum = reader.GetAttribute("spamsum");

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

            obj.Name = reader.GetAttribute("name");
            obj.Region = reader.GetAttribute("region");
            obj.Language = reader.GetAttribute("language");
            obj.Date = reader.GetAttribute("date");
            obj.Default = reader.GetAttribute("default").AsYesNo();

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
            obj.Size = reader.GetAttribute("size");
            obj.CRC16 = reader.GetAttribute("crc16");
            obj.CRC = reader.GetAttribute("crc");
            obj.CRC64 = reader.GetAttribute("crc64");
            obj.MD2 = reader.GetAttribute("md2");
            obj.MD4 = reader.GetAttribute("md4");
            obj.MD5 = reader.GetAttribute("md5");
            obj.RIPEMD128 = reader.GetAttribute("ripemd128");
            obj.RIPEMD160 = reader.GetAttribute("ripemd160");
            obj.SHA1 = reader.GetAttribute("sha1");
            obj.SHA256 = reader.GetAttribute("sha256");
            obj.SHA384 = reader.GetAttribute("sha384");
            obj.SHA512 = reader.GetAttribute("sha512");
            obj.SpamSum = reader.GetAttribute("spamsum");
            obj.xxHash364 = reader.GetAttribute("xxh3_64");
            obj.xxHash3128 = reader.GetAttribute("xxh3_128");
            obj.Merge = reader.GetAttribute("merge");
            obj.Status = reader.GetAttribute("status");
            obj.Serial = reader.GetAttribute("serial");
            obj.Header = reader.GetAttribute("header");
            obj.Date = reader.GetAttribute("date");
            obj.Inverted = reader.GetAttribute("inverted").AsYesNo();
            obj.MIA = reader.GetAttribute("mia").AsYesNo();

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a Sample
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Sample on success, null on error</returns>
        public Sample ParseSample(XmlTextReader reader)
        {
            var obj = new Sample();

            obj.Name = reader.GetAttribute("name");

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a SoftwareList
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled SoftwareList on success, null on error</returns>
        public Data.Models.Logiqx.SoftwareList ParseSoftwareList(XmlTextReader reader)
        {
            var obj = new Data.Models.Logiqx.SoftwareList();

            obj.Tag = reader.GetAttribute("tag");
            obj.Name = reader.GetAttribute("name");
            obj.Status = reader.GetAttribute("status");
            obj.Filter = reader.GetAttribute("filter");

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a Trurip
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Trurip on success, null on error</returns>
        public Trurip ParseTrurip(XmlTextReader reader)
        {
            var obj = new Trurip();

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
                    case "titleid":
                        if (obj.TitleID is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.TitleID = reader.ReadElementContentAsString();
                        break;
                    case "publisher":
                        if (obj.Publisher is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Publisher = reader.ReadElementContentAsString();
                        break;
                    case "developer":
                        if (obj.Developer is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Developer = reader.ReadElementContentAsString();
                        break;
                    case "year":
                        if (obj.Year is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Year = reader.ReadElementContentAsString();
                        break;
                    case "genre":
                        if (obj.Genre is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Genre = reader.ReadElementContentAsString();
                        break;
                    case "subgenre":
                        if (obj.Subgenre is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Subgenre = reader.ReadElementContentAsString();
                        break;
                    case "ratings":
                        if (obj.Ratings is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Ratings = reader.ReadElementContentAsString();
                        break;
                    case "score":
                        if (obj.Score is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Score = reader.ReadElementContentAsString();
                        break;
                    case "players":
                        if (obj.Players is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Players = reader.ReadElementContentAsString();
                        break;
                    case "enabled":
                        if (obj.Enabled is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Enabled = reader.ReadElementContentAsString();
                        break;
                    case "crc":
                        if (obj.CRC is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.CRC = reader.ReadElementContentAsString();
                        break;
                    case "source":
                        if (obj.Source is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Source = reader.ReadElementContentAsString();
                        break;
                    case "cloneof":
                        if (obj.CloneOf is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.CloneOf = reader.ReadElementContentAsString();
                        break;
                    case "relatedto":
                        if (obj.RelatedTo is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.RelatedTo = reader.ReadElementContentAsString();
                        break;

                    default:
                        if (Debug) Console.Error.WriteLine($"Element '{reader.Name}' is not recognized");
                        reader.Skip();
                        break;
                }
            }

            return obj;
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using SabreTools.Data.Models.OfflineList;

namespace SabreTools.Serialization.Readers
{
    public class OfflineList : BaseBinaryReader<Dat>
    {
        /// <inheritdoc/>
        public override Dat? Deserialize(Stream? data)
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
                Dat? dat = null;
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
                        case "dat":
                            if (dat is not null && Debug)
                                Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                            dat = ParseDat(reader);
                            break;

                        default:
                            if (Debug) Console.Error.WriteLine($"Element '{reader.Name}' is not recognized");
                            break;
                    }
                }

                return dat;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <summary>
        /// Parse from an XmlTextReader into a Dat
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Dat on success, null on error</returns>
        public Dat ParseDat(XmlTextReader reader)
        {
            var obj = new Dat();

            // TODO: Fix this schema reading/writing
            obj.NoNamespaceSchemaLocation = reader.GetAttribute("noNamespaceSchemaLocation");

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
                    case "configuration":
                        if (obj.Configuration is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Configuration = ParseConfiguration(reader);
                        break;
                    case "games":
                        if (obj.Games is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Games = ParseGames(reader);
                        break;
                    case "gui":
                        if (obj.GUI is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.GUI = ParseGUI(reader);
                        break;

                    default:
                        if (Debug) Console.Error.WriteLine($"Element '{reader.Name}' is not recognized");
                        break;
                }
            }

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a CanOpen
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled CanOpen on success, null on error</returns>
        public CanOpen ParseCanOpen(XmlTextReader reader)
        {
            var obj = new CanOpen();

            List<string> extensions = [];

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
                    case "extension":
                        var extension = reader.ReadElementContentAsString();
                        if (extension is not null)
                            extensions.Add(extension);

                        break;

                    default:
                        if (Debug) Console.Error.WriteLine($"Element '{reader.Name}' is not recognized");
                        reader.Skip();
                        break;
                }
            }

            if (extensions.Count > 0)
                obj.Extension = [.. extensions];

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a Comment
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Comment on success, null on error</returns>
        public Comment ParseComment(XmlTextReader reader)
        {
            var obj = new Comment();

            obj.Visible = reader.GetAttribute("visible");
            obj.InNamingOption = reader.GetAttribute("inNamingOption");
            obj.Default = reader.GetAttribute("default");

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a Configuration
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Configuration on success, null on error</returns>
        public Configuration ParseConfiguration(XmlTextReader reader)
        {
            var obj = new Configuration();

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
                    case "datName":
                        if (obj.DatName is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.DatName = reader.ReadElementContentAsString();
                        break;
                    case "imFolder":
                        if (obj.ImFolder is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.ImFolder = reader.ReadElementContentAsString();
                        break;
                    case "datVersion":
                        if (obj.DatVersion is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.DatVersion = reader.ReadElementContentAsString();
                        break;
                    case "system":
                        if (obj.System is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.System = reader.ReadElementContentAsString();
                        break;
                    case "screenshotsWidth":
                        if (obj.ScreenshotsWidth is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.ScreenshotsWidth = reader.ReadElementContentAsString();
                        break;
                    case "screenshotsHeight":
                        if (obj.ScreenshotsHeight is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.ScreenshotsHeight = reader.ReadElementContentAsString();
                        break;
                    case "infos":
                        if (obj.Infos is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Infos = ParseInfos(reader);
                        reader.Skip();
                        break;
                    case "canOpen":
                        if (obj.CanOpen is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.CanOpen = ParseCanOpen(reader);
                        reader.Skip();
                        break;
                    case "newDat":
                        if (obj.NewDat is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.NewDat = ParseNewDat(reader);
                        reader.Skip();
                        break;
                    case "search":
                        if (obj.Search is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Search = ParseSearch(reader);
                        reader.Skip();
                        break;
                    case "romTitle":
                        if (obj.RomTitle is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.RomTitle = reader.ReadElementContentAsString();
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
        /// Parse from an XmlTextReader into a DatUrl
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled DatUrl on success, null on error</returns>
        public DatUrl ParseDatUrl(XmlTextReader reader)
        {
            var obj = new DatUrl();

            obj.FileName = reader.GetAttribute("fileName");
            obj.Content = reader.ReadElementContentAsString();

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a FileRomCRC
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled FileRomCRC on success, null on error</returns>
        public FileRomCRC ParseFileRomCRC(XmlTextReader reader)
        {
            var obj = new FileRomCRC();

            obj.Extension = reader.GetAttribute("extension");
            obj.Content = reader.ReadElementContentAsString();

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a Files
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Files on success, null on error</returns>
        public Files ParseFiles(XmlTextReader reader)
        {
            var obj = new Files();

            List<FileRomCRC> romCrcs = [];

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
                    case "romCRC":
                        var romCrc = ParseFileRomCRC(reader);
                        if (romCrc is not null)
                            romCrcs.Add(romCrc);

                        break;

                    default:
                        if (Debug) Console.Error.WriteLine($"Element '{reader.Name}' is not recognized");
                        reader.Skip();
                        break;
                }
            }

            if (romCrcs.Count > 0)
                obj.RomCRC = [.. romCrcs];

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a Find
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Find on success, null on error</returns>
        public Find ParseFind(XmlTextReader reader)
        {
            var obj = new Find();

            obj.Operation = reader.GetAttribute("operation");
            obj.Value = reader.GetAttribute("value");
            obj.Content = reader.ReadElementContentAsString();

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
                    case "imageNumber":
                        if (obj.ImageNumber is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.ImageNumber = reader.ReadElementContentAsString();
                        break;
                    case "releaseNumber":
                        if (obj.ReleaseNumber is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.ReleaseNumber = reader.ReadElementContentAsString();
                        break;
                    case "title":
                        if (obj.Title is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Title = reader.ReadElementContentAsString();
                        break;
                    case "saveType":
                        if (obj.SaveType is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.SaveType = reader.ReadElementContentAsString();
                        break;
                    case "romSize":
                        if (obj.RomSize is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.RomSize = reader.ReadElementContentAsString();
                        break;
                    case "publisher":
                        if (obj.Publisher is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Publisher = reader.ReadElementContentAsString();
                        break;
                    case "location":
                        if (obj.Location is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Location = reader.ReadElementContentAsString();
                        break;
                    case "sourceRom":
                        if (obj.SourceRom is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.SourceRom = reader.ReadElementContentAsString();
                        break;
                    case "language":
                        if (obj.Language is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Language = reader.ReadElementContentAsString();
                        break;
                    case "files":
                        if (obj.Files is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Files = ParseFiles(reader);
                        reader.Skip();
                        break;
                    case "im1CRC":
                        if (obj.Im1CRC is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Im1CRC = reader.ReadElementContentAsString();
                        break;
                    case "im2CRC":
                        if (obj.Im2CRC is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Im2CRC = reader.ReadElementContentAsString();
                        break;
                    case "comment":
                        if (obj.Comment is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Comment = reader.ReadElementContentAsString();
                        break;
                    case "duplicateID":
                        if (obj.DuplicateID is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.DuplicateID = reader.ReadElementContentAsString();
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
        /// Parse from an XmlTextReader into a Games
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Games on success, null on error</returns>
        public Games ParseGames(XmlTextReader reader)
        {
            var obj = new Games();

            List<Game> games = [];

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
                    case "game":
                        var game = ParseGame(reader);
                        if (game is not null)
                            games.Add(game);

                        reader.Skip();
                        break;

                    default:
                        if (Debug) Console.Error.WriteLine($"Element '{reader.Name}' is not recognized");
                        reader.Skip();
                        break;
                }
            }

            if (games.Count > 0)
                obj.Game = [.. games];

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a GUI
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled GUI on success, null on error</returns>
        public GUI ParseGUI(XmlTextReader reader)
        {
            var obj = new GUI();

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
                    case "images":
                        if (obj.Images is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Images = ParseImages(reader);
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
        /// Parse from an XmlTextReader into a Im1CRC
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Im1CRC on success, null on error</returns>
        public Im1CRC ParseIm1CRC(XmlTextReader reader)
        {
            var obj = new Im1CRC();

            obj.Visible = reader.GetAttribute("visible");
            obj.InNamingOption = reader.GetAttribute("inNamingOption");
            obj.Default = reader.GetAttribute("default");

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a Im2CRC
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Im2CRC on success, null on error</returns>
        public Im2CRC ParseIm2CRC(XmlTextReader reader)
        {
            var obj = new Im2CRC();

            obj.Visible = reader.GetAttribute("visible");
            obj.InNamingOption = reader.GetAttribute("inNamingOption");
            obj.Default = reader.GetAttribute("default");

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a Image
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Image on success, null on error</returns>
        public Image ParseImage(XmlTextReader reader)
        {
            var obj = new Image();

            obj.X = reader.GetAttribute("x");
            obj.Y = reader.GetAttribute("y");
            obj.Width = reader.GetAttribute("width");
            obj.Height = reader.GetAttribute("height");

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a ImageNumber
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled ImageNumber on success, null on error</returns>
        public ImageNumber ParseImageNumber(XmlTextReader reader)
        {
            var obj = new ImageNumber();

            obj.Visible = reader.GetAttribute("visible");
            obj.InNamingOption = reader.GetAttribute("inNamingOption");
            obj.Default = reader.GetAttribute("default");

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a Images
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Images on success, null on error</returns>
        public Images ParseImages(XmlTextReader reader)
        {
            var obj = new Images();

            obj.Width = reader.GetAttribute("width");
            obj.Height = reader.GetAttribute("height");

            List<Image> images = [];

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
                    case "image":
                        var image = ParseImage(reader);
                        if (image is not null)
                            images.Add(image);

                        reader.Skip();
                        break;

                    default:
                        if (Debug) Console.Error.WriteLine($"Element '{reader.Name}' is not recognized");
                        reader.Skip();
                        break;
                }
            }

            if (images.Count > 0)
                obj.Image = [.. images];

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a Infos
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Infos on success, null on error</returns>
        public Infos ParseInfos(XmlTextReader reader)
        {
            var obj = new Infos();

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
                    case "title":
                        if (obj.Title is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Title = ParseTitle(reader);
                        reader.Skip();
                        break;
                    case "location":
                        if (obj.Location is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Location = ParseLocation(reader);
                        reader.Skip();
                        break;
                    case "publisher":
                        if (obj.Publisher is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Publisher = ParsePublisher(reader);
                        reader.Skip();
                        break;
                    case "sourceRom":
                        if (obj.SourceRom is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.SourceRom = ParseSourceRom(reader);
                        reader.Skip();
                        break;
                    case "saveType":
                        if (obj.SaveType is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.SaveType = ParseSaveType(reader);
                        reader.Skip();
                        break;
                    case "romSize":
                        if (obj.RomSize is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.RomSize = ParseRomSize(reader);
                        reader.Skip();
                        break;
                    case "releaseNumber":
                        if (obj.ReleaseNumber is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.ReleaseNumber = ParseReleaseNumber(reader);
                        reader.Skip();
                        break;
                    case "imageNumber":
                        if (obj.ImageNumber is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.ImageNumber = ParseImageNumber(reader);
                        reader.Skip();
                        break;
                    case "languageNumber":
                        if (obj.LanguageNumber is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.LanguageNumber = ParseLanguageNumber(reader);
                        reader.Skip();
                        break;
                    case "comment":
                        if (obj.Comment is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Comment = ParseComment(reader);
                        reader.Skip();
                        break;
                    case "romCRC":
                        if (obj.RomCRC is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.RomCRC = ParseRomCRC(reader);
                        reader.Skip();
                        break;
                    case "im1CRC":
                        if (obj.Im1CRC is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Im1CRC = ParseIm1CRC(reader);
                        reader.Skip();
                        break;
                    case "im2CRC":
                        if (obj.Im2CRC is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Im2CRC = ParseIm2CRC(reader);
                        reader.Skip();
                        break;
                    case "languages":
                        if (obj.Languages is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.Languages = ParseLanguages(reader);
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
        /// Parse from an XmlTextReader into a LanguageNumber
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled LanguageNumber on success, null on error</returns>
        public LanguageNumber ParseLanguageNumber(XmlTextReader reader)
        {
            var obj = new LanguageNumber();

            obj.Visible = reader.GetAttribute("visible");
            obj.InNamingOption = reader.GetAttribute("inNamingOption");
            obj.Default = reader.GetAttribute("default");

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a Languages
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Languages on success, null on error</returns>
        public Languages ParseLanguages(XmlTextReader reader)
        {
            var obj = new Languages();

            obj.Visible = reader.GetAttribute("visible");
            obj.InNamingOption = reader.GetAttribute("inNamingOption");
            obj.Default = reader.GetAttribute("default");

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a Location
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Location on success, null on error</returns>
        public Location ParseLocation(XmlTextReader reader)
        {
            var obj = new Location();

            obj.Visible = reader.GetAttribute("visible");
            obj.InNamingOption = reader.GetAttribute("inNamingOption");
            obj.Default = reader.GetAttribute("default");

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a Publisher
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Publisher on success, null on error</returns>
        public Publisher ParsePublisher(XmlTextReader reader)
        {
            var obj = new Publisher();

            obj.Visible = reader.GetAttribute("visible");
            obj.InNamingOption = reader.GetAttribute("inNamingOption");
            obj.Default = reader.GetAttribute("default");

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a ReleaseNumber
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled ReleaseNumber on success, null on error</returns>
        public ReleaseNumber ParseReleaseNumber(XmlTextReader reader)
        {
            var obj = new ReleaseNumber();

            obj.Visible = reader.GetAttribute("visible");
            obj.InNamingOption = reader.GetAttribute("inNamingOption");
            obj.Default = reader.GetAttribute("default");

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a RomCRC
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled RomCRC on success, null on error</returns>
        public RomCRC ParseRomCRC(XmlTextReader reader)
        {
            var obj = new RomCRC();

            obj.Visible = reader.GetAttribute("visible");
            obj.InNamingOption = reader.GetAttribute("inNamingOption");
            obj.Default = reader.GetAttribute("default");

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a RomSize
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled RomSize on success, null on error</returns>
        public RomSize ParseRomSize(XmlTextReader reader)
        {
            var obj = new RomSize();

            obj.Visible = reader.GetAttribute("visible");
            obj.InNamingOption = reader.GetAttribute("inNamingOption");
            obj.Default = reader.GetAttribute("default");

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a SaveType
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled SaveType on success, null on error</returns>
        public SaveType ParseSaveType(XmlTextReader reader)
        {
            var obj = new SaveType();

            obj.Visible = reader.GetAttribute("visible");
            obj.InNamingOption = reader.GetAttribute("inNamingOption");
            obj.Default = reader.GetAttribute("default");

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a SourceRom
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled SourceRom on success, null on error</returns>
        public SourceRom ParseSourceRom(XmlTextReader reader)
        {
            var obj = new SourceRom();

            obj.Visible = reader.GetAttribute("visible");
            obj.InNamingOption = reader.GetAttribute("inNamingOption");
            obj.Default = reader.GetAttribute("default");

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a Title
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Title on success, null on error</returns>
        public Title ParseTitle(XmlTextReader reader)
        {
            var obj = new Title();

            obj.Visible = reader.GetAttribute("visible");
            obj.InNamingOption = reader.GetAttribute("inNamingOption");
            obj.Default = reader.GetAttribute("default");

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a NewDat
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled NewDat on success, null on error</returns>
        public NewDat ParseNewDat(XmlTextReader reader)
        {
            var obj = new NewDat();

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
                    case "datVersionURL":
                        if (obj.DatVersionUrl is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.DatVersionUrl = reader.ReadElementContentAsString();
                        break;
                    case "datURL":
                        if (obj.DatUrl is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.DatUrl = ParseDatUrl(reader);
                        break;
                    case "imURL":
                        if (obj.ImUrl is not null && Debug)
                            Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                        obj.ImUrl = reader.ReadElementContentAsString();
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
        /// Parse from an XmlTextReader into a Search
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Search on success, null on error</returns>
        public Search ParseSearch(XmlTextReader reader)
        {
            var obj = new Search();

            List<To> tos = [];

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
                    case "to":
                        var to = ParseTo(reader);
                        if (to is not null)
                            tos.Add(to);

                        reader.Skip();
                        break;

                    default:
                        if (Debug) Console.Error.WriteLine($"Element '{reader.Name}' is not recognized");
                        reader.Skip();
                        break;
                }
            }

            if (tos.Count > 0)
                obj.To = [.. tos];

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a To
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled To on success, null on error</returns>
        public To ParseTo(XmlTextReader reader)
        {
            var obj = new To();

            obj.Value = reader.GetAttribute("value");
            obj.Default = reader.GetAttribute("default");
            obj.Auto = reader.GetAttribute("auto");

            List<Find> finds = [];

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
                    case "find":
                        var find = ParseFind(reader);
                        if (find is not null)
                            finds.Add(find);

                        break;

                    default:
                        if (Debug) Console.Error.WriteLine($"Element '{reader.Name}' is not recognized");
                        reader.Skip();
                        break;
                }
            }

            if (finds.Count > 0)
                obj.Find = [.. finds];

            return obj;
        }
    }
}

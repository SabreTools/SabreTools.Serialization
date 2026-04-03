using System.IO;
using System.Text;
using System.Xml;
using SabreTools.Data.Models.OfflineList;
using SabreTools.IO.Extensions;
using SabreTools.Text.Extensions;

namespace SabreTools.Serialization.Writers
{
    public class OfflineList : BaseBinaryWriter<Dat>
    {
        /// <inheritdoc/>
        public override Stream? SerializeStream(Dat? obj)
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

            // Write the Dat, if it exists
            WriteDat(obj, writer);
            writer.Flush();

            // Return the stream
            stream.SeekIfPossible(0, SeekOrigin.Begin);
            return stream;
        }

        /// <summary>
        /// Write a Dat to an XmlTextWriter
        /// </summary>
        /// <param name="obj">Dat to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteDat(Dat obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("dat");

            // TODO: Fix this schema reading/writing
            // writer.WriteOptionalAttributeString("noNamespaceSchemaLocation", obj.NoNamespaceSchemaLocation);

            if (obj.Configuration is not null)
                WriteConfiguration(obj.Configuration, writer);

            if (obj.Games is not null)
                WriteGames(obj.Games, writer);

            if (obj.GUI is not null)
                WriteGUI(obj.GUI, writer);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a CanOpen to an XmlTextWriter
        /// </summary>
        /// <param name="obj">CanOpen to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteCanOpen(CanOpen obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("canOpen");

            if (obj.Extension is not null && obj.Extension.Length > 0)
            {
                foreach (var extension in obj.Extension)
                {
                    writer.WriteOptionalElementString("extension", extension);
                }
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a Configuration to an XmlTextWriter
        /// </summary>
        /// <param name="obj">Configuration to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteConfiguration(Configuration obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("configuration");

            writer.WriteOptionalElementString("datName", obj.DatName);
            writer.WriteOptionalElementString("imFolder", obj.ImFolder);
            writer.WriteOptionalElementString("datVersion", obj.DatVersion);
            writer.WriteOptionalElementString("system", obj.System);
            writer.WriteOptionalElementString("screenshotsWidth", obj.ScreenshotsWidth);
            writer.WriteOptionalElementString("screenshotsHeight", obj.ScreenshotsHeight);

            if (obj.Infos is not null)
                WriteInfos(obj.Infos, writer);

            if (obj.CanOpen is not null)
                WriteCanOpen(obj.CanOpen, writer);

            if (obj.NewDat is not null)
                WriteNewDat(obj.NewDat, writer);

            if (obj.Search is not null)
                WriteSearch(obj.Search, writer);

            writer.WriteOptionalElementString("romTitle", obj.RomTitle);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a DatUrl to an XmlTextWriter
        /// </summary>
        /// <param name="obj">DatUrl to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteDatUrl(DatUrl obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("datURL");

            writer.WriteOptionalAttributeString("fileName", obj.FileName);

            if (obj.Content is not null)
                writer.WriteRaw(obj.Content);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a FileRomCRC to an XmlTextWriter
        /// </summary>
        /// <param name="obj">FileRomCRC to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteFileRomCRC(FileRomCRC obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("romCRC");

            writer.WriteOptionalAttributeString("extension", obj.Extension);

            if (obj.Content is not null)
                writer.WriteRaw(obj.Content);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a Files to an XmlTextWriter
        /// </summary>
        /// <param name="obj">Files to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteFiles(Files obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("files");

            if (obj.RomCRC is not null && obj.RomCRC.Length > 0)
            {
                foreach (var extension in obj.RomCRC)
                {
                    WriteFileRomCRC(extension, writer);
                }
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a Find to an XmlTextWriter
        /// </summary>
        /// <param name="obj">Find to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteFind(Find obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("find");

            writer.WriteOptionalAttributeString("operation", obj.Operation);
            writer.WriteOptionalAttributeString("value", obj.Value);

            if (obj.Content is not null)
                writer.WriteRaw(obj.Content);

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

            writer.WriteOptionalElementString("imageNumber", obj.ImageNumber);
            writer.WriteOptionalElementString("releaseNumber", obj.ReleaseNumber);
            writer.WriteOptionalElementString("title", obj.Title);
            writer.WriteOptionalElementString("saveType", obj.SaveType);
            writer.WriteOptionalElementString("romSize", obj.RomSize?.ToString());
            writer.WriteOptionalElementString("publisher", obj.Publisher);
            writer.WriteOptionalElementString("location", obj.Location);
            writer.WriteOptionalElementString("sourceRom", obj.SourceRom);
            writer.WriteOptionalElementString("language", obj.Language);

            if (obj.Files is not null)
                WriteFiles(obj.Files, writer);

            writer.WriteOptionalElementString("im1CRC", obj.Im1CRC);
            writer.WriteOptionalElementString("im2CRC", obj.Im2CRC);
            writer.WriteOptionalElementString("comment", obj.Comment);
            writer.WriteOptionalElementString("duplicateID", obj.DuplicateID);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a Games to an XmlTextWriter
        /// </summary>
        /// <param name="obj">Games to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteGames(Games obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("games");

            if (obj.Game is not null && obj.Game.Length > 0)
            {
                foreach (var game in obj.Game)
                {
                    WriteGame(game, writer);
                }
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a GUI to an XmlTextWriter
        /// </summary>
        /// <param name="obj">GUI to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteGUI(GUI obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("gui");

            if (obj.Images is not null)
                WriteImages(obj.Images, writer);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a Image to an XmlTextWriter
        /// </summary>
        /// <param name="obj">Image to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteImage(Image obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("image");

            writer.WriteOptionalAttributeString("x", obj.X);
            writer.WriteOptionalAttributeString("y", obj.Y);
            writer.WriteOptionalAttributeString("width", obj.Width);
            writer.WriteOptionalAttributeString("height", obj.Height);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a Images to an XmlTextWriter
        /// </summary>
        /// <param name="obj">Images to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteImages(Images obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("images");

            writer.WriteOptionalAttributeString("width", obj.Width);
            writer.WriteOptionalAttributeString("height", obj.Height);

            if (obj.Image is not null && obj.Image.Length > 0)
            {
                foreach (var image in obj.Image)
                {
                    WriteImage(image, writer);
                }
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a InfoBase to an XmlTextWriter
        /// </summary>
        /// <param name="obj">InfoBase to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteInfoBase(InfoBase obj, XmlTextWriter writer)
        {
            if (obj is Title)
                writer.WriteStartElement("title");
            else if (obj is Location)
                writer.WriteStartElement("location");
            else if (obj is Publisher)
                writer.WriteStartElement("publisher");
            else if (obj is SourceRom)
                writer.WriteStartElement("sourceRom");
            else if (obj is SaveType)
                writer.WriteStartElement("saveType");
            else if (obj is RomSize)
                writer.WriteStartElement("romSize");
            else if (obj is ReleaseNumber)
                writer.WriteStartElement("releaseNumber");
            else if (obj is ImageNumber)
                writer.WriteStartElement("imageNumber");
            else if (obj is LanguageNumber)
                writer.WriteStartElement("languageNumber");
            else if (obj is Comment)
                writer.WriteStartElement("comment");
            else if (obj is RomCRC)
                writer.WriteStartElement("romCRC");
            else if (obj is Im1CRC)
                writer.WriteStartElement("im1CRC");
            else if (obj is Im2CRC)
                writer.WriteStartElement("im2CRC");
            else if (obj is Languages)
                writer.WriteStartElement("languages");
            else
                return;

            writer.WriteOptionalAttributeString("visible", obj.Visible);
            writer.WriteOptionalAttributeString("inNamingOption", obj.InNamingOption);
            writer.WriteOptionalAttributeString("default", obj.Default.ToString()?.ToLowerInvariant());

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a Infos to an XmlTextWriter
        /// </summary>
        /// <param name="obj">Infos to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteInfos(Infos obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("infos");

            if (obj.Title is not null)
                WriteInfoBase(obj.Title, writer);

            if (obj.Location is not null)
                WriteInfoBase(obj.Location, writer);

            if (obj.Publisher is not null)
                WriteInfoBase(obj.Publisher, writer);

            if (obj.SourceRom is not null)
                WriteInfoBase(obj.SourceRom, writer);

            if (obj.SaveType is not null)
                WriteInfoBase(obj.SaveType, writer);

            if (obj.RomSize is not null)
                WriteInfoBase(obj.RomSize, writer);

            if (obj.ReleaseNumber is not null)
                WriteInfoBase(obj.ReleaseNumber, writer);

            if (obj.ImageNumber is not null)
                WriteInfoBase(obj.ImageNumber, writer);

            if (obj.LanguageNumber is not null)
                WriteInfoBase(obj.LanguageNumber, writer);

            if (obj.Comment is not null)
                WriteInfoBase(obj.Comment, writer);

            if (obj.RomCRC is not null)
                WriteInfoBase(obj.RomCRC, writer);

            if (obj.Im1CRC is not null)
                WriteInfoBase(obj.Im1CRC, writer);

            if (obj.Im2CRC is not null)
                WriteInfoBase(obj.Im2CRC, writer);

            if (obj.Languages is not null)
                WriteInfoBase(obj.Languages, writer);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a NewDat to an XmlTextWriter
        /// </summary>
        /// <param name="obj">NewDat to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteNewDat(NewDat obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("newDat");

            writer.WriteOptionalElementString("datVersionURL", obj.DatVersionUrl);

            if (obj.DatUrl is not null)
                WriteDatUrl(obj.DatUrl, writer);

            writer.WriteOptionalElementString("imURL", obj.ImUrl);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a Search to an XmlTextWriter
        /// </summary>
        /// <param name="obj">Search to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteSearch(Search obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("search");

            if (obj.To is not null && obj.To.Length > 0)
            {
                foreach (var to in obj.To)
                {
                    WriteTo(to, writer);
                }
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a To to an XmlTextWriter
        /// </summary>
        /// <param name="obj">To to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteTo(To obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("to");

            writer.WriteOptionalAttributeString("value", obj.Value);
            writer.WriteOptionalAttributeString("default", obj.Default.ToString()?.ToLowerInvariant());
            writer.WriteOptionalAttributeString("auto", obj.Auto);

            if (obj.Find is not null && obj.Find.Length > 0)
            {
                foreach (var find in obj.Find)
                {
                    WriteFind(find, writer);
                }
            }

            writer.WriteEndElement();
        }
    }
}

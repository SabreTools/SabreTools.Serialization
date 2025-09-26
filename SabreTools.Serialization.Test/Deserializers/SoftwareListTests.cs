using System.IO;
using System.Linq;
using SabreTools.Serialization.Deserializers;
using Xunit;

namespace SabreTools.Serialization.Test.Deserializers
{
    public class SoftwareListTests
    {
        [Fact]
        public void NullArray_Null()
        {
            byte[]? data = null;
            int offset = 0;
            var deserializer = new SoftwareList();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyArray_Null()
        {
            byte[]? data = [];
            int offset = 0;
            var deserializer = new SoftwareList();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidArray_Null()
        {
            byte[]? data = [.. Enumerable.Repeat<byte>(0xFF, 1024)];
            int offset = 0;
            var deserializer = new SoftwareList();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void NullStream_Null()
        {
            Stream? data = null;
            var deserializer = new SoftwareList();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyStream_Null()
        {
            Stream? data = new MemoryStream([]);
            var deserializer = new SoftwareList();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidStream_Null()
        {
            Stream? data = new MemoryStream([.. Enumerable.Repeat<byte>(0xFF, 1024)]);
            var deserializer = new SoftwareList();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void RoundTripTest()
        {
            // Get the serializer and deserializer
            var deserializer = new SoftwareList();
            var serializer = new SabreTools.Serialization.Serializers.SoftwareList();

            // Build the data
            Data.Models.SoftwareList.SoftwareList sl = Build();

            // Serialize to stream
            Stream? actual = serializer.Serialize(sl);
            Assert.NotNull(actual);

            // Serialize back to original model
            Data.Models.SoftwareList.SoftwareList? newSl = deserializer.Deserialize(actual);

            // Validate the data
            Assert.NotNull(newSl);
            Assert.Equal("XXXXXX", newSl.Name);
            Assert.Equal("XXXXXX", newSl.Description);
            Assert.Equal("XXXXXX", newSl.Notes);

            Assert.NotNull(newSl.Software);
            var newSoftware = Assert.Single(newSl.Software);
            Validate(newSoftware);
        }

        /// <summary>
        /// Build model for serialization and deserialization
        /// </summary>
        private static Data.Models.SoftwareList.SoftwareList Build()
        {
            var info = new Data.Models.SoftwareList.Info
            {
                Name = "XXXXXX",
                Value = "XXXXXX",
            };

            var sharedfeat = new Data.Models.SoftwareList.SharedFeat
            {
                Name = "XXXXXX",
                Value = "XXXXXX",
            };

            var feature = new Data.Models.SoftwareList.Feature
            {
                Name = "XXXXXX",
                Value = "XXXXXX",
            };

            var rom = new Data.Models.SoftwareList.Rom
            {
                Name = "XXXXXX",
                Size = "XXXXXX",
                Length = "XXXXXX",
                CRC = "XXXXXX",
                SHA1 = "XXXXXX",
                Offset = "XXXXXX",
                Value = "XXXXXX",
                Status = "XXXXXX",
                LoadFlag = "XXXXXX",
            };

            var dataarea = new Data.Models.SoftwareList.DataArea
            {
                Name = "XXXXXX",
                Size = "XXXXXX",
                Width = "XXXXXX",
                Endianness = "XXXXXX",
                Rom = [rom],
            };

            var disk = new Data.Models.SoftwareList.Disk
            {
                Name = "XXXXXX",
                MD5 = "XXXXXX",
                SHA1 = "XXXXXX",
                Status = "XXXXXX",
                Writeable = "XXXXXX",
            };

            var diskarea = new Data.Models.SoftwareList.DiskArea
            {
                Name = "XXXXXX",
                Disk = [disk],
            };

            var dipvalue = new Data.Models.SoftwareList.DipValue
            {
                Name = "XXXXXX",
                Value = "XXXXXX",
                Default = "XXXXXX",
            };

            var dipswitch = new Data.Models.SoftwareList.DipSwitch
            {
                Name = "XXXXXX",
                Tag = "XXXXXX",
                Mask = "XXXXXX",
                DipValue = [dipvalue],
            };

            var part = new Data.Models.SoftwareList.Part
            {
                Name = "XXXXXX",
                Interface = "XXXXXX",
                Feature = [feature],
                DataArea = [dataarea],
                DiskArea = [diskarea],
                DipSwitch = [dipswitch],
            };

            var software = new Data.Models.SoftwareList.Software
            {
                Name = "XXXXXX",
                CloneOf = "XXXXXX",
                Supported = "XXXXXX",
                Description = "XXXXXX",
                Year = "XXXXXX",
                Publisher = "XXXXXX",
                Notes = "XXXXXX",
                Info = [info],
                SharedFeat = [sharedfeat],
                Part = [part],
            };

            return new Data.Models.SoftwareList.SoftwareList
            {
                Name = "XXXXXX",
                Description = "XXXXXX",
                Notes = "XXXXXX",
                Software = [software],
            };
        }

        /// <summary>
        /// Validate a Software
        /// </summary>
        private static void Validate(Data.Models.SoftwareList.Software? software)
        {
            Assert.NotNull(software);
            Assert.Equal("XXXXXX", software.Name);
            Assert.Equal("XXXXXX", software.CloneOf);
            Assert.Equal("XXXXXX", software.Supported);
            Assert.Equal("XXXXXX", software.Description);
            Assert.Equal("XXXXXX", software.Year);
            Assert.Equal("XXXXXX", software.Publisher);
            Assert.Equal("XXXXXX", software.Notes);

            Assert.NotNull(software.Info);
            var info = Assert.Single(software.Info);
            Validate(info);

            Assert.NotNull(software.SharedFeat);
            var sharedfeat = Assert.Single(software.SharedFeat);
            Validate(sharedfeat);

            Assert.NotNull(software.Part);
            var part = Assert.Single(software.Part);
            Validate(part);
        }

        /// <summary>
        /// Validate a Info
        /// </summary>
        private static void Validate(Data.Models.SoftwareList.Info? info)
        {
            Assert.NotNull(info);
            Assert.Equal("XXXXXX", info.Name);
            Assert.Equal("XXXXXX", info.Value);
        }

        /// <summary>
        /// Validate a SharedFeat
        /// </summary>
        private static void Validate(Data.Models.SoftwareList.SharedFeat? sharedfeat)
        {
            Assert.NotNull(sharedfeat);
            Assert.Equal("XXXXXX", sharedfeat.Name);
            Assert.Equal("XXXXXX", sharedfeat.Value);
        }

        /// <summary>
        /// Validate a Part
        /// </summary>
        private static void Validate(Data.Models.SoftwareList.Part? part)
        {
            Assert.NotNull(part);
            Assert.Equal("XXXXXX", part.Name);
            Assert.Equal("XXXXXX", part.Interface);

            Assert.NotNull(part.Feature);
            var feature = Assert.Single(part.Feature);
            Validate(feature);

            Assert.NotNull(part.DataArea);
            var dataarea = Assert.Single(part.DataArea);
            Validate(dataarea);

            Assert.NotNull(part.DiskArea);
            var diskarea = Assert.Single(part.DiskArea);
            Validate(diskarea);

            Assert.NotNull(part.DipSwitch);
            var dipswitch = Assert.Single(part.DipSwitch);
            Validate(dipswitch);
        }

        /// <summary>
        /// Validate a Feature
        /// </summary>
        private static void Validate(Data.Models.SoftwareList.Feature? feature)
        {
            Assert.NotNull(feature);
            Assert.Equal("XXXXXX", feature.Name);
            Assert.Equal("XXXXXX", feature.Value);
        }

        /// <summary>
        /// Validate a DataArea
        /// </summary>
        private static void Validate(Data.Models.SoftwareList.DataArea? dataarea)
        {
            Assert.NotNull(dataarea);
            Assert.Equal("XXXXXX", dataarea.Name);
            Assert.Equal("XXXXXX", dataarea.Size);
            Assert.Equal("XXXXXX", dataarea.Width);
            Assert.Equal("XXXXXX", dataarea.Endianness);

            Assert.NotNull(dataarea.Rom);
            var rom = Assert.Single(dataarea.Rom);
            Validate(rom);
        }

        /// <summary>
        /// Validate a Rom
        /// </summary>
        private static void Validate(Data.Models.SoftwareList.Rom? rom)
        {
            Assert.NotNull(rom);
            Assert.Equal("XXXXXX", rom.Name);
            Assert.Equal("XXXXXX", rom.Size);
            Assert.Equal("XXXXXX", rom.Length);
            Assert.Equal("XXXXXX", rom.CRC);
            Assert.Equal("XXXXXX", rom.SHA1);
            Assert.Equal("XXXXXX", rom.Offset);
            Assert.Equal("XXXXXX", rom.Value);
            Assert.Equal("XXXXXX", rom.Status);
            Assert.Equal("XXXXXX", rom.LoadFlag);
        }

        /// <summary>
        /// Validate a DiskArea
        /// </summary>
        private static void Validate(Data.Models.SoftwareList.DiskArea? diskarea)
        {
            Assert.NotNull(diskarea);
            Assert.Equal("XXXXXX", diskarea.Name);

            Assert.NotNull(diskarea.Disk);
            var disk = Assert.Single(diskarea.Disk);
            Validate(disk);
        }

        /// <summary>
        /// Validate a Disk
        /// </summary>
        private static void Validate(Data.Models.SoftwareList.Disk? disk)
        {
            Assert.NotNull(disk);
            Assert.Equal("XXXXXX", disk.Name);
            Assert.Equal("XXXXXX", disk.MD5);
            Assert.Equal("XXXXXX", disk.SHA1);
            Assert.Equal("XXXXXX", disk.Status);
            Assert.Equal("XXXXXX", disk.Writeable);
        }

        /// <summary>
        /// Validate a DipSwitch
        /// </summary>
        private static void Validate(Data.Models.SoftwareList.DipSwitch? dipswitch)
        {
            Assert.NotNull(dipswitch);
            Assert.Equal("XXXXXX", dipswitch.Name);
            Assert.Equal("XXXXXX", dipswitch.Tag);
            Assert.Equal("XXXXXX", dipswitch.Mask);

            Assert.NotNull(dipswitch.DipValue);
            var dipvalue = Assert.Single(dipswitch.DipValue);
            Validate(dipvalue);
        }

        /// <summary>
        /// Validate a DipValue
        /// </summary>
        private static void Validate(Data.Models.SoftwareList.DipValue? dipvalue)
        {
            Assert.NotNull(dipvalue);
            Assert.Equal("XXXXXX", dipvalue.Name);
            Assert.Equal("XXXXXX", dipvalue.Value);
            Assert.Equal("XXXXXX", dipvalue.Default);
        }
    }
}
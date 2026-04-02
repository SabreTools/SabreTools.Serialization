using Xunit;

namespace SabreTools.Serialization.CrossModel.Test
{
    public class SoftwareListTests
    {
        [Fact]
        public void RoundTripTest()
        {
            // Get the cross-model serializer
            var serializer = new SoftwareList();

            // Build the data
            Data.Models.SoftwareList.SoftwareList sl = Build();

            // Serialize to generic model
            Data.Models.Metadata.MetadataFile? metadata = serializer.Serialize(sl);
            Assert.NotNull(metadata);

            // Serialize back to original model
            Data.Models.SoftwareList.SoftwareList? newSl = serializer.Deserialize(metadata);

            // Validate the data
            Assert.NotNull(newSl);
            Assert.Equal("name", newSl.Name);
            Assert.Equal("description", newSl.Description);
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
                Name = "name",
                Value = "value",
            };

            var sharedfeat = new Data.Models.SoftwareList.SharedFeat
            {
                Name = "name",
                Value = "value",
            };

            var feature = new Data.Models.SoftwareList.Feature
            {
                Name = "name",
                Value = "value",
            };

            var rom = new Data.Models.SoftwareList.Rom
            {
                Name = "name",
                Size = "XXXXXX",
                Length = "XXXXXX",
                CRC = "XXXXXX",
                SHA1 = "XXXXXX",
                Offset = "XXXXXX",
                Value = "value",
                Status = Data.Models.Metadata.ItemStatus.Good,
                LoadFlag = Data.Models.Metadata.LoadFlag.Load16Byte,
            };

            var dataarea = new Data.Models.SoftwareList.DataArea
            {
                Name = "name",
                Size = "XXXXXX",
                Width = "XXXXXX",
                Endianness = Data.Models.Metadata.Endianness.Big,
                Rom = [rom],
            };

            var disk = new Data.Models.SoftwareList.Disk
            {
                Name = "name",
                MD5 = "XXXXXX",
                SHA1 = "XXXXXX",
                Status = Data.Models.Metadata.ItemStatus.Good,
                Writeable = true,
            };

            var diskarea = new Data.Models.SoftwareList.DiskArea
            {
                Name = "name",
                Disk = [disk],
            };

            var dipvalue = new Data.Models.SoftwareList.DipValue
            {
                Name = "name",
                Value = "value",
                Default = true,
            };

            var dipswitch = new Data.Models.SoftwareList.DipSwitch
            {
                Name = "name",
                Tag = "tag",
                Mask = "mask",
                DipValue = [dipvalue],
            };

            var part = new Data.Models.SoftwareList.Part
            {
                Name = "name",
                Interface = "interface",
                Feature = [feature],
                DataArea = [dataarea],
                DiskArea = [diskarea],
                DipSwitch = [dipswitch],
            };

            var software = new Data.Models.SoftwareList.Software
            {
                Name = "name",
                CloneOf = "XXXXXX",
                Supported = Data.Models.Metadata.Supported.Yes,
                Description = "description",
                Year = "XXXXXX",
                Publisher = "XXXXXX",
                Notes = "XXXXXX",
                Info = [info],
                SharedFeat = [sharedfeat],
                Part = [part],
            };

            return new Data.Models.SoftwareList.SoftwareList
            {
                Name = "name",
                Description = "description",
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
            Assert.Equal("name", software.Name);
            Assert.Equal("XXXXXX", software.CloneOf);
            Assert.Equal(Data.Models.Metadata.Supported.Yes, software.Supported);
            Assert.Equal("description", software.Description);
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
            Assert.Equal("name", info.Name);
            Assert.Equal("value", info.Value);
        }

        /// <summary>
        /// Validate a SharedFeat
        /// </summary>
        private static void Validate(Data.Models.SoftwareList.SharedFeat? sharedfeat)
        {
            Assert.NotNull(sharedfeat);
            Assert.Equal("name", sharedfeat.Name);
            Assert.Equal("value", sharedfeat.Value);
        }

        /// <summary>
        /// Validate a Part
        /// </summary>
        private static void Validate(Data.Models.SoftwareList.Part? part)
        {
            Assert.NotNull(part);
            Assert.Equal("name", part.Name);
            Assert.Equal("interface", part.Interface);

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
            Assert.Equal("name", feature.Name);
            Assert.Equal("value", feature.Value);
        }

        /// <summary>
        /// Validate a DataArea
        /// </summary>
        private static void Validate(Data.Models.SoftwareList.DataArea? dataarea)
        {
            Assert.NotNull(dataarea);
            Assert.Equal("name", dataarea.Name);
            Assert.Equal("XXXXXX", dataarea.Size);
            Assert.Equal("XXXXXX", dataarea.Width);
            Assert.Equal(Data.Models.Metadata.Endianness.Big, dataarea.Endianness);

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
            Assert.Equal("name", rom.Name);
            Assert.Equal("XXXXXX", rom.Size);
            Assert.Equal("XXXXXX", rom.Length);
            Assert.Equal("XXXXXX", rom.CRC);
            Assert.Equal("XXXXXX", rom.SHA1);
            Assert.Equal("XXXXXX", rom.Offset);
            Assert.Equal("value", rom.Value);
            Assert.Equal(Data.Models.Metadata.ItemStatus.Good, rom.Status);
            Assert.Equal(Data.Models.Metadata.LoadFlag.Load16Byte, rom.LoadFlag);
        }

        /// <summary>
        /// Validate a DiskArea
        /// </summary>
        private static void Validate(Data.Models.SoftwareList.DiskArea? diskarea)
        {
            Assert.NotNull(diskarea);
            Assert.Equal("name", diskarea.Name);

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
            Assert.Equal("name", disk.Name);
            Assert.Equal("XXXXXX", disk.MD5);
            Assert.Equal("XXXXXX", disk.SHA1);
            Assert.Equal(Data.Models.Metadata.ItemStatus.Good, disk.Status);
            Assert.Equal(true, disk.Writeable);
        }

        /// <summary>
        /// Validate a DipSwitch
        /// </summary>
        private static void Validate(Data.Models.SoftwareList.DipSwitch? dipswitch)
        {
            Assert.NotNull(dipswitch);
            Assert.Equal("name", dipswitch.Name);
            Assert.Equal("tag", dipswitch.Tag);
            Assert.Equal("mask", dipswitch.Mask);

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
            Assert.Equal("name", dipvalue.Name);
            Assert.Equal("value", dipvalue.Value);
            Assert.Equal(true, dipvalue.Default);
        }
    }
}

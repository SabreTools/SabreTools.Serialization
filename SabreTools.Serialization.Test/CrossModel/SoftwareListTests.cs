using Xunit;

namespace SabreTools.Serialization.Test.CrossModel
{
    public class SoftwareListTests
    {
        [Fact]
        public void RoundTripTest()
        {
            // Get the cross-model serializer
            var serializer = new Serialization.CrossModel.SoftwareList();

            // Build the data
            Models.SoftwareList.SoftwareList sl = Build();

            // Serialize to generic model
            Models.Metadata.MetadataFile? metadata = serializer.Serialize(sl);
            Assert.NotNull(metadata);

            // Serialize back to original model
            Models.SoftwareList.SoftwareList? newSl = serializer.Deserialize(metadata);

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
        private static Models.SoftwareList.SoftwareList Build()
        {
            var info = new Models.SoftwareList.Info
            {
                Name = "XXXXXX",
                Value = "XXXXXX",
            };

            var sharedfeat = new Models.SoftwareList.SharedFeat
            {
                Name = "XXXXXX",
                Value = "XXXXXX",
            };

            var feature = new Models.SoftwareList.Feature
            {
                Name = "XXXXXX",
                Value = "XXXXXX",
            };

            var rom = new Models.SoftwareList.Rom
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

            var dataarea = new Models.SoftwareList.DataArea
            {
                Name = "XXXXXX",
                Size = "XXXXXX",
                Width = "XXXXXX",
                Endianness = "XXXXXX",
                Rom = [rom],
            };

            var disk = new Models.SoftwareList.Disk
            {
                Name = "XXXXXX",
                MD5 = "XXXXXX",
                SHA1 = "XXXXXX",
                Status = "XXXXXX",
                Writeable = "XXXXXX",
            };

            var diskarea = new Models.SoftwareList.DiskArea
            {
                Name = "XXXXXX",
                Disk = [disk],
            };

            var dipvalue = new Models.SoftwareList.DipValue
            {
                Name = "XXXXXX",
                Value = "XXXXXX",
                Default = "XXXXXX",
            };

            var dipswitch = new Models.SoftwareList.DipSwitch
            {
                Name = "XXXXXX",
                Tag = "XXXXXX",
                Mask = "XXXXXX",
                DipValue = [dipvalue],
            };

            var part = new Models.SoftwareList.Part
            {
                Name = "XXXXXX",
                Interface = "XXXXXX",
                Feature = [feature],
                DataArea = [dataarea],
                DiskArea = [diskarea],
                DipSwitch = [dipswitch],
            };

            var software = new Models.SoftwareList.Software
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

            return new Models.SoftwareList.SoftwareList
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
        private static void Validate(Models.SoftwareList.Software? software)
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
        private static void Validate(Models.SoftwareList.Info? info)
        {
            Assert.NotNull(info);
            Assert.Equal("XXXXXX", info.Name);
            Assert.Equal("XXXXXX", info.Value);
        }

        /// <summary>
        /// Validate a SharedFeat
        /// </summary>
        private static void Validate(Models.SoftwareList.SharedFeat? sharedfeat)
        {
            Assert.NotNull(sharedfeat);
            Assert.Equal("XXXXXX", sharedfeat.Name);
            Assert.Equal("XXXXXX", sharedfeat.Value);
        }

        /// <summary>
        /// Validate a Part
        /// </summary>
        private static void Validate(Models.SoftwareList.Part? part)
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
        private static void Validate(Models.SoftwareList.Feature? feature)
        {
            Assert.NotNull(feature);
            Assert.Equal("XXXXXX", feature.Name);
            Assert.Equal("XXXXXX", feature.Value);
        }

        /// <summary>
        /// Validate a DataArea
        /// </summary>
        private static void Validate(Models.SoftwareList.DataArea? dataarea)
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
        private static void Validate(Models.SoftwareList.Rom? rom)
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
        private static void Validate(Models.SoftwareList.DiskArea? diskarea)
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
        private static void Validate(Models.SoftwareList.Disk? disk)
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
        private static void Validate(Models.SoftwareList.DipSwitch? dipswitch)
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
        private static void Validate(Models.SoftwareList.DipValue? dipvalue)
        {
            Assert.NotNull(dipvalue);
            Assert.Equal("XXXXXX", dipvalue.Name);
            Assert.Equal("XXXXXX", dipvalue.Value);
            Assert.Equal("XXXXXX", dipvalue.Default);
        }
    }
}
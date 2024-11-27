using Xunit;

namespace SabreTools.Serialization.Test.CrossModel
{
    public class OpenMSXTests
    {
        [Fact]
        public void RoundTripTest()
        {
            // Get the cross-model serializer
            var serializer = new Serialization.CrossModel.OpenMSX();

            // Build the data
            Models.OpenMSX.SoftwareDb sdb = Build();

            // Serialize to generic model
            Models.Metadata.MetadataFile? metadata = serializer.Serialize(sdb);
            Assert.NotNull(metadata);

            // Serialize back to original model
            Models.OpenMSX.SoftwareDb? newSdb = serializer.Deserialize(metadata);

            // Validate the data
            Assert.NotNull(newSdb);
            Assert.Equal("XXXXXX", newSdb.Timestamp);

            Assert.NotNull(newSdb.Software);
            var software = Assert.Single(newSdb.Software);
            Validate(software);
        }

        /// <summary>
        /// Build model for serialization and deserialization
        /// </summary>
        private static Models.OpenMSX.SoftwareDb Build()
        {
            var original = new Models.OpenMSX.Original
            {
                Value = "XXXXXX",
                Content = "XXXXXX",
            };

            var rom = new Models.OpenMSX.Rom
            {
                Start = "XXXXXX",
                Type = "XXXXXX",
                Hash = "XXXXXX",
                Remark = "XXXXXX",
            };

            var dump_rom = new Models.OpenMSX.Dump
            {
                Original = original,
                Rom = rom,
            };

            var megarom = new Models.OpenMSX.MegaRom
            {
                Start = "XXXXXX",
                Type = "XXXXXX",
                Hash = "XXXXXX",
                Remark = "XXXXXX",
            };

            var dump_megarom = new Models.OpenMSX.Dump
            {
                Original = original,
                Rom = megarom,
            };

            var sccpluscart = new Models.OpenMSX.SCCPlusCart
            {
                Start = "XXXXXX",
                Type = "XXXXXX",
                Hash = "XXXXXX",
                Remark = "XXXXXX",
            };

            var dump_sccpluscart = new Models.OpenMSX.Dump
            {
                Original = original,
                Rom = sccpluscart,
            };
            
            var software = new Models.OpenMSX.Software
            {
                Title = "XXXXXX",
                GenMSXID = "XXXXXX",
                System = "XXXXXX",
                Company = "XXXXXX",
                Year = "XXXXXX",
                Country = "XXXXXX",
                Dump = [dump_rom, dump_megarom, dump_sccpluscart],
            };

            return new Models.OpenMSX.SoftwareDb
            {
                Timestamp = "XXXXXX",
                Software = [software],
            };
        }

        /// <summary>
        /// Validate a Software
        /// </summary>
        private static void Validate(Models.OpenMSX.Software? software)
        {
            Assert.NotNull(software);
            Assert.Equal("XXXXXX", software.Title);
            Assert.Equal("XXXXXX", software.GenMSXID);
            Assert.Equal("XXXXXX", software.System);
            Assert.Equal("XXXXXX", software.Company);
            Assert.Equal("XXXXXX", software.Year);
            Assert.Equal("XXXXXX", software.Country);

            Assert.NotNull(software.Dump);
            Assert.Equal(3, software.Dump.Length);
            foreach (var dump in software.Dump)
            {
                Validate(dump);
            }
        }

        /// <summary>
        /// Validate a Dump
        /// </summary>
        private static void Validate(Models.OpenMSX.Dump? dump)
        {
            Assert.NotNull(dump);
            Validate(dump.Original);
            Validate(dump.Rom);
        }

        /// <summary>
        /// Validate a Original
        /// </summary>
        private static void Validate(Models.OpenMSX.Original? original)
        {
            Assert.NotNull(original);
            Assert.Equal("XXXXXX", original.Value);
            Assert.Equal("XXXXXX", original.Content);
        }

        /// <summary>
        /// Validate a RomBase
        /// </summary>
        private static void Validate(Models.OpenMSX.RomBase? rombase)
        {
            Assert.NotNull(rombase);
            Assert.Equal("XXXXXX", rombase.Start);
            Assert.Equal("XXXXXX", rombase.Type);
            Assert.Equal("XXXXXX", rombase.Hash);
            Assert.Equal("XXXXXX", rombase.Remark);
        }
    }
}
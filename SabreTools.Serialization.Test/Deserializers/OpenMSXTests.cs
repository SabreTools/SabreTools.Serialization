using System.IO;
using System.Linq;
using SabreTools.Serialization.Deserializers;
using Xunit;

namespace SabreTools.Serialization.Test.Deserializers
{
    public class OpenMSXTests
    {
        [Fact]
        public void NullArray_Null()
        {
            byte[]? data = null;
            int offset = 0;
            var deserializer = new OpenMSX();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyArray_Null()
        {
            byte[]? data = [];
            int offset = 0;
            var deserializer = new OpenMSX();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidArray_Null()
        {
            byte[]? data = [.. Enumerable.Repeat<byte>(0xFF, 1024)];
            int offset = 0;
            var deserializer = new OpenMSX();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void NullStream_Null()
        {
            Stream? data = null;
            var deserializer = new OpenMSX();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyStream_Null()
        {
            Stream? data = new MemoryStream([]);
            var deserializer = new OpenMSX();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidStream_Null()
        {
            Stream? data = new MemoryStream([.. Enumerable.Repeat<byte>(0xFF, 1024)]);
            var deserializer = new OpenMSX();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void RoundTripTest()
        {
            // Get the serializer and deserializer
            var deserializer = new OpenMSX();
            var serializer = new SabreTools.Serialization.Serializers.OpenMSX();

            // Build the data
            Models.OpenMSX.SoftwareDb sdb = Build();

            // Serialize to stream
            Stream? metadata = serializer.Serialize(sdb);
            Assert.NotNull(metadata);

            // Serialize back to original model
            Models.OpenMSX.SoftwareDb? newSdb = deserializer.Deserialize(metadata);

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
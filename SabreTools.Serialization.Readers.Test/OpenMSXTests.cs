using System.IO;
using System.Linq;
using Xunit;

namespace SabreTools.Serialization.Readers.Test
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
            var serializer = new Writers.OpenMSX();

            // Build the data
            Data.Models.OpenMSX.SoftwareDb sdb = Build();

            // Serialize to stream
            Stream? metadata = serializer.SerializeStream(sdb);
            Assert.NotNull(metadata);

            // Serialize back to original model
            Data.Models.OpenMSX.SoftwareDb? newSdb = deserializer.Deserialize(metadata);

            // Validate the data
            Assert.NotNull(newSdb);
            Assert.Equal("timestamp", newSdb.Timestamp);

            Assert.NotNull(newSdb.Software);
            var software = Assert.Single(newSdb.Software);
            Validate(software);
        }

        /// <summary>
        /// Build model for serialization and deserialization
        /// </summary>
        private static Data.Models.OpenMSX.SoftwareDb Build()
        {
            var original = new Data.Models.OpenMSX.Original
            {
                Value = true,
                Content = "content",
            };

            var rom = new Data.Models.OpenMSX.Rom
            {
                Start = "start",
                Type = "type",
                Hash = "hash",
                Remark = "remark",
            };

            var dump_rom = new Data.Models.OpenMSX.Dump
            {
                Original = original,
                Boot = "boot",
                Rom = rom,
            };

            var megarom = new Data.Models.OpenMSX.MegaRom
            {
                Start = "start",
                Type = "type",
                Hash = "hash",
                Remark = "remark",
            };

            var dump_megarom = new Data.Models.OpenMSX.Dump
            {
                Original = original,
                Boot = "boot",
                Rom = megarom,
            };

            var sccpluscart = new Data.Models.OpenMSX.SCCPlusCart
            {
                Start = "start",
                Type = "type",
                Hash = "hash",
                Remark = "remark",
            };

            var dump_sccpluscart = new Data.Models.OpenMSX.Dump
            {
                Original = original,
                Boot = "boot",
                Rom = sccpluscart,
            };

            var software = new Data.Models.OpenMSX.Software
            {
                Title = "title",
                GenMSXID = "genmsxid",
                System = "system",
                Company = "company",
                Year = "year",
                Country = "country",
                Dump = [dump_rom, dump_megarom, dump_sccpluscart],
            };

            return new Data.Models.OpenMSX.SoftwareDb
            {
                Timestamp = "timestamp",
                Software = [software],
            };
        }

        /// <summary>
        /// Validate a Software
        /// </summary>
        private static void Validate(Data.Models.OpenMSX.Software? software)
        {
            Assert.NotNull(software);
            Assert.Equal("title", software.Title);
            Assert.Equal("genmsxid", software.GenMSXID);
            Assert.Equal("system", software.System);
            Assert.Equal("company", software.Company);
            Assert.Equal("year", software.Year);
            Assert.Equal("country", software.Country);

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
        private static void Validate(Data.Models.OpenMSX.Dump? dump)
        {
            Assert.NotNull(dump);
            Assert.Equal("boot", dump.Boot);

            Validate(dump.Original);
            Validate(dump.Rom);
        }

        /// <summary>
        /// Validate a Original
        /// </summary>
        private static void Validate(Data.Models.OpenMSX.Original? original)
        {
            Assert.NotNull(original);
            Assert.Equal(true, original.Value);
            Assert.Equal("content", original.Content);
        }

        /// <summary>
        /// Validate a RomBase
        /// </summary>
        private static void Validate(Data.Models.OpenMSX.RomBase? rombase)
        {
            Assert.NotNull(rombase);
            Assert.Equal("start", rombase.Start);
            Assert.Equal("type", rombase.Type);
            Assert.Equal("hash", rombase.Hash);
            Assert.Equal("remark", rombase.Remark);
        }
    }
}

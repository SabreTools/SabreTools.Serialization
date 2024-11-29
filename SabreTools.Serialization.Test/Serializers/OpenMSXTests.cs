using System;
using System.IO;
using SabreTools.Hashing;
using SabreTools.Serialization.Serializers;
using Xunit;

namespace SabreTools.Serialization.Test.Serializers
{
    public class OpenMSXTests
    {
        [Fact]
        public void SerializeArray_Null_Null()
        {
            var serializer = new OpenMSX();
            byte[]? actual = serializer.SerializeArray(null);
            Assert.Null(actual);
        }

        [Fact]
        public void SerializeStream_Null_Null()
        {
            var serializer = new OpenMSX();
            Stream? actual = serializer.Serialize(null);
            Assert.Null(actual);
        }
    
        [Fact]
        public void SerializeStream_Valid_Filled()
        {
            // Create the object for serialization
            var dat = Build();

            // Deserialize the file
            var actual = OpenMSX.SerializeStream(dat);
            actual?.Seek(0, SeekOrigin.Begin);

            // Validate the values
            Assert.NotNull(actual);
            string? actualHash = HashTool.GetStreamHash(actual, HashType.SHA1);
            Assert.NotNull(actualHash);
            Assert.Equal("285864811c15ad0f3b18c605c62ae3907f3e2f27", actualHash);
        }

        private static Models.OpenMSX.SoftwareDb Build()
        {
            var original = new Models.OpenMSX.Original
            {
                Value = "false",
                Content = "Original Name",
            };

            var rom = new Models.OpenMSX.Rom
            {
                Start = "0x0000",
                Type = "Game",
                Hash = "da39a3ee5e6b4b0d3255bfef95601890afd80709",
                Remark = "Comment",
            };

            var megaRom = new Models.OpenMSX.MegaRom
            {
                Start = "0x1000",
                Type = "Software",
                Hash = "da39a3ee5e6b4b0d3255bfef95601890afd80709",
                Remark = "Comment",
            };

            var sccPlusCart = new Models.OpenMSX.SCCPlusCart
            {
                Start = "0x2000",
                Type = "Utility",
                Hash = "da39a3ee5e6b4b0d3255bfef95601890afd80709",
                Remark = "Comment",
            };

            var dump = new Models.OpenMSX.Dump[]
            {
                new() { Original = original, Rom = rom },
                new() { Rom = megaRom },
                new() { Rom = sccPlusCart },
            };

            var software = new Models.OpenMSX.Software
            {
                Title = "Software Title",
                GenMSXID = "00000", // Not required
                System = "MSX 2",
                Company = "Imaginary Company, Inc.",
                Year = "19xx",
                Country = "Imaginaria",
                Dump = dump,
            };

            return new Models.OpenMSX.SoftwareDb
            {
                Timestamp = "1234567890",
                Software = [software],
            };
        }
    }
}
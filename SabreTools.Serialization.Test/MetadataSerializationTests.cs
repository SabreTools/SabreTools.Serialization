using System;
using System.IO;
using System.Security.Cryptography;
using SabreTools.Serialization.Serializers;
using Xunit;

namespace SabreTools.Serialization.Test
{
    public class MetadataSerializationTests
    {
        [Fact]
        public void OpenMSXSeserializeTest()
        {
            // Create the object for serialization
            var dat = GenerateOpenMSX();

            // Deserialize the file
            var stream = OpenMSX.SerializeStream(dat) as MemoryStream;

            // Validate the values
            Assert.NotNull(stream);
            byte[] hash = SHA1.HashData(stream.GetBuffer());
            string hashstr = BitConverter.ToString(hash).Replace("-", string.Empty);
            Assert.Equal("CCBFAAB56BAAF6BE56A85918055784A615379659", hashstr);
        }

        #region Payload Generators

        /// <summary>
        /// Generate a consistent OpenMSX SoftwareDb for testing
        /// </summary>
        private static Models.OpenMSX.SoftwareDb GenerateOpenMSX()
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

        #endregion
    }
}
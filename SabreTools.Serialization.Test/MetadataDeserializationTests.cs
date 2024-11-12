using System;
using System.IO;
using SabreTools.Hashing;
using SabreTools.Serialization.Deserializers;
using Xunit;

namespace SabreTools.Serialization.Test
{
    /// <remarks>
    /// Filenames that end in `-files` are real examples.
    /// All other files are artificial and may not fully represent real examples.
    /// </remarks>
    public class MetadataDeserializationTests
    {
        [Theory]
        [InlineData("test-archivedotorg-files1.xml", 22)]
        [InlineData("test-archivedotorg-files2.xml", 13)]
        [InlineData("test-archivedotorg-files3.xml", 21)]
        [InlineData("test-archivedotorg-files4.xml", 19)]
        [InlineData("test-archivedotorg-files5.xml", 1390)]
        public void ArchiveDotOrgDeserializeTest(string path, long count)
        {
            // Open the file for reading
            string filename = GetTestFilePath(path);

            // Deserialize the file
            var dat = ArchiveDotOrg.DeserializeFile(filename);

            // Validate the values
            Assert.NotNull(dat?.File);
            Assert.Equal(count, dat.File.Length);

            // Validate we're not missing any attributes or elements
            foreach (var file in dat.File)
            {
                Assert.NotNull(file);
            }
        }

        [Theory]
        [InlineData("test-attractmode-files.txt", 11)]
        public void AttractModeDeserializeTest(string path, long count)
        {
            // Open the file for reading
            string filename = GetTestFilePath(path);

            // Deserialize the file
            var dat = AttractMode.DeserializeFile(filename);

            // Validate texpected: he values
            Assert.NotNull(dat?.Row);
            Assert.Equal(count, dat.Row.Length);

            // Validate we're not missing any attributes or elements
            foreach (var file in dat.Row)
            {
                Assert.NotNull(file);
            }
        }

        [Theory]
        [InlineData("test-cmp-files1.dat", 59, true)]
        [InlineData("test-cmp-files2.dat", 312, false)]
        public void ClrMameProDeserializeTest(string path, long count, bool expectHeader)
        {
            // Open the file for reading
            string filename = GetTestFilePath(path);

            // Deserialize the file
            var dat = ClrMamePro.DeserializeFile(filename, quotes: true);

            // Validate the values
            if (expectHeader)
                Assert.NotNull(dat?.ClrMamePro);
            else
                Assert.Null(dat?.ClrMamePro);

            Assert.NotNull(dat?.Game);
            Assert.Equal(count, dat.Game.Length);
        }

        [Theory]
        [InlineData("test-doscenter-files1.dat.gz", 34965)]
        [InlineData("test-doscenter-files2.dat.gz", 7189)]
        public void DosCenterDeserializeTest(string path, long count)
        {
            // Open the file for reading
            string filename = GetTestFilePath(path);

            // Deserialize the file
            var dat = DosCenter.DeserializeFile(filename);

            // Validate the values
            Assert.NotNull(dat?.DosCenter);

            Assert.NotNull(dat?.Game);
            Assert.Equal(count, dat.Game.Length);

            foreach (var game in dat.Game)
            {
                Assert.NotNull(game.File);
            }
        }

        [Theory]
        [InlineData("test-smdb-files.txt", 6113)]
        public void EverdriveSMDBDeserializeTest(string path, long count)
        {
            // Open the file for reading
            string filename = GetTestFilePath(path);

            // Deserialize the file
            var dat = EverdriveSMDB.DeserializeFile(filename);

            // Validate the values
            Assert.NotNull(dat?.Row);
            Assert.Equal(count, dat.Row.Length);
        }

        [Theory]
        [InlineData("test-sfv-files.sfv", HashType.CRC32, 100)]
        [InlineData("test-md5-files.md5", HashType.MD5, 100)]
        [InlineData("test-sha1-files.sha1", HashType.SHA1, 100)]
        [InlineData("test-sha256.sha256", HashType.SHA256, 1)]
        [InlineData("test-sha384.sha384", HashType.SHA384, 1)]
        [InlineData("test-sha512.sha512", HashType.SHA512, 1)]
        [InlineData("test-spamsum.spamsum", HashType.SpamSum, 1)]
        public void HashfileDeserializeTest(string path, HashType hash, long count)
        {
            // Open the file for reading
            string filename = GetTestFilePath(path);

            // Deserialize the file
            var dat = Hashfile.DeserializeFile(filename, hash);

            // Validate the values
            Assert.NotNull(dat);

            switch (hash)
            {
                case HashType.CRC32:
                    Assert.NotNull(dat.SFV);
                    Assert.Equal(count, dat.SFV.Length);
                    break;
                case HashType.MD5:
                    Assert.NotNull(dat.MD5);
                    Assert.Equal(count, dat.MD5.Length);
                    break;
                case HashType.SHA1:
                    Assert.NotNull(dat.SHA1);
                    Assert.Equal(count, dat.SHA1.Length);
                    break;
                case HashType.SHA256:
                    Assert.NotNull(dat.SHA256);
                    Assert.Equal(count, dat.SHA256.Length);
                    break;
                case HashType.SHA384:
                    Assert.NotNull(dat.SHA384);
                    Assert.Equal(count, dat.SHA384.Length);
                    break;
                case HashType.SHA512:
                    Assert.NotNull(dat.SHA512);
                    Assert.Equal(count, dat.SHA512.Length);
                    break;
                case HashType.SpamSum:
                    Assert.NotNull(dat.SpamSum);
                    Assert.Equal(count, dat.SpamSum.Length);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(hash));
            }
        }

        [Theory]
        [InlineData("test-listrom-files.txt.gz", 45861)]
        public void ListromDeserializeTest(string path, long count)
        {
            // Open the file for reading
            string filename = GetTestFilePath(path);

            // Deserialize the file
            var dat = Listrom.DeserializeFile(filename);

            // Validate the values
            Assert.NotNull(dat?.Set);
            Assert.Equal(count, dat.Set.Length);
        }

        [Theory]
        [InlineData("test-listxml-files1.xml.gz", 45861)]
        [InlineData("test-listxml-files2.xml", 3998)]
        public void ListxmlDeserializeTest(string path, long count)
        {
            // Open the file for reading
            string filename = GetTestFilePath(path);

            // Deserialize the file
            var dat = Listxml.DeserializeFile(filename);

            // Validate the values
            Assert.NotNull(dat?.Game);
            Assert.Equal(count, dat.Game.Length);
        }

        [Theory]
        [InlineData("test-logiqx-files1.xml.gz", 45875)]
        [InlineData("test-logiqx-files2.xml", 761)]
        public void LogiqxDeserializeTest(string path, long count)
        {
            // Open the file for reading
            string filename = GetTestFilePath(path);

            // Deserialize the file
            var dat = Logiqx.DeserializeFile(filename);

            // Validate the values
            Assert.NotNull(dat?.Game);
            Assert.Equal(count, dat.Game.Length);
        }

        [Theory]
        [InlineData("test-offlinelist-files.xml", 6750)]
        public void OfflineListDeserializeTest(string path, long count)
        {
            // Open the file for reading
            string filename = GetTestFilePath(path);

            // Deserialize the file
            var dat = OfflineList.DeserializeFile(filename);

            // Validate the values
            Assert.NotNull(dat?.Games?.Game);
            Assert.Equal(count, dat.Games.Game.Length);
        }

        [Theory]
        [InlineData("test-openmsx-files.xml", 2550)]
        public void OpenMSXDeserializeTest(string path, long count)
        {
            // Open the file for reading
            string filename = GetTestFilePath(path);

            // Deserialize the file
            var dat = OpenMSX.DeserializeFile(filename);

            // Validate the values
            Assert.NotNull(dat);
            Assert.NotNull(dat.Software);
            Assert.Equal(count, dat.Software.Length);
        }

        [Theory]
        [InlineData("test-romcenter-files.dat", 901)]
        public void RomCenterDeserializeTest(string path, long count)
        {
            // Open the file for reading
            string filename = GetTestFilePath(path);

            // Deserialize the file
            var dat = RomCenter.DeserializeFile(filename);

            // Validate the values
            Assert.NotNull(dat?.Games?.Rom);
            Assert.Equal(count, dat.Games.Rom.Length);
        }

        [Theory]
        [InlineData("test-csv-files1.csv", ',', 2)]
        [InlineData("test-csv-files2.csv", ',', 2)]
        [InlineData("test-ssv-files1.ssv", ';', 2)]
        [InlineData("test-ssv-files2.ssv", ';', 2)]
        [InlineData("test-tsv-files1.tsv", '\t', 2)]
        [InlineData("test-tsv-files2.tsv", '\t', 2)]
        public void SeparatedValueDeserializeTest(string path, char delim, long count)
        {
            // Open the file for reading
            string filename = GetTestFilePath(path);

            // Deserialize the file
            var dat = SeparatedValue.DeserializeFile(filename, delim);

            // Validate the values
            Assert.NotNull(dat?.Row);
            Assert.Equal(count, dat.Row.Length);
        }

        [Theory]
        [InlineData("test-softwarelist-files1.xml", 4531)]
        [InlineData("test-softwarelist-files2.xml", 2797)]
        [InlineData("test-softwarelist-files3.xml", 274)]
        public void SoftwareListDeserializeTest(string path, long count)
        {
            // Open the file for reading
            string filename = GetTestFilePath(path);

            // Deserialize the file
            var dat = SoftwareList.DeserializeFile(filename);

            // Validate the values
            Assert.NotNull(dat);
            Assert.NotNull(dat.Software);
            Assert.Equal(count, dat.Software.Length);
        }

        /// <summary>
        /// Get the path to the test file
        /// </summary>
        private static string GetTestFilePath(string path)
        {
            return Path.Combine(Environment.CurrentDirectory, "TestData", path);
        }
    }
}
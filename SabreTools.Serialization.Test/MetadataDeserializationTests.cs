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
            Assert.Null(dat.ADDITIONAL_ATTRIBUTES);
            Assert.Null(dat.ADDITIONAL_ELEMENTS);
            foreach (var file in dat.File)
            {
                Assert.NotNull(file);
                Assert.Null(file.ADDITIONAL_ATTRIBUTES);
                Assert.Null(file.ADDITIONAL_ELEMENTS);
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
                Assert.Null(file.ADDITIONAL_ELEMENTS);
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
            {
                Assert.NotNull(dat?.ClrMamePro);
                Assert.Null(dat.ClrMamePro.ADDITIONAL_ELEMENTS);
            }
            else
            {
                Assert.Null(dat?.ClrMamePro);
            }

            Assert.NotNull(dat?.Game);
            Assert.Equal(count, dat.Game.Length);

            // Validate we're not missing any attributes or elements
            Assert.NotNull(dat?.ADDITIONAL_ELEMENTS);
            Assert.Empty(dat.ADDITIONAL_ELEMENTS);
            foreach (var game in dat.Game)
            {
                Assert.NotNull(game?.ADDITIONAL_ELEMENTS);
                Assert.Empty(game.ADDITIONAL_ELEMENTS);
                foreach (var release in game.Release ?? Array.Empty<Models.ClrMamePro.Release>())
                {
                    Assert.NotNull(release?.ADDITIONAL_ELEMENTS);
                    Assert.Empty(release.ADDITIONAL_ELEMENTS);
                }

                foreach (var biosset in game.BiosSet ?? Array.Empty<Models.ClrMamePro.BiosSet>())
                {
                    Assert.NotNull(biosset?.ADDITIONAL_ELEMENTS);
                    Assert.Empty(biosset.ADDITIONAL_ELEMENTS);
                }

                foreach (var rom in game.Rom ?? Array.Empty<Models.ClrMamePro.Rom>())
                {
                    Assert.NotNull(rom?.ADDITIONAL_ELEMENTS);
                    Assert.Empty(rom.ADDITIONAL_ELEMENTS);
                }

                foreach (var disk in game.Disk ?? Array.Empty<Models.ClrMamePro.Disk>())
                {
                    Assert.NotNull(disk?.ADDITIONAL_ELEMENTS);
                    Assert.Empty(disk.ADDITIONAL_ELEMENTS);
                }

                foreach (var media in game.Media ?? Array.Empty<Models.ClrMamePro.Media>())
                {
                    Assert.NotNull(media?.ADDITIONAL_ELEMENTS);
                    Assert.Empty(media.ADDITIONAL_ELEMENTS);
                }

                foreach (var sample in game.Sample ?? Array.Empty<Models.ClrMamePro.Sample>())
                {
                    Assert.NotNull(sample?.ADDITIONAL_ELEMENTS);
                    Assert.Empty(sample.ADDITIONAL_ELEMENTS);
                }

                foreach (var archive in game.Archive ?? Array.Empty<Models.ClrMamePro.Archive>())
                {
                    Assert.NotNull(archive?.ADDITIONAL_ELEMENTS);
                    Assert.Empty(archive.ADDITIONAL_ELEMENTS);
                }

                foreach (var chip in game.Chip ?? Array.Empty<Models.ClrMamePro.Chip>())
                {
                    Assert.NotNull(chip?.ADDITIONAL_ELEMENTS);
                    Assert.Empty(chip.ADDITIONAL_ELEMENTS);
                }

                foreach (var video in game.Video ?? Array.Empty<Models.ClrMamePro.Video>())
                {
                    Assert.NotNull(video?.ADDITIONAL_ELEMENTS);
                    Assert.Empty(video.ADDITIONAL_ELEMENTS);
                }

                if (game.Sound != null)
                {
                    Assert.NotNull(game.Sound?.ADDITIONAL_ELEMENTS);
                    Assert.Empty(game.Sound.ADDITIONAL_ELEMENTS);
                }

                if (game.Input != null)
                {
                    Assert.NotNull(game.Input?.ADDITIONAL_ELEMENTS);
                    Assert.Empty(game.Input.ADDITIONAL_ELEMENTS);
                }

                foreach (var dipswitch in game.DipSwitch ?? Array.Empty<Models.ClrMamePro.DipSwitch>())
                {
                    Assert.NotNull(dipswitch?.ADDITIONAL_ELEMENTS);
                    Assert.Empty(dipswitch.ADDITIONAL_ELEMENTS);
                }

                if (game.Driver != null)
                {
                    Assert.NotNull(game.Driver?.ADDITIONAL_ELEMENTS);
                    Assert.Empty(game.Driver.ADDITIONAL_ELEMENTS);
                }
            }
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

            // Validate we're not missing any attributes or elements
            Assert.NotNull(dat?.ADDITIONAL_ELEMENTS);
            Assert.Empty(dat.ADDITIONAL_ELEMENTS);

            Assert.NotNull(dat.DosCenter?.ADDITIONAL_ELEMENTS);
            Assert.Empty(dat.DosCenter.ADDITIONAL_ELEMENTS);
            foreach (var game in dat.Game)
            {
                Assert.NotNull(game?.ADDITIONAL_ELEMENTS);
                Assert.Empty(game.ADDITIONAL_ELEMENTS);

                Assert.NotNull(game.File);
                foreach (var file in game.File)
                {
                    Assert.NotNull(file?.ADDITIONAL_ELEMENTS);
                    Assert.Empty(file.ADDITIONAL_ELEMENTS);
                }
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

            // Validate we're not missing any attributes or elements
            foreach (var file in dat.Row)
            {
                Assert.Null(file.ADDITIONAL_ELEMENTS);
            }
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

            // Validate we're not missing any attributes or elements
            Assert.NotNull(dat.ADDITIONAL_ELEMENTS);
            Assert.Empty(dat.ADDITIONAL_ELEMENTS);
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

            // Validate we're not missing any attributes or elements
            Assert.Null(dat.ADDITIONAL_ATTRIBUTES);
            Assert.Null(dat.ADDITIONAL_ELEMENTS);
            foreach (var game in dat.Game)
            {
                Assert.Null(game.ADDITIONAL_ATTRIBUTES);
                Assert.Null(game.ADDITIONAL_ELEMENTS);

                foreach (var biosset in game.BiosSet ?? Array.Empty<Models.Listxml.BiosSet>())
                {
                    Assert.Null(biosset.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(biosset.ADDITIONAL_ELEMENTS);
                }

                foreach (var rom in game.Rom ?? Array.Empty<Models.Listxml.Rom>())
                {
                    Assert.Null(rom.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(rom.ADDITIONAL_ELEMENTS);
                }

                foreach (var disk in game.Disk ?? Array.Empty<Models.Listxml.Disk>())
                {
                    Assert.Null(disk.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(disk.ADDITIONAL_ELEMENTS);
                }

                foreach (var deviceRef in game.DeviceRef ?? Array.Empty<Models.Listxml.DeviceRef>())
                {
                    Assert.Null(deviceRef.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(deviceRef.ADDITIONAL_ELEMENTS);
                }

                foreach (var sample in game.Sample ?? Array.Empty<Models.Listxml.Sample>())
                {
                    Assert.Null(sample.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(sample.ADDITIONAL_ELEMENTS);
                }

                foreach (var chip in game.Chip ?? Array.Empty<Models.Listxml.Chip>())
                {
                    Assert.Null(chip.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(chip.ADDITIONAL_ELEMENTS);
                }

                foreach (var display in game.Display ?? Array.Empty<Models.Listxml.Display>())
                {
                    Assert.Null(display.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(display.ADDITIONAL_ELEMENTS);
                }

                foreach (var video in game.Video ?? Array.Empty<Models.Listxml.Video>())
                {
                    Assert.Null(video.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(video.ADDITIONAL_ELEMENTS);
                }

                if (game.Sound != null)
                {
                    Assert.Null(game.Sound.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(game.Sound.ADDITIONAL_ELEMENTS);
                }

                if (game.Input != null)
                {
                    Assert.Null(game.Input.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(game.Input.ADDITIONAL_ELEMENTS);

                    foreach (var control in game.Input.Control ?? Array.Empty<Models.Listxml.Control>())
                    {
                        Assert.Null(control.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(control.ADDITIONAL_ELEMENTS);
                    }
                }

                foreach (var dipswitch in game.DipSwitch ?? Array.Empty<Models.Listxml.DipSwitch>())
                {
                    Assert.Null(dipswitch.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(dipswitch.ADDITIONAL_ELEMENTS);

                    if (dipswitch.Condition != null)
                    {
                        Assert.Null(dipswitch.Condition.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(dipswitch.Condition.ADDITIONAL_ELEMENTS);
                    }

                    foreach (var diplocation in dipswitch.DipLocation ?? Array.Empty<Models.Listxml.DipLocation>())
                    {
                        Assert.Null(diplocation.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(diplocation.ADDITIONAL_ELEMENTS);
                    }

                    foreach (var dipvalue in dipswitch.DipValue ?? Array.Empty<Models.Listxml.DipValue>())
                    {
                        Assert.Null(dipvalue.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(dipvalue.ADDITIONAL_ELEMENTS);

                        if (dipvalue.Condition != null)
                        {
                            Assert.Null(dipvalue.Condition.ADDITIONAL_ATTRIBUTES);
                            Assert.Null(dipvalue.Condition.ADDITIONAL_ELEMENTS);
                        }
                    }
                }

                foreach (var configuration in game.Configuration ?? Array.Empty<Models.Listxml.Configuration>())
                {
                    Assert.Null(configuration.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(configuration.ADDITIONAL_ELEMENTS);

                    if (configuration.Condition != null)
                    {
                        Assert.Null(configuration.Condition.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(configuration.Condition.ADDITIONAL_ELEMENTS);
                    }

                    foreach (var conflocation in configuration.ConfLocation ?? Array.Empty<Models.Listxml.ConfLocation>())
                    {
                        Assert.Null(conflocation.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(conflocation.ADDITIONAL_ELEMENTS);
                    }

                    foreach (var confsetting in configuration.ConfSetting ?? Array.Empty<Models.Listxml.ConfSetting>())
                    {
                        Assert.Null(confsetting.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(confsetting.ADDITIONAL_ELEMENTS);

                        if (confsetting.Condition != null)
                        {
                            Assert.Null(confsetting.Condition.ADDITIONAL_ATTRIBUTES);
                            Assert.Null(confsetting.Condition.ADDITIONAL_ELEMENTS);
                        }
                    }
                }

                foreach (var port in game.Port ?? Array.Empty<Models.Listxml.Port>())
                {
                    Assert.Null(port.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(port.ADDITIONAL_ELEMENTS);

                    foreach (var analog in port.Analog ?? Array.Empty<Models.Listxml.Analog>())
                    {
                        Assert.Null(analog.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(analog.ADDITIONAL_ELEMENTS);
                    }
                }

                foreach (var adjuster in game.Adjuster ?? Array.Empty<Models.Listxml.Adjuster>())
                {
                    Assert.Null(adjuster.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(adjuster.ADDITIONAL_ELEMENTS);

                    if (adjuster.Condition != null)
                    {
                        Assert.Null(adjuster.Condition.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(adjuster.Condition.ADDITIONAL_ELEMENTS);
                    }
                }

                if (game.Driver != null)
                {
                    Assert.Null(game.Driver.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(game.Driver.ADDITIONAL_ELEMENTS);
                }

                foreach (var feature in game.Feature ?? Array.Empty<Models.Listxml.Feature>())
                {
                    Assert.Null(feature.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(feature.ADDITIONAL_ELEMENTS);
                }

                foreach (var device in game.Device ?? Array.Empty<Models.Listxml.Device>())
                {
                    Assert.Null(device.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(device.ADDITIONAL_ELEMENTS);

                    if (device.Instance != null)
                    {
                        Assert.Null(device.Instance.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(device.Instance.ADDITIONAL_ELEMENTS);
                    }

                    foreach (var extension in device.Extension ?? Array.Empty<Models.Listxml.Extension>())
                    {
                        Assert.Null(extension.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(extension.ADDITIONAL_ELEMENTS);
                    }
                }

                foreach (var slot in game.Slot ?? Array.Empty<Models.Listxml.Slot>())
                {
                    Assert.Null(slot.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(slot.ADDITIONAL_ELEMENTS);

                    foreach (var slotoption in slot.SlotOption ?? Array.Empty<Models.Listxml.SlotOption>())
                    {
                        Assert.Null(slotoption.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(slotoption.ADDITIONAL_ELEMENTS);
                    }
                }

                foreach (var softwarelist in game.SoftwareList ?? Array.Empty<Models.Listxml.SoftwareList>())
                {
                    Assert.Null(softwarelist.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(softwarelist.ADDITIONAL_ELEMENTS);
                }

                foreach (var ramoption in game.RamOption ?? Array.Empty<Models.Listxml.RamOption>())
                {
                    Assert.Null(ramoption.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(ramoption.ADDITIONAL_ELEMENTS);
                }
            }
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

            // Validate we're not missing any attributes or elements
            Assert.Null(dat.ADDITIONAL_ATTRIBUTES);
            Assert.Null(dat.ADDITIONAL_ELEMENTS);
            if (dat.Header != null)
            {
                var header = dat.Header;
                Assert.Null(header.ADDITIONAL_ATTRIBUTES);
                Assert.Null(header.ADDITIONAL_ELEMENTS);

                if (header.ClrMamePro != null)
                {
                    var cmp = header.ClrMamePro;
                    Assert.Null(cmp.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(cmp.ADDITIONAL_ELEMENTS);
                }

                if (header.RomCenter != null)
                {
                    var rc = header.RomCenter;
                    Assert.Null(rc.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(rc.ADDITIONAL_ELEMENTS);
                }
            }

            foreach (var game in dat.Game)
            {
                Assert.Null(game.ADDITIONAL_ATTRIBUTES);
                Assert.Null(game.ADDITIONAL_ELEMENTS);

                foreach (var item in game.Release ?? Array.Empty<Models.Logiqx.Release>())
                {
                    Assert.Null(item.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(item.ADDITIONAL_ELEMENTS);
                }

                foreach (var item in game.BiosSet ?? Array.Empty<Models.Logiqx.BiosSet>())
                {
                    Assert.Null(item.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(item.ADDITIONAL_ELEMENTS);
                }

                foreach (var item in game.Rom ?? Array.Empty<Models.Logiqx.Rom>())
                {
                    Assert.Null(item.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(item.ADDITIONAL_ELEMENTS);
                }

                foreach (var item in game.Disk ?? Array.Empty<Models.Logiqx.Disk>())
                {
                    Assert.Null(item.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(item.ADDITIONAL_ELEMENTS);
                }

                foreach (var item in game.Media ?? Array.Empty<Models.Logiqx.Media>())
                {
                    Assert.Null(item.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(item.ADDITIONAL_ELEMENTS);
                }

                foreach (var item in game.DeviceRef ?? Array.Empty<Models.Logiqx.DeviceRef>())
                {
                    Assert.Null(item.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(item.ADDITIONAL_ELEMENTS);
                }

                foreach (var item in game.Sample ?? Array.Empty<Models.Logiqx.Sample>())
                {
                    Assert.Null(item.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(item.ADDITIONAL_ELEMENTS);
                }

                foreach (var item in game.Archive ?? Array.Empty<Models.Logiqx.Archive>())
                {
                    Assert.Null(item.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(item.ADDITIONAL_ELEMENTS);
                }

                if (game.Driver != null)
                {
                    Assert.Null(game.Driver.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(game.Driver.ADDITIONAL_ELEMENTS);
                }

                foreach (var item in game.SoftwareList ?? Array.Empty<Models.Logiqx.SoftwareList>())
                {
                    Assert.Null(item.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(item.ADDITIONAL_ELEMENTS);
                }

                if (game.Trurip != null)
                {
                    var trurip = game.Trurip;
                    Assert.Null(trurip.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(trurip.ADDITIONAL_ELEMENTS);
                }
            }

            foreach (var dir in dat.Dir ?? Array.Empty<Models.Logiqx.Dir>())
            {
                Assert.NotNull(dir.Game);
                foreach (var game in dir.Game)
                {
                    Assert.Null(game.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(game.ADDITIONAL_ELEMENTS);

                    foreach (var item in game.Release ?? Array.Empty<Models.Logiqx.Release>())
                    {
                        Assert.Null(item.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(item.ADDITIONAL_ELEMENTS);
                    }

                    foreach (var item in game.BiosSet ?? Array.Empty<Models.Logiqx.BiosSet>())
                    {
                        Assert.Null(item.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(item.ADDITIONAL_ELEMENTS);
                    }

                    foreach (var item in game.Rom ?? Array.Empty<Models.Logiqx.Rom>())
                    {
                        Assert.Null(item.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(item.ADDITIONAL_ELEMENTS);
                    }

                    foreach (var item in game.Disk ?? Array.Empty<Models.Logiqx.Disk>())
                    {
                        Assert.Null(item.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(item.ADDITIONAL_ELEMENTS);
                    }

                    foreach (var item in game.Media ?? Array.Empty<Models.Logiqx.Media>())
                    {
                        Assert.Null(item.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(item.ADDITIONAL_ELEMENTS);
                    }

                    foreach (var item in game.DeviceRef ?? Array.Empty<Models.Logiqx.DeviceRef>())
                    {
                        Assert.Null(item.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(item.ADDITIONAL_ELEMENTS);
                    }

                    foreach (var item in game.Sample ?? Array.Empty<Models.Logiqx.Sample>())
                    {
                        Assert.Null(item.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(item.ADDITIONAL_ELEMENTS);
                    }

                    foreach (var item in game.Archive ?? Array.Empty<Models.Logiqx.Archive>())
                    {
                        Assert.Null(item.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(item.ADDITIONAL_ELEMENTS);
                    }

                    if (game.Driver != null)
                    {
                        Assert.Null(game.Driver.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(game.Driver.ADDITIONAL_ELEMENTS);
                    }

                    foreach (var item in game.SoftwareList ?? Array.Empty<Models.Logiqx.SoftwareList>())
                    {
                        Assert.Null(item.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(item.ADDITIONAL_ELEMENTS);
                    }

                    if (game.Trurip != null)
                    {
                        var trurip = game.Trurip;
                        Assert.Null(trurip.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(trurip.ADDITIONAL_ELEMENTS);
                    }
                }
            }
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

            // Validate we're not missing any attributes or elements
            Assert.Null(dat.ADDITIONAL_ATTRIBUTES);
            Assert.Null(dat.ADDITIONAL_ELEMENTS);
            if (dat.Configuration != null)
            {
                var configuration = dat.Configuration;
                Assert.Null(configuration.ADDITIONAL_ATTRIBUTES);
                Assert.Null(configuration.ADDITIONAL_ELEMENTS);

                if (configuration.Infos != null)
                {
                    var infos = configuration.Infos;
                    Assert.Null(infos.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(infos.ADDITIONAL_ELEMENTS);

                    if (infos.Title != null)
                    {
                        var title = infos.Title;
                        Assert.Null(title.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(title.ADDITIONAL_ELEMENTS);
                    }

                    if (infos.Location != null)
                    {
                        var location = infos.Location;
                        Assert.Null(location.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(location.ADDITIONAL_ELEMENTS);
                    }

                    if (infos.Publisher != null)
                    {
                        var publisher = infos.Publisher;
                        Assert.Null(publisher.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(publisher.ADDITIONAL_ELEMENTS);
                    }

                    if (infos.SourceRom != null)
                    {
                        var sourceRom = infos.SourceRom;
                        Assert.Null(sourceRom.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(sourceRom.ADDITIONAL_ELEMENTS);
                    }

                    if (infos.SaveType != null)
                    {
                        var saveType = infos.SaveType;
                        Assert.Null(saveType.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(saveType.ADDITIONAL_ELEMENTS);
                    }

                    if (infos.RomSize != null)
                    {
                        var romSize = infos.RomSize;
                        Assert.Null(romSize.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(romSize.ADDITIONAL_ELEMENTS);
                    }

                    if (infos.ReleaseNumber != null)
                    {
                        var releaseNumber = infos.ReleaseNumber;
                        Assert.Null(releaseNumber.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(releaseNumber.ADDITIONAL_ELEMENTS);
                    }

                    if (infos.LanguageNumber != null)
                    {
                        var languageNumber = infos.LanguageNumber;
                        Assert.Null(languageNumber.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(languageNumber.ADDITIONAL_ELEMENTS);
                    }

                    if (infos.Comment != null)
                    {
                        var comment = infos.Comment;
                        Assert.Null(comment.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(comment.ADDITIONAL_ELEMENTS);
                    }

                    if (infos.RomCRC != null)
                    {
                        var romCRC = infos.RomCRC;
                        Assert.Null(romCRC.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(romCRC.ADDITIONAL_ELEMENTS);
                    }

                    if (infos.Im1CRC != null)
                    {
                        var im1CRC = infos.Im1CRC;
                        Assert.Null(im1CRC.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(im1CRC.ADDITIONAL_ELEMENTS);
                    }

                    if (infos.Im2CRC != null)
                    {
                        var im2CRC = infos.Im2CRC;
                        Assert.Null(im2CRC.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(im2CRC.ADDITIONAL_ELEMENTS);
                    }

                    if (infos.Languages != null)
                    {
                        var languages = infos.Languages;
                        Assert.Null(languages.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(languages.ADDITIONAL_ELEMENTS);
                    }
                }

                if (configuration.CanOpen != null)
                {
                    var canOpen = configuration.CanOpen;
                    Assert.Null(canOpen.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(canOpen.ADDITIONAL_ELEMENTS);
                }

                if (configuration.NewDat != null)
                {
                    var newDat = configuration.NewDat;
                    Assert.Null(newDat.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(newDat.ADDITIONAL_ELEMENTS);

                    if (newDat.DatUrl != null)
                    {
                        var datURL = newDat.DatUrl;
                        Assert.Null(datURL.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(datURL.ADDITIONAL_ELEMENTS);
                    }
                }

                if (configuration.Search != null)
                {
                    var search = configuration.Search;
                    Assert.Null(search.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(search.ADDITIONAL_ELEMENTS);

                    foreach (var to in search.To ?? Array.Empty<Models.OfflineList.To>())
                    {
                        Assert.Null(to.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(to.ADDITIONAL_ELEMENTS);

                        foreach (var find in to.Find ?? Array.Empty<Models.OfflineList.Find>())
                        {
                            Assert.Null(find.ADDITIONAL_ATTRIBUTES);
                            Assert.Null(find.ADDITIONAL_ELEMENTS);
                        }
                    }
                }
            }

            Assert.Null(dat.Games.ADDITIONAL_ATTRIBUTES);
            Assert.Null(dat.Games.ADDITIONAL_ELEMENTS);

            foreach (var game in dat.Games.Game)
            {
                Assert.Null(game.ADDITIONAL_ATTRIBUTES);
                //Assert.Null(game.ADDITIONAL_ELEMENTS); // TODO: Re-enable line when Models is fixed again

                if (game.Files != null)
                {
                    var files = game.Files;
                    Assert.Null(files.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(files.ADDITIONAL_ELEMENTS);

                    foreach (var romCRC in files.RomCRC ?? Array.Empty<Models.OfflineList.FileRomCRC>())
                    {
                        Assert.Null(romCRC.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(romCRC.ADDITIONAL_ELEMENTS);
                    }
                }
            }

            if (dat.GUI != null)
            {
                var gui = dat.GUI;
                Assert.Null(gui.ADDITIONAL_ATTRIBUTES);
                Assert.Null(gui.ADDITIONAL_ELEMENTS);

                if (gui.Images != null)
                {
                    var images = gui.Images;
                    Assert.Null(images.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(images.ADDITIONAL_ELEMENTS);

                    foreach (var image in images.Image ?? Array.Empty<Models.OfflineList.Image>())
                    {
                        Assert.Null(image.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(image.ADDITIONAL_ELEMENTS);
                    }
                }
            }
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

            // Validate we're not missing any attributes or elements
            Assert.Null(dat.ADDITIONAL_ATTRIBUTES);
            Assert.Null(dat.ADDITIONAL_ELEMENTS);
            foreach (var software in dat.Software)
            {
                Assert.Null(software.ADDITIONAL_ATTRIBUTES);
                Assert.Null(software.ADDITIONAL_ELEMENTS);

                foreach (var dump in software.Dump ?? Array.Empty<Models.OpenMSX.Dump>())
                {
                    Assert.Null(dump.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(dump.ADDITIONAL_ELEMENTS);

                    if (dump.Original != null)
                    {
                        Assert.Null(dump.Original.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(dump.Original.ADDITIONAL_ELEMENTS);
                    }

                    if (dump.Rom != null)
                    {
                        Assert.Null(dump.Rom.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(dump.Rom.ADDITIONAL_ELEMENTS);
                    }
                }
            }
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

            // Validate we're not missing any attributes or elements
            Assert.NotNull(dat.ADDITIONAL_ELEMENTS);
            Assert.Empty(dat.ADDITIONAL_ELEMENTS);
            if (dat.Credits != null)
            {
                Assert.NotNull(dat.Credits.ADDITIONAL_ELEMENTS);
                Assert.Empty(dat.Credits.ADDITIONAL_ELEMENTS);
            }

            if (dat.Dat != null)
            {
                Assert.NotNull(dat.Dat.ADDITIONAL_ELEMENTS);
                Assert.Empty(dat.Dat.ADDITIONAL_ELEMENTS);
            }

            if (dat.Emulator != null)
            {
                Assert.NotNull(dat.Emulator.ADDITIONAL_ELEMENTS);
                Assert.Empty(dat.Emulator.ADDITIONAL_ELEMENTS);
            }

            if (dat.Games != null)
            {
                Assert.NotNull(dat.Games.ADDITIONAL_ELEMENTS);
                Assert.Empty(dat.Games.ADDITIONAL_ELEMENTS);
                foreach (var rom in dat.Games.Rom ?? Array.Empty<Models.RomCenter.Rom>())
                {
                    Assert.Null(rom.ADDITIONAL_ELEMENTS);
                }
            }
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

            // Validate we're not missing any attributes or elements
            foreach (var rom in dat.Row ?? Array.Empty<Models.SeparatedValue.Row>())
            {
                Assert.Null(rom.ADDITIONAL_ELEMENTS);
            }
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

            // Validate we're not missing any attributes or elements
            Assert.Null(dat.ADDITIONAL_ATTRIBUTES);
            Assert.Null(dat.ADDITIONAL_ELEMENTS);
            foreach (var software in dat.Software)
            {
                Assert.Null(software.ADDITIONAL_ATTRIBUTES);
                Assert.Null(software.ADDITIONAL_ELEMENTS);

                foreach (var info in software.Info ?? Array.Empty<Models.SoftwareList.Info>())
                {
                    Assert.Null(info.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(info.ADDITIONAL_ELEMENTS);
                }

                foreach (var sharedfeat in software.SharedFeat ?? Array.Empty<Models.SoftwareList.SharedFeat>())
                {
                    Assert.Null(sharedfeat.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(sharedfeat.ADDITIONAL_ELEMENTS);
                }

                foreach (var part in software.Part ?? Array.Empty<Models.SoftwareList.Part>())
                {
                    Assert.Null(part.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(part.ADDITIONAL_ELEMENTS);

                    foreach (var feature in part.Feature ?? Array.Empty<Models.SoftwareList.Feature>())
                    {
                        Assert.Null(feature.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(feature.ADDITIONAL_ELEMENTS);
                    }

                    foreach (var dataarea in part.DataArea ?? Array.Empty<Models.SoftwareList.DataArea>())
                    {
                        Assert.Null(dataarea.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(dataarea.ADDITIONAL_ELEMENTS);

                        foreach (var rom in dataarea.Rom ?? Array.Empty<Models.SoftwareList.Rom>())
                        {
                            Assert.Null(rom.ADDITIONAL_ATTRIBUTES);
                            Assert.Null(rom.ADDITIONAL_ELEMENTS);
                        }
                    }

                    foreach (var diskarea in part.DiskArea ?? Array.Empty<Models.SoftwareList.DiskArea>())
                    {
                        Assert.Null(diskarea.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(diskarea.ADDITIONAL_ELEMENTS);

                        foreach (var disk in diskarea.Disk ?? Array.Empty<Models.SoftwareList.Disk>())
                        {
                            Assert.Null(disk.ADDITIONAL_ATTRIBUTES);
                            Assert.Null(disk.ADDITIONAL_ELEMENTS);
                        }
                    }

                    foreach (var dipswitch in part.DipSwitch ?? Array.Empty<Models.SoftwareList.DipSwitch>())
                    {
                        Assert.Null(dipswitch.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(dipswitch.ADDITIONAL_ELEMENTS);

                        foreach (var dipvalue in dipswitch.DipValue ?? Array.Empty<Models.SoftwareList.DipValue>())
                        {
                            Assert.Null(dipvalue.ADDITIONAL_ATTRIBUTES);
                            Assert.Null(dipvalue.ADDITIONAL_ELEMENTS);
                        }
                    }
                }
            }
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
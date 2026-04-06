using System;
using SabreTools.Data.Models.Metadata;
using Xunit;

namespace SabreTools.Metadata.Filter.Test
{
    public class FilterObjectTests
    {
        #region Constructor

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("Sample.Name")]
        [InlineData("Sample.Name++")]
        public void Constructor_InvalidKey_Throws(string? filterString)
        {
            Assert.Throws<ArgumentException>(() => new FilterObject(filterString));
        }

        [Theory]
        [InlineData("Sample.Name=XXXXXX", Operation.Equals)]
        [InlineData("Sample.Name==XXXXXX", Operation.Equals)]
        [InlineData("Sample.Name:XXXXXX", Operation.Equals)]
        [InlineData("Sample.Name::XXXXXX", Operation.Equals)]
        [InlineData("Sample.Name!XXXXXX", Operation.NotEquals)]
        [InlineData("Sample.Name!=XXXXXX", Operation.NotEquals)]
        [InlineData("Sample.Name!:XXXXXX", Operation.NotEquals)]
        [InlineData("Sample.Name>XXXXXX", Operation.GreaterThan)]
        [InlineData("Sample.Name>=XXXXXX", Operation.GreaterThanOrEqual)]
        [InlineData("Sample.Name<XXXXXX", Operation.LessThan)]
        [InlineData("Sample.Name<=XXXXXX", Operation.LessThanOrEqual)]
        [InlineData("Sample.Name:!XXXXXX", Operation.NONE)]
        [InlineData("Sample.Name=>XXXXXX", Operation.NONE)]
        [InlineData("Sample.Name=<XXXXXX", Operation.NONE)]
        [InlineData("Sample.Name<>XXXXXX", Operation.NONE)]
        [InlineData("Sample.Name><XXXXXX", Operation.NONE)]
        public void Constructor_FilterString(string filterString, Operation expected)
        {
            FilterObject filterObject = new FilterObject(filterString);
            Assert.Equal("sample", filterObject.Key.ItemName);
            Assert.Equal("name", filterObject.Key.FieldName);
            Assert.Equal("XXXXXX", filterObject.Value);
            Assert.Equal(expected, filterObject.Operation);
        }

        [Theory]
        [InlineData("Sample.Name", "XXXXXX", "=", Operation.Equals)]
        [InlineData("Sample.Name", "XXXXXX", "==", Operation.Equals)]
        [InlineData("Sample.Name", "XXXXXX", ":", Operation.Equals)]
        [InlineData("Sample.Name", "XXXXXX", "::", Operation.Equals)]
        [InlineData("Sample.Name", "XXXXXX", "!", Operation.NotEquals)]
        [InlineData("Sample.Name", "XXXXXX", "!=", Operation.NotEquals)]
        [InlineData("Sample.Name", "XXXXXX", "!:", Operation.NotEquals)]
        [InlineData("Sample.Name", "XXXXXX", ">", Operation.GreaterThan)]
        [InlineData("Sample.Name", "XXXXXX", ">=", Operation.GreaterThanOrEqual)]
        [InlineData("Sample.Name", "XXXXXX", "<", Operation.LessThan)]
        [InlineData("Sample.Name", "XXXXXX", "<=", Operation.LessThanOrEqual)]
        [InlineData("Sample.Name", "XXXXXX", "@@", Operation.NONE)]
        [InlineData("Sample.Name", "XXXXXX", ":!", Operation.NONE)]
        [InlineData("Sample.Name", "XXXXXX", "=>", Operation.NONE)]
        [InlineData("Sample.Name", "XXXXXX", "=<", Operation.NONE)]
        public void Constructor_TripleString(string itemField, string? value, string? operation, Operation expected)
        {
            FilterObject filterObject = new FilterObject(itemField, value, operation);
            Assert.Equal("sample", filterObject.Key.ItemName);
            Assert.Equal("name", filterObject.Key.FieldName);
            Assert.Equal("XXXXXX", filterObject.Value);
            Assert.Equal(expected, filterObject.Operation);
        }

        #endregion

        #region MatchesEqual

        [Fact]
        public void MatchesEqual_NoKey()
        {
            var sample = new Sample();
            FilterObject filterObject = new FilterObject("Sample.Name", null, Operation.Equals);
            bool actual = filterObject.Matches(sample);
            Assert.True(actual);
        }

        [Fact]
        public void MatchesEqual_NoValue()
        {
            var sample = new Sample { Name = null };
            FilterObject filterObject = new FilterObject("Sample.Name", null, Operation.Equals);
            bool actual = filterObject.Matches(sample);
            Assert.True(actual);
        }

        [Fact]
        public void MatchesEqual_BoolValue()
        {
            var sample = new Sample { Name = "true" };
            FilterObject filterObject = new FilterObject("Sample.Name", "yes", Operation.Equals);
            bool actual = filterObject.Matches(sample);
            Assert.True(actual);
        }

        [Fact]
        public void MatchesEqual_Int64Value()
        {
            var sample = new Sample { Name = "12345" };
            FilterObject filterObject = new FilterObject("Sample.Name", "12345", Operation.Equals);
            bool actual = filterObject.Matches(sample);
            Assert.True(actual);
        }

        [Fact]
        public void MatchesEqual_DoubleValue()
        {
            var sample = new Sample { Name = "12.345" };
            FilterObject filterObject = new FilterObject("Sample.Name", "12.345", Operation.Equals);
            bool actual = filterObject.Matches(sample);
            Assert.True(actual);
        }

        [Fact]
        public void MatchesEqual_RegexValue()
        {
            var sample = new Sample { Name = "name" };
            FilterObject filterObject = new FilterObject("Sample.Name", "^name$", Operation.Equals);
            bool actual = filterObject.Matches(sample);
            Assert.True(actual);
        }

        [Fact]
        public void MatchesEqual_StringValue()
        {
            var sample = new Sample { Name = "name" };
            FilterObject filterObject = new FilterObject("Sample.Name", "name", Operation.Equals);
            bool actual = filterObject.Matches(sample);
            Assert.True(actual);
        }

        #endregion

        #region MatchesNotEqual

        [Fact]
        public void MatchesNotEqual_NoKey()
        {
            var sample = new Sample();
            FilterObject filterObject = new FilterObject("Sample.Name", null, Operation.NotEquals);
            bool actual = filterObject.Matches(sample);
            Assert.False(actual);
        }

        [Fact]
        public void MatchesNotEqual_NoValue()
        {
            var sample = new Sample { Name = null };
            FilterObject filterObject = new FilterObject("Sample.Name", null, Operation.NotEquals);
            bool actual = filterObject.Matches(sample);
            Assert.False(actual);
        }

        [Fact]
        public void MatchesNotEqual_BoolValue()
        {
            var sample = new Sample { Name = "true" };
            FilterObject filterObject = new FilterObject("Sample.Name", "yes", Operation.NotEquals);
            bool actual = filterObject.Matches(sample);
            Assert.False(actual);
        }

        [Fact]
        public void MatchesNotEqual_Int64Value()
        {
            var sample = new Sample { Name = "12345" };
            FilterObject filterObject = new FilterObject("Sample.Name", "12345", Operation.NotEquals);
            bool actual = filterObject.Matches(sample);
            Assert.False(actual);
        }

        [Fact]
        public void MatchesNotEqual_DoubleValue()
        {
            var sample = new Sample { Name = "12.345" };
            FilterObject filterObject = new FilterObject("Sample.Name", "12.345", Operation.NotEquals);
            bool actual = filterObject.Matches(sample);
            Assert.False(actual);
        }

        [Fact]
        public void MatchesNotEqual_RegexValue()
        {
            var sample = new Sample { Name = "name" };
            FilterObject filterObject = new FilterObject("Sample.Name", "^name$", Operation.NotEquals);
            bool actual = filterObject.Matches(sample);
            Assert.False(actual);
        }

        [Fact]
        public void MatchesNotEqual_StringValue()
        {
            var sample = new Sample { Name = "name" };
            FilterObject filterObject = new FilterObject("Sample.Name", "name", Operation.NotEquals);
            bool actual = filterObject.Matches(sample);
            Assert.False(actual);
        }

        #endregion

        #region MatchesGreaterThan

        [Fact]
        public void MatchesGreaterThan_NoKey()
        {
            var sample = new Sample();
            FilterObject filterObject = new FilterObject("Sample.Name", null, Operation.GreaterThan);
            bool actual = filterObject.Matches(sample);
            Assert.False(actual);
        }

        [Fact]
        public void MatchesGreaterThan_NoValue()
        {
            var sample = new Sample { Name = null };
            FilterObject filterObject = new FilterObject("Sample.Name", null, Operation.GreaterThan);
            bool actual = filterObject.Matches(sample);
            Assert.False(actual);
        }

        [Fact]
        public void MatchesGreaterThan_BoolValue()
        {
            var sample = new Sample { Name = "true" };
            FilterObject filterObject = new FilterObject("Sample.Name", "yes", Operation.GreaterThan);
            bool actual = filterObject.Matches(sample);
            Assert.False(actual);
        }

        [Fact]
        public void MatchesGreaterThan_Int64Value()
        {
            var sample = new Sample { Name = "12346" };
            FilterObject filterObject = new FilterObject("Sample.Name", "12345", Operation.GreaterThan);
            bool actual = filterObject.Matches(sample);
            Assert.True(actual);
        }

        [Fact]
        public void MatchesGreaterThan_DoubleValue()
        {
            var sample = new Sample { Name = "12.346" };
            FilterObject filterObject = new FilterObject("Sample.Name", "12.345", Operation.GreaterThan);
            bool actual = filterObject.Matches(sample);
            Assert.True(actual);
        }

        [Fact]
        public void MatchesGreaterThan_RegexValue()
        {
            var sample = new Sample { Name = "name" };
            FilterObject filterObject = new FilterObject("Sample.Name", "^XXXXXX$", Operation.GreaterThan);
            bool actual = filterObject.Matches(sample);
            Assert.False(actual);
        }

        [Fact]
        public void MatchesGreaterThan_StringValue()
        {
            var sample = new Sample { Name = "name" };
            FilterObject filterObject = new FilterObject("Sample.Name", "XXXXXX", Operation.GreaterThan);
            bool actual = filterObject.Matches(sample);
            Assert.False(actual);
        }

        #endregion

        #region MatchesGreaterThanOrEqual

        [Fact]
        public void MatchesGreaterThanOrEqual_NoKey()
        {
            var sample = new Sample();
            FilterObject filterObject = new FilterObject("Sample.Name", null, Operation.GreaterThanOrEqual);
            bool actual = filterObject.Matches(sample);
            Assert.False(actual);
        }

        [Fact]
        public void MatchesGreaterThanOrEqual_NoValue()
        {
            var sample = new Sample { Name = null };
            FilterObject filterObject = new FilterObject("Sample.Name", null, Operation.GreaterThanOrEqual);
            bool actual = filterObject.Matches(sample);
            Assert.False(actual);
        }

        [Fact]
        public void MatchesGreaterThanOrEqual_BoolValue()
        {
            var sample = new Sample { Name = "true" };
            FilterObject filterObject = new FilterObject("Sample.Name", "yes", Operation.GreaterThanOrEqual);
            bool actual = filterObject.Matches(sample);
            Assert.False(actual);
        }

        [Fact]
        public void MatchesGreaterThanOrEqual_Int64Value()
        {
            var sample = new Sample { Name = "12346" };
            FilterObject filterObject = new FilterObject("Sample.Name", "12345", Operation.GreaterThanOrEqual);
            bool actual = filterObject.Matches(sample);
            Assert.True(actual);
        }

        [Fact]
        public void MatchesGreaterThanOrEqual_DoubleValue()
        {
            var sample = new Sample { Name = "12.346" };
            FilterObject filterObject = new FilterObject("Sample.Name", "12.345", Operation.GreaterThanOrEqual);
            bool actual = filterObject.Matches(sample);
            Assert.True(actual);
        }

        [Fact]
        public void MatchesGreaterThanOrEqual_RegexValue()
        {
            var sample = new Sample { Name = "name" };
            FilterObject filterObject = new FilterObject("Sample.Name", "^XXXXXX$", Operation.GreaterThanOrEqual);
            bool actual = filterObject.Matches(sample);
            Assert.False(actual);
        }

        [Fact]
        public void MatchesGreaterThanOrEqual_StringValue()
        {
            var sample = new Sample { Name = "name" };
            FilterObject filterObject = new FilterObject("Sample.Name", "XXXXXX", Operation.GreaterThanOrEqual);
            bool actual = filterObject.Matches(sample);
            Assert.False(actual);
        }

        #endregion

        #region MatchesLessThan

        [Fact]
        public void MatchesLessThan_NoKey()
        {
            var sample = new Sample();
            FilterObject filterObject = new FilterObject("Sample.Name", null, Operation.LessThan);
            bool actual = filterObject.Matches(sample);
            Assert.False(actual);
        }

        [Fact]
        public void MatchesLessThan_NoValue()
        {
            var sample = new Sample { Name = null };
            FilterObject filterObject = new FilterObject("Sample.Name", null, Operation.LessThan);
            bool actual = filterObject.Matches(sample);
            Assert.False(actual);
        }

        [Fact]
        public void MatchesLessThan_BoolValue()
        {
            var sample = new Sample { Name = "true" };
            FilterObject filterObject = new FilterObject("Sample.Name", "yes", Operation.LessThan);
            bool actual = filterObject.Matches(sample);
            Assert.False(actual);
        }

        [Fact]
        public void MatchesLessThan_Int64Value()
        {
            var sample = new Sample { Name = "12344" };
            FilterObject filterObject = new FilterObject("Sample.Name", "12345", Operation.LessThan);
            bool actual = filterObject.Matches(sample);
            Assert.True(actual);
        }

        [Fact]
        public void MatchesLessThan_DoubleValue()
        {
            var sample = new Sample { Name = "12.344" };
            FilterObject filterObject = new FilterObject("Sample.Name", "12.345", Operation.LessThan);
            bool actual = filterObject.Matches(sample);
            Assert.True(actual);
        }

        [Fact]
        public void MatchesLessThan_RegexValue()
        {
            var sample = new Sample { Name = "name" };
            FilterObject filterObject = new FilterObject("Sample.Name", "^XXXXXX$", Operation.LessThan);
            bool actual = filterObject.Matches(sample);
            Assert.False(actual);
        }

        [Fact]
        public void MatchesLessThan_StringValue()
        {
            var sample = new Sample { Name = "name" };
            FilterObject filterObject = new FilterObject("Sample.Name", "XXXXXX", Operation.LessThan);
            bool actual = filterObject.Matches(sample);
            Assert.False(actual);
        }

        #endregion

        #region MatchesLessThanOrEqual

        [Fact]
        public void MatchesLessThanOrEqual_NoKey()
        {
            var sample = new Sample();
            FilterObject filterObject = new FilterObject("Sample.Name", null, Operation.LessThanOrEqual);
            bool actual = filterObject.Matches(sample);
            Assert.False(actual);
        }

        [Fact]
        public void MatchesLessThanOrEqual_NoValue()
        {
            var sample = new Sample { Name = null };
            FilterObject filterObject = new FilterObject("Sample.Name", null, Operation.LessThanOrEqual);
            bool actual = filterObject.Matches(sample);
            Assert.False(actual);
        }

        [Fact]
        public void MatchesLessThanOrEqual_BoolValue()
        {
            var sample = new Sample { Name = "true" };
            FilterObject filterObject = new FilterObject("Sample.Name", "yes", Operation.LessThanOrEqual);
            bool actual = filterObject.Matches(sample);
            Assert.False(actual);
        }

        [Fact]
        public void MatchesLessThanOrEqual_Int64Value()
        {
            var sample = new Sample { Name = "12344" };
            FilterObject filterObject = new FilterObject("Sample.Name", "12345", Operation.LessThanOrEqual);
            bool actual = filterObject.Matches(sample);
            Assert.True(actual);
        }

        [Fact]
        public void MatchesLessThanOrEqual_DoubleValue()
        {
            var sample = new Sample { Name = "12.344" };
            FilterObject filterObject = new FilterObject("Sample.Name", "12.345", Operation.LessThanOrEqual);
            bool actual = filterObject.Matches(sample);
            Assert.True(actual);
        }

        [Fact]
        public void MatchesLessThanOrEqual_RegexValue()
        {
            var sample = new Sample { Name = "name" };
            FilterObject filterObject = new FilterObject("Sample.Name", "^XXXXXX$", Operation.LessThanOrEqual);
            bool actual = filterObject.Matches(sample);
            Assert.False(actual);
        }

        [Fact]
        public void MatchesLessThanOrEqual_StringValue()
        {
            var sample = new Sample { Name = "name" };
            FilterObject filterObject = new FilterObject("Sample.Name", "XXXXXX", Operation.LessThanOrEqual);
            bool actual = filterObject.Matches(sample);
            Assert.False(actual);
        }

        #endregion

        #region Adjuster

        [Theory]
        [InlineData("adjuster.default", "yes")]
        [InlineData("adjuster.name", "name")]
        public void Matches_Adjsuter(string itemField, string value)
        {
            var filter = new FilterObject(itemField, value, Operation.Equals);
            Adjuster obj = new Adjuster
            {
                Default = true,
                Name = "name",
            };

            bool actual = filter.Matches(obj);
            Assert.True(actual);
        }

        #endregion

        #region Analog

        [Theory]
        [InlineData("analog.mask", "mask")]
        public void Matches_Analog(string itemField, string value)
        {
            var filter = new FilterObject(itemField, value, Operation.Equals);
            Analog obj = new Analog
            {
                Mask = "mask",
            };

            bool actual = filter.Matches(obj);
            Assert.True(actual);
        }

        #endregion

        #region Archive

        [Theory]
        [InlineData("archive.additional", "additional")]
        [InlineData("archive.adult", "1")]
        [InlineData("archive.alt", "1")]
        [InlineData("archive.bios", "1")]
        [InlineData("archive.categories", "categories")]
        [InlineData("archive.clone", "clone")]
        [InlineData("archive.clonetag", "clone")]
        [InlineData("archive.complete", "1")]
        [InlineData("archive.dat", "1")]
        [InlineData("archive.datternote", "datternote")]
        [InlineData("archive.description", "description")]
        [InlineData("archive.devstatus", "devstatus")]
        [InlineData("archive.gameid1", "gameid1")]
        [InlineData("archive.gameid2", "gameid2")]
        [InlineData("archive.langchecked", "langchecked")]
        [InlineData("archive.languages", "languages")]
        [InlineData("archive.licensed", "1")]
        [InlineData("archive.listed", "1")]
        [InlineData("archive.mergeof", "mergeof")]
        [InlineData("archive.mergename", "mergename")]
        [InlineData("archive.name", "name")]
        [InlineData("archive.namealt", "namealt")]
        [InlineData("archive.number", "number")]
        [InlineData("archive.physical", "1")]
        [InlineData("archive.pirate", "1")]
        [InlineData("archive.private", "1")]
        [InlineData("archive.region", "region")]
        [InlineData("archive.regparent", "regparent")]
        [InlineData("archive.showlang", "1")]
        [InlineData("archive.special1", "special1")]
        [InlineData("archive.special2", "special2")]
        [InlineData("archive.stickynote", "stickynote")]
        [InlineData("archive.version1", "version1")]
        [InlineData("archive.version2", "version2")]
        public void Matches_Archive(string itemField, string value)
        {
            var filter = new FilterObject(itemField, value, Operation.Equals);
            Archive obj = new Archive
            {
                Additional = "additional",
                Adult = true,
                Alt = true,
                Bios = true,
                Categories = "categories",
                CloneTag = "clone",
                Complete = true,
                Dat = true,
                DatterNote = "datternote",
                Description = "description",
                DevStatus = "devstatus",
                GameId1 = "gameid1",
                GameId2 = "gameid2",
                LangChecked = "langchecked",
                Languages = "languages",
                Licensed = true,
                Listed = true,
                MergeOf = "mergeof",
                MergeName = "mergename",
                Name = "name",
                NameAlt = "namealt",
                Number = "number",
                Physical = true,
                Pirate = true,
                Private = true,
                Region = "region",
                RegParent = "regparent",
                ShowLang = true,
                Special1 = "special1",
                Special2 = "special2",
                StickyNote = "stickynote",
                Version1 = "version1",
                Version2 = "version2",
            };

            bool actual = filter.Matches(obj);
            Assert.True(actual);
        }

        #endregion

        #region BiosSet

        [Theory]
        [InlineData("biosset.default", "yes")]
        [InlineData("biosset.description", "description")]
        [InlineData("biosset.name", "name")]
        public void Matches_BiosSet(string itemField, string value)
        {
            var filter = new FilterObject(itemField, value, Operation.Equals);
            BiosSet obj = new BiosSet
            {
                Default = true,
                Description = "description",
                Name = "name",
            };

            bool actual = filter.Matches(obj);
            Assert.True(actual);
        }

        #endregion

        #region Chip

        [Theory]
        [InlineData("chip.chiptype", "audio")]
        [InlineData("chip.clock", "12345")]
        [InlineData("chip.flags", "flags")]
        [InlineData("chip.name", "name")]
        [InlineData("chip.soundonly", "yes")]
        [InlineData("chip.tag", "tag")]
        public void Matches_Chip(string itemField, string value)
        {
            var filter = new FilterObject(itemField, value, Operation.Equals);
            Chip obj = new Chip
            {
                ChipType = ChipType.Audio,
                Clock = 12345,
                Flags = "flags",
                Name = "name",
                SoundOnly = true,
                Tag = "tag",
            };

            bool actual = filter.Matches(obj);
            Assert.True(actual);
        }

        #endregion

        #region Condition

        [Theory]
        [InlineData("condition.mask", "mask")]
        [InlineData("condition.relation", "ne")]
        [InlineData("condition.tag", "tag")]
        [InlineData("condition.value", "value")]
        public void Matches_Condition(string itemField, string value)
        {
            var filter = new FilterObject(itemField, value, Operation.Equals);
            Condition obj = new Condition
            {
                Mask = "mask",
                Relation = Relation.NotEqual,
                Tag = "tag",
                Value = "value",
            };

            bool actual = filter.Matches(obj);
            Assert.True(actual);
        }

        #endregion

        #region Configuration

        [Theory]
        [InlineData("configuration.mask", "mask")]
        [InlineData("configuration.name", "name")]
        [InlineData("configuration.tag", "tag")]
        public void Matches_Configuration(string itemField, string value)
        {
            var filter = new FilterObject(itemField, value, Operation.Equals);
            Configuration obj = new Configuration
            {
                Mask = "mask",
                Name = "name",
                Tag = "tag",
            };

            bool actual = filter.Matches(obj);
            Assert.True(actual);
        }

        #endregion

        #region ConfLocation

        [Theory]
        [InlineData("conflocation.inverted", "yes")]
        [InlineData("conflocation.name", "name")]
        [InlineData("conflocation.number", "12345")]
        public void Matches_ConfLocation(string itemField, string value)
        {
            var filter = new FilterObject(itemField, value, Operation.Equals);
            ConfLocation obj = new ConfLocation
            {
                Inverted = true,
                Name = "name",
                Number = 12345,
            };

            bool actual = filter.Matches(obj);
            Assert.True(actual);
        }

        #endregion

        #region ConfSetting

        [Theory]
        [InlineData("confsetting.default", "yes")]
        [InlineData("confsetting.name", "name")]
        [InlineData("confsetting.value", "value")]
        public void Matches_ConfSetting(string itemField, string value)
        {
            var filter = new FilterObject(itemField, value, Operation.Equals);
            ConfSetting obj = new ConfSetting
            {
                Default = true,
                Name = "name",
                Value = "value",
            };

            bool actual = filter.Matches(obj);
            Assert.True(actual);
        }

        #endregion

        #region Control

        [Theory]
        [InlineData("control.buttons", "12345")]
        [InlineData("control.controltype", "lightgun")]
        [InlineData("control.keydelta", "12345")]
        [InlineData("control.maximum", "12345")]
        [InlineData("control.minimum", "12345")]
        [InlineData("control.player", "12345")]
        [InlineData("control.reqbuttons", "12345")]
        [InlineData("control.reverse", "yes")]
        [InlineData("control.sensitivity", "12345")]
        [InlineData("control.ways", "ways")]
        [InlineData("control.ways2", "ways2")]
        [InlineData("control.ways3", "ways3")]
        public void Matches_Control(string itemField, string value)
        {
            var filter = new FilterObject(itemField, value, Operation.Equals);
            Control obj = new Control
            {
                Buttons = 12345,
                ControlType = ControlType.Lightgun,
                KeyDelta = 12345,
                Maximum = 12345,
                Minimum = 12345,
                Player = 12345,
                ReqButtons = 12345,
                Reverse = true,
                Sensitivity = 12345,
                Ways = "ways",
                Ways2 = "ways2",
                Ways3 = "ways3",
            };

            bool actual = filter.Matches(obj);
            Assert.True(actual);
        }

        #endregion

        #region DataArea

        [Theory]
        [InlineData("dataarea.endianness", "big")]
        [InlineData("dataarea.name", "name")]
        [InlineData("dataarea.size", "12345")]
        [InlineData("dataarea.width", "16")]
        public void Matches_DataArea(string itemField, string value)
        {
            var filter = new FilterObject(itemField, value, Operation.Equals);
            DataArea obj = new DataArea
            {
                Endianness = Endianness.Big,
                Name = "name",
                Size = 12345,
                Width = Width.Short,
            };

            bool actual = filter.Matches(obj);
            Assert.True(actual);
        }

        #endregion

        #region Device

        [Theory]
        [InlineData("device.devicetype", "punchcard")]
        [InlineData("device.fixedimage", "fixedimage")]
        [InlineData("device.interface", "interface")]
        [InlineData("device.mandatory", "yes")]
        [InlineData("device.tag", "tag")]
        public void Matches_Device(string itemField, string value)
        {
            var filter = new FilterObject(itemField, value, Operation.Equals);
            Device obj = new Device
            {
                DeviceType = DeviceType.PunchCard,
                FixedImage = "fixedimage",
                Interface = "interface",
                Mandatory = true,
                Tag = "tag",
            };

            bool actual = filter.Matches(obj);
            Assert.True(actual);
        }

        #endregion

        #region Device

        [Theory]
        [InlineData("deviceref.name", "name")]
        public void Matches_DeviceRef(string itemField, string value)
        {
            var filter = new FilterObject(itemField, value, Operation.Equals);
            DeviceRef obj = new DeviceRef
            {
                Name = "name",
            };

            bool actual = filter.Matches(obj);
            Assert.True(actual);
        }

        #endregion

        #region DipLocation

        [Theory]
        [InlineData("diplocation.inverted", "yes")]
        [InlineData("diplocation.name", "name")]
        [InlineData("diplocation.number", "12345")]
        public void Matches_DipLocation(string itemField, string value)
        {
            var filter = new FilterObject(itemField, value, Operation.Equals);
            DipLocation obj = new DipLocation
            {
                Inverted = true,
                Name = "name",
                Number = 12345,
            };

            bool actual = filter.Matches(obj);
            Assert.True(actual);
        }

        #endregion

        #region DipSwitch

        [Theory]
        [InlineData("dipswitch.default", "yes")]
        [InlineData("dipswitch.mask", "mask")]
        [InlineData("dipswitch.name", "name")]
        [InlineData("dipswitch.tag", "tag")]
        public void Matches_DipSwitch(string itemField, string value)
        {
            var filter = new FilterObject(itemField, value, Operation.Equals);
            DipSwitch obj = new DipSwitch
            {
                Default = true,
                Mask = "mask",
                Name = "name",
                Tag = "tag",
            };

            bool actual = filter.Matches(obj);
            Assert.True(actual);
        }

        #endregion

        #region DipValue

        [Theory]
        [InlineData("dipvalue.default", "yes")]
        [InlineData("dipvalue.name", "name")]
        [InlineData("dipvalue.value", "value")]
        public void Matches_DipValue(string itemField, string value)
        {
            var filter = new FilterObject(itemField, value, Operation.Equals);
            DipValue obj = new DipValue
            {
                Default = true,
                Name = "name",
                Value = "value",
            };

            bool actual = filter.Matches(obj);
            Assert.True(actual);
        }

        #endregion

        #region Disk

        [Theory]
        [InlineData("disk.flags", "flags")]
        [InlineData("disk.index", "12345")]
        [InlineData("disk.md5", "md5")]
        [InlineData("disk.merge", "merge")]
        [InlineData("disk.name", "name")]
        [InlineData("disk.optional", "yes")]
        [InlineData("disk.region", "region")]
        [InlineData("disk.sha1", "sha1")]
        [InlineData("disk.status", "nodump")]
        [InlineData("disk.writable", "yes")]
        public void Matches_Disk(string itemField, string value)
        {
            var filter = new FilterObject(itemField, value, Operation.Equals);
            Disk obj = new Disk
            {
                Flags = "flags",
                Index = 12345,
                MD5 = "md5",
                Merge = "merge",
                Name = "name",
                Optional = true,
                Region = "region",
                SHA1 = "sha1",
                Status = ItemStatus.Nodump,
                Writable = true,
            };

            bool actual = filter.Matches(obj);
            Assert.True(actual);
        }

        #endregion

        #region DiskArea

        [Theory]
        [InlineData("diskarea.name", "name")]
        public void Matches_DiskArea(string itemField, string value)
        {
            var filter = new FilterObject(itemField, value, Operation.Equals);
            DiskArea obj = new DiskArea
            {
                Name = "name",
            };

            bool actual = filter.Matches(obj);
            Assert.True(actual);
        }

        #endregion

        #region Display

        [Theory]
        [InlineData("display.aspectx", "12345")]
        [InlineData("display.aspecty", "12345")]
        [InlineData("display.displaytype", "vector")]
        [InlineData("display.screen", "vector")]
        [InlineData("display.flipx", "yes")]
        [InlineData("display.hbend", "12345")]
        [InlineData("display.hbstart", "12345")]
        [InlineData("display.height", "12345")]
        [InlineData("display.y", "12345")]
        [InlineData("display.htotal", "12345")]
        [InlineData("display.pixclock", "12345")]
        [InlineData("display.refresh", "123.45")]
        [InlineData("display.freq", "123.45")]
        [InlineData("display.rotate", "90")]
        [InlineData("display.orientation", "90")]
        [InlineData("display.tag", "tag")]
        [InlineData("display.vbend", "12345")]
        [InlineData("display.vbstart", "12345")]
        [InlineData("display.vtotal", "12345")]
        [InlineData("display.width", "12345")]
        [InlineData("display.x", "12345")]
        public void Matches_Display(string itemField, string value)
        {
            var filter = new FilterObject(itemField, value, Operation.Equals);
            Display obj = new Display
            {
                AspectX = 12345,
                AspectY = 12345,
                DisplayType = DisplayType.Vector,
                FlipX = true,
                HBEnd = 12345,
                HBStart = 12345,
                Height = 12345,
                HTotal = 12345,
                PixClock = 12345,
                Refresh = 123.45,
                Rotate = Rotation.East,
                Tag = "tag",
                VBEnd = 12345,
                VBStart = 12345,
                VTotal = 12345,
                Width = 12345,
            };

            bool actual = filter.Matches(obj);
            Assert.True(actual);
        }

        #endregion
    }
}

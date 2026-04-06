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
        [InlineData("chip.type", "audio")]
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
    }
}

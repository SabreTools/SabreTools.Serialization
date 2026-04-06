using SabreTools.Data.Models.Metadata;
using Xunit;

namespace SabreTools.Metadata.Filter.Test
{
    public class FilterRunnerTests
    {
        private static readonly FilterRunner _filterRunner;

        static FilterRunnerTests()
        {
            FilterObject[] filters =
            [
                new FilterObject("header.author", "auth", Operation.Equals),
                new FilterObject("machine.description", "desc", Operation.Equals),
                new FilterObject("item.name", "name", Operation.Equals),
                new FilterObject("rom.crc", "crc", Operation.Equals),
            ];

            _filterRunner = new FilterRunner(filters);
        }

        #region Header

        [Fact]
        public void Header_Missing_False()
        {
            var header = new Header();
            bool actual = _filterRunner.Run(header);
            Assert.False(actual);
        }

        [Fact]
        public void Header_Null_False()
        {
            var header = new Header { Author = null };
            bool actual = _filterRunner.Run(header);
            Assert.False(actual);
        }

        [Fact]
        public void Header_Empty_False()
        {
            var header = new Header { Author = "" };
            bool actual = _filterRunner.Run(header);
            Assert.False(actual);
        }

        [Fact]
        public void Header_Incorrect_False()
        {
            var header = new Header { Author = "NO_MATCH" };
            bool actual = _filterRunner.Run(header);
            Assert.False(actual);
        }

        [Fact]
        public void Header_Correct_True()
        {
            var header = new Header { Author = "auth" };
            bool actual = _filterRunner.Run(header);
            Assert.True(actual);
        }

        #endregion

        #region Machine

        [Fact]
        public void Machine_Missing_False()
        {
            var machine = new Machine();
            bool actual = _filterRunner.Run(machine);
            Assert.False(actual);
        }

        [Fact]
        public void Machine_Null_False()
        {
            var machine = new Machine { Description = null };
            bool actual = _filterRunner.Run(machine);
            Assert.False(actual);
        }

        [Fact]
        public void Machine_Empty_False()
        {
            var machine = new Machine { Description = "" };
            bool actual = _filterRunner.Run(machine);
            Assert.False(actual);
        }

        [Fact]
        public void Machine_Incorrect_False()
        {
            var machine = new Machine { Description = "NO_MATCH" };
            bool actual = _filterRunner.Run(machine);
            Assert.False(actual);
        }

        [Fact]
        public void Machine_Correct_True()
        {
            var machine = new Machine { Description = "desc" };
            bool actual = _filterRunner.Run(machine);
            Assert.True(actual);
        }

        #endregion

        #region DatItem (General)

        [Fact]
        public void DatItem_Missing_False()
        {
            DatItem datItem = new Sample();
            bool actual = _filterRunner.Run(datItem);
            Assert.False(actual);
        }

        [Fact]
        public void DatItem_Null_False()
        {
            DatItem datItem = new Sample { Name = null };
            bool actual = _filterRunner.Run(datItem);
            Assert.False(actual);
        }

        [Fact]
        public void DatItem_Empty_False()
        {
            DatItem datItem = new Sample { Name = "" };
            bool actual = _filterRunner.Run(datItem);
            Assert.False(actual);
        }

        [Fact]
        public void DatItem_Incorrect_False()
        {
            DatItem datItem = new Sample { Name = "NO_MATCH" };
            bool actual = _filterRunner.Run(datItem);
            Assert.False(actual);
        }

        [Fact]
        public void DatItem_Correct_True()
        {
            DatItem datItem = new Sample { Name = "name" };
            bool actual = _filterRunner.Run(datItem);
            Assert.True(actual);
        }

        #endregion

        #region DatItem (Specific)

        [Fact]
        public void Rom_Missing_False()
        {
            DatItem datItem = new Rom();
            bool actual = _filterRunner.Run(datItem);
            Assert.False(actual);
        }

        [Fact]
        public void Rom_Null_False()
        {
            DatItem datItem = new Rom
            {
                Name = "name",
                CRC32 = null,
            };

            bool actual = _filterRunner.Run(datItem);
            Assert.False(actual);
        }

        [Fact]
        public void Rom_Empty_False()
        {
            DatItem datItem = new Rom
            {
                Name = "name",
                CRC32 = "",
            };

            bool actual = _filterRunner.Run(datItem);
            Assert.False(actual);
        }

        [Fact]
        public void Rom_Incorrect_False()
        {
            DatItem datItem = new Rom
            {
                Name = "name",
                CRC32 = "NO_MATCH",
            };

            bool actual = _filterRunner.Run(datItem);
            Assert.False(actual);
        }

        [Fact]
        public void Rom_Correct_True()
        {
            DatItem datItem = new Rom
            {
                Name = "name",
                CRC32 = "crc",
            };

            bool actual = _filterRunner.Run(datItem);
            Assert.True(actual);
        }

        #endregion
    }
}

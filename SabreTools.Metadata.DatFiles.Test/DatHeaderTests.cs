using Xunit;

namespace SabreTools.Metadata.DatFiles.Test
{
    public class DatHeaderTests
    {
        #region Clone

        [Fact]
        public void CloneTest()
        {
            DatHeader header = new DatHeader { Name = "name" };

            object clone = header.Clone();
            DatHeader? actual = clone as DatHeader;
            Assert.NotNull(actual);
            Assert.Equal("name", actual.Name);
        }

        #endregion

        #region CloneFormat

        [Fact]
        public void CloneFormatTest()
        {
            DatHeader header = new DatHeader { DatFormat = DatFormat.Logiqx };

            object clone = header.Clone();
            DatHeader? actual = clone as DatHeader;
            Assert.NotNull(actual);
            Assert.Equal(DatFormat.Logiqx, actual.DatFormat);
        }

        #endregion

        #region GetInternalClone

        [Fact]
        public void GetInternalCloneTest()
        {
            DatHeader header = new DatHeader { Name = "name" };

            Data.Models.Metadata.Header actual = header.GetInternalClone();
            Assert.Equal("name", actual.Name);
        }

        #endregion

        #region Equals

        [Fact]
        public void Equals_Null_False()
        {
            DatHeader self = new DatHeader();
            DatHeader? other = null;

            bool actual = self.Equals(other);
            Assert.False(actual);
        }

        [Fact]
        public void Equals_DefaultInternal_True()
        {
            DatHeader self = new DatHeader();
            DatHeader? other = new DatHeader();

            bool actual = self.Equals(other);
            Assert.True(actual);
        }

        [Fact]
        public void Equals_MismatchedInternal_False()
        {
            DatHeader self = new DatHeader { Name = "self" };

            DatHeader? other = new DatHeader { Name = "other" };

            bool actual = self.Equals(other);
            Assert.False(actual);
        }

        [Fact]
        public void Equals_EqualInternal_True()
        {
            DatHeader self = new DatHeader { Name = "name" };

            DatHeader? other = new DatHeader { Name = "name" };

            bool actual = self.Equals(other);
            Assert.True(actual);
        }

        #endregion
    }
}

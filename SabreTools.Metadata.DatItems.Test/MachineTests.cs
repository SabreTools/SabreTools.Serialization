using Xunit;

namespace SabreTools.Metadata.DatItems.Test
{
    public class MachineTests
    {
        #region Clone

        [Fact]
        public void CloneTest()
        {
            Machine item = new Machine { Name = "name" };

            object clone = item.Clone();
            Machine? actual = clone as Machine;
            Assert.NotNull(actual);
            Assert.Equal("name", actual.Name);
        }

        #endregion

        #region GetInternalClone

        [Fact]
        public void GetInternalCloneTest()
        {
            Machine item = new Machine { Name = "name" };

            Data.Models.Metadata.Machine actual = item.GetInternalClone();
            Assert.Equal("name", actual.Name);
        }

        #endregion

        #region Equals

        [Fact]
        public void Equals_Null_False()
        {
            Machine self = new Machine();
            Machine? other = null;

            bool actual = self.Equals(other);
            Assert.False(actual);
        }

        [Fact]
        public void Equals_DefaultInternal_True()
        {
            Machine self = new Machine();
            Machine? other = new Machine();

            bool actual = self.Equals(other);
            Assert.True(actual);
        }

        [Fact]
        public void Equals_MismatchedInternal_False()
        {
            Machine self = new Machine { Name = "self" };

            Machine? other = new Machine { Name = "other" };

            bool actual = self.Equals(other);
            Assert.False(actual);
        }

        [Fact]
        public void Equals_EqualInternal_True()
        {
            Machine self = new Machine { Name = "name" };

            Machine? other = new Machine { Name = "name" };

            bool actual = self.Equals(other);
            Assert.True(actual);
        }

        #endregion
    }
}

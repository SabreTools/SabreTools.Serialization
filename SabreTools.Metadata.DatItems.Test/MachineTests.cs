using SabreTools.Data.Extensions;
using Xunit;

namespace SabreTools.Metadata.DatItems.Test
{
    public class MachineTests
    {
        #region Clone

        [Fact]
        public void CloneTest()
        {
            Machine item = new Machine();
            item.SetName("name");

            object clone = item.Clone();
            Machine? actual = clone as Machine;
            Assert.NotNull(actual);
            Assert.Equal("name", actual.GetName());
        }

        #endregion

        #region GetInternalClone

        [Fact]
        public void GetInternalCloneTest()
        {
            Machine item = new Machine();
            item.SetName("name");

            Data.Models.Metadata.Machine actual = item.GetInternalClone();
            Assert.Equal("name", actual.GetName());
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
            Machine self = new Machine();
            self.SetName("self");

            Machine? other = new Machine();
            other.SetName("other");

            bool actual = self.Equals(other);
            Assert.False(actual);
        }

        [Fact]
        public void Equals_EqualInternal_True()
        {
            Machine self = new Machine();
            self.SetName("name");

            Machine? other = new Machine();
            other.SetName("name");

            bool actual = self.Equals(other);
            Assert.True(actual);
        }

        #endregion
    }
}

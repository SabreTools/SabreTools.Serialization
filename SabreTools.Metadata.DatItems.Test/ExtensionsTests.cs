using Xunit;

namespace SabreTools.Metadata.DatItems.Test
{
    public class ExtensionsTests
    {
        #region String to Enum

        [Theory]
        [InlineData(null, MachineType.None)]
        [InlineData("none", MachineType.None)]
        [InlineData("bios", MachineType.Bios)]
        [InlineData("dev", MachineType.Device)]
        [InlineData("device", MachineType.Device)]
        [InlineData("mech", MachineType.Mechanical)]
        [InlineData("mechanical", MachineType.Mechanical)]
        public void AsMachineTypeTest(string? field, MachineType expected)
        {
            MachineType actual = field.AsMachineType();
            Assert.Equal(expected, actual);
        }

        #endregion
    }
}

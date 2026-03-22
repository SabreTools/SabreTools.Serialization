using SabreTools.Data.Models.AtariLynx;
using Xunit;

namespace SabreTools.Data.Extensions.Test
{
    public class AtariLynxCartExtensionsTests
    {
        [Theory]
        [InlineData(Rotation.NoRotation, "No rotation (horizontal, buttons right)")]
        [InlineData(Rotation.RotateLeft, "Rotate left (vertical, buttons down)")]
        [InlineData(Rotation.RotateRight, "Rotate right (vertical, buttons up)")]
        public void FromRotationTest(Rotation rotation, string expected)
        {
            string actual = rotation.FromRotation();
            Assert.Equal(expected, actual);
        }
    }
}

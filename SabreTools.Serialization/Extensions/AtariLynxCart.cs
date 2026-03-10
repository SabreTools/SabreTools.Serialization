using SabreTools.Data.Models.AtariLynx;

namespace SabreTools.Data.Extensions
{
    public static class AtariLynxCart
    {
        /// <summary>
        /// Convert a <see cref="Rotation"/> value to string
        /// </summary>
        public static string FromRotation(this Rotation rotation)
        {
            return rotation switch
            {
                Rotation.NoRotation => "No rotation (horizontal, buttons right)",
                Rotation.RotateLeft => "Rotate left (vertical, buttons down)",
                Rotation.RotateRight => "Rotate right (vertical, buttons up)",
                _ => $"Unknown {(byte)rotation}",
            };
        }
    }
}

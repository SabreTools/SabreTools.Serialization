namespace SabreTools.Data.Models.AtariLynx
{
    /// <summary>
    /// Screen rotation
    /// </summary>
    /// <see href="https://github.com/mozzwald/handy-sdl/blob/master/src/handy-0.95/cart.h"/>
    public enum Rotation : byte
    {
        /// <summary>
        /// No rotation (horizontal, buttons right)
        /// </summary>
        NoRotation = 0,

        /// <summary>
        /// Rotate left (vertical, buttons down)
        /// </summary>
        RotateLeft = 1,

        /// <summary>
        /// Rotate right (vertical, buttons up)
        /// </summary>
        RotateRight = 2,
    }
}

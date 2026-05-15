using SabreTools.Data.Models.NintendoDisc;
using SabreTools.Numerics.Extensions;

namespace SabreTools.Data.Extensions
{
    public static class NintendoDiscExtensions
    {
        /// <summary>
        /// Get the platform associated with a disc header
        /// </summary>
        public static Platform GetPlatform(this DiscHeader header)
        {
            if (header.WiiMagic == Constants.WiiMagicWord)
                return Platform.Wii;
            else if (header.GCMagic == Constants.GCMagicWord)
                return Platform.GameCube;
            else if (header.GameId.Length >= 1 && IsGameCubeTitleType(header.GameId[0]))
                return Platform.GameCube;
            else
                return Platform.Unknown;
        }

        /// <summary>
        /// Get the platform associated with a disc header
        /// </summary>
        public static Platform GetPlatform(this byte[] header)
        {
            // Check for Wii magic bytes
            if (header.Length >= 0x1C)
            {
                int offset = 0x18;
                uint magic = header.ReadUInt32BigEndian(ref offset);
                if (magic == Constants.WiiMagicWord)
                    return Platform.Wii;
            }

            // Check for GameCube magic bytes
            if (header.Length >= 0x20)
            {
                int offset = 0x1C;
                uint magic = header.ReadUInt32BigEndian(ref offset);
                if (magic == Constants.GCMagicWord)
                    return Platform.GameCube;
            }

            // Check for a valid game ID
            if (header.Length >= 4)
            {
                for (int i = 0; i < 4; i++)
                {
                    byte c = header[i];
                    if (!((c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9')))
                        return Platform.Unknown;
                }

                return Platform.GameCube;
            }

            return Platform.Unknown;
        }

        /// <summary>
        /// Returns true if the GameId first character is a known GameCube title type prefix.
        /// Used as a fallback when the GC magic word is absent from the disc image.
        /// </summary>
        public static bool IsGameCubeTitleType(this char c)
            => c == 'G' || c == 'D' || c == 'R';

        /// <summary>
        /// Returns true if the byte is a known Nintendo disc title type code
        /// (first byte of the 6-char GameId, e.g. 'G'=GameCube, 'R'=GameCube,
        ///  'D'=GameCube demo, 'S'=Wii, 'F'=Wii channel)
        /// </summary>
        public static bool IsNintendoDiscTitleType(this byte b)
            => b == 'G' || b == 'D' || b == 'R' || b == 'S' || b == 'F';
    }
}

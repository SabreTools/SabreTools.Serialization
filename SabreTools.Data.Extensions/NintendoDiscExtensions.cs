using SabreTools.Data.Models.NintendoDisc;

namespace SabreTools.Data.Extensions
{
    // TODO: Write tests for these
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
            else if (header.GameId is not null && header.GameId.Length >= 1 && IsGameCubeTitleType(header.GameId[0]))
                return Platform.GameCube;
            else
                return Platform.Unknown;
        }

        /// <summary>
        /// Returns true if the GameId first character is a known GameCube title type prefix.
        /// Used as a fallback when the GC magic word is absent from the disc image.
        /// </summary>
        public static bool IsGameCubeTitleType(this char c)
            => c == 'G' || c == 'D' || c == 'R';
    }
}

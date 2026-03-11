namespace SabreTools.Data.Models.SNES
{
    public static class Constants
    {
        /// <summary>
        /// Super Wild Card header identifier
        /// </summary>
        /// <see href="https://www.raphnet.net/divers/documentation/Sneskart.txt"/>
        public static readonly byte[] SuperWildCardHeaderIdentifier = [0xAA, 0xBB, 0x04];
    }
}

namespace SabreTools.Serialization
{
    /// <summary>
    /// Separated value serializer/deserializer for AttractMode romlists
    /// </summary>
    public static class AttractMode
    {
        public const string HeaderWithoutRomname = "#Name;Title;Emulator;CloneOf;Year;Manufacturer;Category;Players;Rotation;Control;Status;DisplayCount;DisplayType;AltRomname;AltTitle;Extra;Buttons";
        public const int HeaderWithoutRomnameCount = 17;

        public const string HeaderWithRomname = "#Romname;Title;Emulator;Cloneof;Year;Manufacturer;Category;Players;Rotation;Control;Status;DisplayCount;DisplayType;AltRomname;AltTitle;Extra;Buttons;Favourite;Tags;PlayedCount;PlayedTime;FileIsAvailable";
        public const int HeaderWithRomnameCount = 22;
    }
}
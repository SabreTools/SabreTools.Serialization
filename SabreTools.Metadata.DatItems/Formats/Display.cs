using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Text.Extensions;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents one machine display
    /// </summary>
    [JsonObject("display"), XmlRoot("display")]
    public sealed class Display : DatItem<Data.Models.Metadata.Display>
    {
        #region Fields

        /// <inheritdoc>/>
        protected override ItemType ItemType => ItemType.Display;

        #endregion

        #region Constructors

        public Display() : base() { }

        public Display(Data.Models.Metadata.Display item) : base(item)
        {
            // Process flag values
            if (ReadBool(Data.Models.Metadata.Display.FlipXKey) is not null)
                Write<string?>(Data.Models.Metadata.Display.FlipXKey, ReadBool(Data.Models.Metadata.Display.FlipXKey).FromYesNo());
            if (ReadLong(Data.Models.Metadata.Display.HBEndKey) is not null)
                Write<string?>(Data.Models.Metadata.Display.HBEndKey, ReadLong(Data.Models.Metadata.Display.HBEndKey).ToString());
            if (ReadLong(Data.Models.Metadata.Display.HBStartKey) is not null)
                Write<string?>(Data.Models.Metadata.Display.HBStartKey, ReadLong(Data.Models.Metadata.Display.HBStartKey).ToString());
            if (ReadLong(Data.Models.Metadata.Display.HeightKey) is not null)
                Write<string?>(Data.Models.Metadata.Display.HeightKey, ReadLong(Data.Models.Metadata.Display.HeightKey).ToString());
            if (ReadLong(Data.Models.Metadata.Display.HTotalKey) is not null)
                Write<string?>(Data.Models.Metadata.Display.HTotalKey, ReadLong(Data.Models.Metadata.Display.HTotalKey).ToString());
            if (ReadLong(Data.Models.Metadata.Display.PixClockKey) is not null)
                Write<string?>(Data.Models.Metadata.Display.PixClockKey, ReadLong(Data.Models.Metadata.Display.PixClockKey).ToString());
            if (ReadDouble(Data.Models.Metadata.Display.RefreshKey) is not null)
                Write<string?>(Data.Models.Metadata.Display.RefreshKey, ReadDouble(Data.Models.Metadata.Display.RefreshKey).ToString());
            if (ReadLong(Data.Models.Metadata.Display.RotateKey) is not null)
                Write<string?>(Data.Models.Metadata.Display.RotateKey, ReadLong(Data.Models.Metadata.Display.RotateKey).ToString());
            if (ReadString(Data.Models.Metadata.Display.DisplayTypeKey) is not null)
                Write<string?>(Data.Models.Metadata.Display.DisplayTypeKey, ReadString(Data.Models.Metadata.Display.DisplayTypeKey).AsDisplayType().AsStringValue());
            if (ReadLong(Data.Models.Metadata.Display.VBEndKey) is not null)
                Write<string?>(Data.Models.Metadata.Display.VBEndKey, ReadLong(Data.Models.Metadata.Display.VBEndKey).ToString());
            if (ReadLong(Data.Models.Metadata.Display.VBStartKey) is not null)
                Write<string?>(Data.Models.Metadata.Display.VBStartKey, ReadLong(Data.Models.Metadata.Display.VBStartKey).ToString());
            if (ReadLong(Data.Models.Metadata.Display.VTotalKey) is not null)
                Write<string?>(Data.Models.Metadata.Display.VTotalKey, ReadLong(Data.Models.Metadata.Display.VTotalKey).ToString());
            if (ReadLong(Data.Models.Metadata.Display.WidthKey) is not null)
                Write<string?>(Data.Models.Metadata.Display.WidthKey, ReadLong(Data.Models.Metadata.Display.WidthKey).ToString());
        }

        public Display(Data.Models.Metadata.Display item, Machine machine, Source source) : this(item)
        {
            Write<Source?>(SourceKey, source);
            CopyMachineInformation(machine);
        }

        public Display(Data.Models.Metadata.Video item) : base()
        {
            Write(Data.Models.Metadata.Video.AspectXKey, NumberHelper.ConvertToInt64(item.ReadString(Data.Models.Metadata.Video.AspectXKey)));
            Write(Data.Models.Metadata.Video.AspectYKey, NumberHelper.ConvertToInt64(item.ReadString(Data.Models.Metadata.Video.AspectYKey)));
            Write<string?>(Data.Models.Metadata.Display.DisplayTypeKey, item.ReadString(Data.Models.Metadata.Video.ScreenKey).AsDisplayType().AsStringValue());
            Write(Data.Models.Metadata.Display.HeightKey, NumberHelper.ConvertToInt64(item.ReadString(Data.Models.Metadata.Video.HeightKey)));
            Write(Data.Models.Metadata.Display.RefreshKey, NumberHelper.ConvertToDouble(item.ReadString(Data.Models.Metadata.Video.RefreshKey)));
            Write(Data.Models.Metadata.Display.WidthKey, NumberHelper.ConvertToInt64(item.ReadString(Data.Models.Metadata.Video.WidthKey)));

            switch (item.ReadString(Data.Models.Metadata.Video.OrientationKey))
            {
                case "horizontal":
                    Write<long?>(Data.Models.Metadata.Display.RotateKey, 0);
                    break;
                case "vertical":
                    Write<long?>(Data.Models.Metadata.Display.RotateKey, 90);
                    break;
                default:
                    // TODO: Log invalid values
                    break;
            }

            // Process flag values
            if (ReadLong(Data.Models.Metadata.Video.AspectXKey) is not null)
                Write<string?>(Data.Models.Metadata.Video.AspectXKey, ReadLong(Data.Models.Metadata.Video.AspectXKey).ToString());
            if (ReadLong(Data.Models.Metadata.Video.AspectYKey) is not null)
                Write<string?>(Data.Models.Metadata.Video.AspectYKey, ReadLong(Data.Models.Metadata.Video.AspectYKey).ToString());
            if (ReadLong(Data.Models.Metadata.Video.HeightKey) is not null)
                Write<string?>(Data.Models.Metadata.Display.HeightKey, ReadLong(Data.Models.Metadata.Video.HeightKey).ToString());
            if (ReadDouble(Data.Models.Metadata.Video.RefreshKey) is not null)
                Write<string?>(Data.Models.Metadata.Display.RefreshKey, ReadDouble(Data.Models.Metadata.Video.RefreshKey).ToString());
            if (ReadString(Data.Models.Metadata.Video.ScreenKey) is not null)
                Write<string?>(Data.Models.Metadata.Display.DisplayTypeKey, ReadString(Data.Models.Metadata.Video.ScreenKey).AsDisplayType().AsStringValue());
            if (ReadLong(Data.Models.Metadata.Video.WidthKey) is not null)
                Write<string?>(Data.Models.Metadata.Display.WidthKey, ReadLong(Data.Models.Metadata.Video.WidthKey).ToString());
        }

        public Display(Data.Models.Metadata.Video item, Machine machine, Source source) : this(item)
        {
            Write<Source?>(SourceKey, source);
            CopyMachineInformation(machine);
        }

        #endregion
    }
}

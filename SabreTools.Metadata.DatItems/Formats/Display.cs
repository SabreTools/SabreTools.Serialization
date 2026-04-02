using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;
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

        public Data.Models.Metadata.DisplayType? DisplayType
        {
            get => (_internal as Data.Models.Metadata.Display)?.DisplayType;
            set => (_internal as Data.Models.Metadata.Display)?.DisplayType = value;
        }

        public bool? FlipX
        {
            get => (_internal as Data.Models.Metadata.Display)?.FlipX;
            set => (_internal as Data.Models.Metadata.Display)?.FlipX = value;
        }

        public string? Tag
        {
            get => (_internal as Data.Models.Metadata.Display)?.Tag;
            set => (_internal as Data.Models.Metadata.Display)?.Tag = value;
        }

        #endregion

        #region Constructors

        public Display() : base() { }

        public Display(Data.Models.Metadata.Display item) : base(item)
        {
            // Process flag values
            long? hbEnd = ReadLong(Data.Models.Metadata.Display.HBEndKey);
            if (hbEnd is not null)
                Write<string?>(Data.Models.Metadata.Display.HBEndKey, hbEnd.ToString());

            long? hbStart = ReadLong(Data.Models.Metadata.Display.HBStartKey);
            if (hbStart is not null)
                Write<string?>(Data.Models.Metadata.Display.HBStartKey, hbStart.ToString());

            long? height = ReadLong(Data.Models.Metadata.Display.HeightKey);
            if (height is not null)
                Write<string?>(Data.Models.Metadata.Display.HeightKey, height.ToString());

            long? hTotal = ReadLong(Data.Models.Metadata.Display.HTotalKey);
            if (hTotal is not null)
                Write<string?>(Data.Models.Metadata.Display.HTotalKey, hTotal.ToString());

            long? pixClock = ReadLong(Data.Models.Metadata.Display.PixClockKey);
            if (pixClock is not null)
                Write<string?>(Data.Models.Metadata.Display.PixClockKey, pixClock.ToString());

            double? refresh = ReadDouble(Data.Models.Metadata.Display.RefreshKey);
            if (refresh is not null)
                Write<string?>(Data.Models.Metadata.Display.RefreshKey, refresh.ToString());

            long? rotate = ReadLong(Data.Models.Metadata.Display.RotateKey);
            if (rotate is not null)
                Write<string?>(Data.Models.Metadata.Display.RotateKey, rotate.ToString());

            long? vbEnd = ReadLong(Data.Models.Metadata.Display.VBEndKey);
            if (vbEnd is not null)
                Write<string?>(Data.Models.Metadata.Display.VBEndKey, vbEnd.ToString());

            long? vbStart = ReadLong(Data.Models.Metadata.Display.VBStartKey);
            if (vbStart is not null)
                Write<string?>(Data.Models.Metadata.Display.VBStartKey, vbStart.ToString());

            long? vTotal = ReadLong(Data.Models.Metadata.Display.VTotalKey);
            if (vTotal is not null)
                Write<string?>(Data.Models.Metadata.Display.VTotalKey, vTotal.ToString());

            long? width = ReadLong(Data.Models.Metadata.Display.WidthKey);
            if (width is not null)
                Write<string?>(Data.Models.Metadata.Display.WidthKey, width.ToString());
        }

        public Display(Data.Models.Metadata.Display item, Machine machine, Source source) : this(item)
        {
            Source = source;
            CopyMachineInformation(machine);
        }

        public Display(Data.Models.Metadata.Video item) : base()
        {
            // TODO: Convert this block to more traditional set of if/then
            Write(Data.Models.Metadata.Video.AspectXKey, NumberHelper.ConvertToInt64(item.ReadString(Data.Models.Metadata.Video.AspectXKey)));
            Write(Data.Models.Metadata.Video.AspectYKey, NumberHelper.ConvertToInt64(item.ReadString(Data.Models.Metadata.Video.AspectYKey)));
            DisplayType = item.Screen;
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
            long? aspectX = ReadLong(Data.Models.Metadata.Video.AspectXKey);
            if (aspectX is not null)
                Write<string?>(Data.Models.Metadata.Video.AspectXKey, aspectX.ToString());

            long? aspectY = ReadLong(Data.Models.Metadata.Video.AspectYKey);
            if (aspectY is not null)
                Write<string?>(Data.Models.Metadata.Video.AspectYKey, aspectY.ToString());

            long? height = ReadLong(Data.Models.Metadata.Video.HeightKey);
            if (height is not null)
                Write<string?>(Data.Models.Metadata.Display.HeightKey, height.ToString());

            double? refresh = ReadDouble(Data.Models.Metadata.Video.RefreshKey);
            if (refresh is not null)
                Write<string?>(Data.Models.Metadata.Display.RefreshKey, refresh.ToString());

            long? width = ReadLong(Data.Models.Metadata.Video.WidthKey);
            if (width is not null)
                Write<string?>(Data.Models.Metadata.Display.WidthKey, width.ToString());
        }

        public Display(Data.Models.Metadata.Video item, Machine machine, Source source) : this(item)
        {
            Source = source;
            CopyMachineInformation(machine);
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new Display(_internal.DeepClone() as Data.Models.Metadata.Display ?? []);

        #endregion
    }
}

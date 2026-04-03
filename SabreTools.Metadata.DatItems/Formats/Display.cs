using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents one machine display
    /// </summary>
    [JsonObject("display"), XmlRoot("display")]
    public sealed class Display : DatItem<Data.Models.Metadata.Display>
    {
        #region Fields

        public long? AspectX
        {
            get => (_internal as Data.Models.Metadata.Display)?.AspectX;
            set => (_internal as Data.Models.Metadata.Display)?.AspectX = value;
        }

        public long? AspectY
        {
            get => (_internal as Data.Models.Metadata.Display)?.AspectY;
            set => (_internal as Data.Models.Metadata.Display)?.AspectY = value;
        }

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

        public long? HBEnd
        {
            get => (_internal as Data.Models.Metadata.Display)?.HBEnd;
            set => (_internal as Data.Models.Metadata.Display)?.HBEnd = value;
        }

        public long? HBStart
        {
            get => (_internal as Data.Models.Metadata.Display)?.HBStart;
            set => (_internal as Data.Models.Metadata.Display)?.HBStart = value;
        }

        public long? Height
        {
            get => (_internal as Data.Models.Metadata.Display)?.Height;
            set => (_internal as Data.Models.Metadata.Display)?.Height = value;
        }

        public long? HTotal
        {
            get => (_internal as Data.Models.Metadata.Display)?.HTotal;
            set => (_internal as Data.Models.Metadata.Display)?.HTotal = value;
        }

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.Display;

        public long? PixClock
        {
            get => (_internal as Data.Models.Metadata.Display)?.PixClock;
            set => (_internal as Data.Models.Metadata.Display)?.PixClock = value;
        }

        public double? Refresh
        {
            get => (_internal as Data.Models.Metadata.Display)?.Refresh;
            set => (_internal as Data.Models.Metadata.Display)?.Refresh = value;
        }

        public string? Tag
        {
            get => (_internal as Data.Models.Metadata.Display)?.Tag;
            set => (_internal as Data.Models.Metadata.Display)?.Tag = value;
        }

        public long? VBEnd
        {
            get => (_internal as Data.Models.Metadata.Display)?.VBEnd;
            set => (_internal as Data.Models.Metadata.Display)?.VBEnd = value;
        }

        public long? VBStart
        {
            get => (_internal as Data.Models.Metadata.Display)?.VBStart;
            set => (_internal as Data.Models.Metadata.Display)?.VBStart = value;
        }

        public long? VTotal
        {
            get => (_internal as Data.Models.Metadata.Display)?.VTotal;
            set => (_internal as Data.Models.Metadata.Display)?.VTotal = value;
        }

        public long? Width
        {
            get => (_internal as Data.Models.Metadata.Display)?.Width;
            set => (_internal as Data.Models.Metadata.Display)?.Width = value;
        }

        #endregion

        #region Constructors

        public Display() : base() { }

        public Display(Data.Models.Metadata.Display item) : base(item)
        {
            // Process flag values
            long? rotate = ReadLong(Data.Models.Metadata.Display.RotateKey);
            if (rotate is not null)
                Write<string?>(Data.Models.Metadata.Display.RotateKey, rotate.ToString());
        }

        public Display(Data.Models.Metadata.Display item, Machine machine, Source source) : this(item)
        {
            Source = source;
            CopyMachineInformation(machine);
        }

        public Display(Data.Models.Metadata.Video item) : base()
        {
            AspectX = item.AspectX;
            AspectY = item.AspectY;
            DisplayType = item.Screen;
            Height = item.Height;
            Refresh = item.Refresh;
            Width = item.Width;

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

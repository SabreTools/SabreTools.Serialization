using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents one machine display
    /// </summary>
    [JsonObject("display"), XmlRoot("display")]
    public sealed class Display : DatItem<Data.Models.Metadata.Display>
    {
        #region Properties

        public long? AspectX
        {
            get => _internal.AspectX;
            set => _internal.AspectX = value;
        }

        public long? AspectY
        {
            get => _internal.AspectY;
            set => _internal.AspectY = value;
        }

        public Data.Models.Metadata.DisplayType? DisplayType
        {
            get => _internal.DisplayType;
            set => _internal.DisplayType = value;
        }

        public bool? FlipX
        {
            get => _internal.FlipX;
            set => _internal.FlipX = value;
        }

        public long? HBEnd
        {
            get => _internal.HBEnd;
            set => _internal.HBEnd = value;
        }

        public long? HBStart
        {
            get => _internal.HBStart;
            set => _internal.HBStart = value;
        }

        public long? Height
        {
            get => _internal.Height;
            set => _internal.Height = value;
        }

        public long? HTotal
        {
            get => _internal.HTotal;
            set => _internal.HTotal = value;
        }

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.Display;

        public long? PixClock
        {
            get => _internal.PixClock;
            set => _internal.PixClock = value;
        }

        public double? Refresh
        {
            get => _internal.Refresh;
            set => _internal.Refresh = value;
        }

        public Data.Models.Metadata.Rotation? Rotate
        {
            get => _internal.Rotate;
            set => _internal.Rotate = value;
        }

        public string? Tag
        {
            get => _internal.Tag;
            set => _internal.Tag = value;
        }

        public long? VBEnd
        {
            get => _internal.VBEnd;
            set => _internal.VBEnd = value;
        }

        public long? VBStart
        {
            get => _internal.VBStart;
            set => _internal.VBStart = value;
        }

        public long? VTotal
        {
            get => _internal.VTotal;
            set => _internal.VTotal = value;
        }

        public long? Width
        {
            get => _internal.Width;
            set => _internal.Width = value;
        }

        #endregion

        #region Constructors

        public Display() : base() { }

        public Display(Data.Models.Metadata.Display item) : base(item) { }

        public Display(Data.Models.Metadata.Display item, Machine machine, Source source) : this(item)
        {
            Source = source;
            CopyMachineInformation(machine);
        }

        public Display(Data.Models.Metadata.Display item, long machineIndex, long sourceIndex) : this(item)
        {
            SourceIndex = sourceIndex;
            MachineIndex = machineIndex;
        }

        public Display(Data.Models.Metadata.Video item) : base()
        {
            AspectX = item.AspectX;
            AspectY = item.AspectY;
            DisplayType = item.Screen;
            Height = item.Height;
            Refresh = item.Refresh;
            Rotate = item.Orientation;
            Width = item.Width;
        }

        public Display(Data.Models.Metadata.Video item, Machine machine, Source source) : this(item)
        {
            Source = source;
            CopyMachineInformation(machine);
        }

        public Display(Data.Models.Metadata.Video item, long machineIndex, long sourceIndex) : this(item)
        {
            SourceIndex = sourceIndex;
            MachineIndex = machineIndex;
        }

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => null;

        /// <inheritdoc/>
        public override void SetName(string? name) { }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new Display(GetInternalClone());

        /// <inheritdoc/>
        public override Data.Models.Metadata.Display GetInternalClone()
            => _internal.Clone() as Data.Models.Metadata.Display ?? new();

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If the other item is null
            if (other is null)
                return false;

            // If the type matches
            if (other is Display otherDisplay)
                return _internal.Equals(otherDisplay._internal);

            // Everything else fails
            return false;
        }

        #endregion
    }
}

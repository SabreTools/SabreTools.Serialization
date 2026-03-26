using System;
using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents a single port on a machine
    /// </summary>
    [JsonObject("port"), XmlRoot("port")]
    public sealed class Port : DatItem<Data.Models.Metadata.Port>
    {
        #region Fields

        /// <inheritdoc>/>
        protected override ItemType ItemType => ItemType.Port;

        [JsonIgnore]
        public bool AnalogsSpecified
        {
            get
            {
                var analogs = Read<Analog[]?>(Data.Models.Metadata.Port.AnalogKey);
                return analogs is not null && analogs.Length > 0;
            }
        }

        #endregion

        #region Constructors

        public Port() : base() { }

        public Port(Data.Models.Metadata.Port item) : base(item)
        {
            // Handle subitems
            var analogs = item.ReadItemArray<Data.Models.Metadata.Analog>(Data.Models.Metadata.Port.AnalogKey);
            if (analogs is not null)
            {
                Analog[] analogItems = Array.ConvertAll(analogs, analog => new Analog(analog));
                Write<Analog[]?>(Data.Models.Metadata.Port.AnalogKey, analogItems);
            }
        }

        public Port(Data.Models.Metadata.Port item, Machine machine, Source source) : this(item)
        {
            Write<Source?>(SourceKey, source);
            CopyMachineInformation(machine);
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override Data.Models.Metadata.Port GetInternalClone()
        {
            var portItem = base.GetInternalClone();

            var analogs = Read<Analog[]?>(Data.Models.Metadata.Port.AnalogKey);
            if (analogs is not null)
            {
                Data.Models.Metadata.Analog[] analogItems = Array.ConvertAll(analogs, analog => analog.GetInternalClone());
                portItem[Data.Models.Metadata.Port.AnalogKey] = analogItems;
            }

            return portItem;
        }

        #endregion
    }
}

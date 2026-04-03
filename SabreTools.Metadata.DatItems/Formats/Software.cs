using System;
using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents one ListXML software
    /// </summary>
    [JsonObject("software"), XmlRoot("software")]
    public sealed class Software : DatItem<Data.Models.Metadata.Software>
    {
        #region Properties

        public string? Description
        {
            get => (_internal as Data.Models.Metadata.Software)?.Description;
            set => (_internal as Data.Models.Metadata.Software)?.Description = value;
        }

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.Software;

        public string? Name
        {
            get => (_internal as Data.Models.Metadata.Software)?.Name;
            set => (_internal as Data.Models.Metadata.Software)?.Name = value;
        }

        public Data.Models.Metadata.Supported? Supported
        {
            get => (_internal as Data.Models.Metadata.Software)?.Supported;
            set => (_internal as Data.Models.Metadata.Software)?.Supported = value;
        }

        #endregion

        #region Constructors

        public Software() : base() { }

        public Software(Data.Models.Metadata.Software item) : base(item)
        {
            // Handle subitems
            var infos = item.ReadArray<Data.Models.Metadata.Info>(Data.Models.Metadata.Software.InfoKey);
            if (infos is not null)
            {
                Info[] infoItems = Array.ConvertAll(infos, info => new Info(info));
                Write<Info[]?>(Data.Models.Metadata.Software.InfoKey, infoItems);
            }

            var parts = item.ReadArray<Data.Models.Metadata.Part>(Data.Models.Metadata.Software.PartKey);
            if (parts is not null)
            {
                Part[] partItems = Array.ConvertAll(parts, part => new Part(part));
                Write<Part[]?>(Data.Models.Metadata.Software.PartKey, partItems);
            }

            var sharedFeats = item.ReadArray<Data.Models.Metadata.SharedFeat>(Data.Models.Metadata.Software.SharedFeatKey);
            if (sharedFeats is not null)
            {
                SharedFeat[] sharedFeatItems = Array.ConvertAll(sharedFeats, sharedFeat => new SharedFeat(sharedFeat));
                Write<SharedFeat[]?>(Data.Models.Metadata.Software.SharedFeatKey, sharedFeatItems);
            }
        }

        public Software(Data.Models.Metadata.Software item, Machine machine, Source source) : this(item)
        {
            Source = source;
            CopyMachineInformation(machine);
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>

        /// <inheritdoc/>
        public override object Clone() => new Software(_internal.DeepClone() as Data.Models.Metadata.Software ?? []);

        /// <inheritdoc/>
        public override Data.Models.Metadata.Software GetInternalClone()
        {
            var softwareItem = base.GetInternalClone();

            var infos = Read<Info[]?>(Data.Models.Metadata.Software.InfoKey);
            if (infos is not null)
            {
                Data.Models.Metadata.Info[] infoItems = Array.ConvertAll(infos, info => info.GetInternalClone());
                softwareItem[Data.Models.Metadata.Software.InfoKey] = infoItems;
            }

            var parts = Read<Part[]?>(Data.Models.Metadata.Software.PartKey);
            if (parts is not null)
            {
                Data.Models.Metadata.Part[] partItems = Array.ConvertAll(parts, part => part.GetInternalClone());
                softwareItem[Data.Models.Metadata.Software.PartKey] = partItems;
            }

            var sharedFeats = Read<SharedFeat[]?>(Data.Models.Metadata.Software.SharedFeatKey);
            if (sharedFeats is not null)
            {
                Data.Models.Metadata.SharedFeat[] sharedFeatItems = Array.ConvertAll(sharedFeats, sharedFeat => sharedFeat.GetInternalClone());
                softwareItem[Data.Models.Metadata.Software.SharedFeatKey] = sharedFeatItems;
            }

            return softwareItem;
        }

        #endregion
    }
}

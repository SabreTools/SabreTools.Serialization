using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    // TODO: IEquatable<Disk>
    [JsonObject("disk"), XmlRoot(elementName: "disk")]
    public class Disk : DatItem, ICloneable
    {
        #region Properties

        public string? Flags { get; set; }

        public long? Index { get; set; }

        public string? MD5 { get; set; }

        public string? Merge { get; set; }

        public string? Name { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? Optional { get; set; }

        public string? Region { get; set; }

        public string? SHA1 { get; set; }

        /// <remarks>(baddump|nodump|good|verified) "good"</remarks>
        public ItemStatus? Status { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? Writable { get; set; }

        #endregion

        public Disk() => ItemType = ItemType.Disk;

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new Disk();

            obj.Flags = Flags;
            obj.Index = Index;
            obj.MD5 = MD5;
            obj.Merge = Merge;
            obj.Name = Name;
            obj.Optional = Optional;
            obj.Region = Region;
            obj.SHA1 = SHA1;
            obj.Status = Status;
            obj.Writable = Writable;

            return obj;
        }
    }
}

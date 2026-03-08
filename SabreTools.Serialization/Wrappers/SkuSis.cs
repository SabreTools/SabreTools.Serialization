using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using SabreTools.Data.Models.VDF;

namespace SabreTools.Serialization.Wrappers
{
    public partial class SkuSis : WrapperBase<Data.Models.VDF.SkuSis>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Valve Data File";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="Models.VDF.SkuSis.Sku"/>
        public Sku? Sku => Model.Sku;

        /// <inheritdoc cref="Models.VDF.Sku.Name"/>
        public string? Name => Sku?.Name;

        /// <inheritdoc cref="Models.VDF.Sku.ProductName"/>
        public string? ProductName => Sku?.ProductName;

        /// <inheritdoc cref="Models.VDF.Sku.SubscriptionId"/>
        public long? SubscriptionId => Sku?.SubscriptionId;

        /// <inheritdoc cref="Models.VDF.Sku.AppId"/>
        public long? AppId => Sku?.AppId;

        /// <inheritdoc cref="Models.VDF.Sku.Disks"/>
        public uint? Disks => Sku?.Disks;

        /// <inheritdoc cref="Models.VDF.Sku.Language"/>
        public string? Language => Sku?.Language;

        /// <inheritdoc cref="Models.VDF.Sku.Disk"/>
        public uint? Disk => Sku?.Disk;

        /// <inheritdoc cref="Models.VDF.Sku.Backup"/>
        public uint? Backup => Sku?.Backup;

        /// <inheritdoc cref="Models.VDF.Sku.contenttype"/>
        public uint? ContentType => Sku?.ContentType;

        /// <inheritdoc cref="Models.VDF.Sku.Apps"/>
        public Dictionary<long, long>? Apps => Sku?.Apps;

        /// <inheritdoc cref="Models.VDF.Sku.Depots"/>
        public Dictionary<long, long>? Depots => Sku?.Depots;

        /// <inheritdoc cref="Models.VDF.Sku.Manifests"/>
        public Dictionary<long, long>? Manifests => Sku?.Manifests;

        /// <inheritdoc cref="Models.VDF.Sku.Chunkstores"/>
        public Dictionary<long, Dictionary<long, long>?>? Chunkstores => Sku?.Chunkstores;

        /// <inheritdoc cref="Models.VDF.Sku.EverythingElse"/>
        public IDictionary<string, JToken>? EverythingElse => Sku?.EverythingElse;

        #endregion

        #region Constructors

        public SkuSis(Data.Models.VDF.SkuSis model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public SkuSis(Data.Models.VDF.SkuSis model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public SkuSis(Data.Models.VDF.SkuSis model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public SkuSis(Data.Models.VDF.SkuSis model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public SkuSis(Data.Models.VDF.SkuSis model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public SkuSis(Data.Models.VDF.SkuSis model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create an SKU sis from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the SKU sis</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>An SKU sis wrapper on success, null on failure</returns>
        public static SkuSis? Create(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data is null || data.Length == 0)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and use that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Create(dataStream);
        }

        /// <summary>
        /// Create an SKU sis from a Stream
        /// </summary>
        /// <param name="data">Stream representing the SKU sis</param>
        /// <returns>An SKU sis wrapper on success, null on failure</returns>
        public static SkuSis? Create(Stream? data)
        {
            // If the data is invalid
            if (data is null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = new Readers.SkuSis().Deserialize(data);
                if (model is null)
                    return null;

                return new SkuSis(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}

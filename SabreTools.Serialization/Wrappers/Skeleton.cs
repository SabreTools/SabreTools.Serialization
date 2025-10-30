using System.Collections.Generic;
using System.IO;
using SabreTools.Data.Models.ISO9660;

namespace SabreTools.Serialization.Wrappers
{
    public class Skeleton : WrapperBase<Volume>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Redumper Skeleton";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="Volume.SystemArea"/>
        public byte[] SystemArea => Model.SystemArea ?? [];

        /// <inheritdoc cref="Volume.VolumeDescriptorSet"/>
        public VolumeDescriptor[] VolumeDescriptorSet => Model.VolumeDescriptorSet ?? [];

        /// <inheritdoc cref="Volume.PathTableGroups"/>
        public PathTableGroup[] PathTableGroups => Model.PathTableGroups ?? [];

        /// <inheritdoc cref="Volume.DirectoryDescriptors"/>
        public Dictionary<int, FileExtent> DirectoryDescriptors => Model.DirectoryDescriptors ?? [];

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public Skeleton(Volume model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public Skeleton(Volume model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public Skeleton(Volume model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public Skeleton(Volume model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public Skeleton(Volume model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public Skeleton(Volume model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        public static ISO9660? Create(byte[]? data, int offset)
        {
            return ISO9660.Create(data, offset);
        }

        public static ISO9660? Create(Stream? data)
        {
            return ISO9660.Create(data);
        }
    }
}

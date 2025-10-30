namespace SabreTools.Serialization.Wrappers
{
    public class Skeleton : ISO9660
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Redumper Skeleton";

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

        public override bool Extract(string outputDirectory, bool includeDebug)
        {
            // Skeleton does not support extraction
            return false;
        }
    }
}

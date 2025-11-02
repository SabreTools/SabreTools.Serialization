namespace SabreTools.Serialization.Wrappers
{
    public partial class CDROM : IExtractable
    {
        /// <inheritdoc/>
        public override bool Extract(string outputDirectory, bool includeDebug)
            => false;
    }
}

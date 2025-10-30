namespace SabreTools.Serialization.Wrappers
{
    public partial class Skeleton : IExtractable
    {
        /// <inheritdoc/>
        public new bool Extract(string outputDirectory, bool includeDebug)
            => false;
    }
}

namespace SabreTools.Serialization.Wrappers
{
    public partial class Skeleton : IExtractable
    {
        /// <inheritdoc/>
        public override bool Extract(string outputDirectory, bool includeDebug)
            => false;
    }
}

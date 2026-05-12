namespace SabreTools.Wrappers
{
    public partial class GCZ : IExtractable
    {
        /// <inheritdoc/>
        public bool Extract(string outputDirectory, bool includeDebug)
        {
            // Decompress GCZ to obtain the inner disc image, then delegate extraction.
            var inner = GetInnerWrapper();
            return inner?.Extract(outputDirectory, includeDebug) ?? false;
        }
    }
}

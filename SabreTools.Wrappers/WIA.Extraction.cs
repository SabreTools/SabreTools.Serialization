namespace SabreTools.Wrappers
{
    public partial class WIA : IExtractable
    {
        /// <inheritdoc/>
        public bool Extract(string outputDirectory, bool includeDebug)
        {
            // Decompress WIA/RVZ to obtain the inner disc image, then delegate extraction.
            var inner = GetInnerWrapper();
            return inner?.Extract(outputDirectory, includeDebug) ?? false;
        }
    }
}

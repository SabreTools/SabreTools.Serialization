namespace SabreTools.Wrappers
{
    public partial class WIA : IExtractable
    {
        /// <inheritdoc/>
        public bool Extract(string outputDirectory, bool includeDebug)
        {
            var inner = GetInnerWrapper();
            return inner?.Extract(outputDirectory, includeDebug) ?? false;
        }
    }
}

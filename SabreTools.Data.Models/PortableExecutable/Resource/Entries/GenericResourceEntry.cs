namespace SabreTools.Data.Models.PortableExecutable.Resource.Entries
{
    /// <summary>
    /// Generic or unparsed resource data
    /// </summary>
    public class GenericResourceEntry : ResourceDataType
    {
        /// <summary>
        /// Unparsed byte data from the resource
        /// </summary>
        public byte[] Data { get; set; } = [];
    }
}

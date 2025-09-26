namespace SabreTools.Data.Models.OLE
{
    /// <summary>
    /// The VectorHeader packet represents the number of scalar values in a vector property type.
    /// </summary>
    /// <see href="https://winprotocoldoc.z19.web.core.windows.net/MS-OLEPS/%5bMS-OLEPS%5d.pdf"/> 
    public class VectorHeader
    {
        /// <summary>
        /// An unsigned integer indicating the number of scalar values following the header.
        /// </summary>
        public uint Length { get; set; }
    }
}

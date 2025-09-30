namespace SabreTools.Data.Models.XZ
{
    public class FilterFlag
    {
        /// <summary>
        /// Filter ID
        /// </summary>
        /// <remarks>Stored as a variable-length integer</remarks>
        public ulong FilterID { get; set; }

        /// <summary>
        /// Filter ID
        /// </summary>
        /// <remarks>Stored as a variable-length integer</remarks>
        public ulong SizeOfProperties { get; set; }

        /// <summary>
        /// Properties of the filter whose length is given by
        /// <see cref="SizeOfProperties"/> 
        /// </summary>
        public byte[]? Properties { get; set; }
    }
}

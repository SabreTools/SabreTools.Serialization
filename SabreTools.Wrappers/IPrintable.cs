using System.Text;

namespace SabreTools.Wrappers
{
    /// <summary>
    /// Marks a wrapper as being able to print model information
    /// </summary>
    public interface IPrintable
    {
        /// <summary>
        /// Export the item information as JSON
        /// </summary>
        /// <param name="recursive">Enable outputting subfile information</param>
        public string ExportJSON(bool recursive);

        /// <summary>
        /// Print information associated with a model
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        /// <param name="recursive">Enable outputting subfile information</param>
        public void PrintInformation(StringBuilder builder, bool recursive);
    }
}

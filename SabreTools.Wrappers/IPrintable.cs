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
        public string ExportJSON();

        /// <summary>
        /// Print information associated with a model
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        public void PrintInformation(StringBuilder builder);
    }
}

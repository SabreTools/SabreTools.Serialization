using System.Text;

namespace SabreTools.Serialization.Wrappers
{
    /// <summary>
    /// Marks a wrapper as being able to print model information
    /// </summary>
    public interface IPrintable
    {
#if NETCOREAPP
        /// <summary>
        /// Export the item information as JSON
        /// </summary>
        string ExportJSON();
#endif

        /// <summary>
        /// Print information associated with a model
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        void PrintInformation(StringBuilder builder);
    }
}

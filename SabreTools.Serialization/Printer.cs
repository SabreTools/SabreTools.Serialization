using System;
using System.Text;
using SabreTools.Serialization.Wrappers;

namespace SabreTools.Serialization
{
    /// <summary>
    /// Generic wrapper around printing methods
    /// </summary>
    public static class Printer
    {
        /// <summary>
        /// Print the item information from a wrapper to console as
        /// pretty-printed text
        /// </summary>
        public static void PrintToConsole(this IWrapper wrapper)
        {
            var sb = ExportStringBuilder(wrapper);
            if (sb == null)
            {
                Console.WriteLine("No item information could be generated");
                return;
            }

            Console.WriteLine(sb.ToString());
        }

        /// <summary>
        /// Export the item information as a StringBuilder
        /// </summary>
        public static StringBuilder? ExportStringBuilder(this IWrapper wrapper)
        {
            // Ignore unprintable types
            if (wrapper is not IPrintable printable)
                return null;

            var builder = new StringBuilder();
            printable.PrintInformation(builder);
            return builder;
        }

#if NETCOREAPP
        /// <summary>
        /// Export the item information as JSON
        /// </summary>
        public static string ExportJSON(this IWrapper wrapper)
        {
            // Ignore unprintable types
            if (wrapper is not IPrintable printable)
                return string.Empty;

            return printable.ExportJSON();
        }
#endif
    }
}

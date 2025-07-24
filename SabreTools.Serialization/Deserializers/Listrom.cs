using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.Models.Listrom;

namespace SabreTools.Serialization.Deserializers
{
    public class Listrom : BaseBinaryDeserializer<MetadataFile>
    {
        /// <inheritdoc/>
        public override MetadataFile? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return default;

            try
            {
                // Setup the reader and output
                var reader = new StreamReader(data, Encoding.UTF8);
                var dat = new MetadataFile();

                Set? set = null;
                var sets = new List<Set>();
                var rows = new List<Row>();

                while (!reader.EndOfStream)
                {
                    // Read the line and don't split yet
                    string? line = reader.ReadLine();
                    if (string.IsNullOrEmpty(line))
                    {
                        // If we have a set to process
                        if (set != null)
                        {
                            set.Row = [.. rows];
                            sets.Add(set);
                            set = null;
                            rows.Clear();
                        }

                        continue;
                    }

                    // Set lines are unique
                    if (line.StartsWith("ROMs required for driver"))
                    {
                        string driver = line.Substring("ROMs required for driver".Length).Trim('"', ' ', '.');
                        set = new Set { Driver = driver };
                        continue;
                    }
                    else if (line.StartsWith("No ROMs required for driver"))
                    {
                        string driver = line.Substring("No ROMs required for driver".Length).Trim('"', ' ', '.');
                        set = new Set { Driver = driver };
                        continue;
                    }
                    else if (line.StartsWith("ROMs required for device"))
                    {
                        string device = line.Substring("ROMs required for device".Length).Trim('"', ' ', '.');
                        set = new Set { Device = device };
                        continue;
                    }
                    else if (line.StartsWith("No ROMs required for device"))
                    {
                        string device = line.Substring("No ROMs required for device".Length).Trim('"', ' ', '.');
                        set = new Set { Device = device };
                        continue;
                    }
                    else if (line.Equals("Name                                   Size Checksum", StringComparison.OrdinalIgnoreCase))
                    {
                        // No-op
                        continue;
                    }

                    // Split the line for the name iteratively
#if NETFRAMEWORK || NETCOREAPP3_1 || NETSTANDARD2_0_OR_GREATER
                    string[] lineParts = line.Split(new string[] { "     " }, StringSplitOptions.RemoveEmptyEntries);
                    if (lineParts.Length == 1)
                        lineParts = line.Split(new string[] { "    " }, StringSplitOptions.RemoveEmptyEntries);
                    if (lineParts.Length == 1)
                        lineParts = line.Split(new string[] { "   " }, StringSplitOptions.RemoveEmptyEntries);
                    if (lineParts.Length == 1)
                        lineParts = line.Split(new string[] { "  " }, StringSplitOptions.RemoveEmptyEntries);
#else
                    string[] lineParts = line.Split("     ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    if (lineParts.Length == 1)
                        lineParts = line.Split("    ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    if (lineParts.Length == 1)
                        lineParts = line.Split("   ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    if (lineParts.Length == 1)
                        lineParts = line.Split("  ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
#endif

                    // Read the name and set the rest of the line for processing
                    string name = lineParts[0];
                    string trimmedLine = line.Substring(name.Length);
                    if (trimmedLine == null)
                        continue;

#if NETFRAMEWORK || NETCOREAPP3_1 || NETSTANDARD2_0_OR_GREATER
                    lineParts = trimmedLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
#else
                    lineParts = trimmedLine.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
#endif

                    // The number of items in the row explains what type of row it is
                    var row = new Row();
                    switch (lineParts.Length)
                    {
                        // Normal CHD (Name, MD5/SHA1)
                        case 1:
                            row.Name = name;
                            if (line.Contains("MD5("))
                                row.MD5 = lineParts[0].Substring("MD5".Length).Trim('(', ')');
                            else
                                row.SHA1 = lineParts[0].Substring("SHA1".Length).Trim('(', ')');
                            break;

                        // Normal ROM (Name, Size, CRC, MD5/SHA1)
                        case 3 when line.Contains("CRC"):
                            row.Name = name;
                            row.Size = lineParts[0];
                            row.CRC = lineParts[1].Substring("CRC".Length).Trim('(', ')');
                            if (line.Contains("MD5("))
                                row.MD5 = lineParts[2].Substring("MD5".Length).Trim('(', ')');
                            else
                                row.SHA1 = lineParts[2].Substring("SHA1".Length).Trim('(', ')');
                            break;

                        // Bad CHD (Name, BAD, SHA1, BAD_DUMP)
                        case 3 when line.Contains("BAD_DUMP"):
                            row.Name = name;
                            row.Bad = true;
                            if (line.Contains("MD5("))
                                row.MD5 = lineParts[1].Substring("MD5".Length).Trim('(', ')');
                            else
                                row.SHA1 = lineParts[1].Substring("SHA1".Length).Trim('(', ')');
                            break;

                        // Nodump CHD (Name, NO GOOD DUMP KNOWN)
                        case 4 when line.Contains("NO GOOD DUMP KNOWN"):
                            row.Name = name;
                            row.NoGoodDumpKnown = true;
                            break;

                        // Bad ROM (Name, Size, BAD, CRC, MD5/SHA1, BAD_DUMP)
                        case 5 when line.Contains("BAD_DUMP"):
                            row.Name = name;
                            row.Size = lineParts[0];
                            row.Bad = true;
                            row.CRC = lineParts[2].Substring("CRC".Length).Trim('(', ')');
                            if (line.Contains("MD5("))
                                row.MD5 = lineParts[3].Substring("MD5".Length).Trim('(', ')');
                            else
                                row.SHA1 = lineParts[3].Substring("SHA1".Length).Trim('(', ')');
                            break;

                        // Nodump ROM (Name, Size, NO GOOD DUMP KNOWN)
                        case 5 when line.Contains("NO GOOD DUMP KNOWN"):
                            row.Name = name;
                            row.Size = lineParts[0];
                            row.NoGoodDumpKnown = true;
                            break;

                        default:
                            row = null;
                            break;
                    }

                    if (row != null)
                        rows.Add(row);
                }

                // If we have a set to process
                if (set != null)
                {
                    set.Row = [.. rows];
                    sets.Add(set);
                    set = null;
                    rows.Clear();
                }

                // Add extra pieces and return
                if (sets.Count > 0)
                {
                    dat.Set = [.. sets];
                    return dat;
                }

                return null;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }
    }
}

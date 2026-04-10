using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using SabreTools.Data.Extensions;
using SabreTools.Metadata.DatItems;
using SabreTools.Metadata.DatItems.Formats;
using SabreTools.Metadata.Filter;

namespace SabreTools.Metadata.DatFiles.Formats
{
    /// <summary>
    /// Represents parsing and writing of a reference SabreDAT JSON
    /// </summary>
    /// TODO: Transform this into direct serialization and deserialization of the Metadata type
    public sealed class SabreJSON : DatFile
    {
        /// <inheritdoc/>
        public override Data.Models.Metadata.ItemType[] SupportedTypes
#if NET5_0_OR_GREATER
            => Enum.GetValues<Data.Models.Metadata.ItemType>();
#else
            => Enum.GetValues(typeof(Data.Models.Metadata.ItemType)) as Data.Models.Metadata.ItemType[] ?? [];
#endif

        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        public SabreJSON(DatFile? datFile) : base(datFile)
        {
            Header.DatFormat = DatFormat.SabreJSON;
        }

        /// <inheritdoc/>
        public override void ParseFile(string filename,
            int indexId,
            bool keep,
            bool statsOnly = false,
            FilterRunner? filterRunner = null,
            bool throwOnError = false)
        {
            // Prepare all internal variables
            var fs = System.IO.File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var sr = new StreamReader(fs, new UTF8Encoding(false));
            var jtr = new JsonTextReader(sr);
            var source = new Source(indexId, filename);
            // long sourceIndex = AddSourceDB(source);

            // If we got a null reader, just return
            if (jtr is null)
                return;

            // Otherwise, read the file to the end
            try
            {
                jtr.Read();
                while (!sr.EndOfStream)
                {
                    // Skip everything not a property name
                    if (jtr.TokenType != JsonToken.PropertyName)
                    {
                        jtr.Read();
                        continue;
                    }

                    switch (jtr.Value)
                    {
                        // Header value
                        case "header":
                            ReadHeader(jtr);
                            jtr.Read();
                            break;

                        // Machine array
                        case "machines":
                            ReadMachines(jtr, statsOnly, source, sourceIndex: 0, filterRunner);
                            jtr.Read();
                            break;

                        default:
                            jtr.Read();
                            break;
                    }
                }
            }
            catch (Exception ex) when (!throwOnError)
            {
                _logger.Warning($"Exception found while parsing '{filename}': {ex}");
            }

            jtr.Close();
        }

        /// <summary>
        /// Read header information
        /// </summary>
        /// <param name="jtr">JsonTextReader to use to parse the header</param>
        private void ReadHeader(JsonTextReader jtr)
        {
            // If the reader is invalid, skip
            if (jtr is null)
                return;

            // Read in the header and apply any new fields
            jtr.Read();
            JsonSerializer js = new();
            DatHeader? header = js.Deserialize<DatHeader>(jtr);
            SetHeader(header);
        }

        /// <summary>
        /// Read machine array information
        /// </summary>
        /// <param name="jtr">JsonTextReader to use to parse the machine</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="source">Source representing the DAT</param>
        /// <param name="sourceIndex">Index of the Source representing the DAT</param>
        /// <param name="filterRunner">Optional FilterRunner to filter items on parse</param>
        private void ReadMachines(JsonTextReader jtr, bool statsOnly, Source source, long sourceIndex, FilterRunner? filterRunner)
        {
            // If the reader is invalid, skip
            if (jtr is null)
                return;

            // Read in the machine array
            jtr.Read();
            var js = new JsonSerializer();
            JArray machineArray = js.Deserialize<JArray>(jtr) ?? [];

            // Loop through each machine object and process
            foreach (JObject machineObj in machineArray.Cast<JObject>())
            {
                ReadMachine(machineObj, statsOnly, source, sourceIndex, filterRunner);
            }
        }

        /// <summary>
        /// Read machine object information
        /// </summary>
        /// <param name="machineObj">JObject representing a single machine</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="source">Source representing the DAT</param>
        /// <param name="sourceIndex">Index of the Source representing the DAT</param>
        /// <param name="filterRunner">Optional FilterRunner to filter items on parse</param>
        private void ReadMachine(JObject machineObj, bool statsOnly, Source source, long sourceIndex, FilterRunner? filterRunner)
        {
            // If object is invalid, skip it
            if (machineObj is null)
                return;

            // Prepare internal variables
            Machine? machine = null;

            // Read the machine info, if possible
            if (machineObj.ContainsKey("machine"))
                machine = machineObj["machine"]?.ToObject<Machine>();

            // If the machine doesn't pass the filter
            if (machine is not null && filterRunner is not null && !machine.PassesFilter(filterRunner))
                return;

            // Add the machine to the dictionary
            // long machineIndex = -1;
            // if (machine is not null)
            //     machineIndex = AddMachineDB(machine);

            // Read items, if possible
            if (machineObj.ContainsKey("items"))
            {
                ReadItems(machineObj["items"] as JArray,
                    statsOnly,
                    source,
                    sourceIndex,
                    machine,
                    machineIndex: 0,
                    filterRunner);
            }
        }

        /// <summary>
        /// Read item array information
        /// </summary>
        /// <param name="itemsArr">JArray representing the items list</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="source">Source representing the DAT</param>
        /// <param name="sourceIndex">Index of the Source representing the DAT</param>
        /// <param name="machine">Machine information to add to the parsed items</param>
        /// <param name="machineIndex">Index of the Machine to add to the parsed items</param>
        /// <param name="filterRunner">Optional FilterRunner to filter items on parse</param>
        private void ReadItems(
            JArray? itemsArr,
            bool statsOnly,

            // Standard Dat parsing
            Source source,
            long sourceIndex,

            // Miscellaneous
            Machine? machine,
            long machineIndex,
            FilterRunner? filterRunner)
        {
            // If the array is invalid, skip
            if (itemsArr is null)
                return;

            // Loop through each datitem object and process
            foreach (JObject itemObj in itemsArr.Cast<JObject>())
            {
                ReadItem(itemObj, statsOnly, source, sourceIndex, machine, machineIndex, filterRunner);
            }
        }

        /// <summary>
        /// Read item information
        /// </summary>
        /// <param name="itemObj">JObject representing a single datitem</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="source">Source representing the DAT</param>
        /// <param name="sourceIndex">Index of the Source representing the DAT</param>
        /// <param name="machine">Machine information to add to the parsed items</param>
        /// <param name="machineIndex">Index of the Machine to add to the parsed items</param>
        /// <param name="filterRunner">Optional FilterRunner to filter items on parse</param>
        private void ReadItem(
            JObject itemObj,
            bool statsOnly,

            // Standard Dat parsing
            Source source,
            long sourceIndex,

            // Miscellaneous
            Machine? machine,
            long machineIndex,
            FilterRunner? filterRunner)
        {
            // If we have an empty item, skip it
            if (itemObj is null)
                return;

            // Prepare internal variables
            DatItem? datItem = null;

            // Read the datitem info, if possible
            if (itemObj.ContainsKey("datitem"))
            {
                JToken? datItemObj = itemObj["datitem"];
                if (datItemObj is null)
                    return;

                switch (datItemObj.Value<string>("type").AsItemType())
                {
                    case Data.Models.Metadata.ItemType.Adjuster:
                        datItem = datItemObj.ToObject<Adjuster>();
                        break;
                    case Data.Models.Metadata.ItemType.Archive:
                        datItem = datItemObj.ToObject<Archive>();
                        break;
                    case Data.Models.Metadata.ItemType.BiosSet:
                        datItem = datItemObj.ToObject<BiosSet>();
                        break;
                    case Data.Models.Metadata.ItemType.Blank:
                        datItem = datItemObj.ToObject<Blank>();
                        break;
                    case Data.Models.Metadata.ItemType.Chip:
                        datItem = datItemObj.ToObject<Chip>();
                        break;
                    case Data.Models.Metadata.ItemType.Configuration:
                        datItem = datItemObj.ToObject<Configuration>();
                        break;
                    case Data.Models.Metadata.ItemType.ConfLocation:
                        datItem = datItemObj.ToObject<ConfLocation>();
                        break;
                    case Data.Models.Metadata.ItemType.ConfSetting:
                        datItem = datItemObj.ToObject<ConfSetting>();
                        break;
                    case Data.Models.Metadata.ItemType.Control:
                        datItem = datItemObj.ToObject<Control>();
                        break;
                    case Data.Models.Metadata.ItemType.Device:
                        datItem = datItemObj.ToObject<Device>();
                        break;
                    case Data.Models.Metadata.ItemType.DeviceRef:
                        datItem = datItemObj.ToObject<DeviceRef>();
                        break;
                    case Data.Models.Metadata.ItemType.DipLocation:
                        datItem = datItemObj.ToObject<DipLocation>();
                        break;
                    case Data.Models.Metadata.ItemType.DipValue:
                        datItem = datItemObj.ToObject<DipValue>();
                        break;
                    case Data.Models.Metadata.ItemType.DipSwitch:
                        datItem = datItemObj.ToObject<DipSwitch>();
                        break;
                    case Data.Models.Metadata.ItemType.Disk:
                        datItem = datItemObj.ToObject<Disk>();
                        break;
                    case Data.Models.Metadata.ItemType.Display:
                        datItem = datItemObj.ToObject<Display>();
                        break;
                    case Data.Models.Metadata.ItemType.Driver:
                        datItem = datItemObj.ToObject<Driver>();
                        break;
                    case Data.Models.Metadata.ItemType.Feature:
                        datItem = datItemObj.ToObject<Feature>();
                        break;
                    case Data.Models.Metadata.ItemType.File:
                        datItem = datItemObj.ToObject<DatItems.Formats.File>();
                        break;
                    case Data.Models.Metadata.ItemType.Info:
                        datItem = datItemObj.ToObject<Info>();
                        break;
                    case Data.Models.Metadata.ItemType.Input:
                        datItem = datItemObj.ToObject<Input>();
                        break;
                    case Data.Models.Metadata.ItemType.Media:
                        datItem = datItemObj.ToObject<Media>();
                        break;
                    case Data.Models.Metadata.ItemType.Original:
                        // Cannot be converted to a DatItem
                        break;
                    case Data.Models.Metadata.ItemType.PartFeature:
                        datItem = datItemObj.ToObject<PartFeature>();
                        break;
                    case Data.Models.Metadata.ItemType.Port:
                        datItem = datItemObj.ToObject<Port>();
                        break;
                    case Data.Models.Metadata.ItemType.RamOption:
                        datItem = datItemObj.ToObject<RamOption>();
                        break;
                    case Data.Models.Metadata.ItemType.Release:
                        datItem = datItemObj.ToObject<Release>();
                        break;
                    case Data.Models.Metadata.ItemType.ReleaseDetails:
                        datItem = datItemObj.ToObject<ReleaseDetails>();
                        break;
                    case Data.Models.Metadata.ItemType.Rom:
                        datItem = datItemObj.ToObject<Rom>();
                        break;
                    case Data.Models.Metadata.ItemType.Sample:
                        datItem = datItemObj.ToObject<Sample>();
                        break;
                    case Data.Models.Metadata.ItemType.Serials:
                        datItem = datItemObj.ToObject<Serials>();
                        break;
                    case Data.Models.Metadata.ItemType.SharedFeat:
                        datItem = datItemObj.ToObject<SharedFeat>();
                        break;
                    case Data.Models.Metadata.ItemType.Slot:
                        datItem = datItemObj.ToObject<Slot>();
                        break;
                    case Data.Models.Metadata.ItemType.SlotOption:
                        datItem = datItemObj.ToObject<SlotOption>();
                        break;
                    case Data.Models.Metadata.ItemType.SoftwareList:
                        datItem = datItemObj.ToObject<DatItems.Formats.SoftwareList>();
                        break;
                    case Data.Models.Metadata.ItemType.Sound:
                        datItem = datItemObj.ToObject<Sound>();
                        break;
                    case Data.Models.Metadata.ItemType.SourceDetails:
                        datItem = datItemObj.ToObject<SourceDetails>();
                        break;

                    // Removed
                    case Data.Models.Metadata.ItemType.DataArea:
                    case Data.Models.Metadata.ItemType.DiskArea:
                    case Data.Models.Metadata.ItemType.Part:
                        break;

                    // TODO: Implement these?
                    case Data.Models.Metadata.ItemType.Dump:
                    case Data.Models.Metadata.ItemType.Video:
                    case Data.Models.Metadata.ItemType.NULL:
                    default:
                        // This should never happen
                        break;
                }
            }

            // If we got a valid datitem, copy machine info and add
            if (datItem is not null)
            {
                // If the item doesn't pass the filter
                if (filterRunner is not null && !datItem.PassesFilter(filterRunner))
                    return;

                datItem.CopyMachineInformation(machine);
                datItem.Source = source;
                AddItem(datItem, statsOnly);
                AddItemDB(datItem, machineIndex, sourceIndex, statsOnly);
            }
        }

        /// <inheritdoc/>
        public override bool WriteToFile(string outfile, bool ignoreblanks = false, bool throwOnError = false)
        {
            try
            {
                _logger.User($"Writing to '{outfile}'...");
                FileStream fs = System.IO.File.Create(outfile);

                // If we get back null for some reason, just log and return
                if (fs is null)
                {
                    _logger.Warning($"File '{outfile}' could not be created for writing! Please check to see if the file is writable");
                    return false;
                }

                StreamWriter sw = new(fs, new UTF8Encoding(false));
                JsonTextWriter jtw = new(sw)
                {
                    Formatting = Formatting.Indented,
                    IndentChar = '\t',
                    Indentation = 1
                };

                // Write out the header
                WriteHeader(jtw);

                // Write out each of the machines and roms
                string? lastgame = null;

                // Use a sorted list of games to output
                foreach (string key in Items.SortedKeys)
                {
                    List<DatItem> datItems = GetItemsForBucket(key, filter: true);

                    // If this machine doesn't contain any writable items, skip
                    if (!ContainsWritable(datItems))
                        continue;

                    // Resolve the names in the block
                    datItems = ResolveNames(datItems);

                    for (int index = 0; index < datItems.Count; index++)
                    {
                        DatItem datItem = datItems[index];

                        // If we have a different game and we're not at the start of the list, output the end of last item
                        if (lastgame is not null && !string.Equals(lastgame, datItem.Machine!.Name, StringComparison.OrdinalIgnoreCase))
                            WriteEndGame(jtw);

                        // If we have a new game, output the beginning of the new item
                        if (lastgame is null || !string.Equals(lastgame, datItem.Machine!.Name, StringComparison.OrdinalIgnoreCase))
                            WriteStartGame(jtw, datItem);

                        // Check for a "null" item
                        datItem = ProcessNullifiedItem(datItem);

                        // Write out the item if we're not ignoring
                        if (!ShouldIgnore(datItem, ignoreblanks))
                            WriteDatItem(jtw, datItem);

                        // Set the new data to compare against
                        lastgame = datItem.Machine!.Name;
                    }
                }

                // Write the file footer out
                WriteFooter(jtw);

                _logger.User($"'{outfile}' written!{Environment.NewLine}");
                jtw.Close();
                fs.Dispose();
            }
            catch (Exception ex) when (!throwOnError)
            {
                _logger.Error(ex);
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public override bool WriteToFileDB(string outfile, bool ignoreblanks = false, bool throwOnError = false)
        {
            try
            {
                _logger.User($"Writing to '{outfile}'...");
                FileStream fs = System.IO.File.Create(outfile);

                // If we get back null for some reason, just log and return
                if (fs is null)
                {
                    _logger.Warning($"File '{outfile}' could not be created for writing! Please check to see if the file is writable");
                    return false;
                }

                StreamWriter sw = new(fs, new UTF8Encoding(false));
                JsonTextWriter jtw = new(sw)
                {
                    Formatting = Formatting.Indented,
                    IndentChar = '\t',
                    Indentation = 1
                };

                // Write out the header
                WriteHeader(jtw);

                // Write out each of the machines and roms
                string? lastgame = null;

                // Use a sorted list of games to output
                foreach (string key in ItemsDB.SortedKeys)
                {
                    // If this machine doesn't contain any writable items, skip
                    var itemsDict = GetItemsForBucketDB(key, filter: true);
                    if (itemsDict is null || !ContainsWritable([.. itemsDict.Values]))
                        continue;

                    // Resolve the names in the block
                    var items = ResolveNamesDB([.. itemsDict]);

                    foreach (var kvp in items)
                    {
                        // Get the machine for the item
                        var machine = GetMachineForItemDB(kvp.Key);

                        // If we have a different game and we're not at the start of the list, output the end of last item
                        if (lastgame is not null && !string.Equals(lastgame, machine.Value!.Name, StringComparison.OrdinalIgnoreCase))
                            WriteEndGame(jtw);

                        // If we have a new game, output the beginning of the new item
                        if (lastgame is null || !string.Equals(lastgame, machine.Value!.Name, StringComparison.OrdinalIgnoreCase))
                            WriteStartGame(jtw, kvp.Value);

                        // Check for a "null" item
                        var datItem = new KeyValuePair<long, DatItem>(kvp.Key, ProcessNullifiedItem(kvp.Value));

                        // Write out the item if we're not ignoring
                        if (!ShouldIgnore(datItem.Value, ignoreblanks))
                            WriteDatItemDB(jtw, datItem);

                        // Set the new data to compare against
                        lastgame = machine.Value!.Name;
                    }
                }

                // Write the file footer out
                WriteFooter(jtw);

                _logger.User($"'{outfile}' written!{Environment.NewLine}");
                jtw.Close();
                fs.Dispose();
            }
            catch (Exception ex) when (!throwOnError)
            {
                _logger.Error(ex);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Write out DAT header using the supplied JsonTextWriter
        /// </summary>
        /// <param name="jtw">JsonTextWriter to output to</param>
        private void WriteHeader(JsonTextWriter jtw)
        {
            jtw.WriteStartObject();

            // Write the DatHeader
            jtw.WritePropertyName("header");
            JsonSerializer js = new() { Formatting = Formatting.Indented };
            js.Serialize(jtw, Header);

            jtw.WritePropertyName("machines");
            jtw.WriteStartArray();

            jtw.Flush();
        }

        /// <summary>
        /// Write out Game start using the supplied JsonTextWriter
        /// </summary>
        /// <param name="jtw">JsonTextWriter to output to</param>
        /// <param name="datItem">DatItem object to be output</param>
        private static void WriteStartGame(JsonTextWriter jtw, DatItem datItem)
        {
            // No game should start with a path separator
            if (!string.IsNullOrEmpty(datItem.Machine!.Name))
                datItem.Machine!.Name = datItem.Machine!.Name!.TrimStart(Path.DirectorySeparatorChar);

            // Build the state
            jtw.WriteStartObject();

            // Write the Machine
            jtw.WritePropertyName("machine");
            JsonSerializer js = new() { Formatting = Formatting.Indented };
            js.Serialize(jtw, datItem.Machine!);

            jtw.WritePropertyName("items");
            jtw.WriteStartArray();

            jtw.Flush();
        }

        /// <summary>
        /// Write out Game end using the supplied JsonTextWriter
        /// </summary>
        /// <param name="jtw">JsonTextWriter to output to</param>
        private static void WriteEndGame(JsonTextWriter jtw)
        {
            // End items
            jtw.WriteEndArray();

            // End machine
            jtw.WriteEndObject();

            jtw.Flush();
        }

        /// <summary>
        /// Write out DatItem using the supplied JsonTextWriter
        /// </summary>
        /// <param name="jtw">JsonTextWriter to output to</param>
        /// <param name="datItem">DatItem object to be output</param>
        private void WriteDatItem(JsonTextWriter jtw, DatItem datItem)
        {
            // Get the machine for the item
            var machine = datItem.Machine;

            // Pre-process the item name
            ProcessItemName(datItem, machine, forceRemoveQuotes: true, forceRomName: false);

            // Build the state
            jtw.WriteStartObject();

            // Write the DatItem
            jtw.WritePropertyName("datitem");
            JsonSerializer js = new() { ContractResolver = new BaseFirstContractResolver(), Formatting = Formatting.Indented };
            js.Serialize(jtw, datItem);

            // End item
            jtw.WriteEndObject();

            jtw.Flush();
        }

        /// <summary>
        /// Write out DatItem using the supplied JsonTextWriter
        /// </summary>
        /// <param name="jtw">JsonTextWriter to output to</param>
        /// <param name="datItem">DatItem object to be output</param>
        private void WriteDatItemDB(JsonTextWriter jtw, KeyValuePair<long, DatItem> datItem)
        {
            // Get the machine for the item
            var machine = GetMachineForItemDB(datItem.Key);

            // Pre-process the item name
            ProcessItemName(datItem.Value, machine.Value, forceRemoveQuotes: true, forceRomName: false);

            // Build the state
            jtw.WriteStartObject();

            // Write the DatItem
            jtw.WritePropertyName("datitem");
            JsonSerializer js = new() { ContractResolver = new BaseFirstContractResolver(), Formatting = Formatting.Indented };
            js.Serialize(jtw, datItem);

            // End item
            jtw.WriteEndObject();

            jtw.Flush();
        }

        /// <summary>
        /// Write out DAT footer using the supplied JsonTextWriter
        /// </summary>
        /// <param name="jtw">JsonTextWriter to output to</param>
        private static void WriteFooter(JsonTextWriter jtw)
        {
            // End items
            jtw.WriteEndArray();

            // End machine
            jtw.WriteEndObject();

            // End machines
            jtw.WriteEndArray();

            // End file
            jtw.WriteEndObject();

            jtw.Flush();
        }

        // https://github.com/dotnet/runtime/issues/728
        private class BaseFirstContractResolver : DefaultContractResolver
        {
            public BaseFirstContractResolver()
            {
                NamingStrategy = new CamelCaseNamingStrategy();
            }

            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                return [.. base.CreateProperties(type, memberSerialization)
                    .Where(p => p is not null)
                    .OrderBy(p => BaseTypesAndSelf(p.DeclaringType).Count())];

                static IEnumerable<Type?> BaseTypesAndSelf(Type? t)
                {
                    while (t is not null)
                    {
                        yield return t;
                        t = t.BaseType;
                    }
                }
            }
        }
    }
}

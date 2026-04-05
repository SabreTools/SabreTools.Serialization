using System;
using System.Collections.Generic;
using SabreTools.Data.Models.Metadata;

namespace SabreTools.Metadata.Filter
{
    /// <summary>
    /// Represents a set of filters that can be run against an object
    /// </summary>
    public class FilterRunner
    {
        /// <summary>
        /// Set of filters to be run against an object
        /// </summary>
        public readonly Dictionary<string, FilterGroup> Filters = [];

        /// <summary>
        /// Cached item type names for filter selection
        /// </summary>
#if NET5_0_OR_GREATER
        private static readonly string[] _datItemTypeNames = Enum.GetNames<ItemType>();
#else
        private static readonly string[] _datItemTypeNames = Enum.GetNames(typeof(ItemType));
#endif

        public FilterRunner(FilterObject[] filters)
        {
            Array.ForEach(filters, AddFilter);
        }

        public FilterRunner(string[] filterStrings)
        {
            Array.ForEach(filterStrings, AddFilter);
        }

        /// <summary>
        /// Run filtering on an item
        /// </summary>
        public bool Run(object obj)
        {
            string? itemName = obj switch
            {
                Header => "header",
                Machine => "machine",
                DatItem datItem => datItem.ItemType.ToString(),
                _ => null,
            };

            // Null is invalid
            if (itemName is null)
                return false;

            // Loop through and run each filter in order
            foreach (var filterKey in Filters.Keys)
            {
                // Skip filters not applicable to the item
                if (filterKey.StartsWith("item.") && Array.IndexOf(_datItemTypeNames, itemName) == -1)
                    continue;
                else if (!filterKey.StartsWith("item.") && !filterKey.StartsWith(itemName, StringComparison.OrdinalIgnoreCase))
                    continue;

                // If we don't get a match, it's a failure
                bool matchOne = Filters[filterKey].Matches(obj);
                if (!matchOne)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Add a single filter to the runner in a group by key
        /// </summary>
        private void AddFilter(FilterObject filter)
        {
            // Get the key as a string
            string key = filter.Key.ToString();

            // Special case for machine types
            if (filter.Key.ItemName == "machine" && filter.Key.FieldName == "isbios")
                key = $"{"machine"}.COMBINEDTYPE";
            else if (filter.Key.ItemName == "machine" && filter.Key.FieldName == "isdevice")
                key = $"{"machine"}.COMBINEDTYPE";
            else if (filter.Key.ItemName == "machine" && filter.Key.FieldName == "ismechanical")
                key = $"{"machine"}.COMBINEDTYPE";

            // Set the expected group type
            GroupType groupType = GroupType.OR;

            // Special case for size
            if (filter.Key.ItemName == "item" && filter.Key.FieldName == "size")
                groupType = GroupType.AND;
            else if (filter.Key.ItemName == "dataarea" && filter.Key.FieldName == "size")
                groupType = GroupType.AND;
            else if (filter.Key.ItemName == "rom" && filter.Key.FieldName == "size")
                groupType = GroupType.AND;

            // Ensure the key exists
            if (!Filters.ContainsKey(key))
                Filters[key] = new FilterGroup(groupType);

            // Add the filter to the set
            Filters[key].AddFilter(filter);
        }

        /// <summary>
        /// Add a single filter to the runner in a group by key
        /// </summary>
        private void AddFilter(string filterString)
        {
            try
            {
                var filter = new FilterObject(filterString);
                AddFilter(filter);
            }
            catch { }
        }
    }
}

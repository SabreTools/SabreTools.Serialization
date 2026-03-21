using System.Collections.Generic;

namespace SabreTools.Data.Models.BSP
{
    /// <summary>
    /// The entity lump is basically a pure ASCII text section.
    /// It consists of the string representations of all entities,
    /// which are copied directly from the input file to the output
    /// BSP file by the compiler.
    ///
    /// Every entity begins and ends with curly brackets. In between
    /// there are the attributes of the entity, one in each line,
    /// which are pairs of strings enclosed by quotes. The first
    /// string is the name of the attribute (the key), the second one
    /// its value. The attribute "classname" is mandatory for every
    /// entity specifiying its type and therefore, how it is
    /// interpreted by the engine.
    /// </summary>
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(GoldSrc)"/>
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/>
    public sealed class Entity
    {
        /// <summary>
        /// Entity attributes
        /// </summary>
        public List<KeyValuePair<string, string>> Attributes { get; set; } = [];
    }
}

using System.Collections.Generic;

namespace SabreTools.Data.Models.STFS
{
    /// <summary>
    /// STFS Volume Descriptor, differs for STFS and SVOD packages
    /// </summary>
    /// <see href="https://free60.org/System-Software/Formats/STFS/"/>
    public abstract class VolumeDescriptor
    {
        // Filled in by child class, SFTSDescriptor or SVODDescriptor
    }
}

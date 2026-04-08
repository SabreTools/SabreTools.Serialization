using System.Collections.Generic;

namespace SabreTools.Data.Models.STFS
{
    /// <summary>
    /// STFS Signature, differs for remotely signed (PIRS/LIVE) and console signed ("CON ") formats
    /// </summary>
    /// <see href="https://free60.org/System-Software/Formats/STFS/"/>
    public abstract class Signature
    {
        // Filled in by child class, MicrosoftSignature or ConsoleSignature
    }
}

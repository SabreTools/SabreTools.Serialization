using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.BSP
{
    /// <see href="https://github.com/ValveSoftware/source-sdk-2013/blob/0d8dceea4310fde5706b3ce1c70609d72a38efdf/sp/src/public/bspfile.h#L583"/> 
    [StructLayout(LayoutKind.Sequential)]
    public sealed class CDispNeighbor
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public CDispSubNeighbor[]? SubNeighbors = new CDispSubNeighbor[2];
    }
}
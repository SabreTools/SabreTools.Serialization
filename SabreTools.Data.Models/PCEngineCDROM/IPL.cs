using SabreTools.Numerics;

namespace SabreTools.Data.Models.PCEngineCDROM
{
    /// <summary>
    /// IPL (Initial Program Loader) Block Data Format
    /// Format information from the Hu7 CD System BIOS Manual "CD-ROM BIOS Ver1.00"
    /// </summary>
    public sealed class IPL
    {
        /// <summary>
        /// "Load Start Record Number of CD"
        /// "Top record no. where the program is contained"
        /// The offset (entry point) of the initial start up program machine code
        /// </summary>
        /// <remarks>Big-Endian</remarks>
        public UInt24 IPLBLK { get; set; } = new();

        /// <summary>
        /// "Load Block Length of CD"
        /// "No. of records for program to read"
        /// Number of sectors to read for the start up program, from the entry point
        /// </summary>
        public byte IPLBLN { get; set; }

        /// <summary>
        /// "Program Load Address"
        /// "Main memory address for program read"
        /// Address for where the program code should be placed into memory
        /// </summary>
        /// <remarks>Little-Endian</remarks>
        public ushort IPLSTA { get; set; }

        /// <summary>
        /// "Program Execute Address"
        /// "Starting address of execution after program read"
        /// Address for where the program code should be executed from memory, after reading
        /// </summary>
        /// <remarks>Little-Endian</remarks>
        public ushort IPLJMP { get; set; }

        /// <summary>
        /// "IPL Set MPR2-6 (+ max_mapping)"
        /// "Bank no. to set to MPR before program read"
        /// "When calling BIOS or using interrupt routine from BIOS, MPR0,1,7 cannot be changed"
        /// Memory Page Register
        /// MPR0 (I/O, $0000 to $1FFF)
        /// MPR1 (WORK RAM, $2000 to $3FFF)
        /// MPR2-6 (USER AREA, $4000 to $5FFF, ..., $C000 to $DFFF)
        /// MPR7 (BIOS ROM, $E000 to $FFFF)
        /// </summary>
        public byte[] IPLMPR { get; set; } = new byte[5];

        /// <summary>
        /// "Opening mode"
        /// </summary>
        public OpenMode OpenMode { get; set; }

        /// <summary>
        /// "Opening Graphic Data Record Number"
        /// "Specifies the top record of data to load"
        /// </summary>
        /// <remarks>Big-Endian</remarks>
        public UInt24 GRPBLK { get; set; } = new();

        /// <summary>
        /// "Opening Graphic Data Length"
        /// "Specifies the total record that contains color palette data, BAT data, and BG font data"
        /// </summary>
        public byte GRPBLN { get; set; }

        /// <summary>
        /// "Opening Graphic Data Read Address"
        /// "Specifies the top VRAM address into which BG font data is read"
        /// </summary>
        /// <remarks>Little-Endian</remarks>
        public ushort GRPADR { get; set; }

        /// <summary>
        /// "Opening ADPCM Data Record Number"
        /// "Specifies the top record of data to load"
        /// </summary>
        /// <remarks>Big-Endian</remarks>
        public UInt24 ADPBLK { get; set; } = new();

        /// <summary>
        /// "Opening ADPCM Data Length"
        /// "Specifies the number of ADPCM data record"
        /// </summary>
        public byte ADPBLN { get; set; }

        /// <summary>
        /// "Opening ADPCM Sampling Rate"
        /// "Specifies the ADPCM sampling rate"
        /// </summary>
        public byte ADPRATE { get; set; }

        /// <summary>
        /// Reserved bytes, zeroed
        /// </summary>
        /// <remarks>7 bytes</remarks>
        public byte[] Reserved { get; set; } = new byte[7];

        /// <summary>
        /// "ID String"
        /// Null-terminated ASCII string
        /// "PC Engine CD-ROM SYSTEM\0"
        /// </summary>
        /// <remarks>24 bytes, NULL terminated</remarks>
        public byte[] SystemString { get; set; } = new byte[24];

        /// <summary>
        /// Null-terminated ASCII string
        /// "Copyright HUDSON SOFT / NEC Home Electronics,Ltd.\0"
        /// </summary>
        /// <remarks>50 bytes, NULL terminated</remarks>
        public byte[] CopyrightString { get; set; } = new byte[50];

        /// <summary>
        /// "Program Name"
        /// ASCII string, not null-terminated
        /// </summary>
        /// <remarks>16 bytes, padding with trailing SPACE \x20</remarks>
        public byte[] ProgramName { get; set; } = new byte[16];

        /// <summary>
        /// Additional optional string
        /// ASCII string, not null-terminated
        /// </summary>
        /// <remarks>6 bytes, padding with trailing SPACE \x20</remarks>
        public byte[] AdditionalString { get; set; } = new byte[16];
    }
}

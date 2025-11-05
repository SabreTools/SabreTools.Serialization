using System.IO;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.CDROM;
using SabreTools.IO.Extensions;

namespace SabreTools.Serialization.Readers
{
    /// <summary>
    /// This intentionally does not inherit from <see cref="BaseBinaryReader"/>
    /// </summary>
    public class CDROM
    {
        /// <summary>
        /// Parse a Stream into a DataSector
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled DataSector on success, null on error</returns>
        /// <remarks>Assumes a seekable stream</remarks>
        public static DataSector? ParseDataSector(Stream data)
        {
            SectorMode sectorMode = data.GetSectorMode();
            return sectorMode switch
            {
                SectorMode.MODE0 => ParseMode0(data),
                SectorMode.MODE1 => ParseMode1(data),
                SectorMode.MODE2 => ParseMode2Formless(data),
                SectorMode.MODE2_FORM1 => ParseMode2Form1(data),
                SectorMode.MODE2_FORM2 => ParseMode2Form2(data),

                SectorMode.UNKNOWN => null,
                _ => null,
            };
        }

        /// <summary>
        /// Parse a Stream into a Mode0
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Mode0 on success, null on error</returns>
        public static Mode0 ParseMode0(Stream data)
        {
            var obj = new Mode0();

            obj.SyncPattern = data.ReadBytes(12);
            obj.Address = data.ReadBytes(3);
            obj.Mode = data.ReadByteValue();

            obj.UserData = data.ReadBytes(2336);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a Mode1
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Mode1 on success, null on error</returns>
        public static Mode1 ParseMode1(Stream data)
        {
            var obj = new Mode1();

            obj.SyncPattern = data.ReadBytes(12);
            obj.Address = data.ReadBytes(3);
            obj.Mode = data.ReadByteValue();

            obj.UserData = data.ReadBytes(2048);
            obj.EDC = data.ReadBytes(4);
            obj.Intermediate = data.ReadBytes(8);
            obj.ECC = data.ReadBytes(276);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a Mode2Formless
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Mode2Formless on success, null on error</returns>
        public static Mode2Formless ParseMode2Formless(Stream data)
        {
            var obj = new Mode2Formless();

            obj.SyncPattern = data.ReadBytes(12);
            obj.Address = data.ReadBytes(3);
            obj.Mode = data.ReadByteValue();

            obj.UserData = data.ReadBytes(2336);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a Mode2Form1
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Mode2Form1 on success, null on error</returns>
        public static Mode2Form1 ParseMode2Form1(Stream data)
        {
            var obj = new Mode2Form1();

            obj.SyncPattern = data.ReadBytes(12);
            obj.Address = data.ReadBytes(3);
            obj.Mode = data.ReadByteValue();

            obj.Subheader = data.ReadBytes(8);
            obj.UserData = data.ReadBytes(2048);
            obj.EDC = data.ReadBytes(4);
            obj.ECC = data.ReadBytes(276);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a Mode2Form2
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Mode2Form2 on success, null on error</returns>
        public static Mode2Form2 ParseMode2Form2(Stream data)
        {
            var obj = new Mode2Form2();

            obj.SyncPattern = data.ReadBytes(12);
            obj.Address = data.ReadBytes(3);
            obj.Mode = data.ReadByteValue();

            obj.Subheader = data.ReadBytes(8);
            obj.UserData = data.ReadBytes(2324);
            obj.EDC = data.ReadBytes(4);

            return obj;
        }
    }
}

using System;
using System.Text;
using SabreTools.Serialization.Interfaces;
using SabreTools.Serialization.Printers;
using Wrapper = SabreTools.Serialization.Wrappers;

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
            var sb = wrapper.ExportStringBuilder();
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
            return wrapper switch
            {
                Wrapper.AACSMediaKeyBlock item => item.PrettyPrint(),
                Wrapper.BDPlusSVM item => item.PrettyPrint(),
                Wrapper.BFPK item => item.PrettyPrint(),
                Wrapper.BSP item => item.PrettyPrint(),
                Wrapper.CFB item => item.PrettyPrint(),
                Wrapper.CHD item => item.PrettyPrint(),
                Wrapper.CIA item => item.PrettyPrint(),
                Wrapper.GCF item => item.PrettyPrint(),
                Wrapper.InstallShieldArchiveV3 item => item.PrettyPrint(),
                Wrapper.InstallShieldCabinet item => item.PrettyPrint(),
                Wrapper.IRD item => item.PrettyPrint(),
                Wrapper.LinearExecutable item => item.PrettyPrint(),
                Wrapper.LZKWAJ item => item.PrettyPrint(),
                Wrapper.LZQBasic item => item.PrettyPrint(),
                Wrapper.LZSZDD item => item.PrettyPrint(),
                Wrapper.MicrosoftCabinet item => item.PrettyPrint(),
                Wrapper.MoPaQ item => item.PrettyPrint(),
                Wrapper.MSDOS item => item.PrettyPrint(),
                Wrapper.N3DS item => item.PrettyPrint(),
                Wrapper.NCF item => item.PrettyPrint(),
                Wrapper.NewExecutable item => item.PrettyPrint(),
                Wrapper.Nitro item => item.PrettyPrint(),
                Wrapper.PAK item => item.PrettyPrint(),
                Wrapper.PFF item => item.PrettyPrint(),
                Wrapper.PIC item => item.PrettyPrint(),
                Wrapper.PKZIP item => item.PrettyPrint(),
                Wrapper.PlayJAudioFile item => item.PrettyPrint(),
                Wrapper.PlayJPlaylist item => item.PrettyPrint(),
                Wrapper.PortableExecutable item => item.PrettyPrint(),
                Wrapper.Quantum item => item.PrettyPrint(),
                Wrapper.SecuROMDFA item => item.PrettyPrint(),
                Wrapper.SGA item => item.PrettyPrint(),
                Wrapper.VBSP item => item.PrettyPrint(),
                Wrapper.VPK item => item.PrettyPrint(),
                Wrapper.WAD3 item => item.PrettyPrint(),
                Wrapper.XeMID item => item.PrettyPrint(),
                Wrapper.XMID item => item.PrettyPrint(),
                Wrapper.XZP item => item.PrettyPrint(),
                _ => null,
            };
        }

#if NETCOREAPP
        /// <summary>
        /// Export the item information as JSON
        /// </summary>
        public static string ExportJSON(this IWrapper wrapper)
        {
            return wrapper switch
            {
                Wrapper.AACSMediaKeyBlock item => item.ExportJSON(),
                Wrapper.BDPlusSVM item => item.ExportJSON(),
                Wrapper.BFPK item => item.ExportJSON(),
                Wrapper.BSP item => item.ExportJSON(),
                Wrapper.CFB item => item.ExportJSON(),
                Wrapper.CHD item => item.ExportJSON(),
                Wrapper.CIA item => item.ExportJSON(),
                Wrapper.GCF item => item.ExportJSON(),
                Wrapper.InstallShieldArchiveV3 item => item.ExportJSON(),
                Wrapper.InstallShieldCabinet item => item.ExportJSON(),
                Wrapper.IRD item => item.ExportJSON(),
                Wrapper.LinearExecutable item => item.ExportJSON(),
                Wrapper.LZKWAJ item => item.ExportJSON(),
                Wrapper.LZQBasic item => item.ExportJSON(),
                Wrapper.LZSZDD item => item.ExportJSON(),
                Wrapper.MicrosoftCabinet item => item.ExportJSON(),
                Wrapper.MoPaQ item => item.ExportJSON(),
                Wrapper.MSDOS item => item.ExportJSON(),
                Wrapper.N3DS item => item.ExportJSON(),
                Wrapper.NCF item => item.ExportJSON(),
                Wrapper.NewExecutable item => item.ExportJSON(),
                Wrapper.Nitro item => item.ExportJSON(),
                Wrapper.PAK item => item.ExportJSON(),
                Wrapper.PFF item => item.ExportJSON(),
                Wrapper.PIC item => item.ExportJSON(),
                Wrapper.PKZIP item => item.ExportJSON(),
                Wrapper.PlayJAudioFile item => item.ExportJSON(),
                Wrapper.PlayJPlaylist item => item.ExportJSON(),
                Wrapper.PortableExecutable item => item.ExportJSON(),
                Wrapper.Quantum item => item.ExportJSON(),
                Wrapper.SecuROMDFA item => item.ExportJSON(),
                Wrapper.SGA item => item.ExportJSON(),
                Wrapper.VBSP item => item.ExportJSON(),
                Wrapper.VPK item => item.ExportJSON(),
                Wrapper.WAD3 item => item.ExportJSON(),
                Wrapper.XeMID item => item.ExportJSON(),
                Wrapper.XMID item => item.ExportJSON(),
                Wrapper.XZP item => item.ExportJSON(),
                _ => string.Empty,
            };
        }
#endif

        #region Static Printing Implementations

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this Wrapper.AACSMediaKeyBlock item)
        {
            var builder = new StringBuilder();
            AACSMediaKeyBlock.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this Wrapper.BDPlusSVM item)
        {
            var builder = new StringBuilder();
            BDPlusSVM.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this Wrapper.BFPK item)
        {
            var builder = new StringBuilder();
            BFPK.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this Wrapper.BSP item)
        {
            var builder = new StringBuilder();
            BSP.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this Wrapper.CFB item)
        {
            var builder = new StringBuilder();
            CFB.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this Wrapper.CHD item)
        {
            var builder = new StringBuilder();
            CHD.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this Wrapper.CIA item)
        {
            var builder = new StringBuilder();
            CIA.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this Wrapper.GCF item)
        {
            var builder = new StringBuilder();
            GCF.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this Wrapper.InstallShieldArchiveV3 item)
        {
            var builder = new StringBuilder();
            InstallShieldArchiveV3.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this Wrapper.InstallShieldCabinet item)
        {
            var builder = new StringBuilder();
            InstallShieldCabinet.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this Wrapper.IRD item)
        {
            var builder = new StringBuilder();
            IRD.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this Wrapper.LinearExecutable item)
        {
            var builder = new StringBuilder();
            LinearExecutable.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this Wrapper.LZKWAJ item)
        {
            var builder = new StringBuilder();
            LZKWAJ.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this Wrapper.LZQBasic item)
        {
            var builder = new StringBuilder();
            LZQBasic.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this Wrapper.LZSZDD item)
        {
            var builder = new StringBuilder();
            LZSZDD.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this Wrapper.MicrosoftCabinet item)
        {
            var builder = new StringBuilder();
            MicrosoftCabinet.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this Wrapper.MoPaQ item)
        {
            var builder = new StringBuilder();
            MoPaQ.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this Wrapper.MSDOS item)
        {
            var builder = new StringBuilder();
            MSDOS.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this Wrapper.N3DS item)
        {
            var builder = new StringBuilder();
            N3DS.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this Wrapper.NCF item)
        {
            var builder = new StringBuilder();
            NCF.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this Wrapper.NewExecutable item)
        {
            var builder = new StringBuilder();
            NewExecutable.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this Wrapper.Nitro item)
        {
            var builder = new StringBuilder();
            Nitro.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this Wrapper.PAK item)
        {
            var builder = new StringBuilder();
            PAK.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this Wrapper.PFF item)
        {
            var builder = new StringBuilder();
            PFF.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this Wrapper.PIC item)
        {
            var builder = new StringBuilder();
            PIC.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this Wrapper.PKZIP item)
        {
            var builder = new StringBuilder();
            PKZIP.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this Wrapper.PlayJAudioFile item)
        {
            var builder = new StringBuilder();
            PlayJAudioFile.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this Wrapper.PlayJPlaylist item)
        {
            var builder = new StringBuilder();
            PlayJPlaylist.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this Wrapper.PortableExecutable item)
        {
            var builder = new StringBuilder();
            PortableExecutable.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this Wrapper.Quantum item)
        {
            var builder = new StringBuilder();
            Quantum.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this Wrapper.SecuROMDFA item)
        {
            var builder = new StringBuilder();
            SecuROMDFA.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this Wrapper.SGA item)
        {
            var builder = new StringBuilder();
            SGA.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this Wrapper.VBSP item)
        {
            var builder = new StringBuilder();
            VBSP.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this Wrapper.VPK item)
        {
            var builder = new StringBuilder();
            VPK.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this Wrapper.WAD3 item)
        {
            var builder = new StringBuilder();
            WAD3.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this Wrapper.XeMID item)
        {
            var builder = new StringBuilder();
            XeMID.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this Wrapper.XMID item)
        {
            var builder = new StringBuilder();
            XMID.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this Wrapper.XZP item)
        {
            var builder = new StringBuilder();
            XZP.Print(builder, item.Model);
            return builder;
        }

        #endregion
    }
}

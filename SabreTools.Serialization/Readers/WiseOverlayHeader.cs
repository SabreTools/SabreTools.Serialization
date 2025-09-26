using System;
using System.IO;
using System.Text;
using SabreTools.Data.Models.WiseInstaller;
using SabreTools.IO.Extensions;

namespace SabreTools.Serialization.Readers
{
    public class WiseOverlayHeader : BaseBinaryDeserializer<OverlayHeader>
    {
        /// <inheritdoc/>
        public override OverlayHeader? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long initialOffset = data.Position;

                var overlayHeader = ParseOverlayHeader(data);

                // DllName
                if (overlayHeader.DllName != null && overlayHeader.DllName.IndexOfAny(Path.GetInvalidFileNameChars()) > 0)
                    return null;

                // WiseColors.dib
                if (overlayHeader.DibDeflatedSize >= data.Length)
                    return null;
                else if (overlayHeader.DibDeflatedSize > overlayHeader.DibInflatedSize)
                    return null;

                // WiseScript.bin
                if (overlayHeader.WiseScriptDeflatedSize == 0)
                    return null;
                else if (overlayHeader.WiseScriptDeflatedSize >= data.Length)
                    return null;
                else if (overlayHeader.WiseScriptDeflatedSize > overlayHeader.WiseScriptInflatedSize)
                    return null;

                // WISE0001.DLL
                if (overlayHeader.WiseDllDeflatedSize >= data.Length)
                    return null;

                // FILE00XX.DAT
                if (overlayHeader.FinalFileDeflatedSize == 0)
                    return null;
                else if (overlayHeader.FinalFileDeflatedSize >= data.Length)
                    return null;
                else if (overlayHeader.FinalFileDeflatedSize > overlayHeader.FinalFileInflatedSize)
                    return null;

                // Valid for older overlay headers
                if (overlayHeader.Endianness == 0x0000 && overlayHeader.InitTextLen == 0)
                    return overlayHeader;
                if (overlayHeader.Endianness != Endianness.LittleEndian && overlayHeader.Endianness != Endianness.BigEndian)
                    return null;

                return overlayHeader;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <summary>
        /// Parse a Stream into an OverlayHeader
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled OverlayHeader on success, null on error</returns>
        private static OverlayHeader ParseOverlayHeader(Stream data)
        {
            var obj = new OverlayHeader();

            obj.DllNameLen = data.ReadByteValue();
            if (obj.DllNameLen > 0)
            {
                byte[] dllName = data.ReadBytes(obj.DllNameLen);
                obj.DllName = Encoding.ASCII.GetString(dllName);
                obj.DllSize = data.ReadUInt32LittleEndian();
            }

            // Read as a single block
            obj.Flags = (OverlayHeaderFlags)data.ReadUInt32LittleEndian();

            // Read as a single block
            obj.GraphicsData = data.ReadBytes(12);

            // Read as a single block
            obj.WiseScriptExitEventOffset = data.ReadUInt32LittleEndian();
            obj.WiseScriptCancelEventOffset = data.ReadUInt32LittleEndian();

            // Read as a single block
            obj.WiseScriptInflatedSize = data.ReadUInt32LittleEndian();
            obj.WiseScriptDeflatedSize = data.ReadUInt32LittleEndian();
            obj.WiseDllDeflatedSize = data.ReadUInt32LittleEndian();
            obj.Ctl3d32DeflatedSize = data.ReadUInt32LittleEndian();
            obj.SomeData4DeflatedSize = data.ReadUInt32LittleEndian();
            obj.RegToolDeflatedSize = data.ReadUInt32LittleEndian();
            obj.ProgressDllDeflatedSize = data.ReadUInt32LittleEndian();
            obj.SomeData7DeflatedSize = data.ReadUInt32LittleEndian();
            obj.SomeData8DeflatedSize = data.ReadUInt32LittleEndian();
            obj.SomeData9DeflatedSize = data.ReadUInt32LittleEndian();
            obj.SomeData10DeflatedSize = data.ReadUInt32LittleEndian();
            obj.FinalFileDeflatedSize = data.ReadUInt32LittleEndian();
            obj.FinalFileInflatedSize = data.ReadUInt32LittleEndian();
            obj.EOF = data.ReadUInt32LittleEndian();

            // Newer installers read this and DibInflatedSize in the above block
            obj.DibDeflatedSize = data.ReadUInt32LittleEndian();

            // Handle older overlay data
            if (obj.DibDeflatedSize > data.Length)
            {
                obj.DibDeflatedSize = 0;
                data.Seek(-4, SeekOrigin.Current);
                return obj;
            }

            obj.DibInflatedSize = data.ReadUInt32LittleEndian();

            // Peek at the next 2 bytes
            ushort peek = data.ReadUInt16LittleEndian();
            data.Seek(-2, SeekOrigin.Current);

            // If the next value is a known Endianness
            if (Enum.IsDefined(typeof(Endianness), peek))
            {
                obj.Endianness = (Endianness)data.ReadUInt16LittleEndian();
            }
            else
            {
                // The first two values are part of the sizes block above
                obj.InstallScriptDeflatedSize = data.ReadUInt32LittleEndian();
                obj.CharacterSet = (CharacterSet)data.ReadUInt32LittleEndian();
                obj.Endianness = (Endianness)data.ReadUInt16LittleEndian();
            }

            // Endianness and init text len are read in a single block
            obj.InitTextLen = data.ReadByteValue();
            if (obj.InitTextLen > 0)
            {
                byte[] initText = data.ReadBytes(obj.InitTextLen);
                obj.InitText = Encoding.ASCII.GetString(initText);
            }

            return obj;
        }
    }
}

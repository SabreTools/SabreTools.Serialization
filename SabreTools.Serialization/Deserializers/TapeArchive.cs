using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.IO.Extensions;
using SabreTools.Models.TAR;

namespace SabreTools.Serialization.Deserializers
{
    public class TapeArchive : BaseBinaryDeserializer<Archive>
    {
        /// <inheritdoc/>
        protected override bool SkipCompression => true;

        /// <inheritdoc/>
        public override Archive? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long initialOffset = data.Position;

                var archive = new Archive();

                // Read all entries sequentially
                List<Entry> entries = [];
                while (data.Position < data.Length)
                {
                    var entry = ParseEntry(data);
                    if (entry == null)
                        break;

                    entries.Add(entry);
                }

                // No entires indicates invalid
                if (entries.Count == 0)
                    return null;

                // Assign the entries
                archive.Entries = [.. entries];

                return archive;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <summary>
        /// Parse a Stream into an Entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Entry on success, null on error</returns>
        public static Entry? ParseEntry(Stream data)
        {
            var obj = new Entry();

            #region Header

            var header = ParseHeader(data);
            if (header == null)
                return null;

            obj.Header = header;

            // TODO: Replace this with AlignToBoundary when IO is updated
            // Align to the block size
            while (data.Position % 512 != 0 && data.Position < data.Length)
            {
                _ = data.ReadByteValue();
            }

            #endregion

            #region Blocks

            // Exit if the size is invalid
            string sizeOctalString = Encoding.ASCII.GetString(header.Size!).TrimEnd('\0');
            if (sizeOctalString.Length == 0)
                return obj;

            // Get the block count from the size
            int octalSize = Convert.ToInt32(sizeOctalString, 8);
            int blockCount = (int)Math.Ceiling((decimal)octalSize / 512);

            // Read all blocks sequentially
            var blocks = new Block[blockCount];
            for (int i = 0; i < blocks.Length; i++)
            {
                var block = ParseBlock(data);
                if (block == null)
                    break;

                blocks[i] = block;
            }

            // TODO: Make this a direct assignment when Models is updated
            obj.Blocks = [.. blocks];

            #endregion

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a Header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Header on success, null on error</returns>
        public static Header? ParseHeader(Stream data)
        {
            var obj = new Header();

            byte[] filenameBytes = data.ReadBytes(100);
            obj.FileName = Encoding.ASCII.GetString(filenameBytes);
            obj.Mode = data.ReadBytes(8);
            obj.UID = data.ReadBytes(8);
            obj.GID = data.ReadBytes(8);
            obj.Size = data.ReadBytes(12);
            obj.ModifiedTime = data.ReadBytes(12);
            obj.Checksum = data.ReadBytes(8);
            obj.TypeFlag = (TypeFlag)data.ReadByteValue();
            byte[] linkNameBytes = data.ReadBytes(100);
            obj.LinkName = Encoding.ASCII.GetString(linkNameBytes);

            // If end of stream has been reached
            if (data.Position >= data.Length)
                return obj;

            // Peek at the next 5 bytes
            byte[] temp = data.ReadBytes(5);
            string tempString = Encoding.ASCII.GetString(temp);
            data.Seek(-5, SeekOrigin.Current);
            if (tempString != "ustar")
                return obj;

            byte[] magicBytes = data.ReadBytes(5);
            obj.Magic = Encoding.ASCII.GetString(magicBytes);
            byte[] versionBytes = data.ReadBytes(3);
            obj.Version = Encoding.ASCII.GetString(versionBytes);
            byte[] userNameBytes = data.ReadBytes(32);
            obj.UserName = Encoding.ASCII.GetString(userNameBytes);
            byte[] groupNameBytes = data.ReadBytes(32);
            obj.GroupName = Encoding.ASCII.GetString(groupNameBytes);
            byte[] devMajorBytes = data.ReadBytes(8);
            obj.DevMajor = Encoding.ASCII.GetString(devMajorBytes);
            byte[] devMinorBytes = data.ReadBytes(8);
            obj.DevMinor = Encoding.ASCII.GetString(devMinorBytes);
            byte[] prefixBytes = data.ReadBytes(155);
            obj.Prefix = Encoding.ASCII.GetString(prefixBytes);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a Block
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Block on success, null on error</returns>
        public static Block? ParseBlock(Stream data)
        {
            // Handle incomplete blocks
            if (data.Position + 512 >= data.Length)
                return null;

            var obj = new Block();

            // TODO: Assign this to obj.Data directly when Models is updated
            byte[] temp = data.ReadBytes(512);
            Array.Copy(temp, 0, obj.Data, 0, 512);

            return obj;
        }
    }
}

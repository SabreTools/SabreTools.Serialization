using System;
using System.IO;
using System.Text;
using SabreTools.Hashing;
using SabreTools.IO.Writers;
using SabreTools.Models.Hashfile;

namespace SabreTools.Serialization.Serializers
{
    public class Hashfile : BaseBinarySerializer<Models.Hashfile.Hashfile>
    {
        #region IByteSerializer

        /// <inheritdoc cref="Interfaces.IByteSerializer.SerializeArray(T?)"/>
        public static byte[]? SerializeBytes(Models.Hashfile.Hashfile? obj, HashType hash = HashType.CRC32)
        {
            var serializer = new Hashfile();
            return serializer.SerializeArray(obj, hash);
        }

        /// <inheritdoc/>
        public override byte[]? SerializeArray(Models.Hashfile.Hashfile? obj)
            => SerializeArray(obj, HashType.CRC32);

        /// <inheritdoc/>
        public byte[]? SerializeArray(Models.Hashfile.Hashfile? obj, HashType hash)
        {
            using var stream = SerializeStream(obj, hash);
            if (stream == null)
                return null;

            byte[] bytes = new byte[stream.Length];
            int read = stream.Read(bytes, 0, bytes.Length);
            return bytes;
        }

        #endregion

        #region IFileSerializer

        /// <inheritdoc cref="IFileSerializer.Serialize(T?, string?)"/>
        public static bool SerializeFile(Models.Hashfile.Hashfile? obj, string? path, HashType hash = HashType.CRC32)
        {
            var serializer = new Hashfile();
            return serializer.Serialize(obj, path, hash);
        }

        /// <inheritdoc/>
        public override bool Serialize(Models.Hashfile.Hashfile? obj, string? path)
            => Serialize(obj, path, HashType.CRC32);

        /// <inheritdoc/>
        public bool Serialize(Models.Hashfile.Hashfile? obj, string? path, HashType hash)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            using var stream = SerializeStream(obj, hash);
            if (stream == null)
                return false;

            using var fs = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.None);
            stream.CopyTo(fs);
            fs.Flush();

            return true;
        }

        #endregion

        #region IStreamSerializer

        /// <inheritdoc cref="IStreamSerializer.Serialize(T?)"/>
        public static Stream? SerializeStream(Models.Hashfile.Hashfile? obj, HashType hash = HashType.CRC32)
        {
            var serializer = new Hashfile();
            return serializer.Serialize(obj, hash);
        }

        /// <inheritdoc/>
        public override Stream? Serialize(Models.Hashfile.Hashfile? obj)
            => Serialize(obj, HashType.CRC32);

        /// <inheritdoc/>
        public Stream? Serialize(Models.Hashfile.Hashfile? obj, HashType hash)
        {
            // If the metadata file is null
            if (obj == null)
                return null;

            // Setup the writer and output
            var stream = new MemoryStream();
            var writer = new SeparatedValueWriter(stream, Encoding.UTF8)
            {
                Separator = ' ',
                Quotes = false,
                VerifyFieldCount = false,
            };

            // Write out the items, if they exist
            switch (hash)
            {
                case HashType.CRC32:
                    WriteSFV(obj.SFV, writer);
                    break;
                case HashType.MD2:
                    WriteMD2(obj.MD2, writer);
                    break;
                case HashType.MD4:
                    WriteMD4(obj.MD4, writer);
                    break;
                case HashType.MD5:
                    WriteMD5(obj.MD5, writer);
                    break;
                case HashType.RIPEMD128:
                    WriteRIPEMD128(obj.RIPEMD128, writer);
                    break;
                case HashType.RIPEMD160:
                    WriteRIPEMD160(obj.RIPEMD160, writer);
                    break;
                case HashType.SHA1:
                    WriteSHA1(obj.SHA1, writer);
                    break;
                case HashType.SHA256:
                    WriteSHA256(obj.SHA256, writer);
                    break;
                case HashType.SHA384:
                    WriteSHA384(obj.SHA384, writer);
                    break;
                case HashType.SHA512:
                    WriteSHA512(obj.SHA512, writer);
                    break;
                case HashType.SpamSum:
                    WriteSpamSum(obj.SpamSum, writer);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(hash));
            }

            // Return the stream
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        /// <summary>
        /// Write SFV information to the current writer
        /// </summary>
        /// <param name="sfvs">Array of SFV objects representing the files</param>
        /// <param name="writer">SeparatedValueWriter representing the output</param>
        private static void WriteSFV(SFV[]? sfvs, SeparatedValueWriter writer)
        {
            // If the item information is missing, we can't do anything
            if (sfvs == null || sfvs.Length == 0)
                return;

            // Loop through and write out the items
            foreach (var sfv in sfvs)
            {
                if (string.IsNullOrEmpty(sfv.File) || string.IsNullOrEmpty(sfv.Hash))
                    continue;

                writer.WriteValues([sfv.File!, sfv.Hash!]);
                writer.Flush();
            }
        }

        /// <summary>
        /// Write MD2 information to the current writer
        /// </summary>
        /// <param name="md2s">Array of MD2 objects representing the files</param>
        /// <param name="writer">SeparatedValueWriter representing the output</param>
        private static void WriteMD2(MD2[]? md2s, SeparatedValueWriter writer)
        {
            // If the item information is missing, we can't do anything
            if (md2s == null || md2s.Length == 0)
                return;

            // Loop through and write out the items
            foreach (var md2 in md2s)
            {
                if (string.IsNullOrEmpty(md2.Hash) || string.IsNullOrEmpty(md2.File))
                    continue;

                writer.WriteValues([md2.Hash!, md2.File!]);
                writer.Flush();
            }
        }

        /// <summary>
        /// Write MD4 information to the current writer
        /// </summary>
        /// <param name="md4s">Array of MD4 objects representing the files</param>
        /// <param name="writer">SeparatedValueWriter representing the output</param>
        private static void WriteMD4(MD4[]? md4s, SeparatedValueWriter writer)
        {
            // If the item information is missing, we can't do anything
            if (md4s == null || md4s.Length == 0)
                return;

            // Loop through and write out the items
            foreach (var md4 in md4s)
            {
                if (string.IsNullOrEmpty(md4.Hash) || string.IsNullOrEmpty(md4.File))
                    continue;

                writer.WriteValues([md4.Hash!, md4.File!]);
                writer.Flush();
            }
        }

        /// <summary>
        /// Write MD5 information to the current writer
        /// </summary>
        /// <param name="md5s">Array of MD5 objects representing the files</param>
        /// <param name="writer">SeparatedValueWriter representing the output</param>
        private static void WriteMD5(MD5[]? md5s, SeparatedValueWriter writer)
        {
            // If the item information is missing, we can't do anything
            if (md5s == null || md5s.Length == 0)
                return;

            // Loop through and write out the items
            foreach (var md5 in md5s)
            {
                if (string.IsNullOrEmpty(md5.Hash) || string.IsNullOrEmpty(md5.File))
                    continue;

                writer.WriteValues([md5.Hash!, md5.File!]);
                writer.Flush();
            }
        }

        /// <summary>
        /// Write RIPEMD128 information to the current writer
        /// </summary>
        /// <param name="ripemd128s">Array of RIPEMD128 objects representing the files</param>
        /// <param name="writer">SeparatedValueWriter representing the output</param>
        private static void WriteRIPEMD128(RIPEMD128[]? ripemd128s, SeparatedValueWriter writer)
        {
            // If the item information is missing, we can't do anything
            if (ripemd128s == null || ripemd128s.Length == 0)
                return;

            // Loop through and write out the items
            foreach (var ripemd128 in ripemd128s)
            {
                if (string.IsNullOrEmpty(ripemd128.Hash) || string.IsNullOrEmpty(ripemd128.File))
                    continue;

                writer.WriteValues([ripemd128.Hash!, ripemd128.File!]);
                writer.Flush();
            }
        }

        /// <summary>
        /// Write RIPEMD160 information to the current writer
        /// </summary>
        /// <param name="ripemd160s">Array of RIPEMD160 objects representing the files</param>
        /// <param name="writer">SeparatedValueWriter representing the output</param>
        private static void WriteRIPEMD160(RIPEMD160[]? ripemd160s, SeparatedValueWriter writer)
        {
            // If the item information is missing, we can't do anything
            if (ripemd160s == null || ripemd160s.Length == 0)
                return;

            // Loop through and write out the items
            foreach (var ripemd160 in ripemd160s)
            {
                if (string.IsNullOrEmpty(ripemd160.Hash) || string.IsNullOrEmpty(ripemd160.File))
                    continue;

                writer.WriteValues([ripemd160.Hash!, ripemd160.File!]);
                writer.Flush();
            }
        }

        /// <summary>
        /// Write SHA1 information to the current writer
        /// </summary>
        /// <param name="sha1s">Array of SHA1 objects representing the files</param>
        /// <param name="writer">SeparatedValueWriter representing the output</param>
        private static void WriteSHA1(SHA1[]? sha1s, SeparatedValueWriter writer)
        {
            // If the item information is missing, we can't do anything
            if (sha1s == null || sha1s.Length == 0)
                return;

            // Loop through and write out the items
            foreach (var sha1 in sha1s)
            {
                if (string.IsNullOrEmpty(sha1.Hash) || string.IsNullOrEmpty(sha1.File))
                    continue;

                writer.WriteValues([sha1.Hash!, sha1.File!]);
                writer.Flush();
            }
        }

        /// <summary>
        /// Write SHA256 information to the current writer
        /// </summary>
        /// <param name="sha256s">Array of SHA256 objects representing the files</param>
        /// <param name="writer">SeparatedValueWriter representing the output</param>
        private static void WriteSHA256(SHA256[]? sha256s, SeparatedValueWriter writer)
        {
            // If the item information is missing, we can't do anything
            if (sha256s == null || sha256s.Length == 0)
                return;

            // Loop through and write out the items
            foreach (var sha256 in sha256s)
            {
                if (string.IsNullOrEmpty(sha256.Hash) || string.IsNullOrEmpty(sha256.File))
                    continue;

                writer.WriteValues([sha256.Hash!, sha256.File!]);
                writer.Flush();
            }
        }

        /// <summary>
        /// Write SHA384 information to the current writer
        /// </summary>
        /// <param name="sha384s">Array of SHA384 objects representing the files</param>
        /// <param name="writer">SeparatedValueWriter representing the output</param>
        private static void WriteSHA384(SHA384[]? sha384s, SeparatedValueWriter writer)
        {
            // If the item information is missing, we can't do anything
            if (sha384s == null || sha384s.Length == 0)
                return;

            // Loop through and write out the items
            foreach (var sha384 in sha384s)
            {
                if (string.IsNullOrEmpty(sha384.Hash) || string.IsNullOrEmpty(sha384.File))
                    continue;

                writer.WriteValues([sha384.Hash!, sha384.File!]);
                writer.Flush();
            }
        }

        /// <summary>
        /// Write SHA512 information to the current writer
        /// </summary>
        /// <param name="sha512s">Array of SHA512 objects representing the files</param>
        /// <param name="writer">SeparatedValueWriter representing the output</param>
        private static void WriteSHA512(SHA512[]? sha512s, SeparatedValueWriter writer)
        {
            // If the item information is missing, we can't do anything
            if (sha512s == null || sha512s.Length == 0)
                return;

            // Loop through and write out the items
            foreach (var sha512 in sha512s)
            {
                if (string.IsNullOrEmpty(sha512.Hash) || string.IsNullOrEmpty(sha512.File))
                    continue;

                writer.WriteValues([sha512.Hash!, sha512.File!]);
                writer.Flush();
            }
        }

        /// <summary>
        /// Write SpamSum information to the current writer
        /// </summary>
        /// <param name="spamsums">Array of SpamSum objects representing the files</param>
        /// <param name="writer">SeparatedValueWriter representing the output</param>
        private static void WriteSpamSum(SpamSum[]? spamsums, SeparatedValueWriter writer)
        {
            // If the item information is missing, we can't do anything
            if (spamsums == null || spamsums.Length == 0)
                return;

            // Loop through and write out the items
            foreach (var spamsum in spamsums)
            {
                if (string.IsNullOrEmpty(spamsum.Hash) || string.IsNullOrEmpty(spamsum.File))
                    continue;

                writer.WriteValues([spamsum.Hash!, spamsum.File!]);
                writer.Flush();
            }
        }

        #endregion
    }
}

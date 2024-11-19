using System;
using System.Collections.Generic;
using System.IO;
using SabreTools.Hashing;
using SabreTools.Models.Hashfile;

namespace SabreTools.Serialization.Deserializers
{
    // TODO: Create variants for the implemented types
    public class Hashfile : BaseBinaryDeserializer<Models.Hashfile.Hashfile>
    {
        #region IByteDeserializer

        /// <inheritdoc cref="IByteDeserializer.Deserialize(byte[]?, int)"/>
        public static Models.Hashfile.Hashfile? DeserializeBytes(byte[]? data, int offset, HashType hash = HashType.CRC32)
        {
            var deserializer = new Hashfile();
            return deserializer.Deserialize(data, offset, hash);
        }

        /// <inheritdoc/>
        public override Models.Hashfile.Hashfile? Deserialize(byte[]? data, int offset)
            => Deserialize(data, offset, HashType.CRC32);

        /// <inheritdoc/>
        public Models.Hashfile.Hashfile? Deserialize(byte[]? data, int offset, HashType hash)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return DeserializeStream(dataStream, hash);
        }

        #endregion

        #region IFileDeserializer

        /// <inheritdoc cref="IFileDeserializer.Deserialize(string?)"/>
        public static Models.Hashfile.Hashfile? DeserializeFile(string? path, HashType hash = HashType.CRC32)
        {
            var deserializer = new Hashfile();
            return deserializer.Deserialize(path, hash);
        }

        /// <inheritdoc/>
        public override Models.Hashfile.Hashfile? Deserialize(string? path)
            => Deserialize(path, HashType.CRC32);

        /// <inheritdoc/>
        public Models.Hashfile.Hashfile? Deserialize(string? path, HashType hash)
        {
            using var stream = PathProcessor.OpenStream(path);
            return DeserializeStream(stream, hash);
        }

        #endregion

        #region IStreamDeserializer

        /// <inheritdoc cref="IStreamDeserializer.Deserialize(Stream?)"/>
        public static Models.Hashfile.Hashfile? DeserializeStream(Stream? data, HashType hash = HashType.CRC32)
        {
            var deserializer = new Hashfile();
            return deserializer.Deserialize(data, hash);
        }

        /// <inheritdoc/>
        public override Models.Hashfile.Hashfile? Deserialize(Stream? data)
            => Deserialize(data, HashType.CRC32);

        /// <inheritdoc cref="Deserialize(Stream)"/>
        public Models.Hashfile.Hashfile? Deserialize(Stream? data, HashType hash)
        {
            // If the stream is null
            if (data == null)
                return default;

            // Setup the reader and output
            var reader = new StreamReader(data);
            var dat = new Models.Hashfile.Hashfile();

            // Create lists for each hash type
            var sfvList = new List<SFV>();
            var md2List = new List<MD2>();
            var md4List = new List<MD4>();
            var md5List = new List<MD5>();
            var sha1List = new List<SHA1>();
            var sha256List = new List<SHA256>();
            var sha384List = new List<SHA384>();
            var sha512List = new List<SHA512>();
            var spamsumList = new List<SpamSum>();

            // Loop through the rows and parse out values
            while (!reader.EndOfStream)
            {
                // Read and split the line
                string? line = reader.ReadLine();
                string[]? lineParts = line?.Split([' '], StringSplitOptions.RemoveEmptyEntries);
                if (lineParts == null)
                    continue;

                // Parse the line into a hash
                switch (hash)
                {
                    case HashType.CRC32:
                        var sfv = new SFV
                        {
                            File = string.Join(" ", lineParts, 0, lineParts.Length - 1),
                            Hash = lineParts[lineParts.Length - 1],
                        };
                        sfvList.Add(sfv);
                        break;
                    case HashType.MD2:
                        var md2 = new MD2
                        {
                            Hash = lineParts[0],
                            File = string.Join(" ", lineParts, 1, lineParts.Length - 1),
                        };
                        md2List.Add(md2);
                        break;
                    case HashType.MD4:
                        var md4 = new MD4
                        {
                            Hash = lineParts[0],
                            File = string.Join(" ", lineParts, 1, lineParts.Length - 1),
                        };
                        md4List.Add(md4);
                        break;
                    case HashType.MD5:
                        var md5 = new MD5
                        {
                            Hash = lineParts[0],
                            File = string.Join(" ", lineParts, 1, lineParts.Length - 1),
                        };
                        md5List.Add(md5);
                        break;
                    case HashType.SHA1:
                        var sha1 = new SHA1
                        {
                            Hash = lineParts[0],
                            File = string.Join(" ", lineParts, 1, lineParts.Length - 1),
                        };
                        sha1List.Add(sha1);
                        break;
                    case HashType.SHA256:
                        var sha256 = new SHA256
                        {
                            Hash = lineParts[0],
                            File = string.Join(" ", lineParts, 1, lineParts.Length - 1),
                        };
                        sha256List.Add(sha256);
                        break;
                    case HashType.SHA384:
                        var sha384 = new SHA384
                        {
                            Hash = lineParts[0],
                            File = string.Join(" ", lineParts, 1, lineParts.Length - 1),
                        };
                        sha384List.Add(sha384);
                        break;
                    case HashType.SHA512:
                        var sha512 = new SHA512
                        {
                            Hash = lineParts[0],
                            File = string.Join(" ", lineParts, 1, lineParts.Length - 1),
                        };
                        sha512List.Add(sha512);
                        break;
                    case HashType.SpamSum:
                        var spamSum = new SpamSum
                        {
                            Hash = lineParts[0],
                            File = string.Join(" ", lineParts, 1, lineParts.Length - 1),
                        };
                        spamsumList.Add(spamSum);
                        break;
                }
            }

            // Assign the hashes to the hashfile and return
            switch (hash)
            {
                case HashType.CRC32:
                    dat.SFV = [.. sfvList];
                    break;
                case HashType.MD2:
                    dat.MD2 = [.. md2List];
                    break;
                case HashType.MD4:
                    dat.MD4 = [.. md4List];
                    break;
                case HashType.MD5:
                    dat.MD5 = [.. md5List];
                    break;
                case HashType.SHA1:
                    dat.SHA1 = [.. sha1List];
                    break;
                case HashType.SHA256:
                    dat.SHA256 = [.. sha256List];
                    break;
                case HashType.SHA384:
                    dat.SHA384 = [.. sha384List];
                    break;
                case HashType.SHA512:
                    dat.SHA512 = [.. sha512List];
                    break;
                case HashType.SpamSum:
                    dat.SpamSum = [.. spamsumList];
                    break;
            }

            return dat;
        }

        #endregion
    }
}
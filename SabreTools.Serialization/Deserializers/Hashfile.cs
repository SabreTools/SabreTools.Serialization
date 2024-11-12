using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            // Loop through the rows and parse out values
            var hashes = new List<object>();
            while (!reader.EndOfStream)
            {
                // Read and split the line
                string? line = reader.ReadLine();
#if NETFRAMEWORK || NETCOREAPP3_1
                string[]? lineParts = line?.Split([' '], StringSplitOptions.RemoveEmptyEntries);
#else
                string[]? lineParts = line?.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
#endif
                if (lineParts == null)
                    continue;

                // Parse the line into a hash
                switch (hash)
                {
                    case HashType.CRC32:
                        var sfv = new SFV
                        {
#if NETFRAMEWORK
                            File = string.Join(" ", lineParts.Take(lineParts.Length - 1).ToArray()),
                            Hash = lineParts[lineParts.Length - 1],
#else
                            File = string.Join(" ", lineParts[..^1]),
                            Hash = lineParts[^1],
#endif
                        };
                        hashes.Add(sfv);
                        break;
                    case HashType.MD5:
                        var md5 = new MD5
                        {
                            Hash = lineParts[0],
#if NETFRAMEWORK
                            File = string.Join(" ", lineParts.Skip(1).ToArray()),
#else
                            File = string.Join(" ", lineParts[1..]),
#endif
                        };
                        hashes.Add(md5);
                        break;
                    case HashType.SHA1:
                        var sha1 = new SHA1
                        {
                            Hash = lineParts[0],
#if NETFRAMEWORK
                            File = string.Join(" ", lineParts.Skip(1).ToArray()),
#else
                            File = string.Join(" ", lineParts[1..]),
#endif
                        };
                        hashes.Add(sha1);
                        break;
                    case HashType.SHA256:
                        var sha256 = new SHA256
                        {
                            Hash = lineParts[0],
#if NETFRAMEWORK
                            File = string.Join(" ", lineParts.Skip(1).ToArray()),
#else
                            File = string.Join(" ", lineParts[1..]),
#endif
                        };
                        hashes.Add(sha256);
                        break;
                    case HashType.SHA384:
                        var sha384 = new SHA384
                        {
                            Hash = lineParts[0],
#if NETFRAMEWORK
                            File = string.Join(" ", lineParts.Skip(1).ToArray()),
#else
                            File = string.Join(" ", lineParts[1..]),
#endif
                        };
                        hashes.Add(sha384);
                        break;
                    case HashType.SHA512:
                        var sha512 = new SHA512
                        {
                            Hash = lineParts[0],
#if NETFRAMEWORK
                            File = string.Join(" ", lineParts.Skip(1).ToArray()),
#else
                            File = string.Join(" ", lineParts[1..]),
#endif
                        };
                        hashes.Add(sha512);
                        break;
                    case HashType.SpamSum:
                        var spamSum = new SpamSum
                        {
                            Hash = lineParts[0],
#if NETFRAMEWORK
                            File = string.Join(" ", lineParts.Skip(1).ToArray()),
#else
                            File = string.Join(" ", lineParts[1..]),
#endif
                        };
                        hashes.Add(spamSum);
                        break;
                }
            }

            // Assign the hashes to the hashfile and return
            switch (hash)
            {
                case HashType.CRC32:
                    dat.SFV = hashes.Cast<SFV>().ToArray();
                    break;
                case HashType.MD5:
                    dat.MD5 = hashes.Cast<MD5>().ToArray();
                    break;
                case HashType.SHA1:
                    dat.SHA1 = hashes.Cast<SHA1>().ToArray();
                    break;
                case HashType.SHA256:
                    dat.SHA256 = hashes.Cast<SHA256>().ToArray();
                    break;
                case HashType.SHA384:
                    dat.SHA384 = hashes.Cast<SHA384>().ToArray();
                    break;
                case HashType.SHA512:
                    dat.SHA512 = hashes.Cast<SHA512>().ToArray();
                    break;
                case HashType.SpamSum:
                    dat.SpamSum = hashes.Cast<SpamSum>().ToArray();
                    break;
            }

            return dat;
        }

        #endregion
    }
}
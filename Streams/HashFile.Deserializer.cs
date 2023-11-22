using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SabreTools.Models.Hashfile;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class Hashfile : IStreamSerializer<Models.Hashfile.Hashfile>
    {
        /// <inheritdoc/>
        public Models.Hashfile.Hashfile? Deserialize(Stream? data) => Deserialize(data, Hash.CRC);

        /// <inheritdoc cref="Deserialize(Stream)"/>
        public Models.Hashfile.Hashfile? Deserialize(Stream? data, Hash hash)
        {
            // If the stream is null
            if (data == null)
                return default;

            // Setup the reader and output
            var reader = new StreamReader(data);
            var dat = new Models.Hashfile.Hashfile();
            var additional = new List<string>();

            // Loop through the rows and parse out values
            var hashes = new List<object>();
            while (!reader.EndOfStream)
            {
                // Read and split the line
                string? line = reader.ReadLine();
#if NETFRAMEWORK || NETCOREAPP3_1
                string[]? lineParts = line?.Split(new char[] { ' ' } , StringSplitOptions.RemoveEmptyEntries);
#else
                string[]? lineParts = line?.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
#endif
                if (lineParts == null)
                    continue;

                // Parse the line into a hash
                switch (hash)
                {
                    case Hash.CRC:
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
                    case Hash.MD5:
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
                    case Hash.SHA1:
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
                    case Hash.SHA256:
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
                    case Hash.SHA384:
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
                    case Hash.SHA512:
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
                    case Hash.SpamSum:
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
                case Hash.CRC:
                    dat.SFV = hashes.Cast<SFV>().ToArray();
                    break;
                case Hash.MD5:
                    dat.MD5 = hashes.Cast<MD5>().ToArray();
                    break;
                case Hash.SHA1:
                    dat.SHA1 = hashes.Cast<SHA1>().ToArray();
                    break;
                case Hash.SHA256:
                    dat.SHA256 = hashes.Cast<SHA256>().ToArray();
                    break;
                case Hash.SHA384:
                    dat.SHA384 = hashes.Cast<SHA384>().ToArray();
                    break;
                case Hash.SHA512:
                    dat.SHA512 = hashes.Cast<SHA512>().ToArray();
                    break;
                case Hash.SpamSum:
                    dat.SpamSum = hashes.Cast<SpamSum>().ToArray();
                    break;
            }
            dat.ADDITIONAL_ELEMENTS = additional.ToArray();
            return dat;
        }
    }
}
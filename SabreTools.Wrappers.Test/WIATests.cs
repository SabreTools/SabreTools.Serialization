using System;
using System.IO;
using System.Linq;
using SabreTools.Numerics.Extensions;
using SabreTools.Security.Cryptography;
using Xunit;
using static SabreTools.Data.Models.NintendoDisc.Constants;

namespace SabreTools.Wrappers.Test
{
    public class WIATests
    {
        /// <summary>
        /// Arbitrary test-only common key — no relation to any real Wii key.
        /// Used by both <see cref="BuildMinimalWiiIso"/> and <see cref="EncryptTitleKeyIndependent"/>.
        /// </summary>
        private static readonly byte[] TestCommonKey =
        [
            0xDE, 0xAD, 0xBE, 0xEF, 0xCA, 0xFE, 0xF0, 0x0D,
            0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88,
        ];

        #region Constants

        private const int HeaderAreaSize = 0x8000;

        private const long IsoSize = Partition1Data + WiiGroupSize;

        private const long Partition0Offset = 0x60000;

        private const long Partition0Data = Partition0Offset + HeaderAreaSize;

        private const long Partition1Offset = Partition0Data + WiiGroupSize;

        private const long Partition1Data = Partition1Offset + HeaderAreaSize;

        private const long PartitionListOffset = 0x50000;

        private const long PartitionTableOffset = 0x40000;

        #endregion

        [Fact]
        public void NullArray_Null()
        {
            byte[]? data = null;
            int offset = 0;
            var actual = WIA.Create(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyArray_Null()
        {
            byte[]? data = [];
            int offset = 0;
            var actual = WIA.Create(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidArray_Null()
        {
            byte[]? data = [.. Enumerable.Repeat<byte>(0xFF, 1024)];
            int offset = 0;
            var actual = WIA.Create(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void NullStream_Null()
        {
            Stream? data = null;
            var actual = WIA.Create(data);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyStream_Null()
        {
            Stream? data = new MemoryStream([]);
            var actual = WIA.Create(data);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidStream_Null()
        {
            Stream? data = new MemoryStream([.. Enumerable.Repeat<byte>(0xFF, 1024)]);
            var actual = WIA.Create(data);
            Assert.Null(actual);
        }

        /// <summary>
        /// Build the smallest valid WIA we can to get a non-null wrapper,
        /// but for the guard test we only need to exercise the null-path branch.
        /// We can create a real wrapper via the round-trip helper and then call
        /// DumpIso with a null path — that must return false.
        /// </summary>
        [Fact(Skip = "Common keys are validated so this cannot pass")]
        public void DumpIso_NullPath_ReturnsFalse()
        {
            var wia = BuildMinimalWiiWia();

            Assert.NotNull(wia);
            Assert.False(wia!.DumpIso(null));
        }

        /// <summary>
        /// Builds a synthetic Wii disc with 2 fake partitions (each 1 WiiGroup = 64 × 0x8000 bytes of
        /// known plaintext encrypted with an arbitrary key), converts it to WIA (NONE compression),
        /// reads it back through <see cref="WIA.DumpIso"/>, then decrypts every Wii data block in the
        /// dumped ISO using <see cref="WiiDecrypter.DecryptBlock"/> and asserts the decrypted bytes
        /// match the original plaintext.
        ///
        /// This exercises both directions:
        ///   • WIA write path re-encrypts partition data correctly (<see cref="WIA.ConvertFromDiscToStream"/>)
        ///   • WIA read path (<see cref="WiaVirtualStream"/>) re-encrypts WIA decrypted groups back to
        ///     ISO-layout AES-CBC blocks via GetCachedEncGroup / EncryptWiiGroup
        ///
        /// Anti-bias: the final decryption uses <see cref="WiiDecrypter.DecryptBlock"/> — a single-block
        /// AES-CBC call that is completely independent of EncryptWiiGroup — so a symmetric bug
        /// (broken encrypt paired with broken decrypt) would still fail the plaintext comparison.
        /// The title key is encrypted via <see cref="AESCBC.Encrypt"/> (BouncyCastle), while the
        /// verification uses <see cref="WiiDecrypter.DecryptBlock"/> — a different code path.
        /// </summary>
        [Fact(Skip = "Common keys are validated so this cannot pass")]
        public void Wii_WiaNoneRoundTrip_Succeeds()
        {
            // ---- Build synthetic Wii ISO ----
            byte[] iso = BuildMinimalWiiIso(TestCommonKey);

            // ---- NintendoDisc.Create must succeed ----
            var nd = NintendoDisc.Create(new MemoryStream(iso));
            Assert.NotNull(nd);
            Assert.NotNull(nd!.PartitionTableEntries);
            Assert.Equal(2, nd.PartitionTableEntries!.Length);

            // ---- Compress to WIA (NONE, no lossy transforms) ----
            using var wiaMs = new MemoryStream();
            bool written = WIA.ConvertFromDiscToStream(nd, wiaMs,
                isRvz: false,
                compressionType: Data.Models.WIA.WiaRvzCompressionType.None,
                compressionLevel: 5,
                chunkSize: Data.Models.WIA.Constants.DefaultChunkSize,
                out Exception? writeEx);
            Assert.True(written, $"ConvertFromDiscToStream failed: {writeEx?.GetType().Name}: {writeEx?.Message}\n{writeEx?.StackTrace}");

            // ---- Decompress back to ISO ----
            wiaMs.Position = 0;
            var wia = WIA.Create(wiaMs);
            Assert.NotNull(wia);

            string tempIso = Path.GetTempFileName() + ".iso";
            try
            {
                bool dumped = wia!.DumpIso(tempIso);
                Assert.True(dumped, "DumpIso should succeed");

                byte[] dumpedIso = File.ReadAllBytes(tempIso);

                byte[] titleKey =
                [
                    0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF,
                        0xFE, 0xDC, 0xBA, 0x98, 0x76, 0x54, 0x32, 0x10,
                    ];

                byte[] plain0 = new byte[WiiBlocksPerGroup * WiiBlockDataSize];
                for (int i = 0; i < plain0.Length; i++)
                {
                    plain0[i] = 0xAA;
                }

                byte[] plain1 = new byte[WiiBlocksPerGroup * WiiBlockDataSize];
                for (int i = 0; i < plain1.Length; i++)
                {
                    plain1[i] = 0xBB;
                }

                // ---- Anti-bias verification: decrypt each block using DecryptBlock only ----
                VerifyPartitionPlaintext(dumpedIso,
                    Partition0Data,
                    plain0,
                    titleKey,
                    WiiBlocksPerGroup,
                    WiiBlockSize,
                    WiiBlockDataSize,
                    partitionLabel: "Partition 0");

                VerifyPartitionPlaintext(dumpedIso,
                    Partition1Data,
                    plain1,
                    titleKey,
                    WiiBlocksPerGroup,
                    WiiBlockSize,
                    WiiBlockDataSize,
                    partitionLabel: "Partition 1");
            }
            finally
            {
                if (File.Exists(tempIso))
                    File.Delete(tempIso);
            }
        }

        #region Wii test helpers

        /// <summary>
        /// Builds a minimal synthetic Wii disc (one WiiGroup per partition) and returns a live
        /// <see cref="WIA"/> wrapper backed by a <see cref="MemoryStream"/>.
        /// Returns null if any step fails.
        /// </summary>
        private static WIA? BuildMinimalWiiWia()
        {
            try
            {
                byte[] iso = BuildMinimalWiiIso(TestCommonKey);
                var nd = NintendoDisc.Create(new MemoryStream(iso));
                if (nd is null)
                    return null;

                // TODO: Force this?
                nd.WiiDecrypter.RetailCommonKey = TestCommonKey;
                nd.WiiDecrypter.KoreanCommonKey = TestCommonKey;

                var ms = new MemoryStream();
                bool ok = WIA.ConvertFromDiscToStream(nd, ms,
                    isRvz: false,
                    compressionType: Data.Models.WIA.WiaRvzCompressionType.None,
                    compressionLevel: 5,
                    chunkSize: Data.Models.WIA.Constants.DefaultChunkSize,
                    out var exception);

                if (!ok)
                    return null;

                ms.Position = 0;
                return WIA.Create(ms);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Builds a minimal synthetic Wii ISO with 2 partitions (1 WiiGroup each), encrypted
        /// with <paramref name="commonKey"/>.
        /// </summary>
        private static byte[] BuildMinimalWiiIso(byte[] commonKey)
        {
            byte[] titleKey =
            [
                0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF,
                0xFE, 0xDC, 0xBA, 0x98, 0x76, 0x54, 0x32, 0x10,
            ];
            byte[] titleId = [0x00, 0x01, 0x00, 0x45, 0x52, 0x53, 0x42, 0x00];
            byte[] encTitleKey = EncryptTitleKeyIndependent(titleKey, titleId, commonKey);

            byte[] plain0 = new byte[WiiGroupDataSize];
            for (int i = 0; i < plain0.Length; i++)
            {
                plain0[i] = 0xAA;
            }

            byte[] plain1 = new byte[WiiGroupDataSize];
            for (int i = 0; i < plain1.Length; i++)
            {
                plain1[i] = 0xBB;
            }

            byte[] enc0 = WIA.EncryptWiiGroup(plain0, titleKey, WiiBlocksPerGroup);
            byte[] enc1 = WIA.EncryptWiiGroup(plain1, titleKey, WiiBlocksPerGroup);

            byte[] iso = new byte[IsoSize];

            iso[0] = (byte)'R'; iso[1] = (byte)'S'; iso[2] = (byte)'B'; iso[3] = (byte)'E';
            iso[4] = (byte)'0'; iso[5] = (byte)'1';
            iso[0x18] = 0x5D; iso[0x19] = 0x1C; iso[0x1A] = 0x9E; iso[0x1B] = 0xA3;

            int off = (int)PartitionTableOffset;
            iso.WriteBigEndian(ref off, 2u);
            iso.WriteBigEndian(ref off, (uint)(PartitionListOffset >> 2));

            off = (int)PartitionListOffset;
            iso.WriteBigEndian(ref off, (uint)(Partition0Offset >> 2));
            iso.WriteBigEndian(ref off, 0u);
            iso.WriteBigEndian(ref off, (uint)(Partition1Offset >> 2));
            iso.WriteBigEndian(ref off, 1u);

            WritePartitionHeader(iso, Partition0Offset, encTitleKey, titleId, ckIdx: 0);
            WritePartitionHeader(iso, Partition1Offset, encTitleKey, titleId, ckIdx: 0);

            Array.Copy(enc0, 0, iso, Partition0Data, enc0.Length);
            Array.Copy(enc1, 0, iso, Partition1Data, enc1.Length);

            return iso;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="iso"></param>
        /// <param name="partOffset"></param>
        /// <param name="encTitleKey"></param>
        /// <param name="titleId"></param>
        /// <param name="ckIdx"></param>
        private static void WritePartitionHeader(byte[] iso,
            long partOffset,
            byte[] encTitleKey,
            byte[] titleId,
            byte ckIdx)
        {
            // Signature type 0x10001 at partOffset+0
            int off = (int)partOffset;
            iso.WriteBigEndian(ref off, 0x10001u);

            // Encrypted title key at partOffset+0x1BF (16 bytes)
            Array.Copy(encTitleKey, 0, iso, partOffset + 0x1BF, 16);

            // Title ID at partOffset+0x1DC (8 bytes)
            Array.Copy(titleId, 0, iso, partOffset + 0x1DC, 8);

            // Common key index at partOffset+0x1F1
            iso[partOffset + 0x1F1] = ckIdx;

            // Data offset at partOffset+0x2B8 (shifted >>2): data starts at +0x8000
            // 0x8000 >> 2 = 0x2000
            off = (int)(partOffset + 0x2B8);
            iso.WriteBigEndian(ref off, 0x2000u);

            // Data size at partOffset+0x2BC (shifted >>2): exactly 1 WiiGroup = 0x200000
            // 0x200000 >> 2 = 0x80000
            iso.WriteBigEndian(ref off, 0x80000u);
        }

        /// <summary>
        /// Decrypts each block of one WII partition in the dumped ISO using only
        /// <see cref="WiiDecrypter.DecryptBlock"/> (a single-block AES-CBC call that is
        /// completely independent of EncryptWiiGroup) and asserts the decrypted
        /// block data matches the corresponding slice of <paramref name="expectedPlaintext"/>.
        /// </summary>
        private static void VerifyPartitionPlaintext(byte[] iso,
            long dataStart,
            byte[] expectedPlaintext,
            byte[] titleKey,
            int blocksPerGroup,
            int blockSize,
            int blockDataSize,
            string partitionLabel)
        {
            for (int b = 0; b < blocksPerGroup; b++)
            {
                long blockOff = dataStart + ((long)b * blockSize);

                // IV = bytes at offset 0x3D0 within the encrypted hash block
                byte[] iv = new byte[16];
                Array.Copy(iso, blockOff + 0x3D0, iv, 0, 16);

                // Encrypted data block follows the 0x400-byte hash block
                byte[] encData = new byte[blockDataSize];
                Array.Copy(iso, blockOff + 0x400, encData, 0, blockDataSize);

                byte[]? dec = WiiDecrypter.DecryptBlock(encData, titleKey, iv);
                Assert.NotNull(dec);

                // Compare against known plaintext slice
                int plainOff = b * blockDataSize;
                for (int i = 0; i < blockDataSize; i++)
                {
                    if (dec![i] != expectedPlaintext[plainOff + i])
                        Assert.Fail($"{partitionLabel} block {b} byte {i}: expected 0x{expectedPlaintext[plainOff + i]:X2}, got 0x{dec[i]:X2}");
                }
            }
        }

        /// <summary>
        /// Encrypts a Wii title key with the given <paramref name="commonKey"/> using
        /// <see cref="AESCBC.Encrypt"/>.
        /// </summary>
        private static byte[] EncryptTitleKeyIndependent(byte[] titleKey, byte[] titleId, byte[] commonKey)
        {
            byte[] iv = new byte[16];
            Array.Copy(titleId, 0, iv, 0, 8);
            return AESCBC.Encrypt(titleKey, commonKey, iv)
                ?? throw new InvalidOperationException("AESCBC.Encrypt returned null");
        }

        #endregion
    }
}

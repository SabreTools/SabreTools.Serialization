using System.Collections.Generic;
using SabreTools.Numerics.Extensions;

namespace SabreTools.Wrappers
{
    /// <summary>
    /// Lightweight GameCube / Wii File-System Table (FST) reader used by
    /// <see cref="RvzPackEncoder"/> to distinguish real-file regions from junk.
    ///
    /// Mirrors Dolphin's <c>FileSystemGCWii</c> offset-to-file-info cache
    /// (<c>m_offset_file_info_cache</c>).
    /// </summary>
    internal sealed class GcFst
    {
        private const int EntrySize = 12;

        /// <summary>File entry with start and end byte offsets on disc.</summary>
        internal struct FileEntry
        {
            public long FileStart;
            public long FileEnd;
        }

        // Sorted ascending by FileEnd for O(log n) upper_bound queries.
        private readonly List<FileEntry> _files;

        private GcFst(List<FileEntry> files)
        {
            _files = files;
        }

        /// <summary>
        /// Parses a raw FST binary blob and returns a <see cref="GcFst"/>,
        /// or <c>null</c> if the data is too short or structurally invalid.
        /// </summary>
        /// <param name="fstData">
        /// Raw FST bytes exactly as stored on disc (GameCube) or in decrypted
        /// Wii partition data.
        /// </param>
        /// <param name="offsetShift">
        /// Bit-shift to convert raw file-offset fields to byte addresses.
        /// 0 for GameCube (direct bytes); 2 for Wii (offset × 4).
        /// </param>
        public static GcFst? TryParse(byte[] fstData, int offsetShift)
        {
            if (fstData == null || fstData.Length < EntrySize)
                return null;

            // Root entry (index 0): FILE_SIZE field = total number of FST entries.
            int rootOffset = 8;
            uint totalEntries = fstData.ReadUInt32BigEndian(ref rootOffset);
            if (totalEntries < 1 || ((long)totalEntries * EntrySize) > fstData.Length)
                return null;

            var files = new List<FileEntry>((int)(totalEntries - 1));

            for (uint i = 1; i < totalEntries; i++)
            {
                int  off           = (int)(i * EntrySize);
                int  nameOffPos    = off;
                int  fileOffPos    = off + 4;
                int  fileSizePos   = off + 8;
                uint nameOffField  = fstData.ReadUInt32BigEndian(ref nameOffPos);
                uint fileOffField  = fstData.ReadUInt32BigEndian(ref fileOffPos);
                uint fileSizeField = fstData.ReadUInt32BigEndian(ref fileSizePos);

                if ((nameOffField & 0xFF000000u) != 0) continue; // directory entry
                if (fileSizeField == 0)                continue; // empty file

                long fileStart = (long)fileOffField << offsetShift;
                long fileEnd   = fileStart + fileSizeField;
                files.Add(new FileEntry { FileStart = fileStart, FileEnd = fileEnd });
            }

            // Sort ascending by FileEnd so binary-search upper_bound works correctly.
            files.Sort(delegate(FileEntry a, FileEntry b)
            {
                return a.FileEnd.CompareTo(b.FileEnd);
            });

            return new GcFst(files);
        }

        /// <summary>
        /// Returns the file entry whose byte range contains <paramref name="discOffset"/>,
        /// or <c>null</c> if no file does.
        /// </summary>
        public FileEntry? FindFileInfo(long discOffset)
        {
            if (_files.Count == 0)
                return null;

            // Binary search: first index where _files[i].FileEnd > discOffset
            int lo = 0, hi = _files.Count;
            while (lo < hi)
            {
                int mid = (lo + hi) >> 1;
                if (_files[mid].FileEnd <= discOffset)
                    lo = mid + 1;
                else
                    hi = mid;
            }

            if (lo >= _files.Count)
                return null;

            FileEntry e = _files[lo];
            if (e.FileStart <= discOffset)
                return e;

            return null;
        }

        /// <summary>
        /// Returns the smallest FileEnd value strictly greater than
        /// <paramref name="discOffset"/>, or <c>null</c> if there is none.
        /// </summary>
        public long? FindNextFileEnd(long discOffset)
        {
            if (_files.Count == 0)
                return null;

            int lo = 0, hi = _files.Count;
            while (lo < hi)
            {
                int mid = (lo + hi) >> 1;
                if (_files[mid].FileEnd <= discOffset)
                    lo = mid + 1;
                else
                    hi = mid;
            }

            return lo < _files.Count ? _files[lo].FileEnd : null;
        }

        /// <summary>
        /// Returns the smallest FileStart value strictly greater than
        /// <paramref name="discOffset"/>, or <c>null</c> if there is none.
        /// </summary>
        public long? FindNextFileStart(long discOffset)
        {
            if (_files.Count == 0)
                return null;

            // Sort is by FileEnd; scan all entries whose FileEnd > discOffset
            int lo = 0, hi = _files.Count;
            while (lo < hi)
            {
                int mid = (lo + hi) >> 1;
                if (_files[mid].FileEnd <= discOffset)
                    lo = mid + 1;
                else
                    hi = mid;
            }

            long? best = null;
            for (int i = lo; i < _files.Count; i++)
            {
                long start = _files[i].FileStart;
                if (start <= discOffset)
                    continue;

                if (best == null || start < best.Value)
                    best = start;
            }

            return best;
        }

            }
        }

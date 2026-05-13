using System.Collections.Generic;

// TODO: Remove when IO is updated
namespace SabreTools.Wrappers
{
    /// <summary>
    /// Lightweight GameCube / Wii File-System Table (FST) reader used by
    /// <see cref="RvzPackEncoder"/> to distinguish real-file regions from junk.
    ///
    /// Mirrors Dolphin's FileSystemGCWii offset-to-file-info cache
    /// (m_offset_file_info_cache).
    /// </summary>
    public sealed class FileSystemTableReader
    {
        /// <summary>
        /// File entry with start and end byte offsets on disc
        /// </summary>
        public struct FileEntry
        {
            public long FileStart;
            public long FileEnd;
        }

        /// <remarks>
        /// Sorted ascending by FileEnd for O(log n) upper_bound queries.
        /// </remarks>
        private readonly List<FileEntry> _files;

        private FileSystemTableReader(List<FileEntry> files)
        {
            _files = files;
        }

        /// <summary>
        /// Parses a raw FST binary blob and returns a <see cref="FileSystemTableReader"/>,
        /// or null if the data is too short or structurally invalid.
        /// </summary>
        /// <param name="fstData">
        /// Raw FST bytes exactly as stored on disc (GameCube) or in decrypted
        /// Wii partition data.
        /// </param>
        /// <param name="offsetShift">
        /// Bit-shift to convert raw file-offset fields to byte addresses.
        /// 0 for GameCube (direct bytes); 2 for Wii (offset × 4).
        /// </param>
        public static FileSystemTableReader? TryParse(byte[] fstData, int offsetShift)
        {
            // Read the file system table
            var table = Serialization.Readers.NintendoDisc.ParseFileSystemTable(fstData);
            if (table is null)
                return null;

            // Filter out the entries to non-empty files only
            var filtered = new List<FileEntry>();
            foreach (var entry in table.Entries)
            {
                // Directory entry
                if ((entry.NameOffset & 0xFF000000) != 0)
                    continue;

                // Empty file
                if (entry.FileSize == 0)
                    continue;

                long fileStart = entry.FileOffset << offsetShift;
                long fileEnd = fileStart + entry.FileSize;

                var fileEntry = new FileEntry
                {
                    FileStart = fileStart,
                    FileEnd = fileEnd,
                };
                filtered.Add(fileEntry);
            }

            // Sort ascending by FileEnd so binary-search upper_bound works correctly.
            filtered.Sort(delegate (FileEntry a, FileEntry b)
            {
                return a.FileEnd.CompareTo(b.FileEnd);
            });

            return new FileSystemTableReader(filtered);
        }

        /// <summary>
        /// Returns the file entry whose byte range contains <paramref name="discOffset"/>,
        /// or null if no file does.
        /// </summary>
        /// TODO: Determine how to use List<T>.BinarySearch here
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

            var e = _files[lo];
            if (e.FileStart <= discOffset)
                return e;

            return null;
        }

        /// <summary>
        /// Returns the smallest FileEnd value strictly greater than
        /// <paramref name="discOffset"/>, or null if there is none.
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
        /// <paramref name="discOffset"/>, or null if there is none.
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

using System;
using System.IO;
using SabreTools.Models.InstallShieldCabinet;

namespace SabreTools.Serialization.Wrappers
{
    public partial class InstallShieldCabinet : WrapperBase<Cabinet>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "InstallShield Cabinet";

        #endregion

        #region Extension Properties

        /// <summary>
        /// Reference to the next cabinet header
        /// </summary>
        /// <remarks>Only used in multi-file</remarks>
        public InstallShieldCabinet? Next { get; set; }

        /// <summary>
        /// Number of components in the cabinet set
        /// </summary>
        public int ComponentCount => Model.Components?.Length ?? 0;

        /// <summary>
        /// Number of directories in the cabinet set
        /// </summary>
        public ushort DirectoryCount => Model.Descriptor?.DirectoryCount ?? 0;

        /// <summary>
        /// Number of files in the cabinet set
        /// </summary>
        public uint FileCount => Model.Descriptor?.FileCount ?? 0;

        /// <summary>
        /// Number of file groups in the cabinet set
        /// </summary>
        public int FileGroupCount => Model.FileGroups?.Length ?? 0;

        /// <summary>
        /// Indicates if Unicode strings are used
        /// </summary>
        public bool IsUnicode => MajorVersion >= 17;

        /// <summary>
        /// The major version of the cabinet
        /// </summary>
        public int MajorVersion
        {
            get
            {
                uint majorVersion = Model.CommonHeader?.Version ?? 0;
                if (majorVersion >> 24 == 1)
                {
                    majorVersion = (majorVersion >> 12) & 0x0F;
                }
                else if (majorVersion >> 24 == 2 || majorVersion >> 24 == 4)
                {
                    majorVersion = majorVersion & 0xFFFF;
                    if (majorVersion != 0)
                        majorVersion /= 100;
                }

                return (int)majorVersion;
            }
        }

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public InstallShieldCabinet(Cabinet? model, byte[]? data, int offset)
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
        public InstallShieldCabinet(Cabinet? model, Stream? data)
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create an InstallShield Cabinet from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the cabinet</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A cabinet wrapper on success, null on failure</returns>
        public static InstallShieldCabinet? Create(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and use that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Create(dataStream);
        }

        /// <summary>
        /// Create a InstallShield Cabinet from a Stream
        /// </summary>
        /// <param name="data">Stream representing the cabinet</param>
        /// <returns>A cabinet wrapper on success, null on failure</returns>
        public static InstallShieldCabinet? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanRead)
                return null;

            try
            {
                var cabinet = Deserializers.InstallShieldCabinet.DeserializeStream(data);
                if (cabinet == null)
                    return null;

                return new InstallShieldCabinet(cabinet, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Component

        /// <summary>
        /// Get the component name at a given index, if possible
        /// </summary>
        public string? GetComponentName(int index)
        {
            if (Model.Components == null)
                return null;

            if (index < 0 || index >= Model.Components.Length)
                return null;

            var component = Model.Components[index];
            if (component?.Identifier == null)
                return null;

            return component.Identifier.Replace('\\', '/');
        }

        #endregion

        #region Directory

        /// <summary>
        /// Get the directory name at a given index, if possible
        /// </summary>
        public string? GetDirectoryName(int index)
        {
            if (Model.DirectoryNames == null)
                return null;

            if (index < 0 || index >= Model.DirectoryNames.Length)
                return null;

            return Model.DirectoryNames[index];
        }

        /// <summary>
        /// Get the directory index for the given file index
        /// </summary>
        /// <returns>Directory index if found, UInt32.MaxValue on error</returns>
        public uint GetDirectoryIndexFromFile(int index)
        {
            FileDescriptor? descriptor = GetFileDescriptor(index);
            if (descriptor != null)
                return descriptor.DirectoryIndex;
            else
                return uint.MaxValue;
        }

        #endregion

        #region File

        /// <summary>
        /// Returns if the file at a given index is marked as valid
        /// </summary>
        public bool FileIsValid(int index)
        {
            if (Model.Descriptor == null)
                return false;

            if (index < 0 || index > Model.Descriptor.FileCount)
                return false;

            FileDescriptor? descriptor = GetFileDescriptor(index);
            if (descriptor == null)
                return false;

#if NET20 || NET35
            if ((descriptor.Flags & FileFlags.FILE_INVALID) != 0)
#else
            if (descriptor.Flags.HasFlag(FileFlags.FILE_INVALID))
#endif
                return false;

            if (descriptor.NameOffset == default)
                return false;

            if (descriptor.DataOffset == default)
                return false;

            return true;
        }

        /// <summary>
        /// Get the reported expanded file size for a given index
        /// </summary>
        public ulong GetExpandedFileSize(int index)
        {
            FileDescriptor? descriptor = GetFileDescriptor(index);
            if (descriptor != null)
                return descriptor.ExpandedSize;
            else
                return 0;
        }

        /// <summary>
        /// Get the file descriptor at a given index, if possible
        /// </summary>
        public FileDescriptor? GetFileDescriptor(int index)
        {
            if (Model.FileDescriptors == null)
                return null;

            if (index < 0 || index >= Model.FileDescriptors.Length)
                return null;

            return Model.FileDescriptors[index];
        }

        /// <summary>
        /// Get the file name at a given index, if possible
        /// </summary>
        public string? GetFileName(int index)
        {
            var descriptor = GetFileDescriptor(index);
#if NET20 || NET35
            if (descriptor == null || (descriptor.Flags & FileFlags.FILE_INVALID) != 0)
#else
            if (descriptor == null || descriptor.Flags.HasFlag(FileFlags.FILE_INVALID))
#endif
                return null;

            return descriptor.Name;
        }

        #endregion

        #region File Group

        /// <summary>
        /// Get the file group at a given index, if possible
        /// </summary>
        public FileGroup? GetFileGroup(int index)
        {
            if (Model.FileGroups == null)
                return null;

            if (index < 0 || index >= Model.FileGroups.Length)
                return null;

            return Model.FileGroups[index];
        }

        /// <summary>
        /// Get the file group at a given name, if possible
        /// </summary>
        public FileGroup? GetFileGroup(string name)
        {
            if (Model.FileGroups == null)
                return null;

            return Array.Find(Model.FileGroups, fg => fg != null && string.Equals(fg.Name, name));
        }

        /// <summary>
        /// Get the file group for the given file index, if possible
        /// </summary>
        public FileGroup? GetFileGroupFromFile(int index)
        {
            if (Model.FileGroups == null)
                return null;

            if (index < 0 || index >= FileCount)
                return null;

            for (int i = 0; i < FileGroupCount; i++)
            {
                var fileGroup = GetFileGroup(i);
                if (fileGroup == null)
                    continue;

                if (fileGroup.FirstFile > index || fileGroup.LastFile < index)
                    continue;

                return fileGroup;
            }

            return null;
        }

        /// <summary>
        /// Get the file group name at a given index, if possible
        /// </summary>
        public string? GetFileGroupName(int index)
            => GetFileGroup(index)?.Name;

        /// <summary>
        /// Get the file group name at a given file index, if possible
        /// </summary>
        public string? GetFileGroupNameFromFile(int index)
            => GetFileGroupFromFile(index)?.Name;

        #endregion
    }
}

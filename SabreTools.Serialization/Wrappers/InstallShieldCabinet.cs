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

        /// <inheritdoc cref="Cabinet.CommonHeader"/>
        public CommonHeader? CommonHeader => Model.CommonHeader;

        /// <inheritdoc cref="Cabinet.Components"/>
        public Component[]? Components => Model.Components;

        /// <summary>
        /// Number of components in the cabinet set
        /// </summary>
        public int ComponentCount => Components?.Length ?? 0;

        /// <summary>
        /// Number of directories in the cabinet set
        /// </summary>
        public ushort DirectoryCount => Model.Descriptor?.DirectoryCount ?? 0;

        /// <inheritdoc cref="Cabinet.DirectoryNames"/>
        public string[]? DirectoryNames => Model.DirectoryNames;

        /// <summary>
        /// Number of files in the cabinet set
        /// </summary>
        public uint FileCount => Model.Descriptor?.FileCount ?? 0;

        /// <inheritdoc cref="Cabinet.FileDescriptors"/>
        public FileDescriptor[]? FileDescriptors => Model.FileDescriptors;

        /// <inheritdoc cref="Cabinet.FileGroups"/>
        public FileGroup[]? FileGroups => Model.FileGroups;

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
        public int MajorVersion => Model.GetMajorVersion();

        /// <inheritdoc cref="Cabinet.VolumeHeader"/>
        public VolumeHeader? VolumeHeader => Model.VolumeHeader;

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public InstallShieldCabinet(Cabinet model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public InstallShieldCabinet(Cabinet model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public InstallShieldCabinet(Cabinet model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public InstallShieldCabinet(Cabinet model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public InstallShieldCabinet(Cabinet model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public InstallShieldCabinet(Cabinet model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create an InstallShield Cabinet from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the cabinet</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A cabinet wrapper on success, null on failure</returns>
        public static InstallShieldCabinet? Create(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null || data.Length == 0)
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
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = new Deserializers.InstallShieldCabinet().Deserialize(data);
                if (model == null)
                    return null;

                return new InstallShieldCabinet(model, data, currentOffset);
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
            if (Components == null)
                return null;

            if (index < 0 || index >= ComponentCount)
                return null;

            var component = Components[index];
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
            if (DirectoryNames == null)
                return null;

            if (index < 0 || index >= DirectoryNames.Length)
                return null;

            return DirectoryNames[index];
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
            if (index < 0 || index > FileCount)
                return false;

            FileDescriptor? descriptor = GetFileDescriptor(index);
            if (descriptor == null)
                return false;

            if (descriptor.IsInvalid())
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
            if (FileDescriptors == null)
                return null;

            if (index < 0 || index >= FileDescriptors.Length)
                return null;

            return FileDescriptors[index];
        }

        /// <summary>
        /// Get the file descriptor at a given index, if possible
        /// </summary>
        /// <remarks>Verifies the file descriptor flags before returning</remarks>
        public bool TryGetFileDescriptor(int index, out FileDescriptor? fileDescriptor)
        {
            fileDescriptor = GetFileDescriptor(index);
            if (fileDescriptor == null)
            {
                Console.Error.WriteLine($"Failed to get file descriptor for file {index}");
                return false;
            }

            if (fileDescriptor.IsInvalid() || fileDescriptor.DataOffset == 0)
            {
                Console.Error.WriteLine($"File at {index} is marked as invalid");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get the file name at a given index, if possible
        /// </summary>
        public string? GetFileName(int index)
        {
            var descriptor = GetFileDescriptor(index);
            if (descriptor == null || descriptor.IsInvalid())
                return null;

            return descriptor.Name;
        }

        /// <summary>
        /// Get the packed size of a file, if possible
        /// </summary>
        public static ulong GetReadableBytes(FileDescriptor? descriptor)
        {
            if (descriptor == null)
                return 0;

            return descriptor.IsCompressed()
                ? descriptor.CompressedSize
                : descriptor.ExpandedSize;
        }

        /// <summary>
        /// Get the packed size of a file, if possible
        /// </summary>
        public static ulong GetWritableBytes(FileDescriptor? descriptor)
        {
            if (descriptor == null)
                return 0;

            return descriptor.ExpandedSize;
        }

        #endregion

        #region File Group

        /// <summary>
        /// Get the file group at a given index, if possible
        /// </summary>
        public FileGroup? GetFileGroup(int index)
        {
            if (FileGroups == null)
                return null;

            if (index < 0 || index >= FileGroups.Length)
                return null;

            return FileGroups[index];
        }

        /// <summary>
        /// Get the file group at a given name, if possible
        /// </summary>
        public FileGroup? GetFileGroup(string name)
        {
            if (FileGroups == null)
                return null;

            return Array.Find(FileGroups, fg => fg != null && string.Equals(fg.Name, name));
        }

        /// <summary>
        /// Get the file group for the given file index, if possible
        /// </summary>
        public FileGroup? GetFileGroupFromFile(int index)
        {
            if (FileGroups == null)
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

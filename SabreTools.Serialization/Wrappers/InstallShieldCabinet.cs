using System.IO;
using System.Linq;
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
        /// The major version of the cabinet
        /// </summary>
        public int MajorVersion
        {
            get
            {
                uint majorVersion = this.Model.CommonHeader?.Version ?? 0;
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
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var cabinet = Deserializers.InstallShieldCabinet.DeserializeStream(data);
            if (cabinet == null)
                return null;

            try
            {
                return new InstallShieldCabinet(cabinet, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Accessors

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

            return Model.FileGroups.FirstOrDefault(fg => fg != null && string.Equals(fg.Name, name));
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

        /// <summary>
        /// Get the file group name at a given index, if possible
        /// </summary>
        public string? GetFileGroupName(int index)
        {
            if (Model.FileGroups == null)
                return null;

            if (index < 0 || index >= Model.FileGroups.Length)
                return null;

            var fileGroup = Model.FileGroups[index];
            if (fileGroup == null)
                return null;

            return fileGroup.Name;
        }

        #endregion
    }
}

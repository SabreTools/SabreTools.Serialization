using System;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.CrossModel
{
    public partial class M1 : IModelSerializer<SabreTools.Serialization.Models.Listxml.M1, Serialization.Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public SabreTools.Serialization.Models.Listxml.M1? Deserialize(Serialization.Models.Metadata.MetadataFile? obj)
        {
            if (obj == null)
                return null;

            var header = obj.Read<Serialization.Models.Metadata.Header>(Serialization.Models.Metadata.MetadataFile.HeaderKey);
            var m1 = header != null ? ConvertM1FromInternalModel(header) : new SabreTools.Serialization.Models.Listxml.M1();

            var machines = obj.Read<Serialization.Models.Metadata.Machine[]>(Serialization.Models.Metadata.MetadataFile.MachineKey);
            if (machines != null && machines.Length > 0)
                m1.Game = Array.ConvertAll(machines, Listxml.ConvertMachineFromInternalModel);

            return m1;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Header"/> to <see cref="Models.Listxml.M1"/>
        /// </summary>
        private static SabreTools.Serialization.Models.Listxml.M1 ConvertM1FromInternalModel(Serialization.Models.Metadata.Header item)
        {
            var m1 = new SabreTools.Serialization.Models.Listxml.M1
            {
                Version = item.ReadString(Serialization.Models.Metadata.Header.VersionKey),
            };
            return m1;
        }
    }
}

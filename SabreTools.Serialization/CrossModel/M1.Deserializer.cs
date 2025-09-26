using System;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.CrossModel
{
    public partial class M1 : IModelSerializer<Models.Listxml.M1, Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public Models.Listxml.M1? Deserialize(Models.Metadata.MetadataFile? obj)
        {
            if (obj == null)
                return null;

            var header = obj.Read<Models.Metadata.Header>(Models.Metadata.MetadataFile.HeaderKey);
            var m1 = header != null ? ConvertM1FromInternalModel(header) : new Models.Listxml.M1();

            var machines = obj.Read<Models.Metadata.Machine[]>(Models.Metadata.MetadataFile.MachineKey);
            if (machines != null && machines.Length > 0)
                m1.Game = Array.ConvertAll(machines, Listxml.ConvertMachineFromInternalModel);

            return m1;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Header"/> to <see cref="Models.Listxml.M1"/>
        /// </summary>
        private static Models.Listxml.M1 ConvertM1FromInternalModel(Models.Metadata.Header item)
        {
            var m1 = new Models.Listxml.M1
            {
                Version = item.ReadString(Models.Metadata.Header.VersionKey),
            };
            return m1;
        }
    }
}

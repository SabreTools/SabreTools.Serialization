using System;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.CrossModel
{
    public partial class M1 : IModelSerializer<Data.Models.Listxml.M1, Data.Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public Data.Models.Listxml.M1? Deserialize(Data.Models.Metadata.MetadataFile? obj)
        {
            if (obj == null)
                return null;

            var header = obj.Read<Data.Models.Metadata.Header>(Data.Models.Metadata.MetadataFile.HeaderKey);
            var m1 = header != null ? ConvertM1FromInternalModel(header) : new Data.Models.Listxml.M1();

            var machines = obj.Read<Data.Models.Metadata.Machine[]>(Data.Models.Metadata.MetadataFile.MachineKey);
            if (machines != null && machines.Length > 0)
                m1.Game = Array.ConvertAll(machines, Listxml.ConvertMachineFromInternalModel);

            return m1;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Header"/> to <see cref="Models.Listxml.M1"/>
        /// </summary>
        private static Data.Models.Listxml.M1 ConvertM1FromInternalModel(Data.Models.Metadata.Header item)
        {
            var m1 = new Data.Models.Listxml.M1
            {
                Version = item.ReadString(Data.Models.Metadata.Header.VersionKey),
            };
            return m1;
        }
    }
}

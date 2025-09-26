using System;

namespace SabreTools.Serialization.CrossModel
{
    public partial class Mess : BaseMetadataSerializer<Data.Models.Listxml.Mess>
    {
        /// <inheritdoc/>
        public override Data.Models.Listxml.Mess? Deserialize(Data.Models.Metadata.MetadataFile? obj)
        {
            if (obj == null)
                return null;

            var header = obj.Read<Data.Models.Metadata.Header>(Data.Models.Metadata.MetadataFile.HeaderKey);
            var m1 = header != null ? ConvertMessFromInternalModel(header) : new Data.Models.Listxml.Mess();

            var machines = obj.Read<Data.Models.Metadata.Machine[]>(Data.Models.Metadata.MetadataFile.MachineKey);
            if (machines != null && machines.Length > 0)
                m1.Game = Array.ConvertAll(machines, Listxml.ConvertMachineFromInternalModel);

            return m1;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Header"/> to <see cref="Models.Listxml.Mess"/>
        /// </summary>
        private static Data.Models.Listxml.Mess ConvertMessFromInternalModel(Data.Models.Metadata.Header item)
        {
            var m1 = new Data.Models.Listxml.Mess
            {
                Version = item.ReadString(Data.Models.Metadata.Header.VersionKey),
            };
            return m1;
        }
    }
}

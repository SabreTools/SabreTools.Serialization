using System;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.CrossModel
{
    public partial class Mess : IModelSerializer<Models.Listxml.Mess, Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public Models.Listxml.Mess? Deserialize(Models.Metadata.MetadataFile? obj)
        {
            if (obj == null)
                return null;

            var header = obj.Read<Models.Metadata.Header>(Models.Metadata.MetadataFile.HeaderKey);
            var m1 = header != null ? ConvertMessFromInternalModel(header) : new Models.Listxml.Mess();

            var machines = obj.Read<Models.Metadata.Machine[]>(Models.Metadata.MetadataFile.MachineKey);
            if (machines != null && machines.Length > 0)
                m1.Game = Array.ConvertAll(machines, Listxml.ConvertMachineFromInternalModel);

            return m1;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Header"/> to <see cref="Models.Listxml.Mess"/>
        /// </summary>
        private static Models.Listxml.Mess ConvertMessFromInternalModel(Models.Metadata.Header item)
        {
            var m1 = new Models.Listxml.Mess
            {
                Version = item.ReadString(Models.Metadata.Header.VersionKey),
            };
            return m1;
        }
    }
}

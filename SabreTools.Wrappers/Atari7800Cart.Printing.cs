using System.Text;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.Atari7800;

namespace SabreTools.Wrappers
{
    public partial class Atari7800Cart : IPrintable
    {
#if NETCOREAPP
        /// <inheritdoc/>
        public string ExportJSON() => System.Text.Json.JsonSerializer.Serialize(Model, _jsonSerializerOptions);
#endif

        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder)
        {
            builder.AppendLine("Atari 7800 Cart Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            Print(builder, Model.Header);

            //builder.AppendLine(Model.Data, "ROM Data");
            builder.AppendLine(Model.Data.Length, "ROM Data Length");
        }

        private static void Print(StringBuilder builder, Header? header)
        {
            builder.AppendLine("  Header Information:");
            builder.AppendLine("  -------------------------");
            if (header is null)
            {
                builder.AppendLine("  No header present");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(header.HeaderVersion, "  Header version");
            builder.AppendLine(header.MagicText, "  Magic text");
            builder.AppendLine(Encoding.ASCII.GetString(header.MagicText).TrimEnd('\0'), "  Magic text (ASCII)");
            builder.AppendLine(header.CartTitle, "  Cart title");
            builder.AppendLine(Encoding.ASCII.GetString(header.CartTitle).TrimEnd('\0'), "  Cart title (ASCII)");
            builder.AppendLine(header.RomSizeWithoutHeader, "  ROM size without header");

            string cartType = header.CartType.FromCartType();
            builder.AppendLine(cartType, "  Cart type flags");

            string controller1Type = header.Controller1Type.FromControllerType();
            builder.AppendLine(controller1Type, "  Controller 1 type");

            string controller2Type = header.Controller2Type.FromControllerType();
            builder.AppendLine(controller2Type, "  Controller 2 type");

            string tvType = header.TVType.FromTVType();
            builder.AppendLine(tvType, "  TV type");

            string saveDevice = header.SaveDevice.FromSaveDevice();
            builder.AppendLine(saveDevice, "  Save device");

            builder.AppendLine(header.Reserved, "  Reserved");

            string slotPassthroughDevice = header.SlotPassthroughDevice.FromSlotPassthroughDevice();
            builder.AppendLine(slotPassthroughDevice, "  Slot passthrough device");

            if (header.HeaderVersion >= 4)
            {
                string mapper = header.Mapper.FromMapper();
                builder.AppendLine(mapper, "  Mapper");

                string mapperOptions = header.MapperOptions.FromMapperOptions();
                builder.AppendLine(mapperOptions, "  Mapper options");

                string audioDevice = header.AudioDevice.FromAudioDevice();
                builder.AppendLine(audioDevice, "  Audio device");

                string interrupt = header.Interrupt.FromInterrupt();
                builder.AppendLine(interrupt, "  Interrupt");

                builder.AppendLine(header.Padding, "  Padding");
            }
            else
            {
                builder.AppendLine(header.Padding, "  Padding");
            }

            builder.AppendLine(header.EndMagicText, "  End magic text");
            builder.AppendLine(Encoding.ASCII.GetString(header.EndMagicText).TrimEnd('\0'), "  End magic text (ASCII)");
            builder.AppendLine();
        }
    }
}

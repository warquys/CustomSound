using Synapse.Command;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
    
namespace CustomSound
{
    [CommandInformation(
        Name = "PlaySound",
        Aliases = new[] { "sound" },
        Description = "Play a specifique sound file",
        Usage = "target a specifique mpg file",
        Permission = "playsound",
        Arguments = new[] { "filename" },
        Platforms = new[] { Platform.RemoteAdmin }
        )]
    public class Command : ISynapseCommand
    {

        public CommandResult Execute(CommandContext context)
        {
            var result = new CommandResult();

            var mpgFilePath = string.Join(" ", context.Arguments.ToArray());

            if (mpgFilePath == string.Empty)
            {
                result.Message = "Specify the chosen file";
                result.State = CommandResultState.Ok; 
                return result;
            }


            if (!File.Exists(mpgFilePath))
            {
                result.Message = $"File not found : {mpgFilePath}";
                result.State = CommandResultState.Ok;
                return result;
            }

            if (Path.GetExtension(mpgFilePath) != ".mpg")
            {
                result.Message = $"File is not an mpg : {mpgFilePath}";
                result.State = CommandResultState.Ok;
                return result;
            }

            result.Message = "File play";
            result.State = AudioManager.Get.Play(mpgFilePath) ? CommandResultState.Ok : CommandResultState.Error;

            return result;
        }
    }
}

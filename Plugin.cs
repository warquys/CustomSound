using HarmonyLib;
using Synapse.Api.Plugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CustomSound
{
    [PluginInformation(
        Author = "VT",
        Description = "Add a new command use to player new sound",
        Name = "CustomSound",
        SynapseMajor = SynapseVersion.Major,
        SynapseMinor = SynapseVersion.Minor,
        SynapsePatch = SynapseVersion.Patch,
        Version = "1.0.0"
        )]
    public class Plugin : AbstractPlugin
    {
        public Harmony Harmony { get; private set; }
        public Plugin Instance { get; private set; }

        public override void Load()
        {
            Instance = this;
            AudioManager.Get.Init();
            base.Load();
        }

        private void Patch()
        {
            Harmony = new Harmony("CustomSound");
            Harmony.PatchAll();
        }

    }
}
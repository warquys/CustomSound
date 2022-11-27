using Dissonance;
using Dissonance.Integrations.MirrorIgnorance;
using Dissonance.Networking;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CustomSound
{
    [HarmonyPatch(typeof(BaseCommsNetwork<MirrorIgnoranceServer, MirrorIgnoranceClient, MirrorConn, Unit, Unit>), "RunAsDedicatedServer")]
    class DissonanceHostPatch
    {
        public static bool RunAsDedicatedServerPatch(BaseCommsNetwork<MirrorIgnoranceServer, MirrorIgnoranceClient, MirrorConn, Unit, Unit> __instance)
        {
            __instance.CallMethod("RunAsHost", new object[] { Unit.None, Unit.None });
            return false;
        }
    }
}

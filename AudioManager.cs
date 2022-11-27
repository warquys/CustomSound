using MEC;
using Synapse.Api;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CustomSound
{
    internal class AudioManager
    {
        #region Attributes & Properties
        private static AudioManager singleton = new AudioManager();
        public static AudioManager Get => singleton;

        private Controller _controller;

        #endregion

        #region Constructors & Destructor
        internal AudioManager() { }
       
        #endregion

        #region Methods

        internal void Init()
        {
            _controller = new Controller();
        }

        public void Loop(bool enabled)
        {
            _controller.Loop = enabled;
        }

        private void UnmutePlayer(Player player)
        {
            var id = player.Radio.mirrorIgnorancePlayer._playerId;
            _controller.UnMutePlayer(id);
        }

        private void MutePlayer(Player player)
        {
            var id = player.Radio.mirrorIgnorancePlayer._playerId;
            if (_controller.MutedPlayers.Contains(id))
                return;
            _controller.MutePlayer(id);
        }

        public bool Play(string mpgFilePath)
        {
            if (!File.Exists(mpgFilePath))
            {
                Synapse.Api.Logger.Get.Info($"File not found : {mpgFilePath}");
                return false;
            }

            if (Path.GetExtension(mpgFilePath) != ".mpg")
            {
                Synapse.Api.Logger.Get.Info($"File is not an mpg : {mpgFilePath}");
                return false;
            }

            Timing.RunCoroutine(_controller.PlayFromFile(mpgFilePath));
            return true;
        }

        public void Stop()
        {
            _controller.Stop();
        }

        public void Volume(uint volume)
        {
            _controller.Volume = Mathf.Clamp(volume, 0, 100) / 100;
            _controller.RefreshChannels();

        }
        #endregion
    }
}

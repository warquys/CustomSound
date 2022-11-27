using Assets._Scripts.Dissonance;
using Dissonance;
using MEC;
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
    internal class Controller
    {
        #region Attributes & Properties
        public DissonanceComms Comms => Radio.comms;
        public FakeMicrophone Microphone;

        public bool Loop { get; set; }

        public float _volume;
        /// <summary>
        /// It was in %
        /// </summary>
        public float Volume
        {
            get
            {
                return _volume * 100;
            }
            set
            {
                _volume = Mathf.Clamp(value, 0, 100) / 100;
            }
        }

        public List<string> MutedPlayers { get; } = new List<string>();
        #endregion

        #region Constructors & Destructor

        public Controller()
        {
            Microphone = Comms.gameObject.AddComponent<FakeMicrophone>();
            Microphone.AudioController = this;
            InitEvents();
        }
        #endregion

        #region Methods
        public void UnMutePlayer(string playerId)
        {
            MutedPlayers.Remove(playerId);
            Comms.PlayerChannels.Open(playerId, false, ChannelPriority.Default, _volume);
        }

        public void MutePlayer(string playerId)
        {
            MutedPlayers.Add(playerId);
            var channel = Comms.PlayerChannels._openChannelsBySubId.FirstOrDefault(x => x.Value.TargetId == playerId);
            Comms.PlayerChannels.Close(channel.Value);
        }

        private void InitEvents()
        {
            Synapse.Server.Get.Events.Round.RoundRestartEvent += OnRestartingRound;
            Synapse.Server.Get.Events.Round.WaitingForPlayersEvent += OnWaitingForPlayers;

        }

        public IEnumerator<float> PlayFromFile(string path, float volume = 100, bool loop = false)
        {
            if (string.IsNullOrWhiteSpace(path))
                yield break;

            if (!File.Exists(path))
            {
                Synapse.Api.Logger.Get.Error($"Error File not found: {path}.");
                yield break;
            }

            Stop();

            yield return Timing.WaitForOneFrame;
            yield return Timing.WaitForOneFrame;

            Volume = volume;
            RefreshChannels();

            Microphone.File = new FileStream(path, FileMode.Open);
            Microphone.Stop = false;
            Comms._capture.SetField("_microphone", Microphone);
            Comms.ResetMicrophoneCapture();
            Comms.IsMuted = false;
            Loop = loop;
        }

        public void Stop()
        {
            if (Microphone != null)
                Microphone.Stop = true;
        }

        public void RefreshChannels()
        {
            foreach (var channel in Comms.PlayerChannels._openChannelsBySubId.Values.ToList())
            {
                Comms.PlayerChannels.Close(channel);
                Comms.PlayerChannels.Open(channel.TargetId, false, ChannelPriority.Default, _volume);
            }
        }

        #endregion


        #region Events
        private void OnRestartingRound()
        {
            Comms.OnPlayerJoinedSession -= OnPlayerJoinedSession;
        }

        private void OnWaitingForPlayers()
        {
            Comms.OnPlayerJoinedSession += OnPlayerJoinedSession;
            Synapse.Server.Get.Host.Radio.Network_syncPrimaryVoicechatButton = true;
            Synapse.Server.Get.Host.DissonanceUserSetup.NetworkspeakingFlags = SpeakingFlags.IntercomAsHuman;
        }

        private void OnPlayerJoinedSession(VoicePlayerState player)
        {
            Comms.PlayerChannels.Open(player.Name, false, ChannelPriority.Default, _volume);
        }

        #endregion
    }
}

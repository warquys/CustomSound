using Dissonance.Audio.Capture;
using Dissonance.Networking;
using NAudio.Wave;
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
    internal class FakeMicrophone : MonoBehaviour, IMicrophoneCapture
    {

        #region Properties & Variable
        public bool IsRecording { get; private set; }
        public TimeSpan Latency { get; private set; }

        public FileStream File;

        public bool Stop { get; set; }

        private readonly List<IMicrophoneSubscriber> _subscribers = new List<IMicrophoneSubscriber>();

        private readonly WaveFormat _format = new WaveFormat(48000, 1);
        private readonly float[] _frame = new float[960];
        private readonly byte[] _frameBytes = new byte[960 * sizeof(float)];

        private float _elapsedTime;

        public Controller AudioController { get; set; }
        #endregion

        #region Methods
        public WaveFormat StartCapture(string micName)
        {
            if (Stop)
                return null;

            //Synapse.Api.Logger.Get.Info("Starting capture.");
            AudioController.Comms._capture.SetField("_network", AudioController.Comms.GetFieldValueOrPerties<ICommsNetwork>("_net"));

            IsRecording = true;
            Latency = TimeSpan.Zero;
            return _format;
        }

        public void StopCapture()
        {
            //Synapse.Api.Logger.Get.Info("Stopping capture.");

            if (File == null)
                return;

            File.Dispose();
            File = null;
        }

        public void Subscribe(IMicrophoneSubscriber listener) => _subscribers.Add(listener);
        public bool Unsubscribe(IMicrophoneSubscriber listener) => _subscribers.Remove(listener);

        public bool UpdateSubscribers()
        {
            _elapsedTime += Time.unscaledDeltaTime;

            while (_elapsedTime > 0.02f)
            {
                _elapsedTime -= 0.02f;
                var readLength = File.Read(_frameBytes, 0, _frameBytes.Length);
                Array.Clear(_frame, 0, _frame.Length);
                Buffer.BlockCopy(_frameBytes, 0, _frame, 0, readLength);

                foreach (var subscriber in _subscribers)
                    subscriber.ReceiveMicrophoneData(new ArraySegment<float>(_frame), _format);
            }
            if (Stop)
                return true;
            if (File.Position != File.Length)
                return false;

            if (AudioController.Loop)
                File.Position = 0;
            else
            {
                Stop = true;
                AudioController.Stop();
                return true;
            }


            return false;
        }
        #endregion
    }
}

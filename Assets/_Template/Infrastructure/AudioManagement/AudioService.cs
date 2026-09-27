using System.Collections.Generic;
using UnityEngine;

namespace _Template.Infrastructure.AudioManagement
{
    public class AudioService : IAudioService
    {
        private readonly Dictionary<string, AudioData> _audios;

        private readonly AudioPlayerPool _playerPool;

        public AudioService(AudioData[] audioDatas)
        {
            _audios =
                new Dictionary<string, AudioData>();

            var rootObject =
                new GameObject("[AudioService]");

            Object.DontDestroyOnLoad(rootObject);

            var root =
                rootObject.transform;

            _playerPool =
                new AudioPlayerPool(root);

            RegisterAudios(audioDatas);
        }

        private void RegisterAudios(
            AudioData[] audioDatas)
        {
            foreach (var audioData in audioDatas)
            {
                if (audioData == null)
                    continue;

                if (string.IsNullOrWhiteSpace(
                    audioData.id))
                {
                    Debug.LogError(
                        $"AudioData '{audioData.name}' " +
                        $"has an empty ID.");

                    continue;
                }

                if (_audios.ContainsKey(
                    audioData.id))
                {
                    Debug.LogError(
                        $"Duplicate Audio ID detected: " +
                        $"'{audioData.id}'.");

                    continue;
                }

                _audios.Add(
                    audioData.id,
                    audioData);
            }
        }

        public void Play(
            string audioID,
            Vector3 position = default,
            Transform targetFollow = null)
        {
            if (!TryGetAudioData(
                audioID,
                out var audioData))
            {
                return;
            }

            var player =
                _playerPool.Get(audioData);

            player.Play(
                position,
                targetFollow);
        }

        public void Stop(string audioID)
        {
            _playerPool.Stop(audioID);
        }

        public void StopAll()
        {
            _playerPool.StopAll();
        }

        private bool TryGetAudioData(
            string audioID,
            out AudioData audioData)
        {
            if (_audios.TryGetValue(
                audioID,
                out audioData))
            {
                return true;
            }

            Debug.LogWarning(
                $"Audio with ID '{audioID}' " +
                $"was not found.");

            return false;
        }
    }
}
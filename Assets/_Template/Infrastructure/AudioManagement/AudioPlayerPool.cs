using System.Collections.Generic;
using UnityEngine;

namespace _Template.Infrastructure.AudioManagement
{
    public class AudioPlayerPool
    {
        private readonly Transform _root;

        private readonly Stack<AudioPlayer>
            _availablePlayers;

        private readonly HashSet<AudioPlayer>
            _activePlayers;

        public AudioPlayerPool(Transform root)
        {
            _root = root;

            _availablePlayers =
                new Stack<AudioPlayer>();

            _activePlayers =
                new HashSet<AudioPlayer>();
        }

        public AudioPlayer Get(
            AudioData audioData)
        {
            AudioPlayer player;

            if (_availablePlayers.Count > 0)
            {
                player =
                    _availablePlayers.Pop();
            }
            else
            {
                player =
                    CreatePlayer();
            }

            _activePlayers.Add(player);

            player.Initialize(
                audioData,
                Release);

            player.gameObject.SetActive(true);

            return player;
        }

        public void Release(
            AudioPlayer player)
        {
            if (!_activePlayers.Remove(player))
                return;

            player.Reset();

            player.transform.SetParent(
                _root);

            player.gameObject.SetActive(false);

            _availablePlayers.Push(
                player);
        }

        public void Stop(
            string audioID)
        {
            var players =
                new List<AudioPlayer>(
                    _activePlayers);

            foreach (var player in players)
            {
                if (player.AudioID ==
                    audioID)
                {
                    player.Stop();
                }
            }
        }

        public void StopAll()
        {
            var players =
                new List<AudioPlayer>(
                    _activePlayers);

            foreach (var player in players)
            {
                player.Stop();
            }
        }

        private AudioPlayer CreatePlayer()
        {
            var playerObject =
                new GameObject(
                    "AudioPlayer");

            playerObject.transform.SetParent(
                _root);

            var player =
                playerObject.AddComponent<AudioPlayer>();

            playerObject.SetActive(false);

            return player;
        }
    }
}
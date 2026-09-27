using System;
using UnityEngine;

namespace _Template.Infrastructure.AudioManagement
{
    public class AudioPlayer : MonoBehaviour
    {
        private AudioSource _audioSource;
        private AudioData _audioData;

        private Transform _followTarget;
        private Action<AudioPlayer> _onFinished;

        private bool _finished;

        public string AudioID =>
            _audioData != null
                ? _audioData.id
                : string.Empty;

        public void Initialize(
            AudioData audioData,
            Action<AudioPlayer> onFinished)
        {
            _audioData = audioData;
            _onFinished = onFinished;

            if (_audioSource == null)
            {
                _audioSource =
                    gameObject.AddComponent<AudioSource>();
            }

            ConfigureAudioSource();
        }

        private void ConfigureAudioSource()
        {
            if (_audioData.clips == null ||
                _audioData.clips.Length == 0)
            {
                Debug.LogError(
                    $"AudioData '{_audioData.id}' has no clips.");

                return;
            }

            int clipIndex = UnityEngine.Random.Range(
                0,
                _audioData.clips.Length);

            _audioSource.clip =
                _audioData.clips[clipIndex];

            _audioSource.outputAudioMixerGroup =
                _audioData.audioMixer;

            _audioSource.volume =
                _audioData.volume;

            _audioSource.loop =
                _audioData.loop;

            _audioSource.spatialBlend =
                _audioData.spatialBlend;

            _audioSource.minDistance =
                _audioData.minDistance;

            _audioSource.maxDistance =
                _audioData.maxDistance;

            _audioSource.playOnAwake = false;
        }

        public void Play(
            Vector3 position,
            Transform target = null)
        {
            _finished = false;

            _followTarget = target;

            transform.position =
                target != null
                    ? target.position
                    : position;

            _audioSource.Play();
        }

        public void Stop()
        {
            if (_audioSource == null)
                return;

            _audioSource.Stop();

            Finish();
        }

        private void Update()
        {
            if (_followTarget != null)
            {
                transform.position =
                    _followTarget.position;
            }

            if (_audioData == null)
                return;

            if (_audioData.loop)
                return;

            if (!_audioSource.isPlaying)
            {
                Finish();
            }
        }

        private void Finish()
        {
            if (_finished)
                return;

            _finished = true;

            _onFinished?.Invoke(this);
        }

        public void Reset()
        {
            _audioSource.Stop();

            _audioSource.clip = null;

            _audioSource.volume = 1f;
            _audioSource.loop = false;

            _followTarget = null;
            _audioData = null;
            _onFinished = null;

            _finished = false;

            transform.localPosition =
                Vector3.zero;

            transform.localRotation =
                Quaternion.identity;
        }
    }
}
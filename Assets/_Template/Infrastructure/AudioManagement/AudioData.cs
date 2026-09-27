using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "NewAudioData", menuName = "Scriptable Objects/AudioData")]
public class AudioData : ScriptableObject
{
    public string id;
    public AudioClip[] clips;
    public AudioMixerGroup audioMixer;

    [Range(0f, 1f)]
    public float volume = 1f;

    public bool loop;

    [Range(0f, 1f)]
    public float spatialBlend = 1f;

    public float minDistance = 1f;
    public float maxDistance = 50f;
}

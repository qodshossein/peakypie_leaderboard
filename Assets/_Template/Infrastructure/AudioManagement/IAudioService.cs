using UnityEngine;

namespace _Template.Infrastructure.AudioManagement
{
    public interface IAudioService
    {
        public void Play(string audioID, Vector3 position = new Vector3(), Transform targetFollow = null);
        public void Stop(string audioID);
        public void StopAll();
    }
}

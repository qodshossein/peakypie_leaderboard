using UnityEngine;

namespace _Template.Infrastructure.GameManagement
{
    public interface IGameService
    {
        public void Lose();
        public void Win();
        public void Pause();
    }
}

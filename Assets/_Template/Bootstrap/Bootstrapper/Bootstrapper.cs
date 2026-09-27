using _Template.Core.EventSystem;
using _Template.Core.EventSystem.Events.Application;
using _Template.Infrastructure.SceneManagement;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _Template.Bootstrap.Bootstrapper
{
    public class Bootstrapper : IInitializable
    {
        private readonly ISceneLoader _sceneLoader;
        private readonly IEventService _eventService;

        public Bootstrapper(ISceneLoader sceneLoader, IEventService eventService)
        {
            _sceneLoader = sceneLoader;
            _eventService = eventService;
        }

        public void Initialize()
        {
            Debug.Log("Game Bootstrap Initialized");
            _ = Start();
        }

        private async Task Start()
        {
            var startTime = Time.time;

            await _sceneLoader.LoadGroup("Leaderboard");

            var bootTime = Time.time - startTime;
            _eventService.Publish(new ApplicationBootEvent(bootTime));
        }
    }
}

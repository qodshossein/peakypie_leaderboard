using _Template.Core.EventSystem;
using _Template.Core.EventSystem.Events.Scene;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace _Template.Infrastructure.SceneManagement
{
    public class SceneLoader : ISceneLoader
    {
        public List<SceneInstance> CurrentScenes { get; private set; }

        private EventService _eventService;

        public SceneLoader(EventService eventService)
        {
            _eventService = eventService;
        }
        public async Task LoadBaseScene(string sceneName)
        {
            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }

        public async Task<List<SceneInstance>> LoadGroup(string label)
        {
            if (CurrentScenes == null) CurrentScenes = new List<SceneInstance>();

            for (int i = 0; i < CurrentScenes.Count; i++)
            {
                await UnloadScene(CurrentScenes[i]);
            }
            CurrentScenes.Clear();

            var handle = Addressables.LoadResourceLocationsAsync(
                label,
                typeof(SceneInstance));

            var locations = await handle.Task;

            var scenes = new List<SceneInstance>();

            foreach (var location in locations)
            {
                var sceneHandle = Addressables.LoadSceneAsync(
                    location,
                    LoadSceneMode.Additive);

                var sceneInstance = await sceneHandle.Task;

                scenes.Add(sceneInstance);
            }

            Addressables.Release(handle);

            CurrentScenes.AddRange(scenes);

            _eventService.Publish(new SceneLoadEvent(scenes.ToArray(), label));

            return scenes;
        }

        public async Task UnloadScene(string sceneName)
        {
            await SceneManager.UnloadSceneAsync(sceneName);
        }
        public async Task UnloadScene(SceneInstance sceneInstance)
        {
            var op = Addressables.UnloadSceneAsync(sceneInstance);
            var task = await op.Task;
        }
    }
}
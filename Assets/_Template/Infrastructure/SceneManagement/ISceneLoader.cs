using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace _Template.Infrastructure.SceneManagement
{
    public interface ISceneLoader
    {
        Task LoadBaseScene(string sceneName);
        Task<List<SceneInstance>> LoadGroup(string sceneName);
        Task UnloadScene(string sceneName);
        Task UnloadScene(SceneInstance sceneInstance);
    }
}

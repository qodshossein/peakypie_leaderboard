using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace _Template.Core.EventSystem.Events.Scene
{
    public class SceneLoadEvent
    {
        public SceneInstance[] scenesInstance;
        public string Label;

        public SceneLoadEvent(SceneInstance[] scenes, string label)
        {
            scenesInstance = scenes;
            Label = label;

            var sceneNames = label + " (";
            for (int i = 0; i < scenes.Length; i++)
            {
                sceneNames += scenes[i].Scene.name + " _ ";
            }

            Debug.Log(sceneNames + ") ---- is Loaded");
        }
    }
}

using _Template.Core.EventSystem;
using _Template.Core.InputSystem;
using _Template.Core.SaveSystem;
using _Template.Core.SaveSystem.Storage;
using Zenject;

namespace _Template.Core.DI
{
    public class CoreInstaller : Installer<CoreInstaller>
    {
        public override void InstallBindings()
        {
            UnityEngine.Debug.Log("Core Installer Initialized");

            Container.BindInterfacesAndSelfTo<EventService>().AsSingle();
            Container.BindInterfacesAndSelfTo<LocalSaveStorage>().AsSingle();
            Container.BindInterfacesAndSelfTo<ServerSaveStorage>().AsSingle();
            Container.BindInterfacesAndSelfTo<SaveService>().AsSingle();
            Container.BindInterfacesAndSelfTo<InputService>().AsSingle();
        }
    }
}
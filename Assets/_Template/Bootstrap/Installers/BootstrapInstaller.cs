using Zenject;

namespace _Template.Bootstrap.Installers
{
    public class BootstrapInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<Bootstrapper.Bootstrapper>()
                .AsSingle()
                .NonLazy();
        }
    }
}
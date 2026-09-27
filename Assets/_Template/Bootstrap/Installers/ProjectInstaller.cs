using _Template.Core.DI;
using _Template.Infrastructure.Installer;
using Zenject;

namespace _Template.Scripts.Bootstrap
{
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            CoreInstaller.Install(Container);
            InfrastructureInstaller.Install(Container);
        }
    }
}
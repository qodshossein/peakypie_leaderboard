using _Template.Infrastructure.AudioManagement;
using _Template.Infrastructure.GameManagement;
using _Template.Infrastructure.SceneManagement;
using _Template.Infrastructure.UIManagement;
using _Project.Scripts.Leaderboard;
using Zenject;

namespace _Template.Infrastructure.Installer
{
    public class InfrastructureInstaller : Installer<InfrastructureInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<SceneLoader>()
                .AsSingle();
            Container.BindInterfacesTo<GameService>()
                .AsSingle();
            Container.BindInterfacesTo<UIService>()
                .AsSingle();

            Container.Bind<AudioData>().FromResources("Audios").AsSingle().NonLazy();

            Container.BindInterfacesTo<AudioService>()
                .AsSingle().NonLazy();

            Container.Bind<LeaderboardRowUI>().FromComponentInNewPrefabResource("Prefabs/UI/User/UserUI").AsSingle();
        }
    }
}

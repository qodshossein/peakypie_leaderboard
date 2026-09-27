using TMPro;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Leaderboard.Installers
{
    public class UISceneInstaller : MonoInstaller
    {
        [SerializeField] private TMP_InputField searchInput;
        public override void InstallBindings()
        {
            Container.Bind<Canvas>().WithId("LeaderboardCanvas").FromComponentInHierarchy().AsSingle();
            Container.Bind<LeaderboardLoader>().FromNew().AsSingle().Lazy();
            Container.Bind<LeaderboardScrollView>().FromComponentInHierarchy().AsSingle().Lazy();

            Container.BindInstance(searchInput).WithId("SearchID");
        }
    }
}
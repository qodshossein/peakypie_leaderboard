using _Template.Infrastructure.UIManagement;
using _Project.Scripts.Leaderboard;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Scripts.UI.Panels
{
    public class StartPanel : Panel
    {
        [SerializeField] private Button showLeaderboardPanel;

        private IUIService _uiService;

        [Inject]
        private void Construct(LeaderboardLoader leaderboardLoader, IUIService uiService)
        {
            _uiService = uiService;
        }
        private void Start()
        {
            showLeaderboardPanel.onClick.AddListener(() =>
            {
                _uiService.OpenPanel("LeaderboardPanel");
            });
        }
    }
}

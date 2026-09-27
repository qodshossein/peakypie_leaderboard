using _Template.Core.EventSystem;
using _Template.Core.EventSystem.Events;
using _Template.Infrastructure.UIManagement;
using _Project.Scripts.Leaderboard;
using Zenject;
using UnityEngine;
using _Project.Scripts.Events.Leaderboard;

public class LeaderboardPanel : Panel
{
    [SerializeField] private GameObject loadingObject;

    private LeaderboardLoader _leaderboardLoader;
    private IEventService _eventService;
    private LeaderboardScrollView _scrollView;

    [Inject]
    private void Construct(LeaderboardLoader leaderboardLoader, IEventService eventService, LeaderboardScrollView leaderboardScrollView)
    {
        _leaderboardLoader = leaderboardLoader;
        _eventService = eventService;
        _scrollView = leaderboardScrollView;
    }

    public override void Open(OpenPanelEvent data)
    {
        base.Open(data);

        _leaderboardLoader.LoadAsync(false);
        loadingObject.SetActive(true);

        _eventService.Subscribe<LeaderboardLoadAllDataEvent>(OnLoadAllData);
    }
    public override void Close(ClosePanelEvent data)
    {
        base.Close(data);
        _eventService.Unsubscribe<LeaderboardLoadAllDataEvent>(OnLoadAllData);
    }

    private void OnLoadAllData(LeaderboardLoadAllDataEvent data)
    {
        _scrollView.SetData(data.LeaderboardEntries);
        loadingObject.SetActive(false);
    }
}

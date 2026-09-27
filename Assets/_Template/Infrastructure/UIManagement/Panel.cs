using _Template.Core.EventSystem;
using _Template.Core.EventSystem.Events;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace _Template.Infrastructure.UIManagement
{
    public class Panel : MonoBehaviour, IPanel
    {
        [SerializeField] private string panelName;
        [SerializeField] private float openDuration = 0.3f;
        [SerializeField] private float closeDuration = 0.3f;

        private CanvasGroup _canvasGroup;

        private EventService _eventService;

        [Inject]
        private void Construct(EventService eventService)
        {
            _eventService = eventService;

            _eventService.Subscribe<OpenPanelEvent>(Open);
            _eventService.Subscribe<ClosePanelEvent>(Close);

            _canvasGroup = GetComponent<CanvasGroup>();
        }
        private void OnDestroy()
        {
            _eventService.Unsubscribe<OpenPanelEvent>(Open);
            _eventService.Unsubscribe<ClosePanelEvent>(Close);
        }
        public virtual void Open(OpenPanelEvent data)
        {
            if (data.PanelName != panelName) return;

            _canvasGroup.DOFade(1, openDuration);
            _canvasGroup.blocksRaycasts = true;
        }
        public virtual void Close(ClosePanelEvent data)
        {
            if (data.PanelName != panelName) return;

            _canvasGroup.DOFade(0, closeDuration);
            _canvasGroup.blocksRaycasts = false;
        }
    }
}

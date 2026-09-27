using _Template.Core.EventSystem;
using _Template.Core.EventSystem.Events;
using UnityEngine;

namespace _Template.Infrastructure.UIManagement
{
    public class UIService : IUIService
    {
        private IEventService _eventService;
        private UIService(IEventService eventService)
        {
            _eventService = eventService;
        }
        public void OpenPanel(string panelName)
        {
            var openEvent = new OpenPanelEvent() { PanelName = panelName };
            _eventService.Publish(openEvent);
        }
        public void ClosePanel(string panelName)
        {
            var closeEvent = new ClosePanelEvent() { PanelName = panelName };
            _eventService.Publish(closeEvent);
        }
    }
}

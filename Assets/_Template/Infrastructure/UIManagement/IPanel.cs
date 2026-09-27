using _Template.Core.EventSystem.Events;

namespace _Template.Infrastructure.UIManagement
{
    public interface IPanel
    {
        public void Open(OpenPanelEvent data);
        public void Close(ClosePanelEvent data);
    }
}

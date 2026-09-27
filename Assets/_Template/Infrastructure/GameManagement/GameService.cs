using _Template.Core.EventSystem;
using _Template.Core.EventSystem.Events.Game;

namespace _Template.Infrastructure.GameManagement
{
    public class GameService : IGameService
    {
        private EventService _eventService;
        public GameService(EventService eventService) 
        {
            _eventService = eventService;
        }

        public void Lose()
        {
            _eventService.Publish(new GameLoseEvent());
        }

        public void Pause()
        {
            _eventService.Publish(new GamePauseEvent());
        }

        public void Win()
        {
            _eventService.Publish(new GameWinEvent());
        }
    }
}

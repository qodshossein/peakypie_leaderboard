using System;

namespace _Template.Core.EventSystem
{
    public interface IEventService
    {
        void Subscribe<T>(Action<T> listener);
        void Unsubscribe<T>(Action<T> listener);
        void Publish<T>(T eventData);
    }
}

using System;
using System.Collections.Generic;

namespace _Template.Core.EventSystem
{
    public class EventService : IEventService
    {
        private readonly Dictionary<Type, Delegate> _listeners = new();

        public void Subscribe<T>(Action<T> listener)
        {
            if (listener == null)
                return;

            var type = typeof(T);

            if (_listeners.TryGetValue(type, out var existing))
            {
                _listeners[type] = Delegate.Combine(existing, listener);
            }
            else
            {
                _listeners[type] = listener;
            }
        }

        public void Unsubscribe<T>(Action<T> listener)
        {
            if (listener == null)
                return;

            var type = typeof(T);

            if (!_listeners.TryGetValue(type, out var existing))
                return;

            var updated = Delegate.Remove(existing, listener);

            if (updated == null)
                _listeners.Remove(type);
            else
                _listeners[type] = updated;
        }

        public void Publish<T>(T eventData)
        {
            var type = typeof(T);

            if (!_listeners.TryGetValue(type, out var listeners))
                return;

            if (listeners is Action<T> callback)
            {
                callback.Invoke(eventData);
            }
        }
    }
}

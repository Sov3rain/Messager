using System;
using System.Collections.Generic;

namespace EventAggregation
{
    public class EventAggregator
    {
        private static EventAggregator _instance;

        public static EventAggregator Instance
        {
            get
            {
                if (_instance is null)
                {
                    _instance = new EventAggregator();
                }
                return _instance;
            }
        }

        private readonly Dictionary<Type, HashSet<Action<IEvent>>> _listeners;

        public EventAggregator()
        {
            _listeners = new Dictionary<Type, HashSet<Action<IEvent>>>();
        }

        public void AddListener<T>(Action<IEvent> action) where T : IEvent
        {
            if (!_listeners.ContainsKey(typeof(T)))
            {
                _listeners.Add(typeof(T), new HashSet<Action<IEvent>>());
            }
            _listeners[typeof(T)].Add(action);
        }

        public void RemoveListener<T>(Action<IEvent> action) where T : IEvent
        {
            if (!_listeners.ContainsKey(typeof(T)))
            {
                return;
            }
            _listeners[typeof(T)].Remove(action);
        }

        public void Dispatch<T>() where T : IEvent
        {
            Dispatch(default(T));
        }

        public void Dispatch<T>(T eventData) where T : IEvent
        {
            HashSet<Action<IEvent>> listeners = GetListeners<T>();

            if (listeners is null)
            {
                return;
            }

            foreach (Action<IEvent> action in listeners)
            {
                action(eventData);
            }
        }

        private HashSet<Action<IEvent>> GetListeners<T>()
        {
            if (_listeners.TryGetValue(typeof(T), out var listeners))
            {
                return new HashSet<Action<IEvent>>(listeners);
            }
            return null;
        }
    }
}
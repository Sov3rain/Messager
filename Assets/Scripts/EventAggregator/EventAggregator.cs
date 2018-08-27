using System;
using System.Collections.Generic;

namespace EventAggregation
{
    public static class EventAggregator
    {
        private static Dictionary<Type, List<Action<IEventBase>>> _listeners = new Dictionary<Type, List<Action<IEventBase>>>();

        public static void Subscribe<T>(Action<IEventBase> listener) where T : class
        {
            if (!_listeners.ContainsKey(typeof(T)))
            {
                _listeners.Add(typeof(T), new List<Action<IEventBase>>());
            }

            _listeners[typeof(T)].Add(listener);
        }

        public static void Publish<T>(T publishedEvent) where T : class
        {
            if (!_listeners.ContainsKey(typeof(T)))
            {
                return;
            }

            foreach (var action in _listeners[typeof(T)])
            {
                action.Invoke(publishedEvent as IEventBase);
            }
        }

        public static void Unsubscribe<T>(Action<IEventBase> action) where T : class
        {
            if (_listeners.ContainsKey(typeof(T)))
            {
                _listeners[typeof(T)].Remove(action);
            }
        }

        public static void UnsubscribeAll<T>()
        {
            _listeners[typeof(T)].Clear();
        }

        public static void UnsubscribeAll()
        {
            _listeners.Clear();
        }

        public static bool IsSubscribed<T>(Action<IEventBase> action) where T : class
        {
            if (!_listeners.ContainsKey(typeof(T)))
            {
                return false;
            }

            if (_listeners[typeof(T)].Contains(action))
            {
                return true;
            }
            return false;
        }
    } 
}
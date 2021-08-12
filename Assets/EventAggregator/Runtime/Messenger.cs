using System;
using System.Collections.Generic;
using UnityEngine;

namespace Messaging
{
    public sealed class Messenger
    {
        static public Messenger DefaultInstance
        {
            get
            {
                if (_instance is null)
                {
                    _instance = new Messenger();
                }
                return _instance;
            }
        }
        static private Messenger _instance;

        private const string NULL_LISTENER_WARNING =
            "Messenger: destroyed references of '{0}' are still registered " +
            "for '{1}' messages. Clear your listeners upon object destruction " +
            "by calling Cut().";

        private readonly Dictionary<Type, HashSet<Message>> _messages
            = new Dictionary<Type, HashSet<Message>>();

        public void Listen<T>(object owner, params Action<T>[] handlers)
        {
            if (!MessageTypeExists<T>())
            {
                AddMessageType<T>();
            }
            for (int i = 0; i < handlers.Length; i++)
            {
                Action<object> action = Convert(handlers[i]);
                _messages[typeof(T)].Add(new Message(owner, action));
            }
        }

        public void Cut<T>(object owner)
        {
            if (!MessageTypeExists<T>()) return;
            else _messages[typeof(T)].RemoveWhere(o => o.Owner == owner);
        }

        public void Dispatch<T>(T payload) where T : class
        {
            HashSet<Action<T>> actions = GetActionsFrom<T>();

            foreach (Action<T> action in actions)
            {
                action(payload);
            }
        }

        private void AddMessageType<T>()
        {
            _messages.Add(typeof(T), new HashSet<Message>());
        }

        private HashSet<Action<T>> GetActionsFrom<T>()
        {
            var actions = new HashSet<Action<T>>();
            if (_messages.TryGetValue(typeof(T), out HashSet<Message> msgs))
            {
                foreach (Message msg in msgs)
                {
                    if (msg.Owner.Equals(null))
                    {
                        Debug.LogWarning(string.Format(
                            NULL_LISTENER_WARNING,
                            msg.Owner.GetType(),
                            typeof(T)
                        ));
                        continue;
                    }
                    else actions.Add(Convert<T>(msg.Action));
                }
            }
            return actions;
        }

        private bool MessageTypeExists<T>() => _messages.ContainsKey(typeof(T));

        private Action<T> Convert<T>(Action<object> action)
        {
            if (action == null) return null;
            else return new Action<T>(o => action(o));
        }

        private Action<object> Convert<T>(Action<T> action)
        {
            if (action == null) return null;
            else return new Action<object>(o => action((T)o));
        }

        private sealed class Message
        {
            public object Owner { get; }
            public Action<object> Action { get; }

            public Message(object owner, Action<object> action)
            {
                Owner = owner;
                Action = action;
            }
        }
    }
}
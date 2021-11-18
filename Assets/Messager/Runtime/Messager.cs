using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class Messager
{
    public delegate void Middleware(Type type, object owner, Action next);

    public sealed class Subscription
    {
        public object Owner { get; }
        public Action<object> Action { get; }

        public Subscription(object owner, Action<object> action)
        {
            Owner = owner;
            Action = action;
        }
    }

    static public Messager DefaultInstance { get; } = new Messager();

    private const string NULL_LISTENER_WARNING =
        "Messenger: destroyed references of '{0}' are still registered " +
        "for '{1}' messages. Clear your listeners upon object destruction " +
        "by calling Cut().";

    private readonly Dictionary<Type, HashSet<Subscription>> _subscriptions
        = new Dictionary<Type, HashSet<Subscription>>();

    private Middleware _dispatchMiddleware = (t, o, next) => next();
    private Middleware _listenMiddleware = (t, o, next) => next();

    public void Use(
        Middleware onDispatch,
        Middleware onListen
    )
    {
        if (onDispatch != null)
            _dispatchMiddleware = onDispatch;

        if (onListen != null)
            _listenMiddleware = onListen;
    }

    public Messager Listen<T>(object owner, Action<T> handler)
    {
        _listenMiddleware(typeof(T), owner, () =>
        {
            if (!_subscriptions.ContainsKey(typeof(T)))
            {
                _subscriptions.Add(typeof(T), new HashSet<Subscription>());
            }

            Action<object> action = Convert(handler);
            _subscriptions[typeof(T)].Add(new Subscription(owner, action));
        });
        return this;
    }

    public void Cut<T>(object owner)
    {
        if (!_subscriptions.ContainsKey(typeof(T)))
            return;

        _subscriptions[typeof(T)].RemoveWhere(o => o.Owner == owner);
    }

    public void Dispatch<T>(T payload)
    {
        _dispatchMiddleware(typeof(T), payload, () =>
        {
            var actions = new HashSet<Action<T>>();

            if (_subscriptions.TryGetValue(typeof(T), out HashSet<Subscription> subs))
            {
                // Convert back each action of type object to an action of type T.
                foreach (Subscription sub in subs)
                {
                    if (sub.Owner.Equals(null))
                    {
                        Debug.LogWarning(
                            string.Format(NULL_LISTENER_WARNING, sub.Owner.GetType(), typeof(T))
                        );
                        continue;
                    }
                    actions.Add(Convert<T>(sub.Action));
                }
            }

            foreach (Action<T> action in actions)
            {
                action(payload);
            }
        });
    }

    Action<T> Convert<T>(Action<object> action)
    {
        if (action == null)
            return null;

        return new Action<T>(o => action(o));
    }

    Action<object> Convert<T>(Action<T> action)
    {
        if (action == null)
            return null;

        return new Action<object>(o => action((T)o));
    }
}
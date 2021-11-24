using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

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

    private Middleware _dispatch = (t, o, next) => next();
    private Middleware _listen = (t, o, next) => next();
    private Middleware _cut = (t, o, next) => next();

    public void Use(
        Middleware onDispatch = null,
        Middleware onListen = null,
        Middleware onCut = null
    )
    {
        if (onDispatch != null)
            _dispatch = onDispatch;

        if (onListen != null)
            _listen = onListen;

        if (onCut != null)
            _cut = onCut;
    }

    public Messager Listen<T>(object owner, Action<T> handler)
    {
        Assert.IsNotNull(owner, $"parameter '{nameof(owner)}' cannot be null.");
        Assert.IsNotNull(handler, $"parameter '{nameof(handler)}' cannot be null.");

        _listen(typeof(T), owner, () =>
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
        Assert.IsNotNull(owner);

        _cut(typeof(T), owner, () =>
        {
            if (!_subscriptions.ContainsKey(typeof(T)))
                return;

            _subscriptions[typeof(T)].RemoveWhere(o => o.Owner == owner);
        });
    }

    public void Dispatch<T>(T payload)
    {
        _dispatch(typeof(T), payload, () =>
        {
            if (!_subscriptions.ContainsKey(typeof(T)))
                return;

            var actions = _subscriptions[typeof(T)]
                .Where(sub => !sub.Owner.Equals(null))
                .Select(sub => Convert<T>(sub.Action));

            foreach (Action<T> action in actions)
            {
                action(payload);
            }
        });
    }

    static Action<T> Convert<T>(Action<object> action) =>
        new Action<T>(o => action(o));

    static Action<object> Convert<T>(Action<T> action) =>
        new Action<object>(o => action((T)o));
}
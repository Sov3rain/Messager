using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class Messager
{
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

    public Messager Listen<T>(object owner, Action<T> handler)
    {
#if UNITY_EDITOR
        if (UnityEditor.EditorPrefs.GetBool("Messager/Enable Devtools"))
            MessagerDevtools.AddSubscriptionRecord(typeof(T), owner);
#endif

        if (!_subscriptions.ContainsKey(typeof(T)))
        {
            _subscriptions.Add(typeof(T), new HashSet<Subscription>());
        }

        Action<object> action = Convert(handler);
        _subscriptions[typeof(T)].Add(new Subscription(owner, action));
        return this;
    }

    public void Cut<T>(object owner)
    {
        if (!_subscriptions.ContainsKey(typeof(T))) return;
        else _subscriptions[typeof(T)].RemoveWhere(o => o.Owner == owner);
    }

    public void Dispatch<T>(T payload)
    {
#if UNITY_EDITOR
        if (UnityEditor.EditorPrefs.GetBool("Messager/Enable Devtools"))
            MessagerDevtools.AddHistoryRecord(typeof(T), payload);
#endif

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
    }

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
}
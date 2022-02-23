using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

public sealed class Messager
{
    public delegate void Middleware(Context ctx, Action next);

    public sealed class Context
    {
        public Type Type { get; set; }
        public object Obj { get; set; }

        public Context(Type type, object obj) =>
            (Type, Obj) = (type, obj);

        public void Deconstruct(out Type type, out object obj)
        {
            type = Type;
            obj = Obj;
        }
    }

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

    private readonly Dictionary<Type, HashSet<Subscription>> _subscriptions
        = new Dictionary<Type, HashSet<Subscription>>();

    private Middleware _dispatch = (ctx, next) => next();
    private Middleware _listen = (ctx, next) => next();
    private Middleware _cut = (ctx, next) => next();

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

        var ctx = new Context(typeof(T), owner);
        _listen(ctx, () =>
        {
            if (!_subscriptions.ContainsKey(typeof(T)))
                _subscriptions.Add(typeof(T), new HashSet<Subscription>());

            Action<object> action = Convert(handler);
            _subscriptions[typeof(T)].Add(new Subscription(owner, action));
        });
        return this;
    }

    public void Cut<T>(object owner)
    {
        Assert.IsNotNull(owner);

        var ctx = new Context(typeof(T), owner);
        _cut(ctx, () =>
        {
            if (!_subscriptions.ContainsKey(typeof(T)))
                return;

            _subscriptions[typeof(T)].RemoveWhere(o => o.Owner == owner);
        });
    }

    public void Dispatch<T>(T payload)
    {
        var ctx = new Context(typeof(T), payload);
        _dispatch(ctx, () =>
        {
            if (!_subscriptions.ContainsKey(typeof(T)))
                return;

            var actions = new HashSet<Action<T>>();
            foreach (var sub in _subscriptions[typeof(T)])
            {
                if (!sub.Owner.Equals(null))
                    actions.Add(Convert<T>(sub.Action));
            }

            foreach (Action<T> action in actions)
                action(payload);
        });
    }

    static Action<T> Convert<T>(Action<object> action) =>
        new Action<T>(o => action(o));

    static Action<object> Convert<T>(Action<T> action) =>
        new Action<object>(o => action((T)o));
}
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

static public class MessagerDevtools
{
    [Serializable]
    public class HistoryRecord
    {
        public string Time;
        public string Caller;
        public string Type;
        public string Payload;

        [NonSerialized]
        public bool IsVisible = false;

        public HistoryRecord(string time, string caller, string type, string payload)
        {
            Time = time;
            Type = type;
            Payload = payload;
            Caller = caller;
        }
    }

    static HashSet<HistoryRecord> _records = new HashSet<HistoryRecord>();
    static readonly Dictionary<Type, List<object>> _subscriptions
        = new Dictionary<Type, List<object>>();

    public static HistoryRecord[] GetMessageHistory() => _records.ToArray();
    public static Dictionary<Type, List<object>> GetSubscriptions() => _subscriptions;

    internal static void AddHistoryRecord(Type type, object payload)
    {
        var sf = new StackTrace().GetFrames();
        var c = sf.Last().GetMethod().DeclaringType.ToString();
        var m = sf.Last().GetMethod().Name;

        TimeSpan ts = TimeSpan.FromSeconds(Time.time);
        string time = "+" + ts.ToString(@"mm\:ss\.fff");

        _records.Add(new HistoryRecord(
            time: time,
            caller: $"{c}.{m} ()",
            type: type.ToString(),
            payload: JsonUtility.ToJson(payload, true)
        ));
    }

    internal static void AddSubscriptionRecord(Type type, object owner)
    {
        if (!_subscriptions.ContainsKey(type))
        {
            _subscriptions.Add(type, new List<object>());
        }

        _subscriptions[type].Add(owner);
    }
}

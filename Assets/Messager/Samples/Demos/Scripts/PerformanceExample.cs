using System;
using System.Collections.Generic;
using Messaging;
using UnityEngine;

public class PerformanceExample : MonoBehaviour
{
    static public event Action TestAction;

    public int count = 1000;

    private List<TestListener> listenersMessage = new List<TestListener>();
    private List<TestAction> listenersAction = new List<TestAction>();

    private readonly Messager _messager = Messager.DefaultInstance;

    [ContextMenu("Benchmark Messages")]
    private void FireMessage()
    {
        for (int i = 0; i < count; i++)
        {
            listenersMessage.Add(new TestListener(_messager));
        }
        Debug.Log("Start watching messages...");
        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();
        _messager.Dispatch(new WatchedMessage());
        watch.Stop();
        Debug.Log($"Execution time for {listenersMessage.Count} elements: {watch.ElapsedMilliseconds} ms");
    }

    [ContextMenu("Benchmark Actions")]
    private void FireAction()
    {
        for (int i = 0; i < count; i++)
        {
            listenersAction.Add(new TestAction());
        }
        Debug.Log("Start watching actions...");
        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();
        TestAction();
        watch.Stop();
        Debug.Log($"Execution time for {listenersAction.Count} elements: {watch.ElapsedMilliseconds} ms");
    }
}

public sealed class TestListener
{
    public TestListener(Messager messenger)
    {
        messenger.Listen<WatchedMessage>(this, msg =>
        {
            Debug.Log("Message!");
        });
    }
}

public sealed class TestAction
{
    public TestAction()
    {
        PerformanceExample.TestAction += TestAction_SimpleExample;
    }

    private void TestAction_SimpleExample()
    {
        Debug.Log("Action!");
    }
}

public sealed class WatchedMessage { }

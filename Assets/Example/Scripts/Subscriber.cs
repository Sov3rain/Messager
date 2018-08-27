using UnityEngine;
using EventAggregation;

public class Subscriber : MonoBehaviour
{
    private void Awake()
    {
        EventAggregator.Subscribe<IntEvent>(OnIntEvent);
    }

    private void OnIntEvent(IEventBase eventBase)
    {
        IntEvent e = eventBase as IntEvent;

        if (e != null)
        {
            Debug.Log("This was fired by the EventAggregator " + e.Value);
        }
    }
}
using System.Collections;
using EventAggregation;
using UnityEngine;

public class Ping : MonoBehaviour
{
    EventAggregator _eventAggregator;

    void Start()
    {
        _eventAggregator = EventAggregator.DefaultInstance;
        _eventAggregator.AddListener<OnPong>(OnPongHandler);
        _eventAggregator.Dispatch<OnPing>();
    }

    private void OnPongHandler(IEvent obj)
    {
        StartCoroutine(DoResponse());
    }

    private IEnumerator DoResponse()
    {
        Debug.Log("Receive pong!", this);
        yield return new WaitForSeconds(1);
        _eventAggregator.Dispatch(new OnPing { message = "Hello World!" });
    }
}

class OnPing : IEvent
{
    public string message;
}
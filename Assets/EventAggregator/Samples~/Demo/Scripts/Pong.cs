using System.Collections;
using UnityEngine;

namespace EventAggregation.Examples
{
    public class Pong : MonoBehaviour
    {
        EventAggregator _eventAggregator;

        void Start()
        {
            _eventAggregator = EventAggregator.DefaultInstance;
            _eventAggregator.AddListener<OnPing>(OnPingHandler);
        }

        private void OnPingHandler(IEvent obj)
        {
            var eventData = obj as OnPing;
            StartCoroutine(DoResponse(eventData));
        }

        private IEnumerator DoResponse(OnPing eventData)
        {
            Debug.Log($"Receive ping! message: {eventData?.message}", this);
            yield return new WaitForSeconds(1);
            _eventAggregator.Dispatch<OnPong>();
        }
    }

    public class OnPong : IEvent { }
}

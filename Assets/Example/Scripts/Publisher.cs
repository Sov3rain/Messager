using UnityEngine;
using EventAggregation;

public class Publisher : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            IntEvent score = new IntEvent() { Value = 1 };
            EventAggregator.Publish(score); 
        }
    }
}
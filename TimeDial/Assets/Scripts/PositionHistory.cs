using System.Collections.Generic;
using UnityEngine;

public class PositionHistory : MonoBehaviour
{
    private struct Snapshot { public float time; public Vector3 position; }

    [SerializeField] private float maxTrackedSeconds = 5f; 

    private readonly List<Snapshot> history = new List<Snapshot>();

    private void Update()
    {
        history.Add(new Snapshot { time = Time.time, position = transform.position });

        while (history.Count > 0 && Time.time - history[0].time > maxTrackedSeconds)
            history.RemoveAt(0);
    }

    public Vector3 GetPositionSecondsAgo(float seconds)
    {
        float targetTime = Time.time - seconds;

        for (int i = history.Count - 1; i >= 0; i--)
        {
            if (history[i].time <= targetTime)
                return history[i].position;
        }

        return history.Count > 0 ? history[0].position : transform.position;
    }
}

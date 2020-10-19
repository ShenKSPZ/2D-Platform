using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitStop : MonoBehaviour
{

    bool waiting;

    public void Stop(float duration)
    {
        if (waiting)
            return;
        Time.timeScale = 0f;
        StartCoroutine(Wait(duration));
    }

    IEnumerator Wait(float duration)
    {
        waiting = true;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1.0f;
        waiting = false;
    }
}

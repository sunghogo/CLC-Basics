using System;
using System.Collections;
using UnityEngine;

public static class CoroutineLibrary {
    public static IEnumerator WaitSeconds(float s, Action callback)
    {
        float duration = 0f;
        yield return new WaitUntil(() => {
            duration += Time.unscaledDeltaTime;
            return duration > s;
        });
        callback?.Invoke();
    }

}
using System;
using UnityEngine;

public class Clock : MonoBehaviour
{
    [SerializeField] private Transform _hoursPivot, _minutesPivot, _secondsPivot;
    const float hoursToDegrees = -30f, minutesToDegrees = -6f, secondsToDegrees = -6f;

    void Update()
    {
        TimeSpan time = DateTime.Now.TimeOfDay;
        _hoursPivot.localRotation = Quaternion.Euler(0f, 0f, hoursToDegrees * (float)time.TotalHours);
		_minutesPivot.localRotation = Quaternion.Euler(0f, 0f, minutesToDegrees * (float)time.TotalMinutes);
		_secondsPivot.localRotation = Quaternion.Euler(0f, 0f, secondsToDegrees * (float)time.TotalSeconds);   
    }
}

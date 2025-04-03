using UnityEngine;

public class HourIndicatorSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _HourIndicatorPrefab, _MinuteSecondIndicatorPrefab;
    [SerializeField] private float _radius = 4f;
    GameObject HourIndicators, MinuteSecondIndicators;

    void Awake()
    {
        HourIndicators = new GameObject("Hour Indicators");
        HourIndicators.transform.SetParent(transform);
        MinuteSecondIndicators = new GameObject("Minute Second Indicators");
        MinuteSecondIndicators.transform.SetParent(transform);
    }

    void Start()
    {
        int n = 60;
        for (int i = 0; i < n; i ++) {
            float x = Mathf.Cos(2 * Mathf.PI * i / n) * _radius;
            float y = Mathf.Sin(2 * Mathf.PI * i / n) * _radius;
            Vector3 position = new Vector3(x, y, transform.position.z);
            Quaternion rotation = Quaternion.Euler(0f, 0f, 90 + (i * 360f / n));
            bool isHour = i % 5 == 0;
            var prefab = isHour ? _HourIndicatorPrefab : _MinuteSecondIndicatorPrefab;
            var parent = isHour ? HourIndicators.transform : MinuteSecondIndicators.transform;
            Instantiate(prefab, position, rotation, parent);
        }
    }
}

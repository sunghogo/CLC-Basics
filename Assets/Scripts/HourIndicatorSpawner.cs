using UnityEngine;

public class HourIndicatorSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _HourIndicatorPrefab;
    [SerializeField] private float _radius = 4f;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 12; i ++) {
            float x = Mathf.Cos(2 * Mathf.PI * i / 12) * _radius;
            float y = Mathf.Sin(2 * Mathf.PI * i / 12) * _radius;
            Vector3 position = new Vector3(x, y, transform.position.z);
            Quaternion rotation = Quaternion.Euler(0f, 0f, 90 + (i * 360f / 12));
            Instantiate(_HourIndicatorPrefab, position, rotation);
        }
    }
}

using UnityEngine;

public class HourIndicatorSpawner : MonoBehaviour
{
    public GameObject HourIndicatorPrefab;
    public float radius = 4f;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 12; i ++) {
            float x = Mathf.Cos(2 * Mathf.PI * i / 12) * radius;
            float y = Mathf.Sin(2 * Mathf.PI * i / 12) * radius;
            Vector3 position = new Vector3(x, y, transform.position.z);
            Quaternion rotation = Quaternion.Euler(0f, 0f, 90 + (i * 360f / 12));
            Instantiate(HourIndicatorPrefab, position, rotation);
        }
    }
}

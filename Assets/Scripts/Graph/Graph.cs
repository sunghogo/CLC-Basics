using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Graph : MonoBehaviour
{
    [SerializeField] Transform pointPrefab;
    public bool sin = true;
    public bool cos = false;
    public bool tan = false;
    bool[] previousOp;
    [SerializeField, Range(16, 512)] int resolution = 16;
    int previousResolution = 10;
    [SerializeField, Range(1, 10)] float width = 2f;
    float previousWidth = 2f;
    [SerializeField, Range(1, 10)] float frequency = 1f;
    float previousFrequency = 1f;
    GameObject pointsContainer; // Needed to safely destroy all points
    Transform[] points;

    void CreatePoints() {
        if (pointsContainer != null) DestroyImmediate(pointsContainer.gameObject);

        pointsContainer = new GameObject("Points Container");
        pointsContainer.hideFlags = HideFlags.DontSaveInEditor; // Prevents saving and remaining as an override
        pointsContainer.transform.SetParent(transform, false);
        points = new Transform[resolution];
    }
    
    void CalculatePointsX() {
        float step = width / resolution; // Points span width
        Vector3 position = Vector3.zero;
        Vector3 scale = Vector3.one * step;
		for (int i = 0; i < points.Length; i++) {
			Transform point = points[i] = Instantiate(pointPrefab, pointsContainer.transform, false);
            position.x = (i + (0.5f * scale.x)) * step - (0.5f * width); // Adjusts so points are adjacent. Fixes points between -width < x < width
			point.localPosition = position;
			point.localScale = scale;
		}
    }

    void CalculatePointsY() {
        float time = Time.time;
        for (int i = 0; i < points.Length; i++) {
            Transform point = points[i];
            Vector3 position = point.localPosition;
            if (tan) position.y = Mathf.Tan(Mathf.PI * (position.x + time) * frequency);
            else if (cos) position.y = Mathf.Cos(Mathf.PI * (position.x + time) * frequency);
            else  position.y = Mathf.Sin(Mathf.PI * (position.x + time) * frequency);
			point.localPosition = position;
        }
    }

    void DestroyPoints() {
        if (pointsContainer != null) { 
            DestroyImmediate(pointsContainer);
            pointsContainer = null;
            points = null;
        }
    }

    void UpdatePointsIfChanged<T>(T propertyVal, ref T previousPropertyVal) {
        if (!Equals(propertyVal, previousPropertyVal)) {
            DestroyPoints();
            CreatePoints();
            CalculatePointsX();
            previousPropertyVal = propertyVal;
        }
    }

    void SetOps() {
        if (previousOp == null || previousOp.Length < 3) return;

        if (previousOp[0] != sin || previousOp[1] != cos || previousOp[2] != tan) {
            if (sin && !previousOp[0]) previousOp = new bool[] {true, false, false};
            else if (cos && !previousOp[1]) previousOp = new bool[] {false, true, false};
            else if (tan && !previousOp[2]) previousOp = new bool[] {false, false, true};
            else previousOp = new bool[] {true, false, false};
            sin = previousOp[0];
            cos = previousOp[1];
            tan = previousOp[2];
        }
    }

    void Start() {
        previousOp =  new bool[] {true, false, false};
        CreatePoints();
        CalculatePointsX();
	}

    void Update() {
        CalculatePointsY();
    }

    void OnValidate()
    {
        // Queue editor updates to avoid unsafe changes and error messages
        #if UNITY_EDITOR
            if (!pointPrefab || points == null) return;

            EditorApplication.delayCall += () =>
            {
                // Ensure we're not working with destroyed stuff
                if (this == null || gameObject == null) return;

                SetOps();
                UpdatePointsIfChanged(resolution, ref previousResolution);
                UpdatePointsIfChanged(width, ref previousWidth);
                UpdatePointsIfChanged(frequency, ref previousFrequency);
            };
        #endif
    }
}

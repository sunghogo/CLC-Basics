using System;
using System.Collections.Generic;
using UnityEngine;

public class Surfaces : MonoBehaviour
{
    const int MAXRESOLUTION = 128;
    const int RESOLUTION = 32;
    const float MAXWIDTH = 4f;
    const float WIDTH = 2f;
    const float MAXFREQUENCY = 4f;
    const float FREQUENCY = 1f;
    const float FPSDELAY = 1f;

    public event Action OnInitialize;
    public event Action OnInspectorChange;

    [SerializeField] Transform pointPrefab;
    [SerializeField] WaveFunctionLibrary.WaveFunctionName waveFunctionName, transitionWaveFunctionName;
    WaveFunctionLibrary.WaveFunction waveFunction, transitionWaveFunction;
    public enum TransitionMode { Stop, Cycle, Random };
    [SerializeField] TransitionMode transitionMode;
    [SerializeField, Min(0f)] float functionDuration = 2f, transitionDuration = 1f;
    [SerializeField, Range(16, MAXRESOLUTION)] int resolution = RESOLUTION;
    int previousResolution = RESOLUTION;
    [SerializeField, Range(1, MAXWIDTH)] float width = WIDTH;
    [SerializeField, Range(0.2f, MAXFREQUENCY)] float frequency = FREQUENCY;
    GameObject pointsContainer; // Needed to safely destroy all points
    List<Transform> points;
    float duration = 0f;
    bool isTransitioning;
    
    void CreatePoints() {
        if (pointsContainer != null) DestroyImmediate(pointsContainer.gameObject);

        // Create Points Container
        pointsContainer = new GameObject("Points Container");
        pointsContainer.hideFlags = HideFlags.DontSave; // Prevents saving and remaining as an override
        pointsContainer.transform.SetParent(transform, false);

        // Initialize points list
        // Instantiate maximum allowed points to activate/inactivate instead of creating/deleting
        int gridSize = MAXRESOLUTION * MAXRESOLUTION;
        points = new List<Transform>(gridSize);
        
        // Create points and inactivate 
        int activeSize = resolution * resolution;
        for (int i = 0; i < gridSize; i++) {
            Transform point = Instantiate(pointPrefab, pointsContainer.transform, false);
            if (i >= activeSize) point.gameObject.SetActive(false);
            points.Add(point);
        }
    }
    
    void CalculatePoints() {
        float time = Time.time;
        float step = width / resolution;
        Vector3 scale = Vector3.one * step;
        int activeSize = resolution * resolution;
		for (int i = 0, x = 0, z = 0; i < activeSize; i++, x++) {
            if (x == resolution) {
                x = 0;
                z += 1;
            }
            // Adjusts so points are adjacent and shifted half a step to fit the entire width. Fixes points between -width < x < width
            float u = (x + 0.5f) * step - (0.5f * width);
            float v = (z + 0.5f) * step - (0.5f * width);
            Vector3 position = new Vector3(u, 0, v);
            position = waveFunction(position.x, position.z, time, frequency);
            points[i].localPosition = position;
            points[i].localScale = scale;
		}
    }

    void CalculateTransitioningPoints() {
        float time = Time.time;
        float progress = duration / transitionDuration;
        float step = width / resolution;
        Vector3 scale = Vector3.one * step;
        int activeSize = resolution * resolution;
		for (int i = 0, x = 0, z = 0; i < activeSize; i++, x++) {
            if (x == resolution) {
                x = 0;
                z += 1;
            }
            // Adjusts so points are adjacent and shifted half a step to fit the entire width. Fixes points between -width < x < width
            float u = (x + 0.5f) * step - (0.5f * width);
            float v = (z + 0.5f) * step - (0.5f * width);
            Vector3 position = WaveFunctionLibrary.Morph(u, v, time, frequency, waveFunction, transitionWaveFunction, progress);
            points[i].localPosition = position;
            points[i].localScale = scale;
        }
    }

    void DestroyPoints() {
        if (pointsContainer != null) { 
            Destroy(pointsContainer);
            pointsContainer = null;
            points = null;
        }
    }

    void UpdateWaveFunction() {
        if (waveFunctionName != transitionWaveFunctionName && !isTransitioning) {
            waveFunctionName = transitionWaveFunctionName;
            waveFunction = transitionWaveFunction;
            StartCoroutine(CoroutineLibrary.WaitSeconds(FPSDELAY, OnInspectorChange));
        }
    }

    void UpdateResolution() {
        if (resolution != previousResolution) {
            int activeSize = resolution * resolution;
            int currentSize = previousResolution * previousResolution;
            if (resolution > previousResolution) 
                for (int i = currentSize; i < activeSize; i++) points[i].gameObject.SetActive(true);
            else 
                for (int i = currentSize - 1; i >= activeSize; i--) points[i].gameObject.SetActive(false);
            previousResolution = resolution;
            StartCoroutine(CoroutineLibrary.WaitSeconds(FPSDELAY, OnInspectorChange));
        }
    } 

    void RefreshInspectorChanges() {
        UpdateWaveFunction();
        UpdateResolution();
    }

    void PickNextWaveFunction() {
        switch (transitionMode) {
            case TransitionMode.Cycle:
                transitionWaveFunctionName = WaveFunctionLibrary.GetNextFunctionName(transitionWaveFunctionName);
                transitionWaveFunction = WaveFunctionLibrary.GetFunction(transitionWaveFunctionName);
                break;
            case TransitionMode.Random:
                transitionWaveFunctionName = WaveFunctionLibrary.GetRandomFunctionNameOtherThan(transitionWaveFunctionName);
                transitionWaveFunction = WaveFunctionLibrary.GetFunction(transitionWaveFunctionName);
                break;
            default:
                break;
        }
    }

    void Start() {
        transitionMode = TransitionMode.Random;
        transitionWaveFunctionName = waveFunctionName = WaveFunctionLibrary.WaveFunctionName.Wave;
        transitionWaveFunction = waveFunction = WaveFunctionLibrary.GetFunction(transitionWaveFunctionName);
        CreatePoints();
        CalculatePoints();
        StartCoroutine(CoroutineLibrary.WaitSeconds(1f, OnInitialize));
	}

    void Update() {
        duration += Time.deltaTime;
        if (transitionDuration > functionDuration) transitionDuration = functionDuration;
        if (duration >= functionDuration) {
            duration -= functionDuration;
            PickNextWaveFunction();
            if (waveFunctionName != transitionWaveFunctionName) isTransitioning = true;
        } else if (duration >= transitionDuration) {
            isTransitioning = false;
        }

        RefreshInspectorChanges();
        if (isTransitioning) {
            CalculateTransitioningPoints();
        } else {
            CalculatePoints();
        }
    }
}

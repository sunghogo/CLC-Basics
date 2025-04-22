using UnityEngine;
using TMPro;

public class FrameRateCounter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI display;
    [SerializeField, Range(0.1f, 2f)] float sampleDuration = 1f;
    public enum DisplayMode { FPS, MS }

	[SerializeField] DisplayMode displayMode = DisplayMode.FPS;
    [SerializeField] Surfaces surfaces;
    int frames;
    float duration, bestFrameDuration = float.MaxValue, worstFrameDuration = float.MinValue;
    float avgFps, bestFps, worstFps;
    [SerializeField] bool surfacesInitialized = false;
    

    void HandleInitialized() {
        surfacesInitialized = true;
    }

    void HandleInspectorChange() {
        bestFrameDuration = float.MaxValue;
        worstFrameDuration = float.MinValue;
    }

    void OnDestroy()
    {
        if (surfaces != null) surfaces.OnInitialize -= HandleInitialized;
    }

    void Awake()
    {
        surfaces.OnInitialize += HandleInitialized;
        surfaces.OnInspectorChange += HandleInspectorChange;
    }

    void Update()
    {
        if (!surfacesInitialized) return;

        float frameDuration = Time.unscaledDeltaTime;
        frames++;
        duration += frameDuration;

        if (frameDuration < bestFrameDuration) {
            bestFrameDuration = frameDuration;
            bestFps = 1f / bestFrameDuration;
        }
        if (frameDuration > worstFrameDuration) {
            worstFrameDuration = frameDuration;
            worstFps = 1f / worstFrameDuration;
        }

        if (duration >= sampleDuration) {
            if (displayMode == DisplayMode.FPS) {
                avgFps = frames / duration;
                display.SetText("FPS\n{0:0}\n{1:0}\n{2:0}", bestFps, avgFps, worstFps);
            } else {
                float avgFrameDuration = duration / frames;
                display.SetText("MS\n{0:1}\n{1:1}\n{2:1}", 1000f * bestFrameDuration, 1000f * avgFrameDuration, 1000f * worstFrameDuration);
            }
            frames = 0;
            duration = 0f;

        }
    } 
}

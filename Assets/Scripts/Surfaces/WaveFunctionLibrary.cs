using UnityEngine;
using static UnityEngine.Mathf;

public static class WaveFunctionLibrary
{
    public delegate Vector3 WaveFunction (float u, float v, float t, float f);

    public enum WaveFunctionName {Wave, MultiWave, Ripple, Sphere, WaveSphere, RingTorus, WaveTorus};
    static WaveFunction [] functions = {Wave, MultiWave, Ripple, Sphere, WaveSphere, RingTorus, WaveTorus};

    public static WaveFunction GetFunction (WaveFunctionName name) {
        return functions[(int)name];
    }

    public static Vector3 Wave(float u, float v, float t, float f) {
        Vector3 p;
        p.x = u;
        // f(x, t) = sin(pi * f * (x + t))
        p.y = Sin(PI * f * (u + v + t));
        p.z = v;
        return p;
    }

    public static Vector3 MultiWave(float u, float v, float t, float f) {
        Vector3 p;
        p.x = u;
        // f(x, t) = (1 / 2.5) * (sin(pi * f * (x + 0.5 * t)) + 0.5 * sin(2 * pi * f * (x + t))) + sin(pi * (x + z + 0.25f * t))
        float y = Sin(PI * f * (u + 0.5f *  t)); 
        y += 0.5f * Sin(2f * PI * f * (v + t)); 
        y += Sin(PI * (u + v + 0.25f * t)) * (1f / 2.5f);
        p.y = y;
        p.z = v;
        return p;
    }

    public static Vector3 Ripple (float u, float v, float t, float f) {
        Vector3 p;
        p.x = u;
        // f(x) = sin(4 * pi * f * (d - t)) / (1 + 10 * d)
        float dis = Sqrt(u * u + v * v);
        float y = Sin(4f * PI * f * (dis - t)) / (1 + 10 * dis);
        p.y = y;
        p.z = v;
        return p;
    }

    public static Vector3 Sphere (float u, float v, float t, float f) {
        float r = 0.5f + 0.5f * Sin(PI * t * f);
        float s = r * Cos(0.5f * PI * v);
        Vector3 p;
        // f(u, v) = [s * sin(pi * u), r * sin(pi/2 * v), s * cos(pi/u)]
        p.x = s * Sin(PI * u);
        p.y = r * Sin(PI * 0.5f * v);
        p.z = s * Cos(PI * u);
        return p;
    }

    public static Vector3 WaveSphere (float u, float v, float t, float f) {
        // r = (9 + sin(pi(6u + 4v + t)) / 10)
        float r = 0.9f + 0.1f * Sin(PI * (6f * u + 4f * v + t * f));
        float s = r * Cos(0.5f * PI * v);
        Vector3 p;
        // f(u, v) = [s * sin(pi * u), r * sin(pi/2 * v), s * cos(pi/u)]
        p.x = s * Sin(PI * u);
        p.y = r * Sin(PI * 0.5f * v);
        p.z = s * Cos(PI * u);
        return p;
    }

    public static Vector3 RingTorus (float u, float v, float t, float f) {
	    float majorR = 0.75f;
		float minorR = 0.25f;
		float s = majorR + minorR * Cos(PI * v);
		Vector3 p;
		p.x = s * Sin(PI * u);
		p.y = minorR * Sin(PI * v);
		p.z = s * Cos(PI * u);
		return p;
	}

    public static Vector3 WaveTorus (float u, float v, float t, float f) {
		float majorR = 0.7f + 0.1f * Sin(PI * (6f * u + 0.5f * t * f));
		float minorR = 0.15f + 0.05f * Sin(PI * (8f * u + 4f * v + 2f * t * f));
		float s = majorR + minorR * Cos(PI * v);
		Vector3 p;
		p.x = s * Sin(PI * u);
		p.y = minorR * Sin(PI * v);
		p.z = s * Cos(PI * u);
		return p;
	}
}

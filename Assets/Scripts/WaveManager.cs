using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;
    public float amplitude = 1f;
    public float length = 2f;
    public float speed = 1f;
    public float waveOffset = 0f;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        waveOffset += Time.deltaTime * speed;
    }

    public float GetWaveHeight(float _x)
    {
        return amplitude * Mathf.Sin(_x / length + waveOffset);
    }
}

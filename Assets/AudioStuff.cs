using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioStuff : MonoBehaviour
{
    public ShipController sc;

    private bool _isscNotNull;
    [SerializeField] private float lowVolCutoff;

    // Start is called before the first frame update
    void Start()
    {
        _isscNotNull = sc != null;
    }

    // Update is called once per frame
    void Update()
    {
        int dec = 128;
        float[] waveData = new float[dec];
        glitching = false;
        source.GetSpectrumData(samples, 0, FFTWindow.Hamming);

        for (int i = 4; i < movingAverageSamples.Count + 4; i++)
        {
            movingAverageSamples[i - 4] = decayRate * samples[i] + (1 - decayRate) * movingAverageSamples[i - 4];
        }

        // var output = ZScore.StartAlgo(movingAverageSamples, 5, 10, 0);

        for (int i = 1; i < movingAverageSamples.Count; i++)
        {
            Debug.DrawLine(new Vector3((i - 10)/4f, Mathf.Log(movingAverageSamples[i - 1]) + 10, 2), new Vector3((i - 9)/4f, Mathf.Log(movingAverageSamples[i]) + 10, 2), Color.cyan);
        }
        
        var minMaxIdx = movingAverageSamples.Select((v, i) => new {v, i})
            .OrderByDescending(c => c.v)
            .Take(3)
            .Min(c => c.i);
        var totalVolume = 0f;
        var totalWeighted = 0f;
        if (minMaxIdx > 0) minMaxIdx -= 1;
        var max3 = movingAverageSamples.GetRange(minMaxIdx, 3)
            .Select((v, i) => new {v, i});
        foreach (var sample in max3)
        {
            totalVolume += sample.v;
            totalWeighted += (sample.i + minMaxIdx) * sample.v;
        }

        var max = Mathf.Log(movingAverageSamples.Max());
        var mostDiff = max - Mathf.Log(movingAverageSamples.Min());
        
 
        float lvlMax;
        
        var maxIdx = (totalWeighted / totalVolume).Remap(lowestPitchPoint, 0, highestPitchPoint, 1);
        Debug.Log(max + 10);
        if (volumeLabel != null)
        {
            volumeLabel.text = "" + (max + 10);
        }
        lvlMax = (max + 10).Remap(lowestVolumePoint, 2, highestVolumePoint, 8);
        Debug.Log(lvlMax);
        if (mostDiff < mostDiffCutoff)
        {
            if (lowVolCutoff < lvlMax && !glitching)
            {
                _spriteRenderer.material.SetFloat(GlitchAmount, lvlMax * 2 * 0.01f);
                glitching = true;
            }
            lvlMax = 0.001f;
        }

        if (!glitching)
        {
            _spriteRenderer.material.SetFloat(GlitchAmount, 0f);
        }

        if (lvlMax > lowVolCutoff)
        {
            shotCooldown = Math.Min(1 / (lvlMax * 1 / 1.5f - 2/1.5f), 2);
        }
        else
        {
            shotCooldow
                n = 2;
        }
        
        if (lvlMax > lowVolCutoff)
        {
            if (_isscNotNull)
            {
                sc.normalizedPositions = maxIdx;
            }
            // Debug.Log(maxIdx + " " + lvlMax);
        }
    }
}

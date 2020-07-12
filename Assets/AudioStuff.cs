using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class AudioStuff : MonoBehaviour
{
    public ShipController sc;

    float[] samples = new float[1024];
    List<float> movingAverageSamples = CreateList<float>(64);
    
    AudioClip microphoneInput;
    private AudioSource source;
    
    private bool _isscNotNull;
    private static readonly int GlitchAmount = Shader.PropertyToID("_GlitchAmount");
    public float decayRate = 0.5f;

    public TextMeshProUGUI volumeTm;
    public TextMeshProUGUI volumeTmQuiet;
    public TextMeshProUGUI volumeTmLoud;
    public TextMeshProUGUI pitchTm;
    public TextMeshProUGUI pitchTmLow;
    public TextMeshProUGUI pitchTmHigh;
    private bool _ispitchTmNotNull;
    private bool _isvolumeTmNotNull;

    private float lvlMax;
    private float maxIdx;

    private static List<T> CreateList<T>(int capacity)
    {
        List<T> coll = new List<T>(capacity);
        for(int i = 0; i < capacity; i++)
            coll.Add(default(T));
        
        return coll;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        _isvolumeTmNotNull = volumeTm != null;
        _ispitchTmNotNull = pitchTm != null;
        _isscNotNull = sc != null;
        if (Microphone.devices.Length>0){
            microphoneInput = Microphone.Start(Microphone.devices[0],true,2,44100);
            source = GetComponent<AudioSource>();
            source.clip = microphoneInput;
            source.loop = true;
            while (!(Microphone.GetPosition(null) > 0)) { }
            source.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
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
        
 
        lvlMax = max + 10;

        maxIdx = totalWeighted / totalVolume;

        if (_isvolumeTmNotNull)
        {
            volumeTm.text = lvlMax.ToString("0.00");
        }

        if (_ispitchTmNotNull)
        {
            pitchTm.text = maxIdx.ToString("0.00");
        }
        
        if (_isscNotNull)
        {
            sc.glitching = false;
            maxIdx = maxIdx.Remap(ShipController.lowestPitchPoint, 0, ShipController.highestPitchPoint, 1);
            lvlMax = lvlMax.Remap(ShipController.lowestVolumePoint, 2, ShipController.highestVolumePoint, 8);
            
            if (mostDiff < sc.mostDiffCutoff)
            {
                if (sc.lowVolCutoff < lvlMax && !sc.glitching)
                {
                    sc._spriteRenderer.material.SetFloat(GlitchAmount, lvlMax * 2 * 0.01f);
                    sc.glitching = true;
                }
                lvlMax = 0.001f;
            }

            if (!sc.glitching)
            {
                sc._spriteRenderer.material.SetFloat(GlitchAmount, 0f);
            }

            if (lvlMax > sc.lowVolCutoff)
            {
                sc.shotCooldown = Math.Min(1 / (lvlMax * 1 / 1.5f - 2/1.5f), 2);
            }
            else
            {
                sc.shotCooldown = 2;
            }
        
            if (lvlMax > sc.lowVolCutoff)
            {
                sc.normalizedPositions = maxIdx;
                sc.slider.maxValue = sc.shotCooldown;
                var rect = sc._rectTransformCooldownSlider.rect;
                sc._rectTransformCooldownSlider.sizeDelta = new Vector2(sc.shotCooldown * 250, sc._rectTransformCooldownSlider.sizeDelta.y);
            }
        }
    }

    public void QuiteSet()
    {
        ShipController.lowestVolumePoint = lvlMax;
        volumeTmQuiet.text = ShipController.lowestVolumePoint.ToString("0.00");
    }
    
    public void LoudSet()
    {
        ShipController.highestVolumePoint = lvlMax;
        volumeTmLoud.text = ShipController.highestVolumePoint.ToString("0.00");
    }
    
    public void LowestSet()
    {
        ShipController.lowestPitchPoint = maxIdx;
        pitchTmLow.text = ShipController.lowestPitchPoint.ToString("0.00");
    }
    
    public void HighestSet()
    {
        ShipController.highestPitchPoint = maxIdx;
        pitchTmHigh.text = ShipController.highestPitchPoint.ToString("0.00");
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EZCameraShake;
using UnityEngine;
using UnityEngine.UI;

public class ShipController : MonoBehaviour
{
    public CameraShaker shaker;
    public GameObject projectilePrefab;
    public float shotCooldown;

    public float normalizedPositions;
    public float shotCooldownMultiplier;

    public Slider slider;
    
    AudioClip microphoneInput;
    private AudioSource source;
    private Transform controller;
    private GameObject projectile;
    private bool canShoot;
    public float cooldown;

    public float mostDiffCutoff = 5;

    public ParticleSystem fireSparks;
    
    float[] samples = new float[1024];
    List<float> movingAverageSamples = CreateList<float>(64);

    public float decayRate = 0.5f;
    [SerializeField] private float movespeed;
    private RectTransform _rectTransformCooldownSlider;

    private void Awake()
    {
        _rectTransformCooldownSlider = slider.GetComponentInParent<RectTransform>();
    }

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
        controller = gameObject.GetComponent<Transform>();
        projectile = null;
        canShoot = false;
        cooldown = 0;
        Debug.Log(Microphone.devices.Length);
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
        int dec = 128;
        float[] waveData = new float[dec];
    
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
        
        var maxIdx = totalWeighted / totalVolume;

        if (mostDiff > mostDiffCutoff)
        {
            lvlMax = max + 10;
        }
        else
        {
            lvlMax = 0.001f;
        }

        shotCooldown = Math.Min(1 / (lvlMax * lvlMax) * shotCooldownMultiplier, 4);

        if (lvlMax > 1.25)
        {
            normalizedPositions = (maxIdx - 4) * 0.05f;
            slider.maxValue = shotCooldown;
            var rect = _rectTransformCooldownSlider.rect;
            _rectTransformCooldownSlider.sizeDelta = new Vector2(shotCooldown * 125, _rectTransformCooldownSlider.sizeDelta.y);
            // Debug.Log(maxIdx + " " + lvlMax);
        }
        
        var diff = (transform.position.x + 8) / 16 - Mathf.Clamp(normalizedPositions,0, 1);
        if (Math.Abs(diff) > float.Epsilon)
        {
            transform.position += Vector3.left * (Math.Sign(diff) * Math.Min(movespeed * Time.deltaTime, Math.Abs(diff)));
        }
    }

    private void FixedUpdate()
    {
        if (shotCooldown != 4)
        {
            if (!canShoot)
            {
                if (cooldown < shotCooldown)
                {
                    cooldown += Time.fixedDeltaTime;
                }
                else
                {
                    canShoot = true;
                }
                slider.value = cooldown;
            }
            else
            {

                shaker.ShakeOnce(0.5f, 10f, 0.1f, 0.1f);
                projectile = Instantiate(projectilePrefab, controller.position, Quaternion.LookRotation(Vector3.forward));
                fireSparks.Play();
                canShoot = false;
                cooldown = 0;
            }
        }
    }
}

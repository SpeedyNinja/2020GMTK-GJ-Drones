using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ShipController : MonoBehaviour
{
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
    
    float[] samples = new float[1024];
    List<float> movingAverageSamples = CreateList<float>(128);

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

        for (int i = 0; i < movingAverageSamples.Count; i++)
        {
            movingAverageSamples[i] = decayRate * samples[i] + (1 - decayRate) * movingAverageSamples[i];
        }

        // var output = ZScore.StartAlgo(movingAverageSamples, 5, 10, 0);

        for (int i = 1; i < movingAverageSamples.Count - 1; i++)
        {
            Debug.DrawLine(new Vector3(i - 1, Mathf.Log(movingAverageSamples[i - 1]) + 10, 2), new Vector3(i, Mathf.Log(movingAverageSamples[i]) + 10, 2), Color.cyan);
        }
        
        float max = 0;
        // int maxIdx = 0;
        var maxIdx = movingAverageSamples.Select((v, i) => new {v, i})
            .OrderByDescending(c => c.v)
            .Take(3)
            .Min(c => c.i);

        var lvlMax = movingAverageSamples.Average() * 1000;
        
        if (lvlMax > 0.075)
        {
            normalizedPositions = (maxIdx - 4) * 0.05f;
            shotCooldown = Math.Min(1 / lvlMax * shotCooldownMultiplier, 4);
            slider.maxValue = shotCooldown;
            var rect = _rectTransformCooldownSlider.rect;
            _rectTransformCooldownSlider.sizeDelta = new Vector2(shotCooldown * 150, 50);
            Debug.Log(maxIdx + " " + lvlMax);
        }

        var diff = (transform.position.x + 5) / 10 - Mathf.Clamp(normalizedPositions,0, 1);
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
            
                projectile = Instantiate(projectilePrefab, controller.position, Quaternion.LookRotation(Vector3.forward));
                canShoot = false;
                cooldown = 0;
            }
        }
    }
}

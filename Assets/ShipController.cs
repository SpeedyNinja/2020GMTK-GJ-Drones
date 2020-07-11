using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float shotCooldown;

    public float normalizedPositions;
    public float shotCooldownMultiplier;

    AudioClip microphoneInput;
    private AudioSource source;
    private Transform controller;
    private GameObject projectile;
    private bool canShoot;
    private float cooldown;
    
    float[] samples = new float[512];
    List<float> movingAverageSamples = CreateList<float>(128);

    public float decayRate = 0.5f;
    
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
            normalizedPositions = (maxIdx - 2) * 0.1f;
            shotCooldown = 1 / lvlMax * shotCooldownMultiplier;
            Debug.Log(maxIdx + " " + lvlMax);
        }
    }

    private void FixedUpdate()
    {
        
        if (canShoot == false)
        {
            if (cooldown > 0)
            {
                cooldown -= Time.deltaTime;
            }
            else
            {
                canShoot = true;
                cooldown = shotCooldown;
            }
        }
        else if (canShoot == true && Input.GetAxis("Vertical") != 0)
        {
            projectile = Instantiate(projectilePrefab, controller.position, Quaternion.LookRotation(Vector3.forward));
            canShoot = false;
        }
    }
}

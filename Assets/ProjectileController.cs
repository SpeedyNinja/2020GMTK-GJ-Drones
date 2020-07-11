using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float shotCooldown;

    AudioClip microphoneInput;
    private AudioSource source;
    private Transform controller;
    private GameObject projectile;
    private bool canShoot;
    private float cooldown;
    
    float[] samples = new float[512];

    
    // Start is called before the first frame update
    void Start()
    {
        controller = gameObject.GetComponent<Transform>();
        projectile = null;
        canShoot = false;
        cooldown = 0;
        
        if (Microphone.devices.Length>0){
            microphoneInput = Microphone.Start(Microphone.devices[0],true,2,44100);
            source = GetComponent<AudioSource>();
            source.clip = microphoneInput;
            source.loop = true;
            source.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        int dec = 128;
        float[] waveData = new float[dec];
        
        source.GetSpectrumData(samples, 0, FFTWindow.Hamming);
        int micPosition = Microphone.GetPosition(null)-(dec+1); // null means the first microphone
        microphoneInput.GetData(waveData, micPosition);

        for (int i = 1; i < samples.Length - 1; i++)
        {
            Debug.DrawLine(new Vector3(i - 1, samples[i] + 10, 0), new Vector3(i, samples[i + 1] + 10, 0), Color.red);
            Debug.DrawLine(new Vector3(i - 1, Mathf.Log(samples[i - 1]) + 10, 2), new Vector3(i, Mathf.Log(samples[i]) + 10, 2), Color.cyan);
            Debug.DrawLine(new Vector3(Mathf.Log(i - 1), samples[i - 1] - 10, 1), new Vector3(Mathf.Log(i), samples[i] - 10, 1), Color.green);
            Debug.DrawLine(new Vector3(Mathf.Log(i - 1), Mathf.Log(samples[i - 1]), 3), new Vector3(Mathf.Log(i), Mathf.Log(samples[i]), 3), Color.blue);
        }
        
        float max = 0;
        int maxIdx = 0;
        for (var i = 0; i < samples.Length; i++)
        {
            var sample = samples[i];
            if (sample > max)
            {
                max = sample;
                maxIdx = i;
            }
        }


        // Getting a peak on the last 128 samples
        float levelMax = 0;
        for (int i = 0; i < dec; i++) {
            float wavePeak = Math.Abs(waveData[i]);
            if (levelMax < wavePeak) {
                levelMax = wavePeak;
            }
        }

        if (levelMax > 0.05)
        {
            Debug.Log(maxIdx);
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

﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DroneController : MonoBehaviour
{
    public GameObject drone;
    public int difficulty;
    
    private Transform _controller;
    private float _droneSpawnRate;
    private float _droneSpawnCountDown;
    private float _halfScreen;
    
    // Start is called before the first frame update
    void Start()
    {
        var cameraMain = Camera.main;
        var orthographicSize = 0f;
        _halfScreen = 0;
        if (cameraMain != null)
        {
            orthographicSize = cameraMain.orthographicSize;
            _halfScreen = cameraMain.aspect * orthographicSize * 0.7f;
        }
        gameObject.transform.position = new Vector3(0, orthographicSize, gameObject.transform.position.z);
        _droneSpawnRate = 10f / (difficulty + 1);
        _droneSpawnCountDown = 0;
        _controller = gameObject.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (_droneSpawnCountDown > 0)
        {
            _droneSpawnCountDown -= Time.deltaTime;
        }
        else
        {
            difficulty = Score.MainScore._score / 10 + 1;
            
            var newXPos = Random.Range(-_halfScreen, _halfScreen);
            var newYPos = _controller.position.y;
            var newHealth = Math.Min(Random.value * difficulty / 2 + 1, 6);
            var newColour = Color.HSVToRGB(Random.value, 1, 1, true);
            var newScale = Convert.ToSingle((newHealth - 1) / 6 / 2 + 1);
            var newSpeed = newScale;
            var newZigOffset = Convert.ToSingle(Random.value * Math.PI * 2);
            var newZigAmount = Convert.ToSingle(Random.value * 0.6);
            var newCanFire = newHealth > 2 && (Random.value > (difficulty / 10f));
            
            var droneInstance = Instantiate(drone, new Vector3(newXPos, newYPos, _controller.position.z), Quaternion.identity);
            var droneScript = droneInstance.GetComponent<DroneControl>();
            droneScript.SetVars(newHealth, newCanFire, newScale, newColour, newSpeed, newZigOffset, newZigAmount);
            _droneSpawnCountDown = - 1 / 2f * difficulty + 5;
        };
    }
}

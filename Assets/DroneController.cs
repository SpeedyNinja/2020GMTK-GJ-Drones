﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DroneController : MonoBehaviour
{
    public GameObject drone;
    
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
        gameObject.transform.position = new Vector3(0, orthographicSize);
        _droneSpawnRate = 4;
        _droneSpawnCountDown = _droneSpawnRate;
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
            var newXPos = Random.Range(-_halfScreen, _halfScreen);
            var newYPos = _controller.position.y;
            var newHealth = Random.value * 2 + 1;
            var newScale = Convert.ToSingle((newHealth - 1) / 2 / 2 + 1);
            var newSpeed = newScale;
            var newZigOffset = Convert.ToSingle(Random.value * Math.PI * 2);
            var newZigAmount = Convert.ToSingle(Random.value * 0.6);
            
            var droneInstance = Instantiate(drone, new Vector3(newXPos, newYPos), Quaternion.identity);
            var droneScript = droneInstance.GetComponent<DroneControl>();
            droneScript.SetVars(newHealth, newScale, newSpeed, newZigOffset, newZigAmount);
            _droneSpawnCountDown = _droneSpawnRate;
        };
    }
}

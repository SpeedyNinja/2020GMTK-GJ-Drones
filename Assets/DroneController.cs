using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            _halfScreen = cameraMain.aspect * orthographicSize * 0.8f;
        }
        gameObject.transform.position = new Vector3(0, orthographicSize);
        _droneSpawnRate = 2;
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
            Instantiate(drone, new Vector3(Random.Range(-_halfScreen, _halfScreen), _controller.position.y), 
                Quaternion.Euler(0,0,180));
            _droneSpawnCountDown = _droneSpawnRate;
        }
    }
}

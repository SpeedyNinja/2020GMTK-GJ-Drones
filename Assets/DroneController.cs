using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneController : MonoBehaviour
{
    public GameObject drone;
    public int maxDrones;
    
    private Transform controller;
    private List<GameObject> drones;
    
    // Start is called before the first frame update
    void Start()
    {
        controller = gameObject.GetComponent<Transform>();
        drones = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (drones.Count < maxDrones)
        {
            var newDrone = Instantiate(drone, controller.position, Quaternion.identity);
            newDrone.transform.parent = controller;
            drones.Add(newDrone);
        }
    }
}

﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneControl : MonoBehaviour
{
    private Rigidbody rb;
    private BoxCollider bc;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        bc = gameObject.GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        rb.AddForce(rb.transform.right * 2);
    }

    public void ExitBoundary()
    {
        rb.transform.SetPositionAndRotation(rb.transform.parent.position, Quaternion.identity);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneControl : MonoBehaviour
{
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
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

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("Boom");
        rb.transform.SetPositionAndRotation(rb.transform.parent.position, Quaternion.identity);
    }
}

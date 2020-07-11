using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class DroneControl : MonoBehaviour
{
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.AddForce(rb.transform.right * 20, ForceMode2D.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        
    }

    public void ExitBoundary()
    {
        rb.transform.SetPositionAndRotation(rb.transform.parent.position, Quaternion.identity);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        rb.transform.SetPositionAndRotation(rb.transform.parent.position, Quaternion.identity);
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;
        rb.AddForce(rb.transform.right * 20, ForceMode2D.Impulse);
    }
}

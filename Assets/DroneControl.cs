using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using Object = UnityEngine.Object;

public class DroneControl : MonoBehaviour
{
    private Rigidbody2D rb;

    private bool dying;
    private float deathDelay;
    private float deathCountdown;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.AddForce(Vector2.down * 2, ForceMode2D.Impulse);
    }

    private void FixedUpdate()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("hit a thing");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Object.Destroy(gameObject);
    }
}

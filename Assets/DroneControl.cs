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
    private float health;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.AddForce(Vector2.down * 2, ForceMode2D.Impulse);
        health = 1;
    }

    private void FixedUpdate()
    {
        if (health <= 0)
        {
            // TODO: fancy blow up
            Object.Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("hit a thing");
        health -= 1;
        
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Object.Destroy(gameObject);
    }
}

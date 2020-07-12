using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class ProjectileControl : MonoBehaviour
{
    private Rigidbody2D rb;
    public GameObject collisionSparks;
    
    
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name == "Ship") return;
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.name == "Ship") return;
        Instantiate(collisionSparks, transform.position, Quaternion.LookRotation(other.GetContact(0).normal));
        Destroy(gameObject);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private Rigidbody2D rb;
    public GameObject collisionSparks;
    public float bulletSpeed;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        transform.SetPositionAndRotation(transform.position + transform.up * (bulletSpeed * Time.deltaTime), transform.rotation);
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        Destroy(gameObject);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Ship")
        {
            Lives.MainLives.LooseLife();
            Destroy(gameObject);
        }
    }
}

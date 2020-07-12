using System;
using System.Collections;
using System.Collections.Generic;
using EZCameraShake;
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
            Instantiate(collisionSparks, transform.position, Quaternion.LookRotation(other.transform.position - transform.position));;
            CameraShaker.Instance.ShakeOnce(10f, 10f, 0.25f, 0.25f);
        }
    }
}

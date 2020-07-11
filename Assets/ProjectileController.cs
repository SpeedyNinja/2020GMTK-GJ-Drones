using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float shotCooldown;

    private Transform controller;
    private Rigidbody projectile;
    private bool canShoot;
    private float cooldown;
    
    // Start is called before the first frame update
    void Start()
    {
        controller = gameObject.GetComponent<Transform>();
        projectile = null;
        canShoot = false;
        cooldown = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        
        if (canShoot == false)
        {
            if (cooldown > 0)
            {
                cooldown -= Time.deltaTime;
            }
            else
            {
                var instance = Instantiate(projectilePrefab, controller.position, Quaternion.identity);
                instance.transform.parent = gameObject.transform;
                projectile = instance.GetComponent<Rigidbody>();
                canShoot = true;
                cooldown = shotCooldown;
            }
        }
        else if (canShoot == true && Input.GetAxis("Vertical") != 0)
        {
            projectile.AddForce(Vector3.forward * 10, ForceMode.Impulse);
            projectile.useGravity = true;
            canShoot = false;
        }
    }
}

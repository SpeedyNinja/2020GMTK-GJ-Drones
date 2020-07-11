using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class ProjectileControl : MonoBehaviour
{
    private Rigidbody2D rb;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
    }

    public void ExitBoundary()
    {
        Object.Destroy(gameObject);
    }
}

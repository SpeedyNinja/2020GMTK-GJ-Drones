using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignBoundary : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var camera = Camera.main;
        var halfScreen = camera.aspect * camera.orthographicSize;
        gameObject.transform.localScale = new Vector3(halfScreen * 2, camera.orthographicSize * 2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

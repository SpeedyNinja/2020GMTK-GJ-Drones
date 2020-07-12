using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EZCameraShake;
using UnityEngine;
using UnityEngine.UI;

public class ShipController : MonoBehaviour
{
    public CameraShaker shaker;
    public GameObject projectilePrefab;
    public float shotCooldown;

    public float normalizedPositions;
    public float shotCooldownMultiplier;

    public Slider slider;
    
    private Transform controller;
    private GameObject projectile;
    private bool canShoot;
    public float cooldown;

    public bool glitching = false;

    public float mostDiffCutoff = 5;
    public float lowVolCutoff = 2f;

    public ParticleSystem fireSparks;

    [SerializeField] private float movespeed;
    public RectTransform _rectTransformCooldownSlider;
    public SpriteRenderer _spriteRenderer;
    // private static readonly int GlitchAmount = Shader.PropertyToID("_GlitchAmount");

    public static float lowestPitchPoint = 4;
    public static float highestPitchPoint = 24;

    public static float highestVolumePoint = 8;
    public static float lowestVolumePoint = 2;
    
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rectTransformCooldownSlider = slider.GetComponentInParent<RectTransform>();
    }

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
        var diff = (transform.position.x + 8) / 16 - Mathf.Clamp(normalizedPositions,0, 1);
        if (Math.Abs(diff) > float.Epsilon)
        {
            transform.position += Vector3.left * (Math.Sign(diff) * Math.Min(movespeed * Time.deltaTime, Math.Abs(diff)));
        }
    }
    public static Vector3 CalcParabolaVertex(int x1, int y1, int x2, int y2, int x3, int y3)
    {
        
        double denom = (x1 - x2) * (x1 - x3) * (x2 - x3);
        double A     = (x3 * (y2 - y1) + x2 * (y1 - y3) + x1 * (y3 - y2)) / denom;
        double B     = (x3*x3 * (y1 - y2) + x2*x2 * (y3 - y1) + x1*x1 * (y2 - y3)) / denom;
        double C     = (x2 * x3 * (x2 - x3) * y1 + x3 * x1 * (x3 - x1) * y2 + x1 * x2 * (x1 - x2) * y3) / denom;

        return new Vector3((float)A, (float)B, (float)C);
    }

    private void FixedUpdate()
    {
        if (shotCooldown != 2)
        {
            if (!canShoot)
            {
                if (cooldown < shotCooldown)
                {
                    cooldown += Time.fixedDeltaTime;
                }
                else
                {
                    canShoot = true;
                }
                slider.value = cooldown;
            }
            else
            {

                shaker.ShakeOnce(0.5f, 10f, 0.1f, 0.1f);
                projectile = Instantiate(projectilePrefab, controller.position, Quaternion.LookRotation(Vector3.forward));
                fireSparks.Play();
                canShoot = false;
                cooldown = 0;
            }
        }
    }
}

public static class ExtensionMethods {
     
    public static float Remap (this float value, float from1, float to1, float from2, float to2) {
        
        float normal = ( value - from1 ) / ( from2 - from1 );
        float bValue = Mathf.LerpUnclamped(to1, to2, normal);
        return bValue;
    }
       
}


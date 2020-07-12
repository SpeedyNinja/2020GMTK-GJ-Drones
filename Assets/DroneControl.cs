using System;
using System.Collections;
using System.Collections.Generic;
using EZCameraShake;
using UnityEditor.UI;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class DroneControl : MonoBehaviour
{
    public float deathDelay;
    public GameObject projectile;
    public GameObject deathParticles;
    public GameObject winParticles;
    public float reloadDelay;
    
    private Transform _transform;
    private SpriteRenderer _spriteRenderer;

    private Color _colour;
    private float _scale;

    private bool _canFire;
    private float _bulletReloadTimer;
    private bool _dying;
    private float _deathCountdown;
    private float _health;
    private float _maxHealth;
    private float _speed;
    private float _zigOffset;
    private float _zigAmount;
    private float _lowerLimit;
    private float _z;
    private Vector3 _startPos;
    private static readonly int GlowVal = Shader.PropertyToID("_GlowVal");

    // Start is called before the first frame update
    void Start()
    {
        _transform = gameObject.GetComponent<Transform>();
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        _transform.localScale = new Vector3(5 * _scale, 5 * _scale, 1);
        _spriteRenderer.color = _colour;
        _startPos = _transform.position;
        _maxHealth = _health;
        _deathCountdown = deathDelay;
        _lowerLimit = - Camera.main.orthographicSize - 1;
    }

    public void SetVars(float newHealth, bool canFire, float scale, Color newColour, float newSpeed, float zagOffset, float zagAmount)
    {
        _canFire = canFire;
        _health = newHealth;
        _speed = newSpeed;
        _scale = scale;
        _colour = newColour;
        _zigOffset = zagOffset;
        _zigAmount = zagAmount;
    }

    private void Update()
    {
        
        if (!_dying)
        {
            if (_canFire)
            {
                if (_bulletReloadTimer < reloadDelay)
                {
                    _bulletReloadTimer += Time.deltaTime;
                }
                else
                {
                    Instantiate(projectile, _transform.position + _transform.up, _transform.rotation);
                    _bulletReloadTimer = 0;
                }
            }
            var newY = _transform.position.y - _speed * Time.deltaTime;
            var xDelta = Convert.ToSingle(_zigAmount * Math.Sin(newY + _zigOffset));
            var newX = xDelta + _startPos.x;
            var pos = new Vector3(newX, newY, _startPos.z);
            var angle = Quaternion.Euler(0, 0, Convert.ToSingle(-180 - (_zigAmount * Math.Cos(newY + _zigOffset) * 45)));
            _transform.SetPositionAndRotation(pos, angle);
            _spriteRenderer.material.SetFloat(GlowVal, _health / _maxHealth + 1);
        }
        else
        {
            if (_deathCountdown > 0)
            {
                _deathCountdown -= Time.deltaTime;
                _spriteRenderer.color = new Color(_colour.r, _colour.g, _colour.b, _deathCountdown / deathDelay);
                var newY = _transform.position.y - _speed * _deathCountdown / deathDelay * Time.deltaTime;
                var pos = new Vector3(_transform.position.x, newY, _startPos.z);
                var angle = Quaternion.Euler(0, 0, _transform.rotation.eulerAngles.z + (Time.deltaTime * 360 * _deathCountdown / deathDelay));
                _transform.SetPositionAndRotation(pos, angle);
            }
            else
            {
                Object.Destroy(gameObject);
            }
        }
    }

    private void FixedUpdate()
    {
        if (_transform.position.y < _lowerLimit)
        {
            Lives.MainLives.LooseLife();
            Object.Destroy(gameObject);
            CameraShaker.Instance.ShakeOnce(3f, 10f, 0.25f, 0.25f);
            Instantiate(winParticles, transform.position, Quaternion.identity);
        }
        if (!_dying && _health <= 0)
        {
            Instantiate(deathParticles, transform.position, Quaternion.identity, transform);
            CameraShaker.Instance.ShakeOnce(2, 5, 0.25f, 0.25f);
            gameObject.GetComponent<PolygonCollider2D>().enabled = false;
            _dying = true;
            Score.MainScore.Scored();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (_health > 0) _health -= 1;
    }
}

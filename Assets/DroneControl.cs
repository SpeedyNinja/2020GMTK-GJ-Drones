﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class DroneControl : MonoBehaviour
{
    private Transform _transform;
    private SpriteRenderer _spriteRenderer;

    private Color _colour;
    private float _scale;

    private bool _dying;
    private float _deathDelay;
    private float _deathCountdown;
    private float _health;
    private float _max_health;
    private float _speed;
    private float _zigOffset;
    private float _zigAmount;
    private Vector3 _startPos;
    private static readonly int GlowVal = Shader.PropertyToID("_GlowVal");

    // Start is called before the first frame update
    void Start()
    {
        _transform = gameObject.GetComponent<Transform>();
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        _transform.localScale = new Vector3(5 * _scale, 5 * _scale, 1);
        _spriteRenderer.color = new Color((_health - 1) / 5f, 1 - ((_health - 1) / 5f), 1);
        _startPos = _transform.position;
        var val = Random.value + 1;
        _spriteRenderer.material.SetFloat(GlowVal, val);
        _max_health = _health;
    }

    public void SetVars(float newHealth, float scale, float newSpeed, float zagOffset, float zagAmount)
    {
        _health = newHealth;
        _speed = newSpeed;
        _scale = scale;
        _zigOffset = zagOffset;
        _zigAmount = zagAmount;
    }

    private void Update()
    {
        var newY = _transform.position.y - _speed * Time.deltaTime;
        var xDelta = Convert.ToSingle(_zigAmount * Math.Sin(newY + _zigOffset));
        var newX = xDelta + _startPos.x;
        var pos = new Vector3(newX, newY);
        var angle = Quaternion.Euler(0, 0, Convert.ToSingle(-180 - (_zigAmount * Math.Cos(newY + _zigOffset) * 45)));
        _transform.SetPositionAndRotation(pos, angle);
        _spriteRenderer.color = new Color(1 - _health / _max_health, _health / _max_health, 0);
    }

    private void FixedUpdate()
    {
        if (_health <= 0)
        {
            // TODO: fancy blow up
            Object.Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        _health -= 1;
        Debug.Log("hit a thing" + _health);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Object.Destroy(gameObject);
    }
}

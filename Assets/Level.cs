using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class Level : MonoBehaviour
{

    public static Level MainLevel;
    public float textSizeMultiplier;
    public float textPopTime;
    
    private TextMeshProUGUI textMesh;
    private float _baseTextSize;
    private int _letPast;
    public int _level;
    private int _setLevel;
    private float _textPopCountup;
    
    // Start is called before the first frame update
    void Start()
    {
        MainLevel = this;
        textMesh = gameObject.GetComponent<TextMeshProUGUI>();
        _baseTextSize = textMesh.fontSize;
        _level = 0;
        _setLevel = 0;
        _textPopCountup = 1000;
    }

    // Update is called once per frame
    void Update()
    {
        if (_textPopCountup < textPopTime)
        {
            _textPopCountup += Time.deltaTime;
            textMesh.fontSize = _baseTextSize * (float)Math.Max(-Math.Pow(2 * _textPopCountup - 0.5, 2) + textSizeMultiplier, 1);
        }
        else
        {
            textMesh.fontSize = _baseTextSize;
            if (_setLevel != _level)
            {
                _level = _setLevel;
                _setLevel = 0;
                textMesh.SetText("" + _level);
                _textPopCountup = 0;
            }
        }
    }

    public void SetLevel(int newLevel)
    {
        _setLevel = newLevel + 1;
    }
}

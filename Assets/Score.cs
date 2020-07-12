using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    public float textSizeMultiplier;
    public float textPopTime;
    
    private TextMeshPro textMesh;
    private float _baseTextSize;
    private int _letPast;
    private int _score;
    private int _addToScore;
    private float _textPopCountup;
    
    // Start is called before the first frame update
    void Start()
    {
        textMesh = gameObject.GetComponent<TextMeshPro>();
        _baseTextSize = textMesh.fontSize;
        _score = 0;
        _addToScore = 0;
        _textPopCountup = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (_textPopCountup < textPopTime)
        {
            _textPopCountup += Time.deltaTime;
            textMesh.fontSize = _baseTextSize * (float)(-Math.Pow(2 * _textPopCountup - 0.5, 2) + textSizeMultiplier);
        }
        else
        {
            if (_addToScore > 0)
            {
                _score += _addToScore;
                textMesh.SetText("" + _score);
                _textPopCountup = 0;
            }
        }
    }

    public void Scored()
    {
        _addToScore += 1;
    }

    public void Missed()
    {
        _letPast += 1;
    }
}

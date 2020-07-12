using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Lives : MonoBehaviour
{

    public static Lives MainLives;
    public float textSizeMultiplier;
    public float textPopTime;
    public int startLives;
    
    private TextMeshProUGUI textMesh;
    private float _baseTextSize;
    private int _letPast;
    private int _score;
    private int _addToScore;
    private float _textPopCountup;
    
    // Start is called before the first frame update
    void Start()
    {
        MainLives = this;
        textMesh = gameObject.GetComponent<TextMeshProUGUI>();
        _baseTextSize = textMesh.fontSize;
        _score = startLives;
        _addToScore = 0;
        _textPopCountup = 1000;
        textMesh.SetText("" + _score);
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
            if (_addToScore < 0)
            {
                _score += _addToScore;
                _addToScore = 0;
                if (_score <= 0)
                {
                    SceneManager.LoadScene("Scenes/GameOver");
                }
                textMesh.SetText("" + _score);
                _textPopCountup = 0;
            }
        }
    }

    public void LooseLife()
    {
        _addToScore -= 1;
    }
}

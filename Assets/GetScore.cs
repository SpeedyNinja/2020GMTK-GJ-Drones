using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GetScore : MonoBehaviour
{
    private TextMeshProUGUI _textMeshProUgui;

    // Start is called before the first frame update
    void Start()
    {
        _textMeshProUgui = gameObject.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        _textMeshProUgui.text = "" + Score.MainScore._score;
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameOverScreen : MonoBehaviour
{
    
    public TextMeshProUGUI textMeshProUgui;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        textMeshProUgui.text = "" + Score.MainScore._score;
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("Scenes/Menu");
    }
    
    public void TryAgain()
    {
        SceneManager.LoadScene("Scenes/MainScene");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    public GameObject MainMenuUI;
    public GameObject GameUI;
    public GameObject GameOverUI;
    public Text textDistance;
    public Text textScore;
    public Text textScoreBest;
    private GameController game;

    private void Start()
    {
        game = FindObjectOfType<GameController>();
        MainMenuUI.SetActive(true);
    }

    private void Update()
    {
        textDistance.text = game.GetDistance().ToString("F0");
        textScore.text = game.GetDistance().ToString("F0");
        textScoreBest.text = PlayerPrefs.GetInt("BestScore", 0).ToString("F0");
    }
    
    public void ToggleMainMenu(bool enabled)
    {
        ToggleMainMenu(enabled, 0);
    }
    
    public void ToggleGame(bool enabled)
    {
        ToggleGame(enabled, 0);
    }

    public void ToggleGameOver(bool enabled)
    {
        ToggleGameOver(enabled, 0);
    }

    public void ToggleMainMenu(bool enabled, float time)
    {
        StartCoroutine(ToggleGameObject(MainMenuUI, enabled, time));
    }
    
    public void ToggleGame(bool enabled, float time)
    {
        StartCoroutine(ToggleGameObject(GameUI, enabled, time));
    }

    public void ToggleGameOver(bool enabled, float time)
    {
        StartCoroutine(ToggleGameObject(GameOverUI, enabled, time));
    }

    private IEnumerator ToggleGameObject(GameObject gameObject, bool state, float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(state);
    }
}

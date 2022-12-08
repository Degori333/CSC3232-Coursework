using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class GameManager : MonoBehaviour
{
    public static GameManager gameManager { get; private set; }

    public enum GameState
    {
        MainMenu,
        BattleShips,
        TradingSim
    }

    public GameState gameState = GameState.MainMenu;

    public enum TimeState
    {
        Paused,
        InPlay
    }

    public TimeState timeState = TimeState.InPlay;

    // Start is called before the first frame update
    void Awake()
    {
        if (gameManager == null)
        {

            gameManager = this;
            DontDestroyOnLoad(gameObject);

        }
        else if (gameManager != this)
        {
            Destroy(gameManager.gameObject);
            gameManager = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Update()
    {
        TimeInuptHandler();
    }

    public void ResumeGame()
    {
        timeState = TimeState.InPlay;
        Time.timeScale = 1;
    }

    public void PauseGame()
    {
        timeState = TimeState.Paused;
        Time.timeScale = 0;
    }

    public void ToggleActive(GameObject obj)
    {
        obj.SetActive(!obj.activeSelf);
    }

    public void SwitchScene(int sceneIndex)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneIndex);
    }

    public void TimeInuptHandler()
    {
        
    }

    public void ExitGame()
    {
        Application.Quit();
    }

}

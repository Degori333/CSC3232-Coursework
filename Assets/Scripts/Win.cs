using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Win : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject resumeButton;
    public TextMeshProUGUI winText;
    public TextMeshProUGUI buyButton;
    public int winPrice = 10000;
    void Start()
    {
        buyButton.text = "Win\n" + winPrice + "c";
    }

    public void Buy(UnitFunctionality ship)
    {
        if(ship.CoinsPossessed >= winPrice)
        {
            ship.CoinsPossessed -= winPrice;
            WinGame(ship);
        }
    }

    public void WinGame(UnitFunctionality ship)
    {
        GameManager.gameManager.PauseGame();
        GameManager.gameManager.ToggleActive(pauseMenu);
        GameManager.gameManager.ToggleActive(resumeButton);
        winText.text = ship.gameObject.name + " Won";
        GameManager.gameManager.ToggleActive(winText.gameObject);
    }

    // Update is called once per frame
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            buyButton.gameObject.transform.parent.gameObject.SetActive(true);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            buyButton.gameObject.transform.parent.gameObject.SetActive(true);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController : UnitFunctionality
{
    private bool inRangeOfDest = true;
    public GameObject pauseMenu;
    public GameObject reloadTextObject;
    private TextMeshProUGUI reloadText;
    public GameObject healthBar;
    private Slider healthSlider;
    [SerializeField] TextMeshProUGUI cargoText;
    [SerializeField] TextMeshProUGUI coinsText;

    override public float HealthPoints
    {
        get
        {
            return base.HealthPoints;
        }
        set
        {
            base.HealthPoints = value;
            if(healthSlider != null) healthSlider.value = value;
        }
    }
    override public int CurrCargo 
    {
        get
        {
            return base.CurrCargo;
        }
        set 
        {
            base.CurrCargo = value;
            if (cargoText != null)
            {
                cargoText.text = "Cargo: " + CurrCargo + "/" + MaxCargo;
            }
            
        }
    }

    override public int CoinsPossessed
    {
        get 
        {
            return base.CoinsPossessed; 
        }
        set
        { 
            base.CoinsPossessed = value;
            if (coinsText != null)
            {
                coinsText.text = "Coins: " + CoinsPossessed;
            }
        }
    }

    override protected void Start()
    {
        base.Start();
        destination = GameObject.FindGameObjectWithTag("DestinationPoint").GetComponent<Transform>();
        if(reloadTextObject != null)
        {
            reloadText = reloadTextObject.GetComponent<TextMeshProUGUI>();
        }
        if(healthBar != null)
        {
            healthSlider = healthBar.GetComponent<Slider>();
            healthSlider.maxValue = unitData.healthPoints;
            healthSlider.value = healthSlider.maxValue;
        }
        CoinsPossessed = 1000;
        CurrCargo = 0;
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
    }

    

    private void Update()
    {
        CheckInput();
        if (!inRangeOfDest)
        {
            MoveUnit(destination.position);
        }
        else if(agent.isStopped)
        {
            agent.ResetPath();
        }
        if (reloadText != null)
        {
            if (reloading)
            {
                reloadText.text = "<color=red>Reloading...</color>";
            }
            else
            {
                reloadText.text = "<color=green>Cannons Ready</color>";
            }
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Plane ground = new Plane(Vector3.up, Vector3.zero);

        float rayLength;

        if (ground.Raycast(ray, out rayLength))
        {
            return ray.GetPoint(rayLength);
        }
        else
        {
            return Vector3.zero;
        }

    }

    private void CheckInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            destination.position = GetMouseWorldPosition();
            inRangeOfDest = false;
        }
        if (GameManager.gameManager.gameState == GameManager.GameState.BattleShips)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Shoot(cannonsLeftSide);
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                Shoot(cannonsRightSide);
            }
        }

        if (pauseMenu == null)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {

            if (GameManager.gameManager.gameState == GameManager.GameState.BattleShips || GameManager.gameManager.gameState == GameManager.GameState.TradingSim)
            {
                if (GameManager.gameManager.timeState == GameManager.TimeState.Paused)
                {
                    GameManager.gameManager.ResumeGame();
                }
                else
                {
                    GameManager.gameManager.PauseGame();
                }
                GameManager.gameManager.ToggleActive(pauseMenu);
            }
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("DestinationPoint") && !inRangeOfDest)
        {
            inRangeOfDest = true;
        }
    }
}

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

    override public float HealthPoints
    {
        get
        {
            return base.HealthPoints;
        }
        set
        {
            base.HealthPoints = value;
            healthSlider.value = HealthPoints;
        }
    }

    override protected void Start()
    {
        base.Start();
        destination = GameObject.FindGameObjectWithTag("DestinationPoint").GetComponent<Transform>();
        reloadText = reloadTextObject.GetComponent<TextMeshProUGUI>();
        healthSlider = healthBar.GetComponent<Slider>();
        healthSlider.maxValue = unitData.healthPoints;
        healthSlider.value = healthSlider.maxValue;
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
        if (reloading)
        {
            reloadText.text = "<color=red>Reloading...</color>";
        }
        else
        {
            reloadText.text = "<color=green>Cannons Ready</color>";
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

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Shoot(cannonsLeftSide);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            Shoot(cannonsRightSide);
        }

        if (pauseMenu == null)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {

            if (GameManager.gameManager.gameState == GameManager.GameState.BattleShips)
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
}

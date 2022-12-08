using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    private PlayerController player;
    private UnitData playerShipData;
    [SerializeField] private GameObject cameraMain;
    private CameraController cameraController;
    [Space(20)]
    [Header("Ship prefabs")]
    [SerializeField] private GameObject ship_lvl2;
    [SerializeField] private GameObject ship_lvlMAX;
    [Space(20)]


    [Header("Current Upgrades Purchased")]
    [SerializeField] private int currUpgradeMS = 0;
    [SerializeField] private int currUpgradeHP = 0;
    [SerializeField] private int currUpgradeAttack = 0;
    [SerializeField] private int currUpgradeReload = 0;
    [SerializeField] private int currUpgradeSS = 0;
    [Space(20)]

    [Header("Upgrade Buttons")]
    [SerializeField] private GameObject lvlUpButton;
    [SerializeField] private GameObject upgradeMSButton;
    [SerializeField] private GameObject upgradeHPButton;
    [SerializeField] private GameObject upgradeAttackButton;
    [SerializeField] private GameObject upgradeReloadButton;
    [SerializeField] private GameObject upgradeSSButton;
    [Space(20)]

    [Header("Stat Textboxes")]
    [SerializeField] private GameObject statLvlText;
    [SerializeField] private GameObject statMSText;
    [SerializeField] private GameObject statHPText;
    [SerializeField] private GameObject statAttackText;
    [SerializeField] private GameObject statReloadText;
    [SerializeField] private GameObject statSSText;
    [Space(20)]

    [SerializeField] private GameObject coinsText;


    private void Start()
    {
        FindPlayer();
        cameraController = cameraMain.GetComponent<CameraController>();
        UpdateAllTextBoxes();
        if (!cameraController.PlayerAnchored)
        {
            cameraController.LockCameraOnPlayer();
        }
    }

    void FindPlayer()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        playerShipData = player.unitData;
    }

    bool CanAfford(int cost)
    {
        if(player.CoinsPossessed < cost)
        {
            return false;
        }
        else
        {
            player.CoinsPossessed -= cost;
            return true;
        }
    }

    TextMeshProUGUI GetTextComponent(GameObject gameObject)
    {
        return gameObject.GetComponent<TextMeshProUGUI>();
    }

    void UpdateUpgradeButtonText(TextMeshProUGUI text ,string name, float upgAmount, int curr, int max, int cost)
    {
        text.text = name + upgAmount + "\n" + "(" + curr + "/" + max + ")\ncost: " + cost;
    }


    void UpdateStatText(TextMeshProUGUI text, string initText)
    {
        text.text = initText;
    }
    void UpdateStatText(TextMeshProUGUI text, string initText, float amount)
    {
        text.text = initText + amount;
    }
    

    void UpdateCoins()
    {
        UpdateStatText(GetTextComponent(coinsText), "Coins: ", player.CoinsPossessed);
    }

    void UpdateAllTextBoxes()
    {
        UpdateLvlText();
        UpdateMSText();
        UpdateHPText();
        UpdateAttackText();
        UpdateReloadText();
        UpdateSSText();

        UpdateCoins();
    }

    public void UpgradeMoveSpeed()
    {
        
        if(currUpgradeMS < playerShipData.maxUpgradeMS)
        {
            bool affordable = CanAfford(playerShipData.upgradeCostMS);
            if (!affordable)
            {
                return;
            }
            else
            {
                player.MoveSpeed += playerShipData.amountUpgradedMS;
                currUpgradeMS++;
                UpdateMSText();
                UpdateCoins();
            }
        }
    }
    void UpdateMSText()
    {
        UpdateUpgradeButtonText(GetTextComponent(upgradeMSButton), "Move Speed +", playerShipData.amountUpgradedMS, currUpgradeMS, playerShipData.maxUpgradeMS, playerShipData.upgradeCostMS);
        UpdateStatText(GetTextComponent(statMSText), "Move Speed: ", player.MoveSpeed);
    }

    public void UpgradeHealth()
    {

        if (currUpgradeHP < playerShipData.maxUpgradeHP)
        {
            bool affordable = CanAfford(playerShipData.upgradeCostHP);
            if (!affordable)
            {
                return;
            }
            else
            {
                player.HealthPoints += playerShipData.amountUpgradedHP;
                currUpgradeHP++;
                UpdateHPText();
                UpdateCoins();
            }
        }
    }

    void UpdateHPText()
    {
        UpdateUpgradeButtonText(GetTextComponent(upgradeHPButton), "Health Points +", playerShipData.amountUpgradedHP, currUpgradeHP, playerShipData.maxUpgradeHP, playerShipData.upgradeCostHP);
        UpdateStatText(GetTextComponent(statHPText), "Health Points: ", player.HealthPoints);
    }

    public void UpgradeAttack()
    {

        if (currUpgradeAttack < playerShipData.maxUpgradeAttack)
        {
            bool affordable = CanAfford(playerShipData.upgradeCostAttack);
            if (!affordable)
            {
                return;
            }
            else
            {
                player.AttackPower += playerShipData.amountUpgradedAttack;
                currUpgradeAttack++;
                UpdateAttackText();
                UpdateCoins();
            }
        }
    }

    void UpdateAttackText()
    {
        UpdateUpgradeButtonText(GetTextComponent(upgradeAttackButton), "Attack +", playerShipData.amountUpgradedAttack, currUpgradeAttack, playerShipData.maxUpgradeAttack, playerShipData.upgradeCostAttack);
        UpdateStatText(GetTextComponent(statAttackText), "Attack: ", player.AttackPower);
    }

    public void UpgradeReloadTime()
    {

        if (currUpgradeReload < playerShipData.maxUpgradeReload)
        {
            bool affordable = CanAfford(playerShipData.upgradeCostReload);
            if (!affordable)
            {
                return;
            }
            else
            {
                player.ReloadTime += playerShipData.amountUpgradedReload;
                currUpgradeReload++;
                UpdateReloadText();
                UpdateCoins();
            }
        }
    }

    void UpdateReloadText()
    {
        UpdateUpgradeButtonText(GetTextComponent(upgradeReloadButton), "Reload Time ", playerShipData.amountUpgradedReload, currUpgradeReload, playerShipData.maxUpgradeReload, playerShipData.upgradeCostReload);
        UpdateStatText(GetTextComponent(statReloadText), "Reload Time: ", player.ReloadTime);
    }

    public void UpgradeShotStrength()
    {

        if (currUpgradeSS < playerShipData.maxUpgradeSS)
        {
            bool affordable = CanAfford(playerShipData.upgradeCostStrength);
            if (!affordable)
            {
                return;
            }
            else
            {
                player.ShotStrength += playerShipData.amountUpgradedSS;
                currUpgradeSS++;
                UpdateSSText();
                UpdateCoins();
            }
        }
    }

    void UpdateSSText()
    {
        UpdateUpgradeButtonText(GetTextComponent(upgradeSSButton), "Shot Strength +", playerShipData.amountUpgradedSS, currUpgradeSS, playerShipData.maxUpgradeSS, playerShipData.upgradeCostStrength);
        UpdateStatText(GetTextComponent(statSSText), "Shot Strength: ", player.ShotStrength);
    }

    public void UpgradeShip()
    {
        bool affordable = CanAfford(playerShipData.lvlUpCost);
        if (!affordable)
        {
            return;
        }
        else
        {
            ResetUpgrades();
            if (cameraController.PlayerAnchored) cameraController.LockCameraOnPlayer();
            UnitData newData;

            if (playerShipData.shipLvl == "1")
            {
                newData = (UnitData)Resources.Load("Ship_Lvl2");
                BuildNewShip(newData, ship_lvl2);
            }
            else if (playerShipData.shipLvl == "2")
            {
                newData = (UnitData)Resources.Load("Ship_Lvl3");
                BuildNewShip(newData, ship_lvlMAX);
                lvlUpButton.transform.parent.gameObject.GetComponent<Button>().interactable = false;
            }
            cameraController.player = player.gameObject;
            cameraController.LockCameraOnPlayer();
            UpdateAllTextBoxes();
        }
    }
    
    void BuildNewShip(UnitData newData, GameObject shipPrefab)
    {
        GameObject newShip = Instantiate(shipPrefab, player.transform.position, player.transform.rotation);
        newShip.AddComponent<PlayerController>();
        PlayerController temp = newShip.GetComponent<PlayerController>();
        PlayerController currControllerDataTransfer = player;
        temp.unitData = newData;
        Destroy(player.gameObject);
        player = temp;
        player.pauseMenu = currControllerDataTransfer.pauseMenu;
        player.reloadTextObject = currControllerDataTransfer.reloadTextObject;
        player.healthBar = currControllerDataTransfer.healthBar;
        playerShipData = newData;
        player.Restart();
        player.gameObject.name = "Player";
        player.gameObject.tag = "Player";
        player.CoinsPossessed = currControllerDataTransfer.CoinsPossessed;
    }

    void UpdateLvlText()
    {
        GetTextComponent(lvlUpButton).text = "LEVEL UP\ncost:" + playerShipData.lvlUpCost;
        UpdateStatText(GetTextComponent(statLvlText), "Ship Lvl: " + playerShipData.shipLvl);
    }

    void ResetUpgrades()
    {
        currUpgradeMS = 0;
        currUpgradeHP = 0;
        currUpgradeAttack = 0;
        currUpgradeReload = 0;
        currUpgradeSS = 0;
    }
}

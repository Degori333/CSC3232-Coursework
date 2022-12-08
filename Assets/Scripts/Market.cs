using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Market : MonoBehaviour
{
    private MarketManager marketManager;
    [SerializeField] private GameObject tradeButton;
    [SerializeField] private TextMeshProUGUI nameDisplay;

    [SerializeField] private int minPrice;
    [SerializeField] private int maxPrice;
    [SerializeField] private int basePrice;
    [SerializeField] private int currPrice;

    public int CurrPrice
    {
        get
        {
            return currPrice;
        }
    }
    private int previousPrice;
    public int PreviousPrice
    {
        get
        {
            return previousPrice;
        }
    }

    [SerializeField] private float resellMultiplier;
    [SerializeField] private float resellMaxDelta = 0.1f;
    public float ResellMultiplier
    {
        get
        {
            return resellMultiplier;
        }
        private set
        {
            resellMultiplier = Mathf.Round(Mathf.Clamp(value, 0.1f, 1f) * 10) / 10;

        }
    }

    [SerializeField] private int maxWarehouseSpace;
    public int MaxWarehouseSpace
    {
        get
        {
            return maxWarehouseSpace;
        }
    }
    [SerializeField] private int takenWarehouseSpace;
    public int TakenWarehouseSpace
    {
        get
        {
            return takenWarehouseSpace;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        marketManager = GameObject.Find("MarketManager").GetComponent<MarketManager>();
        nameDisplay = transform.Find("Stall/Top/Canvas/Name").GetComponent<TextMeshProUGUI>();
        nameDisplay.text = name;
        currPrice = basePrice;
        previousPrice = currPrice;
    }

    public void PriceChange()
    {
        float pricePercentage = currPrice / (maxPrice - minPrice);
        //lower price
        previousPrice = currPrice;
        if (Random.value < pricePercentage)
        {
            currPrice = Random.Range(minPrice, currPrice+1);
        }
        //increase price
        else
        {
            currPrice = Random.Range(currPrice, maxPrice+1);
        }
        ResellMultiplier = Random.Range(ResellMultiplier - resellMaxDelta, ResellMultiplier + resellMaxDelta);
    }

    public void BuyCargo(UnitFunctionality shipInfo, int amount)
    {
        if(currPrice * amount <= shipInfo.CoinsPossessed)
        {
            // if curr Cargo plus the amount you want to buy is larger than max capacity then buy to full
            if(shipInfo.CurrCargo + amount > shipInfo.MaxCargo)
            {
                amount = shipInfo.MaxCargo - shipInfo.CurrCargo;
            }
            // if amount you want to buy is larger than cargo in the warehouse
            if (amount > takenWarehouseSpace)
            {
                amount = takenWarehouseSpace;
            }
            shipInfo.CurrCargo += amount;
            shipInfo.CoinsPossessed -= currPrice * amount;
            takenWarehouseSpace -= amount;
        }
        else
        {
            // how much you can afford
            amount = shipInfo.CoinsPossessed / currPrice;
            // if curr Cargo plus the amount you can buy is larger than max capacity then buy to full
            if (shipInfo.CurrCargo + amount > shipInfo.MaxCargo)
            {
                amount = shipInfo.MaxCargo - shipInfo.CurrCargo;
            }
            // if amount you want to buy is larger than cargo in the warehouse
            if (amount > takenWarehouseSpace)
            {
                amount = takenWarehouseSpace;
            }

            shipInfo.CurrCargo += amount;
            shipInfo.CoinsPossessed -= currPrice * amount;
            takenWarehouseSpace -= amount;
        }
    }

    public void SellCargo(UnitFunctionality shipInfo, int amount)
    {
        if (amount > shipInfo.CurrCargo)
        {
            amount = shipInfo.CurrCargo;
        }
        // if amount you want to sell is larger than warehouse leftover space
        if (amount > maxWarehouseSpace - takenWarehouseSpace)
        {
            amount = maxWarehouseSpace - takenWarehouseSpace;
        }
        shipInfo.CurrCargo -= amount;
        shipInfo.CoinsPossessed += Mathf.RoundToInt(currPrice * resellMultiplier) * amount;
        takenWarehouseSpace += amount;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<UnitFunctionality>().visitingMarket = GetComponent<Market>();
            if (collision.gameObject.CompareTag("Player"))
            {
                tradeButton.SetActive(true);
            }
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<UnitFunctionality>().visitingMarket = null;
            if (collision.gameObject.CompareTag("Player"))
            {
                tradeButton.SetActive(false);
            }
        }
    }
}

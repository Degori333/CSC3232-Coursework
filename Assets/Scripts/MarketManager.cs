using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MarketManager : MonoBehaviour
{

    Market[] markets;
    [SerializeField] UnitFunctionality player;
    [SerializeField] float marketRefreshRate;
    public float MarketRefreshRate
    {
        get
        {
            return marketRefreshRate;
        }
    }
    [SerializeField] TextMeshProUGUI marketUpdateAlert;
    [SerializeField] TextMeshProUGUI otherMarketsInfo;
    [SerializeField] TextMeshProUGUI[] tradeButtons;
    [SerializeField] TextMeshProUGUI status;
    [SerializeField] TextMeshProUGUI marketName;

    // Start is called before the first frame update
    void Start()
    {
        markets = FindObjectsOfType<Market>();
        InvokeRepeating(nameof(ChangeAllMarketPrices), MarketRefreshRate, MarketRefreshRate);
    }

    void ChangeAllMarketPrices()
    {
        foreach (Market market in markets)
        {
            market.PriceChange();
        }
        StartCoroutine(MarketUpdateFade());
    }

    IEnumerator MarketUpdateFade()
    {
        marketUpdateAlert.gameObject.SetActive(true);
        float elapsedTime = 0;
        float waitTime = 2f;
        while (elapsedTime < waitTime)
        {
            marketUpdateAlert.alpha = Mathf.Lerp(0, 1, elapsedTime / waitTime);
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        yield return new WaitForSeconds(waitTime);
        elapsedTime = 0;
        while (elapsedTime < waitTime)
        {
            marketUpdateAlert.alpha = Mathf.Lerp(1, 0, elapsedTime / waitTime);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        marketUpdateAlert.gameObject.SetActive(false);
        yield return null;
    }

    public void BuyXCargo(int amount)
    {
        player.visitingMarket.GetComponent<Market>().BuyCargo(player, amount);
    }
    public void BuyMaxCargo()
    {
        player.visitingMarket.GetComponent<Market>().BuyCargo(player, player.MaxCargo - player.CurrCargo);
    }
    public void SellXCargo(int amount)
    {
        player.visitingMarket.GetComponent<Market>().SellCargo(player, amount);
    }
    public void SellMaxCargo()
    {
        player.visitingMarket.GetComponent<Market>().SellCargo(player, player.MaxCargo - player.CurrCargo);
    }

    public void ResetPlayerStatusUIText()
    {
        Market market = player.visitingMarket.GetComponent<Market>();
        status.text = "Coins: " + player.CoinsPossessed + "\nWarehouse: " + market.TakenWarehouseSpace + "/" + market.MaxWarehouseSpace + "\nYour Cargo: " + player.CurrCargo + "/" + player.MaxCargo;
    }

    void AddUIText(Market market)
    {

        otherMarketsInfo.text += market.gameObject.name + ": ";
        if (market.CurrPrice > market.PreviousPrice)
        {
            otherMarketsInfo.text += "<color=green>" + market.CurrPrice + "c</color=green>/<color=green>" + Mathf.RoundToInt(market.CurrPrice * market.ResellMultiplier) + "c </color=green>";
        }
        else if (market.CurrPrice < market.PreviousPrice)
        {
            otherMarketsInfo.text += "<color=red>" + market.CurrPrice + "c</color=red>/<color=red>" + Mathf.RoundToInt(market.CurrPrice * market.ResellMultiplier) + "c </color=red>";
        }
        else
        {
            otherMarketsInfo.text += market.CurrPrice + "c/" + Mathf.RoundToInt(market.CurrPrice * market.ResellMultiplier) + "c ";
        }
        otherMarketsInfo.text += market.TakenWarehouseSpace + "/" + market.MaxWarehouseSpace + "\n\n";
    }
    public void UpdateUIText()
    {
        marketName.text = player.visitingMarket.name;
        otherMarketsInfo.text = "Name - Buy/Sell - Capacity\n\n";
        ResetPlayerStatusUIText();
        foreach (Market market in markets)
        {
            AddUIText(market);
        }
    }

    public void SetAllTradeButtons()
    {
        Market market = player.visitingMarket.GetComponent<Market>();
        tradeButtons[0].text = SetButtonTradeText(market, true, 1);
        tradeButtons[1].text = SetButtonTradeText(market, true, 5);
        tradeButtons[2].text = SetButtonTradeText(market, true, (player.MaxCargo - player.CurrCargo));
        tradeButtons[3].text = SetButtonTradeText(market, false, 1);
        tradeButtons[4].text = SetButtonTradeText(market, false, 5);
        tradeButtons[5].text = SetButtonTradeText(market, false, (player.MaxCargo - player.CurrCargo));
    }

    string SetButtonTradeText(Market market ,bool buying, int amount)
    {
        if (buying)
        {
            return "Buy " + amount + "\n" + market.CurrPrice * amount + "c";
        }
        else
        {
            return "Sell " + amount + "\n"  + Mathf.RoundToInt(market.CurrPrice * market.ResellMultiplier) * amount + "c ";
        }
    }
}

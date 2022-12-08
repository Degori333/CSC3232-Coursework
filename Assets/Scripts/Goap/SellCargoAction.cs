using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellCargoAction : GoapAction
{
	private bool sold = false;

	private float startTime = 0;
	public float workDuration = 5; // seconds

	public SellCargoAction()
	{
		AddPrecondition("hasCargo", true); // we only want to sell cargo when we have it
		RemoveEffect("hasCargo");
		AddEffect("hasCargo", false);
		RemoveEffect("hasSpace");
		AddEffect("hasSpace", true);
		AddEffect("earnMoney", true);
	}


	public override void NewReset()
	{
		sold = false;
		startTime = 0;
	}

	public override bool IsDone()
	{
		return sold;
	}

	public override bool RequiresInRange()
	{
		return true; // yes we need to be near a market
	}

	public override bool CheckProceduralPrecondition(GameObject agent)
	{
		// find the market to sell cargo at 
		Market[] markets = (Market[])FindObjectsOfType(typeof(Market));
		Market optimal = null;
		BuyCargoAction whereToNotSell = GetComponent<BuyCargoAction>();
		whereToNotSell.CheckProceduralPrecondition(agent);
		TraderController trader = agent.GetComponent<TraderController>();
		float price;
		float dist;
		float timeLeft = trader.marketManager.MarketRefreshRate - trader.internalTimer;
		float overallCost = 0;

		foreach (Market market in markets)
		{
			// don't sell in the same market you are right now
			if (trader.buyingAt == market)
            {
				continue;
            }
			if (optimal == null)
			{
				// first one, so choose it for now
				optimal = market;
				dist = (market.gameObject.transform.position - agent.transform.position).magnitude;
				price = market.CurrPrice * market.ResellMultiplier;
				if (timeLeft * trader.MoveSpeed * 10 < dist)
				{
					overallCost = Mathf.NegativeInfinity;
				}
				else
				{
					overallCost = price * (market.MaxWarehouseSpace - market.TakenWarehouseSpace);
				}

			}
			else
			{
				dist = (market.gameObject.transform.position - agent.transform.position).magnitude;
				price = market.CurrPrice * market.ResellMultiplier;
				// get new cost
				float newCost = 0;
				if (timeLeft * trader.MoveSpeed * 10 < dist)
				{
					newCost = Mathf.NegativeInfinity;
				}
				else
				{
					newCost = price * (market.MaxWarehouseSpace - market.TakenWarehouseSpace);

				}
				// is the new cost smaller than the current?
				if (newCost > overallCost)
				{
					// we found a better one, use it
					optimal = market;
					overallCost = newCost;
				}
			}
			
		}
		trader.sellingAt = optimal;

		if (optimal == null)
			return false;
		target = optimal.gameObject;
		return optimal != null;
	}

	public override bool Perform(GameObject agent)
	{
		if (startTime == 0)
			startTime = Time.time;

		if (Time.time - startTime > workDuration)
		{
			// finished selling
			TraderController trader = agent.GetComponent<TraderController>();
			trader.visitingMarket.GetComponent<Market>().SellCargo(trader, trader.MaxCargo);
			sold = true;
		}
		return true;
	}
}

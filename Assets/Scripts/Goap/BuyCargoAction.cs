using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyCargoAction : GoapAction
{
	private bool bought = false;

	private float startTime = 0;
	public float workDuration = 5; // seconds

	public BuyCargoAction()
	{
		AddPrecondition("hasSpace", true); // we need space to buy cargo
		AddPrecondition("hasCargo", false); // we only want to buy cargo when we don't have it
		AddEffect("hasCargo", true);
	}


	public override void NewReset()
	{
		bought = false;
		startTime = 0;
	}

	public override bool IsDone()
	{
		return bought;
	}

	public override bool RequiresInRange()
	{
		return true; // yes we need to be near a market
	}

	public override bool CheckProceduralPrecondition(GameObject agent)
	{
		// find the market to buy cargo at 
		Market[] markets = (Market[]) FindObjectsOfType(typeof(Market));
		Market cheapest = null;
		TraderController trader = agent.GetComponent<TraderController>();
		float price;
		float dist;
		float timeLeft = trader.marketManager.MarketRefreshRate - trader.internalTimer;
		float overallCost = 0;

		foreach (Market market in markets)
		{
			if (trader.sellingAt == market)
            {
				continue;
            }
			if (cheapest == null)
			{
				// first one, so choose it for now
				cheapest = market;
				dist = (market.gameObject.transform.position - agent.transform.position).magnitude;
				price = market.CurrPrice;
				if (timeLeft * trader.MoveSpeed * 10 < dist)
                {
					overallCost = Mathf.Infinity;
                }
                else
                {
					overallCost = dist * price / market.TakenWarehouseSpace;
                }

			}
			else
			{
				dist = (market.gameObject.transform.position - agent.transform.position).magnitude;
				price = market.CurrPrice;
				// get new cost
				float newCost = 0;
				if (timeLeft * trader.MoveSpeed * 10 < dist)
				{
					newCost = Mathf.Infinity;
				}
				else
				{
					newCost = dist * price / market.TakenWarehouseSpace;

				}
				// is the new cost smaller than the current?
				if (newCost < overallCost)
				{
					// we found a better one, use it
					cheapest = market;
					overallCost = newCost;
				}
			}
		}
		trader.buyingAt = cheapest;

		if (cheapest == null)
			return false;

		target = cheapest.gameObject;

		return cheapest != null;
	}

	public override bool Perform(GameObject agent)
	{
		if (startTime == 0)
			startTime = Time.time;

		if (Time.time - startTime > workDuration)
		{
			// finished buying
			UnitFunctionality trader = agent.GetComponent<UnitFunctionality>();
			trader.visitingMarket.GetComponent<Market>().BuyCargo(trader, trader.MaxCargo);
			bought = true;
			if ( trader.CurrCargo >= trader.MaxCargo)
            {
				RemoveEffect("hasSpace");
				AddEffect("hasSpace", false);
            }
		}
		return true;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyIsland : GoapAction
{
	private bool won = false;

	private float startTime = 0;
	public float workDuration = 15; // seconds

	public BuyIsland()
	{
		AddPrecondition("hasEnoughMoney", true); // we only want to buy the island when we can afford it
		AddEffect("winGame", true);
	}


	public override void NewReset()
	{
		won = false;
		startTime = 0;
	}

	public override bool IsDone()
	{
		return won;
	}

	public override bool RequiresInRange()
	{
		return true; // yes we need to be near a specific Island
	}

	public override bool CheckProceduralPrecondition(GameObject agent)
	{
		// find the island
		GameObject winIsland = GameObject.FindGameObjectWithTag("Island");

		if (winIsland == null)
			return false;

		target = winIsland;

		return winIsland != null;
	}

	public override bool Perform(GameObject agent)
	{
		if (startTime == 0)
			startTime = Time.time;

		if (Time.time - startTime > workDuration)
		{
			// finished wining
			Win winIsland = GameObject.FindGameObjectWithTag("Island").GetComponent<Win>();
			winIsland.Buy(agent.GetComponent<UnitFunctionality>());
			won = true;
		}
		return true;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TraderController : UnitFunctionality , IGoap
{
	public HashSet<KeyValuePair<string, object>> GetWorldState()
	{
		HashSet<KeyValuePair<string, object>> worldData = new HashSet<KeyValuePair<string, object>>();

		worldData.Add(new KeyValuePair<string, object>("hasSpace", CurrCargo < MaxCargo));
		worldData.Add(new KeyValuePair<string, object>("hasEnoughMoney", (CoinsPossessed >= 10000)));
		worldData.Add(new KeyValuePair<string, object>("hasCargo", CurrCargo > 0));


		return worldData;
	}

	public HashSet<KeyValuePair<string, object>> CreateGoalState()
    {
		HashSet<KeyValuePair<string, object>> goal = new HashSet<KeyValuePair<string, object>>();

		if (CoinsPossessed >= 10000)
		{
			goal.Add(new KeyValuePair<string, object>("winGame", true));

		}
		else
		{
			goal.Add(new KeyValuePair<string, object>("earnMoney", true));
		}
		return goal;
	}


	public void PlanFailed(HashSet<KeyValuePair<string, object>> failedGoal)
	{
		// Not handling this here since we are making sure our goals will always succeed.
		// But normally you want to make sure the world state has changed before running
		// the same goal again, or else it will just fail.
	}

	public void PlanFound(HashSet<KeyValuePair<string, object>> goal, Queue<GoapAction> actions)
	{
		// Yay we found a plan for our goal
		Debug.Log("<color=green>Plan found</color> " + PrettyPrint(actions));
	}

	public void ActionsFinished()
	{
		// Everything is done, we completed our actions for this gool. Hooray!
		
		Debug.Log("<color=blue>Actions completed</color>");
	}

	public void PlanAborted(GoapAction aborter)
	{
		// An action bailed out of the plan. State has been reset to plan again.
		// Take note of what happened and make sure if you run the same goal again
		// that it can succeed.
		Debug.Log("<color=red>Plan Aborted</color> " + PrettyPrint(aborter));
	}

	GameObject collisionTarget;
	bool enteredRightCollider = false;
	public Market sellingAt = null;
	public Market buyingAt = null;


	public bool MoveAgent(GoapAction nextAction)
	{
		// move towards the NextAction's target
		MoveUnit(nextAction.target.transform.position);
		collisionTarget = nextAction.target;
		if (enteredRightCollider)
		{
			// we are at the target location, we are done
			nextAction.InRange = true;
			enteredRightCollider = false;
			collisionTarget = null;
			agent.ResetPath();
			return true;
		}
		else
			return false;
	}

    private void OnCollisionEnter(Collision collision)
    {
		if(collision.gameObject == collisionTarget)
        {
			enteredRightCollider = true;
			visitingMarket = collisionTarget.GetComponent<Market>();
		}
    }

    // Agent State Scripts
    public enum State
	{
		Idle,
		MoveTo,
		Action
	}

	public State currState = State.Idle;

	private Stack<string> stateStack = new Stack<string>();

	public string PeekState()
	{
		if(stateStack != null) return stateStack.Peek();
		return State.Idle.ToString();
	}

	public void PushState(State state)
	{
		stateStack.Push(state.ToString());
	}

	public string PopState()
	{
		return stateStack.Pop();
	}

	private HashSet<GoapAction> availableActions;
	private Queue<GoapAction> currentActions;

	private IGoap dataProvider; // this is the implementing class that provides our world data and listens to feedback on planning

	private GoapPlanner planner;

	public float internalTimer = 0;

	public MarketManager marketManager;


	override protected void Start()
	{
		base.Start();
		tag = "Trader";

		availableActions = new HashSet<GoapAction>();
		currentActions = new Queue<GoapAction>();
		LoadActions();
		FindDataProvider();
		planner = new GoapPlanner();

		PushState(State.Idle);
		CreateIdleState();
		//PushState(State.Idle);

	}


	void Update()
	{
		internalTimer += Time.deltaTime;
		if(internalTimer > marketManager.MarketRefreshRate)
        {
			internalTimer -= marketManager.MarketRefreshRate;
        }
		currState = (State)Enum.Parse(typeof(State), PeekState());
		if (currState == State.Idle)
        {
			CreateIdleState();
        }
		else if (currState == State.MoveTo)
		{
			CreateMoveToState();
		}
		else if (currState == State.Action)
		{
			CreateActionState();
		}
	}


	public void AddAction(GoapAction a)
	{
		availableActions.Add(a);
	}

	public GoapAction GetAction(Type action)
	{
		foreach (GoapAction g in availableActions)
		{
			if (g.GetType().Equals(action))
				return g;
		}
		return null;
	}

	public void RemoveAction(GoapAction action)
	{
		availableActions.Remove(action);
	}

	private bool HasActionPlan()
	{
		return currentActions.Count > 0;
	}

	// States to create on the stack
	//
	private void CreateIdleState()
	{
		// GOAP planning

		// get the world state and the goal we want to plan for
		HashSet<KeyValuePair<string, object>> worldState = dataProvider.GetWorldState();
		HashSet<KeyValuePair<string, object>> goal = dataProvider.CreateGoalState();

		// Plan
		Queue<GoapAction> plan = planner.Plan(gameObject, availableActions, worldState, goal);
		if (plan != null)
		{
			// we have a plan, hooray!
			currentActions = plan;
			dataProvider.PlanFound(goal, plan);

			PopState(); // move to PerformAction state
			PushState(State.Action);

		}
		else
		{
			// ugh, we couldn't get a plan
			Debug.Log("<color=orange>Failed Plan:</color>" + PrettyPrint(goal));
			dataProvider.PlanFailed(goal);
			PopState(); // move back to IdleAction state
			PushState(State.Idle);
		}

	}

	private void CreateMoveToState()
	{
		
		// move the game object

		GoapAction action = currentActions.Peek();
		if (action.RequiresInRange() && action.target == null)
		{
			Debug.Log("<color=red>Fatal error:</color> Action requires a target but has none. Planning failed. You did not assign the target in your Action.checkProceduralPrecondition()");
			PopState(); // move
			PopState(); // perform
			PushState(State.Idle);
			return;
		}

		// get the agent to move itself
		if (dataProvider.MoveAgent(action))
		{
			PopState();
		}
		
	}

	private void CreateActionState()
	{
		// perform the action

		if (!HasActionPlan())
		{
			// no actions to perform
			Debug.Log("<color=red>Done actions</color>");
			PopState();
			PushState(State.Idle);
			dataProvider.ActionsFinished();
			return;
		}

		GoapAction action = currentActions.Peek();

		if (action.IsDone())
		{
			// the action is done. Remove it so we can perform the next one
			currentActions.Dequeue();
		}

		if (HasActionPlan())
		{
			// perform the next action
			action = currentActions.Peek();
			bool inRange = !action.RequiresInRange() || action.InRange;

			if (inRange)
			{
				// we are in range, so perform the action
				bool success = action.Perform(gameObject);

				if (!success)
				{
					// action failed, we need to plan again
					PopState();
					PushState(State.Idle);
					dataProvider.PlanAborted(action);
				}
			}
			else
			{
				// we need to move there first
				// push moveTo state
				PushState(State.MoveTo);
			}

		}
		else
		{
			// no actions left, move to Plan state
			PopState();
			PushState(State.Idle);
			dataProvider.ActionsFinished();
		}
	}

	private void FindDataProvider()
	{
		foreach (Component comp in gameObject.GetComponents(typeof(Component)))
		{
			if (typeof(IGoap).IsAssignableFrom(comp.GetType()))
			{
				dataProvider = (IGoap)comp;
				return;
			}
		}
	}

	private void LoadActions()
	{
		GoapAction[] actions = gameObject.GetComponents<GoapAction>();
		foreach (GoapAction a in actions)
		{
			availableActions.Add(a);
		}
	}

	public static string PrettyPrint(HashSet<KeyValuePair<string, object>> state)
	{
		string s = "";
		foreach (KeyValuePair<string, object> kvp in state)
		{
			s += kvp.Key + ":" + kvp.Value.ToString();
			s += ", ";
		}
		return s;
	}

	public static string PrettyPrint(Queue<GoapAction> actions)
	{
		string s = "";
		foreach (GoapAction a in actions)
		{
			s += a.GetType().Name;
			s += "-> ";
		}
		s += "GOAL";
		return s;
	}

	public static string PrettyPrint(GoapAction[] actions)
	{
		string s = "";
		foreach (GoapAction a in actions)
		{
			s += a.GetType().Name;
			s += ", ";
		}
		return s;
	}

	public static string PrettyPrint(GoapAction action)
	{
		return "" + action.GetType().Name;
	}
}

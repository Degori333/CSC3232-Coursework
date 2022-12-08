using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Transform),typeof(NavMeshAgent),typeof(Rigidbody))]
public abstract class UnitFunctionality : MonoBehaviour
{
    public UnitData unitData;

    [SerializeField] protected float moveSpeed;
    public float MoveSpeed
    {
        get
        {
            return moveSpeed;
        }
        set
        {
            if (value <= 0) 
            {
                moveSpeed = 0;
            }
            else
            {
                moveSpeed = value;
            }
            turningSpeed = moveSpeed * 0.6f;
            rotationSpeed = moveSpeed * 0.7f;
            agent.acceleration = moveSpeed * 0.7f;
            agent.speed = moveSpeed;
            agent.angularSpeed = turningSpeed;
        }
    }
    [SerializeField] protected float turningSpeed;
    [SerializeField] protected float rotationSpeed;

    public bool isDead = false;
    
    [SerializeField] protected float healthPoints;
    [SerializeField] protected bool invincible = false;
    virtual public float HealthPoints
    {
        get
        {
            return healthPoints;
        }
        set
        {
            if (invincible)
            {
                return;
            }
            if(value <= 0)
            {
                healthPoints = 0;
            }
            else
            {
                healthPoints = value;
            }
        }
    }
    [SerializeField] protected float attackPower;
    public float AttackPower
    {
        get
        {
            return attackPower;
        }
        set
        {
            attackPower = value;
        }
    }

    [SerializeField] protected float reloadTime;
    public float ReloadTime
    {
        get
        {
            return reloadTime;
        }
        set
        {
            if (value <= 0.5f)
            {
                reloadTime = 0.5f;
            }
            else
            {
                reloadTime = value;
            }
        }
    }
    [SerializeField] protected bool reloading = false;

    protected Rigidbody rb;
    protected Transform destination;
    protected NavMeshAgent agent;

    [SerializeField] protected GameObject cannonsLeftSide;
    [SerializeField] protected GameObject cannonsRightSide;
    [SerializeField] protected GameObject cannonBall;
    [SerializeField] protected float shotStrength;
    public float ShotStrength
    {
        get
        {
            return shotStrength;
        }
        set
        {
            if (value <= 0)
            {
                shotStrength = 0;
            }
            else
            {
                shotStrength = value;
            }
        }
    }

    [SerializeField] protected float shotOffset = 0.1f;
    [SerializeField] protected float strengthOffset = 20f;

    [SerializeField] protected int worthCoins = 0;
    public int WorthCoins
    {
        get
        {
            return worthCoins;
        }
    }

    [SerializeField] protected int coinsPossessed = 0;
    virtual public int CoinsPossessed
    {
        get
        {
            return coinsPossessed;
        }
        set
        {
            if (value <= 0)
            {
                coinsPossessed = 0;
            }
            else
            {
                coinsPossessed = value;
            }
        }
    }

    [SerializeField] protected int maxCargo;
    public int MaxCargo
    {
        get
        {
            return maxCargo;
        }
    }
    [SerializeField] protected int currCargo = 0;
    virtual public int CurrCargo
    {
        get
        {
            return currCargo;
        }
        set
        {
            if (value >= maxCargo)
            {
                currCargo = maxCargo;
            }
            else if (value <= 0)
            {
                currCargo = 0;
            }
            else currCargo = value;
        }
    }

    public Market visitingMarket;

    virtual protected void Start()
    {
        if(unitData == null)
        {
            return;
        }
        cannonBall = (GameObject)Resources.Load("CannonBall");
        Transform shipBody = gameObject.transform.Find("Body");
        cannonsLeftSide = shipBody.Find("LeftSideCannons").gameObject;
        cannonsRightSide = shipBody.Find("RightSideCannons").gameObject;
        agent = GetComponent<NavMeshAgent>();

        moveSpeed = unitData.moveSpeed;
        turningSpeed = moveSpeed * 0.6f;
        rotationSpeed = moveSpeed * 0.7f;
        agent.acceleration = moveSpeed * 0.7f;
        agent.speed = moveSpeed;
        agent.angularSpeed = turningSpeed;
        healthPoints = unitData.healthPoints;
        attackPower = unitData.attackPower;
        reloadTime = unitData.reloadTime;
        shotStrength = unitData.shotStrength;
        shotOffset = unitData.shotOffset;
        worthCoins = unitData.worthCoins;
        maxCargo = unitData.maxCargo;

        rb = GetComponent<Rigidbody>();
    }

    virtual public void Restart()
    {
        Start();
    }

    public void MoveUnit(Vector3 destination)
    {
        Quaternion newRotation = Quaternion.LookRotation(destination - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
        agent.destination = destination;
    }

    public void Shoot(GameObject cannons)
    {
        if (!reloading)
        {
            GameObject[] cannonArray = new GameObject[cannons.transform.childCount];

            for (int i = 0; i < cannons.transform.childCount; i++)
            {
                cannonArray[i] = cannons.transform.GetChild(i).gameObject;
            }

            foreach (GameObject cannon in cannonArray)
            {
                
                GameObject cannonBallShot = Instantiate(cannonBall, 
                                                        cannon.transform.GetChild(0).position + new Vector3(Random.Range(0, shotOffset),Random.Range(0, shotOffset),Random.Range(0, shotOffset)),
                                                        cannon.transform.rotation);
                cannonBallShot.GetComponent<Rigidbody>().AddRelativeForce(Vector3.right * Random.Range(shotStrength - strengthOffset, shotStrength), ForceMode.Impulse);
                cannonBallShot.GetComponent<CannonBallBehavior>().parentShip = gameObject;
            }
            reloading = true;
            StartCoroutine(Reload());
        }
    }

    IEnumerator Reload()
    {
        yield return new WaitForSeconds(reloadTime);
        reloading = false;
        yield return reloading;
    }
}

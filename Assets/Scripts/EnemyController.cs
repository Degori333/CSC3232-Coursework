using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : UnitFunctionality
{
    public enum State
    {
        Idle,
        Move,
        Attack
    }

    public State currState = State.Idle;
    public enum MovementState
    {
        Chase,
        Reposition,
        Encircle
    }
    public MovementState currMoveState = MovementState.Chase;

    public enum AttackSide
    {
        Left,
        Right
    }

    public AttackSide attackSide = AttackSide.Left;

    public bool inRange;

    private GameObject player;
    private PlayerController playerController;
    [SerializeField] private float shootingRange;
    [SerializeField] private float dangerDistance = 2f;
    [SerializeField] GameObject destPoint;
    [SerializeField] private float dangerOffset;
    [SerializeField] private float encirclmentSpeed;
    [SerializeField] private float angle = 0;

    private void Awake()
    {
        destPoint = new GameObject("Enemy Destination");
    }

    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();
        player = GameObject.Find("Player");
        playerController = player.GetComponent<PlayerController>();
        name = "Enemy";
        tag = "Enemy";
        shootingRange = (ShotStrength / 10) + 2;
        dangerOffset = shootingRange / 2;
        encirclmentSpeed = 4 * moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        DecideWhatToDo();
        if(currState == State.Move)
        {
            DecideOnMove();
        }
        else if (currState  == State.Attack)
        {
            if(attackSide == AttackSide.Left)
            {
                Shoot(cannonsLeftSide);
            }
            else
            {
                Shoot(cannonsRightSide);
            }
        }

    }

    private void OnDestroy()
    {
        Destroy(destPoint);
    }

    float CheckDistance()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer > shootingRange)
        {
            currMoveState = MovementState.Chase;
            inRange = false;
        }
        else
        {
            inRange = true;
            if (distanceToPlayer > dangerDistance && distanceToPlayer <= dangerDistance + dangerOffset)
            {
                currMoveState = MovementState.Encircle;
            }
            else 
            {
                currMoveState = MovementState.Reposition;
            }
        }

        
        return distanceToPlayer;
    }

    void DecideWhatToDo()
    {
        if (isDead || playerController.isDead)
        {
            currState = State.Idle;
        }
        else
        {
            currState = State.Move;

            if (!reloading)
            {       
                if(Physics.Raycast(transform.position, -transform.right, out RaycastHit hitLeft, shootingRange))
                {
                    if (hitLeft.transform.gameObject.CompareTag("Player"))
                    {
                        currState = State.Attack;
                        attackSide = AttackSide.Left;
                    }                  
                }
                else if (Physics.Raycast(transform.position, transform.right, out RaycastHit hitRight, shootingRange))
                {
                    if (hitRight.transform.gameObject.CompareTag("Player"))
                    {
                        currState = State.Attack;
                        attackSide = AttackSide.Right;
                    }
                }
            }
        }
    }


    void DecideOnMove()
    {
        float distance = CheckDistance();
        if(currMoveState == MovementState.Chase)
        {
            Chase(distance);
        }
        else if(currMoveState == MovementState.Reposition)
        {
            Reposition(distance);
        }
        else if(currMoveState == MovementState.Encircle)
        {
            Encricle();
        }
    }

    void Chase(float distanceToPlayer)
    {
        MoveUnit(player.transform.position);
    }

    void Reposition(float distanceToPlayer)
    {
        destPoint.transform.position = new Vector3((transform.position.x - player.transform.position.x) * (dangerDistance + dangerOffset) / distanceToPlayer,
                                                    player.transform.position.y,
                                                    (transform.position.z - player.transform.position.z) * (dangerDistance + dangerOffset) / distanceToPlayer);
        MoveUnit(destPoint.transform.position);

       
        angle = -player.transform.rotation.y + Vector3.Angle(player.transform.position, transform.position);
        CheckForCloserCannons();
    }

    void Encricle()
    {
        angle += Time.deltaTime * encirclmentSpeed;
        angle %= 360;
        float x = Mathf.Cos(Mathf.Deg2Rad * angle) * (dangerDistance + dangerOffset);
        float z = Mathf.Sin(Mathf.Deg2Rad * angle ) * (dangerDistance + dangerOffset);

        destPoint.transform.position = new Vector3( x, 0, z) + player.transform.position;
        
        MoveUnit(destPoint.transform.position);
        
    }

    void CheckForCloserCannons()
    {

        if(Vector3.Distance(cannonsRightSide.transform.position, player.transform.position) >= Vector3.Distance(cannonsLeftSide.transform.position, player.transform.position))
        {
            encirclmentSpeed = Mathf.Abs(encirclmentSpeed);
        }
        else
        {
            encirclmentSpeed = -Mathf.Abs(encirclmentSpeed);
        }
    }

}

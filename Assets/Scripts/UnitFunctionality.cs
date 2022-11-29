using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Transform),typeof(NavMeshAgent),typeof(Rigidbody))]
public class UnitFunctionality : MonoBehaviour
{
    public float moveSpeed;
    public float turningSpeed;
    public float rotationSpeed;

    public int healthPoints;
    public int attackPower;

    public float reloadTime;
    public bool leftSideRdy;
    public bool rightSideRdy;

    protected Rigidbody rb;
    protected Transform destination;
    protected NavMeshAgent agent;

    private void Start()
    {
        
        agent = GetComponent<NavMeshAgent>();
        turningSpeed = moveSpeed * 0.6f;
        rotationSpeed = turningSpeed * 0.5f;
        agent.acceleration = moveSpeed * 0.7f;
        agent.speed = moveSpeed;
        agent.angularSpeed = turningSpeed;
        rb = GetComponent<Rigidbody>();
    }

    public void MoveUnit(Transform destination) // Think about linking turning speed and rotation speed to the moveSpeed
    {
        Quaternion newRotation = Quaternion.LookRotation(destination.transform.position - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
        agent.destination = destination.position;
    }
}

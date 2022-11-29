using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : UnitFunctionality
{
    private bool inRangeOfDest = true;

    private void Awake()
    {
        destination = GameObject.FindGameObjectWithTag("DestinationPoint").GetComponent<Transform>();
    }

    private void Update()
    {
        CheckInput();
        if (!inRangeOfDest)
        {
            MoveUnit(destination);
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Plane ground = new Plane(Vector3.up, Vector3.zero);

        float rayLength;

        if (ground.Raycast(ray, out rayLength))
        {
            return ray.GetPoint(rayLength);
        }
        else
        {
            return Vector3.zero;
        }

    }

    private void CheckInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            destination.position = GetMouseWorldPosition();
            Debug.Log("Position: " + destination.position);
            inRangeOfDest = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("DestinationPoint"))
        {
            inRangeOfDest = true;
            Debug.Log("Reached the destination");
        }
    }
}

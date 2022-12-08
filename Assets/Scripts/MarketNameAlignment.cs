using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarketNameAlignment : MonoBehaviour
{
    void Update()
    {
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Vector3.up);
    }
}

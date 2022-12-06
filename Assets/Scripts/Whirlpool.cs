using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Whirlpool : MonoBehaviour
{
    public List<GameObject> affectedObjects = new();
    //[SerializeField] private float pullingStrength;
    [SerializeField] private float lifeTime;
    private float timer = 0;
    [SerializeField] private float tick = 0.5f;

    void Start()
    {
        StartCoroutine(LifeTime());
        //pullingStrength = Random.Range(1f, 10f);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        foreach (GameObject ship in affectedObjects)
        {
            if(ship == null)
            {
                affectedObjects.Remove(null);
                continue;
            }
            UnitFunctionality shipFunc = ship.GetComponent<UnitFunctionality>();
            
            if (timer > tick)
            {
                Hurt(shipFunc);
            }
        }
        if (timer > tick)
        {
            timer -= tick;
        }
    }

    // Theoretically it should work, but I think naviagtion with navmesh doesn't mix well with physics
    /*
    void Pull(Rigidbody ship)
    {
        Vector3 direction = transform.position - ship.transform.position;
        direction = direction.normalized;
        ship.AddForce(direction * pullingStrength);
    }
    */

    void Hurt(UnitFunctionality ship)
    {
        ship.HealthPoints--;
        if (ship.HealthPoints == 0) ship.isDead = true;
    }

    IEnumerator LifeTime()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if((collision.transform.root.CompareTag("Player") || collision.transform.root.CompareTag("Enemy")) && !affectedObjects.Contains(collision.transform.root.gameObject))
        {
            affectedObjects.Add(collision.transform.root.gameObject);
            collision.transform.root.gameObject.GetComponent<UnitFunctionality>().MoveSpeed /= 2;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if ((collision.transform.root.CompareTag("Player") || collision.transform.root.CompareTag("Enemy")) && affectedObjects.Contains(collision.transform.root.gameObject))
        {
            affectedObjects.Remove(collision.transform.root.gameObject);
            collision.transform.root.gameObject.GetComponent<UnitFunctionality>().MoveSpeed *= 2;
        }
    }

}

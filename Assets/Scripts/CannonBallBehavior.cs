using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBallBehavior : MonoBehaviour
{

    public GameObject parentShip;

    private UnitFunctionality parentShipFunctionality;
    private UnitFunctionality struckShipFunctionality;

    // Start is called before the first frame update
    void Start()
    {

        parentShipFunctionality = parentShip.GetComponent<UnitFunctionality>();
        
        StartCoroutine(LifeTime());
    }

    IEnumerator LifeTime()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.transform.CompareTag("Water"))
        {
            StopCoroutine(LifeTime());
            Destroy(gameObject);
        }

        // Check if the ship hit has a different tag from the one that shot it
        else if (!parentShip.CompareTag(collision.transform.root.tag) && !collision.transform.root.CompareTag("Whirlpool"))
        {
            Collider myCollider = collision.GetContact(0).otherCollider;
            // If the ship hit was Player's
              
            struckShipFunctionality = collision.gameObject.GetComponent<UnitFunctionality>();
            if (myCollider.CompareTag("HitboxHull"))
            {
                struckShipFunctionality.HealthPoints -= parentShipFunctionality.AttackPower;
            }
            else if (myCollider.CompareTag("HitboxMast"))
            {
                struckShipFunctionality.HealthPoints -= parentShipFunctionality.AttackPower / 2;
            }

            if(struckShipFunctionality.HealthPoints == 0 && !struckShipFunctionality.isDead)
            {
                parentShipFunctionality.coinsPossessed += struckShipFunctionality.WorthCoins;
                struckShipFunctionality.isDead = true;
            }
            StopCoroutine(LifeTime());
            Destroy(gameObject);
        }
    }
}

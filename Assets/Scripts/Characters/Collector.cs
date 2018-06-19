using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D other)
    {        
        if (other.gameObject.CompareTag("Weapon"))
        {
            pickupWeapon(other.gameObject);
        }
    }

    private void pickupWeapon(GameObject weapon) 
    {
        Debug.Log("Pick up weapon " + weapon.name);
    }
}

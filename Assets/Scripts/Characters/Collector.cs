using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : MonoBehaviour {

    private F3DWeaponController WeaponController;

    private void Awake()
    {
        WeaponController = GetComponent<F3DWeaponController>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {        
        if (other.gameObject.CompareTag("Collectable"))
        {
            Collectable collectable = other.GetComponent<Collectable>();
            switch(collectable.CollectableType)
            {
                case CollectableType.Weapon:
                    pickupWeapon((CollectableWeapon)collectable);
                    break;
            }
            collectable.Pickup();
        }
    }

    private void pickupWeapon(CollectableWeapon weapon) 
    {
        WeaponController.ActivateWeapon(weapon.WeaponIdentifier);
    }
}

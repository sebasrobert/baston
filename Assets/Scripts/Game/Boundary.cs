using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundary : MonoBehaviour {

    void OnTriggerExit2D(Collider2D other)
    {        
        GameObject otherGameObject = other.gameObject;
        if(otherGameObject.CompareTag("Player"))
        {
            F3DCharacter character = otherGameObject.GetComponent<F3DCharacter>();
            character.Suicide();
        }
    }
}

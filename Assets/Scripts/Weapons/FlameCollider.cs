using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameCollider : MonoBehaviour {

    public FlameThrower FlameThrower;
    private ParticleSystem Flames;
    private List<ParticleCollisionEvent> CollisionEvents;

    void Start()
    {
        Flames = GetComponent<ParticleSystem>();
        CollisionEvents = new List<ParticleCollisionEvent>();
    }

    void OnParticleCollision(GameObject objectHit)
    {
        F3DDamage damage = objectHit.GetComponentInParent<F3DDamage>();
        if (damage)
        {
            Flames.GetCollisionEvents(objectHit, CollisionEvents);
            FlameThrower.OnFlameHit(damage, CollisionEvents[0].intersection, CollisionEvents[0].normal);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameThrower : F3DGenericWeapon
{
    public ParticleSystem Flames;

    public override void Fire()
    {
        Flames.Play();
    }

    public override void Stop()
    {
        Flames.Stop();
    }

    public void OnFlameHit(F3DDamage damage, Vector2 hitPoint, Vector2 hitNormal)
    {
        damage.OnDamage(transform.root.gameObject, F3DDamage.DamageType.Fire, DamageAmount, hitPoint, hitNormal);
    }

}
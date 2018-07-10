using UnityEngine;
using System.Collections;

public class F3DDamage : MonoBehaviour
{
    public enum DamageType
    {
        Generic,
        Character,
        Mud,
        Wood,
        Metal,
        Shield
    }

    public DamageType Type;
    private IDamageable _damageable;
    public Transform Hit;
    public Vector2 HitNormalOffset;

    private void Awake()
    {
        _damageable = GetComponent<IDamageable>();
    }
 

    public void OnDamage(GameObject source, int damageAmount, Vector3 contactPoint, Vector3 contactNormal)
    {
        if (_damageable != null)
            _damageable.OnDamage(source, damageAmount);
        SpawnHit(contactPoint, contactNormal);
    }

    private void SpawnHit(Vector3 contactPoint, Vector3 contactNormal)
    {
        if (Hit == null) return;
        var normalOffset = contactNormal * Random.Range(HitNormalOffset.x, HitNormalOffset.y);
        var hit = F3DSpawner.Spawn(Hit, contactPoint + normalOffset,
            Quaternion.LookRotation(Vector3.forward, contactNormal), transform);
        F3DSpawner.Despawn(hit, 1f);
    }
}
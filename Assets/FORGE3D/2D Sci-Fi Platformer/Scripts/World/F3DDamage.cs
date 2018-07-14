using UnityEngine;

public class F3DDamage : MonoBehaviour
{
    public enum DamageType
    {
        Shot,
        Freeze
    }

    public DamageableType DamageableType;
    private IDamageable _damageable;
    private Freezeable _freezable;
    public Transform Hit;
    public Vector2 HitNormalOffset;

    private void Awake()
    {
        _damageable = GetComponent<IDamageable>();
        _freezable = GetComponent<Freezeable>();
    }
 

    public void OnDamage(GameObject source, DamageType damageType, int damageAmount, Vector3 contactPoint, Vector3 contactNormal)
    {
        if (damageType == DamageType.Shot)
        {
            if (_damageable != null)
                _damageable.OnDamage(source, damageAmount);
            SpawnHit(contactPoint, contactNormal);
        }
        else if (damageType == DamageType.Freeze)
        {
            if(_freezable != null)
            {
                _freezable.Freeze(damageAmount);
            }
        }
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
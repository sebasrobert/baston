using UnityEngine;

public class F3DDamage : MonoBehaviour
{
    public enum DamageType
    {
        Shot,
        Freeze,
        Fire
    }

    public DamageableType DamageableType;
    public Transform Hit;
    public Vector2 HitNormalOffset;
    public int FireHitRate;

    private IDamageable _damageable;
    private Freezeable _freezable;
    private float LastFireHitTime;

    private void Awake()
    {
        _damageable = GetComponent<IDamageable>();
        _freezable = GetComponent<Freezeable>();
        LastFireHitTime = float.NegativeInfinity;
    }
 

    public void OnDamage(GameObject source, DamageType damageType, int damageAmount, Vector3 contactPoint, Vector3 contactNormal)
    {
        if (damageType == DamageType.Shot)
        {
            OnShot(source, damageAmount, contactPoint, contactNormal);
        }
        else if (damageType == DamageType.Freeze)
        {
            OnFreeze(damageAmount);
        }
        else if(damageType == DamageType.Fire)
        {
            OnFire(source, damageAmount, contactPoint, contactNormal);
        }
    }

    private void OnShot(GameObject source, int damageAmount, Vector3 contactPoint, Vector3 contactNormal)
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

    private void OnFreeze(int damageAmount)
    {
        if (_freezable != null)
        {
            _freezable.Freeze(damageAmount);
        }
    }

    private void OnFire(GameObject source, int damageAmount, Vector3 contactPoint, Vector3 contactNormal)
    {
        if (_damageable == null)
        {
            return;
        }

        var currentTime = Time.time;
        var timeElapsedSinceLastHit = currentTime - LastFireHitTime;

        if(timeElapsedSinceLastHit > 1f / FireHitRate)
        {
            _damageable.OnDamage(source, damageAmount);
            SpawnHit(contactPoint, contactNormal);
            LastFireHitTime = currentTime;
        }
    }
}
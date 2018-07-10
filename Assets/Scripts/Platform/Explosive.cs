using System.Collections;
using UnityEngine;

public class Explosive : MonoBehaviour, IDamageable
{

    public GameObject explosionPrefab;
    public Vector2 explodeVelocity;
    public float destroyExplosionDelay;
    public LayerMask explositionHitLayerMask;
    public float damageDelay;
    public float highDamageRadius;
    public int highDamageAmount;
    public float lowDamageRadius;
    public int lowDamageAmount;

    private Rigidbody2D rigidBody;
    private Collider2D explosiveCollider;
    private Vector2 lastVelocity = new Vector2(0f, 0f);
    private GameObject lastPlayerMovingExplosive;
    private bool exploding;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        explosiveCollider = GetComponent<Collider2D>();
        exploding = false;
    }

    public void OnDamage(GameObject source, int damageAmount)
    {
        Explode(source);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            lastPlayerMovingExplosive = other.gameObject;
        }

        bool velocityCollision = Mathf.Abs(lastVelocity.x) > explodeVelocity.x || Mathf.Abs(lastVelocity.y) > explodeVelocity.y;

        if (velocityCollision)
        {
            Explode(lastPlayerMovingExplosive);
        }
    }

    private void FixedUpdate()
    {
        lastVelocity.x = rigidBody.velocity.x;
        lastVelocity.y = rigidBody.velocity.y;
    }

    public void Explode(GameObject source)
    {
        if (exploding)
        {
            return;
        }

        exploding = true;
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        Destroy(explosion, destroyExplosionDelay);

        StartCoroutine(MakeDamageAround(source));
    }

    private IEnumerator MakeDamageAround(GameObject source)
    {
        yield return new WaitForSeconds(damageDelay);

        Collider2D[] collidersAround = Physics2D.OverlapCircleAll(transform.position, lowDamageRadius, explositionHitLayerMask);
        RaycastHit2D[] explosionHits = new RaycastHit2D[1];

        foreach (Collider2D colliderAround in collidersAround)
        {
            Vector2 raycastDirection = colliderAround.bounds.center - transform.position;
            int numberOfHits = explosiveCollider.Raycast(raycastDirection, explosionHits, lowDamageRadius, explositionHitLayerMask);

            if(numberOfHits == 0 || explosionHits[0].collider != colliderAround)
            {                
                continue;
            }

            GameObject objectHit = colliderAround.gameObject;

            F3DDamage damage = objectHit.GetComponent<F3DDamage>();
            if (damage)
            {
                float sqrMagnitude = (objectHit.transform.position - transform.position).sqrMagnitude;
                var damageAmount = (sqrMagnitude < highDamageRadius * highDamageRadius) ? highDamageAmount : lowDamageAmount;

                damage.OnDamage(source, damageAmount, explosionHits[0].point, explosionHits[0].normal);
            }
        }

        Destroy(gameObject);
    }
}

using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public abstract class Collectable : MonoBehaviour {

    public CollectableType CollectableType;

    public void Pickup()
    {
        Destroy(gameObject);
    }
}

public enum CollectableType
{
    Weapon,
    Health
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GameEvents;

public class F3DCharacter : MonoBehaviour, IDamageable
{
    public enum InputType
    {
        KEYBOAD_MOUSE,
        GAMING_CONTROLLER
    }

    public int maxHealth;
    public int mediumHealthThreshold;
    public int lowHealthThreshod;
    public Slider heathBarSlider;
    public Color highHealthColor;
    public Color mediumHealthColor;
    public Color lowHealthColor;
    public float noHitDurationWhileRespawn;
    public float blinkRateWhileRespawn;
    public InputType inputControllerType;
    public string inputControllerName;

    private F3DCharacterController _controller;
    private Image heathBarImage;
    private int _hitTriggerCounter;
    private float _hitTriggerTimer;
    private Rigidbody2D _rBody;
    private F3DWeaponController _weaponController;
    private bool _isDead;
    private float currentHealth;
    private Collider2D[] _colliders;
    private Vector3 _initialPosition;
    private Quaternion _initialRotation;

    // Use this for initialization
    void Awake()
    {
        _rBody = GetComponent<Rigidbody2D>();
        _controller = GetComponent<F3DCharacterController>();
        _weaponController = GetComponent<F3DWeaponController>();
        _colliders = gameObject.GetComponentsInChildren<Collider2D>();
        heathBarImage = heathBarSlider.GetComponentInChildren<Image>();
        currentHealth = maxHealth;
        _initialPosition = gameObject.transform.position;
        _initialRotation = gameObject.transform.rotation;
    }

    public void OnDamage(GameObject source, int damageAmount)
    {
        if (_controller == null) return;
        if (_isDead) return;

        float newHealth = Mathf.Clamp(currentHealth - damageAmount, 0, maxHealth);
        ChangeHealth(newHealth);

        // Dead Already?
        if (currentHealth < float.Epsilon)
        {
            Die(source);
            return;
        }

        if (!ReferenceEquals(gameObject, source))
        {
            EventManager.TriggerEvent(new GameEvents.WeaponHitPlayerEvent() { Shooter = source, Target = gameObject });
        }

        // Play Hit Animation and limit hit animation triggering 
        if (_hitTriggerCounter < 1)
            _controller.Character.SetTrigger("Hit");
        _hitTriggerCounter++;
    }

    public void RespawnAtInitialPosition()
    {
        _weaponController.ReactivateDefaultWeapon();
        gameObject.transform.position = _initialPosition;
        gameObject.transform.rotation = _initialRotation;
        _rBody.bodyType = RigidbodyType2D.Dynamic;
        _controller.enabled = true;
        _controller.Character.SetBool("DeathFront", false);
        _controller.Character.SetBool("DeathBack", false);
        ChangeHealth(maxHealth);

        EnableCollidersAndDeactivateCollisionWithProjectiles(_colliders);

        _isDead = false;
        StartCoroutine(BlinkEffectWhileRespawn());
    }

    private IEnumerator BlinkEffectWhileRespawn()
    {
        SpriteRenderer[] spriteRenderers = gameObject.GetComponentsInChildren<SpriteRenderer>();

        var newAlpha = 1f;
        var elapsedTime = 0f;
        while (elapsedTime < noHitDurationWhileRespawn)
        {
            newAlpha = (newAlpha < float.Epsilon) ? 1f : 0f;
            ChangeSpriteRenderersAlpha(spriteRenderers, newAlpha);
            yield return new WaitForSeconds(blinkRateWhileRespawn);
            elapsedTime += blinkRateWhileRespawn;
        }

        ChangeSpriteRenderersAlpha(spriteRenderers, 1f);

        ActivateCollisionWithProjectiles(_colliders);
    }

    private void ChangeSpriteRenderersAlpha(SpriteRenderer[] spriteRenderers, float newAlpha)
    {
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            var color = spriteRenderer.color;
            spriteRenderer.color = new Color(color.r, color.g, color.b, newAlpha);
        }
    }

    private void EnableCollidersAndDeactivateCollisionWithProjectiles(Collider2D[] colliders)
    {
        foreach (Collider2D c in colliders)
        {
            c.enabled = true;
            c.gameObject.layer = LayerMask.NameToLayer("NoHit");
        }
    }

    private void ActivateCollisionWithProjectiles(Collider2D[] colliders)
    {
        foreach (Collider2D c in colliders)
        {
            c.gameObject.layer = LayerMask.NameToLayer("Player");
        }

    }

    public void Suicide()
    {
        if(_isDead)
        {
            return;
        }

        _isDead = true;
        _controller.enabled = false;
        gameObject.layer = LayerMask.NameToLayer("Dead");
        foreach (Collider2D c in _colliders)
        {
            c.enabled = false;
        }

        EventManager.TriggerEvent(new PlayerSuicideEvent() { Player = gameObject });
    }

    private void Die(GameObject source)
    {
        _isDead = true;
        _rBody.bodyType = RigidbodyType2D.Static;

        // Player Death sequence
        _controller.Character.SetBool(Random.Range(-1f, 1f) > 0 ? "DeathFront" : "DeathBack", true);

        // Dead dont do shit
        _controller.enabled = false;
        gameObject.layer = LayerMask.NameToLayer("Dead");

        foreach (Collider2D c in _colliders)
        {
            c.enabled = false;
        }
        _weaponController.Drop();

        if (ReferenceEquals(gameObject, source))
        {
            EventManager.TriggerEvent(new PlayerSuicideEvent() { Player = gameObject });
        }
        else
        {
            EventManager.TriggerEvent(new PlayerDieEvent() { Killer = source, Dead = gameObject });    
        }

    }

    private void ChangeHealth(float newHealth) 
    {
        currentHealth = newHealth;
        AdjustHealthBarValueAndColor();
    }

    private void AdjustHealthBarValueAndColor() 
    {
        heathBarSlider.value = currentHealth / maxHealth;

        if (currentHealth > lowHealthThreshod && currentHealth > mediumHealthThreshold)
        {
            heathBarImage.color = highHealthColor;
        }
        else if (currentHealth > lowHealthThreshod && currentHealth <= mediumHealthThreshold)
        {
            heathBarImage.color = mediumHealthColor;
        }
        else
        {
            heathBarImage.color = lowHealthColor;
        }
    }

    private void LateUpdate()
    {
        // Dead... Quit trying
        if (_isDead) return;
    //    if (Input.GetKeyDown(KeyCode.K))
    //        OnDamage(1000);

        // Handle Hit Trigger timer
        if (_hitTriggerTimer > 0.5f) // <- Hit timer
        {
            _hitTriggerCounter = 0;
            _hitTriggerTimer = 0;
        }
        _hitTriggerTimer += Time.deltaTime;
    }
}
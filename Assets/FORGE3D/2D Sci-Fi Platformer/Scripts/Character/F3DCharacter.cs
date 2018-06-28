using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GameEvents;

public class F3DCharacter : MonoBehaviour
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

    // Use this for initialization
    void Awake()
    {
        _rBody = GetComponent<Rigidbody2D>();
        _controller = GetComponent<F3DCharacterController>();
        _weaponController = GetComponent<F3DWeaponController>();
        heathBarImage = heathBarSlider.GetComponentInChildren<Image>();
        currentHealth = maxHealth;
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
            _isDead = true;

            // Player Death sequence
            _controller.Character.SetBool(Random.Range(-1f, 1f) > 0 ? "DeathFront" : "DeathBack", true);

            // Dead dont do shit
            _controller.enabled = false;
            gameObject.layer = LayerMask.NameToLayer("Dead");
            _rBody.drag = 2f;

//            for (int i = 0; i < _colliders.Length; i++)
//                _colliders[i].enabled = false;
            _weaponController.Drop();

            EventManager.TriggerEvent(new PlayerDieEvent() { Killer = source, Dead = gameObject });

            return;
        }

        // Play Hit Animation and limit hit animation triggering 
        if (_hitTriggerCounter < 1)
            _controller.Character.SetTrigger("Hit");
        _hitTriggerCounter++;
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
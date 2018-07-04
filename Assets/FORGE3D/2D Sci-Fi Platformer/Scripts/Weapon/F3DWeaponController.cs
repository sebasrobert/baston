using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class F3DWeaponController : MonoBehaviour
{
    public WeaponIdentifier DefaultWeapon;
    public List<WeaponSlot> Slots;

    //
    private F3DCharacterAvatar _avatar;
    private F3DCharacter _character;
    private WeaponPath[] WeaponPaths;
    private WeaponIdentifier CurrentWeapon;
    private int CurrentSlot;
    private bool nextSlotPressed;

    //
    [Serializable]
    public class WeaponSlot
    {
        [SerializeField] public List<F3DGenericWeapon> Weapons;
        [HideInInspector] public int EquippedWeaponCounter = -1;

        public F3DGenericWeapon GetEquippedWeapon()
        {
            if(!HasEquippedWeapon())
            {
                return null;
            }

            return Weapons[EquippedWeaponCounter];
        }

        public bool HasEquippedWeapon()
        {
            return EquippedWeaponCounter != -1;
        }
    }

    public enum WeaponType
    {
        Pistol,
        Assault,
        Shotgun,
        Machinegun,
        Sniper,
        Beam,
        Launcher,
        EnergyHeavy,
        Flamethrower,
        Tesla,
        Thrown,
        Knife,
        Melee
    }

    private class WeaponPath
    {
        public readonly int SlotIndex;
        public readonly int WeaponIndex;

        public WeaponPath(int slotIndex, int weaponIndex)
        {
            SlotIndex = slotIndex;
            WeaponIndex = weaponIndex;
        }
    }

    private void Awake()
    {
        _avatar = GetComponent<F3DCharacterAvatar>();
        _character = GetComponent<F3DCharacter>();
        BuildWeaponPaths();
        DeactivateAllWeapons();
        ActivateWeapon(DefaultWeapon);
    }

    // Use this for initialization
    private void Start() { }


    // Update is called once per frame
    private void Update()
    {
        // Fire
        if (_character.inputControllerType == F3DCharacter.InputType.KEYBOAD_MOUSE)
        {
            if (Input.GetMouseButtonDown(0))
            {
                GetCurrentWeapon().Fire();
            }

            // Stop
            if (Input.GetMouseButtonUp(0))
            {
                GetCurrentWeapon().Stop();
            }

            // Switch Weapon Slot
            if (Input.GetButton(_character.inputControllerName + "Slot1"))
                ActivateSlot(0);
            if (Input.GetButton(_character.inputControllerName + "Slot2"))
                ActivateSlot(1);
            if (Input.GetButton(_character.inputControllerName + "Slot3"))
                ActivateSlot(2);
        } else {
            if(Input.GetButton(_character.inputControllerName + "Fire")) {
                GetCurrentWeapon().Fire();   
            } else {
                GetCurrentWeapon().Stop();
            }

            if (!nextSlotPressed && Input.GetButtonDown(_character.inputControllerName + "NextSlot"))
            {
                nextSlotPressed = true;
                ActivateNextSlot();
            }
            if (Input.GetButtonUp(_character.inputControllerName + "NextSlot"))
            {
                nextSlotPressed = false;
            }
        }
    }

    private void BuildWeaponPaths()
    {
        int weaponCount = Enum.GetNames(typeof(WeaponIdentifier)).Length;
        WeaponPaths = new WeaponPath[weaponCount];

        for (int slotCounter = 0; slotCounter < Slots.Count; slotCounter++)
        {
            WeaponSlot weaponSlot = Slots[slotCounter];
            for (int weaponCounter = 0; weaponCounter < weaponSlot.Weapons.Count; weaponCounter++)
            {
                WeaponIdentifier weaponIndentifier = weaponSlot.Weapons[weaponCounter].Identifier;
                WeaponPaths[(int)weaponIndentifier] = new WeaponPath(slotCounter, weaponCounter);
            }
        }
    }

    private void ActivateSlot(int slot)
    {
        if (slot < 0 || slot > Slots.Count - 1) return;
        if (!Slots[slot].HasEquippedWeapon()) return;

        WeaponIdentifier activeWeaponIdentifier = Slots[slot].GetEquippedWeapon().Identifier;
        ActivateWeapon(activeWeaponIdentifier);
    }

    private void ActivateNextSlot()
    {
        if(Slots.Count <= 1)
        {
            return;
        }

        for (int i = 1; i < Slots.Count; i++)
        {
            int nextSlot = (CurrentSlot + i) % Slots.Count;
            if(Slots[nextSlot].HasEquippedWeapon())
            {
                ActivateSlot(nextSlot);
                return;
            }
        }
    }

    //////////////////////////////////////////////////////////////
    // Pass animation data to an active weapon controller
    public void SetBool(string var, bool value)
    {
        GetCurrentWeapon().Animator.SetBool(var, value);
    }

    public void SetFloat(string var, float value)
    {
        GetCurrentWeapon().Animator.SetFloat(var, value);
    }
    //////////////////////////////////////////////////////////////

    // Weapon activation
    public void ActivateWeapon(WeaponIdentifier weaponIdentifier)
    {
        WeaponPath weaponPath = WeaponPaths[(int)weaponIdentifier];

        if(weaponPath != null) 
        {
            GetCurrentWeapon().gameObject.SetActive(false);

            SetCurrentWeapon(weaponIdentifier);
            SetEquippedWeaponInSlot(weaponIdentifier);
            GetCurrentWeapon().gameObject.SetActive(true);

            UpdateCharacterHands(_avatar.Characters[_avatar.CharacterId]);
        }
    }

    public void AddEquippedWeapon(WeaponIdentifier weaponIdentifier)
    {
        WeaponPath weaponPath = WeaponPaths[(int)weaponIdentifier];

        if (weaponPath != null)
        {
            Slots[weaponPath.SlotIndex].EquippedWeaponCounter = weaponPath.WeaponIndex;
        }
    }

    private void DeactivateAllWeapons()
    {
        foreach(WeaponSlot slot in Slots)
        {
            foreach(F3DGenericWeapon weapon in slot.Weapons)
            {
                weapon.gameObject.SetActive(false);
            }
        }
    }

    // Weapon drop on character killed
    public void Drop()
    {
        F3DGenericWeapon currentWeapon = GetCurrentWeapon();
        var powerUpPrefab = currentWeapon.PowerUp;
        if (powerUpPrefab != null)
        {
            // Random Rotation
            var rot =currentWeapon.FXSocket.rotation *
                      Quaternion.Euler(0, 0, Random.Range(-5, 5));

            // Spawn
            var powerUp = F3DSpawner.Spawn(powerUpPrefab, currentWeapon.FXSocket.position,
                rot, null);

            // Flip
            var powerUpFlip = Mathf.Sign(currentWeapon.FXSocket.lossyScale.x);
            var curScale = powerUp.localScale;
            curScale.x *= powerUpFlip;
            powerUp.localScale = curScale;

            // Add random force / torque
            var powerUpRb = powerUp.GetComponent<Rigidbody2D>();
            powerUpRb.AddForce((Vector2) currentWeapon.FXSocket.up * Random.Range(8, 12),
                ForceMode2D.Impulse);
            powerUpRb.AddTorque(Random.Range(-250, 250), ForceMode2D.Force);

            // remove gun after 2s
            F3DSpawner.Despawn(powerUp, 2f);
        }

        // Deactivate components
        currentWeapon.gameObject.SetActive(false);
        this.enabled = false;
    }

    public void ReactivateDefaultWeapon()
    {
        this.enabled = true;
        ResetEquippedWeapons();
        ActivateWeapon(DefaultWeapon);
    }

    public F3DGenericWeapon GetCurrentWeapon()
    {
        WeaponPath weaponPath = WeaponPaths[(int)CurrentWeapon];
        return Slots[weaponPath.SlotIndex].Weapons[weaponPath.WeaponIndex];
    }

    private void ResetEquippedWeapons()
    {
        foreach(WeaponSlot slot in Slots)
        {
            slot.EquippedWeaponCounter = -1;
        }
    }

    private void SetCurrentWeapon(WeaponIdentifier weaponIdentifier)
    {
        CurrentWeapon = weaponIdentifier;
    }

    private void SetEquippedWeaponInSlot(WeaponIdentifier weaponIdentifer)
    {
        WeaponPath weaponPath = WeaponPaths[(int)weaponIdentifer];
        Slots[weaponPath.SlotIndex].EquippedWeaponCounter = weaponPath.WeaponIndex;
        CurrentSlot = weaponPath.SlotIndex;
    }

    // Avatar Hands
    public void UpdateCharacterHands(F3DCharacterAvatar.CharacterArmature armature)
    {
        var myWeapon = GetCurrentWeapon();
        myWeapon.LeftHand.sprite = GetSpriteFromHandId(myWeapon.LeftHandId, armature);
        myWeapon.RightHand.sprite = GetSpriteFromHandId(myWeapon.RightHandId, armature);
    }

    private Sprite GetSpriteFromHandId(int id, F3DCharacterAvatar.CharacterArmature armature)
    {
        switch (id)
        {
            case 0:
                return armature.Hand1;
            case 1:
                return armature.Hand2;
            case 2:
                return armature.Hand3;
            case 3:
                return armature.Hand4;
            default:
                return armature.Hand1;
        }
    }
}
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine.Networking;

public class F3DWeaponController : NetworkBehaviour
{
    public int EquippedSlot;
    public int EquippedWeapon;
    public List<WeaponSlot> Slots;

    //
    private F3DCharacterAvatar _avatar;
    private F3DCharacter _character;

    //
    [Serializable]
    public class WeaponSlot
    {
        [SerializeField] public List<F3DGenericWeapon> Weapons;
        public int WeaponSlotCounter = 1;

        public void Forward()
        {
            WeaponSlotCounter++;
            if (WeaponSlotCounter >= Weapons.Count)
                WeaponSlotCounter = 0;
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

    private void Awake()
    {
        _avatar = GetComponent<F3DCharacterAvatar>();
        _character = GetComponent<F3DCharacter>();
        ActivateWeapon(EquippedSlot, EquippedWeapon);
    }

    // Use this for initialization
    private void Start() { }


    // Update is called once per frame
    private void Update()
    {
        if (!hasAuthority) {
            return;
        }

        // Fire
        if (_character.inputControllerType == F3DCharacter.InputType.KEYBOAD_MOUSE)
        {
            if (Input.GetMouseButtonDown(0))
            {
                CmdFire();
            }

            // Stop
            if (Input.GetMouseButtonUp(0))
            {
                CmdStop();
            }
        } else {
            if(Input.GetButton(_character.inputControllerName + "Fire")) {
                CmdFire();
            } else {
                CmdStop();
            }
        }

        // Switch Weapon Slot
        if (Input.GetKeyDown(KeyCode.Alpha1))
            CmdActivateSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            CmdActivateSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            CmdActivateSlot(2);
        if (Input.GetKeyDown(KeyCode.Alpha4))
            CmdActivateSlot(3);
        if (Input.GetKeyDown(KeyCode.Alpha5))
            CmdActivateSlot(4);
        if (Input.GetKeyDown(KeyCode.Alpha6))
            CmdActivateSlot(5);
        if (Input.GetKeyDown(KeyCode.F))
            CmdActivateSlot(6);
    }

    [Command]
    private void CmdFire()
    {
        Slots[EquippedSlot].Weapons[EquippedWeapon].Fire();   
        RpcFire();
    }

    [ClientRpc]
    private void RpcFire()
    {
        if (!isServer) {
            Slots[EquippedSlot].Weapons[EquippedWeapon].Fire();   
        }
    }

    [Command]
    private void CmdStop()
    {
        Slots[EquippedSlot].Weapons[EquippedWeapon].Stop();   
        RpcStop();
    }

    [ClientRpc]
    private void RpcStop()
    {
        Slots[EquippedSlot].Weapons[EquippedWeapon].Stop();   
    }

    [Command]
    private void CmdActivateSlot(int slot)
    {
        RpcActivateSlot(slot);
    }

    [ClientRpc]
    private void RpcActivateSlot(int slot)
    {
        ActivateSlot(slot);
    }

    private void ActivateSlot(int slot)
    {
        if (slot > Slots.Count - 1) return;
        if (slot == EquippedSlot)
            Slots[EquippedSlot].Forward();
        EquippedSlot = slot;
        EquippedWeapon = Slots[EquippedSlot].WeaponSlotCounter;
        ActivateWeapon(EquippedSlot, EquippedWeapon);
    }



    //////////////////////////////////////////////////////////////
    // Pass animation data to an active weapon controller
    public void SetBool(string var, bool value)
    {
        Slots[EquippedSlot].Weapons[EquippedWeapon].Animator.SetBool(var, value);
    }

    public void SetFloat(string var, float value)
    {
        Slots[EquippedSlot].Weapons[EquippedWeapon].Animator.SetFloat(var, value);
    }
    //////////////////////////////////////////////////////////////

    // Weapon activation
    private void ActivateWeapon(int slot, int weapon)
    {
        EquippedSlot = slot;
        EquippedWeapon = weapon;
        for (var i = 0; i < Slots.Count; i++)
        {
            for (var y = 0; y < Slots[i].Weapons.Count; y++)
                Slots[i].Weapons[y].gameObject.SetActive(slot == i && weapon == y);
        }
     //   Slots[slot].Weapons[weapon].OnAnimationReadyEvent();

        UpdateCharacterHands(_avatar.Characters[_avatar.CharacterId]);
    }

    

    // Weapon drop on character killed
    public void Drop()
    {
        var powerUpPrefab = Slots[EquippedSlot].Weapons[EquippedWeapon].PowerUp;
        if (powerUpPrefab != null)
        {
            // Random Rotation
            var rot = Slots[EquippedSlot].Weapons[EquippedWeapon].FXSocket.rotation *
                      Quaternion.Euler(0, 0, Random.Range(-5, 5));

            // Spawn
            var powerUp = F3DSpawner.Spawn(powerUpPrefab, Slots[EquippedSlot].Weapons[EquippedWeapon].FXSocket.position,
                rot, null);

            // Flip
            var powerUpFlip = Mathf.Sign(Slots[EquippedSlot].Weapons[EquippedWeapon].FXSocket.lossyScale.x);
            var curScale = powerUp.localScale;
            curScale.x *= powerUpFlip;
            powerUp.localScale = curScale;

            // Add random force / torque
            var powerUpRb = powerUp.GetComponent<Rigidbody2D>();
            powerUpRb.AddForce((Vector2) Slots[EquippedSlot].Weapons[EquippedWeapon].FXSocket.up * Random.Range(8, 12),
                ForceMode2D.Impulse);
            powerUpRb.AddTorque(Random.Range(-250, 250), ForceMode2D.Force);
        }

        // Deactivate components
        Slots[EquippedSlot].Weapons[EquippedWeapon].gameObject.SetActive(false);
        this.enabled = false;
    }

    public F3DGenericWeapon GetCurrentWeapon()
    {
        return Slots[EquippedSlot].Weapons[EquippedWeapon];
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
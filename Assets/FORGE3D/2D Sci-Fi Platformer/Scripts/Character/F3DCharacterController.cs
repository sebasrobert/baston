using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class F3DCharacterController : NetworkBehaviour
{

    // Components
    public Animator Character;
    public Transform WeaponSocket;

    // Settings
    public LayerMask Ground;

    public float MaxVelocityX;
    public float MaxVelocityY;
    public float MaxSpeed = 1f;
    public float CrouchSpeed = 0.75f;
    public float MaxSpeedBackwards = 1f;
    public float JumpVelocity = 28f;
    public float DoubleJumpVelocity = 24f;
    public float GroundCheckCircleSize;
    public float AimTime;

    //
    private Rigidbody2D _rb2D;
    private F3DWeaponController _weaponController;
    private F3DCharacter _character;
    private F3DCharacterAudio _audio;

    //
    private bool _facingRight = true;
    private bool _jump;
    private bool _doubleJump;
    private bool _crouch;
    private bool _grounded;
    private bool _lastGroundedState;
    private float _horizontal;
    private float _horizontalSignLast;

    // DEBUG GIZMOS
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, GroundCheckCircleSize);
    }

    // Use this for initialization
    private void Awake()
    {
        _character = GetComponent<F3DCharacter>();
        _weaponController = GetComponent<F3DWeaponController>();
        _rb2D = GetComponent<Rigidbody2D>();
        _audio = GetComponent<F3DCharacterAudio>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!hasAuthority)
        {
            return;
        }

        // Debug Teleport
        if (Input.GetKeyDown(KeyCode.T))
        {
            var cursorWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            cursorWorldPoint.z = transform.position.z;
            transform.position = cursorWorldPoint;
        }

        // Check and apply Player Input
        _horizontal = Input.GetAxis(_character.inputControllerName + "Horizontal");
        Character.SetFloat("Horizontal", Mathf.Abs(_horizontal));
        _weaponController.SetFloat("Horizontal", Mathf.Abs(_horizontal));


        // Check Grounded
        var groundedCollider = Physics2D.OverlapCircle(transform.position, GroundCheckCircleSize, Ground);

        //
        _grounded = groundedCollider;
        if (_lastGroundedState != _grounded && _grounded)
        {
            _jump = false;
            _doubleJump = false;
            _audio.Surface = F3DCharacterAudio.SurfaceType.Metal;
            _audio.OnLand();
        }
        _lastGroundedState = _grounded;

        //
        Debug.DrawLine(transform.position, transform.position - Vector3.up * GroundCheckCircleSize);

        // Jump
        if (Input.GetButtonDown(_character.inputControllerName + "Jump"))
        {
            if (!_jump && !_doubleJump)
            {
                _rb2D.velocity = new Vector2(_rb2D.velocity.x, JumpVelocity);
                _jump = true;
                _grounded = false;

                //
                _audio.OnJump();
            }
            else if (_jump && !_doubleJump)
            {
                _rb2D.velocity = new Vector2(_rb2D.velocity.x, DoubleJumpVelocity);
                _doubleJump = true;
                _jump = false;

                //
                _audio.OnDoubleJump();
            }
        }

        // Crouch 
        // Exit crouch state on button up
        if (Input.GetButtonUp(_character.inputControllerName + "Crouch"))
            _crouch = false;

        // Enter crouch on button hold, no jump
        if (Input.GetButton(_character.inputControllerName + "Crouch") && !_jump && !_doubleJump && !_crouch)
            _crouch = true;

        // Jump cancels any current crouch state
        if (_jump || _doubleJump)
            _crouch = false;
        
        // Set Crouch flag 
        Character.SetBool("Crouch", _crouch);
        _weaponController.SetBool("Crouch", _crouch);

        // Set Grounded flag
        Character.SetBool("Grounded", _grounded);
        _weaponController.SetBool("Grounded", _grounded);


        ////////////////////////////////// FIRING
        if (WeaponSocket == null) return;

        // Get Mouse to World Position
        var aimPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        aimPos.z = 0;

        // Look direction
        Vector3 dir;
        if (_character.inputControllerType == F3DCharacter.InputType.KEYBOAD_MOUSE)
        {
            dir = (aimPos - WeaponSocket.position).normalized;
            dir.z = 0;
        }
        else
        {
            float rightH = Input.GetAxis(_character.inputControllerName + "RightX");
            float rightV = Input.GetAxis(_character.inputControllerName + "RightV");
            dir = new Vector3(rightH, -rightV, 0);
        }

        // Weapon socket to FX Socket offset
        var currentWeapon = _weaponController.GetCurrentWeapon();
        if (currentWeapon.Type == F3DWeaponController.WeaponType.Melee)
        {
            WeaponSocket.rotation = Quaternion.identity;
        }
        else
        {
            var offset = currentWeapon.FXSocket.position - WeaponSocket.position;
            offset.z = 0;
            var localOffset = WeaponSocket.InverseTransformVector(offset);
            localOffset.x = 0;
            localOffset.z = 0;
            Debug.DrawLine(Vector3.zero, localOffset * transform.lossyScale.x, Color.yellow);

            //  Debug.DrawLine(WeaponSocket.position, currentWeapon.FXSocket.position, Color.yellow);
            Vector3 weaponDir;
            if (_character.inputControllerType == F3DCharacter.InputType.KEYBOAD_MOUSE)
            {
                var worldOffset = WeaponSocket.TransformVector(localOffset) - WeaponSocket.right * 5 * Mathf.Sign(dir.x);
                weaponDir = (aimPos - (WeaponSocket.position + worldOffset)).normalized;
            }
            else
            {
                float rightH = Input.GetAxis(_character.inputControllerName + "RightX");
                float rightV = Input.GetAxis(_character.inputControllerName + "RightV");
                weaponDir = new Vector3(rightH, -rightV, 0);
            }


            var socketRotation = Quaternion.LookRotation(Vector3.forward,
                Mathf.Sign(dir.x) * Vector3.Cross(Vector3.forward, weaponDir));
            WeaponSocket.rotation = Quaternion.Lerp(WeaponSocket.rotation, socketRotation, Time.deltaTime * AimTime);

            // Lock Weapon Socket Angle
            var rot = WeaponSocket.rotation;
            const float z = 0.35f;
            if (_facingRight && WeaponSocket.rotation.z < -z)
            {
                rot.z = -z;
                WeaponSocket.rotation = rot;
            }
            else if (!_facingRight && WeaponSocket.rotation.z > z)
            {
                rot.z = z;
                WeaponSocket.rotation = rot;
            }
        }

        // Flip
        if (dir.x > 0 && !_facingRight)
            Flip();
        else if (dir.x < 0 && _facingRight)
            Flip();
        else if (dir.x == 0) {
            if (_horizontal > 0 && !_facingRight) 
               Flip();
            else if (_horizontal < 0 && _facingRight)
                Flip();
        }

        // Draw Velocity
        Debug.DrawLine(currentWeapon.FXSocket.position, aimPos, Color.blue);

        // Check Facing
        if (_horizontal > 0) _horizontalSignLast = 1f;
        else if (_horizontal < 0) _horizontalSignLast = -1f;
        var facingSing = (_facingRight ? 1f : -1f) * _horizontalSignLast;

        // Set Animator Vars
        Character.SetFloat("facingRight", facingSing);
        _weaponController.SetFloat("facingRight", facingSing);

        _weaponController.SetFloat("Speed", Mathf.Abs(_rb2D.velocity.x));
        Character.SetFloat("Speed", Mathf.Abs(_rb2D.velocity.x));

        _weaponController.SetFloat("vSpeed", _rb2D.velocity.y);
        Character.SetFloat("vSpeed", _rb2D.velocity.y);
    }

    private void FixedUpdate()
    {
        var newVelocity = _rb2D.velocity;
        var speedDamp = _crouch ? CrouchSpeed : MaxSpeed;
        newVelocity.x = _horizontal * MaxVelocityX * speedDamp;

        _rb2D.velocity = newVelocity;
    }

    private void Flip()
    {
        _facingRight = !_facingRight;
        CmdSetFacingRight(_facingRight);
    }

    [Command]
    void CmdSetFacingRight(bool newFacingRight)
    {
        _facingRight = newFacingRight;
        RpcSetFacingRight(newFacingRight);
    }

    [ClientRpc]
    void RpcSetFacingRight(bool newFacingRight)
    {
        _facingRight = newFacingRight;
        var theScale = transform.localScale;
        if (_facingRight) {
            theScale.x = Mathf.Abs(theScale.x);
        }
        else if (theScale.x >= 0) {
            theScale.x = -theScale.x;
        }
        transform.localScale = theScale;

    }
}
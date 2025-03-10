using System;
using System.Collections.Generic;
using Cinemachine;
using StarterAssets;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif


[RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
[RequireComponent(typeof(PlayerInput))]
#endif
public class PlayerController : NetworkBehaviour
{
    #region FIELDS

    private const float _threshold = 0.01f;

    [Header("Player")] [Tooltip("Move speed of the character in m/s")]
    public float MoveSpeed = 2.0f;

    [Tooltip("Sprint speed of the character in m/s")]
    public float SprintSpeed = 5.335f;

    [Tooltip("How fast the character turns to face movement direction")] [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = 0.12f;

    [Tooltip("Acceleration and deceleration")]
    public float SpeedChangeRate = 10.0f;

    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

    [Space(10)] [Tooltip("The height the player can jump")]
    public float JumpHeight = 1.2f;

    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    public float Gravity = -15.0f;

    [Space(10)] [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    public float JumpTimeout = 0.50f;

    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    public float FallTimeout = 0.15f;

    [Header("Player Grounded")]
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    public bool Grounded = true;

    [Tooltip("Useful for rough ground")] public float GroundedOffset = -0.14f;

    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float GroundedRadius = 0.28f;

    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;

    [Header("Cinemachine")]
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    public GameObject CinemachineCameraTarget;

    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 70.0f;

    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -30.0f;

    [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
    public float CameraAngleOverride;

    [Tooltip("For locking the camera position on all axis")]
    public bool LockCameraPosition;

    private readonly float _terminalVelocity = 53.0f;

    private float _animationBlend;
    private Animator _animator;
    private int _animIDFreeFall;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDMotionSpeed;

    // animation IDs
    private int _animIDSpeed;
    private float _cinemachineTargetPitch;

    // cinemachine
    private float _cinemachineTargetYaw;
    private GameObject _cineMachineVirtualCamera;
    private CharacterController _controller;
    private float _fallTimeoutDelta;

    private bool _hasAnimator;
    private StarterAssetsInputs _input;

    // timeout deltatime
    private float _jumpTimeoutDelta;
    private GameObject _mainCamera;

#if ENABLE_INPUT_SYSTEM
    private PlayerInput _playerInput;
#endif
    private float _rotationVelocity;

    // player
    private float _speed;
    private float _targetRotation;
    private float _verticalVelocity;

    private bool IsCurrentDeviceMouse
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
        }
    }

    #endregion

    public static PlayerController Instance { get; private set; }
    

    private void Awake()
    {
        Debug.Log("PlayerController Awake");
        // get a reference to our main camera
        if (_mainCamera == null) _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        if (_cineMachineVirtualCamera == null)
            _cineMachineVirtualCamera = GameObject.FindGameObjectWithTag("CinemachineVirtualCamera");
    }

    private void Start()
    {
        Debug.Log("PlayerController Start");
        _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

        _hasAnimator = TryGetComponent(out _animator);
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<StarterAssetsInputs>();

        AssignAnimationIDs();
        
        // reset our timeouts on start
        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;
    }

    private void Update()
    {
        if (!IsOwner) return;
        _hasAnimator = TryGetComponent(out _animator);

        JumpAndGravity();
        GroundedCheck();
        Move();
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            HandlePickUpAndDropKeyObject();
        }
    }

    private void HandlePickUpAndDropKeyObject()
    {
        if (currentKeyObject != null)
        {
            Debug.Log("Attempting to drop key object!");
            DropKeyObjectServerRpc();
        }
        else
        {
            Debug.Log("Attempting to pick up key object!");
            AttemptPickupKeyObject();
        }
    }
    
    private void AttemptPickupKeyObject()
    {
        RaycastHit hit;
        CharacterController charContr = GetComponent<CharacterController>();
        Vector3 p1 = transform.position + charContr.center + Vector3.up * (-charContr.height * 0.5F);
        Vector3 p2 = p1 + Vector3.up * charContr.height;

        // Cast character controller shape 10 meters forward to see if it is about to hit anything.
        if (Physics.CapsuleCast(p1, p2, charContr.radius, transform.forward, out hit, TargetDistance, targetLayer))
        {
            Debug.Log("Hit object: " + hit.transform.name);     
            if (hit.transform.TryGetComponent<KeyObject>(out var keyObject) && !keyObject.IsHeld)
            {
                Debug.Log("KeyObject collected by player!" + keyObject.IsHeld);
                KeyObjectController.Instance.PickUpKeyObject(keyObject);
            }
        }
    }
    
    #region DROP KEY OBJECT
    
    [ServerRpc(RequireOwnership = false)]
    private void DropKeyObjectServerRpc(ServerRpcParams rpcParams = default)
    {
        //Change the variable data of the KeyObject(False) - when dropped
        KeyObjectController.Instance.ChangeKeyObjectVariableData(rpcParams.Receive.SenderClientId, false);
        DropKeyObjectClientRpc();
    }

    [ClientRpc]
    private void DropKeyObjectClientRpc()
    {
        if (currentKeyObject == null) return;

        Vector3 dropPosition = transform.position + Vector3.up * 1f + transform.forward;
        currentKeyObject.transform.SetParent(null);
        currentKeyObject.transform.position = dropPosition;
        currentKeyObject.SetHeldState(false);

        KeyObjectController.Instance.NotifyKeyObjectDropped(currentKeyObject.NetworkObjectId, dropPosition);
        currentKeyObject = null;
    }
    
    #endregion
    
    

    private void LateUpdate()
    {
        CameraRotation();
    }

    private void OnDrawGizmosSelected()
    {
        var transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        var transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (Grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(
            new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
            GroundedRadius);
    }
    
    public override void OnNetworkSpawn()
    {
        Debug.Log("PlayerController OnNetworkSpawn");
        if (!IsOwner) return;
        Instance = this;
#if ENABLE_INPUT_SYSTEM
        _playerInput = GetComponent<PlayerInput>();
        _playerInput.enabled = true;
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

        _cineMachineVirtualCamera.transform.GetComponent<CinemachineVirtualCamera>().Follow =
            CinemachineCameraTarget.transform;
        transform.position =
            _spawnPositionList[iXgGameMultiplayer.Instance.GetPlayerDataIndexFromClientId(OwnerClientId)];

        NetworkManager.Singleton.OnClientConnectedCallback += OnNetworkClientConnectedCallBack;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnNetworkClientDisconnectCallBack;
    }

    private void OnNetworkClientDisconnectCallBack(ulong clientId)
    {
        Debug.Log("Client disconnected:- " + clientId);
    }

    private void OnNetworkClientConnectedCallBack(ulong clientId)
    {
        Debug.Log("Client connected:- " + clientId);
    }


    #region MOVEMENT METHODS

    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    private void GroundedCheck()
    {
        // set sphere position, with offset
        var spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
            transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
            QueryTriggerInteraction.Ignore);

        // update animator if using character
        if (_hasAnimator) _animator.SetBool(_animIDGrounded, Grounded);
    }

    private void CameraRotation()
    {
        // if there is an input and camera position is not fixed
        if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
        {
            //Don't multiply mouse input by Time.deltaTime;
            var deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
            _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
        }

        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // Cinemachine will follow this target
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
            _cinemachineTargetYaw, 0.0f);
    }

    private void Move()
    {
        // set target speed based on move speed, sprint speed and if sprint is pressed
        var targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

        // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

        // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is no input, set the target speed to 0
        if (_input.move == Vector2.zero) targetSpeed = 0.0f;

        // a reference to the players current horizontal velocity
        var currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

        var speedOffset = 0.1f;
        var inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                Time.deltaTime * SpeedChangeRate);

            // round speed to 3 decimal places
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        // normalise input direction
        var inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        if (_input.move != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                              _mainCamera.transform.eulerAngles.y;
            var rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                RotationSmoothTime);

            // rotate to face input direction relative to camera position
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }


        var targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

        // move the player
        _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                         new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

        // update animator if using character
        if (_hasAnimator)
        {
            _animator.SetFloat(_animIDSpeed, _animationBlend);
            _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
        }
    }

    private void JumpAndGravity()
    {
        if (Grounded)
        {
            // reset the fall timeout timer
            _fallTimeoutDelta = FallTimeout;

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDJump, false);
                _animator.SetBool(_animIDFreeFall, false);
            }

            // stop our velocity dropping infinitely when grounded
            if (_verticalVelocity < 0.0f) _verticalVelocity = -2f;

            // Jump
            if (_input.jump && _jumpTimeoutDelta <= 0.0f)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                // update animator if using character
                if (_hasAnimator) _animator.SetBool(_animIDJump, true);
            }

            // jump timeout
            if (_jumpTimeoutDelta >= 0.0f) _jumpTimeoutDelta -= Time.deltaTime;
        }
        else
        {
            // reset the jump timeout timer
            _jumpTimeoutDelta = JumpTimeout;

            // fall timeout
            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                // update animator if using character
                if (_hasAnimator) _animator.SetBool(_animIDFreeFall, true);
            }

            // if we are not grounded, do not jump
            _input.jump = false;
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (_verticalVelocity < _terminalVelocity) _verticalVelocity += Gravity * Time.deltaTime;
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
            if (FootstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center),
                    FootstepAudioVolume);
            }
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
            AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center),
                FootstepAudioVolume);
    }

    #endregion
    
    

    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }

    #region ADDED

    private const float TargetDistance = 2f;

    public float capsuleRadius = 0.4f;
    public LayerMask targetLayer;
    public KeyObject currentKeyObject;

    [SerializeField] private List<Vector3> _spawnPositionList;

    public event EventHandler OnDoorInteracted;

    #endregion
}
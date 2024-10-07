using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour, IDamageable
{
    public PhotonView PV;
    
    [Header("CurrState")]
    public String currState;
    
    public StateMachine stateMachine;
    public IdleState idleState;
    public MoveState moveState;
    public SprintState sprintState;
    public JumpState jumpState;
    public CruchIdleState CruchIdleState;
    public CruchMoveState CruchMoveState;
    public CagedState cagedState;
    public DeadState deadState;
    [Space]
    
    [Header("Player")]
    public CharacterController controller;
    public Vector2 moveVec;
    public Vector2 look;
    public Animator anim;
    public GameObject cage;
    [SerializeField] private GameObject cameras;
    [Space] 
    
    [Header("Ragdoll")] 
    [SerializeField]private GameObject rig;
    public Collider[] ragdollCols;
    public Rigidbody[] ragdollRigs;
    [Space]
    
    [Header("Map UI")]
    [SerializeField] private GameObject bigMapUI;
    [SerializeField] private GameObject miniMapUI;
    [Space] 
    
    [Header("Aim")] 
    public bool isAiming;
    public Transform _spine;
    public Transform target;
    
    [Space]
    
    [Header("Move")]
    public float moveSpeed = 4f;
    public float sprintSpeed = 6.335f;
    public float cruchSpeed = 2f;
    
    public bool isSprinting = false;
    public bool isCruching = false;
    
    public float normalHeight = 1.1f;
    public float normalCenter = 0.56f;
    
    public float cruchedHieght = 0.6f;
    public float cruchedCenter = 0.35f;
    
    public float speedChangeRate = 50f;
    [Space] 
    
    [Header("Jump")]
    public bool isJumping = false;
    public float jumpHeight = 1.2f;
    public float gravity = -15f;
    public float jumpTimeOut = 0.3f;
    public float fallTimeOut = 0.15f;
    public float jumpTimeoutDelta;
    public float fallTimeoutDelta;
    [Space] 
    
    [Header("PlayerGrounded")] 
    public bool groundCheckAble = true;
    public bool grounded = true;
    public float groundedOffset = -0.14f;
    public float groundedRadius = 0.28f;
    public LayerMask groundLayers;
    [Space] 
    
    [Header("Interact")] 
    public bool isMapExpand;
    [Space]
    
    [Header("Camera Setting")]
    public Camera mainCam;
    public float rotationSmoothTime;
    [Space] 
    
    [Header("HP and Shield")]
    [SerializeField] private float _maxHP = 100f;
    [SerializeField] private float _maxShield = 100f;
    private float _curHP;
    private float _curShield;
    public float curHP { get => _curHP; set { _curHP = value;
        hpSlider.value = _curHP / _maxHP;
    } }
    public float curShield { get => _curShield; set { _curShield = value;
        shieldText.value = _curShield / _maxShield;
    } }
    public Slider hpSlider;
    public Slider shieldText;
    public Material fullScreenEffectMat;
    private Coroutine _vignetteEffectCoroutine;
    
    
    [HideInInspector] 
    public int animIDX;
    [HideInInspector] 
    public int animIDY;
    [HideInInspector]
    public int animIDJump;
    [HideInInspector]
    public int animIDGrounded;
    [HideInInspector]
    public int animIDFreeFall;
    [HideInInspector]
    public int animIDCruched;
    
    public float animBlendX;
    public float animBlendY;
    // [HideInInspector]
    public float _speed;
    [HideInInspector]
    public float verticalVel;
    [HideInInspector]
    public float rotationVel;
    [HideInInspector]
    public float terminalVel = 53f;
    [HideInInspector]
    public float targetRotation;
    
    

    void Awake()
    {
        ragdollCols = rig.GetComponentsInChildren<Collider>();
        ragdollRigs = rig.GetComponentsInChildren<Rigidbody>();
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        
        if (PV.IsMine)
        {
            mainCam = Camera.main;
            
            bigMapUI.layer = LayerMask.NameToLayer("MyMap");
            miniMapUI.layer = LayerMask.NameToLayer("Minimap");

            stateMachine = new StateMachine();
            
            idleState = new IdleState(this, stateMachine, "IDLE");
            moveState = new MoveState(this, stateMachine, "MOVE");
            sprintState = new SprintState(this, stateMachine, "SPRINT");
            jumpState = new JumpState(this, stateMachine, "JUMP");
            CruchIdleState = new CruchIdleState(this, stateMachine, "CRUCH IDLE");
            CruchMoveState = new CruchMoveState(this, stateMachine, "CRUCH MOVE");
            cagedState = new CagedState(this, stateMachine, "CAGED");
            deadState = new DeadState(this, stateMachine, "DEAD");
        }
        else
        {
            bigMapUI.layer = LayerMask.NameToLayer("OtherMap");
            miniMapUI.layer = LayerMask.NameToLayer("OtherMap");
        }
    }

    void Start()
    {
        if (PV.IsMine)
        {
            SpawnSetting();
            AnimationStringHash();
            stateMachine.Init(cagedState);
            jumpTimeoutDelta = jumpTimeOut;
            fallTimeoutDelta = fallTimeOut;
            
            
            // int layer = LayerMask.NameToLayer("NonTarget");
            // ChangeLayerRecursively(gameObject, layer);
            
            curHP = _maxHP;
            curShield = _maxShield;
        }
    }
    
    void AnimationStringHash()
    {
        animIDX = Animator.StringToHash("dirX");
        animIDY = Animator.StringToHash("dirY");
        animIDJump = Animator.StringToHash("jump");
        animIDGrounded = Animator.StringToHash("grounded");
        animIDFreeFall = Animator.StringToHash("freeFall");
        animIDCruched = Animator.StringToHash("cruched");
    }

    void GroundCheck()
    {
        if (groundCheckAble)
        {
            Vector3 spherePos = new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z);
            grounded = Physics.CheckSphere(spherePos, groundedRadius, groundLayers, QueryTriggerInteraction.Ignore);
            
            anim.SetBool(animIDGrounded, grounded);
        }
    }

    public void SpawnSetting()
    {
        cameras.SetActive(true);
    }
    
    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(
            new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z),
            groundedRadius);
    }

    void Update()
    {
        if (PV.IsMine)
        {
            currState = stateMachine.currState.stateName;
            
            GroundCheck();
            stateMachine.currState.HandleInput();
            stateMachine.currState.LogicalUpdate();
        }
    }

    // private void LateUpdate()
    // {
    //     if (target != null)
    //     {
    //         // Calculate direction from spine to target
    //         var direction = target.position.y - _spine.position.y;
    //         
    //         // Calculate angle from spine to target
    //         var angle = Mathf.Atan2(direction, 1) * Mathf.Rad2Deg;
    //         
    //         // Rotate spine to target
    //         _spine.rotation = Quaternion.Euler(-angle, _spine.rotation.eulerAngles.y, _spine.rotation.eulerAngles.z);
    //     }  
    //          
    // }

    void FixedUpdate()
    {
        if (PV.IsMine)
        {
            stateMachine.currState.PhysicalUpdate();
        }
    }

    #region Input Controls
    void OnMove(InputValue value)
    {
        moveVec = value.Get<Vector2>();
    }

    void OnSprint(InputValue value)
    {
        if (value.isPressed)
        {
            isSprinting = !isSprinting;
        }
    }

    void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            isJumping = true;
        }
    }

    void OnCruch(InputValue value)
    {
        if (value.isPressed)
        {
            isCruching = !isCruching;
        }
    }

    void OnLook(InputValue value)
    {
        look = value.Get<Vector2>();
    }

    void OnAim(InputValue value)
    {
        isAiming = value.isPressed;
    }
    
    public void OnFire()
    {
        if(PV.IsMine)
            anim.SetTrigger("Fire");
    }
    
    public void OnReload()
    {
        if(PV.IsMine)
            anim.SetTrigger("Reload");
    }

    public void OnMapExpand(InputValue value)
    {
        if (value.isPressed)
        {
            isMapExpand = !isMapExpand;
        }
    }
    #endregion

    private void ChangeLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;

        foreach(Transform child in obj.transform)
        {
            ChangeLayerRecursively(child.gameObject, layer);
        }
    }
    
    [PunRPC]
    public void TakeDamageRPC(float damage)
    {
        if(curShield > 0)
        {
            curShield = curShield - damage < 0 ? 0 : curShield - damage;
            if (PV.IsMine)
            {
                if(_vignetteEffectCoroutine != null)
                    StopCoroutine(_vignetteEffectCoroutine);
                fullScreenEffectMat.SetColor("_VignetteColor", new Color(0.5f, 0.8f, 1, 1));
                fullScreenEffectMat.SetFloat("_VignetteIntensity", 1.5f);
                _vignetteEffectCoroutine = StartCoroutine(VignetteEffect());
            }
        }
        else
        {
            curHP -= damage;
            if (PV.IsMine)
            {
                if(_vignetteEffectCoroutine != null)
                    StopCoroutine(_vignetteEffectCoroutine);
                fullScreenEffectMat.SetColor("_VignetteColor", new Color(1, 0, 0, 1));
                fullScreenEffectMat.SetFloat("_VignetteIntensity", 1.5f);
                _vignetteEffectCoroutine = StartCoroutine(VignetteEffect());
            }
        }
    }
    
    IEnumerator VignetteEffect()
    {
        //1초간 점점 줄어듬
        float time = 0;
        while (time < 1)
        {
            time += Time.deltaTime;
            fullScreenEffectMat.SetFloat("_VignetteIntensity", Mathf.Lerp(1.5f, 0, time));
            yield return null;
        }
    }
    
    [PunRPC]
    public void ActivateRagDoll()
    {
        controller.enabled = false;
        anim.enabled = false;
        foreach (Collider col in ragdollCols)
        {
            col.enabled = true;
        }
        foreach(Rigidbody rb in ragdollRigs)
        {
            rb.useGravity = true;
            rb.isKinematic = false;
        }
    }
}

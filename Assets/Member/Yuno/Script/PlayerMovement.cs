using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))] 
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerMovement2D : AbstractGimmick
{
    public static PlayerMovement2D Instance { get; private set; }
    
    // 💡 핵심: 바닥 또는 천장에 붙어있으면 true 반환 -> MemoryManager에서 중력 온오프 가능
    public bool IsGrounded => isGrounded || isCeiling;

    [Header("이동 설정 (Movement)")]
    public float moveSpeed = 8f;        

    [Header("점프 설정 (Jump)")]
    public float jumpForce = 16f;       

    [Header("조작감 보정 (Game Feel)")]
    public float coyoteTime = 0.1f;    
    public float jumpBufferTime = 0.1f; 

    [Header("바닥/천장 감지 (Ground & Ceiling Check)")]
    public Transform groundCheck;       
    public Transform ceilingCheck;      // 👈 인스펙터에서 플레이어 머리 위 오브젝트 할당
    public Vector2 groundCheckSize = new Vector2(0.8f, 0.1f); 
    public LayerMask groundLayer;       

    [Header("파티클 효과 (Particles)")]
    public ParticleSystem movementParticle;

    // 상태 및 내부 변수
    private Rigidbody2D rb;
    private Animator anim;              
    private SpriteRenderer spriteRep;   
    
    private float moveInput;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    
    private bool isGrounded; // 바닥 착지 여부
    private bool isCeiling;  // 천장 착지 여부

    private bool isGravityInverted = false;

    void Awake()
    {
        Instance = this;

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRep = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        SoundManager.instance.PlaySFX("restart");
        if (movementParticle != null)
        {
            var emission = movementParticle.emission;
            emission.enabled = false;
        }
    }

    protected override void HandleStateChanged(bool isProcessOn)
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

        if (isProcessOn) 
        {
            rb.gravityScale = 3f; // 일반 중력
            isGravityInverted = false;
            
            // 💡 플레이어 중력 원상복구 로그
            ConsoleManager.Instance.LogSystem("[SYS_UPDATE] Player gravity normalized.");
        }
        else 
        {
            rb.gravityScale = -3f; // 반전 중력
            isGravityInverted = true;
            
            // 💡 플레이어 중력 반전 로그 (에러 색상으로 띄워 경고 느낌 강조)
            ConsoleManager.Instance.LogError("[WARNING] Player gravity inverted!");
        }
    }

    void Update()
    {
        // 1. 바닥 및 천장 감지
        CheckGroundAndCeiling();

        // 💡 수정된 부분: y축 속도가 0.5 이하일 때(즉, 점프해서 위로 상승 중이 아닐 때)만 착지로 인정!
        if (isGrounded && rb.linearVelocity.y <= 0.5f)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // 2. 이동 입력 처리
        if (isGravityInverted)
        {
            // 중력 반전 중: 천장에 붙어있을 때만 이동 가능 / 공중에 떠 있으면 0 (직진 방지)
            moveInput = isCeiling ? Input.GetAxisRaw("Horizontal") : 0f;
        }
        else
        {
            // 일반 중력: 정상 이동
            moveInput = Input.GetAxisRaw("Horizontal");
        }

        // 3. 점프 입력 처리 (일반 중력 + 바닥 착지 상태에서만 점프 가능 / 천장 점프 불가)
        if (!isGravityInverted && Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
            SoundManager.instance.PlaySFX("Jump");
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // 점프 실행 (바닥 착지 전용)
        if (!isGravityInverted && jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f; 
        }

        UpdateAnimations();
        UpdateParticles();
    }

    // 💡 천장 및 바닥 감지 전용 함수
    private void CheckGroundAndCeiling()
    {
        // 바닥 감지
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);
        }

        // 천장 감지
        if (ceilingCheck != null)
        {
            isCeiling = Physics2D.OverlapBox(ceilingCheck.position, groundCheckSize, 0f, groundLayer);
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    private void UpdateAnimations()
    {
        if (moveInput > 0f) spriteRep.flipX = false; 
        else if (moveInput < 0f) spriteRep.flipX = true;  

        bool isMoving = Mathf.Abs(moveInput) > 0f;
        anim.SetBool("isMoving", isMoving);
    }

    private void UpdateParticles()
    {
        if (movementParticle == null) return;
        var emission = movementParticle.emission;
        bool isMovingActively = Mathf.Abs(moveInput) > 0f || Mathf.Abs(rb.linearVelocity.y) > 0.5f;
        emission.enabled = isMovingActively;
    }

    private void OnDrawGizmosSelected()
    {
        // 바닥 감지 영역 (빨간색)
        Gizmos.color = Color.red;
        if (groundCheck != null)
        {
            Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
        }

        // 천장 감지 영역 (파란색)
        Gizmos.color = Color.blue;
        if (ceilingCheck != null)
        {
            Gizmos.DrawWireCube(ceilingCheck.position, groundCheckSize);
        }
    }
}
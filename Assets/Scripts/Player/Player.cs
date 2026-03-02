using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour, IDamageable
{
    [Header("References")]
    [SerializeField] private GameEvents gameEvents;
    [SerializeField] private Animator animator;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 12f;

    [Header("Lives")]
    [SerializeField] private int maxLives = 3;
    private int currentLives;

    [Header("Invincibility")]
    [SerializeField] private float invincibilityTime = 1.2f;
    private bool isInvincible = false;
    private SpriteRenderer spriteRenderer;

    [Header("Bounce")]
    [SerializeField] private float stompBounceForce = 8f;

    [Header("Double Jump")]
    [SerializeField] private bool hasDoubleJump = false;
    private bool canDoubleJump = false;

    [Header("Dash")]
    [SerializeField] private bool hasDash = false;
    [SerializeField] private float dashForce = 20f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 1f;
    private bool isDashing = false;
    private float dashCooldownTimer = 0f;

    [Header("Jump Feel")]
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;
    [SerializeField] private float coyoteTime = 0.15f;
    [SerializeField] private float jumpBufferTime = 0.15f;

    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    private Rigidbody2D rb;
    private PlayerInputActions inputActions;
    private Vector2 movementInput;
    private bool isGrounded;
    private bool isDead = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        inputActions = new PlayerInputActions();

        currentLives = maxLives;


        inputActions.Player.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += _ => movementInput = Vector2.zero;
        inputActions.Player.Jump.performed += _ => jumpBufferCounter = jumpBufferTime;
        inputActions.Player.Dash.performed += _ => TryDash();
    }

    private void OnEnable() => inputActions.Enable();
    private void OnDisable() => inputActions.Disable();

    private void Start()
    {
        transform.position = GameManager.Instance.GetCurrentCheckpoint();
        gameEvents?.OnLivesChanged?.Invoke(currentLives);
    }

    private void Update()
    {
        if (isDead) return;

        if (movementInput.x != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(movementInput.x), 1, 1);
        }

        if (!isDashing)
            Move(movementInput.x);

        coyoteTimeCounter -= Time.deltaTime;
        jumpBufferCounter -= Time.deltaTime;

        if (jumpBufferCounter > 0)
        {
            TryJump();
        }

        ApplyBetterGravity();

        if (dashCooldownTimer > 0f)
            dashCooldownTimer -= Time.deltaTime;

        animator.SetFloat("Speed", Mathf.Abs(movementInput.x));
        animator.SetFloat("YVelocity", rb.linearVelocity.y);
        animator.SetBool("IsGrounded", isGrounded);
    }

    private void Move(float direction)
    {
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
    }

    private void TryJump()
    {
        if (isGrounded || coyoteTimeCounter > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
            coyoteTimeCounter = 0;
            jumpBufferCounter = 0;
            canDoubleJump = hasDoubleJump;
        }
        else if (canDoubleJump)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            canDoubleJump = false;
            jumpBufferCounter = 0;
            animator.SetTrigger("DoubleJump");
        }
    }

    private void ApplyBetterGravity()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !inputActions.Player.Jump.IsPressed())
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    public void TakeDamage(int amount)
    {
        if (isInvincible || isDead) return;

        currentLives -= amount;
        gameEvents?.OnLivesChanged?.Invoke(currentLives);
        animator.SetTrigger("Hit");

        if (currentLives <= 0) Die();
        else StartCoroutine(InvincibilityCoroutine());
    }

    private System.Collections.IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        float elapsed = 0f;
        while (elapsed < invincibilityTime)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(0.1f);
            elapsed += 0.1f;
        }
        spriteRenderer.enabled = true;
        isInvincible = false;
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;
        gameEvents?.OnGameOver?.Invoke();
    }

    public void Respawn()
    {
        transform.position = GameManager.Instance.GetCurrentCheckpoint();
        rb.linearVelocity = Vector2.zero;
        currentLives = maxLives;
        isDead = false;
        gameEvents?.OnLivesChanged?.Invoke(currentLives);
    }

    public void SetCheckpoint(Vector3 position)
    {
        GameManager.Instance.SetCheckpoint(position);
    }

    private void OnCollisionEnter2D(Collision2D collision) => CheckGrounded(collision, true);
    private void OnCollisionExit2D(Collision2D collision) => CheckGrounded(collision, false);

    private void OnCollisionStay2D(Collision2D collision) => CheckGrounded(collision, true);
    private void CheckGrounded(Collision2D collision, bool state)
    {
        foreach (var contact in collision.contacts)
        {
            if (contact.normal.y > 0.9f)
            {
                isGrounded = state;
                if (state)
                {
                    coyoteTimeCounter = coyoteTime;
                    canDoubleJump = false;
                }
                return;
            }
        }
    }

    public void Bounce() => rb.linearVelocity = new Vector2(rb.linearVelocity.x, stompBounceForce);

    private void TryDash()
    {
        if (!hasDash || isDashing || dashCooldownTimer > 0f) return;
        StartCoroutine(Dash());
    }

    private System.Collections.IEnumerator Dash()
    {
        isDashing = true;
        dashCooldownTimer = dashCooldown;
        float direction = movementInput.x != 0 ? Mathf.Sign(movementInput.x) : transform.localScale.x;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(direction * dashForce, 0f);

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        isDashing = false;
    }

    public void EnableDoubleJump(bool value) => hasDoubleJump = value;
    public void EnableDash(bool value) => hasDash = value;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<IPowerUp>(out var powerUp))
            powerUp.Apply(this);
    }
}
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour, IDamageable
{
    [Header("References")]
    [SerializeField] private GameEvents gameEvents;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 12f;

    [Header("Lives")]
    [SerializeField] private int maxLives = 3;

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

    private Rigidbody2D rb;
    private PlayerInputActions inputActions;

    private Vector2 movementInput;
    private bool jumpPressed;
    private bool isGrounded;

    private int currentLives;
    private Vector3 lastCheckpointPosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputActions = new PlayerInputActions();

        

        currentLives = maxLives;
        lastCheckpointPosition = transform.position;

        gameEvents?.OnLivesChanged?.Invoke(currentLives);

        inputActions.Player.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += _ => movementInput = Vector2.zero;
        inputActions.Player.Jump.performed += _ => jumpPressed = true;
        inputActions.Player.Dash.performed += _ => TryDash();
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Update()
    {
        if (isDead) return;

        if (!isDashing)
            Move(movementInput.x);

        if (jumpPressed)
        {
            Jump();
            jumpPressed = false;
        }

        if (dashCooldownTimer > 0f)
            dashCooldownTimer -= Time.deltaTime;
    }
    private void Move(float direction)
    {
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
    }

    private void Jump()
    {
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
            canDoubleJump = hasDoubleJump;
        }
        else if (canDoubleJump)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            canDoubleJump = false;
        }
    }
    public void EnableDoubleJump(bool value)
    {
        hasDoubleJump = value;
    }

    public void Bounce()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, stompBounceForce);
    }

    public void TakeDamage(int amount)
    {
        currentLives -= amount;
        gameEvents?.OnLivesChanged?.Invoke(currentLives);

        if (currentLives <= 0)
        {
            Die();
        }
    }

    private bool isDead = false;

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        gameEvents?.OnGameOver?.Invoke();
    }

    public void Respawn()
    {
        transform.position = lastCheckpointPosition;
        rb.linearVelocity = Vector2.zero;

        currentLives = maxLives;
        canDoubleJump = false;
        isDashing = false;
        isDead = false;

        gameEvents?.OnLivesChanged?.Invoke(currentLives);
    }

    public void SetCheckpoint(Vector3 position)
    {
        lastCheckpointPosition = position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (var contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
                break;
            }
        }
    }

    private void TryDash()
    {
        if (!hasDash || isDashing || dashCooldownTimer > 0f)
            return;

        StartCoroutine(Dash());
    }

    private System.Collections.IEnumerator Dash()
    {
        isDashing = true;
        dashCooldownTimer = dashCooldown;

        float direction = Mathf.Sign(movementInput.x);
        if (direction == 0)
            direction = transform.localScale.x;

        rb.linearVelocity = new Vector2(direction * dashForce, 0f);

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
    }

    public void EnableDash(bool value)
    {
        hasDash = value;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<IPowerUp>(out var powerUp))
            powerUp.Apply(this);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }
}

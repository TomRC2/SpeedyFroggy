using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour, IDamageable
{
    private Rigidbody2D rb;
    private Vector2 movementInput;
    private bool jumpPressed;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private int maxLives = 3;
    private int currentLives;
    private bool isGrounded;

    [Header("Dash")]
    [SerializeField] private float dashForce = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;

    private bool hasDash = false;
    private bool isDashing = false;
    private float dashTimer;
    private float cooldownTimer;
    private bool dashPressed;

    [Header("DoubleJump")]
    private bool hasDoubleJump = false;
    private bool canDoubleJump = false;


    private PlayerInputActions inputActions;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputActions = new PlayerInputActions();

        currentLives = maxLives;

        inputActions.Player.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => movementInput = Vector2.zero;

        inputActions.Player.Dash.performed += ctx => dashPressed = true;

        inputActions.Player.Jump.performed += ctx => jumpPressed = true;
    }

    private void OnEnable() => inputActions.Enable();
    private void OnDisable() => inputActions.Disable();

    private void Update()
    {
        if (!isDashing)
            Move(movementInput.x);

        if (jumpPressed)
        {
            Jump();
            jumpPressed = false;
        }

        if (hasDash && dashPressed && cooldownTimer <= 0f)
        {
            StartCoroutine(Dash());
            dashPressed = false;
        }

        cooldownTimer -= Time.deltaTime;
    }


    public void Move(float direction)
    {
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
    }

    public void Jump()
    {
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            canDoubleJump = hasDoubleJump;
        }
        else if (canDoubleJump)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            canDoubleJump = false;
        }

    }

    public void TakeDamage(int amount)
    {
        Debug.Log("Daño recibido: " + amount);
        if (currentLives <= 0)
            Die();
    }
    public void EnableDoubleJump(bool value)
    {
        hasDoubleJump = value;
    }
    public void EnableDash(bool value)
    {
        hasDash = value;
    }
    private IEnumerator Dash()
    {
        isDashing = true;
        cooldownTimer = dashCooldown;

        float direction = Mathf.Sign(movementInput.x);
        if (direction == 0) direction = transform.localScale.x;

        rb.linearVelocity = new Vector2(direction * dashForce, 0f);

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
    }


    private void Die()
    {
        Debug.Log("Jugador muerto");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<ICollectible>(out var collectible))
            collectible.Collect();

        if (other.TryGetComponent<IPowerUp>(out var powerup))
            powerup.Apply(this);
    }

    private void OnCollisionEnter2D(Collision2D other) => isGrounded = true;
    private void OnCollisionExit2D(Collision2D other) => isGrounded = false;
}

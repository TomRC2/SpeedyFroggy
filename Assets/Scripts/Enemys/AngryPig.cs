using UnityEngine;

public enum AngryPigState
{
    Walk,
    Run
}

public class AngryPig : Enemigo
{
    [Header("Patrulla")]
    [SerializeField] private Transform puntoA;
    [SerializeField] private Transform puntoB;

    [Header("Stats")]
    [SerializeField] private float runSpeedMultiplier = 2f;

    private Transform destinoActual;
    private Animator animator;
    private bool enraged = false;
    private AngryPigState currentState = AngryPigState.Walk;

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
        destinoActual = puntoB;
        vida = 2;
    }
    public override void TakeDamage(int amount)
    {
        vida -= amount;

        if (vida == 1 && !enraged)
        {
            enraged = true;
            velocidad *= runSpeedMultiplier;
            animator.SetTrigger("Hit");
        }
        else if (vida <= 0)
        {
            Morir();
        }
    }
    private void Update()
    {
        Moverse();
    }

    public override void Moverse()
    {
        float speed = velocidad;

        if (currentState == AngryPigState.Run)
            speed *= runSpeedMultiplier;

        transform.position = Vector2.MoveTowards(
            transform.position,
            destinoActual.position,
            speed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, destinoActual.position) < 0.1f)
        {
            destinoActual = destinoActual == puntoA ? puntoB : puntoA;
            Flip();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.collider.GetComponent<IDamageable>()?.TakeDamage(1);
        }
    }

    protected override void Morir()
    {
        Destroy(gameObject);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public void Stomp()
    {
        vida--;

        animator.SetTrigger("Hit");

        if (vida <= 0)
        {
            Morir();
        }
        else
        {
            currentState = AngryPigState.Run;
            animator.SetBool("IsRunning", true);
        }
    }
}

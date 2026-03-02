using UnityEngine;

public class Slime : Enemigo
{
    [SerializeField] private Transform puntoA;
    [SerializeField] private Transform puntoB;

    private Transform destinoActual;
    private Animator animator;

    private  void Awake()
    {
        animator = GetComponent<Animator>();
        vida = 3;
    }

    private void Start()
    {
        destinoActual = puntoB;
    }

    private void Update()
    {
        Moverse();
    }
    public override void TakeDamage(int amount)
    {
        vida -= amount;
        if (vida <= 0)
            Morir();
        animator.SetTrigger("Hit");
    }
    public override void Moverse()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            destinoActual.position,
            velocidad * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, destinoActual.position) < 0.1f)
        {
            destinoActual = destinoActual == puntoA ? puntoB : puntoA;
            Flip();
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
}

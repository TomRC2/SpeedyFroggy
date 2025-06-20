using UnityEngine;

public class EnemigoBasico : Enemigo
{
    [SerializeField] private Transform puntoA;
    [SerializeField] private Transform puntoB;

    private Transform destinoActual;

    private void Start()
    {
        destinoActual = puntoB;
    }

    private void Update()
    {
        Moverse();
    }

    public override void Moverse()
    {
        transform.position = Vector2.MoveTowards(transform.position, destinoActual.position, velocidad * Time.deltaTime);

        if (Vector2.Distance(transform.position, destinoActual.position) < 0.1f)
        {
            destinoActual = destinoActual == puntoA ? puntoB : puntoA;
            Flip();
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}

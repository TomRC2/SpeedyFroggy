using UnityEngine;

public class Slime : EnemigoBasico
{
    private float saltoIntervalo = 1.5f;
    private float saltoTimer;
    [SerializeField] private float fuerzaSalto = 5f;

    private void Update()
    {
        saltoTimer -= Time.deltaTime;
        if (saltoTimer <= 0f)
        {
            Saltar();
            saltoTimer = saltoIntervalo;
        }
    }

    private void Saltar()
    {
        float direccion = isFacingRight ? 1f : -1f;
        Vector2 salto = new Vector2(direccion * velocidad, fuerzaSalto);
        GetComponent<Rigidbody2D>().linearVelocity = salto;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isFacingRight = !isFacingRight;
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }
}

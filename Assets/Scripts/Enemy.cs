using UnityEngine;

public abstract class Enemigo : MonoBehaviour, IDamageable
{
    [SerializeField] protected int vida = 3;
    [SerializeField] protected float velocidad = 2f;

    protected bool isFacingRight = true;

    public virtual void TakeDamage(int amount)
    {
        vida -= amount;
        if (vida <= 0)
            Morir();
    }

    protected virtual void Morir()
    {
        Destroy(gameObject);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent<IDamageable>(out var player))
        {
            player.TakeDamage(1);
        }
    }

    public abstract void Moverse();
}

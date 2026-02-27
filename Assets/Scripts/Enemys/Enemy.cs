using UnityEngine;

public abstract class Enemigo : MonoBehaviour, IDamageable
{
    [SerializeField] protected int vida = 3;
    [SerializeField] protected float velocidad = 2f;

    protected bool isFacingRight = true;
    [SerializeField] private int damage = 1;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(damage);
        }
    }
    protected virtual void Awake()
    {
    }

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

    public abstract void Moverse();
}
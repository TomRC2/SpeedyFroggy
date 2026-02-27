using UnityEngine;

public class HeadTrigger : MonoBehaviour
{
    private Enemigo enemigo;

    private void Awake()
    {
        enemigo = GetComponentInParent<Enemigo>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Player>(out var player))
        {
            enemigo.TakeDamage(1);
            player.Bounce();
        }
    }
}
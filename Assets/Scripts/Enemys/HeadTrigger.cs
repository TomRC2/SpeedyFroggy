using UnityEngine;

public class HeadTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<Player>(out var player))
            return;

        if (transform.parent.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(1);
            player.Bounce();
        }
    }
}
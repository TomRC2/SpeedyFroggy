using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Player>(out var player))
        {
            player.SetCheckpoint(transform.position);
            gameObject.SetActive(false);
        }
    }
}
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private bool activated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activated) return;

        if (other.TryGetComponent<Player>(out var player))
        {
            activated = true;
            player.SetCheckpoint(transform.position);
            animator.SetTrigger("Claim");
        }
    }
}
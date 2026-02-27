using UnityEngine;

public class Fruit : MonoBehaviour, ICollectible
{
    [SerializeField] private GameEvents gameEvents;
    [SerializeField] private int value = 1;

    private Animator animator;
    private bool collected = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Collect()
    {
        if (collected) return;

        collected = true;
        gameEvents.OnFruitCollected?.Invoke(value);

        animator.SetTrigger("Collected");

        Destroy(gameObject, 0.4f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            Collect();
    }
}
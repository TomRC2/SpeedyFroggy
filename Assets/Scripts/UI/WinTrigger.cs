using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    [SerializeField] private GameEvents gameEvents;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        gameEvents?.OnWin?.Invoke();
        gameObject.SetActive(false);
    }
}
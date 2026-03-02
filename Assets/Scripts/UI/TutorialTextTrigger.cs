using UnityEngine;

public class TutorialTextTrigger : MonoBehaviour
{
    [SerializeField] private GameObject tutorialUI;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        tutorialUI.SetActive(false);
    }
}
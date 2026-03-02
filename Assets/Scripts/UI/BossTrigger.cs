using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    [SerializeField] private SkullBoss boss;
    [SerializeField] private BackgroundChanger backgroundChanger;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        boss.ActivateBoss();
        backgroundChanger.ChangeBackground();
        gameObject.SetActive(false);
    }
}
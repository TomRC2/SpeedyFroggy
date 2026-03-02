using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Player player;
    [SerializeField] private GameEvents gameEvents;

    private void Awake()
    {
        panel.SetActive(false);
    }
    private void OnEnable()
    {
        gameEvents.OnGameOver.AddListener(Show);
    }

    private void OnDisable()
    {
        gameEvents.OnGameOver.RemoveListener(Show);
    }

    private void Show()
    {
        panel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Respawn()
    {
        Time.timeScale = 1f;
        panel.SetActive(false);

        player.gameObject.SetActive(true);
        player.Respawn();
    }
}
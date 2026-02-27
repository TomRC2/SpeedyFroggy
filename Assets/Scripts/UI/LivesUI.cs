using UnityEngine;
using UnityEngine.UI;

public class LivesUI : MonoBehaviour
{
    [SerializeField] private GameEvents gameEvents;
    [SerializeField] private Image[] hearts;

    private void OnEnable()
    {
        gameEvents.OnLivesChanged.AddListener(UpdateLives);
    }

    private void OnDisable()
    {
        gameEvents.OnLivesChanged.RemoveListener(UpdateLives);
    }

    private void UpdateLives(int lives)
    {
        for (int i = 0; i < hearts.Length; i++)
            hearts[i].enabled = i < lives;
    }
}
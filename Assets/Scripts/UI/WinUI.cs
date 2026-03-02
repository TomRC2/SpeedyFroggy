using UnityEngine;
using UnityEngine.SceneManagement;

public class WinUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private GameEvents gameEvents;

    private void Awake()
    {
        panel.SetActive(false);
    }

    private void OnEnable()
    {
        gameEvents.OnWin.AddListener(Show);
    }

    private void OnDisable()
    {
        gameEvents.OnWin.RemoveListener(Show);
    }

    private void Show()
    {
        panel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;

        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }
}
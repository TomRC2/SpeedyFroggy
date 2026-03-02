using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private string firstLevelSceneName = "Level1";

    public void Play()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(firstLevelSceneName);
    }

    public void Quit()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
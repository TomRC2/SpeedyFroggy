using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private Vector2 defaultCheckpoint;
    private Vector2 currentCheckpoint;

    private void Awake()
    {
        currentCheckpoint = defaultCheckpoint;
    }

    public void SetCheckpoint(Vector2 pos)
    {
        currentCheckpoint = pos;
        Debug.Log("Checkpoint actualizado mediante GameManager: " + pos);
    }

    public Vector2 GetCurrentCheckpoint() => currentCheckpoint;
}
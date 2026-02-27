using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private Vector2 checkpointPosition;
    public GameObject player;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SetCheckpoint(Vector2 pos)
    {
        checkpointPosition = pos;
        Debug.Log("Checkpoint guardado en: " + pos);
    }

    public void RespawnPlayer()
    {
        player.transform.position = checkpointPosition;
    }
}
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Game Events")]
public class GameEvents : ScriptableObject
{
    public UnityEvent<int> OnLivesChanged;
    public UnityEvent OnGameOver;
    public UnityEvent OnWin;
    public UnityEvent<int> OnFruitCollected;
}
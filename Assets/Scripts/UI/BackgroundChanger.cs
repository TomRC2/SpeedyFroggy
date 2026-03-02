using UnityEngine;

public class BackgroundChanger : MonoBehaviour
{
    [SerializeField] private SpriteRenderer backgroundRenderer;
    [SerializeField] private Sprite bossBackground;

    public void ChangeBackground()
    {
        backgroundRenderer.sprite = bossBackground;
    }
}

using UnityEngine;

public class PowerUpDoubleJump : MonoBehaviour, IPowerUp
{
    public void Apply(Player player)
    {
        player.EnableDoubleJump(true);
        Destroy(gameObject);
    }
}

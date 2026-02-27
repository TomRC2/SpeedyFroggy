using UnityEngine;

public class PowerUpDash : MonoBehaviour, IPowerUp
{
    public void Apply(Player player)
    {
        player.EnableDash(true);
        Destroy(gameObject);
    }
}

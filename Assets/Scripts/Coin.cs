using UnityEngine;

public class Coin : MonoBehaviour, ICollectible
{
    public int value = 10;

    public void Collect()
    {
        Debug.Log("Moneda recogida, +" + value);
        Destroy(gameObject);
    }
}

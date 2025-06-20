using UnityEngine;

public interface IDamageable
{
    void TakeDamage(int amount);
}
public interface ICollectible
{
    void Collect();
}
public interface IPowerUp
{
    void Apply(Player player);
}
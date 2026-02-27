using UnityEngine;
using UnityEngine.UI;

public class FruitCounterUI : MonoBehaviour
{
    [SerializeField] private GameEvents gameEvents;
    [SerializeField] private Text fruitText;

    private int totalFruits = 0;

    private void OnEnable()
    {
        gameEvents.OnFruitCollected.AddListener(AddFruit);
    }

    private void OnDisable()
    {
        gameEvents.OnFruitCollected.RemoveListener(AddFruit);
    }

    private void AddFruit(int amount)
    {
        totalFruits += amount;
        fruitText.text = totalFruits.ToString();
    }
}
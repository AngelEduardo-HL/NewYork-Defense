using UnityEngine;
using TMPro;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;
    public int points = 10;
    public TMP_Text pointsText;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        UpdateUI();
    }

    public bool Spend(int cost)
    {
        Debug.Log($"Gastar: Tienes {points}, Cuesta {cost}");
        if (points >= cost)
        {
            points -= cost;
            UpdateUI();
            Debug.Log($"Nuevas Monedas: = {points}");
            return true;
        }
        Debug.Log("No hay suficientes Monedas");
        return false;
    }

    public void Add(int amount)
    {
        points += amount;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (pointsText != null)
            pointsText.text = $"Monedas: {points}";
    }
}

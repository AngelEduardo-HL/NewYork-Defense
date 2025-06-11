using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Configuración de vida/puntos")]
    public int maxHealth = 1;
    private int currentHealth;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        SoundManager.Instance?.PlayRatDie();


        //Calcula los puntos según el tipo de rata
        int pts = maxHealth * 10;

        //Sumar al score
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.AddScore(pts);

        //Sumar a currency
        if (CurrencyManager.Instance != null)
            CurrencyManager.Instance.Add(pts);

        //Notificar kill y despawnear
        var mgr = FindObjectOfType<EnemyManager>();
        mgr?.ReportKill();
        mgr?.DespawnEnemy(gameObject);
    }
}

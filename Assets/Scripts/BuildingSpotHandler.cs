using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BuildingSpotHandler : MonoBehaviour
{
    public GameObject towerPrefab;   // Prefab de la torre
    public int towerCost = 10;

    void OnMouseDown()
    {
        // Gastar puntos antes de colocar
        if (!CurrencyManager.Instance.Spend(towerCost))
            return;

        // Instanciar torre
        Instantiate(towerPrefab, transform.position, transform.rotation, transform.parent);
        SoundManager.Instance?.PlayPlacement();
        Destroy(gameObject);
    }
}

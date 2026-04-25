using UnityEngine;

public class AmmoScript : MonoBehaviour
{
    public Transform magazineSpawnPoint;
    private GameObject currentMagazine;

    void Start()
    {
        var weapons = FindObjectsOfType<AsRifleShoot>();

        foreach (var weapon in weapons)
        {
            weapon.OnWeaponGrabbed += OnWeaponGrabbed;
            weapon.OnWeaponReleased += OnWeaponReleased;
        }
    }

    void OnWeaponGrabbed(AsRifleShoot weapon)
    {
        if (currentMagazine != null)
            Destroy(currentMagazine);

        var visual = weapon.GetComponent<WeaponVisualConfig>();

        if (visual == null)
        {
            Debug.LogWarning("[AmmoScript] Нет WeaponVisualConfig!");
            return;
        }

        if (visual.magazineDisplayPrefab != null)
        {
            currentMagazine = Instantiate(
                visual.magazineDisplayPrefab,
                magazineSpawnPoint.position,
                magazineSpawnPoint.rotation,
                magazineSpawnPoint
            );
        }
    }

    void OnWeaponReleased(AsRifleShoot weapon)
    {
        if (currentMagazine != null)
            Destroy(currentMagazine);
    }
}
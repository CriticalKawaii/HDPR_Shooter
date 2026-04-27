using UnityEngine;

[CreateAssetMenu(fileName = "WeaponConfig", menuName = "Weapons/Config")]
public class WeaponConfig : ScriptableObject
{
    [Header("Identity")]
    public string weaponName;
    public WeaponType weaponType;

    [Header("Firing")]
    public FireMode fireMode = FireMode.Auto;
    public float fireRate = 10f;
    public float range = 100f;
    public int magazineCapacity = 30;

    [Header("Recoil")]
    public float recoilBack = 0.06f;
    public float recoilUp = 0.025f;
    public float recoilReturnSpeed = 10f;
    public float recoilKickSpeed = 20f;

    [Header("Bolt")]
    public bool hasBolt = true;
    public Vector3 boltClosedLocalPosition;//set per-weapon

    [Header("Casing")]
    public Vector3 casingEjectDirection = new Vector3(-1f, 1f, 0f);
    public float casingForce = 10f;
    public float casingLifetime = 60f;
}

public enum WeaponType { Pistol, Rifle, SMG, Shotgun, Sniper, MG }
public enum FireMode { Single, Auto, Burst }

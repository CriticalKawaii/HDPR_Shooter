using UnityEngine;

public class BarrelExplode : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private AudioSource audio;
    public GameObject onShotEffect;
    public GameObject barrel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    public void Explode()
    {
        Instantiate(onShotEffect, barrel.transform.position, barrel.transform.rotation);
        Destroy(barrel);
    }
}

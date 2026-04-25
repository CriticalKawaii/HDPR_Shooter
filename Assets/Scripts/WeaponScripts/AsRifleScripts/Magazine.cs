using UnityEngine;

public class Magazine : MonoBehaviour
{
    public GameObject Bullets;
    public Rigidbody mag;
    public int Ammo = 30;

    void Update()
    {
        if (Ammo <= 0 && Bullets != null)
        {
            Destroy(Bullets);
        }
    }

    public void AttachFunc()
    {
        if (mag != null)
            mag.isKinematic = true;
    }

    public void ReleaseFunc()
    {
        if (mag != null)
            mag.isKinematic = false;
    }
}
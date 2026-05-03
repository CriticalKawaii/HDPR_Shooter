using UnityEngine;

public class TargetGoing : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 2f;

    [Header("Destroy")]
    [SerializeField] private float destroyZ;

    private void Update()
    {
        transform.localPosition += Vector3.back * speed * Time.deltaTime;

        if (transform.localPosition.z <= destroyZ)
        {
            Destroy(gameObject);
        }
    }
}
using UnityEngine;

public class TargetSliding : MonoBehaviour
{
    public float minX = -2.9f;
    public float maxX = 1.4f;
    public float speed = 1f;

    private int direction = 1;

    void Update()
    {
        Vector3 pos = transform.localPosition;

        pos.x += direction * speed * Time.deltaTime;

        if (pos.x >= maxX)
        {
            pos.x = maxX;
            direction = -1;
        }
        else if (pos.x <= minX)
        {
            pos.x = minX;
            direction = 1;
        }

        transform.localPosition = pos;
    }

}
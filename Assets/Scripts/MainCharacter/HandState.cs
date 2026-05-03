using UnityEngine;

public class HandState : MonoBehaviour
{
    private Vector3 startLocalPosition;
    private Quaternion startLocalRotation;
    private Vector3 startLocalScale;

    void Start()
    {
        // Запоминаем начальные локальные координаты
        startLocalPosition = transform.localPosition;
        startLocalRotation = transform.localRotation;
        startLocalScale = transform.localScale;
    }

    void Update()
    {
        // Принудительно возвращаем объект в начальное локальное состояние
        transform.localPosition = startLocalPosition;
        transform.localRotation = startLocalRotation;
        transform.localScale = startLocalScale;
    }
}
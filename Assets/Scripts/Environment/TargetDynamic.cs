using System;
using UnityEditor.XR.OpenXR.Features;
using UnityEngine;

public class TargetDynamic : MonoBehaviour
{
    [SerializeField] private BoxCollider headCollider;
    [SerializeField] private BoxCollider bodyCollider;
    [SerializeField] private GameObject self;
    float duration = 0.2f;
    float time = 0f;
    bool isRotating = false;

    Quaternion startRot;
    Quaternion targetRot;
    public void TorsoHeadHit()
    {
        GoingDown();
        Debug.Log("Попадание!");
    }
    public void TorsoBodyHit()
    {
        GoingDown();
        Debug.Log("Вижу повреждение!");
        
    }
    private void GoingDown()
    {
        startRot = transform.localRotation;
        targetRot = Quaternion.Euler(-85f, 180f, 0f);

        time = 0f;
        isRotating = true;
        headCollider.enabled = false;
        bodyCollider.enabled = false;
        Invoke(nameof(WakeUp), 5f);
    }
    private void WakeUp()
    {
        startRot = transform.localRotation;
        targetRot = Quaternion.Euler(0f, 180f, 0f);

        time = 0f;
        isRotating = true;
        headCollider.enabled = true;
        bodyCollider.enabled = true;
    }
    void FixedUpdate()
    {
        if (isRotating)
        {
            time += Time.deltaTime;
            float t = time / duration;

            transform.localRotation = Quaternion.Lerp(startRot, targetRot, t);

            if (t >= 1f)
            {
                transform.localRotation = targetRot;
                isRotating = false;
            }
        }
    }
}

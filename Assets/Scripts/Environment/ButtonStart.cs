using System.Collections;
using UnityEngine;

public class ButtonStart : MonoBehaviour
{
    [Header("Button")]
    [SerializeField] private Transform target;

    [SerializeField] private float pressedLocalY = -0.04f;
    [SerializeField] private float releasedLocalY = 0f;

    [SerializeField] private float duration = 0.2f;

    [Header("References")]
    [SerializeField] private TargetSpawning TS;
    [SerializeField] private Collider triggerCollider;

    private Coroutine moveCoroutine;
    private bool isPressed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isPressed)
            return;

        if (other.CompareTag("LeftHand") || other.CompareTag("RightHand"))
        {
            PressButton();
        }
    }

    private void PressButton()
    {
        isPressed = true;

        if (triggerCollider != null)
            triggerCollider.enabled = false;

        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        moveCoroutine = StartCoroutine(
            MoveRoutine(pressedLocalY, true)
        );
    }

    public void ResetButton()
    {
        isPressed = false;

        if (triggerCollider != null)
            triggerCollider.enabled = true;

        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        moveCoroutine = StartCoroutine(
            MoveRoutine(releasedLocalY, false)
        );
    }

    private IEnumerator MoveRoutine(float targetY, bool startSpawner)
    {
        Vector3 startPos = target.localPosition;

        Vector3 endPos = new Vector3(
            startPos.x,
            targetY,
            startPos.z
        );

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            float t = Mathf.SmoothStep(
                0f,
                1f,
                time / duration
            );

            target.localPosition = Vector3.Lerp(
                startPos,
                endPos,
                t
            );

            yield return null;
        }

        target.localPosition = endPos;

        if (startSpawner)
        {
            TS.Starter();
        }
    }
}
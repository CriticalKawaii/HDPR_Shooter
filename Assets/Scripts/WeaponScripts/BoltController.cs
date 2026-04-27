using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class BoltController : MonoBehaviour
{
    [Header("References")]
    public WeaponController weapon;
    public ConfigurableJoint joint;
    public Transform boltTransform;
    public Transform referenceTransform;   // parent or weapon root
    public Animator weaponAnimator;

    [Header("Closed position (local to referenceTransform)")]
    public Vector3 closedLocalPosition;    // set per-weapon — replaces the hardcoded value

    [Header("Threshold for 'is closed'")]
    public float closedThreshold = 0.005f;

    private float _savedLocalX, _savedLocalY, _savedLocalZ;
    private bool _grabbed;
    private bool _returning;

    void Start()
    {
        var local = referenceTransform.InverseTransformPoint(transform.position);
        _savedLocalX = local.x;
        _savedLocalY = local.y;
        _savedLocalZ = local.z;

        var interactable = GetComponent<XRSimpleInteractable>();
        if (interactable != null)
        {
            interactable.selectEntered.AddListener(OnGrab);
            interactable.selectExited.AddListener(OnRelease);
        }
    }

    void LateUpdate()
    {
        var local = referenceTransform.InverseTransformPoint(transform.position);

        // Lock axes we don't want drifting
        local.x = _savedLocalX;
        local.y = _savedLocalY;

        // Only lock Z when bolt is not being pulled
        if (!_grabbed)
            local.z = _savedLocalZ;

        transform.position = referenceTransform.TransformPoint(local);
    }

    void Update()
    {
        if (!_returning) return;

        var local = referenceTransform.InverseTransformPoint(transform.position);
        float distToClosed = Mathf.Abs(local.z - closedLocalPosition.z);

        if (distToClosed <= closedThreshold)
        {
            // Snap to exact closed position
            boltTransform.localPosition = closedLocalPosition;
            _returning = false;
            _grabbed = false;

            if (weaponAnimator != null) weaponAnimator.enabled = true;
            weapon?.NotifyBoltClosed();
        }
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        _grabbed = true;
        _returning = false;
        if (weaponAnimator != null) weaponAnimator.enabled = false;
        weapon?.NotifyBoltOpened();
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        _returning = true;
    }
}
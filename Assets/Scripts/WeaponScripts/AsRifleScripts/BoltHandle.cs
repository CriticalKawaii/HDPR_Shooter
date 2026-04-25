using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BoltHandle : MonoBehaviour
{
    public Rigidbody boltRb;

    private FixedJoint handJoint;

    void Awake()
    {
        var interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
        interactable.selectEntered.AddListener(OnGrab);
        interactable.selectExited.AddListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        var interactorTransform = args.interactorObject.transform;

        handJoint = boltRb.gameObject.AddComponent<FixedJoint>();
        handJoint.connectedBody = interactorTransform.GetComponentInParent<Rigidbody>();
        handJoint.breakForce = Mathf.Infinity;
        handJoint.breakTorque = Mathf.Infinity;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        if (handJoint != null)
            Destroy(handJoint);
    }
}
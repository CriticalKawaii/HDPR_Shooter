using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
public class XRSocketInteractorTag : XRSocketInteractor
{
    [SerializeField] private string targetTag;
    public override bool CanSelect(IXRSelectInteractable interactable)
    {
        MonoBehaviour monoBehaviour = interactable as MonoBehaviour;
        if(monoBehaviour.CompareTag(targetTag)) showInteractableHoverMeshes = true;
        else showInteractableHoverMeshes = false;
        return base.CanSelect(interactable) && monoBehaviour.CompareTag(targetTag);
    }
}

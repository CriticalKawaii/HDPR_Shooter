using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class TriggerInput : MonoBehaviour
{
    public InputActionReference triggerAction;
    private WeaponController _weapon;
    private XRGrabInteractable _grab;

    void Awake()
    {
        _weapon = GetComponent<WeaponController>();
        _grab = GetComponent<XRGrabInteractable>();
    }

    void OnEnable()
    {
        triggerAction.action.performed += OnTriggerPerformed;
        triggerAction.action.canceled += OnTriggerCanceled;
        triggerAction.action.Enable();
    }

    void OnDisable()
    {
        triggerAction.action.performed -= OnTriggerPerformed;
        triggerAction.action.canceled -= OnTriggerCanceled;
    }

    private void OnTriggerPerformed(InputAction.CallbackContext _)
    {
        // Only fire when the weapon is actually held
        if (_grab.isSelected) _weapon.TriggerPress();
    }

    private void OnTriggerCanceled(InputAction.CallbackContext _)
    {
        _weapon.TriggerRelease();
    }
}

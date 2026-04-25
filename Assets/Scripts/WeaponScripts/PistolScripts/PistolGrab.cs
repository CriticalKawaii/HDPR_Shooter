using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class PistolGrab : MonoBehaviour
{
    [Header("Player Hand Models (hide/show)")]
    [SerializeField] private GameObject leftHandModel;
    [SerializeField] private GameObject rightHandModel;

    [Header("Weapon Hand Models (show/hide)")]
    [SerializeField] private GameObject leftHandOnWeapon;
    [SerializeField] private GameObject rightHandOnWeapon;
    //[SerializeField] private GameObject leftSecondHand;
    //[SerializeField] private GameObject rightSecondHand;

    [Header("Debug")]
    [SerializeField] private bool enableDebug = true;

    private XRGrabInteractable grab;

    private void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();

        if (enableDebug)
            Debug.Log("[AsRifleGrab] Awake: XRGrabInteractable = " + (grab != null));
    }

    private void Start()
    {
        // На старте руки на оружии скрыты
        if (leftHandOnWeapon != null) leftHandOnWeapon.SetActive(false);
        if (rightHandOnWeapon != null) rightHandOnWeapon.SetActive(false);
    }

    private void OnEnable()
    {
        if (grab == null) return;

        grab.selectEntered.AddListener(OnGrab);
        grab.selectExited.AddListener(OnRelease);

        if (enableDebug)
            Debug.Log("[AsRifleGrab] Subscribed to grab events");
    }

    private void OnDisable()
    {
        if (grab == null) return;

        grab.selectEntered.RemoveListener(OnGrab);
        grab.selectExited.RemoveListener(OnRelease);

        if (enableDebug)
            Debug.Log("[AsRifleGrab] Unsubscribed from grab events");
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        if (enableDebug)
            Debug.Log("[AsRifleGrab] OnGrab by: " + args.interactorObject.transform.name);

        HandleHand(args.interactorObject, false);
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        if (enableDebug)
            Debug.Log("[AsRifleGrab] OnRelease by: " + args.interactorObject.transform.name);

        HandleHand(args.interactorObject, true);
    }

    private void HandleHand(IXRInteractor interactor, bool showPlayerHand)
    {
        if (interactor == null)
        {
            if (enableDebug)
                Debug.LogWarning("[AsRifleGrab] Interactor NULL");
            return;
        }

        var t = interactor.transform;

        if (enableDebug)
        {
            Debug.Log("[AsRifleGrab] Interactor: " + t.name);
            Debug.Log("[AsRifleGrab] Tag: " + t.tag);
        }

        // ЛЕВАЯ РУКА
        if (t.CompareTag("LeftHand"))
        {
            if (rightHandOnWeapon.activeSelf)
            {
                leftHandModel.SetActive(showPlayerHand);
            }
            else
            {
                if (leftHandModel != null)
                    leftHandModel.SetActive(showPlayerHand);

                if (leftHandOnWeapon != null)
                    leftHandOnWeapon.SetActive(!showPlayerHand);

                if (enableDebug)
                    Debug.Log("[AsRifleGrab] LEFT: playerHand=" + showPlayerHand +
                              ", weaponHand=" + (!showPlayerHand));
            }
            
        }
        // ПРАВАЯ РУКА
        else if (t.CompareTag("RightHand"))
        {
            if (rightHandModel != null)
                rightHandModel.SetActive(showPlayerHand);

            if (rightHandOnWeapon != null)
                rightHandOnWeapon.SetActive(!showPlayerHand);

            if (enableDebug)
                Debug.Log("[AsRifleGrab] RIGHT: playerHand=" + showPlayerHand +
                          ", weaponHand=" + (!showPlayerHand));
        }
        else
        {
            if (enableDebug)
                Debug.LogWarning("[AsRifleGrab] Unknown tag: " + t.tag);
        }
    }
}
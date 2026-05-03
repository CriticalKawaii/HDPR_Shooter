using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class PistolBoltMechanics : MonoBehaviour
{
    [Header("Player Hand Models (hide/show)")]
    [SerializeField] private GameObject leftHandModel;
    [SerializeField] private GameObject rightHandModel;

    [Header("Weapon Hand Models (show/hide)")]
    [SerializeField] private GameObject leftHandOnWeapon;
    [SerializeField] private GameObject rightHandOnWeapon;

    [Header("Debug")]
    [SerializeField] private bool enableDebug = true;

    private XRGrabInteractable grab;
    public ConfigurableJoint pistolJoint;
    public Transform pistolbolttrans;
    public Rigidbody pistolrb;
    public Rigidbody rbPistol;
    public Transform referencePistol;
    public PistolShoot Pistol;

    public Animator GunAnim;

    private float savedLocalY;
    private float savedLocalX;
    private float savedLocalZ;
    [SerializeField] private bool isFreezed = true;
    [Header("Recoil")]
    private bool Based = true;
    private bool Release = true;
    JointDrive currentXDrive;
    void Start()
    {
        currentXDrive = pistolJoint.xDrive;
        Vector3 localPos = referencePistol.InverseTransformPoint(transform.position);

        savedLocalY = localPos.y;
        savedLocalX = localPos.x;
        savedLocalZ = localPos.z;
    }
    void LateUpdate()
    {
        Vector3 currentLocal = referencePistol.InverseTransformPoint(transform.position);
        if(isFreezed && !GunAnim.GetCurrentAnimatorStateInfo(0).IsName("Shoot")){
            currentLocal.z = savedLocalZ;
        }
        currentLocal.y = savedLocalY;
        currentLocal.x = savedLocalX;

        transform.position = referencePistol.TransformPoint(currentLocal);
    }

    void Update()
    {
        if(GunAnim.GetCurrentAnimatorStateInfo(0).IsName("Shoot") && currentXDrive.positionSpring == 2000f){
            currentXDrive.positionSpring = 0f;
        }
        else if(!GunAnim.GetCurrentAnimatorStateInfo(0).IsName("Shoot") && currentXDrive.positionSpring == 0f){
            currentXDrive.positionSpring = 2000f;
        }
        if(Release && !Based){
            Vector3 currentLocal = referencePistol.InverseTransformPoint(transform.position);
            Debug.Log(currentLocal.z);
            if(currentLocal.z <= -0.125f && currentLocal.z >=  -0.135f)
            {
                Debug.Log("Closed!");
                pistolbolttrans.localPosition = new Vector3(0.05256491f, 0.01729099f, 0);
                Release = false;
                Based = true;
                isFreezed = true;
                GunAnim.enabled = true;
                Pistol.TryToReload();

            }
        }
    }
    public void GrabbedPistol(SelectEnterEventArgs args)
    {
        Release = false;
        Based = false;
        isFreezed = false;
        GunAnim.enabled = false;
        HandleHand(args.interactorObject, false);
    }
    public void ReleasedPistol(SelectExitEventArgs args)
    {
        Release = true;
        HandleHand(args.interactorObject, true);
    }
    public bool IsClosedPistol()
    {
        return Based;
    }

    public void ApplyRecoil()
    {
        return;
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
            if (leftHandOnWeapon.activeSelf)
            {
                rightHandModel.SetActive(showPlayerHand);
            }
            else
            {
                if (rightHandModel != null)
                    rightHandModel.SetActive(showPlayerHand);

                if (rightHandOnWeapon != null)
                    rightHandOnWeapon.SetActive(!showPlayerHand);

                if (enableDebug)
                    Debug.Log("[AsRifleGrab] RIGHT: playerHand=" + showPlayerHand +
                              ", weaponHand=" + (!showPlayerHand));
            }
        }
        else
        {
            if (enableDebug)
                Debug.LogWarning("[AsRifleGrab] Unknown tag: " + t.tag);
        }
    }
}
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class BoltMechanics : MonoBehaviour
{

    private XRGrabInteractable grab;
    public ConfigurableJoint cfgjoint;
    public Transform trans;
    public Rigidbody rb;
    public Rigidbody rbRifle;
    public Transform reference;
    public AsRifleShoot Asrfl;

    public Animator GunAnim;

    private float savedLocalY;
    private float savedLocalX;
    private float savedLocalZ;
    private bool isFreezed = true;
    [Header("Recoil")]
    public float recoilForce = 25f;
    private bool Based = true;
    private bool Release = true;
    JointDrive currentXDrive;
    void Start()
    {
        JointDrive currentXDrive = cfgjoint.xDrive;
        Vector3 localPos = reference.InverseTransformPoint(transform.position);

        savedLocalY = localPos.y;
        savedLocalX = localPos.x;
        savedLocalZ = localPos.z;
    }
    void LateUpdate()
    {
        Vector3 currentLocal = reference.InverseTransformPoint(transform.position);
        if(isFreezed && !GunAnim.GetCurrentAnimatorStateInfo(0).IsName("Shoot")){
            currentLocal.z = savedLocalZ;
        }
        currentLocal.y = savedLocalY;
        currentLocal.x = savedLocalX;

        transform.position = reference.TransformPoint(currentLocal);
    }

    void Update(){
        if(GunAnim.GetCurrentAnimatorStateInfo(0).IsName("Shoot") && currentXDrive.positionSpring == 20000f){
            currentXDrive.positionSpring = 0f;
        }
        else if(!GunAnim.GetCurrentAnimatorStateInfo(0).IsName("Shoot") && currentXDrive.positionSpring == 0f){
            currentXDrive.positionSpring = 20000f;
        }
        if(Release && !Based){
            Vector3 currentLocal = reference.InverseTransformPoint(transform.position);
            Debug.Log(currentLocal.z);
            if(currentLocal.z <= 0.056f && currentLocal.z >= -0.054f){
                Debug.Log("Closed!");
                trans.localPosition = new Vector3(0.05256491f, 0.01729099f, 0);
                Release = false;
                Based = true;
                isFreezed = true;
                GunAnim.enabled = true;
                Asrfl.TryToReload();

            }
        }
    }
    public void Grabbed()
    {
        Release = false;
        Based = false;
        isFreezed = false;
        GunAnim.enabled = false;
    }
    public void Released()
    {
        Release = true; 
    }
    public bool IsClosed()
    {
        return Based;
    }

    public void ApplyRecoil()
    {
        return;
    }
}
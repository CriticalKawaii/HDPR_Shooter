using UnityEngine;

public class PistolBoltMechanics : MonoBehaviour
{
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
        JointDrive currentXDrive = pistolJoint.xDrive;
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

    void Update(){
        if(GunAnim.GetCurrentAnimatorStateInfo(0).IsName("Shoot") && currentXDrive.positionSpring == 20000f){
            currentXDrive.positionSpring = 0f;
        }
        else if(!GunAnim.GetCurrentAnimatorStateInfo(0).IsName("Shoot") && currentXDrive.positionSpring == 0f){
            currentXDrive.positionSpring = 20000f;
        }
        if(Release && !Based){
            Vector3 currentLocal = referencePistol.InverseTransformPoint(transform.position);
            Debug.Log(currentLocal.z);
            if(currentLocal.z <= 0.056f && currentLocal.z >= -0.054f){
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
    public void GrabbedPistol(){
        Release = false;
        Based = false;
        isFreezed = false;
        GunAnim.enabled = false;
    }
    public void ReleasedPistol(){
        Release = true;
    }
    public bool IsClosedPistol()
    {
        return Based;
    }

    public void ApplyRecoil()
    {
        return;
    }
}
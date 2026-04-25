using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PistolShoot : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource Pistolbang;
    public AudioClip PistolbangClip;

    [Header("Animation")]
    public Animator Pistolanim;

    [Header("Weapon Settings")]
    public float PistolfireRate = 10f;
    public float Pistolrange = 100f;
    public GameObject PistolmagazinePrefab;
    public Rigidbody Pistol;
    public int PistolcurrentAmmo = 0;
    public bool ammoInPistol = false;
    

    [Header("Effects")]
    public GameObject PistolimpactEffect;
    public Transform PistolshootPoint;
    public Light PistolmuzzleLight;
    public HybridRecoil PistolrecoilScript;
    public PistolCasingSpawner Pistolcasspawn;

    [Header("References")]
    public Camera PistolplayerCamera;
    public PistolBoltMechanics Pistolbolt;

    private float PistolnextTimeToFire = 0f;
    private bool PistoltriggerPressed = false;

    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor Pistolsocket;
    private GameObject Pistolmagazin;
    private Magazine Pistolmagascript;
    private bool IsEnserted = false;

    public System.Action<PistolShoot> PistolOnWeaponGrabbed;
    public System.Action<PistolShoot> PistolOnWeaponReleased;
 
    public void Grab()
    {
        PistolOnWeaponGrabbed?.Invoke(this);
    }

    public void Release()
    {
        PistolOnWeaponReleased?.Invoke(this);
    }

    void Start()
    {
        if (PistolplayerCamera == null)
            PistolplayerCamera = Camera.main;
        Pistolsocket.selectEntered.AddListener(OnObjectInserted);
        Pistolsocket.selectExited.AddListener(OnObjectRemoved);
    }

    private void OnObjectInserted(SelectEnterEventArgs args)
    {
        // Получаем объект, который вставили
        Pistolmagazin = args.interactableObject.transform.gameObject;
        Debug.Log($"Вставлен объект: {Pistolmagazin.name}");
        
        // Можно получить компоненты
        Pistolmagascript = Pistolmagazin.GetComponent<Magazine>();
        IsEnserted = true;
        PistolcurrentAmmo = Pistolmagascript.Ammo;
    }

    private void OnObjectRemoved(SelectExitEventArgs args)
    {
        Debug.Log("Объект извлечен");
        Pistolmagazin = null;
        IsEnserted = false;
        PistolcurrentAmmo = 0;
    }


    void Update()
    {
        if (PistoltriggerPressed && Time.time >= PistolnextTimeToFire)
        {
            TryShoot();
            PistolnextTimeToFire = Time.time + 1f / PistolfireRate;
        }
    }

    public void Shoot()
    {
        PistoltriggerPressed = true;
    }

    public void Shootoff()
    {
        PistoltriggerPressed = false;

        if (Pistolanim != null)
        {
            Pistolanim.Play("Idle");
        }
    }
    public void TryToReload(){
        if(!ammoInPistol){
            if(PistolcurrentAmmo > 0){
                PistolcurrentAmmo-=1;
                ammoInPistol = true;
                Pistolmagascript.Ammo-=1;
            }
        }
        
    }
    private void TryShoot()
    {
        if (Pistolbolt == null)
        {
            Debug.LogError("BoltMechanics не назначен!");
            return;
        }

        if (!Pistolbolt.IsClosedPistol())
            return;

        PerformShoot();

    }

    private void PerformShoot()
    {
        if(!ammoInPistol){
            Pistolanim.Play("ShootEmpty");
            return;
        }
        if (Pistolanim != null)
        {
            Pistolanim.Play("Shoot");
        }

        if (Pistolbang != null && PistolbangClip != null)
        {
            Pistolbang.PlayOneShot(PistolbangClip);
        }

        if (PistolshootPoint == null)
        {
            Debug.LogError("Shoot Point не назначен!");
            return;
        }
        //PistolrecoilScript.Fire();
        StartCoroutine(SpawnWithDelay());
        Vector3 rayOrigin = PistolshootPoint.position;
        Vector3 rayDirection = PistolshootPoint.forward;
        rayDirection += new Vector3(
            Random.Range(-0.01f, 0.01f),
            Random.Range(-0.01f, 0.01f),
            0);

        rayDirection.Normalize();

        RaycastHit hit;

        if (Physics.Raycast(rayOrigin, rayDirection, out hit, Pistolrange))
        {
            if (PistolimpactEffect != null)
            {
                GameObject impact = Instantiate(PistolimpactEffect, hit.point, Quaternion.identity);

                impact.transform.rotation = Quaternion.LookRotation(hit.normal);
                impact.transform.rotation *= Quaternion.Euler(0, 180, 0);
                impact.transform.position += hit.normal * 0.01f;
                impact.transform.RotateAround(hit.point, hit.normal, Random.Range(0, 360));

                Destroy(impact, 5f);
            }
        }
        if(PistolcurrentAmmo > 0){
            PistolcurrentAmmo-=1;
  //          Pistolmagascript.Ammo = PistolcurrentAmmo;
        }
        else{
            ammoInPistol = false;
        }
        StartCoroutine(FlashLight());
    }
    

    private System.Collections.IEnumerator SpawnWithDelay()
    {
        yield return new WaitForSeconds(0f);
        Pistolcasspawn.PistolCasSpawn(5f);
    }
    private System.Collections.IEnumerator FlashLight()
    {
        if (PistolmuzzleLight != null)
        {
            PistolmuzzleLight.enabled = true;
            yield return new WaitForSeconds(0.05f);
            PistolmuzzleLight.enabled = false;
        }
    }
}
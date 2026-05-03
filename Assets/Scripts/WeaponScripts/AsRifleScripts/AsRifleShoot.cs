using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class AsRifleShoot : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource bang;
    public AudioClip bangClip;

    [Header("Animation")]
    public Animator gunanim;

    [Header("Weapon Settings")]
    public float fireRate = 10f;
    public float range = 100f;
    public int currentAmmo = 0;
    public bool ammoInRifle = false;

    [Header("Effects")]
    public GameObject impactEffect;
    public Transform shootPoint;
    public Light muzzleLight;
    public CasingSpawner casspawn;

    [Header("Recoil (VR Correct)")]
    public Transform recoilRoot;
    public float recoilBackDistance = 0.06f;
    public float recoilUpDistance = 0.025f;
    public float recoilReturnSpeed = 10f;
    public float recoilKickSpeed = 20f;

    private Vector3 currentRecoil;
    private Vector3 targetRecoil;
    private Vector3 initialLocalPosition; // 🔥 ФИКС

    [Header("Physics (optional)")]
    public Rigidbody weaponRb;
    public float recoilTorque = 3f;

    [Header("References")]
    public Camera playerCamera;
    public BoltMechanics bolt;
    public XRSocketInteractor socket;
    public XRGrabInteractable grabInteractable;

    [Header("Debug")]
    public bool debug = true;

    private float nextTimeToFire = 0f;
    private bool triggerPressed = false;

    private GameObject magazine;
    private Magazine magazineScript;

    public System.Action<AsRifleShoot> OnWeaponGrabbed;
    public System.Action<AsRifleShoot> OnWeaponReleased;

    void Awake()
    {
        if (grabInteractable == null)
            grabInteractable = GetComponent<XRGrabInteractable>();

        if (weaponRb == null)
            weaponRb = GetComponent<Rigidbody>();

        if (recoilRoot == null)
            recoilRoot = transform;
    }

    void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        // 🔥 СОХРАНЯЕМ ИСХОДНУЮ ПОЗИЦИЮ (ФИКС ТЕЛЕПОРТА)
        if (recoilRoot != null)
            initialLocalPosition = recoilRoot.localPosition;

        if (socket != null)
        {
            socket.selectEntered.AddListener(OnObjectInserted);
            socket.selectExited.AddListener(OnObjectRemoved);
        }

        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(_ => Grab());
            grabInteractable.selectExited.AddListener(_ => Release());
        }
    }

    void Update()
    {
        UpdateRecoil();

        if (triggerPressed && Time.time >= nextTimeToFire)
        {
            TryShoot();
            nextTimeToFire = Time.time + 1f / fireRate;
        }
    }

    // ===================== RECOIL =====================

    private void UpdateRecoil()
    {
        targetRecoil = Vector3.Lerp(targetRecoil, Vector3.zero, Time.deltaTime * recoilReturnSpeed);
        currentRecoil = Vector3.Lerp(currentRecoil, targetRecoil, Time.deltaTime * recoilKickSpeed);

        if (recoilRoot != null)
            recoilRoot.localPosition = initialLocalPosition + currentRecoil; // 🔥 ФИКС
    }

    private void ApplyRecoil()
    {
        // Локальный вектор отдачи (относительно оружия/камеры)
        Vector3 localRecoil = new Vector3(0, recoilUpDistance, -recoilBackDistance);

        // Преобразуем локальный вектор в мировые координаты с учетом поворота оружия
        Vector3 worldRecoil = transform.TransformDirection(localRecoil);

        targetRecoil += worldRecoil;

        if (weaponRb != null)
        {
            // Торк тоже нужно применять локально
            Vector3 localTorque = new Vector3(-recoilTorque, 0, 0);
            Vector3 worldTorque = transform.TransformDirection(localTorque);
            weaponRb.AddTorque(worldTorque, ForceMode.Impulse);
        }
    }

    // ===================== GRAB =====================

    public void Grab()
    {
        if (debug) Debug.Log("[Weapon] Grab");
        OnWeaponGrabbed?.Invoke(this);
    }

    public void Release()
    {
        if (debug) Debug.Log("[Weapon] Release");
        OnWeaponReleased?.Invoke(this);
    }

    // ===================== MAGAZINE =====================

    private void OnObjectInserted(SelectEnterEventArgs args)
    {
        magazine = args.interactableObject.transform.gameObject;
        magazineScript = magazine.GetComponent<Magazine>();

        if (magazineScript != null)
        {
            currentAmmo = magazineScript.Ammo;
            if (debug) Debug.Log("[Weapon] Magazine inserted: " + currentAmmo);
        }
    }

    private void OnObjectRemoved(SelectExitEventArgs args)
    {
        if (debug) Debug.Log("[Weapon] Magazine removed");

        magazine = null;
        magazineScript = null;
        currentAmmo = 0;
    }

    // ===================== SHOOT =====================

    public void Shoot() => triggerPressed = true;

    public void Shootoff()
    {
        triggerPressed = false;
        gunanim?.Play("Idle");
    }

    public void TryToReload()
    {
        if (!ammoInRifle && currentAmmo > 0 && magazineScript != null)
        {
            currentAmmo--;
            magazineScript.Ammo--;
            ammoInRifle = true;
        }
    }

    private void TryShoot()
    {
        if (bolt == null || !bolt.IsClosed())
            return;

        PerformShoot();
    }

    private void PerformShoot()
    {
        if (!ammoInRifle)
        {
            gunanim?.Play("ShootEmpty");
            return;
        }

        gunanim?.Play("Shoot");
        bang?.PlayOneShot(bangClip);

        ApplyRecoil();

        StartCoroutine(SpawnWithDelay());

        if (shootPoint == null)
        {
            Debug.LogError("shootPoint NULL!");
            return;
        }

        Vector3 dir = shootPoint.forward + new Vector3(
            Random.Range(-0.01f, 0.01f),
            Random.Range(-0.01f, 0.01f),
            0f);

        if (Physics.Raycast(shootPoint.position, dir.normalized, out RaycastHit hit, range))
        {
            SpawnImpact(hit);
            Rigidbody rb = hit.rigidbody;
            if (hit.collider.CompareTag("TorsoTargetBody"))
            {
                TargetDynamic targetscript = hit.collider.GetComponent<TargetDynamic>();
                if (targetscript != null)
                    targetscript.TorsoBodyHit();
            }
            else if (hit.collider.CompareTag("TorsoTargetHead"))
            {
                TargetDynamic targetscript = hit.collider.GetComponentInParent<TargetDynamic>();
                if (targetscript != null)
                    targetscript.TorsoHeadHit();
            }
            else if (hit.collider.CompareTag("Barrel"))
            {
                BarrelExplode barrelExplode = hit.collider.GetComponent<BarrelExplode>();
                if (barrelExplode != null)
                {
                    barrelExplode.Explode();
                }
            }
            else if (rb != null)
            {
                float forceMagnitude = 10f;
                Vector3 forceDirection = dir;
                Vector3 force = forceDirection * forceMagnitude;
                rb.AddForceAtPosition(force, hit.point, ForceMode.Impulse);
            }
            
        }

        if (currentAmmo > 0)
        {
            currentAmmo--;
            if (magazineScript != null)
                magazineScript.Ammo = currentAmmo;
        }
        else
        {
            ammoInRifle = false;
        }

        StartCoroutine(FlashLight());
    }

    // ===================== IMPACT =====================

    private void SpawnImpact(RaycastHit hit)
    {
        if (impactEffect == null) return;

        // 1. Создаём объект в мире (без родителя)
        Vector3 position = hit.point + hit.normal * 0.01f;
        Quaternion rotation = Quaternion.LookRotation(hit.normal) * Quaternion.Euler(0, 180, 0);
        GameObject impact = Instantiate(impactEffect, position, rotation);

        // 2. Задаём мировой масштаб (не локальный, т.к. родителя ещё нет)
        impact.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);

        // 3. Прикрепляем к объекту, на который попали, НО сохраняем мировые трансформации (worldPositionStays = true)
        impact.transform.SetParent(hit.collider.transform, true);

        // 4. Удаляем через 5 секунд
        Destroy(impact, 5f);
    }

    // ===================== FX =====================

    private System.Collections.IEnumerator SpawnWithDelay()
    {
        yield return null;
        casspawn?.Spawn();
    }

    private System.Collections.IEnumerator FlashLight()
    {
        if (muzzleLight != null)
        {
            muzzleLight.enabled = true;
            yield return new WaitForSeconds(0.05f);
            muzzleLight.enabled = false;
        }
    }
}
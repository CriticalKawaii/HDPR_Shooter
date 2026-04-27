using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRGrabInteractable))]
public class WeaponController : MonoBehaviour
{
    [Header("Config")]
    public WeaponConfig config;

    [Header("References — assign in prefab")]
    public Transform shootPoint;
    public Transform recoilRoot;
    public XRSocketInteractor magazineSocket;
    public Animator weaponAnimator;
    public AudioSource audioSource;
    public AudioClip fireSound;
    public AudioClip emptySound;
    public Light muzzleLight;
    public GameObject impactEffectPrefab;
    public GameObject casingPrefab;
    public Transform casingEjectPoint;

    [Header("Hand Models")]
    public GameObject leftHandPlayer;
    public GameObject rightHandPlayer;
    public GameObject leftHandOnWeapon;
    public GameObject rightHandOnWeapon;
    public GameObject leftHandSecondary;   // for two-handed rifles
    public GameObject rightHandSecondary;

    // ── State ──────────────────────────────────────────────────────────────
    public int CurrentAmmo { get; private set; }
    public bool RoundChambered { get; private set; }
    public bool BoltClosed { get; private set; } = true;

    private bool _triggerHeld;
    private float _nextFireTime;
    private Magazine _magazine;
    private XRGrabInteractable _grab;
    private Vector3 _recoilInitialLocalPos;
    private Vector3 _currentRecoil;
    private Vector3 _targetRecoil;

    // ── Events ─────────────────────────────────────────────────────────────
    public event System.Action<WeaponController> OnGrabbed;
    public event System.Action<WeaponController> OnReleased;

    // ───────────────────────────────────────────────────────────────────────

    void Awake()
    {
        _grab = GetComponent<XRGrabInteractable>();
    }

    void Start()
    {
        if (recoilRoot != null)
            _recoilInitialLocalPos = recoilRoot.localPosition;

        // Hand models start hidden
        SetHandModels(leftHandOnWeapon, false);
        SetHandModels(rightHandOnWeapon, false);
        SetHandModels(leftHandSecondary, false);
        SetHandModels(rightHandSecondary, false);

        // Grab events
        _grab.selectEntered.AddListener(OnSelectEntered);
        _grab.selectExited.AddListener(OnSelectExited);

        // Magazine socket
        if (magazineSocket != null)
        {
            magazineSocket.selectEntered.AddListener(OnMagazineInserted);
            magazineSocket.selectExited.AddListener(OnMagazineRemoved);
        }
    }

    void Update()
    {
        UpdateRecoil();

        if (_triggerHeld && Time.time >= _nextFireTime)
        {
            TryFire();

            // For single/burst, stop after one shot
            if (config.fireMode == FireMode.Single)
                _triggerHeld = false;

            _nextFireTime = Time.time + 1f / config.fireRate;
        }
    }

    // ── Trigger input (called by XR Input Action or TriggerInput component) ─

    public void TriggerPress() => _triggerHeld = true;

    public void TriggerRelease()
    {
        _triggerHeld = false;
        weaponAnimator?.Play("Idle");
    }

    // ── Bolt (called by BoltController) ────────────────────────────────────

    public void NotifyBoltOpened() => BoltClosed = false;

    public void NotifyBoltClosed()
    {
        BoltClosed = true;
        ChamberRound();
    }

    public void ChamberRound()
    {
        if (!RoundChambered && CurrentAmmo > 0)
        {
            RoundChambered = true;
            CurrentAmmo--;
            if (_magazine != null) _magazine.Ammo = CurrentAmmo;
        }
    }

    // ── Firing ─────────────────────────────────────────────────────────────

    private void TryFire()
    {
        if (!BoltClosed) return;

        if (!RoundChambered)
        {
            PlaySound(emptySound);
            weaponAnimator?.Play("ShootEmpty");
            return;
        }

        Fire();
    }

    private void Fire()
    {
        RoundChambered = false;

        weaponAnimator?.Play("Shoot");
        PlaySound(fireSound);
        ApplyRecoil();
        StartCoroutine(MuzzleFlash());
        StartCoroutine(EjectCasing());
        CastBullet();

        // Auto-chamber next round if bolt stays closed (semi/auto)
        if (config.hasBolt)
            ChamberRound();
    }

    private void CastBullet()
    {
        if (shootPoint == null) return;

        Vector3 dir = shootPoint.forward + new Vector3(
            Random.Range(-0.01f, 0.01f),
            Random.Range(-0.01f, 0.01f), 0f);

        if (!Physics.Raycast(shootPoint.position, dir.normalized, out RaycastHit hit, config.range))
            return;

        SpawnImpact(hit);

        if (hit.rigidbody != null)
            hit.rigidbody.AddForceAtPosition(dir * 10f, hit.point, ForceMode.Impulse);

        if (hit.collider.CompareTag("Barrel"))
            hit.collider.GetComponent<BarrelExplode>()?.Explode();
    }

    private void SpawnImpact(RaycastHit hit)
    {
        if (impactEffectPrefab == null) return;
        var impact = Instantiate(impactEffectPrefab,
            hit.point + hit.normal * 0.01f,
            Quaternion.LookRotation(hit.normal) * Quaternion.Euler(0, 180, 0));
        impact.transform.localScale = Vector3.one * 0.05f;
        impact.transform.SetParent(hit.collider.transform, true);
        Destroy(impact, 5f);
    }

    // ── Recoil ─────────────────────────────────────────────────────────────

    private void ApplyRecoil()
    {
        _targetRecoil += new Vector3(0f, config.recoilUp, -config.recoilBack);
    }

    private void UpdateRecoil()
    {
        _targetRecoil = Vector3.Lerp(_targetRecoil, Vector3.zero,
            Time.deltaTime * config.recoilReturnSpeed);
        _currentRecoil = Vector3.Lerp(_currentRecoil, _targetRecoil,
            Time.deltaTime * config.recoilKickSpeed);

        if (recoilRoot != null)
            recoilRoot.localPosition = _recoilInitialLocalPos + _currentRecoil;
    }

    // ── Magazine ───────────────────────────────────────────────────────────

    private void OnMagazineInserted(SelectEnterEventArgs args)
    {
        _magazine = args.interactableObject.transform.GetComponent<Magazine>();
        if (_magazine != null)
            CurrentAmmo = _magazine.Ammo;
    }

    private void OnMagazineRemoved(SelectExitEventArgs args)
    {
        _magazine = null;
        CurrentAmmo = 0;
    }

    // ── Grab / Hand models ─────────────────────────────────────────────────

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        var t = args.interactorObject.transform;
        bool isLeft = t.CompareTag("LeftHand");
        bool isRight = t.CompareTag("RightHand");

        if (isLeft)
        {
            // If right hand already holds it, show secondary grip
            bool rightAlreadyHolding = rightHandOnWeapon != null && rightHandOnWeapon.activeSelf;
            SetHandModels(leftHandPlayer, false);
            SetHandModels(rightAlreadyHolding ? leftHandSecondary : leftHandOnWeapon, true);
        }
        else if (isRight)
        {
            bool leftAlreadyHolding = leftHandOnWeapon != null && leftHandOnWeapon.activeSelf;
            SetHandModels(rightHandPlayer, false);
            SetHandModels(leftAlreadyHolding ? rightHandSecondary : rightHandOnWeapon, true);
        }

        OnGrabbed?.Invoke(this);
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        var t = args.interactorObject.transform;

        if (t.CompareTag("LeftHand"))
        {
            SetHandModels(leftHandPlayer, true);
            SetHandModels(leftHandOnWeapon, false);
            SetHandModels(leftHandSecondary, false);
        }
        else if (t.CompareTag("RightHand"))
        {
            SetHandModels(rightHandPlayer, true);
            SetHandModels(rightHandOnWeapon, false);
            SetHandModels(rightHandSecondary, false);
        }

        OnReleased?.Invoke(this);
    }

    private static void SetHandModels(GameObject obj, bool active)
    {
        if (obj != null) obj.SetActive(active);
    }

    // ── FX coroutines ──────────────────────────────────────────────────────

    private IEnumerator MuzzleFlash()
    {
        if (muzzleLight == null) yield break;
        muzzleLight.enabled = true;
        yield return new WaitForSeconds(0.05f);
        muzzleLight.enabled = false;
    }

    private IEnumerator EjectCasing()
    {
        yield return null;   // one frame delay so animation has started
        if (casingPrefab == null || casingEjectPoint == null) yield break;

        var go = Instantiate(casingPrefab, casingEjectPoint.position,
            casingEjectPoint.rotation * Quaternion.Euler(-90f, 0f, 0f));

        var rb = go.GetComponent<Rigidbody>() ?? go.AddComponent<Rigidbody>();

        Vector3 worldDir = casingEjectPoint.TransformDirection(
            config.casingEjectDirection.normalized);
        rb.AddForce(worldDir * config.casingForce, ForceMode.Impulse);

        Destroy(go, config.casingLifetime);
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }
}

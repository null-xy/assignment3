using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class MageSpellCaster : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Transform leftHand;
    public Transform rightHand;
    public InputActionReference leftTrigger;
    public InputActionReference rightGrip;
    public GameObject chargeEffectPrefab;
    public GameObject projectilePrefab;
    public float maxChargeDistance = 2f;
    public float projectileSpeed = 20f;

    //public float maxForce = 50f;

    private GameObject chargeEffect;
    public bool isCharging;
    private Vector3 initialHandPosition;

    void OnEnable()
    {
        leftTrigger.action.Enable();
        rightGrip.action.Enable();
    }

    void OnDisable()
    {
        leftTrigger.action.Disable();
        rightGrip.action.Disable();
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        HandleCharging();
    }

    private void HandleCharging()
    {
        bool leftTriggerPressed = leftTrigger.action.ReadValue<float>() > 0.1f;
        bool rightGripPressed = rightGrip.action.ReadValue<float>() > 0.1f;

        if (!isCharging && leftTriggerPressed && rightGripPressed)
        {
            StartCharging();
        }
        else if (isCharging && !leftTriggerPressed && rightGripPressed)
        {
            ReleaseSpell();
        }

        if (isCharging)
        {
            UpdateChargeEffect();
        }
    }

    private void StartCharging()
    {
        isCharging = true;
        initialHandPosition = rightHand.position;
        chargeEffect = Instantiate(chargeEffectPrefab);
    }

    private void UpdateChargeEffect()
    {
        if (chargeEffect == null) return;

        Vector3 midpoint = (leftHand.position + rightHand.position) * 0.5f;
        float distance = Vector3.Distance(leftHand.position, rightHand.position);

        chargeEffect.transform.position = midpoint;
        chargeEffect.transform.LookAt(rightHand.position);
        chargeEffect.transform.localScale = new Vector3(0.1f, 0.1f, distance);
    }

    private void ReleaseSpell()
    {
        if (chargeEffect != null)
        {
            Destroy(chargeEffect);
        }

        if (isCharging)
        {
            FireProjectile();
            isCharging = false;
        }
    }

    private void FireProjectile()
    {
        Vector3 direction = (rightHand.position - leftHand.position).normalized;
        GameObject projectile = Instantiate(projectilePrefab, leftHand.position, Quaternion.LookRotation(direction));
        //if (projectile.TryGetComponent<Rigidbody>(out var rb))
        //{
        //rb.AddForce(projectileSpawnPoint.forward * force, ForceMode.Impulse);
        //rb.AddForce(direction * 10f, ForceMode.Impulse);
        //}
        //MagicProjectile magicProjectile = projectile.GetComponent<MagicProjectile>();
        //magicProjectile.Launch(direction * projectileSpeed);
        if (projectile.TryGetComponent(out MagicProjectile magicProjectile))
        {
            magicProjectile.SetDirection(direction);
            //lastFireTime = Time.time;
        }
        else
        {
            Debug.LogError("Projectile prefab missing MagicProjectile component!", this);
        }
    }
}

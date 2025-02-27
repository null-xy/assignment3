using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BladeController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Cutting Settings")]
    [SerializeField] private float slashDamage = 25f;
    [SerializeField] private float slashCooldown = 0.5f;
    [SerializeField] private float minSlashVelocity = 0.5f;

    [Header("Effects")]
    [SerializeField] private ParticleSystem slashEffect;
    [SerializeField] private AudioClip[] slashSounds;

    private AudioSource audioSource;
    private Rigidbody bladeRigidbody;
    private float lastSlashTime;
    private bool canSlash = true;
    private Vector3 prevPosition;
    private Vector3 derivedVelocity;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        bladeRigidbody = GetComponent<Rigidbody>();
        prevPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        derivedVelocity = (transform.position - prevPosition) / Time.deltaTime;
        prevPosition = transform.position;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!canSlash) return;

        // Check if we hit a mob and have sufficient velocity
        if (collision.gameObject.CompareTag("mob") && derivedVelocity.magnitude > minSlashVelocity)
        {
            TrySlashTarget(collision);
            //Destroy(collision.gameObject);
            //GameManager.instance.OnMobKilled();
        }
    }

    private void TrySlashTarget(Collision collision)
    {
        if (Time.time < lastSlashTime + slashCooldown) return;

        lastSlashTime = Time.time;

        HealthSystem healthSystem = collision.gameObject.GetComponent<HealthSystem>();
        if (healthSystem != null)
        {
            healthSystem.TakeDamage(slashDamage);
        }

        PlaySlashEffects(collision.contacts[0].point, collision);
        StartCoroutine(SlashCooldown());
    }

    private void PlaySlashEffects(Vector3 contactPoint, Collision collision)
    {
        if (slashEffect != null)
        {
            Quaternion effectRotation = Quaternion.LookRotation(collision.contacts[0].normal);
//            ParticleSystem effect = Instantiate(slashEffect, contactPoint, Quaternion.identity);
            ParticleSystem effect = Instantiate(slashEffect, contactPoint, effectRotation);

            effect.Play();
            Destroy(effect.gameObject, effect.main.duration);
            //Destroy(effect.gameObject, 1f);
        }

        if (slashSounds.Length > 0)
        {
            AudioClip randomClip = slashSounds[Random.Range(0, slashSounds.Length)];
            audioSource.PlayOneShot(randomClip);
        }
    }
    private System.Collections.IEnumerator SlashCooldown()
    {
        canSlash = false;
        yield return new WaitForSeconds(slashCooldown);
        canSlash = true;
    }

}

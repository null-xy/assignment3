using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float lifetime = 5f;
    public float initialStabilizerForce = 2f;
    public ParticleSystem impactEffect;
    public AudioClip impactSound;
    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnCollisionEnter(Collision collision)
    {
        // Impact effects
        if (impactEffect != null)
            Instantiate(impactEffect, transform.position, Quaternion.identity);

        if (impactSound != null)
            AudioSource.PlayClipAtPoint(impactSound, transform.position);

        Destroy(gameObject);
    }
}

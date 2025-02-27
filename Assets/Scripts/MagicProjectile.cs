using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

//[RequireComponent(typeof(Rigidbody))]
public class MagicProjectile : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 5f;
    public float pushForce = 5f;
    private Rigidbody rb;
    private bool isMoving;
    private Vector3 movementDirection;
    [Header("Effects")]
    [SerializeField] private ParticleSystem slashEffect;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            // Move with consistent speed regardless of frame rate
            transform.Translate(movementDirection * speed * Time.deltaTime, Space.World);
        }
    }
    //public void Launch(Vector3 direction)
    //{
    //    rb.AddForce(direction * speed, ForceMode.VelocityChange);
    //}
    public void SetDirection(Vector3 direction)
    {
        movementDirection = direction.normalized;
        isMoving = true;

        // Rotate projectile to face movement direction
        if (movementDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(movementDirection);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("mob"))
        {
            PlaySlashEffects(collision.contacts[0].point, collision);
            Destroy(collision.gameObject);
            GameManager.instance.OnMobKilled();
        }
        else
        {
            Rigidbody otherRb = collision.rigidbody;
            if (otherRb != null && otherRb != rb)
            {
                Vector3 impactDirection = rb.linearVelocity.normalized;
                Vector3 collisionPoint = collision.GetContact(0).point;

                otherRb.AddForceAtPosition(impactDirection * pushForce, collisionPoint, ForceMode.Impulse);
            }
        }

        //Destroy(gameObject);
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

    }
}

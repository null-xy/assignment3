using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private ParticleSystem bloodEffect;
    private float currentHealth;
    void Start()
    {
        currentHealth = maxHealth;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        // Play blood effect
        if (bloodEffect != null)
        {
            bloodEffect.Play();
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Add death logic here
        Destroy(gameObject);
        GameManager.instance.OnMobKilled();
    }
}

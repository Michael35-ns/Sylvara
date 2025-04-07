using UnityEngine;

public class FireballProjectile : MonoBehaviour
{
    public float speed = 10f;
    public float damage = 20f;
    public float lifetime = 5f;

    public GameObject explosionEffect;

    private Transform target;

    void Start()
    {
        // Ignorar colisiones con enemigos
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("EnemyProjectile"), LayerMask.NameToLayer("Enemy"));

        // Autodestrucción por tiempo
        Destroy(gameObject, lifetime);
    }

    public void SetTarget(Transform playerTarget)
    {
        target = playerTarget;
    }

    void Update()
    {
        if (target == null) return;

        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;
        transform.forward = dir;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var health = other.GetComponent<HealthSystem>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }

        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}

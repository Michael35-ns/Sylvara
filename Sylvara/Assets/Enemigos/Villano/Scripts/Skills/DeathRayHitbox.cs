using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathRayHitbox : MonoBehaviour
{
    public float damagePerTick = 70f;
    public float tickInterval = 1f;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(DamageOverTime(other));
        }
    }

    private HashSet<GameObject> affectedTargets = new HashSet<GameObject>();

    private IEnumerator DamageOverTime(Collider target)
    {
        if (affectedTargets.Contains(target.gameObject)) yield break;
        affectedTargets.Add(target.gameObject);

        var health = target.GetComponent<HealthSystem>();
        while (target != null && health != null && gameObject.activeSelf)
        {
            health.TakeDamage(damagePerTick);
            yield return new WaitForSeconds(tickInterval);
        }
    }

}

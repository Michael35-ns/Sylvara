using UnityEngine;

public class BossArena : MonoBehaviour
{
    public BossController bossController;
    public Transform arenaCenter;
    public float arenaRadius = 15f;

    private void Update()
    {
        if (bossController == null || arenaCenter == null) return;

        Vector3 bossPos = bossController.transform.position;
        Vector3 center = arenaCenter.position;

        float distance = Vector3.Distance(new Vector3(bossPos.x, 0, bossPos.z), new Vector3(center.x, 0, center.z));

        if (distance > arenaRadius)
        {
            // Forzar dirección de retorno al centro
            bossController.ForceMoveTo(center);
        }
    }
}

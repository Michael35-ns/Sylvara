using UnityEngine;

public class PlayerCheckpoint : MonoBehaviour
{
    private Vector3 currentCheckpoint;

    public void UpdateCheckpoint(Vector3 newCheckpoint)
    {
        currentCheckpoint = newCheckpoint;
        Debug.Log("Checkpoint actualizado a: " + currentCheckpoint);
    }

    public void Respawn()
    {
        transform.position = currentCheckpoint;
    }
}

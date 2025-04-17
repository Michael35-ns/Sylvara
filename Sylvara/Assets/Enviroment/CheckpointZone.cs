using UnityEngine;

public class CheckpointZone : MonoBehaviour
{
    public Transform checkpointPosition; // Punto que se guardará como checkpoint

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerCheckpoint player = other.GetComponent<PlayerCheckpoint>();
            if (player != null)
            {
                player.UpdateCheckpoint(checkpointPosition.position);
            }
        }
    }
}

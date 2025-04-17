using UnityEngine;

public class DesaparecerAlTocar : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            gameObject.SetActive(false);
        }
    }
}

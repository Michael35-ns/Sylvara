using UnityEngine;

public class DarEspada : MonoBehaviour
{
    public WeaponData armaQueDa;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            WeaponInventory inventory = other.GetComponent<WeaponInventory>();
            if (inventory != null)
            {
                int emptySlot = inventory.FindFirstEmptySlot();
                if (emptySlot != -1)
                {
                    inventory.SetWeaponInSlot(armaQueDa, emptySlot);
                    inventory.EquipWeapon(emptySlot);
                }
                else
                {
                    Debug.LogWarning("⚠️ Inventario lleno. No se pudo guardar el arma.");
                }
            }

            Destroy(gameObject);
        }
    }
}

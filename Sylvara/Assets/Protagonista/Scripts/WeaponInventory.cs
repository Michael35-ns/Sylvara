using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;


public class WeaponInventory : MonoBehaviour
{
    public WeaponSlot[] weaponSlots = new WeaponSlot[3];
    private int currentIndex = 0;

    public List<Button> slotButtons;
    public Sprite emptySlotSprite;
    public Sprite emptySlotSelectedSprite;

    private PlayerController player;

    void Start()
    {
        player = GetComponent<PlayerController>();
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            weaponSlots[i] = new WeaponSlot();
        }

        EquipWeapon(currentIndex);
        UpdateSlotVisuals();
    }

    public void EquipWeapon(int index)
    {
        if (index < 0 || index >= weaponSlots.Length) return;

        currentIndex = index;

        var data = weaponSlots[index].weaponData;
        if (data == null)
        {
            Debug.LogWarning("❌ No hay arma en este slot.");
            player.QuitarEspada();
            UpdateSlotVisuals();
            return;
        }

        player.QuitarEspada();
        player.espadaPrefab = data.weaponPrefab;
        player.attack.damage = data.damage;
        player.ObtenerEspada();
        UpdateSlotVisuals();

        Debug.Log($"🗡️ Equipaste: {data.weaponName}");
    }


    public void UpdateSlotVisuals()
    {
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            var data = weaponSlots[i].weaponData;
            var text = slotButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            var buttonImage = slotButtons[i].image;

            bool isSelected = (i == currentIndex);

            if (data != null)
            {
                text.text = data.weaponName;
                buttonImage.sprite = isSelected && data.iconSelected != null ? data.iconSelected : data.icon;
            }
            else
            {
                text.text = "Vacío";
                buttonImage.sprite = isSelected && emptySlotSelectedSprite != null ? emptySlotSelectedSprite : emptySlotSprite;
            }
        }
    }


    public int FindFirstEmptySlot()
    {
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if (weaponSlots[i].weaponData == null)
            {
                return i;
            }
        }
        return -1;
    }


    public void NextWeapon()
    {
        int nextIndex = (currentIndex + 1) % weaponSlots.Length;
        EquipWeapon(nextIndex);
    }

    public void SetWeaponInSlot(WeaponData data, int index)
    {
        if (index >= 0 && index < weaponSlots.Length)
        {
            weaponSlots[index].weaponData = data;
            UpdateSlotVisuals();
        }
    }


    public int GetCurrentSlotIndex() => currentIndex;
}

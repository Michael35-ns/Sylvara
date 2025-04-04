using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Image healthFill;

    public void UpdateHealth(float current, float max)
    {
        float percent = Mathf.Clamp01(current / max);
        healthFill.fillAmount = percent;
    }
}

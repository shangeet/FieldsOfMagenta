using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public void SetHealth(float healthRatio) {
        Slider slider = gameObject.GetComponent<Slider>();
        slider.value = (float) System.Math.Round(healthRatio, 2);
    }

}

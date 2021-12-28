using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpBar : MonoBehaviour
{
    public void SetExperience(float expRatio) {
        Slider slider = gameObject.GetComponent<Slider>();
        slider.value = (float) System.Math.Round(expRatio, 2);
    }
}

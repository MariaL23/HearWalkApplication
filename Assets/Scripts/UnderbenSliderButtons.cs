using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnderbenSliderButtons : MonoBehaviour
{
    public Slider slider; // Reference to the Slider component in the Inspector
    
    public Slider slider2;

    // This function is called when the button is pressed
    public void IncreaseSliderValue()
    {
        slider.value += 1; // Increase the slider value by 1
    }

    public void DecreaseSliderValue()
    {
        slider.value -= 1; // Decrease the slider value by 1
    }

    public void ResetSliderValue()
    {
        slider.value = 0; // Decrease the slider value by 1
        slider2.value = 0;
    }
}

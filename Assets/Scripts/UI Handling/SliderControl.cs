using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderControl : MonoBehaviour
{
    public DataProcessorAHRS dataProcessor; 
    public Slider mapFuncGainSlider;       
    public int postprocessorIndex;
    public TextMeshProUGUI mapFuncGainText;

    public void OnMapFuncGainSliderValueChanged(float newValue)
    {
        dataProcessor.UpdateMapFuncGain(postprocessorIndex, newValue);
        mapFuncGainText.text = newValue.ToString("0.00");

    }


}

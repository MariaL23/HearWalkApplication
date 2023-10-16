using UnityEngine;
using UnityEngine.UI;

public class ToggleSliderColorChanger : MonoBehaviour
{
    public Toggle H�jretoggle;
    public Slider H�jreslider1;
    public Slider H�jreslider2;

    public Toggle Venstretoggle;
    public Slider Venstreslider1;
    public Slider Venstreslider2;

    public Color activeColor = Color.green;
    public Color inactiveColor = Color.red;
    public Color activeBackgroundColor = Color.green;
    public Color inactiveBackgroundColor = Color.red;

    private Image H�jreSliderBackground1;
    private Image H�jreSliderBackground2;
    private Image VenstreSliderBackground1;
    private Image VenstreSliderBackground2;

    private void Start()
    {
        H�jreSliderBackground1 = H�jreslider1.GetComponentInChildren<Image>();
        H�jreSliderBackground2 = H�jreslider2.GetComponentInChildren<Image>();
        VenstreSliderBackground1 = Venstreslider1.GetComponentInChildren<Image>();
        VenstreSliderBackground2 = Venstreslider2.GetComponentInChildren <Image>();

        H�jretoggle.onValueChanged.AddListener(OnH�jreToggleValueChanged);
        Venstretoggle.onValueChanged.AddListener(OnVenstreToggleValueChanged);
    }

    private void OnH�jreToggleValueChanged(bool isOn)
    {
        H�jreSliderBackground1.color = isOn ? activeBackgroundColor : inactiveBackgroundColor;
        H�jreSliderBackground2.color = isOn ? activeBackgroundColor : inactiveBackgroundColor;
    }

    private void OnVenstreToggleValueChanged(bool isOn)
    {
        VenstreSliderBackground1.color = isOn ? activeBackgroundColor : inactiveBackgroundColor;
        VenstreSliderBackground2.color = isOn ? activeBackgroundColor : inactiveBackgroundColor;
    }
}

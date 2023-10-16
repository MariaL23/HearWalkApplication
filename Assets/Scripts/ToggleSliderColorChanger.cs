using UnityEngine;
using UnityEngine.UI;

public class ToggleSliderColorChanger : MonoBehaviour
{
    public Toggle Højretoggle;
    public Slider Højreslider1;
    public Slider Højreslider2;

    public Toggle Venstretoggle;
    public Slider Venstreslider1;
    public Slider Venstreslider2;

    public Color activeColor = Color.green;
    public Color inactiveColor = Color.red;
    public Color activeBackgroundColor = Color.green;
    public Color inactiveBackgroundColor = Color.red;

    private Image HøjreSliderBackground1;
    private Image HøjreSliderBackground2;
    private Image VenstreSliderBackground1;
    private Image VenstreSliderBackground2;

    private void Start()
    {
        HøjreSliderBackground1 = Højreslider1.GetComponentInChildren<Image>();
        HøjreSliderBackground2 = Højreslider2.GetComponentInChildren<Image>();
        VenstreSliderBackground1 = Venstreslider1.GetComponentInChildren<Image>();
        VenstreSliderBackground2 = Venstreslider2.GetComponentInChildren <Image>();

        Højretoggle.onValueChanged.AddListener(OnHøjreToggleValueChanged);
        Venstretoggle.onValueChanged.AddListener(OnVenstreToggleValueChanged);
    }

    private void OnHøjreToggleValueChanged(bool isOn)
    {
        HøjreSliderBackground1.color = isOn ? activeBackgroundColor : inactiveBackgroundColor;
        HøjreSliderBackground2.color = isOn ? activeBackgroundColor : inactiveBackgroundColor;
    }

    private void OnVenstreToggleValueChanged(bool isOn)
    {
        VenstreSliderBackground1.color = isOn ? activeBackgroundColor : inactiveBackgroundColor;
        VenstreSliderBackground2.color = isOn ? activeBackgroundColor : inactiveBackgroundColor;
    }
}

using UnityEngine;
using UnityEngine.UI;

public class ButtonSetting : MonoBehaviour
{
    public string setting_name;
    private Image image;

    public void Toggle()
    {
        bool setting = !Prefs.GetBool(setting_name, true);
        Prefs.SetBool(setting_name, setting);
        UpdateColor();
    }

    private void UpdateColor()
    {
        bool setting = Prefs.GetBool(setting_name, true);
        image.color = setting ? Color.white : Color.red;
    }

    private void Start()
    {
        image = GetComponent<Image>();
        UpdateColor();
    }
}
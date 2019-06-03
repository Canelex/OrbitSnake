using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonPurchase : MonoBehaviour
{
    private Image image;
    private Button button;
    public LocalizationText text;
    public string[] conditions;
    public Color colorEnabled;
    public Color colorDisabled;

    void Update()
    {
        CheckStatus();
    }

    void CheckStatus()
    {
        if (IsEnabled())
        {
            image.color = colorEnabled;
            button.interactable = true;
        }
        else
        {
            image.color = colorDisabled;
            button.interactable = false;
            text.key = "active_button";
        }
    }

    bool IsEnabled()
    {
        foreach (string condition in conditions)
        {
            if (Prefs.GetBool(condition, false)) return false;
        }
        return true;
    }

    void Start()
    {
        image = GetComponent<Image>();
        button = GetComponent<Button>();
        CheckStatus();
    }
}

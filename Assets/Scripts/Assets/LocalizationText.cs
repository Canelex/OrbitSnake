using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LocalizationText : MonoBehaviour
{
    private AssetManager assetManager;
    private Text text;
    public string key;

    public void UpdateText()
    {
        if (AssetManager.IsReady)
        {
            text.text = assetManager.GetLocalizedString(key);
        }
    }

    public void Update()
    {
        UpdateText();
    }

    public void Start()
    {
        text = GetComponent<Text>();
        assetManager = AssetManager.Instance;
        UpdateText();
    }
}

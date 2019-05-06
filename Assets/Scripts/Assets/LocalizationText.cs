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

    private void Update()
    {
        if (assetManager.IsReady())
        {
            text.text = assetManager.GetLocalizedString(key);
        }
    }

    private void Awake()
    {
        text = GetComponent<Text>();
        assetManager = AssetManager.Instance;
    }
}

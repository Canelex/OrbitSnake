using UnityEngine;
using UnityEngine.UI;

public class ButtonSkin : MonoBehaviour
{
    public string skinName;
    public Transform selector;
    public Text textCost;
    public Color colorEnabled;
    public Color colorDisabled;
    public Skin skin;

    private void Update()
    {
        if (IsUnlocked())
        {
            textCost.gameObject.SetActive(false);
        }
        
        textCost.text = skin.GetCost().ToString();
    }

    public void OnClick()
    {
        if (IsUnlocked())
        {
            Prefs.activeSkin = skinName;
            selector.localPosition = transform.localPosition;
        }
        else
        {
            Skin thisSkin = AssetManager.Instance.GetSkin(skinName);
            CanvasManager canvasManager = CanvasManager.Instance;
            canvasManager.CloseAllPages();
            canvasManager.ShowPage("Confirm");
            canvasManager.GetField("SkinPreview").image.sprite = thisSkin.sprite;
            canvasManager.GetField("SkinPreview").text.text = thisSkin.GetCost().ToString();
            
            canvasManager.GetField("PurchaseButton").image.color = Prefs.credits >= thisSkin.GetCost() ? colorEnabled : colorDisabled;
            
            FindObjectOfType<SkinStartup>().SelectSkin(thisSkin);
        }
    }

    public bool IsSelected()
    {
        return Prefs.activeSkin == skinName;
    }

    public bool IsUnlocked()
    {
        return Prefs.GetBool("skin_unlocked_" + skinName, skinName == "default");
    }

    private void Start()
    {
        if (IsSelected())
        {
            selector.localPosition = transform.localPosition;
        }
    }
}

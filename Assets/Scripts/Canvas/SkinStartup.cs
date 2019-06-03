using UnityEngine;
using UnityEngine.UI;

public class SkinStartup : MonoBehaviour
{
    private CanvasManager canvasManager;
    private Skin selectedSkin;
    public RectTransform canvas;
    public ButtonSkin prefabButtonSkin;
    public Transform skinSelector;

    private void Update()
    {
        canvasManager.GetField("Credits").text.text = Prefs.credits.ToString();
    }

    private void Start()
    {
        canvasManager = CanvasManager.Instance;

        AssetManager assetManger = AssetManager.Instance;
        int count = 0;
        foreach (Skin skin in assetManger.skins)
        {
            int row = count / 3 + 1;
            int col = count % 3;
            ButtonSkin btn = Instantiate(prefabButtonSkin, canvas);
            btn.skinName = skin.name;
            btn.GetComponent<Image>().sprite = skin.sprite;
            btn.transform.localPosition = new Vector2(col * 300 - 300, row * -300);
            btn.selector = skinSelector;
            btn.skin = skin;
            btn.textCost.text = skin.GetCost().ToString();
            count++;
        }
    }

    public void SelectSkin(Skin skin)
    {
        selectedSkin = skin;
    }

    public void PurchaseSkin()
    {
        if (Prefs.credits >= selectedSkin.GetCost())
        {
            Prefs.credits -= selectedSkin.GetCost();
            Prefs.SetBool("skin_unlocked_" + selectedSkin.name, true);
            CanvasManager.Instance.CloseAllPages();
            CanvasManager.Instance.ShowPage("Shop");
        }
    }
}
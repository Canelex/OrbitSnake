using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlanetButton : MonoBehaviour
{
    [Header("Properties")]
    public float percent = 0;
    public float period = 0.25F;
    public string sceneToLoad;
    public RectTransform rect;
    private bool isPressed;
    private bool loading;
    private int fingerId;

    public void SetPressed(bool pressed)
    {
        this.isPressed = pressed;
    }

    private void Update()
    {
        if (loading) return;
        if (Shutters.Instance.LoadingScene) return;

        if (isPressed)
        {
            percent += (1 / period) * Time.deltaTime;

            if (!AssetManager.Instance.IsSoundPlaying())
            {
                AssetManager.Instance.PlaySound("Blop");
            }
        }

        if (percent >= 1)
        {
            Shutters.Instance.LoadSceneWithShutters(sceneToLoad);
            loading = true;
        }
        else
        {
            rect.localScale = Vector3.one * (1 + percent) / 2F;
        }
    }

    private void Start()
    {
        rect.localScale = Vector3.one * (1 + percent) / 2F;
    }
}

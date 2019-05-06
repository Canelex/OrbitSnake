using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlanetButton : MonoBehaviour
{
    [Header("Properties")]
    public float radiusGrowSpeed = 3;
    public float radius = 1;
    public float radiusBreakAfter = 2;
    public Color colorBase = Color.white;
    public Color colorDetails = Color.white;
    public string sceneToLoad;
    [Header("Prefabs")]
    public ParticleSystem prefabEffect;
    public Image imageBase;
    public Image imageDetails;
    private RectTransform rect;
    private bool isPressed;
    private int fingerId;

    private void Update()
    {
        if (IsPressed())
        {
            radius += radiusGrowSpeed * Time.deltaTime;
        }

        if (radius >= radiusBreakAfter)
        {
            ParticleSystem ps = Instantiate(prefabEffect, rect.position, Quaternion.identity);
            ParticleSystem.MainModule main = ps.main;
            main.startColor = new ParticleSystem.MinMaxGradient(colorBase, colorDetails);
            main.startSize = new ParticleSystem.MinMaxCurve(0.1F, 0.5F);
            Destroy(ps.gameObject, 1F);
            Destroy(gameObject);
            Shutters.Instance.LoadScene(sceneToLoad);
        }
        else
        {
            rect.sizeDelta = Vector3.one * radius;
        }
    }

    private bool IsPressed()
    {
        foreach (Touch touch in Input.touches)
        {
            if (!isPressed && touch.phase == TouchPhase.Began)
            {
                Vector3 position = Camera.main.ScreenToWorldPoint(touch.position);
                if (Vector2.Distance(position, rect.position) < radius)
                {
                    isPressed = true;
                    fingerId = touch.fingerId;
                }
            }

            if (isPressed && touch.phase == TouchPhase.Ended && fingerId == touch.fingerId)
            {
                isPressed = false;
            }
        }

        return isPressed;
    }

    private IEnumerator LoadScene()
    {
        yield return Shutters.Instance.Close(0.5F);
        SceneManager.LoadScene(sceneToLoad);
        yield return Shutters.Instance.Open(0.5F);   
    }

    private void Start()
    {
        rect = (RectTransform)transform;

        imageBase.color = colorBase;
        imageDetails.color = colorDetails;
    }
}

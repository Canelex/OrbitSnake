using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : Mass
{
    public float lifetime;
    private bool isPressed;
    private int fingerId;
    private SpriteRenderer iconSp;
    private Collider2D iconCo;

    private void Start()
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        sprite.color = Color.HSVToRGB(Random.Range(0F, 1F), 0.7F, 1.0F);
        StartCoroutine(FadeIcon(1F));
        StartCoroutine(DestroyMe(3F, 10F));
    }

    private void Awake()
    {
        GameObject icon = transform.GetChild(0).gameObject;
        iconSp = icon.GetComponent<SpriteRenderer>();
        iconCo = icon.GetComponent<Collider2D>();
    }

    public void UpdatePlanet()
    {
        if (iconCo.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
        {
            if (Input.GetMouseButtonDown(0))
            {
                isPressed = true;
                iconSp.enabled = false;
            }
        }

        if (Input.GetMouseButtonUp(0)) isPressed = false;

        foreach (Touch touch in Input.touches)
        {
            if (isPressed) // Check for release
            {
                if (touch.phase == TouchPhase.Ended && touch.fingerId == fingerId)
                {
                    isPressed = false;
                }
            }
            else // Check for press
            {
                if (touch.phase == TouchPhase.Began)
                {
                    if (iconCo.OverlapPoint(Camera.main.ScreenToWorldPoint(touch.position)))
                    {
                        isPressed = true;
                        fingerId = touch.fingerId;
                        iconSp.enabled = false;
                    }
                }
            }
        }
    }

    public bool IsPressed()
    {
        return isPressed;
    }

    public void ChangeMassAndScale(float deltaMass, float deltaScale)
    {
        ChangeMass(deltaMass);
        transform.localScale += (Vector3)Vector2.one * deltaScale;
        iconSp.transform.localScale = Vector3.one;
    }

    private IEnumerator FadeIcon(float period)
    {
        bool fadeOut = true;

        while (iconSp.enabled)
        {
            for (float t = 0; t < period; t += Time.deltaTime)
            {
                if (t > period) t = period;

                float alpha = (fadeOut ? t / period : 1 - t / period);
                iconSp.color = new Color(1F, 1F, 1F, alpha);
                yield return null;
            }

            fadeOut = !fadeOut;
            yield return null;
        }
    }

    private IEnumerator DestroyMe(float time, float minDist)
    {
        do
        {
            yield return new WaitForSeconds(time);
        }
        while (Vector2.Distance(transform.position, Camera.main.transform.position) < minDist);
        Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    // Planet variables
    public float lifetime;
    public float radius;
    public float kConst;
    
    [Space(20)] // Objects
    public SpriteRenderer planetSprite;
    public SpriteRenderer outlineSprite;
    public Collider2D outlineCollider;
    public SpriteRenderer detailsSprite;
    public SpriteMask detailsMask;
    public ParticleSystem prefabEffect;

    // Variables
    private Color primary;
    private Color second;
    private bool isPressed;
    private int fingerId;

    public void UpdatePlanet()
    {
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
                    if (outlineCollider.OverlapPoint(Camera.main.ScreenToWorldPoint(touch.position)))
                    {
                        isPressed = true;
                        fingerId = touch.fingerId;
                        DisableOutline();
                    }
                }
            }
        }

        transform.localScale = Vector3.one * radius; // Update radius
        //mass = density * Mathf.PI * radius * radius; // Update Mass
    }

    public void DisableOutline()
    {
        outlineSprite.enabled = false;
    }

    public bool IsPressed()
    {
        return isPressed;
    }

    public void FadeIcon(bool fadingIn, float percent)
    {
        outlineSprite.color = new Color(1F, 1F, 1F, fadingIn ? percent : 1 - percent);
    }

    public Color GetPrimaryColor()
    {
        return primary;
    }

    public Color GetSecondaryColor()
    {
        return second;
    }

    private IEnumerator DestroyMe(float time, float minDist)
    {
        do
        {
            // Only delete when planet is well out of view
            yield return new WaitForSeconds(time);
        }
        while (Vector2.Distance(transform.position, Camera.main.transform.position) < minDist);
        Destroy(gameObject);
    }

    public void Destroy()
    {
        ParticleSystem ps = Instantiate(prefabEffect, transform.position, Quaternion.identity);
        ParticleSystem.MainModule main = ps.main;
        main.startColor = new ParticleSystem.MinMaxGradient(primary, second);
        main.startSize = new ParticleSystem.MinMaxCurve(0.1F, radius / 4F);
        SoundManager.Instance.PlaySound("Destroy");
        Destroy(ps.gameObject, 1F);
        Destroy(gameObject);
    }

    private void Start()
    {
        // Correct rendering order so masks don't fudge up
        int randomOrder = Random.Range(0, 32767);
        planetSprite.sortingOrder = randomOrder;
        detailsSprite.sortingOrder = randomOrder + 1;
        detailsMask.frontSortingOrder = randomOrder + 1;
        detailsMask.backSortingOrder = randomOrder;

        // Set planet's random colors
        float randomHue = Random.Range(0F, 1F);
        primary = Color.HSVToRGB(randomHue, 0.8F, 1F);
        second = Color.HSVToRGB(randomHue, 1F, 0.8F);
        planetSprite.color = primary;
        detailsSprite.sprite = TextureManager.Instance.GetRandomTexture();
        detailsSprite.color = second;

        // Set planet's random rotation and arrange for it's despawning coroutine
        transform.Rotate(0, 0, Random.Range(-180, 180));
        StartCoroutine(DestroyMe(3F, 10F));
    }
}

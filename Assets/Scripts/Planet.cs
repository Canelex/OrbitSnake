using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : Mass
{
    public float lifetime;
    [SerializeField]
    private SpriteRenderer planetSprite;
    [SerializeField]
    private SpriteRenderer outlineSprite;
    [SerializeField]
    private Collider2D outlineCollider;
    [SerializeField]
    private SpriteRenderer detailsSprite;
    [SerializeField]
    private SpriteMask detailsMask;
    private Color primary;
    private Color second;
    private bool isPressed;
    private int fingerId;

    public void UpdatePlanet()
    {
        // TODO: Delete this part
        if (outlineCollider.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
        {
            if (Input.GetMouseButtonDown(0))
            {
                isPressed = true;
                outlineSprite.enabled = false;
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
                    if (outlineCollider.OverlapPoint(Camera.main.ScreenToWorldPoint(touch.position)))
                    {
                        isPressed = true;
                        fingerId = touch.fingerId;
                        outlineSprite.enabled = false;
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
        outlineSprite.transform.localScale = Vector3.one;
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
        detailsSprite.sprite = PlanetTextures.Instance.GetRandomTexture();
        detailsSprite.color = second;

        // Set planet's random rotation and arrange for it's despawning coroutine
        transform.Rotate(0, 0, Random.Range(-180, 180));
        StartCoroutine(DestroyMe(3F, 10F));
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

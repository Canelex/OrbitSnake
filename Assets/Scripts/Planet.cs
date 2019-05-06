using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    // Planet variables
    [Header("Properties")]
    public float radius;
    public float radiusPress;
    public float kConst;
    // Planet look
    [Header("Visuals")]
    public SpriteRenderer sprite;
    public SpriteRenderer details;
    public SpriteMask mask;
    public ParticleSystem prefabEffect;
    // Variables
    private Color primary;
    private Color second;
    private bool isPressed;
    private int fingerId;

    public bool IsPressed()
    {
        return isPressed;
    }

    public bool IsPressed(int finger)
    {
        return isPressed && fingerId == finger;
    }

    public bool OverPlanet(Vector3 point)
    {
        return Distance(point) < Mathf.Max(radius, radiusPress);
    }

    public float Distance(Vector3 position)
    {
        return Vector2.Distance(position, transform.position);
    }

    public void Press(int finger)
    {
        fingerId = finger;
        isPressed = true;
    }

    public void Unpress()
    {
        isPressed = false;
    }

    public void Destroy()
    {
        ParticleSystem ps = Instantiate(prefabEffect, transform.position, Quaternion.identity);
        ParticleSystem.MainModule main = ps.main;
        main.startColor = new ParticleSystem.MinMaxGradient(primary, second);
        main.startSize = new ParticleSystem.MinMaxCurve(0.1F, radius / 4F);
        AssetManager.Instance.PlaySound("Destroy");
        Destroy(ps.gameObject, 1F);
        Destroy(gameObject);
    }

    private void Start()
    {
        // Correct rendering order so masks don't fudge up
        int randomOrder = Random.Range(0, 32767);
        sprite.sortingOrder = randomOrder;
        details.sortingOrder = randomOrder + 1;
        mask.frontSortingOrder = randomOrder + 1;
        mask.backSortingOrder = randomOrder;

        // Set planet's random colors
        float[] hues = {0/360F, 17/360F, 90/360F, 207/360F, 260/360F, 325/360F};
        float randomHue = hues[Random.Range(0, hues.Length)]; //Random.Range(0F, 1F);
        primary = Color.HSVToRGB(randomHue, 0.8F, 1F);
        second = Color.HSVToRGB(randomHue, 1F, 0.8F);
        sprite.color = primary;
        details.sprite = AssetManager.Instance.GetRandomTexture();
        details.color = second;

        // Set planet's random rotation and arrange for it's despawning coroutine
        transform.Rotate(0, 0, Random.Range(-180, 180));
    }
}
